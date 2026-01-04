using Firebase.Analytics;
using SCN.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace SCN.IAP
{
    public class SubscriptionPopup : MonoBehaviour
    {
        [SerializeField] string _idSubscriptionMonthly;
        [SerializeField] string _idSubscriptionAnnual;
        [SerializeField] string _idUnlockAll;

        [SerializeField] Transform _popupTrans;
        [SerializeField] Button _closeButton;
        [SerializeField] Button _buyButtonMonthly;
        [SerializeField] Text _costBuyMonthlyTxt;
        [SerializeField] Button _buyButtonAnnual;
        [SerializeField] Text _costBuyAnnualTxt;
        [SerializeField] Button _buyButtonUnlockAll;
        [SerializeField] Text _costBuyUnlockAllTxt;
        [SerializeField] Button _restoreButton;

        [SerializeField] Button _policyButton;
        [SerializeField] Button _termsButton;

        [SerializeField] GameObject _subDetailGO;
        [SerializeField] Text _subStatusText;
        [SerializeField] Text _expireDateText;
        [SerializeField] Text _remainingTimeText;
        [SerializeField] Text _trialStatusText;

        SubscriptionManager subManager;
        SubscriptionInfo subInfo;

        private const string policyUrl = "https://wolfoogames.com/privacy-policy.html";
        private const string termsUrl = "https://wolfoogames.com/terms-conditions.html";

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
            _idSubscriptionMonthly = IAPManager.Instance.IdSubscriptionMonthly;
            _idSubscriptionAnnual = IAPManager.Instance.IdSubscriptionAnnual;
            _idUnlockAll = IAPManager.Instance.IdUnlockAll;

            _closeButton.onClick.AddListener(OnClose);

            if (_buyButtonMonthly != null && _idSubscriptionMonthly != "")
            {
                _buyButtonMonthly.onClick.AddListener(() =>
                {
                    IAPManager.Instance.BuyProductID(_idSubscriptionMonthly, IAPManager.Instance.GetProduct(_idSubscriptionMonthly).metadata.localizedTitle, _costBuyMonthlyTxt.text, IAPManager.Instance.GetProduct(_idSubscriptionMonthly).metadata.isoCurrencyCode);
                });
            }
            if (_buyButtonAnnual != null && _idSubscriptionAnnual != "")
            {
                _buyButtonAnnual.onClick.AddListener(() =>
                {
                    IAPManager.Instance.BuyProductID(_idSubscriptionAnnual, IAPManager.Instance.GetProduct(_idSubscriptionAnnual).metadata.localizedTitle, _costBuyAnnualTxt.text, IAPManager.Instance.GetProduct(_idSubscriptionAnnual).metadata.isoCurrencyCode);
                });
            }
            if (_buyButtonUnlockAll != null && _idUnlockAll != "")
            {
                _buyButtonUnlockAll.onClick.AddListener(() =>
                {
                    IAPManager.Instance.BuyProductID(_idUnlockAll, IAPManager.Instance.GetProduct(_idUnlockAll).metadata.localizedTitle, _costBuyUnlockAllTxt.text, IAPManager.Instance.GetProduct(_idUnlockAll).metadata.isoCurrencyCode);
                });
            }
            _termsButton.onClick.AddListener(OpenTermsAndConditions);
            _policyButton.onClick.AddListener(OpenPrivacyPolicy);

            _trialStatusText.gameObject.SetActive(false);
            _restoreButton.gameObject.SetActive(true);
            _restoreButton.onClick.AddListener(Callback_RestorePurchase);
            AssignSaleOff();
        }

        public void OpenPopup()
        {
            gameObject.SetActive(true);
            DOTweenManager.Instance.ScaleToShow(_popupTrans);
            IAPManager.Instance.CheckSubscriptionStatus();
            StartCoroutine(DelayCallMaster.WaitForEndOfFrame(() =>
            {
                UpdatePopupUI();
            }));
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
            if (product.definition.id == _idSubscriptionMonthly
                || product.definition.id == _idSubscriptionAnnual
                || product.definition.id == _idUnlockAll)
            {
                OnSubscription();
            }
            Debug.Log($"Purchase complete - Product: '{product.definition.id}'");
        }
        private void OnSubscription()
        {
            //AdsManager.Instance.SetRemovedAds();
            //AdsManager.Instance.HideBanner();

            //Bắn event xử lý ở đây
            //EventDispatcher.Instance.Dispatch(new EventKey.OnRemoveAds());
            IAPManager.Instance.CheckSubscriptionStatus();
            StartCoroutine(DelayCallMaster.WaitForEndOfFrame(() =>
            {
                UpdatePopupUI();
            }));
            Debug.Log("Subscription");

            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            //FirebaseAnalytics.LogEvent("subscription", "subscription", 1);
            //GAManager.Instance.TrackBuySubscription();
        }
        private void AssignSaleOff()
        {
            Product productMonthly = IAPManager.Instance.GetProduct(_idSubscriptionMonthly);
            if (productMonthly != null)
            {
                _costBuyMonthlyTxt.text = $"Monthly plan - {productMonthly.metadata.localizedPriceString}/month";
            }
            else
            {
                _costBuyMonthlyTxt.text = "Monthly plan - Loading...";
            }

            Product productAnnual = IAPManager.Instance.GetProduct(_idSubscriptionAnnual);
            if (productAnnual != null)
            {
                _costBuyAnnualTxt.text = $"Annual plan - {productAnnual.metadata.localizedPriceString}/year";
            }
            else
            {
                _costBuyAnnualTxt.text = "Annual plan - Loading";
            }

            Product productUnlockAll = IAPManager.Instance.GetProduct(_idUnlockAll);
            if (productUnlockAll != null)
            {
                _costBuyUnlockAllTxt.text = $"Unlock for life plan - {productUnlockAll.metadata.localizedPriceString}";
            }
            else
            {
                _costBuyUnlockAllTxt.text = "Unlock for life plan - Loading";
            }
        }

        void Callback_RestorePurchase()
        {
            IAPManager.Instance.RestorePurchases();
            IAPManager.Instance.CheckSubscriptionStatus();
            StartCoroutine(DelayCallMaster.WaitForEndOfFrame(() =>
            {
                UpdatePopupUI();
            }));
        }

        private void UpdatePopupUI()
        {
            IAPManager.Instance.CheckSubscriptionStatus();

            _subDetailGO.gameObject.SetActive(false);
            if (_buyButtonMonthly != null)
            {
                _buyButtonMonthly.gameObject.SetActive(false);
            }
            if (_buyButtonAnnual)
            {
                _buyButtonAnnual.gameObject.SetActive(false);
            }
            if (_buyButtonUnlockAll)
            {
                _buyButtonUnlockAll.gameObject.SetActive(false);
            }
            _trialStatusText.gameObject.SetActive(false);
            _restoreButton.gameObject.SetActive(false);

            Product subProductMothly = IAPManager.Instance.GetProduct(_idSubscriptionMonthly);
            Product subProductAnnual = IAPManager.Instance.GetProduct(_idSubscriptionAnnual);
            Product productUnlockAll = IAPManager.Instance.GetProduct(_idUnlockAll);

            StartCoroutine(DelayCallMaster.WaitForEndOfFrame(() =>
            {
                if (productUnlockAll != null)
                {
                    if (productUnlockAll.hasReceipt)
                    {
                        Debug.Log("Subscription product: unlock all");
                        UpdatePopupUI_NonConsume_UnlockAll();
                        return;
                    }
                }
                if (subProductAnnual != null)
                {
                    if (subProductAnnual.hasReceipt
                    && new SubscriptionManager(subProductAnnual, null).getSubscriptionInfo().isSubscribed() == Result.True
                    //&& new SubscriptionManager(subProductAnnual, null).getSubscriptionInfo().isCancelled() != Result.True
                    && new SubscriptionManager(subProductAnnual, null).getSubscriptionInfo().isExpired() != Result.True)
                    {
                        Debug.Log("Subscription product: yearly");
                        subManager = new SubscriptionManager(subProductAnnual, null);
                        subInfo = subManager.getSubscriptionInfo();
                        UpdatePopupUI_Subscription();
                        return;
                    }
                }

                if (subProductMothly != null)
                {
                    if (subProductMothly.hasReceipt
                    && new SubscriptionManager(subProductMothly, null).getSubscriptionInfo().isSubscribed() == Result.True
                    //&& new SubscriptionManager(subProductMothly, null).getSubscriptionInfo().isCancelled() != Result.True
                    && new SubscriptionManager(subProductMothly, null).getSubscriptionInfo().isExpired() != Result.True)
                    {
                        Debug.Log("Subscription product: monthly");
                        subManager = new SubscriptionManager(subProductMothly, null);
                        subInfo = subManager.getSubscriptionInfo();
                        UpdatePopupUI_Subscription();
                        return;
                    }
                }

                Debug.Log("Subscription product is Not Active");
                UpdatePopupUI_Other();
            }));
        }

        void UpdatePopupUI_Subscription()
        {
            #region Test Free Trail (Fail)
            //Note: Khong ro loi tai sao isFreeTrial luon bang false, tam thoi khong them logic nguoi dung cancel khi dang freetrial
            //Debug.Log(subInfo.isFreeTrial().ToString());
            //if (subInfo.isFreeTrial() == Result.True)
            //{
            //    Debug.Log("Subscription is in free trial");
            //    _trialStatusText.text = $"Free trial remaining for: {subInfo.getFreeTrialPeriod().Days} days, {subInfo.getFreeTrialPeriod().Hours}h, {subInfo.getFreeTrialPeriod().Minutes}m, {subInfo.getFreeTrialPeriod().Seconds}s";
            //    _trialStatusText.gameObject.SetActive(true);
            //}
            //else
            //{
            //    Debug.Log("Subscription is not in free trial");
            //    _trialStatusText.gameObject.SetActive(false);
            //}
            #endregion 

            Debug.Log("Subscription is actived");
            _subStatusText.text = "SUBCRIBED!";
            if (subInfo.isCancelled() == Result.True)
            {
                _expireDateText.text = "Subscription will expired at: " + subInfo.getExpireDate().ToString("MM/dd/yyyy HH:mm:ss");
            }
            else if (subInfo.isAutoRenewing() == Result.True)
            {
                _expireDateText.text = "Subscription will renew at: " + subInfo.getExpireDate().ToString("MM/dd/yyyy HH:mm:ss");
            }
            else
            {
                _expireDateText.text = "Subscription will end at: " + subInfo.getExpireDate().ToString("MM/dd/yyyy HH:mm:ss");
            }

            _remainingTimeText.text = $"Subscription remaining for: {subInfo.getRemainingTime().Days} days, {subInfo.getRemainingTime().Hours}h, {subInfo.getRemainingTime().Minutes}m, {subInfo.getRemainingTime().Seconds}s ";
            _subDetailGO.SetActive(true);
        }
        void UpdatePopupUI_NonConsume_UnlockAll()
        {
            _subStatusText.text = "SUBCRIBED!";
            _expireDateText.text = "This package will not be expired";
            _remainingTimeText.text = "Thank you for buying the unlock all content pack";
            _subDetailGO.SetActive(true);
        }
        void UpdatePopupUI_Other()
        {
            Debug.Log("Subscription is inactive (new player), expired, or unsuported");
            _subStatusText.text = "SUBSCRIBE NOW\nGET PREMIUM!";
            if (_buyButtonMonthly != null && _idSubscriptionMonthly != "")
            {
                _buyButtonMonthly.gameObject.SetActive(true);
            }
            if (_buyButtonAnnual != null && _idSubscriptionAnnual != "")
            {
                _buyButtonAnnual.gameObject.SetActive(true);
            }
            if (_buyButtonUnlockAll != null && _idUnlockAll != "")
            {
                _buyButtonUnlockAll.gameObject.SetActive(true);
            }
            _restoreButton.gameObject.SetActive(true);
        }

        void OpenTermsAndConditions()
        {
            Application.OpenURL(termsUrl);
        }
        void OpenPrivacyPolicy()
        {
            Application.OpenURL(policyUrl);
        }
    }
}
