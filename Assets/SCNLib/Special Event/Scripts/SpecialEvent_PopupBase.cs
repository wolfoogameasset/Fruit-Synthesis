using DG.Tweening;
using SCN.Common;
using SCN.FirebaseLib.FA;
//using SCN.IAP;
using System.Collections;
using System.Collections.Generic;
//using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.SpecialEvent
{
    public class SpecialEvent_PopupBase : MonoBehaviour
    {
        [SerializeField] string _url;
        [SerializeField] Transform _popupTrans;
        [SerializeField] Button _closeButton;
        [SerializeField] Button _openLinkButton;

        protected virtual void Awake()
        {
            SpecialEvent_RemoteManager.OnShowPopup += ShowPopup;
            SpecialEvent_RemoteManager.OnHidePopup += HidePopup;
        }

        protected virtual void OnDestroy()
        {
            SpecialEvent_RemoteManager.OnShowPopup -= ShowPopup;
            SpecialEvent_RemoteManager.OnHidePopup -= HidePopup;
        }
        protected virtual void Start()
        {
            _closeButton.onClick.AddListener(HidePopup);
            _openLinkButton.onClick.AddListener(OpenLink);
            gameObject.SetActive(false);
        }

        protected virtual void ShowPopup()
        {
            _url = SpecialEvent_RemoteManager.Instance.Event_URL;
            gameObject.SetActive(true);
            DOTweenManager.Instance.ScaleToShow(_popupTrans).OnComplete(() =>
            {
                GAManager.Instance.LogEvent("specialevent_showpopup", new Firebase.Analytics.Parameter("url", _url));
            });
        }
        protected virtual void HidePopup()
        {
            DOTweenManager.Instance.ScaleToHide(_popupTrans).OnComplete(() =>
            {
                GAManager.Instance.LogEvent("specialevent_hidepopup", new Firebase.Analytics.Parameter("url", _url));
                gameObject.SetActive(false);
            });
        }
        protected virtual void OpenLink()
        {
            GAManager.Instance.LogEvent("specialevent_showparentgate", new Firebase.Analytics.Parameter("url", _url));
            //ParentGateManager.Instance.OpenDialog(() =>
            //{
            //    Application.OpenURL(_url);
            //    GAManager.Instance.LogEvent("specialevent_openlink", new Firebase.Analytics.Parameter("url", _url));
            //    HidePopup();
            //});
        }
    }
}