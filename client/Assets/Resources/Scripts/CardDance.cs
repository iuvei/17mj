using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardDance : MonoBehaviour {
   
    public enum CardDirection { Horizontal, VerticalLeft, VerticalRight };
    public enum DanceType { Default, Swing };

    [Tooltip("牌組方向")]
    public CardDirection cardDirection;

    [Tooltip("動畫類型")]
    public DanceType danceType;

    [Tooltip("反向播放")]
    public bool _PlayBack = false;

    [Tooltip("循環類型")]
    public bool _LoopYoyo = false;

    public float _delayStartTime = 0;
    public float _delayLoopTime = 0;

    [Range(0.05f, 0.2f), Tooltip("黏著程度")]
    public float _stick = 0.15f;

    private Transform[] _cards;
    private List<Transform> cards = new List<Transform>();
    private Sequence sequence;
    private int _sign = 1;

    void Start () {
        _sign = (cardDirection ==  CardDirection.VerticalRight) ? -1 : 1;

        sequence = DOTween.Sequence();

        _cards = gameObject.GetComponentsInChildren<Transform>();

        sequence.AppendInterval(_delayStartTime);

        foreach (Transform c in _cards)
                cards.Add(c);
            cards.RemoveAt(0);

            for (int i = 0; i < cards.Count; i++)
            {
                if(_PlayBack)
                    Action(cards[cards.Count - 1 - i], i* _stick);
                else
                    Action(cards[i], i * _stick);
        }
            sequence.AppendInterval(_delayLoopTime);

    }

    private void Action(Transform ts, float _t)
    {
        switch (cardDirection) {
            case CardDirection.Horizontal:
                switch (danceType)
                {
                    case DanceType.Default:
                    case DanceType.Swing:
                        sequence.Insert(_t, ts.DOLocalMoveY(-1.8f, 0.2f)).SetEase(Ease.Linear);
                        sequence.Insert(_t, ts.DOLocalMoveY(-2.18f, 0.2f).SetEase(Ease.Linear).SetDelay(0.2f));
                        break;
                }
                break;
            case CardDirection.VerticalLeft:
            case CardDirection.VerticalRight:
                switch (danceType) {
                    case DanceType.Default:
                        sequence.Insert(_t, ts.DOBlendableMoveBy(new Vector3(-0.142f * _sign, 0.185f, 0),0.2f)).SetEase(Ease.Linear);
                        sequence.Insert(_t, ts.DOMove(ts.position, 0.2f).SetEase(Ease.Linear).SetDelay(0.2f));
                        break;
                    case DanceType.Swing:
                        sequence.Insert(_t, ts.DOBlendableMoveBy(new Vector3(-0.242f * _sign, 0, 0), 0.2f)).SetEase(Ease.Linear);
                        sequence.Insert(_t, ts.DOMove(ts.position, 0.2f).SetEase(Ease.Linear).SetDelay(0.2f));
                        sequence.Insert(_t, ts.DOBlendableMoveBy(new Vector3(0.242f * _sign, 0, 0), 0.2f).SetEase(Ease.Linear).SetDelay(0.4f));
                        sequence.Insert(_t, ts.DOMove(ts.position, 0.2f).SetEase(Ease.Linear).SetDelay(0.6f));
                        break;
                }
                break;
        }

        sequence.SetLoops(-1, _LoopYoyo? LoopType.Yoyo : LoopType.Restart);
        
    }
}
