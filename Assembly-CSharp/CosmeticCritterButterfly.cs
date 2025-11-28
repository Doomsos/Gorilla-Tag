using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class CosmeticCritterButterfly : CosmeticCritter
{
	// Token: 0x1700004F RID: 79
	// (get) Token: 0x0600044B RID: 1099 RVA: 0x00018DC7 File Offset: 0x00016FC7
	public ParticleSystem.EmitParams GetEmitParams
	{
		get
		{
			return this.emitParams;
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00018DCF File Offset: 0x00016FCF
	public void SetStartPos(Vector3 initialPos)
	{
		this.startPosition = initialPos;
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00018DD8 File Offset: 0x00016FD8
	public override void SetRandomVariables()
	{
		this.direction = Random.insideUnitSphere;
		this.emitParams.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
		this.particleSystem.Emit(this.emitParams, 1);
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00018E35 File Offset: 0x00017035
	public override void Tick()
	{
		base.transform.position = this.startPosition + (float)base.GetAliveTime() * this.speed * this.direction;
	}

	// Token: 0x040004DA RID: 1242
	[Tooltip("The speed this Butterfly will move at.")]
	[SerializeField]
	private float speed = 1f;

	// Token: 0x040004DB RID: 1243
	[Tooltip("Emit one particle from this particle system when spawning.")]
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x040004DC RID: 1244
	private Vector3 startPosition;

	// Token: 0x040004DD RID: 1245
	private Vector3 direction;

	// Token: 0x040004DE RID: 1246
	private ParticleSystem.EmitParams emitParams;
}
