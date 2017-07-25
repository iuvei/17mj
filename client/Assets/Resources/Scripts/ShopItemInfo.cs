using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemInfo {
    [HideInInspector]
    public int Id = 0;
    [HideInInspector]
    public int Path2D = 0;
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public int Price;
}

public class ShopItemInfos
{
    public List<ShopItemInfo> dataList = new List<ShopItemInfo>();
}