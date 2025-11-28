using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200038F RID: 911
[RequireComponent(typeof(PhotonView))]
public class MaterialCyclerNetworked : MonoBehaviour
{
	// Token: 0x17000221 RID: 545
	// (get) Token: 0x060015BA RID: 5562 RVA: 0x0007A153 File Offset: 0x00078353
	public float SyncTimeOut
	{
		get
		{
			return this.syncTimeOut;
		}
	}

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x060015BB RID: 5563 RVA: 0x0007A15C File Offset: 0x0007835C
	// (remove) Token: 0x060015BC RID: 5564 RVA: 0x0007A194 File Offset: 0x00078394
	public event Action<int, int3> OnSynchronize;

	// Token: 0x060015BD RID: 5565 RVA: 0x0007A1C9 File Offset: 0x000783C9
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x0007A1D8 File Offset: 0x000783D8
	public void Synchronize(int materialIndex, Color c)
	{
		if (!this.masterClientOnly || PhotonNetwork.IsMasterClient)
		{
			int num = Mathf.CeilToInt(c.r * 9f);
			int num2 = Mathf.CeilToInt(c.g * 9f);
			int num3 = Mathf.CeilToInt(c.b * 9f);
			int num4 = num | num2 << 8 | num3 << 16;
			this.photonView.RPC("RPC_SynchronizePacked", 1, new object[]
			{
				materialIndex,
				num4
			});
		}
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x0007A25C File Offset: 0x0007845C
	[PunRPC]
	public void RPC_SynchronizePacked(int index, int colourPacked, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPC_SynchronizePacked");
		RigContainer rigContainer;
		if (this.OnSynchronize == null || (this.masterClientOnly && !info.Sender.IsMasterClient) || !VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.IsPositionInRange(base.transform.position, 5f) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 21, info.SentServerTime))
		{
			return;
		}
		int num = colourPacked & 255;
		int num2 = colourPacked >> 8 & 255;
		int num3 = colourPacked >> 16 & 255;
		num = Mathf.Clamp(num, 0, 9);
		num2 = Mathf.Clamp(num2, 0, 9);
		num3 = Mathf.Clamp(num3, 0, 9);
		this.OnSynchronize.Invoke(index, new int3(num, num2, num3));
	}

	// Token: 0x04002020 RID: 8224
	[SerializeField]
	private float syncTimeOut = 1f;

	// Token: 0x04002021 RID: 8225
	private PhotonView photonView;

	// Token: 0x04002022 RID: 8226
	[SerializeField]
	private bool masterClientOnly;
}
