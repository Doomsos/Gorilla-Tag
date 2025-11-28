using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E6B RID: 3691
	public class RecyclerForceVolume : MonoBehaviour
	{
		// Token: 0x06005C4C RID: 23628 RVA: 0x001DA5FA File Offset: 0x001D87FA
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.hasWindFX = (this.windEffectRenderer != null);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x06005C4D RID: 23629 RVA: 0x001DA630 File Offset: 0x001D8830
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

		// Token: 0x06005C4E RID: 23630 RVA: 0x001DA690 File Offset: 0x001D8890
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.enterPos = transform.position;
			ObjectPools.instance.Instantiate(this.windSFX, this.enterPos, true);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.transform.position = base.transform.position + Vector3.Dot(this.enterPos - base.transform.position, base.transform.right) * base.transform.right;
				this.windEffectRenderer.enabled = true;
			}
		}

		// Token: 0x06005C4F RID: 23631 RVA: 0x001DA740 File Offset: 0x001D8940
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x06005C50 RID: 23632 RVA: 0x001DA774 File Offset: 0x001D8974
		public void OnTriggerStay(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
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
			Vector3 vector2 = Vector3.Dot(base.transform.position - transform.position, base.transform.up) * base.transform.up;
			float num = vector2.magnitude + 0.0001f;
			Vector3 vector3 = vector2 / num;
			float num2 = Vector3.Dot(vector, vector3);
			float num3 = this.accel;
			if (this.maxDepth > -1f)
			{
				float num4 = Vector3.Dot(transform.position - this.enterPos, vector3);
				float num5 = this.maxDepth - num4;
				float num6 = 0f;
				if (num5 > 0.0001f)
				{
					num6 = num2 * num2 / num5;
				}
				num3 = Mathf.Max(this.accel, num6);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector4 = base.transform.forward * num3 * deltaTime;
			vector += vector4;
			Vector3 vector5 = Vector3.Dot(vector, base.transform.up) * base.transform.up;
			Vector3 vector6 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 vector7 = Mathf.Clamp(Vector3.Dot(vector, base.transform.forward), -1f * this.maxSpeed, this.maxSpeed) * base.transform.forward;
			float num7 = 1f;
			float num8 = 1f;
			if (this.dampenLateralVelocity)
			{
				num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				num8 = 1f - this.dampenYVelPerc * 0.01f * deltaTime;
			}
			vector = num8 * vector5 + num7 * vector6 + vector7;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector3;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Clamp(num2, -1f * num9, num9);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector3;
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.linearVelocity = vector;
		}

		// Token: 0x040069D5 RID: 27093
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x040069D6 RID: 27094
		[SerializeField]
		private float accel;

		// Token: 0x040069D7 RID: 27095
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x040069D8 RID: 27096
		[SerializeField]
		private float maxSpeed;

		// Token: 0x040069D9 RID: 27097
		[SerializeField]
		private bool disableGrip;

		// Token: 0x040069DA RID: 27098
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x040069DB RID: 27099
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x040069DC RID: 27100
		[FormerlySerializedAs("dampenZVelPerc")]
		[SerializeField]
		private float dampenYVelPerc;

		// Token: 0x040069DD RID: 27101
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x040069DE RID: 27102
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x040069DF RID: 27103
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x040069E0 RID: 27104
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x040069E1 RID: 27105
		private Collider volume;

		// Token: 0x040069E2 RID: 27106
		public GameObject windSFX;

		// Token: 0x040069E3 RID: 27107
		[SerializeField]
		private MeshRenderer windEffectRenderer;

		// Token: 0x040069E4 RID: 27108
		private bool hasWindFX;

		// Token: 0x040069E5 RID: 27109
		private Vector3 enterPos;
	}
}
