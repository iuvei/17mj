using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
#if UNITY_5 && (UNITY_IOS || UNITY_TVOS)
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class MyPluginPostProcessBuild
{
	[PostProcessBuild]
	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
	{
		#if UNITY_5 && (UNITY_IOS || UNITY_TVOS)
		if ( buildTarget == BuildTarget.iOS )
		{
			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// background location useage key (new in iOS 8)
			rootDict.SetString("NSMicrophoneUsageDescription", "xxx");
			rootDict.SetString("NSCameraUsageDescription", "yyy");
			rootDict.SetString ("UIBackgroundModes", "remote-notification");

			// background modes
			PlistElementArray bgModes = rootDict.CreateArray("UIBackgroundModes");
			bgModes.AddString("remote-notification");
			//bgModes.AddString("location");
			//bgModes.AddString("fetch");

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());

			string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));

			string target = proj.TargetGuidByName("Unity-iPhone");

			proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

			File.WriteAllText(projPath, proj.WriteToString());
		}
		#endif
	}
}