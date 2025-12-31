using SCN.Ads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using Unity.VisualScripting.FullSerializer;
using System;
using SCN.FirebaseLib.FA;

public class NativeAdvance : MonoBehaviour
{
    [SerializeField] RawImage AdIconTexture;
    [SerializeField] Text AdHeadline;
    [SerializeField] Text AdDescription;
    [SerializeField] GameObject AdLoaded;
    [SerializeField] GameObject AdLoading;

    private NativeAd nativeAd;
    private bool nativeLoaded = false;
    bool isRequesting;
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

    int retry= 0;
    private void OnEnable()
    {
        RequestNativeAd();
    }
    #region NativeAds
    private void RequestNativeAd()
    {
        if (isRequesting)
        {
            return;
        }
        if (retry >= retryTimes.Length)
        {
            retry = retryTimes[retryTimes.Length - 1];
        }
        int num = retryTimes[retry];
        isRequesting = true;
        DelayCallback(num, delegate
        {
            if (IsInternetAvailable)
            {
                DoRequestNative();
            }
            else
            {
                WaitInternet(DoRequestNative);
            }
        });
       
    }
    private void DoRequestNative()
    {
        AdmobConfig instance = AdsManager.Instance.AdmobControl.Config;
        AdLoader adLoader = new AdLoader.Builder(instance.NativeID)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        //adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        adLoader.OnNativeAdClicked += this.HandleAdClicked;
        adLoader.OnNativeAdImpression += this.HandleAdImpression;
        adLoader.OnNativeAdOpening += this.HandleAdOpening;
        adLoader.OnNativeAdClosed += this.HandleAdClose;
        adLoader.LoadAd(new AdRequest());
    }

    private void DelayCallback(float delayTime, Action callback)
    {
        if (callback != null)
        {
            if (delayTime == 0f)
            {
                callback?.Invoke();
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
    private void HandleAdFailedToLoad(object sender)//, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Native ad failed to load: ");// + args.LoadAdError.GetMessage());
        GAManager.Instance.LogEvent("ad_native_failed_to_load");
        isRequesting = false;
        retry++;
        RequestNativeAd();
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        retry = 0;
        isRequesting = false;
        Debug.Log("Native ad loaded.");
        this.nativeAd = args.nativeAd;

        //set textures and details
        AdIconTexture.texture = nativeAd.GetIconTexture();
        AdHeadline.text = nativeAd.GetHeadlineText();
        AdDescription.text = nativeAd.GetBodyText();

        AdIconTexture.gameObject.SetActive(true);
        AdHeadline.gameObject.SetActive(true);
        AdDescription.gameObject.SetActive(true);
        //register gameobjects with native ads api
        if (!nativeAd.RegisterIconImageGameObject(AdIconTexture.gameObject))
        {
            Debug.Log("error registering icon");
            AdIconTexture.gameObject.SetActive(false);
        }
        if (!nativeAd.RegisterHeadlineTextGameObject(AdHeadline.gameObject))
        {
            Debug.Log("error registering headline");
            AdHeadline.gameObject.SetActive(false);
        }
        if (!nativeAd.RegisterBodyTextGameObject(AdDescription.gameObject))
        {
            AdDescription.gameObject.SetActive(false);
            Debug.Log("error registering description");
        }
        //disable loading and enable ad object
        AdLoaded.gameObject.SetActive(true);
        AdLoading.gameObject.SetActive(false);
    }
    private void HandleAdClicked(object sender,EventArgs eventArgs)
    {
        Debug.Log("Native ad was clicked: " + eventArgs.ToString());
        GAManager.Instance.LogEvent("ad_native_click");
    }
    private void HandleAdImpression(object sender, EventArgs eventArgs)
    {
        Debug.Log("Native ad record impression: " + eventArgs.ToString());
        GAManager.Instance.LogEvent("ad_native_impression");
    }
    private void HandleAdOpening(object sender, EventArgs eventArgs)
    {
        Debug.Log("Native ad opening: " + eventArgs.ToString());
    }
    private void HandleAdClose(object sender, EventArgs eventArgs)
    {
        Debug.Log("Native ad Close: " + eventArgs.ToString());
    }
    #endregion
}
