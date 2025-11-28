using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000787 RID: 1927
[DisallowMultipleComponent]
public class GorillaHandSocket : MonoBehaviour
{
	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x06003280 RID: 12928 RVA: 0x001105A0 File Offset: 0x0010E7A0
	public GorillaHandNode attachedHand
	{
		get
		{
			return this._attachedHand;
		}
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x06003281 RID: 12929 RVA: 0x001105A8 File Offset: 0x0010E7A8
	public bool inUse
	{
		get
		{
			return this._inUse;
		}
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x001105B0 File Offset: 0x0010E7B0
	public static bool FetchSocket(Collider collider, out GorillaHandSocket socket)
	{
		return GorillaHandSocket.gColliderToSocket.TryGetValue(collider, ref socket);
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x001105BE File Offset: 0x0010E7BE
	public bool CanAttach()
	{
		return !this._inUse && this._sinceSocketStateChange.HasElapsed(this.attachCooldown, true);
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x001105DC File Offset: 0x0010E7DC
	public void Attach(GorillaHandNode hand)
	{
		if (!this.CanAttach())
		{
			return;
		}
		if (hand == null)
		{
			return;
		}
		hand.attachedToSocket = this;
		this._attachedHand = hand;
		this._inUse = true;
		this.OnHandAttach();
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x0011060C File Offset: 0x0010E80C
	public void Detach()
	{
		GorillaHandNode gorillaHandNode;
		this.Detach(out gorillaHandNode);
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x00110624 File Offset: 0x0010E824
	public void Detach(out GorillaHandNode hand)
	{
		if (this._inUse)
		{
			this._inUse = false;
		}
		if (this._attachedHand == null)
		{
			hand = null;
			return;
		}
		hand = this._attachedHand;
		hand.attachedToSocket = null;
		this._attachedHand = null;
		this.OnHandDetach();
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnHandAttach()
	{
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnHandDetach()
	{
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x0011067A File Offset: 0x0010E87A
	protected virtual void OnUpdateAttached()
	{
		this._attachedHand.transform.position = base.transform.position;
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x00110697 File Offset: 0x0010E897
	private void OnEnable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.TryAdd(this.collider, this);
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x001106BA File Offset: 0x0010E8BA
	private void OnDisable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.Remove(this.collider);
	}

	// Token: 0x0600328C RID: 12940 RVA: 0x001106DC File Offset: 0x0010E8DC
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x001106E4 File Offset: 0x0010E8E4
	private void FixedUpdate()
	{
		if (!this._inUse)
		{
			return;
		}
		if (!this._attachedHand)
		{
			return;
		}
		this.OnUpdateAttached();
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x00110704 File Offset: 0x0010E904
	private void Setup()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(UnityTag.GorillaHandSocket);
		base.gameObject.SetLayer(UnityLayer.GorillaHandSocket);
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x040040DA RID: 16602
	public Collider collider;

	// Token: 0x040040DB RID: 16603
	public float attachCooldown = 0.5f;

	// Token: 0x040040DC RID: 16604
	public HandSocketConstraint constraint;

	// Token: 0x040040DD RID: 16605
	[NonSerialized]
	private GorillaHandNode _attachedHand;

	// Token: 0x040040DE RID: 16606
	[NonSerialized]
	private bool _inUse;

	// Token: 0x040040DF RID: 16607
	[NonSerialized]
	private TimeSince _sinceSocketStateChange;

	// Token: 0x040040E0 RID: 16608
	private static readonly Dictionary<Collider, GorillaHandSocket> gColliderToSocket = new Dictionary<Collider, GorillaHandSocket>(64);
}
