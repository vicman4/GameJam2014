using UnityEngine;
using System.Collections;

public class InteractiveItemScript : MonoBehaviour {
	public AffectedBlockController affectedBlockController;

	private InteractiveBlockController interactiveBlocController;
	void Start(){
		interactiveBlocController = transform.parent.GetComponent<InteractiveBlockController> ();
		affectedBlockController = interactiveBlocController.affectedBlockController;
	}

	void OnMouseDown(){
		Debug.Log ("Mouse Down");
		affectedBlockController.toggle ();
	}
}
