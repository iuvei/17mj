using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpenSceneManager : MonoBehaviour {

    public Transform _17NewTitle;  // 片頭動畫
    public GameObject enterLoadingPanel;   // 讀取畫面

    private Animator _17LogoAnim;
    private Transform _17logoStartPanel;    // 版權分級介面
    private GameObject flowLightLogo;      // 流光Logo
    private Animator _StartBtnAnim;        // START按鈕動畫
    private Button _StartBtn;

	private void Awake()
	{
		MJApi.getServerIP ();
	}


    void Start () {

        if (!_17NewTitle)
            Debug.LogError("No found 17 Title Panel");
        else {
            _17LogoAnim = _17NewTitle.GetComponent<Animator>();
            _17logoStartPanel = _17NewTitle.Find("StartInfoPanel");
            _StartBtnAnim = _17NewTitle.Find("StartInfoPanel/StartButton/ScaleBtnText").gameObject.GetComponent<Animator>();
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
        if (EnterLoading.instance)
        {
            EnterLoading.instance.StartLoading(); //讀取下一場景

            //while (!EnterLoading.instance.LoadedDone)
            //    yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(6f);


        if (_StartBtn)
            _StartBtn.enabled = true;

        EnterLoading.instance._autoToNextScene = true;
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

        //if (EnterLoading.instance) {
        //    EnterLoading.instance._autoToNextScene = true;
        //    EnterLoading.instance.StartLoading(); //讀取下一場景
        //}   
    }

    IEnumerator StartLoading() {
        yield return new WaitForSeconds(1.5f);
        EnterLoading.instance.StartLoading();
    }
}
