using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Facebook.MiniJSON;
using System;

public class HorseLight : MonoBehaviour {
    public static HorseLight instance;
    public Text _horseLightText_1;
    public Text _horseLightText_2;
    public List<string> _rewardLists;
    public string[] _canMessages; // 當無得獎者 設定的罐頭訊息

    //跑馬燈速度
    [Range(0, 6)]
    public int RunSpeed = 1; 

    [HideInInspector]
    public bool _acceptPass = true; // 允許放行

    [HideInInspector]
    public string _bulletinNum = "5"; // 取DB得獎人數量

    private float _horseLightLength_1;
    private float _horseLightLength_2;
    private bool _fixedTimeCheck = false;

    private bool _horseEmpty_1 = true;
    private bool _horseEmpty_2 = true;
    private bool _horseReadyRun_1 = false;
    private bool _horseReadyRun_2 = false;
    private IDictionary dict;

    private int _cuurSpeed;
    private bool _readyToStart = false; //取得DB資料更新完 準備起跑

    void Awake() {
        instance = this;
    }

    void Start() {
        //Insert test Data
        //MJApi.setBulletin("bulletin", "新手禮包特價中,17玩麻將 一週年紀念活動開跑開跑", setBCallback);
        //MJApi.setBulletin("Reward", "恭喜林小晴1獲得500顆寶石!", setBCallback);

        InvokeRepeating("RegularCallBulletin", 0, 150);
    }

    private void RegularCallBulletin() {
        if (_rewardLists.Count < 10)
            MJApi.getBulletin(_bulletinNum, BulletinCallback);
    }

