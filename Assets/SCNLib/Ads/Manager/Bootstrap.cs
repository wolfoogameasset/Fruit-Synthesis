using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Ads
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            AdsManager.Instance.Preload(transform);
        }
    }
}
