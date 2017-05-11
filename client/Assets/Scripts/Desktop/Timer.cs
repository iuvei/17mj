using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.Desktop
{
    public class Timer : MonoBehaviour
    {

        public Image image;
		public Text Remain;

        public int time = 0;

		void Awake()
		{
			if (image != null) {
				image.enabled = false;
			}
		}

        public void Show()
        {
			//Debug.Log (this.name+".Show()");
			if (time > 0) {
				image.enabled = true;
				if (Remain != null) {
					Remain.text = time.ToString ();
				}
			} else {
				image.enabled = false;
				if (Remain != null) {
					Remain.text = string.Empty;
				}
			}
            //StartCoroutine(ShowTimer(time));
        }
		/*
        private IEnumerator ShowTimer(int time)
        {
			//Debug.Log ("ShowTimer("+time+")");
            if (time <= 0)
            {
                //ActiveNext;
                Hide();
                //PhotonNetwork.RaiseEvent(5, null, true, null);
                yield break;
            }

            yield return new WaitForSeconds(1f);

            //this.transform.Rotate(new Vector3(0, 0, 1));
            time--;

            //StartCoroutine(ShowTimer(time));
        }
        */

        public void Hide()
        {
			//Debug.Log (this.name+".Hide()");
            image.enabled = false;
        }
    }
}