using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B0 RID: 4272
	public class PickupableCosmetic : PickupableVariant
	{
		// Token: 0x06006AE3 RID: 27363 RVA: 0x002310CC File Offset: 0x0022F2CC
		private void Awake()
		{
			this.rigOwnedPhysicsBody = base.GetComponent<RigOwnedPhysicsBody>();
			this.bodyCollider = base.GetComponent<Collider>();
		}

		// Token: 0x06006AE4 RID: 27364 RVA: 0x00035C6E File Offset: 0x00033E6E
		private void Start()
		{
			base.enabled = false;
		}

		// Token: 0x06006AE5 RID: 27365 RVA: 0x002310E6 File Offset: 0x0022F2E6
		private void OnEnable()
		{
			if (this.rigOwnedPhysicsBody != null)
			{
				this.rigOwnedPhysicsBody.enabled = true;
			}
		}

		// Token: 0x06006AE6 RID: 27366 RVA: 0x00231102 File Offset: 0x0022F302
		private void OnDisable()
		{
			if (this.rigOwnedPhysicsBody != null)
			{
				this.rigOwnedPhysicsBody.enabled = false;
			}
		}

		// Token: 0x06006AE7 RID: 27367 RVA: 0x00231120 File Offset: 0x0022F320
		protected internal override void Pickup(bool isAutoPickup = false)
		{
			if (!isAutoPickup)
			{
				UnityEvent onPickupShared = this.OnPickupShared;
				if (onPickupShared != null)
				{
					onPickupShared.Invoke();
				}
			}
			this.rb.linearVelocity = Vector3.zero;
			this.rb.isKinematic = true;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.placedOnFloorTime = -1f;
			this.placedOnFloor = false;
			this.broken = false;
			this.brokenTime = -1f;
			if (this.isBreakable && this.transferrableParent != null && this.transferrableParent.IsLocalObject())
			{
				int num = (int)this.transferrableParent.itemState;
				num &= ~PickupableCosmetic.breakableBitmask;
				this.transferrableParent.itemState = (TransferrableObject.ItemStates)num;
				if (this.breakEffect != null && this.breakEffect.isPlaying)
				{
					this.breakEffect.Stop();
				}
			}
			this.ShowRenderers(true);
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			base.enabled = false;
		}

		// Token: 0x06006AE8 RID: 27368 RVA: 0x00231272 File Offset: 0x0022F472
		protected internal override void DelayedPickup()
		{
			base.StartCoroutine(this.DelayedPickup_Internal());
		}

		// Token: 0x06006AE9 RID: 27369 RVA: 0x00231281 File Offset: 0x0022F481
		private IEnumerator DelayedPickup_Internal()
		{
			yield return new WaitForSeconds(1f);
			this.Pickup(false);
			yield break;
		}

		// Token: 0x06006AEA RID: 27370 RVA: 0x00231290 File Offset: 0x0022F490
		protected internal override void Release(HoldableObject holdable, Vector3 startPosition, Vector3 velocity, float playerScale)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.linearVelocity = velocity;
			this.rb.detectCollisions = true;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.scale = playerScale;
			base.enabled = true;
			this.transferrableParent = (this.holdableParent as TransferrableObject);
			this.currentRayIndex = 0;
			this.frameCounter = 0;
		}

		// Token: 0x06006AEB RID: 27371 RVA: 0x00231354 File Offset: 0x0022F554
		private void FixedUpdate()
		{
			if (this.isBreakable && this.broken)
			{
				if (Time.time > this.respawnDelay + this.brokenTime)
				{
					this.Pickup(false);
				}
				return;
			}
			if (this.isBreakable && this.placedOnFloor)
			{
				bool flag = (this.transferrableParent.itemState & (TransferrableObject.ItemStates)PickupableCosmetic.breakableBitmask) > (TransferrableObject.ItemStates)0;
				if (flag != this.broken && flag)
				{
					this.OnBreakReplicated();
				}
			}
			if (this.autoPickupAfterSeconds > 0f && this.placedOnFloor && Time.time - this.placedOnFloorTime > this.autoPickupAfterSeconds)
			{
				this.Pickup(true);
				ThrowablePickupableCosmetic throwablePickupableCosmetic = this.transferrableParent as ThrowablePickupableCosmetic;
				if (throwablePickupableCosmetic)
				{
					UnityEvent onReturnToDockPositionShared = throwablePickupableCosmetic.OnReturnToDockPositionShared;
					if (onReturnToDockPositionShared != null)
					{
						onReturnToDockPositionShared.Invoke();
					}
				}
			}
			if (this.autoPickupDistance > 0f && this.transferrableParent != null && (this.transferrableParent.ownerRig.transform.position - base.transform.position).IsLongerThan(this.autoPickupDistance))
			{
				this.Pickup(false);
			}
			if (!this.placedOnFloor && base.enabled)
			{
				this.frameCounter++;
				if (this.frameCounter % this.stepEveryNFrames != 0)
				{
					return;
				}
				float num = this.RaycastCheckDist * this.scale;
				int value = this.floorLayerMask.value;
				Vector3[] cachedDirections = this.GetCachedDirections(this.RaycastChecksMax);
				int num2 = 0;
				while (num2 < this.raysPerStep && this.currentRayIndex < cachedDirections.Length)
				{
					Vector3 vector = cachedDirections[this.currentRayIndex];
					this.currentRayIndex++;
					num2++;
					RaycastHit hitInfo;
					if (Physics.Raycast(this.GetSafeRayOrigin(this.raycastOrigin.position, vector), vector, ref hitInfo, num, value, 1) && (!this.dontStickToWall || Vector3.Angle(hitInfo.normal, Vector3.up) < 40f))
					{
						this.SettleBanner(hitInfo);
						UnityEvent onPlacedShared = this.OnPlacedShared;
						if (onPlacedShared != null)
						{
							onPlacedShared.Invoke();
						}
						this.placedOnFloor = true;
						this.placedOnFloorTime = Time.time;
						break;
					}
				}
				if (this.currentRayIndex >= cachedDirections.Length)
				{
					this.currentRayIndex = 0;
				}
			}
		}

		// Token: 0x06006AEC RID: 27372 RVA: 0x00231594 File Offset: 0x0022F794
		private void SettleBanner(RaycastHit hitInfo)
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
			this.rb.detectCollisions = false;
			Vector3 normal = hitInfo.normal;
			base.transform.position = hitInfo.point + normal * this.placementOffset;
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
			base.transform.rotation = rotation;
		}

		// Token: 0x06006AED RID: 27373 RVA: 0x0023161C File Offset: 0x0022F81C
		private Vector3 GetFibonacciSphereDirection(int index, int total)
		{
			float num = Mathf.Acos(1f - 2f * ((float)index + 0.5f) / (float)total);
			float num2 = 3.1415927f * (1f + Mathf.Sqrt(5f)) * ((float)index + 0.5f);
			float num3 = Mathf.Sin(num) * Mathf.Cos(num2);
			float num4 = Mathf.Sin(num) * Mathf.Sin(num2);
			float num5 = Mathf.Cos(num);
			return new Vector3(num3, num4, num5).normalized;
		}

		// Token: 0x06006AEE RID: 27374 RVA: 0x00231698 File Offset: 0x0022F898
		private Vector3[] GetCachedDirections(int count)
		{
			if (count <= 0)
			{
				return PickupableCosmetic.tmpEmpty;
			}
			Vector3[] array;
			if (PickupableCosmetic.directionCache.TryGetValue(count, ref array))
			{
				return array;
			}
			array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.GetFibonacciSphereDirection(i, count);
			}
			PickupableCosmetic.directionCache[count] = array;
			return array;
		}

		// Token: 0x06006AEF RID: 27375 RVA: 0x002316F0 File Offset: 0x0022F8F0
		private Vector3 GetSafeRayOrigin(Vector3 rawOrigin, Vector3 dir)
		{
			float num = this.selfSkinOffset;
			if (this.bodyCollider != null)
			{
				float magnitude = this.bodyCollider.bounds.extents.magnitude;
				num = Mathf.Max(this.selfSkinOffset, magnitude * 0.05f);
			}
			return rawOrigin - dir.normalized * num;
		}

		// Token: 0x06006AF0 RID: 27376 RVA: 0x00231754 File Offset: 0x0022F954
		public void BreakPlaceable()
		{
			if (!this.isBreakable || !this.placedOnFloor)
			{
				return;
			}
			if (this.transferrableParent != null && this.transferrableParent.IsLocalObject())
			{
				int num = (int)this.transferrableParent.itemState;
				num |= PickupableCosmetic.breakableBitmask;
				this.transferrableParent.itemState = (TransferrableObject.ItemStates)num;
				return;
			}
			GTDev.LogError<string>("PickupableCosmetic " + base.gameObject.name + " has no TransferrableObject parent. Break effects cannot be replicated", null);
		}

		// Token: 0x06006AF1 RID: 27377 RVA: 0x002317CE File Offset: 0x0022F9CE
		private void OnBreakReplicated()
		{
			this.PlayBreakEffects();
		}

		// Token: 0x06006AF2 RID: 27378 RVA: 0x002317D8 File Offset: 0x0022F9D8
		protected virtual void PlayBreakEffects()
		{
			if (!this.isBreakable || !this.placedOnFloor || this.broken)
			{
				return;
			}
			this.broken = true;
			this.brokenTime = Time.time;
			if (this.breakEffect != null)
			{
				if (this.breakEffect.isPlaying)
				{
					this.breakEffect.Stop();
				}
				this.breakEffect.Play();
			}
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.ShowRenderers(false);
			UnityEvent onBrokenShared = this.OnBrokenShared;
			if (onBrokenShared == null)
			{
				return;
			}
			onBrokenShared.Invoke();
		}

		// Token: 0x06006AF3 RID: 27379 RVA: 0x00231874 File Offset: 0x0022FA74
		protected virtual void ShowRenderers(bool visible)
		{
			if (this.hideOnBreak.IsNullOrEmpty<Renderer>())
			{
				return;
			}
			for (int i = 0; i < this.hideOnBreak.Length; i++)
			{
				Renderer renderer = this.hideOnBreak[i];
				if (!(renderer == null))
				{
					renderer.forceRenderingOff = !visible;
				}
			}
		}

		// Token: 0x04007B20 RID: 31520
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04007B21 RID: 31521
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04007B22 RID: 31522
		[SerializeField]
		private Transform raycastOrigin;

		// Token: 0x04007B23 RID: 31523
		[Tooltip("Allow player to grab the placed object")]
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04007B24 RID: 31524
		[SerializeField]
		private float autoPickupAfterSeconds;

		// Token: 0x04007B25 RID: 31525
		[SerializeField]
		private float autoPickupDistance;

		// Token: 0x04007B26 RID: 31526
		[Tooltip("Amount to offset the placed object from the hit position in the hit normal direction")]
		[SerializeField]
		private float placementOffset;

		// Token: 0x04007B27 RID: 31527
		[Tooltip("Prevent sticking if the hit surface normal is not within 40 degrees of world up")]
		[SerializeField]
		private bool dontStickToWall;

		// Token: 0x04007B28 RID: 31528
		[Tooltip("Layers to raycast against for placement")]
		[SerializeField]
		private LayerMask floorLayerMask = 134218241;

		// Token: 0x04007B29 RID: 31529
		[Tooltip("The distance to check if the banner is close to the floor (from a raycast check).")]
		public float RaycastCheckDist = 0.2f;

		// Token: 0x04007B2A RID: 31530
		[Tooltip("How many checks should we attempt for a raycast.")]
		public int RaycastChecksMax = 12;

		// Token: 0x04007B2B RID: 31531
		[FormerlySerializedAs("OnPickup")]
		[Space]
		public UnityEvent OnPickupShared;

		// Token: 0x04007B2C RID: 31532
		[FormerlySerializedAs("OnPlaced")]
		public UnityEvent OnPlacedShared;

		// Token: 0x04007B2D RID: 31533
		[SerializeField]
		private bool isBreakable;

		// Token: 0x04007B2E RID: 31534
		[Tooltip("Particle system played OnBrokenShared")]
		[SerializeField]
		private ParticleSystem breakEffect;

		// Token: 0x04007B2F RID: 31535
		[Tooltip("Renderers disabled OnBrokenShared and enabled OnPickupShared")]
		[SerializeField]
		private Renderer[] hideOnBreak = new Renderer[0];

		// Token: 0x04007B30 RID: 31536
		[Tooltip("Time after BreakPlaceable to reset item")]
		[SerializeField]
		private float respawnDelay = 0.5f;

		// Token: 0x04007B31 RID: 31537
		[FormerlySerializedAs("OnBroken")]
		[Space]
		public UnityEvent OnBrokenShared;

		// Token: 0x04007B32 RID: 31538
		private static int breakableBitmask = 32;

		// Token: 0x04007B33 RID: 31539
		private bool placedOnFloor;

		// Token: 0x04007B34 RID: 31540
		private float placedOnFloorTime = -1f;

		// Token: 0x04007B35 RID: 31541
		private bool broken;

		// Token: 0x04007B36 RID: 31542
		private float brokenTime = -1f;

		// Token: 0x04007B37 RID: 31543
		private VRRig cachedLocalRig;

		// Token: 0x04007B38 RID: 31544
		private HoldableObject holdableParent;

		// Token: 0x04007B39 RID: 31545
		private TransferrableObject transferrableParent;

		// Token: 0x04007B3A RID: 31546
		private RigOwnedPhysicsBody rigOwnedPhysicsBody;

		// Token: 0x04007B3B RID: 31547
		private double throwSettledTime = -1.0;

		// Token: 0x04007B3C RID: 31548
		private int landingSide;

		// Token: 0x04007B3D RID: 31549
		private float scale;

		// Token: 0x04007B3E RID: 31550
		private Collider bodyCollider;

		// Token: 0x04007B3F RID: 31551
		[Tooltip("How many directions to test per physics tick (spreads work across frames).")]
		[SerializeField]
		[Min(1f)]
		private int raysPerStep = 3;

		// Token: 0x04007B40 RID: 31552
		[Tooltip("Run a raycast step only every N physics ticks (1 = every FixedUpdate).")]
		[SerializeField]
		[Min(1f)]
		private int stepEveryNFrames = 2;

		// Token: 0x04007B41 RID: 31553
		[Tooltip("Small skin so rays start just outside our own collider volume.")]
		[SerializeField]
		[Range(0.005f, 0.1f)]
		private float selfSkinOffset = 0.02f;

		// Token: 0x04007B42 RID: 31554
		[SerializeField]
		private bool debugPlacementRays;

		// Token: 0x04007B43 RID: 31555
		private int currentRayIndex;

		// Token: 0x04007B44 RID: 31556
		private int frameCounter;

		// Token: 0x04007B45 RID: 31557
		private static readonly Dictionary<int, Vector3[]> directionCache = new Dictionary<int, Vector3[]>();

		// Token: 0x04007B46 RID: 31558
		private static readonly Vector3[] tmpEmpty = Array.Empty<Vector3>();
	}
}
