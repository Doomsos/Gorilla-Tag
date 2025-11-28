using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000164 RID: 356
[NetworkBehaviourWeaved(3)]
public class BarrelCannon : NetworkComponent
{
	// Token: 0x06000982 RID: 2434 RVA: 0x0003346D File Offset: 0x0003166D
	private void Update()
	{
		if (base.IsMine)
		{
			this.AuthorityUpdate();
		}
		else
		{
			this.ClientUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x0003348C File Offset: 0x0003168C
	private void AuthorityUpdate()
	{
		float time = Time.time;
		this.syncedState.hasAuthorityPassenger = this.localPlayerInside;
		switch (this.syncedState.currentState)
		{
		default:
			if (this.localPlayerInside)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Loaded;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Loaded:
			if (time - this.stateStartTime > this.cannonEntryDelayTime)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.MovingToFirePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.MovingToFirePosition:
			if (this.moveToFiringPositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = Mathf.Clamp01((time - this.stateStartTime) / this.moveToFiringPositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 1f;
			}
			if (this.syncedState.firingPositionLerpValue >= 1f - Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Firing;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Firing:
			if (this.localPlayerInside && this.localPlayerRigidbody != null)
			{
				Vector3 vector = base.transform.position - GorillaTagger.Instance.headCollider.transform.position;
				this.localPlayerRigidbody.MovePosition(this.localPlayerRigidbody.position + vector);
			}
			if (time - this.stateStartTime > this.preFiringDelayTime)
			{
				base.transform.localPosition = this.firingPositionOffset;
				base.transform.localRotation = Quaternion.Euler(this.firingRotationOffset);
				this.FireBarrelCannonLocal(base.transform.position, base.transform.up);
				if (PhotonNetwork.InRoom && GorillaGameManager.instance != null)
				{
					base.SendRPC("FireBarrelCannonRPC", 1, new object[]
					{
						base.transform.position,
						base.transform.up
					});
				}
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.PostFireCooldown;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.PostFireCooldown:
			if (time - this.stateStartTime > this.postFiringCooldownTime)
			{
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.ReturningToIdlePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.ReturningToIdlePosition:
			if (this.returnToIdlePositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f - Mathf.Clamp01((time - this.stateStartTime) / this.returnToIdlePositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 0f;
			}
			if (this.syncedState.firingPositionLerpValue <= Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 0f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Idle;
			}
			break;
		}
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00033790 File Offset: 0x00031990
	private void ClientUpdate()
	{
		if (!this.syncedState.hasAuthorityPassenger && this.syncedState.currentState == BarrelCannon.BarrelCannonState.Idle && this.localPlayerInside)
		{
			base.RequestOwnership();
		}
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x000337BC File Offset: 0x000319BC
	private void SharedUpdate()
	{
		if (this.syncedState.firingPositionLerpValue != this.localFiringPositionLerpValue)
		{
			this.localFiringPositionLerpValue = this.syncedState.firingPositionLerpValue;
			base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.firingPositionOffset, this.firePositionAnimationCurve.Evaluate(this.localFiringPositionLerpValue));
			base.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, this.firingRotationOffset, this.fireRotationAnimationCurve.Evaluate(this.localFiringPositionLerpValue)));
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00002789 File Offset: 0x00000989
	private void FireBarrelCannonRPC(Vector3 cannonCenter, Vector3 firingDirection)
	{
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0003384C File Offset: 0x00031A4C
	private void FireBarrelCannonLocal(Vector3 cannonCenter, Vector3 firingDirection)
	{
		if (this.audioSource != null)
		{
			this.audioSource.GTPlay();
		}
		if (this.localPlayerInside && this.localPlayerRigidbody != null)
		{
			Vector3 vector = cannonCenter - GorillaTagger.Instance.headCollider.transform.position;
			this.localPlayerRigidbody.position = this.localPlayerRigidbody.position + vector;
			this.localPlayerRigidbody.linearVelocity = firingDirection * this.firingSpeed;
		}
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x000338D8 File Offset: 0x00031AD8
	private void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = true;
			this.localPlayerRigidbody = rigidbody;
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00033900 File Offset: 0x00031B00
	private void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = false;
			this.localPlayerRigidbody = null;
		}
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00033926 File Offset: 0x00031B26
	private bool LocalPlayerTriggerFilter(Collider other, out Rigidbody rb)
	{
		rb = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
		}
		return rb != null;
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0003395C File Offset: 0x00031B5C
	private bool IsLocalPlayerInCannon()
	{
		Vector3 vector;
		Vector3 vector2;
		this.GetCapsulePoints(this.triggerCollider, out vector, out vector2);
		Physics.OverlapCapsuleNonAlloc(vector, vector2, this.triggerCollider.radius, this.triggerOverlapResults);
		for (int i = 0; i < this.triggerOverlapResults.Length; i++)
		{
			Rigidbody rigidbody;
			if (this.LocalPlayerTriggerFilter(this.triggerOverlapResults[i], out rigidbody))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x000339BC File Offset: 0x00031BBC
	private void GetCapsulePoints(CapsuleCollider capsule, out Vector3 pointA, out Vector3 pointB)
	{
		float num = capsule.height * 0.5f - capsule.radius;
		pointA = capsule.transform.position + capsule.transform.up * num;
		pointB = capsule.transform.position - capsule.transform.up * num;
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x0600098D RID: 2445 RVA: 0x00033A2B File Offset: 0x00031C2B
	// (set) Token: 0x0600098E RID: 2446 RVA: 0x00033A55 File Offset: 0x00031C55
	[Networked]
	[NetworkedWeaved(0, 3)]
	private unsafe BarrelCannon.BarrelCannonSyncedStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00033A80 File Offset: 0x00031C80
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00033A94 File Offset: 0x00031C94
	public override void ReadDataFusion()
	{
		this.syncedState.currentState = this.Data.CurrentState;
		this.syncedState.hasAuthorityPassenger = this.Data.HasAuthorityPassenger;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00033AD8 File Offset: 0x00031CD8
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.syncedState.currentState);
		stream.SendNext(this.syncedState.hasAuthorityPassenger);
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00033B06 File Offset: 0x00031D06
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.syncedState.currentState = (BarrelCannon.BarrelCannonState)stream.ReceiveNext();
		this.syncedState.hasAuthorityPassenger = (bool)stream.ReceiveNext();
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x00033B34 File Offset: 0x00031D34
	public override void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
		if (!this.localPlayerInside)
		{
			targetView.TransferOwnership(requestingPlayer);
		}
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00033C09 File Offset: 0x00031E09
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x00033C21 File Offset: 0x00031E21
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000B9C RID: 2972
	[SerializeField]
	private float firingSpeed = 10f;

	// Token: 0x04000B9D RID: 2973
	[Header("Cannon's Movement Before Firing")]
	[SerializeField]
	private Vector3 firingPositionOffset = Vector3.zero;

	// Token: 0x04000B9E RID: 2974
	[SerializeField]
	private Vector3 firingRotationOffset = Vector3.zero;

	// Token: 0x04000B9F RID: 2975
	[SerializeField]
	private AnimationCurve firePositionAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000BA0 RID: 2976
	[SerializeField]
	private AnimationCurve fireRotationAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000BA1 RID: 2977
	[Header("Cannon State Change Timing Parameters")]
	[SerializeField]
	private float moveToFiringPositionTime = 0.5f;

	// Token: 0x04000BA2 RID: 2978
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float cannonEntryDelayTime = 0.25f;

	// Token: 0x04000BA3 RID: 2979
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float preFiringDelayTime = 0.25f;

	// Token: 0x04000BA4 RID: 2980
	[SerializeField]
	[Tooltip("The minimum time to wait after the cannon fires before it starts moving back to the idle position.")]
	private float postFiringCooldownTime = 0.25f;

	// Token: 0x04000BA5 RID: 2981
	[SerializeField]
	private float returnToIdlePositionTime = 1f;

	// Token: 0x04000BA6 RID: 2982
	[Header("Component References")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000BA7 RID: 2983
	[SerializeField]
	private CapsuleCollider triggerCollider;

	// Token: 0x04000BA8 RID: 2984
	[SerializeField]
	private Collider[] colliders;

	// Token: 0x04000BA9 RID: 2985
	private BarrelCannon.BarrelCannonSyncedState syncedState = new BarrelCannon.BarrelCannonSyncedState();

	// Token: 0x04000BAA RID: 2986
	private Collider[] triggerOverlapResults = new Collider[16];

	// Token: 0x04000BAB RID: 2987
	private bool localPlayerInside;

	// Token: 0x04000BAC RID: 2988
	private Rigidbody localPlayerRigidbody;

	// Token: 0x04000BAD RID: 2989
	private float stateStartTime;

	// Token: 0x04000BAE RID: 2990
	private float localFiringPositionLerpValue;

	// Token: 0x04000BAF RID: 2991
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private BarrelCannon.BarrelCannonSyncedStateData _Data;

	// Token: 0x02000165 RID: 357
	private enum BarrelCannonState
	{
		// Token: 0x04000BB1 RID: 2993
		Idle,
		// Token: 0x04000BB2 RID: 2994
		Loaded,
		// Token: 0x04000BB3 RID: 2995
		MovingToFirePosition,
		// Token: 0x04000BB4 RID: 2996
		Firing,
		// Token: 0x04000BB5 RID: 2997
		PostFireCooldown,
		// Token: 0x04000BB6 RID: 2998
		ReturningToIdlePosition
	}

	// Token: 0x02000166 RID: 358
	private class BarrelCannonSyncedState
	{
		// Token: 0x04000BB7 RID: 2999
		public BarrelCannon.BarrelCannonState currentState;

		// Token: 0x04000BB8 RID: 3000
		public bool hasAuthorityPassenger;

		// Token: 0x04000BB9 RID: 3001
		public float firingPositionLerpValue;
	}

	// Token: 0x02000167 RID: 359
	[NetworkStructWeaved(3)]
	[StructLayout(2, Size = 12)]
	private struct BarrelCannonSyncedStateData : INetworkStruct
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000998 RID: 2456 RVA: 0x00033C35 File Offset: 0x00031E35
		// (set) Token: 0x06000999 RID: 2457 RVA: 0x00033C47 File Offset: 0x00031E47
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe BarrelCannon.BarrelCannonState CurrentState
		{
			readonly get
			{
				return *(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState);
			}
			set
			{
				*(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState) = value;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600099A RID: 2458 RVA: 0x00033C5A File Offset: 0x00031E5A
		// (set) Token: 0x0600099B RID: 2459 RVA: 0x00033C6C File Offset: 0x00031E6C
		[Networked]
		[NetworkedWeaved(1, 1)]
		public unsafe NetworkBool HasAuthorityPassenger
		{
			readonly get
			{
				return *(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger);
			}
			set
			{
				*(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger) = value;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600099C RID: 2460 RVA: 0x00033C7F File Offset: 0x00031E7F
		// (set) Token: 0x0600099D RID: 2461 RVA: 0x00033C87 File Offset: 0x00031E87
		public float FiringPositionLerpValue { readonly get; set; }

		// Token: 0x0600099E RID: 2462 RVA: 0x00033C90 File Offset: 0x00031E90
		public BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonState state, bool hasAuthPassenger, float firingPosLerpVal)
		{
			this.CurrentState = state;
			this.HasAuthorityPassenger = hasAuthPassenger;
			this.FiringPositionLerpValue = firingPosLerpVal;
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00033CAC File Offset: 0x00031EAC
		public static implicit operator BarrelCannon.BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonSyncedState state)
		{
			return new BarrelCannon.BarrelCannonSyncedStateData(state.currentState, state.hasAuthorityPassenger, state.firingPositionLerpValue);
		}

		// Token: 0x04000BBA RID: 3002
		[FixedBufferProperty(typeof(BarrelCannon.BarrelCannonState), typeof(UnityValueSurrogate@ReaderWriter@BarrelCannon__BarrelCannonState), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _CurrentState;

		// Token: 0x04000BBB RID: 3003
		[FixedBufferProperty(typeof(NetworkBool), typeof(UnityValueSurrogate@ElementReaderWriterNetworkBool), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _HasAuthorityPassenger;
	}
}
