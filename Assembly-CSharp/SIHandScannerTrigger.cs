using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200011D RID: 285
public class SIHandScannerTrigger : MonoBehaviour, IClickable
{
	// Token: 0x0600073C RID: 1852 RVA: 0x00027C47 File Offset: 0x00025E47
	private void Awake()
	{
		if (this.parentScanner == null)
		{
			this.parentScanner = base.GetComponentInParent<SIHandScanner>();
		}
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00027C64 File Offset: 0x00025E64
	private void OnTriggerEnter(Collider other)
	{
		SIScannableHand component = other.GetComponent<SIScannableHand>();
		if (component == null)
		{
			return;
		}
		this.OnPlayerScanned(component.parentPlayer);
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00027C8E File Offset: 0x00025E8E
	private void OnPlayerScanned(SIPlayer player)
	{
		this.parentScanner.HandScanned(player);
		this.onHandScanned.Invoke();
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00027CA7 File Offset: 0x00025EA7
	public void Click(bool leftHand = false)
	{
		this.OnPlayerScanned(VRRig.LocalRig.GetComponent<SIPlayer>());
	}

	// Token: 0x0400091C RID: 2332
	public SIHandScanner parentScanner;

	// Token: 0x0400091D RID: 2333
	public UnityEvent onHandScanned;
}
