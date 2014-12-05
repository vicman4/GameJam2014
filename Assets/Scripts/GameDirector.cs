using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDirector : MonoBehaviour {

	public GameObject playerPrefab;

	public Vector3 mapStartPosition = Vector3.zero;
	public GameObject spawnBlockPrefab;
	public GameObject exitBlockPrefab;
	public GameObject[] blocksPrefabs;
	
	public float blockWidth=20.0f;
	public float blockHeight=1.0f;	
	public float widthSizeInBlocks = 4.0f;
	public float heightSizeInBlocks = 4.0f;
	public float verticalMarginInBlocks = 10.0f;
	
	public int maxTimeLevelsWindow = 4;
	
	private List<Vector3> levelsStartPoints;
	private List<Vector3> levelsTimeTravelPoints;
	private BlockEndExitController lastGeneratedExit;
	
	private int actualStartPoint;
	private Vector3 nextBlockPosition;

	private GameObject player;
	
	private List<Level> map;
	private class Level {
		private List<GameObject> blocks;
		
		public Level() {
			blocks = new List<GameObject>();
		}
		
		public void ClearLevel() {
			blocks.ForEach( b => {
				Destroy(b);
			});
		}
		
		public void AddBlock(GameObject block) {
			blocks.Add(block);
		}
	}

	void Start () {
		lastGeneratedExit = null;
		levelsStartPoints = new List<Vector3>();
		levelsTimeTravelPoints = new List<Vector3>();
		map = new List<Level>();
		
		nextBlockPosition = GenerateMapAt(mapStartPosition);
		player = (GameObject)Instantiate(playerPrefab, nextBlockPosition, Quaternion.identity);
		player.transform.position = levelsStartPoints[0];
		transform.parent = player.transform;
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.KeypadPlus)) {
			nextBlockPosition = GenerateMapAt(nextBlockPosition);
		}
	
	}
	
	
	Vector3 GenerateMapAt(Vector3 initialPosition) {
		if (map.Count >= maxTimeLevelsWindow) {
			map.GetRange(0, maxTimeLevelsWindow).ForEach( lvl => {
				lvl.ClearLevel();
			});
			map.RemoveRange(0, maxTimeLevelsWindow);
		}
		
		Debug.Log("Map Generation Starts...");
		nextBlockPosition = initialPosition;
		for (int level=0; level < heightSizeInBlocks; level++) {
			Level mapLevel = new Level();
			
			// Generando Bloque de Inicio/Spawn
			Debug.Log("Generando bloque de inicio de Level");
			GameObject nextBlock = (GameObject)Instantiate(spawnBlockPrefab, nextBlockPosition, Quaternion.identity);
			mapLevel.AddBlock(nextBlock); // Guardamos el bloque en el nivel completo
			Vector3 spawnPosition = nextBlock.transform.GetChild(0).position; // Buscamos la posicion de Spawn de este nivel
			levelsStartPoints.Add(spawnPosition); // Guardamos la posición en la lista de posiciones Spawn
			if (lastGeneratedExit != null) lastGeneratedExit.nextPosition = spawnPosition; // Si es el segundo nivel (y en adelante) enlazamos la salida anterior con este spawn
			nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			
			
			// Generación de bloques/trampas intermedios/as
			Debug.Log("Generando bloques/trampas de level");
			for (int cell=1; cell < (widthSizeInBlocks-1); cell++) {
				int nextBlockIndex = Random.Range(0, blocksPrefabs.Length);
				nextBlock = (GameObject)Instantiate(blocksPrefabs[nextBlockIndex], nextBlockPosition, Quaternion.identity);
				mapLevel.AddBlock(nextBlock); // Guardamos el bloque en el nivel completo
				nextBlockPosition = new Vector3(nextBlockPosition.x + blockWidth, nextBlockPosition.y, nextBlockPosition.z);
			}
			
			// Generando Bloque de Salida/Exit
			Debug.Log("Generando bloque de Salida de Level");
			nextBlock = (GameObject)Instantiate(exitBlockPrefab, nextBlockPosition, Quaternion.identity);
			mapLevel.AddBlock(nextBlock); // Guardamos el bloque en el nivel completo
			lastGeneratedExit = nextBlock.GetComponentInChildren<BlockEndExitController>(); // Guardamos la última salida generada
			if (level == (heightSizeInBlocks-1)) {
				lastGeneratedExit.autoGenerateMap = true;
				lastGeneratedExit.gameDirector = this;
			}
			
			map.Add(mapLevel); // Añadimos el nivel a la lista de niveles del mapa
			
			nextBlockPosition = new Vector3(initialPosition.x, nextBlockPosition.y - (blockHeight * verticalMarginInBlocks), nextBlockPosition.z);
		}
		nextBlockPosition = new Vector3(initialPosition.x, nextBlockPosition.y - (blockHeight * verticalMarginInBlocks), nextBlockPosition.z);
		Debug.Log("Map Generation Ends...");
		return nextBlockPosition;
	}
	
	public void AutoGenerateMap() {
		nextBlockPosition = GenerateMapAt(nextBlockPosition);	
	}
	
}
