using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using DG.Tweening;

public class ForgotUI : MonoBehaviour
{
    public static ForgotUI instance;

    public GameObject ForgotPage1;  // 忘記密碼 第一頁
    public GameObject ForgotPage2;  // 忘記密碼 第二頁

    public InputField _mail;        // 用戶信箱
    public InputField _confirmCode; // 驗證碼
    public GameObject _mailHint;    // 用戶信箱提示
    public GameObject _timeHint;    // 剩餘時間提示
    public GameObject _codeHint;    // 驗證碼錯誤提示
    public Button btnSendCode;      // 發送驗證碼鈕
    public Button btnAccept;        // 驗證碼確定鈕
    public Sprite[] btnBackground;
    public Material flowLightMat;

    public InputField _password1;   // 新密碼
    public InputField _password2;   // 新密碼確認
    public Image _passEye1;         // 新密碼顯示按鈕1
    public Image _passEye2;         // 新密碼顯示按鈕2
    public Text _PassText;
    public Text _ConfirmPassText;
    public GameObject _passHint;         // 錯誤提示
    public Button buttonPassAccept;      // 密碼確定鈕
    public Sprite[] changePassButtonBG;
    public Sprite[] EyeButton;           // 顯示密碼 眼睛圖
    public GameObject connectingPanel;  //連線視窗

    private string playerMail = string.Empty; //暫存玩家信箱
    private bool _needHideConnect = false;
    private bool _sendAuthCodeCb = false;
    private bool _forgotCodeCb = false;
    private bool _setUserPwdCb = false;
    private string sendAuthCodeCbResult = string.Empty;
    private string forgotCodeCbResult = string.Empty;
    private string setUserPwdCbResult = string.Empty;


    void Start()
    {
        instance = this;
    }

    void Update() {

        if (_needHideConnect)
        {
            ConnectPanelSwitch(false); //關閉連線視窗
            _needHideConnect = false;
        }

        if (_forgotCodeCb) {
            _forgotCodeCb = false;

            if (forgotCodeCbResult == "OK")
            {
                GoResetPassPage(); //前往重設密碼
            }
            else
            {
                _confirmCode.text = ""; //清空驗證碼欄位
                ButtonUISwitch(btnAccept, true); //確定驗證碼按鈕UI變化
                ShowHint(_codeHint, "輸入錯誤，請重新輸入");
            }
        }


        if (_sendAuthCodeCb) {
            _sendAuthCodeCb = false;

            if (sendAuthCodeCbResult == "OK")
            {
                ButtonUISwitch(btnSendCode, true); //發送驗證碼按鈕變色
                _timeHint.GetComponent<CountDown>().Show(30); //倒數
                ShowHint(_mailHint, "驗證碼已發送，請至信箱收信");
            } else if (sendAuthCodeCbResult == "Already Sent.")
            {
                ShowHint(_mailHint, "已寄出，請稍後再試");
            } else {
                ShowHint(_mailHint, "此帳戶不存在");
            }
        }

        if (_setUserPwdCb)
        {
            _setUserPwdCb = false;

            if (setUserPwdCbResult == "OK")
            {
                //重置所有欄位 回登入頁
                UIManager.instance.ExitForgotPage();
			} else if (setUserPwdCbResult == "The remote server returned an error: (410) Gone.") {

			}
            else
            {
                Debug.Log("重置密碼失敗 = " + setUserPwdCbResult);
                ShowHint(_passHint, "不明原因錯誤，請稍後再試");
            }
        }

    }

    // 發送驗證碼按鈕
    public void SendConfirm()
    {
        string forgotMail = _mail.text;
        playerMail = forgotMail;

        Text ForgotHintText = _mailHint.GetComponent<Text>();

        //檢查帳號是否合法
        if (!CheckInput.instance.CheckEmail(forgotMail))
        {
            ShowHint(_mailHint, "信箱格式有誤，請重新輸入");
        }
        else
        {
            MJApi.getAuthCode(forgotMail, AuthCodeCallback);
            ConnectPanelSwitch(true); //開啟連線視窗
        }
    }

