using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class MazePlayerCollection : MonoBehaviour
{
	// Token: 0x060009A3 RID: 2467 RVA: 0x00033D64 File Offset: 0x00031F64
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00033D87 File Offset: 0x00031F87
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00033DAC File Offset: 0x00031FAC
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (!this.containedRigs.Contains(component))
		{
			this.containedRigs.Add(component);
		}
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00033DFC File Offset: 0x00031FFC
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component))
		{
			this.containedRigs.Remove(component);
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00033E50 File Offset: 0x00032050
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => ((r != null) ? r.creator : null) == null || r.creator == otherPlayer);
	}

	// Token: 0x04000BC2 RID: 3010
	public List<VRRig> containedRigs = new List<VRRig>();

	// Token: 0x04000BC3 RID: 3011
	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
