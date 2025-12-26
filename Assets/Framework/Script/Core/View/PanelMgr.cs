using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMgr
{

    protected static PanelMgr mInstance;

    public static bool hasInstance
    {
        get { return mInstance != null; }
    }

    public static PanelMgr GetInstance
    {
        get
        {
            if (!hasInstance)
            {
                mInstance = new PanelMgr();
            }

            return mInstance;
        }
    }

    private PanelMgr()
    {
        panels = new Dictionary<PanelName, PanelBase>();
        panelsDethList = new List<PanelBase>();
    }

    public Action<object> ShowAction;
    public Action<object> HideAction;


    public enum PanelShowStyle
    {
        Nomal,

        CenterScaleBigNomal,

        UpToSlide,

        DownToSlide,

        LeftToSlide,

        RightToSlide,

        SomeplaceToSlide
    }

    public enum PanelMaskSytle
    {
        None,

        OpacityNone,

        Opacity,

        TranslucenceNone,

        Translucence,

        LucencyNone,

        Lucency,
    }

    public Dictionary<PanelName, PanelBase> panels;

    public List<PanelBase> panelsDethList;

    public PanelBase current;

    public Queue<PannelModel> PannelQueue = new Queue<PannelModel>();


    public void Destroy()
    {
    }

    public void ShowPanel(PanelName panelName, params object[] panelArgs)
    {
        MainPanel.isXIALUO = false;
        if (panels.ContainsKey(panelName))
        {
            current = panels[panelName];
        }
        else
        {
            GameObject go = new GameObject(panelName.ToString());
            Type mType = Type.GetType(panelName.ToString());
            PanelBase pb = go.AddComponent(mType) as PanelBase; 
            pb.OnInit(panelArgs);
            pb.isShow = true;
            panels.Add(pb.type, pb);
            MaskStyle(pb);
            panelsDethList.Add(pb);
            ChangePanelDeth(pb);
            current = pb;
            pb.OnShowing();
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            StartShowPanel(current, current.PanelShowStyle, true);
        }
    }

    public void AddPanel(PanelName panelName, params object[] panelArgs)
    {
        PannelModel pm = new PannelModel();
        pm.pn = panelName;
        pm.po = panelArgs;
        PannelQueue.Enqueue(pm);
        TestingPannel();
    }

    public void TestingPannel()
    {
        if (PannelQueue.Count >= 1)
        {
            var p = PannelQueue.Dequeue();
            ShowPanel(p.pn, p.po);
        }
    }

    public void CloseAllPanel()
    {
        Dictionary<PanelName, PanelBase>.ValueCollection vs = panels.Values;
        foreach (PanelBase item in vs)
        {
            StartShowPanel(item, item.PanelShowStyle, false);
        }

        panelsDethList.Clear();
    }

    private void StartShowPanel(PanelBase go, PanelShowStyle showStyle, bool isOpen)
    {
        switch (showStyle)
        {
            case PanelShowStyle.Nomal:
                ShowNomal(go, isOpen);
                break;
            case PanelShowStyle.CenterScaleBigNomal:
                CenterScaleBigNomal(go, isOpen);
                break;
            case PanelShowStyle.LeftToSlide:
                LeftAndRightToSlide(go, false, isOpen);
                break;
            case PanelShowStyle.RightToSlide:
                LeftAndRightToSlide(go, true, isOpen);
                break;
            case PanelShowStyle.UpToSlide:
                TopAndDownToSlide(go, true, isOpen);
                break;
            case PanelShowStyle.DownToSlide:
                TopAndDownToSlide(go, false, isOpen);
                break;
            case PanelShowStyle.SomeplaceToSlide:
                SomeplaceToSlide(go, isOpen);
                break;
        }
    }

    private void ShowNomal(PanelBase go, bool isOpen)
    {
        if (isOpen)
        {
            current.gameObject.SetActive(true);
            current.OnShowed();
        }
        else
        {
            DestroyPanel(go.type);
        }
    }

    private void CenterScaleBigNomal(PanelBase go, bool isOpen)
    {
       
       
        go.gameObject.SetActive(true);
    }

    private void LeftAndRightToSlide(PanelBase go, bool isRight, bool isOpen)
    {
        go.gameObject.SetActive(true);
    }

    private void TopAndDownToSlide(PanelBase go, bool isTop, bool isOpen)
    {
        go.gameObject.SetActive(true);
    }

    private void SomeplaceToSlide(PanelBase go, bool isOpen)
    {
       
        go.gameObject.SetActive(true);
    }


    private void MaskStyle(PanelBase go)
    {
        Transform mask = ResourceMgr.GetInstance.CreateTransform("PanelMask", true);
        mask.GetComponent<RectTransform>().sizeDelta =
            GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
        float alpha = 0;
        switch (go.PanelMaskStyle)
        {
            case PanelMaskSytle.None:
                alpha = 0;
                break;
            case PanelMaskSytle.OpacityNone:
                alpha = 1f;
                break;
            case PanelMaskSytle.Opacity:
                alpha = 1f;
                break;
            case PanelMaskSytle.TranslucenceNone:
                alpha = 0.9f;
                break;
            case PanelMaskSytle.Translucence:
                alpha = 0.9f;
                break;
            case PanelMaskSytle.LucencyNone:
                alpha = 0f;
                break;
            case PanelMaskSytle.Lucency:
                alpha = 0;
                break;
        }

        mask.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
        mask.SetParent(go.gameObject.transform);
        mask.localPosition(Vector3.zero).localEulerAngles(Vector3.zero).localScale(1);
        LayerMgr.GetInstance.SetLayer(go.gameObject, LayerType.Panel);
    }

    public void HidePanel(PanelName type)
    {
        if (panels.ContainsKey(type))
        {
            PanelBase pb = null;
            pb = panels[type];
            pb.OnHideFront();
            StartShowPanel(pb, pb.PanelShowStyle, false);
            panelsDethList.Remove(pb);
        }
    }

    public void ChangePanelDeth(PanelBase type)
    {
        if (panelsDethList.Contains(type))
        {
            if (current == type)
            {
                return;
            }

            panelsDethList.Remove(type);
            panelsDethList.Add(type);
        }
        else
        {
            return;
        }

        LayerMgr.GetInstance.SetPanelsLayer(panelsDethList);
    }

    public void DestroyPanel(PanelName type)
    {
        if (panels.ContainsKey(type))
        {
            PanelBase pb = panels[type];
            if (!pb.cache)
            {
                pb.isShow = false;
                pb.OnHideDone();
                GameObject.Destroy(pb.gameObject);
                panels.Remove(type);
                panelsDethList.Remove(pb);
            }
        }
    }
}

public class PannelModel
{
    public PanelName pn;
    public object[] po;
}