using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDown : MonoBehaviour {
    private int countDownTime = 0; //CountDown second
	public Text CountDownText;
	public Animator Anim;

    void Start () {
		gameObject.SetActive (false);
    }

	public void Show(int num) {
		countDownTime = num;
		gameObject.SetActive (true);
		CountDownText.text = countDownTime.ToString ();
		StopCoroutine ("countDown");
		transform.localScale = Vector3.one;
		StartCoroutine ("countDown");
    }

    public void Hide() {
		countDownTime = 0;
		gameObject.SetActive (false);
		CountDownText.text = countDownTime.ToString();
		StopCoroutine ("countDown");
	}

	private IEnumerator countDown() {
		while (countDownTime > 0) {
			yield return new WaitForSeconds (1);
			countDownTime--;
			if (countDownTime > 0 && CountDownText != null) {
                if(Anim)
				    Anim.enabled = true;
				gameObject.SetActive (true);
				CountDownText.text = countDownTime.ToString();
			} else {
                if (Anim)
                    Anim.enabled = false;
				gameObject.SetActive (false);
				StopCoroutine ("countDown");
			}
		}
        ForgotUI.instance.UnLockSendAuthCodeBtn();
    }
}
