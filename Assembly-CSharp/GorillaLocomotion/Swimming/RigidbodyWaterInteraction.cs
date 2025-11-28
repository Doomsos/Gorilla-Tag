using System;
using System.Collections.Generic;
using AA;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F89 RID: 3977
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyWaterInteraction : MonoBehaviour
	{
		// Token: 0x060063D1 RID: 25553 RVA: 0x002072B2 File Offset: 0x002054B2
		protected void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.baseAngularDrag = this.rb.angularDamping;
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x060063D2 RID: 25554 RVA: 0x002072D7 File Offset: 0x002054D7
		protected void OnEnable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x060063D3 RID: 25555 RVA: 0x002072EA File Offset: 0x002054EA
		protected void OnDisable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x060063D4 RID: 25556 RVA: 0x002072FD File Offset: 0x002054FD
		private void OnDestroy()
		{
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x060063D5 RID: 25557 RVA: 0x00207308 File Offset: 0x00205508
		public void InvokeFixedUpdate()
		{
			if (this.rb.isKinematic)
			{
				return;
			}
			bool flag = this.overlappingWaterVolumes.Count > 0;
			WaterVolume.SurfaceQuery surfaceQuery = default(WaterVolume.SurfaceQuery);
			float num = float.MinValue;
			if (flag && this.enablePreciseWaterCollision)
			{
				Vector3 vector = base.transform.position + Vector3.down * 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
				bool flag2 = false;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery2;
					if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery2, false))
					{
						float num2 = Vector3.Dot(surfaceQuery2.surfacePoint - vector, surfaceQuery2.surfaceNormal);
						if (num2 > num)
						{
							num = num2;
							surfaceQuery = surfaceQuery2;
							flag2 = true;
						}
						WaterCurrent waterCurrent = this.overlappingWaterVolumes[i].Current;
						if (this.applyWaterCurrents && waterCurrent != null && num2 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (flag2)
				{
					bool flag3 = num > -(1f - this.buoyancyEquilibrium) * 2f * this.objectRadiusForWaterCollision;
					float num3 = this.enablePreciseWaterCollision ? this.objectRadiusForWaterCollision : 0f;
					bool flag4 = base.transform.position.y + num3 - (surfaceQuery.surfacePoint.y - surfaceQuery.maxDepth) > 0f;
					flag = (flag3 && flag4);
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				float fixedDeltaTime = Time.fixedDeltaTime;
				Vector3 vector2 = this.rb.linearVelocity;
				Vector3 vector3 = Vector3.zero;
				if (this.applyWaterCurrents)
				{
					Vector3 vector4 = Vector3.zero;
					for (int j = 0; j < this.activeWaterCurrents.Count; j++)
					{
						WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
						Vector3 startingVelocity = vector2 + vector3;
						Vector3 vector5;
						Vector3 vector6;
						if (waterCurrent2.GetCurrentAtPoint(base.transform.position, startingVelocity, fixedDeltaTime, out vector5, out vector6))
						{
							vector4 += vector5;
							vector3 += vector6;
						}
					}
					if (this.enablePreciseWaterCollision)
					{
						Vector3 vector7 = (surfaceQuery.surfacePoint + (base.transform.position + Vector3.down * this.objectRadiusForWaterCollision)) * 0.5f;
						this.rb.AddForceAtPosition(vector3 * this.rb.mass, vector7, 1);
					}
					else
					{
						vector2 += vector3;
					}
				}
				if (this.applyBuoyancyForce)
				{
					Vector3 vector8 = Vector3.zero;
					if (this.enablePreciseWaterCollision)
					{
						float num4 = 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
						float num5 = Mathf.InverseLerp(0f, num4, num);
						vector8 = -Physics.gravity * this.underWaterBuoyancyFactor * num5 * fixedDeltaTime;
					}
					else
					{
						vector8 = -Physics.gravity * this.underWaterBuoyancyFactor * fixedDeltaTime;
					}
					if (vector3.sqrMagnitude > 0.001f)
					{
						float magnitude = vector3.magnitude;
						Vector3 vector9 = vector3 / magnitude;
						float num6 = Vector3.Dot(vector8, vector9);
						if (num6 < 0f)
						{
							vector8 -= num6 * vector9;
						}
					}
					vector2 += vector8;
				}
				float magnitude2 = vector2.magnitude;
				if (magnitude2 > 0.001f && this.applyDamping)
				{
					Vector3 vector10 = vector2 / magnitude2;
					float num7 = Spring.DamperDecayExact(magnitude2, this.underWaterDampingHalfLife, fixedDeltaTime, 1E-05f);
					if (this.enablePreciseWaterCollision)
					{
						float num8 = Spring.DamperDecayExact(magnitude2, this.waterSurfaceDampingHalfLife, fixedDeltaTime, 1E-05f);
						float num9 = Mathf.Clamp(-(base.transform.position.y - surfaceQuery.surfacePoint.y) / this.objectRadiusForWaterCollision, -1f, 1f) * 0.5f + 0.5f;
						vector2 = Mathf.Lerp(num8, num7, num9) * vector10;
					}
					else
					{
						vector2 = num7 * vector10;
					}
				}
				if (this.applySurfaceTorque && this.enablePreciseWaterCollision)
				{
					float num10 = base.transform.position.y - surfaceQuery.surfacePoint.y;
					if (num10 < this.objectRadiusForWaterCollision && num10 > 0f)
					{
						Vector3 vector11 = vector2 - Vector3.Dot(vector2, surfaceQuery.surfaceNormal) * surfaceQuery.surfaceNormal;
						Vector3 normalized = Vector3.Cross(surfaceQuery.surfaceNormal, vector11).normalized;
						float num11 = Vector3.Dot(this.rb.angularVelocity, normalized);
						float num12 = vector11.magnitude / this.objectRadiusForWaterCollision - num11;
						if (num12 > 0f)
						{
							this.rb.AddTorque(this.surfaceTorqueAmount * num12 * normalized, 5);
						}
					}
				}
				this.rb.linearVelocity = vector2;
				this.rb.angularDamping = this.angularDrag;
				return;
			}
			this.rb.angularDamping = this.baseAngularDrag;
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x00207848 File Offset: 0x00205A48
		protected void OnTriggerEnter(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && !this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Add(component);
			}
		}

		// Token: 0x060063D7 RID: 25559 RVA: 0x00207880 File Offset: 0x00205A80
		protected void OnTriggerExit(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Remove(component);
			}
		}

		// Token: 0x04007345 RID: 29509
		public bool applyDamping = true;

		// Token: 0x04007346 RID: 29510
		public bool applyBuoyancyForce = true;

		// Token: 0x04007347 RID: 29511
		public bool applyAngularDrag = true;

		// Token: 0x04007348 RID: 29512
		public bool applyWaterCurrents = true;

		// Token: 0x04007349 RID: 29513
		public bool applySurfaceTorque = true;

		// Token: 0x0400734A RID: 29514
		public float underWaterDampingHalfLife = 0.25f;

		// Token: 0x0400734B RID: 29515
		public float waterSurfaceDampingHalfLife = 1f;

		// Token: 0x0400734C RID: 29516
		public float underWaterBuoyancyFactor = 0.5f;

		// Token: 0x0400734D RID: 29517
		public float angularDrag = 0.5f;

		// Token: 0x0400734E RID: 29518
		public float surfaceTorqueAmount = 0.5f;

		// Token: 0x0400734F RID: 29519
		public bool enablePreciseWaterCollision;

		// Token: 0x04007350 RID: 29520
		public float objectRadiusForWaterCollision = 0.25f;

		// Token: 0x04007351 RID: 29521
		[Range(0f, 1f)]
		public float buoyancyEquilibrium = 0.8f;

		// Token: 0x04007352 RID: 29522
		private Rigidbody rb;

		// Token: 0x04007353 RID: 29523
		private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

		// Token: 0x04007354 RID: 29524
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04007355 RID: 29525
		private float baseAngularDrag = 0.05f;
	}
}
