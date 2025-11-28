using System;
using System.Collections.Generic;
using BoingKit;
using UnityEngine;

// Token: 0x02000020 RID: 32
public class BushFieldReactorMain : MonoBehaviour
{
	// Token: 0x0600007B RID: 123 RVA: 0x0000463C File Offset: 0x0000283C
	public void Start()
	{
		Random.InitState(0);
		for (int i = 0; i < this.NumBushes; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.Bush);
			float num = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
			gameObject.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject.transform.localScale = num * Vector3.one;
			BoingBehavior component = gameObject.GetComponent<BoingBehavior>();
			if (component != null)
			{
				component.Reboot();
			}
		}
		for (int j = 0; j < this.NumBlossoms; j++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Blossom);
			float num2 = Random.Range(this.BlossomScaleRange.x, this.BlossomScaleRange.y);
			gameObject2.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.y), 0.2f * num2, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject2.transform.localScale = num2 * Vector3.one;
			BoingBehavior component2 = gameObject2.GetComponent<BoingBehavior>();
			if (component2 != null)
			{
				component2.Reboot();
			}
		}
		this.m_aSphere = new List<GameObject>(this.NumSpheresPerCircle * this.NumCircles);
		for (int k = 0; k < this.NumCircles; k++)
		{
			for (int l = 0; l < this.NumSpheresPerCircle; l++)
			{
				this.m_aSphere.Add(Object.Instantiate<GameObject>(this.Sphere));
			}
		}
		this.m_basePhase = 0f;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x000048A8 File Offset: 0x00002AA8
	public void Update()
	{
		int num = 0;
		for (int i = 0; i < this.NumCircles; i++)
		{
			float num2 = this.MaxCircleRadius / (float)(i + 1);
			for (int j = 0; j < this.NumSpheresPerCircle; j++)
			{
				float num3 = this.m_basePhase + (float)j / (float)this.NumSpheresPerCircle * 2f * 3.1415927f;
				num3 *= ((i % 2 == 0) ? 1f : -1f);
				this.m_aSphere[num].transform.position = new Vector3(num2 * Mathf.Cos(num3), 0.2f, num2 * Mathf.Sin(num3));
				num++;
			}
		}
		this.m_basePhase -= this.CircleSpeed / this.MaxCircleRadius * Time.deltaTime;
	}

	// Token: 0x04000087 RID: 135
	public GameObject Bush;

	// Token: 0x04000088 RID: 136
	public GameObject Blossom;

	// Token: 0x04000089 RID: 137
	public GameObject Sphere;

	// Token: 0x0400008A RID: 138
	public int NumBushes;

	// Token: 0x0400008B RID: 139
	public Vector2 BushScaleRange;

	// Token: 0x0400008C RID: 140
	public int NumBlossoms;

	// Token: 0x0400008D RID: 141
	public Vector2 BlossomScaleRange;

	// Token: 0x0400008E RID: 142
	public Vector2 FieldBounds;

	// Token: 0x0400008F RID: 143
	public int NumSpheresPerCircle;

	// Token: 0x04000090 RID: 144
	public int NumCircles;

	// Token: 0x04000091 RID: 145
	public float MaxCircleRadius;

	// Token: 0x04000092 RID: 146
	public float CircleSpeed;

	// Token: 0x04000093 RID: 147
	private List<GameObject> m_aSphere;

	// Token: 0x04000094 RID: 148
	private float m_basePhase;
}
