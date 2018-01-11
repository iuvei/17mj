﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using ExitGames.Client.Photon.Encry;
using System.Net;

namespace com.Lobby
{
    public class Launcher : Photon.PunBehaviour
    {

        #region PUBLIC
        public static Launcher instance;
        //客户端版本
        public string _gameVersion = "1.0";

        //玩家名字
        public Text nameField;

        public GameObject lobbyPanel;
        public GameObject waitroomPanel;
		public GameObject loadingPanel;
		//public GameObject settingPanel;
        public RectTransform settingSign;
        public GameObject settingDropdown;
        public GameObject settingPanelNew;
        public RectTransform popupLogout;

        public GameObject roomlistPanel;
        public GameObject roomlistPopupSetting;

        public GameObject createPopupPanel;

        public GameObject shopPanel;
        public Transform  shopItemTarget;
        public RectTransform popupBuy;
        public RectTransform popupSad;

        public GameObject depositPanel;

        public GameObject bagPanel;
        public Transform  bagItemTarget;

        public GameObject rankPanel;
        public Transform  rankItemTarget;
        public Scrollbar  rankScrollbar;

        public GameObject activityPanel;
        public RectTransform coinAdSign;

        public GameObject balloonPanel;
        //public bool coinAPIcallback = true;

        public Transform[] playRoomBtns;
        public Transform[] btmMenuBtns;

        public GameObject connectingPanel;
        public GameObject actionDonePanel;
        public GameObject dailyBonusPanel;
        public Transform dailyBonusTarget;

        public Image[] playerPhotos;
		public Text[] playerNames;
        public Text[] playerCoins;
        public Text[] playerLvs;
        public Text userOnline;

		public GameObject alertPanel;

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

        public enum SubPage { Main, List, Shop, Deposit, Bag, Rank, Active, Setting };
		public GameObject storyPanel;
        #endregion

        public Logout _logoutScript;

        #region PRIVATE 
        private string _roomname = string.Empty;
		private byte _roommax = 2;
        private bool isConnecting;
		private string BGM_name = "BGM_Lobby";
		private List<Button> buttons = new List<Button>();
        private Text hint;
        private Toggle toggleAll;
        private GameObject roomlistEmpty;
        private int[] _createRoomTypeIndex = new int[] {0,0,0};
        private string[] _createRoomChipType = new string[] { "200 / 100","500 / 200","1000 / 500"};
        private string[] _createRoomCircleType = new string[] { "1 圈", "2 圈" };
        private string[] _createRoomTimeType = new string[] { "3 秒", "5 秒", "7 秒" };
        private string[][] _createRoomType;
        private Text _popupBuyItemName;
        private Text _popupBuyItemPrice;
        private InputField _popupBuyItemNum;
        private Text _popupBuyItemTotal;
        private int currentNum = 1;
        private int currentPrice;
        private GameObject childSetting;
        private GameObject childSettingService;
        private GameObject childSettingRule;
        private GameObject childSettingToturial;
        private GameObject recordPanel;
        private GameObject gamePanel;
        private GameObject profilePanel;
        private GameObject depositRecoPanel;
        private GameObject coinRecoPanel;
        private Transform  servicePopup;
        private Sequence mySequence;
        private GameObject actDailyPanel;
        private GameObject actMissionPanel;
        private Transform playerRankPanel;
        private Text playerRankText;
        private Text playerRankLv;
        private Text playerRankWin;
        private Text playerRankLose;
        private Text playerRankProb;
        private ScrollRect settingProfileSR;
        private Scrollbar settingProfileScr;
        private Button replaceNickname;
        private InputField settingNickname;
		private InputField settingAccount;
        private Image settingLoginTypeImg;
        private Text settingLoginTypeTxt;
        private GameObject setLoginHint;
        private Button joinMenber;
        private Button bindAccount;
        private Button replacePass;
        private GameObject settingPanelBind;
        private GameObject settingPanelPass;
        private InputField settingBindMail;
        private InputField settingBindPass1;
        private InputField settingBindPass2;
        private InputField settingReplacePassOld;
        private InputField settingReplacePass1;
        private InputField settingReplacePass2;
        private Text settingBindCbHint;
        private Text settingRePassCbHint;
        private Transform _connectingSign;
        private Text _connectingText;
        private Text _actionText;
        private Transform popupDailyBonus;
        private Text popupDailyBonusText;
        private Image dailyBonuGirlEye;
        private Image dailyBonuSparkle;
        private DailyBonus dailyBonusToday;
        private Image btmDepositLight;
        private bool _changeFlag = true;
        private int localCoin = 0;
        private List<string> rankTotalDatas = new List<string>();
        private List<string> roomTotalDatas = new List<string>();
        private bool _setCoin = false;
        private bool _resetUserType = false;
        private bool _resetPass = false;
        private bool _resetName = false;
        private bool _getOnline = false;
        private bool _setItem = false;
        private bool _getBag = false;
        private bool _getRank = false;
        private bool _getRoomPhoto = false;
        private bool _needHideConnect = false;
        private bool _setUserPhoto = false;
        private string setCoinResult = string.Empty;
        private string resetUserTypeResult = string.Empty;
        private string resetPassResult = string.Empty;
        private string resetNameResult = string.Empty;
        private string getItemNumResult = string.Empty;
        private string setItemResult = string.Empty;
        private string getOnlineResult = string.Empty;
        private string getRankResult = string.Empty;
        private string getRoomPhotoResult = string.Empty;
        private string setUserPhotoResult = string.Empty;
        private int _randomOnlineUsers;
        private int _currentItemId = 0;
        private SubPage currentPage = SubPage.Main;
        private RoomInfo[] _roomLists;
        private bool _ERRLogout = false;


        [SerializeField]
        private Unimgpicker imagePicker;
        #endregion


        private void Awake()
        {
            instance = this;

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
			//Debug.Log ("name="+PhotonNetwork.playerName);
			//Debug.Log ("nickname="+PhotonNetwork.player.NickName);
			if (PhotonNetwork.player.NickName == "") {
				int idx = UnityEngine.Random.Range (0, ran_names.Length - 1);
				PhotonNetwork.player.NickName = ran_names [idx];
			}

            //#关键
            //我们不加入大厅 这里不需要得到房间列表所以不用加入大厅去
			PhotonNetwork.autoJoinLobby = true;

            //#关键
            //这里保证所有主机上调用 PhotonNetwork.LoadLevel() 的时候主机和客户端能同时进入新的场景
            PhotonNetwork.automaticallySyncScene = true;
            if (loadingPanel)
            {
                loadingPanel.SetActive(true);
                //Text xx2 = loadingPanel.transform.Find("processbar/Text").GetComponent<Text>();
                //mySequence = DOTween.Sequence();
                //mySequence.Append(xx2.DOText("載入中...", 3)).SetLoops(-1, LoopType.Restart);
            }
            //DontDestroyOnLoad (this.gameObject);

            imagePicker.Completed += (string path) =>
            {
                StartCoroutine(LoadImage(path));
            };
        }

        private IEnumerator LoadImage(string path)
        {
            var url = "file://" + path;
            var www = new WWW(url);
            yield return www;

            var texture = www.texture;
            if (texture == null)            
                Debug.LogError("Failed to load texture url:" + url);

            if (texture != null)
            {
                if (playerPhotos[2])
                {
                    Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    playerPhotos[2].sprite = sp;
                    CryptoPrefs.SetString("USERPHOTO", Convert.ToBase64String(texture.EncodeToJPG()));
                    //SetPlayerPhotos();

                    string sName = CryptoPrefs.GetString("USERNAME");
                    string sToken = CryptoPrefs.GetString("USERTOKEN");
                    string sPhoto = CryptoPrefs.GetString("USERPHOTO");
                    MJApi.setUserPhoto(sToken, sName, sPhoto, setPhotoCallback);
					AccountManager.Instance.ShowConnecting (); //開啟連線視窗
                }
            }          
        }

        public void setPhotoCallback(WebExceptionStatus status, string result)
        {
            _needHideConnect = true; //需要關閉連線視窗
            setUserPhotoResult = result;
            _setUserPhoto = true;
            if (status != WebExceptionStatus.Success) {
                setUserPhotoResult = "setPhotoCallback Failed!";
                //Debug.Log ("setPhotoCallback Failed! " + result);
			}
            //Debug.Log("setPhotoCallback =  " + result);
        }

        void Start()
        {
            //SubPageInit();
            setProcess (0.1f);
            Connect();
			AudioManager.Instance.PlayBGM (BGM_name);
            SetPlayerName();

            lobbyPanel.transform.DOScaleY(1, 1f);
            //btnExit.onClick.AddListener(delegate { StartCoroutine(ExitRoom()); });
            btnStart.onClick.AddListener(delegate { StartGame(); });

            hint = rListPanel.parent.GetComponentInChildren<Text>();

            
            SubPageInit();
            IniPlayerInfo();
            _createRoomType = new string[][] { _createRoomChipType, _createRoomCircleType, _createRoomTimeType };
            SettingInitAnim();
            InvokeRepeating("AutoBirthBalloon", 3f, 10f);
        }

		public override void OnJoinedLobby()
		{
			//PhotonNetwork.JoinRandomRoom();
			//Debug.Log("OnJoinedLobby()");
			//ShowAlert ("已進入遊戲大廳...");
			AccountManager.Instance.HideConnecting ();
		}

        /// <summary>
        /// 连接到大厅
        /// </summary>
        public void Connect()
        {
            isConnecting = true;
			setProcess (0.1f);
            //已經連接上了服務器
            if (PhotonNetwork.connected)
            {
				AccountManager.Instance.HideConnecting ();
				setProcess (1.0f);
                //Debug.Log("Connected");
				if (loadingPanel) {
					loadingPanel.SetActive (false);
                    //mySequence.Kill();
                }
            }
            else
            {
				AccountManager.Instance.ShowConnecting ();
				setProcess (0.7f);
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }
        
        /// <summary>
        /// 成功连接到大厅
        /// </summary>
        public override void OnConnectedToPhoton()
        {
			//Debug.Log("OnConnectedToPhoton()");
            base.OnConnectedToPhoton();
			setProcess (1.0f);
			AccountManager.Instance.HideConnecting ();
			//ShowAlert ("成功連接遊戲大廳...");
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
			Debug.LogError("OnFailedToConnect()");
			ShowAlert ("連接大廳失敗...網路重新連線...");
			Connect ();
        }

        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("Launcher Create Room faileds");
			Connect();
        }
        public void SetPlayerName()
        {
			//Debug.Log ("SetPlayerName("+PhotonNetwork.player.NickName+")");
			nameField.text = PhotonNetwork.player.NickName;
			//Debug.Log ("nickname="+PhotonNetwork.player.NickName);
        }

