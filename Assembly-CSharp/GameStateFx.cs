using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200021E RID: 542
public class GameStateFx : MonoBehaviour, IGameStateReceiver, IDelayedExecListener
{
	// Token: 0x06000EC0 RID: 3776 RVA: 0x0004E600 File Offset: 0x0004C800
	protected void Awake()
	{
		IGameStateProvider gameStateProvider = this.m_stateProvider as IGameStateProvider;
		if (gameStateProvider == null)
		{
			GTDev.LogError<string>("[GT/GameStateFx]  ERROR!!!  Awake: The supplied State Provider is not type `IGameStateProvider`. Path=" + base.transform.GetPathQ(), null);
			this._isValid = false;
			base.enabled = false;
			return;
		}
		this._stateProvider = gameStateProvider;
		if (!this._IsAllValid())
		{
			return;
		}
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			if (array != null)
			{
				Array.Sort<GameStateFx.StateReaction>(array, new Comparison<GameStateFx.StateReaction>(GameStateFx._DelaySortCompare));
			}
		}
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0004E6AC File Offset: 0x0004C8AC
	private static int _DelaySortCompare(GameStateFx.StateReaction a, GameStateFx.StateReaction b)
	{
		return a.delay.CompareTo(b.delay);
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0004E6BF File Offset: 0x0004C8BF
	protected void OnEnable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			base.enabled = false;
			return;
		}
		this._stateProvider.GameStateReceiverRegister(this);
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0004E6E4 File Offset: 0x0004C8E4
	protected void OnDisable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._stateProvider.GameStateReceiverUnregister(this);
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0004E704 File Offset: 0x0004C904
	void IGameStateReceiver.GameStateReceiverOnStateChanged(long oldState, long newState)
	{
		GameStateFx.StateReaction[] array;
		if (!this.m_stateMap.TryGet(newState, out array))
		{
			return;
		}
		this._delayedExecContextFrameNum = Time.frameCount;
		this._reactionQueue.Clear();
		foreach (GameStateFx.StateReaction stateReaction in array)
		{
			if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Delay) != (GameStateFx.StateReaction.EOptions)0)
			{
				this._reactionQueue.Enqueue(stateReaction);
				GTDelayedExec.Add(this, stateReaction.delay, Time.frameCount);
			}
			else
			{
				GameStateFx._PerformReactions(stateReaction);
			}
		}
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0004E77B File Offset: 0x0004C97B
	void IDelayedExecListener.OnDelayedAction(int contextFrameNum)
	{
		if (contextFrameNum != this._delayedExecContextFrameNum || !base.isActiveAndEnabled)
		{
			return;
		}
		GameStateFx._PerformReactions(this._reactionQueue.Dequeue());
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0004E7A0 File Offset: 0x0004C9A0
	private static void _PerformReactions(GameStateFx.StateReaction reaction)
	{
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
		{
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Sound) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.resource = reaction.soundInfo.sound;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Volume) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.volume = reaction.soundInfo.volume;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Pitch) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.pitch = reaction.soundInfo.pitch;
			}
			reaction.soundInfo.source.GTPlay();
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.GameObjectInfo gameObjectInfo in reaction.gameObjectInfos)
			{
				gameObjectInfo.gameObject.SetActive(gameObjectInfo.activate);
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.BehaviourInfo behaviourInfo in reaction.behaviourInfos)
			{
				behaviourInfo.behaviour.enabled = behaviourInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.RenderInfo renderInfo in reaction.renderers)
			{
				renderInfo.renderer.enabled = renderInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
		{
			GameStateFx.MaterialInfo[] materialInfos = reaction.materialInfos;
			for (int i = 0; i < materialInfos.Length; i++)
			{
				foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[i].entries)
				{
					entry.slotInfo.renderer.GetSharedMaterials(GameStateFx._g_materialsCache);
					if (entry.slotInfo.slot >= 0 && entry.slotInfo.slot < GameStateFx._g_materialsCache.Count)
					{
						GameStateFx._g_materialsCache[entry.slotInfo.slot] = entry.material;
						entry.slotInfo.renderer.SetSharedMaterials(GameStateFx._g_materialsCache);
					}
				}
			}
		}
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0004E9C8 File Offset: 0x0004CBC8
	private bool _IsAllValid()
	{
		this._isValid = true;
		bool flag = false;
		this._hasDefaultAudioSource = (this.m_defaultAudioSource != null);
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			foreach (GameStateFx.StateReaction stateReaction in array)
			{
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
				{
					if ((stateReaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Source) != (GameStateFx.SoundEntry.EOptions)0)
					{
						if (!this._IsOneValid(stateReaction.soundInfo.source != null, "an AudioSource is unassigned."))
						{
							return false;
						}
					}
					else
					{
						flag = true;
						stateReaction.soundInfo.source = this.m_defaultAudioSource;
					}
					if (!this._IsOneValid(stateReaction.soundInfo.sound != null, "A sound is unassigned."))
					{
						return false;
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.GameObjectInfo gameObjectInfo in stateReaction.gameObjectInfos)
					{
						if (!this._IsOneValid(gameObjectInfo.gameObject != null, "A GameObject is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.BehaviourInfo behaviourInfo in stateReaction.behaviourInfos)
					{
						if (!this._IsOneValid(behaviourInfo.behaviour != null, "A Behaviour is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.RenderInfo renderInfo in stateReaction.renderers)
					{
						if (!this._IsOneValid(renderInfo.renderer != null, "A Renderer is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
				{
					GameStateFx.MaterialInfo[] materialInfos = stateReaction.materialInfos;
					for (int j = 0; j < materialInfos.Length; j++)
					{
						foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[j].entries)
						{
							if (!this._IsOneValid(entry.slotInfo.renderer != null, "A mat swap Renderer is unassigned"))
							{
								return false;
							}
						}
					}
				}
			}
		}
		if (flag && !this._hasDefaultAudioSource)
		{
			base.enabled = false;
			this._isValid = false;
			return false;
		}
		return true;
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x0004EC6C File Offset: 0x0004CE6C
	private bool _IsOneValid(bool isValidCondition, string msgFailReason)
	{
		if (isValidCondition)
		{
			return true;
		}
		this._isValid = false;
		base.enabled = false;
		return false;
	}

	// Token: 0x040011E4 RID: 4580
	private const string preLog = "[GT/GameStateFx]  ";

	// Token: 0x040011E5 RID: 4581
	private const string preErr = "[GT/GameStateFx]  ERROR!!!  ";

	// Token: 0x040011E6 RID: 4582
	private bool _isValid;

	// Token: 0x040011E7 RID: 4583
	[SerializeField]
	private MonoBehaviour m_stateProvider;

	// Token: 0x040011E8 RID: 4584
	private IGameStateProvider _stateProvider;

	// Token: 0x040011E9 RID: 4585
	[SerializeField]
	private AudioSource m_defaultAudioSource;

	// Token: 0x040011EA RID: 4586
	private bool _hasDefaultAudioSource;

	// Token: 0x040011EB RID: 4587
	[SerializeField]
	private GTEnumValueMap<GameStateFx.StateReaction[]> m_stateMap;

	// Token: 0x040011EC RID: 4588
	private int _delayedExecContextFrameNum;

	// Token: 0x040011ED RID: 4589
	private Queue<GameStateFx.StateReaction> _reactionQueue = new Queue<GameStateFx.StateReaction>(4);

	// Token: 0x040011EE RID: 4590
	private static readonly List<Material> _g_materialsCache = new List<Material>(8);

	// Token: 0x0200021F RID: 543
	[Serializable]
	internal class StateReaction
	{
		// Token: 0x040011EF RID: 4591
		[Tooltip("Options for what this reaction should do.")]
		public GameStateFx.StateReaction.EOptions options;

		// Token: 0x040011F0 RID: 4592
		public float delay;

		// Token: 0x040011F1 RID: 4593
		public GameStateFx.SoundEntry soundInfo;

		// Token: 0x040011F2 RID: 4594
		public GameStateFx.GameObjectInfo[] gameObjectInfos;

		// Token: 0x040011F3 RID: 4595
		public GameStateFx.BehaviourInfo[] behaviourInfos;

		// Token: 0x040011F4 RID: 4596
		public GameStateFx.RenderInfo[] renderers;

		// Token: 0x040011F5 RID: 4597
		public GameStateFx.MaterialInfo[] materialInfos;

		// Token: 0x02000220 RID: 544
		[Flags]
		public enum EOptions
		{
			// Token: 0x040011F7 RID: 4599
			Delay = 1,
			// Token: 0x040011F8 RID: 4600
			Sound = 2,
			// Token: 0x040011F9 RID: 4601
			GameObjects = 4,
			// Token: 0x040011FA RID: 4602
			Behaviours = 8,
			// Token: 0x040011FB RID: 4603
			Renderers = 16,
			// Token: 0x040011FC RID: 4604
			Materials = 32
		}
	}

	// Token: 0x02000221 RID: 545
	[Serializable]
	public struct SoundEntry
	{
		// Token: 0x040011FD RID: 4605
		public GameStateFx.SoundEntry.EOptions options;

		// Token: 0x040011FE RID: 4606
		public AudioSource source;

		// Token: 0x040011FF RID: 4607
		public AudioResource sound;

		// Token: 0x04001200 RID: 4608
		public float volume;

		// Token: 0x04001201 RID: 4609
		public float pitch;

		// Token: 0x02000222 RID: 546
		[Flags]
		public enum EOptions
		{
			// Token: 0x04001203 RID: 4611
			Source = 1,
			// Token: 0x04001204 RID: 4612
			Sound = 2,
			// Token: 0x04001205 RID: 4613
			Volume = 4,
			// Token: 0x04001206 RID: 4614
			Pitch = 8
		}
	}

	// Token: 0x02000223 RID: 547
	[Serializable]
	internal struct GameObjectInfo
	{
		// Token: 0x04001207 RID: 4615
		public bool activate;

		// Token: 0x04001208 RID: 4616
		public GameObject gameObject;
	}

	// Token: 0x02000224 RID: 548
	[Serializable]
	internal struct BehaviourInfo
	{
		// Token: 0x04001209 RID: 4617
		public bool enable;

		// Token: 0x0400120A RID: 4618
		public Behaviour behaviour;
	}

	// Token: 0x02000225 RID: 549
	[Serializable]
	internal struct RenderInfo
	{
		// Token: 0x0400120B RID: 4619
		public bool enable;

		// Token: 0x0400120C RID: 4620
		public Renderer renderer;
	}

	// Token: 0x02000226 RID: 550
	[Serializable]
	internal struct MaterialInfo
	{
		// Token: 0x0400120D RID: 4621
		public GameStateFx.MaterialInfo.Entry[] entries;

		// Token: 0x02000227 RID: 551
		[Serializable]
		internal struct Entry
		{
			// Token: 0x0400120E RID: 4622
			public GTRendererMatSlot slotInfo;

			// Token: 0x0400120F RID: 4623
			public Material material;
		}
	}
}
