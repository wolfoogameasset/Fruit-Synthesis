using DG.Tweening;
using SCN.Ads;
using SCN.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCN.FruitSynthesis
{
    public class RateUsPanelController : MonoBehaviour
    {
        [SerializeField] GameObject blackImage;
        [SerializeField] GameObject blockImage;
        [SerializeField] Transform rateUsPopupTrans;
        [SerializeField] EventTrigger[] starTrigger;
        [SerializeField] Button submitButton;
        int currentRating;
        private void Awake()
        {
            ReviewInGameManager.OnEnoughPopupCount += Show_RateUsPopup;
        }

        private void OnDestroy()
        {
            ReviewInGameManager.OnEnoughPopupCount -= Show_RateUsPopup;
        }

        void Start()
        {
            for (int i = 0; i < starTrigger.Length; i++)
            {
                Master.AddEventTriggerListener(starTrigger[i], EventTriggerType.PointerDown, data => ChangeRating(data, i));
            }
            submitButton.onClick.AddListener(Rate);
            blackImage.SetActive(false);
            blockImage.SetActive(false);
            rateUsPopupTrans.gameObject.SetActive(false);
        }
        void Show_RateUsPopup()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            for (int i = 0; i < starTrigger.Length; i++)
            {
                starTrigger[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            rateUsPopupTrans.DOScale(0.8f, 0).OnComplete(() =>
            {
                rateUsPopupTrans.gameObject.SetActive(true);
                rateUsPopupTrans.DOScale(1, 0.25f).OnComplete(() =>
                {
                    //revivePanelTrans.
                    blockImage.SetActive(false);
                });
            });
        }
        void Hide_RateUsPopup()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            rateUsPopupTrans.DOScale(0.8f, 0.25f).OnComplete(() =>
            {
                rateUsPopupTrans.gameObject.SetActive(false);
                blackImage.SetActive(false);
                blockImage.SetActive(false);
                AdsManager.Instance.ShowInterstitial();
            });
        }
        void ChangeRating(BaseEventData data, int starRating)
        {
            currentRating = starRating;
            for (int i = 0; i < starTrigger.Length; i++)
            {
                if (i <= starRating)
                {
                    starTrigger[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    starTrigger[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        void Rate()
        {
            if (currentRating <= 2)
            {
                Hide_RateUsPopup();
                ReviewInGameManager.Ins.isFourStared = false;
            }
            else
            {
                ReviewInGameManager.Ins.OpenRateUs();
                ReviewInGameManager.Ins.isFourStared = true;
                Hide_RateUsPopup();
            }
        }
    }
}