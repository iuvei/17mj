using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mah jong tools.
/// 台灣一般玩麻將為十六張麻將
/// 3張為一堆 可以是刻(三張一樣)或是順(三張為順) 最後兩張為雀 必須兩張一樣
/// 當符合以上牌姿就是胡牌 當缺一張胡牌稱為聽牌
/// 
/// 第一步 先拿17張判斷胡牌
/// 首先當然牌必須先排序
/// 排序後能夠把相同的牌放在一起
/// 並且按照順序由小到大
/// 這樣要判斷順或刻就容易許多
/// 基本上再打麻將的時候也是這樣
/// 
/// 我們先把已經湊成順或刻的丟到一邊
/// 等全部分類完 看剩下最後的兩張是不是一樣
/// 一樣就是胡牌了
/// 然而分類我是使用遞迴去寫的 這樣會快速很多
/// 遞迴可以保留程式狀態 當發現錯誤可以回朔到之前狀態
/// 
/// 胡牌寫完後
/// 要判斷聽牌就不困難啦
/// 把每一種牌丟進去配合判斷是否胡牌就好了
/// </summary>
namespace com.Desktop
{
	/// <summary>
	/// Mah jong tools.
	/// </summary>
    public class MahJongTools
    {
		public static bool IsCanDiscard(List<int> mah, int ID)
		{
			bool result = true;
			List<int> pais = new List<int>(mah);
			pais.Add(ID);

			List<int> currentMahes = pais.FindAll(delegate (int a)
			{
				return a == ID;
			});
			//Debug.Log ("currentMahes.Count="+currentMahes.Count);
			if (currentMahes.Count >= 2)
			{
				result = false;
			}
			return result;
		}
		/// <summary>
		/// 判斷是否胡牌
		/// </summary>
		/// <returns><c>true</c> if is can H the specified mah ID; otherwise, <c>false</c>.</returns>
		/// <param name="mah">Mah.</param>
		/// <param name="ID">I.</param>
		public static bool IsCanHU(List<int> mah, int ID, bool ismopai = false)
        {
			bool result = false;

            List<int> pais = new List<int>(mah);

			//if (!ismopai) {
			pais.Add (ID);
			//}
            //只剩兩張牌 且兩張牌一樣
            if (pais.Count == 2)
            {
				result = (pais[0] == pais[1]);
				//Debug.Log ("IsCanHU !!!!!!!!!!!!!!!!!!!!! ("+result+")");
				return result;
            }

            //先排序
            pais.Sort();

			string tmp = string.Empty;
			int a = 0;
			foreach (int x in pais) {
				tmp += x.ToString ();
				if (a < pais.Count-1)
					tmp += ",";
				a++;
			}
			//Debug.LogError ("* IsCanHU() count="+pais.Count+" pais=["+tmp+"]");

			//Debug.LogError ("************* IsCanHU() pais.count="+pais.Count);

            //依据牌的顺序从左到右依次分出将牌
            for (int i = 0; i < pais.Count; i++)
            {
                List<int> paiT = new List<int>(pais);
                List<int> ds = pais.FindAll(delegate (int d)
                {
					//Debug.Log("d="+d);
					result = (pais[i] == d);
					//Debug.Log ("xxxxxx aaaaa aresult="+result+" ");
					return result;
                });

				string zz = string.Empty;
				foreach (int x in ds) {
					zz += x.ToString () + ",";
				}
				//Debug.Log ("ds=["+zz+"]");

                //判断是否能做将牌
                if (ds.Count >= 2)
                {
                    //移除两张将牌
                    paiT.Remove(pais[i]);
                    paiT.Remove(pais[i]);
                    i++;

                    if (HuPaiPanDin(paiT))
                    {
						result = true;
						//Debug.Log ("IsCanHU()  final!!!!!("+result+")");
						return result;
						//break;
                    }
                }
            }
			result = false;
			//Debug.Log ("[c] IsCanHU fail.....("+result+")");
			return result;
        }
		/*
		/// <summary>
		/// 判斷是否聽牌
		/// </summary>
		/// <returns><c>true</c> if is can tin the specified mah ID; otherwise, <c>false</c>.</returns>
		/// <param name="mah">Mah.</param>
		/// <param name="ID">I.</param>
		public static bool IsCanTin(List<int> mah, int ID)
		{
			bool result = false;

			List<int> pais = new List<int>(mah);

			pais.Add(ID);
			//只有两张牌
			if (pais.Count == 1)
			{
				result = true;
				Debug.Log ("IsCanTin("+result+")");
				return result;
			}

			//先排序
			pais.Sort();

			//依据牌的顺序从左到右依次分出将牌
			for (int i = 0; i < pais.Count; i++)
			{
				List<int> paiT = new List<int>(pais);
				List<int> ds = pais.FindAll(delegate (int d)
				{
					result = (pais[i] == d);
					//Debug.Log ("IsCanHU("+result+")");
					return result;
					//if(result)
					//	break;
				});

				//判断是否能做将牌
				if (ds.Count >= 2)
				{
					//移除两张将牌
					paiT.Remove(pais[i]);
					paiT.Remove(pais[i]);
					i++;

					if (HuPaiPanDin(paiT))
					{
						result = true;
						Debug.Log ("IsCanHU   ("+result+")");
						//return result;
						break;
					}
				}
			}
			result = false;
			//Debug.Log ("[c] IsCanHU("+result+")");
			return result;
		}
		*/

