using UnityEngine;
public class Singleton<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}

public class SingletonGetMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    public virtual void Awake ()
    {
        if (instance == null)
        {
            instance = GetComponent<T>();
        }
    }

    public static T Instance
    {
        get
        {
            return instance;
        }
    }
}

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{

    private static string MonoSingletionName = "MonoSingletionRoot";
    private static GameObject MonoSingletionRoot;
    private static T instance;

    public static T Instance
    {
        get
        {
            if (MonoSingletionRoot == null)
            {
                MonoSingletionRoot = GameObject. Find(MonoSingletionName);
                if (MonoSingletionRoot == null)
                {
                    MonoSingletionRoot = new GameObject();
                    MonoSingletionRoot. name = MonoSingletionName;
                    DontDestroyOnLoad(MonoSingletionRoot);
                }
            }
            if (instance == null)
            {
                instance = MonoSingletionRoot. GetComponent<T>();
                if (instance == null)
                {
                    instance = MonoSingletionRoot. AddComponent<T>();
                }
            }
            return instance;
        }
    }

}

