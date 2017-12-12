using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Games : MonoBehaviour {
    public GameObject loadingPanel;
    public GameObject AudioPanel;
    public GameObject[] _gamePanels;

    private string BGM_name = "BGM_Lobby";
    private GameObject AccountManagerPanel;
    private GameObject childSetting;

    void Start()
    {
        if (!AccountManagerPanel)
        {
            AccountManagerPanel = GameObject.FindGameObjectWithTag("AccountManager");
        }

        AudioManager.Instance.PlayBGM(BGM_name);
    }

    public void LoadLobbyScene() {
        if (loadingPanel)
        {
            loadingPanel.SetActive(true);
            EnterLoading.instance.StartLoading();
        }
    }

    public void ClickRank()
    {
        AccountManager.Instance.ShowConnecting(); //開啟連線視窗

        //if (btmMenuBtns[0])
        //    btmMenuBtns[0].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

        //SetPlayerCoins();

        //if (rankPanel)
        //{
        //    rankPanel.transform.DOMoveX(-19.5f, 0, true);
        //    rankPanel.transform.DOMoveX(0, 0.5f, true).SetEase(Ease.OutCubic);
        //}
    }

    public void ShowLogoutPopup()
    {
        //childSetting = settingPanelNew.transform.Find("Setting").gameObject;
        //GameObject popupBG = childSetting.transform.Find("Profile/popupBG").gameObject;

        //if (popupLogout)
        //{
        //    if (popupBG)
        //    {
        //        popupBG.SetActive(true);
        //        popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
        //    }
        //    popupLogout.transform.DOScale(Vector3.zero, 0);
        //    popupLogout.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        //}
    }

    public void ExitLogoutPopup()
    {
        GameObject popupBG = childSetting.transform.Find("Profile/popupBG").gameObject;

        //if (popupLogout)
        //{
        //    popupLogout.transform.DOScale(Vector3.one, 0);
        //    popupLogout.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine);
        //    if (popupBG)
        //    {
        //        popupBG.GetComponent<Image>().DOFade(0, 0.3f);
        //        StartCoroutine(HideGameObject(popupBG, 0.3f));
        //    }
        //}
    }
}
