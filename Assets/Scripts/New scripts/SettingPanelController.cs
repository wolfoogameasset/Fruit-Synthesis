using DG.Tweening;
using SCN.BinaryData;
using SCN.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCN.FruitSynthesis
{
    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] GameObject blackImage;
        [SerializeField] EventTrigger blackImageTrigger;
        [SerializeField] GameObject blockImage;
        [SerializeField] Transform popup;
        [SerializeField] Button closeButton;
        [SerializeField] Button continueButton;
        [SerializeField] Button musicButton;
        [SerializeField] Button soundButton;
        [SerializeField] Button vibrateButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button quitButton;
        [SerializeField] Button policyButton;

        [SerializeField] Sprite[] musicButtonSprite;
        [SerializeField] Sprite[] soundButtonSprite;
        [SerializeField] Sprite[] vibrateButtonSprite;
        private const string policyUrl = "https://wolfoogames.com/privacy-policy.html";
        private const string termsUrl = "https://wolfoogames.com/terms-conditions.html";
        private void Awake()
        {
            MainPanelController.OnClickSettingButton += Show_Popup;
        }
        private void OnDestroy()
        {
            MainPanelController.OnClickSettingButton -= Show_Popup;
        }

        void Start()
        {
            Update_ButtonStatus();
            blackImage.SetActive(false);
            popup.gameObject.SetActive(false);
            blockImage.SetActive(false);

            Master.AddEventTriggerListener(blackImageTrigger, EventTriggerType.PointerDown, Hide_Popup);
            closeButton.onClick.AddListener(() => Hide_Popup(null));
            continueButton.onClick.AddListener(() => Hide_Popup(null));
            musicButton.onClick.AddListener(Set_MusicVolume);
            soundButton.onClick.AddListener(Set_SoundVolume);
            vibrateButton.onClick.AddListener(Set_Vibration);
            policyButton.onClick.AddListener(ShowPolicyPrivacy);
            quitButton.onClick.AddListener(QuitApplication);
            restartButton.onClick.AddListener(RestartGame);
        }

        void Show_Popup()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            popup.DOScale(0.8f, 0).OnComplete(() =>
            {
                popup.gameObject.SetActive(true);
                popup.DOScale(1, 0.25f).OnComplete(() =>
                {
                    blockImage.SetActive(false);
                });
            });
        }
        void Hide_Popup(BaseEventData data)
        {
            blockImage.SetActive(true);
            popup.DOScale(0.8f, 0.25f).OnComplete(() =>
            {
                popup.gameObject.SetActive(false);
                blockImage.SetActive(false);
                blackImage.SetActive(false);
            });
        }
        void RestartGame()
        {
            SceneMgr.GetInstance.SwitchingScene(SceneType.SplashPanel);
            //SceneMgr.GetInstance.SwitchingScene(SceneType.MainPanel);
        }
        void QuitApplication()
        {
            Application.Quit();
        }
        void Set_MusicVolume()
        {
            int musicVolumeToChange = 1 - LocalDataManager.Instance.UserLocalData.MusicVolume;
            AudioManager.Instance.ChangeMusicVolume(musicVolumeToChange);
            LocalDataManager.Instance.UserLocalData.MusicVolume = musicVolumeToChange;
            LocalDataManager.Instance.SaveLocalData();
            Update_ButtonStatus();
        }
        void Set_SoundVolume()
        {
            int soundVolumeToChange = 1 - LocalDataManager.Instance.UserLocalData.SoundVolume;
            AudioManager.Instance.ChangeSoundVolume(soundVolumeToChange);
            LocalDataManager.Instance.UserLocalData.SoundVolume = soundVolumeToChange;
            LocalDataManager.Instance.SaveLocalData();
            Update_ButtonStatus();
        }
        void Set_Vibration()
        {
            LocalDataManager.Instance.UserLocalData.IsVibrate = !LocalDataManager.Instance.UserLocalData.IsVibrate;
            LocalDataManager.Instance.SaveLocalData();
            Update_ButtonStatus();
        }
        void Update_ButtonStatus()
        {
            musicButton.image.sprite = musicButtonSprite[LocalDataManager.Instance.UserLocalData.MusicVolume];
            soundButton.image.sprite = musicButtonSprite[LocalDataManager.Instance.UserLocalData.SoundVolume];
            vibrateButton.image.sprite = vibrateButtonSprite[LocalDataManager.Instance.UserLocalData.IsVibrate ? 1 : 0];
        }

        void ShowPolicyPrivacy()
        {
            OpenTermsAndConditions();
            OpenPrivacyPolicy();
        }
        void OpenTermsAndConditions()
        {
            OpenURLBridge.OpenURL(termsUrl);
        }
        void OpenPrivacyPolicy()
        {
            OpenURLBridge.OpenURL(policyUrl);
        }
    }
}