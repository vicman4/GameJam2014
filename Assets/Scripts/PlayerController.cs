using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Animator animController;							
	public float animSpeed = 1f;				
	public Vector3 lookTarget;
	public float speed = 1;
	
	public bool grounded;
	public bool frontBlocked;
	
	public bool isDownHit;
	public bool isFrontHit;
	
	public LayerMask frontRayHitsOnLayers;
	public LayerMask downRayHitsOnLayers;
	
	public float frontRayRange;
	public float downRayRange;
	
	public Vector3 frontRayOffset;
	public Vector3 downRayOffset;
	
	public GameDirector gameDirector;
	
	
	private RaycastHit frontRaycastHit;	
	private RaycastHit downRaycastHit;	
	
	public bool stop = false;
	
	private Color frontRayColor;
	private Color downRayColor;


	
	void Start () {
		animController = GetComponent<Animator>();					  
	}
	
	void FixedUpdate () {

		isFrontHit = Physics.Raycast (transform.position+frontRayOffset, transform.forward,out frontRaycastHit, frontRayRange, frontRayHitsOnLayers);
		if (isFrontHit) {
			if (frontRaycastHit.transform.tag == "Ground") {
				frontRayColor = Color.red;
				//frontBlocked = true;
			}
		} else {
			frontRayColor = Color.green;
			frontBlocked = false;
		}
		Debug.DrawRay(transform.position + frontRayOffset, transform.forward  * frontRayRange, frontRayColor);
		
		isDownHit = Physics.Raycast (transform.position+downRayOffset, Vector3.down,out downRaycastHit, downRayRange, downRayHitsOnLayers);
		if (isDownHit) {
			if (downRaycastHit.transform.tag == "Ground") {
				downRayColor = Color.green;
				grounded = true;
				animController.SetBool("JumpDown", false);
			}
		} else {
			downRayColor = Color.red;
			//grounded = false;
			animController.SetBool("JumpDown", true);
		}
		Debug.DrawRay(transform.position + downRayOffset, Vector3.down  * downRayRange, downRayColor);
	
		if (frontBlocked || !grounded) {
			stop = true;
		} else {
			stop = false;
		}
	
		if (!stop) {
			animController.SetFloat("Speed", speed);							
		} else {
			animController.SetFloat("Speed", 0);											
		}
		animController.speed = animSpeed;								
	}
	
	
	void OnCollisionEnter(Collision col) {
		if (col.transform.tag == "Muerte") {
			gameDirector.GameOver();
		}
		
		if (col.transform.tag == "Player") {
			gameDirector.SpaceTimeConflict();
		}
	}
	
	public void StopAndAction() {
		animController.SetTrigger("StopAndAction");
	}
	
	public void Freeze(bool on) {
		if (on) {
			animController.enabled = false;
		} else {
			animController.enabled = true;
		}
	}

	public void setSpeed(int new_speed){
		speed = new_speed;
	}
	
	
	
}
