using UnityEngine;
using System.Collections;

public class AffectedItemScript : MonoBehaviour {

	public bool useAnimation = false; //Definimos si al activar vamos a activar una animacion
	public bool useToggle = false;
	public bool useActivateScript = false;

	bool _toggle = true;
	Animator animator;  // La variable de la animación será un booleano activate

	// Use this for initialization
	void Start () {
		if(useAnimation)
			animator = GetComponent<Animator> ();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void activate(){
		Debug.Log ("Activado "+transform.name);

		if (animator) {
			animator.SetBool ("activate", _toggle);
		}
		if (useToggle) {
			_toggle = !_toggle;
		}
	}
}
