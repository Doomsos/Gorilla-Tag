using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class SIGadgetStilt : SIGadget
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060005FB RID: 1531 RVA: 0x00021CB2 File Offset: 0x0001FEB2
	// (set) Token: 0x060005FC RID: 1532 RVA: 0x00021CBA File Offset: 0x0001FEBA
	public bool TriggerToExtend { get; private set; }

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060005FD RID: 1533 RVA: 0x00021CC3 File Offset: 0x0001FEC3
	// (set) Token: 0x060005FE RID: 1534 RVA: 0x00021CCB File Offset: 0x0001FECB
	public bool hasMotor { get; private set; }

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060005FF RID: 1535 RVA: 0x00021CD4 File Offset: 0x0001FED4
	// (set) Token: 0x06000600 RID: 1536 RVA: 0x00021CDC File Offset: 0x0001FEDC
	public bool StickToAdjustLength { get; private set; }

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000601 RID: 1537 RVA: 0x00021CE5 File Offset: 0x0001FEE5
	// (set) Token: 0x06000602 RID: 1538 RVA: 0x00021CED File Offset: 0x0001FEED
	public bool CanTag { get; private set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000603 RID: 1539 RVA: 0x00021CF6 File Offset: 0x0001FEF6
	// (set) Token: 0x06000604 RID: 1540 RVA: 0x00021CFE File Offset: 0x0001FEFE
	public bool CanStun { get; private set; }

	// Token: 0x06000605 RID: 1541 RVA: 0x00021D08 File Offset: 0x0001FF08
	private void Awake()
	{
		this.tipDefaultOffset = this.tip.transform.localPosition;
		this.hasMotor = (this.motorTransform != null);
		this.hasEndB = (this.stiltEndB != null);
		this.hasEndC = (this.stiltEndC != null);
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00021E14 File Offset: 0x00020014
	private void DisableCurrentStilt()
	{
		if (this.currentStiltID != StiltID.None)
		{
			GTPlayer.Instance.DisableStilt(this.currentStiltID);
			this.currentStiltID = StiltID.None;
		}
		if (this.currentStiltIDB != StiltID.None)
		{
			GTPlayer.Instance.DisableStilt(this.currentStiltIDB);
			this.currentStiltIDB = StiltID.None;
		}
		if (this.currentStiltIDC != StiltID.None)
		{
			GTPlayer.Instance.DisableStilt(this.currentStiltIDC);
			this.currentStiltIDC = StiltID.None;
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00021E84 File Offset: 0x00020084
	private void OnGrabbed()
	{
		this.DisableCurrentStilt();
		this.HandleStartInteraction();
		if (this.IsEquippedLocal())
		{
			this.activatedLocally = true;
			if (this.gameEntity.heldByHandIndex == 0)
			{
				this.currentStiltID = StiltID.Held_Left;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, true, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Held_Left2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, true, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Held_Left3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, true, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
			}
			else
			{
				this.currentStiltID = StiltID.Held_Right;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, false, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Held_Right2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, false, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Held_Right3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, false, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
			}
		}
		else
		{
			this.activatedLocally = false;
		}
		this.wasSnappedByLocalJoint = SnapJointType.None;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x0002205C File Offset: 0x0002025C
	private void OnReleased()
	{
		this.DisableCurrentStilt();
		this.HandleStopInteraction();
		if (this.gameEntity.WasLastHeldByLocalPlayer() && this.TriggerToExtend && !Mathf.Approximately(this.targetLength, this.retractedLength))
		{
			this.targetLength = this.retractedLength;
			this.gameEntity.RequestState(this.gameEntity.id, this.PackStateForNetwork());
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x000220C8 File Offset: 0x000202C8
	private void OnSnapped()
	{
		this.DisableCurrentStilt();
		this.HandleStartInteraction();
		if (this.IsEquippedLocal())
		{
			this.wasSnappedByLocalJoint = this.gameEntity.snappedJoint;
			if (this.wasSnappedByLocalJoint == SnapJointType.HandL)
			{
				this.currentStiltID = StiltID.Snapped_Left;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, true, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Snapped_Left2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, true, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Snapped_Left3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, true, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
					return;
				}
			}
			else if (this.wasSnappedByLocalJoint == SnapJointType.HandR)
			{
				this.currentStiltID = StiltID.Snapped_Right;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, false, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Snapped_Right2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, false, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Snapped_Right3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, false, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
					return;
				}
			}
		}
		else
		{
			this.wasSnappedByLocalJoint = SnapJointType.None;
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x000222A5 File Offset: 0x000204A5
	private void OnUnsnapped()
	{
		this.DisableCurrentStilt();
		this.HandleStopInteraction();
		if (this.wasSnappedByLocalJoint == SnapJointType.HandL)
		{
			this.wasSnappedByLocalJoint = SnapJointType.None;
			return;
		}
		if (this.wasSnappedByLocalJoint == SnapJointType.HandR)
		{
			this.wasSnappedByLocalJoint = SnapJointType.None;
		}
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000222D4 File Offset: 0x000204D4
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.DisableCurrentStilt();
		if (this.attachedVRRig != null)
		{
			VRRig vrrig = this.attachedVRRig;
			vrrig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(vrrig.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00022324 File Offset: 0x00020524
	protected override void OnUpdateAuthority(float dt)
	{
		bool isSpinning = this.IsSpinning;
		bool flag = false;
		if (this.currentStiltID != StiltID.None)
		{
			bool flag2 = !this.TriggerToExtend || this.CheckInput();
			this.IsSpinning = (this.hasMotor && this.CheckInput());
			bool flag3 = false;
			float oldLength = this.targetLength;
			if (this.IsSpinning)
			{
				this.SpinMotor(dt);
				flag = true;
			}
			if (flag2)
			{
				if (this.StickToAdjustLength)
				{
					Vector2 joystickInput = base.GetJoystickInput();
					if (Mathf.Abs(joystickInput.y) > 0.75f && Mathf.Abs(joystickInput.x) < 0.5f)
					{
						this.currentExtendedLength = Mathf.Clamp(this.currentExtendedLength + joystickInput.y * this.lengthChangeSpeed * Time.deltaTime, this.retractedLength, this.maxLength);
					}
				}
				if (!Mathf.Approximately(this.targetLength, this.currentExtendedLength))
				{
					this.targetLength = this.currentExtendedLength;
				}
				if (!Mathf.Approximately(this.targetLength, this.lastSentLength) && Time.time > this.nextAdjustmentSendTime)
				{
					this.nextAdjustmentSendTime = Time.time + this.adjustmentSendRate;
					this.lastSentLength = this.targetLength;
					flag3 = true;
				}
			}
			else if (!Mathf.Approximately(this.targetLength, this.retractedLength))
			{
				this.targetLength = this.retractedLength;
				this.lastSentLength = this.targetLength;
				flag3 = true;
			}
			if (flag3 || this.IsSpinning != isSpinning)
			{
				this.CheckPlaySounds(oldLength, this.targetLength);
				this.gameEntity.RequestState(this.gameEntity.id, this.PackStateForNetwork());
			}
		}
		if (this.hasMotor && !flag && this.motorAudio.isPlaying)
		{
			this.motorAudio.Stop();
		}
		isSpinning = this.IsSpinning;
		this.UpdateEndPoints(this.IsSpinning);
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x000224F0 File Offset: 0x000206F0
	private long PackStateForNetwork()
	{
		long num = 0L;
		if (this.IsSpinning)
		{
			num |= 1L;
		}
		else if (this.hasMotor)
		{
			long num2 = (long)Mathf.RoundToInt(this.currentMotorAngle);
			num |= num2 << 1;
		}
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(this.targetLength * 1000f), 0, 3000);
		return num | num3 << 10;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00022550 File Offset: 0x00020750
	private void UnpackStateFromNetwork(long state)
	{
		this.IsSpinning = ((state & 1L) != 0L);
		if (this.hasMotor && !this.IsSpinning)
		{
			this.currentMotorAngle = (float)(state >> 1 & 511L);
			this.motorTransform.localRotation = Quaternion.AngleAxis(this.currentMotorAngle, Vector3.right);
		}
		int num = (int)(state >> 10 & 4095L);
		this.targetLength = Mathf.Clamp((float)num * 0.001f, this.retractedLength, this.maxLength);
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x000225D4 File Offset: 0x000207D4
	private void SpinMotor(float dt)
	{
		this.currentMotorAngle = (this.currentMotorAngle + this.rotateSpeedFactor * dt) % 360f;
		this.motorTransform.localRotation = Quaternion.AngleAxis(this.currentMotorAngle, Vector3.right);
		if (!this.motorAudio.isPlaying)
		{
			this.motorAudio.Play();
		}
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00022630 File Offset: 0x00020830
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		if (this.hasMotor)
		{
			if (this.IsSpinning && (this.gameEntity.heldByActorNumber >= 0 || this.gameEntity.snappedByActorNumber >= 0))
			{
				this.SpinMotor(dt);
			}
			else if (this.motorAudio.isPlaying)
			{
				this.motorAudio.Stop();
			}
		}
		this.UpdateEndPoints(false);
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00022698 File Offset: 0x00020898
	private bool CheckInput()
	{
		return this.buttonActivatable.CheckInput(true, true, 0.25f, true, true);
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x000226B0 File Offset: 0x000208B0
	public override SIUpgradeSet FilterUpgradeNodes(SIUpgradeSet upgrades)
	{
		if (this.restrictedUpgrades.Length == 0)
		{
			return upgrades;
		}
		SIUpgradeSet result = default(SIUpgradeSet);
		foreach (SIUpgradeType upgrade in this.restrictedUpgrades)
		{
			if (upgrades.Contains(upgrade))
			{
				result.Add(upgrade);
			}
		}
		return result;
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x000226FC File Offset: 0x000208FC
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.CanTag = withUpgrades.Contains(SIUpgradeType.Stilt_Tag_Tip);
		this.CanStun = withUpgrades.Contains(SIUpgradeType.Stilt_Stun_Tip);
		this.TriggerToExtend = (this.buttonActivatable != null && withUpgrades.Contains(SIUpgradeType.Stilt_Retractable));
		this.StickToAdjustLength = (this.TriggerToExtend && withUpgrades.Contains(SIUpgradeType.Stilt_Adjustable_Length));
		this.extendSpeed = (withUpgrades.Contains(SIUpgradeType.Stilt_Retract_Speed) ? this.extendSpeedUpgraded : this.extendSpeedNormal);
		this.retractSpeed = (withUpgrades.Contains(SIUpgradeType.Stilt_Retract_Speed) ? this.retractSpeedUpgraded : this.retractSpeedNormal);
		this.maxLength = ((this.TriggerToExtend && withUpgrades.Contains(SIUpgradeType.Stilt_Max_Length)) ? this.maxLengthUpgraded : this.maxLengthNormal);
		this.currentExtendedLength = this.maxLength;
		this.targetLength = (this.TriggerToExtend ? this.retractedLength : this.currentExtendedLength);
		this.currentLength = this.targetLength;
		this.ApplyCurrentLength();
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00022800 File Offset: 0x00020A00
	private void UpdateEndPoints(bool force)
	{
		if (!force && Mathf.Approximately(this.currentLength, this.targetLength))
		{
			return;
		}
		float num = (this.targetLength > this.currentLength) ? this.extendSpeed : this.retractSpeed;
		this.currentLength = Mathf.MoveTowards(this.currentLength, this.targetLength, num * Time.deltaTime);
		this.ApplyCurrentLength();
		if (this.currentStiltID != StiltID.None)
		{
			GTPlayer.Instance.UpdateStiltOffset(this.currentStiltID, this.stiltEnd.position);
		}
		if (this.currentStiltIDB != StiltID.None)
		{
			GTPlayer.Instance.UpdateStiltOffset(this.currentStiltIDB, this.stiltEndB.position);
		}
		if (this.currentStiltIDC != StiltID.None)
		{
			GTPlayer.Instance.UpdateStiltOffset(this.currentStiltIDC, this.stiltEndC.position);
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x000228D4 File Offset: 0x00020AD4
	private void ApplyCurrentLength()
	{
		this.tip.transform.localPosition = this.offsetDir * this.currentLength + this.tipDefaultOffset;
		Vector3 localScale = this.midpoint.transform.localScale;
		localScale.z = this.currentLength;
		this.midpoint.transform.localScale = localScale;
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0002293C File Offset: 0x00020B3C
	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal())
		{
			return;
		}
		float oldLength = this.targetLength;
		this.UnpackStateFromNetwork(newState);
		this.CheckPlaySounds(oldLength, this.targetLength);
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0002296D File Offset: 0x00020B6D
	private void CheckPlaySounds(float oldLength, float newLength)
	{
		if (Mathf.Approximately(oldLength, newLength))
		{
			return;
		}
		if (Mathf.Approximately(newLength, this.retractedLength))
		{
			this.retractSoundBank.Play();
			return;
		}
		if (Mathf.Approximately(oldLength, this.retractedLength))
		{
			this.extendSoundBank.Play();
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x000229AC File Offset: 0x00020BAC
	private void HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.attachedPlayerActorNr = base.GetAttachedPlayerActorNumber();
		this.attachedNetPlayer = NetworkSystem.Instance.GetPlayer(this.attachedPlayerActorNr);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		if (this.attachedVRRig != null)
		{
			VRRig vrrig = this.attachedVRRig;
			vrrig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(vrrig.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		}
		this.attachedVRRig = gamePlayer.rig;
		VRRig vrrig2 = this.attachedVRRig;
		vrrig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(vrrig2.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		int num = this.isTagged ? 2 : 0;
		if (num != this.attachedVRRig.setMatIndex)
		{
			this.HandleVRRigMaterialIndexChanged(num, this.attachedVRRig.setMatIndex);
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00022A88 File Offset: 0x00020C88
	private void HandleStopInteraction()
	{
		this.attachedPlayerActorNr = -1;
		this.attachedNetPlayer = null;
		if (this.attachedVRRig != null)
		{
			VRRig vrrig = this.attachedVRRig;
			vrrig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(vrrig.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		}
		this.attachedVRRig = null;
		if (this.isTagged)
		{
			this.HandleVRRigMaterialIndexChanged(2, 0);
		}
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x00022AF0 File Offset: 0x00020CF0
	private void HandleVRRigMaterialIndexChanged(int oldMatIndex, int newMatIndex)
	{
		if (this.attachedPlayerActorNr != -1 && (newMatIndex == 2 || newMatIndex == 1) && this.CanTag)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				this.isTagged = (this.attachedNetPlayer != null && superInfectionGame.IsInfected(this.attachedNetPlayer));
				if (this.matDest)
				{
					this.matDest.sharedMaterial = this.tagActivatedMat;
				}
				if (this.skinnedMatDest)
				{
					this.skinnedMatDest.sharedMaterial = this.tagActivatedMat;
					goto IL_C5;
				}
				goto IL_C5;
			}
		}
		this.isTagged = false;
		if (this.matDest)
		{
			this.matDest.sharedMaterial = this.defaultMat;
		}
		if (this.skinnedMatDest)
		{
			this.skinnedMatDest.sharedMaterial = this.defaultMat;
		}
		IL_C5:
		GameObject[] array = this.tagActivatedObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(this.isTagged);
		}
	}

	// Token: 0x04000766 RID: 1894
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x04000767 RID: 1895
	public GameObject tip;

	// Token: 0x04000768 RID: 1896
	[SerializeField]
	private Vector3 offsetDir = Vector3.forward;

	// Token: 0x04000769 RID: 1897
	private Vector3 tipDefaultOffset;

	// Token: 0x0400076A RID: 1898
	public GameObject midpoint;

	// Token: 0x0400076B RID: 1899
	public Transform stiltEnd;

	// Token: 0x0400076C RID: 1900
	private bool hasEndB;

	// Token: 0x0400076D RID: 1901
	public Transform stiltEndB;

	// Token: 0x0400076E RID: 1902
	private bool hasEndC;

	// Token: 0x0400076F RID: 1903
	public Transform stiltEndC;

	// Token: 0x04000770 RID: 1904
	public Transform motorTransform;

	// Token: 0x04000771 RID: 1905
	[SerializeField]
	private AudioSource motorAudio;

	// Token: 0x04000772 RID: 1906
	[SerializeField]
	private SIUpgradeType[] restrictedUpgrades;

	// Token: 0x04000773 RID: 1907
	[SerializeField]
	private float maxLengthNormal;

	// Token: 0x04000774 RID: 1908
	[SerializeField]
	private float maxLengthUpgraded;

	// Token: 0x04000775 RID: 1909
	[SerializeField]
	private float retractedLength;

	// Token: 0x04000776 RID: 1910
	[SerializeField]
	private float lengthChangeSpeed;

	// Token: 0x04000777 RID: 1911
	[SerializeField]
	private float maxArmLength;

	// Token: 0x04000778 RID: 1912
	[SerializeField]
	private float extendSpeedNormal;

	// Token: 0x04000779 RID: 1913
	[SerializeField]
	private float extendSpeedUpgraded;

	// Token: 0x0400077A RID: 1914
	[SerializeField]
	private float retractSpeedNormal;

	// Token: 0x0400077B RID: 1915
	[SerializeField]
	private float retractSpeedUpgraded;

	// Token: 0x0400077C RID: 1916
	[SerializeField]
	private float rotateSpeedFactor;

	// Token: 0x0400077D RID: 1917
	[SerializeField]
	private SoundBankPlayer retractSoundBank;

	// Token: 0x0400077E RID: 1918
	[SerializeField]
	private SoundBankPlayer extendSoundBank;

	// Token: 0x0400077F RID: 1919
	[SerializeField]
	private Material defaultMat;

	// Token: 0x04000780 RID: 1920
	[SerializeField]
	private Material tagActivatedMat;

	// Token: 0x04000781 RID: 1921
	[SerializeField]
	private GameObject[] tagActivatedObjects;

	// Token: 0x04000782 RID: 1922
	[SerializeField]
	private MeshRenderer matDest;

	// Token: 0x04000783 RID: 1923
	[SerializeField]
	private SkinnedMeshRenderer skinnedMatDest;

	// Token: 0x04000784 RID: 1924
	private float currentExtendedLength;

	// Token: 0x0400078A RID: 1930
	private float targetLength;

	// Token: 0x0400078B RID: 1931
	private float currentLength;

	// Token: 0x0400078C RID: 1932
	private float maxLength;

	// Token: 0x0400078D RID: 1933
	private float extendSpeed;

	// Token: 0x0400078E RID: 1934
	private float retractSpeed;

	// Token: 0x0400078F RID: 1935
	private float currentMotorAngle;

	// Token: 0x04000790 RID: 1936
	private float adjustmentSendRate = 0.25f;

	// Token: 0x04000791 RID: 1937
	private float lastSentLength;

	// Token: 0x04000792 RID: 1938
	private float nextAdjustmentSendTime = -1f;

	// Token: 0x04000793 RID: 1939
	private bool IsSpinning;

	// Token: 0x04000794 RID: 1940
	private StiltID currentStiltID = StiltID.None;

	// Token: 0x04000795 RID: 1941
	private StiltID currentStiltIDB = StiltID.None;

	// Token: 0x04000796 RID: 1942
	private StiltID currentStiltIDC = StiltID.None;

	// Token: 0x04000797 RID: 1943
	private SnapJointType wasSnappedByLocalJoint;

	// Token: 0x04000798 RID: 1944
	private const long IsSpinningBit = 1L;

	// Token: 0x04000799 RID: 1945
	private int attachedPlayerActorNr = int.MinValue;

	// Token: 0x0400079A RID: 1946
	private NetPlayer attachedNetPlayer;

	// Token: 0x0400079B RID: 1947
	private VRRig attachedVRRig;

	// Token: 0x0400079C RID: 1948
	private bool isTagged;
}
