using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class ClackerCosmetic : MonoBehaviour
{
	// Token: 0x06000F17 RID: 3863 RVA: 0x0004FFEC File Offset: 0x0004E1EC
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.arm1.parent = this;
		this.arm2.parent = this;
		this.arm1.transform = this.clackerArm1;
		this.arm2.transform = this.clackerArm2;
		this.arm1.lastWorldPosition = this.clackerArm1.transform.TransformPoint(this.LocalCenterOfMass);
		this.arm2.lastWorldPosition = this.clackerArm2.transform.TransformPoint(this.LocalCenterOfMass);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x000500A8 File Offset: 0x0004E2A8
	private void Update()
	{
		Vector3 lastWorldPosition = this.arm1.lastWorldPosition;
		this.arm1.UpdateArm();
		this.arm2.UpdateArm();
		Vector3 eulerAngles = this.clackerArm1.transform.eulerAngles;
		Vector3 eulerAngles2 = this.clackerArm2.transform.eulerAngles;
		Mathf.DeltaAngle(eulerAngles.y, eulerAngles2.y);
		if ((this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).IsShorterThan(this.collisionDistance))
		{
			float sqrMagnitude = (this.arm1.velocity - this.arm2.velocity).sqrMagnitude;
			if (this.parentHoldable.InHand())
			{
				if (sqrMagnitude > this.heavyClackSpeed * this.heavyClackSpeed)
				{
					this.heavyClackAudio.Play();
				}
				else if (sqrMagnitude > this.mediumClackSpeed * this.mediumClackSpeed)
				{
					this.mediumClackAudio.Play();
				}
				else if (sqrMagnitude > this.minimumClackSpeed * this.minimumClackSpeed)
				{
					this.lightClackAudio.Play();
				}
			}
			Vector3 vector = (this.arm1.lastWorldPosition + this.arm2.lastWorldPosition) / 2f;
			Vector3 vector2 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * (this.collisionDistance + 0.001f) / 2f;
			Vector3 vector3 = vector + vector2;
			Vector3 vector4 = vector - vector2;
			if ((lastWorldPosition - vector3).IsLongerThan(lastWorldPosition - vector4))
			{
				vector2 = -vector2;
			}
			this.arm1.SetPosition(vector + vector2);
			this.arm2.SetPosition(vector - vector2);
			ref Vector3 ptr = ref this.arm1.velocity;
			Vector3 velocity = this.arm2.velocity;
			Vector3 velocity2 = this.arm1.velocity;
			ptr = velocity;
			this.arm2.velocity = velocity2;
			Vector3 vector5 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * this.pushApartStrength * Mathf.Sqrt(sqrMagnitude);
			this.arm1.velocity = this.arm1.velocity + vector5;
			this.arm2.velocity = this.arm2.velocity - vector5;
		}
	}

	// Token: 0x0400126B RID: 4715
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x0400126C RID: 4716
	[SerializeField]
	private Transform clackerArm1;

	// Token: 0x0400126D RID: 4717
	[SerializeField]
	private Transform clackerArm2;

	// Token: 0x0400126E RID: 4718
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x0400126F RID: 4719
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04001270 RID: 4720
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04001271 RID: 4721
	[SerializeField]
	private float drag;

	// Token: 0x04001272 RID: 4722
	[SerializeField]
	private float gravity;

	// Token: 0x04001273 RID: 4723
	[SerializeField]
	private float localFriction;

	// Token: 0x04001274 RID: 4724
	[SerializeField]
	private float minimumClackSpeed;

	// Token: 0x04001275 RID: 4725
	[SerializeField]
	private SoundBankPlayer lightClackAudio;

	// Token: 0x04001276 RID: 4726
	[SerializeField]
	private float mediumClackSpeed;

	// Token: 0x04001277 RID: 4727
	[SerializeField]
	private SoundBankPlayer mediumClackAudio;

	// Token: 0x04001278 RID: 4728
	[SerializeField]
	private float heavyClackSpeed;

	// Token: 0x04001279 RID: 4729
	[SerializeField]
	private SoundBankPlayer heavyClackAudio;

	// Token: 0x0400127A RID: 4730
	[SerializeField]
	private float collisionDistance;

	// Token: 0x0400127B RID: 4731
	private float centerOfMassRadius;

	// Token: 0x0400127C RID: 4732
	[SerializeField]
	private float pushApartStrength;

	// Token: 0x0400127D RID: 4733
	private ClackerCosmetic.PerArmData arm1;

	// Token: 0x0400127E RID: 4734
	private ClackerCosmetic.PerArmData arm2;

	// Token: 0x0400127F RID: 4735
	private Quaternion RotationCorrection;

	// Token: 0x02000240 RID: 576
	private struct PerArmData
	{
		// Token: 0x06000F1A RID: 3866 RVA: 0x00050334 File Offset: 0x0004E534
		public void UpdateArm()
		{
			Vector3 vector = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
			Vector3 vector2 = this.lastWorldPosition + this.velocity * Time.deltaTime * this.parent.drag;
			Vector3 vector3 = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			Vector3 vector4 = this.transform.position + UnityVectorExtensions.ProjectOntoPlane(vector2 - this.transform.position, vector3).normalized * this.parent.centerOfMassRadius;
			vector4 = Vector3.MoveTowards(vector4, vector, this.parent.localFriction * Time.deltaTime);
			this.velocity = (vector4 - this.lastWorldPosition) / Time.deltaTime;
			this.velocity += Vector3.down * this.parent.gravity * Time.deltaTime;
			this.lastWorldPosition = vector4;
			this.transform.rotation = Quaternion.LookRotation(vector3, vector4 - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0005049C File Offset: 0x0004E69C
		public void SetPosition(Vector3 newPosition)
		{
			Vector3 vector = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			this.transform.rotation = Quaternion.LookRotation(vector, newPosition - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x04001280 RID: 4736
		public ClackerCosmetic parent;

		// Token: 0x04001281 RID: 4737
		public Transform transform;

		// Token: 0x04001282 RID: 4738
		public Vector3 velocity;

		// Token: 0x04001283 RID: 4739
		public Vector3 lastWorldPosition;
	}
}
