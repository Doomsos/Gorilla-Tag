using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B5 RID: 4277
	public class StickObjectToPlayer : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06006B19 RID: 27417 RVA: 0x00231EBE File Offset: 0x002300BE
		// (set) Token: 0x06006B1A RID: 27418 RVA: 0x00231EC6 File Offset: 0x002300C6
		public bool TickRunning { get; set; }

		// Token: 0x06006B1B RID: 27419 RVA: 0x00231ECF File Offset: 0x002300CF
		public void Tick()
		{
			if (!this.canSpawn && Time.time - this.lastSpawnedTime >= this.cooldown)
			{
				this.canSpawn = true;
			}
		}

		// Token: 0x06006B1C RID: 27420 RVA: 0x00231EF4 File Offset: 0x002300F4
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			this.canSpawn = true;
		}

		// Token: 0x06006B1D RID: 27421 RVA: 0x00018787 File Offset: 0x00016987
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006B1E RID: 27422 RVA: 0x00231F03 File Offset: 0x00230103
		public void SetOwner(NetPlayer player)
		{
			this.ownerPlayer = player;
		}

		// Token: 0x06006B1F RID: 27423 RVA: 0x00231F0C File Offset: 0x0023010C
		private Transform MakeOrGetStickyContainer(Transform parent)
		{
			Transform transform = parent;
			foreach (Transform transform2 in parent.GetComponentsInChildren<Transform>(true))
			{
				if (!this.firstPersonView && transform2.CompareTag(this.parentTag))
				{
					transform = transform2;
					break;
				}
			}
			string text = "StickyObjects_" + this.objectToSpawn.name;
			Transform transform3 = transform.Find(text);
			if (transform3 != null)
			{
				return transform3;
			}
			GameObject gameObject = new GameObject(text);
			gameObject.transform.SetParent(transform, false);
			return gameObject.transform;
		}

		// Token: 0x06006B20 RID: 27424 RVA: 0x00231F98 File Offset: 0x00230198
		public void Stick(bool leftHand, Collider other)
		{
			if (!this.canSpawn || other == null || !base.enabled)
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (!componentInParent)
			{
				return;
			}
			if (this.ownerPlayer != null && componentInParent.creator == this.ownerPlayer)
			{
				return;
			}
			Vector3 vector = (this.spawnerRigidbody != null) ? this.spawnerRigidbody.linearVelocity : Vector3.zero;
			Vector3 vector2 = Time.fixedDeltaTime * 2f * vector;
			Vector3 vector3 = vector2.normalized;
			if (vector3 == Vector3.zero)
			{
				vector3 = base.transform.forward;
				vector2 = vector3 * 0.01f;
			}
			Vector3 vector4 = base.transform.position - vector2;
			Vector3 vector5;
			if (this.alignToHitNormal)
			{
				float magnitude = vector2.magnitude;
				RaycastHit raycastHit;
				if (other.Raycast(new Ray(vector4, vector3), ref raycastHit, 2f * magnitude))
				{
					vector5 = raycastHit.point;
				}
				else
				{
					vector5 = other.ClosestPoint(vector4);
				}
			}
			else
			{
				vector5 = other.ClosestPoint(vector4);
			}
			Vector3 vector6 = this.GetSpawnPosition(this.spawnLocation, componentInParent).TransformPoint(this.positionOffset);
			if ((vector5 - vector6).magnitude <= this.stickRadius * componentInParent.scaleFactor)
			{
				if (NetworkSystem.Instance.LocalPlayer == componentInParent.creator)
				{
					if (this.firstPersonView && this.spawnLocation == StickObjectToPlayer.SpawnLocation.Head)
					{
						this.StickFirstPersonView();
					}
				}
				else
				{
					if (!this.thirdPersonView)
					{
						return;
					}
					Transform parent = this.MakeOrGetStickyContainer(componentInParent.transform);
					this.StickTo(parent, vector6, this.localEulerAngles);
				}
				UnityEvent onStickShared = this.OnStickShared;
				if (onStickShared == null)
				{
					return;
				}
				onStickShared.Invoke();
			}
		}

		// Token: 0x06006B21 RID: 27425 RVA: 0x00232144 File Offset: 0x00230344
		private void StickFirstPersonView()
		{
			Transform cosmeticsHeadTarget = GTPlayer.Instance.CosmeticsHeadTarget;
			Vector3 position = cosmeticsHeadTarget.TransformPoint(this.FPVOffset);
			Transform parent = this.MakeOrGetStickyContainer(cosmeticsHeadTarget);
			this.StickTo(parent, position, this.FPVlocalEulerAngles);
		}

		// Token: 0x06006B22 RID: 27426 RVA: 0x00232180 File Offset: 0x00230380
		private void StickTo(Transform parent, Vector3 position, Vector3 eulerAngle)
		{
			int num = 0;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i).gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			if (num >= this.maxActiveStickies)
			{
				return;
			}
			this.stickyObject = ObjectPools.instance.Instantiate(this.objectToSpawn, true);
			if (this.stickyObject == null)
			{
				return;
			}
			this.stickyObject.transform.SetParent(parent, false);
			this.stickyObject.transform.position = position;
			this.stickyObject.transform.localEulerAngles = eulerAngle;
			this.lastSpawnedTime = Time.time;
			this.canSpawn = false;
		}

		// Token: 0x06006B23 RID: 27427 RVA: 0x00232230 File Offset: 0x00230430
		private Transform GetSpawnPosition(StickObjectToPlayer.SpawnLocation spawnType, VRRig hitRig)
		{
			switch (spawnType)
			{
			case StickObjectToPlayer.SpawnLocation.Head:
				return hitRig.head.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.RightHand:
				return hitRig.rightHand.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.LeftHand:
				return hitRig.leftHand.rigTarget.transform;
			default:
				return null;
			}
		}

		// Token: 0x06006B24 RID: 27428 RVA: 0x00232288 File Offset: 0x00230488
		public void Debug_StickToLocalPlayer()
		{
			Vector3 position = this.GetSpawnPosition(this.spawnLocation, VRRig.LocalRig).TransformPoint(this.positionOffset);
			this.StickTo(VRRig.LocalRig.transform, position, this.localEulerAngles);
		}

		// Token: 0x06006B25 RID: 27429 RVA: 0x002322C9 File Offset: 0x002304C9
		public void Debug_StickToLocalPlayerFPV()
		{
			this.StickFirstPersonView();
		}

		// Token: 0x04007B65 RID: 31589
		[Header("Shared Settings")]
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject objectToSpawn;

		// Token: 0x04007B66 RID: 31590
		[Tooltip("Optional: how many objects can be active at once")]
		[SerializeField]
		private int maxActiveStickies = 1;

		// Token: 0x04007B67 RID: 31591
		[SerializeField]
		private StickObjectToPlayer.SpawnLocation spawnLocation;

		// Token: 0x04007B68 RID: 31592
		[SerializeField]
		private float stickRadius = 0.5f;

		// Token: 0x04007B69 RID: 31593
		[SerializeField]
		private bool alignToHitNormal = true;

		// Token: 0x04007B6A RID: 31594
		[SerializeField]
		private Rigidbody spawnerRigidbody;

		// Token: 0x04007B6B RID: 31595
		[SerializeField]
		private string parentTag = "GorillaHead";

		// Token: 0x04007B6C RID: 31596
		[SerializeField]
		private float cooldown;

		// Token: 0x04007B6D RID: 31597
		[Header("Third Person View")]
		[Tooltip("If you are only interested in the FPV, don't check this box so that others don't see it.")]
		[SerializeField]
		private bool thirdPersonView = true;

		// Token: 0x04007B6E RID: 31598
		[SerializeField]
		private Vector3 positionOffset = new Vector3(0f, 0.02f, 0.17f);

		// Token: 0x04007B6F RID: 31599
		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 localEulerAngles = Vector3.zero;

		// Token: 0x04007B70 RID: 31600
		[Header("First Person View")]
		[SerializeField]
		private bool firstPersonView;

		// Token: 0x04007B71 RID: 31601
		[SerializeField]
		private Vector3 FPVOffset = new Vector3(0f, 0.02f, 0.17f);

		// Token: 0x04007B72 RID: 31602
		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 FPVlocalEulerAngles = Vector3.zero;

		// Token: 0x04007B73 RID: 31603
		[Header("Events")]
		public UnityEvent OnStickShared;

		// Token: 0x04007B74 RID: 31604
		private GameObject stickyObject;

		// Token: 0x04007B75 RID: 31605
		private float lastSpawnedTime;

		// Token: 0x04007B76 RID: 31606
		private bool canSpawn = true;

		// Token: 0x04007B77 RID: 31607
		private NetPlayer ownerPlayer;

		// Token: 0x020010B6 RID: 4278
		private enum SpawnLocation
		{
			// Token: 0x04007B7A RID: 31610
			Head,
			// Token: 0x04007B7B RID: 31611
			RightHand,
			// Token: 0x04007B7C RID: 31612
			LeftHand
		}
	}
}
