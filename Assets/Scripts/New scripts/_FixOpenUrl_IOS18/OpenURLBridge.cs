using UnityEngine;
using System.Runtime.InteropServices;

public class OpenURLBridge
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void OpenURLiOS(string url);
#endif

    public static void OpenURL(string url)
    {
#if UNITY_IOS && !UNITY_EDITOR
        OpenURLiOS(url);
#else
        Application.OpenURL(url); // Android, Editor, PC
#endif
    }
}