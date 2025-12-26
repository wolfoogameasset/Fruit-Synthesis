using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBase : MonoBehaviour
{
#if NGUI
    private List<Collider> colliderList = new List<Collider>();
#endif

    private List<Transform> transList = new List<Transform>();

    private string mainSkinPath;

    private bool isInit;

    private GameObject _skin;

    public GameObject skin
    {
        get { return _skin; }
    }

    public Transform skinTrs
    {
        get { return _skin.transform; }
    }

    private GameObject m_Canvas;

    public RectTransform M_Canvas
    {
        get { return m_Canvas.GetComponent<RectTransform>(); }
    }

    protected virtual void OnClick(Transform target)
    {
    }

    protected virtual void OnDown(Transform target)
    {
    }

    protected virtual void OnUp(Transform target)
    {
    }

    protected virtual void OnInitSkinFront()
    {
    }

    protected virtual void OnInitSkin()
    {
        if (mainSkinPath != null)
        {
            _skin = LoadSrc(mainSkinPath);
        }
        else
        {
            _skin = new GameObject("Skin");
        }

        skin.transform.SetParent(transform);
        skin.transform.localEulerAngles(Vector3.zero).localScale(1);
    }

    protected virtual void OnInitFront()
    {
        transList.Clear();
        m_Canvas = GameObject.Find("Canvas");
    }
#if NGUI
    public virtual void Init()
    {
        if (!isInit)
        {
            OnInitFront();
            OnInitSkinFront();
            OnInitSkin();
        }
        //Profiler.BeginSample(" ：");
        Collider[] triggers = this.GetComponentsInChildren<Collider>(true);
        for (int i = 0, max = triggers.Length; i < max; i++)
        {
            Collider trigger = triggers[i];
            if (trigger.gameObject.name.StartsWith("Btn") == true) 
            {
                UIEventListener listener = UIEventListener.Get(trigger.gameObject);
                listener.onClick = Click;
            }
            colliderList.Add(trigger);
            //UIButtonScale btnScale = trigger.gameObject.GetComponent<UIButtonScale>();
            //if (btnScale != null)
            //{
            //    btnScale.hover = Vector3.one;
            //}
        }
        isInit = true;
        //UIButtonScale btnScale = trigger.gameObject.GetComponent<UIButtonScale>();
        //if (btnScale != null)
        //{
        //    btnScale.hover = Vector3.one;
        //}
        //Profiler.EndSample();
    }
#endif
    public virtual void Init()
    {
        if (!isInit)
        {
            OnInitFront();
            OnInitSkinFront();
            OnInitSkin();
        }

        Transform[] transforms = this.GetComponentsInChildren<Transform>(true);
       
        for (int i = 0, max = transforms.Length; i < max; i++)
        {
            Transform transform = transforms[i];
            if (transform.name.StartsWith("Btn") == true) 
            {
                if (transform.GetComponent<Button>())
                {
                    Button listener = transform.GetComponent<Button>();
                    listener.onClick.AddListener(() => { OnClick(listener.transform); });
                }
            }

            transList.Add(transform);
        }

        isInit = true;
    }
#if NGUI
    public void SetColliderEnabled(bool enabled)
    {
        foreach (BoxCollider bc in colliderList)
        {
            bc.enabled = enabled;
        }
    }

#endif

    protected void SetMainSkinPath(string path)
    {
        mainSkinPath = path;
    }

    protected GameObject LoadSrc(string path)
    {
        return ResourceMgr.GetInstance.CreateGameObject(path, false);
    }
}