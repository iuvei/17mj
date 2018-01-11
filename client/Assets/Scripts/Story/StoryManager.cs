using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Desktop
{
	public class StoryManager : MonoBehaviour {
		
		public List<MahPlayer> Users = new List<MahPlayer>();
		public List<int> AbanMjs = new List<int>();
		public Mahjong mahJong;

		private static StoryManager _instance = null;
		private int _currentIndex = 0;
		private bool _isgameover = false;
		private MahPlayer _activePlayer = null;
		private int _lastDaPai = 0;
		private int _lastMoPaiPlayerID = -1;
		private int _lastDaPaiPlayerID = -1;
		private int MaxWaitTime = 100;
		private int _remainTime = 0;

		public static StoryManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StoryManager();
				}
				return _instance;
			}
		}

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

		// Use this for initialization
		void Start () {
			AudioManager.Instance.PlayBGM ("BGM_Playing");
			//StopAllCoroutines ();
			StartCoroutine ("ReadyPlay");
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		IEnumerator ReadyPlay() {
			//StopAllCoroutines ();
			//bcGameStart ();
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

		/// <summary>
		/// 绑定玩家与photonplayer
		/// </summary>
		private void doGameInit()
		{
			Debug.LogError ("[s] 0.doGameInit()");
			int[] ids = new int[2];
			//Users [0].ID = PhotonNetwork.player.ID;
			//Users [0].photonPlayer = PhotonNetwork.player;
			Users [0].isAI = false;
			Users [0].keepedMah.Clear ();
			Users [0].ponMah.Clear ();
			Users [0].abandanedMah.Clear ();
			//Users [0].AutoPlay = false;
			//Users [1].ID = pp.ID;
			//Users [1].photonPlayer = pp;
			Users [1].isAI = true;
			Users [1].keepedMah.Clear ();
			Users [1].ponMah.Clear ();
			Users [1].abandanedMah.Clear ();
			//Users [1].AutoPlay = false;
			/*
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
			*/
			/*
			_currentIndex = 0;
			_lastDaPai = 0;
			_lastMoPaiPlayerID = -1;
			_lastDaPaiPlayerID = -1;
			this._isgameover = false;
			_activePlayer = null;
			//Debug.Log (ids);
			photonView.RPC("BundlePlayer", PhotonTargets.Others, ids);
			*/
		}

		private void doShuffleMah()
		{
			Debug.LogError ("[s] 1.doShuffleMah()");
			if(mahJong!=null)
				mahJong.ShuffleMah();
		}

		/// <summary>
		/// 發牌
		/// </summary>
		void doDispatchPai()
		{
			Debug.LogError ("[s] 2.doDispatchPai()");
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
				/*
				if(PhotonNetwork.player.ID!=mp.ID) {
					//	photonView.RPC ("dispatchPai", PhotonTargets.MasterClient, i, mahm.ToArray ());
					//else
					object[] param = { mp.ID, mahm.ToArray() };
					//photonView.RPC ("dispatchPai", PhotonTargets.Others, param);
				}
				*/
				//i++;
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
			//photonView.RPC("ShowActivePlayer", PhotonTargets.All, param);
			_lastMoPaiPlayerID = -1;
			_lastDaPaiPlayerID = -1;
			yield return StartCoroutine (AutoChangeActivePlayerCo ());
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
				//photonView.RPC ("RemainTime", PhotonTargets.All, param);
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

		public int getRemainPai()
		{
			int remain = 0;
			if (mahJong != null && mahJong.allMah != null) {
				remain = mahJong.allMah.Count;
			}
			return remain;
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

		public void bcGameOver()
		{
			this._isgameover = true;
			//photonView.RPC ("GameOver", PhotonTargets.All, null);
			//Invoke("InvateYES", 5.0f);
		}
	}
}
