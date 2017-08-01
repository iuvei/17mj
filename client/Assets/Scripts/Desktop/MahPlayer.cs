using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
		public int ID = 0;
		public bool isAI = false;
		public bool autoPlay = false;
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


		/// <summary>
		/// 摸的牌
		/// </summary>
		public GameObject plane_mo;

		public Transform tmp;

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

        //Photon通讯组件
        public PhotonView photonView;

        #endregion

        #region UI引用

        public Button btnPon;
		public Button btnChi;
        public Button btnWin;
        public Button btnGang;
        public Button btnPass;
		public Toggle btnAuto;

        public Timer timer;
        #endregion

		private int moMahId = 0;

        private void Start()
        {
            //注册RaiseEvent事件函数
            //PhotonNetwork.OnEventCall += OnEventCall;

            //吃碰槓胡面板隱藏
			if(btnPon)
            	btnPon.transform.parent.parent.gameObject.SetActive(false);
        }
        /// <summary>
        /// 事件绑定
        /// </summary>
        private void BundleUIEvent()
        {
			//Debug.Log ("[c] "+this.name+".BundleUI("+photonPlayer.NickName+")");
			if (photonPlayer != null) {
				playerName.text = photonPlayer.NickName;
			}

			if (this.ID==PhotonNetwork.player.ID) {

				HideMenu ();
				if (btnWin != null) {
					btnWin.onClick.RemoveAllListeners ();
					btnWin.onClick.AddListener (delegate () {
						AskWin ();
					});
				}
				if (btnPon != null) {
					btnPon.onClick.RemoveAllListeners ();
					btnPon.onClick.AddListener (delegate () {
						AskPon ();
					});
				}
				if (btnChi != null) {
					btnChi.onClick.RemoveAllListeners ();
					btnChi.onClick.AddListener (delegate () {
						AskChi ();
					});
				}
				if (btnGang != null) {
					btnGang.onClick.RemoveAllListeners ();
					btnGang.onClick.AddListener (delegate () {
						AskGan ();
					});
				}
				if (btnPass != null) {
					btnPass.onClick.RemoveAllListeners ();
					btnPass.onClick.AddListener (delegate () {
						AskPass ();
					});
				}
				if (btnAuto != null) {
					btnAuto.onValueChanged.RemoveAllListeners ();
					btnAuto.onValueChanged.AddListener (delegate {
						AskAutoPlay ();
					});
				}
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
				isCanPon = MahJongTools.IsCanPon (keepedMah, pai_id);
				isCanGan = MahJongTools.IsCanGan (keepedMah, pai_id, ismopai, ponMah);
				isCanChi = MahJongTools.IsCanChi (keepedMah, pai_id, out chitype, ismopai);
			}
			if(btnWin)
				btnWin.gameObject.SetActive(isCanWin);
			if(btnPon)
				btnPon.gameObject.SetActive(isCanPon);
			if(btnChi)
				btnChi.gameObject.SetActive(isCanChi);
			if(btnGang)
				btnGang.gameObject.SetActive(isCanGan);

			if (!isCanWin && !isCanPon && !isCanGan && !isCanChi)
			{//不跳出選單, 直接摸一張牌
				//Debug.Log ("[c] !display pass button");
				if (btnPass) {
					btnPass.gameObject.SetActive (false);
					btnPass.transform.parent.parent.gameObject.SetActive (false);
				}
				//if(!this.autoPlay && !this.isAI && this.ID!=PhotonNetwork.player.ID)
				//	AskPass ();
			}
			else
			{
				//Debug.Log ("[c] display pass button");
				if (btnPass) {
					btnPass.gameObject.SetActive (true);
					btnPass.transform.parent.parent.gameObject.SetActive (true);
				}
			}
		}

		public void clearPAI()
		{
			//Debug.Log ("[c] "+this.name+".clearPAI()");
			for (int i = plane_keep.transform.childCount - 1; i >= 0; i--) {
				Destroy(plane_keep.transform.GetChild(i).gameObject);
			}
		}

        /// <summary>
        /// 显示自己的牌
        /// </summary>
        public void ShowPAI()
        {
			//Debug.Log ("[c] "+this.name+".ShowPAI()");
            BundleUIEvent();
			if (this.ID==PhotonNetwork.player.ID) {
				keepedMah.Sort ();
				//Debug.Log ("[c] "+p.NickName+".ShowPAI(Count="+keepedMah.Count+")");
				foreach (int a in keepedMah) {
					GameObject d = Instantiate (Resources.Load ("MahJong/" + a) as GameObject);
					d.name = a + "";
					d.transform.SetParent (plane_keep.transform);
					d.transform.localScale = Vector3.one;
					d.transform.localRotation = Quaternion.identity;
					MahJongObject mah = d.GetComponent<MahJongObject> ();
					mah.ID = a;
					mah.CanCilcked = true;
					mah.player = this;
				}

			} else {
				//is AI
				for (int i = 0; i < Mahjong.MAXPAI; i++)
				{
					GameObject d = Instantiate(Resources.Load("MahJong/" + 0) as GameObject);
					d.name = 0 + "";
					d.transform.SetParent(plane_keep.transform);
					d.transform.localScale = Vector3.one;
					d.transform.localRotation = Quaternion.identity;
				}
			}
        }


        /// <summary>
        /// 把摸到的牌放入keeper
        /// </summary>
		public void fromMoToKeep(int GotID)
        {
			//Debug.LogError ("[c] "+this.name+".fromMoToKeep(id="+GotID+")");
			if (this.ID==PhotonNetwork.player.ID) {
				Transform t1 = plane_mo.transform.Find (GotID + "");
				if (t1 != null) {
					GameObject g = t1.gameObject;
					g.transform.SetParent (plane_keep.transform);
					g.transform.localScale = Vector3.one;
					g.transform.localRotation = Quaternion.identity;
					MahJongObject mah = g.GetComponent<MahJongObject> ();
					SetGotMahPosition (mah);
				}
			} else {
				GotID = 0;
				GameObject d = Instantiate (Resources.Load ("MahJong/" + GotID) as GameObject);
				d.name = GotID + "";
				d.transform.SetParent (plane_keep.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
        }

		public void createPaiToMo(int mahID)
		{
			//Debug.LogError ("[c] "+this.name+".createPaiToMo(id="+GotID+")");
			if (this.ID==PhotonNetwork.player.ID) {
				this.moMahId = mahID;
				//bool isZimo = MahJongTools.IsCanHU (keepedMah, GotID);
				GameObject d = Instantiate (Resources.Load ("MahJong/" + mahID) as GameObject);
				d.name = mahID + "";
				d.transform.SetParent (plane_mo.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
				//d.transform.

				MahJongObject mah = d.GetComponent<MahJongObject> ();
				mah.ID = mahID;
				mah.CanCilcked = true;
				mah.player = this;
			} else {
				mahID = 0;
				GameObject d = Instantiate (Resources.Load ("MahJong/" + mahID) as GameObject);
				d.name = mahID + "";
				d.transform.SetParent (plane_mo.transform);
				d.transform.localScale = Vector3.one;
				d.transform.localRotation = Quaternion.identity;
			}
		}

		public void DaPaiToAban(int mahID, int cnt) {
			//Debug.LogError ("[c] "+this.name+" DaPaiToAban(id="+mahID+", cnt="+cnt+")");
			Vector2 dp = new Vector2((cnt-1)*74+37, -(int)(cnt / 18)*90+45);
			RectTransform apos = GameManager.Instance.abanPos;
			if (this.ID==PhotonNetwork.player.ID) {
				//Debug.Log ("QQQQQQQQQ");
				Transform t1 = plane_mo.transform.Find (mahID + "");
				if (t1 == null) {
					t1 = plane_keep.transform.Find (mahID + "");
				}
				if (t1 == null) {
					t1 = tmp.Find (mahID + "");
				}
				if (t1 != null) {
					t1.transform.SetParent (this.tmp);
					t1.transform.localPosition = Vector3.zero;
					t1.transform.localScale = Vector3.one;
					t1.transform.localRotation = Quaternion.identity;

					//Vector2 fp =  new Vector3(apos.anchoredPosition.x+dp.x, apos.anchoredPosition.y+dp.y);
					Vector2 fp =  new Vector3(-650+dp.x, 282);
					//Debug.Log ("fp="+fp);
					RectTransform r1 = t1.GetComponent<RectTransform>();
					t1.transform.DOScale (new Vector3 (0.9f, 0.9f, 0.9f), 0.3f).OnComplete (() => {
						t1.transform.localScale = Vector3.one;
					});
					r1.DOAnchorPos (fp, 0.3f, false).OnComplete(() => {
						//Debug.Log("Complete!");
						t1.transform.SetParent (plane_abandan.transform);
						t1.transform.localScale = Vector3.one;
						t1.transform.localRotation = Quaternion.identity;
					});
				}
				if (this.moMahId != mahID) {
					this.fromMoToKeep (this.moMahId);
				}
			} else {
				//GotID = 0;
				//Debug.Log ("tttttttt");
				Transform t1 = plane_mo.transform.Find (0 + "");
				if (t1 == null) {
					t1 = plane_keep.transform.Find (mahID + "");
				}
				if (t1 != null) {
					Destroy (t1.gameObject);
				}
				GameObject t2 = Instantiate (Resources.Load ("MahJong/" + mahID) as GameObject);
				t2.name = mahID + "";
				t2.transform.SetParent (this.tmp);
				t2.transform.localPosition = Vector3.zero;
				t2.transform.localScale = Vector3.one;
				t2.transform.localRotation = Quaternion.identity;
				RectTransform r2 = t2.GetComponent<RectTransform>();
				//RectTransform r1 = t1.GetComponent<RectTransform> ();
				//r2.anchoredPosition = new Vector2 (-800, 200);
				Vector2 fp =  new Vector3(-650+dp.x, -90);
				t2.transform.DOScale (new Vector3 (0.9f, 0.9f, 0.9f), 0.3f).OnComplete(() => {
					t2.transform.localScale = Vector3.one;
				});
				r2.DOAnchorPos (fp, 0.3f, false).OnComplete(() => {
					//Debug.Log("Complete!");
					t2.transform.SetParent (plane_abandan.transform);
					//t2.transform.localScale = Vector3.one;
					t2.transform.localRotation = Quaternion.identity;
				});
			}
		}

        /// <summary>
        /// 将取得的牌按照顺序插入
        /// </summary>
		void SetGotMahPosition(MahJongObject mah)
        {
			//Debug.Log (this.name+".SetGotMahPosition()");
            keepedMah.Sort();
            int index = 0;
            for (int i = 0; i < keepedMah.Count; i++)
            {
				if (keepedMah[i] >= mah.ID)
                {
                    break;
                }
                index++;
            }

			mah.transform.SetSiblingIndex(index);
        }

		/// <summary>
		/// 胡牌
		/// </summary>
		/// <param name="MahID">胡牌的ID</param>
		public void AskWin()
		{
            //Debug.Log ("[c] AskWin()");
            //photonView.RPC("WinPai", PhotonTargets.MasterClient, param); 
			object[] param = { (int)GameCommand.WINPAI, this.ID };
            photonView.RPC("SendRequestToServer", PhotonTargets.MasterClient, param);
            HideMenu ();
		}
			
		public void AskAutoPlay()
		{
			bool en = !this.autoPlay;
			//Debug.Log ("[c] AskAutoPlay()"); 
			object[] param = { (int)GameCommand.AUTOPLAY, this.ID,  en};
			photonView.RPC("SendRequestToServer", PhotonTargets.MasterClient, param);
		}

		/// <summary>
		/// 過牌
		/// </summary>
		public void AskPass()
		{
			Debug.Log (this.name+" [c] AskPass() auto="+this.autoPlay+" ,isAi="+this.isAI);
			object[] param = { (int)GameCommand.MOPAI, this.ID };
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
			object[] param = { (int)GameCommand.GANPAI, this.ID };
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
			object[] param = { (int)GameCommand.PONPAI, this.ID };
            //Debug.Log("param = "+ param);
			photonView.RPC ("SendRequestToServer", PhotonTargets.MasterClient, param);
			HideMenu ();
		}

		/// <summary>
		/// 吃牌
		/// </summary>
		public void AskChi()
		{
			//Debug.Log ("[c] AskChi()");
			object[] param = { (int)GameCommand.CHIPAI, this.ID };
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
			object[] param = { (int)GameCommand.DAPAI, this.ID, mahID};
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
			int[] param = { this.ID, mahID };
			photonView.RPC ("PonPai", PhotonTargets.All, param);
		}

		public void handleMoPai(int mahID, int cnt)
		{
			//Debug.Log (this.name+".handleMoPai("+mahID+")");
			this.state = PLAYERSTATE.MOPAING;//更改為摸牌狀態
			keepedMah.Add (mahID);
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleMoPai("+mahID+", "+keepedMah.Count+")");
			//string amahname = string.Empty;
			//keepedMah.Add (mahID);
			string amahname = Mahjong.getName (mahID);
			int[] param = { this.ID, mahID, cnt};
			photonView.RPC ("MoPai", PhotonTargets.All, param);
		}

		public void handleAskAutoPlay(bool auto)
		{
			//Debug.Log ("handleAskAutoPlay(this.autoPlay="+auto+")");
			this.autoPlay = auto;
			object[] param = { this.ID, auto};
			photonView.RPC ("SetupAI", PhotonTargets.All, param);

		}

		public void handleDaPai(int mahID)
		{
			//Debug.Log (this.name+".handleDaPai("+mahID+")");
			this.state = PLAYERSTATE.WAITING;//更改為摸牌狀態
			keepedMah.Remove(mahID);
			abandanedMah.Add(mahID);
			//GameManager.Instance.doHandleDaPai(
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleDaPai("+mahID+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			int[] param = { this.ID, mahID};
			photonView.RPC("DaPai", PhotonTargets.All, param);
		}

		public void collectPonPai(int mahID)
		{
			//Debug.LogError ("[RPC] collectPonPai(" + mahID + ")");
			GameObject go = new GameObject("Pon_set");
			//go.AddComponent<Image> ();

			LayoutElement le = go.AddComponent<LayoutElement> ();
			le.minWidth = 67 * 3;
			le.minHeight = 100;

			GridLayoutGroup glg = go.AddComponent<GridLayoutGroup> ();
			glg.cellSize = new Vector2 (67, 82);
			glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
			glg.constraintCount = 1;

			if (this.ID==PhotonNetwork.player.ID) {
				if (mahID > 0) {
					keepedMah.Remove (mahID);
					keepedMah.Remove (mahID);
					ponMah.Add (mahID);
					ponMah.Add (mahID);
					ponMah.Add (mahID);
					Transform t1 = plane_abandan.transform.Find (mahID + "");
					if (t1 == null) {
						t1 = tmp.Find (mahID + "");
					}
					if (t1 != null) {
						t1.SetParent (go.transform);
						t1.localScale = Vector3.one;
					} else {
						Debug.Log ("t1="+t1);
					}
					Transform t2 = plane_keep.transform.Find (mahID + "");
					if (t2 != null) {
						t2.SetParent (go.transform);
						t2.localScale = Vector3.one;
					} else {
						Debug.Log ("t2="+t2);
					}
					Transform t3 = plane_keep.transform.Find (mahID + "");
					if (t3 != null) {
						t3.SetParent (go.transform);
						t3.localScale = Vector3.one;
					} else {
						Debug.Log ("t3="+t3);
					}
					go.transform.SetParent (plane_pon.transform);
					go.transform.localScale = Vector3.one;
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
				//GameObject go = new GameObject("set");
				Transform t1 = plane_abandan.transform.Find (mahID + "");
				if (t1 == null) {
					t1 = tmp.Find (mahID + "");
				}
				if (t1 != null) {
					t1.SetParent (go.transform);
					t1.localScale = Vector3.one;
				}
                for (int i = 0; i < 2; i++)
                {

                    //显示碰了的牌
					GameObject d = Instantiate(Resources.Load("MahJong/" + mahID) as GameObject);
					d.name = mahID + "";
					d.transform.SetParent(go.transform);
					d.transform.localScale = Vector3.one;
                    d.transform.localRotation = Quaternion.identity;

                    MahJongObject mahObj = d.GetComponent<MahJongObject>();
					mahObj.ID = mahID;
                    mahObj.player = this;
                    mahObj.CanCilcked = false;
                }

				go.transform.SetParent (plane_pon.transform);
				go.transform.localScale = Vector3.one;

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
			//Debug.LogError ("[s] handleChi("+mahID+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			int chitype = 0;
			if (MahJongTools.IsCanChi (this.keepedMah, mahID, out chitype)) {
				this.state = PLAYERSTATE.CHIPAING;
				if (mahID > 0) {
					switch (chitype) {
					case 1://0,+1, +2
						//keepedMah.Remove (mahID);
						keepedMah.Remove (mahID + 1);
						keepedMah.Remove (mahID + 2);
						ponMah.Add (mahID);
						ponMah.Add (mahID + 1);
						ponMah.Add (mahID + 2);
						break;
					case 2://-1,0,+1
						keepedMah.Remove (mahID - 1);
						//keepedMah.Remove (mahID);
						keepedMah.Remove (mahID + 1);
						ponMah.Add (mahID - 1);
						ponMah.Add (mahID);
						ponMah.Add (mahID + 1);
						break;
					case 3://-2,-1, 0
						keepedMah.Remove (mahID - 2);
						keepedMah.Remove (mahID - 1);
						//keepedMah.Remove (mahID);
						ponMah.Add (mahID - 2);
						ponMah.Add (mahID - 1);
						ponMah.Add (mahID);
						break;
					}
					Debug.LogError ("[s] " + this.name + ".handleChi(" + mahID + "," + chitype + ")");
					int[] param = { this.ID, mahID, chitype };
					photonView.RPC ("ChiPai", PhotonTargets.All, param);
				}
			}
		}

		public void collectChiPai(int mahID, int chitype)
		{
			//Debug.LogError ("[RPC] collectChiPai(" + mahID + ", chitype="+chitype+")");
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
			GameObject go = new GameObject("Chi_set");
			//go.AddComponent<Image> ();
			LayoutElement le = go.AddComponent<LayoutElement> ();
			le.minWidth = 67 * 3;
			le.minHeight = 100;
			GridLayoutGroup glg = go.AddComponent<GridLayoutGroup> ();
			glg.cellSize = new Vector2 (67, 82);
			glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
			glg.constraintCount = 1;
			if (this.ID==PhotonNetwork.player.ID)
			{
				if (mahID > 0) {
					//Debug.LogError ("[RPC] 你喊了吃 !!!" + mahID + "(" + chitype + ")");
					switch (chitype) {
					case 1://0,+1, +2
						if (!PhotonNetwork.player.IsMasterClient) {
							keepedMah.Remove (mahID);
							keepedMah.Remove ((mahID + 1));
							keepedMah.Remove ((mahID + 2));
							ponMah.Add (mahID);
							ponMah.Add ((mahID + 1));
							ponMah.Add ((mahID + 2));
						}
						t2 = plane_keep.transform.Find (mahID + 1 + "");
						if (t2 == null) {
							t2 = tmp.Find (mahID + "");
						}
						if (t2 != null) {
							t2.SetParent (go.transform);
							t2.localScale = Vector3.one;
						}

						t1 = plane_abandan.transform.Find (mahID + "");
						if (t1 == null) {
							t1 = tmp.Find (mahID + "");
						}
						if (t1 != null) {
							t1.SetParent (go.transform);
							t1.localScale = Vector3.one;
						}

						t3 = plane_keep.transform.Find (mahID + 2 + "");
						if (t3 == null) {
							t3 = tmp.Find (mahID + "");
						}
						if (t3 != null) {
							t3.SetParent (go.transform);
							t3.localScale = Vector3.one;
						}
						break;
					case 2://-1,0,+1
						if (!PhotonNetwork.player.IsMasterClient) {
							keepedMah.Remove ((mahID - 1));
							keepedMah.Remove (mahID);
							keepedMah.Remove ((mahID + 1));
							ponMah.Add ((mahID - 1));
							ponMah.Add (mahID);
							ponMah.Add ((mahID + 1));
						}

						t2 = plane_keep.transform.Find (mahID - 1 + "");
						if (t2 == null) {
							t2 = tmp.Find (mahID + "");
						}
						if (t2 != null) {
							t2.SetParent (go.transform);
							t2.localScale = Vector3.one;
						}

						t1 = plane_abandan.transform.Find (mahID + "");
						if (t1 == null) {
							t1 = tmp.Find (mahID + "");
						}
						if (t1 != null) {
							t1.SetParent (go.transform);
							t1.localScale = Vector3.one;
						}

						t3 = plane_keep.transform.Find (mahID + 1 + "");
						if (t3 == null) {
							t3 = tmp.Find (mahID + "");
						}
						if (t3 != null) {
							t3.SetParent (go.transform);
							t3.localScale = Vector3.one;
						}
						break;
					case 3://-2,-1, 0
						if (!PhotonNetwork.player.IsMasterClient) {
							keepedMah.Remove ((mahID - 2));
							keepedMah.Remove ((mahID - 1));
							keepedMah.Remove (mahID);
							ponMah.Add ((mahID - 2));
							ponMah.Add ((mahID - 1));
							ponMah.Add (mahID);
						}

						t2 = plane_keep.transform.Find (mahID - 1 + "");
						if (t2 == null) {
							t2 = tmp.Find (mahID + "");
						}
						if (t2 != null) {
							t2.SetParent (go.transform);
							t2.localScale = Vector3.one;
						} else {
							Debug.Log ("t2="+t2);
						}

						t1 = plane_abandan.transform.Find (mahID + "");
						if (t1 == null) {
							t1 = tmp.Find (mahID + "");
						}
						if (t1 != null) {
							t1.SetParent (go.transform);
							t1.localScale = Vector3.one;
						} else {
							Debug.Log ("t1="+t1);
						}

						t3 = plane_keep.transform.Find (mahID - 2 + "");
						if (t3 == null) {
							t3 = tmp.Find (mahID + "");
						}
						if (t3 != null) {
							t3.SetParent (go.transform);
							t3.localScale = Vector3.one;
						} else {
							Debug.Log ("t3="+t3);
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
						if (!PhotonNetwork.player.IsMasterClient) {
							ponMah.Add (mahID);
							ponMah.Add ((mahID + 1));
							ponMah.Add ((mahID + 2));
						}
						d1 = Instantiate(Resources.Load("MahJong/" + (mahID+1)) as GameObject);
						d1.name = (mahID+1) + "";
						d1.transform.SetParent(go.transform);
						d1.transform.localScale = Vector3.one;
						d1.transform.localRotation = Quaternion.identity;

						mahObj1 = d1.GetComponent<MahJongObject>();
						mahObj1.ID = (mahID+1);
						mahObj1.player = this;
						mahObj1.CanCilcked = false;

						t1 = plane_abandan.transform.Find (mahID + "");
						if (t1 == null) {
							t1 = tmp.Find (mahID + "");
						}
						if (t1 != null) {
							t1.SetParent (go.transform);
							t1.transform.localScale = Vector3.one;
						}

						d2 = Instantiate(Resources.Load("MahJong/" + (mahID+2)) as GameObject);
						d2.name = (mahID+2) + "";
						d2.transform.SetParent(go.transform);
						d2.transform.localScale = Vector3.one;
						d2.transform.localRotation = Quaternion.identity;

						mahObj2 = d2.GetComponent<MahJongObject>();
						mahObj2.ID = (mahID+2);
						mahObj2.player = this;
						mahObj2.CanCilcked = false;
						break;
					case 2://-1,0,+1
						if (!PhotonNetwork.player.IsMasterClient) {
							ponMah.Add ((mahID - 1));
							ponMah.Add (mahID);
							ponMah.Add ((mahID + 1));
						}
						d1 = Instantiate(Resources.Load("MahJong/" + (mahID-1)) as GameObject);
						d1.name = (mahID-1) + "";
						d1.transform.SetParent(go.transform);
						d1.transform.localScale = Vector3.one;
						d1.transform.localRotation = Quaternion.identity;

						mahObj1 = d1.GetComponent<MahJongObject>();
						mahObj1.ID = (mahID-1);
						mahObj1.player = this;
						mahObj1.CanCilcked = false;

						t1 = plane_abandan.transform.Find (mahID + "");
						if (t1 == null) {
							t1 = tmp.Find (mahID + "");
						}
						if (t1 != null) {
							t1.SetParent (go.transform);
							t1.transform.localScale = Vector3.one;
						}

						d2 = Instantiate(Resources.Load("MahJong/" + (mahID+1)) as GameObject);
						d2.name = (mahID+1) + "";
						d2.transform.SetParent(go.transform);
						d2.transform.localScale = Vector3.one;
						d2.transform.localRotation = Quaternion.identity;

						mahObj2 = d2.GetComponent<MahJongObject>();
						mahObj2.ID = (mahID+1);
						mahObj2.player = this;
						mahObj2.CanCilcked = false;
						break;
					case 3://-2,-1, 0
						if (!PhotonNetwork.player.IsMasterClient) {
							ponMah.Add ((mahID - 2));
							ponMah.Add ((mahID - 1));
							ponMah.Add (mahID);
						}
						d1 = Instantiate(Resources.Load("MahJong/" + (mahID-2)) as GameObject);
						d1.name = (mahID-2) + "";
						d1.transform.SetParent(go.transform);
						d1.transform.localScale = Vector3.one;
						d1.transform.localRotation = Quaternion.identity;

						mahObj1 = d1.GetComponent<MahJongObject>();
						mahObj1.ID = (mahID-2);
						mahObj1.player = this;
						mahObj1.CanCilcked = false;

						t1 = plane_abandan.transform.Find (mahID + "");
						if (t1 == null) {
							t1 = tmp.Find (mahID + "");
						}
						if (t1 != null) {
							t1.SetParent (go.transform);
							t1.transform.localScale = Vector3.one;
						}

						d2 = Instantiate(Resources.Load("MahJong/" + (mahID-1)) as GameObject);
						d2.name = (mahID-1) + "";
						d2.transform.SetParent(go.transform);
						d2.transform.localScale = Vector3.one;
						d2.transform.localRotation = Quaternion.identity;

						mahObj2 = d2.GetComponent<MahJongObject>();
						mahObj2.ID = (mahID-1);
						mahObj2.player = this;
						mahObj2.CanCilcked = false;
						break;
					}
					if (!PhotonNetwork.player.IsMasterClient) {
						keepedMah.RemoveAt (0);
						keepedMah.RemoveAt (0);
					}

					//移除手牌
					for (int i = 0; i < 3; i++)
					{
						GameObject g = plane_keep.transform.GetChild(i).gameObject;
						Destroy(g);
					}
				}
			}
			go.transform.SetParent (plane_pon.transform);
			go.transform.localScale = Vector3.one;
		}

		public void handleGan(int mahID)
		{
			//Debug.LogError ("[s] "+this.photonPlayer.NickName+".handleGan("+mahID+")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			this.state = PLAYERSTATE.GANPAING;
			if (!PhotonNetwork.player.IsMasterClient) {
				keepedMah.Remove (mahID);
				keepedMah.Remove (mahID);
				keepedMah.Remove (mahID);

				ponMah.Add (mahID);
				ponMah.Add (mahID);
				ponMah.Add (mahID);
				ponMah.Add (mahID);
			}

			int[] param = { this.ID, mahID};
			photonView.RPC("GanPai", PhotonTargets.All, param);
		}

		public void collectGanPai(int mahID)
		{
			if (mahID <= 0)
				return;
			Debug.LogError ("[RPC] collectGanPai(" + mahID + ")");
			string amahname = string.Empty;
			amahname = Mahjong.getName (mahID);
			GameObject go = new GameObject("Gan_set");
			LayoutElement le = go.AddComponent<LayoutElement> ();
			le.minWidth = 67 * 4;
			le.minHeight = 100;
			GridLayoutGroup glg = go.AddComponent<GridLayoutGroup> ();
			glg.cellSize = new Vector2 (67, 82);
			glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
			glg.constraintCount = 1;
			if (this.ID==PhotonNetwork.player.ID)
            {
				if (mahID > 0) {
					//显示杠了的牌
					Transform t1 = plane_abandan.transform.Find (mahID + "");
					if (t1 == null) {
						t1 = tmp.Find (mahID + "");
					}
					if (t1 != null) {
						t1.SetParent (go.transform);
						t1.transform.localScale = Vector3.one;
					}

					Transform t2 = plane_keep.transform.Find (mahID + "");
					if (t2 != null) {
						t2.SetParent (go.transform);
						t2.transform.localScale = Vector3.one;
					}
					Transform t3 = plane_keep.transform.Find (mahID + "");
					if (t3 != null) {
						t3.SetParent (go.transform);
						t3.transform.localScale = Vector3.one;
					}
					Transform t4 = plane_keep.transform.Find (mahID + "");
					if (t4 != null) {
						t4.SetParent (go.transform);
						t4.transform.localScale = Vector3.one;
					}
				}
            } else {
				Transform t1 = plane_abandan.transform.Find (mahID + "");
				if (t1 == null) {
					t1 = tmp.Find (mahID + "");
				}
				if (t1 != null) {
					t1.SetParent (go.transform);
					t1.transform.localScale = Vector3.one;
				}

                for (int i = 0; i < 3; i++)
                {
					GameObject d = Instantiate(Resources.Load("MahJong/" + mahID) as GameObject);
					d.name = mahID + "";
					d.transform.SetParent(go.transform);
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
			go.transform.SetParent (plane_pon.transform);
			go.transform.localScale = Vector3.one;
        }

        public void handleWin(int mahID)
        {
            //Debug.LogError ("[s] handleWin(" + mahID + ")");
            string amahname = string.Empty;
            amahname = Mahjong.getName(mahID);
            this.state = PLAYERSTATE.WAITING;
            int[] param = { this.ID, mahID };
			photonView.RPC("WinPai", PhotonTargets.All, param);
        }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public void HideMenu()
        {
			//Debug.Log ("[c] HideMenu()");
			if(btnPon)
            	btnPon.gameObject.SetActive(false);
			if(btnWin)
            	btnWin.gameObject.SetActive(false);
			if(btnGang)
				btnGang.gameObject.SetActive(false);
			if(btnPass)
				btnPass.gameObject.SetActive(false);
			if(btnPon)
				btnPon.transform.parent.parent.gameObject.SetActive (false);
        }

		public IEnumerator doAiThink(int mahID)
		{
			Debug.LogError (this.name+".doAiThink(id="+mahID+")");
			//yield return new WaitForSeconds(1.0f);
			if (this.state == PLAYERSTATE.WAITING) {
				Debug.Log (this.name+"防止自打自碰");
				yield break;
			}
			bool iscan = AICheckPai(mahID);
			bool isCanWin = false;
			bool isCanPon = false;
			bool isCanGan = false;
			bool isCanChi = false;
			int chitype = 0;
			if (iscan) {
				//有的吃就吃 有的槓就槓
				if (mahID > 0 && this.state== PLAYERSTATE.PLAYING) {
					isCanWin = MahJongTools.IsCanHU (keepedMah, mahID);
					isCanPon = MahJongTools.IsCanPon (keepedMah, mahID);
					isCanGan = MahJongTools.IsCanGan (keepedMah, mahID, false, ponMah);
					isCanChi = MahJongTools.IsCanChi (keepedMah, mahID, out chitype, false);
					if (isCanWin)
						AskWin ();
					else if (isCanGan)
						AskGan ();
					else if (isCanPon)
						AskPon ();
					else if (isCanChi)
						AskChi ();
				}
				//Debug.LogError (" isCanGan="+isCanGan+", isCanPon="+isCanPon+", isCanChi="+isCanChi+")");
			} else {
				if(this.state==PLAYERSTATE.PLAYING)
					doMoPai();
			}

			yield return new WaitForSeconds(1.0f);

			isCanWin = false;
			isCanPon = false;
			isCanGan = false;
			isCanChi = false;
			iscan = false;
			int curr = 0;


			iscan = AICheckPai(this.moMahId);
			if (iscan) {
				isCanWin = MahJongTools.IsCanHU (keepedMah, this.moMahId);
				isCanPon = MahJongTools.IsCanPon (keepedMah, this.moMahId);
				isCanGan = MahJongTools.IsCanGan (keepedMah, this.moMahId, false, ponMah);
				isCanChi = MahJongTools.IsCanChi (keepedMah, this.moMahId, out chitype, false);
				if (isCanWin) {
					GameManager.Instance.doHandleWin (this.ID);
				} else {
					curr = this.keepedMah.Count - 1;
					//Debug.Log ("curr="+curr+", keepedMah.Count="+this.keepedMah.Count);
					while (curr >= 0 && curr < this.keepedMah.Count && AICheckPai (this.keepedMah [curr])) {
						if (curr < 0) {
							//curr = this.keepedMah.Count - 1;
							break;
						}
						curr--;
						//Debug.Log ("curr="+curr+", keepedMah.Count="+this.keepedMah.Count);
					}
					if (curr < 0) {
						curr = this.keepedMah.Count - 1;
					}
					//Debug.Log ("curr="+curr);
				}
			} else {
				curr = this.keepedMah.Count - 1;
			}
			if (this.state == PLAYERSTATE.PLAYING) {//如果是什麼都沒做, 先摸牌 再打牌
				GameManager.Instance.doHandleDaPai(this.ID, this.keepedMah [curr]);
			} else if (this.state == PLAYERSTATE.MOPAING) {//如果是摸完牌, 直接打牌
				//curr = this.keepedMah.Count - 1;
				GameManager.Instance.doHandleDaPai(this.ID, this.keepedMah [curr]);
			} else if (this.state == PLAYERSTATE.CHIPAING) {//如果是吃牌狀態, 直接打牌
				//curr = this.keepedMah.Count - 1;
				GameManager.Instance.doHandleDaPai(this.ID, this.keepedMah [curr]);
			} else if (this.state == PLAYERSTATE.PONPAING) {//如果是碰牌狀態, 直接打牌
				//curr = this.keepedMah.Count - 1;
				this.handleDaPai (this.keepedMah [curr]);
			} else if (this.state == PLAYERSTATE.GANPAING) {//如果是槓牌狀態, 直接打牌
				//curr = this.keepedMah.Count - 1;
				GameManager.Instance.doHandleDaPai(this.ID, this.keepedMah [curr]);
			}

			this.state = PLAYERSTATE.WAITING;//更改狀態為完成狀態

		}

		public bool AICheckPai(int mahID)
		{
			if (mahID == 0)
				return false;
			//Debug.Log ("AICheckPai("+pai_id+")");
			bool iscan = false;
			bool isCanWin = false;
			bool isCanPon = false;
			bool isCanGan = false;
			bool isCanChi = false;
			int chitype = 0;
			if (mahID > 0) {
				isCanWin = MahJongTools.IsCanHU (keepedMah, mahID);
				isCanPon = MahJongTools.IsCanPon (keepedMah, mahID);
				isCanGan = MahJongTools.IsCanGan (keepedMah, mahID, false, ponMah);
				isCanChi = MahJongTools.IsCanChi (keepedMah, mahID, out chitype, false);
			}
			iscan = isCanWin || isCanPon || isCanGan || isCanChi;
			//Debug.Log ("AICheckPai("+mahID+", iscan="+iscan+")");
			return iscan;
		}

		public void doMoPai()
		{
			this.moMahId = GameManager.Instance.getMahjongPai ();
			int remain = GameManager.Instance.getRemainPai ();
			handleMoPai (moMahId, remain);
		}

		public void sortPai()
		{
			for (int i = plane_keep.transform.childCount - 1; i >= 0; i--) {
				Destroy(plane_keep.transform.GetChild(i).gameObject);
			}
		}
    }
}