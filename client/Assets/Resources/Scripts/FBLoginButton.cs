using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System.Collections;
using System;
using System.Net;

public class FBLoginButton : MonoBehaviour {

    public Text _logText;

    private string fbId = string.Empty;
    private string fbMail = string.Empty;
    private string fbPhoto = string.Empty;
    private string sName = string.Empty;

    void Awake () {
        sName = CryptoPrefs.GetString("USERNAME");
        fbPhoto = CryptoPrefs.GetString("USERPHOTO");

        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
            //Debug.Log("Awake FB.IsInitialized");
            _logText.text = "Awake FB.IsInitialized";
        } else {
            FB.Init(OnFBInitComplete, OnHideUnity);
            //Debug.Log("Awake FB.Is NOT Initialized");
            _logText.text = "Awake FB.Is NOT Initialized0";
        }
    }

    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            FBLogin();
        });
    }

    private void OnFBInitComplete()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
#if UNITY_ANDROID || UNITY_IOS && !UNITY_EDITOR
            FB.ActivateApp();
#endif
            if (FB.IsLoggedIn)
            {
                var aToken = AccessToken.CurrentAccessToken;
                fbId = aToken.UserId;
                if (string.IsNullOrEmpty(fbPhoto))
                    FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPhotoCallback);
                FB.API("me?fields=name,email", HttpMethod.GET, FBUserCallBack);
                _logText.text += "\n OnFBInitComplete > FB.IsInitialized >　FB.IsLoggedIn";
            }
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
            _logText.text += "\n Failed to Initialize the Facebook SDK";
        }
    }

    void FBPhotoCallback(IGraphResult result)
    {
        Debug.Log("FBPhotoCallback()");
        if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        {
            string stringData = Convert.ToBase64String(result.Texture.EncodeToPNG());
            CryptoPrefs.SetString("USERPHOTO", stringData);
        }
        _logText.text += "\n FBPhotoCallback()";
    }

    private void OnHideUnity(bool isGameShown)
    {
    }

    private void FBLogin()
    {
        Debug.Log("doFBLogin()");
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;
            fbId = aToken.UserId;
            int i = 0;
            foreach (string perm in aToken.Permissions)
                i++;
            if (string.IsNullOrEmpty(fbPhoto))
                FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPhotoCallback);
            FB.API("me?fields=name,email", HttpMethod.GET, FBUserCallBack);
            _logText.text += "\n AuthCallback >　FB.IsLoggedIn";
        }
        else
        {
            Debug.Log("User cancelled login");
            _logText.text += "\n AuthCallback > User cancelled login";
        }
    }

    public void clearLog() {
        _logText.text = "";
    }

    public void FBLogout()
    {
        Debug.Log("FBLogout");
        FB.LogOut();
        CryptoPrefs.DeleteKey("USERPHOTO");
        CryptoPrefs.DeleteKey("USERNAME");
        CryptoPrefs.DeleteKey("USERLEVEL");
        CryptoPrefs.DeleteKey("USERCOIN");
        CryptoPrefs.DeleteKey("USERONLINE");
        CryptoPrefs.DeleteKey("USERTOKEN");
        _logText.text += "\n FBLogout";
    }

    private void LoginCallback(WebExceptionStatus status, string result)
    {
        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("Failed! " + result);
        }
        else
        {
            //Debug.Log("ConnectSuccess!" + result);
            string uName = string.Empty;
            string uToken = string.Empty;
            string uLevel = string.Empty;
            string uCoin = string.Empty;

            IDictionary dict = Json.Deserialize(result) as IDictionary;
            if (dict["Name"] != null)
            {
                uName = dict["Name"].ToString();
                CryptoPrefs.SetString("USERNAME", uName);
            }
            if (dict["Token"] != null)
            {
                uToken = dict["Token"].ToString();
                CryptoPrefs.SetString("USERTOKEN", uToken);
            }
            if (dict["Level"] != null)
            {
                uLevel = dict["Level"].ToString();
                CryptoPrefs.SetString("USERLEVEL", uLevel);
            }
            if (dict["Coin"] != null)
            {
                uCoin = dict["Coin"].ToString();
                CryptoPrefs.SetString("USERCOIN", uCoin);
            }
        }
        EnterLoading.instance._autoToNextScene = true;
    }


    void FBUserCallBack(IResult result)
    {
        Debug.Log("UserCallBack()");
        IDictionary dict = Json.Deserialize(result.RawResult) as IDictionary;
        if (dict["name"] != null && string.IsNullOrEmpty(sName))
            sName = dict["name"].ToString();
        if (dict["email"] != null)
            fbMail = dict["email"].ToString();
        doLogin();
        _logText.text += "\n FBUserCallBack()";
    }

    public void doLogin()
    {
        string stype = "F";
        string token = CryptoPrefs.GetString("USERTOKEN");

        if (string.IsNullOrEmpty(token))
        {
            MJApi.AddMember(fbId, fbMail, "1", sName, stype, LoginCallback);
        }
        else
        {
            MJApi.Login(stype, fbMail, token, LoginCallback);
        }
        _logText.text += "\n doLogin";
        UIManager.instance.StartSetEnterLoading();
    }   
}
