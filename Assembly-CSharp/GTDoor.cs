using System;
using BoingKit;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

// Token: 0x020002CA RID: 714
public class GTDoor : NetworkSceneObject
{
	// Token: 0x0600119D RID: 4509 RVA: 0x0005CB6C File Offset: 0x0005AD6C
	protected override void Start()
	{
		base.Start();
		Collider[] array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		this.tLastOpened = 0f;
		GTDoorTrigger[] array2 = this.doorButtonTriggers;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].TriggeredEvent.AddListener(new UnityAction(this.DoorButtonTriggered));
		}
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x0005CBD8 File Offset: 0x0005ADD8
	private void Update()
	{
		if (this.currentState == GTDoor.DoorState.Open || this.currentState == GTDoor.DoorState.Closed)
		{
			if (Time.time < this.lastChecked + this.secondsCheck)
			{
				return;
			}
			this.lastChecked = Time.time;
		}
		this.UpdateDoorState();
		this.UpdateDoorAnimation();
		Collider[] array;
		if (this.currentState == GTDoor.DoorState.Closed)
		{
			array = this.doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x0005CC68 File Offset: 0x0005AE68
	private void UpdateDoorState()
	{
		this.peopleInHoldOpenVolume = false;
		foreach (GTDoorTrigger gtdoorTrigger in this.doorHoldOpenTriggers)
		{
			gtdoorTrigger.ValidateOverlappingColliders();
			if (gtdoorTrigger.overlapCount > 0)
			{
				this.peopleInHoldOpenVolume = true;
				break;
			}
		}
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
			if (this.buttonTriggeredThisFrame)
			{
				this.buttonTriggeredThisFrame = false;
				if (!NetworkSystem.Instance.InRoom)
				{
					this.OpenDoor();
				}
				else
				{
					this.currentState = GTDoor.DoorState.OpeningWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", 5, new object[]
					{
						GTDoor.DoorState.Opening
					});
				}
			}
			break;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			break;
		case GTDoor.DoorState.Closing:
			if (this.doorSpring.Value < 1f)
			{
				this.currentState = GTDoor.DoorState.Closed;
			}
			if (this.peopleInHoldOpenVolume)
			{
				this.currentState = GTDoor.DoorState.HeldOpenLocally;
				if (NetworkSystem.Instance.InRoom && base.IsMine)
				{
					this.photonView.RPC("ChangeDoorState", 5, new object[]
					{
						GTDoor.DoorState.HeldOpen
					});
				}
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
			}
			break;
		case GTDoor.DoorState.Open:
			if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
			{
				if (this.peopleInHoldOpenVolume)
				{
					this.currentState = GTDoor.DoorState.HeldOpenLocally;
					if (NetworkSystem.Instance.InRoom && base.IsMine)
					{
						this.photonView.RPC("ChangeDoorState", 5, new object[]
						{
							GTDoor.DoorState.HeldOpen
						});
					}
				}
				else if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", 5, new object[]
					{
						GTDoor.DoorState.Closing
					});
				}
			}
			break;
		case GTDoor.DoorState.Opening:
			if (this.doorSpring.Value > 89f)
			{
				this.currentState = GTDoor.DoorState.Open;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			if (!this.peopleInHoldOpenVolume)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", 5, new object[]
					{
						GTDoor.DoorState.Closing
					});
				}
			}
			break;
		case GTDoor.DoorState.HeldOpenLocally:
			if (!this.peopleInHoldOpenVolume)
			{
				this.CloseDoor();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			GTDoor.DoorState doorState = this.currentState;
			if (doorState == GTDoor.DoorState.ClosingWaitingOnRPC)
			{
				this.CloseDoor();
				return;
			}
			if (doorState != GTDoor.DoorState.OpeningWaitingOnRPC)
			{
				return;
			}
			this.OpenDoor();
		}
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0005CF0C File Offset: 0x0005B10C
	private void DoorButtonTriggered()
	{
		GTDoor.DoorState doorState = this.currentState;
		if (doorState - GTDoor.DoorState.Open <= 4)
		{
			return;
		}
		this.buttonTriggeredThisFrame = true;
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0005CF30 File Offset: 0x0005B130
	private void OpenDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			this.ResetDoorOpenedTime();
			this.audioSource.GTPlayOneShot(this.openSound, 1f);
			this.currentState = GTDoor.DoorState.Opening;
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0005CF98 File Offset: 0x0005B198
	private void CloseDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.Opening:
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.audioSource.GTPlayOneShot(this.closeSound, 1f);
			this.currentState = GTDoor.DoorState.Closing;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0005CFF8 File Offset: 0x0005B1F8
	private void UpdateDoorAnimation()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
			return;
		}
		this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
		this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x0005D0D7 File Offset: 0x0005B2D7
	public void ResetDoorOpenedTime()
	{
		this.tLastOpened = Time.time;
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0005D0E4 File Offset: 0x0005B2E4
	[PunRPC]
	public void ChangeDoorState(GTDoor.DoorState shouldOpenState, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ChangeDoorState");
		this.ChangeDoorStateShared(shouldOpenState);
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x0005D0F8 File Offset: 0x0005B2F8
	[Rpc]
	public unsafe static void RPC_ChangeDoorState(NetworkRunner runner, GTDoor.DoorState shouldOpenState, int doorId)
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != 4)
			{
				int num = 8;
				num += 4;
				num += 4;
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)"));
						int num2 = 8;
						*(GTDoor.DoorState*)(ptr2 + num2) = shouldOpenState;
						num2 += 4;
						*(int*)(ptr2 + num2) = doorId;
						num2 += 4;
						ptr.Offset = num2 * 8;
						ptr.SetStatic();
						runner.SendRpc(ptr);
					}
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)", num);
			}
			return;
		}
		IL_10:
		GTDoor[] array = Object.FindObjectsByType<GTDoor>(1, 0);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (GTDoor gtdoor in array)
		{
			if (gtdoor.GTDoorID == doorId)
			{
				gtdoor.ChangeDoorStateShared(shouldOpenState);
			}
		}
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0005D250 File Offset: 0x0005B450
	private void ChangeDoorStateShared(GTDoor.DoorState shouldOpenState)
	{
		switch (shouldOpenState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.HeldOpenLocally:
			break;
		case GTDoor.DoorState.Closing:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpen:
				this.CloseDoor();
				return;
			default:
				return;
			}
			break;
		case GTDoor.DoorState.Opening:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
				this.OpenDoor();
				return;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			default:
				return;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
				break;
			case GTDoor.DoorState.Closing:
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpenLocally:
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			default:
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x0005D344 File Offset: 0x0005B544
	public void SetupDoorIDs()
	{
		GTDoor[] array = Object.FindObjectsByType<GTDoor>(1, 0);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GTDoorID = i + 1;
		}
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0005D3A8 File Offset: 0x0005B5A8
	[NetworkRpcStaticWeavedInvoker("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ChangeDoorState@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		GTDoor.DoorState doorState = *(GTDoor.DoorState*)(ptr + num);
		num += 4;
		GTDoor.DoorState shouldOpenState = doorState;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int doorId = num2;
		NetworkBehaviourUtils.InvokeRpc = true;
		GTDoor.RPC_ChangeDoorState(runner, shouldOpenState, doorId);
	}

	// Token: 0x0400160F RID: 5647
	[SerializeField]
	private Transform doorTransform;

	// Token: 0x04001610 RID: 5648
	[SerializeField]
	private Collider[] doorColliders;

	// Token: 0x04001611 RID: 5649
	[SerializeField]
	private GTDoorTrigger[] doorButtonTriggers;

	// Token: 0x04001612 RID: 5650
	[SerializeField]
	private GTDoorTrigger[] doorHoldOpenTriggers;

	// Token: 0x04001613 RID: 5651
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001614 RID: 5652
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x04001615 RID: 5653
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x04001616 RID: 5654
	[SerializeField]
	private float doorOpenSpeed = 1f;

	// Token: 0x04001617 RID: 5655
	[SerializeField]
	private float doorCloseSpeed = 1f;

	// Token: 0x04001618 RID: 5656
	[SerializeField]
	[Range(1.5f, 10f)]
	private float timeUntilDoorCloses = 3f;

	// Token: 0x04001619 RID: 5657
	private int GTDoorID;

	// Token: 0x0400161A RID: 5658
	[DebugOption]
	private GTDoor.DoorState currentState;

	// Token: 0x0400161B RID: 5659
	private float tLastOpened;

	// Token: 0x0400161C RID: 5660
	private FloatSpring doorSpring;

	// Token: 0x0400161D RID: 5661
	[DebugOption]
	private bool peopleInHoldOpenVolume;

	// Token: 0x0400161E RID: 5662
	[DebugOption]
	private bool buttonTriggeredThisFrame;

	// Token: 0x0400161F RID: 5663
	private float lastChecked;

	// Token: 0x04001620 RID: 5664
	private float secondsCheck = 1f;

	// Token: 0x020002CB RID: 715
	public enum DoorState
	{
		// Token: 0x04001622 RID: 5666
		Closed,
		// Token: 0x04001623 RID: 5667
		ClosingWaitingOnRPC,
		// Token: 0x04001624 RID: 5668
		Closing,
		// Token: 0x04001625 RID: 5669
		Open,
		// Token: 0x04001626 RID: 5670
		OpeningWaitingOnRPC,
		// Token: 0x04001627 RID: 5671
		Opening,
		// Token: 0x04001628 RID: 5672
		HeldOpen,
		// Token: 0x04001629 RID: 5673
		HeldOpenLocally
	}
}
