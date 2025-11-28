using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000CAD RID: 3245
public class Firework : MonoBehaviour
{
	// Token: 0x06004F4D RID: 20301 RVA: 0x00198BB2 File Offset: 0x00196DB2
	private void Launch()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._controller)
		{
			this._controller.Launch(this);
		}
	}

	// Token: 0x06004F4E RID: 20302 RVA: 0x00198BD8 File Offset: 0x00196DD8
	private void OnValidate()
	{
		if (!this._controller)
		{
			this._controller = base.GetComponentInParent<FireworksController>();
		}
		if (!this._controller)
		{
			return;
		}
		Firework[] array = this._controller.fireworks;
		if (Enumerable.Contains<Firework>(array, this))
		{
			return;
		}
		array = Enumerable.ToArray<Firework>(Enumerable.Where<Firework>(Enumerable.Concat<Firework>(array, new Firework[]
		{
			this
		}), (Firework x) => x != null));
		this._controller.fireworks = array;
	}

	// Token: 0x06004F4F RID: 20303 RVA: 0x00198C68 File Offset: 0x00196E68
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x06004F50 RID: 20304 RVA: 0x00198C89 File Offset: 0x00196E89
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x04005DBF RID: 23999
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x04005DC0 RID: 24000
	[Space]
	public Transform origin;

	// Token: 0x04005DC1 RID: 24001
	public Transform target;

	// Token: 0x04005DC2 RID: 24002
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x04005DC3 RID: 24003
	public Color colorTarget = Color.magenta;

	// Token: 0x04005DC4 RID: 24004
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x04005DC5 RID: 24005
	public AudioSource sourceTarget;

	// Token: 0x04005DC6 RID: 24006
	[Space]
	public ParticleSystem trail;

	// Token: 0x04005DC7 RID: 24007
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x04005DC8 RID: 24008
	[Space]
	public bool doTrail = true;

	// Token: 0x04005DC9 RID: 24009
	public bool doTrailAudio = true;

	// Token: 0x04005DCA RID: 24010
	public bool doExplosion = true;
}
