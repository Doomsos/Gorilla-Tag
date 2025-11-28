using System;
using UnityEngine;

// Token: 0x0200079C RID: 1948
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x060032F6 RID: 13046 RVA: 0x0011348C File Offset: 0x0011168C
	public void Awake()
	{
		this.lights = new Color[this.lightTextures.Length][];
		this.dirs = new Color[this.dirTextures.Length][];
		for (int i = 0; i < this.dirTextures.Length; i++)
		{
			float value = Random.value;
			Debug.Log(value.ToString() + " before load " + Time.realtimeSinceStartup.ToString());
			this.dirs[i] = this.dirTextures[i].GetPixels();
			this.lights[i] = this.lightTextures[i].GetPixels();
			Debug.Log(value.ToString() + " after load " + Time.realtimeSinceStartup.ToString());
		}
	}

	// Token: 0x04004165 RID: 16741
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04004166 RID: 16742
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04004167 RID: 16743
	public Color[][] lights;

	// Token: 0x04004168 RID: 16744
	public Color[][] dirs;

	// Token: 0x04004169 RID: 16745
	public bool done;
}
