using System;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020005E2 RID: 1506
public class Flocking : MonoBehaviour
{
	// Token: 0x170003CF RID: 975
	// (get) Token: 0x060025F0 RID: 9712 RVA: 0x000CADCD File Offset: 0x000C8FCD
	// (set) Token: 0x060025F1 RID: 9713 RVA: 0x000CADD5 File Offset: 0x000C8FD5
	public FlockingManager.FishArea FishArea { get; set; }

	// Token: 0x060025F2 RID: 9714 RVA: 0x000CADDE File Offset: 0x000C8FDE
	private void Awake()
	{
		this.manager = base.GetComponentInParent<FlockingManager>();
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000CADEC File Offset: 0x000C8FEC
	private void Start()
	{
		this.speed = Random.Range(this.minSpeed, this.maxSpeed);
		this.fishState = Flocking.FishState.patrol;
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000CAE0C File Offset: 0x000C900C
	private void OnDisable()
	{
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Remove(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Remove(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.UnregisterFlocking(this);
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000CAE70 File Offset: 0x000C9070
	public void InvokeUpdate()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		this.AvoidPlayerHands();
		this.MaybeTurn();
		switch (this.fishState)
		{
		case Flocking.FishState.flock:
			this.Flock(this.FishArea.nextWaypoint);
			this.SwitchState(Flocking.FishState.patrol);
			break;
		case Flocking.FishState.patrol:
			if (Random.Range(0, 10) < 2)
			{
				this.SwitchState(Flocking.FishState.flock);
			}
			break;
		case Flocking.FishState.followFood:
			if (this.isTurning)
			{
				return;
			}
			if (this.isRealFood)
			{
				if ((double)Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFoodStopDistance)
				{
					this.FollowFood();
				}
				else
				{
					this.followingFood = false;
					this.Flock(this.projectileGameObject.transform.position);
					this.feedingTimeStarted += Time.deltaTime;
					if (this.feedingTimeStarted > this.eatFoodDuration)
					{
						this.SwitchState(Flocking.FishState.patrol);
					}
				}
			}
			else if (Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFakeFoodStopDistance)
			{
				this.FollowFood();
			}
			else
			{
				this.followingFood = false;
				this.SwitchState(Flocking.FishState.patrol);
			}
			break;
		}
		if (!this.followingFood)
		{
			base.transform.Translate(0f, 0f, this.speed * Time.deltaTime);
		}
		this.pos = base.transform.position;
		this.rot = base.transform.rotation;
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000CB00C File Offset: 0x000C920C
	private void MaybeTurn()
	{
		if (!this.manager.IsInside(base.transform.position, this.FishArea))
		{
			this.Turn(this.FishArea.colliderCenter);
			if (Vector3.Angle(this.FishArea.colliderCenter - base.transform.position, Vector3.forward) > 5f)
			{
				this.isTurning = true;
				return;
			}
		}
		else
		{
			this.isTurning = false;
		}
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000CB084 File Offset: 0x000C9284
	private void Turn(Vector3 towardPoint)
	{
		this.isTurning = true;
		Quaternion quaternion = Quaternion.LookRotation(towardPoint - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000CB0D7 File Offset: 0x000C92D7
	private void SwitchState(Flocking.FishState state)
	{
		this.fishState = state;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x000CB0E0 File Offset: 0x000C92E0
	private void Flock(Vector3 nextGoal)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		float num = 1f;
		int num2 = 0;
		foreach (Flocking flocking in this.FishArea.fishList)
		{
			if (flocking.gameObject != base.gameObject)
			{
				float num3 = Vector3.Distance(flocking.transform.position, base.transform.position);
				if (num3 <= this.maxNeighbourDistance)
				{
					vector += flocking.transform.position;
					num2++;
					if (num3 < this.flockingAvoidanceDistance)
					{
						vector2 += base.transform.position - flocking.transform.position;
					}
					num += flocking.speed;
				}
			}
		}
		if (num2 > 0)
		{
			this.fishState = Flocking.FishState.flock;
			vector = vector / (float)num2 + (nextGoal - base.transform.position);
			this.speed = num / (float)num2;
			this.speed = Mathf.Clamp(this.speed, this.minSpeed, this.maxSpeed);
			Vector3 vector3 = vector + vector2 - base.transform.position;
			if (vector3 != Vector3.zero)
			{
				Quaternion quaternion = Quaternion.LookRotation(vector3);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
			}
		}
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000CB284 File Offset: 0x000C9484
	private void HandleOnFoodDetected(FlockingManager.FishFood fishFood)
	{
		bool flag = false;
		foreach (BoxCollider boxCollider in this.FishArea.colliders)
		{
			if (fishFood.collider == boxCollider)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.followFood);
		this.feedingTimeStarted = 0f;
		this.projectileGameObject = fishFood.slingshotProjectile.gameObject;
		this.isRealFood = fishFood.isRealFood;
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000CB2F4 File Offset: 0x000C94F4
	private void HandleOnFoodDestroyed(BoxCollider collider)
	{
		bool flag = false;
		foreach (BoxCollider boxCollider in this.FishArea.colliders)
		{
			if (collider == boxCollider)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.patrol);
		this.projectileGameObject = null;
		this.followingFood = false;
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000CB348 File Offset: 0x000C9548
	private void FollowFood()
	{
		this.followingFood = true;
		Quaternion quaternion = Quaternion.LookRotation(this.projectileGameObject.transform.position - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.projectileGameObject.transform.position, this.speed * this.followFoodSpeedMult * Time.deltaTime);
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000CB3E8 File Offset: 0x000C95E8
	private void AvoidPlayerHands()
	{
		foreach (GameObject gameObject in FlockingManager.avoidPoints)
		{
			Vector3 position = gameObject.transform.position;
			if ((base.transform.position - position).IsShorterThan(this.avointPointRadius))
			{
				Vector3 randomPointInsideCollider = this.manager.GetRandomPointInsideCollider(this.FishArea);
				this.Turn(randomPointInsideCollider);
				this.speed = this.avoidHandSpeed;
			}
		}
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000CB480 File Offset: 0x000C9680
	internal void SetSyncPosRot(Vector3 syncPos, Quaternion syncRot)
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		if (this.FishArea == null)
		{
			Debug.LogError("FISH AREA NULL");
		}
		if (syncRot.IsValid())
		{
			this.rot = syncRot;
		}
		float num = 10000f;
		if (syncPos.IsValid(num))
		{
			this.pos = this.manager.RestrictPointToArea(syncPos, this.FishArea);
		}
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000CB4F4 File Offset: 0x000C96F4
	private void OnEnable()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Combine(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Combine(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.RegisterFlocking(this);
	}

	// Token: 0x040031C3 RID: 12739
	[Tooltip("Speed is randomly generated from min and max speed")]
	public float minSpeed = 2f;

	// Token: 0x040031C4 RID: 12740
	public float maxSpeed = 4f;

	// Token: 0x040031C5 RID: 12741
	public float rotationSpeed = 360f;

	// Token: 0x040031C6 RID: 12742
	[Tooltip("Maximum distance to the neighbours to form a flocking group")]
	public float maxNeighbourDistance = 4f;

	// Token: 0x040031C7 RID: 12743
	public float eatFoodDuration = 10f;

	// Token: 0x040031C8 RID: 12744
	[Tooltip("How fast should it follow the food? This value multiplies by the current speed")]
	public float followFoodSpeedMult = 3f;

	// Token: 0x040031C9 RID: 12745
	[Tooltip("How fast should it run away from players hand?")]
	public float avoidHandSpeed = 1.2f;

	// Token: 0x040031CA RID: 12746
	[FormerlySerializedAs("avoidanceDistance")]
	[Tooltip("When flocking they will avoid each other if the distance between them is less than this value")]
	public float flockingAvoidanceDistance = 2f;

	// Token: 0x040031CB RID: 12747
	[Tooltip("Follow the fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFood")]
	public double FollowFoodStopDistance = 0.20000000298023224;

	// Token: 0x040031CC RID: 12748
	[Tooltip("Follow any fake fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFakeFood")]
	public float FollowFakeFoodStopDistance = 2f;

	// Token: 0x040031CD RID: 12749
	private float speed;

	// Token: 0x040031CE RID: 12750
	private Vector3 averageHeading;

	// Token: 0x040031CF RID: 12751
	private Vector3 averagePosition;

	// Token: 0x040031D0 RID: 12752
	private float feedingTimeStarted;

	// Token: 0x040031D1 RID: 12753
	private GameObject projectileGameObject;

	// Token: 0x040031D2 RID: 12754
	private bool followingFood;

	// Token: 0x040031D3 RID: 12755
	private FlockingManager manager;

	// Token: 0x040031D4 RID: 12756
	private GameObjectManagerWithId _fishSceneGameObjectsManager;

	// Token: 0x040031D5 RID: 12757
	private UnityEvent<string, Transform> sendIdEvent;

	// Token: 0x040031D6 RID: 12758
	private Flocking.FishState fishState;

	// Token: 0x040031D7 RID: 12759
	[HideInInspector]
	public Vector3 pos;

	// Token: 0x040031D8 RID: 12760
	[HideInInspector]
	public Quaternion rot;

	// Token: 0x040031D9 RID: 12761
	private float velocity;

	// Token: 0x040031DA RID: 12762
	private bool isTurning;

	// Token: 0x040031DB RID: 12763
	private bool isRealFood;

	// Token: 0x040031DC RID: 12764
	public float avointPointRadius = 0.5f;

	// Token: 0x040031DD RID: 12765
	private float cacheSpeed;

	// Token: 0x020005E3 RID: 1507
	public enum FishState
	{
		// Token: 0x040031E0 RID: 12768
		flock,
		// Token: 0x040031E1 RID: 12769
		patrol,
		// Token: 0x040031E2 RID: 12770
		followFood
	}
}
