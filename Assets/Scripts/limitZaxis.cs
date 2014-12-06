using UnityEngine;
using System.Collections;

public class limitZaxis : MonoBehaviour {
	public float maxZ = 0;
	public float minZ = 0;

	Vector3 actualPos;

	
	// Update is called once per frame
	void Update () {
		actualPos = transform.localPosition;
		actualPos.z = Mathf.Clamp (actualPos.z, minZ, maxZ);
		transform.localPosition = actualPos;
	}
}
