﻿using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Collections;
using Facebook.MiniJSON;

public class LoginUI : MonoBehaviour {
    public static LoginUI Instance;

	public InputField ClubLoginAccount;       // 登入帳號
	public InputField ClubLoginPass;          // 登入密碼
	public GameObject ClubLoginHint_Account;  // 帳號錯誤提示
    public GameObject ClubLoginHint_Password; // 密碼錯誤提示

    void Awake() {
        Instance = this;
    }

    void Start() {
        if(!ClubLoginAccount)
            Debug.Log("No found Login Account InputField");
        else
            ClubLoginAccount.text = "";

        if (!ClubLoginPass)
            Debug.Log("No found Login Password InputField");
        else
            ClubLoginPass.text = "";

        if (!ClubLoginHint_Account)
            Debug.Log("No found Login LoginHint_Account");
        if (!ClubLoginHint_Password)
            Debug.Log("No found Login LoginHint_Password");            
    }

	//登入按鈕
	public void ClubSigninClick() {
		string userName = ClubLoginAccount.text;
		string userPass = ClubLoginPass.text;

        if (!CheckInput.instance.CheckPass(userPass)) //檢查密碼
        {
            ClubLoginHint_Password.GetComponentInChildren<Text>().text = "密碼格式錯誤";
            ClubLoginHint_Password.SetActive(true);
        }
        if (!CheckInput.instance.CheckEmail(userName)) //檢查帳號
        {  
            ClubLoginHint_Account.GetComponentInChildren<Text>().text = "帳號格式錯誤";
            ClubLoginHint_Account.SetActive (true);
		} else {
            string stype = "C";
            MJApi.Login(stype, userName, userPass, LoginCallback);
        }
	}

    public void LoginCallback(WebExceptionStatus status, string result)
	{
		if (status!=WebExceptionStatus.Success){
            ClubLoginHint_Password.GetComponentInChildren<Text> ().text = "登入失敗: 輸入資訊錯誤";
            ClubLoginHint_Password.SetActive (true);
            //Debug.Log("登入失敗: 輸入資訊錯誤");		
		} else {
            //Debug.Log ("登入成功! Token= "+ result);
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
            UIManager.instance.StartSetEnterLoading(); //載入下個場景
            EnterLoading.instance._autoToNextScene = true;
        }
    }

    //點擊錯誤提示區塊 清空欄位並聚焦
    public void ClickHintBlock(Button targetHint) {
        targetHint.gameObject.SetActive(false);
        targetHint.GetComponentInParent<InputField>().ActivateInputField();
    }

    public void Logout()
    {
        CryptoPrefs.DeleteKey("USERPHOTO");
        CryptoPrefs.DeleteKey("USERNAME");
        CryptoPrefs.DeleteKey("USERLEVEL");
        CryptoPrefs.DeleteKey("USERCOIN");
        CryptoPrefs.DeleteKey("USERONLINE");
        CryptoPrefs.DeleteKey("USERTOKEN");
    }
}
