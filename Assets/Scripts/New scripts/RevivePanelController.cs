using DG.Tweening;
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
        [SerializeField] Button refuseButton;
        [SerializeField] Button closeButton;

        public static Action OnChooseRevive;

        private void Awake()
        {
            LevelSphere.OnTouchDeathline += Show_RevivePanel;
        }

        private void OnDestroy()
        {
            LevelSphere.OnTouchDeathline -= Show_RevivePanel;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            blackImage.SetActive(false);
            blockImage.SetActive(false);
            revivePanelTrans.gameObject.SetActive(false);
            reviveButton.onClick.AddListener(Revive);
        }
        void Show_RevivePanel()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            revivePanelTrans.DOScale(0.8f, 0).OnComplete(() =>
            {
                revivePanelTrans.gameObject.SetActive(true);
                revivePanelTrans.DOScale(1, 0.25f).OnComplete(() =>
                {
                    //revivePanelTrans.
                });
            });
        }
        void Hide_RevivePanel()
        {

        }
        void Revive()
        {
            //watch reward ads
            OnChooseRevive?.Invoke();
        }
    }
}
