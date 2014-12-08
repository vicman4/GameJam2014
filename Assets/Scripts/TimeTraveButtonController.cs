using UnityEngine;
using System.Collections;

public class TimeTraveButtonController : MonoBehaviour {
	Transform panoramicCam;
	GameDirector gameDirector;
	public bool lookAtCam = false;
	

	void Start () {
		panoramicCam = GameObject.Find("PanoramicCamera").transform;
		gameDirector = GameObject.Find("Main Camera").transform.GetComponent<GameDirector>();
	}
	
	void FixedUpdate () {
		if (lookAtCam) transform.LookAt(panoramicCam);
	}
	
	void OnMouseDown() {
		if (!gameDirector.IsDoppelganger()) {
			gameDirector.TravelTo(transform.parent.position);
		}
	}
}
