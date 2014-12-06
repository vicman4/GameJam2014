using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameDirector : MonoBehaviour {

	public GameObject playerPrefab;

	public Vector3 mapStartPosition = Vector3.zero;
	public GameObject spawnBlockPrefab;
	public GameObject exitBlockPrefab;
	public GameObject[] interactiveBlocks;				// Bloques con interruptores que afectan a bloques de niveles del futuro
	public GameObject[]	trapsBlocks;					// Bloques con trampas del nivel actual
	public GameObject[]	neutralBlocks;					// Bloques neutrales del nivel actual

	public GameObject[]	tmp;					// Bloques neutrales del nivel actual

	public int interactiveBlocksPerLevel = 1;			// Numero de bloques interactivos del futuro por nivel
	public int trapsBlocksPerLevel = 2;					// Numero de trampas en el nivel actual por nivel
	
	public float blockWidth=20.0f; 						// Ancho de los bloques en unidades Unity
	public float blockHeight=1.0f; 						// Alto de los bloques en unidades Unity
	public int widthSizeInBlocks = 4; 					// Cuantos bloques componen un nivel
	public int heightSizeInBlocks = 4;					// Cuantos niveles se generan a la vez y cuantos se mantienen en memoria (el resto se eliminan)
	public float verticalMarginInBlocks = 10.0f;		// Espacio en bloques que se deja entre niveles
	
	private List<Vector3> levelsTimeTravelPoints;		// Puntos a los que se puede viajar en el tiempo
	
	private Vector3 nextBlockPosition;					// Posicion en la que se generara el proximo bloque
	private GameObject player;							// Player generado
	private int lastGeneratedLevel = 0;					// Indice (de map) del proximo nivel generado
	
	private List<Level> map;							// Mapa: contiene todos los niveles que genera el juego
	private class Level {								// Level: contiene los bloques que componen un nivel
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
			return blocks[blocks.Count-1].transform.GetChild(0).GetComponent<BlockEndExitController>();
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
		
	}
	
	private int interactiveBlocksInThisLevel;
	private int trapsBlocksInThisLevel;

	void Start () {

		interactiveBlocks = Resources.LoadAll ("Prefabs/LevelBlocks/InteractiveFuture", typeof(GameObject)).Cast<GameObject>().ToArray();			
		trapsBlocks = Resources.LoadAll ("Prefabs/LevelBlocks/Traps", typeof(GameObject)).Cast<GameObject>().ToArray();					
		neutralBlocks = Resources.LoadAll ("Prefabs/LevelBlocks/Neutrals", typeof(GameObject)).Cast<GameObject> ().ToArray ();

		levelsTimeTravelPoints = new List<Vector3>();
		lastGeneratedLevel = 0;
		map = new List<Level>();
		
		nextBlockPosition = GenerateMapAt(mapStartPosition);
		player = (GameObject)Instantiate(playerPrefab, nextBlockPosition, Quaternion.identity);
		player.transform.position = map[0].SpawnPosition();
		transform.parent = player.transform;

		foreach(GameObject g in Resources.LoadAll("Prefabs/LevelBlocks/Traps", typeof(GameObject)))
		{
			Debug.Log("prefab found: " + g.name);
			//tmp.Add(g);
		}


	
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Space)) {
			nextBlockPosition = GenerateMapAt(nextBlockPosition);
		}
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
		for (int level=0; level < heightSizeInBlocks; level++) {
			interactiveBlocksInThisLevel = 0;
			trapsBlocksInThisLevel = 0;
			
			// Generando Bloque de Inicio/Spawn
			//Debug.Log("Generando bloque de inicio de Level");
			GameObject nextBlock = (GameObject)Instantiate(spawnBlockPrefab, nextBlockPosition, Quaternion.identity);
			map[level].AddBlockAt(nextBlock,0); // Guardamos el bloque de spawn en el primer bloque del level
			Vector3 spawnPosition = nextBlock.transform.GetChild(0).position; // Buscamos la posicion de Spawn de este nivel
			
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
								nextBlockIndex = Random.Range(0, neutralBlocks.Length);
								nextBlock = (GameObject)Instantiate(neutralBlocks[nextBlockIndex], nextBlockPosition, Quaternion.identity);
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
				nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			}
			
			// Generando Bloque de Salida/Exit
			//Debug.Log("Generando bloque de Salida de Level");
			nextBlock = (GameObject)Instantiate(exitBlockPrefab, nextBlockPosition, Quaternion.identity);
			map[level].AddBlockAt(nextBlock, widthSizeInBlocks-1); // Guardamos el bloque en el nivel completo
			BlockEndExitController exitController = nextBlock.GetComponentInChildren<BlockEndExitController>(); // Guardamos la última salida generada
			
			if (level == (heightSizeInBlocks-1)) { // Si es la salida del ultimo nivel a generar
				exitController.autoGenerateMap = true; // indicamos que la salida solicite autogenerar mas mapa
				exitController.gameDirector = this;
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
	
}
