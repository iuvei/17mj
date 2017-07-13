using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BombTimer : MonoBehaviour {
    public GameObject _bombLine;
    public GameObject _bombFire;

    void Start () {
        if (_bombFire) {
            _bombFire.transform.DOScale(new Vector3(0.5f, 0.5f ,1), 0.3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            //_bombFire.GetComponent<Image>().DOFade(0.8f, 0.3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        
    }

}
