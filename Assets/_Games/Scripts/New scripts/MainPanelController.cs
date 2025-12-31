using DG.Tweening;
using SCN.Ads;
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
        [SerializeField] TMPro.TextMeshProUGUI usingSkillText;

        [SerializeField] Image nextItemImage;
        [SerializeField] Sprite[] itemSprite;

        [SerializeField] GameObject dartPrefab;
        [SerializeField] Transform dartParent;
        [SerializeField] EventTrigger dartSpawnAreaTrigger;
        [SerializeField] Transform dartStartPos;
        [SerializeField] Transform dartEndPos;
        [SerializeField] Transform dartFieldTrans;
        [SerializeField] Transform dartTopLimit;
        [SerializeField] Transform dartBottomLimit;

        [SerializeField] Transform leftWallTrans;
        [SerializeField] Transform rightWallTrans;

        bool isUsingSkill;
        int hammerCost = 100;
        List<LevelSphere> alive = new();
        public float currentScore = 0;

        float canvasScale = 1;
        public float leftWallPosX = 0;
        public float rightWallPosX = 0;

        public static Action OnClickSettingButton;
        public static Action OnClickHammerItemButton;
        public static Action<GameObject[]> OnRevive;
        public static Action OnClickDartItemButton;
        public static Action OnFinishUsingDart;

        public static MainPanelController Instance;

        private void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Config.OnRandomNextItem += Show_NextItem;
            LevelSphere.OnMerged += HandleMerged;
            //LevelSphere.OnSphereEnabled += HandleEnabled;
            MainPanel.OnDropSphere += HandleEnabled;
            LevelSphere.OnSphereDisabled += HandleDisabled;
            LevelSphere.OnSphereDestroyBySkill += AddScore;
            RevivePanelController.OnChooseRevive += Callback_Revive;
        }
        private void OnDestroy()
        {
            Config.OnRandomNextItem -= Show_NextItem;
            LevelSphere.OnMerged -= HandleMerged;
            //LevelSphere.OnSphereEnabled -= HandleEnabled;
            MainPanel.OnDropSphere -= HandleEnabled;
            LevelSphere.OnSphereDisabled -= HandleDisabled;
            LevelSphere.OnSphereDestroyBySkill -= AddScore;
            RevivePanelController.OnChooseRevive -= Callback_Revive;
        }
        void Start()
        {
            leftWallPosX = leftWallTrans.position.x;
            rightWallPosX = rightWallTrans.position.x;

            hammerItemButton.onClick.AddListener(Active_HammerItem);
            dartItemButton.onClick.AddListener(Active_DartItem);
            settingButton.onClick.AddListener(Call_ShowSettingPanel);

            Master.AddEventTriggerListener(dartSpawnAreaTrigger, EventTriggerType.PointerClick, SpawnDart);
            Master.AddEventTriggerListener(dartSpawnAreaTrigger, EventTriggerType.PointerDown, OnBeginDragLine);
            Master.AddEventTriggerListener(dartSpawnAreaTrigger, EventTriggerType.Drag, OnDragLine);
            Master.AddEventTriggerListener(dartSpawnAreaTrigger, EventTriggerType.EndDrag, OnEndDragLine);

            dartSpawnAreaTrigger.gameObject.SetActive(false);
            dartFieldTrans.gameObject.SetActive(false);
            usingSkillText.gameObject.SetActive(false);
            hammerCostText.text = hammerCost.ToString();
            Update_Score();
            Update_Coin();
        }
        void Show_NextItem(int nextFruitValue)
        {
            nextItemImage.sprite = itemSprite[(int)Mathf.Log(nextFruitValue, 2f) - 1];
        }

        void Active_HammerItem()
        {
            if (isUsingSkill) return;
            if (alive.Count == 0) return;

            if (CurrencyManager.Instance.GetCurrentCoin() >= hammerCost)
            {
                isUsingSkill = true;
                CurrencyManager.Instance.RemoveCoin(hammerCost, currencySpendSource.hammer);
                OnClickHammerItemButton?.Invoke();
                Show_SkillText(true);
            }
            else
            {
                //show reward ads
                AdsManager.Instance.ShowRewardVideo(() =>
                {
                    isUsingSkill = true;
                    OnClickHammerItemButton?.Invoke();
                    Show_SkillText(true);
                });
            }
        }
        void Active_DartItem()
        {
            //show reward ads
            if (isUsingSkill) return;
            if (alive.Count == 0) return;

            AdsManager.Instance.ShowRewardVideo(() =>
            {
                isUsingSkill = true;
                OnClickDartItemButton?.Invoke();
                dartSpawnAreaTrigger.gameObject.SetActive(true);
                dartFieldTrans.transform.position = new Vector3(dartFieldTrans.position.x, 0, transform.position.z);
                dartFieldTrans.gameObject.SetActive(true);
                Show_SkillText(false);
            });
        }

        void OnBeginDragLine(BaseEventData data)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var limitedPosY = Math.Clamp(mousePos.y, dartBottomLimit.position.y, dartTopLimit.position.y);
            dartFieldTrans.transform.position = new Vector3(dartFieldTrans.position.x, limitedPosY, transform.position.z);
        }
        void OnDragLine(BaseEventData data)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var limitedPosY = Math.Clamp(mousePos.y, dartBottomLimit.position.y, dartTopLimit.position.y);
            dartFieldTrans.transform.position = new Vector3(dartFieldTrans.position.x, limitedPosY, transform.position.z);
        }
        void OnEndDragLine(BaseEventData data)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        void SpawnDart(BaseEventData data)
        {
            float dartDuration = 0.15f;
            float canvasScale = Master.GetTopmostCanvas(this).transform.localScale.x;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var limitedPosY = Math.Clamp(mousePos.y, dartBottomLimit.position.y, dartTopLimit.position.y);
            Vector3 dartSpawnPos = new Vector3(dartStartPos.position.x, limitedPosY, transform.position.z);
            GameObject dartGO = Instantiate(dartPrefab, dartSpawnPos, Quaternion.identity, dartParent);
            dartSpawnAreaTrigger.gameObject.SetActive(false);
            usingSkillText.gameObject.SetActive(false);
            dartGO.transform.DOMoveX(dartEndPos.position.x, dartDuration).OnComplete(() =>
            {
                Destroy(dartGO);
            }).SetEase(Ease.Linear);
            DOVirtual.DelayedCall(dartDuration, () =>
            {
                dartFieldTrans.gameObject.SetActive(false);
                isUsingSkill = false;
                OnFinishUsingDart?.Invoke();
            });
        }
        void Call_ShowSettingPanel()
        {
            OnClickSettingButton?.Invoke();
        }
        void AddScore(float totalReward, bool isDestroyByHammer = false)
        {
            if (isDestroyByHammer)
            {
                isUsingSkill = false;
                usingSkillText.gameObject.SetActive(false);
            }
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
            if (CurrencyManager.Instance.GetCurrentCoin() >= hammerCost)
            {
                hammerCostImageGO[0].SetActive(true);
                hammerCostImageGO[1].SetActive(false);
            }
            else
            {
                hammerCostImageGO[0].SetActive(false);
                hammerCostImageGO[1].SetActive(true);
            }

        }
        void HandleEnabled(Transform droppedSphere)
        {
            LevelSphere s = droppedSphere.GetComponent<LevelSphere>();
            if (s == null) return;
            if (!alive.Contains(s)) alive.Add(s);
        }
        void HandleMerged(LevelSphere winner, float oldV, float newV, LevelSphere removed)
        {
            // removed will be unspawn -> remove from list
            alive.Remove(removed);
            if (!alive.Contains(winner)) alive.Add(winner); // usually already inside
                                                            // winner value changed -> no need remove/add, only re-evaluate top3 when needed

            CurrencyManager.Instance.AddCoin((int)oldV/2, currencyEarnSourceType.merge);
            AddScore(oldV/2);
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
            if (alive.Count <= 3) return;
            var top3 = GetTop3();

            Debug.Log(top3.Count);

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
            CurrencyManager.Instance.AddCoin(totalReward/2, currencyEarnSourceType.merge);
            AddScore(totalReward / 2);
        }
        void Show_SkillText(bool isUsingHammer)
        {
            usingSkillText.gameObject.SetActive(true);
            if (isUsingHammer)
            {
                usingSkillText.text = "Select A Character To Remove";
            }
            else 
            {
                usingSkillText.text = "Select An Area To Remove";
            }
        }
    }

}