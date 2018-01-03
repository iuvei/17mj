﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using DG.Tweening;

public class EnterLoading : MonoBehaviour {
    public static EnterLoading instance;
    public string _sceneName; 
    public Text loadText;
    public Text loadTextA;
    public Image loadImage;
    public float _delayTime = 0;

    public bool _autoToNextScene = false; // 是否載入完自動切換場景
    public bool _fadeOutBGM = false;      // 切換場景前是否要淡出背景音樂
    public bool _fadeOutAnim = true;      // 切換場景前是否要淡出目前場景

    public Transform _runImg;
    private bool _isLoadedDone = false;

    //public Image guideImage;          //遊戲教學圖片輪播
    //public Sprite[] guideImages;
    //private int guideImageNum;
    //private int guideImageIndex = 0;

    void Awake() {
        instance = this;

        //if (_sceneName == "")
        //    Debug.Log("No Found Scene Name");
        //if(!GetComponent<SoundEffect>())
        //    Debug.Log("No Found Sound Effect Component");
        //if (!GetComponent<Animator>())
        //    Debug.Log("No Found Animator Component");

        //guideImageNum = guideImages.Length; //圖片輪播


        if (_runImg) {
            _runImg.localPosition = new Vector2(-500, -140);
        }

        loadTextA.DOText("載入中...", 3).SetLoops(-1, LoopType.Restart);
    }

    // 準備進入遊戲場景
    public void StartLoading() {
        //顯示載入畫面
        if (GetComponent<Animator>())
            GetComponent<Animator>().SetTrigger("EnterLoading");
        //InvokeRepeating("GuideImages", 0f, 2f); //圖片輪播

        StartCoroutine(DisplayLoadingScreen(_sceneName, _delayTime));
    }

   
    IEnumerator DisplayLoadingScreen(string sceneName, float delayT)
    {

        if(delayT > 0)
            yield return new WaitForSeconds(delayT);

        int displayProgress = 0;
        int toProgress = 0;

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            toProgress = (int) async.progress * 100;

            while (displayProgress < toProgress) {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return null;
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return null;
        }
        _isLoadedDone = true; //載入完成

        while (!_autoToNextScene)
            yield return new WaitForSeconds(1);

        if (_fadeOutBGM) {  //背景音樂淡出
            GetComponent<SoundEffect>().FadeOut();     
            yield return new WaitForSeconds(1.0f);
        }
        if (_fadeOutAnim) { //當前場景淡出
            GetComponent<Animator>().SetBool("EnterLoadingDone", true);
            yield return new WaitForSeconds(1.5f);
        }

        GC.Collect(); //釋放記憶體
        async.allowSceneActivation = true;  //進入下一場景

    }

    private void SetLoadingPercentage(int progress) {
        loadText.text = progress.ToString() + " %";
        loadImage.fillAmount = (float)progress / 100;
        //Debug.Log("async.progress = " + progress);

        if (_runImg)
        {
            _runImg.localPosition = new Vector2(-500 + (float)progress*10, -140);
            //Debug.Log("async.progress = " + progress);
        }
    }

    //更換載入畫面教學圖片
    //private void GuideImages() {
    //    guideImageIndex += 1;
    //    guideImage.sprite = guideImages[guideImageIndex % guideImageNum];
    //}

    public bool LoadedDone
    {
        get { return _isLoadedDone; }
    }

}
