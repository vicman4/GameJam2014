using UnityEngine;
using System.Collections;

public class dragablePaltform : MonoBehaviour {
	// Define some target to place object in. It can be any Collider
	Vector3
		scanPos,
		curPosition;
	public float relativeMove = 0.1f;
	public  Vector3 limitTop, limitDown, limitLeft, limitRigth;
	
	

	void Start(){
		limitLeft = transform.parent.Find ("left").position;
		limitRigth = transform.parent.Find ("right").position;
		limitDown = transform.parent.Find ("down").position;
		limitTop = transform.parent.Find ("top").position;

	}
	
	void OnMouseDown() {
		scanPos = Input.mousePosition;
	}
	
	
	
	
	void OnMouseDrag() {

		
		curPosition = Input.mousePosition;
		curPosition = (curPosition - scanPos)*relativeMove;
		//curPosition.x = (float)(Mathf.Round(curPosition.x) * gridSize);

		curPosition.x  = Mathf.Clamp(curPosition.x+transform.position.x, limitLeft.x, limitRigth.x);
		curPosition.y  = Mathf.Clamp(curPosition.y+transform.position.y, limitDown.y, limitTop.y);
		curPosition.z = transform.position.z;
		transform.position = curPosition;
		scanPos = Input.mousePosition;
	}

}
