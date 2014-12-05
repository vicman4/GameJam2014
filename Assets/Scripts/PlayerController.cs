using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Vector3 speed;

	void FixedUpdate () {
		rigidbody.AddForce(speed);
	}
}
