using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxGirl : MonoBehaviour {
    public GameObject _MJToy;

    [HideInInspector]
    public bool isShowToy = true;

    public void SetVisibality() {
        if (_MJToy) {
            _MJToy.SetActive(isShowToy);
        }
    }

}
