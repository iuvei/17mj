using UnityEngine;

internal class PickerUnsupported : IPicker
{
    public void Show(string title, string outputFileName, string objName, int maxSize)
    {
        var message = "Unimgpicker is not supported on this platform.";
        Debug.LogError(message);

        var receiver = GameObject.Find(objName);
        if (receiver != null)
        {
            receiver.SendMessage("OnFailure", message);
        }
    }
}
