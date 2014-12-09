using UnityEngine;
using System.Collections;

public class AffectedItemScript : MonoBehaviour {

	public bool useAnimation = false; 				// Definimos si al activar vamos a activar una animacion
	public bool useToggle = false;					// Si actua como un Toggle
	public bool useActivateScript = false;			// ?
	public bool useRandomInitialState = true;		// Si el estado inicial es aleatorio
	public bool initialState = true;				// Establece el estado inicial

	private bool _toggle = true;					// Estado actual
	private Animator animator;  					// La variable de la animación será un booleano activate

	void Start () {
		if(useAnimation) animator = GetComponent<Animator> ();
		_toggle = initialState;
		if (useRandomInitialState) _toggle =(Random.Range(0, 2) == 1);
		animator.SetBool ("activate", _toggle); // Posicionamos estado inicial
		_toggle = !_toggle; // La siguiente activación será en el estado contrario
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
