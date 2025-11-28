using System;
using TMPro;
using UnityEngine;

// Token: 0x02000740 RID: 1856
public class GRUIEmployeeBadgeDispenser : MonoBehaviour
{
	// Token: 0x06002FE8 RID: 12264 RVA: 0x00105F46 File Offset: 0x00104146
	public void Setup(GhostReactor reactor, int employeeIndex)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x00105F50 File Offset: 0x00104150
	public void Refresh()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.actorNr);
		if (player != null && player.InRoom)
		{
			this.playerName.text = player.SanitizedNickName;
			if (this.idBadge != null)
			{
				this.idBadge.RefreshText(player);
				return;
			}
		}
		else
		{
			this.playerName.text = "";
		}
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x00105FB8 File Offset: 0x001041B8
	public void CreateBadge(NetPlayer player, GameEntityManager entityManager)
	{
		if (entityManager.IsAuthority())
		{
			entityManager.RequestCreateItem(this.idBadgePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)(player.ActorNumber * 100 + this.index));
		}
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x0010600B File Offset: 0x0010420B
	public Transform GetSpawnMarker()
	{
		return this.spawnLocation;
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x00106013 File Offset: 0x00104213
	public bool IsDispenserForBadge(GRBadge badge)
	{
		return badge == this.idBadge;
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x00106021 File Offset: 0x00104221
	public Vector3 GetSpawnPosition()
	{
		return this.spawnLocation.position;
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x0010602E File Offset: 0x0010422E
	public Quaternion GetSpawnRotation()
	{
		return this.spawnLocation.rotation;
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x0010603B File Offset: 0x0010423B
	public void ClearBadge()
	{
		this.actorNr = -1;
		this.idBadge = null;
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x0010604C File Offset: 0x0010424C
	public void AttachIDBadge(GRBadge linkedBadge, NetPlayer _player)
	{
		this.actorNr = ((_player == null) ? -1 : _player.ActorNumber);
		this.idBadge = linkedBadge;
		this.playerName.text = ((_player == null) ? null : _player.SanitizedNickName);
		this.idBadge.Setup(_player, this.index);
	}

	// Token: 0x04003EDA RID: 16090
	[SerializeField]
	private TMP_Text msg;

	// Token: 0x04003EDB RID: 16091
	[SerializeField]
	private TMP_Text playerName;

	// Token: 0x04003EDC RID: 16092
	[SerializeField]
	private Transform spawnLocation;

	// Token: 0x04003EDD RID: 16093
	[SerializeField]
	private GameEntity idBadgePrefab;

	// Token: 0x04003EDE RID: 16094
	[SerializeField]
	private LayerMask badgeLayerMask;

	// Token: 0x04003EDF RID: 16095
	public int index;

	// Token: 0x04003EE0 RID: 16096
	public int actorNr;

	// Token: 0x04003EE1 RID: 16097
	public GRBadge idBadge;

	// Token: 0x04003EE2 RID: 16098
	private GhostReactor reactor;

	// Token: 0x04003EE3 RID: 16099
	private Coroutine getSpawnedBadgeCoroutine;

	// Token: 0x04003EE4 RID: 16100
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x04003EE5 RID: 16101
	private bool isEmployee;

	// Token: 0x04003EE6 RID: 16102
	private const string GR_DATA_KEY = "GRData";
}
