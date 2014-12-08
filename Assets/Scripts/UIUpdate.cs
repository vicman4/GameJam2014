using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIUpdate : MonoBehaviour {

	public Text score_text, level_text;
	public GameDirector gd;
	// Use this for initialization
	void Start () {
		score_text.text = "000";
	}
	
	// Update is called once per frame
	void Update () {
		var time = Mathf.Round(Time.timeSinceLevelLoad * 100);
		level_text.text = " "+gd.playerLevel.ToString ();
		score_text.text = " "+ time.ToString();
	}
}
