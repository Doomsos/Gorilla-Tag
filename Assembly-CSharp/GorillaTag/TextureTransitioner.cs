using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FEA RID: 4074
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x06006706 RID: 26374 RVA: 0x002183F7 File Offset: 0x002165F7
		protected void Awake()
		{
			if (Application.isPlaying || this.editorPreview)
			{
				TextureTransitionerManager.EnsureInstanceIsAvailable();
			}
			this.RefreshShaderParams();
			this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
			this.ResetToDefaultState();
		}

		// Token: 0x06006707 RID: 26375 RVA: 0x0021842C File Offset: 0x0021662C
		protected void OnEnable()
		{
			TextureTransitionerManager.Register(this);
			if (Application.isPlaying && !this.remapInfo.IsValid())
			{
				Debug.LogError("Bad min/max values for remapRanges: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.textures.Length == 0)
			{
				Debug.LogError("Textures array is empty: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.iDynamicFloat == null)
			{
				if (this.dynamicFloatComponent == null)
				{
					Debug.LogError("dynamicFloatComponent cannot be null: " + this.GetComponentPath(int.MaxValue), this);
				}
				this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
				if (this.iDynamicFloat == null)
				{
					Debug.LogError("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath(int.MaxValue), this);
					base.enabled = false;
				}
			}
		}

		// Token: 0x06006708 RID: 26376 RVA: 0x0021851A File Offset: 0x0021671A
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x06006709 RID: 26377 RVA: 0x00218522 File Offset: 0x00216722
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x0600670A RID: 26378 RVA: 0x00218557 File Offset: 0x00216757
		public void ResetToDefaultState()
		{
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x04007589 RID: 30089
		public bool editorPreview;

		// Token: 0x0400758A RID: 30090
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x0400758B RID: 30091
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x0400758C RID: 30092
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x0400758D RID: 30093
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x0400758E RID: 30094
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x0400758F RID: 30095
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x04007590 RID: 30096
		public Texture[] textures;

		// Token: 0x04007591 RID: 30097
		public Renderer[] renderers;

		// Token: 0x04007592 RID: 30098
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x04007593 RID: 30099
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x04007594 RID: 30100
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x04007595 RID: 30101
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x04007596 RID: 30102
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x04007597 RID: 30103
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x04007598 RID: 30104
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x04007599 RID: 30105
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02000FEB RID: 4075
		public enum DirectionRetentionMode
		{
			// Token: 0x0400759B RID: 30107
			None,
			// Token: 0x0400759C RID: 30108
			IncreaseOnly,
			// Token: 0x0400759D RID: 30109
			DecreaseOnly
		}
	}
}
