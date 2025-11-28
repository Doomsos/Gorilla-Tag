using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class SquashAndStretchComparison : MonoBehaviour
{
	// Token: 0x06000051 RID: 81 RVA: 0x00003088 File Offset: 0x00001288
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00003098 File Offset: 0x00001298
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
		}
	}

	// Token: 0x0400003F RID: 63
	public float Run = 11f;

	// Token: 0x04000040 RID: 64
	public float Period = 3f;

	// Token: 0x04000041 RID: 65
	public float Rest = 3f;

	// Token: 0x04000042 RID: 66
	public Transform BonesA;

	// Token: 0x04000043 RID: 67
	public Transform BonesB;

	// Token: 0x04000044 RID: 68
	private float m_timer;
}
