using System;
using UnityEngine;

// Token: 0x020004E7 RID: 1255
[RequireComponent(typeof(Animator))]
public class RandomAnimationSelector : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600204C RID: 8268 RVA: 0x000AB588 File Offset: 0x000A9788
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animationTrigger = Animator.StringToHash(this.animationTriggerName);
		this.animationSelect = Animator.StringToHash(this.animationSelectName);
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x000AB5B8 File Offset: 0x000A97B8
	public void OnEnable()
	{
		if (this.animator != null)
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lastSliceUpdateTime = Time.time;
		}
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000AB5DC File Offset: 0x000A97DC
	public void SliceUpdate()
	{
		float num = Time.time - this.lastSliceUpdateTime;
		this.lastSliceUpdateTime = Time.time;
		float num2 = 1f - Mathf.Exp(-this.animationChancePerSecond * num);
		if (Random.value < num2)
		{
			float num3 = Time.time - (float)((int)Time.time);
			this.animator.SetFloat(this.animationSelect, num3);
			this.animator.SetTrigger(this.animationTrigger);
		}
	}

	// Token: 0x04002ABB RID: 10939
	[SerializeField]
	private string animationTriggerName;

	// Token: 0x04002ABC RID: 10940
	private int animationTrigger;

	// Token: 0x04002ABD RID: 10941
	[SerializeField]
	private string animationSelectName;

	// Token: 0x04002ABE RID: 10942
	private int animationSelect;

	// Token: 0x04002ABF RID: 10943
	[Range(0f, 1f)]
	[SerializeField]
	private float animationChancePerSecond = 0.33f;

	// Token: 0x04002AC0 RID: 10944
	private Animator animator;

	// Token: 0x04002AC1 RID: 10945
	private float lastSliceUpdateTime;
}
