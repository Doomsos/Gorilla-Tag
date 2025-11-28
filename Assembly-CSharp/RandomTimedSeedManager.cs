using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002F6 RID: 758
[NetworkBehaviourWeaved(2)]
public class RandomTimedSeedManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001291 RID: 4753 RVA: 0x0006179A File Offset: 0x0005F99A
	// (set) Token: 0x06001292 RID: 4754 RVA: 0x000617A1 File Offset: 0x0005F9A1
	public static RandomTimedSeedManager instance { get; private set; }

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001293 RID: 4755 RVA: 0x000617A9 File Offset: 0x0005F9A9
	// (set) Token: 0x06001294 RID: 4756 RVA: 0x000617B1 File Offset: 0x0005F9B1
	public int seed { get; private set; }

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06001295 RID: 4757 RVA: 0x000617BA File Offset: 0x0005F9BA
	// (set) Token: 0x06001296 RID: 4758 RVA: 0x000617C2 File Offset: 0x0005F9C2
	public float currentSyncTime { get; private set; }

	// Token: 0x06001297 RID: 4759 RVA: 0x000617CB File Offset: 0x0005F9CB
	protected override void Awake()
	{
		base.Awake();
		RandomTimedSeedManager.instance = this;
		this.seed = Random.Range(-1000000, -1000000);
		this.idealSyncTime = 0f;
		this.currentSyncTime = 0f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x0006180A File Offset: 0x0005FA0A
	public void AddCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Add(callback);
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x00061818 File Offset: 0x0005FA18
	public void RemoveCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Remove(callback);
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x0600129A RID: 4762 RVA: 0x00061827 File Offset: 0x0005FA27
	// (set) Token: 0x0600129B RID: 4763 RVA: 0x0006182F File Offset: 0x0005FA2F
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x0600129C RID: 4764 RVA: 0x00061838 File Offset: 0x0005FA38
	void ITickSystemTick.Tick()
	{
		this.currentSyncTime += Time.deltaTime;
		this.idealSyncTime += Time.deltaTime;
		if (this.idealSyncTime > 1E+09f)
		{
			this.idealSyncTime -= 1E+09f;
			this.currentSyncTime -= 1E+09f;
		}
		if (!base.GetView.AmOwner)
		{
			this.currentSyncTime = Mathf.Lerp(this.currentSyncTime, this.idealSyncTime, 0.1f);
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x0600129D RID: 4765 RVA: 0x000618C3 File Offset: 0x0005FAC3
	// (set) Token: 0x0600129E RID: 4766 RVA: 0x000618ED File Offset: 0x0005FAED
	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe RandomTimedSeedManager.RandomTimedSeedManagerData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00061918 File Offset: 0x0005FB18
	public override void WriteDataFusion()
	{
		this.Data = new RandomTimedSeedManager.RandomTimedSeedManagerData(this.seed, this.currentSyncTime);
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x00061934 File Offset: 0x0005FB34
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.seed, this.Data.currentSyncTime);
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00061963 File Offset: 0x0005FB63
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.seed);
		stream.SendNext(this.currentSyncTime);
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00061998 File Offset: 0x0005FB98
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int seedVal = (int)stream.ReceiveNext();
		float testTime = (float)stream.ReceiveNext();
		this.ReadDataShared(seedVal, testTime);
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000619D4 File Offset: 0x0005FBD4
	private void ReadDataShared(int seedVal, float testTime)
	{
		if (!float.IsFinite(testTime))
		{
			return;
		}
		this.seed = seedVal;
		if (testTime >= 0f && testTime <= 1E+09f)
		{
			if (this.idealSyncTime - testTime > 500000000f)
			{
				this.currentSyncTime = testTime;
			}
			this.idealSyncTime = testTime;
		}
		if (this.seed != this.cachedSeed && this.seed >= -1000000 && this.seed <= -1000000)
		{
			this.currentSyncTime = this.idealSyncTime;
			this.cachedSeed = this.seed;
			foreach (Action action in this.callbacksOnSeedChanged)
			{
				action.Invoke();
			}
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x00061AB7 File Offset: 0x0005FCB7
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x00061ACF File Offset: 0x0005FCCF
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400172C RID: 5932
	private List<Action> callbacksOnSeedChanged = new List<Action>();

	// Token: 0x0400172E RID: 5934
	private float idealSyncTime;

	// Token: 0x04001730 RID: 5936
	private int cachedSeed;

	// Token: 0x04001731 RID: 5937
	private const int SeedMin = -1000000;

	// Token: 0x04001732 RID: 5938
	private const int SeedMax = -1000000;

	// Token: 0x04001733 RID: 5939
	private const float MaxSyncTime = 1E+09f;

	// Token: 0x04001735 RID: 5941
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 2)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private RandomTimedSeedManager.RandomTimedSeedManagerData _Data;

	// Token: 0x020002F7 RID: 759
	[NetworkStructWeaved(2)]
	[StructLayout(2, Size = 8)]
	private struct RandomTimedSeedManagerData : INetworkStruct
	{
		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060012A7 RID: 4775 RVA: 0x00061AE3 File Offset: 0x0005FCE3
		// (set) Token: 0x060012A8 RID: 4776 RVA: 0x00061AF1 File Offset: 0x0005FCF1
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int seed
		{
			readonly get
			{
				return *(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed);
			}
			set
			{
				*(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed) = value;
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060012A9 RID: 4777 RVA: 0x00061B00 File Offset: 0x0005FD00
		// (set) Token: 0x060012AA RID: 4778 RVA: 0x00061B0E File Offset: 0x0005FD0E
		[Networked]
		[NetworkedWeaved(1, 1)]
		public unsafe float currentSyncTime
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime) = value;
			}
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00061B1D File Offset: 0x0005FD1D
		public RandomTimedSeedManagerData(int seed, float currentSyncTime)
		{
			this.seed = seed;
			this.currentSyncTime = currentSyncTime;
		}

		// Token: 0x04001736 RID: 5942
		[FixedBufferProperty(typeof(int), typeof(UnityValueSurrogate@ElementReaderWriterInt32), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _seed;

		// Token: 0x04001737 RID: 5943
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _currentSyncTime;
	}
}
