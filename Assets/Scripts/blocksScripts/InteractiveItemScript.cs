using UnityEngine;
using System.Collections;

public class InteractiveItemScript : MonoBehaviour {
	public AffectedBlockController affectedBlockController;
	void OnMouseDown(){
		Debug.Log ("Mouse Down");
		affectedBlockController.toggle ();
	}
}
