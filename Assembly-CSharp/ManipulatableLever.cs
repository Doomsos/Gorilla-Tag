using System;
using UnityEngine;

// Token: 0x02000484 RID: 1156
public class ManipulatableLever : ManipulatableObject
{
	// Token: 0x06001D8C RID: 7564 RVA: 0x0009B9BF File Offset: 0x00099BBF
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x0009B9D4 File Offset: 0x00099BD4
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = this.leverGrip.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x0009BA14 File Offset: 0x00099C14
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 position = hand.transform.position;
		Vector3 vector = Vector3.Normalize(this.localSpace.MultiplyPoint3x4(position) - base.transform.localPosition);
		Vector3 eulerAngles = Quaternion.LookRotation(Vector3.forward, vector).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, this.minAngle, this.maxAngle);
		base.transform.localEulerAngles = eulerAngles;
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x0009BACC File Offset: 0x00099CCC
	public void SetValue(float value)
	{
		float z = Mathf.Lerp(this.minAngle, this.maxAngle, value);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.z = z;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x0009BB0C File Offset: 0x00099D0C
	public void SetNotch(int notchValue)
	{
		if (this.notches == null)
		{
			return;
		}
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (leverNotch.value == notchValue)
			{
				this.SetValue(Mathf.Lerp(leverNotch.minAngleValue, leverNotch.maxAngleValue, 0.5f));
				return;
			}
		}
	}

	// Token: 0x06001D91 RID: 7569 RVA: 0x0009BB64 File Offset: 0x00099D64
	public float GetValue()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.z > 180f)
		{
			localEulerAngles.z -= 360f;
		}
		else if (localEulerAngles.z < -180f)
		{
			localEulerAngles.z += 360f;
		}
		return Mathf.InverseLerp(this.minAngle, this.maxAngle, localEulerAngles.z);
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x0009BBD0 File Offset: 0x00099DD0
	public int GetNotch()
	{
		if (this.notches == null)
		{
			return 0;
		}
		float value = this.GetValue();
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (value >= leverNotch.minAngleValue && value <= leverNotch.maxAngleValue)
			{
				return leverNotch.value;
			}
		}
		return 0;
	}

	// Token: 0x04002789 RID: 10121
	[SerializeField]
	private float breakDistance = 0.2f;

	// Token: 0x0400278A RID: 10122
	[SerializeField]
	private Transform leverGrip;

	// Token: 0x0400278B RID: 10123
	[SerializeField]
	private float maxAngle = 22.5f;

	// Token: 0x0400278C RID: 10124
	[SerializeField]
	private float minAngle = -22.5f;

	// Token: 0x0400278D RID: 10125
	[SerializeField]
	private ManipulatableLever.LeverNotch[] notches;

	// Token: 0x0400278E RID: 10126
	private Matrix4x4 localSpace;

	// Token: 0x02000485 RID: 1157
	[Serializable]
	public class LeverNotch
	{
		// Token: 0x0400278F RID: 10127
		public float minAngleValue;

		// Token: 0x04002790 RID: 10128
		public float maxAngleValue;

		// Token: 0x04002791 RID: 10129
		public int value;
	}
}
