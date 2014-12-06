using UnityEngine;
using System.Collections;

public class InteractiveBlockController : MonoBehaviour {
	public GameObject affectedBlockPrefab;			// Prefab del bloque sobre el que actua
	public int maxLevelsDistance;					// Distancia vertical máxima a la que se puede colocar el bloque en el que actua
	public AudioClip soundAid;						// Pista sonora
	public Color color = Color.blue;				// Color de conexión con el affectedController
	
	public AffectedBlockController affectedBlockController;
	
	public void SetAffectedBlock(AffectedBlockController affectedController) {
		affectedBlockController = affectedController;
		foreach (InteractiveItemScript itemScript in transform.GetComponentsInChildren<InteractiveItemScript>()) {
			itemScript.affectedBlockController = affectedBlockController.transform.GetComponent<AffectedBlockController>();
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = color;
		if (affectedBlockController != null) {
			Gizmos.DrawLine(transform.position, affectedBlockController.transform.position);
				}

	
	}
}
