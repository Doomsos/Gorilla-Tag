using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020008E5 RID: 2277
public class SpawnPooledObject : MonoBehaviour
{
	// Token: 0x06003A51 RID: 14929 RVA: 0x00133E2E File Offset: 0x0013202E
	private void Awake()
	{
		if (this._pooledObject == null)
		{
			return;
		}
		this._pooledObjectHash = PoolUtils.GameObjHashCode(this._pooledObject);
	}

	// Token: 0x06003A52 RID: 14930 RVA: 0x00133E50 File Offset: 0x00132050
	public void SpawnObject()
	{
		if (!this.ShouldSpawn())
		{
			return;
		}
		if (this._pooledObject == null || this._spawnLocation == null)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._pooledObjectHash, true);
		gameObject.transform.position = this.SpawnLocation();
		gameObject.transform.rotation = this.SpawnRotation();
		gameObject.transform.localScale = base.transform.lossyScale;
	}

	// Token: 0x06003A53 RID: 14931 RVA: 0x00133ECB File Offset: 0x001320CB
	private Vector3 SpawnLocation()
	{
		return this._spawnLocation.transform.position + this.offset;
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x00133EE8 File Offset: 0x001320E8
	private Quaternion SpawnRotation()
	{
		Quaternion result = this._spawnLocation.transform.rotation;
		if (this.facePlayer)
		{
			result = Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.position - this._spawnLocation.transform.position);
		}
		if (this.upright)
		{
			result.eulerAngles = new Vector3(0f, result.eulerAngles.y, 0f);
		}
		return result;
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x00133F68 File Offset: 0x00132168
	private bool ShouldSpawn()
	{
		return Random.Range(0, 100) < this.chanceToSpawn;
	}

	// Token: 0x0400498B RID: 18827
	[SerializeField]
	private Transform _spawnLocation;

	// Token: 0x0400498C RID: 18828
	[SerializeField]
	private GameObject _pooledObject;

	// Token: 0x0400498D RID: 18829
	[FormerlySerializedAs("_offset")]
	public Vector3 offset;

	// Token: 0x0400498E RID: 18830
	[FormerlySerializedAs("_upright")]
	public bool upright;

	// Token: 0x0400498F RID: 18831
	[FormerlySerializedAs("_facePlayer")]
	public bool facePlayer;

	// Token: 0x04004990 RID: 18832
	[FormerlySerializedAs("_chanceToSpawn")]
	[Range(0f, 100f)]
	public int chanceToSpawn = 100;

	// Token: 0x04004991 RID: 18833
	private int _pooledObjectHash;
}
