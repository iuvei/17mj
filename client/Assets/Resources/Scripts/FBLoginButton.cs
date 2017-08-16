using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System.Collections;
using System;
using System.Net;

public class FBLoginButton : MonoBehaviour {

    public Button[] _fbLoginBtn;
    public GameObject ConnectingPanel; // 連線中

    private bool _loginSuccess = false;  //設定資料
    private IDictionary dict;
    private bool _setPhoto = false;      //設定頭像
    private string stringData;
    private bool _loginDone = false;
    private bool _setPhotoDone = false;
    private string fbId = string.Empty;
    private string fbMail = string.Empty;
    private string fbPhoto = string.Empty;
    private string sName = string.Empty;
    private static FBLoginButton _instance = null;

    void Awake () {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        sName = CryptoPrefs.GetString("USERNAME");
        fbPhoto = CryptoPrefs.GetString("USERPHOTO");

        if (FB.IsInitialized && !string.IsNullOrEmpty(sName))
        {
            FB.ActivateApp();
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
            //Debug.Log("Awake FB.IsInitialized");
        } else {
            FB.Init(OnFBInitComplete, OnHideUnity);
            //Debug.Log("Awake FB.Is NOT Initialized");
        }
    }

    void Start () {
        //Button btn = GetComponent<Button>();
        //btn.onClick.AddListener(delegate
        //{
        //    FBLogin();
        //});

        if (_fbLoginBtn.Length != 0) {
            for (int i = 0; i < _fbLoginBtn.Length; i++)
            {
				if (_fbLoginBtn [i] != null) {
					_fbLoginBtn [i].onClick.AddListener (delegate {
						FBLogin ();
					});
				}
            }
        }
    }

    private void OnFBInitComplete()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
#if UNITY_ANDROID || UNITY_IOS && !UNITY_EDITOR
            FB.ActivateApp();
#endif
            /*
            if (FB.IsLoggedIn)
            {
                var aToken = AccessToken.CurrentAccessToken;
                fbId = aToken.UserId;
                if (string.IsNullOrEmpty(fbPhoto))
                    FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPhotoCallback);
                FB.API("me?fields=name,email", HttpMethod.GET, FBUserCallBack);
            }*/
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    void FBPhotoCallback(IGraphResult result)
    {
        Debug.Log("FBPhotoCallback()");
        if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        {
            stringData = Convert.ToBase64String(result.Texture.EncodeToPNG());
            _setPhoto = true;
        }
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
            else
                _setPhotoDone = true;
            FB.API("me?fields=name,email", HttpMethod.GET, FBUserCallBack);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void FBLogout()
    {
        Debug.Log("FBLogout");
        FB.LogOut();
    }

    private void LoginCallback(WebExceptionStatus status, string result)
    {
        if (ConnectingPanel) {
            ConnectingPanel.SetActive(false);
            UIManager.instance.StopConnectingAnim();
        }

        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("Failed! " + result);
        }
        else
        {
            dict = Json.Deserialize(result) as IDictionary;
            _loginSuccess = true;
        }
    }


    void FBUserCallBack(IResult result)
    {
        Debug.Log("UserCallBack()");
        IDictionary dictUser = Json.Deserialize(result.RawResult) as IDictionary;
        if (dictUser["name"] != null && string.IsNullOrEmpty(sName))
            sName = dictUser["name"].ToString();
        if (dictUser["email"] != null)
            fbMail = dictUser["email"].ToString();
        doLogin();
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
        if (ConnectingPanel) {
            ConnectingPanel.SetActive(true);
            UIManager.instance.PlayConnectingAnim();
        }
        //UIManager.instance.StartSetEnterLoading();
    }

    void Update() {
        if (_loginSuccess) {

            string uName = string.Empty;
            string uToken = string.Empty;
            string uLevel = string.Empty;
            string uCoin = string.Empty;


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
            _loginSuccess = false;
            _loginDone = true;
        }

        if (_setPhoto) {
            CryptoPrefs.SetString("USERPHOTO", stringData);
            _setPhoto = false;
            _setPhotoDone = true;
        }

        if (_loginDone && _setPhotoDone) {
            UIManager.instance.StartSetEnterLoading();
            _loginDone = false;
            _setPhotoDone = false;
        }
    }
}
