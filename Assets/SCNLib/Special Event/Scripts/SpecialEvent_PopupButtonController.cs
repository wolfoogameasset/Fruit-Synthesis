using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.SpecialEvent
{
    public class SpecialEvent_PopupButtonController : MonoBehaviour
    {
        [SerializeField] Button _popupButton;
        [SerializeField] ParticleSystem _specialEventParticle;

        //them sprite co button tai day, thu tu tuong ung voi pop up o remote manager
        [SerializeField] Sprite[] _eventButtonSprite;

        private void Awake()
        {
            _popupButton.gameObject.SetActive(false);
            SpecialEvent_RemoteManager.OnCompleteCheckEventID += UpdateButtonStatus;
        }

        private void OnDestroy()
        {
            SpecialEvent_RemoteManager.OnCompleteCheckEventID -= UpdateButtonStatus;
        }

        private void Start()
        {
            _popupButton.gameObject.SetActive(false);
            UpdateButtonStatus(SpecialEvent_RemoteManager.Instance.Event_ID);
        }

        //Sau khi check co event dac biet hay khong thi se an hoac hien nut bam
        void UpdateButtonStatus(int eventID)
        {
            //neu id == default == 0 => khong co event, tat nut special event di
            if (eventID == 0)
            {
                Debug.Log("Default Value " + eventID + "There are not any active event ");
                _popupButton.gameObject.SetActive(false);
            }

            //neu id> 0 => co event, doi sprite cua nut theo id, vi default == 0 nen can lay sprite co id -1
            else if (eventID > 0)
            {
                Debug.Log("Variant " + eventID);

                //neu khong co sprite khac o _eventButtonSprite thi button se khong tu dong doi hinh
                if (_eventButtonSprite.Length != 0)
                {
                    _popupButton.image.sprite = _eventButtonSprite[eventID - 1];
                }
                _popupButton.transform.DOScale(0, 0).OnComplete(() =>
                {
                    _popupButton.gameObject.SetActive(true);
                    _popupButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        _specialEventParticle?.Play();
                        _popupButton.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                    });
                });
                _popupButton.onClick.RemoveAllListeners();
                _popupButton.onClick.AddListener(Call_ShowPopup);
            }
        }
        void Call_ShowPopup()
        {
            SpecialEvent_RemoteManager.OnShowPopup?.Invoke();
        }
    }
}
