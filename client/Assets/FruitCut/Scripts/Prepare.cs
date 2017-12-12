using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Prepare : MonoBehaviour {

	public Text guiPoints;

	MouseControl mouseControl;


	void Start () {
		mouseControl = GetComponent<MouseControl>();
	}


	void Update () {
		guiPoints.text = "Points: " + mouseControl.points;
	}

}