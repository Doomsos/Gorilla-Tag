using System;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;

// Token: 0x02000BCA RID: 3018
public class PUNErrorLogging : MonoBehaviour
{
	// Token: 0x06004AD0 RID: 19152 RVA: 0x00187AA8 File Offset: 0x00185CA8
	private void Start()
	{
		PhotonNetwork.InternalEventError = (Action<EventData, Exception>)Delegate.Combine(PhotonNetwork.InternalEventError, new Action<EventData, Exception>(this.PUNError));
		PlayFabTitleDataCache.Instance.GetTitleData("PUNErrorLogging", delegate(string data)
		{
			int num;
			if (!int.TryParse(data, ref num))
			{
				return;
			}
			PUNErrorLogging.LogFlags logFlags = (PUNErrorLogging.LogFlags)num;
			this.m_logSerializeView = logFlags.HasFlag(PUNErrorLogging.LogFlags.SerializeView);
			this.m_logOwnershipTransfer = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipTransfer);
			this.m_logOwnershipRequest = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipRequest);
			this.m_logOwnershipUpdate = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipUpdate);
			this.m_logRPC = logFlags.HasFlag(PUNErrorLogging.LogFlags.RPC);
			this.m_logInstantiate = logFlags.HasFlag(PUNErrorLogging.LogFlags.Instantiate);
			this.m_logDestroy = logFlags.HasFlag(PUNErrorLogging.LogFlags.Destroy);
			this.m_logDestroyPlayer = logFlags.HasFlag(PUNErrorLogging.LogFlags.DestroyPlayer);
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	// Token: 0x06004AD1 RID: 19153 RVA: 0x00187B10 File Offset: 0x00185D10
	private void PUNError(EventData data, Exception exception)
	{
		NetworkSystem.Instance.GetPlayer(data.Sender);
		byte code = data.Code;
		switch (code)
		{
		case 200:
			this.PrintException(exception, this.m_logRPC);
			return;
		case 201:
		case 206:
			this.PrintException(exception, this.m_logSerializeView);
			return;
		case 202:
			this.PrintException(exception, this.m_logInstantiate);
			return;
		case 203:
		case 205:
		case 208:
		case 211:
			break;
		case 204:
			this.PrintException(exception, this.m_logDestroy);
			return;
		case 207:
			this.PrintException(exception, this.m_logDestroyPlayer);
			return;
		case 209:
			this.PrintException(exception, this.m_logOwnershipRequest);
			return;
		case 210:
			this.PrintException(exception, this.m_logOwnershipTransfer);
			return;
		case 212:
			this.PrintException(exception, this.m_logOwnershipUpdate);
			return;
		default:
			if (code == 254)
			{
				this.PrintException(exception, true);
				return;
			}
			break;
		}
		this.PrintException(exception, true);
	}

	// Token: 0x06004AD2 RID: 19154 RVA: 0x00187BFE File Offset: 0x00185DFE
	private void PrintException(Exception e, bool print)
	{
		if (print)
		{
			Debug.LogException(e);
		}
	}

	// Token: 0x04005AF6 RID: 23286
	[SerializeField]
	private bool m_logSerializeView = true;

	// Token: 0x04005AF7 RID: 23287
	[SerializeField]
	private bool m_logOwnershipTransfer = true;

	// Token: 0x04005AF8 RID: 23288
	[SerializeField]
	private bool m_logOwnershipRequest = true;

	// Token: 0x04005AF9 RID: 23289
	[SerializeField]
	private bool m_logOwnershipUpdate = true;

	// Token: 0x04005AFA RID: 23290
	[SerializeField]
	private bool m_logRPC = true;

	// Token: 0x04005AFB RID: 23291
	[SerializeField]
	private bool m_logInstantiate = true;

	// Token: 0x04005AFC RID: 23292
	[SerializeField]
	private bool m_logDestroy = true;

	// Token: 0x04005AFD RID: 23293
	[SerializeField]
	private bool m_logDestroyPlayer = true;

	// Token: 0x02000BCB RID: 3019
	[Flags]
	private enum LogFlags
	{
		// Token: 0x04005AFF RID: 23295
		SerializeView = 1,
		// Token: 0x04005B00 RID: 23296
		OwnershipTransfer = 2,
		// Token: 0x04005B01 RID: 23297
		OwnershipRequest = 4,
		// Token: 0x04005B02 RID: 23298
		OwnershipUpdate = 8,
		// Token: 0x04005B03 RID: 23299
		RPC = 16,
		// Token: 0x04005B04 RID: 23300
		Instantiate = 32,
		// Token: 0x04005B05 RID: 23301
		Destroy = 64,
		// Token: 0x04005B06 RID: 23302
		DestroyPlayer = 128
	}
}
