using DG.Tweening;
using SCN.BinaryData;
using SCN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCN.FruitSynthesis
{
    public class MainPanelController : MonoBehaviour
    {
        [SerializeField] Button settingButton;
        [SerializeField] Button hammerItemButton;
        [SerializeField] Button dartItemButton;

        [SerializeField] TMPro.TextMeshProUGUI coinText;
        [SerializeField] TMPro.TextMeshProUGUI highScoreText;
        [SerializeField] TMPro.TextMeshProUGUI currentScoreText;
        [SerializeField] TMPro.TextMeshProUGUI hammerCostText;
        [SerializeField] GameObject[] hammerCostImageGO;
        [SerializeField] GameObject dartCostImageGO;

        [SerializeField] Image nextItemImage;
        [SerializeField] Sprite[] itemSprite;

        [SerializeField] GameObject dartPrefab;
        [SerializeField] EventTrigger dartSpawnAreaTrigger;
        [SerializeField] Transform dartStartPos;
        [SerializeField] Transform dartEndPos;

        int hammerCost = 100;
        List<LevelSphere> alive = new();
        float currentScore = 0;

        public static Action OnClickSettingButton;
        public static Action OnClickHammerItemButton;
        public static Action<GameObject[]> OnRevive;
        public static Action OnClickDartItemButton;

        private void Awake()
        {
            Config.OnRandomNextItem += Show_NextItem;
            LevelSphere.OnMerged += HandleMerged;
            LevelSphere.OnSphereDisabled += HandleDisabled;
            RevivePanelController.OnChooseRevive += Callback_Revive;
            LevelSphere.OnSphereEnabled += HandleEnabled;
        }
        private void OnDestroy()
        {
            Config.OnRandomNextItem -= Show_NextItem;
            LevelSphere.OnMerged -= HandleMerged;
            LevelSphere.OnSphereDisabled -= HandleDisabled;
            RevivePanelController.OnChooseRevive -= Callback_Revive;
            LevelSphere.OnSphereEnabled -= HandleEnabled;
        }
        void Start()
        {
            hammerItemButton.onClick.AddListener(Active_HammerItem);
            dartItemButton.onClick.AddListener(Active_DartItem);
            Update_Score();
            Update_Coin();
            settingButton.onClick.AddListener(Call_ShowSettingPanel);
            Master.AddEventTriggerListener(dartSpawnAreaTrigger, EventTriggerType.PointerClick, SpawnDart);
            dartSpawnAreaTrigger.gameObject.SetActive(false);
        }
        void Show_NextItem(int nextFruitValue)
        {
            nextItemImage.sprite = itemSprite[(int)Mathf.Log(nextFruitValue, 2f) - 1];
        }

        void Active_HammerItem()
        {
            if (CurrencyManager.Instance.GetCurrentCoin() >= hammerCost)
            {
                OnClickHammerItemButton?.Invoke();
            }
            else
            {
                //show reward ads
                OnClickHammerItemButton?.Invoke();
            }
        }
        void Active_DartItem()
        {
            //show reward ads
            OnClickDartItemButton?.Invoke();
            dartSpawnAreaTrigger.gameObject.SetActive(true);
        }
        void SpawnDart(BaseEventData data)
        {
            Debug.Log("SpawnDart");
            PointerEventData eventData = data as PointerEventData;
            Vector3 dartSpawnPos = new Vector3(dartStartPos.position.x, eventData.position.y, transform.position.z);
            GameObject dartGO = Instantiate(dartPrefab, dartSpawnPos, Quaternion.identity, dartSpawnAreaTrigger.transform);
            dartGO.transform.DOMoveX(dartEndPos.position.x, 0.15f).OnComplete(() =>
            {
                Destroy(dartGO);
            });
            DOVirtual.DelayedCall(0.16f, () =>
            {
                dartSpawnAreaTrigger.gameObject.SetActive(false);
            });
        }
        void Call_ShowSettingPanel()
        {
            OnClickSettingButton?.Invoke();
        }
        void AddScore(float totalReward)
        {
            currentScore += totalReward;
            Update_Score();
            Update_Coin();
        }
        void Update_Score()
        {
            currentScoreText.text = currentScore.ToString();
            if (currentScore > LocalDataManager.Instance.UserLocalData.HighScore)
            {
                LocalDataManager.Instance.UserLocalData.HighScore = (int)currentScore;
                LocalDataManager.Instance.SaveLocalData();
            }
            highScoreText.text = LocalDataManager.Instance.UserLocalData.HighScore.ToString();
        }

        private void Update_Coin()
        {
            coinText.text = CurrencyManager.Instance.GetCurrentCoin().ToString();
        }
        void HandleEnabled(LevelSphere s)
        {
            if (s == null) return;
            if (!alive.Contains(s)) alive.Add(s);
        }
        void HandleMerged(LevelSphere winner, float oldV, float newV, LevelSphere removed)
        {
            // removed will be unspawn -> remove from list
            alive.Remove(removed);
            if (!alive.Contains(winner)) alive.Add(winner); // usually already inside
                                                            // winner value changed -> no need remove/add, only re-evaluate top3 when needed

            CurrencyManager.Instance.AddCoin((int)oldV, currencyEarnSourceType.merge);
            AddScore(oldV);
        }

        void HandleDisabled(LevelSphere s)
        {
            alive.Remove(s);
        }
        List<LevelSphere> GetTop3() =>
          alive.Where(x => x != null && x.gameObject.activeInHierarchy)
               .OrderByDescending(x => x.SphereNum)
               .Take(3).ToList();

        void Callback_Revive()
        {
            var top3 = GetTop3();

            int totalReward = 0;

            foreach (var s in top3)
            {
                if (s == null) continue;

                // Reward rule: same as "merge reward"
                // Example: SphereNum 2->1, 4->2, 8->4 => reward = SphereNum / 2
                int reward = Mathf.Max(1, (int)(s.SphereNum / 2));
                totalReward += reward;

                // Remove from alive first to avoid late events re-adding it
                alive.Remove(s);

                // Remove fruit from scene (pooled)
                ObjectPool.Instance.Unspawn(s.gameObject);
            }

            // Add coin
            CurrencyManager.Instance.AddCoin(totalReward, currencyEarnSourceType.merge);
            AddScore(totalReward);
        }
    }
}