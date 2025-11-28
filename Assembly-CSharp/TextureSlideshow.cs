using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class TextureSlideshow : MonoBehaviour
{
	// Token: 0x060000AE RID: 174 RVA: 0x00005258 File Offset: 0x00003458
	private void Awake()
	{
		this._renderer = base.GetComponent<Renderer>();
		this._renderer.material.mainTexture = this.textures[0];
	}

	// Token: 0x060000AF RID: 175 RVA: 0x0000527E File Offset: 0x0000347E
	private void OnEnable()
	{
		base.StartCoroutine(this.runSlideshow());
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x0000528D File Offset: 0x0000348D
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00005295 File Offset: 0x00003495
	private IEnumerator runSlideshow()
	{
		yield return new WaitForSecondsRealtime(this.prePause);
		int i = 0;
		for (;;)
		{
			yield return new WaitForSecondsRealtime(Random.Range(this.minMaxPause.x, this.minMaxPause.y));
			this._renderer.material.mainTexture = this.textures[i];
			i = (i + 1) % this.textures.Length;
		}
		yield break;
	}

	// Token: 0x040000C7 RID: 199
	private Renderer _renderer;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	private Texture[] textures;

	// Token: 0x040000C9 RID: 201
	[SerializeField]
	private Vector2 minMaxPause;

	// Token: 0x040000CA RID: 202
	[SerializeField]
	private float prePause = 1f;
}
