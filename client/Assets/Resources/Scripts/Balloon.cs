using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Balloon : MonoBehaviour {
    public Image _balloonImg;
    public GameObject _particle;
    private Sprite[] _sprite;

    void Start() {
        _sprite = Resources.LoadAll<Sprite>("Image/balloon");
        _balloonImg.sprite = _sprite[Random.Range(0, 5)];
    }


    public void ClickBalloon() {
        transform.DOScale(new Vector3(1.3f, 1.3f, 1), 0.2f).SetEase(Ease.OutElastic);
        _balloonImg.DOFade(0, 0.1f).SetEase(Ease.InSine);
        _particle.SetActive(true);
    }

    public void RandomBalloon() {

        
    }
}
