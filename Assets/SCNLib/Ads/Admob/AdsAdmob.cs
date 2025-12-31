using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using DG.Tweening;
using SCN.Common;
using SCN.FirebaseLib.FA;

namespace SCN.Ads
{
    public class AdsAdmob : SafeCallBack
    {
        BannerView bannerView;

        InterstitialAd inter;

        RewardedAd rewardVideo;
        RewardedInterstitialAd rewardInter;

        Action onRewardSuccess;
        Action onRewardClose;

        private AppOpenAd appOpenAd;
        private Action onAOASuccess;
        private Action onAOAFailed;

        Action<bool> onInterSuccess;

        bool isRequestingBanner;

        bool isRequestingInter;

        bool isRequestingRewardVideo;
        bool isRequestingRewardInter;

        bool waitToShowBanner;

        bool isBannerShowing;
        bool isRewardInterReady;
        private IEnumerator ieWaitInternet;
        private bool IsInternetAvailable => (int)Application.internetReachability > 0;

        readonly int[] retryTimes = new int[14]
        {
            0,
            2,
            5,
            10,
            20,
            60,
            60,
            120,
            120,
            240,
            240,
            400,
            400,
            600
        };

        int retryRewardItem = 0;

        int retryInters = 0;

        int retryBanner = 0;
        int retryRewardInter = 0;

        AdmobConfig config;

        public AdmobConfig Config => config;

        public float BannerHeight => (bannerView != null) ? bannerView.GetHeightInPixels() : 0f;

        public bool HasInter
        {
            get
            {
                if (config.IsBlockAds)
                {
                    return false;
                }

                if (inter != null && inter.CanShowAd())
                {
                    return true;
                }

                RequestInterstitial();
                return false;
            }
        }

        public bool HasRewardVideo
        {
            get
            {
                if (config.IsBlockAds)
                {
                    return false;
                }

                if (rewardVideo != null && rewardVideo.CanShowAd())
                {
                    return true;
                }

                RequestRewardVideo();
                return false;
            }
        }


        public event Action OnRewardAdLoaded;

        public void Setup()
        {
            if (Master.IsAndroid)
            {
                config = Resources.Load<AdmobConfig>(AdmobConfig.AssetNameAndroid);
                if (config == null)
                {
                    Debug.LogError($"Create <color=yellow>Admob config android</color> SO");
                }
            }
            else
            {
                config = Resources.Load<AdmobConfig>(AdmobConfig.AssetNameIOS);
                if (config == null)
                {
                    Debug.LogError($"Create <color=yellow>Admob config ios</color> SO");
                }
            }

            if (config == null || config.IsBlockAds)
            {
                return;
            }

            RequestConfiguration requestConfiguration = new RequestConfiguration();
            requestConfiguration.TagForChildDirectedTreatment =
                TagForChildDirectedTreatment.Unspecified; // Dành cho trẻ em hay không (đang để mặc định)


            AdsManager.Log("", $"Start Initializing: appID = {config.AppId}" +
                               $", useBanner={config.UseBannerAd}" +
                               $", useInterstitial={config.UseInterstitialAd}" +
                               $", useRewardVideo={config.UseRewardVideoAd}", config.EnableLog);

            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize((Action<InitializationStatus>)delegate(InitializationStatus status)
            {
                //IL_0022: Unknown result type (might be due to invalid IL or missing references)
                //IL_0028: Invalid comparison between Unknown and I4
                Dictionary<string, AdapterStatus> adapterStatusMap = status.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> item in adapterStatusMap)
                {
                    if ((int)item.Value.InitializationState == 1)
                    {
                        Debug.Log((object)("[Ads.Admob] Adapter: " + item.Key + " initialized."));
                    }
                    else
                    {
                        Debug.LogError((object)("[Ads.Admob] Adapter: " + item.Key + " not ready."));
                    }
                }

                if (config.AutoShowBanner)
                {
                    RequestBanner();
                }

                RequestRewardVideo();
                RequestInterstitial();
                if (config.UseAOA)
                {
                    LoadAppOpenAd();
                }
            });

