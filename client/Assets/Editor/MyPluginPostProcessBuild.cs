using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class MyPluginPostProcessBuild
{
	[PostProcessBuild]
	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
	{
#if UNITY_IOS
		if ( buildTarget == BuildTarget.iOS )
		{
			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// background location useage key (new in iOS 8)
			rootDict.SetString("NSMicrophoneUsageDescription", "Microphone Access Warning");
			rootDict.SetString("NSCameraUsageDescription", "Camera Access Warning");
            rootDict.SetString("NSPhotoLibraryUsageDescription", "Photo Library Access Warning");
			rootDict.SetString ("UIBackgroundModes", "remote-notification");
            rootDict.SetString ("UIRequiresFullScreen", "YES");
			rootDict.SetString ("App Uses Non-Exempt Encryption", "NO");

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

			proj.AddFrameworkToProject(target, "SafariServices.framework", false /*not weak*/);

			proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

			File.WriteAllText(projPath, proj.WriteToString());
		}
#endif
    }
}
