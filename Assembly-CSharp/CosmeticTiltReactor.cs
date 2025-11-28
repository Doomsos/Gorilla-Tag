using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005CC RID: 1484
public class CosmeticTiltReactor : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002596 RID: 9622 RVA: 0x000C8D20 File Offset: 0x000C6F20
	private void Awake()
	{
		this.referenceDirection.Normalize();
		if (!this.useTransform && this.referenceDirection == Vector3.zero)
		{
			GTDev.LogError<string>("CosmeticTiltReactor " + base.gameObject.name + " referenceDirection cannot be 0 vector", null);
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			GTDev.LogError<string>("CosmeticTiltReactor " + base.gameObject.name + " referenceTransform cannot be null", null);
		}
		this.hasContinuousProperties = (this.continuousProperties != null && this.continuousProperties.Count > 0);
		this.calculateDot = this.hasContinuousProperties;
		using (List<CosmeticTiltReactor.TiltEvent>.Enumerator enumerator = this.events.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.comparisonMethod == CosmeticTiltReactor.TiltEvent.ComparisonMethod.DotProduct)
				{
					this.calculateDot = true;
				}
				else
				{
					this.calculateAngle = true;
				}
				if (this.calculateDot && this.calculateAngle)
				{
					break;
				}
			}
		}
		this._rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		if (this._rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
		{
			this._rig = GorillaTagger.Instance.offlineVRRig;
		}
		if (this._rig == null && !this.syncForAllPlayers)
		{
			GTDev.LogError<string>("CosmeticTiltReactor on " + base.gameObject.name + " set to not syncForAllPlayers and has no VR Rig parent. Events will not fire", null);
		}
		else if (this._rig != null)
		{
			this.isLocallyOwned = this._rig.isLocal;
		}
		if (this.parentTransferable == null && this.onlyWhileHeld)
		{
			GTDev.LogError<string>("CosmeticTiltReactor on " + base.gameObject.name + " set to OnlyWhileHeld but has no TransferrableObject parent. Events will not fire", null);
		}
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x000C8F10 File Offset: 0x000C7110
	public void OnEnable()
	{
		if (!this.syncForAllPlayers && !this.isLocallyOwned)
		{
			return;
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			return;
		}
		Vector3 vector = this.useTransform ? this.referenceTransform.up : this.referenceDirection;
		if (this.calculateAngle)
		{
			this.angle = Vector3.Angle(base.transform.up, vector);
		}
		if (this.calculateDot)
		{
			this.dotProduct = Vector3.Dot(base.transform.up, vector);
		}
		this.ResetEvents();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x000C8FAC File Offset: 0x000C71AC
	public void OnDisable()
	{
		if (!this.syncForAllPlayers && !this.isLocallyOwned)
		{
			return;
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			return;
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x000C8FE0 File Offset: 0x000C71E0
	public void SliceUpdate()
	{
		if (this.onlyWhileHeld)
		{
			bool flag = this.parentTransferable != null && this.parentTransferable.InHand();
			if (!flag && this.wasInHand)
			{
				this.ResetEvents();
			}
			this.wasInHand = flag;
			if (!flag)
			{
				return;
			}
		}
		Vector3 vector = this.useTransform ? this.referenceTransform.up : this.referenceDirection;
		if (this.calculateAngle)
		{
			this.angle = Vector3.Angle(base.transform.up, vector);
		}
		if (this.calculateDot)
		{
			this.dotProduct = Vector3.Dot(base.transform.up, vector);
		}
		this.FireEvents();
		if (this.hasContinuousProperties)
		{
			this.continuousProperties.ApplyAll(this.dotProduct);
		}
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000C90A8 File Offset: 0x000C72A8
	private void ResetEvents()
	{
		if (this.events == null || this.events.Count <= 0)
		{
			return;
		}
		foreach (CosmeticTiltReactor.TiltEvent tiltEvent in this.events)
		{
			switch (tiltEvent.tiltEventType)
			{
			case CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold:
				tiltEvent.wasGreater = true;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold:
				tiltEvent.wasGreater = false;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThresholdForDuration:
				tiltEvent.wasGreater = true;
				tiltEvent.hasFired = false;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThresholdForDuration:
				tiltEvent.wasGreater = false;
				tiltEvent.hasFired = false;
				break;
			}
			tiltEvent.thresholdCrossTime = double.MinValue;
		}
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000C916C File Offset: 0x000C736C
	private void FireEvents()
	{
		if (this.events == null || this.events.Count <= 0)
		{
			return;
		}
		foreach (CosmeticTiltReactor.TiltEvent tiltEvent in this.events)
		{
			bool flag = (tiltEvent.comparisonMethod == CosmeticTiltReactor.TiltEvent.ComparisonMethod.Angle) ? (this.angle > tiltEvent.angleThreshold) : (this.dotProduct > tiltEvent.dotThreshold);
			CosmeticTiltReactor.TiltEvent.TiltEventType tiltEventType = tiltEvent.tiltEventType;
			if (tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold || tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold)
			{
				if (flag != tiltEvent.wasGreater)
				{
					if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold && flag)
					{
						if (tiltEvent.thresholdCrossTime + (double)tiltEvent.retriggerDelay <= Time.timeAsDouble)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
							tiltEvent.wasGreater = true;
							UnityEvent onTiltEvent = tiltEvent.OnTiltEvent;
							if (onTiltEvent != null)
							{
								onTiltEvent.Invoke();
							}
						}
					}
					else if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold && !flag)
					{
						if (tiltEvent.thresholdCrossTime + (double)tiltEvent.retriggerDelay <= Time.timeAsDouble)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
							tiltEvent.wasGreater = false;
							UnityEvent onTiltEvent2 = tiltEvent.OnTiltEvent;
							if (onTiltEvent2 != null)
							{
								onTiltEvent2.Invoke();
							}
						}
					}
					else
					{
						tiltEvent.wasGreater = flag;
					}
				}
			}
			else
			{
				if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThresholdForDuration)
				{
					if (flag)
					{
						if (!tiltEvent.wasGreater)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
						}
						else if (!tiltEvent.hasFired && tiltEvent.thresholdCrossTime + (double)tiltEvent.duration <= Time.timeAsDouble)
						{
							UnityEvent onTiltEvent3 = tiltEvent.OnTiltEvent;
							if (onTiltEvent3 != null)
							{
								onTiltEvent3.Invoke();
							}
							tiltEvent.hasFired = true;
						}
					}
					else
					{
						tiltEvent.hasFired = false;
					}
				}
				if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThresholdForDuration)
				{
					if (!flag)
					{
						if (tiltEvent.wasGreater)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
						}
						else if (!tiltEvent.hasFired && tiltEvent.thresholdCrossTime + (double)tiltEvent.duration <= Time.timeAsDouble)
						{
							UnityEvent onTiltEvent4 = tiltEvent.OnTiltEvent;
							if (onTiltEvent4 != null)
							{
								onTiltEvent4.Invoke();
							}
							tiltEvent.hasFired = true;
						}
					}
					else
					{
						tiltEvent.hasFired = false;
					}
				}
				tiltEvent.wasGreater = flag;
			}
		}
	}

	// Token: 0x0400311A RID: 12570
	[SerializeField]
	private bool useTransform;

	// Token: 0x0400311B RID: 12571
	[Tooltip("Direction to which this transform's y is compared in world space")]
	[SerializeField]
	private Vector3 referenceDirection = Vector3.up;

	// Token: 0x0400311C RID: 12572
	[Tooltip("compare referenceTransform's y to this transform's y")]
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x0400311D RID: 12573
	[SerializeField]
	private List<CosmeticTiltReactor.TiltEvent> events;

	// Token: 0x0400311E RID: 12574
	[Tooltip("input for continuous properties is the dot product of this transform's y and the reference direction")]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x0400311F RID: 12575
	[Tooltip("Should this script be run for all clients or just the owner")]
	[SerializeField]
	private bool syncForAllPlayers = true;

	// Token: 0x04003120 RID: 12576
	[Tooltip("option to run only if this transferrable object is in the hand")]
	[SerializeField]
	private bool onlyWhileHeld;

	// Token: 0x04003121 RID: 12577
	private VRRig _rig;

	// Token: 0x04003122 RID: 12578
	private TransferrableObject parentTransferable;

	// Token: 0x04003123 RID: 12579
	private bool isLocallyOwned;

	// Token: 0x04003124 RID: 12580
	private bool hasContinuousProperties;

	// Token: 0x04003125 RID: 12581
	private float angle;

	// Token: 0x04003126 RID: 12582
	private float dotProduct;

	// Token: 0x04003127 RID: 12583
	private bool calculateAngle;

	// Token: 0x04003128 RID: 12584
	private bool calculateDot;

	// Token: 0x04003129 RID: 12585
	private bool wasInHand;

	// Token: 0x020005CD RID: 1485
	[Serializable]
	public class TiltEvent
	{
		// Token: 0x0600259D RID: 9629 RVA: 0x000C93B4 File Offset: 0x000C75B4
		public TiltEvent()
		{
			this.tiltEventType = CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold;
			this.comparisonMethod = CosmeticTiltReactor.TiltEvent.ComparisonMethod.DotProduct;
			this.angleThreshold = 15f;
			this.retriggerDelay = 0f;
			this.duration = 0.5f;
		}

		// Token: 0x0400312A RID: 12586
		public CosmeticTiltReactor.TiltEvent.ComparisonMethod comparisonMethod;

		// Token: 0x0400312B RID: 12587
		public CosmeticTiltReactor.TiltEvent.TiltEventType tiltEventType;

		// Token: 0x0400312C RID: 12588
		[Range(0f, 180f)]
		[Tooltip("Angle in degrees from the reference direction")]
		public float angleThreshold;

		// Token: 0x0400312D RID: 12589
		[Range(-1f, 1f)]
		[Tooltip("Dot product compared to the reference direction")]
		public float dotThreshold;

		// Token: 0x0400312E RID: 12590
		[Tooltip("Minimum time between events firing")]
		public float retriggerDelay;

		// Token: 0x0400312F RID: 12591
		[Tooltip("Amount of time the angle or dot product should be less/greater than the threshold before firing an event")]
		public float duration;

		// Token: 0x04003130 RID: 12592
		public UnityEvent OnTiltEvent;

		// Token: 0x04003131 RID: 12593
		[NonSerialized]
		public bool wasGreater;

		// Token: 0x04003132 RID: 12594
		[NonSerialized]
		public bool hasFired;

		// Token: 0x04003133 RID: 12595
		[NonSerialized]
		public double thresholdCrossTime = double.MinValue;

		// Token: 0x020005CE RID: 1486
		public enum ComparisonMethod
		{
			// Token: 0x04003135 RID: 12597
			DotProduct,
			// Token: 0x04003136 RID: 12598
			Angle
		}

		// Token: 0x020005CF RID: 1487
		public enum TiltEventType
		{
			// Token: 0x04003138 RID: 12600
			LessThanThreshold,
			// Token: 0x04003139 RID: 12601
			GreaterThanThreshold,
			// Token: 0x0400313A RID: 12602
			LessThanThresholdForDuration,
			// Token: 0x0400313B RID: 12603
			GreaterThanThresholdForDuration
		}
	}
}
