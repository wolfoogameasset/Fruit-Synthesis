using DG.Tweening;
using SCN.FruitSynthesis;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSphere : MonoBehaviour
{
    public float sphereNum;
    [SerializeField] Sprite[] SphereSprite;
    [SerializeField] Button sphereButton;
    [SerializeField] GameObject guideLine;
    [SerializeField] ParticleSystem[] mergeParticle;
    [SerializeField] SkeletonGraphic[] charAnim;
    [SerializeField, SpineAnimation] string idle;
    [SerializeField, SpineAnimation] string shock;
    [SerializeField, SpineAnimation] string shock_idle;
    [SerializeField, SpineAnimation] string smug;
    [SerializeField, SpineAnimation] string happy;

    public float SphereNum
    {
        get { return sphereNum; }

        set
        {
            sphereNum = value;
            SetThisValue(value);
        }
    }

    internal enum Type
    {
        nomal,
        redPacket,
        gravity,
        universal,
        Puzzle
    }

    internal Type sphereType = Type.nomal;

    internal Type SphereType
    {
        get { return sphereType; }
        set
        {
            sphereType = value;
            switch (value)
            {
                case Type.nomal:
                    break;
                case Type.redPacket:
                    break;
                case Type.gravity:
                    SetThisValue();
                    m_Rigidbody.mass = 100000;
                    break;
                case Type.universal:
                    SetThisValue();
                    break;
                case Type.Puzzle:
                    SetThisValue();
                    break;
            }
        }
    }

    public int id;
    internal RectTransform m_Rect;

    internal Rigidbody2D m_Rigidbody;

    private float X = 0.3f;

    private float Y = 0.2f;

    internal CircleCollider2D m_Circle;
    private int m_Sizes = 120;
    private bool isCheckAddNum;

    private Image m_Image;

    private int Blood = 1;

    private bool _subscribed;
    bool isReviving = false;

    public static Action OnTouchDeathline;
    public static Action OnSelectedByHammer;
    public static Action<LevelSphere, float, float, LevelSphere> OnMerged;
    public static Action<LevelSphere> OnSphereEnabled;
    public static Action<LevelSphere> OnSphereDisabled;
    public static Action<float, bool> OnSphereDestroyBySkill;

    private void OnEnable()
    {
        m_Rect = transform.GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Circle = GetComponent<CircleCollider2D>();
        m_Rect.anchoredPosition(new Vector2(0, -100)).localEulerAngles(Vector3.zero);
        m_Circle.enabled = true;

        sphereButton = GetComponent<Button>();
        if (!_subscribed)
        {
            sphereButton.onClick.AddListener(() =>
            {
                //RemoveObj();
                UseHammer();
                _subscribed = true;
            });
        }
        sphereButton.interactable = false;
        OnSphereEnabled?.Invoke(this);
        guideLine.gameObject.SetActive(true);

        MainPanelController.OnClickHammerItemButton += ReadyToChoose;
        OnSelectedByHammer += SetBackToNormal;
        MainPanel.OnDropSphere += SetToDropState;
        for (int i = 0; i < charAnim.Length; i++)
        {
            charAnim[i].AnimationState.Complete += OnCompleteAnim;
        }
        RevivePanelController.OnChooseRevive += SetRevivingState;
    }

    private void OnDisable()
    {
        OnSphereDisabled?.Invoke(this);
        MainPanelController.OnClickHammerItemButton -= ReadyToChoose;
        OnSelectedByHammer -= SetBackToNormal;
        MainPanel.OnDropSphere -= SetToDropState;
        for (int i = 0; i < charAnim.Length; i++)
        {
            charAnim[i].AnimationState.Complete -= OnCompleteAnim;
        }
        RevivePanelController.OnChooseRevive -= SetRevivingState;
    }


    private float[] whs = { 78, 116, 162, 178, 226, 272, 296, 378, 456, 546, 602 };
    private float[] spineScale = { 0.25f, 0.36f, 0.49f, 0.55f, 0.68f, 0.8f, 0.9f, 1.15f, 1.36f, 1.65f, 1.8f };
    int spriteID = 0;
    private Vector2 numToV2
    {
        get
        {
            int index = log2(Convert.ToInt32(SphereNum)) - 1;
            var sp = SphereSprite[index];
            spriteID = index;
            if (m_Circle != null)
            {
                SetCir(whs[index] / 2 - 1);
            }

            PlayerPrefs.SetInt("", PlayerPrefs.GetInt("") + 1);
            if (m_Rigidbody != null) m_Rigidbody.mass = index * 2 + 5;

            m_Image.sprite = sp;
            m_Image.DOFade(0, 0);
            for (int i = 0; i < charAnim.Length; i++)
            {
                if (i == index)
                {
                    charAnim[i].transform.localScale = new Vector3(spineScale[index], spineScale[index], spineScale[index]);
                    charAnim[i].gameObject.SetActive(true);
                }
                else
                {
                    charAnim[i].gameObject.SetActive(false);
                }
            }

            return new Vector2(whs[index], whs[index]);
        }
    }

    int log2(int n)
    {
        int count = 0;
        if (n == 1)
            return 0;

        return 1 + log2(n >> 1);
    }

    private void SetThisValue(float temp)
    {
        switch (sphereType)
        {
            case Type.nomal:
                m_Rect.sizeDelta = numToV2;
                break;
            case Type.redPacket:
                break;
        }
    }

    private void SetThisValue()
    {
        switch (sphereType)
        {
            case Type.gravity:
                m_Image.sprite = Resources.Load<Sprite>("gravity");
                m_Image.color = Color.white;
                m_Rect.sizeDelta = Vector2.one * 160;
                SetCir(160 / 2);
                break;
            case Type.universal:
                m_Image.sprite = Resources.Load<Sprite>("universal");
                m_Image.color = Color.white;
                m_Rect.sizeDelta = Vector2.one * 148;
                SetCir(148 / 2);
                break;
            case Type.Puzzle:
                m_Image.sprite = Resources.Load<Sprite>("sui");
                m_Image.color = Color.white;
                m_Rect.sizeDelta = Vector2.one * 148;
                SetCir(148 / 2);
                break;
        }
    }

    private void SetCir(float radius)
    {
        m_Circle.radius = radius;
        m_Circle.offset = Vector2.zero;
    }

    internal void RefreshSphere(LevelSphere _temp)
    {
        if (SphereNum != _temp.SphereNum)
        {
            ObjectPool.Instance.Cando = true;
            Config.Instance.isMerge = true;
            return;
        }
        float old = SphereNum;
        SetThisValue(SphereNum *= 2);
        float now = SphereNum;

        transform.name = transform.name.Split('_')[0] + "_" + SphereNum;

        ObjectPool.Instance.Unspawn(_temp.gameObject);

        ObjectPool.Instance.Cando = false;

        Play_MergeParticle((int)SphereNum);
        charAnim[spriteID].AnimationState.SetAnimation(0, happy, false);
        AudioManager.Instance.Play(AudioName.SFX_Merge, false);

        if (gameObject != null && gameObject.activeSelf)
            StartCoroutine(Merge((int)SphereNum));
        else
        {
            ObjectPool.Instance.Cando = true;
        }
        OnMerged?.Invoke(this, old, now, _temp);
    }

    private void Play_MergeParticle(int num)
    {
    }

    IEnumerator Merge(int fen)
    {
        DataManager.compositeBall();
        m_Rigidbody.linearVelocity = Vector2.zero;
        yield return new WaitForSecondsRealtime(0.3f);
        ObjectPool.Instance.Cando = true;
        Config.Instance.isMerge = true;
    }

    private IEnumerator UnspawnGravity()
    {
        yield return new WaitForSeconds(0.5f);
        ObjectPool.Instance.Unspawn(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("m_Sphere"))
        {
            LevelSphere sphere = collision.transform.GetComponent<LevelSphere>();
            switch (SphereType)
            {
                case Type.nomal:
                    if (sphere != null && sphere.SphereNum == SphereNum && (int)SphereNum < 2048)
                    {
                        if (Config.Instance.mList == null)
                        {
                            return;
                        }
                        if (Config.Instance.mList.Contains(id + "_" + sphere.id) ||
                            Config.Instance.mList.Contains(sphere.id + "_" + id))
                        {
                            return;
                        }
                        if (id > sphere.id && Config.Instance.isMerge)
                        {
                            Config.Instance.isMerge = false;
                            Config.Instance.mList.Add(id + "_" + sphere.id);
                            RefreshSphere(sphere);

                            if (Config.Instance.mList.Contains(id + "_" + sphere.id))
                            {
                                Config.Instance.mList.Remove(id + "_" + sphere.id);
                            }

                            if (Config.Instance.mList.Contains(sphere.id + "_" + id))
                            {
                                Config.Instance.mList.Remove(sphere.id + "_" + id);
                            }
                        }
                    }

                    break;
                case Type.redPacket:
                    break;
                case Type.gravity:
                    break;
                case Type.universal:
                    break;
                case Type.Puzzle:
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (charAnim[spriteID].AnimationState.GetCurrent(0).Animation.Name != idle)
        {
            charAnim[spriteID].AnimationState.SetAnimation(0, idle, true);
        }
        if (collision.transform.CompareTag("m_Sphere"))
        {
            LevelSphere sphere = collision.transform.GetComponent<LevelSphere>();
            switch (SphereType)
            {
                case Type.nomal:

                    if (sphere.SphereNum == SphereNum && (int)SphereNum < 2048)
                    {
                        if (id >= sphere.id && Config.Instance.isMerge)
                        {
                            RefreshSphere(sphere);

                            if (Config.Instance.mList.Contains(id + "_" + sphere.id))
                            {
                                Config.Instance.mList.Remove(id + "_" + sphere.id);
                            }

                            if (Config.Instance.mList.Contains(sphere.id + "_" + id))
                            {
                                Config.Instance.mList.Remove(sphere.id + "_" + id);
                            }
                        }
                    }

                    break;
                case Type.redPacket:
                    break;
                case Type.gravity:
                    break;
                case Type.universal:
                    if (sphere.sphereType == Type.universal)
                    {
                        Destroy(collision.gameObject);
                        Destroy(this);
                    }

                    break;
                case Type.Puzzle:
                    if (sphere.sphereType == Type.Puzzle)
                    {
                        Destroy(collision.gameObject);
                        Destroy(this);
                    }

                    break;
            }
        }

        else if (collision.transform.tag == "Border")
        {
            if (sphereType == Type.gravity)
            {
                StartCoroutine(UnspawnGravity());
            }
        }
        else if (collision.transform.tag == "Death" && !isReviving)
        {
            //Blood--;
            //if (Blood <= 0)
            //{
            //    PanelMgr.GetInstance.ShowPanel(PanelName.RestartPanel, false);
            //    Blood = 1;
            //}
            OnTouchDeathline?.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("pierced");
        if (collision.CompareTag("Dart"))
        {
            DestroyFruitAndCallAddScore(false);
        }
    }


    private void RemoveObj()
    {
        if (MainPanel.isRemoveClassSphere && GetComponent<Collider2D>().isActiveAndEnabled)
        {
            Destroy(gameObject);
            MainPanel.isRemoveClassSphere = false;
        }
    }

    void SetToDropState(Transform droppedSphere)
    {
        if (droppedSphere == transform)
        {
            guideLine.gameObject.SetActive(false);
            charAnim[spriteID].AnimationState.SetAnimation(0, shock, false);
            AudioManager.Instance.Play(AudioName.SFX_Drop, false);
        }
    }
    void ReadyToChoose()
    {
        if (!m_Circle.enabled) return;
        sphereButton.interactable = true;
    }
    void SetBackToNormal()
    {
        sphereButton.interactable = false;
    }
    void UseHammer()
    {
        DestroyFruitAndCallAddScore(true);
        OnSelectedByHammer?.Invoke();
    }
    void DestroyFruitAndCallAddScore(bool isUsingHammer)
    {
        if (MainPanelController.Instance == null) return;
        sphereButton.interactable = false;
        Play_MergeParticle(spriteID);
        OnSphereDestroyBySkill?.Invoke(sphereNum, isUsingHammer);
        ObjectPool.Instance.Unspawn(gameObject);
        Vibration.Vibrate();
    }
    void OnCompleteAnim(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == shock)
        {
            charAnim[spriteID].AnimationState.SetAnimation(0, shock_idle, true);
        }
        else if (trackEntry.Animation.Name == happy)
        {
            charAnim[spriteID].AnimationState.SetAnimation(0, idle, true);
        }
    }
    void SetRevivingState()
    {
        isReviving = true;
        DOVirtual.DelayedCall(1f, () =>
        {
            isReviving = false;
        });
    }
}