            MobileAds.SetiOSAppPauseOnBackground(true);
        }

        private void Update()
        {
            while (safeCallback.Count > 0)
            {
                Action action = null;
                lock (safeCallback)
                {
                    action = safeCallback.Dequeue();
                }

                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                }
            }
        }

        private void SafeCallback(Action callback)
        {
            if (callback != null)
            {
                safeCallback.Enqueue(callback);
            }
        }

        private void DelayCallback(float delayTime, Action callback)
        {
            if (callback != null)
            {
                if (delayTime == 0f)
                {
                    SafeCallback(callback);
                }
                else
                {
                    ((MonoBehaviour)this).StartCoroutine(IEDelayCallback(delayTime, callback));
                }
            }
        }

        private IEnumerator IEDelayCallback(float delayTime, Action callback)
        {
            yield return (object)new WaitForSecondsRealtime(delayTime);
            callback?.Invoke();
        }

        private void WaitInternet(Action callback)
        {
            if (callback != null)
            {
                ((MonoBehaviour)this).StartCoroutine(IEWaitInternet(callback));
            }
        }

        private IEnumerator IEWaitInternet(Action callback)
        {
            if (ieWaitInternet == null)
            {
                ieWaitInternet = (IEnumerator)new WaitUntil((Func<bool>)(() => IsInternetAvailable));
            }

            yield return ieWaitInternet;
            callback?.Invoke();
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest();
        }

        private void Log(string adType, string msg)
        {
            if (config.EnableLog)
            {
                Debug.Log((object)("[Ads.Admob." + adType + "] " + msg));
            }
        }

        private void LogError(string adType, string msg)
        {
            Debug.LogError((object)("[Ads.Admob." + adType + "] " + msg));
        }

        private void RequestBanner()
        {
            if (config.IsBlockAds) return;
            if (!config.UseBannerAd || isRequestingBanner)
            {
                return;
            }

            if (retryBanner >= retryTimes.Length)
            {
                retryBanner = retryTimes[retryTimes.Length - 1];
            }

            int num = retryTimes[retryBanner];
            isRequestingBanner = true;
            Log("Banner", $"Will Request after {num}s, retry={retryBanner}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestBanner();
                }
                else
                {
                    LogError("Banner", "Request: Waiting for internet...");
                    WaitInternet(DoRequestBanner);
                }
            });
        }

        private void DoRequestBanner()
        {
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0044: Expected O, but got Unknown
            Log("Banner", "Request starting...");
            GAManager.Instance.LogEvent("ad_banner_request");
            DestroyBanner();
            bannerView = new BannerView(config.BannerID, GetAdsize(), (AdPosition)(config.ShowBannerOnBottom ? 1 : 0));
            bannerView.OnBannerAdLoaded += () => OnBannerAdLoaded();
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) => OnBannerAdFailedToLoad(error);
            bannerView.OnAdImpressionRecorded += () => OnBannerAdOpened();

            bannerView.LoadAd(CreateAdRequest());
        }

        private AdSize GetAdsize()
        {
            var configBanner = config.BannerSize;
            switch (configBanner)
            {
                case BannerSizeOp.SmartBanner:
                    return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

                case BannerSizeOp.Banner_320x50:
                    return AdSize.Banner;

                case BannerSizeOp.IABBanner_468x60:
                    return AdSize.IABBanner;

                case BannerSizeOp.Leaderboard_728x90:
                    return AdSize.Leaderboard;

                default: return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            }
        }

        public void ShowBanner()
        {
            if (!isBannerShowing)
            {
                if (bannerView != null)
                {
                    waitToShowBanner = false;
                    isBannerShowing = true;
                    bannerView.Show();
                }
                else
                {
                    waitToShowBanner = true;
                    RequestBanner();
                }
            }
        }

        public void HideBanner()
        {
            if (bannerView != null)
            {
                waitToShowBanner = false;
                isBannerShowing = false;
                bannerView.Hide();
            }
        }

        public void DestroyBanner()
        {
            if (bannerView != null)
            {
                waitToShowBanner = false;
                isBannerShowing = false;
                bannerView.Destroy();
                bannerView = null;
            }
        }

        private void RequestInterstitial()
        {
            if (config.IsBlockAds) return;
            if (!config.UseInterstitialAd || isRequestingInter)
            {
                return;
            }

            if (retryInters >= retryTimes.Length)
            {
                retryInters = retryTimes[retryTimes.Length - 1];
            }

            int num = retryTimes[retryInters];
            isRequestingInter = true;
            Log("Interstitial", $"Will Request after {num}s, retry={retryInters}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestInterstitial();
                }
                else
                {
                    LogError("Interstitial", "Request: Waiting for internet...");
                    WaitInternet(DoRequestInterstitial);
                }
            });
        }


        private void DoRequestInterstitial()
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002e: Expected O, but got Unknown
            Log("Interstitial", "Request starting...");
            GAManager.Instance.LogEvent("ad_inter_request");
            //DestroyInter();
            InterstitialAd.Load(config.InterID, CreateAdRequest(), (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    OnInterFailedToLoad(loadError);
                    return;
                }
                else if (ad == null)
                {
                    Log("Interstitial", "Interstitial ad failed to load.");
                    return;
                }

                Log("Interstitial", "Interstitial ad loaded.");
                inter = ad;
                OnInterLoaded();
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Log("Interstitial", "Interstitial ad opening.");
                    OnInterstitialAdOpened();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Log("Interstitial", "Interstitial ad closed.");
                    OnInterClosed();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Log("Interstitial", "Interstitial ad recorded an impression.");
                    GAManager.Instance.LogEvent("ad_inter_impression");
                    OnInterstitialAdImpressionRecorded();
                };
                ad.OnAdClicked += () =>
                {
                    Log("Interstitial", "Interstitial ad recorded a click.");
                    GAManager.Instance.LogEvent("ad_inter_click");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Log("Interstitial", "Interstitial ad failed to show with error: " +
                                        error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                        "Interstitial ad received a paid event.",
                        adValue.CurrencyCode,
                        adValue.Value);
                    Log("Interstitial", msg);
                };
            });
        }

        public void ShowInterstitial(Action<bool> callback = null)
        {
            if (config.IsBlockAds)
            {
                callback?.Invoke(false);
                return;
            }

            if (HasInter)
            {
                onInterSuccess = callback;
                Log("Interstitial", "Show start..");
                inter.Show();
            }
            else
            {
                Log("Interstitial", "Show failed: ad not ready. Invoke callback.");
                callback?.Invoke(true);
                RequestInterstitial();
            }
        }

        public void DestroyInter()
        {
            if (inter != null)
            {
                inter.Destroy();
                inter = null;
            }
        }

        #region RewardVideo

        private void RequestRewardVideo()
        {
            if (config.IsBlockAds) return;
            if (!config.UseRewardVideoAd || isRequestingRewardVideo)
            {
                return;
            }

            if (retryRewardItem >= retryTimes.Length)
            {
                retryRewardItem = retryTimes[retryTimes.Length - 1];
            }

            int num = retryTimes[retryRewardItem];
            isRequestingRewardVideo = true;
            Log("RewardedVideo", $"Request after {num}s, retry={retryRewardItem}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestReward();
                }
                else
                {
                    LogError("RewardedVideo", "Request: Waiting for internet...");
                    WaitInternet(DoRequestReward);
                }
            });
        }

        private void DoRequestReward()
        {
            Log("RewardedVideo", "Request starting...");
            RewardedAd.Load(config.RewardVideoID, CreateAdRequest(), (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Log("RewardedVideo", "Rewarded ad failed to load with error: " +
                                         loadError.GetMessage());
                    GAManager.Instance.LogEvent("ad_rv_failed_to_load");
                    OnRewardFailedToLoad(loadError);
                    return;
                }
                else if (ad == null)
                {
                    Log("RewardedVideo", "Rewarded ad failed to load.");
                    GAManager.Instance.LogEvent("ad_rv_failed_to_load");
                    return;
                }

                Log("RewardedVideo", "Rewarded ad loaded.");
                OnRewardLoaded();
                rewardVideo = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Log("RewardedVideo", "Rewarded ad opening.");
                    OnRewardOpening();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Log("RewardedVideo", "Rewarded ad closed.");
                    OnRewardClosed();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Log("RewardedVideo", "Rewarded ad recorded an impression.");
                    GAManager.Instance.LogEvent("ad_rv_impression");
                };
                ad.OnAdClicked += () =>
                {
                    OnRewardClick();
                    Log("RewardedVideo", "Rewarded ad recorded a click.");
                    GAManager.Instance.LogEvent("ad_rv_click");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    OnRewardFailedToShow(error);
                    Log("RewardedVideo", "Rewarded ad failed to show with error: " +
                                         error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                        "Rewarded ad received a paid event.",
                        adValue.CurrencyCode,
                        adValue.Value);

                    Log("RewardedVideo", (msg));
                };
            });
        }

        public void ShowRewardVideo(Action onSuccess, Action onClosed = null)
        {
            if (config.IsBlockAds)
            {
                onSuccess?.Invoke();
                Debug.Log("Here 1");
                return;
            }

            if (HasRewardVideo)
            {
                onRewardSuccess = onSuccess;
                onRewardClose = onClosed;
                Log("RewardedVideo", "Show start...");
                rewardVideo.Show((Reward reward) =>
                {
                    Log("RewardedVideo", "Rewarded ad granted a reward: " + reward.Amount);
                    OnRewardEarnedReward();
                });
            }
            else
            {
                Log("RewardedVideo", "Show failed: ad not ready. Invoke onClosed callback.");
                onClosed?.Invoke();
                RequestRewardVideo();
            }
        }

        private void OnRewardLoaded()
        {
            Log("RewardedVideo", "OnAdLoaded.");
            retryRewardItem = 0;
            isRequestingRewardVideo = false;
            SafeCallback(this.OnRewardAdLoaded);
        }

        private void OnRewardFailedToLoad(LoadAdError loadError)
        {
            Debug.LogError("RewardedVideo" + "OnAdFailedToLoad: " + loadError.GetMessage());
            isRequestingRewardVideo = false;
            if (config.RequestOnLoadFailed)
            {
                retryRewardItem++;
                RequestRewardVideo();
            }
        }

        private void OnRewardOpening()
        {
            Log("RewardedVideo", "OnAdOpening...");
            Debug.Log("Here OnRewardOpening");
        }

        private void OnRewardFailedToShow(AdError loadError)
        {
            LogError("RewardedVideo", "OnAdFailedToShow: " + loadError.GetMessage() + ".");
            SafeCallback(onRewardClose);
            RequestRewardVideo();
            Debug.Log("Here OnRewardFailedToShow");
        }

        private void OnRewardClosed()
        {
            Log("RewardedVideo", "OnAdClosed.");
            SafeCallback(onRewardClose);
            RequestRewardVideo();
            Debug.Log("Here OnRewardClosed");
        }

        private void OnRewardEarnedReward()
        {
            Log("RewardedVideo", "OnEarnedReward successfully.");
            SafeCallback(onRewardSuccess);
            Debug.Log("Here OnRewardEarnedReward");
        }

        private void OnRewardClick()
        {
        }

        #endregion

        #region InterBanner

        private void OnBannerAdLoaded()
        {
            Log("Banner", "OnAdLoaded.");
            retryBanner = 0;
            isRequestingBanner = false;
            if (config.AutoShowBanner || waitToShowBanner)
            {
                ShowBanner();
            }
        }

        private void OnBannerAdFailedToLoad(LoadAdError error)
        {
            LogError("Banner", "OnAdFailedToLoad: " + error.GetMessage() + ".");
            GAManager.Instance.LogEvent("ad_banner_failed_to_load");
            isRequestingBanner = false;
            retryBanner++;
            RequestBanner();
        }

        private void OnBannerAdOpened()
        {
            GAManager.Instance.LogEvent("ad_banner_impression");
        }

        private void OnBannerAdClosed(object sender, EventArgs args)
        {
            Log("Banner", "OnAdClosed.");
            isBannerShowing = false;
            if (config.AutoShowBanner)
            {
                RequestBanner();
            }
        }

        private void OnBannerAdLeftApplication(object sender, EventArgs args)
        {
            Log("Banner", "OnAdLeftApplication.");
        }

        private void OnInterLoaded()
        {
            Log("Interstitial", "OnAdLoaded.");
            retryInters = 0;
            isRequestingInter = false;
        }


        private void OnInterFailedToLoad(LoadAdError loadError)
        {
            LogError("Interstitial", "OnAdFailedToLoad: " + loadError.GetMessage() + ".");
            GAManager.Instance.LogEvent("ad_inter_failed_to_load");
            isRequestingInter = false;
            if (config.RequestOnLoadFailed)
            {
                retryInters++;
                RequestInterstitial();
            }
        }

        private void OnInterstitialAdOpened()
        {
            Log("Interstitial", "OnAdOpened...");
        }

        private void OnInterClosed()
        {
            Time.timeScale = 1;
            Log("Interstitial", "OnAdClosed.");
            SafeCallback(() => onInterSuccess(true));
            RequestInterstitial();
        }

        private void OnInterstitialAdLeftApplication(object sender, EventArgs args)
        {
            Log("Interstitial", "OnAdLeftApplication.");
        }

        private void OnInterstitialAdImpressionRecorded()
        {
            Time.timeScale = 1;
            Log("Interstitial", "OnAdClosed.");
            // SafeCallback(onInterSuccess);
            RequestInterstitial();
        }

        #endregion

        #region AOA

        public void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            Debug.Log("Loading the app open ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            AppOpenAd.Load(config.AOAID, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("app open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("App open ad loaded with response : "
                              + ad.GetResponseInfo());

                    appOpenAd = ad;
                    RegisterEventHandlers(ad);
                });
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
                GAManager.Instance.LogEvent("ad_AOA_impression");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
                GAManager.Instance.LogEvent("ad_AOA_click");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () => { Debug.Log("App open ad full screen content opened."); };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");
                SafeCallback(onAOASuccess);
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);
            };
        }

        public bool IsAOAAvailable
        {
            get
            {
                return appOpenAd != null && !config.IsBlockAds
                                         && appOpenAd.CanShowAd();
            }
        }

        public void ShowAppOpenAd(Action onSuccess, Action onClosed = null)
        {
            onAOASuccess = onSuccess;
            onAOAFailed = onClosed;
            if (IsAOAAvailable)
            {
                Debug.Log("Showing app open ad.");
                appOpenAd.Show();
            }
            else
            {
                Debug.LogError("App open ad is not ready yet.");
            }
        }

        #endregion


        void HandleAdPaidEvent(AdValue adValue, string unitID)
        {
            //AdValue adValue = args.AdValue;
            // Log an event with ad value parameters
            Firebase.Analytics.Parameter[] LTVParameters =
            {
                // Log ad value in micros.
                new Firebase.Analytics.Parameter("valuemicros", adValue.Value),
                // These values below won’t be used in ROAS recipe.
                // But log for purposes of debugging and future reference.
                new Firebase.Analytics.Parameter("currency", adValue.CurrencyCode),
                new Firebase.Analytics.Parameter("precision", (int)adValue.Precision),
                new Firebase.Analytics.Parameter("adunitid", unitID),
                new Firebase.Analytics.Parameter("network",
                    rewardVideo.GetResponseInfo().GetMediationAdapterClassName())
            };

            GAManager.Instance.LogEvent("paid_ad_impression", LTVParameters);
        }
    }
}