using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DailyBonus : MonoBehaviour {
    public Image _mainImg;
    public Image _takeImg;
    public Text _dateText;

    //已領取獎勵
    public void AlreadyTake() {
        _takeImg.DOFade(1, 0);
        ChangeMainImg();
    }

    //準備領取
    public void ReadyTake() {
        _takeImg.DOFade(0, 0);

        Sequence tdSeq = DOTween.Sequence();
        tdSeq.PrependInterval(1);
        tdSeq.Append(_takeImg.transform.DOScale(new Vector3(1.3f, 1.3f, 1), 0));
        tdSeq.Append(_takeImg.DOFade(1, 0.3f));
        tdSeq.Append(_takeImg.transform.DOScale(new Vector3(0.93f, 0.93f, 1), 0.5f).SetEase(Ease.InCirc).SetDelay(0.3f));
        tdSeq.AppendInterval(0.05f);
        tdSeq.OnComplete(ChangeMainImg);
    }

    //尚未領取
    public void NotTake(int _date) {
        _takeImg.DOFade(0, 0);
        Sprite _sp  = Resources.Load<Sprite>("Image/DailyBonus/" + _date);
        if (_sp) {
            _mainImg.sprite = _sp;
        }else
            _mainImg.sprite = Resources.Load<Sprite>("Image/DailyBonus/def1");
    }

    public void ShowDate(int _date) {
        _dateText.text = "第" + _date + "天";
    }

    private void ChangeMainImg() {
        _mainImg.sprite = Resources.Load<Sprite>("Image/DailyBonus/def0");
    }
}
