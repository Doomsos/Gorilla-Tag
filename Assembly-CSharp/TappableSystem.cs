using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000836 RID: 2102
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x0600375A RID: 14170 RVA: 0x0012A338 File Offset: 0x00128538
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnTapRPC");
		if (key < 0 || key >= this._instances.Count || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		this._instances[key].OnTapLocal(tapStrength, Time.time, new PhotonMessageInfoWrapped(info));
	}
}
