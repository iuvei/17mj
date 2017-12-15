using UnityEngine;
using Facebook.Unity;
using Facebook.MiniJSON;

public class Logout : MonoBehaviour {
    public GameObject loadingPanel;
    public GameObject AudioPanel;

    private GameObject AccountManagerPanel;
    private GameObject UnityFacebookSDK;

    void Start() {
        if (!AccountManagerPanel) {
            AccountManagerPanel = GameObject.FindGameObjectWithTag("AccountManager");   
        }
    }

    public void ClickLogout() {
    #if UNITY_ANDROID || UNITY_IOS && !UNITY_EDITOR
            GLoginOut();
            FBLogout();
    #endif

        ClearCache();

        if (loadingPanel) {
            loadingPanel.SetActive(true);
            EnterLoading.instance.StartLoading();
            if(AccountManagerPanel)
                Destroy(AccountManagerPanel);
            if (AudioPanel)
                //Destroy(AudioPanel);
                AudioPanel.SetActive(false);
        }

    }

    private void GLoginOut()
    {
        AccountManagerPanel.GetComponent<GLoginButton>().GLoginOut();
    }

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
        CryptoPrefs.DeleteKey("USERTOKEN");
		CryptoPrefs.DeleteKey("USERTYPE");
		CryptoPrefs.DeleteKey("USERMAIL");
		CryptoPrefs.DeleteKey("USERFLOGIN");
		CryptoPrefs.DeleteKey("USERLOGINTOTAL");
		CryptoPrefs.DeleteKey("USERWIN");
		CryptoPrefs.DeleteKey("USERLOSE");
    }
}
