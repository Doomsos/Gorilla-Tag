using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005E5 RID: 1509
[NetworkBehaviourWeaved(337)]
public class FlockingManager : NetworkComponent
{
	// Token: 0x06002609 RID: 9737 RVA: 0x000CB6F4 File Offset: 0x000C98F4
	protected override void Awake()
	{
		base.Awake();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			Flocking[] componentsInChildren = gameObject.GetComponentsInChildren<Flocking>(false);
			FlockingManager.FishArea fishArea = new FlockingManager.FishArea();
			fishArea.id = gameObject.name;
			fishArea.colliders = gameObject.GetComponentsInChildren<BoxCollider>();
			fishArea.colliderCenter = fishArea.colliders[0].bounds.center;
			fishArea.fishList.AddRange(componentsInChildren);
			fishArea.zoneBasedObject = gameObject.GetComponent<ZoneBasedObject>();
			this.areaToWaypointDict[fishArea.id] = Vector3.zero;
			Flocking[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FishArea = fishArea;
			}
			this.fishAreaList.Add(fishArea);
			this.allFish.AddRange(fishArea.fishList);
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				component.OnProjectileTriggerExit += this.ProjectileHitExit;
			}
			else
			{
				Debug.LogError("Needs SlingshotProjectileHitNotifier added to each fish area");
			}
		}
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x0003436B File Offset: 0x0003256B
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x000CB84C File Offset: 0x000C9A4C
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.fishAreaList.Clear();
		this.areaToWaypointDict.Clear();
		this.allFish.Clear();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerExit -= this.ProjectileHitExit;
				component.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
			}
		}
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x000CB8F4 File Offset: 0x000C9AF4
	private void Update()
	{
		if (Random.Range(0, 10000) < 50)
		{
			foreach (FlockingManager.FishArea fishArea in this.fishAreaList)
			{
				if (fishArea.zoneBasedObject != null)
				{
					fishArea.zoneBasedObject.gameObject.SetActive(fishArea.zoneBasedObject.IsLocalPlayerInZone());
				}
				fishArea.nextWaypoint = this.GetRandomPointInsideCollider(fishArea);
				this.areaToWaypointDict[fishArea.id] = fishArea.nextWaypoint;
				Debug.DrawLine(fishArea.nextWaypoint, Vector3.forward * 5f, Color.magenta);
			}
		}
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x000CB9C0 File Offset: 0x000C9BC0
	public Vector3 GetRandomPointInsideCollider(FlockingManager.FishArea fishArea)
	{
		int num = Random.Range(0, fishArea.colliders.Length);
		BoxCollider boxCollider = fishArea.colliders[num];
		Vector3 vector = boxCollider.size / 2f;
		Vector3 vector2;
		vector2..ctor(Random.Range(-vector.x, vector.x), Random.Range(-vector.y, vector.y), Random.Range(-vector.z, vector.z));
		return boxCollider.transform.TransformPoint(vector2);
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x000CBA40 File Offset: 0x000C9C40
	public bool IsInside(Vector3 point, FlockingManager.FishArea fish)
	{
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			vector -= center;
			Vector3 size = boxCollider.size;
			if (Mathf.Abs(vector.x) < size.x / 2f && Mathf.Abs(vector.y) < size.y / 2f && Mathf.Abs(vector.z) < size.z / 2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000CBADC File Offset: 0x000C9CDC
	public Vector3 RestrictPointToArea(Vector3 point, FlockingManager.FishArea fish)
	{
		Vector3 result = default(Vector3);
		float num = float.MaxValue;
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			Vector3 vector2 = vector - center;
			Vector3 size = boxCollider.size;
			float num2 = size.x / 2f;
			float num3 = size.y / 2f;
			float num4 = size.z / 2f;
			if (Mathf.Abs(vector2.x) < num2 && Mathf.Abs(vector2.y) < num3 && Mathf.Abs(vector2.z) < num4)
			{
				return point;
			}
			Vector3 vector3;
			vector3..ctor(center.x - num2, center.y - num3, center.z - num4);
			Vector3 vector4;
			vector4..ctor(center.x + num2, center.y + num3, center.z + num4);
			Vector3 vector5;
			vector5..ctor(Mathf.Clamp(vector.x, vector3.x, vector4.x), Mathf.Clamp(vector.y, vector3.y, vector4.y), Mathf.Clamp(vector.z, vector3.z, vector4.z));
			float num5 = Vector3.Distance(vector, vector5);
			if (num5 < num)
			{
				num = num5;
				if (num5 > 1f)
				{
					Vector3 vector6 = Vector3.Normalize(vector - vector5);
					result = boxCollider.transform.TransformPoint(vector5 + vector6 * 1f);
				}
				else
				{
					result = point;
				}
			}
		}
		return result;
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x000CBC8C File Offset: 0x000C9E8C
	private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider1)
	{
		bool isRealFood = projectile.CompareTag(this.foodProjectileTag);
		FlockingManager.FishFood fishFood = new FlockingManager.FishFood
		{
			collider = (collider1 as BoxCollider),
			isRealFood = isRealFood,
			slingshotProjectile = projectile
		};
		UnityAction<FlockingManager.FishFood> unityAction = this.onFoodDetected;
		if (unityAction == null)
		{
			return;
		}
		unityAction.Invoke(fishFood);
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x000CBCD7 File Offset: 0x000C9ED7
	private void ProjectileHitExit(SlingshotProjectile projectile, Collider collider2)
	{
		UnityAction<BoxCollider> unityAction = this.onFoodDestroyed;
		if (unityAction == null)
		{
			return;
		}
		unityAction.Invoke(collider2 as BoxCollider);
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06002612 RID: 9746 RVA: 0x000CBCEF File Offset: 0x000C9EEF
	// (set) Token: 0x06002613 RID: 9747 RVA: 0x000CBD19 File Offset: 0x000C9F19
	[Networked]
	[NetworkedWeaved(0, 337)]
	public unsafe FlockingData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(FlockingData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(FlockingData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000CBD44 File Offset: 0x000C9F44
	public override void WriteDataFusion()
	{
		this.Data = new FlockingData(this.allFish);
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000CBD58 File Offset: 0x000C9F58
	public override void ReadDataFusion()
	{
		for (int i = 0; i < this.Data.count; i++)
		{
			Vector3 syncPos = this.Data.Positions[i];
			Quaternion syncRot = this.Data.Rotations[i];
			this.allFish[i].SetSyncPosRot(syncPos, syncRot);
		}
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x00002789 File Offset: 0x00000989
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000CBDC3 File Offset: 0x000C9FC3
	public static void RegisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Add(obj);
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000CBDD0 File Offset: 0x000C9FD0
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Remove(obj);
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x000CBE1E File Offset: 0x000CA01E
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600261D RID: 9757 RVA: 0x000CBE36 File Offset: 0x000CA036
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040031E6 RID: 12774
	public List<GameObject> fishAreaContainer;

	// Token: 0x040031E7 RID: 12775
	public string foodProjectileTag = "WaterBalloonProjectile";

	// Token: 0x040031E8 RID: 12776
	private Dictionary<string, Vector3> areaToWaypointDict = new Dictionary<string, Vector3>();

	// Token: 0x040031E9 RID: 12777
	private List<FlockingManager.FishArea> fishAreaList = new List<FlockingManager.FishArea>();

	// Token: 0x040031EA RID: 12778
	private List<Flocking> allFish = new List<Flocking>();

	// Token: 0x040031EB RID: 12779
	public UnityAction<FlockingManager.FishFood> onFoodDetected;

	// Token: 0x040031EC RID: 12780
	public UnityAction<BoxCollider> onFoodDestroyed;

	// Token: 0x040031ED RID: 12781
	private bool hasBeenSerialized;

	// Token: 0x040031EE RID: 12782
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();

	// Token: 0x040031EF RID: 12783
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 337)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private FlockingData _Data;

	// Token: 0x020005E6 RID: 1510
	public class FishArea
	{
		// Token: 0x040031F0 RID: 12784
		public string id;

		// Token: 0x040031F1 RID: 12785
		public List<Flocking> fishList = new List<Flocking>();

		// Token: 0x040031F2 RID: 12786
		public Vector3 colliderCenter;

		// Token: 0x040031F3 RID: 12787
		public BoxCollider[] colliders;

		// Token: 0x040031F4 RID: 12788
		public Vector3 nextWaypoint = Vector3.zero;

		// Token: 0x040031F5 RID: 12789
		public ZoneBasedObject zoneBasedObject;
	}

	// Token: 0x020005E7 RID: 1511
	public class FishFood
	{
		// Token: 0x040031F6 RID: 12790
		public BoxCollider collider;

		// Token: 0x040031F7 RID: 12791
		public bool isRealFood;

		// Token: 0x040031F8 RID: 12792
		public SlingshotProjectile slingshotProjectile;
	}
}
