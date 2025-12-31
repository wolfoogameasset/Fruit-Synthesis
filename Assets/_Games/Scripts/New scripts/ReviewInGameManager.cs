using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Play.Review;
public class ReviewInGameManager : MonoBehaviour
{
    public static ReviewInGameManager Ins;
    ReviewManager _reviewManager;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Ins = this; 
    }


    public void OpenRateUs()
    {
        StartCoroutine(StartTheReview());
    }

    public IEnumerator StartTheReview()
    {
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("Show rate app");
        PlayerPrefs.SetInt("RateApp", 1);
        _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            //Debug.Log("requestFlowOperation Fail" + requestFlowOperation.Error.ToString());
            yield break;
        }
        else
        {
            //Debug.Log("requestFlowOperation NOT Fail");
        }

        var _playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);

        yield return launchFlowOperation;

        _playReviewInfo = null;

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
}
