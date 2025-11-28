using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class OneStringGuitar : TransferrableObject
{
	// Token: 0x06001DD9 RID: 7641 RVA: 0x0009CBF8 File Offset: 0x0009ADF8
	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x0009CC00 File Offset: 0x0009AE00
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.chestColliderLeft = this._GetChestColliderByPath(rig, "GorillaPlayerNetworkedRigAnchor/rig/body/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformLeft");
		this.chestColliderRight = this._GetChestColliderByPath(rig, "GorillaPlayerNetworkedRigAnchor/rig/body/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformRight");
		this.currentChestCollider = this.chestColliderLeft;
		Transform[] array;
		string text;
		if (!GTHardCodedBones.TryGetBoneXforms(rig, out array, out text))
		{
			Debug.LogError("OneStringGuitar: Error getting bone Transforms: " + text, this);
			return;
		}
		this.parentHandLeft = array[9];
		this.parentHandRight = array[27];
		this.parentHand = this.parentHandRight;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.nullHit = default(RaycastHit);
		this.strumList.Add(this.strumCollider);
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.startingLeftChestOffset = this.chestOffsetLeft;
		this.startingRightChestOffset = this.chestOffsetRight;
		this.startingUnsnapDistance = this.unsnapDistance;
		this.selfInstrumentIndex = rig.AssignInstrumentToInstrumentSelfOnly(this);
		for (int i = 0; i < this.frets.Length; i++)
		{
			this.fretsList.Add(this.frets[i]);
		}
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x0009CD48 File Offset: 0x0009AF48
	private Collider _GetChestColliderByPath(VRRig vrRig, string chestColliderLeftPath)
	{
		Transform transform;
		if (!vrRig.transform.TryFindByExactPath(chestColliderLeftPath, out transform))
		{
			Debug.LogError("DEACTIVATING! do you move this without updating the script? could not find this transform: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		Collider component = transform.GetComponent<Collider>();
		if (!component)
		{
			Debug.LogError("DEACTIVATING! found transform but couldn't find collider at path: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		return component;
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x0009CDB8 File Offset: 0x0009AFB8
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this.currentState == TransferrableObject.PositionState.InLeftHand)
		{
			this.fretHandIndicator = this.leftHandIndicator;
			this.strumHandIndicator = this.rightHandIndicator;
		}
		else
		{
			this.fretHandIndicator = this.rightHandIndicator;
			this.strumHandIndicator = this.leftHandIndicator;
		}
		if (base.IsLocalObject())
		{
			this.parentHand = GTPlayer.Instance.GetHandFollower(this.currentState == TransferrableObject.PositionState.InLeftHand);
		}
		this.initOffset = Vector3.zero;
		this.initRotation = Quaternion.identity;
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x0009CE3D File Offset: 0x0009B03D
	internal override void OnDisable()
	{
		base.OnDisable();
		this.angleSnapped = false;
		this.positionSnapped = false;
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x0009CE61 File Offset: 0x0009B061
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
		return true;
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x0009CE8C File Offset: 0x0009B08C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.lastState != (OneStringGuitar.GuitarStates)this.itemState)
		{
			this.angleSnapped = false;
			this.positionSnapped = false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			Vector3 positionTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startPositionLeft : this.startPositionRight;
			Quaternion rotationTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startQuatLeft : this.startQuatRight;
			this.UpdateNonPlayingPosition(positionTarget, rotationTarget);
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			Vector3 positionTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripPositionLeft : this.reverseGripPositionRight;
			Quaternion rotationTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripQuatLeft : this.reverseGripQuatRight;
			this.UpdateNonPlayingPosition(positionTarget2, rotationTarget2);
			if (this.IsMyItem() && (this.chestTouch.transform.position - this.currentChestCollider.transform.position).magnitude < this.snapDistance)
			{
				this.itemState = TransferrableObject.ItemStates.State2;
				this.angleSnapped = false;
				this.positionSnapped = false;
				this.currentChestCollider.gameObject.SetActive(true);
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			Quaternion quaternion = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.holdingOffsetRotationLeft : this.holdingOffsetRotationRight;
			Vector3 vector = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.chestOffsetLeft : this.chestOffsetRight;
			Quaternion quaternion2 = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * quaternion;
			if (!this.angleSnapped && Quaternion.Angle(base.transform.rotation, quaternion2) > this.angleLerpSnap)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion2, this.lerpValue);
			}
			else
			{
				this.angleSnapped = true;
				base.transform.rotation = quaternion2;
			}
			Vector3 vector2 = this.currentChestCollider.transform.position + base.transform.rotation * vector;
			if (!this.positionSnapped && (base.transform.position - vector2).magnitude > this.vectorLerpSnap)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.currentChestCollider.transform.position + base.transform.rotation * vector, this.lerpValue);
			}
			else
			{
				this.positionSnapped = true;
				base.transform.position = vector2;
			}
			if (this.currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.parentHand = this.parentHandRight;
			}
			else
			{
				this.parentHand = this.parentHandLeft;
			}
			if (this.IsMyItem())
			{
				this.unsnapDistance = this.startingUnsnapDistance * base.myRig.transform.localScale.x;
				if (this.currentState == TransferrableObject.PositionState.InRightHand)
				{
					this.chestOffsetRight = Vector3.Scale(this.startingRightChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderRight;
					this.fretHandIndicator = this.rightHandIndicator;
					this.strumHandIndicator = this.leftHandIndicator;
				}
				else
				{
					this.chestOffsetLeft = Vector3.Scale(this.startingLeftChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderLeft;
					this.fretHandIndicator = this.leftHandIndicator;
					this.strumHandIndicator = this.rightHandIndicator;
				}
				if (this.Unsnap())
				{
					this.itemState = TransferrableObject.ItemStates.State1;
					this.angleSnapped = false;
					this.positionSnapped = false;
					if (this.currentState == TransferrableObject.PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
					this.currentChestCollider.gameObject.SetActive(false);
				}
				else
				{
					if (!this.handIn)
					{
						this.CheckFretFinger(this.fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref this.collidersHitCount, this.interactableMask, this.sphereRadius, ref this.nullHit, ref this.raycastHits, ref this.raycastHitList, ref this.spherecastSweep, ref this.strumHandIndicator);
						if (this.collidersHitCount > 0)
						{
							int i = 0;
							while (i < this.collidersHitCount)
							{
								if (this.raycastHits[i].collider != null && this.strumCollider == this.raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(this.strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									this.PlayNote(this.currentFretIndex, Mathf.Max(Mathf.Min(1f, this.strumHandIndicator.currentVelocity.magnitude / this.maxVelocity) * this.maxVolume, this.minVolume));
									if (!NetworkSystem.Instance.InRoom || this.selfInstrumentIndex <= -1)
									{
										break;
									}
									NetworkView myVRRig = GorillaTagger.Instance.myVRRig;
									if (myVRRig == null)
									{
										break;
									}
									myVRRig.SendRPC("RPC_PlaySelfOnlyInstrument", 1, new object[]
									{
										this.selfInstrumentIndex,
										this.currentFretIndex,
										this.audioSource.volume
									});
									break;
								}
								else
								{
									i++;
								}
							}
						}
					}
					this.handIn = HitChecker.CheckHandIn(ref this.anyHit, ref this.collidersHit, this.sphereRadius * base.transform.lossyScale.x, this.interactableMask, ref this.strumHandIndicator, ref this.strumList);
				}
			}
		}
		this.lastState = (OneStringGuitar.GuitarStates)this.itemState;
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x0009D448 File Offset: 0x0009B648
	public override void PlayNote(int note, float volume)
	{
		this.audioSource.time = 0.005f;
		this.audioSource.clip = this.audioClips[note];
		this.audioSource.volume = volume;
		this.audioSource.GTPlay();
		base.PlayNote(note, volume);
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x0009D498 File Offset: 0x0009B698
	private bool Unsnap()
	{
		return (this.parentHand.position - this.chestTouch.position).magnitude > this.unsnapDistance;
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x0009D4D0 File Offset: 0x0009B6D0
	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = null;
		}
		this.collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, this.sphereRadius, this.collidersHit, this.interactableMask, 2);
		this.currentFretIndex = 5;
		if (this.collidersHitCount > 0)
		{
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.fretsList.Contains(this.collidersHit[j]))
				{
					this.currentFretIndex = this.fretsList.IndexOf(this.collidersHit[j]);
					if (this.currentFretIndex != this.lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					this.lastFretIndex = this.currentFretIndex;
					return;
				}
			}
			return;
		}
		if (this.lastFretIndex != -1)
		{
			GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.lastFretIndex = -1;
	}

	// Token: 0x06001DE3 RID: 7651 RVA: 0x0009D604 File Offset: 0x0009B804
	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!this.angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, base.transform.localRotation) < this.angleLerpSnap)
			{
				this.angleSnapped = true;
				base.transform.localRotation = rotationTarget;
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, rotationTarget, this.lerpValue);
			}
		}
		if (!this.positionSnapped)
		{
			if ((base.transform.localPosition - positionTarget).magnitude < this.vectorLerpSnap)
			{
				this.positionSnapped = true;
				base.transform.localPosition = positionTarget;
				return;
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, positionTarget, this.lerpValue);
		}
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x0009D6C8 File Offset: 0x0009B8C8
	public override bool CanDeactivate()
	{
		return !base.gameObject.activeSelf || this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x0009D6EB File Offset: 0x0009B8EB
	public override bool CanActivate()
	{
		return this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x0009D701 File Offset: 0x0009B901
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.itemState = TransferrableObject.ItemStates.State1;
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x0009D724 File Offset: 0x0009B924
	public void GenerateVectorOffsetLeft()
	{
		this.chestOffsetLeft = base.transform.position - this.chestColliderLeft.transform.position;
		this.holdingOffsetRotationLeft = Quaternion.LookRotation(base.transform.position - this.chestColliderLeft.transform.position);
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x0009D784 File Offset: 0x0009B984
	public void GenerateVectorOffsetRight()
	{
		this.chestOffsetRight = base.transform.position - this.chestColliderRight.transform.position;
		this.holdingOffsetRotationRight = Quaternion.LookRotation(base.transform.position - this.chestColliderRight.transform.position);
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x0009D7E2 File Offset: 0x0009B9E2
	public void GenerateReverseGripOffsetLeft()
	{
		this.reverseGripPositionLeft = base.transform.localPosition;
		this.reverseGripQuatLeft = base.transform.localRotation;
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x0009D806 File Offset: 0x0009BA06
	public void GenerateClubOffsetLeft()
	{
		this.startPositionLeft = base.transform.localPosition;
		this.startQuatLeft = base.transform.localRotation;
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x0009D82A File Offset: 0x0009BA2A
	public void GenerateReverseGripOffsetRight()
	{
		this.reverseGripPositionRight = base.transform.localPosition;
		this.reverseGripQuatRight = base.transform.localRotation;
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x0009D84E File Offset: 0x0009BA4E
	public void GenerateClubOffsetRight()
	{
		this.startPositionRight = base.transform.localPosition;
		this.startQuatRight = base.transform.localRotation;
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x0009D872 File Offset: 0x0009BA72
	public void TestClubPositionRight()
	{
		base.transform.localPosition = this.startPositionRight;
		base.transform.localRotation = this.startQuatRight;
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x0009D896 File Offset: 0x0009BA96
	public void TestReverseGripPositionRight()
	{
		base.transform.localPosition = this.reverseGripPositionRight;
		base.transform.localRotation = this.reverseGripQuatRight;
	}

	// Token: 0x06001DEF RID: 7663 RVA: 0x0009D8BC File Offset: 0x0009BABC
	public void TestPlayingPositionRight()
	{
		base.transform.rotation = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * this.holdingOffsetRotationRight;
		base.transform.position = this.chestColliderRight.transform.position + base.transform.rotation * this.chestOffsetRight;
	}

	// Token: 0x040027C4 RID: 10180
	public Vector3 chestOffsetLeft;

	// Token: 0x040027C5 RID: 10181
	public Vector3 chestOffsetRight;

	// Token: 0x040027C6 RID: 10182
	public Quaternion holdingOffsetRotationLeft;

	// Token: 0x040027C7 RID: 10183
	public Quaternion holdingOffsetRotationRight;

	// Token: 0x040027C8 RID: 10184
	public Quaternion chestRotationOffset;

	// Token: 0x040027C9 RID: 10185
	[NonSerialized]
	public Collider currentChestCollider;

	// Token: 0x040027CA RID: 10186
	[NonSerialized]
	public Collider chestColliderLeft;

	// Token: 0x040027CB RID: 10187
	[NonSerialized]
	public Collider chestColliderRight;

	// Token: 0x040027CC RID: 10188
	public float lerpValue = 0.25f;

	// Token: 0x040027CD RID: 10189
	public AudioSource audioSource;

	// Token: 0x040027CE RID: 10190
	private Transform parentHand;

	// Token: 0x040027CF RID: 10191
	private Transform parentHandLeft;

	// Token: 0x040027D0 RID: 10192
	private Transform parentHandRight;

	// Token: 0x040027D1 RID: 10193
	public float unsnapDistance;

	// Token: 0x040027D2 RID: 10194
	public float snapDistance;

	// Token: 0x040027D3 RID: 10195
	public Vector3 startPositionLeft;

	// Token: 0x040027D4 RID: 10196
	public Quaternion startQuatLeft;

	// Token: 0x040027D5 RID: 10197
	public Vector3 reverseGripPositionLeft;

	// Token: 0x040027D6 RID: 10198
	public Quaternion reverseGripQuatLeft;

	// Token: 0x040027D7 RID: 10199
	public Vector3 startPositionRight;

	// Token: 0x040027D8 RID: 10200
	public Quaternion startQuatRight;

	// Token: 0x040027D9 RID: 10201
	public Vector3 reverseGripPositionRight;

	// Token: 0x040027DA RID: 10202
	public Quaternion reverseGripQuatRight;

	// Token: 0x040027DB RID: 10203
	public float angleLerpSnap = 1f;

	// Token: 0x040027DC RID: 10204
	public float vectorLerpSnap = 0.01f;

	// Token: 0x040027DD RID: 10205
	private bool angleSnapped;

	// Token: 0x040027DE RID: 10206
	private bool positionSnapped;

	// Token: 0x040027DF RID: 10207
	public Transform chestTouch;

	// Token: 0x040027E0 RID: 10208
	private int collidersHitCount;

	// Token: 0x040027E1 RID: 10209
	private Collider[] collidersHit = new Collider[20];

	// Token: 0x040027E2 RID: 10210
	private RaycastHit[] raycastHits = new RaycastHit[20];

	// Token: 0x040027E3 RID: 10211
	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	// Token: 0x040027E4 RID: 10212
	private RaycastHit nullHit;

	// Token: 0x040027E5 RID: 10213
	public Collider[] collidersToBeIn;

	// Token: 0x040027E6 RID: 10214
	public LayerMask interactableMask;

	// Token: 0x040027E7 RID: 10215
	public int currentFretIndex;

	// Token: 0x040027E8 RID: 10216
	public int lastFretIndex;

	// Token: 0x040027E9 RID: 10217
	public Collider[] frets;

	// Token: 0x040027EA RID: 10218
	private List<Collider> fretsList = new List<Collider>();

	// Token: 0x040027EB RID: 10219
	public AudioClip[] audioClips;

	// Token: 0x040027EC RID: 10220
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x040027ED RID: 10221
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x040027EE RID: 10222
	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	// Token: 0x040027EF RID: 10223
	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	// Token: 0x040027F0 RID: 10224
	private float sphereRadius;

	// Token: 0x040027F1 RID: 10225
	private bool anyHit;

	// Token: 0x040027F2 RID: 10226
	private bool handIn;

	// Token: 0x040027F3 RID: 10227
	private Vector3 spherecastSweep;

	// Token: 0x040027F4 RID: 10228
	public Collider strumCollider;

	// Token: 0x040027F5 RID: 10229
	public float maxVolume = 1f;

	// Token: 0x040027F6 RID: 10230
	public float minVolume = 0.05f;

	// Token: 0x040027F7 RID: 10231
	public float maxVelocity = 2f;

	// Token: 0x040027F8 RID: 10232
	private List<Collider> strumList = new List<Collider>();

	// Token: 0x040027F9 RID: 10233
	private int selfInstrumentIndex = -1;

	// Token: 0x040027FA RID: 10234
	private OneStringGuitar.GuitarStates lastState;

	// Token: 0x040027FB RID: 10235
	private Vector3 startingLeftChestOffset;

	// Token: 0x040027FC RID: 10236
	private Vector3 startingRightChestOffset;

	// Token: 0x040027FD RID: 10237
	private float startingUnsnapDistance;

	// Token: 0x0200048E RID: 1166
	private enum GuitarStates
	{
		// Token: 0x040027FF RID: 10239
		Club = 1,
		// Token: 0x04002800 RID: 10240
		HeldReverseGrip,
		// Token: 0x04002801 RID: 10241
		Playing = 4
	}
}
