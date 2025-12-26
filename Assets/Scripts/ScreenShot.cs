using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;
public class DevelopTool : EditorWindow
{
    [MenuItem("Tool/ScreenShot #A")]
    public static void ScreenShot()
    {
        string projectName = PlayerSettings.productName;
        string companyName = PlayerSettings.companyName;
        string direct = System.Environment.CurrentDirectory + "/" + companyName + "_" + projectName + "_屏幕截图";
        if (!Directory.Exists(direct))
        {
            Directory.CreateDirectory(direct);
        }


        if (string.IsNullOrEmpty(projectName))
        {
            return;
        }
        string screenType;
        int i = Screen.height * Screen.width / 1000000;
        switch (i)
        {
            case 2:
                screenType = "6";
                //if (!Directory.Exists(direct + "/8"))
                //{
                //    Directory.CreateDirectory(direct + "/8");
                //}
                break;
            case 3:
                screenType = "X";
                //if (!Directory.Exists(direct + "/X"))
                //{
                //    Directory.CreateDirectory(direct + "/X");
                //}
                break;
            case 5:
                screenType = "ipad";
                //if (!Directory.Exists(direct + "/ipad"))
                //{
                //    Directory.CreateDirectory(direct + "/ipad");
                //}
                break;
            default:
                screenType = "";
                break;
        }
        DirectoryInfo TheFolder = new DirectoryInfo(direct);
        int fileNum = 0;
        foreach (FileInfo NextFile in TheFolder.GetFiles())
        {
            if (NextFile.Name.Contains(screenType))
            {
                fileNum++;
            }
           
        }

        string fileName = screenType + (fileNum + 1);
        string tail = ".png";
        string path = direct + "/" + fileName + tail;
        Debug.Log(path);
        ScreenCapture.CaptureScreenshot(path);

    }
}
