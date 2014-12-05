using UnityEngine;
using System.Collections;

public class BlockEndExitController : MonoBehaviour {
	public Vector3 nextPosition;
	public bool autoGenerateMap = false;
	public GameDirector gameDirector;
	
	void OnTriggerEnter(Collider other) {
		
		if (autoGenerateMap == true) {
			Debug.Log(gameDirector);
			Debug.Log("Auto Generar Mapa!");
			gameDirector.AutoGenerateMap();
		}
		
		other.transform.position = nextPosition;
	}
}
