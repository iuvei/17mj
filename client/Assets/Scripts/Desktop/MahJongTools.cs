using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Desktop
{
    public class MahJongTools
    {

        public static bool IsCanHU(List<int> mah, int ID)
        {
			bool result = false;

            List<int> pais = new List<int>(mah);

            pais.Add(ID);
            //只有两张牌
            if (pais.Count == 2)
            {
				result = (pais[0] == pais[1]);
				//Debug.Log ("IsCanHU("+result+")");
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
						//Debug.Log ("IsCanHU("+result+")");
						return result;
						//break;
                    }
                }
            }
			result = false;
			//Debug.Log ("[c] IsCanHU("+result+")");
			return result;
        }

        private static bool HuPaiPanDin(List<int> mahs)
        {
            if (mahs.Count == 0)
            {
                return true;
            }

            List<int> fs = mahs.FindAll(delegate (int a)
            {
                return mahs[0] == a;
            });

            //组成克子
            if (fs.Count == 3)
            {
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);
                mahs.Remove(mahs[0]);

                return HuPaiPanDin(mahs);
            }
            else
            { //组成顺子
                if (mahs.Contains(mahs[0] + 1) && mahs.Contains(mahs[0] + 2))
                {
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
        /// <returns>能不能杠牌</returns>
		public static bool IsCanPon(List<int> mahs, int MahID, PLAYERSTATE state)
        {
			//if (state == PLAYERSTATE.RESTING)
			//{
			//	Debug.LogError (state+" == PLAYERSTATE.RESTING");
			//	return false;
			//}
            bool isCanPon = false;

            List<int> currentMahes = mahs.FindAll(delegate (int a)
            {
                return a == MahID;
            });
			//Debug.Log ("currentMahes.Count="+currentMahes.Count);
            if (currentMahes.Count >= 2)
            {
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
		public static bool IsCanGan(List<int> mahs, int MahID, PLAYERSTATE state, bool ismopai = false, List<int> ponmahs = null)
        {
			//Debug.Log ("IsCanGang()");
			//if (state == PLAYERSTATE.RESTING)
            //{
			//	Debug.LogError (state+" == PLAYERSTATE.RESTING");
            //    return false;
            //}
            bool isCanGang = false;

            List<int> currentMahes = mahs.FindAll(delegate (int a)
            {
                return a == MahID;
            });

            if (currentMahes.Count == 3)
            {
				isCanGang = true;
            }
			//Debug.Log ("ismopai="+ismopai+", ponmahs.Count="+ponmahs.Count);
			if (ismopai && ponmahs != null) {
				List<int> xxxMahes = ponmahs.FindAll(delegate (int b)
				{
					return b == MahID;
				});
				//Debug.Log ("xxxMahes.Count="+xxxMahes.Count);
				if (xxxMahes.Count == 3)
				{
					isCanGang = true;
				}
			}
			//Debug.Log ("IsCanGang("+isCanGang+")");
			return isCanGang;
        }

		public static bool IsCanChi(List<int> mahs, int MahID, out int chitype, bool ismopai = false)
		{
			//if (state == PLAYERSTATE.RESTING)
			//{
			//	Debug.LogError (state+" == PLAYERSTATE.RESTING");
			//	return false;
			//}
			bool iscanchi = false;
			chitype = 0;

			if (ismopai) {
				iscanchi = false;
			} else {
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
			}
			//Debug.Log ("[c] IsCanChi("+iscanchi+")");
			return iscanchi;
		}

    }
}