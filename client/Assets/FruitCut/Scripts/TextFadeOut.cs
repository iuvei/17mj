using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFadeOut : MonoBehaviour {

	public float speed = 0.5f; 
	Color color;

	void Start () {

		color = GetComponent<Text>().color;
	
	}
	
	void Update () {

		if (gameObject.activeSelf)
		{
			color.a -= Time.deltaTime * speed;
			GetComponent<Text>().color = color;
		}
	
	}
}
