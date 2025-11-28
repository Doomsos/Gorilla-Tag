using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DF6 RID: 3574
	[NetworkBehaviourWeaved(6)]
	public class LurkerGhost : NetworkComponent
	{
		// Token: 0x06005924 RID: 22820 RVA: 0x001C8186 File Offset: 0x001C6386
		protected override void Awake()
		{
			base.Awake();
			this.possibleTargets = new List<NetPlayer>();
			this.targetPlayer = null;
			this.targetTransform = null;
			this.targetVRRig = null;
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x001C81AE File Offset: 0x001C63AE
		protected override void Start()
		{
			base.Start();
			this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
			this.PickNextWaypoint();
			this.ChangeState(LurkerGhost.ghostState.patrol);
		}

		// Token: 0x06005926 RID: 22822 RVA: 0x001C81D4 File Offset: 0x001C63D4
		private void LateUpdate()
		{
			this.UpdateState();
			this.UpdateGhostVisibility();
		}

		// Token: 0x06005927 RID: 22823 RVA: 0x001C81E4 File Offset: 0x001C63E4
		private void PickNextWaypoint()
		{
			if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
			{
				ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, "");
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
					this.waypoints.Add(transform);
				}
			}
			int num = Random.Range(0, this.waypoints.Count);
			this.currentWaypoint = this.waypoints[num];
			this.targetRotation = Quaternion.LookRotation(this.currentWaypoint.position - base.transform.position);
			this.waypoints.RemoveAt(num);
		}

		// Token: 0x06005928 RID: 22824 RVA: 0x001C8304 File Offset: 0x001C6504
		private void Patrol()
		{
			Transform transform = this.currentWaypoint;
			if (transform != null)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 360f * Time.deltaTime);
			}
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x001C837C File Offset: 0x001C657C
		private void PlaySound(AudioClip clip, bool loop)
		{
			if (this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.audioSource && clip != null)
			{
				this.audioSource.clip = clip;
				this.audioSource.loop = loop;
				this.audioSource.GTPlay();
			}
		}

		// Token: 0x0600592A RID: 22826 RVA: 0x001C83E8 File Offset: 0x001C65E8
		private bool PickPlayer(float maxDistance)
		{
			if (base.IsMine)
			{
				this.possibleTargets.Clear();
				for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
				{
					if ((GorillaParent.instance.vrrigs[i].transform.position - base.transform.position).magnitude < maxDistance && GorillaParent.instance.vrrigs[i].creator != this.targetPlayer)
					{
						this.possibleTargets.Add(GorillaParent.instance.vrrigs[i].creator);
					}
				}
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
				if (this.possibleTargets.Count > 0)
				{
					int num = Random.Range(0, this.possibleTargets.Count);
					this.PickPlayer(this.possibleTargets[num]);
				}
			}
			else
			{
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
			}
			return this.targetPlayer != null && this.targetTransform != null;
		}

		// Token: 0x0600592B RID: 22827 RVA: 0x001C8518 File Offset: 0x001C6718
		private void PickPlayer(NetPlayer player)
		{
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == player);
			if (num > -1 && num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.targetTransform = GorillaParent.instance.vrrigs[num].head.rigTarget;
				this.targetVRRig = GorillaParent.instance.vrrigs[num];
			}
		}

		// Token: 0x0600592C RID: 22828 RVA: 0x001C85C0 File Offset: 0x001C67C0
		private void SeekPlayer()
		{
			if (this.targetTransform == null)
			{
				this.ChangeState(LurkerGhost.ghostState.patrol);
				return;
			}
			this.targetPosition = this.targetTransform.position + this.targetTransform.forward.x0z() * this.seekAheadDistance;
			this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.seekSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x0600592D RID: 22829 RVA: 0x001C8694 File Offset: 0x001C6894
		private void ChargeAtPlayer()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.chargeSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x0600592E RID: 22830 RVA: 0x001C86FC File Offset: 0x001C68FC
		private void UpdateGhostVisibility()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.seek:
			case LurkerGhost.ghostState.charge:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600592F RID: 22831 RVA: 0x001C8820 File Offset: 0x001C6A20
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

		// Token: 0x06005930 RID: 22832 RVA: 0x001C8884 File Offset: 0x001C6A84
		private void ChangeState(LurkerGhost.ghostState newState)
		{
			this.currentState = newState;
			VRRig vrrig = null;
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.PlaySound(this.patrolAudio, true);
				this.passingPlayer = null;
				this.cooldownTimeRemaining = Random.Range(this.cooldownDuration, this.maxCooldownDuration);
				this.currentRepeatHuntTimes = 0;
				break;
			case LurkerGhost.ghostState.charge:
				this.PlaySound(this.huntAudio, false);
				this.targetPosition = this.targetTransform.position;
				this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.PlaySound(this.possessedAudio, true);
					GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
					GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
				}
				vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				break;
			}
			Shader.SetGlobalFloat(this._BlackAndWhite, (float)((newState == LurkerGhost.ghostState.possess && this.targetPlayer == NetworkSystem.Instance.LocalPlayer) ? 1 : 0));
			if (vrrig != this.lastHauntedVRRig && this.lastHauntedVRRig != null)
			{
				this.lastHauntedVRRig.IsHaunted = false;
			}
			if (vrrig != null)
			{
				vrrig.IsHaunted = true;
			}
			this.lastHauntedVRRig = vrrig;
			this.UpdateGhostVisibility();
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x001C8A02 File Offset: 0x001C6C02
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		}

		// Token: 0x06005932 RID: 22834 RVA: 0x001C8A20 File Offset: 0x001C6C20
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.Patrol();
				if (base.IsMine)
				{
					if (this.currentWaypoint == null || Vector3.Distance(base.transform.position, this.currentWaypoint.position) < 0.2f)
					{
						this.PickNextWaypoint();
					}
					this.cooldownTimeRemaining -= Time.deltaTime;
					if (this.cooldownTimeRemaining <= 0f)
					{
						this.cooldownTimeRemaining = 0f;
						if (this.PickPlayer(this.maxHuntDistance))
						{
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
					}
				}
				break;
			case LurkerGhost.ghostState.seek:
				this.SeekPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < this.seekCloseEnoughDistance * this.seekCloseEnoughDistance)
				{
					this.ChangeState(LurkerGhost.ghostState.charge);
					return;
				}
				break;
			case LurkerGhost.ghostState.charge:
				this.ChargeAtPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < 0.25f)
				{
					if ((this.targetTransform.position - this.targetPosition).magnitude < this.minCatchDistance)
					{
						this.ChangeState(LurkerGhost.ghostState.possess);
						return;
					}
					this.huntedPassedTime = 0f;
					this.ChangeState(LurkerGhost.ghostState.patrol);
					return;
				}
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetTransform != null)
				{
					float num = this.SpookyMagicNumbers.x + MathF.Abs(MathF.Sin(Time.time * this.SpookyMagicNumbers.y));
					float num2 = this.HauntedMagicNumbers.x * MathF.Sin(Time.time * this.HauntedMagicNumbers.y) + this.HauntedMagicNumbers.z * MathF.Sin(Time.time * this.HauntedMagicNumbers.w);
					float num3 = 0.5f + 0.5f * MathF.Sin(Time.time * this.SpookyMagicNumbers.z);
					Vector3 vector = this.targetTransform.position + new Vector3(num * (float)Math.Sin((double)num2), num3, num * (float)Math.Cos((double)num2));
					base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.chargeSpeed);
					base.transform.rotation = Quaternion.LookRotation(base.transform.position - this.targetTransform.position);
				}
				if (base.IsMine)
				{
					this.huntedPassedTime += Time.deltaTime;
					if (this.huntedPassedTime >= this.PossessionDuration)
					{
						this.huntedPassedTime = 0f;
						if (this.hauntNeighbors && this.currentRepeatHuntTimes < this.maxRepeatHuntTimes && this.PickPlayer(this.maxRepeatHuntDistance))
						{
							this.currentRepeatHuntTimes++;
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
						this.ChangeState(LurkerGhost.ghostState.patrol);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x06005933 RID: 22835 RVA: 0x001C8D2D File Offset: 0x001C6F2D
		// (set) Token: 0x06005934 RID: 22836 RVA: 0x001C8D57 File Offset: 0x001C6F57
		[Networked]
		[NetworkedWeaved(0, 6)]
		private unsafe LurkerGhost.LurkerGhostData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(LurkerGhost.LurkerGhostData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(LurkerGhost.LurkerGhostData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005935 RID: 22837 RVA: 0x001C8D82 File Offset: 0x001C6F82
		public override void WriteDataFusion()
		{
			this.Data = new LurkerGhost.LurkerGhostData(this.currentState, this.currentIndex, this.targetPlayer.ActorNumber, this.targetPosition);
		}

		// Token: 0x06005936 RID: 22838 RVA: 0x001C8DAC File Offset: 0x001C6FAC
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentIndex, this.Data.TargetActor, this.Data.TargetPos);
		}

		// Token: 0x06005937 RID: 22839 RVA: 0x001C8DF8 File Offset: 0x001C6FF8
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentState);
			stream.SendNext(this.currentIndex);
			if (this.targetPlayer != null)
			{
				stream.SendNext(this.targetPlayer.ActorNumber);
			}
			else
			{
				stream.SendNext(-1);
			}
			stream.SendNext(this.targetPosition);
		}

		// Token: 0x06005938 RID: 22840 RVA: 0x001C8E74 File Offset: 0x001C7074
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			LurkerGhost.ghostState state = (LurkerGhost.ghostState)stream.ReceiveNext();
			int index = (int)stream.ReceiveNext();
			int targetActorNumber = (int)stream.ReceiveNext();
			Vector3 targetPos = (Vector3)stream.ReceiveNext();
			this.ReadDataShared(state, index, targetActorNumber, targetPos);
		}

		// Token: 0x06005939 RID: 22841 RVA: 0x001C8ECC File Offset: 0x001C70CC
		private void ReadDataShared(LurkerGhost.ghostState state, int index, int targetActorNumber, Vector3 targetPos)
		{
			LurkerGhost.ghostState ghostState = this.currentState;
			this.currentState = state;
			this.currentIndex = index;
			NetPlayer netPlayer = this.targetPlayer;
			this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
			this.targetPosition = targetPos;
			float num = 10000f;
			if (!this.targetPosition.IsValid(num))
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(this.targetPlayer, out rigContainer))
				{
					this.targetPosition = (this.targetPlayer.IsLocal ? rigContainer.Rig.transform.position : rigContainer.Rig.syncPos);
				}
				else
				{
					this.targetPosition = base.transform.position;
				}
			}
			if (this.targetPlayer != netPlayer)
			{
				this.PickPlayer(this.targetPlayer);
			}
			if (ghostState != this.currentState || this.targetPlayer != netPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x0600593A RID: 22842 RVA: 0x001C8FAD File Offset: 0x001C71AD
		public override void OnOwnerChange(Player newOwner, Player previousOwner)
		{
			base.OnOwnerChange(newOwner, previousOwner);
			if (newOwner == PhotonNetwork.LocalPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x0600593C RID: 22844 RVA: 0x001C90D0 File Offset: 0x001C72D0
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600593D RID: 22845 RVA: 0x001C90E8 File Offset: 0x001C72E8
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04006636 RID: 26166
		public float patrolSpeed = 3f;

		// Token: 0x04006637 RID: 26167
		public float seekSpeed = 6f;

		// Token: 0x04006638 RID: 26168
		public float chargeSpeed = 6f;

		// Token: 0x04006639 RID: 26169
		[Tooltip("Cooldown until the next time the ghost needs to hunt a new player")]
		public float cooldownDuration = 10f;

		// Token: 0x0400663A RID: 26170
		[Tooltip("Max Cooldown (randomized)")]
		public float maxCooldownDuration = 10f;

		// Token: 0x0400663B RID: 26171
		[Tooltip("How long the possession effects should last")]
		public float PossessionDuration = 15f;

		// Token: 0x0400663C RID: 26172
		[Tooltip("Hunted objects within this radius will get triggered ")]
		public float sphereColliderRadius = 2f;

		// Token: 0x0400663D RID: 26173
		[Tooltip("Maximum distance to the possible player to get hunted")]
		public float maxHuntDistance = 20f;

		// Token: 0x0400663E RID: 26174
		[Tooltip("Minimum distance from the player to start the possession effects")]
		public float minCatchDistance = 2f;

		// Token: 0x0400663F RID: 26175
		[Tooltip("Maximum distance to the possible player to get repeat hunted")]
		public float maxRepeatHuntDistance = 5f;

		// Token: 0x04006640 RID: 26176
		[Tooltip("Maximum times the lurker can haunt a nearby player before going back on cooldown")]
		public int maxRepeatHuntTimes = 3;

		// Token: 0x04006641 RID: 26177
		[Tooltip("Time in seconds before a haunted player can pass the lurker to another player by tagging")]
		public float tagCoolDown = 2f;

		// Token: 0x04006642 RID: 26178
		[Tooltip("UP & DOWN, IN & OUT")]
		public Vector3 SpookyMagicNumbers = new Vector3(1f, 1f, 1f);

		// Token: 0x04006643 RID: 26179
		[Tooltip("SPIN, SPIN, SPIN, SPIN")]
		public Vector4 HauntedMagicNumbers = new Vector4(1f, 2f, 3f, 1f);

		// Token: 0x04006644 RID: 26180
		[Tooltip("Haptic vibration when haunted by the ghost")]
		public float hapticStrength = 1f;

		// Token: 0x04006645 RID: 26181
		public float hapticDuration = 1.5f;

		// Token: 0x04006646 RID: 26182
		public GameObject waypointsContainer;

		// Token: 0x04006647 RID: 26183
		private ZoneBasedObject[] waypointRegions;

		// Token: 0x04006648 RID: 26184
		private ZoneBasedObject lastWaypointRegion;

		// Token: 0x04006649 RID: 26185
		private List<Transform> waypoints = new List<Transform>();

		// Token: 0x0400664A RID: 26186
		private Transform currentWaypoint;

		// Token: 0x0400664B RID: 26187
		public Material visibleMaterial;

		// Token: 0x0400664C RID: 26188
		public Material scryableMaterial;

		// Token: 0x0400664D RID: 26189
		public Material visibleMaterialBones;

		// Token: 0x0400664E RID: 26190
		public Material scryableMaterialBones;

		// Token: 0x0400664F RID: 26191
		public MeshRenderer meshRenderer;

		// Token: 0x04006650 RID: 26192
		public MeshRenderer bonesMeshRenderer;

		// Token: 0x04006651 RID: 26193
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006652 RID: 26194
		public AudioClip patrolAudio;

		// Token: 0x04006653 RID: 26195
		public AudioClip huntAudio;

		// Token: 0x04006654 RID: 26196
		public AudioClip possessedAudio;

		// Token: 0x04006655 RID: 26197
		public ThrowableSetDressing scryingGlass;

		// Token: 0x04006656 RID: 26198
		public float scryingAngerAngle;

		// Token: 0x04006657 RID: 26199
		public float scryingAngerDelay;

		// Token: 0x04006658 RID: 26200
		public float seekAheadDistance;

		// Token: 0x04006659 RID: 26201
		public float seekCloseEnoughDistance;

		// Token: 0x0400665A RID: 26202
		private float scryingAngerAfterTimestamp;

		// Token: 0x0400665B RID: 26203
		private int currentRepeatHuntTimes;

		// Token: 0x0400665C RID: 26204
		public UnityAction<GameObject> TriggerHauntedObjects;

		// Token: 0x0400665D RID: 26205
		private int currentIndex;

		// Token: 0x0400665E RID: 26206
		private LurkerGhost.ghostState currentState;

		// Token: 0x0400665F RID: 26207
		private float cooldownTimeRemaining;

		// Token: 0x04006660 RID: 26208
		private List<NetPlayer> possibleTargets;

		// Token: 0x04006661 RID: 26209
		private NetPlayer targetPlayer;

		// Token: 0x04006662 RID: 26210
		private Transform targetTransform;

		// Token: 0x04006663 RID: 26211
		private float huntedPassedTime;

		// Token: 0x04006664 RID: 26212
		private Vector3 targetPosition;

		// Token: 0x04006665 RID: 26213
		private Quaternion targetRotation;

		// Token: 0x04006666 RID: 26214
		private VRRig targetVRRig;

		// Token: 0x04006667 RID: 26215
		private ShaderHashId _BlackAndWhite = "_BlackAndWhite";

		// Token: 0x04006668 RID: 26216
		private VRRig lastHauntedVRRig;

		// Token: 0x04006669 RID: 26217
		private float nextTagTime;

		// Token: 0x0400666A RID: 26218
		private NetPlayer passingPlayer;

		// Token: 0x0400666B RID: 26219
		[SerializeField]
		private bool hauntNeighbors = true;

		// Token: 0x0400666C RID: 26220
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 6)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private LurkerGhost.LurkerGhostData _Data;

		// Token: 0x02000DF7 RID: 3575
		private enum ghostState
		{
			// Token: 0x0400666E RID: 26222
			patrol,
			// Token: 0x0400666F RID: 26223
			seek,
			// Token: 0x04006670 RID: 26224
			charge,
			// Token: 0x04006671 RID: 26225
			possess
		}

		// Token: 0x02000DF8 RID: 3576
		[NetworkStructWeaved(6)]
		[StructLayout(2, Size = 24)]
		private struct LurkerGhostData : INetworkStruct
		{
			// Token: 0x1700085A RID: 2138
			// (get) Token: 0x0600593E RID: 22846 RVA: 0x001C90FC File Offset: 0x001C72FC
			// (set) Token: 0x0600593F RID: 22847 RVA: 0x001C9104 File Offset: 0x001C7304
			public LurkerGhost.ghostState CurrentState { readonly get; set; }

			// Token: 0x1700085B RID: 2139
			// (get) Token: 0x06005940 RID: 22848 RVA: 0x001C910D File Offset: 0x001C730D
			// (set) Token: 0x06005941 RID: 22849 RVA: 0x001C9115 File Offset: 0x001C7315
			public int CurrentIndex { readonly get; set; }

			// Token: 0x1700085C RID: 2140
			// (get) Token: 0x06005942 RID: 22850 RVA: 0x001C911E File Offset: 0x001C731E
			// (set) Token: 0x06005943 RID: 22851 RVA: 0x001C9126 File Offset: 0x001C7326
			public int TargetActor { readonly get; set; }

			// Token: 0x1700085D RID: 2141
			// (get) Token: 0x06005944 RID: 22852 RVA: 0x001C912F File Offset: 0x001C732F
			// (set) Token: 0x06005945 RID: 22853 RVA: 0x001C9141 File Offset: 0x001C7341
			[Networked]
			[NetworkedWeaved(3, 3)]
			public unsafe Vector3 TargetPos
			{
				readonly get
				{
					return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos);
				}
				set
				{
					*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos) = value;
				}
			}

			// Token: 0x06005946 RID: 22854 RVA: 0x001C9154 File Offset: 0x001C7354
			public LurkerGhostData(LurkerGhost.ghostState state, int index, int actor, Vector3 pos)
			{
				this.CurrentState = state;
				this.CurrentIndex = index;
				this.TargetActor = actor;
				this.TargetPos = pos;
			}

			// Token: 0x04006675 RID: 26229
			[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(12)]
			private FixedStorage@3 _TargetPos;
		}
	}
}