		/// <summary>
		/// Hu the pai pan din.
		/// </summary>
		/// <returns><c>true</c>, if pai pan din was hued, <c>false</c> otherwise.</returns>
		/// <param name="mahs">Mahs.</param>
        private static bool HuPaiPanDin(List<int> mahs)
        {//遞回去找 順子或刻子 如果最後數量為0就表示胡牌
			//Debug.Log ("!!!!!!!!!!! 遞回胡牌判定 HuPaiPanDin()");
			string zz = string.Empty;
			int i = 0;
			foreach (int x in mahs) {
				zz += x.ToString ();
				if (i < mahs.Count-1)
					zz += ",";
				i++;
			}
			//Debug.Log ("mahs=["+zz+"] Count="+mahs.Count);
            if (mahs.Count == 0)
            {
				//Debug.Log ("最後回傳數量為0就表示胡牌");
                return true;
            }

            List<int> fs = mahs.FindAll(delegate (int a)
            {
				//Debug.Log("xxxx a="+a+" mah[0]="+mahs[0]);
                return mahs[0] == a;
            });
			//Debug.Log ("gggg fs.count="+fs.Count);
			string tmp = string.Empty;
			i = 0;
			foreach (int x in fs) {
				tmp += x.ToString ();
				if (i < fs.Count-1)
					tmp += ",";
				i++;
			}
			//Debug.Log ("fs=["+tmp+"] count="+fs.Count);

            //组成克子
            if (fs.Count == 3)
            {
				//Debug.Log ("找到一組刻子 ("+mahs[0]+","+mahs[0]+","+mahs[0]+")");
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);

                return HuPaiPanDin(mahs);
            }
            else
            { //组成顺子
				//Debug.Log ("fs.count!=3 mahs[0]="+mahs[0]);
				if(! Mahjong.Suit.Contains(mahs[0])) {//不是萬條筒, 不能組成順子
					//Debug.LogError(mahs[0]+"不是萬條筒, 不能組成順子!!!!");
					return false;
				}
                if (mahs.Contains(mahs[0] + 1) && mahs.Contains(mahs[0] + 2))
                {
					//Debug.Log ("找到一組順子 ("+mahs[0]+","+(mahs[0]+1)+","+(mahs[0]+2)+")");
                    mahs.Remove(mahs[0] + 2);
                    mahs.Remove(mahs[0] + 1);
                    mahs.Remove(mahs[0]);

                    return HuPaiPanDin(mahs);
                }
                return false;
            }
        }

        /// <summary>
        /// 判断是否能碰牌的ID为MahID的牌
        /// </summary>
        /// <param name="mahs">手牌</param>
        /// <param name="MahID">被打出来的牌</param>
        /// <param name="state">当前玩家状态</param>
        /// <returns>能不能碰牌</returns>
		public static bool IsCanPon(List<int> mahs, int MahID, bool ismopai = false)
        {
			//Debug.Log ("IsCanPon() mahs.Count="+mahs.Count);
			string mm = string.Empty;
			int i = 0;
			foreach (int ma in mahs) {
				//Debug.Log (ma);
				mm += ma.ToString();
				if(i<mahs.Count-1)
					mm += ",";
				i++;
			}
			//Debug.LogError ("* IsCanPon() count="+mahs.Count+" pais=["+mm+"]");
            bool isCanPon = false;

            List<int> currentMahes = mahs.FindAll(delegate (int a)
            {
                return a == MahID;
            });
			//Debug.Log ("IsCanPon() currentMahes.Count="+currentMahes.Count);
			/*
			if (ismopai && currentMahes.Count == 3)
            {//自己摸牌
				//Debug.Log ();
				string mm = string.Empty;
				foreach (int ma in mahs) {
					//Debug.Log (ma);
					mm += ma.ToString() +" ";
				}
				Debug.Log (mm+" total:"+mahs.Count);

                isCanPon = true;
            }
            */

			if (!ismopai && currentMahes.Count == 2)
			{//上家的牌
				//Debug.Log ("mm=["+mm+"] Count="+mahs.Count);
				isCanPon = true;
			}

			//Debug.Log ("IsCanPon("+isCanPon+")");
            return isCanPon;
        }