        public override void OnJoinedRoom()
        {
			AccountManager.Instance.HideConnecting ();
			Debug.Log ("OnJoinedRoom()");
			ShowAlert ("你進入房間...");
			//Debug.Log("OnJoinedRoom()");
			PhotonNetwork.LoadLevel ("03.Room");
			//SetPlayerName();
			this.Players = PhotonNetwork.playerList.ToList ();

            UpdateRoomInfo ();


            //StartCoroutine(GetInRoom());
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
            Debug.Log ("OnPhotonPlayerConnected()");
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
			Debug.Log ("OnPhotonPlayerDisconnected()"+otherPlayer.NickName+"斷線了");
			Players.Remove (otherPlayer);
			UpdateRoomInfo ();
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom()
        {
			//Debug.Log (this.name+".CreateRoom()");
			//AccountManager.Instance.ShowConnecting ();
			if (PhotonNetwork.connected) {
				AccountManager.Instance.ShowConnecting ();
				Hashtable customp = new Hashtable ();
				string cname = PhotonNetwork.player.NickName;

                customp.Add ("CRoomName", cname);
				string sToken = CryptoPrefs.GetString ("USERTOKEN");
				customp.Add ("UserToken", sToken);
				#if UNITY_IOS
                    _roomname = "MJ"+UnityEngine.Random.Range(0, 1000000000).ToString();
				#elif UNITY_ANDROID
				    _roomname = "AJ"+UnityEngine.Random.Range(0, 1000000000).ToString();
				#elif UNITY_STANDALONE
					_roomname = "SJ"+UnityEngine.Random.Range(0, 1000000000).ToString();
				#endif
				// 房間選項
				RoomOptions roomOptions = new RoomOptions ();
				roomOptions.isVisible = true;
				roomOptions.isOpen = true;
				roomOptions.maxPlayers = _roommax;
				roomOptions.customRoomProperties = customp;
				roomOptions.customRoomPropertiesForLobby = new string[] { "CRoomName","UserToken" };
				//Debug.Log ();
				//创建房间成功
				if (PhotonNetwork.CreateRoom (_roomname, roomOptions, null)) {
					//AccountManager.Instance.HideConnecting ();
					//Debug.Log (PhotonNetwork.room);
					Debug.Log ("房間創建成功!\n房間ID：" + _roomname);
					//ShowAlert ("房間創建成功!\n房間ID：" + _roomname);
					//StartCoroutine(ChangeRoom());
					//ChangeRoom();
					//ConnectPanelSwitch (true);
					AccountManager.Instance.ShowConnecting ();
				}
			} else {
				AccountManager.Instance.HideConnecting ();
				Debug.LogError ("網路連線失敗!!...重新連線中1...");
				ShowAlert ("網路連線失敗!!...重新連線中1...");
				Connect ();
				//CreateRoom ();
			}
        }

		/// <summary>
		/// 顯示房間列表
		/// </summary>
		public void ShowRoomList()
		{
			//Debug.Log ("ShowRoomList()");
            if(playRoomBtns[2])
                playRoomBtns[2].DOScale(0.95f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);
			if (!PhotonNetwork.connected) {
				Debug.LogError ("網路連線失敗!!...重新連線中...");
				ShowAlert ("網路連線失敗!!...重新連線中...");
				Connect ();
			}
            //Debug.Log ("GetRoomList()");
            if (roomlistPanel) {
				roomlistPanel.SetActive (true);

                if (CurrIsToggleAll())
                {
                    //Debug.Log ("CreateRoom()");
                    StartCoroutine(reloadRoomlist());
                }
            }
		}

		public void showStoryMap()
		{
			if (storyPanel) {
				storyPanel.SetActive (true);
			}
		}

		public void hideStoryMap()
		{
			if (storyPanel) {
				storyPanel.SetActive (false);
			}
		}

		public void playStoryMode(int level)
		{
			PhotonNetwork.LoadLevel("04.Story");
		}

		/// <summary>
		/// Hides the room list.
		/// </summary>
		public void HideRoomList()
		{
            foreach (Transform child in rListPanel)
                Destroy(child.gameObject);

            //Debug.Log ("HideRoomList()");
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
			/*
            if (playRoomBtns[1])
                playRoomBtns[1].DOScale(0.95f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

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
			*/
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
		/*
        /// <summary>
        /// 退出房間
        /// </summary>
        /// <returns></returns>
        IEnumerator ExitRoom()
        {
			Debug.Log ("ExitRoom()");
			ShowAlert ("ExitRoom()");
			if (waitroomPanel) {
				waitroomPanel.transform.DOScaleY (0, 0.8f);
				AccountManager.Instance.ShowConnecting ();
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
		*/

        /// <summary>
        /// 進入房間getItemCallback
        /// </summary>
        /// <returns></returns>
        IEnumerator GetInRoom()
        {
			Debug.Log ("GetInRoom()");
			ShowAlert ("GetInRoom()");
            //lobbyPanel.transform.DOScaleY(0,.8f);
			if (waitroomPanel) {
				waitroomPanel.SetActive (true);
				yield return new WaitForSeconds (1f);
				waitroomPanel.transform.DOScaleY (1, 1f);
			}
        }

		/*
		void ChangeRoom()
		{
			Debug.Log ("ChangeRoom()");
			//yield return new WaitForSeconds (1.0f);
			PhotonNetwork.LoadLevel ("03.Room");
		}
		*/

		/*
		public void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom()");
			PhotonNetwork.LoadLevel ("03.Room");
		}
		*/

		public void OnCreatedRoom()
		{
			//Debug.Log("OnCreatedRoom()");
			AccountManager.Instance.HideConnecting ();
			//PhotonNetwork.LoadLevel("Stage01");
			PhotonNetwork.LoadLevel ("03.Room");
			AccountManager.Instance.ShowConnecting ();
		}

		public void getPhotoCallback(WebExceptionStatus status, string result)
		{
            getRoomPhotoResult = result;
            _getRoomPhoto = true;

            if (status != WebExceptionStatus.Success)
			{
                getRoomPhotoResult = "getRoomPhotoCallback Failed!";
                Debug.Log("getRoomPhotoCallback Failed! " + result);
			}
            Debug.Log("getRoomPhotoCallback " + result);
        }


		IEnumerator reloadRoomlist()
		{
			//Text hint = rListPanel.parent.GetComponentInChildren<Text> ();
			if (hint) {
				hint.gameObject.SetActive (true);
				hint.text = "載入中...";
			}
			if (rListPanel != null) {
				int childs = rListPanel.childCount;
				for (int i = childs-1; i >=0; i--)
				{
					GameObject.DestroyImmediate(rListPanel.GetChild(i).gameObject);
				}
			}
			//hint.gameObject.SetActive (false);
			yield return new WaitForSeconds(1.0f);

			string sPName = string.Empty;

			if (PhotonNetwork.connected) {
                _roomLists = PhotonNetwork.GetRoomList();
                if (_roomLists.Length > 0) {
					foreach (RoomInfo ri in _roomLists) {
                        /*
						GameObject g = GameObject.Instantiate (Resources.Load ("Lobby/RoomItem") as GameObject);
						g.transform.SetParent(rListPanel,false);
						g.transform.localScale = Vector3.one;
						g.transform.localPosition = Vector3.zero;
                        Text txtName = g.transform.Find("Name").GetComponent<Text>();
                        Text txtNum = g.transform.Find("People/Num").GetComponent<Text>();
                        
                        int player_count = ri.PlayerCount;
                        */
                        Hashtable cp = ri.CustomProperties;

                        /*
                        if (cp ["CRoomName"] != null) {
							string name = cp ["CRoomName"].ToString ();
                            //txtName.text = name;
                            //txtNum.text = String.Format("{0:#,0}", player_count);
                            //Debug.Log("API 中 塞入順序 name = " + name);
                        }
                        */

                        if (cp ["UserToken"] != null) {
							string token = cp ["UserToken"].ToString ();

							if (string.IsNullOrEmpty (sPName)) {
								sPName = token;
							} else {
								sPName = sPName + "," + token;
							}
                        }

                        /*
						Button bt = g.GetComponent<Button> ();
						bt.onClick.AddListener (delegate {
							joinRoom (ri.Name);
						});
                        */
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
			} else {
				Debug.LogError ("網路連線失敗!!...重新連線中...");
				ShowAlert ("網路連線失敗!!...重新連線中...");
				Connect ();
			}

			if (!string.IsNullOrEmpty (sPName)) {
                string sName = CryptoPrefs.GetString ("USERNAME");
				string sToken = CryptoPrefs.GetString ("USERTOKEN");
				sPName = sPName + ",";
				MJApi.getUserPhoto (sToken, sName, sPName, getPhotoCallback);
                AccountManager.Instance.ShowConnecting(); //開啟連線視窗
            }
		}

		private void joinRoom(string name) {
			if (PhotonNetwork.connected) {
				//ShowAlert ("進入房間中...");
				//Debug.Log ("joinRoom(" + name + ")");
				AccountManager.Instance.ShowConnecting ();
				PhotonNetwork.JoinRoom (name);
			} else {
				AccountManager.Instance.HideConnecting ();
				Debug.LogError ("網路連線失敗!!...重新連線中...");
				ShowAlert ("網路連線失敗!!...重新連線中...");
				Connect ();
			}
		}

		public void setProcess(float t)
		{
			if (loadingPanel) {
				Text xx = loadingPanel.transform.Find ("processbar/processtext").GetComponent<Text> ();
                xx.text = (t*100).ToString ()+"%";
                Image pp = loadingPanel.transform.Find ("processbar/process").GetComponent<Image> ();
                Transform _girl = loadingPanel.transform.Find("FoxGirl_tail");
                if (_girl) _girl.localPosition = new Vector2(-500 + t * 1000, -140);
                pp.fillAmount = t;
				if(t>=1) {
					StartCoroutine(HideLoading());
                    mySequence.Kill();
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

            string uFirstLogin = CryptoPrefs.GetString("USERFLOGIN");
            //Debug.Log("初次登入嗎 " + uFirstLogin + " / 本月登入次數 " + uTotalLogin);
            if(uFirstLogin == "T")
                Invoke("DailyFirstLogin", 0.8f); // 出現每日登入獎勵畫面
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
            if (settingSign)
                settingSign.DOScale(1.3f, 0.15f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            //if (settingPanel) {
            //	settingPanel.SetActive(true);
            //}

            ShowSettingDropdown();
        }

		public void HideSetting()
		{
			//if (settingPanel) {
			//	settingPanel.SetActive(false);
			//}
		}

        public void ClickDespoit()
        {
            if (btmMenuBtns[2])
                btmMenuBtns[2].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            if (depositPanel)
            {
                depositPanel.transform.DOMoveY(-11, 0, true);
                depositPanel.transform.DOMoveY(0, 0.5f, true).SetEase(Ease.OutCubic);
            }
        }

        public void ExitDespoit()
        {
            if (depositPanel)
            {
                depositPanel.transform.DOMoveY(0, 0, true);
                depositPanel.transform.DOMoveY(-11, 0.5f, true).SetEase(Ease.OutCubic);
            }
        }

        public void ClickShop()
        {
            if (btmMenuBtns[1])
                btmMenuBtns[1].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            currentPage = SubPage.Shop;
            SetPlayerCoins();

            if (shopPanel)
            {
                shopPanel.transform.DOMoveX(-19.5f, 0, true);
                shopPanel.transform.DOMoveX(0, 0.5f, true).SetEase(Ease.OutCubic);
            }
        }

        public void ExitShop()
        {
            if (shopPanel)
            {
                shopPanel.transform.DOMoveX(0, 0, true);
                shopPanel.transform.DOMoveX(-19.5f, 0.5f, true).SetEase(Ease.OutCubic);
                currentPage = SubPage.Main;
                SetPlayerCoins();
            }
        }

        public void ClickBag()
        {
            // 讀背包清單
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            MJApi.getUserItem(sToken, sName, 0, getItemCallback);
			AccountManager.Instance.ShowConnecting (); //開啟連線視窗

            if (btmMenuBtns[3])
                btmMenuBtns[3].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);
        }

        private void paintBagUI() {
            currentPage = SubPage.Bag;
            SetPlayerCoins();
            GameObject bagEmpty = bagPanel.transform.Find("empty").gameObject;

            foreach (Transform child in bagItemTarget)
                Destroy(child.gameObject);

            //玩家的背包資料
            List<ItemInfo> itemInfos = loadUserItemData();

            if (itemInfos.Count == 0)
            {
                if (bagEmpty)
                    bagEmpty.SetActive(true);
            }
            else
            {
                if (bagEmpty)
                    bagEmpty.SetActive(false);

                foreach (ItemInfo info in itemInfos)
                {
                    GameObject go = GameObject.Instantiate(Resources.Load("Prefab/bagItem") as GameObject);
                    BagItemInfo bagInfo = go.GetComponent<BagItemInfo>();
                    bagInfo.setInfo(info);

                    go.transform.SetParent(bagItemTarget);
                    RectTransform rectT = go.GetComponent<RectTransform>();
                    rectT.localPosition = Vector3.zero;
                    rectT.localScale = Vector3.one;
                }
            }

            if (bagPanel)
            {
                bagPanel.transform.DOMoveX(-19.5f, 0, true);
                bagPanel.transform.DOMoveX(0, 0.5f, true).SetEase(Ease.OutCubic);
            }
        }

        public void ExitBag()
        {
            if (bagPanel)
            {
                bagPanel.transform.DOMoveX(0, 0, true);
                bagPanel.transform.DOMoveX(-19.5f, 0.5f, true).SetEase(Ease.OutCubic);
                currentPage = SubPage.Main;
                SetPlayerCoins();
            }
        }

		public void getItemCallback(WebExceptionStatus status, string result)
		{
            _needHideConnect = true; //需要關閉連線視窗
            _getBag = true;
            Debug.Log("getItemCallback " + result);
            if (status != WebExceptionStatus.Success)
		    {
                getItemNumResult = "getItemCallback Failed!";
                Debug.Log("getItemCallback Failed! " + result);
			}
			else if (!string.IsNullOrEmpty (result))
		    {
                getItemNumResult = result;
                string[] tokens = result.Split(new string[] { "," }, StringSplitOptions.None);
                Debug.Log(" getItemCallback  =  " + result);
		    }
        }

        //讀取背包資料
        private List<ItemInfo> loadUserItemData()
        {
            string[] tokens = getItemNumResult.Split(new string[] { "," }, StringSplitOptions.None);
            int sItems = tokens.Length / 2;
            //Debug.Log(" 商品項目數量  =  " + sItems);

            ItemInfos itemInfos = new ItemInfos();
            if (sItems != 0) {
                for (int i = 0; i < sItems; i++)
                {
                    ItemInfo data = new ItemInfo();
                    data.Id = int.Parse(tokens[2*i]);
                    data.Name = ItemNameList(data.Id); 
                    data.Num = int.Parse(tokens[2*i+1]);
                    data.Path2D = data.Id;
                    itemInfos.dataList.Add(data);
                }
            }
            return itemInfos.dataList;
        }

        private string ItemNameList(int _id) {
            string _itemName = string.Empty;
            switch (_id) {
                case 1:
                    _itemName = "渡假別墅";
                    break;
                case 2:
                    _itemName = "高級腕錶";
                    break;
                case 3:
                    _itemName = "金元寶";
                    break;
                case 4:
                    _itemName = "I-RIMO鑽戒";
                    break;
                case 5:
                    _itemName = "高級房車";
                    break;
                case 6:
                    _itemName = "香奈兒包";
                    break;
            }
            return _itemName;
        }

        public void RoomListToggle(bool isOn)
        {
            string _target = EventSystem.current.currentSelectedGameObject.name;
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

            foreach (Transform child in rListPanel)
                Destroy(child.gameObject);

            if (roomlistEmpty)
                roomlistEmpty.SetActive(false);
            if (hint)
                hint.gameObject.SetActive(true);

            if (isOn)
            {
                switch (_target)
                {
                    case "Btn_all": //列表-全部
                        StartCoroutine(reloadRoomlist());
                        break;
                    case "Btn_follow"://列表-追蹤
                        StopCoroutine(reloadRoomlist());

                        if (roomlistEmpty) {
                            roomlistEmpty.SetActive(true);
                        }

                        if (hint)
                            hint.gameObject.SetActive(false);
                        break;
                }
            }
        }

        private bool CurrIsToggleAll() {
            bool _ans = false;
            if (toggleAll)
                _ans =  toggleAll.isOn ? true : false;

            return _ans;
        }

        public void ShowRoomListSetting()
        {
            GameObject popupBG = roomlistPanel.transform.Find("popupBG").gameObject;
            if (popupBG) {
                popupBG.SetActive(true);
                popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
            }
            roomlistPopupSetting.transform.DOScale(Vector3.zero, 0);
            roomlistPopupSetting.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }

        public void HideRoomListSetting()
        {
            GameObject popupBG = roomlistPanel.transform.Find("popupBG").gameObject;
            if (popupBG)
            {
                popupBG.GetComponent<Image>().DOFade(0, 0.3f);
                StartCoroutine(HideGameObject(popupBG, 0.3f));
            }

            ChangeRoomListSetting();
            roomlistPopupSetting.transform.DOScale(Vector3.one, 0);
            roomlistPopupSetting.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine);
        }

        IEnumerator HideGameObject(GameObject go, float _time)
        {
            if (_time > 0)
                yield return new WaitForSeconds(_time);

            if (go)
                go.SetActive(false);
        }

        public void ClickToWatchRoom() {
            if (toggleAll)
                toggleAll.isOn = true;
            StartCoroutine(reloadRoomlist());
        }

        public void ChangeRoomListSetting()
        {
            Text _text = roomlistPanel.transform.Find("BtnFilter/Text").GetComponent<Text>();
            Toggle[] tgSec = roomlistPopupSetting.transform.Find("sec").GetComponentsInChildren<Toggle>();
            Toggle[] tgType = roomlistPopupSetting.transform.Find("type").GetComponentsInChildren<Toggle>();

            foreach (Toggle tg in tgSec)
            {
                if (tg.isOn)
                    _text.text = "出牌" + tg.transform.Find("Background/Label").GetComponent<Text>().text;
            }

            foreach (Toggle tg in tgType)
            {
                if (tg.isOn)
                    _text.text += "、" + tg.transform.Find("Background/Label").GetComponent<Text>().text;
            }
        }

        public void ClickCreateAdd(int _type) {
            CreateRoomSetting(_type, true);
        }

        public void ClickCreateSubtract(int _type)
        {
            CreateRoomSetting(_type, false);
        }

        private void CreateRoomSetting(int _type, bool isAdd) {
            string _targetP = "";
            string[] _targetType = new string[] { };

            if (_type == 0)
            {
                _targetP = "Chip";   // 底台
            }
            else if (_type == 1)
            {
                _targetP = "Circle"; // 圈數
            }
            else if (_type == 2)
            {
                _targetP = "Time";   // 秒數
            }

            Text _text = createPopupPanel.transform.Find("content/" + _targetP + "/numBg/Text").GetComponent<Text>();

            if (isAdd)
                _createRoomTypeIndex[_type] = (_createRoomTypeIndex[_type] + 1 > _createRoomType[_type].Length - 1) ? 0 : _createRoomTypeIndex[_type] + 1;
            else
                _createRoomTypeIndex[_type] = (_createRoomTypeIndex[_type] - 1 < 0) ? _createRoomType[_type].Length - 1 : _createRoomTypeIndex[_type] - 1;

            if (_text)
                _text.text = _createRoomType[_type][_createRoomTypeIndex[_type]];
        }

        public void ClickConfirmCreateRoom() {
            //for (int i = 0; i < _createRoomType.Length ; i++)
            //{
                //Debug.Log("開房設定: " + _createRoomType[i][_createRoomTypeIndex[i]] + " ; ");
            //}

            CreateRoom();
        }


        public void ShowCreatRoomSetting()
        {
            if (playRoomBtns[0])
                playRoomBtns[0].DOScale(0.95f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            GameObject popupBG = createPopupPanel.transform.parent.Find("popupBG").gameObject;
            if (popupBG)
            {
                popupBG.SetActive(true);
                popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
            }
            createPopupPanel.transform.DOScale(Vector3.zero, 0);
            createPopupPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }

        public void HideCreatRoomSetting()
        {
            GameObject popupBG = createPopupPanel.transform.parent.Find("popupBG").gameObject;
            if (popupBG)
            {
                popupBG.GetComponent<Image>().DOFade(0, 0.3f);
                StartCoroutine(HideGameObject(popupBG, 0.3f));
            }

            ChangeRoomListSetting();
            createPopupPanel.transform.DOScale(Vector3.one, 0);
            createPopupPanel.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine);
        }

		public void setItemCallback(WebExceptionStatus status, string result)
		{
            _needHideConnect = true; //需要關閉連線視窗
            _setItem = true;

            if (status != WebExceptionStatus.Success)
            {
                setItemResult = result;
                Debug.Log("setItemCallback Failed! " + result);
			}
            else if(!string.IsNullOrEmpty(result))
            {
                Debug.Log(" setItemCallback  =  " + result);
                string[] tokens = result.Split(new string[] { "," }, StringSplitOptions.None);
                if (tokens[0] == "OK")
                    setItemResult = tokens[1];
                else
                    setItemResult = "something wrong!";
            }

        }

        public void ClickShopBuy()
        {
            //GameObject popupBG = shopPanel.transform.Find("popupBG").gameObject;
            ShopItem _item = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<ShopItem>();
            //Debug.Log("name = " + _info.ItemName + "  price = " + _info.ItemPrice);
            currentNum = 1;
            currentPrice = _item.Price;
            _popupBuyItemName.text = _item.Name;
            _popupBuyItemPrice.text = String.Format("{0:0,0}", currentPrice);
            _popupBuyItemNum.text = currentNum.ToString();
            _popupBuyItemTotal.text = String.Format("{0:0,0}", currentPrice);
            _currentItemId = _item.Id;

            // 取得目前該商品數量
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            
            MJApi.getUserItem(sToken, sName, _currentItemId, getItemCallback);
			AccountManager.Instance.ShowConnecting ();

            //if (popupBuy)
            //{
            //    if (popupBG) {
            //        popupBG.SetActive(true);
            //        popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
            //    }
            //    popupBuy.transform.DOScale(Vector3.zero, 0);
            //    popupBuy.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            //}
        }

        public void ExitShopBuy()
        {
            GameObject popupBG = shopPanel.transform.Find("popupBG").gameObject;

            if (popupBuy)
            {
                popupBuy.transform.DOScale(Vector3.one, 0);
                popupBuy.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine);
                if (popupBG) {
                    popupBG.GetComponent<Image>().DOFade(0, 0.3f);
                    StartCoroutine(HideGameObject(popupBG, 0.3f));
                }
            }
        }

        public void ClickShopAdd()
        {
            if (currentNum < 99)
            {
                currentNum++;
                _popupBuyItemNum.text = currentNum.ToString();
                ShopCaculate();
            }
        }

        public void ClickShopSubtract()
        {
            if (currentNum > 1)
            {
                currentNum--;
                _popupBuyItemNum.text = currentNum.ToString();
                ShopCaculate();
            }
        }

        public void CheckWallet() {
            string sCoin = CryptoPrefs.GetString("USERCOIN"); //取得目前餘額
            int tPrice = (currentNum * currentPrice);
            if (int.Parse(sCoin) >= tPrice)
            {
                ClickBuyItem(-tPrice);    //改變商品數量
				AccountManager.Instance.ShowConnecting (); //開啟連線視窗
            }
            else {
                GoShopSad();
            }
        }

        public void ClickBuyItem(int _tPrice) {
            // 購買商品
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            int oldCoin = localCoin;
            int newCoin = localCoin + _tPrice;

            string currentItemNum = (string.IsNullOrEmpty(getItemNumResult)) ? "0" : getItemNumResult;
            int oldNum = int.Parse(currentItemNum);
            int finalNum = currentNum + oldNum;
            MJApi.setUserItem(sToken, sName, _currentItemId, oldNum, finalNum, oldCoin, newCoin, setItemCallback);

            Debug.Log("購買商品: 原始金幣: " + oldCoin + " ; 購買後餘額: " + newCoin + " ; 購買商品ID: " + _currentItemId + " ; 該商品持有數: " + oldNum + " ; 購買後持有數: " + finalNum);
        }

        private void SetUserCoin() {

        }

        public void GoShopSad()
        {
            popupSad.transform.DOScale(Vector3.zero, 0);
            popupSad.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
        }

        public void ExitShopSad()
        {
            popupSad.transform.DOScale(Vector3.one, 0);
            popupSad.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine);
        }

        private void ShopCaculate()
        {
            _popupBuyItemTotal.text = string.Format("{0:0,0}", (currentNum * currentPrice));
        }

        public void ChangeCurrentNum()
        {
            if (!int.TryParse(_popupBuyItemNum.text, out currentNum))
            {
                _popupBuyItemNum.text = "1";
                currentNum = 1;
            }
            else
                currentNum = int.Parse(_popupBuyItemNum.text);

            if (currentNum < 1)
            {
                _popupBuyItemNum.text = "1";
                currentNum = 1;
            }

            if (currentNum > 99)
            {
                currentNum = 99;
            }

            ShopCaculate();
        }

        public void BirtnShopItem()
        {
            foreach (Transform child in shopItemTarget)
                Destroy(child.gameObject);

            List<ShopItemInfo> shopItemInfos = loadShopData();
            foreach (ShopItemInfo info in shopItemInfos)
            {
                GameObject go = GameObject.Instantiate(Resources.Load("Prefab/shopItem") as GameObject);
                ShopItem shopItem = go.GetComponent<ShopItem>();
                shopItem.setInfo(info);

                shopItem.Name = info.Name;
                shopItem.Price = info.Price;
                go.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate{ ClickShopBuy();});

                go.transform.SetParent(shopItemTarget);
                RectTransform rectT = go.GetComponent<RectTransform>();
                rectT.localPosition = Vector3.zero;
                rectT.localScale = Vector3.one;
            }
        }

        private List<ShopItemInfo> loadShopData()
        {
            int priceIdx = 1; //物價指數
            ShopItemInfos itemInfos = new ShopItemInfos();
            ShopItemInfo data1 = new ShopItemInfo();
            data1.Id = 1;
            data1.Name = "渡假別墅";
            data1.Path2D = 1;
            data1.Price = 40 * priceIdx;
            itemInfos.dataList.Add(data1);

            ShopItemInfo data2 = new ShopItemInfo();
            data2.Id = 2;
            data2.Name = "高級腕錶";
            data2.Path2D = 2;
            data2.Price = 30 * priceIdx;
            itemInfos.dataList.Add(data2);

            ShopItemInfo data3 = new ShopItemInfo();
            data3.Id = 3;
            data3.Name = "金元寶";
            data3.Path2D = 3;
            data3.Price = 10 * priceIdx;
            itemInfos.dataList.Add(data3);

            ShopItemInfo data4 = new ShopItemInfo();
            data4.Id = 4;
            data4.Name = "I-RIMO鑽戒";
            data4.Path2D = 4;
            data4.Price = 130 * priceIdx;
            itemInfos.dataList.Add(data4);

            ShopItemInfo data5 = new ShopItemInfo();
            data5.Id = 5;
            data5.Name = "高級房車";
            data5.Path2D = 5;
            data5.Price = 50 * priceIdx;
            itemInfos.dataList.Add(data5);

            ShopItemInfo data6 = new ShopItemInfo();
            data6.Id = 6;
            data6.Name = "香奈兒包";
            data6.Path2D = 6;
            data6.Price = 20 * priceIdx;
            itemInfos.dataList.Add(data6);

            return itemInfos.dataList;
        }

        private void SettingInitAnim() {
            //設定頁齒輪動畫
            if (settingSign) {
                settingSign.DORotateQuaternion(Quaternion.Euler(0, 0, 30), 1f).SetEase(Ease.OutElastic).SetLoops(-1, LoopType.Yoyo);
            }

            //入口按鈕
            if (playRoomBtns != null) {
                playRoomBtns[0].DOLocalMoveY(-20, 1f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);
                playRoomBtns[1].DOLocalMoveY(-20, 1f).SetEase(Ease.InOutFlash).SetDelay(0.5f).SetLoops(-1, LoopType.Yoyo);
                playRoomBtns[2].DOLocalMoveY(-20, 1f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);

                //骰子
                Transform diceL = playRoomBtns[0].Find("01/btm/dice_L");
                Transform diceM = playRoomBtns[0].Find("01/btm/dice_M");
                Transform diceR = playRoomBtns[0].Find("01/btm/dice_R");
                Vector3[] waypoints = new[] { new Vector3(-2.467865f, -1.895259f, -2.020508f), new Vector3(-1.8596f, -1.465896f, -2.020508f), new Vector3(-1.269226f, -2.02049f, -2.020508f) };

                Sequence diceSeq1 = DOTween.Sequence();
                Sequence diceSeq2 = DOTween.Sequence();
                Sequence diceSeq3 = DOTween.Sequence();
                diceSeq1.PrependInterval(2);
                diceSeq1.Append(diceL.DORotate(new Vector3(0, 0, -15), 0.1f).SetEase(Ease.InFlash).SetLoops(2, LoopType.Yoyo));
                diceSeq1.SetLoops(-1, LoopType.Yoyo);

                diceSeq2.PrependInterval(2);
                diceSeq2.Append(diceM.DORotate(new Vector3(0, 0, -15), 0.1f).SetEase(Ease.InFlash).SetLoops(2, LoopType.Yoyo));
                diceSeq2.SetLoops(-1, LoopType.Yoyo);

                diceSeq3.PrependInterval(3);
                diceSeq3.Insert(0, diceR.GetComponent<SpriteRenderer>().DOFade(1, 0.2f));
                diceSeq3.Insert(0, diceR.DORotate(new Vector3(0, 0, 120), 0.3f).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo));
                diceSeq3.Insert(0, diceR.DOPath(waypoints, 0.8f).SetEase(Ease.InOutQuad));
                diceSeq3.Insert(2.7f, diceR.GetComponent<SpriteRenderer>().DOFade(0, 0.2f));
                diceSeq3.SetLoops(-1, LoopType.Restart);

                
                //中發白
                Transform cardChun = playRoomBtns[1].Find("02/card_chun");
                Transform cardFa = playRoomBtns[1].Find("02/card_fa");
                Transform cardBai = playRoomBtns[1].Find("02/card_bai");

                //cardChun.DOMove(new Vector3(2.51f, 0.51f, -2.1f), 1.5f).SetLoops(-1, LoopType.Yoyo); // 因應RWD
                cardChun.DOBlendableMoveBy(new Vector3(1.27f, 0.03f, -0.1f), 1.5f).SetLoops(-1, LoopType.Yoyo);
                cardChun.DORotate(new Vector3(0, 0, -24.552f), 1.5f).SetLoops(-1, LoopType.Yoyo);
                cardFa.DOScale(new Vector3(1.05f, 1.05f, 1), 1.5f).SetEase(Ease.OutElastic).SetLoops(-1, LoopType.Yoyo);
                //cardBai.DOMove(new Vector3(1.24f, 0.48f, -2f), 1.5f).SetLoops(-1, LoopType.Yoyo); // 因應RWD
                cardBai.DOBlendableMoveBy(new Vector3(-1.27f, -0.03f, 0.1f), 1.5f).SetLoops(-1, LoopType.Yoyo);
                cardBai.DORotate(new Vector3(0, 0, 20.797f), 1.5f).SetLoops(-1, LoopType.Yoyo);
            }

            //底部分頁
            if (btmMenuBtns != null) {
                btmMenuBtns[0].DOLocalMoveY(-15, 1f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);
                btmMenuBtns[1].DOLocalMoveY(-15, 1f).SetEase(Ease.InOutFlash).SetDelay(1).SetLoops(-1, LoopType.Yoyo);
                btmMenuBtns[2].DOLocalMoveY(-15, 1f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);
                btmMenuBtns[3].DOLocalMoveY(-15, 1f).SetEase(Ease.InOutFlash).SetDelay(1).SetLoops(-1, LoopType.Yoyo);
                btmMenuBtns[4].DOLocalMoveY(-15, 1f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);
            }

            //每日登入女模閉眼
            if (dailyBonuGirlEye) {
                Sequence dBSeq = DOTween.Sequence();
                Vector3[] waypoints = new[] { new Vector3(6.1f, 3.24f, -1f), new Vector3(6.93f, 3.69f, -1f) };

                dBSeq.Append(dailyBonuGirlEye.DOFade(1, 0.3f).SetLoops(2, LoopType.Yoyo));
                dBSeq.Insert(0, dailyBonuSparkle.DOFade(1, 0.3f));
                dBSeq.Insert(0, dailyBonuSparkle.transform.DOScale(new Vector3(1, 1, 1), 0.6f));
                dBSeq.Insert(0, dailyBonuSparkle.transform.DORotate(new Vector3(0, 0, 30), 0.1f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo));
                //dBSeq.Insert(0.1f, dailyBonuSparkle.transform.DOPath(waypoints, 0.5f, PathType.CatmullRom, PathMode.TopDown2D, 5).SetEase(Ease.OutQuad)); //因應RWD 以下兩行
                dBSeq.Insert(0.1f, dailyBonuSparkle.transform.DOBlendableMoveBy(new Vector3(0.2f, -0.1f, -1f), 0.2f).SetEase(Ease.Linear));
                dBSeq.Insert(0.3f, dailyBonuSparkle.transform.DOBlendableMoveBy(new Vector3(1f, 0.6f, 0f), 0.3f).SetEase(Ease.OutQuad));
                dBSeq.Insert(0.5f, dailyBonuSparkle.DOFade(0, 0.1f));
                dBSeq.AppendInterval(1);
                dBSeq.SetLoops(-1, LoopType.Restart).SetUpdate(true);
            }

            //點廣告領金幣
            if (coinAdSign) {
                coinAdSign.DOLocalMoveY(30, 1.5f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo).Pause();
            }

            Transform depositeFlash = depositPanel.transform.Find("bg/flash");
            if(depositeFlash)
                depositeFlash.DOLocalRotate(new Vector3(0, 0, 180), 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

            //設定連線中動畫
            if (connectingPanel)
            {
                _connectingSign.DOLocalRotate(new Vector3(0, 0, 180), 1.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart); //.Pause();
                _connectingText.DOText("連線中...", 3).SetLoops(-1, LoopType.Restart); //.Pause();
            }

            if(btmDepositLight)
                InvokeRepeating("ChangeSpotSprite", .5f, .5f);
        }

        private void ChangeSpotSprite()
        {
            string _num = (_changeFlag) ? "1" : "2";
            if (btmDepositLight)
                btmDepositLight.sprite = Resources.Load<Sprite>("Image/menuL" + _num);

            _changeFlag = !_changeFlag;
  
        }

        public void ClickRank()
        {
            // 讀排行榜
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            MJApi.getRankList(sToken, sName, getRankDataCallback);
			AccountManager.Instance.ShowConnecting (); //開啟連線視窗

            if (btmMenuBtns[0])
                btmMenuBtns[0].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);
            

            //currentPage = SubPage.Rank;
            //SetPlayerCoins();
            //BirthRankItem(); //產生排行榜項目
            
            //if (rankPanel)
            //{
            //    rankPanel.transform.DOMoveX(-19.5f, 0, true);
            //    rankPanel.transform.DOMoveX(0, 0.5f, true).SetEase(Ease.OutCubic);
            //}
        }

        private void paintRankUI() {
            currentPage = SubPage.Rank;
            SetPlayerCoins();

            BirthRankItem(); //產生排行榜項目

            if (rankPanel)
            {
                rankPanel.transform.DOMoveX(-19.5f, 0, true);
                rankPanel.transform.DOMoveX(0, 0.5f, true).SetEase(Ease.OutCubic);
            }
        }


        public void ExitRank()
        {
            if (rankPanel)
            {
                rankPanel.transform.DOMoveX(0, 0, true);
                rankPanel.transform.DOMoveX(-19.5f, 0.5f, true).SetEase(Ease.OutCubic);
                currentPage = SubPage.Main;
                SetPlayerCoins();
            }
        }

        public void BirthRankItem()
        {
            foreach (Transform child in rankItemTarget)
                Destroy(child.gameObject);

            List<RankItemInfo> rankItemInfos = loadRankData();
            //rankItemInfos.Clear(); //清空
            foreach (RankItemInfo info in rankItemInfos)
            {
                GameObject go = GameObject.Instantiate(Resources.Load("Prefab/rankItem") as GameObject);
                RankItem rankItem = go.GetComponent<RankItem>();
                rankItem.setInfo(info);

                go.transform.SetParent(rankItemTarget);
                RectTransform rectT = go.GetComponent<RectTransform>();
                rectT.localPosition = Vector3.zero;
                rectT.localScale = Vector3.one;
            }
        }

        private List<RankItemInfo> loadRankData()
        {
            rankTotalDatas.Clear();
            string[] tokens = getRankResult.Split(new string[] { "," }, StringSplitOptions.None);
            int _oncePrint = 5;
            Debug.Log("排行榜共 " + tokens.Length / 7 + "筆資料");

            for (int i = 0; i < tokens.Length; i++)
                rankTotalDatas.Add(tokens[i]);

            // 填入自己的資料
            playerRankText.text = "我的排名\n" + rankTotalDatas[0];
            playerRankLv.text = "Lv " + rankTotalDatas[1];
            playerRankWin.text = string.Format("勝：{0:#,0}", rankTotalDatas[4]);
            playerRankLose.text = string.Format("敗：{0:#,0}", rankTotalDatas[5]);
            playerRankProb.text = string.Format("    勝率：{0:0.##}%", int.Parse(rankTotalDatas[6]) / 100f);
            rankTotalDatas.RemoveRange(0, 7);

            RankItemInfos itemInfos = new RankItemInfos();
            _oncePrint = (rankTotalDatas.Count >= 35) ? 5 : rankTotalDatas.Count / 7;

            return loadNextRankData(_oncePrint);
        }

        public void ReadyToLoadNextRank() {
            int num = (rankTotalDatas.Count >= 35) ? 5 : rankTotalDatas.Count / 7;
            CanvasGroup empty = rankPanel.transform.Find("empty").GetComponent<CanvasGroup>(); // 已無更多資料

            if (rankScrollbar.value == 0) {
                if (rankTotalDatas.Count > 0)
                {
                    List<RankItemInfo> rankItemInfos = loadNextRankData(num); // 生成下一批列表
                    foreach (RankItemInfo info in rankItemInfos)
                    {
                        GameObject go = GameObject.Instantiate(Resources.Load("Prefab/rankItem") as GameObject);
                        RankItem rankItem = go.GetComponent<RankItem>();
                        rankItem.setInfo(info);

                        go.transform.SetParent(rankItemTarget);
                        RectTransform rectT = go.GetComponent<RectTransform>();
                        rectT.localPosition = Vector3.zero;
                        rectT.localScale = Vector3.one;
                    }
                }
                else {
                    empty.DOKill();
                    empty.DOFade(1, 0.5f);
                    empty.DOFade(0, 0.2f).SetDelay(1.2f);
                }
            }

        }

        private List<RankItemInfo> loadNextRankData(int _oncePrint) {
            RankItemInfos itemInfos = new RankItemInfos();
            for (int i = 0; i < _oncePrint; i++) 
            {
                RankItemInfo data = new RankItemInfo();
                data.Rank = int.Parse(rankTotalDatas[7 * i]);
                data.Photo = rankTotalDatas[7 * i + 1];
                data.Name = rankTotalDatas[7 * i + 2];
                data.Lv = int.Parse(rankTotalDatas[7 * i + 3]);
                data.Win = int.Parse(rankTotalDatas[7 * i + 4]);
                data.Lose = int.Parse(rankTotalDatas[7 * i + 5]);
                data.Probability = int.Parse(rankTotalDatas[7 * i + 6]) / 100f;
                itemInfos.dataList.Add(data);
            }

            rankTotalDatas.RemoveRange(0, _oncePrint * 7);
            Debug.Log("rankDatas剩 " + rankTotalDatas.Count + "筆資料");

            /*
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
            
            for (int i = 0; i < 10; i++) // 一次產生十筆假名單
            {
                RankItemInfo data1 = new RankItemInfo();
                data1.Rank = cuurTotal + i + 1;
                //data1.Photo = 1;
                data1.Name = ran_names[UnityEngine.Random.Range(0, ran_names.Length - 1)];
                data1.Lv = UnityEngine.Random.Range(1,500);
                data1.Win = UnityEngine.Random.Range(80, 2000);
                data1.Lose = UnityEngine.Random.Range(10, 800);
                data1.Probability = UnityEngine.Random.Range(10f, 50f);
                itemInfos.dataList.Add(data1);
            }
            */
            return itemInfos.dataList;
        }

        public void ShowSettingDropdown() {
            Transform dropDown = settingDropdown.transform.Find("dropDown");
            GameObject popupBG = settingDropdown.transform.Find("bg").gameObject;
            Time.timeScale = 0;

            if (popupBG)
            {
                popupBG.SetActive(true);
                popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f).SetUpdate(true);
            }

            if (dropDown) {
                dropDown.DOMoveY(12.2f, 0, true);
                dropDown.DOMoveY(5.4f, 0.3f, true).SetEase(Ease.InSine).SetUpdate(true);
            }
        }

