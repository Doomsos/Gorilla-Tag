using UnityEngine;

[ExecuteInEditMode]
public class GorillaEventAnimation : MonoBehaviour
{
	[SerializeField]
	private Animation _animation;

	public bool controlledAnimation;

	public float offsetTime;

	public int animationClipIndex;

	[Header("Ensure this matches the list of clips on the animation component")]
	public AnimationClip[] clips;

	private int lastClipIndex;

	public bool PreviewAudioInEditMode;

	private void OnEnable()
	{
		if (!controlledAnimation)
		{
			_animation.enabled = true;
			return;
		}
		_animation.enabled = true;
		_animation.clip = clips[animationClipIndex];
		if (_animation.GetClip(clips[animationClipIndex].name) == null)
		{
			_animation.AddClip(clips[animationClipIndex], clips[animationClipIndex].name);
		}
		_animation.clip.legacy = true;
		_animation.Play(clips[animationClipIndex].name);
		_animation[_animation.clip.name].time = offsetTime;
	}

	private void OnDisable()
	{
		if (controlledAnimation)
		{
			_animation.enabled = false;
		}
	}

	private void Update()
	{
		if (controlledAnimation)
		{
			if (animationClipIndex != lastClipIndex)
			{
				OnEnable();
			}
			lastClipIndex = animationClipIndex;
		}
	}

	public void PlayClip(string clipName)
	{
		_animation.Play(clipName);
	}
}
