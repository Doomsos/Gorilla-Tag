using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000BA1 RID: 2977
public class SteamAuthTicket : IDisposable
{
	// Token: 0x0600497E RID: 18814 RVA: 0x0018160E File Offset: 0x0017F80E
	private SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		this.m_hAuthTicket = hAuthTicket;
	}

	// Token: 0x0600497F RID: 18815 RVA: 0x0018161D File Offset: 0x0017F81D
	public static implicit operator SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		return new SteamAuthTicket(hAuthTicket);
	}

	// Token: 0x06004980 RID: 18816 RVA: 0x00181628 File Offset: 0x0017F828
	~SteamAuthTicket()
	{
		this.Dispose();
	}

	// Token: 0x06004981 RID: 18817 RVA: 0x00181654 File Offset: 0x0017F854
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		if (this.m_hAuthTicket != HAuthTicket.Invalid)
		{
			try
			{
				SteamUser.CancelAuthTicket(this.m_hAuthTicket);
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning("Failed to invalidate a Steam auth ticket because the Steam API was shut down. Was it supposed to be disposed of sooner?");
			}
			this.m_hAuthTicket = HAuthTicket.Invalid;
		}
	}

	// Token: 0x04005A05 RID: 23045
	private HAuthTicket m_hAuthTicket;
}
