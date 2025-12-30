using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneMgr
{

    protected static SceneMgr mInstance;

    public static bool hasInstance
    {
        get { return mInstance != null; }
    }

    public static SceneMgr GetInstance
    {
        get
        {
            if (!hasInstance)
            {
                mInstance = new SceneMgr();
            }

            return mInstance;
        }
    }

    public delegate void OnSwitchingScene(SceneType type);

    public OnSwitchingScene OnSwitchingSceneHandler;

    public Dictionary<SceneType, SceneBase> scenes;

    public SceneBase current;

    private List<SwitchRecorder> switchRecoders;

    private const SceneType mainSceneType = SceneType.MainPanel;

    private SceneMgr()
    {
        scenes = new Dictionary<SceneType, SceneBase>();
        switchRecoders = new List<SwitchRecorder>();
    }

    public void Destroy()
    {
        OnSwitchingSceneHandler = null;

        switchRecoders.Clear();
        switchRecoders = null;

        scenes.Clear();
        scenes = null;
    }

    public void SwitchingScene(SceneType sceneType, params object[] sceneArgs)
    {
        float delayTime = 0;
        if (current != null)
        {
            if (sceneType == current.type)
            {
                //return;
            }
        }
        if (sceneType == mainSceneType)
        {
            switchRecoders.Clear();
        }
        switchRecoders.Add(new SwitchRecorder(sceneType, sceneArgs));
        HideCurrentScene();
        ShowScene(sceneType, sceneArgs);
        if (OnSwitchingSceneHandler != null)
        {
            OnSwitchingSceneHandler(sceneType);
        }
    }

    public void SwitchingToPrevScene()
    {
        if (switchRecoders.Count < 2)
        {
            return;
        }

        SwitchRecorder sr = switchRecoders[switchRecoders.Count - 2];
        switchRecoders.RemoveRange(switchRecoders.Count - 2, 2);
        SwitchingScene(sr.sceneType, sr.sceneArgs);
    }

    private void ShowScene(SceneType sceneType, params object[] sceneArgs)
    {
        if (scenes.ContainsKey(sceneType))
        {
            current = scenes[sceneType];
            current.OnShowing();
            current.OnResetArgs(sceneArgs);
            current.gameObject.SetActive(true);
            current.OnShowed();
        }
        else
        {
            if (sceneType == SceneType.None)
            {
                current = null;
                return;
            }

            GameObject go = new GameObject(sceneType.ToString());
            Type mType = Type.GetType(sceneType.ToString());
            current = go.AddComponent(mType) as SceneBase;
            current.OnInit(sceneArgs);
            scenes.Add(current.type, current);
            current.OnShowing();
            LayerMgr.GetInstance.SetLayer(current.gameObject, LayerType.Scene);
            go.transform.localPosition(Vector3.zero).localRotation(Quaternion.identity).localScale(1);
            current.OnShowed();
        }
    }

    private void HideCurrentScene()
    {
        if (current != null)
        {
            current.OnHiding();
            current.OnHided();
            if (!current.cache)
            {
                scenes.Remove(current.type);
                GameObject.Destroy(current.gameObject);
            }
        }
    }

    internal struct SwitchRecorder
    {
        internal SceneType sceneType;
        internal object[] sceneArgs;

        internal SwitchRecorder(SceneType sceneType, params object[] sceneArgs)
        {
            this.sceneType = sceneType;
            this.sceneArgs = sceneArgs;
        }
    }
}