        public void ExitSetting()
        {
            Transform dropDown = settingDropdown.transform.Find("dropDown");
            GameObject popupBG = settingDropdown.transform.Find("bg").gameObject;
            Time.timeScale = 1;
            if (popupBG)
            {
                popupBG.GetComponent<Image>().DOFade(0, 0.3f);
                StartCoroutine(HideGameObject(popupBG, 0.3f));
            }

            if (dropDown)
            {
                dropDown.DOMoveY(5.4f, 0, true);
                dropDown.DOMoveY(12.2f, 0.3f, true).SetEase(Ease.InSine); ;
            }
        }

        public void ShowSettingLayout()
        {
            string _target = EventSystem.current.currentSelectedGameObject.name;
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

            childSetting.SetActive(false);
            childSettingService.SetActive(false);
            childSettingRule.SetActive(false);
            childSettingToturial.SetActive(false);

            switch (_target)
            {
                case "Btn_setting": //設定頁
                    childSetting.SetActive(true);

                    if (settingPanelNew)
                    {
                        settingPanelNew.transform.DOMoveX(-19.5f, 0, true);
                        settingPanelNew.transform.DOMoveX(0, 0.3f, true).SetEase(Ease.OutCubic);
                        ExitSetting();
                    }
                    break;
                case "Btn_service"://客服頁
                    Application.OpenURL("https://www.facebook.com/17%E7%8E%A9%E9%BA%BB%E5%B0%87-394416694307519#");

                    //if (childSettingService)
                    //{
                    //    InputField _text = childSettingService.transform.Find("Content/InputField").GetComponent<InputField>();
                    //    _text.text = "請詳述您的問題：\n\n方便聯絡的時間：\n\n聯絡電話：";
                    //    childSettingService.SetActive(true);
                    //    Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                    //}

                    if (settingPanelNew)
                    {
                        ExitSetting();
                    }
                    break;
                case "Btn_rules": //條款頁
                    childSettingRule.SetActive(true);

                    if (settingPanelNew)
                    {
                        settingPanelNew.transform.DOMoveX(-19.5f, 0, true);
                        settingPanelNew.transform.DOMoveX(0, 0.3f, true).SetEase(Ease.OutCubic);
                        ExitSetting();
                    }
                    break;
                case "Btn_toturial": //教學說明頁
                    childSettingToturial.SetActive(true);

                    if (settingPanelNew)
                    {
                        settingPanelNew.transform.DOMoveX(-19.5f, 0, true);
                        settingPanelNew.transform.DOMoveX(0, 0.3f, true).SetEase(Ease.OutCubic);
                        ExitSetting();
                    }
                    break;
            }
        }

