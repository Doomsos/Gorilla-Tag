using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200047D RID: 1149
public class GeodeItem : TransferrableObject
{
	// Token: 0x06001D3B RID: 7483 RVA: 0x0009A43D File Offset: 0x0009863D
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x0009A45F File Offset: 0x0009865F
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.prevItemState = TransferrableObject.ItemStates.State0;
		this.InitToDefault();
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x0009A47B File Offset: 0x0009867B
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x0009A490 File Offset: 0x00098690
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && this.itemState != TransferrableObject.ItemStates.State0 && !base.InHand();
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x0009A4B4 File Offset: 0x000986B4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		UnityEvent<GeodeItem> onGeodeGrabbed = this.OnGeodeGrabbed;
		if (onGeodeGrabbed == null)
		{
			return;
		}
		onGeodeGrabbed.Invoke(this);
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x0009A4D0 File Offset: 0x000986D0
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		this.effectsHaveBeenPlayed = false;
		if (this.hasEffectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.geodeFullMesh.SetActive(true);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(false);
		}
		this.hitLastFrame = false;
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x0009A538 File Offset: 0x00098738
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				this.OnItemStateChanged();
			}
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minHitVelocity)
		{
			return;
		}
		if (base.InHand())
		{
			int num = Physics.SphereCastNonAlloc(this.geodeFullMesh.transform.position, this.sphereRayRadius * Mathf.Abs(this.geodeFullMesh.transform.lossyScale.x), this.geodeFullMesh.transform.TransformDirection(Vector3.forward), this.collidersHit, this.rayCastMaxDistance, this.collisionLayerMask, 2);
			this.hitLastFrame = (num > 0);
		}
		if (!this.hitLastFrame)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		UnityEvent<GeodeItem> onGeodeCracked = this.OnGeodeCracked;
		if (onGeodeCracked != null)
		{
			onGeodeCracked.Invoke(this);
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
		this.index = (this.randomizeGeode ? this.RandomPickCrackedGeode() : 0);
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x0009A660 File Offset: 0x00098860
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.currentItemState = this.itemState;
		if (this.currentItemState != this.prevItemState)
		{
			this.OnItemStateChanged();
		}
		this.prevItemState = this.currentItemState;
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x0009A694 File Offset: 0x00098894
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.InitToDefault();
			return;
		}
		this.geodeFullMesh.SetActive(false);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(i == this.index);
		}
		RigContainer rigContainer;
		if (NetworkSystem.Instance.InRoom && GorillaGameManager.instance != null && !this.effectsHaveBeenPlayed && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
		{
			rigContainer.Rig.netView.SendRPC("RPC_PlayGeodeEffect", 0, new object[]
			{
				this.geodeFullMesh.transform.position
			});
			this.effectsHaveBeenPlayed = true;
		}
		if (!NetworkSystem.Instance.InRoom && !this.effectsHaveBeenPlayed)
		{
			if (this.audioSource)
			{
				this.audioSource.GTPlay();
			}
			this.effectsHaveBeenPlayed = true;
		}
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x0009A78D File Offset: 0x0009898D
	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, this.geodeCrackedMeshes.Length);
	}

	// Token: 0x04002740 RID: 10048
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04002741 RID: 10049
	public LayerMask collisionLayerMask;

	// Token: 0x04002742 RID: 10050
	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04002743 RID: 10051
	public float cooldown = 5f;

	// Token: 0x04002744 RID: 10052
	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	// Token: 0x04002745 RID: 10053
	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	// Token: 0x04002746 RID: 10054
	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	// Token: 0x04002747 RID: 10055
	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	// Token: 0x04002748 RID: 10056
	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	// Token: 0x04002749 RID: 10057
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x0400274A RID: 10058
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x0400274B RID: 10059
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400274C RID: 10060
	public bool randomizeGeode = true;

	// Token: 0x0400274D RID: 10061
	public UnityEvent<GeodeItem> OnGeodeCracked;

	// Token: 0x0400274E RID: 10062
	public UnityEvent<GeodeItem> OnGeodeGrabbed;

	// Token: 0x0400274F RID: 10063
	private bool hasEffectsGameObject;

	// Token: 0x04002750 RID: 10064
	private bool effectsHaveBeenPlayed;

	// Token: 0x04002751 RID: 10065
	private RaycastHit hit;

	// Token: 0x04002752 RID: 10066
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04002753 RID: 10067
	private TransferrableObject.ItemStates currentItemState;

	// Token: 0x04002754 RID: 10068
	private TransferrableObject.ItemStates prevItemState;

	// Token: 0x04002755 RID: 10069
	private int index;
}
