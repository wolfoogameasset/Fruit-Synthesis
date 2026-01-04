using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.IAP
{
    public class SubscriptionButton : MonoBehaviour
    {
        [Header("Adjust")]
        [Tooltip("Neu bo tick se can goi ham Init")]
        [SerializeField] bool initAwake = true;

        [SerializeField] Button removeAdsButton;

        private void Start()
        {
            if (initAwake)
            {
                Init();
            }
        }

        public void Init()
        {
            removeAdsButton.onClick.AddListener(ShowSubscriptionPopup);
        }
        void ShowSubscriptionPopup()
        {
            ParentGateManager.Instance.OpenDialog(() =>
            {
                IAPManager.Instance.OpenSubscriptionPanel();
            });
        }
    }
}
