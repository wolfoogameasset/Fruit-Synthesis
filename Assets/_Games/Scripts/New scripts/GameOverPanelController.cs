using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.FruitSynthesis
{
    public class GameOverPanelController : MonoBehaviour
    {
        [SerializeField] GameObject blackImage;
        [SerializeField] GameObject blockImage;
        [SerializeField] Transform gameOverPopupTrans;
        [SerializeField] Button restartButton;
        [SerializeField] TMPro.TextMeshProUGUI scoreText;
        private void Awake()
        {
            RevivePanelController.OnChooseRefuse += Show_GameOverPanel;
        }
        private void OnDestroy()
        {
            RevivePanelController.OnChooseRefuse -= Show_GameOverPanel;
        }
        void Start()
        {
            blackImage.SetActive(false);
            blockImage.SetActive(false);
            gameOverPopupTrans.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }
        void Show_GameOverPanel()
        {
            blackImage.SetActive(true);
            blockImage.SetActive(true);
            scoreText.text = MainPanelController.Instance.currentScore.ToString();
            gameOverPopupTrans.DOScale(0.8f, 0).OnComplete(() =>
            {
                gameOverPopupTrans.gameObject.SetActive(true);
                gameOverPopupTrans.DOScale(1, 0.25f).OnComplete(() =>
                {
                    //revivePanelTrans.
                    blockImage.SetActive(false);
                });
            });
        }
        void RestartGame()
        {
            SceneMgr.GetInstance.SwitchingScene(SceneType.SplashPanel);
            //SceneMgr.GetInstance.SwitchingScene(SceneType.MainPanel);
        }
    }
}
