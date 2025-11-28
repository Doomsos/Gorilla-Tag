using System;
using UnityEngine;

// Token: 0x020004E4 RID: 1252
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemEventShortcut : MonoBehaviour
{
	// Token: 0x0600203C RID: 8252 RVA: 0x000AB220 File Offset: 0x000A9420
	private void InitIfNeeded()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			this.ps = base.GetComponent<ParticleSystem>();
			this.shape = this.ps.shape;
			this.poolExists = ObjectPools.instance.DoesPoolExist(base.gameObject);
		}
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x000AB26F File Offset: 0x000A946F
	public void StopAndClear()
	{
		this.InitIfNeeded();
		this.ps.Stop(true, 0);
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000AB284 File Offset: 0x000A9484
	public void ClearAndPlay()
	{
		this.InitIfNeeded();
		this.ps.Clear();
		this.ps.Play();
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000AB2A2 File Offset: 0x000A94A2
	public void PlayFromMesh(MeshRenderer mesh)
	{
		this.InitIfNeeded();
		this.shape.shapeType = 13;
		this.shape.meshRenderer = mesh;
		this.ps.Play();
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000AB2CE File Offset: 0x000A94CE
	public void PlayFromSkin(SkinnedMeshRenderer skin)
	{
		this.InitIfNeeded();
		this.shape.shapeType = 14;
		this.shape.skinnedMeshRenderer = skin;
		this.ps.Play();
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x000AB2FA File Offset: 0x000A94FA
	public void ReturnToPool()
	{
		this.InitIfNeeded();
		if (this.poolExists)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000AB31A File Offset: 0x000A951A
	private void OnParticleSystemStopped()
	{
		this.ReturnToPool();
	}

	// Token: 0x04002AAA RID: 10922
	private bool initialized;

	// Token: 0x04002AAB RID: 10923
	private ParticleSystem ps;

	// Token: 0x04002AAC RID: 10924
	private ParticleSystem.ShapeModule shape;

	// Token: 0x04002AAD RID: 10925
	private bool poolExists;
}
