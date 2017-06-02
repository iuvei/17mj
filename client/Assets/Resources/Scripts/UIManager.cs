using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public Transform MainPanel;
    public GameObject[] LoginBtns;

    private GameObject loginPanel;
    private GameObject registerPanel;
    private GameObject forgotPanel;
    private GameObject rulePanel;
    private Transform enterLoadingPanel;
    private Animator enterLoadingAnim;
    private Animator mainPanelAnim;

    void Start() {
        instance = this;

        if (!MainPanel)
            Debug.LogError("No found MainPanel");
        else {
            mainPanelAnim = MainPanel.GetComponent<Animator>();

            //抓取各個 panel
            loginPanel = MainPanel.Find("LoginPanel").gameObject;
            registerPanel = MainPanel.Find("RegisterPanel").gameObject;
            forgotPanel = MainPanel.Find("ForgotPanel").gameObject;
            rulePanel = MainPanel.Find("RulePanel").gameObject;
            enterLoadingPanel = MainPanel.Find("LoadingPanel");

            if (!loginPanel)
                Debug.Log("No found LoginPanel");
            if (!registerPanel)
                Debug.Log("No found RegisterPanel");
            if (!forgotPanel)
                Debug.Log("No found ForgotPanel");
            if (!rulePanel)
                Debug.Log("No found RulePanel");
            if (!enterLoadingPanel)
                Debug.Log("No found LoadingPanel");
            else
                enterLoadingAnim = enterLoadingPanel.GetComponent<Animator>();
        }

        if (LoginBtns.Length == 0)
            Debug.LogError("No found GameLobbyButton");

        InitialLoginPanel();
        StartCoroutine("PlayOP"); 
    }

    // 遊戲流程
    IEnumerator PlayOP() {
        yield return new WaitForSeconds(1f);
        mainPanelAnim.SetTrigger("loginFlag"); //畫面淡入
    }

    // 01-入口按鈕樣式初始化
    private void InitialLoginPanel()
    {
        for (int i = 0; i < 4; i++)
            LoginBtns[i].SetActive(true);
        for (int i = 4; i < 8; i++)
            LoginBtns[i].SetActive(false);
    }

    // 01-點擊"17玩麻將"按鈕，其他入口按鈕樣式改變
    public void Click17Play() {
        foreach (GameObject go in LoginBtns) go.SetActive((!go.activeSelf));
    }

    // 02-點擊"註冊"按鈕
    public void GoRegisterPage() {
        registerPanel.SetActive(true);
    }

    // 02-離開註冊醬玩會員
    public void ExitRegisterPage()
    {
        registerPanel.SetActive(false);
    }

    // 03-進入忘記密碼頁
    public void GoForgotPage() {
        forgotPanel.SetActive(true);
    }

    // 03-離開忘記密碼頁
    public void ExitForgotPage()
    {
        forgotPanel.SetActive(false);
        ForgotUI.instance.ResetAllInput();
    }

    // 04-進入服務條款頁
    public void GoRulePage()
    {
        rulePanel.SetActive(true);
    }

    // 04-離開服務條款頁
    public void ExitRulePage()
    {
        rulePanel.SetActive(false);
    }

    // 05-準備進入讀取畫面
    public void StartSetEnterLoading()
    {
        loginPanel.SetActive(false);
        EnterLoading.instance.StartLoading();
    }
}
