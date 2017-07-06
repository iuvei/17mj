using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class Shop : MonoBehaviour {
    public GameObject popupBackground;
    public RectTransform popupBuy;
    public RectTransform popupSad;

    private Text _popupBuyItemName;
    private Text _popupBuyItemPrice;
    private InputField _popupBuyItemNum;
    private Text _popupBuyItemTotal;

    private int currentNum = 1;
    private int currentPrice;

    void Start() {
        if (popupBuy)
        {
            _popupBuyItemName = popupBuy.Find("content/Item/name").GetComponent<Text>();
            _popupBuyItemPrice = popupBuy.Find("content/Price/price").GetComponent<Text>();
            _popupBuyItemNum = popupBuy.Find("content/Num/num").GetComponent<InputField>();
            _popupBuyItemTotal = popupBuy.Find("content/Total/total").GetComponent<Text>();
        }
    }

    public void ClickShopBuy()
    {
        
        ShopItemInfo _info = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.GetComponent<ShopItemInfo>();
        Debug.Log("name = " + _info.ItemName + "  price = " + _info.ItemPrice);
        currentNum = 1;
        currentPrice = _info.ItemPrice;
        _popupBuyItemName.text = _info.ItemName;
        _popupBuyItemPrice.text = String.Format("{0:0,0}", currentPrice);
        _popupBuyItemNum.text = currentNum.ToString();
        _popupBuyItemTotal.text = String.Format("{0:0,0}", currentPrice);

        if (popupBuy) {
            popupBackground.SetActive(true);
            popupBackground.GetComponent<Image>().DOFade(0.6f, 0.3f);
            popupBuy.transform.DOScale(Vector3.zero, 0);
            popupBuy.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void ExitShopBuy()
    {
        if (popupBuy)
        {
            popupBuy.transform.DOScale(Vector3.one, 0);
            popupBuy.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine);
            popupBackground.GetComponent<Image>().DOFade(0, 0.3f);
            StartCoroutine(HideGameObject(popupBackground, 0.3f));
        }
    }

    public void ClickAdd()
    {
        if (currentNum < 99) {
            currentNum++;
            _popupBuyItemNum.text = currentNum.ToString();
            Caculate();
        }
    }

    public void ClickSubtract()
    {
        if (currentNum > 1) {
            currentNum--;
            _popupBuyItemNum.text = currentNum.ToString();
            Caculate();
        }
    }

    public void GoSaid()
    {
        popupSad.transform.DOScale(Vector3.zero, 0);
        popupSad.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
    }

    public void ExitSad()
    {
        popupSad.transform.DOScale(Vector3.one, 0);
        popupSad.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine);
    }

    IEnumerator HideGameObject(GameObject go, float _time)
    {
        if (_time > 0)
            yield return new WaitForSeconds(_time);

        if (go)
            go.SetActive(false);
    }

    private void Caculate() {
        _popupBuyItemTotal.text = string.Format("{0:0,0}", (currentNum * currentPrice));
    }
    public void ChangeCurrentNum() {
        if (!Int32.TryParse(_popupBuyItemNum.text, out currentNum))
        {
            _popupBuyItemNum.text = "1";
            currentNum = 1;
        }
        else
            currentNum = Int32.Parse(_popupBuyItemNum.text);


        if (currentNum < 1) {
            _popupBuyItemNum.text ="1";
            currentNum = 1;
        }
           
        if (currentNum > 99) {
            currentNum = 99;
        }

        Caculate();
    }
}
