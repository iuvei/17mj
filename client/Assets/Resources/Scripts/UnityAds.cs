using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour {
    public GameObject _earnRewardPanel;
    private Text _earnRewardTitle;
    private Text _earnRewardContent;

    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            ShowRewardedAd();
        });

        if (_earnRewardPanel)
        {
            _earnRewardTitle = _earnRewardPanel.transform.Find("main/Title").GetComponent<Text>();
            _earnRewardContent = _earnRewardPanel.transform.Find("main/midBg/Content").GetComponent<Text>();
            _earnRewardPanel.SetActive(false);
        }
            
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
        ShowEarnReward(result);
    }


    private void ShowEarnReward(ShowResult result, string reward = "$ 1,000")
    {
        string _title = string.Empty;
        string _content = string.Empty;

        if (_earnRewardPanel) {
            switch (result) {
                case ShowResult.Finished:
                    _title = "恭喜您獲得";
                    _content = reward;
                    break;
                case ShowResult.Skipped:
                    _title = "真可惜，您跳過了廣告";
                    _content = "$ 0";
                    break;
                case ShowResult.Failed:
                    _title = "抱歉，廣告商遇到了一些問題";
                    _content = "@#$%&*!";
                    break;
            }

            _earnRewardTitle.text = _title;
            _earnRewardContent.text = _content;

            _earnRewardPanel.SetActive(true);

        }
    }

    public void ClickAcceptEarnReward() {
        if (_earnRewardPanel)
            _earnRewardPanel.SetActive(false);
    }

}
