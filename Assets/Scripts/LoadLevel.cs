using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	void OnCollisionEnter(Collision col){
		if(col.transform.tag == "Player"){
			Debug.Log("Player");
			Application.LoadLevel("Map");
		}

	}
}
