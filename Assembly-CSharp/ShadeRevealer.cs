using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B5 RID: 181
public class ShadeRevealer : TransferrableObject
{
	// Token: 0x0600047F RID: 1151 RVA: 0x00019CD8 File Offset: 0x00017ED8
	protected override void Awake()
	{
		base.Awake();
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		for (int i = 0; i < this.enableWhenScanning.Length; i++)
		{
			hashSet.Add(this.enableWhenScanning[i]);
		}
		for (int j = 0; j < this.enableWhenTracking.Length; j++)
		{
			hashSet.Add(this.enableWhenTracking[j]);
		}
		for (int k = 0; k < this.enableWhenLocked.Length; k++)
		{
			hashSet.Add(this.enableWhenLocked[k]);
		}
		for (int l = 0; l < this.enableWhenPrimed.Length; l++)
		{
			hashSet.Add(this.enableWhenPrimed[l]);
		}
		this.objectsToDisableWhenOff = new GameObject[hashSet.Count];
		hashSet.CopyTo(this.objectsToDisableWhenOff);
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00019D9C File Offset: 0x00017F9C
	private float GetDistanceToBeamRay(Vector3 toPosition)
	{
		return Vector3.Cross(this.beamForward.forward, toPosition).magnitude;
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00019DC4 File Offset: 0x00017FC4
	public ShadeRevealer.State GetBeamStateForPosition(Vector3 toPosition, float tolerance)
	{
		if (toPosition.magnitude <= this.beamLength + tolerance && Vector3.Dot(toPosition.normalized, this.beamForward.forward) > 0f)
		{
			float num = this.GetDistanceToBeamRay(toPosition) - tolerance;
			if (num <= this.lockThreshold)
			{
				return ShadeRevealer.State.LOCKED;
			}
			if (num <= this.trackThreshold)
			{
				return ShadeRevealer.State.TRACKING;
			}
		}
		return ShadeRevealer.State.SCANNING;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00019E21 File Offset: 0x00018021
	public ShadeRevealer.State GetBeamStateForCritter(CosmeticCritter critter, float tolerance)
	{
		return this.GetBeamStateForPosition(critter.transform.position - this.beamForward.position, tolerance);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00019E45 File Offset: 0x00018045
	public bool CritterWithinBeamThreshold(CosmeticCritter critter, ShadeRevealer.State criteria, float tolerance)
	{
		return this.GetBeamStateForCritter(critter, tolerance) >= criteria;
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00019E55 File Offset: 0x00018055
	public void SetBestBeamState(ShadeRevealer.State state)
	{
		if (state > this.pendingBeamState)
		{
			this.pendingBeamState = state;
		}
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00019E68 File Offset: 0x00018068
	private void SetObjectsEnabledFromState(ShadeRevealer.State state)
	{
		for (int i = 0; i < this.objectsToDisableWhenOff.Length; i++)
		{
			this.objectsToDisableWhenOff[i].SetActive(false);
		}
		GameObject[] array;
		switch (state)
		{
		case ShadeRevealer.State.SCANNING:
			array = this.enableWhenScanning;
			break;
		case ShadeRevealer.State.TRACKING:
			array = this.enableWhenTracking;
			break;
		case ShadeRevealer.State.LOCKED:
			array = this.enableWhenLocked;
			break;
		case ShadeRevealer.State.PRIMED:
			array = this.enableWhenPrimed;
			break;
		default:
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(true);
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00019EE8 File Offset: 0x000180E8
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.currentBeamState != this.pendingBeamState)
		{
			this.currentBeamState = this.pendingBeamState;
			this.SetObjectsEnabledFromState(this.currentBeamState);
		}
		this.beamSFX.pitch = 1f + this.shadeCatcher.GetActionTimeFrac() * 2f;
		if (this.isScanning)
		{
			this.pendingBeamState = ShadeRevealer.State.SCANNING;
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00019F52 File Offset: 0x00018152
	public void StartScanning()
	{
		this.shadeCatcher.enabled = true;
		this.initialActivationSFX.GTPlay();
		this.beamSFX.GTPlay();
		this.isScanning = true;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.SCANNING;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00019F8C File Offset: 0x0001818C
	public void StopScanning()
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			UnityEvent unityEvent = this.onShadeLaunched;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.shadeCatcher.enabled = false;
		this.initialActivationSFX.GTStop();
		this.beamSFX.GTStop();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.OFF;
		this.SetObjectsEnabledFromState(ShadeRevealer.State.OFF);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00019FF4 File Offset: 0x000181F4
	public void ShadeCaught()
	{
		this.shadeCatcher.enabled = false;
		this.beamSFX.GTStop();
		this.catchSFX.GTPlay();
		this.catchFX.Play();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.PRIMED;
	}

	// Token: 0x0400052D RID: 1325
	[SerializeField]
	private AudioSource initialActivationSFX;

	// Token: 0x0400052E RID: 1326
	[SerializeField]
	private AudioSource beamSFX;

	// Token: 0x0400052F RID: 1327
	[SerializeField]
	private AudioSource catchSFX;

	// Token: 0x04000530 RID: 1328
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x04000531 RID: 1329
	[Space]
	[SerializeField]
	private CosmeticCritterCatcherShade shadeCatcher;

	// Token: 0x04000532 RID: 1330
	[Space]
	[Tooltip("The transform that represents the origin of the revealer beam.")]
	[SerializeField]
	private Transform beamForward;

	// Token: 0x04000533 RID: 1331
	[Tooltip("The maximum length of the beam.")]
	[SerializeField]
	private float beamLength;

	// Token: 0x04000534 RID: 1332
	[Tooltip("If the Shade is this close to the beam, set it to flee and have all Revealers enter Tracking mode.")]
	[SerializeField]
	private float trackThreshold;

	// Token: 0x04000535 RID: 1333
	[Tooltip("If the Shade is this close to the beam, slow it down.")]
	[SerializeField]
	private float lockThreshold;

	// Token: 0x04000536 RID: 1334
	[Tooltip("Editor-only object to help test the thresholds.")]
	[SerializeField]
	private Transform thresholdTester;

	// Token: 0x04000537 RID: 1335
	[Tooltip("Whether to draw the tester or not.")]
	[SerializeField]
	private bool drawThresholdTesterInEditor = true;

	// Token: 0x04000538 RID: 1336
	[Space]
	[Tooltip("Enable these objects while the beam is in Scanning mode.")]
	[SerializeField]
	private GameObject[] enableWhenScanning;

	// Token: 0x04000539 RID: 1337
	[Tooltip("Enable these objects while the beam is in Tracking mode.")]
	[SerializeField]
	private GameObject[] enableWhenTracking;

	// Token: 0x0400053A RID: 1338
	[Tooltip("Enable these objects while the beam is in Locked mode.")]
	[SerializeField]
	private GameObject[] enableWhenLocked;

	// Token: 0x0400053B RID: 1339
	[Tooltip("Enable these objects while ready to fire.")]
	[SerializeField]
	private GameObject[] enableWhenPrimed;

	// Token: 0x0400053C RID: 1340
	[Space]
	[SerializeField]
	private UnityEvent onShadeLaunched;

	// Token: 0x0400053D RID: 1341
	private bool isScanning;

	// Token: 0x0400053E RID: 1342
	private ShadeRevealer.State currentBeamState;

	// Token: 0x0400053F RID: 1343
	private ShadeRevealer.State pendingBeamState;

	// Token: 0x04000540 RID: 1344
	private GameObject[] objectsToDisableWhenOff;

	// Token: 0x020000B6 RID: 182
	public enum State
	{
		// Token: 0x04000542 RID: 1346
		OFF,
		// Token: 0x04000543 RID: 1347
		SCANNING,
		// Token: 0x04000544 RID: 1348
		TRACKING,
		// Token: 0x04000545 RID: 1349
		LOCKED,
		// Token: 0x04000546 RID: 1350
		PRIMED
	}
}
