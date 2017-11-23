
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour {
    public Image _rankImg;
    public Text _rankText;
    public Image _photoImg;
    public Text _nameText;
    public Text _lvText;
    public Text _winText;
    public Text _loseText;
    public Text _probText;

    public void setInfo(RankItemInfo info)
    {
        int rankBadge;

        switch (info.Rank) {
            case 1:
            case 2:
            case 3:
                rankBadge = info.Rank;
                break;
            default:
                _rankText.text = info.Rank.ToString();
                rankBadge = 0;
                break;
        }

        _rankImg.sprite = Resources.Load<Sprite>("Image/Rank/" + string.Format("{0:00}", rankBadge));
        //_photoImg.sprite = Resources.Load<Sprite>("Image/Rank/temp/" + Random.Range(1, 8));
        SetPhotos(info.Photo);
        _nameText.text = info.Name;
        _lvText.text = "Lv " + info.Lv.ToString();
        _winText.text = string.Format("勝：{0:#,0}", info.Win);
        _loseText.text = string.Format("敗：{0:#,0}", info.Lose);
        _probText.text = string.Format("    勝率：{0:0.##}%", info.Probability);
    }

    private void SetPhotos(string _photoType)
    {
        if (_photoType == "0") //名次三名以外 圖片隨機
        {
            //_photoImg.sprite = Resources.Load<Sprite>("Image/Rank/temp/" + Random.Range(1, 8));
            _photoImg.sprite = Resources.Load<Sprite>("Image/defaultUserP");
        }
        else 
        {
            string sPhoto;
            if (_photoType == "1") // 玩家自身
                sPhoto = CryptoPrefs.GetString("USERPHOTO");
            else
                sPhoto = _photoType;  // 前三名

            if (!string.IsNullOrEmpty(sPhoto))
            {
                Texture2D newPhoto = new Texture2D(1, 1);
                newPhoto.LoadImage(System.Convert.FromBase64String(sPhoto));
                newPhoto.Apply();

                _photoImg.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
            }
        }
    }
}
