using UnityEngine;
using System.Collections;

public class InteractiveItemScript : MonoBehaviour {

	InteractiveBlockController interactiveBlockController;
	AffectedBlockController affectedBlockController;
	// Use this for initialization
	void Start () {

		interactiveBlockController = transform.parent.GetComponent<InteractiveBlockController> ();
		affectedBlockController = interactiveBlockController.affectedBlock.GetComponent<AffectedBlockController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		Debug.Log ("Mouse Down");
		affectedBlockController.toggle ();
	}
}
