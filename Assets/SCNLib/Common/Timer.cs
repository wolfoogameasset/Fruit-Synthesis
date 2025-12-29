using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Common
{
    [System.Serializable]
    public class Timer
    {
        public System.Action OnStart;
        public System.Action<int> OnTimeChange;
        public System.Action OnDone;

        public bool PauseClock = false;
        public bool IsRunning = false;

        MonoBehaviour _mono;
        Coroutine _corou;

        [SerializeField] int currentTime = 0;
        public int CurrentTime => currentTime;

        public Timer(MonoBehaviour mono)
        {
            _mono = mono;
            PauseClock = false;
            IsRunning = false;
        }

        public void Start(int _time, bool isUsingUnScaleTime)
        {
            Stop();
            _corou = _mono.StartCoroutine(StartClockDownCorou(_time, isUsingUnScaleTime));
        }

        public void Stop(bool callDone = false)
        {
            IsRunning = false;
            currentTime = 0;

            if (_corou != null && _mono != null)
                _mono.StopCoroutine(_corou);

            if (callDone) OnDone?.Invoke();
        }

        IEnumerator StartClockDownCorou(int _time, bool isUsingUnScaleTime)
        {
            IsRunning = true;
            PauseClock = false;

            OnStart?.Invoke();

            currentTime = _time;
            OnTimeChange?.Invoke(currentTime);

            while (currentTime > 0)
            {
                if (isUsingUnScaleTime)
                    yield return new WaitForSecondsRealtime(1);
                else
                    yield return new WaitForSeconds(1);

				if (PauseClock)
				{
                    yield return new WaitUntil(() => !PauseClock);

                    if (isUsingUnScaleTime)
                        yield return new WaitForSecondsRealtime(1);
                    else
                        yield return new WaitForSeconds(1);
                }

                currentTime--;
                OnTimeChange?.Invoke(currentTime);
            }

            IsRunning = false;
            OnDone?.Invoke();
        }
    }
}