using System;
using System.Runtime.CompilerServices;
using KID.Model;
using UnityEngine;

// Token: 0x02000A0A RID: 2570
public class GetPlayerData_Data
{
	// Token: 0x060041B0 RID: 16816 RVA: 0x0015CCCC File Offset: 0x0015AECC
	public GetPlayerData_Data(GetSessionResponseType type, GetPlayerDataResponse response)
	{
		this.responseType = type;
		if (response == null)
		{
			if (this.responseType == GetSessionResponseType.OK)
			{
				this.responseType = GetSessionResponseType.ERROR;
				Debug.LogError("[KID::GET_PLAYER_DATA_DATA] Incoming [GetPlayerDataResponse] is NULL");
			}
			return;
		}
		this.AgeStatus = response.AgeStatus;
		this.status = response.Status;
		if (this.status != null)
		{
			this.session = new TMPSession(response.Session, response.DefaultSession, response.Age, this.status.Value);
			this.session.SetOptInPermissions(response.Permissions);
			Debug.Log("[KID::GET_PLAYER_DATA_DATA::OptInRefactor] Setting Opt-in Permissions: " + string.Join(", ", this.session.GetOptedInPermissions()));
		}
		this.HasConfirmedSetup = response.HasConfirmedSetup;
	}

	// Token: 0x0400529F RID: 21151
	public readonly AgeStatusType? AgeStatus;

	// Token: 0x040052A0 RID: 21152
	public readonly GetSessionResponseType responseType;

	// Token: 0x040052A1 RID: 21153
	public readonly SessionStatus? status;

	// Token: 0x040052A2 RID: 21154
	public readonly TMPSession session;

	// Token: 0x040052A3 RID: 21155
	[Nullable(new byte[]
	{
		2,
		0
	})]
	public readonly string[] OptInPermissions;

	// Token: 0x040052A4 RID: 21156
	public readonly bool HasConfirmedSetup;
}
