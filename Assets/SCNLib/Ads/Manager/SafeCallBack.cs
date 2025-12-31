using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Ads
{
    public abstract class SafeCallBack : MonoBehaviour
    {
		protected readonly Queue<Action> safeCallback = new Queue<Action>();
		
		protected virtual void Update()
		{
			while (safeCallback.Count > 0)
			{
				Action action = null;
				lock (safeCallback)
				{
					action = safeCallback.Dequeue();
				}
				action?.Invoke();
			}
		}

		protected void SafeCallback(Action callback)
		{
			if (callback != null)
			{
				safeCallback.Enqueue(callback);
			}
		}

		public void DelayCallback(float delayTime, Action callback)
		{
			if (callback != null)
			{
				if (delayTime == 0f)
				{
					SafeCallback(callback);
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(IEDelayCallback(delayTime, callback));
				}
			}
		}
		IEnumerator IEDelayCallback(float delayTime, Action callback)
		{
			yield return (object)new WaitForSecondsRealtime(delayTime);
			callback?.Invoke();
		}
	}
}