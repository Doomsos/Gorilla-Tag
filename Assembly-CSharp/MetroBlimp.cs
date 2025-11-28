using System;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class MetroBlimp : MonoBehaviour
{
	// Token: 0x060009FB RID: 2555 RVA: 0x000361DB File Offset: 0x000343DB
	private void Awake()
	{
		this._startLocalHeight = base.transform.localPosition.y;
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000361F4 File Offset: 0x000343F4
	public void Tick()
	{
		bool flag = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f < 0.0001f;
		int num = Mathf.CeilToInt(this._numHandsOnBlimp / 2f);
		if (this._numHandsOnBlimp == 0f)
		{
			this._topStayTime = 0f;
			if (flag)
			{
				this.blimpRenderer.material.DisableKeyword("_INNER_GLOW");
			}
		}
		else
		{
			this._topStayTime += Time.deltaTime;
			if (flag)
			{
				this.blimpRenderer.material.EnableKeyword("_INNER_GLOW");
			}
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition;
		float y = vector.y;
		float num2 = this._startLocalHeight + this.descendOffset;
		float deltaTime = Time.deltaTime;
		if (num > 0)
		{
			if (y > num2)
			{
				vector += Vector3.down * (this.descendSpeed * (float)num * deltaTime);
			}
		}
		else if (y < this._startLocalHeight)
		{
			vector += Vector3.up * (this.ascendSpeed * deltaTime);
		}
		base.transform.localPosition = Vector3.Slerp(localPosition, vector, 0.5f);
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00036323 File Offset: 0x00034523
	private static bool IsPlayerHand(Collider c)
	{
		return c.gameObject.IsOnLayer(UnityLayer.GorillaHand);
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x00036332 File Offset: 0x00034532
	private void OnTriggerEnter(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp += 1f;
		}
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0003634E File Offset: 0x0003454E
	private void OnTriggerExit(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp -= 1f;
		}
	}

	// Token: 0x04000C38 RID: 3128
	public MetroSpotlight spotLightLeft;

	// Token: 0x04000C39 RID: 3129
	public MetroSpotlight spotLightRight;

	// Token: 0x04000C3A RID: 3130
	[Space]
	public BoxCollider topCollider;

	// Token: 0x04000C3B RID: 3131
	public Material blimpMaterial;

	// Token: 0x04000C3C RID: 3132
	public Renderer blimpRenderer;

	// Token: 0x04000C3D RID: 3133
	[Space]
	public float ascendSpeed = 1f;

	// Token: 0x04000C3E RID: 3134
	public float descendSpeed = 0.5f;

	// Token: 0x04000C3F RID: 3135
	public float descendOffset = -24.1f;

	// Token: 0x04000C40 RID: 3136
	public float descendReactionTime = 3f;

	// Token: 0x04000C41 RID: 3137
	[Space]
	[NonSerialized]
	private float _startLocalHeight;

	// Token: 0x04000C42 RID: 3138
	[NonSerialized]
	private float _topStayTime;

	// Token: 0x04000C43 RID: 3139
	[NonSerialized]
	private float _numHandsOnBlimp;

	// Token: 0x04000C44 RID: 3140
	[NonSerialized]
	private bool _lowering;

	// Token: 0x04000C45 RID: 3141
	private const string _INNER_GLOW = "_INNER_GLOW";
}
