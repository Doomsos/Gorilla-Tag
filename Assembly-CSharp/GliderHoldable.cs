using System;
using System.Collections;
using System.Runtime.InteropServices;
using AA;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000BA6 RID: 2982
[RequireComponent(typeof(Rigidbody))]
[NetworkBehaviourWeaved(11)]
public class GliderHoldable : NetworkHoldableObject, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x170006DA RID: 1754
	// (get) Token: 0x06004994 RID: 18836 RVA: 0x00181A64 File Offset: 0x0017FC64
	private bool OutOfBounds
	{
		get
		{
			return this.maxDistanceRespawnOrigin != null && (this.maxDistanceRespawnOrigin.position - base.transform.position).sqrMagnitude > this.maxDistanceBeforeRespawn * this.maxDistanceBeforeRespawn;
		}
	}

	// Token: 0x06004995 RID: 18837 RVA: 0x00181AB4 File Offset: 0x0017FCB4
	protected override void Awake()
	{
		base.Awake();
		base.transform.parent = null;
		this.defaultMaxDistanceBeforeRespawn = this.maxDistanceBeforeRespawn;
		this.spawnPosition = (this.skyJungleSpawnPostion = base.transform.position);
		this.spawnRotation = (this.skyJungleSpawnRotation = base.transform.rotation);
		this.skyJungleRespawnOrigin = this.maxDistanceRespawnOrigin;
		this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		this.rb = base.GetComponent<Rigidbody>();
		this.yaw = base.transform.rotation.eulerAngles.y;
		this.oneHandRotationRateExp = Mathf.Exp(this.oneHandHoldRotationRate);
		this.twoHandRotationRateExp = Mathf.Exp(this.twoHandHoldRotationRate);
		this.subtlePlayerPitchRateExp = Mathf.Exp(this.subtlePlayerPitchRate);
		this.subtlePlayerRollRateExp = Mathf.Exp(this.subtlePlayerRollRate);
		this.accelSmoothingFollowRateExp = Mathf.Exp(this.accelSmoothingFollowRate);
		this.networkSyncFollowRateExp = Mathf.Exp(this.networkSyncFollowRate);
		this.ownershipGuard.AddCallbackTarget(this);
		this.calmAudio.volume = 0f;
		this.activeAudio.volume = 0f;
		this.whistlingAudio.volume = 0f;
	}

	// Token: 0x06004996 RID: 18838 RVA: 0x00181C02 File Offset: 0x0017FE02
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.ownershipGuard != null)
		{
			this.ownershipGuard.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06004997 RID: 18839 RVA: 0x000DBAF8 File Offset: 0x000D9CF8
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x06004998 RID: 18840 RVA: 0x00181C24 File Offset: 0x0017FE24
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.Respawn();
		base.OnDisable();
	}

	// Token: 0x06004999 RID: 18841 RVA: 0x00181C38 File Offset: 0x0017FE38
	public void Respawn()
	{
		if ((base.IsValid && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			if (EquipmentInteractor.instance != null)
			{
				if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.leftHand);
				}
				if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.rightHand);
				}
			}
			this.rb.isKinematic = true;
			base.transform.position = this.spawnPosition;
			base.transform.rotation = this.spawnRotation;
			this.lastHeldTime = -1f;
			this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		}
	}

	// Token: 0x0600499A RID: 18842 RVA: 0x00181D09 File Offset: 0x0017FF09
	public void CustomMapLoad(Transform placeholderTransform, float respawnDistance)
	{
		this.maxDistanceRespawnOrigin = placeholderTransform;
		this.spawnPosition = placeholderTransform.position;
		this.spawnRotation = placeholderTransform.rotation;
		this.maxDistanceBeforeRespawn = respawnDistance;
		this.Respawn();
	}

	// Token: 0x0600499B RID: 18843 RVA: 0x00181D37 File Offset: 0x0017FF37
	public void CustomMapUnload()
	{
		this.maxDistanceRespawnOrigin = this.skyJungleRespawnOrigin;
		this.spawnPosition = this.skyJungleSpawnPostion;
		this.spawnRotation = this.skyJungleSpawnRotation;
		this.maxDistanceBeforeRespawn = this.defaultMaxDistanceBeforeRespawn;
		this.Respawn();
	}

	// Token: 0x170006DB RID: 1755
	// (get) Token: 0x0600499C RID: 18844 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool TwoHanded
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600499D RID: 18845 RVA: 0x00181D70 File Offset: 0x0017FF70
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
		}
	}

	// Token: 0x0600499E RID: 18846 RVA: 0x00181DEC File Offset: 0x0017FFEC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
			return;
		}
		if (NetworkSystem.Instance.InRoom && !base.IsMine && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
		}
	}

	// Token: 0x0600499F RID: 18847 RVA: 0x00181E94 File Offset: 0x00180094
	public void OnGrabAuthority(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if ((flag && !EquipmentInteractor.instance.isLeftGrabbing) || (!flag && !EquipmentInteractor.instance.isRightGrabbing))
		{
			return;
		}
		if (this.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		Vector3 worldGrabPoint = this.ClosestPointInHandle(grabbingHand.transform.position, pointGrabbed);
		if (flag)
		{
			this.leftHold.Activate(grabbingHand.transform, base.transform, worldGrabPoint);
		}
		else
		{
			this.rightHold.Activate(grabbingHand.transform, base.transform, worldGrabPoint);
		}
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 handsVector = this.GetHandsVector(this.leftHold.transform.position, this.rightHold.transform.position, GTPlayer.Instance.headCollider.transform.position, true);
			this.twoHandRotationOffsetAxis = Vector3.Cross(handsVector, base.transform.right).normalized;
			if ((double)this.twoHandRotationOffsetAxis.sqrMagnitude < 0.001)
			{
				this.twoHandRotationOffsetAxis = base.transform.right;
				this.twoHandRotationOffsetAngle = 0f;
			}
			else
			{
				this.twoHandRotationOffsetAngle = Vector3.SignedAngle(handsVector, base.transform.right, this.twoHandRotationOffsetAxis);
			}
		}
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.ridersMaterialOverideIndex = 0;
		if (this.cosmeticMaterialOverrides.Length != 0)
		{
			VRRig offlineVRRig = this.cachedRig;
			if (offlineVRRig == null)
			{
				offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			}
			if (offlineVRRig != null)
			{
				for (int i = 0; i < this.cosmeticMaterialOverrides.Length; i++)
				{
					if (this.cosmeticMaterialOverrides[i].cosmeticName != null && offlineVRRig.cosmeticSet != null && offlineVRRig.cosmeticSet.HasItem(this.cosmeticMaterialOverrides[i].cosmeticName))
					{
						this.ridersMaterialOverideIndex = i + 1;
						break;
					}
				}
			}
		}
		this.infectedState = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			this.infectedState = this.syncedState.tagged;
		}
		if (this.infectedState)
		{
			this.leafMesh.material = this.GetInfectedMaterial();
		}
		else
		{
			this.leafMesh.material = this.GetMaterialFromIndex((byte)this.ridersMaterialOverideIndex);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != EquipmentInteractor.instance.rightHandHeldEquipment)
		{
			this.holdingTwoGliders = true;
		}
	}

	// Token: 0x060049A0 RID: 18848 RVA: 0x00182220 File Offset: 0x00180420
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		this.holdingTwoGliders = false;
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		if (this.leftHold.active && this.rightHold.active)
		{
			if (flag)
			{
				this.rightHold.Activate(this.rightHold.transform, base.transform, this.ClosestPointInHandle(this.rightHold.transform.position, this.handle));
			}
			else
			{
				this.leftHold.Activate(this.leftHold.transform, base.transform, this.ClosestPointInHandle(this.leftHold.transform.position, this.handle));
			}
		}
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
		(flag ? this.leftHold : this.rightHold).Deactivate();
		EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.audioLevel = 0f;
			this.riderId = -1;
			this.cachedRig = null;
			this.subtlePlayerPitch = 0f;
			this.subtlePlayerRoll = 0f;
			this.leftHoldPositionLocal = default(Vector3?);
			this.rightHoldPositionLocal = default(Vector3?);
			this.ridersMaterialOverideIndex = 0;
			if (base.IsMine || !NetworkSystem.Instance.InRoom)
			{
				this.rb.isKinematic = false;
				this.rb.useGravity = true;
				this.rb.linearVelocity = averageVelocity;
				this.syncedState.riderId = -1;
				this.syncedState.tagged = false;
				this.syncedState.materialIndex = 0;
				this.syncedState.position = base.transform.position;
				this.syncedState.rotation = base.transform.rotation;
				this.syncedState.audioLevel = 0;
			}
			this.leafMesh.material = this.baseLeafMaterial;
		}
		return true;
	}

	// Token: 0x060049A1 RID: 18849 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060049A2 RID: 18850 RVA: 0x00182438 File Offset: 0x00180638
	public void FixedUpdate()
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (this.holdingTwoGliders)
		{
			instance.AddForce(Physics.gravity, 5);
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.previousVelocity = this.currentVelocity;
			this.currentVelocity = instance.RigidbodyVelocity;
			float magnitude = this.currentVelocity.magnitude;
			this.accelerationAverage.AddSample((this.currentVelocity - this.previousVelocity) / Time.fixedDeltaTime, Time.fixedTime);
			float rollAngle180Wrapping = this.GetRollAngle180Wrapping();
			float num = this.liftIncreaseVsRoll.Evaluate(Mathf.Clamp01(Mathf.Abs(rollAngle180Wrapping / 180f))) * this.liftIncreaseVsRollMaxAngle;
			Vector3 vector = Vector3.RotateTowards(this.currentVelocity, Quaternion.AngleAxis(num, -base.transform.right) * base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime);
			Vector3 vector2 = vector - this.currentVelocity;
			float num2 = this.NormalizeAngle180(Vector3.SignedAngle(Vector3.ProjectOnPlane(this.currentVelocity, base.transform.right), base.transform.forward, base.transform.right));
			if (num2 > 90f)
			{
				num2 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num2));
			}
			else if (num2 < -90f)
			{
				num2 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num2));
			}
			float num3 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, num2));
			Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, this.pitch));
			float num4 = this.liftVsAttack.Evaluate(num3);
			instance.AddForce(vector2 * num4, 2);
			float num5 = this.dragVsAttack.Evaluate(num3);
			float num6 = (this.syncedState.riderId != -1 && this.syncedState.materialIndex == 1) ? (this.dragVsSpeedMaxSpeed + this.infectedSpeedIncrease) : this.dragVsSpeedMaxSpeed;
			float num7 = this.dragVsSpeed.Evaluate(Mathf.Clamp01(magnitude / num6));
			float num8 = Mathf.Clamp01(num5 * this.attackDragFactor + num7 * this.dragVsSpeedDragFactor);
			instance.AddForce(-this.currentVelocity * num8, 5);
			if (this.pitch > 0f && this.currentVelocity.y > 0f && (this.currentVelocity - this.previousVelocity).y > 0f)
			{
				float num9 = Mathf.InverseLerp(0f, this.pullUpLiftActivationVelocity, this.currentVelocity.y);
				float num10 = Mathf.InverseLerp(0f, this.pullUpLiftActivationAcceleration, (this.currentVelocity - this.previousVelocity).y / fixedDeltaTime);
				float num11 = Mathf.Min(num9, num10);
				instance.AddForce(-Physics.gravity * this.pullUpLiftBonus * num11, 5);
			}
			if (Vector3.Dot(vector, Physics.gravity) > 0f)
			{
				instance.AddForce(-Physics.gravity * this.gravityCompensation, 5);
				return;
			}
		}
		else
		{
			Vector3 vector3 = this.WindResistanceForceOffset(base.transform.up, Vector3.down);
			Vector3 vector4 = base.transform.position - vector3 * this.gravityUprightTorqueMultiplier;
			this.rb.AddForceAtPosition(-this.fallingGravityReduction * Physics.gravity * this.rb.mass, vector4, 0);
		}
	}

	// Token: 0x060049A3 RID: 18851 RVA: 0x00182840 File Offset: 0x00180A40
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.AuthorityUpdate(deltaTime);
			return;
		}
		this.RemoteSyncUpdate(deltaTime);
	}

	// Token: 0x060049A4 RID: 18852 RVA: 0x00182880 File Offset: 0x00180A80
	private void AuthorityUpdate(float dt)
	{
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.AuthorityUpdateUnheld(dt);
		}
		else if (this.leftHold.active || this.rightHold.active)
		{
			this.AuthorityUpdateHeld(dt);
		}
		this.syncedState.audioLevel = (byte)Mathf.FloorToInt(255f * this.audioLevel);
	}

	// Token: 0x060049A5 RID: 18853 RVA: 0x001828F0 File Offset: 0x00180AF0
	private void AuthorityUpdateHeld(float dt)
	{
		if (this.gliderState != GliderHoldable.GliderState.LocallyHeld)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyHeld;
		}
		this.rb.isKinematic = true;
		this.lastHeldTime = Time.time;
		if (this.leftHold.active)
		{
			this.leftHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.leftHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		if (this.rightHold.active)
		{
			this.rightHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.rightHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		Vector3 vector = Vector3.zero;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector = (this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos)) * 0.5f;
		}
		else if (this.leftHold.active)
		{
			vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos);
		}
		else if (this.rightHold.active)
		{
			vector = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos);
		}
		this.UpdateGliderPosition();
		float magnitude = this.currentVelocity.magnitude;
		if (this.setMaxHandSlipDuringFlight && magnitude > this.maxSlipOverrideSpeedThreshold)
		{
			if (this.leftHold.active)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
			}
			if (this.rightHold.active)
			{
				GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			}
		}
		bool flag = false;
		GorillaTagManager gorillaTagManager = GorillaGameManager.instance as GorillaTagManager;
		if (gorillaTagManager != null)
		{
			flag = gorillaTagManager.IsInfected(NetworkSystem.Instance.LocalPlayer);
		}
		bool flag2 = flag != this.infectedState;
		this.infectedState = flag;
		if (flag2)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		Vector3 average = this.accelerationAverage.GetAverage();
		this.accelerationSmoothed = Mathf.Lerp(average.magnitude, this.accelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
		float num = Mathf.InverseLerp(this.hapticMaxSpeedInputRange.x, this.hapticMaxSpeedInputRange.y, magnitude);
		float num2 = Mathf.InverseLerp(this.hapticAccelInputRange.x, this.hapticAccelInputRange.y, this.accelerationSmoothed);
		float num3 = Mathf.InverseLerp(this.hapticSpeedInputRange.x, this.hapticSpeedInputRange.y, magnitude);
		this.UpdateAudioSource(this.calmAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.activeAudio, num2 * num * this.audioVolumeMultiplier);
		if (this.infectedState)
		{
			this.UpdateAudioSource(this.whistlingAudio, Mathf.InverseLerp(this.whistlingAudioSpeedInputRange.x, this.whistlingAudioSpeedInputRange.y, magnitude) * num2 * num * this.audioVolumeMultiplier);
		}
		else
		{
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
		float amplitude = Mathf.Max(num2 * this.hapticAccelOutputMax * num, num3 * this.hapticSpeedOutputMax);
		if (this.rightHold.active)
		{
			GorillaTagger.Instance.DoVibration(5, amplitude, dt);
		}
		if (this.leftHold.active)
		{
			GorillaTagger.Instance.DoVibration(4, amplitude, dt);
		}
		Vector3 vector2 = this.handle.transform.position + this.handle.transform.rotation * new Vector3(0f, 0f, 1f);
		if (Time.frameCount % 2 == 0)
		{
			Vector3 vector3 = this.handle.transform.rotation * new Vector3(-0.707f, 0f, 0.707f);
			RaycastHit raycastHit;
			if (this.leftWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector3), ref raycastHit, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, 1))
			{
				this.leftWhooshStartTime = Time.time;
				this.leftWhooshHitPoint = raycastHit.point;
				this.leftWhooshAudio.GTStop();
				this.leftWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.leftWhooshAudio.GTPlay();
			}
		}
		else
		{
			Vector3 vector4 = this.handle.transform.rotation * new Vector3(0.707f, 0f, 0.707f);
			RaycastHit raycastHit2;
			if (this.rightWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector4), ref raycastHit2, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, 1))
			{
				this.rightWhooshStartTime = Time.time;
				this.rightWhooshHitPoint = raycastHit2.point;
				this.rightWhooshAudio.GTStop();
				this.rightWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.rightWhooshAudio.GTPlay();
			}
		}
		Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
		if (this.leftWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.leftWhooshAudio.transform.position = this.leftWhooshHitPoint;
		}
		else
		{
			this.leftWhooshAudio.transform.localPosition = new Vector3(-this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.rightWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.rightWhooshAudio.transform.position = this.rightWhooshHitPoint;
		}
		else
		{
			this.rightWhooshAudio.transform.localPosition = new Vector3(this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.extendTagRangeInFlight)
		{
			float tagRadiusOverrideThisFrame = Mathf.Lerp(this.tagRangeOutput.x, this.tagRangeOutput.y, Mathf.InverseLerp(this.tagRangeSpeedInput.x, this.tagRangeSpeedInput.y, magnitude));
			GorillaTagger.Instance.SetTagRadiusOverrideThisFrame(tagRadiusOverrideThisFrame);
			if (this.debugDrawTagRange)
			{
				GorillaTagger.Instance.DebugDrawTagCasts(Color.yellow);
			}
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized;
		Vector3 normalized2 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num4 = -Vector3.Dot(vector - this.handle.transform.position, normalized2);
		Vector3 vector5 = this.handle.transform.position - normalized2 * (this.riderPosRange.y * 0.5f + this.riderPosRangeOffset + num4);
		float num5 = Vector3.Dot(headCenterPosition - vector5, normalized);
		float num6 = Vector3.Dot(headCenterPosition - vector5, normalized2);
		num5 /= this.riderPosRange.x * 0.5f;
		num6 /= this.riderPosRange.y * 0.5f;
		this.riderPosition.x = Mathf.Sign(num5) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.x, 1f, Mathf.Abs(num5)));
		this.riderPosition.y = Mathf.Sign(num6) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.y, 1f, Mathf.Abs(num6)));
		Vector3 vector6;
		Vector3 vector7;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector6 = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			vector7 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
		}
		else if (this.leftHold.active)
		{
			vector6 = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			Vector3 vector8 = vector6 + this.leftHold.transform.forward * this.oneHandSimulatedHoldOffset.x;
			if (this.rightHoldPositionLocal != null)
			{
				this.rightHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector8), this.rightHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector7 = GTPlayer.Instance.transform.TransformPoint(this.rightHoldPositionLocal.Value);
			}
			else
			{
				vector7 = vector8;
				this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
			}
		}
		else
		{
			vector7 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
			Vector3 vector9 = vector7 + this.rightHold.transform.forward * this.oneHandSimulatedHoldOffset.x;
			if (this.leftHoldPositionLocal != null)
			{
				this.leftHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector9), this.leftHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector6 = GTPlayer.Instance.transform.TransformPoint(this.leftHoldPositionLocal.Value);
			}
			else
			{
				vector6 = vector9;
				this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			}
		}
		Vector3 vector10;
		Vector3 vector11;
		this.GetHandsOrientationVectors(vector6, vector7, GTPlayer.Instance.headCollider.transform, false, out vector10, out vector11);
		float num7 = this.riderPosition.y * this.riderPosDirectPitchMax;
		if (!this.leftHold.active || !this.rightHold.active)
		{
			num7 *= this.oneHandPitchMultiplier;
		}
		Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num7, 0f, this.pitchHalfLife, dt);
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
		Quaternion quaternion = Quaternion.AngleAxis(this.pitch, Vector3.right);
		this.twoHandRotationOffsetAngle = Mathf.Lerp(0f, this.twoHandRotationOffsetAngle, Mathf.Exp(-8f * dt));
		Vector3 vector12 = this.twoHandGliderInversionOnYawInsteadOfRoll ? vector11 : Vector3.up;
		Quaternion quaternion2 = Quaternion.AngleAxis(this.twoHandRotationOffsetAngle, this.twoHandRotationOffsetAxis) * Quaternion.LookRotation(vector10, vector12) * Quaternion.AngleAxis(-90f, Vector3.up);
		float num8 = (this.leftHold.active && this.rightHold.active) ? this.twoHandRotationRateExp : this.oneHandRotationRateExp;
		base.transform.rotation = Quaternion.Slerp(quaternion2 * quaternion, base.transform.rotation, Mathf.Exp(-num8 * dt));
		if (this.subtlePlayerPitchActive || this.subtlePlayerRollActive)
		{
			float num9 = Mathf.InverseLerp(this.subtlePlayerRotationSpeedRampMinMax.x, this.subtlePlayerRotationSpeedRampMinMax.y, this.currentVelocity.magnitude);
			Quaternion quaternion3 = Quaternion.identity;
			if (this.subtlePlayerRollActive)
			{
				float num10 = this.GetRollAngle180Wrapping();
				if (num10 > 90f)
				{
					num10 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num10));
				}
				else if (num10 < -90f)
				{
					num10 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num10));
				}
				Vector3 normalized3 = new Vector3(this.currentVelocity.x, 0f, this.currentVelocity.z).normalized;
				Vector3 vector13 = new Vector3(average.x, 0f, average.z);
				float num11 = Vector3.Dot(vector13 - Vector3.Dot(vector13, normalized3) * normalized3, Vector3.Cross(normalized3, Vector3.up));
				this.turnAccelerationSmoothed = Mathf.Lerp(num11, this.turnAccelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
				float num12 = 0f;
				if (num11 * num10 > 0f)
				{
					num12 = Mathf.InverseLerp(this.subtlePlayerRollAccelMinMax.x, this.subtlePlayerRollAccelMinMax.y, Mathf.Abs(this.turnAccelerationSmoothed));
				}
				float num13 = num10 * this.subtlePlayerRollFactor * Mathf.Min(num9, num12);
				this.subtlePlayerRoll = Mathf.Lerp(num13, this.subtlePlayerRoll, Mathf.Exp(-this.subtlePlayerRollRateExp * dt));
				quaternion3 = Quaternion.AngleAxis(this.subtlePlayerRoll, base.transform.forward);
			}
			Quaternion quaternion4 = Quaternion.identity;
			if (this.subtlePlayerPitchActive)
			{
				float num14 = this.pitch * this.subtlePlayerPitchFactor * Mathf.Min(num9, 1f);
				this.subtlePlayerPitch = Mathf.Lerp(num14, this.subtlePlayerPitch, Mathf.Exp(-this.subtlePlayerPitchRateExp * dt));
				quaternion4 = Quaternion.AngleAxis(this.subtlePlayerPitch, -base.transform.right);
			}
			GTPlayer.Instance.PlayerRotationOverride = quaternion4 * quaternion3;
		}
		this.UpdateGliderPosition();
		if (this.syncedState.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = (this.syncedState.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		this.syncedState.tagged = this.infectedState;
		this.syncedState.materialIndex = (byte)this.ridersMaterialOverideIndex;
		if (this.cachedRig != null)
		{
			this.syncedState.position = this.cachedRig.transform.InverseTransformPoint(base.transform.position);
			this.syncedState.rotation = Quaternion.Inverse(this.cachedRig.transform.rotation) * base.transform.rotation;
		}
		else
		{
			Debug.LogError("Glider failed to get a reference to the local player's VRRig while the player was flying", this);
		}
		this.audioLevel = num2 * num;
		if (this.OutOfBounds)
		{
			this.Respawn();
		}
		if (this.leftHold.active && EquipmentInteractor.instance.leftHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.leftHand);
		}
		if (this.rightHold.active && EquipmentInteractor.instance.rightHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.rightHand);
		}
	}

	// Token: 0x060049A6 RID: 18854 RVA: 0x00183898 File Offset: 0x00181A98
	private void AuthorityUpdateUnheld(float dt)
	{
		this.syncedState.position = base.transform.position;
		this.syncedState.rotation = base.transform.rotation;
		if (this.gliderState != GliderHoldable.GliderState.LocallyDropped)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.tagged = false;
			this.leafMesh.material = this.baseLeafMaterial;
		}
		if (this.audioLevel * this.audioVolumeMultiplier > 0.001f)
		{
			this.audioLevel = Mathf.Lerp(0f, this.audioLevel, Mathf.Exp(-2f * dt));
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.audioVolumeMultiplier);
		}
		if (this.OutOfBounds || (this.lastHeldTime > 0f && this.lastHeldTime < Time.time - this.maxDroppedTimeToRespawn))
		{
			this.Respawn();
		}
	}

	// Token: 0x060049A7 RID: 18855 RVA: 0x001839CC File Offset: 0x00181BCC
	private void RemoteSyncUpdate(float dt)
	{
		this.rb.isKinematic = true;
		int num = this.syncedState.riderId;
		bool flag = this.riderId != num;
		if (flag)
		{
			this.riderId = num;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		if (this.riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.cachedRig = null;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.audioLevel = 0;
		}
		if (this.syncedState.riderId == -1)
		{
			base.transform.position = Vector3.Lerp(this.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.rotation = Quaternion.Slerp(this.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
		}
		else if (this.cachedRig != null)
		{
			this.positionLocalToVRRig = Vector3.Lerp(this.syncedState.position, this.positionLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			this.rotationLocalToVRRig = Quaternion.Slerp(this.syncedState.rotation, this.rotationLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.position = this.cachedRig.transform.TransformPoint(this.positionLocalToVRRig);
			base.transform.rotation = this.cachedRig.transform.rotation * this.rotationLocalToVRRig;
		}
		bool flag2 = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			flag2 = this.syncedState.tagged;
		}
		bool flag3 = flag2 != this.infectedState;
		this.infectedState = flag2;
		if (flag3 || flag)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		float num2 = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
		if (this.audioLevel != num2)
		{
			this.audioLevel = num2;
			if (this.syncedState.riderId != -1 && this.syncedState.tagged)
			{
				this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				return;
			}
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
	}

	// Token: 0x060049A8 RID: 18856 RVA: 0x00183CD0 File Offset: 0x00181ED0
	private VRRig getNewHolderRig(int riderId)
	{
		if (riderId >= 0)
		{
			NetPlayer netPlayer;
			if (riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				netPlayer = NetworkSystem.Instance.LocalPlayer;
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(riderId);
			}
			RigContainer rigContainer;
			if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				return rigContainer.Rig;
			}
		}
		return null;
	}

	// Token: 0x060049A9 RID: 18857 RVA: 0x00183D28 File Offset: 0x00181F28
	private Vector3 ClosestPointInHandle(Vector3 startingPoint, InteractionPoint interactionPoint)
	{
		CapsuleCollider component = interactionPoint.GetComponent<CapsuleCollider>();
		Vector3 vector = startingPoint;
		if (component != null)
		{
			Vector3 vector2 = (component.direction == 0) ? Vector3.right : ((component.direction == 1) ? Vector3.up : Vector3.forward);
			Vector3 vector3 = component.transform.rotation * vector2;
			Vector3 vector4 = component.transform.position + component.transform.rotation * component.center;
			float num = Mathf.Clamp(Vector3.Dot(vector - vector4, vector3), -component.height * 0.5f, component.height * 0.5f);
			vector = vector4 + vector3 * num;
		}
		return vector;
	}

	// Token: 0x060049AA RID: 18858 RVA: 0x00183DE8 File Offset: 0x00181FE8
	private void UpdateGliderPosition()
	{
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			Vector3 vector2 = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
			base.transform.position = (vector + vector2) * 0.5f;
			return;
		}
		if (this.leftHold.active)
		{
			base.transform.position = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			return;
		}
		if (this.rightHold.active)
		{
			base.transform.position = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
		}
	}

	// Token: 0x060049AB RID: 18859 RVA: 0x00183F40 File Offset: 0x00182140
	private Vector3 GetHandsVector(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 headPos, bool flipBasedOnFacingDir)
	{
		Vector3 vector = rightHandPos - leftHandPos;
		Vector3 vector2 = (rightHandPos + leftHandPos) * 0.5f - headPos;
		Vector3 normalized = Vector3.Cross(Vector3.up, vector2).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(vector, normalized) < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	// Token: 0x060049AC RID: 18860 RVA: 0x00183F9C File Offset: 0x0018219C
	private void GetHandsOrientationVectors(Vector3 leftHandPos, Vector3 rightHandPos, Transform head, bool flipBasedOnFacingDir, out Vector3 handsVector, out Vector3 handsUpVector)
	{
		handsVector = rightHandPos - leftHandPos;
		float magnitude = handsVector.magnitude;
		handsVector /= Mathf.Max(magnitude, 0.001f);
		Vector3 position = head.position;
		float num = 1f;
		Vector3 vector = (Vector3.Dot(head.right, handsVector) < 0f) ? handsVector : (-handsVector);
		Vector3 normalized = Vector3.ProjectOnPlane(-head.forward, vector).normalized;
		Vector3 vector2 = normalized * num + position;
		Vector3 vector3 = (leftHandPos + rightHandPos) * 0.5f;
		Vector3 vector4 = Vector3.ProjectOnPlane(vector3 - head.position, Vector3.up);
		float magnitude2 = vector4.magnitude;
		vector4 /= Mathf.Max(magnitude2, 0.001f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(-base.transform.forward, Vector3.up).normalized;
		Vector3 vector5 = -vector4 * num + position;
		float num2 = Vector3.Dot(normalized2, -vector4);
		float num3 = Vector3.Dot(normalized2, normalized);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			num2 = Mathf.Abs(num2);
			num3 = Mathf.Abs(num3);
		}
		num2 = Mathf.Max(num2, 0f);
		num3 = Mathf.Max(num3, 0f);
		Vector3 vector6 = (vector5 * num2 + vector2 * num3) / Mathf.Max(num2 + num3, 0.001f);
		Vector3 vector7 = vector3 - vector6;
		Vector3 normalized3 = Vector3.Cross(Vector3.up, vector7).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(handsVector, normalized3) < 0f)
		{
			handsVector = -handsVector;
		}
		handsUpVector = Vector3.Cross(Vector3.ProjectOnPlane(vector7, Vector3.up), handsVector).normalized;
	}

	// Token: 0x060049AD RID: 18861 RVA: 0x001841C7 File Offset: 0x001823C7
	private Material GetMaterialFromIndex(byte materialIndex)
	{
		if (materialIndex < 1 || (int)materialIndex > this.cosmeticMaterialOverrides.Length)
		{
			return this.baseLeafMaterial;
		}
		return this.cosmeticMaterialOverrides[(int)(materialIndex - 1)].material;
	}

	// Token: 0x060049AE RID: 18862 RVA: 0x001841F4 File Offset: 0x001823F4
	private float GetRollAngle180Wrapping()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float angle = Vector3.SignedAngle(Vector3.Cross(Vector3.up, normalized).normalized, base.transform.right, base.transform.forward);
		return this.NormalizeAngle180(angle);
	}

	// Token: 0x060049AF RID: 18863 RVA: 0x00184255 File Offset: 0x00182455
	private float SignedAngleInPlane(Vector3 from, Vector3 to, Vector3 normal)
	{
		from = Vector3.ProjectOnPlane(from, normal);
		to = Vector3.ProjectOnPlane(to, normal);
		return Vector3.SignedAngle(from, to, normal);
	}

	// Token: 0x060049B0 RID: 18864 RVA: 0x00184271 File Offset: 0x00182471
	private float NormalizeAngle180(float angle)
	{
		angle = (angle + 180f) % 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle - 180f;
	}

	// Token: 0x060049B1 RID: 18865 RVA: 0x0018429C File Offset: 0x0018249C
	private void UpdateAudioSource(AudioSource source, float level)
	{
		source.volume = level;
		if (!source.isPlaying && level > 0.01f)
		{
			source.GTPlay();
			return;
		}
		if (source.isPlaying && level < 0.01f && this.syncedState.riderId == -1)
		{
			source.GTStop();
		}
	}

	// Token: 0x060049B2 RID: 18866 RVA: 0x001842EB File Offset: 0x001824EB
	private Material GetInfectedMaterial()
	{
		if (GorillaGameManager.instance is GorillaFreezeTagManager)
		{
			return this.frozenLeafMaterial;
		}
		return this.infectedLeafMaterial;
	}

	// Token: 0x060049B3 RID: 18867 RVA: 0x00184308 File Offset: 0x00182508
	public void OnTriggerStay(Collider other)
	{
		GliderWindVolume component = other.GetComponent<GliderWindVolume>();
		if (component == null)
		{
			return;
		}
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (Time.frameCount == this.windVolumeForceAppliedFrame)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			Vector3 accelFromVelocity = component.GetAccelFromVelocity(GTPlayer.Instance.RigidbodyVelocity);
			GTPlayer.Instance.AddForce(accelFromVelocity, 5);
			this.windVolumeForceAppliedFrame = Time.frameCount;
			return;
		}
		Vector3 accelFromVelocity2 = component.GetAccelFromVelocity(this.rb.linearVelocity);
		Vector3 vector = this.WindResistanceForceOffset(base.transform.up, component.WindDirection);
		Vector3 vector2 = base.transform.position + vector * this.windUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(accelFromVelocity2 * this.rb.mass, vector2, 0);
		this.windVolumeForceAppliedFrame = Time.frameCount;
	}

	// Token: 0x060049B4 RID: 18868 RVA: 0x00184406 File Offset: 0x00182606
	private Vector3 WindResistanceForceOffset(Vector3 upDir, Vector3 windDir)
	{
		if (Vector3.Dot(upDir, windDir) < 0f)
		{
			upDir *= -1f;
		}
		return Vector3.ProjectOnPlane(upDir - windDir, upDir);
	}

	// Token: 0x170006DC RID: 1756
	// (get) Token: 0x060049B5 RID: 18869 RVA: 0x00184430 File Offset: 0x00182630
	// (set) Token: 0x060049B6 RID: 18870 RVA: 0x0018445A File Offset: 0x0018265A
	[Networked]
	[NetworkedWeaved(0, 11)]
	internal unsafe GliderHoldable.SyncedState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GliderHoldable.SyncedState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GliderHoldable.SyncedState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060049B7 RID: 18871 RVA: 0x00184488 File Offset: 0x00182688
	public override void ReadDataFusion()
	{
		int num = this.syncedState.riderId;
		this.syncedState = this.Data;
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x060049B8 RID: 18872 RVA: 0x001844DB File Offset: 0x001826DB
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x060049B9 RID: 18873 RVA: 0x001844EC File Offset: 0x001826EC
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		PunNetPlayer punNetPlayer = (PunNetPlayer)this.ownershipGuard.actualOwner;
		if (sender != ((punNetPlayer != null) ? punNetPlayer.PlayerRef : null))
		{
			return;
		}
		int num = this.syncedState.riderId;
		this.syncedState.riderId = (int)stream.ReceiveNext();
		this.syncedState.tagged = (bool)stream.ReceiveNext();
		this.syncedState.materialIndex = (byte)stream.ReceiveNext();
		this.syncedState.audioLevel = (byte)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.syncedState.position.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.syncedState.rotation.SetValueSafe(quaternion);
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x060049BA RID: 18874 RVA: 0x001845F4 File Offset: 0x001827F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		object sender = info.Sender;
		NetPlayer actualOwner = this.ownershipGuard.actualOwner;
		if (!sender.Equals((actualOwner != null) ? actualOwner.GetPlayerRef() : null))
		{
			return;
		}
		stream.SendNext(this.syncedState.riderId);
		stream.SendNext(this.syncedState.tagged);
		stream.SendNext(this.syncedState.materialIndex);
		stream.SendNext(this.syncedState.audioLevel);
		stream.SendNext(this.syncedState.position);
		stream.SendNext(this.syncedState.rotation);
	}

	// Token: 0x060049BB RID: 18875 RVA: 0x001846AF File Offset: 0x001828AF
	private IEnumerator ReenableOwnershipRequest()
	{
		yield return new WaitForSeconds(3f);
		this.pendingOwnershipRequest = false;
		yield break;
	}

	// Token: 0x060049BC RID: 18876 RVA: 0x001846C0 File Offset: 0x001828C0
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.pendingOwnershipRequest = false;
			if (!this.leftHold.active && !this.rightHold.active && (this.spawnPosition - base.transform.position).sqrMagnitude > 1f)
			{
				this.rb.isKinematic = false;
				this.rb.WakeUp();
				this.lastHeldTime = Time.time;
			}
		}
	}

	// Token: 0x060049BD RID: 18877 RVA: 0x00184742 File Offset: 0x00182942
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !base.IsMine || !NetworkSystem.Instance.InRoom || (!this.leftHold.active && !this.rightHold.active);
	}

	// Token: 0x060049BE RID: 18878 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060049BF RID: 18879 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x060049C0 RID: 18880 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060049C4 RID: 18884 RVA: 0x00184C31 File Offset: 0x00182E31
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060049C5 RID: 18885 RVA: 0x00184C49 File Offset: 0x00182E49
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04005A13 RID: 23059
	[Header("Flight Settings")]
	[SerializeField]
	private Vector2 pitchMinMax = new Vector2(-80f, 80f);

	// Token: 0x04005A14 RID: 23060
	[SerializeField]
	private Vector2 rollMinMax = new Vector2(-70f, 70f);

	// Token: 0x04005A15 RID: 23061
	[SerializeField]
	private float pitchHalfLife = 0.2f;

	// Token: 0x04005A16 RID: 23062
	public Vector2 pitchVelocityTargetMinMax = new Vector2(-60f, 60f);

	// Token: 0x04005A17 RID: 23063
	public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-1f, 1f);

	// Token: 0x04005A18 RID: 23064
	[SerializeField]
	private float pitchVelocityFollowRateAngle = 60f;

	// Token: 0x04005A19 RID: 23065
	[SerializeField]
	private float pitchVelocityFollowRateMagnitude = 5f;

	// Token: 0x04005A1A RID: 23066
	[SerializeField]
	private AnimationCurve liftVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005A1B RID: 23067
	[SerializeField]
	private AnimationCurve dragVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005A1C RID: 23068
	[SerializeField]
	[Range(0f, 1f)]
	public float attackDragFactor = 0.1f;

	// Token: 0x04005A1D RID: 23069
	[SerializeField]
	private AnimationCurve dragVsSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005A1E RID: 23070
	[SerializeField]
	public float dragVsSpeedMaxSpeed = 30f;

	// Token: 0x04005A1F RID: 23071
	[SerializeField]
	[Range(0f, 1f)]
	public float dragVsSpeedDragFactor = 0.2f;

	// Token: 0x04005A20 RID: 23072
	[SerializeField]
	private AnimationCurve liftIncreaseVsRoll = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005A21 RID: 23073
	[SerializeField]
	private float liftIncreaseVsRollMaxAngle = 20f;

	// Token: 0x04005A22 RID: 23074
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityCompensation = 0.8f;

	// Token: 0x04005A23 RID: 23075
	[Range(0f, 1f)]
	public float pullUpLiftBonus = 0.1f;

	// Token: 0x04005A24 RID: 23076
	public float pullUpLiftActivationVelocity = 1f;

	// Token: 0x04005A25 RID: 23077
	public float pullUpLiftActivationAcceleration = 3f;

	// Token: 0x04005A26 RID: 23078
	[Header("Body Positioning Control")]
	[SerializeField]
	private float riderPosDirectPitchMax = 70f;

	// Token: 0x04005A27 RID: 23079
	[SerializeField]
	private Vector2 riderPosRange = new Vector2(2.2f, 0.75f);

	// Token: 0x04005A28 RID: 23080
	[SerializeField]
	private float riderPosRangeOffset = 0.15f;

	// Token: 0x04005A29 RID: 23081
	[SerializeField]
	private Vector2 riderPosRangeNormalizedDeadzone = new Vector2(0.15f, 0.05f);

	// Token: 0x04005A2A RID: 23082
	[Header("Direct Handle Control")]
	[SerializeField]
	private float oneHandHoldRotationRate = 2f;

	// Token: 0x04005A2B RID: 23083
	private Vector3 oneHandSimulatedHoldOffset = new Vector3(0.5f, -0.35f, 0.25f);

	// Token: 0x04005A2C RID: 23084
	private float oneHandPitchMultiplier = 0.8f;

	// Token: 0x04005A2D RID: 23085
	[SerializeField]
	private float twoHandHoldRotationRate = 4f;

	// Token: 0x04005A2E RID: 23086
	[SerializeField]
	private bool twoHandGliderInversionOnYawInsteadOfRoll;

	// Token: 0x04005A2F RID: 23087
	[Header("Player Settings")]
	[SerializeField]
	private bool setMaxHandSlipDuringFlight = true;

	// Token: 0x04005A30 RID: 23088
	[SerializeField]
	private float maxSlipOverrideSpeedThreshold = 5f;

	// Token: 0x04005A31 RID: 23089
	[Header("Player Camera Rotation")]
	[SerializeField]
	private float subtlePlayerPitchFactor = 0.2f;

	// Token: 0x04005A32 RID: 23090
	[SerializeField]
	private float subtlePlayerPitchRate = 2f;

	// Token: 0x04005A33 RID: 23091
	[SerializeField]
	private float subtlePlayerRollFactor = 0.2f;

	// Token: 0x04005A34 RID: 23092
	[SerializeField]
	private float subtlePlayerRollRate = 2f;

	// Token: 0x04005A35 RID: 23093
	[SerializeField]
	private Vector2 subtlePlayerRotationSpeedRampMinMax = new Vector2(2f, 8f);

	// Token: 0x04005A36 RID: 23094
	[SerializeField]
	private Vector2 subtlePlayerRollAccelMinMax = new Vector2(0f, 30f);

	// Token: 0x04005A37 RID: 23095
	[SerializeField]
	private Vector2 subtlePlayerPitchAccelMinMax = new Vector2(0f, 10f);

	// Token: 0x04005A38 RID: 23096
	[SerializeField]
	private float accelSmoothingFollowRate = 2f;

	// Token: 0x04005A39 RID: 23097
	[Header("Haptics")]
	[SerializeField]
	private Vector2 hapticAccelInputRange = new Vector2(5f, 20f);

	// Token: 0x04005A3A RID: 23098
	[SerializeField]
	private float hapticAccelOutputMax = 0.35f;

	// Token: 0x04005A3B RID: 23099
	[SerializeField]
	private Vector2 hapticMaxSpeedInputRange = new Vector2(5f, 10f);

	// Token: 0x04005A3C RID: 23100
	[SerializeField]
	private Vector2 hapticSpeedInputRange = new Vector2(3f, 30f);

	// Token: 0x04005A3D RID: 23101
	[SerializeField]
	private float hapticSpeedOutputMax = 0.15f;

	// Token: 0x04005A3E RID: 23102
	[SerializeField]
	private Vector2 whistlingAudioSpeedInputRange = new Vector2(15f, 30f);

	// Token: 0x04005A3F RID: 23103
	[Header("Audio")]
	[SerializeField]
	private float audioVolumeMultiplier = 0.25f;

	// Token: 0x04005A40 RID: 23104
	[SerializeField]
	private float infectedAudioVolumeMultiplier = 0.5f;

	// Token: 0x04005A41 RID: 23105
	[SerializeField]
	private Vector2 whooshSpeedThresholdInput = new Vector2(10f, 25f);

	// Token: 0x04005A42 RID: 23106
	[SerializeField]
	private Vector2 whooshVolumeOutput = new Vector2(0.2f, 0.75f);

	// Token: 0x04005A43 RID: 23107
	[SerializeField]
	private float whooshCheckDistance = 2f;

	// Token: 0x04005A44 RID: 23108
	[Header("Tag Adjustment")]
	[SerializeField]
	private bool extendTagRangeInFlight = true;

	// Token: 0x04005A45 RID: 23109
	[SerializeField]
	private Vector2 tagRangeSpeedInput = new Vector2(5f, 20f);

	// Token: 0x04005A46 RID: 23110
	[SerializeField]
	private Vector2 tagRangeOutput = new Vector2(0.03f, 3f);

	// Token: 0x04005A47 RID: 23111
	[SerializeField]
	private bool debugDrawTagRange = true;

	// Token: 0x04005A48 RID: 23112
	[Header("Infected State")]
	[SerializeField]
	private float infectedSpeedIncrease = 5f;

	// Token: 0x04005A49 RID: 23113
	[Header("Glider Materials")]
	[SerializeField]
	private MeshRenderer leafMesh;

	// Token: 0x04005A4A RID: 23114
	[SerializeField]
	private Material baseLeafMaterial;

	// Token: 0x04005A4B RID: 23115
	[SerializeField]
	private Material infectedLeafMaterial;

	// Token: 0x04005A4C RID: 23116
	[SerializeField]
	private Material frozenLeafMaterial;

	// Token: 0x04005A4D RID: 23117
	[SerializeField]
	private GliderHoldable.CosmeticMaterialOverride[] cosmeticMaterialOverrides;

	// Token: 0x04005A4E RID: 23118
	[Header("Network Syncing")]
	[SerializeField]
	private float networkSyncFollowRate = 2f;

	// Token: 0x04005A4F RID: 23119
	[Header("Life Cycle")]
	[SerializeField]
	private Transform maxDistanceRespawnOrigin;

	// Token: 0x04005A50 RID: 23120
	[SerializeField]
	private float maxDistanceBeforeRespawn = 180f;

	// Token: 0x04005A51 RID: 23121
	[SerializeField]
	private float maxDroppedTimeToRespawn = 120f;

	// Token: 0x04005A52 RID: 23122
	[Header("Rigidbody")]
	[SerializeField]
	private float windUprightTorqueMultiplier = 1f;

	// Token: 0x04005A53 RID: 23123
	[SerializeField]
	private float gravityUprightTorqueMultiplier = 0.5f;

	// Token: 0x04005A54 RID: 23124
	[SerializeField]
	private float fallingGravityReduction = 0.1f;

	// Token: 0x04005A55 RID: 23125
	[Header("References")]
	[SerializeField]
	private AudioSource calmAudio;

	// Token: 0x04005A56 RID: 23126
	[SerializeField]
	private AudioSource activeAudio;

	// Token: 0x04005A57 RID: 23127
	[SerializeField]
	private AudioSource whistlingAudio;

	// Token: 0x04005A58 RID: 23128
	[SerializeField]
	private AudioSource leftWhooshAudio;

	// Token: 0x04005A59 RID: 23129
	[SerializeField]
	private AudioSource rightWhooshAudio;

	// Token: 0x04005A5A RID: 23130
	[SerializeField]
	private InteractionPoint handle;

	// Token: 0x04005A5B RID: 23131
	[SerializeField]
	private RequestableOwnershipGuard ownershipGuard;

	// Token: 0x04005A5C RID: 23132
	private bool subtlePlayerPitchActive = true;

	// Token: 0x04005A5D RID: 23133
	private bool subtlePlayerRollActive = true;

	// Token: 0x04005A5E RID: 23134
	private float subtlePlayerPitch;

	// Token: 0x04005A5F RID: 23135
	private float subtlePlayerRoll;

	// Token: 0x04005A60 RID: 23136
	private float subtlePlayerPitchRateExp = 0.75f;

	// Token: 0x04005A61 RID: 23137
	private float subtlePlayerRollRateExp = 0.025f;

	// Token: 0x04005A62 RID: 23138
	private float defaultMaxDistanceBeforeRespawn = 180f;

	// Token: 0x04005A63 RID: 23139
	private GliderHoldable.HoldingHand leftHold = new GliderHoldable.HoldingHand();

	// Token: 0x04005A64 RID: 23140
	private GliderHoldable.HoldingHand rightHold = new GliderHoldable.HoldingHand();

	// Token: 0x04005A65 RID: 23141
	private GliderHoldable.SyncedState syncedState;

	// Token: 0x04005A66 RID: 23142
	private Vector3 twoHandRotationOffsetAxis = Vector3.forward;

	// Token: 0x04005A67 RID: 23143
	private float twoHandRotationOffsetAngle;

	// Token: 0x04005A68 RID: 23144
	private Rigidbody rb;

	// Token: 0x04005A69 RID: 23145
	private Vector2 riderPosition = Vector2.zero;

	// Token: 0x04005A6A RID: 23146
	private Vector3 previousVelocity;

	// Token: 0x04005A6B RID: 23147
	private Vector3 currentVelocity;

	// Token: 0x04005A6C RID: 23148
	private float pitch;

	// Token: 0x04005A6D RID: 23149
	private float yaw;

	// Token: 0x04005A6E RID: 23150
	private float roll;

	// Token: 0x04005A6F RID: 23151
	private float pitchVel;

	// Token: 0x04005A70 RID: 23152
	private float yawVel;

	// Token: 0x04005A71 RID: 23153
	private float rollVel;

	// Token: 0x04005A72 RID: 23154
	private float oneHandRotationRateExp;

	// Token: 0x04005A73 RID: 23155
	private float twoHandRotationRateExp;

	// Token: 0x04005A74 RID: 23156
	private Quaternion playerFacingRotationOffset = Quaternion.identity;

	// Token: 0x04005A75 RID: 23157
	private const float accelAveragingWindow = 0.1f;

	// Token: 0x04005A76 RID: 23158
	private AverageVector3 accelerationAverage = new AverageVector3(0.1f);

	// Token: 0x04005A77 RID: 23159
	private float accelerationSmoothed;

	// Token: 0x04005A78 RID: 23160
	private float turnAccelerationSmoothed;

	// Token: 0x04005A79 RID: 23161
	private float accelSmoothingFollowRateExp = 1f;

	// Token: 0x04005A7A RID: 23162
	private float networkSyncFollowRateExp = 2f;

	// Token: 0x04005A7B RID: 23163
	private bool pendingOwnershipRequest;

	// Token: 0x04005A7C RID: 23164
	private Vector3 positionLocalToVRRig = Vector3.zero;

	// Token: 0x04005A7D RID: 23165
	private Quaternion rotationLocalToVRRig = Quaternion.identity;

	// Token: 0x04005A7E RID: 23166
	private Coroutine reenableOwnershipRequestCoroutine;

	// Token: 0x04005A7F RID: 23167
	private Vector3 spawnPosition;

	// Token: 0x04005A80 RID: 23168
	private Quaternion spawnRotation;

	// Token: 0x04005A81 RID: 23169
	private Vector3 skyJungleSpawnPostion;

	// Token: 0x04005A82 RID: 23170
	private Quaternion skyJungleSpawnRotation;

	// Token: 0x04005A83 RID: 23171
	private Transform skyJungleRespawnOrigin;

	// Token: 0x04005A84 RID: 23172
	private float lastHeldTime = -1f;

	// Token: 0x04005A85 RID: 23173
	private Vector3? leftHoldPositionLocal;

	// Token: 0x04005A86 RID: 23174
	private Vector3? rightHoldPositionLocal;

	// Token: 0x04005A87 RID: 23175
	private float whooshSoundDuration = 1f;

	// Token: 0x04005A88 RID: 23176
	private float whooshSoundRetriggerThreshold = 0.5f;

	// Token: 0x04005A89 RID: 23177
	private float leftWhooshStartTime = -1f;

	// Token: 0x04005A8A RID: 23178
	private Vector3 leftWhooshHitPoint = Vector3.zero;

	// Token: 0x04005A8B RID: 23179
	private Vector3 whooshAudioPositionOffset = new Vector3(0.5f, -0.25f, 0.5f);

	// Token: 0x04005A8C RID: 23180
	private float rightWhooshStartTime = -1f;

	// Token: 0x04005A8D RID: 23181
	private Vector3 rightWhooshHitPoint = Vector3.zero;

	// Token: 0x04005A8E RID: 23182
	private int ridersMaterialOverideIndex;

	// Token: 0x04005A8F RID: 23183
	private int windVolumeForceAppliedFrame = -1;

	// Token: 0x04005A90 RID: 23184
	private bool holdingTwoGliders;

	// Token: 0x04005A91 RID: 23185
	private GliderHoldable.GliderState gliderState;

	// Token: 0x04005A92 RID: 23186
	private float audioLevel;

	// Token: 0x04005A93 RID: 23187
	private int riderId = -1;

	// Token: 0x04005A94 RID: 23188
	[SerializeField]
	private VRRig cachedRig;

	// Token: 0x04005A95 RID: 23189
	private bool infectedState;

	// Token: 0x04005A96 RID: 23190
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 11)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private GliderHoldable.SyncedState _Data;

	// Token: 0x02000BA7 RID: 2983
	private enum GliderState
	{
		// Token: 0x04005A98 RID: 23192
		LocallyHeld,
		// Token: 0x04005A99 RID: 23193
		LocallyDropped,
		// Token: 0x04005A9A RID: 23194
		RemoteSyncing
	}

	// Token: 0x02000BA8 RID: 2984
	private class HoldingHand
	{
		// Token: 0x060049C6 RID: 18886 RVA: 0x00184C60 File Offset: 0x00182E60
		public void Activate(Transform handTransform, Transform gliderTransform, Vector3 worldGrabPoint)
		{
			this.active = true;
			this.transform = handTransform.transform;
			this.holdLocalPos = handTransform.InverseTransformPoint(worldGrabPoint);
			this.handleLocalPos = gliderTransform.InverseTransformVector(gliderTransform.position - worldGrabPoint);
			this.localHoldRotation = Quaternion.Inverse(handTransform.rotation) * gliderTransform.rotation;
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x00184CC1 File Offset: 0x00182EC1
		public void Deactivate()
		{
			this.active = false;
			this.transform = null;
			this.holdLocalPos = Vector3.zero;
			this.handleLocalPos = Vector3.zero;
			this.localHoldRotation = Quaternion.identity;
		}

		// Token: 0x04005A9B RID: 23195
		public bool active;

		// Token: 0x04005A9C RID: 23196
		public Transform transform;

		// Token: 0x04005A9D RID: 23197
		public Vector3 holdLocalPos;

		// Token: 0x04005A9E RID: 23198
		public Vector3 handleLocalPos;

		// Token: 0x04005A9F RID: 23199
		public Quaternion localHoldRotation;
	}

	// Token: 0x02000BA9 RID: 2985
	[NetworkStructWeaved(11)]
	[StructLayout(2, Size = 44)]
	internal struct SyncedState : INetworkStruct
	{
		// Token: 0x060049C9 RID: 18889 RVA: 0x00184CF2 File Offset: 0x00182EF2
		public void Init(Vector3 defaultPosition, Quaternion defaultRotation)
		{
			this.riderId = -1;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.position = defaultPosition;
			this.rotation = defaultRotation;
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x00184D17 File Offset: 0x00182F17
		public SyncedState(int id = -1)
		{
			this.riderId = id;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.tagged = default(NetworkBool);
			this.position = default(Vector3);
			this.rotation = default(Quaternion);
		}

		// Token: 0x04005AA0 RID: 23200
		[FieldOffset(0)]
		public int riderId;

		// Token: 0x04005AA1 RID: 23201
		[FieldOffset(4)]
		public byte materialIndex;

		// Token: 0x04005AA2 RID: 23202
		[FieldOffset(8)]
		public byte audioLevel;

		// Token: 0x04005AA3 RID: 23203
		[FieldOffset(12)]
		public NetworkBool tagged;

		// Token: 0x04005AA4 RID: 23204
		[FieldOffset(16)]
		public Vector3 position;

		// Token: 0x04005AA5 RID: 23205
		[FieldOffset(28)]
		public Quaternion rotation;
	}

	// Token: 0x02000BAA RID: 2986
	[Serializable]
	private struct CosmeticMaterialOverride
	{
		// Token: 0x04005AA6 RID: 23206
		public string cosmeticName;

		// Token: 0x04005AA7 RID: 23207
		public Material material;
	}
}
