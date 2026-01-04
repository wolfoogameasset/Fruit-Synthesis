using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing.Security;
using System.Collections;
using SCN.Common;
using SCN.FirebaseLib.FA;
using UnityEngine.Purchasing.Extension;
using DG.Tweening;
using DG.Tweening.Core.Easing;

namespace SCN.IAP
{
    public class IAPManager : MonoBehaviour, IStoreListener, IDetailedStoreListener
    {
        public static IAPManager Instance;

        [SerializeField] GameObject _parentGatePopup;
        [SerializeField] IAPPopupThemeEnum _iAPPopupTheme;

        [SerializeField] bool _isUsingHybridPopup;

        [SerializeField] GameObject _removeAdsPopup;
        [SerializeField] string _idRemoveAds;
        [SerializeField] Sprite _removeAdsBannerImageSprite;

        [SerializeField] bool _isUsingSubscription;
        [SerializeField] GameObject _subscriptionPopup;
        [SerializeField] string _idSubscriptionMonthly;
        [SerializeField] string _idSubscriptionAnnual;
        [SerializeField] string _idUnlockAll;

        const string SubscriptionKey = "SUBSCRIBED";

        public enum IAPPopupThemeEnum
        {
            Theme_0 = 0,
            Theme_1 = 1,
            Theme_2 = 2,
            Theme_3 = 3,
        }

        #region Parent gate
        [Space]
        [Header("Parent gate setings")]

        [Tooltip("So lan chon lai dap an")]
        [SerializeField] int _retryTime = 3;

        [Tooltip("Thoi gian cho lam thu tiep theo")]
        [SerializeField] int _nextTryTime = 60;

        [Tooltip("Layer dialog")]
        [SerializeField] string _sortingLayer;
        [Tooltip("Set sorting de dialog co order cao nhat")]
        [SerializeField] int _orderInLayer;
        #endregion

        #region public var
        public IAPPopupThemeEnum IAPPopupTheme { get => _iAPPopupTheme; set => _iAPPopupTheme = value; }
        public GameObject ParentGatePopup { get => _parentGatePopup; set => _parentGatePopup = value; }
        public int RetryTime { get => _retryTime; set => _retryTime = value; }
        public int NextTryTime { get => _nextTryTime; set => _nextTryTime = value; }
        public string SortingLayer { get => _sortingLayer; set => _sortingLayer = value; }
        public int OrderInLayer { get => _orderInLayer; set => _orderInLayer = value; }

        public GameObject RemoveAdsPopup { get => _removeAdsPopup; set => _removeAdsPopup = value; }
        public string IdRemoveAds { get => _idRemoveAds; set => _idRemoveAds = value; }
        public Sprite RemoveAdsBannerImageSprite { get => _removeAdsBannerImageSprite; set => _removeAdsBannerImageSprite = value; }

        public bool IsUsingSubscription { get => _isUsingSubscription; set => _isUsingSubscription = value; }
        public GameObject SubscriptionPopup { get => _subscriptionPopup; set => _subscriptionPopup = value; }
        public string IdSubscriptionMonthly { get => _idSubscriptionMonthly; set => _idSubscriptionMonthly = value; }
        public string IdSubscriptionAnnual { get => _idSubscriptionAnnual; set => _idSubscriptionAnnual = value; }
        public string IdUnlockAll { get => _idUnlockAll; set => _idUnlockAll = value; }

        #endregion

        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        //public UnityEvent OnBuyDone = null, OnBuyFail = null;
        public Action<Product> OnBuyDone = null, OnBuyFail = null;
        public Action OnRestoreFail = null;
        public static Action<bool> OnFinishCheckSubscriptionStatus;

        public bool IsInitDone => IsInitialized();


        private bool isBuying = false;

        GameObject removeAdsPopup, subscriptionPopup, spawnedRemoveAdsPopup, spawnedSubscriptionPopup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public void Start()
        {
            StartCoroutine(IEStart());
        }

        public IEnumerator IEStart()
        {
            yield return new WaitForEndOfFrame();
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            var catalog = ProductCatalog.LoadDefaultCatalog();

            foreach (var product in catalog.allValidProducts)
            {
                if (product.allStoreIDs.Count > 0)
                {
                    var ids = new IDs();
                    foreach (var storeID in product.allStoreIDs)
                    {
                        ids.Add(storeID.id, storeID.store);
                    }
                    builder.AddProduct(product.id, product.type, ids);
                }
                else
                {
                    builder.AddProduct(product.id, product.type);
                }

            }

            // Continue adding the non-consumable product.

            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 


            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);