    //發送驗證碼 Callback
    private void AuthCodeCallback(WebExceptionStatus status, string result)
    {
        _needHideConnect = true; //需要關閉連線視窗
        sendAuthCodeCbResult = result;
        _sendAuthCodeCb = true;

        Debug.Log("Status = " + status + ", result = " + result);
        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("發送驗證碼 AuthCodeCallback Statue != WebExceptionStatus.Success ");
        }
    }

    // 確定驗證碼
    public void AcceptAuthCode()
    {
		string forgotMail = _mail.text;
		string forgotConfirmCode = _confirmCode.text;

        MJApi.setAuthCode(forgotMail, forgotConfirmCode, ForgotCodeCallback);
        ConnectPanelSwitch(true); //開啟連線視窗
    }

    //確認驗證碼 Callback
    private void ForgotCodeCallback(WebExceptionStatus status, string result)
    {
        _needHideConnect = true; //需要關閉連線視窗
        forgotCodeCbResult = result;
        _forgotCodeCb = true;
        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("確認驗證碼 ForgotCodeCallback Statue != WebExceptionStatus.Success ");
            Debug.Log("Statue = " + status + ", result = " + result);
        }
        else
        {
            Debug.Log("確認驗證碼成功 result =" + result);
        }
    }

    // 前往重設密碼
    private void GoResetPassPage()
    {
        ForgotPage2.SetActive(true);
    }

    // 依傳入字串顯示題示
    private void ShowHint(GameObject _targetHint, string _str)
    {
        _targetHint.GetComponent<Text>().text = _str;
        _targetHint.SetActive(true);
    }

    // 按鈕UI變化
    private void ButtonUISwitch(Button _targetButton, bool _lock) {
        if (_lock)
        {
            if (_targetButton == buttonPassAccept)
                _targetButton.GetComponent<Image>().sprite = changePassButtonBG[0];
            else
                _targetButton.GetComponent<Image>().sprite = btnBackground[0];

            _targetButton.GetComponent<Image>().material = null;
            _targetButton.GetComponent<Button>().enabled = false;
            _targetButton.GetComponentInChildren<Text>().color = Color.gray;
        }
        else {
            if (_targetButton == btnSendCode)
            {
                _timeHint.GetComponent<CountDown>().Hide();
                _targetButton.GetComponent<Image>().sprite = btnBackground[1];
            }
            else if (_targetButton == btnAccept)
                _targetButton.GetComponent<Image>().sprite = btnBackground[2];
            else
                _targetButton.GetComponent<Image>().sprite = changePassButtonBG[1];

            _targetButton.GetComponent<Image>().material = flowLightMat;
            _targetButton.GetComponent<Button>().enabled = true;
            _targetButton.GetComponentInChildren<Text>().color = Color.white;
        }
    }

    //發送驗證碼按鈕 UI變化
    public void UnLockSendAuthCodeBtn()
    {
        ButtonUISwitch(btnSendCode, false);
    }

    //重置密碼
    public void ResetPassword()
    {
        //檢查 1.各欄位必填 2.兩次密碼
        string resetPass1 = _password1.text;
        string resetPass2 = _password2.text;

        Text ForgotHintText = _passHint.GetComponent<Text>();

        //檢查密碼是否合法
        if (resetPass1 != resetPass2)
        {
            ButtonUISwitch(buttonPassAccept, true);
            ShowHint(_passHint, "密碼不符，請重新輸入");
        }
        else
        {
            MJApi.setForgetPwd(playerMail, resetPass1, setUserPwdCallback);
            ConnectPanelSwitch(true); //開啟連線視窗
        }
    }

    //忘記密碼 - 重置密碼 Callback
    private void setUserPwdCallback(WebExceptionStatus status, string result)
    {
        _needHideConnect = true; //需要關閉連線視窗
        setUserPwdCbResult = result;
        _setUserPwdCb = true;

        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("重置密碼 Statue != WebExceptionStatus.Success ");
		}
        else
        {
            Debug.Log("重置成功 " + result);
        }
    }

    // 密碼眼睛鈕
    public void PassButtonEyeToggle(int _index) {
        switch (_index) {
            case 1:
                if (_passEye1.sprite == EyeButton[0])
                {
                    ShowHideToogle(_index, true);
                    _passEye1.sprite = EyeButton[1];
                }
                else {
                    ShowHideToogle(_index, false);
                    _passEye1.sprite = EyeButton[0];
                }
                break;
            case 2:
                if (_passEye2.sprite == EyeButton[0])
                {
                    ShowHideToogle(_index, true);
                    _passEye2.sprite = EyeButton[1];
                }
                else
                {
                    ShowHideToogle(_index, false);
                    _passEye2.sprite = EyeButton[0];
                }
                break;
        }
    }

    private void ShowHideToogle(int _index, bool _isShow) {
        switch (_index) {
            case 1:
                if (_isShow) {
                    _password1.inputType = InputField.InputType.Standard;
                    _PassText.text = _password1.text;
                }
                else {
                    _password1.inputType = InputField.InputType.Password;
                    _PassText.text = "";
                    for (int i = 0; i < _password1.ToString().Length; i++)
                        _PassText.text += "*";
                }
                break;
            case 2:
                if (_isShow)
                {
                    _password2.inputType = InputField.InputType.Standard;
                    _ConfirmPassText.text = _password2.text;
                }
                else
                {
                    _password2.inputType = InputField.InputType.Password;
                    _ConfirmPassText.text = "";
                    for (int i = 0; i < _password2.ToString().Length; i++)
                        _ConfirmPassText.text += "*";
                }
                break;
        }
    }

    //檢查 Email Input欄位內容
    public void CheckMailInputContent()
    {
        _mailHint.SetActive(false);
        if (CheckInput.instance.CheckEmail(_mail.text))
            ButtonUISwitch(btnSendCode, false);
        else
            ButtonUISwitch(btnSendCode, true);
    }

    //檢查驗證碼格式
    public void CheckCodeInputContent()
    {
        _codeHint.SetActive(false);
        if (_confirmCode.text.Length == 5) //※驗證碼長度5
            ButtonUISwitch(btnAccept, false);
        else
            ButtonUISwitch(btnAccept, true);
    }

    //檢查 Pass、ConfirmPass Input欄位內容
    public void CheckPassInputContent()
    {
        _passHint.SetActive(false);
        if (CheckInput.instance.CheckPass(_password1.text) && CheckInput.instance.CheckPass(_password2.text))
            ButtonUISwitch(buttonPassAccept, false);
        else
            ButtonUISwitch(buttonPassAccept, true);
    }

    public void ResetAllInput()
    {
        _mail.text = "";
        _confirmCode.text = "";
        _password1.text = "";
        _password2.text = "";
        ShowHideToogle(1, false);
        ShowHideToogle(2, false);
        _passEye1.sprite = EyeButton[0];
        _passEye2.sprite = EyeButton[0];
        ForgotPage2.SetActive(false);
    }

    private void ConnectPanelSwitch(bool _turnOn)
    {
        if (_turnOn)
			AccountManager.Instance.ShowConnecting ();
        else
			AccountManager.Instance.HideConnecting ();
    }
}