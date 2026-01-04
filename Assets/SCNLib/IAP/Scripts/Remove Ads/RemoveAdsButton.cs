using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SCN.Animation;
using SCN.Common;

namespace SCN.IAP
{
    public class RemoveAdsButton : MonoBehaviour
    {
        AnimationSpineController animController;
        NoAdsAnim noAdsAnim;

        RectTransform btnTrans;

        Vector3 idleSize = new Vector3(300, 300);
        Vector3 smallSize = new Vector3(640, 200);
        Vector3 wideSize = new Vector3(1300, 300);

        [Header("Adjust")]
        [Tooltip("Neu bo tick se can goi ham Init")]
        [SerializeField] bool initAwake = true;
        [Tooltip("Hieu ung animation cua nut bam")]
        [SerializeField] AnimType expandType;

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
            removeAdsButton.onClick.AddListener(ShowRemoveAdsPopup);

            btnTrans = transform.GetChild(0).GetComponent<RectTransform>();
            animController = transform.GetChild(1).GetComponent<AnimationSpineController>();
            noAdsAnim = animController.GetComponent<NoAdsAnim>();

            animController.InitValue();

            if (expandType == AnimType.Small)
            {
                animController.PlayAnimation(noAdsAnim.AnimSmall, true);
            }
            else if (expandType == AnimType.Wide)
            {
                animController.PlayAnimation(noAdsAnim.AnimWide, true);
            }

            animController.OnEvent += Event_OnEvent;
        }

        void ShowRemoveAdsPopup()
        {
            ParentGateManager.Instance.OpenDialog(() =>
            {
                IAPManager.Instance.OpenRemoveAdsPanel();
            });
        }

        void Event_OnEvent(string animName, int order)
        {
            if (animName == noAdsAnim.AnimSmall.animName) // ngang 2
            {
                if (order == 0)
                {
                    DOTweenManager.Instance.TweenSizeDelta(btnTrans, idleSize, smallSize, 0.28f);
                }
                else if (order == 1)
                {
                    DOTweenManager.Instance.TweenSizeDelta(btnTrans, smallSize, idleSize, 0.6f);
                }
            }
            else if (animName == noAdsAnim.AnimWide.animName) // ngang
            {
                if (order == 0)
                {
                    DOTweenManager.Instance.TweenSizeDelta(btnTrans, idleSize, wideSize, 0.32f);
                }
                else if (order == 1)
                {
                    DOTweenManager.Instance.TweenSizeDelta(btnTrans, wideSize, idleSize, 0.42f);
                }
            }
        }

        enum AnimType
        {
            Small = 0,
            Wide = 1
        }
    }
}