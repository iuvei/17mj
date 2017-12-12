using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TetrisScore : MonoBehaviour {
    public Text _score;
    public Text _scoreH;
    public Text _clear;
    public GameObject _over;
    public static TetrisScore Instance;
    private int _highS;
    private int _currS = 0;
    private Color _clearColor;
    private Vector3 _clearPos;

    void Awake() {
        Instance = this;

        string hs = PlayerPrefs.GetString("HIGHSCORE");
        _highS = (string.IsNullOrEmpty(hs)) ? 0 : int.Parse(hs);
        _scoreH.text = "" + _highS;
        _score.text = "0";
        _clearColor = _clear.color;
        _clearPos = _clear.transform.position;
    }

    public void WriteScore(int s) {
        _currS += s;
        _score.text = "" + _currS;
    }

    public void ShowGameOver(bool isOn) {
        _over.SetActive(isOn);

        if(!isOn)
            ResetScore();
    }

    public void ShowExcellent() {
        if (_clear) {
            _clear.transform.position = _clearPos;
            _clear.color = _clearColor;
            _clear.text = "";
            _clear.DOText("EXCELLENT!", 0.6f, false);//.SetEase(Ease.InCirc);
            _clear.transform.DOBlendableMoveBy(new Vector3(0, 0.2f, 0), 0.5f).SetEase(Ease.Linear).SetDelay(0.6f);
            _clear.DOFade(0, 0.5f).SetDelay(0.6f);
            WriteScore(300);
        }
    }


    private void ResetScore() {
        CheckScore();
        _currS = 0;
        _score.text = "0"; 
    }

    private void CheckScore() {
        if (_currS > _highS) {
            _scoreH.text = "" + _currS;
            PlayerPrefs.SetString("HIGHSCORE", _scoreH.text);
        }
    }


}
