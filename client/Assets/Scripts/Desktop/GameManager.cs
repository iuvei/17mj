using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace com.Desktop
{
	public class GameManager : Photon.PunBehaviour
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
		public GameObject PanelAlert;
		public Text RoomUserName;
        public RectTransform SettingCanvas;
        public RectTransform _invitePlayPop;
        public Image VideoBG;
        //public Animator AllCanvasAnim; // 過場移入
        //public Animator GameInfoAnim;  //局風顯示

        private bool _readyWinAdd = false;
        private bool _readyLoseAdd = false;
        private string _readyWinAddresult = string.Empty;
        private string _readyLoseAddresult = string.Empty;

        private Color _colorAplha = new Color(0, 0, 0, 0);
        private Color _colorWhite = new Color(1, 1, 1, 1);

        //結算畫面UI
        private Image _winPhoto;
        private Text _winName;
        private Text _winLv;
        private Text _winCoin;
        private Image _losPhoto;
        private Text _losName;
        private Text _losLv;
        private Text _losCoin;

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
		private int MaxWaitTime = 100;
		private int _remainTime = 0;
		private string _gameVersion = "1.0";
		private int _targetid = 0;
		private int _currentIndex = 0;
		private bool _isgameover = false;

		private MahPlayer _activePlayer = null;
		private string liveUrl = string.Empty;
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


		/// <summary>
		/// 成功连接到大厅
		/// </summary>
		public override void OnConnectedToPhoton()
		{
			Debug.LogError (this.name+".OnConnectedToPhoton()");
			//base.OnConnectedToPhoton();
			//setProcess (1.0f);
			//if (loadingPanel) {
			//loadingPanel.SetActive (false);
			//}
		}


		public override void OnJoinedLobby()
		{
			Debug.LogError (this.name+".OnJoinedLobby()");
			//if (PhotonNetwork.room == null) {
			//	if (PhotonNetwork.CreateRoom(PhotonNetwork.playerName, new RoomOptions { MaxPlayers = 100 }, null))
			//	{
			//		//Debug.Log("CreateRoom() 成功 ! roomname="+PhotonNetwork.playerName);
			//		//StartCoroutine(ChangeRoom());
			//	}
			//}
		}


        private void InitUILayout() {
            if (OverPanel) {
                Transform _winnerP;
                Transform _loserP;
                _winnerP = OverPanel.transform.Find("Winner");
                _loserP = OverPanel.transform.Find("Loser");

                if (_winnerP) {
                    _winPhoto = _winnerP.transform.Find("outline/Photo").GetComponent<Image>();
                    _winName = _winnerP.transform.Find("Name").GetComponent<Text>();
                    _winLv = _winnerP.transform.Find("Lv").GetComponent<Text>();
                    _winCoin = _winnerP.transform.Find("Coin").GetComponent<Text>();
                }

                if (_loserP) {
                    _losPhoto = _loserP.transform.Find("outline/Photo").GetComponent<Image>();
                    _losName = _loserP.transform.Find("Name").GetComponent<Text>();
                    _losLv = _loserP.transform.Find("Lv").GetComponent<Text>();
                    _losCoin = _loserP.transform.Find("Coin").GetComponent<Text>();
                }
            }
        }

		private void OnFailedToConnect(NetworkConnectionError error)
		{
			Debug.LogError(this.name+" fail to Connect");
		}


		public override void OnJoinedRoom()
		{
			Debug.LogError ("OnJoinedRoom()");
			if (PhotonNetwork.room != null) {
				string name = PhotonNetwork.room.Name;
				if (ChatText != null) {
					ChatText.text += "房間編號: #"+name+"\n";
				}
				if (RoomUserName != null) {
					//PhotonNetwork.playerList
					PhotonPlayer mp = null;
					foreach(PhotonPlayer pp in PhotonNetwork.playerList) {
						if (pp.IsMasterClient) {
							mp = pp;
							break;
						}

					}
					Hashtable cp = PhotonNetwork.room.customProperties;
					string cname = (string)cp ["CRoomName"];
					RoomUserName.text = cname;
					if (ChatText != null) {
						ChatText.text += "房間名稱: "+cname+"的房間\n";
						ChatText.text += "你進入這個房間\n";
					}
				}
				//string name = PhotonNetwork.room.Name;

				liveUrl = "rtmp://17mj.ddns.net:9100/live/" + name;

				if (!PhotonNetwork.isMasterClient) {
					Debug.LogError ("[s] !PhotonNetwork.isMasterClient");
					#if UNITY_IOS || UNITY_ANDROID
					VideoRecordingBridge.StartPlay (liveUrl,name);
					#endif
					if (InvateBtn != null) {
						if (_invitePlayPop) //邀請搖晃動畫
							_invitePlayPop.DOScale(new Vector3(1.15f, 1.15f, 1), .8f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
						InvateBtn.gameObject.SetActive (true);
					}
					return;
				}
				#if UNITY_IOS || UNITY_ANDROID
				VideoRecordingBridge.StartRecord (liveUrl);
				#endif
				if (InvateBtn != null) {
					InvateBtn.gameObject.SetActive (false);
				}
			}
		}


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

			AudioManager.Instance.PlayBGM ("BGM_Playing");
            //Debug.Log ("Start()");

            InitUILayout();
        }

        // 邀請玩麻將
        public void ClickInvatePlay() {
			Debug.Log ("ClickInvatePlay()");
			if (PhotonNetwork.playerList.Length < 2) {
				Debug.Log ("兩個人以上才能開桌");
				return;
			}

			string name = PhotonNetwork.masterClient.NickName;
			ShowAlert ("你邀請房主["+name+"]一起玩麻將, 等待回應中...", 5.0f);

			int[] param = { (int)PhotonNetwork.player.ID };
			photonView.RPC("InvatePlayMJ", PhotonTargets.MasterClient, param);

        }

		[PunRPC]
		private void InvatePlayMJ(int[] param)
		{
			int player_id = (int)param [0];
			Debug.LogError ("[RPC] InvatePlayMJ(id="+player_id+")");
			List<PhotonPlayer> all = PhotonNetwork.playerList.ToList ();
			PhotonPlayer pplayer = all.Find(x => x.ID.Equals (player_id));
			string name = string.Empty;
			if (pplayer != null) {
				//mplayer.handlePon (_lastDaPai);
				name = pplayer.NickName;
				if (PanelInvate != null) {
					Text txt = PanelInvate.GetComponentInChildren<Text> ();
					txt.text = name;
					PanelInvate.SetActive (true);
				}
			}
		}

		[PunRPC]
		private void ExitGame()
		{
			Debug.LogError ("[RPC] ExitGame()");
			LayoutEnd ();
		}

		[PunRPC]
		private void GameStart()
		{
			Debug.LogError ("[RPC] GameStart()");
			LayoutStart();
			RestView ();
			if (OverPanel != null) {
				OverPanel.gameObject.SetActive (false);
			}
		}

		public void InvateYES()
		{
			//if (OverPanel != null) {
			//	OverPanel.gameObject.SetActive (false);
			//}
			if (PhotonNetwork.isMasterClient) {
				
				if (PanelInvate != null) {
					PanelInvate.SetActive (false);
				}
				//StopCoroutine ("ReadyPlay");
				StopAllCoroutines ();
				StartCoroutine ("ReadyPlay");
			}
		}

		public void InvateNO()
		{
			if (PanelInvate != null) {
				PanelInvate.SetActive (false);
			}
		}

        IEnumerator ReadyPlay() {
			//StopAllCoroutines ();
			bcGameStart ();
			//0.Game init
			doGameInit();
			yield return new WaitForSeconds(1.0f);

            //1.洗牌
            doShuffleMah();
			yield return new WaitForSeconds(1.0f);

            //2.發牌
			doDispatchPai();
			yield return new WaitForSeconds(1.0f);

            //4.設定開始玩家
			MahPlayer next = Users[_currentIndex];
			//yield return 
			StartCoroutine( setActivePlayer (next));
        }

        public void onLivePlayREConnect()
        {
			#if UNITY_IOS || UNITY_ANDROID
			VideoRecordingBridge.REConnect(liveUrl);
			#endif
        }


		public void OnDisconnectedFromPhoton()
		{
			Debug.LogError("OnDisconnectedFromPhoton()");

			// Back to main menu        
			//Application.LoadLevel(Application.loadedLevelName);
		}

		public void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			Debug.LogError("OnPhotonPlayerConnected(): " + player.NickName);
			if (Online != null) {
				Online.text = PhotonNetwork.playerList.Length.ToString();
			}
			if (ChatText != null) {
				ChatText.text += player.NickName+"進入這個房間\n";
			}
			int ppl =  PhotonNetwork.room.PlayerCount;
			photonView.RPC ("BroadcastOnlinePpl", PhotonTargets.All, ppl);
		}

		public void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			Debug.LogError("OnPhotonPlayerDisconnected(): " + player.NickName+ " 是否為房主:"+player.IsMasterClient);
			//Debug.Log (PhotonNetwork.player.IsMasterClient);
			//PhotonNetwork.room.MasterClientId
			//if (player.ID==PhotonNetwork.room.MasterClientId) {
			Hashtable cp = PhotonNetwork.room.customProperties;
			string cname = (string)cp ["CRoomName"];
			if (player.NickName==cname) {
				Debug.LogError("[房主] "+player.NickName+"離開房間, 房間關閉, 所有人回到大廳");
				if (ChatText != null) {
					ChatText.text += "[房主] "+player.NickName+"離開房間, 房間關閉, 所有人回到大廳\n";
				}
				//房主離開 結束遊戲房 回到大廳
				PhotonNetwork.LeaveRoom();
				return;
			} else {
				int ppl =  PhotonNetwork.room.PlayerCount;
				photonView.RPC ("BroadcastOnlinePpl", PhotonTargets.All, ppl);
				if (ChatText != null) {
					ChatText.text += player.NickName+"離開這個房間\n";
				}
				foreach (MahPlayer mp in Users) {
					if (mp.photonPlayer!=null && player.ID == mp.photonPlayer.ID) {
						GameStop ();
						if (ChatText != null) {
							ChatText.text += "終止麻將遊戲！\n";
						}
					}
				}
			}
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
						Users [1].keepedMah.Clear ();
						Users [1].ponMah.Clear ();
						Users [1].abandanedMah.Clear ();
						//Users [1].AutoPlay = false;
						ids [0] = pp.ID;
						//break;
					} else {
						Users [0].ID = PhotonNetwork.player.ID;
						Users [0].photonPlayer = PhotonNetwork.player;
						Users [0].isAI = false;
						Users [0].keepedMah.Clear ();
						Users [0].ponMah.Clear ();
						Users [0].abandanedMah.Clear ();
						//Users [0].AutoPlay = false;
						ids [1] = PhotonNetwork.player.ID;
					}
					i++;
				}
			}

			_currentIndex = 0;
			_lastDaPai = 0;
			//private int moMahId = 0;
			_lastMoPaiPlayerID = -1;
			_lastDaPaiPlayerID = -1;
			this._isgameover = false;
			_activePlayer = null;
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
			//Debug.Log (ids);
			int i = 0;
			foreach (int id in ids) {
				Users [i].ID = id;
				Users [i].photonPlayer = PhotonPlayer.Find(id);
				Users [i].isAI = false;
				Users [i].keepedMah.Clear ();
				Users [i].ponMah.Clear ();
				Users [i].abandanedMah.Clear ();
				//Users [i].AutoPlay = false;
				i++;
			}
        }

		[PunRPC]
		private void BroadcastOnlinePpl(int ppl)
		{
			Debug.LogError ("[RPC] BroadcastOnlinePpl("+ppl+")");
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
							bcGameOver();
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
			Debug.Log ("doHandleWin("+playerid+")");
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
				Debug.LogError ("doHandleDaPai() Server Exec Only!! doHandleDaPai()");
				return;
			} else {
				if (mid != 0) {
					if (_activePlayer != null && _activePlayer.ID==playerid && _activePlayer.ID != _lastDaPaiPlayerID) {
						_lastDaPaiPlayerID = _activePlayer.ID;
						//_lastDaPai = mid;
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
					if (_activePlayer.checkPai (_lastDaPai, false)) {
						Debug.Log ("xxxxxxxxxxxxx");
						yield return null;
					}
					//_activePlayer.state = PLAYERSTATE.MOPAING;//更改為摸牌狀態
					//Debug.LogError ("[s] 4.doMoPai("+_activePlayer.photonPlayer.NickName+")");
					//Debug.Log("xxxxxx_activePlayer.state="+_activePlayer.state+" "+_activePlayer.photonPlayer.NickName);
					int GotID = getMahjongPai ();
					if (GotID <= 0) {
						bcGameOver();
						yield break;
					}
					//Debug.Log ("mopai"+GotID);
					string name = Mahjong.getName (GotID);
					_lastMoPaiPlayerID = _activePlayer.ID;
					//keepedMah.Add (GotID);
					//Debug.LogError ("[s] 4.doMoPai(GotID="+GotID+", isfirst="+isfirst+")");
					//Debug.LogError ("[s] 4. doMoPai("+_activePlayer.photonPlayer.NickName+", GotID="+GotID+")");
					_activePlayer.handleMoPai (GotID, getRemainPai());
					_activePlayer.checkPai (GotID, true);
				}
			}
        }

		private IEnumerator setActivePlayer(MahPlayer player) {
			if (this._isgameover)
				yield break;
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
			if (this._isgameover)
				yield break;
			if (_activePlayer != null) {
				yield return StartCoroutine( _activePlayer.doAiThink (this._lastDaPai));
				//yield return new WaitForSeconds (1);
				if (getRemainPai()> 0) {
					doHandleNext ();
				} else {
					bcGameOver();
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
					if (_activePlayer.isAI || _activePlayer.AutoPlay) {
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
				bcGameOver();
			}
			//StopCoroutine ("AutoChangeActivePlayerCo");
		}

		public int getMahjongPai()
		{
			int remain = getRemainPai ();

			//牌光了 游戏结束
			if (remain == 0)
			{
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
			Debug.LogError ("[RPC] 流局()");
            //OverPanel.gameObject.SetActive(true);
			AbanMjs.Clear ();
			//if (ChatText != null) {
			//	ChatText.text += "遊戲結束\n";
			//}
			ShowAlert("流局", 5.0f);
        }
		[PunRPC]
		private void SetupAI(object[] param)
		{
			int player_id = (int)param[0];
			bool en = (bool)param[1];
			Debug.LogError ("[RPC] SetupAI(pid="+player_id+", en="+en+")");
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer != null) {
				mplayer.AutoPlay = en;
			}
		}

		[PunRPC]
		private void dispatchPai(object[] param)
		{
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
			Debug.LogError ("[RPC] 輪到玩家(id="+player_id+")");
			MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			if (mplayer!=null && !(mplayer.isAI||mplayer.AutoPlay)) {
				Debug.Log ("!!!!!!!!!!!!!!!!!!!!!!!!");
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
					mplayer.checkPai (pai_id, true);
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
					Debug.LogError ("[RPC] " + mplayer.name + "出牌 " + pai_name + "("+pai_id+")"+ "[" + i + "+" + j + "]");
					mplayer.HideMenu ();
					_lastDaPai = pai_id;
					Speak (pai_id);
				} else {
					if (!PhotonNetwork.player.IsMasterClient) {
						if(mplayer.keepedMah.Count>0)
							mplayer.keepedMah.RemoveAt (0);
					}
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + mplayer.name + "出牌 " + pai_name + "(" + i + "+" + j + ")");
					Debug.LogError ("[RPC] " + mplayer.name + "出牌 " + pai_name + "("+pai_id+")"+ "[" + i + "+" + j + "]");
					mplayer.DaPaiToAban (pai_id, AbanMjs.Count);
					_lastDaPai = pai_id;
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
            MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			StartCoroutine(_OverPanel(5f));
			if (mplayer != null) {
				string winner = mplayer.playerName.text;
				Debug.LogError ("[RPC] 玩家(" + winner + ") 胡牌了!");
				text.text = winner;

                //傳送勝敗場數
                string sToken = CryptoPrefs.GetString("USERTOKEN");
                string sName = CryptoPrefs.GetString("USERNAME");
                string sWin = CryptoPrefs.GetString("USERWIN");
                string sLose = CryptoPrefs.GetString("USERLOSE");
                int sOldWin = (string.IsNullOrEmpty(sWin)) ? 0 : int.Parse(sWin);
                int sOldLose = (string.IsNullOrEmpty(sLose)) ? 0 : int.Parse(sLose);
                int sRate = 0;

                if (mplayer.ID == PhotonNetwork.player.ID) {
					//Debug.LogError ("[RPC] 你贏了 ");
                    SetWinnerInfo("Win");
					playWinSound ();
					nagiEffectPlayerA.ShowNagi (Nagieffect.NagiType.HU2);
					nagiEffectPlayerB.ShowNagi (Nagieffect.NagiType.PAU);

                    if (sOldWin != 0)
                        sRate = (sOldWin + 1) * 10000 / (sOldWin + sOldLose + 1);
                    MJApi.setUserWin(sToken, sName, sOldWin, sOldWin + 1, sRate, setUserWinCallback);
				} else {
                    SetWinnerInfo("Lose");
					playWinSound ();
					nagiEffectPlayerA.ShowNagi (Nagieffect.NagiType.PAU);
					nagiEffectPlayerB.ShowNagi (Nagieffect.NagiType.HU);

                    if (sOldWin != 0)
                        sRate = (sOldWin) * 10000 / (sOldWin + sOldLose + 1);
                    MJApi.setUserLose(sToken, sName, sOldLose, sOldLose + 1, sRate, setUserLoseCallback);
				}
                Debug.Log("目前勝場數 " + sOldWin + " ;目前敗場數 " + sOldLose + " ;勝率 " + sRate + "%");
            }
        }

        private void SetWinnerInfo(string _PlayerResult) {
            string sName = CryptoPrefs.GetString("USERNAME");
            string sPhoto = CryptoPrefs.GetString("USERPHOTO");
            string sLv = CryptoPrefs.GetString("USERLEVEL");
            string sCoin = CryptoPrefs.GetString("USERCOIN");
            //string oName = CryptoPrefs.GetString("OPPNAME");
            //string oPhoto = CryptoPrefs.GetString("OPPPHOTO");
            //string oLv = CryptoPrefs.GetString("OPPLEVEL");
            //string oCoin = CryptoPrefs.GetString("OPPCOIN");

            if (_PlayerResult == "Win") {
                if (!string.IsNullOrEmpty(sPhoto))
                {
                    Texture2D newPhoto = new Texture2D(1, 1);
                    newPhoto.LoadImage(Convert.FromBase64String(sPhoto));
                    newPhoto.Apply();
                    _winPhoto.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
                }

                _winName.text = sName;
                _winLv.text = "Lv " + sLv;
                _winCoin.text = String.Format("{0:0,0}", int.Parse(sCoin));
            }
            else if(_PlayerResult == "Lose")
            {
                //if (!string.IsNullOrEmpty(oPhoto))
                //{
                //    Texture2D newPhoto = new Texture2D(1, 1);
                //    newPhoto.LoadImage(Convert.FromBase64String(oPhoto));
                //    newPhoto.Apply();
                //    _winPhoto.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
                //}

                //_winName.text = oName;
                //_winLv.text = oLv;
                //_winCoin.text = String.Format("{0:0,0}", int.Parse(oCoin));
            }
        }

        public void setUserWinCallback(WebExceptionStatus status, string result)
        {
            _readyWinAddresult = result;
            _readyWinAdd = true;
            if (status != WebExceptionStatus.Success)
            {
                Debug.Log("setUserWinCallback Failed! " + result);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                Debug.Log("CB: setUserWinCallback =  " + result);
            }
        }

        public void setUserLoseCallback(WebExceptionStatus status, string result)
        {
            _readyLoseAddresult = result;
            _readyLoseAdd = true;
            if (status != WebExceptionStatus.Success)
            {
                Debug.Log("setUserLoseCallback Failed! " + result);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                Debug.Log("CB: setUserLoseCallback =  " + result);
            }
        }

        void Update()
        {
            if (_readyWinAdd) {
                _readyWinAdd = false;
                CryptoPrefs.SetString("USERWIN", _readyWinAddresult);
            }

            if (_readyLoseAdd)
            {
                _readyLoseAdd = false;
                CryptoPrefs.SetString("USERLOSE", _readyLoseAddresult);
            }

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
		/*
        public void OneMoreMahjong()
        {
			Debug.Log ("OneMoreMahjong()");
            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
                //Text text = OverPanel.GetComponentInChildren<Text>();
                //text.text = "等待房主确认...";
				ShowAlert("等待房主同意", 5.0f);
                return;
            }
			//Debug.Log ("[s] OneMoreMahjong()");

			photonView.RPC("OneMore", PhotonTargets.MasterClient, null);
        }
		*/

		/*
        [PunRPC]
        private void OneMore()
        {
			//Debug.LogError ("[RPC] OneMore()");
			if (OverPanel != null) {
				OverPanel.gameObject.SetActive (false);
			}

            RestView();

            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
                return;
            }
			//Invoke ("", 5);
			//Invoke ("InvateYES", 5.0f);
			//Invoke ("InvateYES", 5.0f);
			//StartCoroutine("ReadyPlay");
			//InvateYES();
        }
		*/

        private void RestView()
        {
			Debug.Log ("[s] RestView("+panels.Length+")");
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

		void OnApplicationPause(bool pauseStatus)
		{
			//if (pauseStatus == true)
			//	Back ();
		}

        public void Back()
        {
			//Debug.Log ("Back()");
			#if UNITY_IOS || UNITY_ANDROID
			VideoRecordingBridge.StopRecord ();
			VideoRecordingBridge.StopPlay ();
			#endif
            PhotonNetwork.LeaveRoom();
            //SceneManager.LoadScene("Game");
        }

		public void OnLeftRoom()
		{
			//Debug.Log("OnLeftRoom (local)");
			#if UNITY_IOS || UNITY_ANDROID
			VideoRecordingBridge.StopRecord ();
			VideoRecordingBridge.StopPlay ();
			#endif
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
        public void ClickSetting() {
            //SettingCanvas.transform.DOMoveY(10.8f, 0, true);
            SettingCanvas.transform.DOMoveY(0, 1f, false).SetEase(Ease.InOutBack);
        }

        public void ExitSetting()
        {
            //SettingCanvas.transform.DOMoveY(0, 0, true);
            SettingCanvas.transform.DOMoveY(10.8f, 1, false).SetEase(Ease.InOutBack);
        }

        //畫面移入
        private void LayoutStart()
        {
            if (VideoBG) //直播畫面背景圖
                VideoBG.color = _colorAplha;

            //VideoCanvas.transform.DOScaleX (1, 0);
            AllCanvas.transform.DOMoveX (-14, 0, true);
			AllCanvas.transform.DOMoveX(0, 1, false).SetEase(Ease.InOutBack).OnComplete(ShowInfo);
			#if UNITY_IOS || UNITY_ANDROID
			VideoRecordingBridge.MoveRight ();
			#endif
			//VideoCanvas.transform.DOScaleX (0.3f, 1).SetEase(Ease.InOutBack);
            //Invoke("ShowInfo", 0.45f);
			//ShowInfo();
        }

		private void LayoutEnd()
		{
            if (VideoBG)  //直播畫面背景圖
                VideoBG.color = _colorWhite;

            ExitSetting();
            OverPanel.gameObject.SetActive(false);
            AllCanvas.transform.DOMoveX (0, 0, true);
			AllCanvas.transform.DOMoveX (-14, 1, false).SetEase(Ease.InOutBack);
			#if UNITY_IOS || UNITY_ANDROID
			VideoRecordingBridge.MoveLeft ();
			#endif
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

		public void ShowAlert(string msg, float t = 0)
		{
			if (PanelAlert != null) {
				Text txt = PanelAlert.GetComponentInChildren<Text> ();
				txt.text = msg;
				PanelAlert.SetActive (true);
				if (t > 0) {
					Invoke ("HideAlert", t);
				}
			}
		}

		public void HideAlert()
		{
			if (PanelAlert != null) {
				PanelAlert.SetActive (false);
			}
		}

		public void bcGameOver()
		{
			this._isgameover = true;
			photonView.RPC ("GameOver", PhotonTargets.All, null);
			Invoke("InvateYES", 5.0f);
		}

		public void bcGameWin()
		{
			this._isgameover = true;
			//photonView.RPC ("GameOver", PhotonTargets.All, null);
		}

		public void bcGameStart()
		{
			this._isgameover = false;
			photonView.RPC ("GameStart", PhotonTargets.All, null);
		}
    }
}