        /// <summary>
        /// 判断是否能杠ID为MahID的牌
        /// </summary>
        /// <param name="mahs">手牌</param>
        /// <param name="MahID">被打出来的牌</param>
        /// <param name="state">当前玩家状态</param>
        /// <returns>能不能杠牌</returns>
		public static bool IsCanGan(List<int> mahs, int MahID, bool ismopai = false, List<int> ponmahs = null)
        {
			string mm = string.Empty;
			int i = 0;
			foreach (int ma in mahs) {
				//Debug.Log (ma);
				mm += ma.ToString();
				if(i<mahs.Count-1)
					mm += ",";
				i++;
			}
			//Debug.Log ("IsCanGang() mahs.Count="+mahs.Count);
			//Debug.LogError ("* IsCanGang() count="+mahs.Count+" pais=["+mm+"]");
            bool isCanGang = false;

            List<int> currentMahes = mahs.FindAll(delegate (int a)
            {
                return a == MahID;
            });
			
			if (!ismopai && currentMahes.Count == 3)
			{
				//Debug.Log ("mm=["+mm+"] Count="+mahs.Count);
				isCanGang = true;
            }
            
			//Debug.Log ("ismopai="+ismopai+", ponmahs.Count="+ponmahs.Count);
			if (!ismopai && ponmahs != null) {
				List<int> xxxMahes = ponmahs.FindAll(delegate (int b)
				{
					return b == MahID;
				});
				//Debug.Log ("IsCanGan() xxxMahes.Count="+xxxMahes.Count);
				if (xxxMahes.Count == 3)
				{
					isCanGang = true;
				}
			}
			//Debug.Log ("IsCanGang("+isCanGang+")");
			return isCanGang;
        }

		/// <summary>
		/// 檢查可不可以吃牌
		/// </summary>
		/// <returns><c>true</c> if is can chi the specified mahs MahID chitype ismopai; otherwise, <c>false</c>.</returns>
		/// <param name="mahs">Mahs.</param>
		/// <param name="MahID">Mah I.</param>
		/// <param name="chitype">Chitype.</param>
		/// <param name="ismopai">If set to <c>true</c> ismopai.</param>
		public static bool IsCanChi(List<int> mahs, int MahID, out int chitype, bool ismopai = false)
		{
			string mm = string.Empty;
			int i = 0;
			foreach (int ma in mahs) {
				//Debug.Log (ma);
				mm += ma.ToString();
				if(i<mahs.Count-1)
					mm += ",";
				i++;
			}
			//Debug.LogError ("* IsCanChi() count="+mahs.Count+" pais=["+mm+"]");
			bool iscanchi = false;
			chitype = 0;

			if (ismopai) {
				return iscanchi;
			}
			if(! Mahjong.Suit.Contains(MahID)) {//不是萬條筒, 不能吃
				return iscanchi;
			}
			if (mahs.Contains (MahID + 1) && mahs.Contains (MahID + 2)) {
				iscanchi = true;
				chitype = 1;
			}
			if (mahs.Contains (MahID - 1) && mahs.Contains (MahID + 1)) {
				iscanchi = true;
				chitype = 2;
			}
			if (mahs.Contains (MahID - 2) && mahs.Contains (MahID - 1)) {
				iscanchi = true;
				chitype = 3;
			}
			//Debug.Log ("mm=["+mm+"] Count="+mahs.Count);
			//Debug.Log (mm+" total:"+mahs.Count);
			//Debug.Log ("[c] IsCanChi("+iscanchi+")");
			return iscanchi;
		}

    }
}