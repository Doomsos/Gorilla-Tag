using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class SnowballMaker : MonoBehaviourPostTick
{
	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000B01 RID: 2817 RVA: 0x0003BD0B File Offset: 0x00039F0B
	// (set) Token: 0x06000B02 RID: 2818 RVA: 0x0003BD12 File Offset: 0x00039F12
	public static SnowballMaker leftHandInstance { get; private set; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000B03 RID: 2819 RVA: 0x0003BD1A File Offset: 0x00039F1A
	// (set) Token: 0x06000B04 RID: 2820 RVA: 0x0003BD21 File Offset: 0x00039F21
	public static SnowballMaker rightHandInstance { get; private set; }

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000B05 RID: 2821 RVA: 0x0003BD29 File Offset: 0x00039F29
	// (set) Token: 0x06000B06 RID: 2822 RVA: 0x0003BD31 File Offset: 0x00039F31
	public SnowballThrowable[] snowballs { get; private set; }

	// Token: 0x06000B07 RID: 2823 RVA: 0x0003BD3C File Offset: 0x00039F3C
	private void Awake()
	{
		if (this.isLeftHand)
		{
			if (SnowballMaker.leftHandInstance == null)
			{
				SnowballMaker.leftHandInstance = this;
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
		else
		{
			if (SnowballMaker.rightHandInstance == null)
			{
				SnowballMaker.rightHandInstance = this;
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0003BD90 File Offset: 0x00039F90
	private void Start()
	{
		this.handTransform = (this.isLeftHand ? GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.leftHandTransform : GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.rightHandTransform);
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0003BDCC File Offset: 0x00039FCC
	internal void SetupThrowables(SnowballThrowable[] newThrowables)
	{
		this.snowballs = newThrowables;
		for (int i = 0; i < this.snowballs.Length; i++)
		{
			for (int j = 0; j < this.snowballs[i].matDataIndexes.Count; j++)
			{
				this.matSnowballLookup.TryAdd(this.snowballs[i].matDataIndexes[j], this.snowballs[i]);
			}
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0003BE38 File Offset: 0x0003A038
	public override void PostTick()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		if (this.snowballs == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance != null && BuilderPieceInteractor.instance.BlockSnowballCreation())
		{
			return;
		}
		if (!GTPlayer.hasInstance || !EquipmentInteractor.hasInstance || !GorillaTagger.hasInstance || !GorillaTagger.Instance.offlineVRRig || this.snowballs.Length == 0)
		{
			return;
		}
		int materialTouchIndex = GTPlayer.Instance.GetMaterialTouchIndex(this.isLeftHand);
		if (materialTouchIndex == 0)
		{
			if (Time.time > this.lastGroundContactTime + this.snowballCreationCooldownTime)
			{
				this.requiresFreshMaterialContact = false;
			}
			return;
		}
		this.lastGroundContactTime = Time.time;
		EquipmentInteractor instance = EquipmentInteractor.instance;
		bool flag = (this.isLeftHand ? instance.leftHandHeldEquipment : instance.rightHandHeldEquipment) != null;
		bool flag2 = this.isLeftHand ? instance.isLeftGrabbing : instance.isRightGrabbing;
		bool flag3 = false;
		if (flag2 && !this.requiresFreshMaterialContact)
		{
			int num = -1;
			for (int i = 0; i < this.snowballs.Length; i++)
			{
				if (this.snowballs[i].gameObject.activeSelf)
				{
					num = i;
					break;
				}
			}
			SnowballThrowable snowballThrowable = (num > -1) ? this.snowballs[num] : null;
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			bool flag4 = this.isLeftHand ? (!ConnectedControllerHandler.Instance.RightValid) : (!ConnectedControllerHandler.Instance.LeftValid);
			if (growingSnowballThrowable != null && (!GrowingSnowballThrowable.twoHandedSnowballGrowing || flag4 || flag3))
			{
				if (snowballThrowable.matDataIndexes.Contains(materialTouchIndex))
				{
					growingSnowballThrowable.IncreaseSize(1);
					GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.requiresFreshMaterialContact = true;
					return;
				}
			}
			else if (!flag)
			{
				SnowballThrowable snowballThrowable2;
				if (!this.matSnowballLookup.TryGetValue(materialTouchIndex, ref snowballThrowable2))
				{
					return;
				}
				Transform transform = snowballThrowable2.transform;
				Transform transform2 = this.handTransform;
				XformOffset spawnOffset = snowballThrowable2.SpawnOffset;
				snowballThrowable2.SetSnowballActiveLocal(true);
				snowballThrowable2.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				this.requiresFreshMaterialContact = true;
			}
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0003C0BC File Offset: 0x0003A2BC
	public bool TryCreateSnowball(int materialIndex, out SnowballThrowable result)
	{
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			if (snowballThrowable.matDataIndexes.Contains(materialIndex))
			{
				Transform transform = snowballThrowable.transform;
				Transform transform2 = this.handTransform;
				XformOffset spawnOffset = snowballThrowable.SpawnOffset;
				snowballThrowable.SetSnowballActiveLocal(true);
				snowballThrowable.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				result = snowballThrowable;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x04000D6F RID: 3439
	public bool isLeftHand;

	// Token: 0x04000D71 RID: 3441
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000D72 RID: 3442
	private float snowballCreationCooldownTime = 0.1f;

	// Token: 0x04000D73 RID: 3443
	private float lastGroundContactTime;

	// Token: 0x04000D74 RID: 3444
	private bool requiresFreshMaterialContact;

	// Token: 0x04000D75 RID: 3445
	private Transform handTransform;

	// Token: 0x04000D76 RID: 3446
	private Dictionary<int, SnowballThrowable> matSnowballLookup = new Dictionary<int, SnowballThrowable>();
}
