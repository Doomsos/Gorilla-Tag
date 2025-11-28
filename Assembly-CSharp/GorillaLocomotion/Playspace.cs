using System;
using UnityEngine;

namespace GorillaLocomotion
{
	// Token: 0x02000F86 RID: 3974
	public sealed class Playspace : MonoBehaviour
	{
		// Token: 0x060063CA RID: 25546 RVA: 0x00206EEA File Offset: 0x002050EA
		private void Awake()
		{
			this._sqrSphereRadius = this._sphereRadius * this._sphereRadius;
			this._sqrSnapToThreshold = this._snapToThreshold * this._snapToThreshold;
		}

		// Token: 0x060063CB RID: 25547 RVA: 0x00206F14 File Offset: 0x00205114
		private void Update()
		{
			Vector3 vector = this._localGorillaHead.transform.position - base.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			if (GTPlayer.Instance.enableHoverMode || GTPlayer.Instance.isClimbing || vector.sqrMagnitude > this._sqrSnapToThreshold)
			{
				base.transform.position = this._localGorillaHead.transform.position;
				return;
			}
			Vector3 normalized = vector.normalized;
			vector = this.GetChaseSpeed() * Time.deltaTime * normalized;
			base.transform.position = ((vector.sqrMagnitude > sqrMagnitude) ? this._localGorillaHead.transform.position : (base.transform.position + vector));
			if ((this._localGorillaHead.transform.position - base.transform.position).sqrMagnitude > this._sqrSphereRadius)
			{
				this._localGorillaHead.transform.position = base.transform.position + this._sphereRadius * normalized;
			}
		}

		// Token: 0x060063CC RID: 25548 RVA: 0x0020703F File Offset: 0x0020523F
		private float GetChaseSpeed()
		{
			return this._defaultChaseSpeed;
		}

		// Token: 0x060063CD RID: 25549 RVA: 0x00207047 File Offset: 0x00205247
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(base.transform.position, this._sphereRadius);
		}

		// Token: 0x04007316 RID: 29462
		[SerializeField]
		private GameObject _localGorillaHead;

		// Token: 0x04007317 RID: 29463
		[SerializeField]
		private float _sphereRadius;

		// Token: 0x04007318 RID: 29464
		private float _sqrSphereRadius;

		// Token: 0x04007319 RID: 29465
		[SerializeField]
		private float _defaultChaseSpeed;

		// Token: 0x0400731A RID: 29466
		[SerializeField]
		private float _snapToThreshold;

		// Token: 0x0400731B RID: 29467
		private float _sqrSnapToThreshold;
	}
}
