using System;
using UnityEngine;

// Token: 0x02000490 RID: 1168
public class OwlLook : MonoBehaviour
{
	// Token: 0x06001DF2 RID: 7666 RVA: 0x0009D9B3 File Offset: 0x0009BBB3
	private void Awake()
	{
		this.overlapRigs = new VRRig[10];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x0009D9DC File Offset: 0x0009BBDC
	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			if (!(this.rigs[i] == this.myRig))
			{
				Vector3 vector = this.rigs[i].tagSound.transform.position - base.transform.position;
				if (vector.magnitude <= this.lookRadius)
				{
					float num4 = Vector3.Dot(-base.transform.up, vector.normalized);
					if (num4 > num2)
					{
						this.overlapRigs[num3++] = this.rigs[i];
					}
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 vector = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, vector);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 vector2 = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector2 = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector3 = this.neck.InverseTransformDirection(vector2);
		vector3.y = Mathf.Clamp(vector3.y, this.minNeckY, this.maxNeckY);
		vector2 = this.neck.TransformDirection(vector3.normalized);
		Vector3 vector4 = Vector3.RotateTowards(this.head.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(vector4, this.neck.up);
	}

	// Token: 0x04002804 RID: 10244
	public Transform head;

	// Token: 0x04002805 RID: 10245
	public Transform lookTarget;

	// Token: 0x04002806 RID: 10246
	public Transform neck;

	// Token: 0x04002807 RID: 10247
	public float lookRadius = 0.5f;

	// Token: 0x04002808 RID: 10248
	public Collider[] overlapColliders;

	// Token: 0x04002809 RID: 10249
	public VRRig[] rigs = new VRRig[10];

	// Token: 0x0400280A RID: 10250
	public VRRig[] overlapRigs;

	// Token: 0x0400280B RID: 10251
	public float rotSpeed = 1f;

	// Token: 0x0400280C RID: 10252
	public float lookAtAngleDegrees = 60f;

	// Token: 0x0400280D RID: 10253
	public float maxNeckY;

	// Token: 0x0400280E RID: 10254
	public float minNeckY;

	// Token: 0x0400280F RID: 10255
	public VRRig myRig;
}
