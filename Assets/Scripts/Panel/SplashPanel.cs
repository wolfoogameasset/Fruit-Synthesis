using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashPanel : SceneBase
{

    protected override void OnInitSkinFront()
    {
        base.OnInitSkinFront();
        SetMainSkinPath("Panel/SplashPanel");
    }

    protected override void OnInitFront()
    {
        base.OnInitFront();
        _type = SceneType.SplashPanel;
    }

    public override void OnInit(params object[] sceneArgs)
    {
        base.OnInit(sceneArgs);
        InitData();
    }

    public override void OnHiding()
    {
        base.OnHiding();
    }


    private Slider m_LoadSliderl;
    private Transform m_Play;
    private Transform m_LoadShow;
    private float m_Value;
    private Transform mTip;

    private void InitData()
    {
        FindObj();
        RetryNetwork();

        Config.Instance.isVIbrate = PlayerPrefs.GetInt("Shock", 1) == 1;
    }

    private void FindObj()
    {
        m_LoadSliderl = skinTrs.SeachTrs<Slider>("LoadSlider");
        m_Play = skinTrs.SeachTrs<Transform>("Btn_Play");
        m_LoadShow = skinTrs.SeachTrs<Transform>("LoadShow");
        mTip = skinTrs.SeachTrs<Transform>("load_tip");
    }


    private void RetryNetwork()
    {
        StartCoroutine(ChangeValue());
    }

    private IEnumerator ChangeValue()
    {
        if (m_Value < m_LoadSliderl.maxValue)
        {
#if UNITY_EDITOR
            m_Value += 1f;
#else
            m_Value += 1f;
#endif
            m_LoadSliderl.value = m_Value;
            yield return new WaitForSeconds(0.01f);

            StartCoroutine(ChangeValue());
        }
        else
        {
            if (Config.Instance.m_First)
            {
                m_LoadSliderl.gameObject.SetActive(false);

                StartGame();
            }
            else
            {
                StartGame();
            }

            StopCoroutine(ChangeValue());
        }
    }

    private void StartGame()
    {
            SceneMgr.GetInstance.SwitchingScene(SceneType.MainPanel);
    }

    protected override void OnClick(Transform target)
    {
        switch (target.name)
        {
            case "Btn_Play":
                StartGame();
                break;
        }
    }
}