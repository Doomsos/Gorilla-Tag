using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200047A RID: 1146
public class EdibleHoldable : TransferrableObject
{
	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001D2B RID: 7467 RVA: 0x00099E56 File Offset: 0x00098056
	// (set) Token: 0x06001D2C RID: 7468 RVA: 0x00099E5E File Offset: 0x0009805E
	public int lastBiterActorID { get; private set; } = -1;

	// Token: 0x06001D2D RID: 7469 RVA: 0x00099E67 File Offset: 0x00098067
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x00099E9C File Offset: 0x0009809C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x00099EB8 File Offset: 0x000980B8
	public override void OnActivate()
	{
		base.OnActivate();
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x00099EC0 File Offset: 0x000980C0
	internal override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x0003CCC0 File Offset: 0x0003AEC0
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x00099EC8 File Offset: 0x000980C8
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x00099ED0 File Offset: 0x000980D0
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && !base.InHand();
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x00099EEC File Offset: 0x000980EC
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			if (Time.time > this.lastFullyEatenTime + this.respawnTime)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				return;
			}
		}
		else if (Time.time > this.lastEatTime + this.eatMinimumCooldown)
		{
			bool flag = false;
			bool flag2 = false;
			float num = this.biteDistance * this.biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			VRRig vrrig = null;
			VRRig vrrig2 = null;
			for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
			{
				VRRig vrrig3 = GorillaParent.instance.vrrigs[i];
				if (!vrrig3.isOfflineVRRig)
				{
					if (vrrig3.head == null || vrrig3.head.rigTarget == null)
					{
						break;
					}
					Transform transform = vrrig3.head.rigTarget.transform;
					if ((transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
						vrrig2 = vrrig3;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (flag && !this.inBiteZone && (!flag2 || base.InHand()) && this.itemState != TransferrableObject.ItemStates.State3)
			{
				if (this.itemState == TransferrableObject.ItemStates.State0)
				{
					this.itemState = TransferrableObject.ItemStates.State1;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State1)
				{
					this.itemState = TransferrableObject.ItemStates.State2;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State2)
				{
					this.itemState = TransferrableObject.ItemStates.State3;
				}
				this.lastEatTime = Time.time;
				this.lastFullyEatenTime = Time.time;
			}
			if (flag)
			{
				if (flag2)
				{
					int lastBiterActorID;
					if (!vrrig)
					{
						lastBiterActorID = -1;
					}
					else
					{
						NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
						lastBiterActorID = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : -1);
					}
					this.lastBiterActorID = lastBiterActorID;
					EdibleHoldable.BiteEvent biteEvent = this.onBiteView;
					if (biteEvent != null)
					{
						biteEvent.Invoke(vrrig, (int)this.itemState);
					}
				}
				else
				{
					int lastBiterActorID2;
					if (!vrrig2)
					{
						lastBiterActorID2 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer2 = vrrig2.OwningNetPlayer;
						lastBiterActorID2 = ((owningNetPlayer2 != null) ? owningNetPlayer2.ActorNumber : -1);
					}
					this.lastBiterActorID = lastBiterActorID2;
					EdibleHoldable.BiteEvent biteEvent2 = this.onBiteWorld;
					if (biteEvent2 != null)
					{
						biteEvent2.Invoke(vrrig2, (int)this.itemState);
					}
				}
			}
			this.inBiteZone = flag;
		}
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x0009A16C File Offset: 0x0009836C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldable.EdibleHoldableStates itemState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		if (itemState != this.previousEdibleState)
		{
			this.OnEdibleHoldableStateChange();
		}
		this.previousEdibleState = itemState;
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0009A19C File Offset: 0x0009839C
	protected virtual void OnEdibleHoldableStateChange()
	{
		float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float volumeScale = 0.08f;
		int num = 0;
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			num = 0;
			if (this.iResettableItems != null)
			{
				foreach (IResettableItem resettableItem in this.iResettableItems)
				{
					if (resettableItem != null)
					{
						resettableItem.ResetToDefaultState();
					}
				}
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num = 1;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			num = 2;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			num = 3;
		}
		int num2 = num - 1;
		if (num2 < 0)
		{
			num2 = this.edibleMeshObjects.Length - 1;
		}
		this.edibleMeshObjects[num2].SetActive(false);
		this.edibleMeshObjects[num].SetActive(true);
		if ((this.itemState != TransferrableObject.ItemStates.State0 && this.onBiteView != null) || this.onBiteWorld != null)
		{
			VRRig vrrig = null;
			float num3 = float.PositiveInfinity;
			for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
			{
				VRRig vrrig2 = GorillaParent.instance.vrrigs[j];
				if (vrrig2.head == null || vrrig2.head.rigTarget == null)
				{
					break;
				}
				Transform transform = vrrig2.head.rigTarget.transform;
				float sqrMagnitude = (transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
			if (vrrig != null)
			{
				EdibleHoldable.BiteEvent biteEvent = vrrig.isOfflineVRRig ? this.onBiteView : this.onBiteWorld;
				if (biteEvent != null)
				{
					biteEvent.Invoke(vrrig, (int)this.itemState);
				}
				if (vrrig.isOfflineVRRig && this.itemState != TransferrableObject.ItemStates.State0)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
			}
		}
		this.eatSoundSource.GTPlayOneShot(this.eatSounds[num], volumeScale);
		if (this.IsMyItem())
		{
			if (base.InHand())
			{
				GorillaTagger.Instance.StartVibration(base.InLeftHand(), amplitude, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(false, amplitude, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(true, amplitude, fixedDeltaTime);
		}
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001D38 RID: 7480 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0400272B RID: 10027
	public AudioClip[] eatSounds;

	// Token: 0x0400272C RID: 10028
	public GameObject[] edibleMeshObjects;

	// Token: 0x0400272E RID: 10030
	public EdibleHoldable.BiteEvent onBiteView;

	// Token: 0x0400272F RID: 10031
	public EdibleHoldable.BiteEvent onBiteWorld;

	// Token: 0x04002730 RID: 10032
	[DebugReadout]
	public float lastEatTime;

	// Token: 0x04002731 RID: 10033
	[DebugReadout]
	public float lastFullyEatenTime;

	// Token: 0x04002732 RID: 10034
	public float eatMinimumCooldown = 1f;

	// Token: 0x04002733 RID: 10035
	public float respawnTime = 7f;

	// Token: 0x04002734 RID: 10036
	public float biteDistance = 0.1666667f;

	// Token: 0x04002735 RID: 10037
	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x04002736 RID: 10038
	public Transform biteSpot;

	// Token: 0x04002737 RID: 10039
	public bool inBiteZone;

	// Token: 0x04002738 RID: 10040
	public AudioSource eatSoundSource;

	// Token: 0x04002739 RID: 10041
	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	// Token: 0x0400273A RID: 10042
	private IResettableItem[] iResettableItems;

	// Token: 0x0200047B RID: 1147
	private enum EdibleHoldableStates
	{
		// Token: 0x0400273C RID: 10044
		EatingState0 = 1,
		// Token: 0x0400273D RID: 10045
		EatingState1,
		// Token: 0x0400273E RID: 10046
		EatingState2 = 4,
		// Token: 0x0400273F RID: 10047
		EatingState3 = 8
	}

	// Token: 0x0200047C RID: 1148
	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
	}
}
