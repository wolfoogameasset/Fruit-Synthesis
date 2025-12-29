using System;
using System.Collections.Generic;
using Firebase.Analytics;
//using SCN.Ads;
using SCN.Common;
using UnityEngine;

namespace SCN.FirebaseLib.FA
{
    [CreateAssetMenu(fileName = AssetName, menuName = "SCN/Scriptable Objects/GA settings")]
    public class GAManager : ScriptableObject
    {
        public const string AssetName = "GA setting";

        static GAManager instance;
        public static GAManager Instance
        {
            get
            {
                if (instance == null)
                {
                    InitializeFirebaseAnalytics();
                }
                return instance;
            }
        }

        [SerializeField] bool debug = true;
        [SerializeField] bool sendTracking = true;

        // Handle initialization of the necessary firebase modules:
        static void InitializeFirebaseAnalytics()
        {
            instance = LoadSource.LoadObject<GAManager>(AssetName);
            if (instance == null)
            {
                Debug.LogError("Missing GA setting SO in Resources folder");
            }
        }

        public void Preload()
		{

		}

        public void LogEvent(string name, params Parameter[] parameters)
        {
			if (debug)
			{
                var logSt = $"<color=yellow>GA log:</color> {name}";
                Debug.Log(logSt);
            }

			if (sendTracking)
			{
                FirebaseAnalytics.LogEvent(name, parameters);
            }
        }

        public void LogEvent(string name, Parameter parameter = null)
        {
            if (debug)
            {
                var logSt = $"<color=yellow>GA log:</color> {name}";
                Debug.Log(logSt);
            }

			if (sendTracking)
			{
                if (parameter == null)
				{
                    FirebaseAnalytics.LogEvent(name);
                }
                else
				{
                    FirebaseAnalytics.LogEvent(name, parameter);
                }
            }
        }

        public void SetUserProperties(string name, string property)
        {
            if (debug)
            {
                var logSt = $"<color=blue>GA properties:</color> {name}_{property}";
                Debug.Log(logSt);
            }

			if (sendTracking)
			{
                FirebaseAnalytics.SetUserProperty(name, property);
            }
        }

		#region Ads
        /// <summary>
        /// Vi tri mong muon show quang cao, phu thuoc vao kich ban
        /// </summary>
        public void TrackPosInterAds(string pos)
		{
            LogEvent($"{EventName.Interstitial_Pos}_{pos}");
        }

        /// <summary>
        /// Quang cao thuc su duoc show, phu thuoc vao kha nang show ads
        /// </summary>
		public void TrackShowInterAds(string showPos, string adsNet = "")
        {
            LogEvent($"{EventName.Interstitial_Show}_{adsNet}_{showPos}");
        }

        /// <summary>
        /// Khi nguoi dung bam vao quang cao Inter cua Woa Ads
        /// </summary>
        public void TrackClickInterAds()
		{
            LogEvent($"{EventName.Interstitial_Click}");
		}
  
        /// <summary>
        /// Khi nut bam xem quang cao "reward video" duoc hien thi cho nguoi dung, phu thuoc vao kich ban
        /// </summary>
        public void TrackDisplayRewardVideoAds(string displayPos)
		{
            LogEvent($"{EventName.RewardVideo_Pos}_{displayPos}");
		}

        /// <summary>
        /// Khi quang cao reward video thuc su duoc show, phu thuoc vao user muon xem ads
        /// </summary>
        public void TrackShowRewardVideoAds(string showPos, string adsNet = "")
		{
            LogEvent($"{EventName.RewardVideo_Show}_{adsNet}_{showPos}");
		}

        /// <summary>
        /// Khi nguoi dung bam vao quang cao Inter cua Woa Ads
        /// </summary>
        public void TrackClickVideoRewardAds()
        {
            LogEvent($"{EventName.RewardVideo_Click}");
        }

        /// <summary>
        /// Vi tri mong muon show quang cao, phu thuoc vao kich ban
        /// </summary>
        public void TrackPosRewardInterAds(string pos)
        {
            LogEvent($"{EventName.RewardInter_Pos}_{pos}");
        }

        /// <summary>
        /// Quang cao thuc su duoc show, phu thuoc vao kha nang show ads
        /// </summary>
		public void TrackShowRewardInterAds(string showPos)
        {
            LogEvent($"{EventName.RewardInter_Show}_{showPos}");
        }

        public void TrackShowIcons()
        {
            LogEvent($"{EventName.Icons_Display}");
        }
        public void TrackClickIcons()
        {
            LogEvent($"{EventName.Icons_Click}");
        }
        public void TrackShowNative()
        {
            LogEvent($"{EventName.Native_Display}");
        }
        public void TrackClickNative()
        {
            LogEvent($"{EventName.Native_Click}");
        }
      
        #endregion

        #region IAP
        public void TrackShowRemoveAds()
		{
            LogEvent($"{EventName.IAP}_{EventName.Show_Remove_ads}");
        }

        public void TrackBuyRemoveAds()
        {
            LogEvent($"{EventName.IAP}_{EventName.Remove_Ads}");
        }
		#endregion

		public static class EventName
        {
            // interstitial
            public const string Interstitial_Pos = "inter_pos";
            public const string Interstitial_Show = "inter_show";
            public const string Interstitial_Click = "inter_click";

            // reward video
            public const string RewardVideo_Pos = "rv_display";
            public const string RewardVideo_Show = "rv_show";
            public const string RewardVideo_Click = "rv_click";

            // reward interstitial
            public const string RewardInter_Pos = "ri_pos";
            public const string RewardInter_Show = "ri_show";

            // Icon
            public const string Icons_Display = "icons_display";
            public const string Icons_Click = "icons_click";
            // Native
            public const string Native_Display = "native_display";
            public const string Native_Click = "native_click";

            public const string IAP = "iap";
            public const string Remove_Ads = "remove_ads";
            public const string Show_Remove_ads = "show_remove_ads";
        }
    }
}