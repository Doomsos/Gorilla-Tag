using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000837 RID: 2103
public class TransferableObjectSpawner : MonoBehaviour
{
	// Token: 0x0600375C RID: 14172 RVA: 0x0012A384 File Offset: 0x00128584
	public void Awake()
	{
		GameObject[] transferrableObjectsToSpawn = this.TransferrableObjectsToSpawn;
		for (int i = 0; i < transferrableObjectsToSpawn.Length; i++)
		{
			TransferrableObject componentInChildren = transferrableObjectsToSpawn[i].GetComponentInChildren<TransferrableObject>();
			if (componentInChildren.IsNotNull())
			{
				this.objectsToSpawn.Add(componentInChildren);
			}
			else
			{
				Debug.LogError("Failed to add object " + componentInChildren.gameObject.name + " - missing a Transferrable object");
			}
		}
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x0012A3E4 File Offset: 0x001285E4
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		foreach (GameObject gameObject in this.TransferrableObjectsToSpawn)
		{
			TransferrableObject componentInChildren = gameObject.GetComponentInChildren<TransferrableObject>();
			if (componentInChildren.IsNull())
			{
				Debug.LogError(string.Concat(new string[]
				{
					base.name,
					" at path ",
					this.GetComponentPath(int.MaxValue),
					" has ",
					gameObject.name,
					" assigned to TransferrableObjectsToSpawn collection, but it does not have a TransferrableObject component.  It will not spawn."
				}));
			}
			else if (componentInChildren.worldShareableInstance == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					base.name,
					" at path ",
					this.GetComponentPath(int.MaxValue),
					" has ",
					gameObject.name,
					" assigned to TransferrableObjectsToSpawn collection, but it's worldShareableInstance is null."
				}));
			}
		}
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x0012A4C7 File Offset: 0x001286C7
	public void Update()
	{
		if (this.spawnTrigger == TransferableObjectSpawner.SpawnTrigger.Timer && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && PhotonNetwork.Time > this.lastSpawnTime + this.SpawnDelay)
		{
			this.SpawnTransferrableObject();
			this.lastSpawnTime = PhotonNetwork.Time;
		}
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0012A504 File Offset: 0x00128704
	private bool SpawnOnGround()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.spawnRadius, Vector3.down), ref raycastHit, 3f, this.groundRaycastMask))
		{
			this.spawnPosition = raycastHit.point;
			this.spawnRotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
			return true;
		}
		return false;
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x0012A580 File Offset: 0x00128780
	private void SpawnAtCurrentLocation()
	{
		this.spawnPosition = base.transform.position;
		this.spawnRotation = base.transform.rotation;
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x0012A5A4 File Offset: 0x001287A4
	public void SpawnTransferrableObject()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		TransferableObjectSpawner.SpawnMode spawnMode = this.spawnMode;
		if (spawnMode != TransferableObjectSpawner.SpawnMode.OnGround)
		{
			if (spawnMode != TransferableObjectSpawner.SpawnMode.AtCurrentTransform)
			{
				return;
			}
			this.SpawnAtCurrentLocation();
		}
		else if (!this.SpawnOnGround())
		{
			return;
		}
		TransferrableObject transferrableObject = null;
		int num = 0;
		foreach (TransferrableObject transferrableObject2 in this.objectsToSpawn)
		{
			if (!transferrableObject2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					transferrableObject = transferrableObject2;
				}
			}
		}
		if (transferrableObject != null)
		{
			if (!transferrableObject.IsLocalOwnedWorldShareable)
			{
				transferrableObject.WorldShareableRequestOwnership();
			}
			if (transferrableObject.worldShareableInstance != null)
			{
				transferrableObject.transform.SetPositionAndRotation(this.spawnPosition, this.spawnRotation);
				transferrableObject.worldShareableInstance.SetWillTeleport();
				return;
			}
			Debug.LogError("WorldShareableInstance for " + transferrableObject.name + " is null");
		}
	}

	// Token: 0x040046B5 RID: 18101
	private Vector3 spawnPosition = Vector3.zero;

	// Token: 0x040046B6 RID: 18102
	private Quaternion spawnRotation = Quaternion.identity;

	// Token: 0x040046B7 RID: 18103
	[SerializeField]
	private GameObject[] TransferrableObjectsToSpawn;

	// Token: 0x040046B8 RID: 18104
	private List<TransferrableObject> objectsToSpawn = new List<TransferrableObject>();

	// Token: 0x040046B9 RID: 18105
	[SerializeField]
	private TransferableObjectSpawner.SpawnMode spawnMode;

	// Token: 0x040046BA RID: 18106
	[SerializeField]
	private TransferableObjectSpawner.SpawnTrigger spawnTrigger;

	// Token: 0x040046BB RID: 18107
	[SerializeField]
	private double SpawnDelay = 5.0;

	// Token: 0x040046BC RID: 18108
	private double lastSpawnTime;

	// Token: 0x040046BD RID: 18109
	[SerializeField]
	private LayerMask groundRaycastMask = LayerMask.NameToLayer("Gorilla Object");

	// Token: 0x040046BE RID: 18110
	[SerializeField]
	private float spawnRadius = 0.5f;

	// Token: 0x02000838 RID: 2104
	private enum SpawnMode
	{
		// Token: 0x040046C0 RID: 18112
		OnGround,
		// Token: 0x040046C1 RID: 18113
		AtCurrentTransform
	}

	// Token: 0x02000839 RID: 2105
	private enum SpawnTrigger
	{
		// Token: 0x040046C3 RID: 18115
		Timer
	}
}
