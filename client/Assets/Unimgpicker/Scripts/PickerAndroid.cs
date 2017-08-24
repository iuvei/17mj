#if UNITY_ANDROID
using UnityEngine;

internal class PickerAndroid : IPicker
{
    private static readonly string PickerClass = "com.kakeragames.unimgpicker.Picker";

    public void Show(string title, string outputFileName, string objName, int maxSize)
    {
        using (var picker = new AndroidJavaClass(PickerClass))
        {
            picker.CallStatic("show", title, outputFileName, objName, maxSize);
        }
    }
}

#endif