using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameDirector : MonoBehaviour {

	private Transform playerTarget;						// Atajo al transfor del jugador
	private List<Vector3> levelsTimeTravelPoints;		// Puntos a los que se puede viajar en el tiempo
	private Vector3 nextBlockPosition;					// Posicion en la que se generara el proximo bloque
	private GameObject player;							// Player generado
	private GameObject doppelganger;					// Doble que viaja en el tiempo
	private int lastGeneratedLevel = 0;					// Indice (de map) del proximo nivel generado
	private int interactiveBlocksInThisLevel;			// Contador interno del numero de bloques interactivos pintados en el nivel actual
	private int trapsBlocksInThisLevel;					// Contador interno del numero de bloques trampa pintados en el nivel actual
	private PlayerController playerController;			// Atajo al controller del jugador
	private int playerMarksCount;						// Contador interno de marcas de tiempo usadas
	private int playerTimeTravelsCount;					// Contador interno de viajes en el tiempo usados
	private List<GameObject> travelMarks;				// Lista de marcas de viaje en el tiempo instanciadas
	private int musicThemeIndex;						// Indice de la musica actual

	public GameObject deadEffectPrefab;					// Efecto al morir
	public GameObject deadDoppelgangerEffectPrefab;		// Efecto morir doppelganger
	public GameObject timeSpaceConflictEffectPrefab;	// Efecto al producirse un conflicto espacio temporal (doppelganger toca a player)
	public Vector3 timeSpaceConflictEffectOffset;
	public GameObject playerPrefab;						// Prefab del personaje del jugador
	public GameObject timeTravelMarkPrefab;				// Prefab que muestra una marca de viaje en el tiempo
	public GameObject spawnBlockPrefab;					// Prefab del bloque inicial de cada nivel
	public GameObject exitBlockPrefab;					// Prefab del bloque final de cada nivel
	public GameObject[] interactiveBlocks;				// Bloques con interruptores que afectan a bloques de niveles del futuro
	public GameObject[]	trapsBlocks;					// Bloques con trampas del nivel actual
	public GameObject[]	neutralBlocks;					// Bloques neutrales del nivel actual
	
	public AudioSource sfxExlosionImplosion;			// Sonido de muerte player
	public AudioSource sfxGolpe;						// Sonido de golpe
	public AudioSource[] musicThemes;					// Musica
	
	public GameObject panoramicCam;						// Camara panoramica
	public Transform panoramicCamPoint;					// Punto de panoramica del mapa
	public Vector3 panoramicCamOffset;					// Offset de ajuste de la posicion de la camara panoramica

	public Vector3 mapStartPosition = Vector3.zero;		// Posicion de inicio de pintado del mapa
	public int interactiveBlocksPerLevel = 1;			// Numero de bloques interactivos del futuro por nivel
	public int trapsBlocksPerLevel = 2;					// Numero de trampas en el nivel actual por nivel
	
	public float blockWidth=20.0f; 						// Ancho de los bloques en unidades Unity
	public float blockHeight=1.0f; 						// Alto de los bloques en unidades Unity
	public int widthSizeInBlocks = 4; 					// Cuantos bloques componen un nivel
	public int heightSizeInBlocks = 4;					// Cuantos niveles se generan a la vez y cuantos se mantienen en memoria (el resto se eliminan)
	public float verticalMarginInBlocks = 10.0f;		// Espacio en bloques que se deja entre niveles
	
	public Vector3 cameraTargetAdjustedPosition;		// Ajuste fino de la posicion de la camara
	
	public bool followTarget = true;					// Si la camara sigue al jugador
	public bool lookAtTarget = true;					// Si la camara mira constantemente al jugador
	
	public int playerLevel;								// Nivel acumulado
	public int playerMapLevel;							// Nivel dentro de la ventana del mapa
	
	public int playerMaxTravelMarks = 3;				// Numero de marcas de viaje en el tiempo máximo
	public int playerMaxTimeTravels = 3; 				// Numero máximo de viajes en el tiempo
	public float time_score ;

	public List<Level> map;								// Mapa: contiene todos los niveles que genera el juego
	public class Level {								// Level: contiene los bloques que componen un nivel
		private Dictionary<int,GameObject> blocks;
		
		public Level() {
			blocks = new Dictionary<int, GameObject>();
		}
		
		public void ClearLevel() {
			foreach(KeyValuePair<int, GameObject> b in blocks) {
				Destroy(b.Value); // Eliminamos objeto de Unity
			}
			blocks.Clear(); // Limpiamos el diccionario
		}
		
		public void AddBlockAt(GameObject block, int position) {
			blocks.Add(position, block);
		}
		
		public BlockEndExitController Exit() {
			return blocks[blocks.Count-1].transform.Cast<Transform>().Where(c=>c.gameObject.tag == "ExitPoint").ToArray()[0].GetComponent<BlockEndExitController>();
		}
		
		public Vector3 SpawnPosition() {
			return blocks[0].transform.GetChild(0).position;
		}
		
		public int BlocksCount() {
			return blocks.Count;
		}
		
		public bool ExistsBlockAt(int index) {
			return blocks.ContainsKey(index);
		}
		
		public GameObject GetBlockAt(int index) {
			return blocks[index];
		}
		
		public void TurnLights(bool on) {
			if (on) {
				foreach(KeyValuePair<int, GameObject> b in blocks) {
					Transform lamp = b.Value.transform.Cast<Transform>().Where(c=>c.gameObject.name == "Lamp").ToArray()[0];
					lamp.GetComponentsInChildren<Animation>()[0].Play();
				}
			} else {
				foreach(KeyValuePair<int, GameObject> b in blocks) {
					b.Value.transform.Cast<Transform>().Where(c=>c.gameObject.name == "Lamp").ToArray()[0].GetComponentsInChildren<Light>()[0].enabled = false;
				}
			}
		}
		
	}
	

	void Start () {
	
		musicThemeIndex = 0;
		playerLevel = 0;
		playerMapLevel = 0;
		playerMarksCount = 0;
		playerTimeTravelsCount = 0;

		interactiveBlocks = Resources.LoadAll ("Prefabs/LevelBlocks/InteractiveFuture", typeof(GameObject)).Cast<GameObject>().ToArray();			
		trapsBlocks = Resources.LoadAll ("Prefabs/LevelBlocks/Traps", typeof(GameObject)).Cast<GameObject>().ToArray();					
		neutralBlocks = Resources.LoadAll ("Prefabs/LevelBlocks/Neutrals", typeof(GameObject)).Cast<GameObject> ().ToArray ();

		levelsTimeTravelPoints = new List<Vector3>();
		lastGeneratedLevel = 0;
		map = new List<Level>();
		travelMarks = new List<GameObject>();
		
		nextBlockPosition = GenerateMapAt(mapStartPosition);
		player = (GameObject)Instantiate(playerPrefab, nextBlockPosition, Quaternion.identity);
		player.transform.Rotate(0f, 90f, 0f);
		player.transform.position = map[0].SpawnPosition();
		player.GetComponent<PlayerController>().gameDirector = this;
		playerTarget = player.transform;
		playerController = player.transform.GetComponent<PlayerController>();
		doppelganger = null;
		map[0].TurnLights(true);
		musicThemes[musicThemeIndex].Play();
	}
	
	void Update() {
		if (Input.GetKey(KeyCode.Return)) {
			LeaveTimeTravelMark();
		}
		
		if (Input.GetKey(KeyCode.Space)) {
			PanoramicVision(true);
		}
		
		if (Input.GetKey(KeyCode.Escape)) {
			Application.LoadLevel("Menu");
		}


	}
	
	
	
	void LateUpdate() {
		// Seguir al jugador y/o Mirar al jugador
		if (followTarget && player != null) transform.position = playerTarget.position + cameraTargetAdjustedPosition;
		if (lookAtTarget && player != null) transform.LookAt(playerTarget);


	}
	
	Vector3 GenerateMapAt(Vector3 initialPosition) {
	
		// Borrado de los niveles anteriores para mantener solo el limite maxTimeLevelsWindow de niveles en el motor
		if (map.Count >= heightSizeInBlocks) {
			map.GetRange(0, heightSizeInBlocks).ForEach( lvl => {
				lvl.ClearLevel();
			});
			map.RemoveRange(0, heightSizeInBlocks);
			Debug.Log("Deleting previous levels!");
		}
		
		//Debug.Log("Map Generation Starts...");
		nextBlockPosition = initialPosition;
		
		
		// Generamos vacios los proximos 'heightSizeInBlocks' niveles
		for (int level=0; level < heightSizeInBlocks; level++) {
			Level mapLevel = new Level(); // Generamos un nivel vacio
			map.Add(mapLevel); // Añadimos el nivel a la lista de niveles del mapa
		}
		
		// Rellenamos los niveles con bloques
		int neutralBlocksPerLevel = widthSizeInBlocks - interactiveBlocksPerLevel - trapsBlocksPerLevel;
		if (neutralBlocksPerLevel < 0) neutralBlocksPerLevel = 0;
		int neutralBlocksInThisLevel = 0;
		for (int level=0; level < heightSizeInBlocks; level++) {
			interactiveBlocksInThisLevel = 0;
			trapsBlocksInThisLevel = 0;
			neutralBlocksInThisLevel = 0;
			
			// Generando Bloque de Inicio/Spawn
			//Debug.Log("Generando bloque de inicio de Level");
			GameObject nextBlock = (GameObject)Instantiate(spawnBlockPrefab, nextBlockPosition, Quaternion.identity);
			map[level].AddBlockAt(nextBlock,0); // Guardamos el bloque de spawn en el primer bloque del level
			// Buscamos la posicion de Spawn de este nivel
			Vector3 spawnPosition = nextBlock.transform.Cast<Transform>().Where(c=>c.gameObject.tag == "SpawnPoint").ToArray()[0].position;
			
			if (level > 0 && map[level-1].BlocksCount() > 1) map[level-1].Exit().nextPosition = spawnPosition; // Si hay una salida previa lo enlazamos con esta
			nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			
			// Generación de bloques/trampas intermedios/as
			//Debug.Log("Generando bloques/trampas de level");
			for (int cell=1; cell < (widthSizeInBlocks-1); cell++) {
				GameObject affectedBlock = null;
			
				if (!map[level].ExistsBlockAt(cell)) { // Comprobamos que no existe ya un bloque en la posicion
					nextBlock = null;
					int nextBlockIndex = -1;
					while (nextBlock == null) {
						switch (Random.Range(0,3)) {
							case 0:
								// Bloque de tipo neutral
								if (neutralBlocksInThisLevel < neutralBlocksPerLevel) {
									nextBlockIndex = Random.Range(0, neutralBlocks.Length);
									nextBlock = (GameObject)Instantiate(neutralBlocks[nextBlockIndex], nextBlockPosition, Quaternion.identity);
									neutralBlocksInThisLevel += 1;
								}
								break;
							case 1: 
								// Bloque de tipo switch Interactivo del futuro
								// No los permitimos si se supera el limite por nivel
								// No los permitimos si estamos en el ultimo nivel de la 'ventana' de niveles
								if (interactiveBlocksInThisLevel < interactiveBlocksPerLevel && level < heightSizeInBlocks-1) {
									nextBlockIndex = Random.Range(0, interactiveBlocks.Length);
									nextBlock = (GameObject)Instantiate(interactiveBlocks[nextBlockIndex], nextBlockPosition, Quaternion.identity);
									interactiveBlocksInThisLevel += 1;
								}
								break;
							case 2:
								// Bloque de tipo trampa de nivel actual
								if (trapsBlocksInThisLevel < trapsBlocksPerLevel) {
									nextBlockIndex = Random.Range(0, trapsBlocks.Length);
									nextBlock = (GameObject)Instantiate(trapsBlocks[nextBlockIndex], nextBlockPosition, Quaternion.identity);
									trapsBlocksInThisLevel += 1;
								}
								break;
						}
					}
					map[level].AddBlockAt(nextBlock, cell); // Guardamos el bloque en el nivel completo
					
					InteractiveBlockController interactiveController = nextBlock.GetComponent<InteractiveBlockController>();
					if (interactiveController != null) { // Si se trata de un bloque interactivo con el futuro, generamos su bloque del futuro
						if (interactiveController.affectedBlockPrefab != null) {
						
							if (interactiveController.maxLevelsDistance > 0) { // Distancia vertical maxima
							
								// TODO: La distancia vertical no puede ser superior a los niveles que quedan en la ventana
								//Debug.Log ("Interactive: Affected max levels distance: "+interactiveController.maxLevelsDistance);
								int affectedBlockLevel = Random.Range(1,interactiveController.maxLevelsDistance);
								Debug.Log("Ramdom level: "+affectedBlockLevel);
								affectedBlockLevel = Mathf.Clamp(affectedBlockLevel+level, level+1, heightSizeInBlocks-1);
								//Debug.Log ("Heigh Size: "+heightSizeInBlocks);
								//Debug.Log ("Level actual: "+level);
								//Debug.Log ("Affected Level: "+(affectedBlockLevel));
								if (!map[affectedBlockLevel].ExistsBlockAt(cell)) {
									Vector3 affectedBlockPosition = new Vector3(initialPosition.x + (cell * blockWidth), nextBlockPosition.y - ((blockHeight * verticalMarginInBlocks) * (affectedBlockLevel-level)), nextBlockPosition.z);
									affectedBlock = (GameObject)Instantiate(interactiveController.affectedBlockPrefab, affectedBlockPosition, Quaternion.identity);
									map[affectedBlockLevel].AddBlockAt(affectedBlock,cell); // Guardamos el bloque de spawn en el primer bloque del level	
									interactiveController.SetAffectedBlock(affectedBlock.GetComponent<AffectedBlockController>());
								} else {
									affectedBlock = map[affectedBlockLevel].GetBlockAt(cell);
									//Debug.Log("Ya existe un bloque ¿afectado? en la posicion: "+affectedBlockLevel+"|"+cell);
									//Debug.Log(affectedBlock);
								}
								//Debug.Log(affectedBlock);
							}
						}
					}
				}
				if (cell == (widthSizeInBlocks/2) && level == (heightSizeInBlocks/2)) { // Si este bloque es el centro del mapa
					// Colocamos el punto de camara panoramica
					panoramicCamPoint.position = nextBlock.transform.position + panoramicCamOffset;
				}
				nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			} // End For Bloques centrales
			
			// Generando Bloque de Salida/Exit
			//Debug.Log("Generando bloque de Salida de Level");
			nextBlock = (GameObject)Instantiate(exitBlockPrefab, nextBlockPosition, Quaternion.identity);
			map[level].AddBlockAt(nextBlock, widthSizeInBlocks-1); // Guardamos el bloque en el nivel completo
			BlockEndExitController exitController = nextBlock.GetComponentInChildren<BlockEndExitController>(); // Guardamos la última salida generada
			exitController.gameDirector = this;
			exitController.mapIndex = level;
			
			if (level == (heightSizeInBlocks-1)) { // Si es la salida del ultimo nivel a generar
				exitController.autoGenerateMap = true; // indicamos que la salida solicite autogenerar mas mapa
			}
			
			
			nextBlockPosition = new Vector3(initialPosition.x, nextBlockPosition.y - (blockHeight * verticalMarginInBlocks), nextBlockPosition.z);
		}
		nextBlockPosition = new Vector3(initialPosition.x, nextBlockPosition.y - (blockHeight * verticalMarginInBlocks), nextBlockPosition.z);
		//Debug.Log("Map Generation Ends...");
		return nextBlockPosition;
	}
	
	public Vector3 AutoGenerateMap() {
		nextBlockPosition = GenerateMapAt(nextBlockPosition);	
		return map[0].SpawnPosition();
	}
	
	public void GameOver() {
		time_score = Time.timeSinceLevelLoad;
		if (doppelganger != null) {	// Ha muerto el doppleganger
			GameObject effect = (GameObject)Instantiate(deadDoppelgangerEffectPrefab, doppelganger.transform.position, Quaternion.identity);
			PanoramicTravelDoppelgangerDie();
		} else {
			if (player != null) {
				GameObject effect = (GameObject)Instantiate(deadEffectPrefab, player.transform.position + timeSpaceConflictEffectOffset, Quaternion.identity);
				sfxExlosionImplosion.Play ();
				Destroy(effect, 10.0f);
				Destroy(player);
				player=null;
				LeanTween.rotateAround(transform.gameObject, Vector3.forward, 0.1f, 0.1f).setEase( LeanTweenType.easeSpring ).setLoopClamp().setRepeat(7).setDelay(0.1f).setOnComplete(() => {
					LeanTween.rotateAround(transform.gameObject, Vector3.forward, 10f, 0.1f).setEase( LeanTweenType.easeSpring ).setLoopClamp().setRepeat(10).setDelay(1.4f).setOnComplete(() => {
						Application.LoadLevel("Menu");
					});
				});
			} else {
				Application.LoadLevel("Menu");
				// SHOW UI STATS
			}
		}


	}
	
	public void LeaveTimeTravelMark() {
		if (playerMarksCount == playerMaxTravelMarks) { // Si se supera el limite de marcas, se elimina la primera (la mas lejana en el tiempo)
			Destroy(travelMarks.ElementAt(0), 1.0f);
			travelMarks.RemoveAt(0);
		} else {
			playerMarksCount += 1;
		}
		playerController.StopAndAction();
		GameObject timeTravelMark = (GameObject)Instantiate(timeTravelMarkPrefab, player.transform.position, Quaternion.identity);
		travelMarks.Add(timeTravelMark);
	}
	
	public void PanoramicVision(bool on) {
		if (on) {
			if (doppelganger == null && travelMarks.Count > 0) { // Solo un viaje en el tiempo a la vez
				panoramicCam.transform.position = transform.position;
				panoramicCam.transform.rotation = transform.rotation;
				panoramicCam.camera.enabled = true;
				camera.enabled = false;
				playerController.Freeze(true);
				playerController.enabled = false;
				LeanTween.value(gameObject, MusicPitch, 1f, 0f, 3f).setEase(LeanTweenType.easeInOutSine);
				LeanTween.move(panoramicCam, panoramicCamPoint.position, 2f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => { });
			}
		} else {
			//panoramicCam.transform.parent = transform;
			LeanTween.value(gameObject, MusicPitch, 0f, 1f, 1f).setEase(LeanTweenType.easeInOutSine);
			LeanTween.move(panoramicCam, transform.position, 1f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => {
				panoramicCam.camera.enabled = false;
				camera.enabled = true;
				if (player != null) {
					playerController.enabled = true;
					playerController.Freeze(false);
				}
			});
		}
	}
	
	
	public void PanoramicTravelDoppelgangerDie() {
			panoramicCam.transform.position = transform.position;
			panoramicCam.transform.rotation = transform.rotation;
			panoramicCam.camera.enabled = true;
			playerController.Freeze(true);
			playerController.enabled = false;
			
			// Retomamos el foco al player
			playerTarget = player.transform;
			playerController = player.transform.GetComponent<PlayerController>();
			
			LeanTween.value(gameObject, MusicPitch, 1f, 0f, 3f).setEase(LeanTweenType.easeInOutSine);
			LeanTween.move(panoramicCam, panoramicCamPoint.position, 2f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => { 
				// Desaparece el Doppelganger
				Destroy(doppelganger);
				doppelganger = null;
				
				LeanTween.value(gameObject, MusicPitch, 0f, 1f, 2f).setEase(LeanTweenType.easeInOutSine);
				LeanTween.move(panoramicCam, transform.position, 2f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => {
					panoramicCam.camera.enabled = false;
					playerController.enabled = true;
					playerController.Freeze(false);
				});
			});
	}
	
	
	public void TravelTo(Vector3 position) {
		// Generamos el Doppelganger
		doppelganger = (GameObject)Instantiate(playerPrefab, position, Quaternion.identity);
		doppelganger.transform.Rotate(0f, 90f, 0f);
		doppelganger.GetComponent<PlayerController>().gameDirector = this;
		
		// Enfocamos al Doppelganger
		playerTarget = doppelganger.transform;
		
		// Tomamos el control del Doppelganger
		playerController = doppelganger.transform.GetComponent<PlayerController>();
		transform.position = doppelganger.transform.position + cameraTargetAdjustedPosition;
		PanoramicVision(false);
	}
	
	
	public void SpaceTimeConflict() {
		if (player != null) {
			GameObject effect = (GameObject)Instantiate(timeSpaceConflictEffectPrefab, player.transform.position + timeSpaceConflictEffectOffset, Quaternion.identity);
			sfxGolpe.Play();
			Destroy(effect, 10.0f);
			Destroy(doppelganger);
			doppelganger = null;
			Destroy(player);
			player = null;
			LeanTween.rotateAround(transform.gameObject, Vector3.forward, 5f, 0.1f).setEase( LeanTweenType.easeSpring ).setLoopClamp().setRepeat(5);
			LeanTween.rotateAround(transform.gameObject, Vector3.forward, 2f, 0.15f).setEase( LeanTweenType.easeSpring ).setLoopClamp().setRepeat(7).setDelay(0.05f).setOnComplete(() => {
				GameOver();
			});
		}
	}
	
	
	public bool IsDoppelganger() {
		return (doppelganger != null);
	}
	
	public void MusicPitch(float pitch) {
		musicThemes[musicThemeIndex].pitch = pitch;
		
	}
	
	public void MusicVolume(float volume) {
		musicThemes[musicThemeIndex].volume = volume;
	}
	
	public void NextTheme() {
		LeanTween.value(gameObject, MusicVolume, 0.75f, 0f, 3f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>{
			musicThemes[musicThemeIndex].Stop ();
			musicThemes[musicThemeIndex].volume = 0.75f;
			musicThemeIndex += 1;
			if (musicThemeIndex >= musicThemes.Length) musicThemeIndex = 0;
			musicThemes[musicThemeIndex].volume = 0;
			musicThemes[musicThemeIndex].Play();
			LeanTween.value(gameObject, MusicVolume, 0f, 0.75f, 3f).setEase(LeanTweenType.easeInOutSine);
		});
	}
	
}
