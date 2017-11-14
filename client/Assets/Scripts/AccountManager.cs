using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AccountManager : MonoBehaviour {
	public static AccountManager Instance = null;
	public GameObject ConnectingPanel; // 連線中
	private Transform _connectingSign;
	private Text _connectingText;
	void Awake () {
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		if (ConnectingPanel) {
			_connectingSign = ConnectingPanel.transform.Find("Image/Img").transform;
			_connectingText = ConnectingPanel.transform.Find("Image/Text").GetComponent<Text>();
			if (_connectingSign && _connectingText)
				ConnectingAnim();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowConnecting() {
		if (ConnectingPanel) {
			ConnectingPanel.SetActive (true);
			PlayConnectingAnim ();
		}
	}

	public void HideConnecting() {
		if (ConnectingPanel) {
			ConnectingPanel.SetActive(false);
			StopConnectingAnim();
		}
	}
	public void ConnectingAnim() {
		_connectingSign.DOLocalRotate(new Vector3(0, 0, 180), 1.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
		_connectingText.DOText("連線中...", 3).SetLoops(-1, LoopType.Restart);
	}

	public void StopConnectingAnim()
	{
		if (_connectingSign && _connectingText) {
			_connectingSign.DOPause();
			_connectingText.DOPause();
		}
	}

	public void PlayConnectingAnim()
	{
		if (_connectingSign && _connectingText) {
			_connectingSign.DOPlay();
			_connectingText.DOPlay();
		}
	}
}
