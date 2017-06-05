using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class VideoRecordingBridge {

string MessageReceivingGameObjectNameVariable;
string MessageFromIosReceivingMethodNameVariable;
//----------------------------------------------Show Recording-------------------------------------------------------------------
	#if UNITY_IOS || UNITY_ANDROID
	[DllImport("__Internal")]
	private static extern void _initRecord ();
	public static void InitRecord(){
		#if !UNITY_EDITOR
		_initRecord ();
		#endif
	}
//----------------------------------------------Show Or Hide Recording-------------------------------------------------------------------
// if isSowRecording is true then recording will be shown in recording view and if false then recording will be hideen
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
#endif
}