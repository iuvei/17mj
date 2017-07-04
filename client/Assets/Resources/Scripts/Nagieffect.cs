using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nagieffect : MonoBehaviour {
    public GameObject _chi;     //吃
    public GameObject _pon;     //碰
    public GameObject _gan;     //槓
    public GameObject _tin;     //聽
    public GameObject _pau;     //放槍
    public GameObject _huSmall; //胡(對家)
    public GameObject _hu;    //胡
    public GameObject _tsumo; //自摸
    public GameObject nagiEffect;
    public GameObject winEffect;

    public enum NagiType {CHI,PON,GAN,TIN,HU,HU2,PAU,TSUMO}

    private GameObject _ob;
    private GameObject _mainOb;

    void Start () {
        if ( !_chi || !_pon || !_gan || !_tin || !_pau)
            Debug.Log("No found correspond effect gameobject.");

        HideAll();
    }

    public void ShowNagi(NagiType _type) {
        StopAllCoroutines();
        HideAll();
        _mainOb = nagiEffect;

        switch (_type) {
            case NagiType.CHI:
                _ob = _chi;
                break;
            case NagiType.PON:
                _ob = _pon;
                break;
            case NagiType.GAN:
                _ob = _gan;
                break;
            case NagiType.TIN:
                _ob = _tin;
                break;
            case NagiType.PAU:
                _ob = _pau;
                break;
            case NagiType.HU:
                _ob = _huSmall;
                break;
            case NagiType.HU2:
                _ob = _hu;
                _mainOb = winEffect;
                break;
            case NagiType.TSUMO:
                _ob = _tsumo;
                _mainOb = winEffect;
                break;
        }
        _ob.SetActive(true);
        _mainOb.SetActive(true);
        StartCoroutine(HideNagi(_ob));
    }


    IEnumerator HideNagi(GameObject _ob)
    {
        if (_ob == _pau || _ob == _tsumo || _ob == _hu)
            yield return new WaitForSeconds(5f);
        else
            yield return new WaitForSeconds(2.3f);

        _ob.SetActive(false);
        winEffect.SetActive(false);
        nagiEffect.SetActive(false);
    }

    private void HideAll() {
        winEffect.SetActive(false);
        nagiEffect.SetActive(false);
        _chi.SetActive(false);
        _pon.SetActive(false);
        _gan.SetActive(false);
        _tin.SetActive(false);
        _hu.SetActive(false);
        _pau.SetActive(false);
        _tsumo.SetActive(false);
    }
}
