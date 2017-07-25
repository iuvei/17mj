using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankItemInfo {
    public int Rank;
    public string Photo;
    public string Name;
    public int Lv;
    public int Win;
    public int Lose;
    public float Probability;
}

public class RankItemInfos
{
    public List<RankItemInfo> dataList = new List<RankItemInfo>();
}
