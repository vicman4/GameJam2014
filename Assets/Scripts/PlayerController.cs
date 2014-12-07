using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Vector3 speed;
	public float maxSpeed = 10f;

	void FixedUpdate () {
		if (rigidbody.velocity.magnitude < maxSpeed)
			rigidbody.AddForce(speed,ForceMode.Force);

	}
}
