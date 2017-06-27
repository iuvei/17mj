using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nagieffect : MonoBehaviour {
    public GameObject _chi;  //吃
    public GameObject _pon;  //碰
    public GameObject _gan;  //槓
    public GameObject _tin;  //聽
    public GameObject _hu;   //胡
    public GameObject _pau;  //放槍

    public enum NagiType {CHI,PON,GAN,TIN,HU,PAU}
    private GameObject _ob;

    void Start () {

        if (!_chi || !_pon || !_gan || !_tin || !_hu || !_pau)
            Debug.Log("No found correspond effect gameobject.");

        gameObject.SetActive(false);
	}

    public void ShowNagi(NagiType _type) {
        StopAllCoroutines();
        HideAll();
        gameObject.SetActive(false);

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
            case NagiType.HU:
                _ob = _hu;
                break;
            case NagiType.PAU:
                _ob = _pau;
                break;
        }
        _ob.SetActive(true);
        gameObject.SetActive(true);
        StartCoroutine(HideNagi(_ob));
    }

    IEnumerator HideNagi(GameObject _ob) {
        yield return new WaitForSeconds(2.3f);
        _ob.SetActive(false);
        gameObject.SetActive(false);
    }

    private void HideAll() {
        _chi.SetActive(false);
        _pon.SetActive(false);
        _gan.SetActive(false);
        _tin.SetActive(false);
        _hu.SetActive(false);
        _pau.SetActive(false);
    }
}
