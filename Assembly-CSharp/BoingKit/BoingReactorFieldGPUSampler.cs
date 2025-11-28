using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011A6 RID: 4518
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x060071F1 RID: 29169 RVA: 0x00255708 File Offset: 0x00253908
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060071F2 RID: 29170 RVA: 0x00255710 File Offset: 0x00253910
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060071F3 RID: 29171 RVA: 0x00255718 File Offset: 0x00253918
		public void Update()
		{
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			if (this.m_fieldResourceSetId != component.GpuResourceSetId)
			{
				if (this.m_matProps == null)
				{
					this.m_matProps = new MaterialPropertyBlock();
				}
				if (component.UpdateShaderConstants(this.m_matProps, this.PositionSampleMultiplier, this.RotationSampleMultiplier))
				{
					this.m_fieldResourceSetId = component.GpuResourceSetId;
					foreach (Renderer renderer in new Renderer[]
					{
						base.GetComponent<MeshRenderer>(),
						base.GetComponent<SkinnedMeshRenderer>()
					})
					{
						if (!(renderer == null))
						{
							renderer.SetPropertyBlock(this.m_matProps);
						}
					}
				}
			}
		}

		// Token: 0x0400825B RID: 33371
		public BoingReactorField ReactorField;

		// Token: 0x0400825C RID: 33372
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x0400825D RID: 33373
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x0400825E RID: 33374
		private MaterialPropertyBlock m_matProps;

		// Token: 0x0400825F RID: 33375
		private int m_fieldResourceSetId = -1;
	}
}
