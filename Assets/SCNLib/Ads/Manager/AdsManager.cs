using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SCN.Ads;
using SCN.FirebaseLib.FA;

namespace SCN.Ads
{
    public class AdsManager : MonoBehaviour
    {
        static AdsManager _instance;
        public static AdsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (new GameObject("AdsManager")).AddComponent<AdsManager>();
                    _instance.Setup();
                }

                return _instance;
            }
        }

        void Setup()
        {
            DontDestroyOnLoad(gameObject);

            // Admob
            _instance.admobControl = _instance.gameObject.AddComponent<AdsAdmob>();
            _instance.admobControl.Setup();
            
            _instance.inter_interval = admobControl.Config.Inter_Interval;
            _instance.rv_interval = admobControl.Config.Rv_interval;

            admobControl.OnRewardAdLoaded += Event_OnRewardAvailable;
        }

        /// <summary>
        /// Use to listening when the reward video ad is available, usefull for turn on some ad button.
        /// </summary>
        public Action OnRewardAvailable;

        void Event_OnRewardAvailable()
        {
            OnRewardAvailable?.Invoke();
        }

		readonly bool enableLog = false;
        float inter_interval;
        float rv_interval;
        float nextTimeInter;
        AdsAdmob admobControl;
        public AdsAdmob AdmobControl => admobControl;

        #region Remove Ads
        const string RemoveAdsKey = "NO_ADS";
        /// <summary>
        /// If true, User no longer to see Banner or Interstitial ads, but still can see Reward video ads.
        /// </summary>
        public bool IsRemovedAds
        {
            get => PlayerPrefs.HasKey(RemoveAdsKey);
        }

        /// <summary>
        /// Remove all Banner & Interstital ads (Still keep reward video ads)
        /// <para>Call this when user buy Remove_Ads.</para>
        /// </summary>
        public void SetRemovedAds()
        {
            PlayerPrefs.SetInt(RemoveAdsKey, 1);
            DestroyBanner();

            GAManager.Instance.TrackBuyRemoveAds();
        }
        #endregion

        /// <summary>
        /// Call this to preload ads to show ingame. Should be call at the first application script
        /// </summary>
        public void Preload() { }

        public void Preload(Transform parent)
        {
            transform.SetParent(parent);
        }

        public bool HasInters
        {
            get
            {
                if (IsRemovedAds || Time.time <= nextTimeInter)
                {
                    return false;
                }

                return admobControl.HasInter;
            }
        }

        public bool HasRewardVideo
        {
			get
            {
                return admobControl.HasRewardVideo;
            }
        } 

        public bool HasRewardInter
        {
            get
            {
                if (IsRemovedAds)
                {
                    return false;
                }

                return admobControl.HasInter;
            }
        }

        #region Banner
        public void ShowBanner()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.ShowBanner();
        }

        public void HideBanner()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.HideBanner();
        }

        public void DestroyBanner()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.DestroyBanner();
        }

        public float GetBannerHeight()
        {
            return admobControl.BannerHeight;
        }
        #endregion

        #region Interstitial
        /// <param name="showPos">Vi tri show ads, dung de tracking</param>
        /// <param name="onShowAdFinished">Call sau khi show ads hoan thanh, call ngay lap tuc khi khong co ads</param>
        public void ShowInterstitial(Action<bool> onShowAdFinished = null)
        {
			if (!HasInters) // Khong du dieu kien show bat ki ads nao
			{
                onShowAdFinished?.Invoke(false);
                return;
			}
            if (admobControl.HasInter)
            {
                AddNextTimeInter(inter_interval);
                admobControl.ShowInterstitial(onShowAdFinished);
            }
        }
        #endregion

        #region Reward video
        /// <param name="showPos">Vi tri show ads, dung de tracking</param>
        /// <param name="onSuccess">Khi xem ads thanh cong</param>
        /// <param name="onClosed">Khi ads het hoac tat ads giua chung</param>
        public void ShowRewardVideo( Action onSuccess = null, Action onClosed = null)
        {
            if (!HasRewardVideo) // Khong du dieu kien show bat ki ads nao
            {
                onClosed?.Invoke();
                return;
            }

            if (admobControl.HasRewardVideo)
            {
                AddNextTimeInter(rv_interval);
                admobControl.ShowRewardVideo( onSuccess, onClosed);
            }
        }
        #endregion

        public void ShowAOAAd()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.ShowAppOpenAd(null);
        }

        void AddNextTimeInter(float time)
		{
            nextTimeInter = Time.time + time;
        }
        
        public static void Log(string title, string msg, bool enableLog)
        {
            if (enableLog)
            {
                Debug.Log($"<color=green>Ads log [{title}]:</color> {msg}");
            }
        }

        public static void LogError(string title, string msg)
        {
            Debug.LogError($"<color=red>Ads log [{title}]:</color> {msg}");
        }
    }
    

    [Serializable]
    public enum BannerSizeOp
    {
        SmartBanner,
        Banner_320x50,
        MediumRectangle_300x250,
        IABBanner_468x60,
        Leaderboard_728x90
    }
}
