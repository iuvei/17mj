using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class VideoRecordingBridge {

string MessageReceivingGameObjectNameVariable;
string MessageFromIosReceivingMethodNameVariable;
//----------------------------------------------Show Recording-------------------------------------------------------------------
	#if UNITY_IOS || UNITY_ANDROID
	[DllImport("__Internal")]
	private static extern void _startRecord ();
	public static void StartRecord(){
		#if !UNITY_EDITOR && UNITY_IOS
		_startRecord ();
		#endif
	}
	[DllImport("__Internal")]
	private static extern void _stopRecord ();
	public static void StopRecord(){
		#if !UNITY_EDITOR && UNITY_IOS
		_stopRecord ();
		#endif
	}
		
	[DllImport("__Internal")]
	private static extern void _startPlay (string str);
	public static void StartPlay(string str){
	#if !UNITY_EDITOR && UNITY_IOS
	_startPlay (str);
	#endif
	}
	[DllImport("__Internal")]
	private static extern void _stopPlay ();
	public static void StopPlay(){
	#if !UNITY_EDITOR && UNITY_IOS
	_stopPlay ();
	#endif
	}
	#endif
}