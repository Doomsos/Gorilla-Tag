using System;
using TMPro;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class HowManyMonkeDisplay : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001470 RID: 5232 RVA: 0x00075340 File Offset: 0x00073540
	public void OnEnable()
	{
		this.currValue = (this.nextValue = HowManyMonke.ThisMany);
		this.text.text = this.currValue.ToString("N0");
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x00075383 File Offset: 0x00073583
	public void OnDisable()
	{
		HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x000753AC File Offset: 0x000735AC
	private void OnDestroy()
	{
		HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000753CE File Offset: 0x000735CE
	private void HowManyMonke_OnCheck(int thisMany)
	{
		this.currValue = this.nextValue;
		this.nextValue = thisMany;
		this.checkTime = Time.time;
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x000753F0 File Offset: 0x000735F0
	public void SliceUpdate()
	{
		float num = Mathf.Lerp((float)this.currValue, (float)this.nextValue, (Time.time - this.checkTime) / HowManyMonke.RecheckDelay);
		this.text.text = num.ToString("N0");
		this.particleSystem.emission.rateOverTime = this.particleSystemRateToCount.Evaluate(num);
		float sqrMagnitude = (VRRig.LocalRig.transform.position - base.transform.position).sqrMagnitude;
		if (this.observable && sqrMagnitude > this.observableDistance)
		{
			this.observable = false;
			HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
			if (this.observableActive)
			{
				this.observableActive.SetActive(this.observable);
				return;
			}
		}
		else if (!this.observable && sqrMagnitude < this.observableDistance)
		{
			this.observable = true;
			HowManyMonke.OnCheck = (Action<int>)Delegate.Combine(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
			if (this.observableActive)
			{
				this.observableActive.SetActive(this.observable);
			}
		}
	}

	// Token: 0x04001F10 RID: 7952
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04001F11 RID: 7953
	[SerializeField]
	private float observableDistance = 100f;

	// Token: 0x04001F12 RID: 7954
	[SerializeField]
	private GameObject observableActive;

	// Token: 0x04001F13 RID: 7955
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x04001F14 RID: 7956
	[SerializeField]
	private AnimationCurve particleSystemRateToCount;

	// Token: 0x04001F15 RID: 7957
	private bool observable;

	// Token: 0x04001F16 RID: 7958
	private int currValue;

	// Token: 0x04001F17 RID: 7959
	private int nextValue;

	// Token: 0x04001F18 RID: 7960
	private float checkTime;
}
