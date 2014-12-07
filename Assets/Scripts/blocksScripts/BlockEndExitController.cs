using UnityEngine;
using System.Collections;

public class BlockEndExitController : MonoBehaviour {
	public Vector3 nextPosition;
	public bool autoGenerateMap = false;
	public GameDirector gameDirector;
	
	void OnTriggerEnter(Collider other) {
		
		if (autoGenerateMap == true) {
			nextPosition = gameDirector.AutoGenerateMap();
		}
		
		other.transform.position = nextPosition;
	}
}
