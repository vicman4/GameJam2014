using UnityEngine;
using System.Collections;

public class menuScript : MonoBehaviour {

	Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}

	public void setAyuda(bool ayuda=true){
		animator.SetBool("ayuda",ayuda);
	}
}
