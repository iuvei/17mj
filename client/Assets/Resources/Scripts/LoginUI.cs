using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class LoginUI : MonoBehaviour {
    public static LoginUI Instance;

	public InputField ClubLoginAccount;       // 登入帳號
	public InputField ClubLoginPass;          // 登入密碼
	public GameObject ClubLoginHint_Account;  // 帳號錯誤提示
    public GameObject ClubLoginHint_Password; // 密碼錯誤提示

    void Awake () {
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

        // [自動登入] 若前次成功登入 這次則自動填入
        //ClubLoginAccount.text = PlayerPrefs.GetString ("USERNAME");
        //ClubLoginPass.text = PlayerPrefs.GetString ("USERPASS");
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
            //儲存此次帳密
            //PlayerPrefs.SetString ("USERNAME", userName);
            //PlayerPrefs.SetString ("USERPASS", userPass);

            MJApi.Login("C", userName, userPass, LoginCallback);
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
            UIManager.instance.StartSetEnterLoading(); //載入下個場景
        }
	}

    //點擊錯誤提示區塊 清空欄位並聚焦
    public void ClickHintBlock(Button targetHint) {
        targetHint.gameObject.SetActive(false);
        targetHint.GetComponentInParent<InputField>().ActivateInputField();
    }
}
