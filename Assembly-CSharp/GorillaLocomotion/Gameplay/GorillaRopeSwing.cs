using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F9A RID: 3994
	public class GorillaRopeSwing : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06006435 RID: 25653 RVA: 0x0020AEE8 File Offset: 0x002090E8
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06006436 RID: 25654 RVA: 0x0020AEF1 File Offset: 0x002090F1
		// (set) Token: 0x06006437 RID: 25655 RVA: 0x0020AEF9 File Offset: 0x002090F9
		public bool isIdle { get; private set; }

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x06006438 RID: 25656 RVA: 0x0020AF02 File Offset: 0x00209102
		// (set) Token: 0x06006439 RID: 25657 RVA: 0x0020AF0A File Offset: 0x0020910A
		public bool isFullyIdle { get; private set; }

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x0600643A RID: 25658 RVA: 0x0020AF13 File Offset: 0x00209113
		public bool SupportsMovingAtRuntime
		{
			get
			{
				return this.supportMovingAtRuntime;
			}
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x0600643B RID: 25659 RVA: 0x0020AF1B File Offset: 0x0020911B
		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		// Token: 0x0600643C RID: 25660 RVA: 0x0020AF38 File Offset: 0x00209138
		protected virtual void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		// Token: 0x0600643D RID: 25661 RVA: 0x0020AF9B File Offset: 0x0020919B
		protected virtual void Start()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			RopeSwingManager.Register(this);
			this.started = true;
		}

		// Token: 0x0600643E RID: 25662 RVA: 0x0020AFB9 File Offset: 0x002091B9
		private void OnDestroy()
		{
			if (RopeSwingManager.instance != null)
			{
				RopeSwingManager.Unregister(this);
			}
		}

		// Token: 0x0600643F RID: 25663 RVA: 0x0020AFD0 File Offset: 0x002091D0
		protected virtual void OnEnable()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			GorillaRopeSwingUpdateManager.RegisterRopeSwing(this);
		}

		// Token: 0x06006440 RID: 25664 RVA: 0x0020B03F File Offset: 0x0020923F
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
			GorillaRopeSwingUpdateManager.UnregisterRopeSwing(this);
		}

		// Token: 0x06006441 RID: 25665 RVA: 0x0020B060 File Offset: 0x00209260
		internal void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
			int staticHash2 = base.GetType().Name.GetStaticHash();
			int num = StaticHash.Compute(staticHash, staticHash2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					Vector3 position = transform.position;
					int i = StaticHash.Compute(position.x, position.y, position.z);
					int instanceID = transform.GetInstanceID();
					int num2 = StaticHash.Compute(num, i, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num2);
				}
				this.ropeId = this.staticId.GetStaticHash();
				return;
			}
			this.ropeId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x06006442 RID: 25666 RVA: 0x0020B11C File Offset: 0x0020931C
		public void InvokeUpdate()
		{
			if (this.isIdle)
			{
				this.isFullyIdle = true;
			}
			if (!this.isIdle)
			{
				int num = -1;
				if (this.localPlayerOn)
				{
					num = this.localPlayerBoneIndex;
				}
				else if (this.remotePlayers.Count > 0)
				{
					num = Enumerable.First<KeyValuePair<int, int>>(this.remotePlayers).Value;
				}
				if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				if (this.localPlayerOn)
				{
					float num2 = MathUtils.Linear(this.velocityTracker.GetLatestVelocity(true).magnitude / this.scaleFactor, 0f, 10f, -0.07f, 0.5f);
					if (num2 > 0f)
					{
						GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num2, Time.deltaTime);
					}
				}
				Transform bone = this.GetBone(this.lastNodeCheckIndex);
				Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
				if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, this.wallLayerMask, 1) > 0)
				{
					this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, default(PhotonMessageInfoWrapped));
				}
				if (nodeVelocity.magnitude <= 0.35f)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true, false);
					this.potentialIdleTimer = 0f;
				}
				this.lastNodeCheckIndex++;
				if (this.lastNodeCheckIndex > this.nodes.Length)
				{
					this.lastNodeCheckIndex = 2;
				}
			}
			if (this.hasMonkeBlockParent && this.supportMovingAtRuntime)
			{
				base.transform.rotation = Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
			}
		}

		// Token: 0x06006443 RID: 25667 RVA: 0x0020B350 File Offset: 0x00209550
		private void SetIsIdle(bool idle, bool resetPos = false)
		{
			this.isIdle = idle;
			this.ropeCreakSFX.gameObject.SetActive(!idle);
			if (idle)
			{
				this.ToggleVelocityTracker(false, 0, default(Vector3));
				if (resetPos)
				{
					Vector3 vector = Vector3.zero;
					for (int i = 0; i < this.nodes.Length; i++)
					{
						this.nodes[i].transform.localRotation = Quaternion.identity;
						this.nodes[i].transform.localPosition = vector;
						vector += new Vector3(0f, -this.ropeBitGenOffset, 0f);
					}
					return;
				}
			}
			else
			{
				this.isFullyIdle = false;
			}
		}

		// Token: 0x06006444 RID: 25668 RVA: 0x0020B3F7 File Offset: 0x002095F7
		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return Enumerable.Last<Transform>(this.nodes);
			}
			return this.nodes[index];
		}

		// Token: 0x06006445 RID: 25669 RVA: 0x0020B418 File Offset: 0x00209618
		public int GetBoneIndex(Transform r)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] == r)
				{
					return i;
				}
			}
			return this.nodes.Length - 1;
		}

		// Token: 0x06006446 RID: 25670 RVA: 0x0020B454 File Offset: 0x00209654
		public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(grabbedBone);
			this.localPlayerBoneIndex = boneIndex;
			velocity /= this.scaleFactor;
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.ropeId;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == 4);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = false;
			}
			this.RefreshAllBonesMass();
			List<Vector3> list = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Transform transform in this.nodes)
				{
					list.Add(transform.position);
				}
			}
			velocity.y = 0f;
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
			{
				RopeSwingManager.instance.SendSetVelocity_RPC(this.ropeId, boneIndex, velocity, true);
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.ToggleVelocityTracker(true, boneIndex, offset);
		}

		// Token: 0x06006447 RID: 25671 RVA: 0x0020B5ED File Offset: 0x002097ED
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localPlayerBoneIndex = 0;
			this.RefreshAllBonesMass();
		}

		// Token: 0x06006448 RID: 25672 RVA: 0x0020B62C File Offset: 0x0020982C
		private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
		{
			if (enable)
			{
				this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
				this.velocityTracker.transform.localPosition = offset;
				this.velocityTracker.ResetState();
			}
			this.velocityTracker.gameObject.SetActive(enable);
			if (enable)
			{
				this.velocityTracker.Tick();
			}
		}

		// Token: 0x06006449 RID: 25673 RVA: 0x0020B690 File Offset: 0x00209890
		private void RefreshAllBonesMass()
		{
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.remotePlayers)
			{
				if (keyValuePair.Value > num)
				{
					num = keyValuePair.Value;
				}
			}
			if (this.localPlayerBoneIndex > num)
			{
				num = this.localPlayerBoneIndex;
			}
			VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, num);
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x0020B714 File Offset: 0x00209914
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Transform bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			this.RefreshAllBonesMass();
			return true;
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x0020B781 File Offset: 0x00209981
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x0020B798 File Offset: 0x00209998
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			float num = 10000f;
			if (!velocity.IsValid(num))
			{
				return;
			}
			velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
			velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
			velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
			boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
			Transform bone = this.GetBone(boneIndex);
			if (!bone)
			{
				return;
			}
			if (info.Sender != null && !info.Sender.IsLocal)
			{
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
				if (!vrrig || Vector3.Distance(bone.position, vrrig.transform.position) > 5f)
				{
					return;
				}
			}
			this.SetIsIdle(false, false);
			if (bone)
			{
				VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
			}
		}

		// Token: 0x0600644D RID: 25677 RVA: 0x0020B8A0 File Offset: 0x00209AA0
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.monkeBlockParent = base.GetComponentInParent<BuilderPiece>();
			this.hasMonkeBlockParent = (this.monkeBlockParent != null);
			int num = StaticHash.Compute(pieceType, pieceId);
			this.staticId = string.Format("#ID_{0:X8}", num);
			this.ropeId = this.staticId.GetStaticHash();
			GorillaRopeSwing gorillaRopeSwing;
			if (this.started && !RopeSwingManager.instance.TryGetRope(this.ropeId, out gorillaRopeSwing))
			{
				RopeSwingManager.Register(this);
			}
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x0020B91C File Offset: 0x00209B1C
		public void OnPieceDestroy()
		{
			RopeSwingManager.Unregister(this);
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x0020B924 File Offset: 0x00209B24
		public void OnPiecePlacementDeserialized()
		{
			VectorizedCustomRopeSimulation.Unregister(this);
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x0020B9AD File Offset: 0x00209BAD
		public void OnPieceActivate()
		{
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x06006451 RID: 25681 RVA: 0x0020B9CC File Offset: 0x00209BCC
		private bool IsAttachedToMovingPiece()
		{
			return this.monkeBlockParent.attachIndex >= 0 && this.monkeBlockParent.attachIndex < this.monkeBlockParent.gridPlanes.Count && this.monkeBlockParent.gridPlanes[this.monkeBlockParent.attachIndex].GetMovingParentGrid() != null;
		}

		// Token: 0x06006452 RID: 25682 RVA: 0x0020BA2C File Offset: 0x00209C2C
		public void OnPieceDeactivate()
		{
			this.supportMovingAtRuntime = false;
		}

		// Token: 0x040073E2 RID: 29666
		public int ropeId;

		// Token: 0x040073E3 RID: 29667
		public string staticId;

		// Token: 0x040073E4 RID: 29668
		public bool useStaticId;

		// Token: 0x040073E5 RID: 29669
		protected float ropeBitGenOffset = 1f;

		// Token: 0x040073E6 RID: 29670
		[SerializeField]
		protected GameObject prefabRopeBit;

		// Token: 0x040073E7 RID: 29671
		[SerializeField]
		private bool supportMovingAtRuntime;

		// Token: 0x040073E8 RID: 29672
		public Transform[] nodes = Array.Empty<Transform>();

		// Token: 0x040073E9 RID: 29673
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x040073EA RID: 29674
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x040073EB RID: 29675
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x040073EC RID: 29676
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x040073ED RID: 29677
		private bool localPlayerOn;

		// Token: 0x040073EE RID: 29678
		private int localPlayerBoneIndex;

		// Token: 0x040073EF RID: 29679
		private XRNode localPlayerXRNode;

		// Token: 0x040073F0 RID: 29680
		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		// Token: 0x040073F1 RID: 29681
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x040073F4 RID: 29684
		private float potentialIdleTimer;

		// Token: 0x040073F5 RID: 29685
		[SerializeField]
		protected int ropeLength = 8;

		// Token: 0x040073F6 RID: 29686
		[SerializeField]
		private GorillaRopeSwingSettings settings;

		// Token: 0x040073F7 RID: 29687
		private bool hasMonkeBlockParent;

		// Token: 0x040073F8 RID: 29688
		private BuilderPiece monkeBlockParent;

		// Token: 0x040073F9 RID: 29689
		[NonSerialized]
		public int ropeDataStartIndex;

		// Token: 0x040073FA RID: 29690
		[NonSerialized]
		public int ropeDataIndexOffset;

		// Token: 0x040073FB RID: 29691
		[SerializeField]
		private LayerMask wallLayerMask;

		// Token: 0x040073FC RID: 29692
		private RaycastHit[] nodeHits = new RaycastHit[1];

		// Token: 0x040073FD RID: 29693
		private float scaleFactor = 1f;

		// Token: 0x040073FE RID: 29694
		private bool started;

		// Token: 0x040073FF RID: 29695
		private int lastNodeCheckIndex = 2;
	}
}
