
using UnityEngine;

namespace SCN.Animation 
{
	[RequireComponent(typeof(AnimationSpineController))]
	public class TestAnim : MonoBehaviour
	{
		AnimationSpineController spineControl;
		[SerializeField] AnimationSpineController.SpineAnim anim;
		[SerializeField] KeyCode key;

		private void Start()
		{
			spineControl = GetComponent<AnimationSpineController>();
			spineControl.InitValue();
		}

		private void Update()
		{
			if (Input.GetKeyDown(key))
			{
				spineControl.PlayAnimation(anim, true);
			}
		}
	}
}