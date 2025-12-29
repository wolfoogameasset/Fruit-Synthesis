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
            reviveButton.onClick.AddListener(Revive);
        }
        void Show_RevivePanel()
        {

        }

        void Revive()
        {
            //watch reward ads
            OnChooseRevive?.Invoke();
        }
    }
}