            if (_isUsingSubscription)
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    CheckSubscriptionStatus();
                });
            }
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }
        public Product GetProduct(string productID)
        {
            Product product = null;
            try
            {
                product = m_StoreController.products.WithID(productID);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                product = null;
            }
            return product;
        }

        public bool CheckBought(string productID)
        {
            Product product = null;
            try
            {
                product = m_StoreController.products.WithID(productID);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                product = null;
            }
            if (product != null && product.hasReceipt)
            {
                return true;
            }
            return false;
        }
        string productId;
        string productName;
        string price;
        string currency;
        public void BuyProductID(string productID, string product_name, string price, string currency)
        {
            if (isBuying) return;
            if (!string.IsNullOrEmpty(productID) && !string.IsNullOrEmpty(product_name))
                Debug.Log($"<color=yellow>Buy ProductId : </color>" + productID + $"<color=yellow> - Product Name : </color> " + product_name + $"<color=yellow> - Price : </color> " + price + $"<color=yellow> - Currency : </color> " + currency);
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                isBuying = true;
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = null;
                try
                {
                    product = m_StoreController.products.WithID(productID);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                    product = null;
                }

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null)
                    this.productId = product.definition.id;
                this.productName = product_name;
                this.price = price;
                this.currency = currency;
                Debug.Log($"<color=yellow>Product : </color>" + (product != null));
                if (product != null)
                    Debug.Log($"<color=yellow>Product availableToPurchase : </color>" + product.availableToPurchase);
                if (product != null && product.availableToPurchase)
                {
                    //Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    //NoticeManager.Instance.LogNotice(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product, productID);
                }
                // Otherwise ...
                else
                {
                    isBuying = false;
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    //NoticeManager.Instance.LogNotice("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                isBuying = false;
                Debug.Log("BuyProductID FAIL. Not initialized.");
                //NoticeManager.Instance.LogNotice("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                //NoticeManager.Instance.LogNotice("RestorePurchases FAIL. Not initialized.");
                return;
            }

            //Product product
            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result, message) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.\n" + message);
                    if (result)
                    {
                        if (CheckBought(IdRemoveAds))
                        {
                            OnBuyDone?.Invoke(GetProduct(IdRemoveAds));
                        }

                        if (CheckBought(IdUnlockAll))
                        {
                            OnBuyDone?.Invoke(GetProduct(IdUnlockAll));
                        }
                        else if (CheckBought(IdSubscriptionMonthly))
                        {
                            OnBuyDone?.Invoke(GetProduct(IdSubscriptionMonthly));
                        }
                        else if (CheckBought(IdSubscriptionAnnual))
                        {
                            OnBuyDone?.Invoke(GetProduct(IdSubscriptionAnnual));
                        }
                    }
                });
            }
            // Otherwise ...
            else if (Application.platform == RuntimePlatform.Android)
            {
                var playStore = m_StoreExtensionProvider.GetExtension<IGooglePlayStoreExtensions>();

                playStore.RestoreTransactions(OnRestore);
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        void OnRestore(bool success, string error)
        {
            var restoreMessage = "";
            if (success)
            {
                // This does not mean anything was restored,
                // merely that the restoration process succeeded.
                restoreMessage = "Restore Successful";

                //Dispatch an event here!!!!!!!!!!!!!!!!!!!!!!!!
                if (CheckBought(IdRemoveAds))
                {
                    OnBuyDone?.Invoke(GetProduct(IdRemoveAds));
                }

                if (CheckBought(IdUnlockAll))
                {
                    OnBuyDone?.Invoke(GetProduct(IdUnlockAll));
                }
                else if (CheckBought(IdSubscriptionMonthly))
                {
                    OnBuyDone?.Invoke(GetProduct(IdSubscriptionMonthly));
                }
                else if (CheckBought(IdSubscriptionAnnual))
                {
                    OnBuyDone?.Invoke(GetProduct(IdSubscriptionAnnual));
                }
            }
            else
            {
                // Restoration failed.
                restoreMessage = $"Restore Failed with error: {error}";
            }

            Debug.Log(restoreMessage);
        }

        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error + "\n" + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var catalog = ProductCatalog.LoadDefaultCatalog();

            foreach (var product in catalog.allValidProducts)
            {

                if (String.Equals(args.purchasedProduct.definition.id, product.id, StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    //#if  UNITY_EDITOR
                    if (OnBuyDone != null)
                    {
                        OnBuyDone.Invoke(args.purchasedProduct);
                        OnBuyDone = null;
                    }
                    //#endif
                    return PurchaseProcessingResult.Complete;
                }
            }
            isBuying = false;
            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Pending;
        }

        bool checkProductList(PurchaseEventArgs args, List<string> ProductList)
        {
            foreach (string s in ProductList)
            {
                if (String.Equals(args.purchasedProduct.definition.id, s, StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    //NoticeManager.Instance.LogNotice(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                    OnBuyFail = null;
                    if (OnBuyDone != null)
                    {
                        OnBuyDone.Invoke(args.purchasedProduct);
                        OnBuyDone = null;
                    }
                    return true;
                }
            }
            return false;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            isBuying = false;
            //OnBuyDone = null;
            if (OnBuyFail != null)
            {
                OnBuyFail.Invoke(product);
                OnBuyFail = null;
            }

            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

        }
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            isBuying = false;
            //OnBuyDone = null;
            if (OnBuyFail != null)
            {
                OnBuyFail.Invoke(product);
                OnBuyFail = null;
            }

            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureDescription));
        }

        public void CheckSubscriptionStatus()
        {
            Product subProductMothly = GetProduct(_idSubscriptionMonthly);
            Product subProductAnnual = GetProduct(_idSubscriptionAnnual);
            Product productUnlockAll = GetProduct(_idUnlockAll);

            //Note: Khong ro loi tai sao isFreeTrial luon bang false, tam thoi khong them logic IAPManager: nguoi dung cancel khi dang freetrial
            if (productUnlockAll != null)
            {
                if (productUnlockAll.hasReceipt)
                {
                    ActiveSubscriptionContent();
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
                    ActiveSubscriptionContent();
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
                    ActiveSubscriptionContent();
                    return;
                }
            }
            ActiveNonSubscriptionContent();
        }

        void ActiveSubscriptionContent()
        {
            PlayerPrefs.SetInt(SubscriptionKey, 1);
            PlayerPrefs.Save();
            OnFinishCheckSubscriptionStatus?.Invoke(true);
            Debug.Log("Active subscription content");
        }
        void ActiveNonSubscriptionContent()
        {
            PlayerPrefs.SetInt(SubscriptionKey, 0);
            PlayerPrefs.Save();
            OnFinishCheckSubscriptionStatus?.Invoke(false);
            Debug.Log("Active non subscription content");
        }

        public bool IsSubscribed
        {
            get => (PlayerPrefs.GetInt(SubscriptionKey) == 1);
        }

        public void SetSubscription()
        {
            PlayerPrefs.SetInt(SubscriptionKey, 1);
            //GAManager.Instance.TrackBuySubscription();
        }

        public void OpenRemoveAdsPanel()
        {
            if (removeAdsPopup == null)
            {
                removeAdsPopup = RemoveAdsPopup;
            }

            if (spawnedRemoveAdsPopup == null)
            {
                spawnedRemoveAdsPopup = Instantiate(removeAdsPopup, transform);
            }

            spawnedRemoveAdsPopup.GetComponent<RemoveAdsPopup>().OpenPopup();

            GAManager.Instance.TrackShowRemoveAds();
        }

        public void OpenSubscriptionPanel()
        {
            if (subscriptionPopup == null)
            {
                subscriptionPopup = SubscriptionPopup;
            }

            if (spawnedSubscriptionPopup == null)
            {
                spawnedSubscriptionPopup = Instantiate(subscriptionPopup, transform);
            }

            spawnedSubscriptionPopup.GetComponent<SubscriptionPopup>().OpenPopup();

            //GAManager.Instance.TrackShowSubscription();
        }

#if UNITY_EDITOR
        public void LoadCurrentIAPTheme()
        {
            ParentGatePopup = LoadSource.LoadAssetAtPath<GameObject>($"Assets/SCNLib/IAP/Prefabs/Parent gate popup/{IAPPopupTheme}_ParentGatePopup.prefab");
            RemoveAdsPopup = LoadSource.LoadAssetAtPath<GameObject>($"Assets/SCNLib/IAP/Prefabs/Remove ads popup/{IAPPopupTheme}_RemoveAdsPopup.prefab");
            //SubscriptionPopup = LoadSource.LoadAssetAtPath<GameObject>($"Assets/SCNLib/IAP/Prefabs/Subscription popup/SubscriptionPopup.prefab");
        }
#endif
    }
}