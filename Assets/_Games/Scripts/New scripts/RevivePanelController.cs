using DG.Tweening;
using SCN.Ads;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.FruitSynthesis
{
    public class RevivePanelController : MonoBehaviour
    {
        [SerializeField] GameObject blackImage;
        [SerializeField] GameObject blockImage;
        [SerializeField] Transform revivePanelTrans;
        [SerializeField] Button reviveButton;
        [SerializeField] TMPro.TextMeshProUGUI reviveCountText;
        [SerializeField] Button refuseButton;
        [SerializeField] Button closeButton;

        float reviveCount = 0;
        float maxRevive = 3;

        public static Action OnChooseRevive;
        public static Action OnChooseRefuse;

        private void Awake()
        {
            LevelSphere.OnTouchDeathline += Show_RevivePanel;
            closeButton.onClick.AddListener(Refuse);
            refuseButton.onClick.AddListener(Refuse);
        }

        private void OnDestroy()
        {
            LevelSphere.OnTouchDeathline -= Show_RevivePanel;
        }
        void Start()
        {
            blackImage.SetActive(false);
            blockImage.SetActive(false);
            revivePanelTrans.gameObject.SetActive(false);
            reviveButton.onClick.AddListener(Revive);
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Show_RevivePanel();
            }
        }
#endif
        void Show_RevivePanel()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            SetupReviveStatus();
            revivePanelTrans.DOScale(0.8f, 0).OnComplete(() =>
            {
                revivePanelTrans.gameObject.SetActive(true);
                revivePanelTrans.DOScale(1, 0.25f).OnComplete(() =>
                {
                    //revivePanelTrans.
                    blockImage.SetActive(false);
                });
            });
            AudioManager.Instance.Play(AudioName.SFX_Lose, false);
            Vibration.Vibrate();
        }
        void Hide_RevivePanel()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            revivePanelTrans.DOScale(0.8f, 0.25f).OnComplete(() =>
            {
                revivePanelTrans.gameObject.SetActive(false);
                blackImage.SetActive(false);
                blockImage.SetActive(false);
                ReviewInGameManager.Ins.AddCount();
            });
        }
        void Revive()
        {
            //watch reward ads
            AdsManager.Instance.ShowRewardVideo(() =>
            {
                Hide_RevivePanel();
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    OnChooseRevive?.Invoke();
                });
                reviveCount++;
            });
        }
        void Refuse()
        {
            Hide_RevivePanel();
            OnChooseRefuse?.Invoke();
        }
        void SetupReviveStatus()
        {
            reviveCountText.text = $"Revive\n{(maxRevive - reviveCount)}/{maxRevive}";
            if (reviveCount == maxRevive)
            {
                reviveButton.interactable = false;
            }
            else
            {
                reviveButton.interactable = true;
            }
        }
    }
}
