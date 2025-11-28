using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007AE RID: 1966
public class GorillaParent : MonoBehaviour
{
	// Token: 0x0600338F RID: 13199 RVA: 0x00115EBC File Offset: 0x001140BC
	public void Awake()
	{
		if (GorillaParent.instance == null)
		{
			GorillaParent.instance = this;
			GorillaParent.hasInstance = true;
			return;
		}
		if (GorillaParent.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x00115EF7 File Offset: 0x001140F7
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x00115F16 File Offset: 0x00114116
	public static void ReplicatedClientReady()
	{
		GorillaParent.replicatedClientReady = true;
		Action action = GorillaParent.onReplicatedClientReady;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x00115F2D File Offset: 0x0011412D
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action.Invoke();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x040041F7 RID: 16887
	public GameObject tagUI;

	// Token: 0x040041F8 RID: 16888
	public GameObject playerParent;

	// Token: 0x040041F9 RID: 16889
	public GameObject vrrigParent;

	// Token: 0x040041FA RID: 16890
	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	// Token: 0x040041FB RID: 16891
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040041FC RID: 16892
	public List<VRRig> vrrigs;

	// Token: 0x040041FD RID: 16893
	public Dictionary<NetPlayer, VRRig> vrrigDict = new Dictionary<NetPlayer, VRRig>();

	// Token: 0x040041FE RID: 16894
	private int i;

	// Token: 0x040041FF RID: 16895
	private static bool replicatedClientReady;

	// Token: 0x04004200 RID: 16896
	private static Action onReplicatedClientReady;
}
