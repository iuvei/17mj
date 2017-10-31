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
            coin.text = string.Format("{0:0,0}", int.Parse(sCoin));
        }

    }
 
}
