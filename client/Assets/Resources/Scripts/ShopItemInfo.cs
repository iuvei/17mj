using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemInfo : MonoBehaviour {
    public Image _targetImg;
    public Text _targetName;
    public Text _targetPrice;

    [HideInInspector]
    public int Id = 0;
    [HideInInspector]
    public int Path2D = 0;
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public int Price;

    public void setInfo(ShopItemInfo info)
    {
        _targetImg.sprite = Resources.Load<Sprite>("Image/Items/" + string.Format("{0:00}", info.Path2D));
        _targetImg.SetNativeSize();
        _targetName.text = info.Name;
        _targetPrice.text = string.Format("{0:0,0}", info.Price);
    }
}

public class ShopItemInfos
{
    public List<ShopItemInfo> dataList = new List<ShopItemInfo>();
}