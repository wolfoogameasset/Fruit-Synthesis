using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Common
{
	public static class InternetManager
	{
		static IEnumerator ieWaitInternet;
		public static bool IsInternetAvailable => (int)Application.internetReachability > 0;

		public static void WaitInternet(MonoBehaviour monoBehaviour, Action callback)
		{
			monoBehaviour.StartCoroutine(IEWaitInternet(callback));
		}

		public static IEnumerator IEWaitInternet(Action callback)
		{
			if (ieWaitInternet == null)
			{
				ieWaitInternet = (IEnumerator)new WaitUntil(() => IsInternetAvailable);
			}
			yield return ieWaitInternet;
			callback?.Invoke();
		}
	}
}