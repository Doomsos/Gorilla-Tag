using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
public abstract class CosmeticCritterHoldable : MonoBehaviour
{
	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x0600255F RID: 9567 RVA: 0x000C81DF File Offset: 0x000C63DF
	// (set) Token: 0x06002560 RID: 9568 RVA: 0x000C81E7 File Offset: 0x000C63E7
	public int OwnerID { get; private set; }

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002561 RID: 9569 RVA: 0x000C81F0 File Offset: 0x000C63F0
	public bool IsLocal
	{
		get
		{
			return this.transferrableObject.IsLocalObject();
		}
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000C81FD File Offset: 0x000C63FD
	public bool OwningPlayerMatches(PhotonMessageInfoWrapped info)
	{
		return this.transferrableObject.targetRig.creator == info.Sender;
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000C8217 File Offset: 0x000C6417
	protected virtual CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 2f, 0.5f);
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000C822A File Offset: 0x000C642A
	public void ResetCallLimiter()
	{
		this.callLimiter.Reset();
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000C8238 File Offset: 0x000C6438
	private void TrySetID()
	{
		if (this.IsLocal)
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.transferrableObject.targetRig != null && this.transferrableObject.targetRig.creator != null)
		{
			string userId = this.transferrableObject.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000C82E6 File Offset: 0x000C64E6
	protected virtual void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.callLimiter = this.CreateCallLimiter();
		if (this.IsLocal)
		{
			CosmeticCritterManager.Instance.RegisterLocalHoldable(this);
		}
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000C8313 File Offset: 0x000C6513
	protected virtual void OnEnable()
	{
		this.TrySetID();
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnDisable()
	{
	}

	// Token: 0x040030FC RID: 12540
	protected TransferrableObject transferrableObject;

	// Token: 0x040030FE RID: 12542
	protected CallLimiter callLimiter;
}
