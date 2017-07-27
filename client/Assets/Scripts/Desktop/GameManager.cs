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
		private int MaxWaitTime = 30;
		private int _remainTime = 0;
		private string _gameVersion = "1.0";
		private int _targetid = 0;
		private int _currentIndex = 0;
		private bool _isgameover = false;

        public Image OverPanel;

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

			//#关键
			//我们不加入大厅 这里不需要得到房间列表所以不用加入大厅去
			//PhotonNetwork.autoJoinLobby = true;

			//#关键
			//这里保证所有主机上调用 PhotonNetwork.LoadLevel() 的时候主机和客户端能同时进入新的场景
			//PhotonNetwork.automaticallySyncScene = true;

			//PhotonNetwork.isMessageQueueRunning = true;

            //PhotonNetwork.OnEventCall += OnEvent;
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
			Debug.Log (PhotonNetwork.room);
			string name = PhotonNetwork.room.Name;
			string url = "rtmp://catpunch.co/live/" + name;
            if (!PhotonNetwork.isMasterClient)
            {
				Debug.LogError ("[s] !PhotonNetwork.isMasterClient");
				VideoRecordingBridge.StartPlay (url);
                return;
            }

			//VideoRecordingBridge.InitRecord ();
			//string name = PhotonNetwork.room.Name;
			//string url = "rtmp://catpunch.co/live/" + name;
			VideoRecordingBridge.StartRecord (url);

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
			this._isgameover = false;
            //只有副机有邀請的權利
            //if (PhotonNetwork.isMasterClient)
            //{
			//	Debug.Log ("!PhotonNetwork.isMasterClient");
            //    return;
            //}
			//只有主机有发牌的权利
			StartCoroutine("ReadyPlay");

            //photonView.RPC("StartMahjong", PhotonTargets.All, null);

        }

        [PunRPC]
        private void StartMahjong()
        {
			Debug.Log ("[RPC] StartMahjong()");
            //LayoutStart(); //畫面移入

            //Debug.LogError ("[RPC] StartMahjong()");

            //RestView();

            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
				Debug.Log ("!PhotonNetwork.isMasterClient");
                return;
            }
			_currentIndex = 0;
			if (PhotonNetwork.playerList.Length < 2) {
				Debug.Log ("兩個人以上才能開桌");
				return;
			}
            StartCoroutine("ReadyPlay");
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
			setActivePlayer(Users[_currentIndex]);
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
		}

		public void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			//Debug.Log("OnPlayerDisconneced: " + player.NickName);
			//Debug.Log (PhotonNetwork.playerList.Length);
			//OverPanel.gameObject.SetActive(true);
			if (PhotonNetwork.playerList.Length < 2) {
				//Back ();
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
			/*
			if (PhotonNetwork.playerList.Length > 0 && PhotonNetwork.playerList.Length < 2) {
				//Debug.Log ("單機模式");
				//單機模式 第一個是自己 第二個是AI
				Users[0].photonPlayer = PhotonNetwork.player;
				Users[0].isAI = false;
				Users[0].autoPlay = true;//for test
				PhotonNetwork.player.mahPlayer = Users [0];
				Users[1].photonPlayer = null;
				Users[1].isAI = true;
			} else {
			*/
			int[] ids = new int[2];
			Debug.Log ("playerList.Length="+PhotonNetwork.playerList.Length);
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
			Debug.Log (ids);
			photonView.RPC("BundlePlayer", PhotonTargets.Others, ids);
			/*
            //按照 ABCD的顺序传photonplayer.ID
            int[] ids = new int[2];

            //添加互相引用
            int ppindex = 0;
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.player.ID == PhotonNetwork.playerList[i].ID)
                {
                    ppindex = i;
                    break;
                }
            }

			for (int i = 0; i < PhotonNetwork.playerList.Length; i++, ppindex++)
            {
				ppindex = ppindex % PhotonNetwork.playerList.Length;
                players[i].photonPlayer = PhotonNetwork.playerList[ppindex];
                PhotonNetwork.playerList[ppindex].mahPlayer = players[i];
                ids[i] = players[i].photonPlayer.ID;
            }
			//Debug.LogError (PhotonNetwork.playerList);
            //Debug.LogError(PhotonNetwork.player.mahPlayer.gameObject.name);

            photonView.RPC("BundlePlayer", PhotonTargets.Others, ids);
			*/
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
			/*
			if (PhotonNetwork.playerList.Length > 1) {
				//對戰模式 第一個是自己 第二個是對戰玩家
				int i = 0;
				foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
					if (pp != PhotonNetwork.player) {
						Users [1].photonPlayer = pp;
						Users [1].isAI = false;
						Users [1].autoPlay = false;
						ids [0] = pp.ID;
						//break;
					} else {
						Users[0].photonPlayer = PhotonNetwork.player;
						Users[0].isAI = false;
						Users[0].autoPlay = false;
						ids [1] = PhotonNetwork.player.ID;
					}
					i++;
				}
			}
			*/
            /*
			int ppindex = 0;
            for (int i = 0; i < ids.Length; i++)
            {
                if (PhotonNetwork.player.ID == ids[i])
                {
                    ppindex = i;
                    break;
                }
            }

			for (int i = 0; i < PhotonNetwork.playerList.Length; i++, ppindex++)
            {
				ppindex = ppindex % PhotonNetwork.playerList.Length;
                players[i].photonPlayer = PhotonPlayer.Find(ids[ppindex]);
                PhotonPlayer.Find(ids[ppindex]).mahPlayer = players[i];
            }
            //Debug.LogError(PhotonNetwork.player.mahPlayer.gameObject.name);
            */
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
			LayoutStart(); //畫面移入
			int i = 0;
			//int[][] pais = new int[Users.Count][];
			//for(int a=0;a<Users.Count;a++) {
			//	pais[a] = new int[Mahjong.MAXPAI];
			//}
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
				//pais [i] = mahm.ToArray ();
				//AbanMjs.Clear ();
				//foreach(int[] aa in content) {
				//	List<int> mahs = new List<int> (aa);
				//	Users [i].clearPAI ();
				//	Users [i].keepedMah = mahs;
				//	Users [i].ShowPAI ();
				//	i++;
				//}
				if(PhotonNetwork.player.ID!=mp.ID) {
					//	photonView.RPC ("dispatchPai", PhotonTargets.MasterClient, i, mahm.ToArray ());
					//else
					object[] param = { mp.ID, mahm.ToArray() };
					photonView.RPC ("dispatchPai", PhotonTargets.Others, param);
				}
				//i++;
			}
			/*
			Debug.Log (pais.Length);
			foreach (MahPlayer mp in Users) {
				if (PhotonNetwork.player.ID == mp.ID) {
					//byte[] xx = DataUtility.Serialize (pais);
					//photonView.RPC ("dispatchPai", PhotonTargets.MasterClient, xx);
				} else {
					int[] tmp = pais [1];
					pais [1] = pais [0];
					pais [0] = tmp;
					//byte[] xx = DataUtility.Serialize (pais);
					photonView.RPC ("dispatchPai", PhotonTargets.Others, xx);
				}
			}
			*/
			/*
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
				if (p.mahPlayer == null)
					continue;
				p.mahPlayer.keepedMah.Clear ();
				p.mahPlayer.ponMah.Clear ();
				p.mahPlayer.abandanedMah.Clear ();
				mahm.Clear ();
				for (int j = 0; j < Mahjong.MAXPAI; j++)
				{
					//TODO 依据骰子数判断取出的是哪一个麻将牌
					int pai_id = mahJong.allMah.Dequeue();
					mahm.Add(pai_id);
				}
				photonView.RPC("dispatchPai", PhotonTargets.AllBuffered, p.ID, mahm.ToArray());
            }
			*/
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
			setActivePlayer (next);
		}

		//private IEnumerator doHandleNextCo(float time) {
		//	yield return new WaitForSeconds (time);
		//	MahPlayer next = _activePlayer.nextPlayer;
		//	setActivePlayer (next);
		//}

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
					/*
					PhotonPlayer photonPlayer = PhotonPlayer.Find(playerid);
					if (photonPlayer != null && photonPlayer.mahPlayer != null) {
						photonPlayer.mahPlayer.handleGan (abanMahId);
					}
					*/
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
					/*
					PhotonPlayer photonPlayer = PhotonPlayer.Find(playerid);
					if (photonPlayer != null && photonPlayer.mahPlayer != null) {
						photonPlayer.mahPlayer.handleChi (abanMahId);
					}
					*/
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
			Debug.LogError ("[s] doHandleDaPai("+playerid+", "+mid+")");
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
				//setActivePlayer(players[0]);
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
					//Debug.Log ("[s] "+_activePlayer.name+" 摸到了 "+name+"("+GotID+")");
					//object[] param = { GotID, _activePlayer.photonPlayer.ID };
					//photonView.RPC ("MoPai", PhotonTargets.All, param);
				}
			}
        }

		private void setActivePlayer(MahPlayer player) {
			//Debug.LogError ("[s] 輪到("+player.name+")");
			//int[] param = { photonPlayer.ID };
			//photonView.RPC("ShowTimer", PhotonTargets.All, param);
			MahPlayer _prevPlayer = _activePlayer;
			//_remainTime = 0;
			if (_prevPlayer!=null) {
				int max = 0;
				//Debug.Log ("******************_activePlayer.state="+_activePlayer.state+" "+_activePlayer.photonPlayer.NickName);
				max = _prevPlayer.keepedMah.Count-1;
				//Debug.LogError ("max="+max);
				if (_prevPlayer.state == PLAYERSTATE.PLAYING) {//如果是什麼都沒做, 先摸牌 再打牌
					doHandleMoPai ();//摸牌
					max = _prevPlayer.keepedMah.Count-1;
					_prevPlayer.handleDaPai (_prevPlayer.keepedMah [max]);
				}
				else if (_prevPlayer.state == PLAYERSTATE.MOPAING) {//如果是摸完牌, 直接打牌
					max = _prevPlayer.keepedMah.Count-1;
					_prevPlayer.handleDaPai (_prevPlayer.keepedMah [max]);
				}
				else if (_prevPlayer.state == PLAYERSTATE.CHIPAING) {//如果是吃牌狀態, 直接打牌
					max = _prevPlayer.keepedMah.Count-1;
					_prevPlayer.handleDaPai (_prevPlayer.keepedMah [max]);
				}
				else if (_prevPlayer.state == PLAYERSTATE.PONPAING) {//如果是碰牌狀態, 直接打牌
					max = _prevPlayer.keepedMah.Count-1;
					_prevPlayer.handleDaPai (_prevPlayer.keepedMah [max]);
				}
				else if (_prevPlayer.state == PLAYERSTATE.GANPAING) {//如果是槓牌狀態, 直接打牌
					max = _prevPlayer.keepedMah.Count-1;
					_prevPlayer.handleDaPai (_prevPlayer.keepedMah [max]);
				}
				_prevPlayer.state = PLAYERSTATE.WAITING;//更改狀態為完成狀態
			}
			_activePlayer = player;
			_activePlayer.actived = false;
			_activePlayer.state = PLAYERSTATE.PLAYING;
			int[] param = { _currentIndex };
			photonView.RPC("ShowActivePlayer", PhotonTargets.All, param);
			_lastMoPaiPlayerID = -1;
			_lastDaPaiPlayerID = -1;
			//StopCoroutine ("AutoChangeActivePlayerCo");

			if (_activePlayer != null && (_activePlayer.isAI||_activePlayer.autoPlay)) {
				StartCoroutine (AutoDaPai ());//自動代打
			} else {
				StartCoroutine (AutoChangeActivePlayerCo ());
			}
			//StartCoroutine (AutoChangeActivePlayerCo ());
			//doMoPai (isfirst);
		}

		private IEnumerator AutoDaPai()
		{
			if (_activePlayer != null) {
				if (this._isgameover)
					yield break;
				yield return StartCoroutine( _activePlayer.doAiThink (this._lastDaPai));
				/*
				int max = 0;
				int rnd = UnityEngine.Random.Range(2, 10);
				_remainTime = MaxWaitTime;
				while (_remainTime>MaxWaitTime-rnd) {
					yield return new WaitForSeconds (1);
					_remainTime--;
					if (this._isgameover)
						yield break;
					//Debug.Log ("waitTime="+_waitTime);
					int[] param = { _remainTime, _currentIndex };
					photonView.RPC ("RemainTime", PhotonTargets.All, param);
				}
				//yield return new WaitForSeconds (rnd);
				if (_activePlayer.state == PLAYERSTATE.PLAYING) {//如果是什麼都沒做, 先摸牌 再打牌
					yield return StartCoroutine (_MoPaiCo (0));//摸牌
					//yield return new WaitForSeconds (1);
					max = _activePlayer.keepedMah.Count - 1;
					//Debug.Log ("max="+max+", "+_activePlayer.keepedMah [max]);
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				} else if (_activePlayer.state == PLAYERSTATE.MOPAING) {//如果是摸完牌, 直接打牌
					max = _activePlayer.keepedMah.Count - 1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				} else if (_activePlayer.state == PLAYERSTATE.CHIPAING) {//如果是吃牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count - 1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				} else if (_activePlayer.state == PLAYERSTATE.PONPAING) {//如果是碰牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count - 1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				} else if (_activePlayer.state == PLAYERSTATE.GANPAING) {//如果是槓牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count - 1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				}
				_activePlayer.state = PLAYERSTATE.WAITING;//更改狀態為完成狀態
				*/
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
			while (_remainTime>0) {
				if (this._isgameover)
					yield break;
				yield return new WaitForSeconds (1);
				_remainTime--;
				//Debug.Log ("waitTime="+_waitTime);
				//int[] param = { _remainTime, _currentIndex };
				if (PhotonNetwork.player.ID == Users [_currentIndex].photonPlayer.ID) {
					int[] param1 = { _remainTime, 0 };
					photonView.RPC ("RemainTime", PhotonTargets.MasterClient, param1);
					int[] param2 = { _remainTime, 1 };
					photonView.RPC ("RemainTime", PhotonTargets.Others, param2);
				} else {
					int[] param1 = { _remainTime, 0 };
					photonView.RPC ("RemainTime", PhotonTargets.Others, param1);
					int[] param2 = { _remainTime, 1 };
					photonView.RPC ("RemainTime", PhotonTargets.MasterClient, param2);
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
			Debug.Log ("SendRequestToServer()");
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

		/*
        [PunRPC]
        private void RemoveOneMah(int[] param)
        {
            int index = param[0];
			if (index < mahJong.allMah.Count) {
				mahJong.allMah.RemoveAt (index);
			}
			//Debug.LogError ("[RPC] RemoveOneMah(index="+index+")");
        }
        */

        [PunRPC]
        private void GameOver()
        {
			//Debug.LogError ("[RPC] 遊戲結束()");
            OverPanel.gameObject.SetActive(true);
			AbanMjs.Clear ();
        }

		[PunRPC]
		private void dispatchPai(object[] param)
		{
			LayoutStart(); //畫面移入
			int player_id = (int)param[0];
			int[] content = (int[])param[1];
			//Debug.LogError ("[RPC] 發牌()");
			//int idx = (int)param[0];
			//int[][] content = (int[][])param [1];
			//int[][] content = DataUtility.Deserialize<int[][]>(param);
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
			//MahPlayer mplayer =  Users.Find (x => x.ID.Equals (player_id));
			//if (mplayer) {
			//	mplayer.clearPAI ();
			//	mplayer.keepedMah = new List<int>(content);
			//	mplayer.ShowPAI ();
			//}
			//AbanMjs.Clear ();
			//Debug.LogError ("[RPC] 發牌(id="+idx+", content.length="+content.Length+")");
			/*
			List<int> mahs = new List<int> (content);
			//Debug.LogError ("[RPC] 系統發牌 (idx=" + idx + "), 張數:" + content.Length);
			if (idx < Users.Count) {
				Users [idx].clearPAI ();
				Users [idx].keepedMah = mahs;
				Users [idx].ShowPAI ();
			}
			/*
			if (Users[idx] == PhotonNetwork.player.ID) {
				//PhotonPlayer photonPlayer = PhotonPlayer.Find (id);
				//if (photonPlayer != null && photonPlayer.mahPlayer != null) {
				//List<int> mahs = new List<int> (content);
				//if (photonPlayer.IsLocal) {
				//photonPlayer.mahPlayer.keepedMah = mahs;
				Users[idx].keepedMah = mahs;
				Debug.LogError ("[RPC] 系統發牌 (idx=" + idx + "), 張數:" + content.Length);
				PhotonNetwork.player.mahPlayer.ShowPAI ();
				//	} else {
				//		photonPlayer.mahPlayer.keepedMah = mahs;
				//		Debug.LogError ("[RPC] 系統發牌給" + photonPlayer.NickName + ", 張數:" + content.Length);
				//	}
				//}
			} else {
				//id = Users [0].photonPlayer.ID;
				//Users [0].photonPlayer
				PhotonPlayer photonPlayer = PhotonPlayer.Find (id);
				if (photonPlayer != null) {
					photonPlayer.mahPlayer.keepedMah = mahs;
					Debug.LogError ("[RPC] 系統發牌給" + photonPlayer.NickName + ", 張數:" + content.Length);
				} else {
					Users[1].keepedMah = mahs;
					Debug.LogError ("[RPC] 系統發牌給AI, 張數:" + content.Length);
				}
			}
			*/
		}
		
		[PunRPC]
		private void RemainTime(int[] param)
		{
			int rtime = (int)param[0];
			int idx = (int)param[1];
			//Debug.LogError ("[RPC] idx="+idx+", RemainTime=" + rtime);
			//PhotonPlayer photonPlayer = PhotonPlayer.Find(pid);
			MahPlayer mahPlayer = Users[idx];
			if (mahPlayer != null) {
				mahPlayer.timer.time = rtime;
				mahPlayer.timer.Show (1.0f);
			}
		}
        [PunRPC]
		void ShowActivePlayer(int[] param)
        {
			int idx = (int)param[0];
			//Debug.LogError ("[RPC] 輪到玩家(idx="+idx+")");
			//foreach (MahPlayer player in Users) {
				
			//}
			MahPlayer mahPlayer = Users[idx];
			//if(mahPlayer.isAI)
			//	Debug.LogError ("[RPC] 輪到電腦"+mahPlayer.name);
			//else
			//	Debug.LogError ("[RPC] 輪到玩家"+mahPlayer.name);
			//Debug.LogError ("[RPC] 輪到你了!!!("+photonPlayer.NickName+")");
			if (idx == 0) {
				if(mahPlayer!=null && !(mahPlayer.isAI||mahPlayer.autoPlay))
					mahPlayer.checkPai (_lastDaPai, false);
			}
			mahPlayer.timer.time = 30;
			mahPlayer.timer.Show (0.5f);
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(id);

            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                player.mahPlayer.timer.Hide();
            }
			if (photonPlayer != null && photonPlayer.mahPlayer != null) {
				if (photonPlayer.IsLocal) {
					//Debug.LogError ("[RPC] 輪到你了!!!("+photonPlayer.NickName+")");
					photonPlayer.mahPlayer.checkPai (abanMahId, false);
				} else {
					//Debug.LogError ("[RPC] 輪到" + photonPlayer.NickName + "了!!!");
					//photonPlayer.mahPlayer.checkPai (abanMahId, false);
				}
				//photonPlayer.mahPlayer.timer.time = 30;
				photonPlayer.mahPlayer.timer.Show ();
			}
			*/
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
					Debug.LogError ("[RPC] " + mplayer.name + "摸牌 " + pname + "(" + i + "+" + j + ")");
					mplayer.createPaiToMo (pai_id);
				} else {
					//mplayer.keepedMah.Add (0);
					i = mplayer.keepedMah.Count;
					j = mplayer.ponMah.Count;
					Debug.LogError ("[RPC] " + mplayer.name + "摸牌 " + pname + "(" + i + "+" + j + ")");
					mplayer.createPaiToMo (0);
				}
				//Debug.Log("剩餘 "+remain_pai_count+"");
				mplayer.HideMenu ();
			}
			//moMahId = pai_id;
			//bool isfirst = (bool)param[2];
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(player_id);
			string pname = Mahjong.getName (pai_id);
			if (photonPlayer != null) {
				if (photonPlayer.IsLocal) {
					photonPlayer.mahPlayer.keepedMah.Add (pai_id);
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] 你(" + photonPlayer.NickName + ")摸牌 " + pname + "(" + i + "+" + j + ")");
					photonPlayer.mahPlayer.createPaiToMo (pai_id);
					//photonPlayer.mahPlayer.HideMenu ();
				} else {
					photonPlayer.mahPlayer.keepedMah.Add (0);
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + photonPlayer.NickName + "摸牌 " + pname + "(" + i + "+" + j + ")");
					photonPlayer.mahPlayer.createPaiToMo (0);
				}
				photonPlayer.mahPlayer.HideMenu ();
			}
			*/
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
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(player_id);
			if (photonPlayer != null) {
				if (photonPlayer.IsLocal) {
					photonPlayer.mahPlayer.keepedMah.Remove (pai_id);
					photonPlayer.mahPlayer.DaPaiToAban (pai_id);
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] 你(" + photonPlayer.NickName + ")出牌 " + pai_name + "(" + i + "+" + j + ")");
					photonPlayer.mahPlayer.HideMenu ();
					Speak (pai_id);
				} else {
					photonPlayer.mahPlayer.keepedMah.RemoveAt (0);
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + photonPlayer.NickName + "出牌 " + pai_name + "(" + i + "+" + j + ")");
					photonPlayer.mahPlayer.DaPaiToAban (pai_id);
					Speak (pai_id);
				}
				playDaiPaiSound ();
				//PhotonPlayer photonPlayer = PhotonPlayer.Find(player_id);
				//photonPlayer.mahPlayer.outPaiFromKeep (pai_id);
			}
			*/

			//abanMahId = pai_id;
			this._remainTime = 1;
			//doHandleNext ();
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
					//Debug.LogError ("[RPC] " + mplayer.name + "喊了碰 " + pai_name + "(" + i + "+" + j + ")");
					playPonSound ();
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PON);
				}
				mplayer.collectPonPai (pai_id);
			}
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(player_id);
			if (photonPlayer != null) {
				photonPlayer.mahPlayer.collectPonPai (pai_id);
				if (photonPlayer.IsLocal) {
					//photonPlayer.mahPlayer.keepedMah.Remove (pai_id);
					//photonPlayer.mahPlayer.keepedMah.Remove (pai_id);
					//photonPlayer.mahPlayer.ponMah.Add (pai_id);
					//photonPlayer.mahPlayer.ponMah.Add (pai_id);
					//photonPlayer.mahPlayer.ponMah.Add (pai_id);
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] 你(" + photonPlayer.NickName + ")喊了碰 " + pai_name + "(" + i + "+" + j + ")");
					playPonSound ();
					photonPlayer.mahPlayer.HideMenu ();
                    nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.PON);
                } else {
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + photonPlayer.NickName + "喊了碰 " + pai_name + "(" + i + "+" + j + ")");
					playPonSound ();
                    nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PON);
                }
			}
			*/
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
					//Debug.LogError ("[RPC] " + mplayer.name + "喊槓 " + pai_name + "(" + i + "+" + j + ")");
					playGanSound ();
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.GAN);
				}
				//mplayer.collectGanPai (pai_id);
			}
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(player_id);
			if (photonPlayer != null) {
				if (photonPlayer.IsLocal) {
					photonPlayer.mahPlayer.keepedMah.Remove (pai_id);
					photonPlayer.mahPlayer.keepedMah.Remove (pai_id);
					photonPlayer.mahPlayer.keepedMah.Remove (pai_id);
					photonPlayer.mahPlayer.ponMah.Add (pai_id);
					photonPlayer.mahPlayer.ponMah.Add (pai_id);
					photonPlayer.mahPlayer.ponMah.Add (pai_id);
					photonPlayer.mahPlayer.ponMah.Add (pai_id);
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] 你(" + photonPlayer.NickName + ")喊槓 " + pai_name + "(" + i + "+" + j + ")");
					playGanSound ();
					photonPlayer.mahPlayer.HideMenu ();
                    nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.GAN);
                } else {
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + photonPlayer.NickName + "喊槓 " + pai_name + "(" + i + "+" + j + ")");
					playGanSound ();
                    nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.GAN);
                }
				photonPlayer.mahPlayer.collectGanPai (pai_id);
			}
			*/
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
					//Debug.LogError ("[RPC] " + mplayer.name + "喊吃 " + pai_name + "(" + i + "+" + j + ")");
					playChiSound ();
					nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.CHI);
				}
			}
			/*
			PhotonPlayer photonPlayer = PhotonPlayer.Find(player_id);
			if (photonPlayer != null) {
				photonPlayer.mahPlayer.collectChiPai (pai_id, chitype);
				if (photonPlayer.IsLocal) {
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] 你(" + photonPlayer.NickName + ")喊吃 " + pai_name + "(" + i + "+" + j + ")");
					playChiSound ();
					photonPlayer.mahPlayer.HideMenu ();
                    nagiEffectPlayerA.ShowNagi(Nagieffect.NagiType.CHI);
                } else {
					i = photonPlayer.mahPlayer.keepedMah.Count;
					j = photonPlayer.mahPlayer.ponMah.Count;
					//Debug.LogError ("[RPC] " + photonPlayer.NickName + "喊吃 " + pai_name + "(" + i + "+" + j + ")");
					playChiSound ();
                    nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.CHI);
                }
			}
			*/
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
            //AllCanvasAnim.SetBool("GameStart", false);
			AllCanvas.transform.DOMoveX (0, 0, true);
			//VideoCanvas.transform.DOScaleX (0.3f, 0);
			AllCanvas.transform.DOMoveX (-14, 1, false).SetEase(Ease.InOutBack);
			//VideoCanvas.transform.DOScaleX (1, 1).SetEase(Ease.InOutBack);
        }
    }
}