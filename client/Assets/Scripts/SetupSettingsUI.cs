using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupSettingsUI : MonoBehaviour {
	public Slider BGM_Silder;
	public Slider Sound_Silder;
	public Toggle BGM_Toggle;
	public Toggle Sound_Toggle;
    public Toggle Vibrate_Toggle;
    public Toggle Beauty_Toggle;
    private float bgm_volume;
	private float sound_volume;
	private bool bgm_enabled;
	private bool sound_enabled;
    private bool vibrate_enabled;
    private bool beauty_enabled;
    // Use this for initialization
    void Start () {
		loadPLayerPrefs ();
		init_slider ();
		init_toggle ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void loadPLayerPrefs() {
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
        //Debug.Log ("Beauty_enabled = ???  " + this.beauty_enabled);
    }

	public void init_slider() {
		if (BGM_Silder) {
			BGM_Silder.value = this.bgm_volume;
			BGM_Silder.onValueChanged.AddListener(delegate {BGMValueChange(); });
		}
		if (Sound_Silder) {
			Sound_Silder.value = this.sound_volume;
			Sound_Silder.onValueChanged.AddListener(delegate {SoundValueChange(); });
		}
	}

	public void init_toggle() {
		if (BGM_Toggle) {
			BGM_Toggle.isOn = this.bgm_enabled;
			BGM_Toggle.onValueChanged.AddListener(delegate {BGMToggleValueChange(); });
		}
		if (Sound_Toggle) {
			Sound_Toggle.isOn = this.sound_enabled;
			Sound_Toggle.onValueChanged.AddListener(delegate {SoundToggleValueChange(); });
		}
        if (Vibrate_Toggle)
        {
            Vibrate_Toggle.isOn = this.vibrate_enabled;
            Vibrate_Toggle.onValueChanged.AddListener(delegate { VibrateToggleValueChange(); });
        }

        if (Beauty_Toggle)
        {
            Beauty_Toggle.isOn = this.beauty_enabled;
            Beauty_Toggle.onValueChanged.AddListener(delegate { BeautyToggleValueChange(); });
        }
    }

	public void BGMValueChange()
	{
		//Debug.Log(BGM_Silder.value);
		if (BGM_Silder.value > 0)
			BGM_Toggle.isOn = true;
		//else
		//	BGM_Toggle.isOn = false;
		AudioManager.Instance.ChangeBGMVolume (BGM_Silder.value);
	}

	public void BGMToggleValueChange()
	{
		//Debug.Log("BGM_Toggle.isOn="+BGM_Toggle.isOn);
		AudioManager.Instance.ControlBGM (BGM_Toggle.isOn);
		loadPLayerPrefs ();
		init_slider ();
	}

	public void SoundValueChange()
	{
		//Debug.Log(Sound_Silder.value);
		if (Sound_Silder.value > 0)
			Sound_Toggle.isOn = true;
		//else
		//	Sound_Toggle.isOn = false;
		AudioManager.Instance.ChangeSoundVolume (Sound_Silder.value);
	}

	public void SoundToggleValueChange()
	{
		//Debug.Log("Sound_Toggle.isOn="+Sound_Toggle.isOn);
		AudioManager.Instance.ControlSound (Sound_Toggle.isOn);
		loadPLayerPrefs ();
		init_slider ();
	}

    public void VibrateToggleValueChange()
    {
        //Debug.Log("Vibrate_Toggle.isOn=" + Vibrate_Toggle.isOn);
        AudioManager.Instance.ControlVibrate(Vibrate_Toggle.isOn);
        loadPLayerPrefs();
    }

    public void BeautyToggleValueChange()
    {
        //Debug.Log("Beauty_Toggle.isOn=" + Beauty_Toggle.isOn);
        AudioManager.Instance.ControlBeauty(Beauty_Toggle.isOn);
        loadPLayerPrefs();
    }
}
