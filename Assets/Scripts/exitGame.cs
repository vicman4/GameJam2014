using UnityEngine;
using System.Collections;

public class exitGame : MonoBehaviour {

	void OnCollisionEnter(Collision col){
		if(col.transform.tag == "Player"){
			Debug.Log("Exit Game");
			Application.Quit();
		}
		
	}
}
