using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDirector : MonoBehaviour {

	public GameObject playerPrefab;

	public Vector3 mapStartPosition = Vector3.zero;
	public GameObject spawnBlockPrefab;
	public GameObject exitBlockPrefab;
	public GameObject[] blocksPrefabs;
	
	public float blockWidth=20.0f; 						// Ancho de los bloques en unidades Unity
	public float blockHeight=1.0f; 						// Alto de los bloques en unidades Unity
	public int widthSizeInBlocks = 4; 					// Cuantos bloques componen un nivel
	public int heightSizeInBlocks = 4;					// Cuantos niveles se generan a la vez y cuantos se mantienen en memoria (el resto se eliminan)
	public float verticalMarginInBlocks = 10.0f;		// Espacio en bloques que se deja entre niveles
	
	private List<Vector3> levelsStartPoints;			// Spawnpoints activos generados
	private List<Vector3> levelsTimeTravelPoints;		// Puntos a los que se puede viajar en el tiempo
	private BlockEndExitController lastGeneratedExit;	// Ultima salida generada sin punto de spawn
	
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
		
	}

	void Start () {
		lastGeneratedExit = null;
		levelsStartPoints = new List<Vector3>();
		levelsTimeTravelPoints = new List<Vector3>();
		lastGeneratedLevel = 0;
		map = new List<Level>();
		
		nextBlockPosition = GenerateMapAt(mapStartPosition);
		player = (GameObject)Instantiate(playerPrefab, nextBlockPosition, Quaternion.identity);
		player.transform.position = map[0].SpawnPosition();
		transform.parent = player.transform;
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
		
		Debug.Log("Map Generation Starts...");
		nextBlockPosition = initialPosition;
		
		
		// Generamos vacios los proximos 'heightSizeInBlocks' niveles
		for (int level=0; level < heightSizeInBlocks; level++) {
			Level mapLevel = new Level(); // Generamos un nivel vacio
			map.Add(mapLevel); // Añadimos el nivel a la lista de niveles del mapa
		}
		
		// Rellenamos los niveles con bloques
		for (int level=0; level < heightSizeInBlocks; level++) {
			
			// Generando Bloque de Inicio/Spawn
			Debug.Log("Generando bloque de inicio de Level");
			GameObject nextBlock = (GameObject)Instantiate(spawnBlockPrefab, nextBlockPosition, Quaternion.identity);
			map[level].AddBlockAt(nextBlock,0); // Guardamos el bloque de spawn en el primer bloque del level
			Vector3 spawnPosition = nextBlock.transform.GetChild(0).position; // Buscamos la posicion de Spawn de este nivel
			
			if (level > 0 && map[level-1].BlocksCount() > 1) map[level-1].Exit().nextPosition = spawnPosition; // Si hay una salida previa lo enlazamos con esta
			nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			
			// Generación de bloques/trampas intermedios/as
			Debug.Log("Generando bloques/trampas de level");
			for (int cell=1; cell < (widthSizeInBlocks-1); cell++) {
				int nextBlockIndex = Random.Range(0, blocksPrefabs.Length);
				nextBlock = (GameObject)Instantiate(blocksPrefabs[nextBlockIndex], nextBlockPosition, Quaternion.identity);
				// TODO: Comprobar si existe bloque en la posicion antes de añadir este
				if (!map[level].ExistsBlockAt(cell)) {
					map[level].AddBlockAt(nextBlock, cell); // Guardamos el bloque en el nivel completo
					InteractiveBlockController interactiveController = nextBlock.GetComponent<InteractiveBlockController>();
					// Bloque interactivo: Generamos el bloque sobre el que actúa
					if (interactiveController != null) {
						if (interactiveController.affectedBlockPrefab != null) {
							if (interactiveController.maxLevelsDistance > 0) {
								int affectedBlockLevel = Random.Range(1,interactiveController.maxLevelsDistance);
								Vector3 affectedBlockPosition = new Vector3(initialPosition.x + (cell * blockWidth), nextBlockPosition.y - ((blockHeight * verticalMarginInBlocks) * affectedBlockLevel), nextBlockPosition.z);
								GameObject affectedBlock = (GameObject)Instantiate(interactiveController.affectedBlockPrefab, affectedBlockPosition, Quaternion.identity);
								map[level+affectedBlockLevel].AddBlockAt(affectedBlock,cell); // Guardamos el bloque de spawn en el primer bloque del level	
								interactiveController.SetAffectedBlock(affectedBlock.GetComponent<AffectedBlockController>());
								
							}
						}
					}
				}
				// TODO: Añadir bloque affectedBlock si no es null (random de altura maxLevels... y random horizontal max..)
				nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			}
			
			// Generando Bloque de Salida/Exit
			Debug.Log("Generando bloque de Salida de Level");
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
		Debug.Log("Map Generation Ends...");
		return nextBlockPosition;
	}
	
	public Vector3 AutoGenerateMap() {
		nextBlockPosition = GenerateMapAt(nextBlockPosition);	
		return map[0].SpawnPosition();
	}
	
}
