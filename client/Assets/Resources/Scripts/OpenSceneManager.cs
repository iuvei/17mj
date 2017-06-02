using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpenSceneManager : MonoBehaviour {
    public Transform _17logoVideoManager;  // 片頭動畫
    public Transform foxGameLogo;          // FoxGame 商標
    public Transform _17logoStartPanel;    // 版權分級介面
    public GameObject enterLoadingPanel;   // 讀取畫面

    private MediaPlayerCtrl _17LogoMovCtrl;
    private Animator foxGameLogoAnim;
    private Animator _17LogoPanelAnim;
    private GameObject flowLightLogo;      // 流光Logo
    private Animator _StartBtnAnim;        // START按鈕動畫
    private Button _StartBtn;

    void Start () {
        if (!_17logoVideoManager)
            Debug.LogError("No found 17 Logo Video Manager");
        else
            _17LogoMovCtrl = _17logoVideoManager.GetComponent<MediaPlayerCtrl>();

        if (!foxGameLogo)
            Debug.LogError("No found FoxGameLogoPanel");
        else
            foxGameLogoAnim = foxGameLogo.GetComponent<Animator>();

        if (!_17logoStartPanel)
            Debug.LogError("No found 17 Logo panel");
        else {
            _17LogoPanelAnim = _17logoStartPanel.GetComponent<Animator>();
            _StartBtnAnim = _17logoStartPanel.Find("Panel/StartButton/ScaleBtnText").gameObject.GetComponent<Animator>();
            flowLightLogo = _17logoStartPanel.Find("FlowLightImg").gameObject;
        }

        if (!_StartBtnAnim)
            Debug.LogError("No found START text");
        else
            _StartBtn = _StartBtnAnim.gameObject.GetComponentInParent<Button>();

        if (!enterLoadingPanel)
            Debug.LogError("No found Loading panel");

        StartCoroutine("PlayOP");  //開始
    }

    //遊戲流程
    IEnumerator PlayOP()
    {
        yield return new WaitForSeconds(0.1f);     
        foxGameLogoAnim.SetTrigger("FoxGameLogo"); // 01-出現 FoxGame
        yield return new WaitForSeconds(3.3f);     

        _17LogoMovCtrl.enabled = true;             // 02-播放17玩麻將 片頭動畫
        yield return new WaitForSeconds(5.2f);

        _17LogoMovCtrl.enabled = false;            // 03-停止17玩麻將 片頭動畫
        _17LogoMovCtrl.gameObject.SetActive(false);

        flowLightLogo.SetActive(true);             // 04-流光Logo&開始介面
        _17LogoPanelAnim.SetTrigger("_17LogoPanel");
        _StartBtn.enabled = true;
    }

    //點擊 START
    public void GoLoginPage() {
        StopCoroutine("PlayOP");

        _StartBtnAnim.SetTrigger("PressScreen");
        _17LogoPanelAnim.SetTrigger("screenFadeOut");

        StartCoroutine("StartLoading"); //讀取下一場景
    }

    IEnumerator StartLoading() {
        yield return new WaitForSeconds(1.5f);
        EnterLoading.instance.StartLoading();
    }
}
