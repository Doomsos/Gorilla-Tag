using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class GragerHoldable : MonoBehaviour
{
	// Token: 0x06000FDB RID: 4059 RVA: 0x0005381C File Offset: 0x00051A1C
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.lastWorldPosition = base.transform.TransformPoint(this.LocalCenterOfMass);
		this.lastClackParentLocalPosition = base.transform.parent.InverseTransformPoint(this.lastWorldPosition);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00053890 File Offset: 0x00051A90
	private void Update()
	{
		Vector3 vector = base.transform.TransformPoint(this.LocalCenterOfMass);
		Vector3 vector2 = this.lastWorldPosition + this.velocity * Time.deltaTime * this.drag;
		Vector3 vector3 = base.transform.parent.TransformDirection(this.LocalRotationAxis);
		Vector3 vector4 = base.transform.position + UnityVectorExtensions.ProjectOntoPlane(vector2 - base.transform.position, vector3).normalized * this.centerOfMassRadius;
		vector4 = Vector3.MoveTowards(vector4, vector, this.localFriction * Time.deltaTime);
		this.velocity = (vector4 - this.lastWorldPosition) / Time.deltaTime;
		this.velocity += Vector3.down * this.gravity * Time.deltaTime;
		this.lastWorldPosition = vector4;
		base.transform.rotation = Quaternion.LookRotation(vector4 - base.transform.position, vector3) * this.RotationCorrection;
		Vector3 vector5 = base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(this.LocalCenterOfMass));
		if ((vector5 - this.lastClackParentLocalPosition).IsLongerThan(this.distancePerClack))
		{
			this.clackAudio.GTPlayOneShot(this.allClacks[Random.Range(0, this.allClacks.Length)], 1f);
			this.lastClackParentLocalPosition = vector5;
		}
	}

	// Token: 0x040013A4 RID: 5028
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x040013A5 RID: 5029
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x040013A6 RID: 5030
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x040013A7 RID: 5031
	[SerializeField]
	private float drag;

	// Token: 0x040013A8 RID: 5032
	[SerializeField]
	private float gravity;

	// Token: 0x040013A9 RID: 5033
	[SerializeField]
	private float localFriction;

	// Token: 0x040013AA RID: 5034
	[SerializeField]
	private float distancePerClack;

	// Token: 0x040013AB RID: 5035
	[SerializeField]
	private AudioSource clackAudio;

	// Token: 0x040013AC RID: 5036
	[SerializeField]
	private AudioClip[] allClacks;

	// Token: 0x040013AD RID: 5037
	private float centerOfMassRadius;

	// Token: 0x040013AE RID: 5038
	private Vector3 velocity;

	// Token: 0x040013AF RID: 5039
	private Vector3 lastWorldPosition;

	// Token: 0x040013B0 RID: 5040
	private Vector3 lastClackParentLocalPosition;

	// Token: 0x040013B1 RID: 5041
	private Quaternion RotationCorrection;
}
