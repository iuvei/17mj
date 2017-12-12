using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class SoundEffect : MonoBehaviour {
    public static SoundEffect Instance;

    //public AudioClip _MoveSE;
    //public AudioClip _FallSE;
    //public AudioClip _DeleteSE;
    //public AudioClip _OverSE;

    private bool _isFadeOut = false;
    private float _volume;


    void Awake() {
        Instance = this;
    }

    public void PlayLoop(AudioClip clip) {
        SoundEffectSource.instance.AudioSource.clip = clip;
        SoundEffectSource.instance.AudioSource.loop = true;
        SoundEffectSource.instance.AudioSource.Play();
    }

    public void Stop() {
        SoundEffectSource.instance.AudioSource.Stop();
    }

    public void PlayOnce(AudioClip clip) {
        if (SoundEffectSource.instance)
            SoundEffectSource.instance.AudioSource.PlayOneShot(clip);
    }

    public void FadeOut() {
        if (SoundEffectSource.instance) {
            _volume = SoundEffectSource.instance.AudioSource.volume;
            _isFadeOut = true;
        }
    }

    void Update() {
        if (_isFadeOut && SoundEffectSource.instance) {
            if (SoundEffectSource.instance.AudioSource.volume > 0)
            {
                SoundEffectSource.instance.AudioSource.volume -= 0.05f * Time.deltaTime;
            } else if (SoundEffectSource.instance.AudioSource.volume < 0.01f) {
                Stop();
                SoundEffectSource.instance.AudioSource.volume = _volume;
                _isFadeOut = false;
            }
        }
    }


    //public void PlayMove()
    //{
    //    if (SoundEffectSource.instance) SoundEffectSource.instance.AudioSource.PlayOneShot(_MoveSE);
    //}

    //public void PlayFall()
    //{
    //    if (SoundEffectSource.instance) SoundEffectSource.instance.AudioSource.PlayOneShot(_FallSE);
    //}

    //public void PlayDelete()
    //{
    //    if (SoundEffectSource.instance) SoundEffectSource.instance.AudioSource.PlayOneShot(_DeleteSE);
    //}

    //public void PlayOver()
    //{
    //    if (SoundEffectSource.instance) SoundEffectSource.instance.AudioSource.PlayOneShot(_OverSE);
    //}

    
}
