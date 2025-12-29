using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SCN.Common
{
	public class DDOL : MonoBehaviour
	{
		/// <summary>
		/// bool: pause
		/// </summary>
		public System.Action<bool> OnApplicationPauseE;
		public System.Action OnApplicationQuitE;
		public System.Action OnUpdateE;

		static DDOL _instance;
		public static DDOL Instance
		{
			get
			{
				Setup();
				return _instance;
			}
		}

		static void Setup()
		{
			if (_instance != null)
			{
				return;
			}

			var obj = new GameObject("DDOL");
			_instance = obj.AddComponent<DDOL>();
			DontDestroyOnLoad(obj);
		}

		public void Preload()
		{

		}

		private void OnApplicationPause(bool pause)
		{
			OnApplicationPauseE?.Invoke(pause);
		}

		private void OnApplicationQuit()
		{
			OnApplicationQuitE?.Invoke();
		}

		private void Update()
		{
			OnUpdateE?.Invoke();
		}
	}
}