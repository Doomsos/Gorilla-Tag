using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class SIScreenRegion : MonoBehaviour
{
	// Token: 0x17000099 RID: 153
	// (get) Token: 0x0600085F RID: 2143 RVA: 0x0002D4D3 File Offset: 0x0002B6D3
	public bool HasPressedButton
	{
		get
		{
			return this._hasPressedButton;
		}
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002D4DC File Offset: 0x0002B6DC
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent != null)
		{
			this.handIndicators.Add(componentInParent);
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0002D500 File Offset: 0x0002B700
	private void OnTriggerExit(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent != null)
		{
			this.handIndicators.Remove(componentInParent);
			if (this.handIndicators.Count == 0)
			{
				this.ClearPressedIndicator();
			}
		}
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0002D537 File Offset: 0x0002B737
	public void RegisterButtonPress()
	{
		if (this.handIndicators.Count > 0)
		{
			this._hasPressedButton = true;
		}
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0002D54E File Offset: 0x0002B74E
	private void ClearPressedIndicator()
	{
		this._hasPressedButton = false;
	}

	// Token: 0x04000A3D RID: 2621
	private HashSet<GorillaTriggerColliderHandIndicator> handIndicators = new HashSet<GorillaTriggerColliderHandIndicator>();

	// Token: 0x04000A3E RID: 2622
	private bool _hasPressedButton;
}
