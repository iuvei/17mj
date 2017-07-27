using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardDance : MonoBehaviour {
    public GameObject _dance;
    private Transform[] _cards;
    private List<Transform> cards = new List<Transform>();
    private Sequence sequence;
    
    void Start () {

        if (_dance) {
            sequence = DOTween.Sequence();
            _cards = _dance.GetComponentsInChildren<Transform>();
            foreach (Transform c in _cards)
                cards.Add(c);
            cards.RemoveAt(0);

            Debug.Log(cards.Count);
            for (int i = 0; i < cards.Count; i++)
            {
                //cards[i].DOLocalMoveY(-1.8f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetDelay(i*0.2f);
                Action(cards[i]);
            }


        }
    }

    private void Action(Transform ts) {
        sequence.Append(ts.DOLocalMoveY(-1.8f, 0.1f));
        sequence.Append(ts.DOLocalMoveY(-2.18f, 0.1f).SetDelay(0.1f));
        //sequence.AppendInterval(0f);
        sequence.SetLoops(-1, LoopType.Yoyo);
    }

}
