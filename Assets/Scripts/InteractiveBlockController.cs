using UnityEngine;
using System.Collections;

public class InteractiveBlockController : MonoBehaviour {
	public GameObject affectedBlockPrefab;			// Prefab del bloque sobre el que actua
	public int maxLevelsDistance;					// Distancia vertical máxima a la que se puede colocar el bloque en el que actua
	public AudioClip soundAid;						// Pista sonora
	public Color color = Color.blue;				// Color de conexión con el affectedController
	
	public GameObject affectedBlock; // Instancia del prefab con el que interacciona
	
	private AffectedBlockController affectedBlockController;
	
	void Start() {
		affectedBlockController = affectedBlock.GetComponent<AffectedBlockController>();
		

	}


	//void OnCollisionEnter(Collision other) {
		// TODO: hacer que suene la pista sonora
	//}





	void OnDrawGizmos() {
		Gizmos.color = color;
		if (affectedBlock != null) {
			Gizmos.DrawLine(transform.position, affectedBlock.transform.position);
				}

	
	}
}
