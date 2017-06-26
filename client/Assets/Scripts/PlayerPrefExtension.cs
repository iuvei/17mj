using UnityEngine;
using System.Collections;
using UnityEngine;

public static class PlayerPrefExtension
{
	public static bool GetBool (string key)
	{
		return PlayerPrefs.GetInt (key) == 1;
	}

	public static void SetBool (string key, bool state)
	{
		PlayerPrefs.SetInt (key, state ? 1 : 0);
	}
}