    public void setBCallback(WebExceptionStatus status, string result)
    {
        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("Failed! " + result);
        }
        //Debug.Log("setBCallback =  " + result);
    }

    public void BulletinCallback(WebExceptionStatus status, string result)
    {
        if (status != WebExceptionStatus.Success)
        {
            Debug.Log("Failed! " + result);
        }
        //Debug.Log("BulletinCallback =  " + result);

        string uBulletin = string.Empty;
        dict = Json.Deserialize(result) as IDictionary;
        if (dict["bulletin"] != null)
        {
            uBulletin = dict["bulletin"].ToString();
            char[] delimiterChars = { ',' };
            string[] words = uBulletin.Split(delimiterChars);
            int i = 0;
            foreach (string s in words)
            {
                _canMessages[i++] = s;
                //Debug.Log("s =  " + s);
            }
        }

        foreach (String key in dict.Keys)
        {
            //Debug.Log("key =  " + key);
            if (key != "bulletin")
            {
                string doc = dict[key].ToString();
                //Debug.Log("doc =  " + doc);
               _rewardLists.Add(doc);
            }
        }

        _readyToStart = true;
    }

    void FixedUpdate() {
        if (_readyToStart)
        {
            ReadyToStart();
            _readyToStart = false;
        }

        //偵測是否準備起跑
        if (_acceptPass) {
            if (_horseReadyRun_1)
            {
                _horseLightText_1.GetComponent<HorseRun>()._horseRun = true;
                _horseReadyRun_1 = false;
                _acceptPass = false;
            }
            else if (_horseReadyRun_2)
            {
                _horseLightText_2.GetComponent<HorseRun>()._horseRun = true;
                _horseReadyRun_2 = false;
                _acceptPass = false;
            }
            else {
                if (!_fixedTimeCheck)
                    InvokeRepeating("CheckNewRewardList", 3, 3);
            }
        }
    }

    //[0] 進入點
    public void ReadyToStart() {
        if (CheckRewardList())
        {
            CheckEmptyHorse();
        }
        else {
            //無得獎者 設定罐頭訊息
            PutCanMsgToList();
        }

    }

    //[1] 檢查中獎清單
    public bool CheckRewardList() {
        if (_rewardLists.Count == 0)
            return false;
        else if (_rewardLists[0] == "") {
            _rewardLists.RemoveAt(0);
            return false;
        }else
            return true;
    }

    //[2] 檢查目前 Text 何者為空
    private void CheckEmptyHorse() {
        if (_horseEmpty_1) {
			if(_horseLightText_1)
            	_horseLightText_1.text = _rewardLists[0];
            _horseEmpty_1 = false;
            StartCoroutine("CalculateHorseLength", 1);
            _rewardLists.RemoveAt(0);
        }
        else if (_horseEmpty_2) {
			if(_horseLightText_2)
            	_horseLightText_2.text = _rewardLists[0];
            _horseEmpty_2 = false;
            StartCoroutine("CalculateHorseLength", 2);
            _rewardLists.RemoveAt(0);
        }
    }

	void OnDisable() {
		StopCoroutine ("CalculateHorseLength");
	}

    //[3] 計算跑馬燈長度
    IEnumerator CalculateHorseLength(int index) {
        yield return new WaitForSeconds(0.5f);

        switch (index) {
			case 1:
			if (_horseLightText_1) {
				_horseLightLength_1 = _horseLightText_1.rectTransform.sizeDelta.x;
				_horseLightText_1.GetComponent<HorseRun> ()._horseLength = _horseLightLength_1;
			}
                break;
			case 2:
			if (_horseLightText_2) {
				_horseLightLength_2 = _horseLightText_2.rectTransform.sizeDelta.x;
				_horseLightText_2.GetComponent<HorseRun> ()._horseLength = _horseLightLength_2;
			}
                break;
            default:
                break;
        }

        ReadyRunHorse(index);
    }

    //[4] 準備發送跑馬燈
    private void ReadyRunHorse(int index) {
        switch (index)
        {
            case 1:
                if (_horseLightLength_1 == 0) {
                    StartCoroutine("CalculateHorseLength", index);
                }
                else {
                    _horseReadyRun_1 = true;
                    //Debug.Log("跑馬燈1長度 = " + _horseLightLength_1 + " 經過時間: " + Time.fixedTime + " 內容 = " + _horseLightText_1.text);
                }
                break;
            case 2:
                if (_horseLightLength_2 == 0) {
                    StartCoroutine("CalculateHorseLength", index);
                }
                else {
                    _horseReadyRun_2 = true;
                    //Debug.Log("跑馬燈2長度 = " + _horseLightLength_2 + " 經過時間: " + Time.fixedTime + " 內容 = " + _horseLightText_2.text);
                }
                break;
            default:
                break;
        }
    }

    //[6] 抵達終點時 清空該Text 且位置回到870 並設置旗標為空
    public void HorseGoal(string textName) {
        switch (textName)
        {
		case "Text_Msg1":
			if (_horseLightText_1) {
				_horseLightText_1.text = "";
				_horseLightText_1.rectTransform.anchoredPosition = new Vector2 (870, _horseLightText_1.rectTransform.anchoredPosition.y);
			}
                _horseEmpty_1 = true;
                break;
		case "Text_Msg2":
			if (_horseLightText_2) {
				_horseLightText_2.text = "";
				_horseLightText_2.rectTransform.anchoredPosition = new Vector2 (870, _horseLightText_2.rectTransform.anchoredPosition.y);
			}
                _horseEmpty_2 = true;
                break;
            default:
                break;
        }
        ReadyToStart();
    }

    //[7] 固定時間檢查有無新名單
    private void CheckNewRewardList() {
        ReadyToStart();
        _fixedTimeCheck = true;
    }

    //[8] 罐頭訊息填入播放清單
    private void PutCanMsgToList() {
        if (_canMessages.Length == 0)
        {
            _rewardLists.Add("恭喜黃大嬸詐胡 獲得9487元");
        }
        else {
            for (int i = 0; i < _canMessages.Length; i++)
            {
                _rewardLists.Add(_canMessages[i]);
            }
        }
    }

    //[9] 暫停& 繼續
    public void IsPlayHorse(bool _isPlay) {
        if (RunSpeed != 0 && !_isPlay)
        {
            _cuurSpeed = RunSpeed;
            RunSpeed = 0;
        }
        else if(RunSpeed == 0 && _isPlay)
            RunSpeed = _cuurSpeed;

        //Debug.Log("目前跑馬燈速度 = " + RunSpeed);
    }

}
