using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E67 RID: 3687
	public class BuilderSpeedBooster : MonoBehaviour
	{
		// Token: 0x06005C34 RID: 23604 RVA: 0x001D99F4 File Offset: 0x001D7BF4
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.windRenderer.enabled = false;
			this.boosting = false;
		}

		// Token: 0x06005C35 RID: 23605 RVA: 0x001D9A18 File Offset: 0x001D7C18
		private void LateUpdate()
		{
			if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
			{
				this.audioSource.enabled = false;
			}
		}

		// Token: 0x06005C36 RID: 23606 RVA: 0x001D9A68 File Offset: 0x001D7C68
		private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
		{
			rb = null;
			xf = null;
			if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
			{
				rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
				xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
			}
			return rb != null && xf != null;
		}

		// Token: 0x06005C37 RID: 23607 RVA: 0x001D9AC8 File Offset: 0x001D7CC8
		private void CheckTableZone()
		{
			if (this.hasCheckedZone)
			{
				return;
			}
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
			{
				this.ignoreMonkeScale = !builderTable.isTableMutable;
			}
			this.hasCheckedZone = true;
		}

		// Token: 0x06005C38 RID: 23608 RVA: 0x001D9B14 File Offset: 0x001D7D14
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			this.positiveForce = (Vector3.Dot(base.transform.up, rigidbody.linearVelocity) > 0f);
			if (this.positiveForce)
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
			else
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
			}
			this.windRenderer.enabled = true;
			this.enterPos = transform.position;
			if (!this.boosting)
			{
				this.boosting = true;
				this.enterTime = Time.timeAsDouble;
			}
		}

		// Token: 0x06005C39 RID: 23609 RVA: 0x001D9C04 File Offset: 0x001D7E04
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.windRenderer.enabled = false;
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.boosting && this.audioSource)
			{
				this.audioSource.enabled = true;
				this.audioSource.Stop();
				this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			}
			this.boosting = false;
		}

		// Token: 0x06005C3A RID: 23610 RVA: 0x001D9CA4 File Offset: 0x001D7EA4
		public void OnTriggerStay(Collider other)
		{
			if (!this.boosting)
			{
				return;
			}
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (Time.timeAsDouble > this.enterTime + (double)this.maxBoostDuration)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (this.disableGrip)
			{
				GTPlayer.Instance.SetMaximumSlipThisFrame();
			}
			SizeManager sizeManager = null;
			if (this.scaleWithSize)
			{
				sizeManager = rigidbody.GetComponent<SizeManager>();
			}
			Vector3 vector = rigidbody.linearVelocity;
			if (this.scaleWithSize && sizeManager)
			{
				vector /= sizeManager.currentScale;
			}
			Vector3 vector2 = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
			Vector3 vector3 = base.transform.position + vector2 - transform.position;
			float num = vector3.magnitude + 0.0001f;
			Vector3 vector4 = vector3 / num;
			float num2 = Vector3.Dot(vector, vector4);
			float num3 = this.accel;
			if (this.maxDepth > -1f)
			{
				float num4 = Vector3.Dot(transform.position - this.enterPos, vector4);
				float num5 = this.maxDepth - num4;
				float num6 = 0f;
				if (num5 > 0.0001f)
				{
					num6 = num2 * num2 / num5;
				}
				num3 = Mathf.Max(this.accel, num6);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector5 = base.transform.up * num3 * deltaTime;
			if (!this.positiveForce)
			{
				vector5 *= -1f;
			}
			vector += vector5;
			if ((double)Vector3.Dot(vector5, Vector3.down) <= 0.1)
			{
				vector += Vector3.up * this.addedWorldUpVelocity * deltaTime;
			}
			Vector3 vector6 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
			Vector3 vector7 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 vector8 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
			float num7 = 1f;
			float num8 = 1f;
			if (this.dampenLateralVelocity)
			{
				num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				num8 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
			}
			vector = vector6 + num7 * vector7 + num8 * vector8;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector4;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Min(num2, num9);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector4;
				if (vector.magnitude > 0.0001f)
				{
					Vector3 vector9 = Vector3.Cross(base.transform.up, vector4);
					float magnitude = vector9.magnitude;
					if (magnitude > 0.0001f)
					{
						vector9 /= magnitude;
						num2 = Vector3.Dot(vector, vector9);
						vector -= num2 * vector9;
						num2 -= this.pullToCenterAccel * deltaTime;
						num2 = Mathf.Max(0f, num2);
						vector += num2 * vector9;
					}
				}
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.linearVelocity = vector;
		}

		// Token: 0x06005C3B RID: 23611 RVA: 0x001DA0B8 File Offset: 0x001D82B8
		public void OnDrawGizmosSelected()
		{
			base.GetComponents<Collider>();
			Gizmos.color = Color.magenta;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
		}

		// Token: 0x040069A0 RID: 27040
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x040069A1 RID: 27041
		[SerializeField]
		private float accel;

		// Token: 0x040069A2 RID: 27042
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x040069A3 RID: 27043
		[SerializeField]
		private float maxSpeed;

		// Token: 0x040069A4 RID: 27044
		[SerializeField]
		private bool disableGrip;

		// Token: 0x040069A5 RID: 27045
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x040069A6 RID: 27046
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x040069A7 RID: 27047
		[SerializeField]
		private float dampenZVelPerc;

		// Token: 0x040069A8 RID: 27048
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x040069A9 RID: 27049
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x040069AA RID: 27050
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x040069AB RID: 27051
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x040069AC RID: 27052
		[SerializeField]
		private float addedWorldUpVelocity = 10f;

		// Token: 0x040069AD RID: 27053
		[SerializeField]
		private float maxBoostDuration = 2f;

		// Token: 0x040069AE RID: 27054
		private bool boosting;

		// Token: 0x040069AF RID: 27055
		private double enterTime;

		// Token: 0x040069B0 RID: 27056
		private Collider volume;

		// Token: 0x040069B1 RID: 27057
		public AudioClip exitClip;

		// Token: 0x040069B2 RID: 27058
		public AudioSource audioSource;

		// Token: 0x040069B3 RID: 27059
		public MeshRenderer windRenderer;

		// Token: 0x040069B4 RID: 27060
		private Vector3 enterPos;

		// Token: 0x040069B5 RID: 27061
		private bool positiveForce = true;

		// Token: 0x040069B6 RID: 27062
		private bool ignoreMonkeScale;

		// Token: 0x040069B7 RID: 27063
		private bool hasCheckedZone;
	}
}
