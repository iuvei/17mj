using UnityEngine;
using UnityEngine.UI;
using System;

public class getUserData : MonoBehaviour {

    public GameObject PhotoImage;
    public GameObject NameText;
    public GameObject LevelText;
    public GameObject CoinText;
    public GameObject OnlineText;

    private string sPhoto;
    private string sName;
    private string sLevel;
    private string sCoin;
    private string sOnline;
    private bool _readUserInfo = false;
    void Start() {
        sPhoto = CryptoPrefs.GetString("USERPHOTO");
        if (!string.IsNullOrEmpty(sPhoto))
        {
            Texture2D newPhoto = new Texture2D(1, 1);
            newPhoto.LoadImage(Convert.FromBase64String(sPhoto));
            newPhoto.Apply();

            Image ProfilePicture = PhotoImage.GetComponent<Image>();
            ProfilePicture.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
        }

        sName = CryptoPrefs.GetString("USERNAME");
        if (!string.IsNullOrEmpty(sName))
        {
            Text name = NameText.GetComponent<Text>();
            name.text = sName;
        }

        sLevel = CryptoPrefs.GetString("USERLEVEL");
        if (!string.IsNullOrEmpty(sLevel))
        {
            Text level = LevelText.GetComponent<Text>();
            level.text = "Lv " + sLevel;
        }

        sCoin = CryptoPrefs.GetString("USERCOIN");
        if (!string.IsNullOrEmpty(sCoin))
        {
            Text coin = CoinText.GetComponent<Text>();
            coin.text = sCoin;
        }

        sOnline = CryptoPrefs.GetString("USERONLINE");
        if (!string.IsNullOrEmpty(sOnline))
        {
            Text online = OnlineText.GetComponent<Text>();
            online.text = "在線人數 " + sOnline;
        }

        //if (string.IsNullOrEmpty(sPhoto) && string.IsNullOrEmpty(sName) &&
        //    string.IsNullOrEmpty(sLevel) && string.IsNullOrEmpty(sCoin) && string.IsNullOrEmpty(sOnline))
        //    _readUserInfo = true;

    }

    //void Update() {
    //    if (_readUserInfo) {

    //        if (!string.IsNullOrEmpty(sPhoto))
    //        {
    //            Texture2D newPhoto = new Texture2D(1, 1);
    //            newPhoto.LoadImage(Convert.FromBase64String(sPhoto));
    //            newPhoto.Apply();

    //            Image ProfilePicture = PhotoImage.GetComponent<Image>();
    //            ProfilePicture.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
    //        }

    //        if (!string.IsNullOrEmpty(sName))
    //        {
    //            Text name = NameText.GetComponent<Text>();
    //            name.text = sName;
    //        }

    //        if (!string.IsNullOrEmpty(sLevel))
    //        {
    //            Text level = LevelText.GetComponent<Text>();
    //            level.text = "Lv " + sLevel;
    //        }

    //        if (!string.IsNullOrEmpty(sCoin))
    //        {
    //            Text coin = CoinText.GetComponent<Text>();
    //            coin.text = sCoin;
    //        }

    //        if (!string.IsNullOrEmpty(sOnline))
    //        {
    //            Text online = OnlineText.GetComponent<Text>();
    //            online.text = "在線人數 " + sOnline;
    //        }

    //        _readUserInfo = false;
    //    }
    //}
}
