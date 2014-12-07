using UnityEngine;
using System.Collections;
using System.Linq;


public class ParedChangeController : MonoBehaviour {

	private Material[] wallMaterials;

	void Start () {
		wallMaterials = Resources.LoadAll ("MaterialsPared", typeof(Material)).Cast<Material>().ToArray();
		int randomWallIndex = Random.Range(0, wallMaterials.Length);
		var materials = renderer.materials;
		materials[0] = wallMaterials[randomWallIndex];
		renderer.materials = materials;
	}
}
