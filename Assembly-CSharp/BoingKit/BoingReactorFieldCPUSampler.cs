using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011A5 RID: 4517
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x060071EC RID: 29164 RVA: 0x0025560E File Offset: 0x0025380E
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060071ED RID: 29165 RVA: 0x00255616 File Offset: 0x00253816
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060071EE RID: 29166 RVA: 0x00255620 File Offset: 0x00253820
		public void SampleFromField()
		{
			this.m_objPosition = base.transform.position;
			this.m_objRotation = base.transform.rotation;
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				return;
			}
			Vector3 vector;
			Vector4 v;
			if (!component.SampleCpuGrid(base.transform.position, out vector, out v))
			{
				return;
			}
			base.transform.position = this.m_objPosition + vector * this.PositionSampleMultiplier;
			base.transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(v, true), this.RotationSampleMultiplier) * this.m_objRotation;
		}

		// Token: 0x060071EF RID: 29167 RVA: 0x002556DF File Offset: 0x002538DF
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x04008255 RID: 33365
		public BoingReactorField ReactorField;

		// Token: 0x04008256 RID: 33366
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04008257 RID: 33367
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04008258 RID: 33368
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04008259 RID: 33369
		private Vector3 m_objPosition;

		// Token: 0x0400825A RID: 33370
		private Quaternion m_objRotation;
	}
}
