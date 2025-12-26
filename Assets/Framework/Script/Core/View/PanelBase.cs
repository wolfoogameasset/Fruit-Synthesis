using UnityEngine;

public class PanelBase : ViewBase
{
    protected bool _cache = false;

    public bool cache
    {
        get { return _cache; }
    }

    protected bool _isShow = false;

    public bool isShow
    {
        get { return _isShow; }
        set { _isShow = value; }
    }


    protected PanelName _type;

    public PanelName type
    {
        get { return _type; }
    }

    protected bool _isClickMaskColse = true;

    public bool isClickMaskColse
    {
        get { return _isClickMaskColse; }
        set { _isClickMaskColse = value; }
    }

    private PanelMgr.PanelShowStyle[] PanelStyles;
    private System.Random random;

    public PanelMgr.PanelShowStyle GetShowStyle()
    {
        PanelStyles = System.Enum.GetValues(typeof(PanelMgr.PanelShowStyle)) as PanelMgr.PanelShowStyle[];
        random = new System.Random();
        return PanelStyles[random.Next(0, PanelStyles.Length)];
    }


    protected PanelMgr.PanelShowStyle _showStyle = PanelMgr.PanelShowStyle.CenterScaleBigNomal;

    public PanelMgr.PanelShowStyle PanelShowStyle
    {
        get { return _showStyle; }
    }

    protected PanelMgr.PanelMaskSytle _maskStyle = PanelMgr.PanelMaskSytle.None;

    public PanelMgr.PanelMaskSytle PanelMaskStyle
    {
        get { return _maskStyle; }
    }

    protected float _openDuration = 0.2f;

    public float OpenDuration
    {
        get { return _openDuration; }
    }


    protected object[] _panelArgs;

    public object[] panelArgs
    {
        get { return _panelArgs; }
    }

    public virtual void OnInit(params object[] panelArgs)
    {
        _panelArgs = panelArgs;
        Init();
    }

    public virtual void OnShowing()
    {
        Config.Instance.isWipes(false);
    }

    public virtual void OnResetArgs(params object[] panelArgs)
    {
        _panelArgs = panelArgs;
    }

    public virtual void OnShowed()
    {
    }

    protected virtual void Close()
    {
        PanelMgr.GetInstance.HidePanel(type);
        PanelMgr.GetInstance.TestingPannel();
        MainPanel.isXIALUO = true;
    }

    protected virtual void Close(PanelName panel)
    {
        PanelMgr.GetInstance.HidePanel(panel);
    }

    protected virtual void CloseImmediate()
    {
        PanelMgr.GetInstance.DestroyPanel(type);
    }

    public virtual void OnHideFront()
    {
        _cache = false;
    }

    public virtual void OnHideDone()
    {
        Config.Instance.SetFreeZeAll(false);
        Config.Instance.isWipes(true);
    }
}

public enum PanelName
{
    None = 0,
    WithdrawPanel,
    RestartPanel,
    RedBagPanel,
}