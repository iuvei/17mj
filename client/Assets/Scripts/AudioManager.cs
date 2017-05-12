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
	private AudioSource bgmSource = null;
	private List<AudioSource> seSources = new List<AudioSource>( );
	private Dictionary<string , AudioClip> bgmDict = new Dictionary<string , AudioClip>( );
	private Dictionary<string , AudioClip> seDict = new Dictionary<string , AudioClip>( );
	private float _nowvolume = 0.5f;


	#region Singleton
	private static AudioManager _instance = null;

	public static AudioManager Instance
	{
		get { return _instance; }
	}

	#endregion

    void Awake()
    {
        if( _instance != null && _instance != this ){
            Destroy(gameObject);
			return;
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
		//create audio sources
		this.bgmSource = this.gameObject.GetComponent<AudioSource>( );
		if (this.bgmSource == null) {
			this.bgmSource = this.gameObject.AddComponent<AudioSource> ();
		}
		this.bgmSource.volume = 0.3f;
		this.bgmSource.loop = true;
		
		loadAllSoundResources ();
    }

	private void loadAllSoundResources() {
		Debug.Log ("載入所有音效資源");
		object[] bgmList = Resources.LoadAll(BGM_PATH);
		Debug.Log ("載入背景音樂檔:"+bgmList.Length);
		object[] seList = Resources.LoadAll(SE_PATH);
		Debug.Log ("載入音效檔:"+seList.Length);

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
			source.volume = this._nowvolume;
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
		this.bgmSource.Stop( );
		this.bgmSource.clip = this.bgmDict[bgmName];
		this.bgmSource.Play( );
	}
	public float PlayBGMLength(string bgmName) {
		this.bgmSource.clip = this.bgmDict[bgmName];
		return this.bgmSource.clip.length;
	}
	public void StopBGM() {
		this.bgmSource.Stop( );
		this.bgmSource.clip = null;
	}

	public void Mute() {
		this._nowvolume = .0f;
		this.bgmSource.volume = this._nowvolume;
		this.seSources.ForEach(s => s.volume = this._nowvolume);
	}

	public void unMute() {
		this._nowvolume = .5f;
		this.bgmSource.volume = this._nowvolume;
		this.seSources.ForEach(s => s.volume = this._nowvolume);
	}
}
