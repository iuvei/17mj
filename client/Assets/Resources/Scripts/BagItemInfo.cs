using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItemInfo : MonoBehaviour {
    public Image bagItemImg;
    public Text bagItemName;
    public Text bagItemNum;

    public void setInfo(ItemInfo info)
    {
        bagItemImg.sprite = Resources.Load<Sprite>("Image/Items/" + string.Format("{0:00}",info.Path2D));
        bagItemImg.SetNativeSize();
        bagItemName.text = info.Name;
        bagItemNum.text = info.Num.ToString();
    }

}
