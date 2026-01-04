using SCN.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.IAP
{
	public class NoAdsAnim : MonoBehaviour
	{
		[SerializeField] AnimationSpineController.SpineAnim animSmall;
		[SerializeField] AnimationSpineController.SpineAnim animWide;

		public AnimationSpineController.SpineAnim AnimSmall => animSmall;
		public AnimationSpineController.SpineAnim AnimWide => animWide;
	}
}