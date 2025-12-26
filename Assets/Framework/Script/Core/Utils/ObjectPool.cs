using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : SingletonGetMono<ObjectPool> {
    public Dictionary<string, List<GameObject>> poolsDict = new Dictionary<string, List<GameObject>>();
    

    public T Spawn<T>(string name, Transform parent) {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/" + name));
        go.transform.SetParent(parent);
        go.name = go.name.Replace("(Clone)", "");
        return go.GetComponent<T>();
    }


    public void Unspawn(GameObject go) {
        Destroy(go);
    }

    public void DestorySpawn(GameObject go) {
        Destroy(go);
    }

    public void ClearPool(string name) {
        if (poolsDict.ContainsKey(name)) {
            foreach (GameObject go in poolsDict[name]) {
                Destroy(go);
            }

            poolsDict.Remove(name);
        }
    }

    internal bool Cando = true;
}