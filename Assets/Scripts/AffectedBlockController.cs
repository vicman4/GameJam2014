using UnityEngine;
using System.Collections;

public class AffectedBlockController : MonoBehaviour {

	public InteractiveBlockController interactivaBlockController;
	private AffectedItemScript[] affectedItems;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void toggle(){
		affectedItems = GetComponentsInChildren<AffectedItemScript> ();
		foreach (AffectedItemScript item in affectedItems) {
			item.activate();
				}
	}


}
