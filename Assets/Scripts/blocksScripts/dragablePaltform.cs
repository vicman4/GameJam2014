using UnityEngine;
using System.Collections;

public class dragablePaltform : MonoBehaviour {
	// Define some target to place object in. It can be any Collider
	public Collider target;
	public Vector3
		screenPoint,
		offset,
		scanPos,
		curPosition,
		curScreenPoint;
	public  Vector3 limitTop, limitDown, limitLeft, limitRigth;
	
	
	float gridSize = 0.20f;
	
	void Start(){
		limitLeft = transform.parent.Find ("left").position;
		limitRigth = transform.parent.Find ("right").position;
		limitDown = transform.parent.Find ("down").position;
		limitTop = transform.parent.Find ("top").position;

	}
	
	void OnMouseDown() {
		scanPos = gameObject.transform.position;
		screenPoint = Camera.main.WorldToScreenPoint(scanPos);
		offset = scanPos - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		
	}
	
	
	
	
	void OnMouseDrag() {
		curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint);// + offset;
		
		
		//curPosition.x = (float)(Mathf.Round(curPosition.x) * gridSize);
		if(limitLeft!= Vector3.zero && limitRigth!= Vector3.zero)
			curPosition.x  = Mathf.Clamp(curPosition.x , limitLeft.x, limitRigth.x);

		if(limitTop!= Vector3.zero && limitDown!= Vector3.zero)
			curPosition.y  = Mathf.Clamp(curPosition.y , limitDown.y, limitTop.y);
		transform.position = curPosition;
	}

}
