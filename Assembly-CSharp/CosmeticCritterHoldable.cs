using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
public abstract class CosmeticCritterHoldable : MonoBehaviour
{
	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x0600255F RID: 9567 RVA: 0x000C81BF File Offset: 0x000C63BF
	// (set) Token: 0x06002560 RID: 9568 RVA: 0x000C81C7 File Offset: 0x000C63C7
	public int OwnerID { get; private set; }

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002561 RID: 9569 RVA: 0x000C81D0 File Offset: 0x000C63D0
	public bool IsLocal
	{
		get
		{
			return this.transferrableObject.IsLocalObject();
		}
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000C81DD File Offset: 0x000C63DD
	public bool OwningPlayerMatches(PhotonMessageInfoWrapped info)
	{
		return this.transferrableObject.targetRig.creator == info.Sender;
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000C81F7 File Offset: 0x000C63F7
	protected virtual CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 2f, 0.5f);
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000C820A File Offset: 0x000C640A
	public void ResetCallLimiter()
	{
		this.callLimiter.Reset();
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000C8218 File Offset: 0x000C6418
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

	// Token: 0x06002566 RID: 9574 RVA: 0x000C82C6 File Offset: 0x000C64C6
	protected virtual void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.callLimiter = this.CreateCallLimiter();
		if (this.IsLocal)
		{
			CosmeticCritterManager.Instance.RegisterLocalHoldable(this);
		}
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000C82F3 File Offset: 0x000C64F3
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
