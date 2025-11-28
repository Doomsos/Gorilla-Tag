using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA2 RID: 4002
	public class OldGorillaRopeSwing : MonoBehaviourPun
	{
		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x06006478 RID: 25720 RVA: 0x0020C40C File Offset: 0x0020A60C
		// (set) Token: 0x06006479 RID: 25721 RVA: 0x0020C414 File Offset: 0x0020A614
		public bool isIdle { get; private set; }

		// Token: 0x0600647A RID: 25722 RVA: 0x0020C41D File Offset: 0x0020A61D
		private void Awake()
		{
			this.SetIsIdle(true);
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x0020C426 File Offset: 0x0020A626
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true);
			}
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x0020C438 File Offset: 0x0020A638
		private void Update()
		{
			if (this.localPlayerOn && this.localGrabbedRigid)
			{
				float magnitude = this.localGrabbedRigid.linearVelocity.magnitude;
				if (magnitude > 2.5f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				float num = MathUtils.Linear(magnitude, 0f, 10f, -0.07f, 0.5f);
				if (num > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num, Time.deltaTime);
				}
			}
			if (!this.isIdle)
			{
				if (!this.localPlayerOn && this.remotePlayers.Count == 0)
				{
					foreach (Rigidbody rigidbody in this.bones)
					{
						float magnitude2 = rigidbody.linearVelocity.magnitude;
						float num2 = Time.deltaTime * this.settings.frictionWhenNotHeld;
						if (num2 < magnitude2 - 0.1f)
						{
							rigidbody.linearVelocity = Vector3.MoveTowards(rigidbody.linearVelocity, Vector3.zero, num2);
						}
					}
				}
				bool flag = false;
				for (int j = 0; j < this.bones.Length; j++)
				{
					if (this.bones[j].linearVelocity.magnitude > 0.1f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true);
					this.potentialIdleTimer = 0f;
				}
			}
		}

		// Token: 0x0600647D RID: 25725 RVA: 0x0020C5E4 File Offset: 0x0020A7E4
		private void SetIsIdle(bool idle)
		{
			this.isIdle = idle;
			this.ToggleIsKinematic(idle);
			if (idle)
			{
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].linearVelocity = Vector3.zero;
					this.bones[i].angularVelocity = Vector3.zero;
					this.bones[i].transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x0600647E RID: 25726 RVA: 0x0020C650 File Offset: 0x0020A850
		private void ToggleIsKinematic(bool kinematic)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].isKinematic = kinematic;
				if (kinematic)
				{
					this.bones[i].interpolation = 0;
				}
				else
				{
					this.bones[i].interpolation = 1;
				}
			}
		}

		// Token: 0x0600647F RID: 25727 RVA: 0x0020C69F File Offset: 0x0020A89F
		public Rigidbody GetBone(int index)
		{
			if (index >= this.bones.Length)
			{
				return Enumerable.Last<Rigidbody>(this.bones);
			}
			return this.bones[index];
		}

		// Token: 0x06006480 RID: 25728 RVA: 0x0020C6C0 File Offset: 0x0020A8C0
		public int GetBoneIndex(Rigidbody r)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i] == r)
				{
					return i;
				}
			}
			return this.bones.Length - 1;
		}

		// Token: 0x06006481 RID: 25729 RVA: 0x0020C6FC File Offset: 0x0020A8FC
		public void AttachLocalPlayer(XRNode xrNode, Rigidbody rigid, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(rigid);
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == 4);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Rigidbody rigidbody in this.bones)
				{
					list.Add(rigidbody.transform.localEulerAngles);
					list2.Add(rigidbody.linearVelocity);
				}
			}
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2f))
			{
				this.SetVelocity_RPC(boneIndex, velocity, true, list.ToArray(), list2.ToArray());
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 2)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.localGrabbedRigid = rigid;
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x0020C87F File Offset: 0x0020AA7F
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localGrabbedRigid = null;
		}

		// Token: 0x06006483 RID: 25731 RVA: 0x0020C8B8 File Offset: 0x0020AAB8
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Rigidbody bone = this.GetBone(boneIndex);
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
			return true;
		}

		// Token: 0x06006484 RID: 25732 RVA: 0x0020C91F File Offset: 0x0020AB1F
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
		}

		// Token: 0x06006485 RID: 25733 RVA: 0x0020C930 File Offset: 0x0020AB30
		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("SetVelocity", 0, new object[]
				{
					boneIndex,
					velocity,
					wholeRope,
					ropeRotations,
					ropeVelocities
				});
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities);
		}

		// Token: 0x06006486 RID: 25734 RVA: 0x0020C994 File Offset: 0x0020AB94
		[PunRPC]
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			this.SetIsIdle(false);
			if (ropeRotations != null && ropeVelocities != null && ropeRotations.Length != 0)
			{
				this.ToggleIsKinematic(true);
				for (int i = 0; i < ropeRotations.Length; i++)
				{
					if (i != 0)
					{
						this.bones[i].transform.localRotation = Quaternion.Euler(ropeRotations[i]);
						this.bones[i].linearVelocity = ropeVelocities[i];
					}
				}
				this.ToggleIsKinematic(false);
			}
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone)
			{
				if (wholeRope)
				{
					int num = 0;
					float num2 = Mathf.Min(velocity.magnitude, 15f);
					foreach (Rigidbody rigidbody in this.bones)
					{
						Vector3 vector = velocity / (float)boneIndex * (float)num;
						vector = Vector3.ClampMagnitude(vector, num2);
						rigidbody.linearVelocity = vector;
						num++;
					}
					return;
				}
				bone.linearVelocity = velocity;
			}
		}

		// Token: 0x0400742B RID: 29739
		public const float kPlayerMass = 0.8f;

		// Token: 0x0400742C RID: 29740
		public const float ropeBitGenOffset = 1f;

		// Token: 0x0400742D RID: 29741
		public const float MAX_ROPE_SPEED = 15f;

		// Token: 0x0400742E RID: 29742
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x0400742F RID: 29743
		public Rigidbody[] bones = Array.Empty<Rigidbody>();

		// Token: 0x04007430 RID: 29744
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x04007431 RID: 29745
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x04007432 RID: 29746
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x04007433 RID: 29747
		private bool localPlayerOn;

		// Token: 0x04007434 RID: 29748
		private XRNode localPlayerXRNode;

		// Token: 0x04007435 RID: 29749
		private Rigidbody localGrabbedRigid;

		// Token: 0x04007436 RID: 29750
		private const float MAX_VELOCITY_FOR_IDLE = 0.1f;

		// Token: 0x04007437 RID: 29751
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x04007439 RID: 29753
		private float potentialIdleTimer;

		// Token: 0x0400743A RID: 29754
		[Header("Config")]
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x0400743B RID: 29755
		[SerializeField]
		private GorillaRopeSwingSettings settings;
	}
}
