using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200016C RID: 364
[NetworkBehaviourWeaved(42)]
public class MonkeyeAI_ReplState : NetworkComponent
{
	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060009D1 RID: 2513 RVA: 0x0003584E File Offset: 0x00033A4E
	// (set) Token: 0x060009D2 RID: 2514 RVA: 0x00035878 File Offset: 0x00033A78
	[Networked]
	[NetworkedWeaved(0, 42)]
	private unsafe MonkeyeAI_ReplState.MonkeyeAI_RepStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x000358A4 File Offset: 0x00033AA4
	public override void WriteDataFusion()
	{
		MonkeyeAI_ReplState.MonkeyeAI_RepStateData data = new MonkeyeAI_ReplState.MonkeyeAI_RepStateData(this.userId, this.attackPos, this.timer, this.floorEnabled, this.portalEnabled, this.freezePlayer, this.alpha, this.state);
		this.Data = data;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x000358F0 File Offset: 0x00033AF0
	public override void ReadDataFusion()
	{
		this.userId = this.Data.UserId.Value;
		this.attackPos = this.Data.AttackPos;
		this.timer = this.Data.Timer;
		this.floorEnabled = this.Data.FloorEnabled;
		this.portalEnabled = this.Data.PortalEnabled;
		this.freezePlayer = this.Data.FreezePlayer;
		this.alpha = this.Data.Alpha;
		this.state = this.Data.State;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x000359B4 File Offset: 0x00033BB4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.userId);
		stream.SendNext(this.attackPos);
		stream.SendNext(this.timer);
		stream.SendNext(this.floorEnabled);
		stream.SendNext(this.portalEnabled);
		stream.SendNext(this.freezePlayer);
		stream.SendNext(this.alpha);
		stream.SendNext(this.state);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00035A44 File Offset: 0x00033C44
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.attackPos.SetValueSafe(vector);
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00035B1C File Offset: 0x00033D1C
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00035B34 File Offset: 0x00033D34
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000C0E RID: 3086
	public MonkeyeAI_ReplState.EStates state;

	// Token: 0x04000C0F RID: 3087
	public string userId;

	// Token: 0x04000C10 RID: 3088
	public Vector3 attackPos;

	// Token: 0x04000C11 RID: 3089
	public float timer;

	// Token: 0x04000C12 RID: 3090
	public bool floorEnabled;

	// Token: 0x04000C13 RID: 3091
	public bool portalEnabled;

	// Token: 0x04000C14 RID: 3092
	public bool freezePlayer;

	// Token: 0x04000C15 RID: 3093
	public float alpha;

	// Token: 0x04000C16 RID: 3094
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 42)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private MonkeyeAI_ReplState.MonkeyeAI_RepStateData _Data;

	// Token: 0x0200016D RID: 365
	public enum EStates
	{
		// Token: 0x04000C18 RID: 3096
		Sleeping,
		// Token: 0x04000C19 RID: 3097
		Patrolling,
		// Token: 0x04000C1A RID: 3098
		Chasing,
		// Token: 0x04000C1B RID: 3099
		ReturnToSleepPt,
		// Token: 0x04000C1C RID: 3100
		GoToSleep,
		// Token: 0x04000C1D RID: 3101
		BeginAttack,
		// Token: 0x04000C1E RID: 3102
		OpenFloor,
		// Token: 0x04000C1F RID: 3103
		DropPlayer,
		// Token: 0x04000C20 RID: 3104
		CloseFloor
	}

	// Token: 0x0200016E RID: 366
	[NetworkStructWeaved(42)]
	[StructLayout(2, Size = 168)]
	public struct MonkeyeAI_RepStateData : INetworkStruct
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060009DA RID: 2522 RVA: 0x00035B48 File Offset: 0x00033D48
		// (set) Token: 0x060009DB RID: 2523 RVA: 0x00035B5A File Offset: 0x00033D5A
		[Networked]
		[NetworkedWeaved(0, 33)]
		public unsafe NetworkString<_32> UserId
		{
			readonly get
			{
				return *(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId);
			}
			set
			{
				*(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId) = value;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060009DC RID: 2524 RVA: 0x00035B6D File Offset: 0x00033D6D
		// (set) Token: 0x060009DD RID: 2525 RVA: 0x00035B7F File Offset: 0x00033D7F
		[Networked]
		[NetworkedWeaved(33, 3)]
		public unsafe Vector3 AttackPos
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos) = value;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060009DE RID: 2526 RVA: 0x00035B92 File Offset: 0x00033D92
		// (set) Token: 0x060009DF RID: 2527 RVA: 0x00035BA0 File Offset: 0x00033DA0
		[Networked]
		[NetworkedWeaved(36, 1)]
		public unsafe float Timer
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer) = value;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060009E0 RID: 2528 RVA: 0x00035BAF File Offset: 0x00033DAF
		// (set) Token: 0x060009E1 RID: 2529 RVA: 0x00035BB7 File Offset: 0x00033DB7
		public NetworkBool FloorEnabled { readonly get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060009E2 RID: 2530 RVA: 0x00035BC0 File Offset: 0x00033DC0
		// (set) Token: 0x060009E3 RID: 2531 RVA: 0x00035BC8 File Offset: 0x00033DC8
		public NetworkBool PortalEnabled { readonly get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060009E4 RID: 2532 RVA: 0x00035BD1 File Offset: 0x00033DD1
		// (set) Token: 0x060009E5 RID: 2533 RVA: 0x00035BD9 File Offset: 0x00033DD9
		public NetworkBool FreezePlayer { readonly get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060009E6 RID: 2534 RVA: 0x00035BE2 File Offset: 0x00033DE2
		// (set) Token: 0x060009E7 RID: 2535 RVA: 0x00035BF0 File Offset: 0x00033DF0
		[Networked]
		[NetworkedWeaved(40, 1)]
		public unsafe float Alpha
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha) = value;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060009E8 RID: 2536 RVA: 0x00035BFF File Offset: 0x00033DFF
		// (set) Token: 0x060009E9 RID: 2537 RVA: 0x00035C07 File Offset: 0x00033E07
		public MonkeyeAI_ReplState.EStates State { readonly get; set; }

		// Token: 0x060009EA RID: 2538 RVA: 0x00035C10 File Offset: 0x00033E10
		public MonkeyeAI_RepStateData(string id, Vector3 atPos, float timer, bool floorOn, bool portalOn, bool freezePlayer, float alpha, MonkeyeAI_ReplState.EStates state)
		{
			this.UserId = id;
			this.AttackPos = atPos;
			this.Timer = timer;
			this.FloorEnabled = floorOn;
			this.PortalEnabled = portalOn;
			this.FreezePlayer = freezePlayer;
			this.Alpha = alpha;
			this.State = state;
		}

		// Token: 0x04000C21 RID: 3105
		[FixedBufferProperty(typeof(NetworkString<_32>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@33 _UserId;

		// Token: 0x04000C22 RID: 3106
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(132)]
		private FixedStorage@3 _AttackPos;

		// Token: 0x04000C23 RID: 3107
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(144)]
		private FixedStorage@1 _Timer;

		// Token: 0x04000C27 RID: 3111
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(160)]
		private FixedStorage@1 _Alpha;
	}
}
