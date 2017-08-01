using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace com.Desktop
{
	public class GameManager : MonoBehaviour
    {
        private static GameManager _instance = null;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 所有玩家的集合
        /// </summary>
		public List<MahPlayer> Users = new List<MahPlayer>();

		public List<int> AbanMjs = new List<int>();
		//public List<MahPlayer> users;

		//public PhotonPlayer xlocalPlayer;

        public GameObject[] panels;

        /// <summary>
        /// 麻将牌
        /// </summary>
        public Mahjong mahJong;

        //通讯插件
        public PhotonView photonView;

        //吃碰牌特效
        public Nagieffect nagiEffectPlayerA;
        public Nagieffect nagiEffectPlayerB;

        //畫面過場控制
		public RectTransform AllCanvas;
		public RectTransform VideoCanvas;
		public GameObject GameInfo;
		public RectTransform abanPos;
		public Button InvateBtn;
		public Text Online;
		public Image OverPanel;
		public GameObject PanelInvate;
		public Text ChatText;
        //public Animator AllCanvasAnim; // 過場移入
        //public Animator GameInfoAnim;  //局風顯示

        #region 
        /// <summary>
        /// 骰子数
        /// </summary>
        private int diceNum;

        /// <summary>
        /// 牌面上被打出去的牌
        /// </summary>
        //public MahJongObject abandonMah;
		private int _lastDaPai = 0;
		//private int moMahId = 0;
		private int _lastMoPaiPlayerID = -1;
		private int _lastDaPaiPlayerID = -1;
		private int MaxWaitTime = 15;
		private int _remainTime = 0;
		private string _gameVersion = "1.0";
		private int _targetid = 0;
		private int _currentIndex = 0;
		private bool _isgameover = false;

		private MahPlayer _activePlayer;
        #endregion

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                //DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

		/*
		private void Connect()
		{
			//isConnecting = true;
			//setProcess (0.1f);
			//已經連接上了服務器
			if (PhotonNetwork.connected)
			{
				//setProcess (1.0f);
				//Debug.Log("Connected");
				//if (loadingPanel) {
				//	loadingPanel.SetActive (false);
				//}
			}
			else
			{
				//setProcess (0.7f);
				PhotonNetwork.ConnectUsingSettings(_gameVersion);
				//我们不加入大厅 这里不需要得到房间列表所以不用加入大厅去
				PhotonNetwork.autoJoinLobby = true;

				//#关键
				//这里保证所有主机上调用 PhotonNetwork.LoadLevel() 的时候主机和客户端能同时进入新的场景
				PhotonNetwork.automaticallySyncScene = true;
			}
		}
		*/

		/*
		/// <summary>
		/// 成功连接到大厅
		/// </summary>
		public override void OnConnectedToPhoton()
		{
			//Debug.Log ("OnConnectedToPhoton()");
			base.OnConnectedToPhoton();
			//setProcess (1.0f);
			//if (loadingPanel) {
			//loadingPanel.SetActive (false);
			//}
		}

		public override void OnJoinedLobby()
		{
			//Debug.Log ("OnJoinedLobby()");
			if (PhotonNetwork.room == null) {
				
				if (PhotonNetwork.CreateRoom(PhotonNetwork.playerName, new RoomOptions { MaxPlayers = 100 }, null))
				{
					//Debug.Log("CreateRoom() 成功 ! roomname="+PhotonNetwork.playerName);
					//StartCoroutine(ChangeRoom());
				}
			}
		}
		private void OnFailedToConnect(NetworkConnectionError error)
		{
			//Debug.Log("fail to Connect");
		}
		*/

        void Start()
        {
			//AudioManager.Instance.PlayBGM ("BGM_Playing");
            //只有主机有发牌的权利
			if (!PhotonNetwork.connected)
			{

				// We must be connected to a photon server! Back to main menu
				//Application.LoadLevel(Application.loadedLevel - 1);
				PhotonNetwork.LoadLevel("02.Lobby");
				return;
				//Connect();
			}

			if (PanelInvate != null) {
				PanelInvate.SetActive (false);
			}

			//Debug.Log (PhotonNetwork.room);
			if (PhotonNetwork.room != null) {
				string name = PhotonNetwork.room.Name;
				if (ChatText != null) {
					ChatText.text += "房間:"+name+"\n";
				}
				//string name = PhotonNetwork.room.Name;
				string url = "rtmp://catpunch.co/live/" + name;
				if (!PhotonNetwork.isMasterClient) {
					Debug.LogError ("[s] !PhotonNetwork.isMasterClient");
					VideoRecordingBridge.StartPlay (url);
					if (InvateBtn != null) {
						InvateBtn.gameObject.SetActive (true);
					}
					return;
				}
				VideoRecordingBridge.StartRecord (url);
				if (InvateBtn != null) {
					InvateBtn.gameObject.SetActive (false);
				}
			}

			AudioManager.Instance.PlayBGM ("BGM_Playing");
            //Debug.Log ("Start()");
        }

        // 邀請玩麻將
        public void ClickInvatePlay() {
			if (PhotonNetwork.playerList.Length < 2) {
				Debug.Log ("兩個人以上才能開桌");
				return;
			}
			//LayoutStart(); //畫面移入
			//this._isgameover = false;
            //只有副机有邀請的權利
            //if (PhotonNetwork.isMasterClient)
            //{
			//	Debug.Log ("!PhotonNetwork.isMasterClient");
            //    return;
            //}
			//只有主机有发牌的权利
			//StartCoroutine("ReadyPlay");

			photonView.RPC("InvatePlayMJ", PhotonTargets.MasterClient, null);

        }

		[PunRPC]
		private void InvatePlayMJ()
		{
			//StartCoroutine("ReadyPlay");
			if (PanelInvate != null) {
				PanelInvate.SetActive (true);
			}
		}

		[PunRPC]
		private void ExitGame()
		{
			LayoutEnd ();
		}

		public void InvateYES()
		{
			if (PanelInvate != null) {
				PanelInvate.SetActive (false);
			}
			StartCoroutine("ReadyPlay");
		}

		public void InvateNO()
		{
			if (PanelInvate != null) {
				PanelInvate.SetActive (false);
			}
		}

        IEnumerator ReadyPlay() {
            yield return new WaitForSeconds(1.0f);

			//0.Game init
			doGameInit();

            //1.洗牌
            doShuffleMah();

            //2.發牌
			doDispatchPai();

            //4.設定開始玩家
			MahPlayer next = Users[_currentIndex];
			StartCoroutine( setActivePlayer (next));
        }

        public void onLivePlayREConnect()
        {
            VideoRecordingBridge.REConnect();
        }


		public void OnDisconnectedFromPhoton()
		{
			Debug.LogError("OnDisconnectedFromPhoton");

			// Back to main menu        
			//Application.LoadLevel(Application.loadedLevelName);
		}

		public void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			Debug.Log("OnPhotonPlayerConnected: " + player.NickName);
			if (Online != null) {
				Online.text = PhotonNetwork.playerList.Length.ToString();
			}
			if (ChatText != null) {
				ChatText.text += player.NickName+"進入這個房間\n";
			}
		}

		public void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			if (ChatText != null) {
				ChatText.text += player.NickName+"離開這個房間\n";
			}
			int ppl =  PhotonNetwork.playerList.Length;
			photonView.RPC ("BroadcastOnlinePpl", PhotonTargets.All, ppl);
		}

        private void doShuffleMah()
        {
			//Debug.LogError ("[s] 1.doShuffleMah()");
            mahJong.ShuffleMah();
        }

        /// <summary>
        /// 绑定玩家与photonplayer
        /// </summary>
        private void doGameInit()
        {
			Debug.LogError ("[s] 0.doGameInit()");
			int[] ids = new int[2];
			//Debug.Log ("playerList.Length="+PhotonNetwork.playerList.Length);
			if (PhotonNetwork.playerList.Length > 1) {
				
				//對戰模式 第一個是自己 第二個是對戰玩家
				int i = 0;
				foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
					if (pp != PhotonNetwork.player) {
						Users [1].ID = pp.ID;
						Users [1].photonPlayer = pp;
						Users [1].isAI = false;
						Users [1].autoPlay = false;
						ids [0] = pp.ID;
						//break;
					} else {
						Users [0].ID = PhotonNetwork.player.ID;
						Users [0].photonPlayer = PhotonNetwork.player;
						Users [0].isAI = false;
						Users [0].autoPlay = false;
						ids [1] = PhotonNetwork.player.ID;
					}
					i++;
				}
			}

			_currentIndex = 0;
			this._isgameover = false;
			//Debug.Log (ids);
			photonView.RPC("BundlePlayer", PhotonTargets.Others, ids);
        }

        /// <summary>
        /// 同步玩家列表
        /// </summary>
        /// <param name="ids"></param>
        [PunRPC]
        public void BundlePlayer(int[] ids)
        {
			Debug.LogError ("[RPC] BundlePlayer("+ids.Length+")");
			Debug.Log (ids);
			int i = 0;
			foreach (int id in ids) {
				Users [i].ID = id;
				Users [i].photonPlayer = PhotonPlayer.Find(id);
				Users [i].isAI = false;
				Users [i].autoPlay = false;
				i++;
			}
        }

		[PunRPC]
		private void BroadcastOnlinePpl(int ppl)
		{
			if (Online != null) {
				Online.text = ppl.ToString();
			}
		}

        /// <summary>
        /// 摇骰子
        /// </summary>
        void doDice()
        {
			//Debug.LogError ("[s] 2.doDice()");
            int a = UnityEngine.Random.Range(1, 6);
            int b = UnityEngine.Random.Range(1, 6);

            diceNum = a + b;
			//Debug.LogError ("[s] 2.doDice("+diceNum+")");
        }

        /// <summary>
        /// 发牌
        /// </summary>
		void doDispatchPai()
        {
			Debug.LogError ("[s] 2.doDispatchPai(玩家數="+PhotonNetwork.playerList.Length+")");
			//List<int> mahm = new List<int>();
			AbanMjs.Clear ();
			LayoutStart();
			int i = 0;
			foreach (MahPlayer mp in Users) {
				if (mp == null)
					continue;
				mp.keepedMah.Clear ();
				mp.ponMah.Clear ();
				mp.abandanedMah.Clear ();
				//mahm.Clear ();
				List<int> mahm = new List<int>();
				for (int j = 0; j < Mahjong.MAXPAI; j++)
				{
					int pai_id = mahJong.allMah.Dequeue();
					mahm.Add(pai_id);
				}
				mp.clearPAI ();
				mp.keepedMah = mahm;
				mp.ShowPAI ();
				if(PhotonNetwork.player.ID!=mp.ID) {
					//	photonView.RPC ("dispatchPai", PhotonTargets.MasterClient, i, mahm.ToArray ());
					//else
					object[] param = { mp.ID, mahm.ToArray() };
					photonView.RPC ("dispatchPai", PhotonTargets.Others, param);
				}
				//i++;
			}
        }

		private void doHandleNext() {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log("Server Exec Only!! doHandleNext()");
				return;
			} 
			//Debug.LogError("[s] doHandleNext()");
			_currentIndex++;
			_currentIndex = _currentIndex % Users.Count;
			//Debug.LogError ("[s] doHandleNext()");
			//StartCoroutine (doHandleNextCo(time));
			MahPlayer next = Users[_currentIndex]; //_activePlayer.nextPlayer;
			StartCoroutine( setActivePlayer (next));
		}

		//處理玩家碰牌
		public void doHandlePon (int playerid) {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log ("Server Exec Only!! doHandlePon()");
				return;
			} else {
				if (_lastDaPai != 0) {
					MahPlayer mplayer =  Users.Find (x => x.ID.Equals (playerid));
					if (mplayer) {
						mplayer.handlePon (_lastDaPai);
					}
				}
			}
		}

		//處理要求代打
		public void doHandleAutoPlay (int playerid, bool auto) {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log ("Server Exec Only!! doHandlePon()");
				return;
			} else {
				//Debug.Log ("doHandleAutoPlay("+playerid+", "+auto+")");
				if (playerid >= 0) {
					MahPlayer mplayer =  Users.Find (x => x.ID.Equals (playerid));
					if (mplayer) {
						mplayer.handleAskAutoPlay (auto);
					}
				}
			}
		}

		//處理玩家槓牌
		public void doHandleGan (int playerid) {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log ("Server Exec Only!! doHandleGan()");
				return;
			} else {
				//Debug.Log ("doHandleGan(");
				if (_lastDaPai != 0) {
					MahPlayer mplayer =  Users.Find (x => x.ID.Equals (playerid));
					if (mplayer) {
						mplayer.handleGan (_lastDaPai);
						int GotID = getMahjongPai ();
						if (GotID <= 0) {
							photonView.RPC ("GameOver", PhotonTargets.All, null);
							return;
						}
						//Debug.Log ("mopai"+GotID);
						string name = Mahjong.getName (GotID);
						_lastMoPaiPlayerID = _activePlayer.ID;
						_activePlayer.handleMoPai (GotID, getRemainPai());
					}
				}
			}
		}

		public void doHandleChi (int playerid) {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log ("Server Exec Only!! doHandleChi()");
				return;
			} else {
				if (_lastDaPai != 0) {
					MahPlayer mplayer =  Users.Find (x => x.ID.Equals (playerid));
					if (mplayer) {
						mplayer.handleChi (_lastDaPai);
					}
				}
			}
		}

        //處理玩家胡牌
        public void doHandleWin(int playerid)
		{
			//Debug.Log ("doHandleWin("+playerid+")");
            if (!PhotonNetwork.isMasterClient)
            {
                //Debug.Log ("Server Exec Only!! doHandledWin()");
                return;
            }
            else
            {
				this._isgameover = true;
				//Debug.Log ("abanMahId="+abanMahId);
                //if (abanMahId != 0)
                //{
				MahPlayer mplayer =  Users.Find (x => x.ID.Equals (playerid));
				if (mplayer) {
					mplayer.handleWin (_lastDaPai);
				}
                //}
            }
        }

        public void doHandleDaPai(int playerid, int mid) {
			//Debug.LogError ("[s] doHandleDaPai("+playerid+", "+mid+")");
			if (!PhotonNetwork.isMasterClient) {
				//Debug.LogError ("Server Exec Only!! doHandleDaPai()");
				return;
			} else {
				if (mid != 0) {
					if (_activePlayer != null && _activePlayer.ID==playerid && _activePlayer.ID != _lastDaPaiPlayerID) {
						_lastDaPaiPlayerID = _activePlayer.ID;
						_lastDaPai = mid;
						_activePlayer.handleDaPai (mid);
					}
				}
			}
		}

        //摸牌
		public void doHandleMoPai()
		{
			Debug.Log ("GameManager.doHandleMoPai()");
			//yield return new WaitForSeconds(isfirst);
			StartCoroutine(_MoPaiCo(0));
		}

		public IEnumerator _MoPaiCo(float time = 0.1f) {

			if (time > 0) {
				yield return new WaitForSeconds (time);
			}

			if (!PhotonNetwork.isMasterClient) {
				//Debug.LogError ("Server Exec Only");
				yield break;
			} else {
				//Debug.LogError ("[s] 4.doMoPai()");
				//players[0].GetMahJong(true);
				if (_activePlayer != null && _activePlayer.ID != _lastMoPaiPlayerID) {
					//_activePlayer.state = PLAYERSTATE.MOPAING;//更改為摸牌狀態
					//Debug.LogError ("[s] 4.doMoPai("+_activePlayer.photonPlayer.NickName+")");
					//Debug.Log("xxxxxx_activePlayer.state="+_activePlayer.state+" "+_activePlayer.photonPlayer.NickName);
					int GotID = getMahjongPai ();
					if (GotID <= 0) {
						photonView.RPC ("GameOver", PhotonTargets.All, null);
						yield break;
					}
					//Debug.Log ("mopai"+GotID);
					string name = Mahjong.getName (GotID);
					_lastMoPaiPlayerID = _activePlayer.ID;
					//keepedMah.Add (GotID);
					//Debug.LogError ("[s] 4.doMoPai(GotID="+GotID+", isfirst="+isfirst+")");
					//Debug.LogError ("[s] 4. doMoPai("+_activePlayer.photonPlayer.NickName+", GotID="+GotID+")");
					_activePlayer.handleMoPai (GotID, getRemainPai());
				}
			}
        }

		private IEnumerator setActivePlayer(MahPlayer player) {
			//Debug.LogError ("[s] 輪到("+player.name+")");
			//int[] param = { photonPlayer.ID };
			//photonView.RPC("ShowTimer", PhotonTargets.All, param);
			MahPlayer _prevPlayer = _activePlayer;
			//_remainTime = 0;
			if (_prevPlayer!=null) {
				yield return StartCoroutine( _prevPlayer.doAiThink (this._lastDaPai));
				//_prevPlayer.state = PLAYERSTATE.WAITING;//更改狀態為完成狀態
			}
			_activePlayer = player;
			_activePlayer.actived = false;
			_activePlayer.state = PLAYERSTATE.PLAYING;
			int[] param = { _activePlayer.ID };
			photonView.RPC("ShowActivePlayer", PhotonTargets.All, param);
			_lastMoPaiPlayerID = -1;
			_lastDaPaiPlayerID = -1;
			yield return StartCoroutine (AutoChangeActivePlayerCo ());
		}

		private IEnumerator AutoDaPai()
		{
			if (_activePlayer != null) {
				if (this._isgameover)
					yield break;
				yield return StartCoroutine( _activePlayer.doAiThink (this._lastDaPai));
				//yield return new WaitForSeconds (1);
				if (getRemainPai()> 0) {
					doHandleNext ();
				} else {
					photonView.RPC ("GameOver", PhotonTargets.All, null);
				}
			}
		}

		private IEnumerator AutoChangeActivePlayerCo()
		{
			if (this._isgameover)
				yield break;
			_remainTime = MaxWaitTime;
			bool iscan = false;
			if(_activePlayer != null)
				iscan = _activePlayer.AICheckPai(this._lastDaPai);
			while (_remainTime>0) {
				if (this._isgameover)
					yield break;
				yield return new WaitForSeconds (1);
				_remainTime--;
				int player_id = Users [_currentIndex].ID;
				int[] param = { _remainTime, player_id };
				photonView.RPC ("RemainTime", PhotonTargets.All, param);
				if (_activePlayer != null ) {
					if (_activePlayer.isAI || _activePlayer.autoPlay) {
						StartCoroutine (AutoDaPai ());//自動代打
						yield break;
					} else {
						if (!iscan && _activePlayer.state == PLAYERSTATE.PLAYING) {
							_activePlayer.doMoPai ();
						}
					}
				}
			}
			//if (_activePlayer != null && mahJong.allMah.Count > 0) {
			if (getRemainPai() > 0) {
				doHandleNext ();
			} else {
				photonView.RPC ("GameOver", PhotonTargets.All, null);
			}
			//StopCoroutine ("AutoChangeActivePlayerCo");
		}

		public int getMahjongPai()
		{
			int remain = getRemainPai ();

			//牌光了 游戏结束
			if (remain == 0)
			{
				//photonView.RPC("GameOver", PhotonTargets.All, null);
				return -1;
			}
			int ID = -1;
			ID = mahJong.allMah.Dequeue ();
			//Debug.LogError ("getMahjongPai(id="+ID+", remain="+remain+")");
			return ID;
		}

		public int getRemainPai()
		{
			int remain = 0;
			if (mahJong != null && mahJong.allMah != null) {
				remain = mahJong.allMah.Count;
			}
			return remain;
		}

		[PunRPC]
		private void SendRequestToServer(object[] param)
		{
			//Debug.Log ("SendRequestToServer()");
			int gcmd = (int)param[0];
			int player_id = -1;
			switch(gcmd) {
			case (int)GameCommand.AUTOPLAY:
				player_id = (int)param[1];
				bool auto = (bool)param[2];
				doHandleAutoPlay (player_id, auto);
				break;
			case (int)GameCommand.MOPAI:
				player_id = (int)param [1];
				if (player_id == _activePlayer.photonPlayer.ID) {
					doHandleMoPai ();
				}
				break;
			case (int)GameCommand.PONPAI:
				player_id = (int)param[1];
				doHandlePon (player_id);
				break;
			case (int)GameCommand.GANPAI:
				player_id = (int)param[1];
				doHandleGan(player_id);
				break;
			case (int)GameCommand.CHIPAI:
				player_id = (int)param[1];
				doHandleChi(player_id);
				break;
			case (int)GameCommand.DAPAI:
				player_id = (int)param[1];
				int mid = (int)param[2];
				doHandleDaPai(player_id, mid);
				break;
            case (int)GameCommand.WINPAI:
                player_id = (int)param[1];
				//bool xx = (bool)param[2];
                doHandleWin(player_id);
                break;
            }
		}

        [PunRPC]
        private void GameOver()
        {
			//Debug.LogError ("[RPC] 遊戲結束()");
            OverPanel.gameObject.SetActive(true);
			AbanMjs.Clear ();
			if (ChatText != null) {
				ChatText.text += "遊戲結束\n";
			}
        }
		[PunRPC]
		private void SetupAI(object[] param)
		{
			int player_id = (int)param[0];
			bool en = (bool)param[1];
			Debug.LogError ("[RPC] SetupAI(pid="+player_id+", en="+en+")");
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer != null) {
				mplayer.autoPlay = en;
			}
		}

		[PunRPC]
		private void dispatchPai(object[] param)
		{
			LayoutStart();
			int player_id = (int)param[0];
			int[] content = (int[])param[1];
			Debug.LogError ("[RPC] 發牌給pid="+player_id+", content.length="+content.Length+" ");
			int i = 0;
			AbanMjs.Clear ();
			int[] init0 = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			foreach (MahPlayer mp in Users) {
				mp.clearPAI ();
				if (mp.ID == player_id) {
					mp.keepedMah = new List<int>(content);
				} else {
					mp.keepedMah = new List<int> (init0);
				}
				mp.ShowPAI ();
			}
		}
		
		[PunRPC]
		private void RemainTime(int[] param)
		{
			int rtime = (int)param[0];
			int player_id = (int)param[1];
			//Debug.LogError ("[RPC] idx="+idx+", RemainTime=" + rtime);
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer != null) {
				mplayer.timer.time = rtime;
				mplayer.timer.Show (0.9f);
			}
		}
        [PunRPC]
		void ShowActivePlayer(int[] param)
        {
			int player_id = (int)param[0];
			//Debug.LogError ("[RPC] 輪到玩家(id="+player_id+")");
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer!=null && !(mplayer.isAI||mplayer.autoPlay)) {
				mplayer.checkPai (_lastDaPai, false);
				mplayer.timer.time = MaxWaitTime;
				mplayer.timer.Show (0.9f);
			}
        }
		/// <summary>
		/// 摸牌.
		/// </summary>
		/// <param name="param">Parameter.</param>
		[PunRPC]
		void MoPai(int[] param)
		{
			int player_id = (int)param[0];
			int pai_id = (int)param[1];
			int remain_pai_count = (int)param[2];
			int i = 0;
			int j = 0;
			string pname = Mahjong.getName (pai_id);

			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer) {
				if (mplayer.ID == PhotonNetwork.player.ID) {
					if (!PhotonNetwork.player.IsMasterClient) {
						mplayer.keepedMah.Add (pai_id);
					}
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + mplayer.name + "摸牌 " + pname + "(" + i + "+" + j + ")");
					mplayer.createPaiToMo (pai_id);
				} else {
					//mplayer.keepedMah.Add (0);
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + mplayer.name + "摸牌 " + pname + "(" + i + "+" + j + ")");
					mplayer.createPaiToMo (0);
				}
				//Debug.Log("剩餘 "+remain_pai_count+"");
				mplayer.HideMenu ();
			}
		}

		/// <summary>
		/// 出牌.
		/// </summary>
		/// <param name="param">Parameter.</param>
		[PunRPC]
		void DaPai(int[] param)
		{
			int player_id = (int)param[0];
			int pai_id = (int)param[1];
			int i = 0;
			int j = 0;
			string pai_name = Mahjong.getName (pai_id);
			AbanMjs.Add (pai_id);
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer) {
				if (mplayer.ID == PhotonNetwork.player.ID) {
					if (!PhotonNetwork.player.IsMasterClient) {
						mplayer.keepedMah.Remove (pai_id);
					}
					mplayer.DaPaiToAban (pai_id, AbanMjs.Count);
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "出牌 " + pai_name + "(" + i + "+" + j + ")");
					mplayer.HideMenu ();
					Speak (pai_id);
				} else {
					if (!PhotonNetwork.player.IsMasterClient) {
						mplayer.keepedMah.RemoveAt (0);
					}
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "出牌 " + pai_name + "(" + i + "+" + j + ")");
					mplayer.DaPaiToAban (pai_id, AbanMjs.Count);
					Speak (pai_id);
				}
				playDaiPaiSound ();
			}

			//abanMahId = pai_id;
			this._remainTime = 1;
		}

		/// <summary>
		/// 碰牌
		/// </summary>
		/// <param name="param">Parameter.</param>
		[PunRPC]
		void PonPai(int[] param)
		{
			int player_id = (int)param[0];
			int pai_id = (int)param[1];
			int i = 0;
			int j = 0;
			string pai_name = Mahjong.getName (pai_id);
			AbanMjs.Remove (pai_id);
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer) {
				//mplayer.collectPonPai (pai_id);
				if (mplayer.ID == PhotonNetwork.player.ID) {
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] 你(" + mplayer.name + ")喊了碰 " + pai_name + "(" + i + "+" + j + ")");
					playPonSound ();
					mplayer.HideMenu ();
					nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.PON);
				} else {
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "喊了碰 " + pai_name + "(" + i + "+" + j + ")");
					playPonSound ();
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PON);
				}
				mplayer.collectPonPai (pai_id);
			}
		}

		/// <summary>
		/// 槓牌
		/// </summary>
		/// <param name="param">Parameter.</param>
		[PunRPC]
		void GanPai(int[] param)
		{
			int player_id = (int)param[0];
			int pai_id = (int)param[1];
			int i = 0;
			int j = 0;
			string pai_name = Mahjong.getName (pai_id);
			AbanMjs.Remove (pai_id);
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer) {
				mplayer.collectGanPai (pai_id);
				if (mplayer.ID == PhotonNetwork.player.ID) {
					mplayer.keepedMah.Remove (pai_id);
					mplayer.keepedMah.Remove (pai_id);
					mplayer.keepedMah.Remove (pai_id);
					mplayer.ponMah.Add (pai_id);
					mplayer.ponMah.Add (pai_id);
					mplayer.ponMah.Add (pai_id);
					mplayer.ponMah.Add (pai_id);
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "喊槓 " + pai_name + "(" + i + "+" + j + ")");
					playGanSound ();
					mplayer.HideMenu ();
					nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.GAN);
				} else {
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "喊槓 " + pai_name + "(" + i + "+" + j + ")");
					playGanSound ();
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.GAN);
				}
				//mplayer.collectGanPai (pai_id);
			}
		}

		/// <summary>
		/// 吃牌
		/// </summary>
		/// <param name="param">Parameter.</param>
		[PunRPC]
		void ChiPai(int[] param)
		{
			int player_id = (int)param[0];
			int pai_id = (int)param[1];
			int chitype = (int)param[2];
			int i = 0;
			int j = 0;
			string pai_name = Mahjong.getName (pai_id);
			AbanMjs.Remove (pai_id);
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer) {
				mplayer.collectChiPai (pai_id, chitype);
				if (mplayer.ID == PhotonNetwork.player.ID) {
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "喊吃 " + pai_name + "(" + i + "+" + j + ")");
					playChiSound ();
					mplayer.HideMenu ();
					nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.CHI);
				} else {
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "喊吃 " + pai_name + "(" + i + "+" + j + ")");
					playChiSound ();
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.CHI);
				}
			}
		}


        [PunRPC]
        void WinPai(int[] param)
        {
			//Debug.Log ("[RPC] 贏牌()");
			int player_id = (int)param[0];
			AbanMjs.Clear ();
            Text text = OverPanel.GetComponentInChildren<Text>();
			//text.text = "玩家" + photonPlayer.NickName + " 胡牌了!";
            //OverPanel.gameObject.SetActive(true);
            //nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PAU);
            StartCoroutine(_OverPanel(5f));

			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			text.text = "玩家" + mplayer.name + " 胡牌了!";
			if (mplayer) {
				if (mplayer.ID == PhotonNetwork.player.ID) {
					//Debug.LogError ("[RPC] 你贏了 ");
					playWinSound ();
					nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.HU2);
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PAU);
				} else {
					playWinSound ();
					nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.PAU);
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.HU);
				}
			}
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(param[0]);
            text.text = "玩家" + photonPlayer.NickName + " 胡牌了!";
			if (photonPlayer.IsLocal) {
				//Debug.LogError ("[RPC] 你贏了 ");
				playWinSound ();
                nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.HU2);
                nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PAU);
            } else {
				//Debug.LogError ("[RPC] "+photonPlayer.NickName+"贏了 ");
				playWinSound ();
                nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.PAU);
                nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.HU);
            }
            */
        }

        private IEnumerator _OverPanel(float time = 0.1f)
        {
            if (time > 0)
            {
                yield return new WaitForSeconds(time);
            }

            OverPanel.gameObject.SetActive(true);
        }

        /*
        private void OnEvent(byte eventcode, object content, int senderid)
        {
			if (eventcode == (byte)GameCommand.ONEMORETIME)
            {
				//Debug.LogError ("OnEvent(GameCommand.ONEMORETIME)");
                OneMoreMahjong();
            }
        }
        */

        public void OneMoreMahjong()
        {
			Debug.Log ("OneMoreMahjong()");
            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
                Text text = OverPanel.GetComponentInChildren<Text>();
                text.text = "等待房主确认...";

                return;
            }
			//Debug.Log ("[s] OneMoreMahjong()");

            photonView.RPC("OneMore", PhotonTargets.All, null);
        }

        [PunRPC]
        private void OneMore()
        {
			//Debug.LogError ("[RPC] OneMore()");
            OverPanel.gameObject.SetActive(false);

            RestView();

            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
                return;
            }

			StartCoroutine("ReadyPlay");
        }

        private void RestView()
        {
			//Debug.Log ("[s] RestView("+panels.Length+")");
            foreach (GameObject p in panels)
            {
				//Debug.Log ("p="+p.gameObject.name);
                MahJongObject[] m_p = p.GetComponentsInChildren<MahJongObject>();
				//Debug.Log ("m_p.length="+m_p.Length);
                foreach (MahJongObject m_o in m_p)
                {
					//Debug.Log ("m_o="+m_o.gameObject.name);
					DestroyImmediate(m_o.gameObject);
                }
            }
        }

        public void Back()
        {
			//Debug.Log ("Back()");
			VideoRecordingBridge.StopRecord ();
			VideoRecordingBridge.StopPlay ();
            PhotonNetwork.LeaveRoom();
            //SceneManager.LoadScene("Game");
        }

		public void OnLeftRoom()
		{
			//Debug.Log("OnLeftRoom (local)");
			VideoRecordingBridge.StopRecord ();
			VideoRecordingBridge.StopPlay ();
			SceneManager.LoadScene("02.Lobby");
			// back to main menu        
			//Application.LoadLevel(Application.loadedLevelName);
		}

		public void Speak(int id) {
			//if (!PhotonNetwork.isMasterClient) {
			//	return;
			//}
			if (id <= 0)
				return;
			AudioManager.Instance.PlaySE ("g_"+id);
		}

		private void playDaiPaiSound() {
			//if (!PhotonNetwork.isMasterClient) {
			//	return;
			//}
			AudioManager.Instance.PlaySE ("da1");
		}

		private void playPonSound() {
			//if (!PhotonNetwork.isMasterClient) {
			//	return;
			//}
			AudioManager.Instance.PlaySE ("g_pon");
		}

		private void playChiSound() {
			//if (!PhotonNetwork.isMasterClient) {
			//	return;
			//}
			//AudioManager.Instance.PlaySE ("");
			AudioManager.Instance.PlaySE ("g_chi");
		}

		private void playGanSound() {
			//if (!PhotonNetwork.isMasterClient) {
			//	return;
			//}
			AudioManager.Instance.PlaySE ("g_gang");
		}

		private void playWinSound() {
			//if (!PhotonNetwork.isMasterClient) {
			//	return;
			//}
			AudioManager.Instance.PlaySE ("g_hu");
		}

		public void Mute()
		{
			AudioManager.Instance.Mute ();
		}

		//畫面移入
        private void LayoutStart()
        {
			//VideoCanvas.transform.DOScaleX (1, 0);
			AllCanvas.transform.DOMoveX (-14, 0, true);
			AllCanvas.transform.DOMoveX(0, 1, false).SetEase(Ease.InOutBack).OnComplete(ShowInfo);
			VideoRecordingBridge.MoveRight ();
			//VideoCanvas.transform.DOScaleX (0.3f, 1).SetEase(Ease.InOutBack);
            //Invoke("ShowInfo", 0.45f);
			//ShowInfo();
        }

		private void LayoutEnd()
		{
			AllCanvas.transform.DOMoveX (0, 0, true);
			AllCanvas.transform.DOMoveX (-14, 1, false).SetEase(Ease.InOutBack);
			VideoRecordingBridge.MoveLeft ();
		}

        public void ShowInfo()
        {
            //GameInfoAnim.SetTrigger("ShowInfo");
			//Debug.Log("ShowInfo()");
			CanvasGroup cg = GameInfo.GetComponent<CanvasGroup> ();
			if (cg != null) {
				cg.DOFade (1, 2);//.SetDelay(1).DOFade(0, 2);
				cg.DOFade (0, 2).SetDelay(1);
			}
        }

        public void GameStop()
        {
			photonView.RPC("ExitGame", PhotonTargets.All, null);
        }
    }
}