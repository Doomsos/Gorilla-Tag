using System;
using UnityEngine;

// Token: 0x02000CB5 RID: 3253
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06004F71 RID: 20337 RVA: 0x00199524 File Offset: 0x00197724
	public void Awake()
	{
		if (this.particleFX == null)
		{
			this.particleFX = base.GetComponent<ParticleSystem>();
		}
		if (this.particleFX == null)
		{
			Debug.LogError("SurfaceImpactFX: No ParticleSystem found! Disabling component.", this);
			base.enabled = false;
			return;
		}
		this.fxMainModule = this.particleFX.main;
	}

	// Token: 0x06004F72 RID: 20338 RVA: 0x0019957D File Offset: 0x0019777D
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x04005DF3 RID: 24051
	public ParticleSystem particleFX;

	// Token: 0x04005DF4 RID: 24052
	public float startingGravityModifier;

	// Token: 0x04005DF5 RID: 24053
	public Vector3 startingScale = Vector3.one;

	// Token: 0x04005DF6 RID: 24054
	private ParticleSystem.MainModule fxMainModule;
}
