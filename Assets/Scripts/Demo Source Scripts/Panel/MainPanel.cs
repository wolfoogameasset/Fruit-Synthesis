using SCN.FruitSynthesis;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainPanel : SceneBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected override void OnInitSkinFront()
    {
        base.OnInitSkinFront();
        SetMainSkinPath("Panel/MainPanel");
    }

    protected override void OnInitFront()
    {
        base.OnInitFront();
        _type = SceneType.MainPanel;
    }

    public override void OnInit(params object[] sceneArgs)
    {
        base.OnInit(sceneArgs);
        InitData();
    }

    public override void OnShowed()
    {
        base.OnShowed();
        Config.Instance.CreateDefute();
        Config.Instance.CreateLevel();
    }

    public override void OnHiding()
    {
        base.OnHiding();
        Config.Instance.JudgeLevelUp -= JudgeLevelUp;
        Config.Instance.RefreshMoney -= RefreshMoney;
        Config.Instance.RetryEvent -= RetryEvent;
        Config.Instance.isWipes -= Isopen;
        Config.Instance.AddLevelFront -= AddLevelFront;
        Config.Instance.AddLevelDone -= AddLevelDone;
    }


    private const int SHOW_RED_BALL_NUMBER = 10;

    private RectTransform m_Btn_DragArea;
    private RectTransform m_Center;
    private RectTransform m_DownBorder;
    private Transform m_SphereParent;
    private Transform m_SphereResultParent;
    private TMP_Text m_LevelLeft;
    private TMP_Text m_LevelRight;
    private Slider m_LevelSlider;
    private TMP_Text m_MaxScore;
    private Text m_Score;
    private Transform Finger;
    private Transform broadcast;
    private TMP_Text broadcast_Text;
    private string tips;
    private string ID;
    private float With;
    private RectTransform m_WithDraw;
    private int composite_ball_number;
    private Transform m_FlyEND;
    private GameObject m_CoinEffect;
    private Transform m_Left;
    private Transform m_Right;
    private Transform m_AllRed;
    private GameObject m_NONetwork;
    private GameObject m_LevelMoneyParent;
    private TMP_Text m_TotalMoney;
    private GameObject BorderDeath;
    private Transform RemoveObjTips;
    private bool isNetwork;
    private float utime = 1;
    public Queue<Transform> Boom = new Queue<Transform>();
    private bool isOpenBoom;
    public static bool isShowRed = true;
    public static bool isOpenCreatMerge;
    public static bool isRemoveClassSphere;

    public static bool isXIALUO = true;

    private float leftLimit = 0;
    private float rightLimit = 0;
    bool isUsingSkill;

    public static Action<Transform> OnDropSphere;

    private void Awake()
    {
        MainPanelController.OnClickHammerItemButton += SetUsingSkillState;
        MainPanelController.OnClickDartItemButton += SetUsingSkillState;
        LevelSphere.OnSphereDestroyBySkill += SetNormalState;
    }

    private void OnDestroy()
    {
        MainPanelController.OnClickHammerItemButton -= SetUsingSkillState;
        MainPanelController.OnClickDartItemButton -= SetUsingSkillState;
        LevelSphere.OnSphereDestroyBySkill -= SetNormalState;
    }

    private void InitData()
    {
        FindObj();
        AddEvent();
        Config.Instance.SphereCreateParent = m_Btn_DragArea;
        Config.Instance.SphereParent = m_SphereParent;
        Config.Instance.SphereResultParent = m_SphereResultParent;
        DataManager.resetGiveUpNum();

        SetFunctionNum("Btn_gravity", DataManager.getGravityNum);
        SetFunctionNum("Btn_universal", DataManager.getUniversalNum);

        Config.Instance.isCanSave = true;
        Tips();
        OpenFinger(true);
        StartCoroutine(OpenNetwork());
    }


    private void FindObj()
    {
        m_WithDraw = skinTrs.SeachTrs<RectTransform>("Btn_WithDraw");
        broadcast = skinTrs.SeachTrs<Transform>("broadcast");
        broadcast_Text = skinTrs.SeachTrs<TMP_Text>("broadcast_Text");
        RemoveObjTips = skinTrs.SeachTrs<Transform>("RemoveObjTips");
        Finger = skinTrs.SeachTrs<RectTransform>("Finger");
        m_Btn_DragArea = skinTrs.SeachTrs<RectTransform>("Btn_DragArea");
        m_SphereParent = skinTrs.SeachTrs<RectTransform>("SphereParent");
        m_SphereResultParent = skinTrs.SeachTrs<RectTransform>("SphereResultParent");
        BorderDeath = skinTrs.SeachTrs<Transform>("BorderDeath").gameObject;
        m_LevelLeft = skinTrs.SeachTrs<TMP_Text>("Txt_LevelLeft");
        m_LevelRight = skinTrs.SeachTrs<TMP_Text>("Txt_LevelRight");
        m_LevelSlider = skinTrs.SeachTrs<Slider>("LevelSlider");
        m_FlyEND = skinTrs.SeachTrs<Transform>("WithDraw_Text");
        m_Left = skinTrs.SeachTrs<Transform>("Left");
        m_Right = skinTrs.SeachTrs<Transform>("Right");
        m_AllRed = skinTrs.SeachTrs<Transform>("AllRedEnvelope");
        m_Center = skinTrs.SeachTrs<RectTransform>("Center");
        m_DownBorder = skinTrs.SeachTrs<RectTransform>("DownBoder");
    }

    private void AddEvent()
    {
        Config.Instance.JudgeLevelUp += JudgeLevelUp;
        Config.Instance.RefreshMoney += RefreshMoney;
        Config.Instance.RetryEvent += RetryEvent;
        Config.Instance.isWipes += Isopen;
        Config.Instance.AddLevelFront += AddLevelFront;
        Config.Instance.AddLevelDone += AddLevelDone;
    }

    private IEnumerator OpenNetwork()
    {
        yield return new WaitForSeconds(20);
        StartCoroutine(OpenNetwork());
    }

    private void Isopen(bool obj)
    {
        m_CoinEffect.SetActive(obj);
    }

    private void OpenFinger(object _isShow)
    {
        bool isShow = (bool)_isShow;
        Finger.gameObject.SetActive(isShow);
    }

    private void GetSaveValue()
    {
        for (int i = 0; i < DataManager.getSphereInfo.Count; i++)
        {
            SphereInfo info = DataManager.getSphereInfo[i];
            LevelSphere sphere = Config.Instance.CreateSphere(m_SphereParent, info.num,
                new Vector2((float)info.pos[0], (float)info.pos[1]));
            sphere.id = -(info.id == 0 ? i : info.id);
            sphere.m_Rigidbody.constraints = RigidbodyConstraints2D.None;
        }
    }


    private void RefreshMoney(double value)
    {
        float temp = float.Parse(skinTrs.SeachTrs<TMP_Text>("WithDraw_Text").text);
        GameObject clone = (GameObject)Instantiate(Resources.Load("Prefabs/FlyNumber"), transform);
        clone.gameObject.transform.SeachTrs<TMP_Text>("money").text = value.ToString();

    }

    private void QQMergaFront(int fen)
    {
        int i = 1;
        int di = 2;
        if (fen < 2048)
        {
            while (di != fen)
            {
                di *= 2;
                i++;
            }
        }
        else
        {
            i = 100;
        }

        DataManager.addScore(i);
    }

    private void QQMergaDone()
    {
        if (isShowRed)
        {
            if (DataManager.getCurrentMoney() == 0)
            {
            }
            else
            {
                composite_ball_number++;
                if (composite_ball_number == SHOW_RED_BALL_NUMBER)
                {
                    if (Config.Instance.isLevelShow)
                    {
                        Config.Instance.showRedBag += showRedBag;
                    }
                    composite_ball_number = 0;
                }
            }

            JudgeLevelUp();
        }
    }

    private void showRedBag()
    {
        PanelMgr.GetInstance.ShowPanel(PanelName.RedBagPanel);
        Config.Instance.showRedBag -= showRedBag;
    }

    private void JudgeLevelUp()
    {
    }

    private void AddLevelFront()
    {
        float leftAngle = m_Left.localEulerAngles.z;


        float rightAngle = m_Right.localEulerAngles.z;

    }

    private void AddLevelDone()
    {
        m_Left.localEulerAngles(Vector3.zero);
        m_Right.localEulerAngles(Vector3.zero);
    }

    private void SetSphere(bool isShow)
    {
        if (m_Btn_DragArea.childCount == 0 || Config.Instance.isDown || !isXIALUO) return;

        Transform tempSphere = m_Btn_DragArea.GetChild(0);
        if (!isShow)
        {
            BorderDeath.SetActive(false);
            CloseFinger();
            Rigidbody2D rigidbody = tempSphere.GetComponent<Rigidbody2D>();
            rigidbody.constraints = RigidbodyConstraints2D.None;
            rigidbody.AddForce(new Vector2(UnityEngine.Random.Range(-1, 2) * 30, 0));
            tempSphere.SetParent(Config.Instance.SphereParent);
            tempSphere.GetComponent<LevelSphere>().m_Circle.enabled = true;
            StartCoroutine(create());
            OnDropSphere?.Invoke(tempSphere);
        }
    }

    IEnumerator create()
    {
        yield return new WaitForSeconds(1f);
        Config.Instance.CreateDefute();
        BorderDeath.SetActive(true);
    }


    IEnumerator DestroyRed(GameObject m_Read)
    {
        yield return new WaitForSeconds(10f);
        if (m_Read != null)
        {
            ObjectPool.Instance.DestorySpawn(m_Read);
        }
    }
    private float initPosX = 0;

    void SetUsingSkillState()
    {
        isUsingSkill = true;
    }

    void SetNormalState(float point)
    {
        isUsingSkill = false;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isUsingSkill) return;
        if (leftLimit == 0)
        {
            leftLimit = MainPanelController.Instance.leftWallPosX;
            rightLimit = MainPanelController.Instance.rightWallPosX;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isNetwork) return;
        if (m_Btn_DragArea.childCount == 0) return;
        if (isUsingSkill) return;

        // Convert pointer position to world position
        Vector3 world;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            m_Btn_DragArea, eventData.position, eventData.pressEventCamera, out world);

        var t = m_Btn_DragArea.GetChild(0);
        var old = t.position;
        float x = Math.Clamp(world.x, leftLimit + 1.5f, rightLimit - 1.5f);
        //float x = Math.Clamp(world.x, -5, 5);
        t.position = new Vector3(x, old.y, old.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isNetwork || !isXIALUO) return;
        if (isUsingSkill) return;
        SetSphere(false);
    }

    private void RetryEvent()
    {
        DataManager.getLevelScore = 0;
        DataManager.getScore = 0;
        DataManager.getSphereInfo = new List<SphereInfo>();
        Config.Instance.CreateLevel();
        Config.Instance.CreateDefute();
        ObjectPool.Instance.Cando = true;
        for (int i = 0; i < m_SphereParent.childCount; i++)
            ObjectPool.Instance.Unspawn(m_SphereParent.GetChild(i).gameObject);
    }
    private bool IsPointerOverUI()
    {
        // For mouse (Editor/PC)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        // For touch (mobile)
        if (Input.touchCount > 0 && EventSystem.current != null)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return false;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetSphere(false);
        }
