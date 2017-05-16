using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Desktop
{
	public enum PLAYERSTATE
	{
		RESTING = 0,//休息状态
		PLAYING = 1,//正在作决策
		MOPAING = 2,//摸牌中
		CHIPAING =3,//吃牌
		PONPAING =4,//碰牌
		GANPAING =5,//槓牌
		WAITING = 6 //完成
	};
    public class MahPlayer : MonoBehaviour
    {
        #region 玩家的牌

        /// <summary>
        /// 是否为庄家
        /// 游戏开始 第一出手的玩家
        /// </summary>
        public bool isDealer = false;

        /// <summary>
        /// 是否在玩
        /// </summary>
        public bool isPlaying = true;

        /// <summary>
        /// 手牌
        /// </summary>
        public List<int> keepedMah = new List<int>();

        public GameObject plane_keep;

        /// <summary>
        /// 弃牌
        /// </summary>
        public List<int> abandanedMah = new List<int>();

        public GameObject plane_abandan;

        /// <summary>
        /// 碰的牌
        /// </summary>
        public List<int> ponMah = new List<int>();

        public GameObject plane_pon;

		public GameObject plane_mo;

        /// <summary>
        /// 取到的牌
        /// </summary>
        private MahJongObject gotMah;

        /// <summary>
        /// 牌面上被打出去的牌
        /// </summary>
        //public MahJongObject abandonMah;

        #endregion

		#region 玩家的状态

		public PLAYERSTATE state = PLAYERSTATE.RESTING;

        //玩家昵称
        public Text playerName;

        //行动完畢？
		public bool actived = false;

        //与PhotonPlayer中的玩家关联
        public PhotonPlayer photonPlayer;

        //下一个玩家
        public MahPlayer nextPlayer;

        //Photon通讯组件
        public PhotonView photonView;

        #endregion

        #region UI引用

        public Button btnPon;
		public Button btnChi;
        public Button btnWin;
        public Button btnGang;
        public Button btnpass;

        public Timer timer;
        #endregion

		private int moMahId = 0;

        private void Start()
        {
            //注册RaiseEvent事件函数
            //PhotonNetwork.OnEventCall += OnEventCall;
        }
        /// <summary>
        /// 事件绑定
        /// </summary>
        private void BundleUI()
        {
			//Debug.Log ("[c] "+this.name+".BundleUI("+photonPlayer.NickName+")");
			playerName.text = photonPlayer.NickName;

			if (photonPlayer.IsLocal) {
				//Debug.Log ("[c] photonPlayer.IsLocal");

				HideMenu ();

				btnWin.onClick.RemoveAllListeners ();
				btnWin.onClick.AddListener (delegate () {
					AskWin ();
				});

				btnPon.onClick.RemoveAllListeners ();
				btnPon.onClick.AddListener (delegate () {
					AskPon ();
				});

				btnChi.onClick.RemoveAllListeners ();
				btnChi.onClick.AddListener (delegate () {
					AskChi ();
				});

				btnGang.onClick.RemoveAllListeners ();
				btnGang.onClick.AddListener (delegate () {
					AskGan ();
				});

				btnpass.onClick.RemoveAllListeners ();
				btnpass.onClick.AddListener (delegate () {
					AskPass ();
				});
			}
        }

		/// <summary>
		/// 檢查手中的牌
		/// </summary>
		public void checkPai(int pai_id, bool ismopai = false)
		{
			//Debug.LogError ("[c] 檢查手中的牌("+pai_id+", "+ismopai+")("+photonPlayer.NickName+")");
			bool isCanWin = false;
			bool isCanPon = false;
			bool isCanGan = false;
			bool isCanChi = false;
			int chitype = 0;
			if (pai_id > 0) {
				isCanWin = MahJongTools.IsCanHU (keepedMah, pai_id);
				isCanPon = MahJongTools.IsCanPon (keepedMah, pai_id, state);
				isCanGan = MahJongTools.IsCanGan (keepedMah, pai_id, state, ismopai, ponMah);
				isCanChi = MahJongTools.IsCanChi (keepedMah, pai_id, out chitype, ismopai);
			}

			btnWin.gameObject.SetActive(isCanWin);
			btnPon.gameObject.SetActive(isCanPon);
			btnChi.gameObject.SetActive(isCanChi);
			btnGang.gameObject.SetActive(isCanGan);

			if (!isCanWin && !isCanPon && !isCanGan && !isCanChi)
			{//不跳出選單, 直接摸一張牌
				//Debug.Log ("[c] !display pass button");
				btnpass.gameObject.SetActive(false);
				btnpass.transform.parent.parent.gameObject.SetActive (false);
				AskPass ();
			}
			else
			{
				//Debug.Log ("[c] display pass button");
				btnpass.gameObject.SetActive(true);
				btnpass.transform.parent.parent.gameObject.SetActive (true);
			}
		}

        /// <summary>
        /// 显示自己的牌
        /// </summary>
        public void ShowPAI()
        {
			//Debug.Log ("[c] "+this.name+".ShowPAI()");
            BundleUI();

			foreach (PhotonPlayer p in PhotonNetwork.playerList)
			{
				if (p.IsLocal) {
					//Debug.Log ("[c] "+p.mahPlayer.gameObject.name+" photonPlayer.IsLocal");
					keepedMah.Sort ();
					//Debug.Log ("[c] "+p.NickName+".ShowPAI(Count="+keepedMah.Count+")");
					foreach (int a in keepedMah) {
						GameObject d = Instantiate (Resources.Load ("MahJong/" + a) as GameObject);
						d.name = a + "";
						d.transform.SetParent (p.mahPlayer.plane_keep.transform);
						d.transform.localScale = Vector3.one;
						d.transform.localRotation = Quaternion.identity;
						MahJongObject mah = d.GetComponent<MahJongObject> ();
						mah.ID = a;
						mah.CanCilcked = true;
						mah.player = this;
					}
					//PhotonNetwork.RaiseEvent((byte)GameCommand.SHOWPAITOOTHER, null, true, null);
				}
				else
				{
					p.mahPlayer.BundleUI ();
					//Debug.Log ("[c] "+p.mahPlayer.gameObject.name+"!photonPlayer.IsLocal");
					//Debug.Log ("[c] "+p.NickName+".ShowPAI(Count="+Mahjong.MAXPAI+")");
					for (int i = 0; i < Mahjong.MAXPAI; i++)
					{
						GameObject d = Instantiate(Resources.Load("MahJong/" + 0) as GameObject);
						d.name = 0 + "";
						d.transform.SetParent(p.mahPlayer.plane_keep.transform);
						d.transform.localScale = Vector3.one;
						d.transform.localRotation = Quaternion.identity;
					}
				}
			}
        }

        /// <summary>
        /// 把摸到的牌放入keeper
        /// </summary>
		public void fromMoToKeep(int GotID)
        {
			Debug.LogError ("[c] "+this.name+".fromMoToKeep(id="+GotID+")");
			//int GotID = GameManager.Instance.getMahjongPai(isfirst);
			if (photonPlayer.IsLocal) {
				Transform t1 = plane_mo.transform.Find (GotID + "");
				if (t1 != null) {
					GameObject g = t1.gameObject;
					g.transform.SetParent (plane_keep.transform);
					g.transform.localScale = Vector3.one;
					g.transform.localRotation = Quaternion.identity;
				}
				//SetGotMahPosition ();
			} else {
				GotID = 0;
				GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				d.name = GotID + "";
				d.transform.SetParent (plane_keep.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
        }

		/*
		public void fromMoToAban(int GotID)
		{
			//Debug.LogError ("[c] "+this.name+".putPaiToKeep(id="+GotID+",isfirst="+isfirst+")");
			Debug.LogError ("[c] "+this.name+".fromMoToAban(id="+GotID+")");
			//int GotID = GameManager.Instance.getMahjongPai(isfirst);
			if (photonPlayer.IsLocal) {
				Transform t1 = plane_mo.transform.Find (GotID + "");
				if (t1 != null) {
					GameObject g = t1.gameObject;
					g.transform.SetParent (plane_abandan.transform);
					g.transform.localScale = Vector3.one;
					g.transform.localRotation = Quaternion.identity;
				}
			} else {
				//GotID = 0;
				Transform t1 = plane_mo.transform.Find (0 + "");
				if (t1 != null) {
					Destroy (t1.gameObject);
				}

				GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				d.name = GotID + "";
				d.transform.SetParent (plane_abandan.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
		}
		*/

		public void createPaiToMo(int GotID)
		{
			//Debug.LogError ("[c] "+this.name+".DisplayMoPai(id="+GotID+",isfirst="+isfirst+")");
			if (photonPlayer.IsLocal) {
				moMahId = GotID;
				//bool isZimo = MahJongTools.IsCanHU (keepedMah, GotID);
				GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				d.name = GotID + "";
				d.transform.SetParent (plane_mo.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
				//d.transform.

				MahJongObject mah = d.GetComponent<MahJongObject> ();
				mah.ID = GotID;
				mah.CanCilcked = true;
				mah.player = this;

				gotMah = mah;
				//SetGotMahPosition ();
			} else {
				GotID = 0;
				GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				d.name = GotID + "";
				d.transform.SetParent (plane_mo.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
		}

		/*
		public void fromKeepToAban(int mahID)
		{
			//Debug.LogError ("[c] "+this.name+".outPaiFromKeep(id="+mahID+")");
			if (photonPlayer.IsLocal) {
				Transform t1 = plane_keep.transform.Find (mahID + "");
				if (t1 != null) {
					GameObject g = t1.gameObject;
					//g.name = mahID + "";
					g.transform.SetParent (plane_abandan.transform);
					g.transform.localScale = Vector3.one;
					g.GetComponent<MahJongObject> ().CanCilcked = false;
				
					//state = PLAYERSTATE.WAITING;
					//AskActiveNext ();

					MahJongObject mah = g.GetComponent<MahJongObject> ();
					mah.ID = mahID;
					mah.CanCilcked = true;
					mah.player = this;

					gotMah = mah;

					SetGotMahPosition ();
				}
				//GameManager.Instance.abandonMah = g.GetComponent<MahJongObject>();

				//PhotonNetwork.RaiseEvent((byte)GameCommand.DAPAICODE, mahID, true, null);

				//SetGotMahPosition();
				//ActiveNext();
				//HideMenu ();
				//bool isZimo = MahJongTools.IsCanHU (keepedMah, GotID);
				//keepedMah.Add (GotID);
				//GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				//d.name = GotID + "";
				//d.transform.SetParent (plane_keep.transform);
				//d.transform.localScale = Vector3.one;
				//d.transform.localRotation = Quaternion.identity;
				//MahJongObject mah = d.GetComponent<MahJongObject> ();
				//mah.ID = GotID;
				//mah.CanCilcked = true;
				//mah.player = this;
				//gotMah = mah;
			} else {
				//GotID = 0;
				//mahID = 0;
				//GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				//d.name = GotID + "";
				//d.transform.SetParent (plane_keep.transform);
				//d.transform.localScale = Vector3.one;
				//d.transform.localRotation = Quaternion.identity;
				GameObject g = plane_keep.transform.Find(0 + "").gameObject;
				Destroy (g);
				GameObject d = Instantiate (Resources.Load ("MahJong/" + mahID) as GameObject);
				d.name = mahID + "";
				//d.transform.localScale = Vector3.one;//new Vector3 (0.7f, 0.7f, 0.7f);
				//d.transform.localRotation = Quaternion.identity;
				d.GetComponent<MahJongObject>().CanCilcked = false;
				d.transform.SetParent(plane_abandan.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
		}
		*/

		public void DaPaiToAban(int mahID) {
			//Debug.LogError ("[c] "+this.name+".outPaiFromKeep(id="+mahID+")");
			if (photonPlayer.IsLocal) {
				Transform t1 = plane_mo.transform.Find (mahID + "");
				if (t1 == null) {
					t1 = plane_keep.transform.Find (mahID + "");
				}
				if (t1 != null) {
					GameObject g = t1.gameObject;
					g.transform.SetParent (plane_abandan.transform);
					g.transform.localScale = Vector3.one;
					g.transform.localRotation = Quaternion.identity;
				}
				if (moMahId > 0) {
					Debug.Log ("moMahId > 0");
					Transform t2 = plane_mo.transform.Find (moMahId + "");
					if (t2 != null) {
						GameObject g = t2.gameObject;
						g.transform.SetParent (plane_keep.transform);
						g.transform.localScale = Vector3.one;
						g.transform.localRotation = Quaternion.identity;
					}
				}
			} else {
				//GotID = 0;
				Transform t1 = plane_mo.transform.Find (0 + "");
				if (t1 != null) {
					Destroy (t1.gameObject);
				}

				GameObject d = Instantiate (Resources.Load ("MahJong/" + mahID) as GameObject);
				d.name = mahID + "";
				d.transform.SetParent (plane_abandan.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
		}

        /// <summary>
        /// 将取得的牌按照顺序插入
        /// </summary>
        void SetGotMahPosition()
        {
			//Debug.Log (this.name+".SetGotMahPosition()");
            keepedMah.Sort();
            int index = 0;
            for (int i = 0; i < keepedMah.Count; i++)
            {
                if (keepedMah[i] >= gotMah.ID)
                {
                    break;
                }
                index++;
            }

            gotMah.transform.SetSiblingIndex(index);
        }

		/// <summary>
		/// 胡牌
		/// </summary>
		/// <param name="MahID">胡牌的ID</param>
		public void AskWin()
		{
			//Debug.Log ("[c] AskWin()");
			int[] param = { photonPlayer.ID };
			photonView.RPC("WinPai", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

		/// <summary>
		/// 過牌
		/// </summary>
		public void AskPass()
		{
			//Debug.Log ("[c] AskPass()");
			int[] param = { (int)GameCommand.MOPAI, photonPlayer.ID };
			photonView.RPC ("SendRequestToServer", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

		/// <summary>
		/// 槓牌
		/// </summary>
		/// <param name="MahID">碰牌的ID为MahID</param>
		public void AskGan()
		{
			//Debug.Log ("[c] AskGan()");
			int[] param = { (int)GameCommand.GANPAI, photonPlayer.ID };
			photonView.RPC ("SendRequestToServer", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

        /// <summary>
        /// 碰牌
        /// </summary>
        /// <param name="MahID">碰牌的ID为MahID</param>
		public void AskPon()
		{
			//Debug.Log ("[c] AskPon()");
			int[] param = { (int)GameCommand.PONPAI, photonPlayer.ID };
			photonView.RPC ("SendRequestToServer", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

		/// <summary>
		/// 吃牌
		/// </summary>
		public void AskChi()
		{
			//Debug.Log ("[c] AskChi()");
			int[] param = { (int)GameCommand.CHIPAI, photonPlayer.ID };
			photonView.RPC ("SendRequestToServer", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

		/// <summary>
		/// 打牌
		/// </summary>
		/// <param name="ID">打出去牌的ID</param>
		public void AskDaPai(int mahID)
		{
			//Debug.Log ("[c] AskDaPai(" + mahID + ")("+photonPlayer.NickName+")");
			//int[] param = { mahID, photonPlayer.ID };
			int[] param = { (int)GameCommand.DAPAI, photonPlayer.ID, mahID};
			photonView.RPC ("SendRequestToServer", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

		public void handlePon(int mahID)
		{
			//Debug.LogError ("[s] handlePon(" + mahID + ")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			this.state = PLAYERSTATE.PONPAING;
			keepedMah.Remove (mahID);
			keepedMah.Remove (mahID);
			ponMah.Add (mahID);
			ponMah.Add (mahID);
			ponMah.Add (mahID);
			int[] param = { photonPlayer.ID, mahID };
			photonView.RPC ("PonPai", PhotonTargets.AllBuffered, param);
		}

		public void handleMoPai(int mahID)
		{
			this.state = PLAYERSTATE.MOPAING;//更改為摸牌狀態
			//keepedMah.Add (mahID);
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleMoPai("+mahID+", "+keepedMah.Count+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			int[] param = { photonPlayer.ID, mahID};
			photonView.RPC("MoPai", PhotonTargets.AllBuffered, param);
		}

		public void handleDaPai(int mahID)
		{
			this.state = PLAYERSTATE.WAITING;//更改為摸牌狀態
			keepedMah.Remove(mahID);
			abandanedMah.Add(mahID);
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleDaPai("+mahID+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			int[] param = { photonPlayer.ID, mahID};
			photonView.RPC("DaPai", PhotonTargets.AllBuffered, param);
		}

		public void collectPonPai(int mahID)
		{
			//Debug.LogError ("[RPC] collectPonPai(" + mahID + ")");
            if (photonPlayer.IsLocal)
            {
				if (mahID > 0) {
					keepedMah.Remove (mahID);
					keepedMah.Remove (mahID);
					ponMah.Add (mahID);
					ponMah.Add (mahID);
					ponMah.Add (mahID);
					//amahname = Mahjong.getName (mahID);
					//amah.gameObject.transform.SetParent (plane_pon.transform);
					Transform t1 = plane_abandan.transform.FindChild (mahID + "");
					if (t1 != null) {
						t1.SetParent (plane_pon.transform);
						t1.localScale = Vector3.one;
					}
					Transform t2 = plane_keep.transform.FindChild (mahID + "");
					if (t2 != null) {
						t2.SetParent (plane_pon.transform);
						t2.localScale = Vector3.one;
					}
					Transform t3 = plane_keep.transform.FindChild (mahID + "");
					if (t3 != null) {
						t3.SetParent (plane_pon.transform);
						t3.localScale = Vector3.one;
					}
				}
                //HideMenu();
            }
            else
            {
				keepedMah.Remove (mahID);
				keepedMah.Remove (mahID);
				ponMah.Add (mahID);
				ponMah.Add (mahID);
				ponMah.Add (mahID);
				Transform t1 = plane_abandan.transform.FindChild (mahID + "");
				if (t1 != null) {
					t1.SetParent (plane_pon.transform);
					t1.localScale = Vector3.one;
				}
                for (int i = 0; i < 2; i++)
                {

                    //显示碰了的牌
					GameObject d = Instantiate(Resources.Load("MahJong/" + mahID) as GameObject);
					d.name = mahID + "";
                    d.transform.SetParent(plane_pon.transform);
					d.transform.localScale = Vector3.one;
                    d.transform.localRotation = Quaternion.identity;

                    MahJongObject mahObj = d.GetComponent<MahJongObject>();
					mahObj.ID = mahID;
                    mahObj.player = this;
                    mahObj.CanCilcked = false;
                }

                //移除手牌
                for (int i = 0; i < 3; i++)
                {
                    GameObject g = plane_keep.transform.GetChild(i).gameObject;
                    Destroy(g);
                }

            }
        }

		public void handleChi(int mahID)
		{
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleChi("+mahID+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			int chitype = 0;
			if (MahJongTools.IsCanChi (this.keepedMah, mahID, out chitype)) {
				this.state = PLAYERSTATE.CHIPAING;
				if (mahID > 0) {
					switch (chitype) {
					case 1://0,+1, +2
						keepedMah.Remove (mahID);
						keepedMah.Remove (mahID + 1);
						keepedMah.Remove (mahID + 2);
						ponMah.Add (mahID);
						ponMah.Add (mahID + 1);
						ponMah.Add (mahID + 2);
						break;
					case 2://-1,0,+1
						keepedMah.Remove (mahID - 1);
						keepedMah.Remove (mahID);
						keepedMah.Remove (mahID + 1);
						ponMah.Add (mahID - 1);
						ponMah.Add (mahID);
						ponMah.Add (mahID + 1);
						break;
					case 3://-2,-1, 0
						keepedMah.Remove (mahID - 2);
						keepedMah.Remove (mahID - 1);
						keepedMah.Remove (mahID);
						ponMah.Add (mahID - 2);
						ponMah.Add (mahID - 1);
						ponMah.Add (mahID);
						break;
					}
					//Debug.LogError ("[s] " + this.photonPlayer.NickName + ".handleChi(" + mahID + "," + chitype + ")");
					int[] param = { photonPlayer.ID, mahID, chitype };
					photonView.RPC ("ChiPai", PhotonTargets.AllBuffered, param);
				}
			}
		}

		public void collectChiPai(int mahID, int chitype)
		{
			//Debug.LogError ("[RPC] collectChiPai(" + mahID + ")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			Transform t1;
			Transform t2;
			Transform t3;
			GameObject d1;
			GameObject d2;
			GameObject d3;
			MahJongObject mahObj1;
			MahJongObject mahObj2;
			if (photonPlayer.IsLocal)
			{
				if (mahID > 0) {
					//Debug.LogError ("[RPC] 你喊了吃 !!!" + mahID + "(" + chitype + ")");
					switch (chitype) {
					case 1://0,+1, +2
						//keepedMah.Remove (mahID);
						keepedMah.Remove ((mahID + 1));
						keepedMah.Remove ((mahID + 2));
						ponMah.Add (mahID);
						ponMah.Add ((mahID + 1));
						ponMah.Add ((mahID + 2));
						t1 = plane_abandan.transform.FindChild (mahID + "");
						if (t1 != null) {
							t1.SetParent (plane_pon.transform);
							t1.localScale = Vector3.one;
						}
						t2 = plane_keep.transform.FindChild (mahID + 1 + "");
						if (t2 != null) {
							t2.SetParent (plane_pon.transform);
							t2.localScale = Vector3.one;
						}
						t3 = plane_keep.transform.FindChild (mahID + 2 + "");
						if (t3 != null) {
							t3.SetParent (plane_pon.transform);
							t3.localScale = Vector3.one;
						}
						break;
					case 2://-1,0,+1
						keepedMah.Remove ((mahID - 1));
						//keepedMah.Remove (mahID);
						keepedMah.Remove ((mahID + 1));
						ponMah.Add ((mahID - 1));
						ponMah.Add (mahID);
						ponMah.Add ((mahID + 1));
						t1 = plane_abandan.transform.FindChild (mahID + "");
						if (t1 != null) {
							t1.SetParent (plane_pon.transform);
							t1.localScale = Vector3.one;
						}
						t2 = plane_keep.transform.FindChild (mahID - 1 + "");
						if (t2 != null) {
							t2.SetParent (plane_pon.transform);
							t2.localScale = Vector3.one;
						}
						t3 = plane_keep.transform.FindChild (mahID + 1 + "");
						if (t3 != null) {
							t3.SetParent (plane_pon.transform);
							t3.localScale = Vector3.one;
						}
						break;
					case 3://-2,-1, 0
						keepedMah.Remove ((mahID - 2));
						keepedMah.Remove ((mahID - 1));
						//keepedMah.Remove (mahID);
						ponMah.Add ((mahID - 2));
						ponMah.Add ((mahID - 1));
						ponMah.Add (mahID);
						t1 = plane_abandan.transform.FindChild (mahID + "");
						if (t1 != null) {
							t1.SetParent (plane_pon.transform);
							t1.localScale = Vector3.one;
						}
						t2 = plane_keep.transform.FindChild (mahID - 1 + "");
						if (t2 != null) {
							t2.SetParent (plane_pon.transform);
							t2.localScale = Vector3.one;
						}
						t3 = plane_keep.transform.FindChild (mahID - 2 + "");
						if (t3 != null) {
							t3.SetParent (plane_pon.transform);
							t3.localScale = Vector3.one;
						}
						break;
					}
				}
			}
			else
			{
				//Debug.LogError ("[RPC] "+photonPlayer.NickName+"喊了吃 !!!"+amahname+"("+mahID+")");
				if (mahID > 0) {
					//Debug.LogError ("[RPC] 你喊了吃 !!!" + amahname + "(" + mahID + ")");
					switch (chitype) {
					case 1://0,+1, +2
						ponMah.Add (mahID);
						ponMah.Add ((mahID + 1));
						ponMah.Add ((mahID + 2));
						d1 = Instantiate(Resources.Load("MahJong/" + (mahID+1)) as GameObject);
						d1.name = (mahID+1) + "";
						d1.transform.SetParent(plane_pon.transform);
						d1.transform.localScale = Vector3.one;
						d1.transform.localRotation = Quaternion.identity;

						mahObj1 = d1.GetComponent<MahJongObject>();
						mahObj1.ID = (mahID+1);
						mahObj1.player = this;
						mahObj1.CanCilcked = false;

						d2 = Instantiate(Resources.Load("MahJong/" + (mahID+2)) as GameObject);
						d2.name = (mahID+2) + "";
						d2.transform.SetParent(plane_pon.transform);
						d2.transform.localScale = Vector3.one;
						d2.transform.localRotation = Quaternion.identity;

						mahObj2 = d2.GetComponent<MahJongObject>();
						mahObj2.ID = (mahID+2);
						mahObj2.player = this;
						mahObj2.CanCilcked = false;
						break;
					case 2://-1,0,+1
						ponMah.Add ((mahID - 1));
						ponMah.Add (mahID);
						ponMah.Add ((mahID + 1));
						d1 = Instantiate(Resources.Load("MahJong/" + (mahID-1)) as GameObject);
						d1.name = (mahID-1) + "";
						d1.transform.SetParent(plane_pon.transform);
						d1.transform.localScale = Vector3.one;
						d1.transform.localRotation = Quaternion.identity;

						mahObj1 = d1.GetComponent<MahJongObject>();
						mahObj1.ID = (mahID-1);
						mahObj1.player = this;
						mahObj1.CanCilcked = false;

						d2 = Instantiate(Resources.Load("MahJong/" + (mahID+1)) as GameObject);
						d2.name = (mahID+1) + "";
						d2.transform.SetParent(plane_pon.transform);
						d2.transform.localScale = Vector3.one;
						d2.transform.localRotation = Quaternion.identity;

						mahObj2 = d2.GetComponent<MahJongObject>();
						mahObj2.ID = (mahID+1);
						mahObj2.player = this;
						mahObj2.CanCilcked = false;
						break;
					case 3://-2,-1, 0
						ponMah.Add ((mahID - 2));
						ponMah.Add ((mahID - 1));
						ponMah.Add (mahID);
						d1 = Instantiate(Resources.Load("MahJong/" + (mahID-2)) as GameObject);
						d1.name = (mahID-2) + "";
						d1.transform.SetParent(plane_pon.transform);
						d1.transform.localScale = Vector3.one;
						d1.transform.localRotation = Quaternion.identity;

						mahObj1 = d1.GetComponent<MahJongObject>();
						mahObj1.ID = (mahID-2);
						mahObj1.player = this;
						mahObj1.CanCilcked = false;

						d2 = Instantiate(Resources.Load("MahJong/" + (mahID-1)) as GameObject);
						d2.name = (mahID-1) + "";
						d2.transform.SetParent(plane_pon.transform);
						d2.transform.localScale = Vector3.one;
						d2.transform.localRotation = Quaternion.identity;

						mahObj2 = d2.GetComponent<MahJongObject>();
						mahObj2.ID = (mahID-1);
						mahObj2.player = this;
						mahObj2.CanCilcked = false;
						break;
					}

					keepedMah.RemoveAt (0);
					keepedMah.RemoveAt (0);

					t1 = plane_abandan.transform.FindChild (mahID + "");
					if (t1 != null) {
						t1.SetParent (plane_pon.transform);
						t1.transform.localScale = Vector3.one;
					}

					//移除手牌
					for (int i = 0; i < 3; i++)
					{
						GameObject g = plane_keep.transform.GetChild(i).gameObject;
						Destroy(g);
					}
				}
			}
		}

		public void handleGan(int mahID)
		{
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleGan("+mahID+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			this.state = PLAYERSTATE.GANPAING;
			keepedMah.Remove (mahID);
			keepedMah.Remove (mahID);
			keepedMah.Remove (mahID);

			ponMah.Add (mahID);
			ponMah.Add (mahID);
			ponMah.Add (mahID);
			ponMah.Add (mahID);

			int[] param = { photonPlayer.ID, mahID};
			photonView.RPC("GanPai", PhotonTargets.AllBuffered, param);
		}

		public void collectGanPai(int mahID)
		{
			Debug.LogError ("[RPC] collectGanPai(" + mahID + ")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
            if (photonPlayer.IsLocal)
            {
				if (mahID > 0) {
					//显示杠了的牌
					Transform t1 = plane_abandan.transform.FindChild (mahID + "");
					if (t1 != null) {
						t1.SetParent (plane_pon.transform);
						t1.transform.localScale = Vector3.one;
					}

					Transform t2 = plane_keep.transform.FindChild (mahID + "");
					if (t2 != null) {
						t2.SetParent (plane_pon.transform);
						t2.transform.localScale = Vector3.one;
					}
					Transform t3 = plane_keep.transform.FindChild (mahID + "");
					if (t3 != null) {
						t3.SetParent (plane_pon.transform);
						t3.transform.localScale = Vector3.one;
					}
					Transform t4 = plane_keep.transform.FindChild (mahID + "");
					if (t4 != null) {
						t4.SetParent (plane_pon.transform);
						t4.transform.localScale = Vector3.one;
					}
				}
            } else {
				Transform t1 = plane_abandan.transform.FindChild (mahID + "");
				if (t1 != null) {
					t1.SetParent (plane_pon.transform);
					t1.transform.localScale = Vector3.one;
				}

                for (int i = 0; i < 3; i++)
                {
					GameObject d = Instantiate(Resources.Load("MahJong/" + mahID) as GameObject);
					d.name = mahID + "";
                    d.transform.SetParent(plane_pon.transform);
					d.transform.localScale = Vector3.one;
                    d.transform.localRotation = Quaternion.identity;

                    MahJongObject mahObj = d.GetComponent<MahJongObject>();
					mahObj.ID = mahID;
                    mahObj.player = this;
                    mahObj.CanCilcked = false;
                }

                //移除手牌
                for (int i = 0; i < 4; i++)
                {
                    GameObject g = plane_keep.transform.GetChild(i).gameObject;
					Destroy(g);
                }

            }
        }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public void HideMenu()
        {
			//Debug.Log ("[c] HideMenu()");
            btnPon.gameObject.SetActive(false);
            btnWin.gameObject.SetActive(false);
            btnGang.gameObject.SetActive(false);
            btnpass.gameObject.SetActive(false);
			btnPon.transform.parent.parent.gameObject.SetActive (false);
        }
    }
}