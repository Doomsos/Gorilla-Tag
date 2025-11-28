using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000565 RID: 1381
public class BuilderSizeLayerChanger : MonoBehaviour
{
	// Token: 0x17000395 RID: 917
	// (get) Token: 0x060022CC RID: 8908 RVA: 0x000B5DAC File Offset: 0x000B3FAC
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x000B5DEC File Offset: 0x000B3FEC
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000B5E04 File Offset: 0x000B4004
	public void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000B5E90 File Offset: 0x000B4090
	public void OnTriggerExit(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x04002D72 RID: 11634
	public float maxScale;

	// Token: 0x04002D73 RID: 11635
	public float minScale;

	// Token: 0x04002D74 RID: 11636
	public bool isAssurance;

	// Token: 0x04002D75 RID: 11637
	public bool affectLayerA = true;

	// Token: 0x04002D76 RID: 11638
	public bool affectLayerB = true;

	// Token: 0x04002D77 RID: 11639
	public bool affectLayerC = true;

	// Token: 0x04002D78 RID: 11640
	public bool affectLayerD = true;

	// Token: 0x04002D79 RID: 11641
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04002D7A RID: 11642
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04002D7B RID: 11643
	[SerializeField]
	private GameObject fxForLayerChange;
}
