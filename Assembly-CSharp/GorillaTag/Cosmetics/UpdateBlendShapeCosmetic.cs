using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001120 RID: 4384
	public class UpdateBlendShapeCosmetic : MonoBehaviour
	{
		// Token: 0x06006DB5 RID: 28085 RVA: 0x00240543 File Offset: 0x0023E743
		private void Awake()
		{
			this.targetWeight = this.blendStartWeight;
			this.currentWeight = 0f;
		}

		// Token: 0x06006DB6 RID: 28086 RVA: 0x0024055C File Offset: 0x0023E75C
		private void Update()
		{
			this.currentWeight = Mathf.Lerp(this.currentWeight, this.targetWeight, Time.deltaTime * this.blendSpeed);
			this.skinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeIndex, this.currentWeight);
		}

		// Token: 0x06006DB7 RID: 28087 RVA: 0x00240598 File Offset: 0x0023E798
		public void SetBlendValue(bool leftHand, float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x06006DB8 RID: 28088 RVA: 0x002405BE File Offset: 0x0023E7BE
		public void SetBlendValue(float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x06006DB9 RID: 28089 RVA: 0x002405E4 File Offset: 0x0023E7E4
		public void FullyBlend()
		{
			this.targetWeight = this.maxBlendShapeWeight;
		}

		// Token: 0x06006DBA RID: 28090 RVA: 0x002405F2 File Offset: 0x0023E7F2
		public void ResetBlend()
		{
			this.targetWeight = 0f;
		}

		// Token: 0x06006DBB RID: 28091 RVA: 0x002405FF File Offset: 0x0023E7FF
		public float GetBlendValue()
		{
			return this.skinnedMeshRenderer.GetBlendShapeWeight(this.blendShapeIndex);
		}

		// Token: 0x04007F34 RID: 32564
		[Tooltip("The SkinnedMeshRenderer whose BlendShape weight will be updated. This must reference a mesh that has BlendShapes defined in its import settings.")]
		[SerializeField]
		private SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04007F35 RID: 32565
		[Tooltip("Maximum blend shape weight applied when fully blended. Usually 100 for standard Unity BlendShapes.")]
		public float maxBlendShapeWeight = 100f;

		// Token: 0x04007F36 RID: 32566
		[Tooltip("Index of the BlendShape to control. You can find this index in the SkinnedMeshRenderer inspector under 'BlendShapes'.")]
		[SerializeField]
		private int blendShapeIndex;

		// Token: 0x04007F37 RID: 32567
		[Tooltip("Speed at which the BlendShape transitions toward its target weight. Higher values make blending more responsive, lower values make it smoother.")]
		[SerializeField]
		private float blendSpeed = 10f;

		// Token: 0x04007F38 RID: 32568
		[Tooltip("Initial BlendShape weight set when the component awakens. Useful for setting a default deformation state.")]
		[SerializeField]
		private float blendStartWeight;

		// Token: 0x04007F39 RID: 32569
		[Tooltip("If enabled, inverts the incoming blend value (e.g. 0 → 1, 0.2 → 0.8). Useful when an input should drive the opposite direction of deformation.")]
		[SerializeField]
		private bool invertPassedBlend;

		// Token: 0x04007F3A RID: 32570
		private float targetWeight;

		// Token: 0x04007F3B RID: 32571
		private float currentWeight;
	}
}
