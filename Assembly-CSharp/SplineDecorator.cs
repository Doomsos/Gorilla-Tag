using System;
using UnityEngine;

// Token: 0x02000C7F RID: 3199
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x06004E28 RID: 20008 RVA: 0x001957C0 File Offset: 0x001939C0
	private void Awake()
	{
		if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
		{
			return;
		}
		float num = (float)(this.frequency * this.items.Length);
		if (this.spline.Loop || num == 1f)
		{
			num = 1f / num;
		}
		else
		{
			num = 1f / (num - 1f);
		}
		int num2 = 0;
		for (int i = 0; i < this.frequency; i++)
		{
			int j = 0;
			while (j < this.items.Length)
			{
				Transform transform = Object.Instantiate<Transform>(this.items[j]);
				Vector3 point = this.spline.GetPoint((float)num2 * num);
				transform.transform.localPosition = point;
				if (this.lookForward)
				{
					transform.transform.LookAt(point + this.spline.GetDirection((float)num2 * num));
				}
				transform.transform.parent = base.transform;
				j++;
				num2++;
			}
		}
	}

	// Token: 0x04005D3D RID: 23869
	public BezierSpline spline;

	// Token: 0x04005D3E RID: 23870
	public int frequency;

	// Token: 0x04005D3F RID: 23871
	public bool lookForward;

	// Token: 0x04005D40 RID: 23872
	public Transform[] items;
}
