using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FEC RID: 4076
	[ExecuteAlways]
	public class TextureTransitionerManager : MonoBehaviour
	{
		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x0600670C RID: 26380 RVA: 0x00218582 File Offset: 0x00216782
		// (set) Token: 0x0600670D RID: 26381 RVA: 0x00218589 File Offset: 0x00216789
		public static TextureTransitionerManager instance { get; private set; }

		// Token: 0x0600670E RID: 26382 RVA: 0x00218591 File Offset: 0x00216791
		protected void Awake()
		{
			if (TextureTransitionerManager.instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TextureTransitionerManager.instance = this;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			this.matPropBlock = new MaterialPropertyBlock();
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x002185D0 File Offset: 0x002167D0
		protected void LateUpdate()
		{
			foreach (TextureTransitioner textureTransitioner in TextureTransitionerManager.components)
			{
				int num = textureTransitioner.textures.Length;
				float num2 = Mathf.Clamp01(textureTransitioner.remapInfo.Remap(textureTransitioner.iDynamicFloat.floatValue));
				TextureTransitioner.DirectionRetentionMode directionRetentionMode = textureTransitioner.directionRetentionMode;
				if (directionRetentionMode != TextureTransitioner.DirectionRetentionMode.IncreaseOnly)
				{
					if (directionRetentionMode == TextureTransitioner.DirectionRetentionMode.DecreaseOnly)
					{
						num2 = Mathf.Min(num2, textureTransitioner.normalizedValue);
					}
				}
				else
				{
					num2 = Mathf.Max(num2, textureTransitioner.normalizedValue);
				}
				float num3 = num2 * (float)(num - 1);
				float num4 = num3 % 1f;
				int num5 = (int)(num4 * 1000f);
				int num6 = (int)num3;
				int num7 = Mathf.Min(num - 1, num6 + 1);
				if (num5 != textureTransitioner.transitionPercent || num6 != textureTransitioner.tex1Index || num7 != textureTransitioner.tex2Index)
				{
					this.matPropBlock.SetFloat(textureTransitioner.texTransitionShaderParam, num4);
					this.matPropBlock.SetTexture(textureTransitioner.tex1ShaderParam, textureTransitioner.textures[num6]);
					this.matPropBlock.SetTexture(textureTransitioner.tex2ShaderParam, textureTransitioner.textures[num7]);
					Renderer[] renderers = textureTransitioner.renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].SetPropertyBlock(this.matPropBlock);
					}
					textureTransitioner.normalizedValue = num2;
					textureTransitioner.transitionPercent = num5;
					textureTransitioner.tex1Index = num6;
					textureTransitioner.tex2Index = num7;
				}
			}
		}

		// Token: 0x06006710 RID: 26384 RVA: 0x00218760 File Offset: 0x00216960
		public static void EnsureInstanceIsAvailable()
		{
			if (TextureTransitionerManager.instance != null)
			{
				return;
			}
			GameObject gameObject = new GameObject();
			TextureTransitionerManager.instance = gameObject.AddComponent<TextureTransitionerManager>();
			gameObject.name = "TextureTransitionerManager (Singleton)";
		}

		// Token: 0x06006711 RID: 26385 RVA: 0x0021878A File Offset: 0x0021698A
		public static void Register(TextureTransitioner component)
		{
			TextureTransitionerManager.components.Add(component);
		}

		// Token: 0x06006712 RID: 26386 RVA: 0x00218797 File Offset: 0x00216997
		public static void Unregister(TextureTransitioner component)
		{
			TextureTransitionerManager.components.Remove(component);
		}

		// Token: 0x0400759F RID: 30111
		public static readonly List<TextureTransitioner> components = new List<TextureTransitioner>(256);

		// Token: 0x040075A0 RID: 30112
		private MaterialPropertyBlock matPropBlock;
	}
}
