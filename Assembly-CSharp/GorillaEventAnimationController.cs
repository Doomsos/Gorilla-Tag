using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class GorillaEventAnimationController : MonoBehaviour
{
	[Serializable]
	public struct AnimToGEAKeyframeData
	{
		public AnimationClip clip;

		public List<GEAKeyframeData> gEAKeyframeData;
	}

	[Serializable]
	public struct GEAKeyframeData
	{
		public GorillaEventAnimation gEA;

		public List<ControlledAnimationKeyframeData> keyframeData;
	}

	[Serializable]
	public struct ControlledAnimationKeyframeData(int _index, float _time, float _startOffset, bool _animEnabled)
	{
		public int animationClipIndex = _index;

		public float startTime = _time;

		public float startOffset = _startOffset;

		public bool animEnabled = _animEnabled;
	}

	public Animation controllingAnimation;

	public bool playAnimation;

	private float lateStart;

	public int animationClipIndex;

	public List<AnimationClip> clips;

	private AnimationClip currentClip;

	private Dictionary<AnimationClip, Dictionary<GorillaEventAnimation, List<ControlledAnimationKeyframeData>>> bakedAnimationData;

	[SerializeField]
	[HideInInspector]
	private List<AnimToGEAKeyframeData> bakedAnimKeyframeData;

	private void Awake()
	{
		bakedAnimationData = new Dictionary<AnimationClip, Dictionary<GorillaEventAnimation, List<ControlledAnimationKeyframeData>>>();
		for (int i = 0; i < bakedAnimKeyframeData.Count; i++)
		{
			AnimationClip clip = bakedAnimKeyframeData[i].clip;
			bakedAnimationData.Add(clip, new Dictionary<GorillaEventAnimation, List<ControlledAnimationKeyframeData>>());
			for (int j = 0; j < bakedAnimKeyframeData[i].gEAKeyframeData.Count; j++)
			{
				bakedAnimationData[clip].Add(bakedAnimKeyframeData[i].gEAKeyframeData[j].gEA, bakedAnimKeyframeData[i].gEAKeyframeData[j].keyframeData);
			}
		}
	}

	private void Update()
	{
		AnimationState animationState = null;
		foreach (AnimationState item in controllingAnimation)
		{
			if (item.weight == 1f)
			{
				animationState = item;
				currentClip = item.clip;
				break;
			}
		}
		if (!playAnimation)
		{
			if (controllingAnimation.isPlaying)
			{
				controllingAnimation.Stop();
			}
			return;
		}
		if (!controllingAnimation.enabled)
		{
			controllingAnimation.enabled = true;
		}
		if (currentClip != clips[animationClipIndex] || animationState == null || !controllingAnimation.isPlaying)
		{
			currentClip = clips[animationClipIndex];
			currentClip.legacy = true;
			while (lateStart > 0f && currentClip.length < lateStart && animationClipIndex < clips.Count - 1)
			{
				lateStart -= currentClip.length;
				currentClip = clips[++animationClipIndex];
				currentClip.legacy = true;
			}
			controllingAnimation.Play(currentClip.name);
			animationState = controllingAnimation[currentClip.name];
			animationState.time = Math.Min(lateStart, currentClip.length);
			lateStart = 0f;
		}
		float time = animationState.time;
		if (!bakedAnimationData.ContainsKey(currentClip))
		{
			return;
		}
		foreach (KeyValuePair<GorillaEventAnimation, List<ControlledAnimationKeyframeData>> item2 in bakedAnimationData[currentClip])
		{
			GorillaEventAnimation key = item2.Key;
			List<ControlledAnimationKeyframeData> value = item2.Value;
			for (int i = 0; i < value.Count; i++)
			{
				if (value.Count < 2 || i == value.Count - 1 || !(time >= value[i].startTime) || !(time >= value[i + 1].startTime))
				{
					bool flag = false;
					int num = value[i].animationClipIndex;
					float startTime = time - value[i].startTime + value[i].startOffset;
					if (key.enabled != value[i].animEnabled)
					{
						key.enabled = value[i].animEnabled;
						flag = value[i].animEnabled;
					}
					if (value[i].animEnabled && (key._clipIndex != value[i].animationClipIndex || !key._animation.IsPlaying(key.clips[num].name)))
					{
						flag = true;
					}
					if (flag)
					{
						key.PlayClipByIndex(value[i].animationClipIndex, startTime);
					}
					break;
				}
			}
		}
	}

	public void SetPlayState(bool isPlaying)
	{
		playAnimation = isPlaying;
	}

	public void SetAnimationClip(int clipIndex)
	{
		animationClipIndex = clipIndex;
	}

	public void StartPlaying(float secondsPast)
	{
		lateStart = secondsPast;
		animationClipIndex = 0;
		playAnimation = true;
	}

	public void StartPlaying()
	{
		StartPlaying(0f);
	}
}
