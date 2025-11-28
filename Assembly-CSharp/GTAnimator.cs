using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class GTAnimator : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000ECF RID: 3791 RVA: 0x0004ECA3 File Offset: 0x0004CEA3
	public Animation animationComponent
	{
		get
		{
			return this.m_animationComponent;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000ED0 RID: 3792 RVA: 0x0004ECAB File Offset: 0x0004CEAB
	// (set) Token: 0x06000ED1 RID: 3793 RVA: 0x0004ECB3 File Offset: 0x0004CEB3
	public bool hasAnimationComponent { get; private set; }

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0004ECBC File Offset: 0x0004CEBC
	protected void Awake()
	{
		this.Init();
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0004ECC4 File Offset: 0x0004CEC4
	public void Init()
	{
		if (this._wasInitCalled)
		{
			return;
		}
		this._wasInitCalled = true;
		this.hasAnimationComponent = (this.m_animationComponent != null);
		bool hasAnimationComponent = this.hasAnimationComponent;
		this.m_animationMap.Init();
		foreach (GTAnimator.AnimClipAndGObjs animClipAndGObjs in this.m_animationMap.Values)
		{
			this._allStaticGobjs.UnionWith(animClipAndGObjs.endStaticGameObjects);
		}
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEnable()
	{
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x0004ED54 File Offset: 0x0004CF54
	public bool IsPlaying
	{
		get
		{
			return this.m_animationComponent.isPlaying;
		}
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0004ED61 File Offset: 0x0004CF61
	public void SetState(long enumValueAsLong)
	{
		if (!this._wasInitCalled)
		{
			this.Init();
		}
		if (this._currentStateAsLong != enumValueAsLong)
		{
			this.TryPlay(enumValueAsLong);
		}
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0004ED84 File Offset: 0x0004CF84
	public bool TryPlay(long enumValueAsLong)
	{
		GTAnimator.AnimClipAndGObjs animClipAndGObjs;
		if (!this.hasAnimationComponent || !this.m_animationMap.TryGet(enumValueAsLong, out animClipAndGObjs))
		{
			return false;
		}
		foreach (GameObject gameObject in this._allStaticGobjs)
		{
			gameObject.SetActive(false);
		}
		GameObject[] animatedGameObjects = this.m_animatedGameObjects;
		for (int i = 0; i < animatedGameObjects.Length; i++)
		{
			animatedGameObjects[i].SetActive(true);
		}
		this._currentStateAsLong = enumValueAsLong;
		this.m_animationComponent.clip = animClipAndGObjs.animClip;
		this.m_animationComponent.Play();
		if (animClipAndGObjs.soundBankToPlayOnStart)
		{
			animClipAndGObjs.soundBankToPlayOnStart.Play();
		}
		if (!animClipAndGObjs.animClip.isLooping)
		{
			this._frameCountWhenLastPlayed = Time.frameCount;
			GTDelayedExec.Add(this, animClipAndGObjs.animClip.length, this._frameCountWhenLastPlayed);
		}
		return true;
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0004EE7C File Offset: 0x0004D07C
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (!base.enabled || this._frameCountWhenLastPlayed != contextId)
		{
			return;
		}
		this.m_animationComponent.Stop();
		for (int i = 0; i < this.m_animatedGameObjects.Length; i++)
		{
			if (this.m_animatedGameObjects[i] != null)
			{
				this.m_animatedGameObjects[i].SetActive(false);
			}
		}
		GTAnimator.AnimClipAndGObjs animClipAndGObjs;
		GameObject[] array;
		if (this.m_animationMap.TryGet(this._currentStateAsLong, out animClipAndGObjs) && animClipAndGObjs.endStaticGameObjects != null && animClipAndGObjs.endStaticGameObjects.Length != 0)
		{
			array = animClipAndGObjs.endStaticGameObjects;
		}
		else
		{
			array = this.m_defaultStaticGameObjects;
		}
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null)
				{
					array[j].SetActive(true);
				}
			}
		}
		if (this._queuedStateAsLong != -9223372036854775808L)
		{
			long queuedStateAsLong = this._queuedStateAsLong;
			this._queuedStateAsLong = long.MinValue;
			this.TryPlay(queuedStateAsLong);
		}
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0004EF63 File Offset: 0x0004D163
	public void Stop()
	{
		if (this.m_animationComponent != null)
		{
			this.m_animationComponent.Stop();
		}
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0004EF80 File Offset: 0x0004D180
	public void QueueState(long enumValueAsLong)
	{
		if (!this._wasInitCalled)
		{
			this.Init();
		}
		if (this._queuedStateAsLong == enumValueAsLong || this._currentStateAsLong == enumValueAsLong)
		{
			return;
		}
		if (!this.IsPlaying || this._IsCurrentClipLoopable())
		{
			this.TryPlay(enumValueAsLong);
			return;
		}
		this._queuedStateAsLong = enumValueAsLong;
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x0004EFD0 File Offset: 0x0004D1D0
	private bool _IsCurrentClipLoopable()
	{
		if (this.m_animationComponent == null)
		{
			return false;
		}
		AnimationClip clip = this.m_animationComponent.clip;
		if (clip == null)
		{
			return false;
		}
		WrapMode wrapMode = clip.wrapMode;
		return wrapMode == 2 || wrapMode == 4;
	}

	// Token: 0x04001210 RID: 4624
	private const string preLog = "[GTAnimator]  ";

	// Token: 0x04001211 RID: 4625
	private const string preErr = "[GTAnimator]  ERROR!!!  ";

	// Token: 0x04001212 RID: 4626
	private const string preErrBeta = "[GTAnimator]  ERROR!!!  (beta only log)  ";

	// Token: 0x04001213 RID: 4627
	[Tooltip("Assign a unity Animation component (not to be confused with less performant Animator Component).")]
	[SerializeField]
	private Animation m_animationComponent;

	// Token: 0x04001215 RID: 4629
	[Tooltip("These will be activated when animation starts playing and deactivated when any anim finishes playing.")]
	[SerializeField]
	private GameObject[] m_animatedGameObjects;

	// Token: 0x04001216 RID: 4630
	[Tooltip("If an enum map value is not defined then these will be activated.")]
	[SerializeField]
	private GameObject[] m_defaultStaticGameObjects;

	// Token: 0x04001217 RID: 4631
	[Header("Enum To Animation Mapping")]
	[Tooltip("Map an enum's values to specific AnimationClips.")]
	[SerializeField]
	internal GTEnumValueMap<GTAnimator.AnimClipAndGObjs> m_animationMap;

	// Token: 0x04001218 RID: 4632
	private readonly HashSet<GameObject> _allStaticGobjs = new HashSet<GameObject>();

	// Token: 0x04001219 RID: 4633
	private const long _k_invalidState = -9223372036854775808L;

	// Token: 0x0400121A RID: 4634
	private long _currentStateAsLong = long.MinValue;

	// Token: 0x0400121B RID: 4635
	private int _frameCountWhenLastPlayed;

	// Token: 0x0400121C RID: 4636
	private bool _wasInitCalled;

	// Token: 0x0400121D RID: 4637
	private long _queuedStateAsLong = long.MinValue;

	// Token: 0x0200022B RID: 555
	[Serializable]
	public struct AnimClipAndGObjs
	{
		// Token: 0x0400121E RID: 4638
		public AnimationClip animClip;

		// Token: 0x0400121F RID: 4639
		public SoundBankPlayer soundBankToPlayOnStart;

		// Token: 0x04001220 RID: 4640
		[Tooltip("These GameObjects will be activated when the animation clip finishes playing.")]
		public GameObject[] endStaticGameObjects;
	}
}
