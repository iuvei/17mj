using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnterLoading : MonoBehaviour {
    public static EnterLoading instance;
    public string _sceneName; 
    public Text loadText;
    public Image loadImage;

    //public bool _autoToNextScene = true; // 是否載入完自動切換場景
    public bool _fadeOutBGM = false;      // 切換場景前是否要淡出背景音樂

    //public Image guideImage;          //遊戲教學圖片輪播
    //public Sprite[] guideImages;
    //private int guideImageNum;
    //private int guideImageIndex = 0;

    void Awake() {
        instance = this;

        if (_sceneName == "")
            Debug.Log("No Found Scene Name");
        if(!GetComponent<SoundEffect>())
            Debug.Log("No Found Sound Effect Component");
        if (!GetComponent<Animator>())
            Debug.Log("No Found Animator Component");

        //guideImageNum = guideImages.Length; //圖片輪播
    }

    // 準備進入遊戲場景
    public void StartLoading() {
        //顯示載入畫面
        GetComponent<Animator>().SetTrigger("EnterLoading");
        //InvokeRepeating("GuideImages", 0f, 2f); //圖片輪播

        StartCoroutine(DisplayLoadingScreen(_sceneName));
    }

   
    IEnumerator DisplayLoadingScreen(string sceneName)
    {
        yield return new WaitForSeconds(3f);

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

        yield return new WaitForSeconds(1);
        if (_fadeOutBGM) {  //背景音樂淡出
            GetComponent<SoundEffect>().FadeOut();     
            yield return new WaitForSeconds(1.0f);
        }
        GetComponent<Animator>().SetBool("EnterLoadingDone", true);
        yield return new WaitForSeconds(1.5f);

        async.allowSceneActivation = true;  //進入下一場景
    }

    private void SetLoadingPercentage(int progress) {
        //loadText.text = progress.ToString() + " %";
        loadImage.fillAmount = (float)progress / 100;
        //Debug.Log("async.progress = " + progress);
    }

    //更換載入畫面教學圖片
    //private void GuideImages() {
    //    guideImageIndex += 1;
    //    guideImage.sprite = guideImages[guideImageIndex % guideImageNum];
    //}
}
