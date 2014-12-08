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
			gameDirector.playerMapLevel = 0;
			gameDirector.map[0].TurnLights(true);
		} else {
			gameDirector.playerLevel += 1;
			//gameDirector.map[mapIndex].TurnLights(false);
			gameDirector.map[mapIndex+1].TurnLights(true);
		}
		
		if (gameDirector.IsDoppelganger()) {
			gameDirector.GameOver();
		} else {
			gameDirector.NextTheme();
			other.transform.position = nextPosition;
		}
	}
}
