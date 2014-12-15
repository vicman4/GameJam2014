using UnityEngine;
using System.Collections;

public class InteractiveItemScript : MonoBehaviour {
	public AffectedBlockController affectedBlockController;
	private Animator animator;
	private InteractiveBlockController interactiveBlocController;
	public bool toggle = true;
	
	void Start(){
		interactiveBlocController = transform.parent.GetComponent<InteractiveBlockController> ();
		affectedBlockController = interactiveBlocController.affectedBlockController;
		animator = GetComponent<Animator> ();
		toggle =(Random.Range(0, 2) == 1);
		if (animator) {
			animator.SetBool ("activate", toggle);
		}
	}


	void OnMouseDown(){
		Debug.Log ("Mouse Down");
		affectedBlockController.toggle ();
		if (animator) {
			toggle = !toggle;
			animator.SetBool ("activate", toggle);
		}

	}


}
