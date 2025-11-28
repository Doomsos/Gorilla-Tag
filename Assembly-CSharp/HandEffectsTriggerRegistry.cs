using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.Shared.Scripts.Utilities;
using TagEffects;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000352 RID: 850
[DefaultExecutionOrder(10000)]
public class HandEffectsTriggerRegistry : MonoBehaviour, ITickSystemTick, ITickSystemPost
{
	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06001445 RID: 5189 RVA: 0x0007460B File Offset: 0x0007280B
	// (set) Token: 0x06001446 RID: 5190 RVA: 0x00074613 File Offset: 0x00072813
	public bool TickRunning { get; set; }

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001447 RID: 5191 RVA: 0x0007461C File Offset: 0x0007281C
	// (set) Token: 0x06001448 RID: 5192 RVA: 0x00074624 File Offset: 0x00072824
	public bool PostTickRunning { get; set; }

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001449 RID: 5193 RVA: 0x0007462D File Offset: 0x0007282D
	// (set) Token: 0x0600144A RID: 5194 RVA: 0x00074634 File Offset: 0x00072834
	public static HandEffectsTriggerRegistry Instance { get; private set; }

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x0600144B RID: 5195 RVA: 0x0007463C File Offset: 0x0007283C
	// (set) Token: 0x0600144C RID: 5196 RVA: 0x00074643 File Offset: 0x00072843
	public static bool HasInstance { get; private set; }

	// Token: 0x0600144D RID: 5197 RVA: 0x0007464B File Offset: 0x0007284B
	public static void FindInstance()
	{
		HandEffectsTriggerRegistry.Instance = Object.FindAnyObjectByType<HandEffectsTriggerRegistry>();
		HandEffectsTriggerRegistry.HasInstance = true;
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x00074660 File Offset: 0x00072860
	private void Awake()
	{
		HandEffectsTriggerRegistry.Instance = this;
		HandEffectsTriggerRegistry.HasInstance = true;
		this.job = new HandEffectsTriggerRegistry.HandEffectsJob
		{
			positionInput = new NativeArray<Vector3>(30, 4, 1),
			closeOutput = new NativeArray<bool>(900, 4, 1),
			actualListSize = this.actualListSz
		};
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x000746B8 File Offset: 0x000728B8
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x000746C6 File Offset: 0x000728C6
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x000746D4 File Offset: 0x000728D4
	public void Register(IHandEffectsTrigger trigger)
	{
		if (this.triggers.Count < 30)
		{
			this.actualListSz++;
			this.triggers.Add(trigger);
		}
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x00074700 File Offset: 0x00072900
	public void Unregister(IHandEffectsTrigger trigger)
	{
		int num = this.triggers.IndexOf(trigger);
		if (num >= 0)
		{
			this.actualListSz--;
			this.triggers.RemoveAt(num);
		}
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x00074738 File Offset: 0x00072938
	private void OnDestroy()
	{
		if (!this.jobHandle.IsCompleted)
		{
			this.jobHandle.Complete();
		}
		this.job.Dispose();
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x00074760 File Offset: 0x00072960
	public void Tick()
	{
		this.CopyInput();
		this.jobHandle = IJobParallelForExtensions.Schedule<HandEffectsTriggerRegistry.HandEffectsJob>(this.job, this.actualListSz, 20, default(JobHandle));
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x00074795 File Offset: 0x00072995
	public void PostTick()
	{
		this.jobHandle.Complete();
		this.CheckForHandEffectOnProcessedOutput();
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x000747A8 File Offset: 0x000729A8
	public void CheckForHandEffectOnProcessedOutput()
	{
		this.newCollisionBits.Clear();
		for (int i = 0; i < this.triggers.Count; i++)
		{
			IHandEffectsTrigger handEffectsTrigger = this.triggers[i];
			int num = i * 30;
			for (int j = i + 1; j < this.triggers.Count; j++)
			{
				if (this.job.closeOutput[i * 30 + j])
				{
					IHandEffectsTrigger handEffectsTrigger2 = this.triggers[j];
					if (handEffectsTrigger.InTriggerZone(handEffectsTrigger2) || handEffectsTrigger2.InTriggerZone(handEffectsTrigger))
					{
						int idx = num + j;
						this.newCollisionBits[idx] = true;
						if (!this.existingCollisionBits[idx] && Time.time - this.triggerTimes[i] > 0.5f && Time.time - this.triggerTimes[j] > 0.5f)
						{
							handEffectsTrigger.OnTriggerEntered(handEffectsTrigger2);
							handEffectsTrigger2.OnTriggerEntered(handEffectsTrigger);
							this.triggerTimes[i] = (this.triggerTimes[j] = Time.time);
						}
					}
				}
			}
		}
		this.existingCollisionBits.CopyFrom(this.newCollisionBits);
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x000748D0 File Offset: 0x00072AD0
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.job.positionInput[i] = this.triggers[i].Transform.position;
		}
		if (this.job.actualListSize != this.actualListSz)
		{
			this.job.actualListSize = this.actualListSz;
		}
	}

	// Token: 0x04001EE0 RID: 7904
	private const int MAX_TRIGGERS = 30;

	// Token: 0x04001EE1 RID: 7905
	private const int BIT_ARRAY_SIZE = 900;

	// Token: 0x04001EE2 RID: 7906
	private const float COOLDOWN_TIME = 0.5f;

	// Token: 0x04001EE3 RID: 7907
	private const float DEFAULT_RADIUS = 0.5f;

	// Token: 0x04001EE4 RID: 7908
	private readonly List<IHandEffectsTrigger> triggers = new List<IHandEffectsTrigger>();

	// Token: 0x04001EE5 RID: 7909
	private readonly float[] triggerTimes = new float[30];

	// Token: 0x04001EE6 RID: 7910
	private readonly GTBitArray existingCollisionBits = new GTBitArray(900);

	// Token: 0x04001EE7 RID: 7911
	private readonly GTBitArray newCollisionBits = new GTBitArray(900);

	// Token: 0x04001EE8 RID: 7912
	private int actualListSz;

	// Token: 0x04001EE9 RID: 7913
	private JobHandle jobHandle;

	// Token: 0x04001EEA RID: 7914
	private HandEffectsTriggerRegistry.HandEffectsJob job;

	// Token: 0x02000353 RID: 851
	[BurstCompile]
	private struct HandEffectsJob : IJobParallelFor, IDisposable
	{
		// Token: 0x06001459 RID: 5209 RVA: 0x0007497C File Offset: 0x00072B7C
		public void Execute(int i)
		{
			for (int j = i + 1; j < this.actualListSize; j++)
			{
				this.closeOutput[i * 30 + j] = (this.positionInput[i] - this.positionInput[j]).IsShorterThan(0.5f);
			}
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x000749D4 File Offset: 0x00072BD4
		public void Dispose()
		{
			this.positionInput.Dispose();
			this.closeOutput.Dispose();
		}

		// Token: 0x04001EEF RID: 7919
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> positionInput;

		// Token: 0x04001EF0 RID: 7920
		[NativeDisableParallelForRestriction]
		public NativeArray<bool> closeOutput;

		// Token: 0x04001EF1 RID: 7921
		public int actualListSize;
	}
}
