using DG.Tweening;
using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.IAP
{
	public class NoticeText : MonoBehaviour
	{
		MonoBehaviour _mono;
		Coroutine _corou;

		[SerializeField] Image _bg;
		[SerializeField] Text _text;

		const float _animTime = 0.3f;
		const string _incorrectAnswerSt = "Incorrect answer!";
		const string _tryLaterSt = "Wrong answer too many times. Please try again later!";

		public void Setup(MonoBehaviour mono)
		{
			_mono = mono;

			gameObject.SetActive(false);
			_text.gameObject.SetActive(false);
		}

		public void ShowIncorrect()
		{
			Show(_incorrectAnswerSt);
		}

		public void ShowTryLater()
		{
			Show(_tryLaterSt);
		}

		#region
		void Show(string st)
		{
			// Tat di neu dang bat
			Close(false);
			ShowAnim(st);

			_corou = _mono.StartCoroutine(DelayCallMaster.WaitAndDoIE(2, ()=> 
			{
				Close(true);
			}));
		}

		void ShowAnim(string st)
		{
			_bg.gameObject.SetActive(true);
			var _bgColor = _bg.color;
			_bgColor.a = 0;
			_bg.color = _bgColor;
			_bg.DOFade(1, _animTime);

			_text.text = st;
			_text.gameObject.SetActive(true);
			var _textColor = _text.color;
			_textColor.a = 0;
			_text.color = _textColor;
			_text.DOFade(1, _animTime);
		}

		void Close(bool fadeOut)
		{
			if (_corou != null)
			{
				_mono.StopCoroutine(_corou);
			}
				
			_text.DOKill();
			_bg.DOKill();

			if (fadeOut)
			{
				_text.DOFade(0, _animTime);
				_bg.DOFade(0, _animTime).OnComplete(()=> 
				{
					_text.gameObject.SetActive(false);
					_bg.gameObject.SetActive(false);
				});
			}
			else
			{
				_text.gameObject.SetActive(false);
				_bg.gameObject.SetActive(false);
			}
		}
		#endregion
	}
}