        public void HideSettingLayout()
        {
            if (settingPanelNew)
            {
                settingPanelNew.transform.DOMoveX(0, 0, true);
                settingPanelNew.transform.DOMoveX(-19.5f, 0.3f, true).SetEase(Ease.OutCubic);
            }
        }

        public void SettingToggle(bool isOn)
        {
            string _target = EventSystem.current.currentSelectedGameObject.name;
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

            profilePanel.SetActive(false);
            gamePanel.SetActive(false);
            recordPanel.SetActive(false);

            if (isOn)
            {
                switch (_target)
                {
                    case "Btn_prof": //個人資訊頁
                        profilePanel.SetActive(true);
                        break;
                    case "Btn_game"://遊戲設定頁
                        gamePanel.SetActive(true);
                        break;
                    case "Btn_reco": //紀錄查詢頁
                        recordPanel.SetActive(true);
                        break;
                }
            }
        }

        public void RecordToggle()
        {
            string _target = EventSystem.current.currentSelectedGameObject.name;
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

            depositRecoPanel.SetActive(false);
            coinRecoPanel.SetActive(false);

            switch (_target)
            {
                case "Btn_coinReco": //儲值紀錄頁
                    coinRecoPanel.SetActive(true);
                    break;
                case "Btn_depositReco"://遊戲幣紀錄頁
                    depositRecoPanel.SetActive(true);
                    break;
            }
        }

