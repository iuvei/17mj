using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MiniGames : MonoBehaviour {
    public static MiniGames Instance;
    public GameObject loadingPanel;
    public GameObject AudioPanel;
    public GameObject[] _game2DPanels;
    public GameObject[] _game3DObjects;
    private string BGM_name = "BGM_Tetris";
    private GameObject AccountManagerPanel;
    private GameObject childSetting;
    private AudioClip[] audioClips;
    private int GameType = 0;
    private int GameTotalNum = 2; //遊戲種類

    void Start()
    {
        Instance = this;

        if (!AccountManagerPanel)
        {
            AccountManagerPanel = GameObject.FindGameObjectWithTag("AccountManager");
        }

        GameType = AccountManager.Instance.GameType;
        SetGameType(GameType);
    }

    private void SetGameType(int _type)
    {

        SetAllPanel(_type);
        switch (_type)
        {
            case 1: //俄羅斯方塊
            default:
                SetSounds();
                //_game2DPanels[0].SetActive(true);
                //_game3DObjects[0].SetActive(true);
                break;
            case 2://水果飛鏢
                //_game2DPanels[1].SetActive(true);
                //_game3DObjects[1].SetActive(true);
                break;
        }
    }

    private void SetAllPanel(int _type) {
        for (int i = 0; i < GameTotalNum; i++)
        {
            _game2DPanels[i].SetActive(i == (_type-1));
            _game3DObjects[i].SetActive(i == (_type-1));
        }
    }

    private void SetSounds()
    {
        PlayBGM(BGM_name);

        string SE_Path = "Sounds/SE/Tetris/";
        audioClips = Resources.LoadAll<AudioClip>(SE_Path);

        audioClips = new AudioClip[4];
        audioClips[0] = Resources.Load<AudioClip>(SE_Path + "Move");
        audioClips[1] = Resources.Load<AudioClip>(SE_Path + "Fall");
        audioClips[2] = Resources.Load<AudioClip>(SE_Path + "Del");
        audioClips[3] = Resources.Load<AudioClip>(SE_Path + "Over");
    }

    private void PlayBGM(string _name)
    {
        AudioClip ac = Resources.Load<AudioClip>("Sounds/BGM/" + _name);
        SoundEffect.Instance.PlayLoop(ac);
    }

    public void PlayMoveSound()
    {
        SoundEffect.Instance.PlayOnce(audioClips[0]);
    }

    public void PlayFallSound()
    {
        SoundEffect.Instance.PlayOnce(audioClips[1]);
    }

    public void PlayDelSound()
    {
        SoundEffect.Instance.PlayOnce(audioClips[2]);
    }

    public void PlayOverSound()
    {
        SoundEffect.Instance.PlayOnce(audioClips[3]);
    }


    public void LoadLobbyScene() {
        if (loadingPanel)
        {
            AccountManager.Instance.GameType = 0;
            loadingPanel.SetActive(true);
            EnterLoading.instance.StartLoading();

            //if (AudioPanel)
            //    Destroy(AudioPanel);
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
