using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestartPanel : PanelBase
{
    private Image adBcImage;


    protected override void OnInitFront()
    {
        base.OnInitFront();
        _type = PanelName.RestartPanel;
        _openDuration = 0.5f;
        _showStyle = PanelMgr.PanelShowStyle.Nomal;
        _maskStyle = PanelMgr.PanelMaskSytle.TranslucenceNone;
        _cache = false;
    }

    protected override void OnInitSkinFront()
    {
        base.OnInitSkinFront();
        SetMainSkinPath("Panel/RestartPanel");
    }

    public override void OnInit(params object[] sceneArgs)
    {
        base.OnInit(sceneArgs);
        skinTrs.GetComponent<RectTransform>().sizeDelta = M_Canvas.sizeDelta;
        InitData();
    }


    protected override void Close()
    {
        base.Close();
        Config.Instance.isOpenCheckpoint = false;
        Config.Instance.JudgeLevelUp();
    }


    private bool isWin;

    private void InitData()
    {
        Config.Instance.isOpenCheckpoint = true;

        if (_panelArgs.Length != 0)
        {
            isWin = (bool)_panelArgs[0];
        }
    }

    protected override void OnClick(Transform target)
    {
        switch (target.name)
        {
            case "Btn_Continue":
                Close();
                break;
            case "Btn_Close":
                Close();
                break;
        }
    }
}