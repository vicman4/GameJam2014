using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Animator animController;							
	public float animSpeed = 1f;				
	public float speed = 1;
	public bool canFall = false; 	// Si el jugador se puede caer. En caso negativo el jugador se para en los precipicios u obstaculos.
	
	public bool grounded;
	public bool frontBlocked;
	
	public bool isDownHit;
	public bool isFrontHit;
	private bool downRay1Hit;
	private bool downRay2Hit;
	
	public LayerMask frontRayHitsOnLayers;
	public LayerMask downRayHitsOnLayers;
	
	public float frontRayRange;
	public float downRay1Range;
	public float downRay2Range;
	
	public Vector3 frontRayOffset;
	public Vector3 downRay1Offset;
	public Vector3 downRay1Direction;
	public Vector3 downRay2Offset;
	public Vector3 downRay2Direction;
	
	
	public GameDirector gameDirector;
	
	private RaycastHit frontRaycastHit;	
	private RaycastHit downRaycastHit;	
	
	public bool stop = false;
	
	private Color frontRayColor;
	private Color downRayColor;


	
	void Start () {
		animController = GetComponent<Animator>();		
		downRay1Hit = false	;
		downRay2Hit = false	;  
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
		
		isDownHit = Physics.Raycast (transform.position+downRay1Offset, transform.TransformDirection(downRay1Direction),out downRaycastHit, downRay1Range, downRayHitsOnLayers);
		if (isDownHit) {
			if (downRaycastHit.transform.tag == "Ground") {
				downRay1Hit = true;
				downRayColor = Color.green;
			}
		} else {
			downRay2Hit = false;
			downRayColor = Color.red;
		}
		Debug.DrawRay(transform.position + downRay1Offset, transform.TransformDirection(downRay1Direction) * downRay1Range, downRayColor);
		
		isDownHit = Physics.Raycast (transform.position+downRay2Offset, transform.TransformDirection(downRay2Direction),out downRaycastHit, downRay2Range, downRayHitsOnLayers);
		if (isDownHit) {
			if (downRaycastHit.transform.tag == "Ground") {
				downRay2Hit = true;
				downRayColor = Color.green;
			}
		} else {
			downRay2Hit = false;
			downRayColor = Color.red;
		}
		Debug.DrawRay(transform.position + downRay2Offset, transform.TransformDirection(downRay2Direction) * downRay2Range, downRayColor);
		
		if (downRay1Hit && downRay2Hit) {
			grounded = true;
			animController.SetBool("Grounded",true);
		} else {
			grounded = false;
			animController.SetBool("Grounded", false);
			animController.SetTrigger("JumpDown");
		}
	
		if (canFall && (frontBlocked || !grounded)) {
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
