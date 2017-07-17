using UnityEngine;
using UnityEngine.UI;
using System;

public class getUserData : MonoBehaviour {

    public GameObject PhotoImage;
    public GameObject NameText;
    public GameObject LevelText;
    public GameObject CoinText;
    public GameObject OnlineText;

    void Start() {
        string sPhoto = CryptoPrefs.GetString("USERPHOTO");
        if (!string.IsNullOrEmpty(sPhoto)) {
            Texture2D newPhoto = new Texture2D(1, 1);
            newPhoto.LoadImage(Convert.FromBase64String(sPhoto));
            newPhoto.Apply();

            Image ProfilePicture = PhotoImage.GetComponent<Image>();
            ProfilePicture.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
        }

        string sName = CryptoPrefs.GetString("USERNAME");
        if (!string.IsNullOrEmpty(sName))
        {
            Text name = NameText.GetComponent<Text>();
            name.text = sName;
        }

        string sLevel = CryptoPrefs.GetString("USERLEVEL");
        if (!string.IsNullOrEmpty(sLevel))
        {
            Text level = LevelText.GetComponent<Text>();
            level.text = "Lv " + sLevel;
        }

        string sCoin = CryptoPrefs.GetString("USERCOIN");
        if (!string.IsNullOrEmpty(sCoin))
        {
            Text coin = CoinText.GetComponent<Text>();
            coin.text = sCoin;
        }

        string sOnline = CryptoPrefs.GetString("USERONLINE");
        if (!string.IsNullOrEmpty(sOnline))
        {
            Text online = OnlineText.GetComponent<Text>();
            online.text = "在線人數 " + sOnline;
        }
    }

}
