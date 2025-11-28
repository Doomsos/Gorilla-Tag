using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000B8A RID: 2954
public class GorillaFriendCollider : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004910 RID: 18704 RVA: 0x00180070 File Offset: 0x0017E270
	public void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.thisBox = base.GetComponent<BoxCollider>();
		this.jiggleAmount = Random.Range(0f, 1f);
		this.tagAndBodyLayerMask = (LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		}));
	}

	// Token: 0x06004911 RID: 18705 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004912 RID: 18706 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004913 RID: 18707 RVA: 0x001800D7 File Offset: 0x0017E2D7
	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	// Token: 0x06004914 RID: 18708 RVA: 0x001800F8 File Offset: 0x0017E2F8
	public void SliceUpdate()
	{
		float time = Time.time;
		if (this._nextUpdateTime < 0f)
		{
			this._nextUpdateTime = time + 1f + this.jiggleAmount;
			return;
		}
		if (time < this._nextUpdateTime)
		{
			return;
		}
		this._nextUpdateTime = time + 1f;
		if (NetworkSystem.Instance.InRoom || this.runCheckWhileNotInRoom)
		{
			this.RefreshPlayersInSphere();
		}
	}

	// Token: 0x06004915 RID: 18709 RVA: 0x00180160 File Offset: 0x0017E360
	public void RefreshPlayersInSphere()
	{
		this.playerIDsCurrentlyTouching.Clear();
		if (this.thisBox != null)
		{
			this.collisions = Physics.OverlapBoxNonAlloc(this.thisBox.transform.position, this.thisBox.size / 2f, this.overlapColliders, this.thisBox.transform.rotation, this.tagAndBodyLayerMask);
		}
		else
		{
			this.collisions = Physics.OverlapSphereNonAlloc(base.transform.position, this.thisCapsule.radius, this.overlapColliders, this.tagAndBodyLayerMask);
		}
		this.collisions = Mathf.Min(this.collisions, this.overlapColliders.Length);
		if (this.collisions > 0)
		{
			for (int i = 0; i < this.collisions; i++)
			{
				this.otherCollider = this.overlapColliders[i];
				if (!(this.otherCollider == null) && !(this.otherCollider.attachedRigidbody == null))
				{
					this.otherColliderGO = this.otherCollider.attachedRigidbody.gameObject;
					this.collidingRig = this.otherColliderGO.GetComponent<VRRig>();
					if (this.collidingRig == null || this.collidingRig.creator == null || this.collidingRig.creator.IsNull || string.IsNullOrEmpty(this.collidingRig.creator.UserId))
					{
						GTPlayer component = this.otherColliderGO.GetComponent<GTPlayer>();
						if (component == null || NetworkSystem.Instance.LocalPlayer == null)
						{
							goto IL_264;
						}
						if (this.thisCapsule != null && this.applyCapsuleYLimits)
						{
							float y = component.bodyCollider.transform.position.y;
							if (y < this.capsuleColliderYLimits.x || y > this.capsuleColliderYLimits.y)
							{
								goto IL_264;
							}
						}
						string userId = NetworkSystem.Instance.LocalPlayer.UserId;
						this.AddUserID(userId);
					}
					else
					{
						if (this.thisCapsule != null && this.applyCapsuleYLimits)
						{
							float y2 = this.collidingRig.bodyTransform.transform.position.y;
							if (y2 < this.capsuleColliderYLimits.x || y2 > this.capsuleColliderYLimits.y)
							{
								goto IL_264;
							}
						}
						string userId = this.collidingRig.creator.UserId;
						this.AddUserID(userId);
					}
					this.overlapColliders[i] = null;
				}
				IL_264:;
			}
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.LocalPlayer != null && this.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
			{
				GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
				GorillaComputer.instance.friendJoinCollider = this;
				GorillaComputer.instance.UpdateScreen();
			}
			this.otherCollider = null;
			this.otherColliderGO = null;
			this.collidingRig = null;
		}
	}

	// Token: 0x0400598C RID: 22924
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	// Token: 0x0400598D RID: 22925
	private CapsuleCollider thisCapsule;

	// Token: 0x0400598E RID: 22926
	private BoxCollider thisBox;

	// Token: 0x0400598F RID: 22927
	[Tooltip("If using a capsule collider, the player position can be checked against these minimum and maximum Y limits (world position) to make it behave more like a cylinder check")]
	public bool applyCapsuleYLimits;

	// Token: 0x04005990 RID: 22928
	[Tooltip("If the player's Y world position is lower than Limits.x or higher than Limits.y, they will not be considered \"Inside\" the friend collider")]
	public Vector2 capsuleColliderYLimits = Vector2.zero;

	// Token: 0x04005991 RID: 22929
	public bool runCheckWhileNotInRoom;

	// Token: 0x04005992 RID: 22930
	public string[] myAllowedMapsToJoin;

	// Token: 0x04005993 RID: 22931
	private readonly Collider[] overlapColliders = new Collider[20];

	// Token: 0x04005994 RID: 22932
	private int tagAndBodyLayerMask;

	// Token: 0x04005995 RID: 22933
	private float jiggleAmount;

	// Token: 0x04005996 RID: 22934
	private Collider otherCollider;

	// Token: 0x04005997 RID: 22935
	private GameObject otherColliderGO;

	// Token: 0x04005998 RID: 22936
	private VRRig collidingRig;

	// Token: 0x04005999 RID: 22937
	private int collisions;

	// Token: 0x0400599A RID: 22938
	private WaitForSeconds wait1Sec = new WaitForSeconds(1f);

	// Token: 0x0400599B RID: 22939
	public bool manualRefreshOnly;

	// Token: 0x0400599C RID: 22940
	private float _nextUpdateTime = -1f;
}
