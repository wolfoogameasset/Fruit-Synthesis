using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace SCN.Animation
{
	public class AnimationSpineController : MonoBehaviour
	{
		/// <summary>
		/// Event nay xay ra khi ket thuc animation, va xay ra truoc khi onComplete.
		/// Vay nen, khong nen cung goi 1 luc 1 animation nao do trong 2 event nay
		/// </summary>
		public System.Action<string, bool> OnEndAnim;
		public System.Action<string, int> OnEvent;
		public System.Action<string> OnStartAnim;

		SkeletonAnimation skeletonAnimation;
		SkeletonGraphic skeletonGraphic;

		Spine.AnimationState animationState;
		Skeleton skeleton;
		[SerializeField] bool isPause;
		[SerializeField] bool loop;

		Coroutine eventAnimCorou;
		Coroutine endAnimCorou;

		[SerializeField] string currentAnim;
		[SerializeField] float timeScaleDefault = 1;

		[SerializeField] MonoBehaviour mono;
		bool isInit = false;

		public Spine.AnimationState AnimationState => animationState;
		public Skeleton Skeleton => skeleton;

		public void InitValue(MonoBehaviour mono = null)
		{
			if (isInit) return;
			isInit = true;

			if (TryGetComponent(out skeletonAnimation))
			{
				animationState = skeletonAnimation.AnimationState;
				skeleton = skeletonAnimation.Skeleton;
			}
			else if (TryGetComponent(out skeletonGraphic))
			{
				animationState = skeletonGraphic.AnimationState;
				skeleton = skeletonGraphic.Skeleton;
			}

			isPause = false;

			animationState.Start += delegate
			{
				skeleton.SetToSetupPose();
			};

			currentAnim = "";
			if (mono == null)
			{
				this.mono = this;
			}
			else
			{
				this.mono = mono;
			}
		}

		public void PlayAnimation(SpineAnim spineAnim, bool isLoop
			, bool hasEvent = false, System.Action onComplete = null)
		{
			if (!isInit) return;

			StopCurrentAnimation();

			currentAnim = spineAnim.animName;
			loop = isLoop;

			var anim = animationState.SetAnimation(0, spineAnim.animName, loop);
			OnStartAnim?.Invoke(spineAnim.animName);

			if (hasEvent)
			{
				eventAnimCorou = mono.StartCoroutine(WaitEventAnim(spineAnim.eventTime, spineAnim.animName));
			}

			if (!loop)
			{
				endAnimCorou = mono.StartCoroutine(WaitEventEnd(
					anim.Animation.Duration, spineAnim.animName, onComplete));
			}
		}

		public void StopCurrentAnimation()
		{
			if (!isInit) return;

			if (currentAnim != "") // Interrupt
			{
				if (eventAnimCorou != null) mono.StopCoroutine(eventAnimCorou);
				if (endAnimCorou != null) mono.StopCoroutine(endAnimCorou);

				OnEndAnim?.Invoke(currentAnim, true);
			}

			currentAnim = "";
			//animationState.ClearTracks();
		}

		public void SetSkin(string skin)
		{
			skeleton.SetSkin(skin);
			skeleton.SetSlotsToSetupPose();
			skeletonGraphic.LateUpdate();
		}

		IEnumerator WaitEventAnim(float[] animTime, string animName)
		{
			for (int i = 0; i < animTime.Length; i++)
			{
				float waitTime;
				if (i == 0) waitTime = animTime[0];
				else waitTime = animTime[i] - animTime[i - 1];

				var timer = 0f;
				while (true)
				{
					if (!isPause) timer += Time.deltaTime;

					if (timer >= waitTime)
					{
						OnEvent?.Invoke(animName, i);
						break;
					}
					else
						yield return null;
				}
			}
		}

		IEnumerator WaitEventEnd(float animTime, string animName, System.Action onComplete)
		{
			var timer = 0f;

			while (true)
			{
				if (!isPause)
					timer += Time.deltaTime;

				if (timer >= animTime)
				{
					currentAnim = "";

					animationState.ClearTracks();
					OnEndAnim?.Invoke(animName, false);

					break;
				}
				else
					yield return null;
			}

			onComplete?.Invoke();
		}

		public void PauseAnim(bool _isPause)
		{
			isPause = _isPause;
			animationState.TimeScale = _isPause ? 0 : timeScaleDefault;
		}

		public void SetAnimOrder(string layerName, int sortingOrder)
		{
			GetComponent<MeshRenderer>().sortingLayerName = layerName;
			GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
		}

		public void SetAnimOrder(string layerName)
		{
			GetComponent<MeshRenderer>().sortingLayerName = layerName;
		}

		[System.Serializable]
		public class SpineAnim
		{
			[SpineAnimation]
			public string animName = "";
			public float[] eventTime;
		}
		
		
		[System.Serializable]
		public class SkinAnim
		{
			[SpineSkin()]
			public string skinName = "";
		}
	}
}