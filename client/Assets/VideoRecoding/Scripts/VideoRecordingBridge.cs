using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class VideoRecordingBridge {
	string MessageReceivingGameObjectNameVariable;
	string MessageFromIosReceivingMethodNameVariable;
//----------------------------------------------Show Recording-------------------------------------------------------------------
	#if UNITY_IOS || UNITY_ANDROID

    #if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void _startRecord ();
	[DllImport("__Internal")]
	private static extern void _stopRecord ();
	[DllImport("__Internal")]
	private static extern void _startPlay (string str);
	[DllImport("__Internal")]
	private static extern void _stopPlay ();
	#elif UNITY_ANDROID
	private static AndroidJavaObject live = null;
	private static bool init = true;
	public static void setup(){
		if(init){
            live = new AndroidJavaClass("com.biginnovation.live.LiveRec");
            live.CallStatic("AddLiveView");
            init = false;
        }
	}
    #endif

	public static void StartRecord(){
		#if !UNITY_EDITOR && UNITY_IOS
		_startRecord ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("RecStart");
		#endif
	}

	public static void StopRecord(){
		#if !UNITY_EDITOR && UNITY_IOS
		_stopRecord ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("RecStop");
		#endif
	}

	public static void StartPlay(string str){
		#if !UNITY_EDITOR && UNITY_IOS
		_startPlay (str);
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("PlayStart");
		#endif
	}


	public static void StopPlay(){
		#if !UNITY_EDITOR && UNITY_IOS
		_stopPlay ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("PlayStop");
		#endif
	}

    public static void REConnect()
    {
#if !UNITY_EDITOR && UNITY_IOS
#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("PlayStart");
#endif
    }

#endif
}