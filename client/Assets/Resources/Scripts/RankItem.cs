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
        _photoImg.sprite = Resources.Load<Sprite>("Image/Rank/temp/" + Random.Range(1, 8));
        //_photoImg.sprite =Sprite.Create(info.Photo, new Rect(0, 0, info.Photo.width, info.Photo.height), Vector2.zero);
        //_photoImg.SetNativeSize();

        _nameText.text = info.Name;
        _lvText.text = "Lv " + info.Lv.ToString();
        _winText.text = string.Format("勝：{0:0,0}", info.Win);
        _loseText.text = string.Format("敗：{0:0,0}", info.Lose);
        _probText.text = string.Format("勝率：{0:00.00}%", info.Probability);

        //----------------FB的圖片解碼 範例-------------------------
        //if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        //{
        //    string stringData = Convert.ToBase64String(result.Texture.EncodeToPNG());
        //    FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPhotoCallback);
        //    Image ProfilePicture;
        //    ProfilePicture = PhotoImage.GetComponent<Image>();
        //    // ProfilePicture.sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), Vector2.zero);

        //    Texture2D newPhoto = new Texture2D(1, 1);
        //    newPhoto.LoadImage(Convert.FromBase64String(uPhoto));
        //    newPhoto.Apply();
        //}
        //--------------------------------------------
    }
}
