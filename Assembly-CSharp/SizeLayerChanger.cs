using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200082D RID: 2093
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x06003712 RID: 14098 RVA: 0x00128ED8 File Offset: 0x001270D8
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

	// Token: 0x06003713 RID: 14099 RVA: 0x00128F18 File Offset: 0x00127118
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x00128F30 File Offset: 0x00127130
	public void OnTriggerEnter(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x00128FB0 File Offset: 0x001271B0
	public void OnTriggerExit(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x0400467E RID: 18046
	public float maxScale;

	// Token: 0x0400467F RID: 18047
	public float minScale;

	// Token: 0x04004680 RID: 18048
	public bool isAssurance;

	// Token: 0x04004681 RID: 18049
	public bool affectLayerA = true;

	// Token: 0x04004682 RID: 18050
	public bool affectLayerB = true;

	// Token: 0x04004683 RID: 18051
	public bool affectLayerC = true;

	// Token: 0x04004684 RID: 18052
	public bool affectLayerD = true;

	// Token: 0x04004685 RID: 18053
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04004686 RID: 18054
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04004687 RID: 18055
	[SerializeField]
	private bool triggerWithBodyCollider;
}
