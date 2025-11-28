using System;
using UnityEngine;

// Token: 0x0200043B RID: 1083
public class RigDuplicationZone : MonoBehaviour
{
	// Token: 0x14000038 RID: 56
	// (add) Token: 0x06001AA2 RID: 6818 RVA: 0x0008CE94 File Offset: 0x0008B094
	// (remove) Token: 0x06001AA3 RID: 6819 RVA: 0x0008CEC8 File Offset: 0x0008B0C8
	public static event RigDuplicationZone.RigDuplicationZoneAction OnEnabled;

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x0008CEFB File Offset: 0x0008B0FB
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x0008CF03 File Offset: 0x0008B103
	private void OnEnable()
	{
		RigDuplicationZone.OnEnabled += this.RigDuplicationZone_OnEnabled;
		if (RigDuplicationZone.OnEnabled != null)
		{
			RigDuplicationZone.OnEnabled(this);
		}
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x0008CF28 File Offset: 0x0008B128
	private void OnDisable()
	{
		RigDuplicationZone.OnEnabled -= this.RigDuplicationZone_OnEnabled;
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x0008CF3B File Offset: 0x0008B13B
	private void RigDuplicationZone_OnEnabled(RigDuplicationZone z)
	{
		if (z == this)
		{
			return;
		}
		if (z.id != this.id)
		{
			return;
		}
		this.setOtherZone(z);
		z.setOtherZone(this);
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x0008CF69 File Offset: 0x0008B169
	private void setOtherZone(RigDuplicationZone z)
	{
		this.otherZone = z;
		this.offsetToOtherZone = z.transform.position - base.transform.position;
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x0008CF94 File Offset: 0x0008B194
	private void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = true;
			return;
		}
		component.SetDuplicationZone(this);
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0008CFCC File Offset: 0x0008B1CC
	private void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = false;
			return;
		}
		component.ClearDuplicationZone(this);
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001AAB RID: 6827 RVA: 0x0008D001 File Offset: 0x0008B201
	public Vector3 VisualOffsetForRigs
	{
		get
		{
			if (!this.otherZone.playerInZone)
			{
				return Vector3.zero;
			}
			return this.offsetToOtherZone;
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001AAC RID: 6828 RVA: 0x0008D01C File Offset: 0x0008B21C
	public bool IsApplyingDisplacement
	{
		get
		{
			return this.otherZone.playerInZone;
		}
	}

	// Token: 0x0400240D RID: 9229
	private RigDuplicationZone otherZone;

	// Token: 0x0400240E RID: 9230
	[SerializeField]
	private string id;

	// Token: 0x0400240F RID: 9231
	private bool playerInZone;

	// Token: 0x04002410 RID: 9232
	private Vector3 offsetToOtherZone;

	// Token: 0x0200043C RID: 1084
	// (Invoke) Token: 0x06001AAF RID: 6831
	public delegate void RigDuplicationZoneAction(RigDuplicationZone z);
}
