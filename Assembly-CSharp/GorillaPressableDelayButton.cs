using System;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class GorillaPressableDelayButton : GorillaPressableButton, IGorillaSliceableSimple
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060003DC RID: 988 RVA: 0x00017578 File Offset: 0x00015778
	// (remove) Token: 0x060003DD RID: 989 RVA: 0x000175B0 File Offset: 0x000157B0
	public event Action onPressBegin;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x060003DE RID: 990 RVA: 0x000175E8 File Offset: 0x000157E8
	// (remove) Token: 0x060003DF RID: 991 RVA: 0x00017620 File Offset: 0x00015820
	public event Action onPressAbort;

	// Token: 0x060003E0 RID: 992 RVA: 0x00017658 File Offset: 0x00015858
	private void Awake()
	{
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00017694 File Offset: 0x00015894
	private new void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touching)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touching = collider;
		this.timer = 0f;
		this.UpdateFillBar();
		Action action = this.onPressBegin;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00017704 File Offset: 0x00015904
	private void OnTriggerExit(Collider other)
	{
		if (other != this.touching)
		{
			return;
		}
		this.touching = null;
		this.timer = 0f;
		this.UpdateFillBar();
		Action action = this.onPressAbort;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x0001773D File Offset: 0x0001593D
	public new void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00017746 File Offset: 0x00015946
	public new void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00017750 File Offset: 0x00015950
	public void SliceUpdate()
	{
		if (this.touching == null)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.delayTime)
		{
			base.OnTriggerEnter(this.touching);
			this.touching = null;
			this.timer = 0f;
		}
		this.UpdateFillBar();
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x000177B0 File Offset: 0x000159B0
	public void SetFillBar(Transform newFillBar)
	{
		this.fillBar = newFillBar;
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x000177F4 File Offset: 0x000159F4
	private void UpdateFillBar()
	{
		if (this.fillBar == null)
		{
			return;
		}
		float num = (this.delayTime > 0f) ? Mathf.Clamp01(this.timer / this.delayTime) : ((float)((this.timer > 0f) ? 1 : 0));
		this.fillBarScale.x = this.fillbarStartingScale.x * num;
		this.fillBar.localScale = this.fillBarScale;
	}

	// Token: 0x0400045C RID: 1116
	private Collider touching;

	// Token: 0x0400045D RID: 1117
	private float timer;

	// Token: 0x0400045E RID: 1118
	[SerializeField]
	[Range(0.01f, 5f)]
	public float delayTime = 1f;

	// Token: 0x0400045F RID: 1119
	[SerializeField]
	private Transform fillBar;

	// Token: 0x04000460 RID: 1120
	private Vector3 fillbarStartingScale;

	// Token: 0x04000461 RID: 1121
	private Vector3 fillBarScale;
}
