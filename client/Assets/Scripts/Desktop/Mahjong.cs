using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Desktop
{
	public enum GameCommand {
		SHUFFLEPAI = 0,       //洗牌
		CHIPAI = 1,           //吃牌
		DAPAI = 3,		  	  //打牌
		SHOWPAI = 4,   		  //显示自己牌的数量
		ACTIVENEXT = 5,       //下一个玩家的行动激活
		NEXTPLAYERGETMAH = 6, //下为玩家取牌
		PONPAI = 7,           //碰牌
		GANPAI = 8,           //杠牌
		WINPAI = 9, 		  //胡牌
		DEALING = 10,     	  //發牌
		MOPAI = 11,      	  //摸牌
		ONEMORETIME = 12      //再玩一次
	};

    public class Mahjong : MonoBehaviour
    {

        /// <summary>
        /// 所有的牌
        /// </summary>
        //public List<int> allMah = new List<int>();
		public Queue<int> allMah = null;
		public static int MAXPAI = 16;
        /// <summary>
        /// 牌的ID
        /// 1-9 -> 筒
        /// 11-19 -> 條
		/// 21-29 ->萬
        /// 31 32 33 34 35 36 37-> 中發白東南西北
        /// </summary>
        //private int[] ID = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17 };
		private int[] ID0 = { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8};
		private int[] ID1 = { 9, 9, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17 };
		private int[] ID2 = {18, 18, 19, 19, 21, 21, 22, 22, 23, 23, 24, 24, 25, 25, 26, 26,
			27, 27, 28, 28, 29, 29, 31, 31, 32, 32, 33, 33, 34, 34, 35, 35, 36, 36, 37, 37};
		//private int[] ID1 = { 18, 19, 21, 22, 23 };
		private static string[] ccame = {
			"", "紅中", "發財", "白板", "東風", "南風", "西風", "北風"
		};
        //private List<int> _list = new List<int>();
		private Queue<int> _list = new Queue<int>();

        #region

        #endregion

        void Awake()
        {
            //注册事件
            //PhotonNetwork.OnEventCall += this.OnEvent;

            if (!PhotonNetwork.isMasterClient)
            {
				//Debug.Log ("[s] Mahjong.Awake() !PhotonNetwork.isMasterClient");
                //return;
            }

			//_list.Clear();
            for (int i = 0; i < ID0.Length; i++)
            {
				_list.Enqueue(ID0[i]);
				//_list.Enqueue(ID0[i]);
            }
			for (int i = 0; i < ID0.Length; i++)
			{
				_list.Enqueue(ID0[i]);
				//_list.Enqueue(ID0[i]);
			}

			for (int i = 0; i < ID1.Length; i++)
			{
				_list.Enqueue(ID1[i]);
			}

			//for (int i = 0; i < ID0.Length; i++)
			//{
			//	_list.Enqueue(ID0[i]);
			//}
			for (int i = 0; i < ID1.Length; i++) {
				_list.Enqueue (ID1 [i]);
			}
			for (int i = 0; i < ID2.Length; i++) {
				_list.Enqueue (ID2 [i]);
			}
			for (int i = 0; i < ID2.Length; i++) {
				_list.Enqueue (ID2 [i]);
			}

			//foreach (int x in _list)
			//{
			//	Debug.Log(x);
			//}
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void ShuffleMah()
        {
			//Debug.LogError ("[s] Mahjong.ShuffleMah()");
			if(allMah!=null)
				allMah.Clear();
			allMah = new Queue<int> (_list);
			//List<int> mahs = new List<int>(_list);
			/*
            while (mahs.Count > 0)
            {
				int index = 0;
				//int index = UnityEngine.Random.Range(0, mahs.Count);
                allMah(mahs[index]);
                mahs.RemoveAt(index);
            }
            */
			//PhotonNetwork.RaiseEvent((byte)GameCommand.SHUFFLECODE, allMah.ToArray(), true, null);

        }
		/*
		private void OnEvent(byte eventcode, object content, int senderid)
        {
			//Debug.Log("MahJong.OnEvent( " + eventcode+")");
			if (eventcode == (byte)GameCommand.SHUFFLECODE)
            {
				Debug.LogError("MahJong.OnEvent(GameCommand.SHUFFLECODE)");
                allMah = new List<int>((int[])content);
				Debug.LogError ("allMah.Count="+allMah.Count);
            }
        }
        */
		public static string getName(int id)
		{
			string name = string.Empty;
			if (id > 0 && id < 10) {
				name = id + "筒";
			} else if (id > 10 && id < 20) {
				name = (id-10) + "條";
			} else if (id > 20 && id < 30) {
				name = (id-20) + "萬";
			} else if (id > 30 && id < 40) {
				name = ccame[(id-30)];
			}
			return name;
		}
    }
}