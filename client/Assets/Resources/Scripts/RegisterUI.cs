using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using Facebook.MiniJSON;

public class RegisterUI : MonoBehaviour {
    static public RegisterUI Instance;

    public InputField RegisterName;     // 註冊頁 暱稱
    public InputField RegisterAccount;  // 註冊頁 帳號(信箱)
    public InputField RegisterPass1;   // 註冊頁 密碼
    public InputField RegisterPass2;   // 註冊頁 密碼確認
    public GameObject RegisterHint_Account;   // 註冊頁 帳號錯誤提示
    public GameObject RegisterHint_Pass1;     // 註冊頁 密碼錯誤提示
    public GameObject RegisterHint_Pass2;     // 註冊頁 第二密碼錯誤提示
    public GameObject RegisterBtn;

    public string registerMail;
    public string registerPass1;
    public string registerNickName;

    public string[] _canNickName; //罐頭暱稱
    private string _defaultNickName = "大島柚子"; //預設暱稱

    void Awake()
    {
        Instance = this;
    }

    void Start() {
        if (!RegisterName)
            Debug.Log("No found Register NickName InputField");
        if (!RegisterAccount)
            Debug.Log("No found Register Account InputField");
        if (!RegisterPass1)
            Debug.Log("No found Register Password1 InputField");
        if (!RegisterPass2)
            Debug.Log("No found Register Password2 InputField");
        if (!RegisterHint_Account)
            Debug.Log("No found Register Hint_Account");
        if (!RegisterHint_Pass1)
            Debug.Log("No found Register Hint_Password1");
        if (!RegisterHint_Pass2)
            Debug.Log("No found Register Hint_Password2");

        ResetAllInput();
    }

    public static string GetUniqueKey(int maxSize)
    {
        char[] chars = new char[62];
        chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[1];
        RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        crypto.GetNonZeroBytes(data);
        data = new byte[maxSize];
        crypto.GetNonZeroBytes(data);
        StringBuilder result = new StringBuilder(maxSize);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }

    //註冊頁-確定鈕
    public void ClubRegisterJoin()
    {
        registerMail = RegisterAccount.text;
        registerPass1 = RegisterPass1.text;
        string registerPass2 = RegisterPass2.text;

        //檢查欄位是否合法
        if (!CheckInput.instance.CheckEmail(registerMail))
        {
            RegisterHint_Account.GetComponentInChildren<Text>().text = "帳號格式錯誤";
            RegisterHint_Account.SetActive(true);
        }
        else
        {
            //檢查密碼是否合法
            if (!CheckInput.instance.CheckPass(registerPass1))
            {
                RegisterHint_Pass1.GetComponentInChildren<Text>().text = "密碼格式錯誤";
                RegisterHint_Pass1.SetActive(true);
            }
            if (!CheckInput.instance.CheckPass(registerPass2))
            {
                RegisterHint_Pass2.GetComponentInChildren<Text>().text = "密碼格式錯誤";
                RegisterHint_Pass2.SetActive(true);
            }
            else if (registerPass1 != registerPass2)
            {
                RegisterHint_Pass2.GetComponentInChildren<Text>().text = "密碼兩次不符";
                RegisterHint_Pass2.SetActive(true);
                Debug.Log("密碼兩次不符");
            }
            else
            {
                //檢查暱稱
                registerNickName = CheckNickName();
                string id = GetUniqueKey(24);
                string stype = "C";
                Button btn = RegisterBtn.GetComponent<Button>();
                btn.interactable = false;
				CryptoPrefs.SetString("USERTYPE", stype);
				CryptoPrefs.SetString("USERMAIL", registerMail);
                MJApi.AddMember(id, registerMail, registerPass1, registerNickName, stype, RegisterCallback);
            }
        }
    }

    //註冊 Callback
    private void RegisterCallback(WebExceptionStatus status, string result)
    {
        if (status != WebExceptionStatus.Success)
        {
            RegisterHint_Account.GetComponentInChildren<Text>().text = "此帳號已存在";
            RegisterHint_Account.SetActive(true);
            //Debug.Log("Failed! " + result);
        }
        else
        {
            //Debug.Log("ConnectSuccess! " + result);

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
            ResetAllInput();
            UIManager.instance.StartSetEnterLoading(); //載入下個場景
        }
        Button btn = RegisterBtn.GetComponent<Button>();
        btn.interactable = true;
    }

    public void ResetAllInput() {
        RegisterName.text = "";
        RegisterAccount.text = "";
        RegisterPass1.text = "";
        RegisterPass2.text = "";
        RegisterHint_Account.SetActive(false);
        RegisterHint_Pass1.SetActive(false);
        RegisterHint_Pass2.SetActive(false);
    }

    //判斷有無填暱稱欄位
    private string CheckNickName() {
        string _nickName;
        if (RegisterName.text != "")
            _nickName = RegisterName.text;
        else {
            if (_canNickName.Length == 0)
                _nickName = _defaultNickName;
            else
            {
                int _randIndex = Random.Range(0, _canNickName.Length);
                _nickName = _canNickName[_randIndex];
            }
            RegisterName.text = _nickName;
        } 
        return _nickName;
    }

    //點擊錯誤提示區塊 清空欄位並聚焦
    public void ClickHintBlock(Button targetHint)
    {
        targetHint.gameObject.SetActive(false);
        targetHint.GetComponentInParent<InputField>().ActivateInputField();
    }

}
