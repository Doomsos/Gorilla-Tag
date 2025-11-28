using System;
using Liv.Lck.Cosmetics;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class GtLckNetworkCosmeticDependantPlayerIdSupplier : MonoBehaviour, ILckCosmeticDependantPlayerIdSupplier
{
	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06001489 RID: 5257 RVA: 0x00075AB8 File Offset: 0x00073CB8
	// (remove) Token: 0x0600148A RID: 5258 RVA: 0x00075AF0 File Offset: 0x00073CF0
	public event PlayerIdUpdatedEvent PlayerIdUpdated;

	// Token: 0x0600148B RID: 5259 RVA: 0x00075B25 File Offset: 0x00073D25
	public string GetPlayerId()
	{
		return this.vrrig.OwningNetPlayer.UserId;
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x00075B37 File Offset: 0x00073D37
	public void UpdatePlayerId()
	{
		Debug.Log("LCK: GtLckNetworkCosmeticDependantPlayerIdSupplier::UpdatePlayerId, ID is now: " + this.vrrig.OwningNetPlayer.UserId);
		PlayerIdUpdatedEvent playerIdUpdated = this.PlayerIdUpdated;
		if (playerIdUpdated == null)
		{
			return;
		}
		playerIdUpdated.Invoke();
	}

	// Token: 0x04001F28 RID: 7976
	[SerializeField]
	private VRRig vrrig;
}
