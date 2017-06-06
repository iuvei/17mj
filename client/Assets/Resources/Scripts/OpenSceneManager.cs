using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpenSceneManager : MonoBehaviour {
    public Transform foxGameLogo;          // FoxGame 商標
    public GameObject _17logoSequence;     // 片頭動畫序列動畫
    public Transform _17logoStartPanel;    // 版權分級介面
    public GameObject enterLoadingPanel;   // 讀取畫面

    private Animator foxGameLogoAnim;
    private Animator _17LogoPanelAnim;
    private GameObject flowLightLogo;      // 流光Logo
    private Animator _StartBtnAnim;        // START按鈕動畫
    private Button _StartBtn;

    void Start () {
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

        if (!_17logoSequence)
            Debug.LogError("No found 17logo Sequence");

        StartCoroutine("PlayOP");  //開始
    }

    //遊戲流程
    IEnumerator PlayOP()
    {
        if (EnterLoading.instance)
        {
            EnterLoading.instance.StartLoading(); //讀取下一場景

            //while (!EnterLoading.instance.LoadedDone)
            //    yield return new WaitForSeconds(1f);
        }
 
        foxGameLogoAnim.SetTrigger("FoxGameLogo"); // 01-出現 FoxGame
        yield return new WaitForSeconds(3.3f);

        _17logoSequence.SetActive(true);           // 02-播放17玩麻將 序列動畫       
        yield return new WaitForSeconds(5.2f);

        _17logoSequence.SetActive(false);          //03 - 停止17玩麻將 序列動畫       

        flowLightLogo.SetActive(true);             // 04-流光Logo&開始介面
        _17LogoPanelAnim.SetTrigger("_17LogoPanel");

        //if(_StartBtn)
        //    _StartBtn.enabled = true;

        //EnterLoading.instance._autoToNextScene = true;
    }

    //點擊 START
    public void GoLoginPage() {
        StopCoroutine("PlayOP");

        if (_StartBtn)
            _StartBtn.enabled = false;

        if (!EnterLoading.instance._autoToNextScene)
            EnterLoading.instance._autoToNextScene = true;

        _StartBtnAnim.SetTrigger("PressScreen");
        //_17LogoPanelAnim.SetTrigger("screenFadeOut");

        //StartCoroutine("StartLoading"); //讀取下一場景
        //EnterLoading.instance._autoToNextScene = true;
    }

    IEnumerator StartLoading() {
        yield return new WaitForSeconds(1.5f);
        EnterLoading.instance.StartLoading();
    }
}
