using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Firebase.Analytics;
using DG.Tweening;
using SCN.Ads;
using SCN.Common;

namespace SCN.IAP
{
    public class RemoveAdsPopup : MonoBehaviour
    {
        [SerializeField] string idRemoveAds;

        [SerializeField] Transform popupTrans;
        [SerializeField] Button closeButton;
        [SerializeField] Button buyButton;
        [SerializeField] Text costBuyTxt;
        [SerializeField] Button restoreButton;
        [SerializeField] Image popupBannerImage;

        int currentTheme;

        private void Awake()
        {
            IAPManager.Instance.OnBuyDone += OnPurchaseComplete;
        }
        private void OnDestroy()
        {
            IAPManager.Instance.OnBuyDone -= OnPurchaseComplete;
        }

        private void Start()
        {
            idRemoveAds = IAPManager.Instance.IdRemoveAds;
            closeButton.onClick.AddListener(OnClose);
            buyButton.onClick.AddListener(() =>
            {
                Debug.Log(IAPManager.Instance != null);
                IAPManager.Instance.BuyProductID(idRemoveAds, IAPManager.Instance.GetProduct(idRemoveAds).metadata.localizedTitle, costBuyTxt.text, IAPManager.Instance.GetProduct(idRemoveAds).metadata.isoCurrencyCode);
            });
            
            restoreButton.gameObject.SetActive(true);
            restoreButton.onClick.AddListener(OnRestoreClick);

            if (IAPManager.Instance.RemoveAdsBannerImageSprite)
            {
                popupBannerImage.sprite = IAPManager.Instance.RemoveAdsBannerImageSprite;
            }
            AssignSaleOff();
        }

        public void OpenPopup()
		{
            gameObject.SetActive(true);
            DOTweenManager.Instance.ScaleToShow(popupTrans);
        }

        private void OnClose()
        {
            gameObject.SetActive(false);
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }
        public void OnPurchaseComplete(Product product)
        {
            if (product.definition.id == idRemoveAds)
            {
                OnRemoveAds();
            }
            Debug.Log($"Purchase complete - Product: '{product.definition.id}'");
        }

        void OnRestoreClick()
        {
            if (IAPManager.Instance.CheckBought(idRemoveAds))
            {
                IAPManager.Instance.RestorePurchases();
            }
        }
        private void OnRemoveAds()
        {
            AdsManager.Instance.SetRemovedAds();
            AdsManager.Instance.HideBanner();

            //Bắn event xử lý ở đây
            //EventDispatcher.Instance.Dispatch(new EventKey.OnRemoveAds());

            DOVirtual.DelayedCall(0.25f, () => {
                transform.gameObject.SetActive(false);
            });
            Debug.Log("Remove Ads");

            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            FirebaseAnalytics.LogEvent("RemovedAds", "RemovedAds", 1);
            //GAManager.Instance.TrackPurchaseIAP();
        }
        private void AssignSaleOff()
        {
            Product product = IAPManager.Instance.GetProduct(idRemoveAds);
            if (product != null)
            {
                costBuyTxt.text = product.metadata.localizedPriceString;
            }
            else
            {
                costBuyTxt.text = "Loading";
            }
        }
    }
}