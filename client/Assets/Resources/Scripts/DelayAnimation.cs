using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayAnimation : MonoBehaviour {
    public float _delaySec = 0f;
    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        if (anim && _delaySec > 0) {
            anim.gameObject.SetActive(false);
            Invoke("TurnOnAnim", _delaySec);
        }
    }

    private void TurnOnAnim() {
        anim.gameObject.SetActive(true);
    }
}
