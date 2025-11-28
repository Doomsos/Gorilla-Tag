using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000174 RID: 372
public class MetroSpotlight : MonoBehaviour
{
	// Token: 0x06000A03 RID: 2563 RVA: 0x00036410 File Offset: 0x00034610
	public void Tick()
	{
		if (!this._light)
		{
			return;
		}
		if (!this._target)
		{
			return;
		}
		this._time += this.speed * Time.deltaTime * Time.deltaTime;
		Vector3 position = this._target.position;
		Vector3 normalized = (position - this._light.position).normalized;
		Vector3 vector = Vector3.Cross(normalized, this._blimp.forward);
		Vector3 yDir = Vector3.Cross(normalized, vector);
		Vector3 vector2 = MetroSpotlight.Figure8(position, vector, yDir, this._radius, this._time, this._offset, this._theta);
		this._light.LookAt(vector2);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x000364C4 File Offset: 0x000346C4
	private static Vector3 Figure8(Vector3 origin, Vector3 xDir, Vector3 yDir, float scale, float t, float offset, float theta)
	{
		float num = 2f / (3f - Mathf.Cos(2f * (t + offset)));
		float num2 = scale * num * Mathf.Cos(t + offset);
		float num3 = scale * num * Mathf.Sin(2f * (t + offset)) / 2f;
		Vector3 vector = Vector3.Cross(xDir, yDir);
		Quaternion quaternion = Quaternion.AngleAxis(theta, vector);
		xDir = quaternion * xDir;
		yDir = quaternion * yDir;
		Vector3 vector2 = xDir * num2 + yDir * num3;
		return origin + vector2;
	}

	// Token: 0x04000C49 RID: 3145
	[SerializeField]
	private Transform _blimp;

	// Token: 0x04000C4A RID: 3146
	[SerializeField]
	private Transform _light;

	// Token: 0x04000C4B RID: 3147
	[SerializeField]
	private Transform _target;

	// Token: 0x04000C4C RID: 3148
	[FormerlySerializedAs("_scale")]
	[SerializeField]
	private float _radius = 1f;

	// Token: 0x04000C4D RID: 3149
	[SerializeField]
	private float _offset;

	// Token: 0x04000C4E RID: 3150
	[SerializeField]
	private float _theta;

	// Token: 0x04000C4F RID: 3151
	public float speed = 16f;

	// Token: 0x04000C50 RID: 3152
	[Space]
	private float _time;
}
