using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000479 RID: 1145
public class DrumsItem : MonoBehaviour, ISpawnable
{
	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001D20 RID: 7456 RVA: 0x00099951 File Offset: 0x00097B51
	// (set) Token: 0x06001D21 RID: 7457 RVA: 0x00099959 File Offset: 0x00097B59
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001D22 RID: 7458 RVA: 0x00099962 File Offset: 0x00097B62
	// (set) Token: 0x06001D23 RID: 7459 RVA: 0x0009996A File Offset: 0x00097B6A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001D24 RID: 7460 RVA: 0x00099974 File Offset: 0x00097B74
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		for (int i = 0; i < this.collidersForThisDrum.Length; i++)
		{
			this.collidersForThisDrumList.Add(this.collidersForThisDrum[i]);
		}
		for (int j = 0; j < this.drumsAS.Length; j++)
		{
			this.myRig.AssignDrumToMusicDrums(j + this.onlineOffset, this.drumsAS[j]);
		}
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x00099A1C File Offset: 0x00097C1C
	private void LateUpdate()
	{
		this.CheckHandHit(ref this.leftHandIn, ref this.leftHandIndicator, true);
		this.CheckHandHit(ref this.rightHandIn, ref this.rightHandIndicator, false);
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x00099A44 File Offset: 0x00097C44
	private void CheckHandHit(ref bool handIn, ref GorillaTriggerColliderHandIndicator handIndicator, bool isLeftHand)
	{
		this.spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (this.spherecastSweep.magnitude < 0.0001f)
		{
			this.spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = this.nullHit;
		}
		this.collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, this.sphereRadius, this.spherecastSweep.normalized, this.collidersHit, this.spherecastSweep.magnitude, this.drumsTouchable, 2);
		this.drumHit = false;
		if (this.collidersHitCount > 0)
		{
			this.hitList.Clear();
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.collidersHit[j].collider != null && this.collidersForThisDrumList.Contains(this.collidersHit[j].collider) && this.collidersHit[j].collider.gameObject.activeSelf)
				{
					this.hitList.Add(this.collidersHit[j]);
				}
			}
			this.hitList.Sort(new Comparison<RaycastHit>(this.RayCastHitCompare));
			int k = 0;
			while (k < this.hitList.Count)
			{
				this.tempDrum = this.hitList[k].collider.GetComponent<Drum>();
				if (this.tempDrum != null)
				{
					this.drumHit = true;
					if (!handIn && !this.tempDrum.disabler)
					{
						this.DrumHit(this.tempDrum, isLeftHand, handIndicator.currentVelocity.magnitude);
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
		}
		if (!this.drumHit & handIn)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration);
		}
		handIn = this.drumHit;
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x00099C5F File Offset: 0x00097E5F
	private int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x00099C88 File Offset: 0x00097E88
	public void DrumHit(Drum tempDrumInner, bool isLeftHand, float hitVelocity)
	{
		if (isLeftHand)
		{
			if (this.leftHandIn)
			{
				return;
			}
			this.leftHandIn = true;
		}
		else
		{
			if (this.rightHandIn)
			{
				return;
			}
			this.rightHandIn = true;
		}
		this.volToPlay = Mathf.Max(Mathf.Min(1f, hitVelocity / this.maxDrumVolumeVelocity) * this.maxDrumVolume, this.minDrumVolume);
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.myRig.isOfflineVRRig)
			{
				NetworkView netView = this.myRig.netView;
				if (netView != null)
				{
					netView.SendRPC("RPC_PlayDrum", 1, new object[]
					{
						tempDrumInner.myIndex + this.onlineOffset,
						this.volToPlay
					});
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayDrum", 1, new object[]
				{
					tempDrumInner.myIndex + this.onlineOffset,
					this.volToPlay
				});
			}
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
		this.drumsAS[tempDrumInner.myIndex].volume = this.maxDrumVolume;
		this.drumsAS[tempDrumInner.myIndex].GTPlayOneShot(this.drumsAS[tempDrumInner.myIndex].clip, this.volToPlay);
	}

	// Token: 0x04002712 RID: 10002
	[Tooltip("Array of colliders for this specific drum.")]
	public Collider[] collidersForThisDrum;

	// Token: 0x04002713 RID: 10003
	private List<Collider> collidersForThisDrumList = new List<Collider>();

	// Token: 0x04002714 RID: 10004
	[Tooltip("AudioSources where each index must match the index given to the corresponding Drum component.")]
	public AudioSource[] drumsAS;

	// Token: 0x04002715 RID: 10005
	[Tooltip("Max volume a drum can reach.")]
	public float maxDrumVolume = 0.2f;

	// Token: 0x04002716 RID: 10006
	[Tooltip("Min volume a drum can reach.")]
	public float minDrumVolume = 0.05f;

	// Token: 0x04002717 RID: 10007
	[Tooltip("Multiplies against actual velocity before capping by min & maxDrumVolume values.")]
	public float maxDrumVolumeVelocity = 1f;

	// Token: 0x04002718 RID: 10008
	private bool rightHandIn;

	// Token: 0x04002719 RID: 10009
	private bool leftHandIn;

	// Token: 0x0400271A RID: 10010
	private float volToPlay;

	// Token: 0x0400271B RID: 10011
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x0400271C RID: 10012
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x0400271D RID: 10013
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x0400271E RID: 10014
	private Collider[] actualColliders = new Collider[20];

	// Token: 0x0400271F RID: 10015
	public LayerMask drumsTouchable;

	// Token: 0x04002720 RID: 10016
	private float sphereRadius;

	// Token: 0x04002721 RID: 10017
	private Vector3 spherecastSweep;

	// Token: 0x04002722 RID: 10018
	private int collidersHitCount;

	// Token: 0x04002723 RID: 10019
	private List<RaycastHit> hitList = new List<RaycastHit>(20);

	// Token: 0x04002724 RID: 10020
	private Drum tempDrum;

	// Token: 0x04002725 RID: 10021
	private bool drumHit;

	// Token: 0x04002726 RID: 10022
	private RaycastHit nullHit;

	// Token: 0x04002727 RID: 10023
	public int onlineOffset;

	// Token: 0x04002728 RID: 10024
	[Tooltip("VRRig object of the player, used to determine if it is an offline rig.")]
	private VRRig myRig;
}
