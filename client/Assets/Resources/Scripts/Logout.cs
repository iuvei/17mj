using UnityEngine;
using Facebook.Unity;
using Facebook.MiniJSON;

public class Logout : MonoBehaviour {
    public GameObject loadingPanel;
    public GameObject AudioPanel;

    private GameObject AccountManagerPanel;
    private GameObject UnityFacebookSDK;
    //private static AndroidJavaObject login = null;
    //private static AndroidJavaObject currentActivity = null;

    void Start() {
        if (!AccountManagerPanel) {
            AccountManagerPanel = GameObject.FindGameObjectWithTag("AccountManager");   
        }

//#if UNITY_ANDROID && !UNITY_EDITOR
//        var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//        currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//        var loginClass = new AndroidJavaClass("com.foxgame.google.GoogleSignInDialog");
//        login = loginClass.CallStatic<AndroidJavaObject>("getInstance");
//        login.CallStatic("checkInit", this.gameObject.name, "OnConnected", currentActivity);
//#endif
    }

    public void ClickLogout() {
        //GLoginOut();
        FBLogout();
        ClearCache();

        if (loadingPanel) {
            loadingPanel.SetActive(true);
            EnterLoading.instance.StartLoading();
            if(AudioPanel)
                Destroy(AudioPanel);
            if(AccountManagerPanel)
                Destroy(AccountManagerPanel);
        }

    }

//    private void GLoginOut()
//    {
//        Debug.Log("GLoginOut()");
//#if UNITY_ANDROID
//        login.CallStatic("LoginOut");
//#endif
//    }

    private void FBLogout()
    {
        FB.LogOut();
    }

    private void ClearCache()
    {
        CryptoPrefs.DeleteKey("USERPHOTO");
        CryptoPrefs.DeleteKey("USERNAME");
        CryptoPrefs.DeleteKey("USERLEVEL");
        CryptoPrefs.DeleteKey("USERCOIN");
        CryptoPrefs.DeleteKey("USERONLINE");
        CryptoPrefs.DeleteKey("USERTOKEN");
    }
}
