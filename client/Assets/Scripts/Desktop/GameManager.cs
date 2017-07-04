using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.Desktop
{
	public class GameManager : Photon.MonoBehaviour
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
        public List<MahPlayer> players;

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
        public Animator AllCanvasAnim; // 過場移入
        public Animator GameInfoAnim;  //局風顯示

        #region 
        /// <summary>
        /// 骰子数
        /// </summary>
        private int diceNum;

        /// <summary>
        /// 牌面上被打出去的牌
        /// </summary>
        //public MahJongObject abandonMah;
		private int abanMahId = 0;
		//private int moMahId = 0;
		private int _lastMoPaiPlayerID = -1;
		private int _lastDaPaiPlayerID = -1;
		private int MaxWaitTime = 30;
		private int _remainTime = 0;

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

			if (!PhotonNetwork.connected)
			{
				// We must be connected to a photon server! Back to main menu
				//Application.LoadLevel(Application.loadedLevel - 1);
				SceneManager.LoadScene("02.Lobby");
				return;
			}

			PhotonNetwork.isMessageQueueRunning = true;

            //PhotonNetwork.OnEventCall += OnEvent;
        }

        void Start()
        {
			//AudioManager.Instance.PlayBGM ("BGM_Playing");
            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
				VideoRecordingBridge.StartPlay ("rtmp://192.168.20.178:1935/mj17/myStream");
				//Debug.LogError ("[s] !PhotonNetwork.isMasterClient");
                return;
            }

			//VideoRecordingBridge.InitRecord ();
			VideoRecordingBridge.StartRecord ();

			AudioManager.Instance.PlayBGM ("BGM_Playing");
            //Debug.Log ("Start()");

            //0.关联Photon玩家 与 Unity中的玩家
            doBundlePlauer();

            //1.洗牌
            //doShuffleMah();

            //2.摇骰子
            //doDice();

            //3.发牌
            //doDealingMahs();

            //4.設定開始玩家
            //setActivePlayer(players[0]);
        }

        // 邀請玩麻將
        public void ClickInvatePlay() {

            //只有副机有邀請的權利
            if (PhotonNetwork.isMasterClient)
            {
                return;
            }

            photonView.RPC("StartMahjong", PhotonTargets.All, null);
        }

        [PunRPC]
        private void StartMahjong()
        {
            LayoutStart(); //畫面移入

            //Debug.LogError ("[RPC] StartMahjong()");

            //RestView();

            //只有主机有发牌的权利
            if (!PhotonNetwork.isMasterClient)
            {
                return;
            }

            StartCoroutine("ReadyPlay");
        }

        IEnumerator ReadyPlay() {
            yield return new WaitForSeconds(5f);

            //1.洗牌
            doShuffleMah();

            //2.摇骰子
            doDice();

            //3.发牌
            doDealingMahs();

            //4.設定開始玩家
            setActivePlayer(players[0]);
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
				Back ();
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
        private void doBundlePlauer()
        {
			//Debug.LogError ("[s] 0.doBundlePlauer()");
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
        }

        /// <summary>
        /// 同步玩家列表
        /// </summary>
        /// <param name="ids"></param>
        [PunRPC]
        public void BundlePlayer(int[] ids)
        {
			//Debug.LogError ("[RPC] BundlePlayer()");
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
        void doDealingMahs()
        {
			//Debug.LogError ("[s] 3.系統發牌(玩家數:"+PhotonNetwork.playerList.Length+")");
			List<int> mahm = new List<int>();
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
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
        }

		private void doHandleNext() {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log("Server Exec Only!! doHandleNext()");
				return;
			} 
			//Debug.LogError ("[s] doHandleNext()");
			//StartCoroutine (doHandleNextCo(time));
			MahPlayer next = _activePlayer.nextPlayer;
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
				if (abanMahId != 0) {
					PhotonPlayer photonPlayer = PhotonPlayer.Find(playerid);
					if (photonPlayer != null && photonPlayer.mahPlayer != null) {
						photonPlayer.mahPlayer.handlePon (abanMahId);
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
				if (abanMahId != 0) {
					PhotonPlayer photonPlayer = PhotonPlayer.Find(playerid);
					if (photonPlayer != null && photonPlayer.mahPlayer != null) {
						photonPlayer.mahPlayer.handleGan (abanMahId);
					}
				}
			}
		}

		public void doHandleChi (int playerid) {
			if (!PhotonNetwork.isMasterClient) {
				//Debug.Log ("Server Exec Only!! doHandleChi()");
				return;
			} else {
				if (abanMahId != 0) {
					PhotonPlayer photonPlayer = PhotonPlayer.Find(playerid);
					if (photonPlayer != null && photonPlayer.mahPlayer != null) {
						photonPlayer.mahPlayer.handleChi (abanMahId);
					}
				}
			}
		}

        //處理玩家胡牌
        public void doHandleWin(int playerid)
        {
            if (!PhotonNetwork.isMasterClient)
            {
                //Debug.Log ("Server Exec Only!! doHandledWin()");
                return;
            }
            else
            {
                if (abanMahId != 0)
                {
                    PhotonPlayer photonPlayer = PhotonPlayer.Find(playerid);
                    if (photonPlayer != null && photonPlayer.mahPlayer != null)
                    {
                        photonPlayer.mahPlayer.handleWin(abanMahId);
                    }
                }
            }
        }

        public void doHandleDaPai(int playerid, int mid) {
			//Debug.LogError ("[s] doHandleDaPai("+playerid+", "+mid+")");
			if (!PhotonNetwork.isMasterClient) {
				//Debug.LogError ("Server Exec Only!! doHandleDaPai()");
				return;
			} else {
				if (mid != 0) {
					if (_activePlayer != null && _activePlayer.photonPlayer.ID==playerid && _activePlayer.photonPlayer.ID != _lastDaPaiPlayerID) {
						_lastDaPaiPlayerID = _activePlayer.photonPlayer.ID;
						_activePlayer.handleDaPai (mid);
						_lastDaPaiPlayerID = _activePlayer.photonPlayer.ID;
					}
				}
			}
		}

        //摸牌
		public void doHandleMoPai()
		{
			//yield return new WaitForSeconds(isfirst);
			StartCoroutine(_MoPaiCo(.0f));
		}

		private IEnumerator _MoPaiCo(float time = 0.1f) {

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
				if (_activePlayer != null && _activePlayer.photonPlayer.ID != _lastMoPaiPlayerID) {
					//_activePlayer.state = PLAYERSTATE.MOPAING;//更改為摸牌狀態
					//Debug.LogError ("[s] 4.doMoPai("+_activePlayer.photonPlayer.NickName+")");
					//Debug.Log("xxxxxx_activePlayer.state="+_activePlayer.state+" "+_activePlayer.photonPlayer.NickName);
					int GotID = getMahjongPai ();
					string name = Mahjong.getName (GotID);
					_lastMoPaiPlayerID = _activePlayer.photonPlayer.ID;
					//keepedMah.Add (GotID);
					//Debug.LogError ("[s] 4.doMoPai(GotID="+GotID+", isfirst="+isfirst+")");
					//Debug.LogError ("[s] 4. doMoPai("+_activePlayer.photonPlayer.NickName+", GotID="+GotID+")");
					_activePlayer.handleMoPai (GotID);
					//Debug.Log ("[s] "+_activePlayer.name+" 摸到了 "+name+"("+GotID+")");
					//object[] param = { GotID, _activePlayer.photonPlayer.ID };
					//photonView.RPC ("MoPai", PhotonTargets.All, param);
				} else {
					//if (xlocalPlayer != null) {
					//	xlocalPlayer.mahPlayer.HideMenu ();
					//}
				}
			}
        }

		private void setActivePlayer(MahPlayer player) {
			//Debug.LogError ("[s] setActivePlayer("+player.photonPlayer.NickName+")");
			//int[] param = { photonPlayer.ID };
			//photonView.RPC("ShowTimer", PhotonTargets.All, param);
			if (_activePlayer) {
				int max = 0;
				//Debug.Log ("******************_activePlayer.state="+_activePlayer.state+" "+_activePlayer.photonPlayer.NickName);
				max = _activePlayer.keepedMah.Count-1;
				//Debug.LogError ("max="+max);
				if (_activePlayer.state == PLAYERSTATE.PLAYING) {//如果是正在做決策狀態, 先摸牌 再打牌
					doHandleMoPai ();//摸牌
					max = _activePlayer.keepedMah.Count-1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);//打牌
				}
				else if (_activePlayer.state == PLAYERSTATE.MOPAING) {//如果是摸牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count-1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				}
				else if (_activePlayer.state == PLAYERSTATE.CHIPAING) {//如果是吃牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count-1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				}
				else if (_activePlayer.state == PLAYERSTATE.PONPAING) {//如果是碰牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count-1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				}
				else if (_activePlayer.state == PLAYERSTATE.GANPAING) {//如果是槓牌狀態, 直接打牌
					max = _activePlayer.keepedMah.Count-1;
					_activePlayer.handleDaPai (_activePlayer.keepedMah [max]);
				}
				_activePlayer.state = PLAYERSTATE.WAITING;//更改狀態為完成狀態
			}
			_activePlayer = player;
			_activePlayer.actived = false;
			_activePlayer.state = PLAYERSTATE.PLAYING;
			int[] param = { _activePlayer.photonPlayer.ID };
			photonView.RPC("ShowActivePlayer", PhotonTargets.All, param);
			_lastMoPaiPlayerID = -1;
			_lastDaPaiPlayerID = -1;
			//StopCoroutine ("AutoChangeActivePlayerCo");
			StartCoroutine (AutoChangeActivePlayerCo());
			//doMoPai (isfirst);
		}

		private IEnumerator AutoChangeActivePlayerCo()
		{
			_remainTime = MaxWaitTime;
			while (_remainTime>0) {
				yield return new WaitForSeconds (1);
				_remainTime--;
				//Debug.Log ("waitTime="+_waitTime);
				int[] param = { _remainTime, _activePlayer.photonPlayer.ID };
				photonView.RPC ("RemainTime", PhotonTargets.All, param);
			}
			doHandleNext ();
			//StopCoroutine ("AutoChangeActivePlayerCo");
		}

		public int getMahjongPai()
		{
			//牌光了 游戏结束
			if (mahJong.allMah.Count == 0)
			{
				photonView.RPC("GameOver", PhotonTargets.All, null);
				return 0;
			}
			int ID = 0;
			ID = mahJong.allMah.Dequeue ();
			return ID;
		}

		[PunRPC]
		private void SendRequestToServer(int[] param)
		{
			int index = (int)param[0];
			int player_id = -1;
			switch(index) {
			//case (int)GameCommand.ACTIVENEXT:
				//doActiveNext ();
			//	break;
			case (int)GameCommand.MOPAI:
				doHandleMoPai ();
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
        }

		[PunRPC]
		private void dispatchPai(object[] param)
		{
			//Debug.LogError ("[RPC] 發牌()");
			int id = (int)param[0];
			int[] content = (int[])param [1];
			//Debug.LogError ("[RPC] 發牌("+id+":"+content.Length+")");

			PhotonPlayer photonPlayer = PhotonPlayer.Find(id);
			if (photonPlayer != null && photonPlayer.mahPlayer != null) {
				List<int> mahs = new List<int>(content);
				if (photonPlayer.IsLocal) {
					photonPlayer.mahPlayer.keepedMah = mahs;
					//Debug.LogError ("[RPC] 系統發牌給你("+photonPlayer.NickName+"), 張數:" + content.Length);
					photonPlayer.mahPlayer.ShowPAI ();
				} else {
					photonPlayer.mahPlayer.keepedMah = mahs;
					//Debug.LogError ("[RPC] 系統發牌給" + photonPlayer.NickName + ", 張數:" + content.Length);
					//foreach(int mid in mahs) {
					//	Debug.Log ("[c] "+mid);
					//}
				}
				//for (int i = 0; i < 13 * 4; i++)
				//{
				//	mahJong.allMah.RemoveAt(0);
				//}
				//photonPlayer.mahPlayer.timer.time = 10;
				//photonPlayer.mahPlayer.timer.Show ();
			}
		}
		
		[PunRPC]
		private void RemainTime(int[] param)
		{
			int rtime = (int)param[0];
			int pid = (int)param[1];
			//Debug.LogError ("[RPC] id="+pid+", RemainTime=" + rtime);
			PhotonPlayer photonPlayer = PhotonPlayer.Find(pid);
			if (photonPlayer != null && photonPlayer.mahPlayer != null) {
				photonPlayer.mahPlayer.timer.time = rtime;
				photonPlayer.mahPlayer.timer.Show ();
			}
		}
        [PunRPC]
		void ShowActivePlayer(int[] param)
        {
			int id = (int)param[0];
			//Debug.LogError ("[RPC] 顯示玩家(id="+id+")");
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
			int i = 0;
			int j = 0;
			//moMahId = pai_id;
			//bool isfirst = (bool)param[2];
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

			abanMahId = pai_id;
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
		}


        [PunRPC]
        void WinPai(int[] param)
        {
			//Debug.Log ("[RPC] 贏牌()");
            Text text = OverPanel.GetComponentInChildren<Text>();
            //OverPanel.gameObject.SetActive(true);
            //nagiEffectPlayerB.ShowNagi(Nagieffect.NagiType.PAU);
            StartCoroutine(_OverPanel(5f));

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
			//Debug.Log ("OneMoreMahjong()");
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

            //1.洗牌
            doShuffleMah();

            //2.摇骰子
            doDice();

            //3.发牌
            doDealingMahs();

			//4.設定開始玩家
			//setActivePlayer(players[0]);
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
            AllCanvasAnim.SetBool("GameStart", true);
            Invoke("ShowInfo", 0.45f);
        }

        public void ShowInfo()
        {
            GameInfoAnim.SetTrigger("ShowInfo");
        }

        public void GameStop()
        {
            AllCanvasAnim.SetBool("GameStart", false);
        }
    }
}