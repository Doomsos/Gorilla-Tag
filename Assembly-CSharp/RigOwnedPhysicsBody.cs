using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000BC9 RID: 3017
public class RigOwnedPhysicsBody : MonoBehaviour
{
	// Token: 0x06004ACA RID: 19146 RVA: 0x001876C0 File Offset: 0x001858C0
	private void Awake()
	{
		this.hasTransformView = (this.transformView != null);
		this.hasRigidbodyView = (this.rigidbodyView != null);
		if (!this.hasTransformView && !this.hasRigidbodyView && this.otherComponents.Length == 0)
		{
			GTDev.LogError<string>("RigOwnedPhysicsBody has nothing to do! No TransformView, RigidbodyView, or otherComponents", null);
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.transform.parent = null;
				return;
			}
			if (this.hasRigidbodyView)
			{
				this.rigidbodyView.transform.parent = null;
			}
		}
	}

	// Token: 0x06004ACB RID: 19147 RVA: 0x00187750 File Offset: 0x00185950
	private void OnEnable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		NetworkSystem.Instance.OnJoinedRoomEvent += new Action(this.OnNetConnect);
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnNetDisconnect);
		if (!this.hasRig)
		{
			this.rig = base.GetComponentInParent<VRRig>();
			this.hasRig = (this.rig != null);
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(true);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(true);
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnNetConnect();
			return;
		}
		this.OnNetDisconnect();
	}

	// Token: 0x06004ACC RID: 19148 RVA: 0x00187828 File Offset: 0x00185A28
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= new Action(this.OnNetConnect);
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnNetDisconnect);
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(false);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(false);
			}
		}
		this.OnNetDisconnect();
	}

	// Token: 0x06004ACD RID: 19149 RVA: 0x001878BC File Offset: 0x00185ABC
	private void OnNetConnect()
	{
		if (this.hasTransformView)
		{
			this.transformView.enabled = this.hasRig;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = this.hasRig;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.hasRig;
		}
		if (!this.hasRig)
		{
			return;
		}
		PhotonView getView = this.rig.netView.GetView;
		List<Component> observedComponents = getView.ObservedComponents;
		if (this.hasTransformView)
		{
			this.transformView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.transformView))
			{
				observedComponents.Add(this.transformView);
			}
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.rigidbodyView))
			{
				observedComponents.Add(this.rigidbodyView);
			}
		}
		foreach (MonoBehaviourPun monoBehaviourPun in this.otherComponents)
		{
			if (!observedComponents.Contains(monoBehaviourPun))
			{
				observedComponents.Add(monoBehaviourPun);
			}
		}
	}

	// Token: 0x06004ACE RID: 19150 RVA: 0x001879D4 File Offset: 0x00185BD4
	private void OnNetDisconnect()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.hasTransformView)
		{
			this.transformView.enabled = false;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = false;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		if (!this.hasRig || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		List<Component> observedComponents = this.rig.netView.GetView.ObservedComponents;
		if (this.hasTransformView)
		{
			observedComponents.Remove(this.transformView);
		}
		if (this.hasRigidbodyView)
		{
			observedComponents.Remove(this.rigidbodyView);
		}
		foreach (MonoBehaviourPun monoBehaviourPun in this.otherComponents)
		{
			observedComponents.Remove(monoBehaviourPun);
		}
	}

	// Token: 0x04005AEE RID: 23278
	private VRRig rig;

	// Token: 0x04005AEF RID: 23279
	public RigOwnedTransformView transformView;

	// Token: 0x04005AF0 RID: 23280
	private bool hasTransformView;

	// Token: 0x04005AF1 RID: 23281
	public RigOwnedRigidbodyView rigidbodyView;

	// Token: 0x04005AF2 RID: 23282
	private bool hasRigidbodyView;

	// Token: 0x04005AF3 RID: 23283
	public MonoBehaviourPun[] otherComponents;

	// Token: 0x04005AF4 RID: 23284
	private bool hasRig;

	// Token: 0x04005AF5 RID: 23285
	[Tooltip("To make a rigidbody unaffected by the movement of the holdable part, put this script on the holdable, make the RigOwnedRigidbodyView a child of it, and check this box")]
	[SerializeField]
	private bool detachTransform;
}
