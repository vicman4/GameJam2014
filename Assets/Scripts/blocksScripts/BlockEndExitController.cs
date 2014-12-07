using UnityEngine;
using System.Collections;

public class BlockEndExitController : MonoBehaviour {
	public Vector3 nextPosition;
	public bool autoGenerateMap = false;
	public GameDirector gameDirector;
	public int mapIndex;
	
	void OnTriggerEnter(Collider other) {
		
		if (autoGenerateMap == true) {
			nextPosition = gameDirector.AutoGenerateMap();
		}
		
		gameDirector.playerLevel += 1;
		gameDirector.playerMapLevel = mapIndex+1;
		
		other.transform.position = nextPosition;
	}
}
