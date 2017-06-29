using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System.Collections;

public class FBLoginButton : MonoBehaviour {

    private string fbId = string.Empty;
    private string fbMail = string.Empty;

    void Awake () {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        } else {
            FB.Init(OnFBInitComplete, OnHideUnity);
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
                FB.API("/me/picture", HttpMethod.GET, FBPhotoCallback);
                FB.API("me?fields=name,email", HttpMethod.GET, FBUserCallBack);
            }
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
            //this.profilePic = result.Texture;
            //Sprite s = Sprite.Create(this.profilePic, new Rect(0, 0, this.profilePic.width, this.profilePic.height), Vector2.zero);
            //if (head != null) {
            //	head.sprite = s;
            //}
            //headMesh.GetComponent<Renderer>().material.mainTexture = result.Texture;
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        //"Success - Check log for details";
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
            Debug.Log("userid=" + aToken.UserId);
            int i = 0;
            foreach (string perm in aToken.Permissions)
                i++;            
            FB.API("/me/picture", HttpMethod.GET, FBPhotoCallback);
            FB.API("me?fields=name,email", HttpMethod.GET, FBUserCallBack);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void FBLogout()
    {
        FB.LogOut();
    }

    void FBUserCallBack(IResult result)
    {
        Debug.Log("UserCallBack()");
        IDictionary dict = Json.Deserialize(result.RawResult) as IDictionary;
        //if (dict["name"]!=null) {
        //	fbname =dict ["name"].ToString(); // 取得用戶名稱
        //}
        //if (nickname != null) {
        //	nickname.text = fbname;
        //}

        if (dict["email"] != null)
        {
            fbMail = dict["email"].ToString();
        }
        doLogin1();
    }

    public void doLogin1()
    {
        Debug.Log("doLogin1(" + fbMail + " " + fbId + ")");
        //YkiApi.Login("1", mail, fbId, waitServerStatusCallback);
        //StartCoroutine(CheckServerStatus(LoginUI.Instance.LoginCallback));
    }

   
    
}