        public void ClickSendService() {
            InputField _titleText = childSettingService.transform.Find("Title/InputField").GetComponent<InputField>();
            InputField _contentText = childSettingService.transform.Find("Content/InputField").GetComponent<InputField>();
            string msg = _titleText.text + ":" + _contentText.text;

            if (servicePopup) {
                GameObject popupBG = servicePopup.Find("bg").gameObject;
                popupBG.SetActive(true);
                popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);

                Transform Popup = servicePopup.Find("popup");
                Popup.transform.DOScale(Vector3.zero, 0);
                Popup.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                Popup.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InSine).SetDelay(1.5f);
                popupBG.GetComponent<Image>().DOFade(0, 0.3f).SetDelay(1.5f);
                StartCoroutine(HideGameObject(popupBG, 1.8f));

                _titleText.text = "";
                _contentText.text = "請詳述您的問題：\n\n方便聯絡的時間：\n\n聯絡電話：";

                settingPanelNew.transform.DOMoveX(0, 0, true);
                settingPanelNew.transform.DOMoveX(-19.5f, 0.3f, true).SetEase(Ease.OutCubic).SetDelay(1.8f);
            }
        }

        private void SubPageInit() {
            currentPage = SubPage.Main;
            if (roomlistPanel) {
                toggleAll = roomlistPanel.transform.Find("Panel_title/Title/Btn_all").GetComponent<Toggle>();
                roomlistEmpty = roomlistPanel.transform.Find("Empty").gameObject;
            }

            if (popupBuy)
            {
                _popupBuyItemName = popupBuy.Find("content/Item/name").GetComponent<Text>();
                _popupBuyItemPrice = popupBuy.Find("content/Price/price").GetComponent<Text>();
                _popupBuyItemNum = popupBuy.Find("content/Num/num").GetComponent<InputField>();
                _popupBuyItemTotal = popupBuy.Find("content/Total/total").GetComponent<Text>();
            }
            BirtnShopItem();

            childSetting = settingPanelNew.transform.Find("Setting").gameObject;
            if (childSetting) {
                settingProfileSR = childSetting.transform.Find("Profile/Mask/Main").GetComponent<ScrollRect>();
                settingProfileScr = childSetting.transform.Find("Profile/Mask/Main/Scrollbar").GetComponent<Scrollbar>();
                profilePanel = childSetting.transform.Find("Profile/Mask/Main/Panel").gameObject;
                gamePanel = childSetting.transform.Find("Game").gameObject;
                recordPanel = childSetting.transform.Find("Record").gameObject;

                if (profilePanel) {
                    settingNickname = profilePanel.transform.Find("IF_nick").GetComponent<InputField>();
                    replaceNickname = profilePanel.transform.Find("Btn_changeName").GetComponent<Button>();
					settingAccount = profilePanel.transform.Find("IF_acc").GetComponent<InputField>();
                    settingLoginTypeImg = profilePanel.transform.Find("LoginTypeImg").GetComponent<Image>();
                    settingLoginTypeTxt = profilePanel.transform.Find("Text_loignType").GetComponent<Text>();
                    setLoginHint = profilePanel.transform.Find("Text_bindHint").gameObject;
                    joinMenber = profilePanel.transform.Find("Btn_join").GetComponent<Button>();

                    settingPanelBind = profilePanel.transform.Find("PanelBind").gameObject;
                    settingPanelPass = profilePanel.transform.Find("PanelPass").gameObject;

                    if (settingPanelBind) {
                        bindAccount = settingPanelBind.transform.Find("Btn_bind").GetComponent<Button>();
                        settingBindMail = settingPanelBind.transform.Find("IF_mail").GetComponent<InputField>();
                        settingBindPass1 = settingPanelBind.transform.Find("IF_pass1").GetComponent<InputField>();
                        settingBindPass2 = settingPanelBind.transform.Find("IF_pass2").GetComponent<InputField>();
                        settingBindCbHint = settingPanelBind.transform.Find("Text_bindCbHint").GetComponent<Text>();
                    }

                    if (settingPanelPass)
                    {
                        replacePass = settingPanelPass.transform.Find("Btn_changePass").GetComponent<Button>();
                        settingReplacePassOld = settingPanelPass.transform.Find("IF_passOld").GetComponent<InputField>();
                        settingReplacePass1 = settingPanelPass.transform.Find("IF_pass1").GetComponent<InputField>();
                        settingReplacePass2 = settingPanelPass.transform.Find("IF_pass2").GetComponent<InputField>();
                        settingRePassCbHint = settingPanelPass.transform.Find("Text_rePassCbHint").GetComponent<Text>();
                    }
                }
                
                if (recordPanel)
                {
                    depositRecoPanel = recordPanel.transform.Find("Panel_depositReco").gameObject;
                    coinRecoPanel = recordPanel.transform.Find("Panel_coinReco").gameObject;
                }
            }

            childSettingService = settingPanelNew.transform.Find("Service").gameObject;
            if (childSettingService) {
                InputField _text = childSettingService.transform.Find("Content/InputField").GetComponent<InputField>();
                _text.text = "請詳述您的問題：\n\n方便聯絡的時間：\n\n聯絡電話：";
                servicePopup = childSettingService.transform.Find("Popup");
            }

            childSettingRule = settingPanelNew.transform.Find("Rule").gameObject;
            childSettingToturial = settingPanelNew.transform.Find("Toturial").gameObject;

            actDailyPanel = activityPanel.transform.Find("Daily").gameObject;
            actMissionPanel= activityPanel.transform.Find("Mission").gameObject;

            if (rankPanel) {
                playerRankPanel = rankPanel.transform.Find("playerRank");
                if (playerRankPanel) {
                    playerRankText = playerRankPanel.transform.Find("Rank/Text").GetComponent<Text>();
                    playerRankLv = playerRankPanel.transform.Find("NameLv/Lv").GetComponent<Text>();
                    playerRankWin = playerRankPanel.transform.Find("WinLose/Win").GetComponent<Text>();
                    playerRankLose = playerRankPanel.transform.Find("WinLose/Lose").GetComponent<Text>();
                    playerRankProb = playerRankPanel.transform.Find("Probability/Prob").GetComponent<Text>();
                }
            }
                
            if (dailyBonusPanel) {
                popupDailyBonus = dailyBonusPanel.transform.Find("main");

                if (popupDailyBonus)
                {
                    popupDailyBonusText = popupDailyBonus.Find("month").GetComponent<Text>();
                    dailyBonuGirlEye = popupDailyBonus.Find("Girl/eye").GetComponent<Image>();
                    dailyBonuSparkle = popupDailyBonus.Find("spakle").GetComponent<Image>();
                }
            }
            BirthDailyBonusItem();

            if (connectingPanel) {
                _connectingSign = connectingPanel.transform.Find("Image/Img");
                _connectingText = connectingPanel.transform.Find("Image/Text").GetComponent<Text>();
            }

            if (actionDonePanel)
                _actionText = actionDonePanel.transform.Find("main/Txt").GetComponent<Text>();


            if (btmMenuBtns[2])
                btmDepositLight = btmMenuBtns[2].Find("light").GetComponent<Image>();


        }

        public void ClickBalloonBtn() {
            AudioManager.Instance.PlaySE("gp" + UnityEngine.Random.Range(0, 9));
            BirthBalloon();
        }

        private void AutoBirthBalloon() {
            StartCoroutine("RandomBalloon");
        }

        IEnumerator RandomBalloon() {
            int _time = UnityEngine.Random.Range(0, 2); //一次最多1顆
            for (int i = 0; i < _time; i++)
            {
                yield return new WaitForSeconds(1f);
                BirthBalloon();
            }
        }

        private void BirthBalloon()
        {
            Vector3 birthPos = new Vector3(UnityEngine.Random.Range(-900f, 900f), -650f, 0);
            GameObject go = GameObject.Instantiate(Resources.Load("Prefab/balloon") as GameObject);
            go.transform.SetParent(balloonPanel.transform);
            RectTransform rectT = go.GetComponent<RectTransform>();
            rectT.localPosition = birthPos;
            rectT.localScale = Vector3.one;
            rectT.DOMoveX(UnityEngine.Random.Range(-9f, 9f), 10f, false).SetEase(Ease.InOutFlash);
            rectT.DOMoveY(7f, 10f, false).SetEase(Ease.InOutFlash);
            rectT.DORotate(new Vector3(0, 0, 15f), 1f).SetLoops(-1, LoopType.Yoyo);
        }

        public void ClickActivity()
        {
            if (btmMenuBtns[4])
                btmMenuBtns[4].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            if (activityPanel)
            {
                coinAdSign.DOPlay();
                activityPanel.transform.DOMoveX(-19.5f, 0, true);
                activityPanel.transform.DOMoveX(0, 0.5f, true).SetEase(Ease.OutCubic);
            }
        }

        public void ExitActivity()
        {
            if (activityPanel)
            {
                activityPanel.transform.DOMoveX(0, 0, true);
                activityPanel.transform.DOMoveX(-19.5f, 0.5f, true).SetEase(Ease.OutCubic);
                coinAdSign.DOPause();
            }
        }

        public void ActivityToggle(bool isOn)
        {
            string _target = EventSystem.current.currentSelectedGameObject.name;
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

            actDailyPanel.SetActive(false);
            actMissionPanel.SetActive(false);

            if (isOn) {
                switch (_target)
                {
                    case "Btn_daily": //每日禮物頁
                        actDailyPanel.SetActive(true);
                        break;
                    case "Btn_mission"://賞金任務頁
                        actMissionPanel.SetActive(true);
                        break;
                }
            }

        }

        public void setMailCallback(WebExceptionStatus status, string result)
		{
            _needHideConnect = true; //需要關閉連線視窗
            resetUserTypeResult = result;
            _resetUserType = true;
			if (status != WebExceptionStatus.Success) {
				Debug.Log ("setMailCallback Failed! " + result);
			}
			else if (!string.IsNullOrEmpty (result))
			{
                //Debug.Log("setMailCallback =  " + result);
            }
		}

		public void setPwdCallback(WebExceptionStatus status, string result)
		{
            _needHideConnect = true; //需要關閉連線視窗
            resetPassResult = result;
            _resetPass = true;
            if (status != WebExceptionStatus.Success)
            {
                Debug.Log("setPwdCallback Failed! " + result);
			}
            else if (!string.IsNullOrEmpty(result))
            {
                Debug.Log("setPwdCallback =  " + result);
            }
 		}

        public void getRankDataCallback(WebExceptionStatus status, string result)
        {
            _needHideConnect = true; //需要關閉連線視窗
            getRankResult = result;
            _getRank = true;
            if (status != WebExceptionStatus.Success)
            {
                Debug.Log("getRankDataCallback Failed! " + result);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                //Debug.Log("getRankDataCallback =  " + result);
            }
        }

        public void ShowLogoutPopup()
        {
            GameObject popupBG = childSetting.transform.Find("Profile/popupBG").gameObject;

            if (popupLogout)
            {
               if (popupBG)
                {
                    popupBG.SetActive(true);
                    popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
                }
                popupLogout.transform.DOScale(Vector3.zero, 0);
                popupLogout.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
        }

        public void ExitLogoutPopup()
        {
            GameObject popupBG = childSetting.transform.Find("Profile/popupBG").gameObject;

            if (popupLogout)
            {
                popupLogout.transform.DOScale(Vector3.one, 0);
                popupLogout.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine);
                if (popupBG)
                {
                    popupBG.GetComponent<Image>().DOFade(0, 0.3f);
                    StartCoroutine(HideGameObject(popupBG, 0.3f));
                }
            }
        }

        public void ClickGames(int _miniGame) {
            if (loadingPanel)
            {
                loadingPanel.SetActive(true);
                AccountManager.Instance.GameType = _miniGame;
                EnterLoading.instance._sceneName = "05.Games";
                EnterLoading.instance.StartLoading();
            }
        }

        private void IniPlayerInfo() {
            SetPlayerPhotos();
            SetPlayerNames();
			SetPlayerAccount ();
            SetPlayerCoins();
            SetPlayerLevels();
            SetUserOnline();
        }

        public void SetPlayerPhotos()
        {
            string sPhoto = CryptoPrefs.GetString("USERPHOTO");
            if (!string.IsNullOrEmpty(sPhoto))
            {
                Texture2D newPhoto = new Texture2D(1, 1);
                newPhoto.LoadImage(Convert.FromBase64String(sPhoto));
                newPhoto.Apply();

                for (int i = 0; i < playerPhotos.Length; i++)
                {
                    playerPhotos[i].sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
                }

                PhotonNetwork.player.Photo = sPhoto;
            }
            
        }

		public void SetPlayerAccount()
        {
			string sType = CryptoPrefs.GetString("USERTYPE");
			//Debug.Log("sType=" + sType);
            string sMail = CryptoPrefs.GetString("USERMAIL");
			//Debug.Log("sMail=" + sMail);

            if (!string.IsNullOrEmpty(sMail))
				settingAccount.text = sMail;

            ShowUserLoginType(sType);
        }

		public void SetPlayerNames()
		{
			//Debug.Log ("SetPlayerNames()");
			string sName = CryptoPrefs.GetString("USERNAME");
			//Debug.Log ("sName="+sName);
			if (!string.IsNullOrEmpty(sName))
			{
				for (int i = 0; i < playerNames.Length; i++)
				{
					playerNames[i].text = sName;
				}

				if(settingNickname) //設定頁暱稱欄位
					settingNickname.text = sName;
				PhotonNetwork.player.NickName = sName;
			}
		}

        public void SetPlayerLevels()
        {
            string sLevel = CryptoPrefs.GetString("USERLEVEL");
            if (!string.IsNullOrEmpty(sLevel))
            {
                for (int i = 0; i < playerLvs.Length; i++)
                {
                    playerLvs[i].text = "Lv " + sLevel;
                }

                PhotonNetwork.player.Level = sLevel;
            }
        }

        public void SetPlayerCoins()
        {
            SubPage _subpage = currentPage;
            int arrayIndex = 0;
            string sCoin = CryptoPrefs.GetString("USERCOIN");
            //Debug.Log("Get Local Coin = " + sCoin);
            if (!string.IsNullOrEmpty(sCoin) && Int32.TryParse(sCoin, out localCoin))
            {
                //localCoin = int.Parse(sCoin);

                switch (_subpage)
                {
                    case SubPage.Main:
                        arrayIndex = 0;
                        break;
                    case SubPage.Rank:
                        arrayIndex = 1;
                        break;
                    case SubPage.Shop:
                        arrayIndex = 2;
                        break;
                    case SubPage.Bag:
                        arrayIndex = 3;
                        break;
                    default:
                        break;
                }
                playerCoins[arrayIndex].text = string.Format("{0:0,0}", localCoin);
                PhotonNetwork.player.Coin = sCoin;
            }
            else
            {
                Debug.Log("localCoin 金幣數量異常 " + sCoin);
            }
            //coinAPIcallback = true;
        }

        public void SetUserOnline()
        {
            _randomOnlineUsers = 123000; // UnityEngine.Random.Range(1, 100) * 1000;
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
			//Debug.Log ("SetUserOnline("+sName+" "+sToken+")");
            MJApi.getUserNum(sToken, sName, getUserNumCallback);
        }

        public void getUserNumCallback(WebExceptionStatus status, string result)
        {
            if (status != WebExceptionStatus.Success)
            {
				Debug.LogError("getUserNumCallback Failed! " + result);
            }
            else {
                getOnlineResult = result;
            }
            _getOnline = true;
        }

        public void setNameCallback(WebExceptionStatus status, string result)
        {
            _needHideConnect = true; //需要關閉連線視窗
            resetNameResult = result;
            _resetName = true;
            if (status != WebExceptionStatus.Success)
            {
                Debug.Log("setNameCallback Failed! " + result);
            }
            else
            {
                //Debug.Log("setNameCallback =  " + result);
            }
        }

        public void SaveNickname()
        {
            if (settingNickname) {
                string oName = CryptoPrefs.GetString("USERNAME");
                string sToken = CryptoPrefs.GetString("USERTOKEN");

	            string checkedName = settingNickname.text;
		        bool isContComma = checkedName.Contains(",");
		        if (isContComma) {
		            checkedName = checkedName.Replace(",", "，");
				    settingNickname.text = checkedName;
	            }
		        MJApi.setPlayerName(sToken, oName, checkedName, setNameCallback);
		        AccountManager.Instance.ShowConnecting (); //開啟連線視窗
            }
        }
		
        public void ClickReplacePhoto() {
            imagePicker.Show("Select Image", "unimgpicker", "ImagePicker", 256);
        }

        private void DailyFirstLogin() {
            ShowDailyBonus();
            dailyBonusToday.ReadyTake();  //顯示領取動畫
        }

		public void setCoinCallback(WebExceptionStatus status, string result)
		{
			if (status != WebExceptionStatus.Success)
			{
				Debug.Log("setCoinCallback Failed! " + result);
			}
			else if (!string.IsNullOrEmpty (result))
			{
                setCoinResult = result;
                //Debug.Log("CB: setCoinResult =  " + result);
                _setCoin = true;
			}
            //coinAPIcallback = true;
        }

		void Update() {

			if (Input.GetKeyUp(KeyCode.Escape))
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
					activity.Call<bool>("moveTaskToBack", true);
				}
				else
				{
					Application.Quit();
				}
			}

            if (_needHideConnect) {
				AccountManager.Instance.HideConnecting (); //關閉連線視窗
                _needHideConnect = false;
            }

			if (_setCoin) {
                _setCoin = false;
                if (setCoinResult == "The remote server returned an error: (410) Gone.") {
                    _ERRLogout = true;
                    actionDonePanel.SetActive(true);
                    _actionText.text = "帳號異常，請重新登入";
                }
                else if (Int32.TryParse(setCoinResult, out localCoin)) {
                    CryptoPrefs.SetString("USERCOIN", setCoinResult);
                    SetPlayerCoins();
                }
            }
            if (_getOnline) {
                _getOnline = false;
                getOnlineResult = (string.IsNullOrEmpty(getOnlineResult)) ? "0": getOnlineResult;
                userOnline.text = "在線人數 " + string.Format("{0:0,0}", _randomOnlineUsers + int.Parse(getOnlineResult));
            }

            if (_resetUserType)
            {
                actionDonePanel.SetActive(true);
                _resetUserType = false;
                Debug.Log("綁定帳號 " + resetUserTypeResult);

                if (resetUserTypeResult == "The remote server returned an error: (401) Unauthorized.")
                {
                    _actionText.text = "此信箱已被註冊";
				} 
				else if (resetUserTypeResult == "The remote server returned an error: (410) Gone.") {
                    _ERRLogout = true;
                    _actionText.text = "帳號異常，請重新登入";
				}
                else if (resetUserTypeResult != "OK")
                {
                    _actionText.text = "綁定失敗";
                }
                else {
                    _actionText.text = "綁定成功";
                    CryptoPrefs.SetString("USERTYPE", "C");
                    CryptoPrefs.SetString("USERMAIL", settingBindMail.text);
                    SetPlayerAccount();

                    settingBindMail.text = string.Empty;
                    settingBindPass1.text = string.Empty;
                    settingBindPass2.text = string.Empty;
                    settingBindCbHint.gameObject.SetActive(false);
                }
            }

            if (_resetPass)
            {
                actionDonePanel.SetActive(true);
                _resetPass = false;
                Debug.Log("更新密碼 " + resetPassResult);

                if (resetPassResult == "The remote server returned an error: (401) Unauthorized.")
                {
                    _actionText.text = "原始密碼輸入錯誤";
				} 
				else if (resetPassResult == "The remote server returned an error: (410) Gone.") {
                    _ERRLogout = true;
                    _actionText.text = "帳號異常，請重新登入";
				}
                else if (resetPassResult != "OK")
                {
                    _actionText.text = "密碼更改失敗";
                }
                else {
                    _actionText.text = "密碼更改成功";
                    settingReplacePassOld.text = string.Empty;
                    settingReplacePass1.text = string.Empty;
                    settingReplacePass2.text = string.Empty;
                    settingRePassCbHint.gameObject.SetActive(false);
                }
            }

            if (_resetName)
            {
                actionDonePanel.SetActive(true);
                _resetName = false;
                Debug.Log("更改暱稱 " + resetNameResult);

                if (resetNameResult == "The remote server returned an error: (401) Unauthorized.")
                {
                    _actionText.text = "參數錯誤";
				} 
				else if (resetNameResult == "The remote server returned an error: (410) Gone.") {
                    _ERRLogout = true;
                    _actionText.text = "帳號異常，請重新登入";
				}
                else if (resetNameResult != "OK")
                {
                    _actionText.text = "暱稱更改失敗";
                }
                else
                {
                    _actionText.text = "暱稱更改成功";
                    CryptoPrefs.SetString("USERNAME", settingNickname.text);
                    SetPlayerNames();
                }
            }

            if (_setItem)
            {
                actionDonePanel.SetActive(true);
                _setItem = false;
                Debug.Log("購買商品 " + setItemResult);

                if (setItemResult == "The remote server returned an error: (401) Unauthorized.")
                {
                    _actionText.text = "購買失敗";
				} else if (setItemResult == "The remote server returned an error: (410) Gone.") {
                    _ERRLogout = true;
                    _actionText.text = "帳號異常，請重新登入"; 
                }
                else if (setItemResult == "something wrong!")
                {
                    _actionText.text = "發生不明錯誤";
                }
                else
                {
                    if (Int32.TryParse(setItemResult, out localCoin))
                    {
                        CryptoPrefs.SetString("USERCOIN", setItemResult);
                        SetPlayerCoins();
                        _actionText.text = "購買成功";
                    }
                    else {
                        _actionText.text = "發生異常";
                    }
                    ExitShopBuy();     //關閉購買視窗
                }
            }

            if (_getBag) {
                _getBag = false;
                Debug.Log("背包內容物 " + getItemNumResult);

                if (getItemNumResult == "getItemCallback Failed!")
                {
                    actionDonePanel.SetActive(true);
                    _actionText.text = "發生錯誤";
                }
				else if(getItemNumResult == "The remote server returned an error: (410) Gone.")
				{
                    _ERRLogout = true;
                    actionDonePanel.SetActive(true);
					_actionText.text = "帳號異常，請重新登入";
				}
                else
                {
                    if (currentPage != SubPage.Shop)
                        paintBagUI();
                    else {
                        GameObject popupBG = shopPanel.transform.Find("popupBG").gameObject;
                        if (popupBuy)
                        {
                            if (popupBG)
                            {
                                popupBG.SetActive(true);
                                popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
                            }
                            popupBuy.transform.DOScale(Vector3.zero, 0);
                            popupBuy.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                        }
                    }
                }
            }

            if (_getRank)
            {
                _getRank = false;
                Debug.Log("排行榜 " + getRankResult);

                if (getRankResult == "getItemCallback Failed!")
                {
                    actionDonePanel.SetActive(true);
                    _actionText.text = "發生錯誤";
                }
				else if(getRankResult == "The remote server returned an error: (410) Gone.")
				{
                    _ERRLogout = true;
                    actionDonePanel.SetActive(true);
					_actionText.text = "帳號異常，請重新登入";
				}
                else
                {
                    paintRankUI();
                }
            }

            if (_setUserPhoto)
            {
                _setUserPhoto = false;

                if (setUserPhotoResult == "setPhotoCallback Failed!") {
                    actionDonePanel.SetActive(true);
                    _actionText.text = "發生錯誤";
                }
                else if(setUserPhotoResult == "The remote server returned an error: (410) Gone.")
                {
                    _ERRLogout = true;
                    actionDonePanel.SetActive(true);
                    _actionText.text = "帳號異常，請重新登入";
                }
                else
                {
                    SetPlayerPhotos();
                }
            }

            if (_getRoomPhoto)
            {
                _needHideConnect = true; //需要關閉連線視窗
                _getRoomPhoto = false;

                if (getRoomPhotoResult == "getRoomPhotoCallback Failed!")
                {
                    actionDonePanel.SetActive(true);
                    _actionText.text = "發生錯誤";
				} else if (getRoomPhotoResult == "The remote server returned an error: (410) Gone.") {
                    _ERRLogout = true;
                    actionDonePanel.SetActive(true);
					_actionText.text = "帳號異常，請重新登入";
				}
                else
                {
                    paintRoomUI(getRoomPhotoResult);
                }
                //_needHideConnect = true; //需要關閉連線視窗
            }

        }


        private void paintRoomUI(string _cb) {
            string[] tokens = _cb.Split(new string[] { "," }, StringSplitOptions.None);

            int _totalRooms = tokens.Length / 4;
            int _count = 0;
            roomTotalDatas.Clear();

            //Debug.Log("共 " + _totalRooms + " 間房; 資料筆數" + tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
                roomTotalDatas.Add(tokens[i]);

			foreach (RoomInfo ri in _roomLists)
            {
                GameObject g = GameObject.Instantiate (Resources.Load ("Lobby/RoomItem") as GameObject);
				g.transform.SetParent(rListPanel,false);
				g.transform.localScale = Vector3.one;
				g.transform.localPosition = Vector3.zero;
                Text txtName = g.transform.Find("Name").GetComponent<Text>();
                Text txtNum = g.transform.Find("People/Num").GetComponent<Text>();
                Image _img = g.transform.Find("PhotoMask/Photo").GetComponent<Image>();
                int player_count = ri.PlayerCount;
                Hashtable cp = ri.CustomProperties;

                if (cp ["CRoomName"] != null) {
					string name = cp ["CRoomName"].ToString ();
                    txtName.text = name;
                    txtNum.text = String.Format("{0:#,0}", player_count);
                }

                //ri.Photo = roomTotalDatas[_count * 4 + 1];
                //ri.Level = roomTotalDatas[_count * 4 + 2];
                //ri.Coin = roomTotalDatas[_count * 4 + 3];

                SetRoomListPhoto(roomTotalDatas[_count * 4 + 1], _img);

                Button bt = g.GetComponent<Button> ();
				bt.onClick.AddListener (delegate {
                    joinRoom (ri.Name);
                });

                _count += 1;
            }

        }

        private void SetRoomListPhoto(string _photoType, Image _photoImg) {

            if (_photoType == "0") //圖片隨機
            {
                //_photoImg.sprite = Resources.Load<Sprite>("Image/Rank/temp/" + UnityEngine.Random.Range(1, 8));
                _photoImg.sprite = Resources.Load<Sprite>("Image/defaultUserP");
            }
            else
            {
                string sPhoto;
                if (_photoType == "1") // 玩家自身
                    sPhoto = CryptoPrefs.GetString("USERPHOTO");
                else
                    sPhoto = _photoType;  // 其他玩家

                if (!string.IsNullOrEmpty(sPhoto))
                {
                    Texture2D newPhoto = new Texture2D(1, 1);
                    newPhoto.LoadImage(System.Convert.FromBase64String(sPhoto));
                    newPhoto.Apply();

                    _photoImg.sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
                }
            }
        }

        public void ChangeCoin(int _earnCoin) {
            //coinAPIcallback = false;
            //Change Coin
            string sName =  CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            int oldCoin = localCoin;
            int newCoin = localCoin + _earnCoin;

            MJApi.setUserCoin(sToken, sName, oldCoin, newCoin, setCoinCallback);
        }

        private void ShowUserLoginType(string _type) {
            bool isScrollable = false;
            bool isShowBindHint = false;
            bool isShowJoinBtn = false;
            bool isShowPanelBind = false;
            bool isShowPanelPass = false;
            
            switch (_type) {
                case "C":
                    isScrollable = true;
                    settingLoginTypeImg.sprite = Resources.Load<Sprite>("Image/Icon_17");
                    settingLoginTypeTxt.text = "17玩麻將會員";
                    isShowPanelPass = true;
                    break;
                case "P":
                    settingLoginTypeImg.sprite = Resources.Load<Sprite>("Image/Icon_guest");
                    settingLoginTypeTxt.text = "訪客";
                    isShowBindHint = true;
                    isShowJoinBtn = true;
                    break;
                case "G":
                    settingLoginTypeImg.sprite = Resources.Load<Sprite>("Image/Icon_goo");
                    settingLoginTypeTxt.text = "Google+ 會員";
                    break;
                case "F":
                    settingLoginTypeImg.sprite = Resources.Load<Sprite>("Image/Icon_fb");
                    settingLoginTypeTxt.text = "facebook 會員";
                    break;
            }

            settingProfileSR.enabled = isScrollable;
            settingProfileScr.value = isScrollable ? 1 : 0;
            setLoginHint.SetActive(isShowBindHint);
            joinMenber.gameObject.SetActive(isShowJoinBtn);
            settingPanelBind.SetActive(isShowPanelBind);
            settingPanelPass.SetActive(isShowPanelPass);
        }

        public void ClickJoinMenber()
        {
            joinMenber.gameObject.SetActive(false);
            setLoginHint.SetActive(false);
            settingPanelBind.SetActive(true);
            settingProfileSR.enabled = true;
            settingProfileScr.value = 0;
        }

        public void ClickReplacePass() {
            // change Passwd
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            string mail = CryptoPrefs.GetString("USERMAIL");
            string replacePassOld = settingReplacePassOld.text;
            string replacePass1 = settingReplacePass1.text;
            string replacePass2 = settingReplacePass2.text;
            GameObject replacePassHint = settingRePassCbHint.gameObject;

            //檢查密碼是否合法
            if (!CheckInput.instance.CheckPass(replacePassOld))
            {
                settingRePassCbHint.text = "原始密碼輸入錯誤";
                replacePassHint.SetActive(true);
            }
            if (!CheckInput.instance.CheckPass(replacePass1) || !CheckInput.instance.CheckPass(replacePass2))
            {
                settingRePassCbHint.text = "新密碼格式錯誤";
                replacePassHint.SetActive(true);
            }
            else if (replacePass1 != replacePass2)
            {
                settingRePassCbHint.text = "兩次密碼不符";
                replacePassHint.SetActive(true);
            }
            else
            {
                MJApi.setUserPwd(sToken, sName, mail, replacePassOld, replacePass1, setPwdCallback);
				AccountManager.Instance.ShowConnecting (); //開啟連線視窗
            }
        }

        public void ClickBindMail()
        {
            //Change Mail
            string sName = CryptoPrefs.GetString("USERNAME");
            string sToken = CryptoPrefs.GetString("USERTOKEN");
            string bindMail = settingBindMail.text;
            string bindPass1 = settingBindPass1.text;
            string bindPass2 = settingBindPass2.text;
            GameObject bindPassHint = settingBindCbHint.gameObject;

            //檢查欄位是否合法
            if (!CheckInput.instance.CheckEmail(bindMail))
            {
                settingBindCbHint.text = "電子信箱格式錯誤";
                bindPassHint.SetActive(true);
            }

            //檢查密碼是否合法
            else if (!CheckInput.instance.CheckPass(bindPass1) || !CheckInput.instance.CheckPass(bindPass2))
            {
                settingBindCbHint.text = "新密碼格式錯誤";
                bindPassHint.SetActive(true);
            }
            else if (bindPass1 != bindPass2)
            {
                settingBindCbHint.text = "兩次密碼不符";
                bindPassHint.SetActive(true);
            }
            else
            {
                MJApi.setUserMail(sToken, sName, bindMail, bindPass1, setMailCallback);
				AccountManager.Instance.ShowConnecting (); //開啟連線視窗
            }
        }

        public void ClickActionDoneBtn() {
            actionDonePanel.SetActive(false);
            _actionText.text = string.Empty;
            if (_ERRLogout)
            {
                _logoutScript.ClickLogout();
            }
        }


        public void ShowDailyBonus()
        {
            GameObject popupBG = dailyBonusPanel.transform.Find("popupBG").gameObject;

            if (popupDailyBonus)
            {
                popupDailyBonus.gameObject.SetActive(true);
                Time.timeScale = 0;
                if (popupBG)
                {
                    popupBG.SetActive(true);
                    popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f).SetUpdate(true);
                }
                popupDailyBonus.DOScale(Vector3.zero, 0);
                popupDailyBonus.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }

        public void ExitDailyBonus()
        {
            GameObject popupBG = dailyBonusPanel.transform.Find("popupBG").gameObject;

            if (popupDailyBonus)
            {
                Time.timeScale = 1;
                popupDailyBonus.DOScale(Vector3.one, 0);
                popupDailyBonus.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine);
                if (popupBG)
                {
                    popupBG.GetComponent<Image>().DOFade(0, 0.3f);
                    StartCoroutine(HideGameObject(popupBG, 0.3f));
                }
            }
        }

        public void BirthDailyBonusItem()
        {
            string uFirstLogin = CryptoPrefs.GetString("USERFLOGIN");
            string uTotalLogin = CryptoPrefs.GetString("USERLOGINTOTAL");
            
            int _tLoginTime = (string.IsNullOrEmpty(uTotalLogin)) ? 1 : int.Parse(uTotalLogin);
            Debug.Log("登入總次數 = " + _tLoginTime);

            int _takedDay = (uFirstLogin == "T") ? _tLoginTime - 1 : _tLoginTime;
            DateTime myDate = DateTime.Now;
            int maxDay = 30;

            if (popupDailyBonusText)
                popupDailyBonusText.text = myDate.Month + "月份";

            switch (myDate.Month) {
                case 1:case 3:case 5:case 7:case 8:case 10:case 12:
                    maxDay = 31;
                    break;
                case 4:case 6:case 9:case 11:
                    maxDay = 30;
                    break;
                case 2:
                    maxDay = (myDate.Year % 4 == 0) ? 29 : 28;
                    break;
            }

            foreach (Transform child in dailyBonusTarget)
                Destroy(child.gameObject);

            for (int i = 0 ;i < maxDay; i++)
            {
                GameObject go = GameObject.Instantiate(Resources.Load("Prefab/dailyBonusItem") as GameObject);
                DailyBonus db = go.GetComponent<DailyBonus>();
                db.ShowDate(i+1);

                if (i >= _takedDay)
                    db.NotTake(i + 1);
                else
                    db.AlreadyTake();
 
                go.transform.SetParent(dailyBonusTarget);
                go.name = (i + 1).ToString();
                RectTransform rectT = go.GetComponent<RectTransform>();
                rectT.localPosition = Vector3.zero;
                rectT.localScale = Vector3.one;

                if (i + 1 == _tLoginTime) //改 myDate.Day 就變日期
                    dailyBonusToday = db; //當天
            }
        }

		public void ShowAlert(string msg, float t = 0)
		{
			if (alertPanel != null) {
				Text txt = alertPanel.GetComponentInChildren<Text> ();
				txt.text = msg;
				alertPanel.SetActive (true);
				if (t > 0) {
					Invoke ("HideAlert", t);
				}
			}
		}

		public void HideAlert()
		{
			if (alertPanel != null) {
				alertPanel.SetActive (false);
			}
		}

    }
}
