using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000B98 RID: 2968
internal class OwnershipGaurdHandler : IPunOwnershipCallbacks
{
	// Token: 0x06004954 RID: 18772 RVA: 0x001812F5 File Offset: 0x0017F4F5
	static OwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(OwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x06004955 RID: 18773 RVA: 0x00181315 File Offset: 0x0017F515
	internal static void RegisterView(PhotonView view)
	{
		if (view == null || OwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		OwnershipGaurdHandler.gaurdedViews.Add(view);
	}

	// Token: 0x06004956 RID: 18774 RVA: 0x0018133C File Offset: 0x0017F53C
	internal static void RegisterViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGaurdHandler.RegisterView(photonViews[i]);
		}
	}

	// Token: 0x06004957 RID: 18775 RVA: 0x00181361 File Offset: 0x0017F561
	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		OwnershipGaurdHandler.gaurdedViews.Remove(view);
	}

	// Token: 0x06004958 RID: 18776 RVA: 0x0018137C File Offset: 0x0017F57C
	internal static void RemoveViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGaurdHandler.RemoveView(photonViews[i]);
		}
	}

	// Token: 0x06004959 RID: 18777 RVA: 0x001813A4 File Offset: 0x0017F5A4
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!OwnershipGaurdHandler.gaurdedViews.Contains(targetView))
		{
			return;
		}
		if (targetView.IsRoomView)
		{
			if (targetView.Owner != PhotonNetwork.MasterClient)
			{
				targetView.OwnerActorNr = 0;
				targetView.ControllerActorNr = 0;
				return;
			}
		}
		else if (targetView.OwnerActorNr != targetView.CreatorActorNr || targetView.ControllerActorNr != targetView.CreatorActorNr)
		{
			targetView.OwnerActorNr = targetView.CreatorActorNr;
			targetView.ControllerActorNr = targetView.CreatorActorNr;
		}
	}

	// Token: 0x0600495A RID: 18778 RVA: 0x00002789 File Offset: 0x00000989
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x0600495B RID: 18779 RVA: 0x00002789 File Offset: 0x00000989
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x040059E2 RID: 23010
	private static HashSet<PhotonView> gaurdedViews = new HashSet<PhotonView>();

	// Token: 0x040059E3 RID: 23011
	private static readonly OwnershipGaurdHandler callbackInstance = new OwnershipGaurdHandler();
}