#endif


        if (isOpenBoom && Boom.Count > 0)
        {
            isOpenBoom = false;
            BoomEffet(Boom.Dequeue());
            if (Boom.Count == 0)
            {
                PanelMgr.GetInstance.ShowPanel(PanelName.RestartPanel, false);
            }
        }
        //if (IsPointerOverUI())
        //    return;
        //if (utime < 0.3f)
        //{
        //    utime += Time.deltaTime;
        //}
        //else
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        if (!isNetwork)
        //        {
        //            if (m_Btn_DragArea.childCount == 0) return;
        //            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //            Vector3 old = m_Btn_DragArea.GetChild(0).position;

        //            m_Btn_DragArea.GetChild(0).position = new Vector3(mousePosition.x, old.y, old.z);
        //        }
        //    }

        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        if (!isNetwork)
        //        {
        //            if (m_Btn_DragArea.childCount == 0) return;
        //            BorderDeath.SetActive(false);
        //            SetSphere(false);

        //            utime = 0;
        //        }
        //    }
        //}
    }

    protected override void OnClick(Transform target)
    {
        switch (target.name)
        {
            case "Btn_WithDraw":
                PanelMgr.GetInstance.ShowPanel(PanelName.WithdrawPanel);
                break;
            case "Btn_Retry":
                RemoveClassSphere();
                break;
            case "Btn_DragArea":
                SetSphere(false);
                break;
            case "Btn_Setting":
                break;
            case "Btn_universal":
                if (DataManager.getUniversalNum > 0)
                {
                    if (Config.Instance.SphereCreateParent.GetChild(0).GetComponent<LevelSphere>().SphereType !=
                        LevelSphere.Type.nomal) return;
                    if (Config.Instance.hasUniversal)
                    {
                        return;
                    }

                    DataManager.setUniversalNum();
                    Config.Instance.CreateTypeSphere(LevelSphere.Type.universal);
                    Config.Instance.hasUniversal = true;
                }
                else
                {
#if UNITY_EDITOR
                    DataManager.setUniversalNum(true);
#elif UNITY_ANDROID
#endif
                }

                SetFunctionNum(target.name, DataManager.getUniversalNum);
                break;
            case "Btn_gravity":
                if (DataManager.getGravityNum > 0)
                {
                    if (Config.Instance.SphereCreateParent.GetChild(0).GetComponent<LevelSphere>().SphereType !=
                        LevelSphere.Type.nomal) return;
                    DataManager.setGravityNum();
                    Config.Instance.CreateTypeSphere(LevelSphere.Type.gravity);
                }
                else
                {
#if UNITY_EDITOR
                    DataManager.setGravityNum(true);
#elif UNITY_ANDROID
#endif
                }

                SetFunctionNum(target.name, DataManager.getGravityNum);
                break;
            case "Btn_with_draw":
                PanelMgr.GetInstance.ShowPanel(PanelName.WithdrawPanel);
                break;
        }
    }

    protected override void OnDown(Transform target)
    {
        switch (target.name)
        {
            case "Btn_DragArea":
                SetSphere(true);
                break;
        }
    }

    protected override void OnUp(Transform target)
    {
        switch (target.name)
        {
            case "Btn_DragArea":
                break;
        }
    }

    private void SetFunctionNum(string name, int num)
    {
        bool isShow = num > 0;
    }

    private void CloseFinger()
    {
        OpenFinger(false);
    }

    private void GenerateText()
    {
        int max = 6;
        for (int i = 0; i < max; i++)
        {
            ID += UnityEngine.Random.Range(0, 10).ToString();
        }
    }

    private void Tips()
    {
        int time = UnityEngine.Random.Range(10, 30);
        GenerateText();

    }

    private void RemoveClassSphere()
    {

        if (!PlayerPrefs.HasKey("diyiciqingchu"))
        {
            isRemoveClassSphere = true;
            PlayerPrefs.SetInt("diyiciqingchu", 1);
            RemoveObjTips.gameObject.SetActive(true);
            StartCoroutine(WaitTimeClose());
        }
    }

    IEnumerator WaitTimeClose()
    {
        yield return new WaitForSeconds(2f);
        RemoveObjTips.gameObject.SetActive(false);
    }

    private void BoomEffet(Transform fen)
    {

        if (fen.GetComponent<LevelSphere>() != null)
        {

            int num = (int)fen.GetComponent<LevelSphere>().sphereNum;
            fen.Find("zha/" + num).GetComponent<ParticleSystem>().Play();
            StartCoroutine(WaitBoom(fen.gameObject));
        }
    }
    IEnumerator WaitBoom(GameObject ga)
    {
        yield return new WaitForSeconds(0.2f);
        int i = 1;
        int di = 2;
        if (ga.GetComponent<LevelSphere>() != null)
        {
            var fen = ga.GetComponent<LevelSphere>().SphereNum;
            if (fen < 2048)
            {

                while (di != fen)
                {
                    di *= 2;
                    i++;
                }
            }
            else
            {
                i = 10;
            }
            DataManager.addScore(i);
            Destroy(ga);
            isOpenBoom = true;
        }
    }
}