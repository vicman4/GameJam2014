using UnityEngine;
using System.Collections;

public class rotateScript : MonoBehaviour {
	public float velocity;
	public Vector3 ejes;
	
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (ejes * velocity * Time.deltaTime);
	}
}
