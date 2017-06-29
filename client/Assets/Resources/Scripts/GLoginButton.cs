﻿using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

public class GLoginButton : MonoBehaviour {
    private string gId = string.Empty;
    private string gMail = string.Empty;
    private static AndroidJavaObject login = null;
    private static AndroidJavaObject currentActivity = null;

#if UNITY_ANDROID && !UNITY_EDITOR
    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            GLogin();
        });

        var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var loginClass = new AndroidJavaClass("com.foxgame.google.GoogleSignInDialog");
        login = loginClass.CallStatic<AndroidJavaObject>("getInstance");
    }

    private void GLogin()
    {
        Debug.Log("GLogin()");
        login.CallStatic("Login", this.gameObject.name, "OnConnected", currentActivity);
    }   

    public void OnConnected(string name)
    {
        Debug.Log("OnConnected() = " + name);
    }

#endif     
}
