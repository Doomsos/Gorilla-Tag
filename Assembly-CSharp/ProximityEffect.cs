using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200027E RID: 638
public class ProximityEffect : MonoBehaviour, ITickSystemTick
{
	// Token: 0x0600105A RID: 4186 RVA: 0x00055ADD File Offset: 0x00053CDD
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.enableVisualization = false;
		if (this.visualizer)
		{
			Object.Destroy(this.visualizer);
		}
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00055B0A File Offset: 0x00053D0A
	public void AddReceiver(IProximityEffectReceiver receiver)
	{
		if (this.receivers == null)
		{
			List<IProximityEffectReceiver> list = new List<IProximityEffectReceiver>();
			list.Add(receiver);
			this.receivers = list;
			return;
		}
		if (!this.receivers.Contains(receiver))
		{
			this.receivers.Add(receiver);
		}
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x00055B41 File Offset: 0x00053D41
	public void RemoveReceiver(IProximityEffectReceiver receiver)
	{
		this.receivers.Remove(receiver);
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00055B50 File Offset: 0x00053D50
	private void StartCalculating()
	{
		this.centerTransform.position = (this.leftTransform.position + this.rightTransform.position) / 2f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x00055B88 File Offset: 0x00053D88
	private void StopCalculating()
	{
		ProximityEffect.ProximityEvent[] array = this.events;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetAllEvents();
		}
		ContinuousPropertyArray continuousPropertyArray = this.continuousProperties;
		if (continuousPropertyArray != null)
		{
			continuousPropertyArray.ApplyAll(0f);
		}
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(0f);
		}
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x00055BE4 File Offset: 0x00053DE4
	private void OnEnable()
	{
		if (this.triggersToActivate == 0)
		{
			this.StartCalculating();
		}
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00055BF4 File Offset: 0x00053DF4
	private void OnDisable()
	{
		if (this.triggersToActivate == 0)
		{
			this.StopCalculating();
		}
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00055C04 File Offset: 0x00053E04
	public void AddTrigger()
	{
		if (this.numTriggers < this.triggersToActivate)
		{
			this.numTriggers++;
			if (this.numTriggers == this.triggersToActivate)
			{
				this.StartCalculating();
			}
		}
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00055C36 File Offset: 0x00053E36
	public void RemoveTrigger()
	{
		if (this.numTriggers > 0)
		{
			if (this.numTriggers == this.triggersToActivate)
			{
				this.StopCalculating();
			}
			this.numTriggers--;
		}
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x00055C64 File Offset: 0x00053E64
	private void CalculateProximityScores()
	{
		float num;
		float num2;
		float num3;
		Vector3 vector;
		this.CalculateProximityScores(true, out num, out num2, out num3, out vector);
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00055C80 File Offset: 0x00053E80
	private void CalculateProximityScores(out float distance, out float alignment, out float parallel, out Vector3 midpoint)
	{
		this.CalculateProximityScores(false, out distance, out alignment, out parallel, out midpoint);
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00055C90 File Offset: 0x00053E90
	private void CalculateProximityScores(bool drawGizmos, out float distance, out float alignment, out float parallel, out Vector3 midpoint)
	{
		float num = (this.rig != null) ? this.rig.scaleFactor : 1f;
		Vector3 position = this.leftTransform.position;
		Vector3 position2 = this.rightTransform.position;
		Vector3 forward = this.leftTransform.forward;
		Vector3 forward2 = this.rightTransform.forward;
		Vector3 vector = (position2 - position) / num;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		distance = this.scoreCurves.distanceModifierCurve.Evaluate(magnitude);
		alignment = this.scoreCurves.alignmentModifierCurve.Evaluate(-Vector3.Dot(forward, forward2));
		parallel = this.scoreCurves.parallelModifierCurve.Evaluate((Vector3.Dot(forward, vector2) + Vector3.Dot(forward2, -vector2)) / 2f);
		midpoint = position + 0.5f * vector;
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00055D88 File Offset: 0x00053F88
	private void MoveTransform(Transform target, float score, Vector3 midpoint)
	{
		Vector3 vector;
		Quaternion quaternion;
		target.GetPositionAndRotation(ref vector, ref quaternion);
		Vector3 vector2 = Vector3.Lerp(vector, midpoint, ProximityEffect.<MoveTransform>g__ExpT|40_0(this.positionCTLerpSpeed));
		if (this.rotateCT)
		{
			Vector3 vector3 = (vector2 - vector) / Time.deltaTime;
			if (vector3 != Vector3.zero)
			{
				Quaternion quaternion2 = Quaternion.LookRotation(vector3);
				Quaternion quaternion3 = Quaternion.LookRotation(vector2 - this.rig.syncPos);
				Quaternion quaternion4 = Quaternion.Slerp(quaternion, Quaternion.Slerp(quaternion3, quaternion2, vector3.magnitude), ProximityEffect.<MoveTransform>g__ExpT|40_0(this.rotationCTLerpSpeed));
				target.SetPositionAndRotation(vector2, quaternion4);
			}
		}
		else
		{
			target.position = vector2;
		}
		if (this.scaleCT)
		{
			target.localScale = Vector3.Lerp(target.localScale, score * this.scaleCTMult * Vector3.one, ProximityEffect.<MoveTransform>g__ExpT|40_0(this.scaleCTLerpSpeed));
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06001067 RID: 4199 RVA: 0x00055E64 File Offset: 0x00054064
	// (set) Token: 0x06001068 RID: 4200 RVA: 0x00055E6C File Offset: 0x0005406C
	public bool TickRunning { get; set; }

	// Token: 0x06001069 RID: 4201 RVA: 0x00055E78 File Offset: 0x00054078
	public void Tick()
	{
		float num;
		float num2;
		float num3;
		Vector3 midpoint;
		this.CalculateProximityScores(out num, out num2, out num3, out midpoint);
		if (this.receivers != null)
		{
			for (int i = 0; i < this.receivers.Count; i++)
			{
				this.receivers[i].OnProximityCalculated(num, num2, num3);
			}
		}
		float num4 = num * num2 * num3;
		ContinuousPropertyArray continuousPropertyArray = this.continuousProperties;
		if (continuousPropertyArray != null)
		{
			continuousPropertyArray.ApplyAll(num4);
		}
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num4);
		}
		if (this.centerTransform != null)
		{
			this.MoveTransform(this.centerTransform, num4, midpoint);
		}
		this.anyAboveThreshold = false;
		foreach (ProximityEffect.ProximityEvent proximityEvent in this.events)
		{
			this.anyAboveThreshold = (proximityEvent.Evaluate(num4) || this.anyAboveThreshold);
		}
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00055FDD File Offset: 0x000541DD
	[CompilerGenerated]
	internal static float <MoveTransform>g__ExpT|40_0(float speed)
	{
		return 1f - Mathf.Exp(-speed * Time.deltaTime);
	}

	// Token: 0x0400144C RID: 5196
	[SerializeField]
	private Transform leftTransform;

	// Token: 0x0400144D RID: 5197
	[SerializeField]
	private Transform rightTransform;

	// Token: 0x0400144E RID: 5198
	[SerializeField]
	[Tooltip("How many times AddTrigger() needs to be called before the events are allowed to be invoked. Used for pausing events until certain actions are performed (like squeezing the triggers of both controllers).")]
	private int triggersToActivate;

	// Token: 0x0400144F RID: 5199
	[Space]
	[SerializeField]
	[Tooltip("The transform that moves to follow the midpoint of the left and right transforms.")]
	private Transform centerTransform;

	// Token: 0x04001450 RID: 5200
	private const string SHOW_CONDITION = "@centerTransform != null";

	// Token: 0x04001451 RID: 5201
	[SerializeField]
	private float positionCTLerpSpeed = 10f;

	// Token: 0x04001452 RID: 5202
	[SerializeField]
	private bool rotateCT;

	// Token: 0x04001453 RID: 5203
	private const string SHOW_ROTATE_CONDITION = "@centerTransform != null && rotateCT";

	// Token: 0x04001454 RID: 5204
	[SerializeField]
	private float rotationCTLerpSpeed = 10f;

	// Token: 0x04001455 RID: 5205
	[SerializeField]
	private bool scaleCT;

	// Token: 0x04001456 RID: 5206
	private const string SHOW_SCALE_CONDITION = "@centerTransform != null && scaleCT";

	// Token: 0x04001457 RID: 5207
	[SerializeField]
	private float scaleCTLerpSpeed = 10f;

	// Token: 0x04001458 RID: 5208
	[SerializeField]
	private float scaleCTMult = 1f;

	// Token: 0x04001459 RID: 5209
	[Space]
	[SerializeField]
	[Tooltip("The curves that get evaluated to determine the alignment score. They get multiplied together, so their Y values should all range from 0-1. The result is compared against the thresholds of the ProximityEvents.")]
	private ProximityEffectScoreCurvesSO scoreCurves;

	// Token: 0x0400145A RID: 5210
	[Space]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x0400145B RID: 5211
	[SerializeField]
	private UnityEvent<float> onScoreCalculated;

	// Token: 0x0400145C RID: 5212
	[SerializeField]
	private ProximityEffect.ProximityEvent[] events;

	// Token: 0x0400145D RID: 5213
	[Header("Editor Only")]
	[SerializeField]
	private Vector3 defaultLeftHandLocalPosition = new Vector3(-0.0568f, 0.04311f, 0.00249f);

	// Token: 0x0400145E RID: 5214
	[SerializeField]
	private Vector3 defaultLeftHandLocalEuler = new Vector3(173.176f, 80.201f, 3.615f);

	// Token: 0x0400145F RID: 5215
	[Header("Visualization is currently NOT WORKING IN PLAY MODE due to tick optimization")]
	[SerializeField]
	private bool enableVisualization = true;

	// Token: 0x04001460 RID: 5216
	[SerializeField]
	private Material visualizationMaterial;

	// Token: 0x04001461 RID: 5217
	[SerializeField]
	[Range(0f, 1f)]
	private float visualizationLineThickness = 0.01f;

	// Token: 0x04001462 RID: 5218
	[SerializeField]
	[HideInInspector]
	private LineRenderer visualizer;

	// Token: 0x04001463 RID: 5219
	private List<IProximityEffectReceiver> receivers;

	// Token: 0x04001464 RID: 5220
	private VRRig rig;

	// Token: 0x04001465 RID: 5221
	private bool anyAboveThreshold;

	// Token: 0x04001466 RID: 5222
	private int numTriggers;

	// Token: 0x0200027F RID: 639
	[Serializable]
	private class ProximityEvent
	{
		// Token: 0x0600106C RID: 4204 RVA: 0x00055FF4 File Offset: 0x000541F4
		public bool Evaluate(float score)
		{
			if (score >= this.highThreshold)
			{
				if (!this.wasAboveThreshold && Time.time - this.lastThresholdTime >= this.highThresholdBufferTime)
				{
					UnityEvent unityEvent = this.onThresholdHigh;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.wasAboveThreshold = true;
					this.wasBelowThreshold = false;
				}
				if (this.wasAboveThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
				return true;
			}
			if (score < this.lowThreshold)
			{
				if (!this.wasBelowThreshold && Time.time - this.lastThresholdTime >= this.lowThresholdBufferTime)
				{
					UnityEvent unityEvent2 = this.onThresholdLow;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasAboveThreshold = false;
					this.wasBelowThreshold = true;
				}
				if (this.wasBelowThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
			}
			return false;
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x000560B2 File Offset: 0x000542B2
		public void ResetAllEvents()
		{
			this.wasAboveThreshold = false;
			this.wasBelowThreshold = true;
		}

		// Token: 0x04001468 RID: 5224
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("High-threshold events will only fire if the alignment score is above this value.")]
		private float highThreshold = 0.5f;

		// Token: 0x04001469 RID: 5225
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the high-threshold events.")]
		private float highThresholdBufferTime;

		// Token: 0x0400146A RID: 5226
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Low-threshold events will only fire if the alignment score is below this value.")]
		private float lowThreshold = 0.3f;

		// Token: 0x0400146B RID: 5227
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the low-threshold events.")]
		private float lowThresholdBufferTime;

		// Token: 0x0400146C RID: 5228
		public UnityEvent onThresholdHigh;

		// Token: 0x0400146D RID: 5229
		public UnityEvent onThresholdLow;

		// Token: 0x0400146E RID: 5230
		private bool wasAboveThreshold;

		// Token: 0x0400146F RID: 5231
		private bool wasBelowThreshold = true;

		// Token: 0x04001470 RID: 5232
		private float lastThresholdTime = -100f;
	}
}
