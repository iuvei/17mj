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
	private static extern void _startRecord (string str);
	[DllImport("__Internal")]
	private static extern void _stopRecord ();
	[DllImport("__Internal")]
	private static extern void _startPlay (string url, string room);
	[DllImport("__Internal")]
	private static extern void _stopPlay ();
	[DllImport("__Internal")]
	private static extern void _moveRight ();
	[DllImport("__Internal")]
	private static extern void _moveLeft ();

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

	public static void StartRecord(string str){
		//Debug.Log ("StartRecord('"+str+"')");
		Debug.Log ("StartRecord()");
		#if !UNITY_EDITOR && UNITY_IOS
		_startRecord (str);
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("RecStart",str);
        #endif
    }

    public static void StopRecord(){
		Debug.Log ("StopRecord()");
		#if !UNITY_EDITOR && UNITY_IOS
		_stopRecord ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("RecStop");
		#endif
	}

	public static void StartPlay(string str, string room){
		Debug.Log ("StartPlay('"+str+"')");
        #if !UNITY_EDITOR && UNITY_IOS
	    _startPlay (str,room);
        #elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("PlayStart",str,room);
        #endif
    }


    public static void StopPlay(){
		Debug.Log ("StopPlay()");
		#if !UNITY_EDITOR && UNITY_IOS
		_stopPlay ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
		live.CallStatic("PlayStop");
		#endif
	}

	public static void MoveRight(){
		//Debug.Log ("MoveRight()");
		#if !UNITY_EDITOR && UNITY_IOS
		_moveRight ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		//setup();
		//live.CallStatic("PlayStop");
		#endif
	}

	public static void MoveLeft(){
		//Debug.Log ("MoveRight()");
		#if !UNITY_EDITOR && UNITY_IOS
		_moveLeft ();
		#elif !UNITY_EDITOR && UNITY_ANDROID
		//setup();
		//live.CallStatic("PlayStop");
		#endif
	}

	public static void REConnect(string str)
    {
        #if !UNITY_EDITOR && UNITY_IOS
        #elif !UNITY_EDITOR && UNITY_ANDROID
		setup();
	Debug.Log ("REConnect="+str);
	    live.CallStatic("PlayStart",str);
        #endif
    }

#endif
}
