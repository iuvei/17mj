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
        GLoginOut();
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
        PlayerPrefs.DeleteKey("USERPHOTO");
        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("USERLEVEL");
        PlayerPrefs.DeleteKey("USERCOIN");
        PlayerPrefs.DeleteKey("USERONLINE");
        PlayerPrefs.DeleteKey("USERTOKEN");
    }
}
