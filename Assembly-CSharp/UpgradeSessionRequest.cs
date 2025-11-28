using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000A23 RID: 2595
[Serializable]
public class UpgradeSessionRequest : KIDRequestData
{
	// Token: 0x040052F4 RID: 21236
	public List<RequestedPermission> Permissions;
}
