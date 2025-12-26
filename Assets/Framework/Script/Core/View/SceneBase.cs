using UnityEngine;

public class SceneBase : ViewBase
{
    public bool cache = false;

    protected SceneType _type;

    public SceneType type
    {
        get { return _type; }
    }

    protected object[] _sceneArgs;

    public object[] sceneArgs
    {
        get { return _sceneArgs; }
    }

    protected override void OnInitSkin()
    {
        base.OnInitSkin();
        skin.GetComponent<RectTransform>().sizeDelta = M_Canvas.sizeDelta;
    }

    public virtual void OnResetArgs(params object[] sceneArgs)
    {
        _sceneArgs = sceneArgs;
    }

    public virtual void OnInit(params object[] sceneArgs)
    {
        _sceneArgs = sceneArgs;
        Init();
    }

    public virtual void OnShowing()
    {
    }

    public virtual void OnShowed()
    {
    }

    public virtual void OnHiding()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnHided()
    {
    }
}

public enum SceneType
{
    None,
    MainPanel,
    SplashPanel,
}