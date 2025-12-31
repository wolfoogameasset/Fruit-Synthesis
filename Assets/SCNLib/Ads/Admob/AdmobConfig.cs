using SCN.Common;
using UnityEngine;

namespace SCN.Ads
{
	[CreateAssetMenu(fileName = AssetNameCommon, menuName = "SCN/Scriptable Objects/Admob Config")]
	public class AdmobConfig : ScriptableObject
	{
		public const string AssetNameCommon = "Admob config android-ios";
		public const string AssetNameAndroid = "Admob config android";
		public const string AssetNameIOS = "Admob config ios";

		[SerializeField] bool enableLog = false;

		[Tooltip("if enableTest, you no need to fill any id.")]
		[SerializeField] bool useTestID = false;

		[Header("BLOCK REQUEST")]
		[SerializeField] bool isBlockAds = false;

		[Header("Retry to request?")]
		[Tooltip("By default, when the ad request failed, it will retry to request with the increasing delay time" +
			". \nIf disable this, it will not retry any one until the method `Show` be called.")]
		[SerializeField] bool requestOnLoadFailed = true;

		[Header("App id")]
		[SerializeField] string appID;

		[Header("Banner")]
		[SerializeField] bool useBannerAd = false;
		[SerializeField] string bannerID;

		[SerializeField] BannerSizeOp bannerSize = BannerSizeOp.SmartBanner;
		  
		[Tooltip("Set to false if you want to call method 'ShowBanner' normaly")]
		[SerializeField] bool autoShowBanner = false;

		[Tooltip("Set to false if you want to show banner on top")]
		[SerializeField] bool showBannerOnBottom = false;

		[Header("Interstitial")]
		[SerializeField] bool useInterstitialAd = false;
		[SerializeField] string interID;
		[SerializeField] float inter_interval = 0;

		[Header("RewardVideo")]
		[SerializeField] bool useRewardVideoAd = false;
		[SerializeField] string rewardVideoID;
		[SerializeField] float rv_interval = 0;

		[Header("AppOpenAds")]
		[SerializeField] bool useAOA = false;
		[SerializeField] string aoaID;

        [Header("NativeAds")]
        [SerializeField] bool useNative = false;
        [SerializeField] string nativeID;

        public bool EnableLog => enableLog;
		public bool UseTestID => useTestID;
        public bool IsBlockAds { get => isBlockAds; set => isBlockAds = value; }
        public bool RequestOnLoadFailed => requestOnLoadFailed;

		public bool UseBannerAd => useBannerAd;
		public BannerSizeOp BannerSize => bannerSize;
		public bool ShowBannerOnBottom => showBannerOnBottom;
		public bool AutoShowBanner => autoShowBanner;

		public bool UseInterstitialAd => useInterstitialAd;
		public float Inter_Interval => inter_interval;

		public bool UseRewardVideoAd => useRewardVideoAd;
		public float Rv_interval => rv_interval;

		public bool UseAOA => useAOA;

		public string AppId
		{
			get
			{
				if (UseTestID)
				{
					if (Master.IsAndroid)
					{
						return "ca-app-pub-3940256099942544~3347511713";
					}
					else
					{
						return "ca-app-pub-3940256099942544~1458002511";
					}
				}
				else
				{
					return appID;
				}
			}
		}

		public string BannerID
		{
			get
			{
				if (!useBannerAd)
				{
					return string.Empty;
				}

				if (useTestID)
				{
					if (Master.IsAndroid)
					{
						return "ca-app-pub-3940256099942544/6300978111";
					}
					else
					{
						return "ca-app-pub-3940256099942544/2934735716";
					}
				}
				else
				{
					return bannerID;
				}
			}
		}

		public string InterID
		{
			get
			{
				if (!useInterstitialAd)
				{
					return string.Empty;
				}

				if (useTestID)
				{
					if (Master.IsAndroid)
					{
						return "ca-app-pub-3940256099942544/1033173712";
					}
					else
					{
						return "ca-app-pub-3940256099942544/4411468910";
					}
				}
				else
				{
					return interID;
				}
			}
		}

		public string RewardVideoID
		{
			get
			{
				if (!useRewardVideoAd)
				{
					return string.Empty;
				}

				if (useTestID)
				{
					if (Master.IsAndroid)
					{
						return "ca-app-pub-3940256099942544/5224354917";
					}
					else
					{
						return "ca-app-pub-3940256099942544/1712485313";
					}
				}
				else
				{
					return rewardVideoID;
				}
			}
		}
		
        public string AOAID
		{
			get
			{
				if (!UseAOA)
				{
					return string.Empty;
				}

				if (useTestID)
				{
					if (Master.IsAndroid)
					{
						return "ca-app-pub-3940256099942544/5354046379";
					}
					else
					{
						return "ca-app-pub-3940256099942544/6978759866";
					}
				}
				else
				{
					return aoaID;
				}
			}
		}
        public string NativeID
        {
            get
            {
                if (!useNative)
                {
                    return string.Empty;
                }

                if (useTestID)
                {
                    if (Master.IsAndroid)
                    {
                        return "ca-app-pub-3940256099942544/5354046379";
                    }
                    else
                    {
                        return "ca-app-pub-3940256099942544/6978759866";
                    }
                }
                else
                {
                    return nativeID;
                }
            }
        }

    }
}