using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class PoseStiffnessComparison : MonoBehaviour
{
	// Token: 0x0600004E RID: 78 RVA: 0x00002E38 File Offset: 0x00001038
	private void Start()
	{
		this.m_timer = 0f;
		this.m_yA = this.BonesA.position.y;
		this.m_yB = this.BonesB.position.y;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00002E74 File Offset: 0x00001074
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		float[] array2 = new float[]
		{
			this.m_yA,
			this.m_yB
		};
		IEnumerable<BoingBones> enumerable = Enumerable.Concat<BoingBones>(components, components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			for (int i = 0; i < 2; i++)
			{
				Transform transform = Enumerable.ElementAt<Transform>(array, i);
				float y = Enumerable.ElementAt<float>(array2, i);
				Vector3 position = transform.position;
				position.y = y;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 1.5f);
		for (int j = 0; j < 2; j++)
		{
			Transform transform2 = Enumerable.ElementAt<Transform>(array, j);
			float num4 = Enumerable.ElementAt<float>(array2, j);
			Vector3 position2 = transform2.position;
			position2.y = num4 + 2f * Mathf.Sin(12.566371f * num3);
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
		}
	}

	// Token: 0x04000036 RID: 54
	public float Run = 11f;

	// Token: 0x04000037 RID: 55
	public float Tilt = 15f;

	// Token: 0x04000038 RID: 56
	public float Period = 3f;

	// Token: 0x04000039 RID: 57
	public float Rest = 3f;

	// Token: 0x0400003A RID: 58
	public Transform BonesA;

	// Token: 0x0400003B RID: 59
	public Transform BonesB;

	// Token: 0x0400003C RID: 60
	private float m_yA;

	// Token: 0x0400003D RID: 61
	private float m_yB;

	// Token: 0x0400003E RID: 62
	private float m_timer;
}
