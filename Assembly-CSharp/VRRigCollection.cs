using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C9B RID: 3227
[RequireComponent(typeof(CompositeTriggerEvents))]
public class VRRigCollection : MonoBehaviour
{
	// Token: 0x1700074A RID: 1866
	// (get) Token: 0x06004ED7 RID: 20183 RVA: 0x00197BF2 File Offset: 0x00195DF2
	public List<RigContainer> Rigs
	{
		get
		{
			return this.containedRigs;
		}
	}

	// Token: 0x06004ED8 RID: 20184 RVA: 0x00197BFA File Offset: 0x00195DFA
	private void OnEnable()
	{
		this.collisionTriggerEvents.CompositeTriggerEnter += this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit += this.OnRigTriggerExit;
	}

	// Token: 0x06004ED9 RID: 20185 RVA: 0x00197C2C File Offset: 0x00195E2C
	private void OnDisable()
	{
		for (int i = this.containedRigs.Count - 1; i >= 0; i--)
		{
			this.RigDisabled(this.containedRigs[i]);
		}
		this.collisionTriggerEvents.CompositeTriggerEnter -= this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit -= this.OnRigTriggerExit;
	}

	// Token: 0x06004EDA RID: 20186 RVA: 0x00197C94 File Offset: 0x00195E94
	private void OnRigTriggerEnter(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(ref rigContainer) || other != rigContainer.HeadCollider || this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		rigContainer.RigEvents.disableEvent += new Action<RigContainer>(this.RigDisabled);
		this.containedRigs.Add(rigContainer);
		Action<RigContainer> action = this.playerEnteredCollection;
		if (action == null)
		{
			return;
		}
		action.Invoke(rigContainer);
	}

	// Token: 0x06004EDB RID: 20187 RVA: 0x00197D18 File Offset: 0x00195F18
	private void OnRigTriggerExit(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(ref rigContainer) || other != rigContainer.HeadCollider || !this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		rigContainer.RigEvents.disableEvent -= new Action<RigContainer>(this.RigDisabled);
		this.containedRigs.Remove(rigContainer);
		Action<RigContainer> action = this.playerLeftCollection;
		if (action == null)
		{
			return;
		}
		action.Invoke(rigContainer);
	}

	// Token: 0x06004EDC RID: 20188 RVA: 0x00197D9C File Offset: 0x00195F9C
	private void RigDisabled(RigContainer rig)
	{
		this.collisionTriggerEvents.ResetColliderMask(rig.HeadCollider);
		this.collisionTriggerEvents.ResetColliderMask(rig.BodyCollider);
	}

	// Token: 0x06004EDD RID: 20189 RVA: 0x00197DC0 File Offset: 0x00195FC0
	private bool HasRig(VRRig rig)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004EDE RID: 20190 RVA: 0x00197E00 File Offset: 0x00196000
	private bool HasRig(NetPlayer player)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Creator == player)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04005D8B RID: 23947
	public readonly List<RigContainer> containedRigs = new List<RigContainer>(10);

	// Token: 0x04005D8C RID: 23948
	[SerializeField]
	private CompositeTriggerEvents collisionTriggerEvents;

	// Token: 0x04005D8D RID: 23949
	public Action<RigContainer> playerEnteredCollection;

	// Token: 0x04005D8E RID: 23950
	public Action<RigContainer> playerLeftCollection;
}
