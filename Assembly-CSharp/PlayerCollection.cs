using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class PlayerCollection : MonoBehaviour
{
	// Token: 0x060018F3 RID: 6387 RVA: 0x0008594A File Offset: 0x00083B4A
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x0008596D File Offset: 0x00083B6D
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x00085990 File Offset: 0x00083B90
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

	// Token: 0x060018F6 RID: 6390 RVA: 0x000859E0 File Offset: 0x00083BE0
	public void OnTriggerExit(Collider other)
	{
		SphereCollider component = other.GetComponent<SphereCollider>();
		if (!component)
		{
			return;
		}
		VRRig component2 = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component2 == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component2))
		{
			Collider[] components = base.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				Vector3 vector;
				float num;
				if (Physics.ComputePenetration(components[i], base.transform.position, base.transform.rotation, component, component.transform.position, component.transform.rotation, ref vector, ref num))
				{
					return;
				}
			}
			this.containedRigs.Remove(component2);
		}
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x00085A84 File Offset: 0x00083C84
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => r.creator == null || r.creator == otherPlayer);
	}

	// Token: 0x0400224D RID: 8781
	[DebugReadout]
	[NonSerialized]
	public readonly List<VRRig> containedRigs = new List<VRRig>(10);
}
