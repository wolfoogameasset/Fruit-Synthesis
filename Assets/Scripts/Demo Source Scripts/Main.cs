using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        
        DataManager.setCreateTime();
        DataManager.setLoginTime();
        Config.Instance.SetFirst();

        SceneMgr.GetInstance.SwitchingScene(SceneType.SplashPanel);
    }

    private void OnApplicationPause(bool focus)
    {
        if (focus)
        {
            Config.Instance.Save();
        }
    }

    private void OnApplicationQuit()
    {
        Config.Instance.Save();
    }
}