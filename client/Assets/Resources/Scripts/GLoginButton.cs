using UnityEngine;
using Facebook.MiniJSON;
using System;
using System.Collections;
using System.Net;
using UnityEngine.UI;
using Facebook.Unity;


public class GLoginButton : MonoBehaviour {
    public Text _logText;

    private static AndroidJavaObject login = null;
    private static AndroidJavaObject currentActivity = null;

    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            GLogin();
        });
#if UNITY_ANDROID && !UNITY_EDITOR
        var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var loginClass = new AndroidJavaClass("com.foxgame.google.GoogleSignInDialog");
        login = loginClass.CallStatic<AndroidJavaObject>("getInstance");
        login.CallStatic("checkInit", this.gameObject.name, "OnConnected", currentActivity);
        _logText.text = "Awake GLogin";
#endif
    }

    private void GLogin()
    {
        Debug.Log("GLogin()");
#if UNITY_ANDROID
        login.CallStatic("Login", this.gameObject.name, "OnConnected", currentActivity);
#endif
    }   

    public void GLoginOut()
    {
        Debug.Log("GLoginOut()");
        _logText.text += " \n GLoginOut ";
#if UNITY_ANDROID
        login.CallStatic("LoginOut");
#endif
        CryptoPrefs.DeleteKey("USERPHOTO");
        CryptoPrefs.DeleteKey("USERNAME");
        CryptoPrefs.DeleteKey("USERLEVEL");
        CryptoPrefs.DeleteKey("USERCOIN");
        CryptoPrefs.DeleteKey("USERONLINE");
        CryptoPrefs.DeleteKey("USERTOKEN");
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

    private IEnumerator GetGooglePhoto()
    {
#if UNITY_ANDROID
        byte[] result = login.CallStatic<byte[]>("GetUserPhoto");
        if (result != null)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.DXT1, false);
            tex.LoadImage(result);
            string stringData = Convert.ToBase64String(tex.EncodeToPNG());
            CryptoPrefs.SetString("USERPHOTO", stringData);
        }
#endif
        yield return null;
    }

    public void OnConnected(string result)
    {
        Debug.Log("OnConnected() = " + result);
        string uName = string.Empty;
        string uGid = string.Empty;
        string uMail = string.Empty;
        string[] tokens = result.Split(new string[] { "," }, StringSplitOptions.None);

        if (tokens[0] != null)
            uMail = tokens[0];

        if (uMail == "No Init")
        {
            string cName = CryptoPrefs.GetString("USERNAME");
            string cToken = CryptoPrefs.GetString("USERTOKEN");
            if (!string.IsNullOrEmpty(cName) && !string.IsNullOrEmpty(cToken))
            {
                string type = "C1";
                MJApi.Login(type, cName, cToken, LoginCallback);
                UIManager.instance.StartSetEnterLoading();
            }
        }
        else
        {
            if (tokens[1] != null)
                uGid = tokens[1];
            if (tokens[2] != null)
                uName = tokens[2];

            string Photo = CryptoPrefs.GetString("USERPHOTO");
            if (string.IsNullOrEmpty(Photo))
                StartCoroutine(GetGooglePhoto());

            string stype = "G";
            string token = CryptoPrefs.GetString("USERTOKEN");
            if (string.IsNullOrEmpty(token))
            {
                MJApi.AddMember(uGid, uMail, "1", uName, stype, LoginCallback);
            }
            else
            {
                MJApi.Login(stype, uMail, token, LoginCallback);
            }
            UIManager.instance.StartSetEnterLoading();
        }
        _logText.text += " \n OnConnected() "+ result;
    }

}
