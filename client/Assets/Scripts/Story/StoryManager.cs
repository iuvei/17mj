using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {

	private static StoryManager _instance = null;

	public static StoryManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new StoryManager();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			//DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
