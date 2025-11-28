using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020001CA RID: 458
public class GreyZoneSummoner : MonoBehaviour
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000C68 RID: 3176 RVA: 0x000437BC File Offset: 0x000419BC
	public Vector3 SummoningFocusPoint
	{
		get
		{
			return this.summoningFocusPoint.position;
		}
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000C69 RID: 3177 RVA: 0x000437C9 File Offset: 0x000419C9
	public float SummonerMaxDistance
	{
		get
		{
			return this.areaTriggerCollider.radius + 1f;
		}
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x000437DC File Offset: 0x000419DC
	private void OnEnable()
	{
		this.greyZoneManager = GreyZoneManager.Instance;
		if (this.greyZoneManager == null)
		{
			return;
		}
		this.greyZoneManager.RegisterSummoner(this);
		this.areaTriggerNotifier.TriggerEnterEvent += this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent += this.ColliderExitedArea;
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x00043840 File Offset: 0x00041A40
	private void OnDisable()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.DeregisterSummoner(this);
		}
		this.areaTriggerNotifier.TriggerEnterEvent -= this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent -= this.ColliderExitedArea;
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x00043898 File Offset: 0x00041A98
	public void UpdateProgressFeedback(bool greyZoneAvailable)
	{
		if (this.greyZoneManager == null)
		{
			return;
		}
		if (greyZoneAvailable && !this.candlesParent.gameObject.activeSelf)
		{
			this.candlesParent.gameObject.SetActive(true);
		}
		this.candlesTimeline.time = (double)Mathf.Clamp01(this.greyZoneManager.SummoningProgress) * this.candlesTimeline.duration;
		this.candlesTimeline.Evaluate();
		if (!this.greyZoneManager.GreyZoneActive)
		{
			float num = (float)this.summoningTones.Count * this.greyZoneManager.SummoningProgress;
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				float num2 = Mathf.InverseLerp((float)i, (float)i + 1f + this.summoningTonesFadeOverlap, num);
				this.summoningTones[i].volume = num2 * this.summoningTonesMaxVolume;
			}
		}
		this.greyZoneActivationButton.isOn = this.greyZoneManager.GreyZoneActive;
		this.greyZoneActivationButton.UpdateColor();
		for (int j = 0; j < this.greyZoneGravityFactorButtons.Count; j++)
		{
			this.greyZoneGravityFactorButtons[j].isOn = (this.greyZoneManager.GravityFactorSelection == j);
			this.greyZoneGravityFactorButtons[j].UpdateColor();
		}
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x000439E1 File Offset: 0x00041BE1
	public void OnGreyZoneActivated()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeOutSummoningTones());
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x000439F6 File Offset: 0x00041BF6
	private IEnumerator FadeOutSummoningTones()
	{
		float fadeStartTime = Time.time;
		float fadeRate = 1f / this.summoningTonesFadeTime;
		while (Time.time < fadeStartTime + this.summoningTonesFadeTime)
		{
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				this.summoningTones[i].volume = Mathf.MoveTowards(this.summoningTones[i].volume, 0f, this.summoningTonesMaxVolume * fadeRate * Time.deltaTime);
			}
			yield return null;
		}
		for (int j = 0; j < this.summoningTones.Count; j++)
		{
			this.summoningTones[j].volume = 0f;
		}
		yield break;
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x00043A08 File Offset: 0x00041C08
	public void ColliderEnteredArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntityBSP component = other.GetComponent<ZoneEntityBSP>();
		VRRig vrrig = (component != null) ? component.entityRig : null;
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigEnteredSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00043A54 File Offset: 0x00041C54
	public void ColliderExitedArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntityBSP component = other.GetComponent<ZoneEntityBSP>();
		VRRig vrrig = (component != null) ? component.entityRig : null;
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigExitedSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x04000F54 RID: 3924
	[SerializeField]
	private Transform summoningFocusPoint;

	// Token: 0x04000F55 RID: 3925
	[SerializeField]
	private Transform candlesParent;

	// Token: 0x04000F56 RID: 3926
	[SerializeField]
	private PlayableDirector candlesTimeline;

	// Token: 0x04000F57 RID: 3927
	[SerializeField]
	private TriggerEventNotifier areaTriggerNotifier;

	// Token: 0x04000F58 RID: 3928
	[SerializeField]
	private SphereCollider areaTriggerCollider;

	// Token: 0x04000F59 RID: 3929
	[SerializeField]
	private GorillaPressableButton greyZoneActivationButton;

	// Token: 0x04000F5A RID: 3930
	[SerializeField]
	private List<AudioSource> summoningTones = new List<AudioSource>();

	// Token: 0x04000F5B RID: 3931
	[SerializeField]
	private float summoningTonesMaxVolume = 1f;

	// Token: 0x04000F5C RID: 3932
	[SerializeField]
	private float summoningTonesFadeOverlap = 0.5f;

	// Token: 0x04000F5D RID: 3933
	[SerializeField]
	private float summoningTonesFadeTime = 4f;

	// Token: 0x04000F5E RID: 3934
	[SerializeField]
	private List<GorillaPressableButton> greyZoneGravityFactorButtons = new List<GorillaPressableButton>();

	// Token: 0x04000F5F RID: 3935
	private GreyZoneManager greyZoneManager;
}
