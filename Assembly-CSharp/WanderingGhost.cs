using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009BB RID: 2491
[NetworkBehaviourWeaved(1)]
public class WanderingGhost : NetworkComponent
{
	// Token: 0x06003FA6 RID: 16294 RVA: 0x001553B0 File Offset: 0x001535B0
	protected override void Start()
	{
		base.Start();
		this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
		this.idlePassedTime = 0f;
		ThrowableSetDressing[] array = this.allFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchor.position = this.flowerDisabledPosition;
		}
		base.Invoke("DelayedStart", 0.5f);
	}

	// Token: 0x06003FA7 RID: 16295 RVA: 0x00155417 File Offset: 0x00153617
	private void DelayedStart()
	{
		this.PickNextWaypoint();
		base.transform.position = this.currentWaypoint._transform.position;
		this.PickNextWaypoint();
		this.ChangeState(WanderingGhost.ghostState.patrol);
	}

	// Token: 0x06003FA8 RID: 16296 RVA: 0x00155448 File Offset: 0x00153648
	private void LateUpdate()
	{
		this.UpdateState();
		this.hoverVelocity -= this.mrenderer.transform.localPosition * this.hoverRectifyForce * Time.deltaTime;
		this.hoverVelocity += Random.insideUnitSphere * this.hoverRandomForce * Time.deltaTime;
		this.hoverVelocity = Vector3.MoveTowards(this.hoverVelocity, Vector3.zero, this.hoverDrag * Time.deltaTime);
		this.mrenderer.transform.localPosition += this.hoverVelocity * Time.deltaTime;
	}

	// Token: 0x06003FA9 RID: 16297 RVA: 0x0015550C File Offset: 0x0015370C
	private void PickNextWaypoint()
	{
		if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
		{
			ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, this.debugForceWaypointRegion);
			if (zoneBasedObject == null)
			{
				zoneBasedObject = this.lastWaypointRegion;
			}
			if (zoneBasedObject == null)
			{
				return;
			}
			this.lastWaypointRegion = zoneBasedObject;
			this.waypoints.Clear();
			foreach (object obj in zoneBasedObject.transform)
			{
				Transform transform = (Transform)obj;
				this.waypoints.Add(new WanderingGhost.Waypoint(transform.name.Contains("_v_"), transform));
			}
		}
		int num = Random.Range(0, this.waypoints.Count);
		this.currentWaypoint = this.waypoints[num];
		this.waypoints.RemoveAt(num);
	}

	// Token: 0x06003FAA RID: 16298 RVA: 0x0015561C File Offset: 0x0015381C
	private void Patrol()
	{
		this.idlePassedTime = 0f;
		this.mrenderer.sharedMaterial = this.scryableMaterial;
		Transform transform = this.currentWaypoint._transform;
		base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(transform.position - base.transform.position), 360f * Time.deltaTime);
	}

	// Token: 0x06003FAB RID: 16299 RVA: 0x001556C0 File Offset: 0x001538C0
	private bool MaybeHideGhost()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, this.hitColliders);
		for (int i = 0; i < num; i++)
		{
			if (this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaHand) || this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ChangeState(WanderingGhost.ghostState.patrol);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003FAC RID: 16300 RVA: 0x0015572C File Offset: 0x0015392C
	private void ChangeState(WanderingGhost.ghostState newState)
	{
		this.currentState = newState;
		this.mrenderer.sharedMaterial = ((newState == WanderingGhost.ghostState.idle) ? this.visibleMaterial : this.scryableMaterial);
		if (newState == WanderingGhost.ghostState.patrol)
		{
			this.audioSource.GTStop();
			this.audioSource.volume = this.patrolVolume;
			this.audioSource.clip = this.patrolAudio;
			this.audioSource.GTPlay();
			return;
		}
		if (newState != WanderingGhost.ghostState.idle)
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.volume = this.idleVolume;
		this.audioSource.GTPlayOneShot(this.appearAudio.GetRandomItem<AudioClip>(), 1f);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.SpawnFlowerNearby();
		}
	}

	// Token: 0x06003FAD RID: 16301 RVA: 0x001557E8 File Offset: 0x001539E8
	private void UpdateState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		if (ghostState != WanderingGhost.ghostState.patrol)
		{
			if (ghostState != WanderingGhost.ghostState.idle)
			{
				return;
			}
			this.idlePassedTime += Time.deltaTime;
			if (this.idlePassedTime >= this.idleStayDuration || this.MaybeHideGhost())
			{
				this.PickNextWaypoint();
				this.ChangeState(WanderingGhost.ghostState.patrol);
			}
		}
		else
		{
			if (this.currentWaypoint._transform == null)
			{
				this.PickNextWaypoint();
				return;
			}
			this.Patrol();
			if (Vector3.Distance(base.transform.position, this.currentWaypoint._transform.position) < 0.2f)
			{
				if (this.currentWaypoint._visible)
				{
					this.ChangeState(WanderingGhost.ghostState.idle);
					return;
				}
				this.PickNextWaypoint();
				return;
			}
		}
	}

	// Token: 0x06003FAE RID: 16302 RVA: 0x001558AC File Offset: 0x00153AAC
	private void HauntObjects()
	{
		Collider[] array = new Collider[20];
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
		for (int i = 0; i < num; i++)
		{
			if (array[i].CompareTag("HauntedObject"))
			{
				UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
				if (triggerHauntedObjects != null)
				{
					triggerHauntedObjects.Invoke(array[i].gameObject);
				}
			}
		}
	}

	// Token: 0x170005CF RID: 1487
	// (get) Token: 0x06003FAF RID: 16303 RVA: 0x0015590D File Offset: 0x00153B0D
	// (set) Token: 0x06003FB0 RID: 16304 RVA: 0x00155937 File Offset: 0x00153B37
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe WanderingGhost.ghostState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (WanderingGhost.ghostState)this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = (int)value;
		}
	}

	// Token: 0x06003FB1 RID: 16305 RVA: 0x00155962 File Offset: 0x00153B62
	public override void WriteDataFusion()
	{
		this.Data = this.currentState;
	}

	// Token: 0x06003FB2 RID: 16306 RVA: 0x00155970 File Offset: 0x00153B70
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data);
	}

	// Token: 0x06003FB3 RID: 16307 RVA: 0x0015597E File Offset: 0x00153B7E
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.currentState);
	}

	// Token: 0x06003FB4 RID: 16308 RVA: 0x001559A0 File Offset: 0x00153BA0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		WanderingGhost.ghostState state = (WanderingGhost.ghostState)stream.ReceiveNext();
		this.ReadDataShared(state);
	}

	// Token: 0x06003FB5 RID: 16309 RVA: 0x001559CE File Offset: 0x00153BCE
	private void ReadDataShared(WanderingGhost.ghostState state)
	{
		WanderingGhost.ghostState ghostState = this.currentState;
		this.currentState = state;
		if (ghostState != this.currentState)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06003FB6 RID: 16310 RVA: 0x001559F1 File Offset: 0x00153BF1
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06003FB7 RID: 16311 RVA: 0x00155A10 File Offset: 0x00153C10
	private void SpawnFlowerNearby()
	{
		Vector3 position = base.transform.position + Vector3.down * 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.flowerSpawnRadius, Vector3.down), ref raycastHit, 3f, this.flowerGroundMask))
		{
			position = raycastHit.point;
		}
		ThrowableSetDressing throwableSetDressing = null;
		int num = 0;
		foreach (ThrowableSetDressing throwableSetDressing2 in this.allFlowers)
		{
			if (!throwableSetDressing2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					throwableSetDressing = throwableSetDressing2;
				}
			}
		}
		if (throwableSetDressing != null)
		{
			if (!throwableSetDressing.IsLocalOwnedWorldShareable)
			{
				throwableSetDressing.WorldShareableRequestOwnership();
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = position;
			throwableSetDressing.StartRespawnTimer(this.flowerSpawnDuration);
		}
	}

	// Token: 0x06003FB9 RID: 16313 RVA: 0x00155B50 File Offset: 0x00153D50
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003FBA RID: 16314 RVA: 0x00155B68 File Offset: 0x00153D68
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040050CD RID: 20685
	public float patrolSpeed = 3f;

	// Token: 0x040050CE RID: 20686
	public float idleStayDuration = 5f;

	// Token: 0x040050CF RID: 20687
	public float sphereColliderRadius = 2f;

	// Token: 0x040050D0 RID: 20688
	public ThrowableSetDressing[] allFlowers;

	// Token: 0x040050D1 RID: 20689
	public Vector3 flowerDisabledPosition;

	// Token: 0x040050D2 RID: 20690
	public float flowerSpawnRadius;

	// Token: 0x040050D3 RID: 20691
	public float flowerSpawnDuration;

	// Token: 0x040050D4 RID: 20692
	public LayerMask flowerGroundMask;

	// Token: 0x040050D5 RID: 20693
	public MeshRenderer mrenderer;

	// Token: 0x040050D6 RID: 20694
	public Material visibleMaterial;

	// Token: 0x040050D7 RID: 20695
	public Material scryableMaterial;

	// Token: 0x040050D8 RID: 20696
	public GameObject waypointsContainer;

	// Token: 0x040050D9 RID: 20697
	private ZoneBasedObject[] waypointRegions;

	// Token: 0x040050DA RID: 20698
	private ZoneBasedObject lastWaypointRegion;

	// Token: 0x040050DB RID: 20699
	private List<WanderingGhost.Waypoint> waypoints = new List<WanderingGhost.Waypoint>();

	// Token: 0x040050DC RID: 20700
	private WanderingGhost.Waypoint currentWaypoint;

	// Token: 0x040050DD RID: 20701
	public string debugForceWaypointRegion;

	// Token: 0x040050DE RID: 20702
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040050DF RID: 20703
	public AudioClip[] appearAudio;

	// Token: 0x040050E0 RID: 20704
	public float idleVolume;

	// Token: 0x040050E1 RID: 20705
	public AudioClip patrolAudio;

	// Token: 0x040050E2 RID: 20706
	public float patrolVolume;

	// Token: 0x040050E3 RID: 20707
	private WanderingGhost.ghostState currentState;

	// Token: 0x040050E4 RID: 20708
	private float idlePassedTime;

	// Token: 0x040050E5 RID: 20709
	public UnityAction<GameObject> TriggerHauntedObjects;

	// Token: 0x040050E6 RID: 20710
	private Vector3 hoverVelocity;

	// Token: 0x040050E7 RID: 20711
	public float hoverRectifyForce;

	// Token: 0x040050E8 RID: 20712
	public float hoverRandomForce;

	// Token: 0x040050E9 RID: 20713
	public float hoverDrag;

	// Token: 0x040050EA RID: 20714
	private const int maxColliders = 10;

	// Token: 0x040050EB RID: 20715
	private Collider[] hitColliders = new Collider[10];

	// Token: 0x040050EC RID: 20716
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private WanderingGhost.ghostState _Data;

	// Token: 0x020009BC RID: 2492
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x06003FBB RID: 16315 RVA: 0x00155B7C File Offset: 0x00153D7C
		public Waypoint(bool visible, Transform tr)
		{
			this._visible = visible;
			this._transform = tr;
		}

		// Token: 0x040050ED RID: 20717
		[Tooltip("The ghost will be visible when its reached to this waypoint")]
		public bool _visible;

		// Token: 0x040050EE RID: 20718
		public Transform _transform;
	}

	// Token: 0x020009BD RID: 2493
	private enum ghostState
	{
		// Token: 0x040050F0 RID: 20720
		patrol,
		// Token: 0x040050F1 RID: 20721
		idle
	}
}
