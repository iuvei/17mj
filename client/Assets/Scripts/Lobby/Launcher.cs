using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using ExitGames.Client.Photon.Encry;

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

        public Transform[] playRoomBtns;
        public Transform[] btmMenuBtns;

        public Image[] playerPhotos;
        public Text[] playerNames;
        public Text[] playerCoins;
        public Text[] playerLvs;
        public Text userOnline;


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
        private Button replaceNickname;
        private InputField settingNickname;
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
                Text xx2 = loadingPanel.transform.Find("processbar/Text").GetComponent<Text>();
                mySequence = DOTween.Sequence();
                mySequence.Append(xx2.DOText("載入中...", 3)).SetLoops(-1, LoopType.Restart);
            }
			//DontDestroyOnLoad (this.gameObject);
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

            hint = rListPanel.parent.GetComponentInChildren<Text>();

            
            SubPageInit();
            IniPlayerInfo();
            _createRoomType = new string[][] { _createRoomChipType, _createRoomCircleType, _createRoomTimeType };
            SettingInitAnim();
            InvokeRepeating("AutoBirthBalloon", 3f, 8f);
        }

		public override void OnJoinedLobby()
		{
			//PhotonNetwork.JoinRandomRoom();
			Debug.Log("OnJoinedLobby()");
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
                    //mySequence.Kill();
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
			Debug.Log ("OnConnectedToPhoton()");
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
			Connect ();
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
			Debug.Log ("OnJoinedRoom()");
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
			//Debug.Log (this.name+".CreateRoom()");
            if (PhotonNetwork.connected)
            {
				Hashtable customp = new Hashtable ();
				string cname = PhotonNetwork.player.NickName;
				customp.Add("CRoomName", cname);
				_roomname = "MJ"+UnityEngine.Random.Range(0, 1000000000).ToString();
				// 房間選項
				RoomOptions roomOptions = new RoomOptions ();
				roomOptions.isVisible = true;
				roomOptions.isOpen = true;
				roomOptions.maxPlayers = _roommax;
				roomOptions.customRoomProperties = customp;
				roomOptions.customRoomPropertiesForLobby = new string[] {"CRoomName"};
				//Debug.Log ();
                //创建房间成功
				if (PhotonNetwork.CreateRoom(_roomname, roomOptions, null))
                {
					//Debug.Log (PhotonNetwork.room);
					Debug.Log("Launcher.CreateRoom(#"+_roomname+ " success!!)");
                    //StartCoroutine(ChangeRoom());
					//ChangeRoom();
                }
            }
        }

		/// <summary>
		/// 顯示房間列表
		/// </summary>
		public void ShowRoomList()
		{
            if(playRoomBtns[2])
                playRoomBtns[2].DOScale(0.95f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

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

		/// <summary>
		/// Hides the room list.
		/// </summary>
		public void HideRoomList()
		{
            foreach (Transform child in rListPanel)
                Destroy(child.gameObject);

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
			//PhotonNetwork.LoadLevel("Stage01");
			PhotonNetwork.LoadLevel ("03.Room");
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

			if (PhotonNetwork.connected) {
				RoomInfo[] lists = PhotonNetwork.GetRoomList ();
				if (lists.Length > 0) {
					foreach (RoomInfo ri in lists) {
						GameObject g = GameObject.Instantiate (Resources.Load ("Lobby/RoomItem") as GameObject);
						g.transform.parent = rListPanel;
						g.transform.localScale = Vector3.one;
						g.transform.localPosition = Vector3.zero;
						Text txt = g.GetComponentInChildren<Text> ();
						Hashtable cp = ri.customProperties;
						if (cp ["CRoomName"] != null) {
							string name = cp ["CRoomName"].ToString ();
							txt.text = name;
						}
						//txt.text = ri.Name;
						Button bt = g.GetComponent<Button> ();
						bt.onClick.AddListener(delegate{ joinRoom(ri.Name);});
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

		private void joinRoom(string name) {
			Debug.Log ("joinRoom("+name+")");
			PhotonNetwork.JoinRoom (name);
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
            }
        }

        public void ClickBag()
        {
            if (btmMenuBtns[3])
                btmMenuBtns[3].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            GameObject bagEmpty = bagPanel.transform.Find("empty").gameObject;

            foreach (Transform child in bagItemTarget)
                Destroy(child.gameObject);

            //玩家的背包資料
            List<ItemInfo> itemInfos = loadItemData();
            //itemInfos.Clear(); //清空

            if (itemInfos.Count == 0)
            {
                if (bagEmpty)
                    bagEmpty.SetActive(true);
            }
            else {
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
            }
        }

        //讀取背包資料
        private List<ItemInfo> loadItemData() {
            ItemInfos itemInfos = new ItemInfos();
            ItemInfo data1 = new ItemInfo();
            data1.Id = 1;
            data1.Name = "渡假別墅";
            data1.Num = 1;
            data1.Path2D = 1;
            itemInfos.dataList.Add(data1);

            ItemInfo data2 = new ItemInfo();
            data2.Id = 2;
            data2.Name = "金元寶";
            data2.Num = 3;
            data2.Path2D = 2;
            itemInfos.dataList.Add(data2);

            ItemInfo data3 = new ItemInfo();
            data3.Id = 3;
            data3.Name = "I-RIMO鑽戒";
            data3.Num = 4;
            data3.Path2D = 3;
            itemInfos.dataList.Add(data3);
            
            return itemInfos.dataList;
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
            for (int i = 0; i < _createRoomType.Length ; i++)
            {
                //Debug.Log("開房設定: " + _createRoomType[i][_createRoomTypeIndex[i]] + " ; ");
            }

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

        public void ClickShopBuy()
        {
            GameObject popupBG = shopPanel.transform.Find("popupBG").gameObject;
            ShopItem _item = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<ShopItem>();
            //Debug.Log("name = " + _info.ItemName + "  price = " + _info.ItemPrice);
            currentNum = 1;
            currentPrice = _item.Price;
            _popupBuyItemName.text = _item.Name;
            _popupBuyItemPrice.text = String.Format("{0:0,0}", currentPrice);
            _popupBuyItemNum.text = currentNum.ToString();
            _popupBuyItemTotal.text = String.Format("{0:0,0}", currentPrice);

            if (popupBuy)
            {
                if (popupBG) {
                    popupBG.SetActive(true);
                    popupBG.GetComponent<Image>().DOFade(0.6f, 0.3f);
                }
                popupBuy.transform.DOScale(Vector3.zero, 0);
                popupBuy.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
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

        public void GoShopSaid()
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
            ShopItemInfos itemInfos = new ShopItemInfos();
            ShopItemInfo data1 = new ShopItemInfo();
            data1.Name = "渡假別墅";
            data1.Path2D = 1;
            data1.Price = 20000;
            itemInfos.dataList.Add(data1);

            ShopItemInfo data2 = new ShopItemInfo();
            data2.Name = "高級腕錶";
            data2.Path2D = 2;
            data2.Price = 15000;
            itemInfos.dataList.Add(data2);

            ShopItemInfo data3 = new ShopItemInfo();
            data3.Name = "金元寶";
            data3.Path2D = 3;
            data3.Price = 5000;
            itemInfos.dataList.Add(data3);

            ShopItemInfo data4 = new ShopItemInfo();
            data4.Name = "I-RIMO鑽戒";
            data4.Path2D = 4;
            data4.Price = 65000;
            itemInfos.dataList.Add(data4);

            ShopItemInfo data5 = new ShopItemInfo();
            data5.Name = "高級房車";
            data5.Path2D = 5;
            data5.Price = 25000;
            itemInfos.dataList.Add(data5);

            ShopItemInfo data6 = new ShopItemInfo();
            data6.Name = "香奈兒包";
            data6.Path2D = 6;
            data6.Price = 10000;
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
                Sequence diceSeq3= DOTween.Sequence();
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

                cardChun.DOMove(new Vector3(2.51f, 0.51f, -2.1f), 1.5f).SetLoops(-1, LoopType.Yoyo);
                cardChun.DORotate(new Vector3(0, 0, -24.552f), 1.5f).SetLoops(-1, LoopType.Yoyo);
                cardFa.DOScale(new Vector3(1.05f, 1.05f, 1), 1.5f).SetEase(Ease.OutElastic).SetLoops(-1, LoopType.Yoyo);
                cardBai.DOMove(new Vector3(1.24f, 0.48f, -2f), 1.5f).SetLoops(-1, LoopType.Yoyo);
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

            //點廣告領金幣
            if (coinAdSign) {
                coinAdSign.DOLocalMoveY(30, 1.5f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo).Pause();
            }

            Transform depositeFlash = depositPanel.transform.Find("bg/flash");
            if(depositeFlash)
                depositeFlash.DOLocalRotate(new Vector3(0, 0, 180), 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        //---
        public void ClickRank()
        {
            if (btmMenuBtns[0])
                btmMenuBtns[0].DOScale(1.05f, 0.1f).SetEase(Ease.InOutBack).SetLoops(2, LoopType.Yoyo);

            BirtnRankItem(); //產生排行榜項目

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
            }
        }

        public void BirtnRankItem()
        {
            foreach (Transform child in rankItemTarget)
                Destroy(child.gameObject);

            List<RankItemInfo> rankItemInfos = loadRankData();
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
            RankItemInfos itemInfos = new RankItemInfos();
            RankItemInfo data1 = new RankItemInfo();
            data1.Rank = 1;
            //data1.Photo = 1;
            data1.Name = "萱萱寶貝";
            data1.Lv = 192;
            data1.Win = 1223;
            data1.Lose = 5;
            data1.Probability = 76.9f;
            itemInfos.dataList.Add(data1);

            RankItemInfo data2 = new RankItemInfo();
            data2.Rank = 2;
            //data2.Photo = 2;
            data2.Name = "萌萌小野兔Q妹";
            data2.Lv = 188;
            data2.Win = 8392;
            data2.Lose = 129;
            data2.Probability = 94.1f;
            itemInfos.dataList.Add(data2);

            RankItemInfo data3 = new RankItemInfo();
            data3.Rank = 3;
            //data3.Photo = 3;
            data3.Name = "飛炫北鼻萱萱";
            data3.Lv = 220;
            data3.Win = 3223;
            data3.Lose = 215;
            data3.Probability = 88.9f;
            itemInfos.dataList.Add(data3);

            RankItemInfo data4 = new RankItemInfo();
            data4.Rank = 4;
            //data4.Photo = 4;
            data4.Name = "大老闆抽雪茄";
            data4.Lv = 109;
            data4.Win = 291;
            data4.Lose = 9;
            data4.Probability = 87.1f;
            itemInfos.dataList.Add(data4);

            RankItemInfo data5 = new RankItemInfo();
            data5.Rank = 5;
            //data1.Photo = 5;
            data5.Name = "永遠第五人";
            data5.Lv = 79;
            data5.Win = 542;
            data5.Lose = 12;
            data5.Probability = 69.3f;
            itemInfos.dataList.Add(data5);

            return itemInfos.dataList;
        }

        public void ReadyToLoadNextRank() {
            int num = rankItemTarget.childCount;
            CanvasGroup empty = rankPanel.transform.Find("empty").GetComponent<CanvasGroup>();

            if (rankScrollbar.value == 0) {
                if (num < 20)
                {
                    List<RankItemInfo> rankItemInfos = loadNextRankData(num);
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

        private List<RankItemInfo> loadNextRankData(int cuurTotal) {
            RankItemInfos itemInfos = new RankItemInfos();

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
            
            for (int i = 0; i < 10; i++)
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
                dropDown.DOMoveY(9.5f, 0, true);
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
                dropDown.DOMoveY(9.5f, 0.3f, true).SetEase(Ease.InSine); ;
            }
        }

        public void ShowSettingLayout()
        {
            string _target = EventSystem.current.currentSelectedGameObject.name;
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

            childSetting.SetActive(false);
            childSettingService.SetActive(false);
            childSettingRule.SetActive(false);

            switch (_target)
            {
                case "Btn_setting": //設定頁
                    childSetting.SetActive(true);
                    break;
                case "Btn_service"://客服頁
                    if (childSettingService)
                    {
                        InputField _text = childSettingService.transform.Find("Content/InputField").GetComponent<InputField>();
                        _text.text = "請詳述您的問題：\n\n方便聯絡的時間：\n\n聯絡電話：";
                        childSettingService.SetActive(true);
                        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                    }            
                    break;
                case "Btn_rules": //條款頁
                    childSettingRule.SetActive(true);
                    break;
            }

            if (settingPanelNew)
            {
                settingPanelNew.transform.DOMoveX(-19.5f, 0, true);
                settingPanelNew.transform.DOMoveX(0, 0.3f, true).SetEase(Ease.OutCubic);
                ExitSetting();
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
                profilePanel = childSetting.transform.Find("Profile").gameObject;
                gamePanel = childSetting.transform.Find("Game").gameObject;
                recordPanel = childSetting.transform.Find("Record").gameObject;

                if (profilePanel) {
                    settingNickname = profilePanel.transform.Find("IF_nick").GetComponent<InputField>();
                    replaceNickname = profilePanel.transform.Find("Btn_changeName").GetComponent<Button>();
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

            actDailyPanel = activityPanel.transform.Find("Daily").gameObject;
            actMissionPanel= activityPanel.transform.Find("Mission").gameObject;

            if (rankPanel)
                playerRankPanel = rankPanel.transform.Find("playerRank");

        }

        public void ClickBalloonBtn() {
            AudioManager.Instance.PlaySE("gp" + UnityEngine.Random.Range(0, 9));
            BirthBalloon();
        }

        private void AutoBirthBalloon() {
            StartCoroutine("RandomBalloon");
        }

        IEnumerator RandomBalloon() {
            int _time = UnityEngine.Random.Range(1, 5); //一次最多5顆
            for (int i = 0; i < _time; i++)
            {
                yield return new WaitForSeconds(0.5f);
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

        public void ShowLogoutPopup()
        {
            GameObject popupBG = profilePanel.transform.Find("popupBG").gameObject;

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
            GameObject popupBG = profilePanel.transform.Find("popupBG").gameObject;

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

        private void IniPlayerInfo() {
            SetPlayerPhotos();
            SetPlayerNames();
            SetPlayerCoins();
            SetPlayerLevels();
            SetUserOnline();
        }

        public void SetPlayerPhotos()
        {
            string sPhoto = PlayerPrefs.GetString("USERPHOTO");
            if (!string.IsNullOrEmpty(sPhoto))
            {
                Texture2D newPhoto = new Texture2D(1, 1);
                newPhoto.LoadImage(Convert.FromBase64String(sPhoto));
                newPhoto.Apply();

                for (int i = 0; i < playerPhotos.Length; i++)
                {
                    playerPhotos[i].sprite = Sprite.Create(newPhoto, new Rect(0, 0, newPhoto.width, newPhoto.height), Vector2.zero);
                }
            }
        }

        public void SetPlayerNames()
        {
            string sName = PlayerPrefs.GetString("USERNAME");
            if (!string.IsNullOrEmpty(sName))
            {
                for (int i = 0; i < playerNames.Length; i++)
                {
                    playerNames[i].text = sName;
                }

                if(settingNickname) //設定頁暱稱欄位
                    settingNickname.text = sName;
            }
        }

        public void SetPlayerLevels()
        {
            string sLevel = PlayerPrefs.GetString("USERLEVEL");
            if (!string.IsNullOrEmpty(sLevel))
            {
                for (int i = 0; i < playerLvs.Length; i++)
                {
                    playerLvs[i].text = "Lv " + sLevel;
                }
            }
        }

        public void SetPlayerCoins()
        {
            string sCoin = PlayerPrefs.GetString("USERCOIN");
            if (!string.IsNullOrEmpty(sCoin))
            {
                for (int i = 0; i < playerCoins.Length; i++)
                {
                    playerCoins[i].text = string.Format("{0:0,0}", int.Parse(sCoin));
                }
            }
        }

        public void SetUserOnline()
        {
            string sOnline = PlayerPrefs.GetString("USERONLINE");
            if (!string.IsNullOrEmpty(sOnline))
            {
                userOnline.text = "在線人數 " + string.Format("{0:0,0}", int.Parse(sOnline));
            }
        }

        public void NicknameChange() {
            if (replaceNickname)
                replaceNickname.interactable = true;
        }

        public void SaveNickname()
        {
            if (settingNickname) {
                PlayerPrefs.SetString("USERNAME", settingNickname.text);
                SetPlayerNames();
            }

            if (replaceNickname)
                replaceNickname.interactable = false;
        }
    }
}