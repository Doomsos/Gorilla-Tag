using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000830 RID: 2096
public class SizeManager : MonoBehaviour
{
	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x0600371F RID: 14111 RVA: 0x001291B0 File Offset: 0x001273B0
	public float currentScale
	{
		get
		{
			if (this.targetRig != null)
			{
				return this.targetRig.ScaleMultiplier;
			}
			if (this.targetPlayer != null)
			{
				return this.targetPlayer.ScaleMultiplier;
			}
			return 1f;
		}
	}

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x06003720 RID: 14112 RVA: 0x001291EB File Offset: 0x001273EB
	// (set) Token: 0x06003721 RID: 14113 RVA: 0x00129220 File Offset: 0x00127420
	public int currentSizeLayerMaskValue
	{
		get
		{
			if (this.targetPlayer)
			{
				return this.targetPlayer.sizeLayerMask;
			}
			if (this.targetRig)
			{
				return this.targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if (this.targetPlayer)
			{
				this.targetPlayer.sizeLayerMask = value;
				if (this.targetRig != null)
				{
					this.targetRig.SizeLayerMask = value;
					return;
				}
			}
			else if (this.targetRig)
			{
				this.targetRig.SizeLayerMask = value;
			}
		}
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0012927A File Offset: 0x0012747A
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
		SizeManagerManager.UnregisterSM(this);
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x00129294 File Offset: 0x00127494
	private void OnEnable()
	{
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x0012929C File Offset: 0x0012749C
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x001292EC File Offset: 0x001274EC
	public void BuildInitialize()
	{
		this.rate = 650f;
		if (this.targetRig != null)
		{
			this.CollectLineRenderers(this.targetRig.gameObject);
		}
		else if (this.targetPlayer != null)
		{
			this.CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		this.mainCameraTransform = Camera.main.transform;
		if (this.targetPlayer != null)
		{
			this.myType = SizeManager.SizeChangerType.LocalOffline;
		}
		else if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.netView != null && this.targetRig.netView.Owner != NetworkSystem.Instance.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
		}
		else
		{
			this.myType = SizeManager.SizeChangerType.LocalOnline;
		}
		this.buildInitialized = true;
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x001293D0 File Offset: 0x001275D0
	private void Awake()
	{
		if (!this.buildInitialized)
		{
			this.BuildInitialize();
		}
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06003727 RID: 14119 RVA: 0x001293E8 File Offset: 0x001275E8
	public void InvokeFixedUpdate()
	{
		float num = 1f;
		SizeChanger sizeChanger = this.ControllingChanger(this.targetRig.transform);
		switch (this.myType)
		{
		case SizeManager.SizeChangerType.LocalOffline:
			num = this.ScaleFromChanger(sizeChanger, this.mainCameraTransform, Time.fixedDeltaTime);
			this.targetPlayer.SetScaleMultiplier((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.LocalOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.OtherOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		}
		if (num != this.lastScale)
		{
			for (int i = 0; i < this.lineRenderers.Length; i++)
			{
				this.lineRenderers[i].widthMultiplier = num * this.initLineScalar[i];
			}
			Vector3 scaleCenter;
			if (sizeChanger != null && sizeChanger.TryGetScaleCenterPoint(out scaleCenter))
			{
				if (this.myType == SizeManager.SizeChangerType.LocalOffline)
				{
					this.targetPlayer.ScaleAwayFromPoint(this.lastScale, num, scaleCenter);
				}
				else if (this.myType == SizeManager.SizeChangerType.LocalOnline)
				{
					GTPlayer.Instance.ScaleAwayFromPoint(this.lastScale, num, scaleCenter);
				}
			}
			if (this.myType == SizeManager.SizeChangerType.LocalOffline)
			{
				this.CheckSizeChangeEvents(num);
			}
		}
		this.lastScale = num;
	}

	// Token: 0x06003728 RID: 14120 RVA: 0x00129588 File Offset: 0x00127788
	private SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.alwaysControlWhenEntered || (sizeChanger.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold))
			{
				return sizeChanger;
			}
		}
		return null;
	}

	// Token: 0x06003729 RID: 14121 RVA: 0x00129614 File Offset: 0x00127814
	private float ScaleFromChanger(SizeChanger sC, Transform t, float deltaTime)
	{
		if (sC == null)
		{
			return 1f;
		}
		SizeChanger.ChangerType changerType = sC.MyType;
		if (changerType == SizeChanger.ChangerType.Static)
		{
			return this.SizeOverTime(sC.MinScale, sC.StaticEasing, deltaTime);
		}
		if (changerType == SizeChanger.ChangerType.Continuous)
		{
			Vector3 vector = Vector3.Project(t.position - sC.StartPos.position, sC.EndPos.position - sC.StartPos.position);
			return Mathf.Clamp(sC.MaxScale - vector.magnitude / (sC.StartPos.position - sC.EndPos.position).magnitude * (sC.MaxScale - sC.MinScale), sC.MinScale, sC.MaxScale);
		}
		return 1f;
	}

	// Token: 0x0600372A RID: 14122 RVA: 0x001296E6 File Offset: 0x001278E6
	private float SizeOverTime(float targetSize, float easing, float deltaTime)
	{
		if (easing <= 0f || Mathf.Abs(this.targetRig.ScaleMultiplier - targetSize) < 0.05f)
		{
			return targetSize;
		}
		return Mathf.MoveTowards(this.targetRig.ScaleMultiplier, targetSize, deltaTime / easing);
	}

	// Token: 0x0600372B RID: 14123 RVA: 0x00129720 File Offset: 0x00127920
	private void CheckSizeChangeEvents(float newSize)
	{
		if (newSize < this.smallThreshold)
		{
			if (!this.isSmall)
			{
				this.isSmall = true;
				this.isLarge = false;
				PlayerGameEvents.MiscEvent("SizeSmall", 1);
				return;
			}
		}
		else if (newSize > this.largeThreshold)
		{
			if (!this.isLarge)
			{
				this.isLarge = true;
				this.isSmall = false;
				PlayerGameEvents.MiscEvent("SizeLarge", 1);
				return;
			}
		}
		else
		{
			this.isLarge = false;
			this.isSmall = false;
		}
	}

	// Token: 0x04004691 RID: 18065
	public List<SizeChanger> touchingChangers;

	// Token: 0x04004692 RID: 18066
	private LineRenderer[] lineRenderers;

	// Token: 0x04004693 RID: 18067
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04004694 RID: 18068
	public VRRig targetRig;

	// Token: 0x04004695 RID: 18069
	public GTPlayer targetPlayer;

	// Token: 0x04004696 RID: 18070
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04004697 RID: 18071
	public float rate = 650f;

	// Token: 0x04004698 RID: 18072
	public Transform mainCameraTransform;

	// Token: 0x04004699 RID: 18073
	public SizeManager.SizeChangerType myType;

	// Token: 0x0400469A RID: 18074
	public float lastScale;

	// Token: 0x0400469B RID: 18075
	private bool buildInitialized;

	// Token: 0x0400469C RID: 18076
	private const float returnToNormalEasing = 0.33f;

	// Token: 0x0400469D RID: 18077
	private float smallThreshold = 0.6f;

	// Token: 0x0400469E RID: 18078
	private float largeThreshold = 1.5f;

	// Token: 0x0400469F RID: 18079
	private bool isSmall;

	// Token: 0x040046A0 RID: 18080
	private bool isLarge;

	// Token: 0x02000831 RID: 2097
	public enum SizeChangerType
	{
		// Token: 0x040046A2 RID: 18082
		LocalOffline,
		// Token: 0x040046A3 RID: 18083
		LocalOnline,
		// Token: 0x040046A4 RID: 18084
		OtherOnline
	}
}
