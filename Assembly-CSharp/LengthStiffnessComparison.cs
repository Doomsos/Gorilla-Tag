using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class LengthStiffnessComparison : MonoBehaviour
{
	// Token: 0x0600004B RID: 75 RVA: 0x00002C4B File Offset: 0x00000E4B
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00002C58 File Offset: 0x00000E58
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		IEnumerable<BoingBones> enumerable = Enumerable.Concat<BoingBones>(components, components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			foreach (Transform transform in array)
			{
				Vector3 position = transform.position;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 6f);
		foreach (Transform transform2 in array)
		{
			Vector3 position2 = transform2.position;
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
			transform2.rotation = Quaternion.AngleAxis(this.Tilt * (1f - num3), Vector3.right);
		}
	}

	// Token: 0x0400002F RID: 47
	public float Run = 11f;

	// Token: 0x04000030 RID: 48
	public float Tilt = 15f;

	// Token: 0x04000031 RID: 49
	public float Period = 3f;

	// Token: 0x04000032 RID: 50
	public float Rest = 3f;

	// Token: 0x04000033 RID: 51
	public Transform BonesA;

	// Token: 0x04000034 RID: 52
	public Transform BonesB;

	// Token: 0x04000035 RID: 53
	private float m_timer;
}
