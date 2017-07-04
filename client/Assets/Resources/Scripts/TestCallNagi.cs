using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCallNagi : MonoBehaviour {
    public bool _callChi = false;
    public bool _callPon = false;
    public bool _callGan = false;
    public bool _callTin = false;
    public bool _callHuSmall = false;
    public bool _callPau = false;
    public bool _callHu = false;
    public bool _callTsumo = false;

    public Nagieffect nagiEffect;

    void Update() {
        if (_callChi) {
            nagiEffect.ShowNagi(Nagieffect.NagiType.CHI);
            _callChi = false;
        }

        if (_callPon) {
            nagiEffect.ShowNagi(Nagieffect.NagiType.PON);
            _callPon = false;
        }
            
        if (_callGan) {
            nagiEffect.ShowNagi(Nagieffect.NagiType.GAN);
            _callGan = false;
        }
           
        if (_callTin) {
            nagiEffect.ShowNagi(Nagieffect.NagiType.TIN);
            _callTin = false;
        }
           
        if (_callHuSmall) {
            nagiEffect.ShowNagi(Nagieffect.NagiType.HU);
            _callHuSmall = false;
        }

        if (_callHu)
        {
            nagiEffect.ShowNagi(Nagieffect.NagiType.HU2);
            _callHu = false;
        }

        if (_callPau) {
            nagiEffect.ShowNagi(Nagieffect.NagiType.PAU);
            _callPau = false;
        }

        if (_callTsumo)
        {
            nagiEffect.ShowNagi(Nagieffect.NagiType.TSUMO);
            _callTsumo = false;
        }

    }
}
