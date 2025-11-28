using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200053B RID: 1339
public class AutomaticAdjustIPD : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060021B1 RID: 8625 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000B0258 File Offset: 0x000AE458
	public void SliceUpdate()
	{
		if (!this.headset.isValid)
		{
			this.headset = InputDevices.GetDeviceAtXRNode(3);
		}
		if (this.headset.isValid && this.headset.TryGetFeatureValue(CommonUsages.leftEyePosition, ref this.leftEyePosition) && this.headset.TryGetFeatureValue(CommonUsages.rightEyePosition, ref this.rightEyePosition))
		{
			this.currentIPD = (this.rightEyePosition - this.leftEyePosition).magnitude;
			if (Mathf.Abs(this.lastIPD - this.currentIPD) < 0.01f)
			{
				return;
			}
			this.lastIPD = this.currentIPD;
			for (int i = 0; i < this.adjustXScaleObjects.Length; i++)
			{
				Transform transform = this.adjustXScaleObjects[i];
				if (!transform)
				{
					return;
				}
				transform.localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (this.currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}

	// Token: 0x04002C5D RID: 11357
	public InputDevice headset;

	// Token: 0x04002C5E RID: 11358
	public float currentIPD;

	// Token: 0x04002C5F RID: 11359
	public Vector3 leftEyePosition;

	// Token: 0x04002C60 RID: 11360
	public Vector3 rightEyePosition;

	// Token: 0x04002C61 RID: 11361
	public bool testOverride;

	// Token: 0x04002C62 RID: 11362
	public Transform[] adjustXScaleObjects;

	// Token: 0x04002C63 RID: 11363
	public float sizeAt58mm = 1f;

	// Token: 0x04002C64 RID: 11364
	public float sizeAt63mm = 1.12f;

	// Token: 0x04002C65 RID: 11365
	public float lastIPD;
}
