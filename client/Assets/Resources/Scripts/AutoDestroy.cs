using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
    public float _time;

	void Start () {
        if (_time > 0)
            Invoke("Destroy", _time);
	}

    private void Destroy() {
        Destroy(gameObject);
    }

}
