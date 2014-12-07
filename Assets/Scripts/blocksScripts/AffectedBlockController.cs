using UnityEngine;
using System.Collections;

public class AffectedBlockController : MonoBehaviour {

	public InteractiveBlockController interactivaBlockController;
	private AffectedItemScript[] affectedItems;



	public void toggle(){
		affectedItems = GetComponentsInChildren<AffectedItemScript> ();
		foreach (AffectedItemScript item in affectedItems) {
			item.activate();
				}
	}


}
