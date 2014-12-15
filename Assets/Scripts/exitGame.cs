using UnityEngine;
using System.Collections;

public class exitGame : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		if(col.transform.tag == "Player"){
			Debug.Log("Exit Game");
			Application.Quit();
		}
		
	}
}
