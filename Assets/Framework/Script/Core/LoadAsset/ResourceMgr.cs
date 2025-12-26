using System. Collections;
using UnityEngine;

public class ResourceMgr : MonoBehaviour
{
    private static ResourceMgr mInstance;

    public static ResourceMgr GetInstance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GameObject("_ResourceMgr"). AddComponent<ResourceMgr>();
            }
            return mInstance;
        }

    }
    private ResourceMgr ()
    {
        hashtable = new Hashtable();
    }

    private Hashtable hashtable;

    public T Load<T> (string path, bool cache) where T : UnityEngine. Object
    {
        if (hashtable. Contains(path))
        {
            return hashtable [ path ] as T;
        }

        T assetObj = Resources. Load<T>(path);
        if (assetObj == null)
        {
        }
        if (cache)
        {
            hashtable. Add(path, assetObj);
        }
        return assetObj;
    }

    public GameObject CreateGameObject (string path, bool cache)
    {
        GameObject assetObj = Load<GameObject>(path, cache);
        GameObject go = Instantiate(assetObj) as GameObject;
        return go;
    }

    public Transform CreateTransform (string path, bool cache)
    {
        Transform assetObj = Load<Transform>(path, cache);
        Transform go = Instantiate(assetObj) as Transform;
        return go;
    }
}
