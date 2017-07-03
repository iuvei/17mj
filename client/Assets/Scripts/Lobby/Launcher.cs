using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace com.Lobby
{
    public class Launcher : Photon.PunBehaviour
    {

        #region PUBLIC

        //客户端版本
        public string _gameVersion = "1.0";

        //玩家名字
        public Text nameField;

        public GameObject lobbyPanel;

        public GameObject waitroomPanel;

		public GameObject loadingPanel;

		public GameObject settingPanel;

		public GameObject roomlistPanel;

        //房间列表
        public RectTransform LobbyPanel;

        //玩家列表
        public RectTransform playersPanel;

		public RectTransform rListPanel;

        //退出房间按钮
        public Button btnExit;

        //开始按钮
        public Button btnStart;

		public List<PhotonPlayer> Players = new List<PhotonPlayer>();
        #endregion

        #region PRIVATE 
		private string _roomname = string.Empty;
		private byte _roommax = 2;
        private bool isConnecting;
		private string BGM_name = "BGM_Lobby";
		private List<Button> buttons = new List<Button>();
        #endregion

        private void Awake()
        {
            //#不重要
            //强制Log等级为全部
			PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;

			string[] ran_names = {
				"雲盤金城武",
				"高雄彭玉燕",
				"鼓山張學友",
				"唐山綾波零",
				"成大金城武",
				"韓國林志穎",
				"釜山林志玲",
				"左營林志玲",
				"太極張三豐",
				"三民陳金城",
				"台南李炳輝",
				"彰化波多野",
				"屏東張韶涵",
				"台北郭金發",
				"基隆日本橋"
			};
			int idx = UnityEngine.Random.Range(0, ran_names.Length-1 );
			PhotonNetwork.playerName = ran_names[idx];

            //#关键
            //我们不加入大厅 这里不需要得到房间列表所以不用加入大厅去
            PhotonNetwork.autoJoinLobby = true;

            //#关键
            //这里保证所有主机上调用 PhotonNetwork.LoadLevel() 的时候主机和客户端能同时进入新的场景
            PhotonNetwork.automaticallySyncScene = true;
            if (loadingPanel)
            {
                loadingPanel.SetActive(true);
            }
        }

        void Start()
        {
			setProcess (0.1f);
            Connect();
			AudioManager.Instance.PlayBGM (BGM_name);
            SetPlayerName();

            lobbyPanel.transform.DOScaleY(1, 1f);
            btnExit.onClick.AddListener(delegate { StartCoroutine(ExitRoom()); });
            btnStart.onClick.AddListener(delegate { StartGame(); });
        }

        /// <summary>
        /// 连接到大厅
        /// </summary>
        private void Connect()
        {
            isConnecting = true;
			setProcess (0.1f);
            //已經連接上了服務器
            if (PhotonNetwork.connected)
            {
				setProcess (1.0f);
                //Debug.Log("Connected");
				if (loadingPanel) {
					loadingPanel.SetActive (false);
				}
            }
            else
            {
				setProcess (0.7f);
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }
        
        /// <summary>
        /// 成功连接到大厅
        /// </summary>
        public override void OnConnectedToPhoton()
        {
            base.OnConnectedToPhoton();
			setProcess (1.0f);
			//if (loadingPanel) {
				//loadingPanel.SetActive (false);
			//}
        }

        /// <summary>
        /// 连接大厅失败
        /// </summary>
        /// <param name="error"></param>
        private void OnFailedToConnect(NetworkConnectionError error)
        {
            Debug.Log("fail to Connect");
        }

        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("Launcher Create Room faileds");
        }

        public void SetPlayerName()
        {
			//Debug.Log ("SetPlayerName("+PhotonNetwork.player.NickName+")");
			nameField.text = PhotonNetwork.player.NickName;
        }

        public override void OnReceivedRoomListUpdate()
        {
			//Debug.Log("OnReceivedRoomListUpdate()");
			/*
            RoomInLobby[] ts = LobbyPanel.GetComponentsInChildren<RoomInLobby>();
            foreach (RoomInLobby t in ts)
            {
                Destroy(t.gameObject);
            }

            RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            foreach (RoomInfo room in rooms)
            {
                GameObject g = GameObject.Instantiate(Resources.Load("Lobby/RoomItem") as GameObject);
                RoomInLobby ril = g.GetComponent<RoomInLobby>();

                ril.t.text = room.Name;
                g.name = room.Name;
                g.transform.SetParent(LobbyPanel);
                g.transform.localScale = Vector3.one;
            }
            */
        }

        public override void OnJoinedRoom()
        {
			//Debug.Log ("OnJoinedRoom()");
			//SetPlayerName();
			this.Players = PhotonNetwork.playerList.ToList ();
			UpdateRoomInfo ();
            StartCoroutine(GetInRoom());
			/*
            Text[] ts = playersPanel.GetComponentsInChildren<Text>();
            foreach (Text t in ts)
            {
                Destroy(t.gameObject.transform.parent.gameObject);
            }
            PhotonPlayer[] players = PhotonNetwork.playerList;
            foreach (PhotonPlayer player in players)
            {
                GameObject g = GameObject.Instantiate(Resources.Load("Lobby/PlayerItem") as GameObject);
                Text t = g.transform.Find("Text").GetComponent<Text>();
                t.text = player.NickName;
                g.name = player.NickName;
                g.transform.SetParent(playersPanel);
                g.transform.localScale = Vector3.one;
            }
            */
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
			//Debug.Log ("OnPhotonPlayerConnected()");
			Players.Add (newPlayer);
			//Debug.Log ("cnt="+Players.Count);
			UpdateRoomInfo ();
			if (Players.Count >= _roommax) {
				StartGame ();
			}
            /*
			GameObject g = GameObject.Instantiate(Resources.Load("Lobby/PlayerItem") as GameObject);
            Text t = g.transform.Find("Text").GetComponent<Text>();
            t.text = newPlayer.NickName;
            g.name = newPlayer.NickName;
            g.transform.SetParent(playersPanel);
            g.transform.localScale = Vector3.one;
            */
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
			Debug.Log ("OnPhotonPlayerDisconnected()");
			Players.Remove (otherPlayer);
			UpdateRoomInfo ();
			ExitRoom ();
            //GameObject g = playersPanel.FindChild(otherPlayer.NickName).gameObject;
            //Destroy(g);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom()
        {
			Debug.Log ("CreateRoom()");
            if (PhotonNetwork.connected)
            {
				_roomname = "mj_room";
                //创建房间成功
				if (PhotonNetwork.CreateRoom(_roomname, new RoomOptions { MaxPlayers = _roommax }, null))
                {
					Debug.Log("Launcher.CreateRoom() 成功");

                    StartCoroutine(GetInRoom());
                }
            }
        }

		/// <summary>
		/// 顯示房間列表
		/// </summary>
		public void ShowRoomList()
		{
			Debug.Log ("GetRoomList()");
			if (roomlistPanel) {
				roomlistPanel.SetActive (true);
				//Debug.Log ("CreateRoom()");
				StartCoroutine(reloadRoomlist());
			}
		}

		/// <summary>
		/// Hides the room list.
		/// </summary>
		public void HideRoomList()
		{
			Debug.Log ("HideRoomList()");
			if (buttons.Count > 0) {
				foreach (Button bu in buttons) {
					Destroy (bu.gameObject);
				}
				buttons.Clear ();
			}
			if (roomlistPanel) {
				roomlistPanel.SetActive (false);
			}
		}

		/// <summary>
		/// 加入或者建立房間
		/// </summary>
		public void JoinOrCreateRoom()
		{
			//Debug.Log ("JoinOrCreateRoom()");
			if (PhotonNetwork.connected)
			{
				_roomname = "mj_room";
				//创建房间成功
				if (PhotonNetwork.JoinOrCreateRoom(_roomname, new RoomOptions { MaxPlayers = _roommax }, null))
				{
					//Debug.Log("[s] Launcher.JoinOrCreateRoom() 成功");

					StartCoroutine(GetInRoom());
				}
			}
		}
        
        /// <summary>
        /// 開始遊戲
        /// </summary>
        void StartGame()
        {
            if (!PhotonNetwork.isMasterClient)
            {
				Debug.Log ("!PhotonNetwork.isMasterClient");
                return;
            }
			//Debug.LogError ("[s] StartGame()");
			if (PhotonNetwork.playerList.Length >= 2) {
				PhotonNetwork.LoadLevel ("03.Room");
			} else {
				Debug.Log ("[s] PhotonNetwork.playerList.Length <2");
			}
        }

        /// <summary>
        /// 退出房間
        /// </summary>
        /// <returns></returns>
        IEnumerator ExitRoom()
        {
			Debug.Log ("ExitRoom()");
			if (waitroomPanel) {
				waitroomPanel.transform.DOScaleY (0, 0.8f);
				PhotonNetwork.LeaveRoom ();
				yield return new WaitForSeconds (1);
				waitroomPanel.SetActive (false);
				this.Players.Clear ();
			}
            //PhotonNetwork.LeaveRoom();
			//
            //yield return new WaitForSeconds(1f);
            //lobbyPanel.transform.DOScaleY(1, 1f);
        }

        /// <summary>
        /// 進入房間
        /// </summary>
        /// <returns></returns>
        IEnumerator GetInRoom()
        {
			Debug.Log ("GetInRoom()");
            //lobbyPanel.transform.DOScaleY(0,.8f);
			if (waitroomPanel) {
				waitroomPanel.SetActive (true);
				yield return new WaitForSeconds (1f);
				waitroomPanel.transform.DOScaleY (1, 1f);
			}
        }

		IEnumerator reloadRoomlist()
		{
			Text hint = rListPanel.parent.GetComponentInChildren<Text> ();
			if (hint) {
				hint.gameObject.SetActive (true);
				hint.text = "載入中...";
			}
			//hint.gameObject.SetActive (false);
			yield return new WaitForSeconds(1.0f);

			if (PhotonNetwork.connected) {
				RoomInfo[] lists = PhotonNetwork.GetRoomList ();
				if (lists.Length > 0) {
					foreach (RoomInfo ri in lists) {
						GameObject g = GameObject.Instantiate (Resources.Load ("Lobby/RoomItem") as GameObject);
						g.transform.parent = rListPanel;
						g.transform.localScale = Vector3.one;
						g.transform.localPosition = Vector3.zero;
						Text txt = g.GetComponentInChildren<Text> ();
						txt.text = ri.Name;
					}
					if (hint) {
						hint.gameObject.SetActive (false);
					}
				} else {
					if (hint) {
						hint.text = "目前沒有任何人開桌...";
						//hint.gameObject.SetActive (false);
					}
				}
			}
		}

		public void setProcess(float t)
		{
			if (loadingPanel) {
				Text xx = loadingPanel.transform.Find ("processbar/processtext").GetComponent<Text> ();
				xx.text = (t*100).ToString ()+"%";
				Image pp = loadingPanel.transform.Find ("processbar/process").GetComponent<Image> ();
				pp.fillAmount = t;
				if(t>=1) {
					StartCoroutine(HideLoading());
				}
			}
		}

		IEnumerator HideLoading()
		{
			//Debug.Log ("HideLoading()");
			//lobbyPanel.transform.DOScaleY(0,.8f);
			yield return new WaitForSeconds(1f);
			loadingPanel.SetActive(false);
			StopCoroutine (HideLoading());
			//roomPanel.transform.DOScaleY(1, 1f);
		}

		public void UpdateRoomInfo()
		{
			if (waitroomPanel) {
				Text tt = waitroomPanel.transform.Find ("RoomText").GetComponent<Text> ();
				tt.text = _roomname + " "+Players.Count + " / " + _roommax;
			}
		}

		public void ShowSetting()
		{
			if (settingPanel) {
				settingPanel.SetActive(true);
			}
		}

		public void HideSetting()
		{
			if (settingPanel) {
				settingPanel.SetActive(false);
			}
		}
    }
}