using UnityEngine;
using System.Collections;

public class InteractiveBlockController : MonoBehaviour {
	public GameObject affectedBlockPrefab;			// Prefab del bloque sobre el que actua
	public int maxLevelsDistance;					// Distancia vertical máxima a la que se puede colocar el bloque en el que actua
	public AudioClip soundAid;						// Pista sonora
	
	//void OnCollisionEnter(Collision other) {
		// TODO: hacer que suene la pista sonora
	//}
}
