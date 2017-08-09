using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DailyBonus : MonoBehaviour {
    public Image _contentImg;
    public Image _takeImg;
    public Image _takeMask;
    public Text _dateText;


    //已領取獎勵
    public void AlreadyTake() {
        _takeImg.DOFade(1, 0);
        ShowTakeMask();
    }

    //準備領取
    public void ReadyTake() {
        _takeImg.DOFade(0, 0);

        Sequence tdSeq = DOTween.Sequence();
        tdSeq.PrependInterval(1f);
        tdSeq.Append(_takeImg.transform.DOScale(new Vector3(1.3f, 1.3f, 1), 0));
        tdSeq.Append(_takeImg.DOFade(1, 0.3f));
        tdSeq.Append(_takeImg.transform.DOScale(new Vector3(0.93f, 0.93f, 1), 0.5f).SetEase(Ease.InCirc).SetDelay(0.3f));
        tdSeq.AppendInterval(0.2f);
        tdSeq.Append(_takeMask.DOFade(0.9f, 0.3f).SetLoops(2, LoopType.Yoyo));
        tdSeq.OnComplete(ShowTakeMask).SetUpdate(true);
    }

    //尚未領取
    public void NotTake(int _date) {
        _takeImg.DOFade(0, 0);
        Sprite _sp  = Resources.Load<Sprite>("Image/DailyBonus/" + _date);
        if (_sp) {
            _contentImg.sprite = _sp;
        }else
            _contentImg.sprite = Resources.Load<Sprite>("Image/DailyBonus/gift");
    }

    public void ShowDate(int _date) {
        _dateText.text = "第" + _date + "天";
    }

    private void ShowTakeMask() {
        _takeMask.sprite = Resources.Load<Sprite>("Image/DailyBonus/def1");
        _takeMask.DOFade(1, 0);
    }

}
