using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    bool run = false;
    bool showTimeLeft = true;

    float startTime = 0.0f;
    float curTime = 0.0f;
    string curStrTime = string.Empty;
    bool pause = false;

    public float timeAvailable = 30f; // 30 sec
    float showTime = 0;

    public Text guiTimer;
	public GameObject finishedUI;
	public FruitDispenser fd;

    void Start()
    {
        RunTimer();
    }

    public void RunTimer()
    {
        run = true;
        startTime = Time.time;
    }

    public void PauseTimer(bool b)
    {
        pause = b;
    }
		
	void Update () {

        if (pause)
        {
            startTime = startTime + Time.deltaTime;
            return;
        }

        if (run)
        {
            curTime = Time.time - startTime;
        }

        if (showTimeLeft)
        {
            showTime = timeAvailable - curTime;
            if (showTime <= 0)
            {
                showTime = 0;

				Pause();
				finishedUI.SetActive(true);
            }
        }

        int minutes = (int) (showTime / 60);
        int seconds = (int) (showTime % 60);
        int fraction = (int) ((showTime * 100) % 100);

        curStrTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
        guiTimer.text = "Time: " + curStrTime;
	
	}

	public void Pause()
	{
		Rigidbody[] rs = GameObject.FindObjectsOfType<Rigidbody> ();

		foreach (Rigidbody r in rs) {
			r.Sleep ();
			fd.pause = true;
		}
	}
}
