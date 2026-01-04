using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerMgr : MonoBehaviour
{
    private static LayerMgr mInstance;

    public static LayerMgr GetInstance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GameObject("_LayerMgr").AddComponent<LayerMgr>();
            }
            return mInstance;
        }
    }

    private LayerMgr()
    {
        mLayerDic = new Dictionary<LayerType, GameObject>();
    }

    public Dictionary<LayerType, GameObject> mLayerDic;
    private GameObject mParent;

    public void LayerInit()
    {
        mParent = GameObject.Find("Canvas");
        Transform mParentT = mParent.transform;
        int nums = Enum.GetNames(typeof(LayerType)).Length;
        for (int i = 0; i < nums; i++)
        {
            object obj = Enum.GetValues(typeof(LayerType)).GetValue(i);
            mLayerDic.Add((LayerType)obj, CreateLayerGameObject(obj.ToString(), (LayerType)obj));
        }
    }

    private GameObject CreateLayerGameObject(string name, LayerType type)
    {
        GameObject layer = new GameObject(name);
        layer.transform.parent = mParent.transform;
        Canvas canvas = layer.GetOrAddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = (int)type;
        layer.GetOrAddComponent<GraphicRaycaster>();
        layer.transform.localPosition(Vector3.zero).localEulerAngles(Vector3.zero).localScale(1);

        layer.AddComponent<RectTransform>();
        RectTransform rectTransform = layer.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);

        rectTransform.offsetMin = new Vector2(0f, 0f);
        rectTransform.offsetMax = new Vector2(0f, 0f);

        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        Debug.Log(rectTransform);
        return layer;
    }

    public void SetLayer(GameObject current, LayerType type)
    {
        if (mLayerDic.Count < Enum.GetNames(typeof(LayerType)).Length)
        {
            LayerInit();
        }
        current.transform.SetParent(mLayerDic[type].transform);
        Canvas[] panelArr = current.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas panel in panelArr)
        {
            panel.sortingOrder += (int)type;
            Renderer renderer = panel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = panel.sortingOrder;
            }
        }
    }

    public void SetPanelsLayer(List<PanelBase> pbList)
    {
        for (int i = 0; i < pbList.Count; i++)
        {
            pbList[i].transform.SetAsLastSibling();
            pbList[i].skinTrs.SetAsLastSibling();
        }
    }
}
public enum LayerType
{
    Scene = 50,
    Panel = 200,
    Tips = 400,
    Notice = 1000,
}
