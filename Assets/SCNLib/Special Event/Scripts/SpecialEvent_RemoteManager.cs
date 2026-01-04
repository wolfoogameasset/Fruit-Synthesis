using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Firebase.Extensions;
using SCN.FirebaseLib.FA;
using System;

namespace SCN.SpecialEvent
{
    public class SpecialEvent_RemoteManager : MonoBehaviour
    {
        [Header("Custom Variables")]
        // dieu chinh tuy theo set up layer cua tro choi, hay de y ca setup layer cua parent gate trong iap manager
        [SerializeField] RenderMode _renderMode = RenderMode.ScreenSpaceCamera;
        [SerializeField] int _sortingOrder = 10;

        // moi khi co event moi thi them popup custom cua event vao day
        [SerializeField] GameObject[] _eventPopupGO;

        [Header("Don't need to Custom this variables")] 
        [SerializeField] Canvas _thisCanvas;
        public static SpecialEvent_RemoteManager Instance;
        public int Event_ID;
        public string Event_URL;

        GameObject currentEventGO;
        const string _eventKey_ID = "event_id";
        const string _eventKey_URL = "event_url";

        // goi bat tat popup tu cac script khac bang cach invoke action
        public static Action<int> OnCompleteCheckEventID;
        public static Action OnShowPopup;
        public static Action OnHidePopup;

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
            DontDestroyOnLoad(gameObject);
            OnShowPopup += SetLayerOrder;
        }
        private void OnDestroy()
        {
            OnShowPopup -= SetLayerOrder;
        }
        private void Start()
        {
            GAManager.Instance.LogEvent("check_special_event");

            DOVirtual.DelayedCall(1.5f, () =>
            {
                FetchDataAsync();
            });
        }
        Task FetchDataAsync()
        {
            Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        void FetchComplete(Task fetchTask)
        {
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task =>
            {

            });
            Event_ID = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(_eventKey_ID).LongValue;
            Event_URL = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(_eventKey_URL).StringValue;

            Debug.Log($"Fetch: {Event_ID} // {Event_URL}" );
            UpdateStatus();
        }

        void UpdateStatus()
        {
            OnCompleteCheckEventID?.Invoke(Event_ID);
            if (Event_ID != 0)
            {
                currentEventGO = Instantiate(_eventPopupGO[Event_ID - 1], transform);
            }
        }
        void SetLayerOrder()
        {
            _thisCanvas.renderMode = _renderMode;
            _thisCanvas.worldCamera = Camera.main;
            _thisCanvas.sortingOrder = _sortingOrder;
        }
    }
}
