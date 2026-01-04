using DG.Tweening;
using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.IAP
{
	public class ParentGateManager : MonoBehaviour
	{
		#region Init
		public static ParentGateManager Instance 
		{
			get 
			{
				if (_instance == null)
				{
					_instance = Instantiate(Resources.Load<GameObject>("Parent gate manager"))
						.GetComponent<ParentGateManager>();
					_instance.Setup();
				}
					
				return _instance;
			}
		}
		static ParentGateManager _instance;

		void Setup()
		{
			// spawn popup theo theme
			_popUp = Instantiate(IAPManager.Instance.ParentGatePopup, transform).transform.GetComponent<ParentGatePopup>();

			// Setup
			_blockArea.SetActive(false);
			_popUp.Dialog.SetActive(false);
			_noticeText.Setup(this);

			//Setup Texture
			//_popUp.PopUpImage.sprite = IAPManager.Instance.ParentGatePopup.ParentGateBackgoundSprite;
			//_popUp.CloseBtn.GetComponent<Image>().sprite = IAPManager.Instance.ParentGatePopup.CloseButtonSprite;
			//for (int i = 0; i < _popUp.Options.childCount; i++)
			//{
			//	_popUp.Options.GetChild(i).GetComponent<Image>().sprite = IAPManager.Instance.ParentGatePopup.ParentGateButtonSprite;
			//}

			// Canvas
			var canvas = GetComponent<Canvas>();
			canvas.worldCamera = Camera.main;
			canvas.sortingLayerName = IAPManager.Instance.SortingLayer;
			canvas.sortingOrder = IAPManager.Instance.OrderInLayer;

			_blockBtn = false;
			_currentRetry = IAPManager.Instance.RetryTime;

			// Add listener
			_popUp.CloseBtn.onClick.RemoveAllListeners();
			_popUp.CloseBtn.onClick.AddListener(()=> 
			{
				if (_blockBtn)
				{
					return;
				}
				CloseDialog();
			});

			for (int i = 0; i < _popUp.Options.childCount; i++)
			{
				var index = i;

                _popUp.Options.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                _popUp.Options.GetChild(i).GetComponent<Button>().onClick.AddListener(()=> 
				{
					if (_blockBtn)
					{
						return;
					}
					OnClickOptions(index);
				});
			}

			timer = new Timer(this)
			{
				OnDone = () =>
				{
					_currentRetry++;
					if (_currentRetry > IAPManager.Instance.RetryTime)
					{
						_currentRetry = IAPManager.Instance.RetryTime;
					}

					CdRetryTime();
				}
			};
		}
		#endregion

		System.Action _onSuccess;

		[SerializeField] GameObject _blockArea;

		[SerializeField] ParentGatePopup _popUp;
		[SerializeField] NoticeText _noticeText;

		int _currentCorrectAnswer;
		bool _blockBtn;

		[SerializeField] int _currentRetry;
		[SerializeField] Timer timer;

		public void OpenDialog(System.Action onSuccess)
		{
            if (_currentRetry == 0)
			{
				_noticeText.ShowTryLater();
				return;
			}

			_onSuccess = onSuccess;

			_blockArea.SetActive(true);
            _popUp.Dialog.SetActive(true);

            _popUp.Dialog.transform.DOKill();
			DOTweenManager.Instance.ScaleToShow(_popUp.Dialog.transform);
            //_popUp.Dialog.transform.localScale = Vector3.one * 0.7f;
            //_popUp.Dialog.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

			RandomQuest();
		}

		void CloseDialog()
		{
			_blockArea.SetActive(false);
            _popUp.Dialog.SetActive(false);
		}

		void RandomQuest()
		{
			var _numb0 = Random.Range(3, 10);
			var _numb1 = Random.Range(3, 10);
			var _answer = _numb0 * _numb1;
			_currentCorrectAnswer = Random.Range(0, _popUp.Options.childCount);

			var _tempList = new List<int>();
			for (int i = 0; i < 10; i++)
			{
				if (i == 5) continue;

				_tempList.Add(_answer - 5 + i);
			}

			var _random = new RandomNoRepeat<int>(_tempList);

			for (int i = 0; i < _popUp.Options.childCount; i++)
			{
                _popUp.Options.GetChild(i).GetChild(0).GetComponent<Text>().text = i == _currentCorrectAnswer ?
					_answer.ToString() : _random.Random().ToString();
			}

			_popUp.CalculationText.text = _numb0 + " x " + _numb1 + " =";
			_popUp.AnswerText.text = "";
		}

		void OnClickOptions(int order)
		{
            _popUp.AnswerText.text = _popUp.Options.GetChild(order).GetChild(0).GetComponent<Text>().text;
			_blockBtn = true;

			_ = StartCoroutine(DelayCallMaster.WaitAndDoIE(0.5f, () =>
			{
				_blockBtn = false;
				if (order == _currentCorrectAnswer)
				{
					_currentRetry = IAPManager.Instance.RetryTime;
					timer.Stop();

					CloseDialog();
					_onSuccess?.Invoke();
				}
				else
				{
					_currentRetry--;
					if (_currentRetry == IAPManager.Instance.RetryTime - 1)
					{
						CdRetryTime();
					}

					if (_currentRetry == 0)
					{
						CloseDialog();
						_noticeText.ShowTryLater();
					}
					else
					{
						_noticeText.ShowIncorrect();
						RandomQuest();
					}
				}
			}));
		}

		void CdRetryTime()
		{
			if (_currentRetry < IAPManager.Instance.RetryTime)
			{
				timer.Start(IAPManager.Instance.NextTryTime, true);
			}
		}
	}
}
