using System;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class WingsWearable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000B38 RID: 2872 RVA: 0x0003D0B8 File Offset: 0x0003B2B8
	private void Awake()
	{
		if (this.animator == null)
		{
			GTDev.LogError<string>("WingsWearable on " + base.gameObject.name + " missing animator", null);
			return;
		}
		this.xform = this.animator.transform;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003D105 File Offset: 0x0003B305
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.oldPos = this.xform.localPosition;
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0003D120 File Offset: 0x0003B320
	public void SliceUpdate()
	{
		Vector3 position = this.xform.position;
		float num = (position - this.oldPos).magnitude / Time.deltaTime;
		float num2 = this.flapSpeedCurve.Evaluate(Mathf.Abs(num));
		this.animator.SetFloat(this.flapSpeedParamID, num2);
		this.oldPos = position;
	}

	// Token: 0x04000DA9 RID: 3497
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	// Token: 0x04000DAA RID: 3498
	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	// Token: 0x04000DAB RID: 3499
	private Transform xform;

	// Token: 0x04000DAC RID: 3500
	private Vector3 oldPos;

	// Token: 0x04000DAD RID: 3501
	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
