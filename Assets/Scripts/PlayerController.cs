using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Animator animController;							
	public float animSpeed = 1.5f;				
	public Vector3 lookTarget;
	
	void Start () {
		animController = GetComponent<Animator>();					  
	}

	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
		animController.SetFloat("Speed", 1);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		animController.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
	}
}
