using System;
using System.Collections.Generic;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020006A0 RID: 1696
public class GRElevator : MonoBehaviour
{
	// Token: 0x06002B50 RID: 11088 RVA: 0x000E82E4 File Offset: 0x000E64E4
	private void OnEnable()
	{
		GRElevatorManager.RegisterElevator(this);
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.Play();
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000E8308 File Offset: 0x000E6508
	private void OnDisable()
	{
		GRElevatorManager.DeregisterElevator(this);
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000E8310 File Offset: 0x000E6510
	private void Awake()
	{
		this.typeButtonDict = new Dictionary<GRElevator.ButtonType, GRElevatorButton>();
		for (int i = 0; i < this.elevatorButtons.Count; i++)
		{
			this.typeButtonDict.TryAdd(this.elevatorButtons[i].buttonType, this.elevatorButtons[i]);
		}
		this.travelDistance = (this.openTargetTop.position - this.closedTargetTop.position).magnitude;
		this.doorOpenSpeed = this.travelDistance / this.openTravelDuration;
		this.doorCloseSpeed = this.travelDistance / this.closeTravelDuration;
		this.state = GRElevator.ElevatorState.DoorClosed;
		this.UpdateLocalState(this.state);
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000E83C9 File Offset: 0x000E65C9
	public void PressButton(int type)
	{
		GRElevatorManager.ElevatorButtonPressed((GRElevator.ButtonType)type, this.location);
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x000E83D7 File Offset: 0x000E65D7
	public void PressButtonVisuals(GRElevator.ButtonType type)
	{
		this.typeButtonDict[type].Pressed();
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000E83EA File Offset: 0x000E65EA
	public void PlayDing()
	{
		this.ambientAudio.PlayOneShot(this.dingClip);
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x000E83FD File Offset: 0x000E65FD
	public void PlayButtonPress()
	{
		this.buttonBank.Play();
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000E840C File Offset: 0x000E660C
	public void PlayElevatorMoving()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.travellingLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.travellingLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000E8478 File Offset: 0x000E6678
	public void PlayElevatorStopped()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.ambientLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000E84E3 File Offset: 0x000E66E3
	public void PlayElevatorMusic(float time = 0f)
	{
		if (this.musicAudio.isPlaying)
		{
			return;
		}
		this.musicAudio.time = time;
		this.musicAudio.Play();
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x000E850A File Offset: 0x000E670A
	public void PlayDoorOpenBegin()
	{
		this.doorAudio.clip = this.doorOpenClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x000E8538 File Offset: 0x000E6738
	public void PlayDoorCloseBegin()
	{
		this.doorAudio.clip = this.doorCloseClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000E8566 File Offset: 0x000E6766
	public void PlayDoorOpenTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.openBeginDuration;
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000E8580 File Offset: 0x000E6780
	public void PlayDoorCloseTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.closeBeginDuration;
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000E859C File Offset: 0x000E679C
	public bool DoorsFullyClosed()
	{
		return (this.upperDoor.position - this.closedTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000E85D4 File Offset: 0x000E67D4
	public bool DoorsFullyOpen()
	{
		return (this.upperDoor.position - this.openTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x000E860C File Offset: 0x000E680C
	public void UpdateLocalState(GRElevator.ElevatorState newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (newState)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (this.DoorsFullyClosed())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorClosedBeginTime();
			this.PlayDoorCloseBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingClosing:
			this.PlayDoorCloseTravel();
			return;
		case GRElevator.ElevatorState.DoorEndClosing:
		case GRElevator.ElevatorState.DoorEndOpening:
			break;
		case GRElevator.ElevatorState.DoorClosed:
			this.upperDoor.position = this.closedTargetTop.position;
			this.lowerDoor.position = this.closedTargetBottom.position;
			return;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (this.DoorsFullyOpen())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorOpenBeginTime();
			this.PlayDoorOpenBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingOpening:
			this.PlayDoorOpenTravel();
			return;
		case GRElevator.ElevatorState.DoorOpen:
			this.upperDoor.position = this.openTargetTop.position;
			this.lowerDoor.position = this.openTargetBottom.position;
			break;
		default:
			return;
		}
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x000E8708 File Offset: 0x000E6908
	public void UpdateRemoteState(GRElevator.ElevatorState remoteNewState)
	{
		if (GRElevator.StateIsOpeningState(remoteNewState) && GRElevator.StateIsClosingState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginOpening);
			return;
		}
		if (GRElevator.StateIsClosingState(remoteNewState) && GRElevator.StateIsOpeningState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x000E8744 File Offset: 0x000E6944
	public void SetDoorOpenBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.openTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.openTravelDuration;
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x000E8794 File Offset: 0x000E6994
	public void SetDoorClosedBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.closedTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.closeTravelDuration;
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x000E87E1 File Offset: 0x000E69E1
	public static bool StateIsOpeningState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingOpening || checkState == GRElevator.ElevatorState.DoorBeginOpening || checkState == GRElevator.ElevatorState.DoorEndOpening || checkState == GRElevator.ElevatorState.DoorOpen;
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x000E87F5 File Offset: 0x000E69F5
	public static bool StateIsClosingState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingClosing || checkState == GRElevator.ElevatorState.DoorBeginClosing || checkState == GRElevator.ElevatorState.DoorEndClosing || checkState == GRElevator.ElevatorState.DoorClosed;
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x000E8808 File Offset: 0x000E6A08
	public bool DoorIsOpening()
	{
		return GRElevator.StateIsOpeningState(this.state);
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x000E8815 File Offset: 0x000E6A15
	public bool DoorIsClosing()
	{
		return GRElevator.StateIsClosingState(this.state);
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x000E8824 File Offset: 0x000E6A24
	public void PhysicalElevatorUpdate()
	{
		switch (this.state)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (Time.time > this.doorMoveBeginTime + this.closeBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorEndClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration + this.closeEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
			}
			break;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (Time.time > this.doorMoveBeginTime + this.openBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorEndOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration + this.openEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
			}
			break;
		}
		GRElevator.ElevatorState elevatorState = this.state;
		Transform transform;
		Transform transform2;
		float num;
		if (elevatorState != GRElevator.ElevatorState.DoorMovingClosing)
		{
			if (elevatorState == GRElevator.ElevatorState.DoorMovingOpening)
			{
				transform = this.openTargetTop;
				transform2 = this.openTargetBottom;
				num = this.doorOpenSpeed;
			}
			else
			{
				transform = this.upperDoor;
				transform2 = this.lowerDoor;
				num = 1f;
			}
		}
		else
		{
			transform = this.closedTargetTop;
			transform2 = this.closedTargetBottom;
			num = this.doorCloseSpeed;
		}
		this.upperDoor.position = Vector3.MoveTowards(this.upperDoor.position, transform.position, Time.deltaTime * num);
		this.lowerDoor.position = Vector3.MoveTowards(this.lowerDoor.position, transform2.position, Time.deltaTime * num);
	}

	// Token: 0x040037C2 RID: 14274
	public GRElevatorManager.ElevatorLocation location;

	// Token: 0x040037C3 RID: 14275
	public Transform upperDoor;

	// Token: 0x040037C4 RID: 14276
	public Transform lowerDoor;

	// Token: 0x040037C5 RID: 14277
	public Transform closedTargetTop;

	// Token: 0x040037C6 RID: 14278
	public Transform closedTargetBottom;

	// Token: 0x040037C7 RID: 14279
	public Transform openTargetTop;

	// Token: 0x040037C8 RID: 14280
	public Transform openTargetBottom;

	// Token: 0x040037C9 RID: 14281
	public TextMeshPro outerText;

	// Token: 0x040037CA RID: 14282
	public TextMeshPro innerText;

	// Token: 0x040037CB RID: 14283
	public List<GRElevatorButton> elevatorButtons;

	// Token: 0x040037CC RID: 14284
	private Dictionary<GRElevator.ButtonType, GRElevatorButton> typeButtonDict;

	// Token: 0x040037CD RID: 14285
	public GorillaFriendCollider friendCollider;

	// Token: 0x040037CE RID: 14286
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x040037CF RID: 14287
	public SoundBankPlayer buttonBank;

	// Token: 0x040037D0 RID: 14288
	public AudioSource doorAudio;

	// Token: 0x040037D1 RID: 14289
	public AudioSource ambientAudio;

	// Token: 0x040037D2 RID: 14290
	public AudioSource musicAudio;

	// Token: 0x040037D3 RID: 14291
	public AudioClip travellingLoopClip;

	// Token: 0x040037D4 RID: 14292
	public AudioClip ambientLoopClip;

	// Token: 0x040037D5 RID: 14293
	public AudioClip dingClip;

	// Token: 0x040037D6 RID: 14294
	public AudioClip doorOpenClip;

	// Token: 0x040037D7 RID: 14295
	public AudioClip doorCloseClip;

	// Token: 0x040037D8 RID: 14296
	public float adjustedOffsetTime;

	// Token: 0x040037D9 RID: 14297
	public float doorMoveBeginTime;

	// Token: 0x040037DA RID: 14298
	public float doorOpenSpeed = 0.5f;

	// Token: 0x040037DB RID: 14299
	public float doorCloseSpeed = 0.5f;

	// Token: 0x040037DC RID: 14300
	public float closeBeginDuration;

	// Token: 0x040037DD RID: 14301
	public float closeTravelDuration;

	// Token: 0x040037DE RID: 14302
	public float closeEndDuration;

	// Token: 0x040037DF RID: 14303
	public float openBeginDuration;

	// Token: 0x040037E0 RID: 14304
	public float openTravelDuration;

	// Token: 0x040037E1 RID: 14305
	public float openEndDuration;

	// Token: 0x040037E2 RID: 14306
	public float travelDistance;

	// Token: 0x040037E3 RID: 14307
	public GRElevator.ElevatorState state;

	// Token: 0x040037E4 RID: 14308
	public GameObject collidersAndVisuals;

	// Token: 0x020006A1 RID: 1697
	public enum ElevatorState
	{
		// Token: 0x040037E6 RID: 14310
		DoorBeginClosing,
		// Token: 0x040037E7 RID: 14311
		DoorMovingClosing,
		// Token: 0x040037E8 RID: 14312
		DoorEndClosing,
		// Token: 0x040037E9 RID: 14313
		DoorClosed,
		// Token: 0x040037EA RID: 14314
		DoorBeginOpening,
		// Token: 0x040037EB RID: 14315
		DoorMovingOpening,
		// Token: 0x040037EC RID: 14316
		DoorEndOpening,
		// Token: 0x040037ED RID: 14317
		DoorOpen,
		// Token: 0x040037EE RID: 14318
		None
	}

	// Token: 0x020006A2 RID: 1698
	[Serializable]
	public enum ButtonType
	{
		// Token: 0x040037F0 RID: 14320
		Stump = 1,
		// Token: 0x040037F1 RID: 14321
		City,
		// Token: 0x040037F2 RID: 14322
		GhostReactor,
		// Token: 0x040037F3 RID: 14323
		Open,
		// Token: 0x040037F4 RID: 14324
		Close,
		// Token: 0x040037F5 RID: 14325
		Summon,
		// Token: 0x040037F6 RID: 14326
		MonkeBlocks,
		// Token: 0x040037F7 RID: 14327
		Count
	}
}
