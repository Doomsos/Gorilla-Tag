using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010A5 RID: 4261
	public class AOESender : MonoBehaviour
	{
		// Token: 0x06006A99 RID: 27289 RVA: 0x0022F3A7 File Offset: 0x0022D5A7
		private void Awake()
		{
			if (this.hits == null || this.hits.Length != this.maxColliders)
			{
				this.hits = new Collider[Mathf.Max(8, this.maxColliders)];
			}
		}

		// Token: 0x06006A9A RID: 27290 RVA: 0x0022F3D8 File Offset: 0x0022D5D8
		private void OnEnable()
		{
			if (this.applyOnEnable)
			{
				this.ApplyAOE();
			}
			this.nextTime = Time.time + this.repeatInterval;
		}

		// Token: 0x06006A9B RID: 27291 RVA: 0x0022F3FA File Offset: 0x0022D5FA
		private void Update()
		{
			if (this.repeatInterval > 0f && Time.time >= this.nextTime)
			{
				this.ApplyAOE();
				this.nextTime = Time.time + this.repeatInterval;
			}
		}

		// Token: 0x06006A9C RID: 27292 RVA: 0x0022F42E File Offset: 0x0022D62E
		public void ApplyAOE()
		{
			this.ApplyAOE(base.transform.position);
		}

		// Token: 0x06006A9D RID: 27293 RVA: 0x0022F444 File Offset: 0x0022D644
		public void ApplyAOE(Vector3 worldOrigin)
		{
			this.visited.Clear();
			int num = Physics.OverlapSphereNonAlloc(worldOrigin, this.radius, this.hits, this.layerMask, this.triggerInteraction);
			float num2 = Mathf.Max(0.0001f, this.radius);
			for (int i = 0; i < num; i++)
			{
				Collider collider = this.hits[i];
				if (collider)
				{
					AOEReceiver componentInChildren = (collider.attachedRigidbody ? collider.attachedRigidbody.transform : collider.transform).GetComponentInChildren<AOEReceiver>(true);
					if (componentInChildren != null && this.TagValidation(componentInChildren.gameObject) && !this.visited.Contains(componentInChildren))
					{
						this.visited.Add(componentInChildren);
						float num3 = Vector3.Distance(worldOrigin, componentInChildren.transform.position);
						float num4 = Mathf.Clamp01(num3 / num2);
						float num5 = this.EvaluateFalloff(num4);
						float finalStrength = Mathf.Max(this.minStrength, this.strength * num5);
						AOEReceiver.AOEContext aoecontext = new AOEReceiver.AOEContext
						{
							origin = worldOrigin,
							radius = this.radius,
							instigator = base.gameObject,
							baseStrength = this.strength,
							finalStrength = finalStrength,
							distance = num3,
							normalizedDistance = num4
						};
						componentInChildren.ReceiveAOE(aoecontext);
					}
				}
			}
		}

		// Token: 0x06006A9E RID: 27294 RVA: 0x0022F5BC File Offset: 0x0022D7BC
		private float EvaluateFalloff(float t)
		{
			switch (this.falloffMode)
			{
			case AOESender.FalloffMode.None:
				return 1f;
			case AOESender.FalloffMode.Linear:
				return 1f - t;
			case AOESender.FalloffMode.AnimationCurve:
				return Mathf.Max(0f, this.falloffCurve.Evaluate(t));
			default:
				return 1f;
			}
		}

		// Token: 0x06006A9F RID: 27295 RVA: 0x0022F610 File Offset: 0x0022D810
		private bool TagValidation(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.includeTags == null || this.includeTags.Length == 0)
			{
				return true;
			}
			string tag = go.tag;
			foreach (string text in this.includeTags)
			{
				if (!string.IsNullOrEmpty(text) && tag == text)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04007ABC RID: 31420
		[Min(0f)]
		[SerializeField]
		private float radius = 3f;

		// Token: 0x04007ABD RID: 31421
		[SerializeField]
		private LayerMask layerMask = -1;

		// Token: 0x04007ABE RID: 31422
		[SerializeField]
		private QueryTriggerInteraction triggerInteraction = 2;

		// Token: 0x04007ABF RID: 31423
		[Tooltip("If empty, all AOEReceiver targets pass. If not empty, only receivers with these tags pass.")]
		[SerializeField]
		private string[] includeTags;

		// Token: 0x04007AC0 RID: 31424
		[SerializeField]
		private AOESender.FalloffMode falloffMode = AOESender.FalloffMode.Linear;

		// Token: 0x04007AC1 RID: 31425
		[SerializeField]
		private AnimationCurve falloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04007AC2 RID: 31426
		[Tooltip("Base strength before distance falloff.")]
		[SerializeField]
		private float strength = 1f;

		// Token: 0x04007AC3 RID: 31427
		[Tooltip("Optional after falloff, applied as: max(minStrength, base*falloff).")]
		[SerializeField]
		private float minStrength;

		// Token: 0x04007AC4 RID: 31428
		[SerializeField]
		private bool applyOnEnable;

		// Token: 0x04007AC5 RID: 31429
		[Min(0f)]
		[SerializeField]
		private float repeatInterval;

		// Token: 0x04007AC6 RID: 31430
		[SerializeField]
		[Tooltip("Max colliders captured per trigger/apply.")]
		private int maxColliders = 16;

		// Token: 0x04007AC7 RID: 31431
		private Collider[] hits;

		// Token: 0x04007AC8 RID: 31432
		private readonly HashSet<AOEReceiver> visited = new HashSet<AOEReceiver>();

		// Token: 0x04007AC9 RID: 31433
		private float nextTime;

		// Token: 0x020010A6 RID: 4262
		private enum FalloffMode
		{
			// Token: 0x04007ACB RID: 31435
			None,
			// Token: 0x04007ACC RID: 31436
			Linear,
			// Token: 0x04007ACD RID: 31437
			AnimationCurve
		}
	}
}
