using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour 
{
	private const string BGM_PATH = "Sounds/BGM";
	private const string SE_PATH = "Sounds/SE";
	public int MaxSE = 10;
	public AudioSource bgmSource = null;
	public List<AudioSource> seSources = new List<AudioSource>( );
	public Dictionary<string , AudioClip> bgmDict = new Dictionary<string , AudioClip>( );
	public Dictionary<string , AudioClip> seDict = new Dictionary<string , AudioClip>( );
	private float sound_volume = 0.1f;
	private float bgm_volume = 0.1f;
	private bool bgm_enabled = false;
	private bool sound_enabled = false;
    private bool vibrate_enabled = false;
    private bool beauty_enabled = false;

    #region Singleton
    private static AudioManager _instance = null;

	public static AudioManager Instance
	{
		get { return _instance; }
	}

	#endregion

    void Awake()
    {
   //     if( _instance != null && _instance != this ){
   //         Destroy(gameObject);
			//return;
   //     } else {
   //         _instance = this;
   //         DontDestroyOnLoad(gameObject);
   //     }
        _instance = this;
        loadPLayerPrefs ();

		//create audio sources
		this.bgmSource = this.gameObject.GetComponent<AudioSource>( );
		if (this.bgmSource == null) {
			this.bgmSource = this.gameObject.AddComponent<AudioSource> ();
		}
		this.bgmSource.volume = this.bgm_volume;
		this.bgmSource.loop = true;
		
		loadAllSoundResources ();
    }

	private void loadPLayerPrefs() {
		//this.bgm_volume = PlayerPrefs.GetFloat ("BGM_Volume");
		//this.sound_volume = PlayerPrefs.GetFloat ("Sound_Volume");
		this.bgm_enabled = PlayerPrefExtension.GetBool ("BGM_enabled");
		if (this.bgm_enabled) {
			this.bgm_volume = PlayerPrefs.GetFloat ("BGM_Volume");
		} else {
			this.bgm_volume = .0f;
		}
			
		this.sound_enabled = PlayerPrefExtension.GetBool ("Sound_enabled");
		if (this.sound_enabled) {
			this.sound_volume = PlayerPrefs.GetFloat ("Sound_Volume");
		} else {
			this.sound_volume = .0f;
		}

        this.vibrate_enabled = PlayerPrefExtension.GetBool("Vibrate_enabled");

        this.beauty_enabled = PlayerPrefExtension.GetBool("Beauty_enabled");
        //Debug.Log ("AudioManager loadPLayerPrefs() bgm_enabled="+this.bgm_enabled);
    }

	private void loadAllSoundResources() {
		//Debug.Log ("[c] 載入所有音效資源");
		object[] bgmList = Resources.LoadAll(BGM_PATH);
		//Debug.Log ("[c] 載入背景音樂檔:"+bgmList.Length);
		object[] seList = Resources.LoadAll(SE_PATH);
		//Debug.Log ("[c] 載入音效檔:"+seList.Length);

		foreach ( AudioClip bgm in bgmList ) {
			//Debug.Log ("bgm="+bgm.name);
			this.bgmDict[bgm.name] = bgm;
		}
		foreach ( AudioClip se in seList ) {
			//Debug.Log ("se="+se.name);
			this.seDict[se.name] = se;
		}
	}

	public void PlaySE(string seName) {
		if ( !this.seDict.ContainsKey(seName) ) throw new ArgumentException(seName + " not found" , "seName");

		AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
		if ( source == null ) {
			if ( this.seSources.Count >= this.MaxSE ) {
				Debug.Log("SE AudioSource is full");
				return;
			}
			source = this.gameObject.AddComponent<AudioSource>( );
			source.volume = this.sound_volume;
			this.seSources.Add(source);
		}
		source.clip = this.seDict[seName];
		source.Play( );
	}

	public void StopSE() {
		this.seSources.ForEach(s => s.Stop( ));
	}

	public void PlayBGM(string bgmName) {
		if ( !this.bgmDict.ContainsKey(bgmName) ) throw new ArgumentException(bgmName + " not found" , "bgmName");
		if ( this.bgmSource.clip == this.bgmDict[bgmName] ) return;
		//if (!this.bgm_enabled)return;
		this.bgmSource.Stop( );
		this.bgmSource.clip = this.bgmDict[bgmName];
		this.bgmSource.Play( );
	}
	public float PlayBGMLength(string bgmName) {
		this.bgmSource.clip = this.bgmDict[bgmName];
		return this.bgmSource.clip.length;
	}
	public void StopBGM() {
		Debug.Log ("StopBGM()");
		this.bgmSource.Stop( );
		this.bgmSource.clip = null;
	}

	/*
	public void MuteSound() {
		this.sound_volume = .0f;
		this.seSources.ForEach(s => s.volume = this.sound_volume);
	}

	public void unMuteSound() {
		this.sound_volume = 1.0f;
		this.seSources.ForEach(s => s.volume = this.sound_volume);
	}
	*/

	public void ControlBGM(bool isOn) {
		Debug.Log ("ControlBGM("+isOn+")");
		this.bgmSource.mute = !isOn;
		this.bgm_enabled = isOn;
		PlayerPrefExtension.SetBool("BGM_enabled", isOn);
	}

	public void ControlSound(bool isOn) {
		this.sound_enabled = isOn;
		this.seSources.ForEach(s => s.mute = !isOn);
		PlayerPrefExtension.SetBool("Sound_enabled", isOn);
	}

    public void ControlVibrate(bool isOn)
    {
        this.vibrate_enabled = isOn;
        PlayerPrefExtension.SetBool("Vibrate_enabled", isOn);
    }

    public void ControlBeauty(bool isOn)
    {
        this.beauty_enabled = isOn;
        PlayerPrefExtension.SetBool("Beauty_enabled", isOn);
    }

    //public void unMuteBGM() {
    //	this.bgm_volume = 1.0f;
    //	this.bgmSource.volume = this.bgm_volume;
    //}

    public void ChangeSoundVolume(float _volume) {
		this.sound_volume = _volume;
		this.seSources.ForEach(s => s.volume = this.sound_volume);
		PlayerPrefs.SetFloat ("Sound_Volume", this.sound_volume);
	}

	public void ChangeBGMVolume(float _volume) {
		Debug.Log ("ChangeBGMVolume("+_volume+")");
		if (_volume > 0) {
			this.bgm_volume = _volume;
			this.bgmSource.volume = this.bgm_volume;
			PlayerPrefs.SetFloat ("BGM_Volume", this.bgm_volume);
			PlayerPrefExtension.SetBool("BGM_enabled", true);
		}
		/*
		if (this.bgm_volume > 0) {
			if (!this.bgm_enabled) {
				this.bgm_enabled = true;
				PlayerPrefExtension.SetBool ("Sound_enabled", this.bgm_enabled);
				loadPLayerPrefs ();
			}
		}
		*/
	}

	public void Mute() {
		//MuteSound ();
		//MuteBGM ();
	}

	public void unMute() {
		//unMuteSound ();
		//unMuteBGM ();
	}
}
