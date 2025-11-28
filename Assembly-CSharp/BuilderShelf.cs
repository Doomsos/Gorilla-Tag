using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005AD RID: 1453
public class BuilderShelf : MonoBehaviour
{
	// Token: 0x060024A9 RID: 9385 RVA: 0x000C5760 File Offset: 0x000C3960
	public void Init()
	{
		this.shelfSlot = 0;
		this.buildPieceSpawnIndex = 0;
		this.spawnCount = 0;
		this.count = 0;
		this.spawnCosts = new List<BuilderResources>(this.buildPieceSpawns.Count);
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
			BuilderPiece component = this.buildPieceSpawns[i].buildPiecePrefab.GetComponent<BuilderPiece>();
			this.spawnCosts.Add(component.cost);
		}
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000C57FB File Offset: 0x000C39FB
	public bool HasOpenSlot()
	{
		return this.shelfSlot < this.count;
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000C580C File Offset: 0x000C3A0C
	public void BuildNextPiece(BuilderTable table)
	{
		if (!this.HasOpenSlot())
		{
			return;
		}
		BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
		BuilderResources resources = this.spawnCosts[this.buildPieceSpawnIndex];
		while (!table.HasEnoughUnreservedResources(resources) && this.buildPieceSpawnIndex < this.buildPieceSpawns.Count - 1)
		{
			int num = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num;
			this.spawnCount = 0;
			this.buildPieceSpawnIndex++;
			buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
			resources = this.spawnCosts[this.buildPieceSpawnIndex];
		}
		if (!table.HasEnoughUnreservedResources(resources))
		{
			int num2 = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num2;
			this.spawnCount = 0;
			return;
		}
		int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
		int materialType = string.IsNullOrEmpty(buildPieceSpawn.materialID) ? -1 : buildPieceSpawn.materialID.GetHashCode();
		Vector3 position;
		Quaternion rotation;
		this.GetSpawnLocation(this.shelfSlot, buildPieceSpawn, out position, out rotation);
		int pieceId = table.CreatePieceId();
		table.CreatePiece(staticHash, pieceId, position, rotation, materialType, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
		this.spawnCount++;
		this.shelfSlot++;
		if (this.spawnCount >= buildPieceSpawn.count)
		{
			this.buildPieceSpawnIndex++;
			this.spawnCount = 0;
		}
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000C5988 File Offset: 0x000C3B88
	public void InitCount()
	{
		this.count = 0;
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
		}
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000C59D0 File Offset: 0x000C3BD0
	public void BuildItems(BuilderTable table)
	{
		int num = 0;
		this.InitCount();
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[i];
			if (buildPieceSpawn != null && buildPieceSpawn.count != 0)
			{
				int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
				int materialType = string.IsNullOrEmpty(buildPieceSpawn.materialID) ? -1 : buildPieceSpawn.materialID.GetHashCode();
				int num2 = 0;
				while (num2 < buildPieceSpawn.count && num < this.count)
				{
					Vector3 position;
					Quaternion rotation;
					this.GetSpawnLocation(num, buildPieceSpawn, out position, out rotation);
					int pieceId = table.CreatePieceId();
					table.CreatePiece(staticHash, pieceId, position, rotation, materialType, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
					num++;
					num2++;
				}
			}
		}
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000C5A9C File Offset: 0x000C3C9C
	public void GetSpawnLocation(int slot, BuilderShelf.BuildPieceSpawn spawn, out Vector3 spawnPosition, out Quaternion spawnRotation)
	{
		if (this.center == null)
		{
			this.center = base.transform;
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		BuilderPiece component = spawn.buildPiecePrefab.GetComponent<BuilderPiece>();
		if (component != null)
		{
			vector = component.desiredShelfOffset;
			vector2 = component.desiredShelfRotationOffset;
		}
		spawnRotation = this.center.rotation * Quaternion.Euler(vector2);
		float num = (float)slot * this.separation - (float)(this.count - 1) * this.separation / 2f;
		spawnPosition = this.center.position + this.center.rotation * (spawn.localAxis * num + vector);
	}

	// Token: 0x0400302E RID: 12334
	private int count;

	// Token: 0x0400302F RID: 12335
	public float separation;

	// Token: 0x04003030 RID: 12336
	public Transform center;

	// Token: 0x04003031 RID: 12337
	public List<BuilderShelf.BuildPieceSpawn> buildPieceSpawns;

	// Token: 0x04003032 RID: 12338
	private List<BuilderResources> spawnCosts;

	// Token: 0x04003033 RID: 12339
	private int shelfSlot;

	// Token: 0x04003034 RID: 12340
	private int buildPieceSpawnIndex;

	// Token: 0x04003035 RID: 12341
	private int spawnCount;

	// Token: 0x020005AE RID: 1454
	[Serializable]
	public class BuildPieceSpawn
	{
		// Token: 0x04003036 RID: 12342
		public GameObject buildPiecePrefab;

		// Token: 0x04003037 RID: 12343
		public string materialID;

		// Token: 0x04003038 RID: 12344
		public int count = 1;

		// Token: 0x04003039 RID: 12345
		public Vector3 localAxis = Vector3.right;

		// Token: 0x0400303A RID: 12346
		[Tooltip("Optional Editor Visual")]
		public Mesh previewMesh;
	}
}
