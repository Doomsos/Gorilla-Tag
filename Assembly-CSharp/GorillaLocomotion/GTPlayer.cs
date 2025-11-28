using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AA;
using BoingKit;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaLocomotion
{
	// Token: 0x02000F7D RID: 3965
	public class GTPlayer : MonoBehaviour
	{
		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06006317 RID: 25367 RVA: 0x001FEAB3 File Offset: 0x001FCCB3
		public static GTPlayer Instance
		{
			get
			{
				return GTPlayer._instance;
			}
		}

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06006318 RID: 25368 RVA: 0x001FEABA File Offset: 0x001FCCBA
		public GTPlayer.HandState LeftHand
		{
			get
			{
				return this.leftHand;
			}
		}

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06006319 RID: 25369 RVA: 0x001FEAC2 File Offset: 0x001FCCC2
		public GTPlayer.HandState RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x001FEACA File Offset: 0x001FCCCA
		public int GetMaterialTouchIndex(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).materialTouchIndex;
		}

		// Token: 0x0600631B RID: 25371 RVA: 0x001FEAE2 File Offset: 0x001FCCE2
		public GorillaSurfaceOverride GetSurfaceOverride(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).surfaceOverride;
		}

		// Token: 0x0600631C RID: 25372 RVA: 0x001FEAFA File Offset: 0x001FCCFA
		public RaycastHit GetTouchHitInfo(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).hitInfo;
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x001FEB12 File Offset: 0x001FCD12
		public bool IsHandTouching(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).wasColliding;
		}

		// Token: 0x0600631E RID: 25374 RVA: 0x001FEB2A File Offset: 0x001FCD2A
		public GorillaVelocityTracker GetHandVelocityTracker(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).velocityTracker;
		}

		// Token: 0x0600631F RID: 25375 RVA: 0x001FEB42 File Offset: 0x001FCD42
		public GorillaVelocityTracker GetInteractPointVelocityTracker(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).interactPointVelocityTracker;
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x001FEB5A File Offset: 0x001FCD5A
		public Transform GetControllerTransform(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).controllerTransform;
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x001FEB72 File Offset: 0x001FCD72
		public Transform GetHandFollower(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handFollower;
		}

		// Token: 0x06006322 RID: 25378 RVA: 0x001FEB8A File Offset: 0x001FCD8A
		public Vector3 GetHandOffset(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handOffset;
		}

		// Token: 0x06006323 RID: 25379 RVA: 0x001FEBA2 File Offset: 0x001FCDA2
		public Quaternion GetHandRotOffset(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handRotOffset;
		}

		// Token: 0x06006324 RID: 25380 RVA: 0x001FEBBA File Offset: 0x001FCDBA
		public Vector3 GetHandPosition(bool isLeftHand, StiltID stiltID = StiltID.None)
		{
			return ((stiltID != StiltID.None) ? this.stiltStates[(int)stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).lastPosition;
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x001FEBE4 File Offset: 0x001FCDE4
		public void GetHandTapData(bool isLeftHand, StiltID stiltID, out bool wasHandTouching, out bool wasSliding, out int handMatIndex, out GorillaSurfaceOverride surfaceOverride, out RaycastHit handHitInfo, out Vector3 handPosition, out GorillaVelocityTracker handVelocityTracker)
		{
			((stiltID != StiltID.None) ? this.stiltStates[(int)stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).GetHandTapData(out wasHandTouching, out wasSliding, out handMatIndex, out surfaceOverride, out handHitInfo, out handPosition, out handVelocityTracker);
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x001FEC29 File Offset: 0x001FCE29
		public void SetHandOffsets(bool isLeftHand, Vector3 handOffset, Quaternion handRotOffset)
		{
			if (isLeftHand)
			{
				this.leftHand.handOffset = handOffset;
				this.leftHand.handRotOffset = handRotOffset;
				return;
			}
			this.rightHand.handOffset = handOffset;
			this.rightHand.handRotOffset = handRotOffset;
		}

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06006327 RID: 25383 RVA: 0x001FEC5F File Offset: 0x001FCE5F
		public Vector3 InstantaneousVelocity
		{
			get
			{
				return this.currentVelocity;
			}
		}

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06006328 RID: 25384 RVA: 0x001FEC67 File Offset: 0x001FCE67
		public Vector3 AveragedVelocity
		{
			get
			{
				return this.averagedVelocity;
			}
		}

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x06006329 RID: 25385 RVA: 0x001FEC6F File Offset: 0x001FCE6F
		public Transform CosmeticsHeadTarget
		{
			get
			{
				return this.cosmeticsHeadTarget;
			}
		}

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x0600632A RID: 25386 RVA: 0x001FEC77 File Offset: 0x001FCE77
		public float scale
		{
			get
			{
				return this.scaleMultiplier * this.nativeScale;
			}
		}

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x0600632B RID: 25387 RVA: 0x001FEC86 File Offset: 0x001FCE86
		public float NativeScale
		{
			get
			{
				return this.nativeScale;
			}
		}

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x0600632C RID: 25388 RVA: 0x001FEC8E File Offset: 0x001FCE8E
		public float ScaleMultiplier
		{
			get
			{
				return this.scaleMultiplier;
			}
		}

		// Token: 0x0600632D RID: 25389 RVA: 0x001FEC96 File Offset: 0x001FCE96
		public void SetScaleMultiplier(float s)
		{
			this.scaleMultiplier = s;
		}

		// Token: 0x0600632E RID: 25390 RVA: 0x001FECA0 File Offset: 0x001FCEA0
		public void SetNativeScale(NativeSizeChangerSettings s)
		{
			float num = this.nativeScale;
			if (s != null && s.playerSizeScale > 0f && s.playerSizeScale != 1f)
			{
				this.activeSizeChangerSettings = s;
			}
			else
			{
				this.activeSizeChangerSettings = null;
			}
			if (this.activeSizeChangerSettings == null)
			{
				this.nativeScale = 1f;
			}
			else
			{
				this.nativeScale = this.activeSizeChangerSettings.playerSizeScale;
			}
			if (num != this.nativeScale && NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig != null;
			}
		}

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x0600632F RID: 25391 RVA: 0x001FED2B File Offset: 0x001FCF2B
		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06006330 RID: 25392 RVA: 0x001FED45 File Offset: 0x001FCF45
		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x06006331 RID: 25393 RVA: 0x001FED57 File Offset: 0x001FCF57
		public List<GTPlayer.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x06006332 RID: 25394 RVA: 0x001FED64 File Offset: 0x001FCF64
		// (set) Token: 0x06006333 RID: 25395 RVA: 0x001FED6C File Offset: 0x001FCF6C
		protected bool IsFrozen { get; set; }

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x06006334 RID: 25396 RVA: 0x001FED75 File Offset: 0x001FCF75
		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x06006335 RID: 25397 RVA: 0x001FED7D File Offset: 0x001FCF7D
		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x06006336 RID: 25398 RVA: 0x001FED85 File Offset: 0x001FCF85
		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06006337 RID: 25399 RVA: 0x001FED8D File Offset: 0x001FCF8D
		public WaterVolume CurrentWaterVolume
		{
			get
			{
				if (this.bodyOverlappingWaterVolumes.Count <= 0)
				{
					return null;
				}
				return this.bodyOverlappingWaterVolumes[0];
			}
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06006338 RID: 25400 RVA: 0x001FEDAB File Offset: 0x001FCFAB
		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06006339 RID: 25401 RVA: 0x001FEDB3 File Offset: 0x001FCFB3
		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x0600633A RID: 25402 RVA: 0x001FEDBB File Offset: 0x001FCFBB
		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x0600633B RID: 25403 RVA: 0x001FEDC3 File Offset: 0x001FCFC3
		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x0600633C RID: 25404 RVA: 0x001FEDCB File Offset: 0x001FCFCB
		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x0600633D RID: 25405 RVA: 0x001FEDD3 File Offset: 0x001FCFD3
		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.leftHand.lastPosition;
			}
		}

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x0600633E RID: 25406 RVA: 0x001FEDE0 File Offset: 0x001FCFE0
		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.rightHand.lastPosition;
			}
		}

		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x0600633F RID: 25407 RVA: 0x001FEDED File Offset: 0x001FCFED
		public Vector3 RigidbodyVelocity
		{
			get
			{
				return this.playerRigidBody.linearVelocity;
			}
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06006340 RID: 25408 RVA: 0x001FEDFA File Offset: 0x001FCFFA
		public Vector3 HeadCenterPosition
		{
			get
			{
				return this.headCollider.transform.position + this.headCollider.transform.rotation * new Vector3(0f, 0f, -0.11f);
			}
		}

		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06006341 RID: 25409 RVA: 0x001FEE3A File Offset: 0x001FD03A
		public bool HandContactingSurface
		{
			get
			{
				return this.leftHand.isColliding || this.rightHand.isColliding;
			}
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06006342 RID: 25410 RVA: 0x001FEE56 File Offset: 0x001FD056
		public bool BodyOnGround
		{
			get
			{
				return this.bodyGroundContactTime >= Time.time - 0.05f;
			}
		}

		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06006343 RID: 25411 RVA: 0x001FEE6E File Offset: 0x001FD06E
		public bool IsGroundedHand
		{
			get
			{
				return this.HandContactingSurface || this.isClimbing || this.leftHand.isHolding || this.rightHand.isHolding;
			}
		}

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x06006344 RID: 25412 RVA: 0x001FEE9A File Offset: 0x001FD09A
		public bool IsGroundedButt
		{
			get
			{
				return this.BodyOnGround;
			}
		}

		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x06006345 RID: 25413 RVA: 0x001FEEA2 File Offset: 0x001FD0A2
		// (set) Token: 0x06006346 RID: 25414 RVA: 0x001FEEAA File Offset: 0x001FD0AA
		public int ThrusterActiveAtFrame { get; set; }

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x06006347 RID: 25415 RVA: 0x001FEEB3 File Offset: 0x001FD0B3
		public bool IsThrusterActive
		{
			get
			{
				return this.ThrusterActiveAtFrame == Time.frameCount;
			}
		}

		// Token: 0x1700096C RID: 2412
		// (set) Token: 0x06006348 RID: 25416 RVA: 0x001FEEC2 File Offset: 0x001FD0C2
		public Quaternion PlayerRotationOverride
		{
			set
			{
				this.playerRotationOverride = value;
				this.playerRotationOverrideFrame = Time.frameCount;
			}
		}

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x06006349 RID: 25417 RVA: 0x001FEED6 File Offset: 0x001FD0D6
		// (set) Token: 0x0600634A RID: 25418 RVA: 0x001FEEDE File Offset: 0x001FD0DE
		public bool IsBodySliding { get; set; }

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x0600634B RID: 25419 RVA: 0x001FEEE7 File Offset: 0x001FD0E7
		public GorillaClimbable CurrentClimbable
		{
			get
			{
				return this.currentClimbable;
			}
		}

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x0600634C RID: 25420 RVA: 0x001FEEEF File Offset: 0x001FD0EF
		public GorillaHandClimber CurrentClimber
		{
			get
			{
				return this.currentClimber;
			}
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x0600634D RID: 25421 RVA: 0x001FEEF7 File Offset: 0x001FD0F7
		// (set) Token: 0x0600634E RID: 25422 RVA: 0x001FEEFF File Offset: 0x001FD0FF
		public float jumpMultiplier
		{
			get
			{
				return this._jumpMultiplier;
			}
			set
			{
				this._jumpMultiplier = value;
			}
		}

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x0600634F RID: 25423 RVA: 0x001FEF08 File Offset: 0x001FD108
		// (set) Token: 0x06006350 RID: 25424 RVA: 0x001FEF10 File Offset: 0x001FD110
		public float LastTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06006351 RID: 25425 RVA: 0x001FEF19 File Offset: 0x001FD119
		// (set) Token: 0x06006352 RID: 25426 RVA: 0x001FEF21 File Offset: 0x001FD121
		public float LastHandTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x06006353 RID: 25427 RVA: 0x001FEF2C File Offset: 0x001FD12C
		public void EnableStilt(StiltID stiltID, bool isLeftHand, Vector3 currentTipWorldPos, float maxArmLength, bool canTag, bool canStun, float customBoostFactor = 0f, GorillaVelocityTracker velocityTracker = null)
		{
			this.stiltStates[(int)stiltID] = new GTPlayer.HandState
			{
				isActive = true,
				controllerTransform = (isLeftHand ? this.leftHand : this.rightHand).controllerTransform,
				velocityTracker = ((velocityTracker != null) ? velocityTracker : (isLeftHand ? this.leftHand : this.rightHand).velocityTracker),
				handRotOffset = Quaternion.identity,
				canTag = canTag,
				canStun = canStun,
				customBoostFactor = customBoostFactor,
				hasCustomBoost = (customBoostFactor > 0f)
			};
			this.stiltStates[(int)stiltID].Init(this, isLeftHand, maxArmLength);
			this.UpdateStiltOffset(stiltID, currentTipWorldPos);
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x001FEFF2 File Offset: 0x001FD1F2
		public void DisableStilt(StiltID stiltID)
		{
			this.stiltStates[(int)stiltID].isActive = false;
		}

		// Token: 0x06006355 RID: 25429 RVA: 0x001FF006 File Offset: 0x001FD206
		public void UpdateStiltOffset(StiltID stiltID, Vector3 currentTipWorldPos)
		{
			this.stiltStates[(int)stiltID].handOffset = this.stiltStates[(int)stiltID].controllerTransform.InverseTransformPoint(currentTipWorldPos);
		}

		// Token: 0x06006356 RID: 25430 RVA: 0x001FF030 File Offset: 0x001FD230
		private void Awake()
		{
			if (GTPlayer._instance != null && GTPlayer._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				GTPlayer._instance = this;
				GTPlayer.hasInstance = true;
			}
			this.InitializeValues();
			this.playerRigidbodyInterpolationDefault = this.playerRigidBody.interpolation;
			this.playerRigidBody.maxAngularVelocity = 0f;
			this.bodyOffsetVector = new Vector3(0f, -this.bodyCollider.height / 2f, 0f);
			this.bodyInitialHeight = this.bodyCollider.height;
			this.bodyInitialRadius = this.bodyCollider.radius;
			this.rayCastNonAllocColliders = new RaycastHit[5];
			this.crazyCheckVectors = new Vector3[7];
			this.emptyHit = default(RaycastHit);
			this.crazyCheckVectors[0] = Vector3.up;
			this.crazyCheckVectors[1] = Vector3.down;
			this.crazyCheckVectors[2] = Vector3.left;
			this.crazyCheckVectors[3] = Vector3.right;
			this.crazyCheckVectors[4] = Vector3.forward;
			this.crazyCheckVectors[5] = Vector3.back;
			this.crazyCheckVectors[6] = Vector3.zero;
			if (this.controllerState == null)
			{
				this.controllerState = base.GetComponent<ConnectedControllerHandler>();
			}
			this.layerChanger = base.GetComponent<LayerChanger>();
			this.bodyTouchedSurfaces = new Dictionary<GameObject, PhysicsMaterial>();
			if (Application.isPlaying)
			{
				Application.onBeforeRender += new UnityAction(this.OnBeforeRenderInit);
			}
		}

		// Token: 0x06006357 RID: 25431 RVA: 0x001FF1C4 File Offset: 0x001FD3C4
		protected void Start()
		{
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.farClipPlane = 500f;
			this.lastScale = this.scale;
			this.layerChanger.InitializeLayers(base.transform);
			float degrees = Quaternion.Angle(Quaternion.identity, GorillaTagger.Instance.offlineVRRig.transform.rotation) * Mathf.Sign(Vector3.Dot(Vector3.up, GorillaTagger.Instance.offlineVRRig.transform.right));
			this.Turn(degrees);
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x001FF261 File Offset: 0x001FD461
		protected void OnDestroy()
		{
			if (GTPlayer._instance == this)
			{
				GTPlayer._instance = null;
				GTPlayer.hasInstance = false;
			}
			if (this.climbHelper)
			{
				Object.Destroy(this.climbHelper.gameObject);
			}
		}

		// Token: 0x06006359 RID: 25433 RVA: 0x001FF29C File Offset: 0x001FD49C
		public void InitializeValues()
		{
			Physics.SyncTransforms();
			this.playerRigidBody = base.GetComponent<Rigidbody>();
			this.velocityHistory = new Vector3[this.velocityHistorySize];
			this.slideAverageHistory = new Vector3[this.velocityHistorySize];
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Vector3.zero;
				this.slideAverageHistory[i] = Vector3.zero;
			}
			this.leftHand.Init(this, true, this.maxArmLength);
			this.rightHand.Init(this, false, this.maxArmLength);
			this.lastHeadPosition = this.headCollider.transform.position;
			this.velocityIndex = 0;
			this.averagedVelocity = Vector3.zero;
			this.slideVelocity = Vector3.zero;
			this.lastPosition = base.transform.position;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			this.ForceRigidBodySync();
		}

		// Token: 0x0600635A RID: 25434 RVA: 0x001FF408 File Offset: 0x001FD608
		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x001FF439 File Offset: 0x001FD639
		public void TeleportToTrain(bool enable)
		{
			this.teleportToTrain = enable;
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x001FF444 File Offset: 0x001FD644
		public void TeleportTo(Vector3 position, Quaternion rotation, bool keepVelocity = false, bool center = false)
		{
			if (center)
			{
				Vector3 position2 = base.transform.position;
				Vector3 vector = this.mainCamera.transform.position - position2;
				position -= vector;
			}
			this.ClearHandHolds();
			if (this.playerRigidBody != null)
			{
				this.playerRigidBody.isKinematic = true;
				this.playerRigidBody.position = position;
				this.playerRigidBody.rotation = rotation;
				this.playerRigidBody.isKinematic = false;
			}
			this.playerRigidBody.position = position;
			this.playerRigidBody.rotation = rotation;
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.lastPosition = position;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.leftHand.OnTeleport();
			this.rightHand.OnTeleport();
			if (!keepVelocity)
			{
				this.playerRigidBody.linearVelocity = Vector3.zero;
			}
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			Physics.SyncTransforms();
			GorillaTagger.Instance.offlineVRRig.transform.position = position;
			GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
			GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
			this.ForceRigidBodySync();
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x001FF600 File Offset: 0x001FD800
		public void TeleportTo(Transform destination, bool matchDestinationRotation = true, bool maintainVelocity = true)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = this.mainCamera.transform.position - position;
			Vector3 position2 = destination.position - vector;
			float num = destination.rotation.eulerAngles.y - this.mainCamera.transform.rotation.eulerAngles.y;
			Vector3 playerVelocity = this.currentVelocity;
			if (!maintainVelocity)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			else if (matchDestinationRotation)
			{
				playerVelocity = Quaternion.AngleAxis(num, base.transform.up) * this.currentVelocity;
				this.SetPlayerVelocity(playerVelocity);
			}
			if (matchDestinationRotation)
			{
				this.Turn(num);
			}
			this.TeleportTo(position2, base.transform.rotation, false, false);
			if (maintainVelocity)
			{
				this.SetPlayerVelocity(playerVelocity);
			}
			this.ForceRigidBodySync();
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x001FF6E4 File Offset: 0x001FD8E4
		public void AddForce(Vector3 force, ForceMode mode)
		{
			if (mode == 2)
			{
				this.playerRigidBody.AddForce(force * this.playerRigidBody.mass, 1);
				return;
			}
			if (mode == 5)
			{
				this.playerRigidBody.AddForce(force * this.playerRigidBody.mass, 0);
				return;
			}
			this.playerRigidBody.AddForce(force, mode);
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x001FF744 File Offset: 0x001FD944
		public void SetPlayerVelocity(Vector3 newVelocity)
		{
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = newVelocity;
			}
			this.playerRigidBody.AddForce(newVelocity - this.playerRigidBody.linearVelocity, 2);
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x001FF78E File Offset: 0x001FD98E
		public void SetGravityOverride(Object caller, Action<GTPlayer> gravityFunction)
		{
			this.gravityOverrides[caller] = gravityFunction;
		}

		// Token: 0x06006361 RID: 25441 RVA: 0x001FF79D File Offset: 0x001FD99D
		public void UnsetGravityOverride(Object caller)
		{
			this.gravityOverrides.Remove(caller);
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x001FF7AC File Offset: 0x001FD9AC
		private void ApplyGravityOverrides()
		{
			foreach (KeyValuePair<Object, Action<GTPlayer>> keyValuePair in this.gravityOverrides)
			{
				keyValuePair.Value.Invoke(this);
			}
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x001FF808 File Offset: 0x001FDA08
		public void ApplyKnockback(Vector3 direction, float speed, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.leftHand.wasColliding || this.rightHand.wasColliding)
				{
					this.leftHand.wasColliding = false;
					this.rightHand.wasColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.averagedVelocity, direction);
				float num2 = Mathf.InverseLerp(1.5f, 0.5f, num / speed);
				Vector3 vector = this.averagedVelocity + direction * speed * num2;
				this.playerRigidBody.linearVelocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x06006364 RID: 25444 RVA: 0x001FF8F8 File Offset: 0x001FDAF8
		public void ApplyClampedKnockback(Vector3 direction, float speed, float boostMultiplier, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.leftHand.wasColliding || this.rightHand.wasColliding)
				{
					this.leftHand.wasColliding = false;
					this.rightHand.wasColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.playerRigidBody.linearVelocity, direction.normalized);
				if (num >= speed)
				{
					return;
				}
				float num2 = Mathf.Clamp(speed - num, 0f, speed * boostMultiplier);
				Vector3 vector = this.playerRigidBody.linearVelocity + direction.normalized * num2;
				this.playerRigidBody.linearVelocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x06006365 RID: 25445 RVA: 0x001FF9FC File Offset: 0x001FDBFC
		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			this.IsFrozen = (GorillaTagger.Instance.offlineVRRig.IsFrozen || this.debugFreezeTag);
			bool isDefaultScale = this.IsDefaultScale;
			this.playerRigidBody.useGravity = false;
			if (this.gravityOverrides.Count > 0)
			{
				this.ApplyGravityOverrides();
			}
			else
			{
				if (!this.isClimbing)
				{
					this.playerRigidBody.AddForce(Physics.gravity * this.scale * this.playerRigidBody.mass, 0);
				}
				if (this.halloweenLevitationBonusStrength > 0f || this.halloweenLevitationStrength > 0f)
				{
					float num = Time.time - this.lastTouchedGroundTimestamp;
					if (num < this.halloweenLevitationTotalDuration)
					{
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num) * this.playerRigidBody.mass, 0);
					}
					float y = this.playerRigidBody.linearVelocity.y;
					if (y <= this.halloweenLevitateBonusFullAtYSpeed)
					{
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * this.playerRigidBody.mass, 0);
					}
					else if (y <= this.halloweenLevitateBonusOffAtYSpeed)
					{
						Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.linearVelocity.y);
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.linearVelocity.y) * this.playerRigidBody.mass, 0);
					}
				}
			}
			if (this.enableHoverMode)
			{
				this.playerRigidBody.linearVelocity = this.HoverboardFixedUpdate(this.playerRigidBody.linearVelocity);
			}
			else
			{
				this.didHoverLastFrame = false;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.bodyInWater = false;
			Vector3 vector = this.swimmingVelocity;
			this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, this.swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
			this.leftHandNonDiveHapticsAmount = 0f;
			this.rightHandNonDiveHapticsAmount = 0f;
			if (this.bodyOverlappingWaterVolumes.Count > 0)
			{
				WaterVolume waterVolume = null;
				float num2 = float.MinValue;
				Vector3 vector2 = this.headCollider.transform.position + Vector3.down * this.swimmingParams.floatingWaterLevelBelowHead * this.scale;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.bodyOverlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery;
					if (this.bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector2, out surfaceQuery, false))
					{
						float num3 = Vector3.Dot(surfaceQuery.surfacePoint - vector2, surfaceQuery.surfaceNormal);
						if (num3 > num2)
						{
							num2 = num3;
							waterVolume = this.bodyOverlappingWaterVolumes[i];
							this.waterSurfaceForHead = surfaceQuery;
						}
						WaterCurrent waterCurrent = this.bodyOverlappingWaterVolumes[i].Current;
						if (waterCurrent != null && num3 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (waterVolume != null)
				{
					Vector3 linearVelocity = this.playerRigidBody.linearVelocity;
					float magnitude = linearVelocity.magnitude;
					bool flag = this.headInWater;
					this.headInWater = (this.headCollider.transform.position.y < this.waterSurfaceForHead.surfacePoint.y && this.headCollider.transform.position.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth);
					if (this.headInWater && !flag)
					{
						this.audioSetToUnderwater = true;
						this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot, 0.1f);
					}
					else if (!this.headInWater && flag)
					{
						this.audioSetToUnderwater = false;
						this.audioManager.UnsetMixerSnapshot(0.1f);
					}
					this.bodyInWater = (vector2.y < this.waterSurfaceForHead.surfacePoint.y && vector2.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth);
					if (this.bodyInWater)
					{
						GTPlayer.LiquidProperties liquidProperties = this.liquidPropertiesList[(int)waterVolume.LiquidType];
						if (waterVolume != null)
						{
							float num7;
							if (this.swimmingParams.extendBouyancyFromSpeed)
							{
								float num4 = Mathf.Clamp(Vector3.Dot(linearVelocity / this.scale, this.waterSurfaceForHead.surfaceNormal), this.swimmingParams.speedToBouyancyExtensionMinMax.x, this.swimmingParams.speedToBouyancyExtensionMinMax.y);
								float num5 = this.swimmingParams.speedToBouyancyExtension.Evaluate(num4);
								this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, num5);
								float num6 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist + this.buoyancyExtension, num2 / this.scale + this.buoyancyExtension);
								this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, this.swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime, 1E-05f);
								num7 = num6;
							}
							else
							{
								num7 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist, num2 / this.scale);
							}
							Vector3 vector3 = Physics.gravity * this.scale;
							Vector3 vector4 = liquidProperties.buoyancy * -vector3 * num7;
							if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
							{
								vector4 *= this.frozenBodyBuoyancyFactor;
							}
							this.playerRigidBody.AddForce(vector4 * this.playerRigidBody.mass, 0);
						}
						Vector3 vector5 = Vector3.zero;
						Vector3 vector6 = Vector3.zero;
						for (int j = 0; j < this.activeWaterCurrents.Count; j++)
						{
							WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
							Vector3 startingVelocity = linearVelocity + vector5;
							Vector3 vector7;
							Vector3 vector8;
							if (waterCurrent2.GetCurrentAtPoint(this.bodyCollider.transform.position, startingVelocity, fixedDeltaTime, out vector7, out vector8))
							{
								vector6 += vector7;
								vector5 += vector8;
							}
						}
						if (magnitude > Mathf.Epsilon)
						{
							float num8 = 0.01f;
							Vector3 vector9 = linearVelocity / magnitude;
							Vector3 right = this.leftHand.handFollower.right;
							Vector3 dir = -this.rightHand.handFollower.right;
							Vector3 forward = this.leftHand.handFollower.forward;
							Vector3 forward2 = this.rightHand.handFollower.forward;
							Vector3 vector10 = vector9;
							float num9 = 0f;
							float num10 = 0f;
							float num11 = 0f;
							if (this.swimmingParams.applyDiveSteering && !this.disableMovement && isDefaultScale)
							{
								float num12 = Vector3.Dot(linearVelocity - vector6, vector9);
								float num13 = Mathf.Clamp(num12, this.swimmingParams.swimSpeedToRedirectAmountMinMax.x, this.swimmingParams.swimSpeedToRedirectAmountMinMax.y);
								float num14 = this.swimmingParams.swimSpeedToRedirectAmount.Evaluate(num13);
								num13 = Mathf.Clamp(num12, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y);
								float num15 = this.swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(num13);
								float num16 = Mathf.Acos(Vector3.Dot(vector9, forward)) / 3.1415927f * -2f + 1f;
								float num17 = Mathf.Acos(Vector3.Dot(vector9, forward2)) / 3.1415927f * -2f + 1f;
								float num18 = Mathf.Clamp(num16, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num19 = Mathf.Clamp(num17, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num20 = (!float.IsNaN(num18)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num18) : 0f;
								float num21 = (!float.IsNaN(num19)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num19) : 0f;
								Vector3 vector11 = Vector3.ProjectOnPlane(vector9, right);
								Vector3 vector12 = Vector3.ProjectOnPlane(vector9, right);
								float num22 = Mathf.Min(vector11.magnitude, 1f);
								float num23 = Mathf.Min(vector12.magnitude, 1f);
								float magnitude2 = this.leftHand.velocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float magnitude3 = this.rightHand.velocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float num24 = Mathf.Clamp(magnitude2, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float num25 = Mathf.Clamp(magnitude3, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float num26 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(num24);
								float num27 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(num25);
								float averageSpeedChangeMagnitudeInDirection = this.leftHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, false, this.swimmingParams.diveVelocityAveragingWindow);
								float averageSpeedChangeMagnitudeInDirection2 = this.rightHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(dir, false, this.swimmingParams.diveVelocityAveragingWindow);
								float num28 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float num29 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float num30 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(num28);
								float num31 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(num29);
								num9 = Mathf.Min(num20, Mathf.Min(num26, num30));
								float num32 = (Vector3.Dot(vector9, forward) > 0f) ? (Mathf.Min(num9, num14) * num22) : 0f;
								num10 = Mathf.Min(num21, Mathf.Min(num27, num31));
								float num33 = (Vector3.Dot(vector9, forward2) > 0f) ? (Mathf.Min(num10, num14) * num23) : 0f;
								if (this.swimmingParams.reduceDiveSteeringBelowVelocityPlane)
								{
									Vector3 vector13;
									if (Vector3.Dot(this.headCollider.transform.up, vector9) > 0.95f)
									{
										vector13 = -this.headCollider.transform.forward;
									}
									else
									{
										vector13 = Vector3.Cross(Vector3.Cross(vector9, this.headCollider.transform.up), vector9).normalized;
									}
									Vector3 position = this.headCollider.transform.position;
									Vector3 vector14 = position - this.leftHand.handFollower.position;
									Vector3 vector15 = position - this.rightHand.handFollower.position;
									float reduceDiveSteeringBelowPlaneFadeStartDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
									float reduceDiveSteeringBelowPlaneFadeEndDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
									float num34 = Vector3.Dot(vector14, Vector3.up);
									float num35 = Vector3.Dot(vector15, Vector3.up);
									float num36 = Vector3.Dot(vector14, vector13);
									float num37 = Vector3.Dot(vector15, vector13);
									float num38 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(num34), Mathf.Abs(num36)));
									float num39 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(num35), Mathf.Abs(num37)));
									num32 *= num38;
									num33 *= num39;
								}
								float num40 = num33 + num32;
								Vector3 vector16 = Vector3.zero;
								if (this.swimmingParams.applyDiveSteering && num40 > num8)
								{
									vector16 = ((num32 * vector11 + num33 * vector12) / num40).normalized;
									vector16 = Vector3.Lerp(vector9, vector16, num40);
									vector10 = Vector3.RotateTowards(vector9, vector16, 0.017453292f * num15 * fixedDeltaTime, 0f);
								}
								else
								{
									vector10 = vector9;
								}
								num11 = Mathf.Clamp01((num9 + num10) * 0.5f);
							}
							float num41 = Mathf.Clamp(Vector3.Dot(vector, vector9), 0f, magnitude);
							float num42 = magnitude - num41;
							if (this.swimmingParams.applyDiveSwimVelocityConversion && !this.disableMovement && num11 > num8 && num41 < this.swimmingParams.diveMaxSwimVelocityConversion)
							{
								float num43 = Mathf.Min(this.swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num42) * num11;
								num41 += num43;
								num42 -= num43;
							}
							float halflife = this.swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float halflife2 = this.swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num44 = Spring.DamperDecayExact(num41 / this.scale, halflife, fixedDeltaTime, 1E-05f) * this.scale;
							float num45 = Spring.DamperDecayExact(num42 / this.scale, halflife2, fixedDeltaTime, 1E-05f) * this.scale;
							if (this.swimmingParams.applyDiveDampingMultiplier && !this.disableMovement)
							{
								float num46 = Mathf.Lerp(1f, this.swimmingParams.diveDampingMultiplier, num11);
								num44 = Mathf.Lerp(num41, num44, num46);
								num45 = Mathf.Lerp(num42, num45, num46);
								float num47 = Mathf.Clamp((1f - num9) * (num41 + num42), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num8, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num8);
								float num48 = Mathf.Clamp((1f - num10) * (num41 + num42), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num8, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num8);
								this.leftHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(num47);
								this.rightHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(num48);
							}
							this.swimmingVelocity = num44 * vector10 + vector5 * this.scale;
							this.playerRigidBody.linearVelocity = this.swimmingVelocity + num45 * vector10;
						}
					}
				}
			}
			else if (this.audioSetToUnderwater)
			{
				this.audioSetToUnderwater = false;
				this.audioManager.UnsetMixerSnapshot(0.1f);
			}
			this.handleClimbing(Time.fixedDeltaTime);
			this.stuckHandsCheckFixedUpdate();
			this.FixedUpdate_HandHolds(Time.fixedDeltaTime);
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06006366 RID: 25446 RVA: 0x002008C9 File Offset: 0x001FEAC9
		// (set) Token: 0x06006367 RID: 25447 RVA: 0x002008D1 File Offset: 0x001FEAD1
		public bool isHoverAllowed { get; private set; }

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06006368 RID: 25448 RVA: 0x002008DA File Offset: 0x001FEADA
		// (set) Token: 0x06006369 RID: 25449 RVA: 0x002008E2 File Offset: 0x001FEAE2
		public bool enableHoverMode { get; private set; }

		// Token: 0x0600636A RID: 25450 RVA: 0x002008EB File Offset: 0x001FEAEB
		public void SetHoverboardPosRot(Vector3 worldPos, Quaternion worldRot)
		{
			this.hoverboardPlayerLocalPos = this.headCollider.transform.InverseTransformPoint(worldPos);
			this.hoverboardPlayerLocalRot = this.headCollider.transform.InverseTransformRotation(worldRot);
		}

		// Token: 0x0600636B RID: 25451 RVA: 0x0020091C File Offset: 0x001FEB1C
		private void HoverboardLateUpdate()
		{
			Vector3 eulerAngles = this.headCollider.transform.eulerAngles;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				RaycastHit raycastHit;
				hoverBoardCast.didHit = Physics.SphereCast(new Ray(this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin), this.hoverboardVisual.transform.rotation * hoverBoardCast.localDirection), hoverBoardCast.sphereRadius, ref raycastHit, hoverBoardCast.distance, this.locomotionEnabledLayers);
				if (hoverBoardCast.didHit)
				{
					HoverboardCantHover hoverboardCantHover;
					if (raycastHit.collider.TryGetComponent<HoverboardCantHover>(ref hoverboardCantHover))
					{
						hoverBoardCast.didHit = false;
					}
					else
					{
						hoverBoardCast.pointHit = raycastHit.point;
						hoverBoardCast.normalHit = raycastHit.normal;
					}
				}
				this.hoverboardCasts[i] = hoverBoardCast;
				if (hoverBoardCast.didHit)
				{
					flag = true;
				}
			}
			this.hasHoverPoint = flag;
			this.bodyCollider.enabled = (this.bodyCollider.transform.position - this.hoverboardVisual.transform.TransformPoint(Vector3.up * this.hoverBodyCollisionRadiusUpOffset)).IsLongerThan(this.hoverBodyHasCollisionsOutsideRadius);
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x00200A64 File Offset: 0x001FEC64
		private Vector3 HoverboardFixedUpdate(Vector3 velocity)
		{
			this.hoverboardVisual.transform.position = this.headCollider.transform.TransformPoint(this.hoverboardPlayerLocalPos);
			this.hoverboardVisual.transform.rotation = this.headCollider.transform.TransformRotation(this.hoverboardPlayerLocalRot);
			if (this.didHoverLastFrame)
			{
				velocity += Vector3.up * this.hoverGeneralUpwardForce * Time.fixedDeltaTime;
			}
			Vector3 position = this.hoverboardVisual.transform.position;
			Vector3 vector = position + velocity * Time.fixedDeltaTime;
			Vector3 vector2 = this.hoverboardVisual.transform.forward;
			Vector3 vector3 = this.hoverboardCasts[0].didHit ? this.hoverboardCasts[0].normalHit : Vector3.up;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				if (hoverBoardCast.didHit)
				{
					Vector3 vector4 = position + Vector3.Project(hoverBoardCast.pointHit - position, vector2);
					Vector3 vector5 = vector + Vector3.Project(hoverBoardCast.pointHit - position, vector2);
					bool flag2 = hoverBoardCast.isSolid || Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - vector5) + this.hoverIdealHeight > 0f;
					float num = hoverBoardCast.isSolid ? (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin + hoverBoardCast.localDirection * hoverBoardCast.distance)) + hoverBoardCast.sphereRadius) : (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - vector4) + this.hoverIdealHeight);
					if (flag2)
					{
						flag = true;
						this.boostEnabledUntilTimestamp = Time.time + this.hoverboardBoostGracePeriod;
						if (Vector3.Dot(velocity, hoverBoardCast.normalHit) < 0f)
						{
							velocity = Vector3.ProjectOnPlane(velocity, hoverBoardCast.normalHit);
						}
						this.playerRigidBody.transform.position += hoverBoardCast.normalHit * num;
						Vector3 vector6 = this.turnParent.transform.rotation * (this.hoverboardVisual.IsLeftHanded ? this.leftHand.velocityTracker : this.rightHand.velocityTracker).GetAverageVelocity(false, 0.15f, false);
						if (Vector3.Dot(vector6, hoverBoardCast.normalHit) < 0f)
						{
							velocity -= Vector3.Project(vector6, hoverBoardCast.normalHit) * this.hoverSlamJumpStrengthFactor * Time.fixedDeltaTime;
						}
						vector = position + velocity * Time.fixedDeltaTime;
					}
				}
			}
			float num2 = Mathf.Abs(Mathf.DeltaAngle(0f, Mathf.Acos(Vector3.Dot(this.hoverboardVisual.transform.up, Vector3.ProjectOnPlane(vector3, vector2).normalized)) * 57.29578f));
			float num3 = this.hoverCarveAngleResponsiveness.Evaluate(num2);
			vector2 = (vector2 + Vector3.ProjectOnPlane(this.hoverboardVisual.transform.up, vector3) * this.hoverTiltAdjustsForwardFactor).normalized;
			if (!flag)
			{
				this.didHoverLastFrame = false;
				num3 = 0f;
			}
			Vector3 vector7 = velocity;
			if (this.enableHoverMode && this.hasHoverPoint)
			{
				Vector3 vector8 = Vector3.ProjectOnPlane(velocity, vector3);
				Vector3 vector9 = velocity - vector8;
				Vector3 vector10 = Vector3.Project(vector8, vector2);
				float num4 = vector8.magnitude;
				if (num4 <= this.hoveringSlowSpeed)
				{
					num4 *= this.hoveringSlowStoppingFactor;
				}
				Vector3 vector11 = vector8 - vector10;
				float num5 = 0f;
				bool flag3 = false;
				if (num3 > 0f)
				{
					if (vector11.IsLongerThan(vector10))
					{
						num5 = Mathf.Min((vector11.magnitude - vector10.magnitude) * this.hoverCarveSidewaysSpeedLossFactor * num3, num4);
						if (num5 > 0f && num4 > this.hoverMinGrindSpeed)
						{
							flag3 = true;
							this.hoverboardVisual.PlayGrindHaptic();
						}
						num4 -= num5;
					}
					vector11 *= 1f - num3 * this.sidewaysDrag;
					if (!this.leftHand.isColliding && !this.rightHand.isColliding)
					{
						velocity = (vector10 + vector11).normalized * num4 + vector9;
					}
				}
				else
				{
					velocity = vector8.normalized * num4 + vector9;
				}
				float magnitude = (velocity - vector7).magnitude;
				this.hoverboardAudio.UpdateAudioLoop(velocity.magnitude, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, magnitude, flag3 ? num5 : 0f);
				if (magnitude > 0f && !flag3)
				{
					this.hoverboardVisual.PlayCarveHaptic(magnitude);
				}
			}
			else
			{
				this.hoverboardAudio.UpdateAudioLoop(0f, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, 0f, 0f);
			}
			return velocity;
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x00200FD4 File Offset: 0x001FF1D4
		public void GrabPersonalHoverboard(bool isLeftHand, Vector3 pos, Quaternion rot, Color col)
		{
			if (this.hoverboardVisual.IsHeld)
			{
				this.hoverboardVisual.DropFreeBoard();
			}
			this.hoverboardVisual.SetIsHeld(isLeftHand, pos, rot, col);
			this.hoverboardVisual.ProxyGrabHandle(isLeftHand);
			FreeHoverboardManager.instance.PreserveMaxHoverboardsConstraint(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x00201030 File Offset: 0x001FF230
		public void SetHoverAllowed(bool allowed, bool force = false)
		{
			if (allowed)
			{
				this.hoverAllowedCount++;
				this.isHoverAllowed = true;
				return;
			}
			this.hoverAllowedCount = ((force || this.hoverAllowedCount == 0) ? 0 : (this.hoverAllowedCount - 1));
			if (this.hoverAllowedCount == 0 && this.isHoverAllowed)
			{
				this.isHoverAllowed = false;
				if (this.enableHoverMode)
				{
					this.SetHoverActive(false);
					VRRig.LocalRig.hoverboardVisual.SetNotHeld();
				}
			}
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x002010A8 File Offset: 0x001FF2A8
		public void SetHoverActive(bool enable)
		{
			if (enable && !this.isHoverAllowed)
			{
				return;
			}
			this.enableHoverMode = enable;
			if (!enable)
			{
				this.bodyCollider.enabled = true;
				this.hasHoverPoint = false;
				this.didHoverLastFrame = false;
				for (int i = 0; i < this.hoverboardCasts.Length; i++)
				{
					this.hoverboardCasts[i].didHit = false;
				}
				this.hoverboardAudio.Stop();
			}
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x00201118 File Offset: 0x001FF318
		private void BodyCollider()
		{
			if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
			{
				if (this.scale > 0f)
				{
					this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
				}
				if (Physics.SphereCast(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, Vector3.down, ref this.bodyHitInfo, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, this.locomotionEnabledLayers, 1))
				{
					this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
				}
				else
				{
					this.bodyHitInfo = this.emptyHit;
					this.bodyCollider.height = this.bodyInitialHeight;
				}
				if (!this.bodyCollider.gameObject.activeSelf)
				{
					this.bodyCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bodyCollider.gameObject.SetActive(false);
			}
			this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
			this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
			this.bodyOffsetVector = Vector3.down * this.bodyCollider.height / 2f;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x0020132C File Offset: 0x001FF52C
		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		// Token: 0x06006372 RID: 25458 RVA: 0x00201350 File Offset: 0x001FF550
		public void ScaleAwayFromPoint(float oldScale, float newScale, Vector3 scaleCenter)
		{
			if (oldScale < newScale)
			{
				this.lastHeadPosition = GTPlayer.ScalePointAwayFromCenter(this.lastHeadPosition, this.headCollider.radius, oldScale, newScale, scaleCenter);
				this.leftHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.leftHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
				this.rightHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.rightHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
			}
		}

		// Token: 0x06006373 RID: 25459 RVA: 0x002013C8 File Offset: 0x001FF5C8
		private static Vector3 ScalePointAwayFromCenter(Vector3 point, float baseRadius, float oldScale, float newScale, Vector3 scaleCenter)
		{
			float magnitude = (point - scaleCenter).magnitude;
			float num = magnitude + Mathf.Epsilon + baseRadius * (newScale - oldScale);
			return scaleCenter + (point - scaleCenter) * num / magnitude;
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x00201410 File Offset: 0x001FF610
		private void OnBeforeRenderInit()
		{
			if (Application.isPlaying && !this.hasCorrectedForTracking && this.mainCamera != null && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				this.ForceRigidBodySync();
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
			}
			Application.onBeforeRender -= new UnityAction(this.OnBeforeRenderInit);
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x0020149C File Offset: 0x001FF69C
		private void LateUpdate()
		{
			Vector3 vector = this.antiDriftLastPosition.GetValueOrDefault();
			if (this.antiDriftLastPosition == null)
			{
				vector = base.transform.position;
				this.antiDriftLastPosition = new Vector3?(vector);
			}
			if ((double)(this.antiDriftLastPosition.Value - base.transform.position).sqrMagnitude < 1E-08)
			{
				base.transform.position = this.antiDriftLastPosition.Value;
			}
			else
			{
				this.antiDriftLastPosition = new Vector3?(base.transform.position);
			}
			if (!this.hasCorrectedForTracking && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
				Application.onBeforeRender -= new UnityAction(this.OnBeforeRenderInit);
			}
			if (this.playerRigidBody.isKinematic)
			{
				return;
			}
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			if (this.playerRotationOverrideFrame < Time.frameCount - 1)
			{
				this.playerRotationOverride = Quaternion.Slerp(Quaternion.identity, this.playerRotationOverride, Mathf.Exp(-this.playerRotationOverrideDecayRate * Time.deltaTime));
			}
			base.transform.rotation = this.playerRotationOverride;
			this.turnParent.transform.localScale = VRRig.LocalRig.transform.localScale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			if (Mathf.Abs(this.lastScale - this.scale) > 0.001f)
			{
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
				}
				this.mainCamera.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			}
			this.lastScale = this.scale;
			this.debugLastRightHandPosition = this.rightHand.lastPosition;
			this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
			if (this.debugMovement)
			{
				this.tempRealTime = Time.time;
				this.calcDeltaTime = Time.deltaTime;
				this.lastRealTime = this.tempRealTime;
			}
			else
			{
				this.tempRealTime = Time.realtimeSinceStartup;
				this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
				this.lastRealTime = this.tempRealTime;
				if (this.calcDeltaTime > 0.1f)
				{
					this.calcDeltaTime = 0.05f;
				}
			}
			Vector3 vector2;
			if (this.lastFrameHasValidTouchPos && this.lastPlatformTouched != null && GTPlayer.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out vector2))
			{
				this.refMovement = vector2 - this.lastFrameTouchPosWorld;
			}
			else
			{
				this.refMovement = Vector3.zero;
			}
			Vector3 vector3 = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 pivot = this.headCollider.transform.position;
			Vector3 vector4;
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeWorldHitPoint(this.lastMovingSurfaceHit, this.lastMovingSurfaceTouchLocal, out vector4))
			{
				if (this.wasMovingSurfaceMonkeBlock && (this.lastMonkeBlock == null || this.lastMonkeBlock.state != BuilderPiece.State.AttachedAndPlaced))
				{
					this.movingSurfaceOffset = Vector3.zero;
				}
				else
				{
					this.movingSurfaceOffset = vector4 - this.lastMovingSurfaceTouchWorld;
					vector3 = this.movingSurfaceOffset / this.calcDeltaTime;
					quaternion = this.lastMovingSurfaceHit.collider.transform.rotation * Quaternion.Inverse(this.lastMovingSurfaceRot);
					pivot = vector4;
				}
			}
			else
			{
				this.movingSurfaceOffset = Vector3.zero;
			}
			float num = 40f * this.scale;
			if (vector3.sqrMagnitude >= num * num)
			{
				this.movingSurfaceOffset = Vector3.zero;
				vector3 = Vector3.zero;
				quaternion = Quaternion.identity;
			}
			if (!this.didAJump && (this.leftHand.wasColliding || this.rightHand.wasColliding))
			{
				base.transform.position = base.transform.position + 4.9f * Vector3.down * this.calcDeltaTime * this.calcDeltaTime * this.scale;
				if (Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) <= 0f && Vector3.Dot(Vector3.up, this.slideAverageNormal) > 0f)
				{
					base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, Vector3.down);
				}
			}
			if (!this.didAJump && this.anyHandWasSliding)
			{
				base.transform.position = base.transform.position + this.slideVelocity * this.calcDeltaTime;
				this.slideVelocity += 9.8f * Vector3.down * this.calcDeltaTime * this.scale;
			}
			float paddleBoostFactor = (Time.time > this.boostEnabledUntilTimestamp) ? 0f : (Time.deltaTime * Mathf.Clamp(this.playerRigidBody.linearVelocity.magnitude * this.hoverboardPaddleBoostMultiplier, 0f, this.hoverboardPaddleBoostMax));
			int num2 = 0;
			Vector3 vector5 = Vector3.zero;
			this.anyHandIsColliding = false;
			this.anyHandIsSliding = false;
			this.anyHandIsSticking = false;
			this.leftHand.FirstIteration(ref vector5, ref num2, paddleBoostFactor);
			this.rightHand.FirstIteration(ref vector5, ref num2, paddleBoostFactor);
			for (int i = 0; i < 12; i++)
			{
				if (this.stiltStates[i].isActive)
				{
					this.stiltStates[i].FirstIteration(ref vector5, ref num2, 0f);
				}
			}
			if (num2 != 0)
			{
				vector5 /= (float)num2;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT || this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT)
			{
				vector5 += this.movingSurfaceOffset;
			}
			else if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.BODY)
			{
				Vector3 vector6 = this.lastHeadPosition + this.movingSurfaceOffset - this.headCollider.transform.position;
				vector5 += vector6;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
			}
			Vector3 vector7;
			float num3;
			if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + vector5 - this.lastHeadPosition, Vector3.zero, out vector7, false, out num3, out this.junkHit, true))
			{
				vector5 = vector7 - this.headCollider.transform.position;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + vector5, true, out this.maxSphereSize1) || !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition + vector5))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
				vector5 = this.lastHeadPosition - this.headCollider.transform.position;
			}
			else if (this.headCollider.radius * 0.9f * 0.825f * this.scale < this.maxSphereSize1)
			{
				this.lastOpenHeadPosition = this.headCollider.transform.position + vector5;
			}
			if (vector5 != Vector3.zero)
			{
				base.transform.position += vector5;
			}
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && quaternion != Quaternion.identity && !this.isClimbing && !this.rightHand.isHolding && !this.leftHand.isHolding)
			{
				this.RotateWithSurface(quaternion, pivot);
			}
			this.lastHeadPosition = this.headCollider.transform.position;
			this.areBothTouching = ((!this.leftHand.isColliding && !this.leftHand.wasColliding) || (!this.rightHand.isColliding && !this.rightHand.wasColliding));
			this.HandleHandLink();
			this.HandleTentacleMovement();
			this.anyHandIsColliding = false;
			this.anyHandIsSliding = false;
			this.anyHandIsSticking = false;
			this.leftHand.FinalizeHandPosition();
			this.rightHand.FinalizeHandPosition();
			for (int j = 0; j < 12; j++)
			{
				if (this.stiltStates[j].isActive)
				{
					this.stiltStates[j].FinalizeHandPosition();
					GTPlayer.HandState handState = this.stiltStates[j];
					GorillaTagger.Instance.SetExtraHandPosition((StiltID)j, handState.finalPositionThisFrame, handState.canTag, handState.canStun);
				}
			}
			Vector3 vector8 = this.lastPosition;
			GTPlayer.MovingSurfaceContactPoint movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
			int num4 = -1;
			int num5 = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this.rightHand.isColliding && this.IsTouchingMovingSurface(this.rightHand.GetLastPosition(), this.rightHand.lastHitInfo, out num4, out flag, out flag2);
			if (flag4 && !flag)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
				this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
			}
			else
			{
				bool flag5 = false;
				BuilderPiece builderPiece = flag4 ? this.lastMonkeBlock : null;
				if (this.leftHand.isColliding && this.IsTouchingMovingSurface(this.leftHand.GetLastPosition(), this.leftHand.lastHitInfo, out num5, out flag5, out flag3))
				{
					if (flag5 && flag2 == flag3)
					{
						if (flag && num5.Equals(num4) && (double)Vector3.Dot(this.leftHand.lastHitInfo.point - this.leftHand.GetLastPosition(), this.rightHand.lastHitInfo.point - this.rightHand.GetLastPosition()) < 0.3)
						{
							movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
							this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
							this.lastMonkeBlock = builderPiece;
						}
					}
					else
					{
						movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.LEFT;
						this.lastMovingSurfaceHit = this.leftHand.lastHitInfo;
					}
				}
			}
			this.StoreVelocities();
			if (this.InWater)
			{
				PlayerGameEvents.PlayerSwam((this.lastPosition - vector8).magnitude, this.currentVelocity.magnitude);
			}
			else
			{
				PlayerGameEvents.PlayerMoved((this.lastPosition - vector8).magnitude, this.currentVelocity.magnitude);
			}
			this.didAJump = false;
			bool flag6 = this.exitMovingSurface;
			this.exitMovingSurface = false;
			if (this.leftHand.IsSlipOverriddenToMax() && this.rightHand.IsSlipOverriddenToMax())
			{
				this.didAJump = true;
				this.exitMovingSurface = true;
			}
			else if (this.anyHandIsSliding)
			{
				this.slideAverageNormal = Vector3.zero;
				int num6 = 0;
				this.averageSlipPercentage = 0f;
				bool flag7 = false;
				if (this.leftHand.isSliding)
				{
					this.slideAverageNormal += this.leftHand.slideNormal.normalized;
					this.averageSlipPercentage += this.leftHand.slipPercentage;
					num6++;
				}
				if (this.rightHand.isSliding)
				{
					flag7 = true;
					this.slideAverageNormal += this.rightHand.slideNormal.normalized;
					this.averageSlipPercentage += this.rightHand.slipPercentage;
					num6++;
				}
				for (int k = 0; k < this.stiltStates.Length; k++)
				{
					if (this.stiltStates[k].isActive && this.stiltStates[k].isSliding)
					{
						if (!this.stiltStates[k].isLeftHand)
						{
							flag7 = true;
						}
						this.slideAverageNormal += this.stiltStates[k].slideNormal.normalized;
						this.averageSlipPercentage += this.stiltStates[k].slipPercentage;
						num6++;
					}
				}
				this.slideAverageNormal = this.slideAverageNormal.normalized;
				this.averageSlipPercentage /= (float)num6;
				if (num6 == 1)
				{
					this.surfaceDirection = (flag7 ? Vector3.ProjectOnPlane(this.rightHand.handFollower.forward, this.rightHand.slideNormal) : Vector3.ProjectOnPlane(this.leftHand.handFollower.forward, this.leftHand.slideNormal));
					if (Vector3.Dot(this.slideVelocity, this.surfaceDirection) > 0f)
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
					else
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, -this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
				}
				if (!this.anyHandWasSliding)
				{
					this.slideVelocity = ((Vector3.Dot(this.playerRigidBody.linearVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.playerRigidBody.linearVelocity, this.slideAverageNormal) : this.playerRigidBody.linearVelocity);
				}
				else
				{
					this.slideVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
				}
				this.slideVelocity = this.slideVelocity.normalized * Mathf.Min(this.slideVelocity.magnitude, Mathf.Max(0.5f, this.averagedVelocity.magnitude * 2f));
				this.playerRigidBody.linearVelocity = Vector3.zero;
			}
			else if (this.anyHandIsColliding)
			{
				if (!this.turnedThisFrame)
				{
					this.playerRigidBody.linearVelocity = Vector3.zero;
				}
				else
				{
					this.playerRigidBody.linearVelocity = this.playerRigidBody.linearVelocity.normalized * Mathf.Min(2f, this.playerRigidBody.linearVelocity.magnitude);
				}
			}
			else if (this.anyHandWasSliding)
			{
				this.playerRigidBody.linearVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
			}
			if (this.anyHandIsColliding && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
			{
				if (this.anyHandIsSliding)
				{
					if (Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > this.slideVelocityLimit * this.scale && Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) > 0f && Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > Vector3.Project(this.slideVelocity, this.slideAverageNormal).magnitude)
					{
						this.leftHand.isSliding = false;
						this.rightHand.isSliding = false;
						for (int l = 0; l < this.stiltStates.Length; l++)
						{
							this.stiltStates[l].isSliding = false;
						}
						this.anyHandIsSliding = false;
						this.didAJump = true;
						float num7 = this.ApplyNativeScaleAdjustment(Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude));
						this.playerRigidBody.linearVelocity = num7 * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal);
						if (num7 > this.slideVelocityLimit * this.scale * this.exitMovingSurfaceThreshold)
						{
							this.exitMovingSurface = true;
						}
					}
				}
				else if (this.averagedVelocity.magnitude > this.velocityLimit * this.scale)
				{
					float num8 = (this.InWater && this.CurrentWaterVolume != null) ? this.liquidPropertiesList[(int)this.CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f;
					float num9 = this.ApplyNativeScaleAdjustment(this.enableHoverMode ? Mathf.Min(this.hoverMaxPaddleSpeed, this.averagedVelocity.magnitude) : Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num8 * this.averagedVelocity.magnitude));
					Vector3 vector9 = num9 * this.averagedVelocity.normalized;
					this.didAJump = true;
					this.playerRigidBody.linearVelocity = vector9;
					if (this.InWater)
					{
						this.swimmingVelocity += vector9 * this.swimmingParams.underwaterJumpsAsSwimVelocityFactor;
					}
					if (num9 > this.velocityLimit * this.scale * this.exitMovingSurfaceThreshold)
					{
						this.exitMovingSurface = true;
					}
				}
			}
			this.stuckHandsCheckLateUpdate(ref this.leftHand.finalPositionThisFrame, ref this.rightHand.finalPositionThisFrame);
			if (this.lastPlatformTouched != null && this.currentPlatform == null)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += this.refMovement / this.calcDeltaTime;
				}
				this.refMovement = Vector3.zero;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += this.lastMovingSurfaceVelocity;
				}
				this.lastMovingSurfaceVelocity = Vector3.zero;
			}
			if (this.enableHoverMode)
			{
				this.HoverboardLateUpdate();
			}
			else
			{
				this.hasHoverPoint = false;
			}
			Vector3 vector10 = Vector3.zero;
			float num10 = 0f;
			float num11 = 0f;
			if (this.bodyInWater)
			{
				Vector3 vector11;
				if (this.GetSwimmingVelocityForHand(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out vector11) && !this.turnedThisFrame)
				{
					num10 = Mathf.InverseLerp(0f, 0.2f, vector11.magnitude) * this.swimmingParams.swimmingHapticsStrength;
					vector10 += vector11;
				}
				Vector3 vector12;
				if (this.GetSwimmingVelocityForHand(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out vector12) && !this.turnedThisFrame)
				{
					num11 = Mathf.InverseLerp(0f, 0.15f, vector12.magnitude) * this.swimmingParams.swimmingHapticsStrength;
					vector10 += vector12;
				}
			}
			Vector3 vector13 = Vector3.zero;
			Vector3 vector14;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.leftHand.velocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out vector14))
			{
				if (time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown)
				{
					vector13 += vector14;
				}
				this.lastWaterSurfaceJumpTimeLeft = Time.time;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			Vector3 vector15;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.rightHand.velocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.rightHandWaterVolume, this.rightHandWaterSurface, out vector15))
			{
				if (time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown)
				{
					vector13 += vector15;
				}
				this.lastWaterSurfaceJumpTimeRight = Time.time;
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			vector13 = Vector3.ClampMagnitude(vector13, this.swimmingParams.waterSurfaceJumpMaxSpeed * this.scale);
			float num12 = Mathf.Max(num10, this.leftHandNonDiveHapticsAmount);
			if (num12 > 0.001f && time - this.lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(4, num12, this.calcDeltaTime);
			}
			float num13 = Mathf.Max(num11, this.rightHandNonDiveHapticsAmount);
			if (num13 > 0.001f && time - this.lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(5, num13, this.calcDeltaTime);
			}
			if (!this.disableMovement)
			{
				this.swimmingVelocity += vector10;
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += vector10 + vector13;
				}
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
			}
			if (GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				if (!this.IsFrozen || !this.primaryButtonPressed)
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
					if (this.bodyTouchedSurfaces.Count > 0)
					{
						foreach (KeyValuePair<GameObject, PhysicsMaterial> keyValuePair in this.bodyTouchedSurfaces)
						{
							MeshCollider meshCollider;
							if (keyValuePair.Key.TryGetComponent<MeshCollider>(ref meshCollider))
							{
								meshCollider.material = keyValuePair.Value;
							}
						}
						this.bodyTouchedSurfaces.Clear();
					}
				}
				else if (this.BodyOnGround && this.primaryButtonPressed)
				{
					float num14 = this.bodyInitialHeight / 2f - this.bodyInitialRadius;
					RaycastHit raycastHit;
					if (Physics.SphereCast(this.bodyCollider.transform.position - new Vector3(0f, num14, 0f), this.bodyInitialRadius - 0.01f, Vector3.down, ref raycastHit, 1f, ~LayerMask.GetMask(new string[]
					{
						"Gorilla Body Collider",
						"GorillaInteractable"
					}), 1))
					{
						this.IsBodySliding = true;
						MeshCollider meshCollider2;
						if (!this.bodyTouchedSurfaces.ContainsKey(raycastHit.transform.gameObject) && raycastHit.transform.gameObject.TryGetComponent<MeshCollider>(ref meshCollider2))
						{
							this.bodyTouchedSurfaces.Add(raycastHit.transform.gameObject, meshCollider2.material);
							raycastHit.transform.gameObject.GetComponent<MeshCollider>().material = this.slipperyMaterial;
						}
					}
				}
				else
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
				}
			}
			else
			{
				this.IsBodySliding = false;
				if (this.bodyTouchedSurfaces.Count > 0)
				{
					foreach (KeyValuePair<GameObject, PhysicsMaterial> keyValuePair2 in this.bodyTouchedSurfaces)
					{
						MeshCollider meshCollider3;
						if (keyValuePair2.Key.TryGetComponent<MeshCollider>(ref meshCollider3))
						{
							meshCollider3.material = keyValuePair2.Value;
						}
					}
					this.bodyTouchedSurfaces.Clear();
				}
			}
			this.leftHand.OnEndOfFrame();
			this.rightHand.OnEndOfFrame();
			for (int m = 0; m < 12; m++)
			{
				if (this.stiltStates[m].isActive)
				{
					this.stiltStates[m].OnEndOfFrame();
				}
			}
			this.leftHand.PositionHandFollower();
			this.rightHand.PositionHandFollower();
			this.anyHandWasSliding = this.anyHandIsSliding;
			this.anyHandWasColliding = this.anyHandIsColliding;
			this.anyHandWasSticking = this.anyHandIsSticking;
			if (this.anyHandIsSticking)
			{
				this.lastTouchedGroundTimestamp = Time.time;
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.IsGroundedHand || this.IsThrusterActive)
				{
					this.LastHandTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
				else if (this.IsGroundedButt)
				{
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
			}
			else
			{
				this.LastHandTouchedGroundAtNetworkTime = 0f;
				this.LastTouchedGroundAtNetworkTime = 0f;
			}
			this.degreesTurnedThisFrame = 0f;
			this.lastPlatformTouched = this.currentPlatform;
			this.currentPlatform = null;
			this.lastMovingSurfaceVelocity = vector3;
			Vector3 vector16;
			if (GTPlayer.ComputeLocalHitPoint(this.lastHitInfoHand, out vector16))
			{
				this.lastFrameHasValidTouchPos = true;
				this.lastFrameTouchPosLocal = vector16;
				this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
			}
			else
			{
				this.lastFrameHasValidTouchPos = false;
				this.lastFrameTouchPosLocal = Vector3.zero;
				this.lastFrameTouchPosWorld = Vector3.zero;
			}
			this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
			RaycastHit raycastHit2 = this.emptyHit;
			this.BodyCollider();
			if (this.bodyHitInfo.collider != null)
			{
				this.wasBodyOnGround = true;
				raycastHit2 = this.bodyHitInfo;
			}
			else if (movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.bodyCollider.gameObject.activeSelf)
			{
				bool flag8 = false;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				Vector3 vector17 = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + (this.bodyInitialHeight * this.scale - this.bodyMaxRadius) * Vector3.down;
				this.bufferCount = Physics.SphereCastNonAlloc(vector17, this.bodyMaxRadius, Vector3.down, this.rayCastNonAllocColliders, this.minimumRaycastDistance * this.scale, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int n = 0; n < this.bufferCount; n++)
					{
						if (this.tempHitInfo.distance > 0f && (!flag8 || this.rayCastNonAllocColliders[n].distance < this.tempHitInfo.distance))
						{
							flag8 = true;
							raycastHit2 = this.rayCastNonAllocColliders[n];
						}
					}
				}
				this.wasBodyOnGround = flag8;
			}
			int num15 = -1;
			bool flag9 = false;
			bool flag10;
			if (this.wasBodyOnGround && movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.IsTouchingMovingSurface(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), raycastHit2, out num15, out flag10, out flag9) && !flag10)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.BODY;
				this.lastMovingSurfaceHit = raycastHit2;
			}
			Vector3 vector18;
			if (movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeLocalHitPoint(this.lastMovingSurfaceHit, out vector18))
			{
				this.lastMovingSurfaceTouchLocal = vector18;
				this.lastMovingSurfaceTouchWorld = this.lastMovingSurfaceHit.point;
				this.lastMovingSurfaceRot = this.lastMovingSurfaceHit.collider.transform.rotation;
				this.lastAttachedToMovingSurfaceFrame = Time.frameCount;
			}
			else
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
				this.lastMovingSurfaceTouchLocal = Vector3.zero;
				this.lastMovingSurfaceTouchWorld = Vector3.zero;
				this.lastMovingSurfaceRot = Quaternion.identity;
			}
			Vector3 position2 = this.lastMovingSurfaceTouchWorld;
			int num16 = -1;
			bool flag11 = false;
			switch (movingSurfaceContactPoint)
			{
			case GTPlayer.MovingSurfaceContactPoint.NONE:
				if (flag6)
				{
					this.exitMovingSurface = true;
				}
				num16 = -1;
				break;
			case GTPlayer.MovingSurfaceContactPoint.RIGHT:
				num16 = num4;
				flag11 = flag2;
				position2 = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.LEFT:
				num16 = num5;
				flag11 = flag3;
				position2 = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.BODY:
				num16 = num15;
				flag11 = flag9;
				position2 = GorillaTagger.Instance.offlineVRRig.bodyTransform.position;
				break;
			}
			if (!flag11)
			{
				this.lastMonkeBlock = null;
			}
			if (num16 != this.lastMovingSurfaceID || this.lastMovingSurfaceContact != movingSurfaceContactPoint || flag11 != this.wasMovingSurfaceMonkeBlock)
			{
				if (num16 == -1)
				{
					if (Time.frameCount - this.lastAttachedToMovingSurfaceFrame > 3)
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (flag11)
				{
					if (this.lastMonkeBlock != null)
					{
						VRRig.AttachLocalPlayerToMovingSurface(num16, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, this.lastMonkeBlock.transform.InverseTransformPoint(position2), flag11);
						this.lastMovingSurfaceID = num16;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (MovingSurfaceManager.instance != null)
				{
					MovingSurface movingSurface;
					if (MovingSurfaceManager.instance.TryGetMovingSurface(num16, out movingSurface))
					{
						VRRig.AttachLocalPlayerToMovingSurface(num16, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, movingSurface.transform.InverseTransformPoint(position2), flag11);
						this.lastMovingSurfaceID = num16;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else
				{
					VRRig.DetachLocalPlayerFromMovingSurface();
					this.lastMovingSurfaceID = -1;
				}
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE && movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			this.lastMovingSurfaceContact = movingSurfaceContactPoint;
			this.wasMovingSurfaceMonkeBlock = flag11;
			if (this.activeSizeChangerSettings != null)
			{
				if (this.activeSizeChangerSettings.ExpireOnDistance > 0f && Vector3.Distance(base.transform.position, this.activeSizeChangerSettings.WorldPosition) > this.activeSizeChangerSettings.ExpireOnDistance)
				{
					this.SetNativeScale(null);
				}
				if (this.activeSizeChangerSettings.ExpireAfterSeconds > 0f && Time.time - this.activeSizeChangerSettings.ActivationTime > this.activeSizeChangerSettings.ExpireAfterSeconds)
				{
					this.SetNativeScale(null);
				}
			}
			HandLink grabbedLink = VRRig.LocalRig.leftHandLink.grabbedLink;
			if (grabbedLink != null)
			{
				double time2 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime = this.LastHandTouchedGroundAtNetworkTime;
				double time3 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime2 = grabbedLink.myRig.LastHandTouchedGroundAtNetworkTime;
			}
			if (this.didAJump || this.anyHandIsColliding || this.anyHandIsSliding || this.anyHandIsSticking || this.IsGroundedHand || this.forceRBSync)
			{
				this.forceRBSync = false;
			}
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x0020340C File Offset: 0x0020160C
		private float ApplyNativeScaleAdjustment(float adjustedMagnitude)
		{
			if (this.nativeScale > 0f && this.nativeScale != 1f)
			{
				return adjustedMagnitude *= this.nativeScaleMagnitudeAdjustmentFactor.Evaluate(this.nativeScale);
			}
			return adjustedMagnitude;
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x00203440 File Offset: 0x00201640
		private float RotateWithSurface(Quaternion rotationDelta, Vector3 pivot)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(rotationDelta, Vector3.up, out quaternion, out quaternion2);
			float num = quaternion2.eulerAngles.y;
			if (num > 270f)
			{
				num -= 360f;
			}
			else if (num > 90f)
			{
				num -= 180f;
			}
			if (Mathf.Abs(num) < 90f * this.calcDeltaTime)
			{
				this.turnParent.transform.RotateAround(pivot, base.transform.up, num);
				return num;
			}
			return 0f;
		}

		// Token: 0x06006378 RID: 25464 RVA: 0x002034C4 File Offset: 0x002016C4
		private void stuckHandsCheckFixedUpdate()
		{
			Vector3 currentHandPosition = this.leftHand.GetCurrentHandPosition();
			this.stuckLeft = (!this.controllerState.LeftValid || (this.leftHand.isColliding && (currentHandPosition - this.leftHand.GetLastPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition - this.headCollider.transform.position).normalized, (currentHandPosition - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
			Vector3 currentHandPosition2 = this.rightHand.GetCurrentHandPosition();
			this.stuckRight = (!this.controllerState.RightValid || (this.rightHand.isColliding && (currentHandPosition2 - this.rightHand.GetLastPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition2 - this.headCollider.transform.position).normalized, (currentHandPosition2 - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
		}

		// Token: 0x06006379 RID: 25465 RVA: 0x00203650 File Offset: 0x00201850
		private void stuckHandsCheckLateUpdate(ref Vector3 finalLeftHandPosition, ref Vector3 finalRightHandPosition)
		{
			if (this.stuckLeft)
			{
				finalLeftHandPosition = this.leftHand.GetCurrentHandPosition();
				this.stuckLeft = (this.leftHand.isColliding = false);
			}
			if (this.stuckRight)
			{
				finalRightHandPosition = this.rightHand.GetCurrentHandPosition();
				this.stuckRight = (this.rightHand.isColliding = false);
			}
		}

		// Token: 0x0600637A RID: 25466 RVA: 0x002036BC File Offset: 0x002018BC
		private void handleClimbing(float deltaTime)
		{
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 vector = Vector3.zero;
			if (this.isClimbing && (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1f)
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.linearVelocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
				vector = this.currentClimber.transform.position - this.climbHelper.position;
				vector = ((vector.sqrMagnitude > this.maxArmLength * this.maxArmLength) ? (vector.normalized * this.maxArmLength) : vector);
				if (this.isClimbableMoving)
				{
					Quaternion rotationDelta = this.currentClimbable.transform.rotation * Quaternion.Inverse(this.lastClimbableRotation);
					this.RotateWithSurface(rotationDelta, this.currentClimber.handRoot.position);
					this.lastClimbableRotation = this.currentClimbable.transform.rotation;
				}
				this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector);
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
		}

		// Token: 0x0600637B RID: 25467 RVA: 0x0020387B File Offset: 0x00201A7B
		public void RequestTentacleMove(bool isLeftHand, Vector3 move)
		{
			if (isLeftHand)
			{
				this.hasLeftHandTentacleMove = true;
				this.leftHandTentacleMove = move;
				return;
			}
			this.hasRightHandTentacleMove = true;
			this.rightHandTentacleMove = move;
		}

		// Token: 0x0600637C RID: 25468 RVA: 0x002038A0 File Offset: 0x00201AA0
		public void HandleTentacleMovement()
		{
			Vector3 vector;
			if (this.hasLeftHandTentacleMove)
			{
				if (this.hasRightHandTentacleMove)
				{
					vector = (this.leftHandTentacleMove + this.rightHandTentacleMove) * 0.5f;
					this.hasRightHandTentacleMove = (this.hasLeftHandTentacleMove = false);
				}
				else
				{
					vector = this.leftHandTentacleMove;
					this.hasLeftHandTentacleMove = false;
				}
			}
			else
			{
				if (!this.hasRightHandTentacleMove)
				{
					return;
				}
				vector = this.rightHandTentacleMove;
				this.hasRightHandTentacleMove = false;
			}
			this.playerRigidBody.transform.position += vector;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x0600637D RID: 25469 RVA: 0x00203940 File Offset: 0x00201B40
		public HandLinkAuthorityStatus GetSelfHandLinkAuthority()
		{
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			if (this.IsGroundedHand)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded);
			}
			if ((double)(this.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, this.LastHandTouchedGroundAtNetworkTime, actorNumber);
			}
			if (this.IsGroundedButt)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded);
			}
			return new HandLinkAuthorityStatus(HandLinkAuthorityType.None, this.LastTouchedGroundAtNetworkTime, actorNumber);
		}

		// Token: 0x0600637E RID: 25470 RVA: 0x002039A8 File Offset: 0x00201BA8
		private void HandleHandLink()
		{
			HandLink leftHandLink = VRRig.LocalRig.leftHandLink;
			HandLink rightHandLink = VRRig.LocalRig.rightHandLink;
			bool flag = leftHandLink.grabbedLink != null;
			bool flag2 = rightHandLink.grabbedLink != null;
			if (!flag && !flag2)
			{
				return;
			}
			HandLinkAuthorityStatus selfHandLinkAuthority = this.GetSelfHandLinkAuthority();
			int num = -1;
			HandLinkAuthorityStatus chainAuthority = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag)
			{
				chainAuthority = leftHandLink.GetChainAuthority(out num);
			}
			int num2 = -1;
			HandLinkAuthorityStatus chainAuthority2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag2)
			{
				chainAuthority2 = rightHandLink.GetChainAuthority(out num2);
			}
			if (flag && flag2)
			{
				if (leftHandLink.grabbedPlayer == rightHandLink.grabbedPlayer)
				{
					switch (selfHandLinkAuthority.CompareTo(chainAuthority))
					{
					case -1:
						this.HandLink_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 0:
						this.HandLink_PositionBoth_BothHands(leftHandLink, rightHandLink);
						return;
					case 1:
						this.HandLink_PositionChild_RemotePlayer_BothHands(leftHandLink, rightHandLink);
						return;
					default:
						return;
					}
				}
				else
				{
					int num3 = selfHandLinkAuthority.CompareTo(chainAuthority);
					int num4 = selfHandLinkAuthority.CompareTo(chainAuthority2);
					switch (num3 * 3 + num4)
					{
					case -3:
					case -2:
						this.HandLink_PositionChild_LocalPlayer(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					case -1:
					case 2:
						this.HandLink_PositionChild_LocalPlayer(rightHandLink);
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						this.HandLink_PositionTriple(leftHandLink, rightHandLink);
						return;
					case 1:
						this.HandLink_PositionBoth(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					case 3:
						this.HandLink_PositionBoth(rightHandLink);
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 4:
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					}
					switch (chainAuthority.CompareTo(chainAuthority2))
					{
					case -1:
						this.HandLink_PositionChild_LocalPlayer(rightHandLink);
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						if (num > num2)
						{
							this.HandLink_PositionChild_LocalPlayer(rightHandLink);
							this.HandLink_PositionChild_RemotePlayer(leftHandLink);
							return;
						}
						if (num < num2)
						{
							this.HandLink_PositionChild_LocalPlayer(leftHandLink);
							this.HandLink_PositionChild_RemotePlayer(rightHandLink);
							return;
						}
						this.HandLink_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 1:
						this.HandLink_PositionChild_LocalPlayer(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					default:
						return;
					}
				}
			}
			else if (flag)
			{
				switch (selfHandLinkAuthority.CompareTo(chainAuthority))
				{
				case -1:
					this.HandLink_PositionChild_LocalPlayer(leftHandLink);
					return;
				case 0:
					this.HandLink_PositionBoth(leftHandLink);
					return;
				case 1:
					this.HandLink_PositionChild_RemotePlayer(leftHandLink);
					return;
				default:
					return;
				}
			}
			else
			{
				switch (selfHandLinkAuthority.CompareTo(chainAuthority2))
				{
				case -1:
					this.HandLink_PositionChild_LocalPlayer(rightHandLink);
					return;
				case 0:
					this.HandLink_PositionBoth(rightHandLink);
					return;
				case 1:
					this.HandLink_PositionChild_RemotePlayer(rightHandLink);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x0600637F RID: 25471 RVA: 0x00203BFC File Offset: 0x00201DFC
		private void HandLink_PositionTriple(HandLink linkA, HandLink linkB)
		{
			Vector3 vector = linkA.transform.position - linkA.grabbedLink.transform.position;
			Vector3 vector2 = linkB.transform.position - linkB.grabbedLink.transform.position;
			Vector3 vector3 = (vector + vector2) * 0.33f;
			bool flag;
			bool flag2;
			linkA.grabbedLink.myRig.TrySweptOffsetMove(vector - vector3, out flag, out flag2);
			bool flag3;
			bool flag4;
			linkB.grabbedLink.myRig.TrySweptOffsetMove(vector2 - vector3, out flag3, out flag4);
			this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector3);
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006380 RID: 25472 RVA: 0x00203CC0 File Offset: 0x00201EC0
		private void HandLink_PositionBoth(HandLink link)
		{
			Vector3 vector = (link.grabbedLink.transform.position - link.transform.position) * 0.5f;
			bool flag;
			bool flag2;
			link.grabbedLink.myRig.TrySweptOffsetMove(-vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(link);
			}
			else
			{
				this.playerRigidBody.transform.position += vector;
			}
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006381 RID: 25473 RVA: 0x00203D4C File Offset: 0x00201F4C
		private void HandLink_PositionBoth_BothHands(HandLink link1, HandLink link2)
		{
			Vector3 vector = (link1.grabbedLink.transform.position - link1.transform.position) * 0.5f;
			Vector3 vector2 = (link2.grabbedLink.transform.position - link2.transform.position) * 0.5f;
			Vector3 vector3 = (vector + vector2) * 0.5f;
			bool flag;
			bool flag2;
			link1.grabbedLink.myRig.TrySweptOffsetMove(-vector3, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(link1, link2);
			}
			else
			{
				this.playerRigidBody.transform.position += vector3;
			}
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006382 RID: 25474 RVA: 0x00203E14 File Offset: 0x00202014
		private void HandLink_PositionChild_LocalPlayer(HandLink parentLink)
		{
			Vector3 vector = parentLink.grabbedLink.transform.position - parentLink.transform.position;
			this.playerRigidBody.transform.position += vector;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006383 RID: 25475 RVA: 0x00203E70 File Offset: 0x00202070
		private void HandLink_PositionChild_LocalPlayer(HandLink linkA, HandLink linkB)
		{
			Vector3 vector = linkA.grabbedLink.transform.position - linkA.transform.position;
			Vector3 vector2 = linkB.grabbedLink.transform.position - linkB.transform.position;
			this.playerRigidBody.transform.position += (vector + vector2) * 0.5f;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006384 RID: 25476 RVA: 0x00203EFC File Offset: 0x002020FC
		private void HandLink_PositionChild_RemotePlayer(HandLink childLink)
		{
			Vector3 movement = childLink.transform.position - childLink.grabbedLink.transform.position;
			bool flag;
			bool flag2;
			childLink.grabbedLink.myRig.TrySweptOffsetMove(movement, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(childLink);
			}
		}

		// Token: 0x06006385 RID: 25477 RVA: 0x00203F4C File Offset: 0x0020214C
		private void HandLink_PositionChild_RemotePlayer_BothHands(HandLink childLink1, HandLink childLink2)
		{
			Vector3 vector = childLink1.transform.position - childLink1.grabbedLink.transform.position;
			Vector3 vector2 = childLink2.transform.position - childLink2.grabbedLink.transform.position;
			Vector3 movement = (vector + vector2) * 0.5f;
			bool flag;
			bool flag2;
			childLink1.grabbedLink.myRig.TrySweptOffsetMove(movement, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(childLink1, childLink2);
			}
		}

		// Token: 0x06006386 RID: 25478 RVA: 0x00203FD0 File Offset: 0x002021D0
		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, Vector3 boostVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
		{
			slipPercentage = this.defaultSlideFactor;
			if (!this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				endPosition = Vector3.zero;
				return false;
			}
			this.firstPosition = endPosition;
			iterativeHitInfo = this.tempIterativeHit;
			this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((this.slideFactor != this.defaultSlideFactor) ? this.slideFactor : ((!singleHand) ? this.defaultSlideFactor : 0.001f));
			if (fullSlide)
			{
				slipPercentage = 1f;
			}
			this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
			Vector3 vector = Vector3.zero;
			if (boostVector.IsLongerThan(0f))
			{
				vector = Vector3.ProjectOnPlane(boostVector, iterativeHitInfo.normal);
				this.movementToProjectedAboveCollisionPlane += vector;
				this.CollisionsSphereCast(this.firstPosition, sphereRadius, vector, out endPosition, out this.tempIterativeHit);
				this.firstPosition = endPosition;
			}
			if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector + vector - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		// Token: 0x06006387 RID: 25479 RVA: 0x0020418C File Offset: 0x0020238C
		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
		{
			this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
			bool flag = false;
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int i = 0; i < this.bufferCount; i++)
				{
					if (this.tempHitInfo.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
					{
						flag = true;
						this.tempHitInfo = this.rayCastNonAllocColliders[i];
					}
				}
			}
			if (flag)
			{
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, 1);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int j = 0; j < this.bufferCount; j++)
					{
						if (this.rayCastNonAllocColliders[j].collider && this.rayCastNonAllocColliders[j].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[j];
						}
					}
					finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
				}
				this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int k = 0; k < this.bufferCount; k++)
					{
						if (this.rayCastNonAllocColliders[k].collider != null && this.rayCastNonAllocColliders[k].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[k];
						}
					}
					finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
					collisionsHitInfo = this.tempHitInfo;
				}
				return true;
			}
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int l = 0; l < this.bufferCount; l++)
				{
					if (this.rayCastNonAllocColliders[l].collider != null && this.rayCastNonAllocColliders[l].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[l];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = startPosition;
				return true;
			}
			finalPosition = startPosition + movementVector;
			collisionsHitInfo = default(RaycastHit);
			return false;
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x0020459C File Offset: 0x0020279C
		public float GetSlidePercentage(RaycastHit raycastHit)
		{
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				return this.FreezeTagSlidePercentage();
			}
			this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
			BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
			if (component != null)
			{
				this.currentPlatform = component;
			}
			if (this.currentOverride != null)
			{
				if (this.currentOverride.slidePercentageOverride >= 0f)
				{
					return this.currentOverride.slidePercentageOverride;
				}
				this.currentMaterialIndex = this.currentOverride.overrideIndex;
				if (this.currentMaterialIndex < 0 || this.currentMaterialIndex >= this.materialData.Count)
				{
					return this.defaultSlideFactor;
				}
				if (!this.materialData[this.currentMaterialIndex].overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			else
			{
				this.meshCollider = (raycastHit.collider as MeshCollider);
				if (this.meshCollider == null || this.meshCollider.sharedMesh == null || this.meshCollider.convex)
				{
					return this.defaultSlideFactor;
				}
				this.collidedMesh = this.meshCollider.sharedMesh;
				if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, ref this.sharedMeshTris))
				{
					this.sharedMeshTris = this.collidedMesh.triangles;
					this.meshTrianglesDict.Add(this.collidedMesh, (int[])this.sharedMeshTris.Clone());
				}
				this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
				this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
				this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
				this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
				if (this.slideRenderer != null)
				{
					this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
				}
				else
				{
					this.tempMaterialArray.Clear();
				}
				if (this.tempMaterialArray.Count > 1)
				{
					for (int i = 0; i < this.tempMaterialArray.Count; i++)
					{
						this.collidedMesh.GetTriangles(this.trianglesList, i);
						int j = 0;
						while (j < this.trianglesList.Count)
						{
							if (this.trianglesList[j] == this.vertex1 && this.trianglesList[j + 1] == this.vertex2 && this.trianglesList[j + 2] == this.vertex3)
							{
								this.findMatName = this.tempMaterialArray[i].name;
								if (this.findMatName.EndsWith("Uber"))
								{
									string text = this.findMatName;
									this.findMatName = text.Substring(0, text.Length - 4);
								}
								this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								if (this.currentMaterialIndex == -1)
								{
									this.currentMaterialIndex = 0;
								}
								if (!this.foundMatData.overrideSlidePercent)
								{
									return this.defaultSlideFactor;
								}
								return this.foundMatData.slidePercent;
							}
							else
							{
								j += 3;
							}
						}
					}
				}
				else if (this.tempMaterialArray.Count > 0)
				{
					return this.defaultSlideFactor;
				}
				this.currentMaterialIndex = 0;
				return this.defaultSlideFactor;
			}
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x00204934 File Offset: 0x00202B34
		public bool IsTouchingMovingSurface(Vector3 rayOrigin, RaycastHit raycastHit, out int movingSurfaceId, out bool sideTouch, out bool isMonkeBlock)
		{
			movingSurfaceId = -1;
			sideTouch = false;
			isMonkeBlock = false;
			float num = Vector3.Dot(rayOrigin - raycastHit.point, Vector3.up);
			if (num < -0.3f)
			{
				return false;
			}
			if (num < 0f)
			{
				sideTouch = true;
			}
			if (raycastHit.collider == null)
			{
				return false;
			}
			MovingSurface component = raycastHit.collider.GetComponent<MovingSurface>();
			if (component != null)
			{
				isMonkeBlock = false;
				movingSurfaceId = component.GetID();
				return true;
			}
			if (!BuilderTable.IsLocalPlayerInBuilderZone())
			{
				return false;
			}
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
			if (builderPieceFromCollider != null && builderPieceFromCollider.IsPieceMoving())
			{
				isMonkeBlock = true;
				movingSurfaceId = builderPieceFromCollider.pieceId;
				this.lastMonkeBlock = builderPieceFromCollider;
				return true;
			}
			sideTouch = false;
			return false;
		}

		// Token: 0x0600638A RID: 25482 RVA: 0x002049F0 File Offset: 0x00202BF0
		public void Turn(float degrees)
		{
			Vector3 position = this.headCollider.transform.position;
			bool flag = this.rightHand.isColliding || this.rightHand.isHolding;
			bool flag2 = this.leftHand.isColliding || this.leftHand.isHolding;
			if (flag != flag2 && flag)
			{
				position = this.rightHand.controllerTransform.position;
			}
			if (flag != flag2 && flag2)
			{
				position = this.leftHand.controllerTransform.position;
			}
			this.turnParent.transform.RotateAround(position, base.transform.up, degrees);
			this.degreesTurnedThisFrame = degrees;
			this.averagedVelocity = Vector3.zero;
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Quaternion.Euler(0f, degrees, 0f) * this.velocityHistory[i];
				this.averagedVelocity += this.velocityHistory[i];
			}
			this.averagedVelocity /= (float)this.velocityHistorySize;
		}

		// Token: 0x0600638B RID: 25483 RVA: 0x00204B24 File Offset: 0x00202D24
		public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
		{
			if (this.currentClimber != null)
			{
				this.EndClimbing(this.currentClimber, true, false);
			}
			try
			{
				Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
				if (onBeforeClimb != null)
				{
					onBeforeClimb.Invoke(hand, climbableRef);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			Rigidbody rigidbody;
			climbable.TryGetComponent<Rigidbody>(ref rigidbody);
			this.VerifyClimbHelper();
			this.climbHelper.SetParent(climbable.transform);
			this.climbHelper.position = hand.transform.position;
			Vector3 localPosition = this.climbHelper.localPosition;
			if (climbable.snapX)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|406_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|406_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|406_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			if (climbable.climbOnlyWhileSmall)
			{
				BuilderPiece componentInParent = climbable.GetComponentInParent<BuilderPiece>();
				if (componentInParent != null && componentInParent.IsPieceMoving())
				{
					this.isClimbableMoving = true;
					this.lastClimbableRotation = climbable.transform.rotation;
				}
				else
				{
					this.isClimbableMoving = false;
				}
			}
			else
			{
				this.isClimbableMoving = false;
			}
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			PhotonView view;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (climbable.TryGetComponent<GorillaRopeSegment>(ref gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(ref gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<PhotonView>(ref view))
			{
				VRRig.AttachLocalPlayerToPhotonView(view, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.TryGetComponent<PhotonViewXSceneRef>(ref photonViewXSceneRef))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonViewXSceneRef.photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == 4, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == 4);
			}
		}

		// Token: 0x0600638C RID: 25484 RVA: 0x00204D90 File Offset: 0x00202F90
		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x00204DC8 File Offset: 0x00202FC8
		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			hand.SetCanRelease(true);
			if (!startingNewClimb)
			{
				this.enablePlayerGravity(true);
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(ref rigidbody);
				this.currentClimbable.isBeingClimbed = false;
			}
			Vector3 vector = Vector3.zero;
			if (this.currentClimber)
			{
				this.currentClimber.isClimbing = false;
				if (doDontReclimb)
				{
					this.currentClimber.dontReclimbLast = this.currentClimbable;
				}
				else
				{
					this.currentClimber.dontReclimbLast = null;
				}
				this.currentClimber.queuedToBecomeValidToGrabAgain = true;
				this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
				if (!startingNewClimb && this.currentClimbable)
				{
					GorillaVelocityTracker interactPointVelocityTracker = this.GetInteractPointVelocityTracker(this.currentClimber.xrNode == 4);
					if (rigidbody)
					{
						this.playerRigidBody.linearVelocity = rigidbody.linearVelocity;
					}
					else if (this.currentSwing)
					{
						this.playerRigidBody.linearVelocity = this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f, false);
					}
					else if (this.currentZipline)
					{
						this.playerRigidBody.linearVelocity = this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed;
					}
					else
					{
						this.playerRigidBody.linearVelocity = Vector3.zero;
					}
					vector = this.turnParent.transform.rotation * -interactPointVelocityTracker.GetAverageVelocity(false, 0.1f, true) * this.scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * this.scale);
					this.playerRigidBody.AddForce(vector, 2);
				}
			}
			if (this.currentSwing)
			{
				this.currentSwing.DetachLocalPlayer();
			}
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (this.currentClimbable.TryGetComponent<PhotonView>(ref photonView) || this.currentClimbable.TryGetComponent<PhotonViewXSceneRef>(ref photonViewXSceneRef) || this.currentClimbable.IsPlayerAttached)
			{
				VRRig.DetachLocalPlayerFromPhotonView();
			}
			if (!startingNewClimb && vector.magnitude > 2f && this.currentClimbable && this.currentClimbable.clipOnFullRelease)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == 4);
			}
			this.currentClimbable = null;
			this.currentClimber = null;
			this.currentSwing = null;
			this.currentZipline = null;
			this.isClimbing = false;
		}

		// Token: 0x0600638E RID: 25486 RVA: 0x00205058 File Offset: 0x00203258
		public void ResetRigidbodyInterpolation()
		{
			this.playerRigidBody.interpolation = this.playerRigidbodyInterpolationDefault;
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x0600638F RID: 25487 RVA: 0x0020506B File Offset: 0x0020326B
		// (set) Token: 0x06006390 RID: 25488 RVA: 0x00205078 File Offset: 0x00203278
		public RigidbodyInterpolation RigidbodyInterpolation
		{
			get
			{
				return this.playerRigidBody.interpolation;
			}
			set
			{
				this.playerRigidBody.interpolation = value;
			}
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x00205086 File Offset: 0x00203286
		private void enablePlayerGravity(bool useGravity)
		{
			this.playerRigidBody.useGravity = useGravity;
		}

		// Token: 0x06006392 RID: 25490 RVA: 0x00205094 File Offset: 0x00203294
		public void SetVelocity(Vector3 velocity)
		{
			this.playerRigidBody.linearVelocity = velocity;
		}

		// Token: 0x06006393 RID: 25491 RVA: 0x002050A2 File Offset: 0x002032A2
		internal void RigidbodyMovePosition(Vector3 pos)
		{
			this.playerRigidBody.MovePosition(pos);
		}

		// Token: 0x06006394 RID: 25492 RVA: 0x002050B0 File Offset: 0x002032B0
		public void TempFreezeHand(bool isLeft, float freezeDuration)
		{
			(isLeft ? this.leftHand : this.rightHand).TempFreezeHand(freezeDuration);
		}

		// Token: 0x06006395 RID: 25493 RVA: 0x002050D8 File Offset: 0x002032D8
		private void StoreVelocities()
		{
			this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
			this.currentVelocity = (base.transform.position - this.lastPosition - this.MovingSurfaceMovement()) / this.calcDeltaTime;
			this.velocityHistory[this.velocityIndex] = this.currentVelocity;
			this.averagedVelocity = this.velocityHistory.Average();
			this.lastPosition = base.transform.position;
		}

		// Token: 0x06006396 RID: 25494 RVA: 0x00205168 File Offset: 0x00203368
		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.linearVelocity.magnitude * this.calcDeltaTime)
			{
				this.ForceRigidBodySync();
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

		// Token: 0x06006397 RID: 25495 RVA: 0x002051F8 File Offset: 0x002033F8
		private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
		{
			overlapRadiusTest = testRadius;
			this.overlapAttempts = 0;
			int num = 100;
			while (this.overlapAttempts < num && overlapRadiusTest > testRadius * 0.75f)
			{
				this.ClearColliderBuffer(ref this.overlapColliders);
				this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, 1);
				if (ignoreOneWay)
				{
					int num2 = 0;
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.overlapColliders[i].CompareTag("NoCrazyCheck"))
						{
							num2++;
						}
					}
					if (num2 == this.bufferCount)
					{
						return true;
					}
				}
				if (this.bufferCount <= 0)
				{
					overlapRadiusTest *= 0.995f;
					return true;
				}
				overlapRadiusTest = Mathf.Lerp(testRadius, 0f, (float)this.overlapAttempts / (float)num);
				this.overlapAttempts++;
			}
			return false;
		}

		// Token: 0x06006398 RID: 25496 RVA: 0x002052D8 File Offset: 0x002034D8
		private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
		{
			for (int i = 0; i < this.crazyCheckVectors.Length; i++)
			{
				if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[i] * sphereSize) > 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x00205320 File Offset: 0x00203520
		private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
		{
			Vector3 vector = endPosition - startPosition;
			int num = Physics.RaycastNonAlloc(startPosition, vector, this.rayCastNonAllocColliders, vector.magnitude, this.locomotionEnabledLayers.value, 1);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!this.rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
				{
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x0020538C File Offset: 0x0020358C
		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		// Token: 0x0600639B RID: 25499 RVA: 0x002053B0 File Offset: 0x002035B0
		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		// Token: 0x0600639C RID: 25500 RVA: 0x002053DA File Offset: 0x002035DA
		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement + this.movingSurfaceOffset;
		}

		// Token: 0x0600639D RID: 25501 RVA: 0x002053F0 File Offset: 0x002035F0
		private static bool ComputeLocalHitPoint(RaycastHit hit, out Vector3 localHitPoint)
		{
			if (hit.collider == null || hit.point.sqrMagnitude < 0.001f)
			{
				localHitPoint = Vector3.zero;
				return false;
			}
			localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
			return true;
		}

		// Token: 0x0600639E RID: 25502 RVA: 0x0020544E File Offset: 0x0020364E
		private static bool ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint, out Vector3 worldHitPoint)
		{
			if (hit.collider == null)
			{
				worldHitPoint = Vector3.zero;
				return false;
			}
			worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
			return true;
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x00205488 File Offset: 0x00203688
		private float ExtraVelMultiplier()
		{
			float num = 1f;
			if (this.leftHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHand.surfaceOverride.extraVelMultiplier);
			}
			if (this.rightHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHand.surfaceOverride.extraVelMultiplier);
			}
			return num;
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x002054F0 File Offset: 0x002036F0
		private float ExtraVelMaxMultiplier()
		{
			float num = 1f;
			if (this.leftHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHand.surfaceOverride.extraVelMaxMultiplier);
			}
			if (this.rightHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHand.surfaceOverride.extraVelMaxMultiplier);
			}
			return num * this.scale;
		}

		// Token: 0x060063A1 RID: 25505 RVA: 0x00205561 File Offset: 0x00203761
		public void SetMaximumSlipThisFrame()
		{
			this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
			this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x060063A2 RID: 25506 RVA: 0x00205583 File Offset: 0x00203783
		public void SetLeftMaximumSlipThisFrame()
		{
			this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x00205595 File Offset: 0x00203795
		public void SetRightMaximumSlipThisFrame()
		{
			this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x060063A4 RID: 25508 RVA: 0x002055A7 File Offset: 0x002037A7
		public void ChangeLayer(string layerName)
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.ChangeLayer(base.transform.parent, layerName);
			}
		}

		// Token: 0x060063A5 RID: 25509 RVA: 0x002055CE File Offset: 0x002037CE
		public void RestoreLayer()
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.RestoreOriginalLayers();
			}
		}

		// Token: 0x060063A6 RID: 25510 RVA: 0x002055EC File Offset: 0x002037EC
		public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireInWater)
			{
				this.SetNativeScale(null);
			}
			if (playerCollider == this.headCollider)
			{
				if (!this.headOverlappingWaterVolumes.Contains(volume))
				{
					this.headOverlappingWaterVolumes.Add(volume);
					return;
				}
			}
			else if (playerCollider == this.bodyCollider && !this.bodyOverlappingWaterVolumes.Contains(volume))
			{
				this.bodyOverlappingWaterVolumes.Add(volume);
			}
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x00205666 File Offset: 0x00203866
		public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				this.headOverlappingWaterVolumes.Remove(volume);
				return;
			}
			if (playerCollider == this.bodyCollider)
			{
				this.bodyOverlappingWaterVolumes.Remove(volume);
			}
		}

		// Token: 0x060063A8 RID: 25512 RVA: 0x002056A0 File Offset: 0x002038A0
		private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
		{
			contactingWaterVolume = null;
			this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, 2);
			if (this.bufferCount > 0)
			{
				float num = float.MinValue;
				for (int i = 0; i < this.bufferCount; i++)
				{
					WaterVolume component = this.overlapColliders[i].GetComponent<WaterVolume>();
					WaterVolume.SurfaceQuery surfaceQuery;
					if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out surfaceQuery, false) && surfaceQuery.surfacePoint.y > num)
					{
						num = surfaceQuery.surfacePoint.y;
						contactingWaterVolume = component;
						waterSurface = surfaceQuery;
					}
				}
			}
			if (contactingWaterVolume != null)
			{
				Vector3 vector = endingHandPosition - startingHandPosition;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
				if (this.turnedThisFrame)
				{
					Vector3 vector4 = startingHandPosition - this.headCollider.transform.position;
					vector2 = Quaternion.AngleAxis(this.degreesTurnedThisFrame, Vector3.up) * vector4 - vector4;
				}
				float num2 = Vector3.Dot(vector - vector2 - vector3, palmForwardDirection);
				float num3 = 0f;
				if (num2 > 0f)
				{
					Plane surfacePlane = waterSurface.surfacePlane;
					float distanceToPoint = surfacePlane.GetDistanceToPoint(startingHandPosition);
					float distanceToPoint2 = surfacePlane.GetDistanceToPoint(endingHandPosition);
					if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
					{
						num3 = 1f;
					}
					else if (distanceToPoint > 0f && distanceToPoint2 <= 0f)
					{
						num3 = -distanceToPoint2 / (distanceToPoint - distanceToPoint2);
					}
					else if (distanceToPoint <= 0f && distanceToPoint2 > 0f)
					{
						num3 = -distanceToPoint / (distanceToPoint2 - distanceToPoint);
					}
					if (num3 > Mathf.Epsilon)
					{
						float resistance = this.liquidPropertiesList[(int)contactingWaterVolume.LiquidType].resistance;
						swimmingVelocityChange = -palmForwardDirection * num2 * 2f * resistance * num3;
						Vector3 forward = this.mainCamera.transform.forward;
						if (forward.y < 0f)
						{
							Vector3 vector5 = forward.x0z();
							float magnitude = vector5.magnitude;
							vector5 /= magnitude;
							float num4 = Vector3.Dot(swimmingVelocityChange, vector5);
							if (num4 > 0f)
							{
								Vector3 vector6 = vector5 * num4;
								swimmingVelocityChange = swimmingVelocityChange - vector6 + vector6 * magnitude + Vector3.up * forward.y * num4;
							}
						}
						return true;
					}
				}
			}
			swimmingVelocityChange = Vector3.zero;
			return false;
		}

		// Token: 0x060063A9 RID: 25513 RVA: 0x0020595C File Offset: 0x00203B5C
		private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
		{
			if (contactingWaterVolume != null)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
				if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
				{
					float num = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
					float num2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
					float num3 = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(num, 0.01f, 0.99f));
					float num4 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(num2, 0.01f, 0.99f));
					jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * num3 * num4;
					return true;
				}
			}
			jumpVelocity = Vector3.zero;
			return false;
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x00205A55 File Offset: 0x00203C55
		private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > eps)
			{
				normalized = input / magnitude;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x00205A82 File Offset: 0x00203C82
		private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > 1f)
			{
				normalized = input / magnitude;
				return true;
			}
			if (magnitude >= eps)
			{
				normalized = input;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x060063AC RID: 25516 RVA: 0x00205AC4 File Offset: 0x00203CC4
		private float FreezeTagSlidePercentage()
		{
			if (this.materialData[this.currentMaterialIndex].overrideSlidePercent && this.materialData[this.currentMaterialIndex].slidePercent > this.freezeTagHandSlidePercent)
			{
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			return this.freezeTagHandSlidePercent;
		}

		// Token: 0x060063AD RID: 25517 RVA: 0x00205B24 File Offset: 0x00203D24
		private void OnCollisionStay(Collision collision)
		{
			this.bodyCollisionContactsCount = collision.GetContacts(this.bodyCollisionContacts);
			float num = -1f;
			for (int i = 0; i < this.bodyCollisionContactsCount; i++)
			{
				float num2 = Vector3.Dot(this.bodyCollisionContacts[i].normal, Vector3.up);
				if (num2 > num)
				{
					this.bodyGroundContact = this.bodyCollisionContacts[i];
					num = num2;
				}
			}
			float num3 = 0.5f;
			if (num > num3)
			{
				this.bodyGroundContactTime = Time.time;
			}
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x00205BAC File Offset: 0x00203DAC
		public void DoLaunch(Vector3 velocity)
		{
			GTPlayer.<DoLaunch>d__442 <DoLaunch>d__;
			<DoLaunch>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DoLaunch>d__.<>4__this = this;
			<DoLaunch>d__.velocity = velocity;
			<DoLaunch>d__.<>1__state = -1;
			<DoLaunch>d__.<>t__builder.Start<GTPlayer.<DoLaunch>d__442>(ref <DoLaunch>d__);
		}

		// Token: 0x060063AF RID: 25519 RVA: 0x00205BEB File Offset: 0x00203DEB
		private void OnEnable()
		{
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		}

		// Token: 0x060063B0 RID: 25520 RVA: 0x00205C08 File Offset: 0x00203E08
		private void OnJoinedRoom()
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireOnRoomJoin)
			{
				this.SetNativeScale(null);
			}
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x00205C26 File Offset: 0x00203E26
		private void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x00205C43 File Offset: 0x00203E43
		public void ForceRigidBodySync()
		{
			this.forceRBSync = true;
		}

		// Token: 0x060063B3 RID: 25523 RVA: 0x00205C4C File Offset: 0x00203E4C
		internal void ClearHandHolds()
		{
			this.leftHand.isHolding = false;
			this.rightHand.isHolding = false;
			this.wasHoldingHandhold = false;
			this.activeHandHold = default(GTPlayer.HandHoldState);
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			this.OnChangeActiveHandhold();
		}

		// Token: 0x060063B4 RID: 25524 RVA: 0x00205C8C File Offset: 0x00203E8C
		internal void AddHandHold(Transform objectHeld, Vector3 localPositionHeld, GorillaGrabber grabber, bool forLeftHand, bool rotatePlayerWhenHeld, out Vector3 grabbedVelocity)
		{
			if (!this.leftHand.isHolding && !this.rightHand.isHolding)
			{
				grabbedVelocity = -this.bodyCollider.attachedRigidbody.linearVelocity;
				this.playerRigidBody.AddForce(grabbedVelocity, 2);
			}
			else
			{
				grabbedVelocity = Vector3.zero;
			}
			this.secondaryHandHold = this.activeHandHold;
			Vector3 position = grabber.transform.position;
			this.activeHandHold = new GTPlayer.HandHoldState
			{
				grabber = grabber,
				objectHeld = objectHeld,
				localPositionHeld = localPositionHeld,
				localRotationalOffset = grabber.transform.rotation.eulerAngles.y - objectHeld.rotation.eulerAngles.y,
				applyRotation = rotatePlayerWhenHeld
			};
			if (forLeftHand)
			{
				this.leftHand.isHolding = true;
			}
			else
			{
				this.rightHand.isHolding = true;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x00205D90 File Offset: 0x00203F90
		internal void RemoveHandHold(GorillaGrabber grabber, bool forLeftHand)
		{
			this.activeHandHold.objectHeld == grabber;
			if (this.activeHandHold.grabber == grabber)
			{
				this.activeHandHold = this.secondaryHandHold;
			}
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			if (forLeftHand)
			{
				this.leftHand.isHolding = false;
			}
			else
			{
				this.rightHand.isHolding = false;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x00205E00 File Offset: 0x00204000
		private void OnChangeActiveHandhold()
		{
			if (this.activeHandHold.objectHeld != null)
			{
				PhotonView view;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonView>(ref view))
				{
					VRRig.AttachLocalPlayerToPhotonView(view, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
					return;
				}
				PhotonViewXSceneRef photonViewXSceneRef;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonViewXSceneRef>(ref photonViewXSceneRef))
				{
					PhotonView photonView = photonViewXSceneRef.photonView;
					if (photonView != null)
					{
						VRRig.AttachLocalPlayerToPhotonView(photonView, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
						return;
					}
				}
				BuilderPieceHandHold builderPieceHandHold;
				if (this.activeHandHold.objectHeld.TryGetComponent<BuilderPieceHandHold>(ref builderPieceHandHold) && builderPieceHandHold.IsHandHoldMoving())
				{
					this.isHandHoldMoving = true;
					this.lastHandHoldRotation = builderPieceHandHold.transform.rotation;
					this.movingHandHoldReleaseVelocity = this.playerRigidBody.linearVelocity;
				}
				else
				{
					this.isHandHoldMoving = false;
					this.lastHandHoldRotation = Quaternion.identity;
					this.movingHandHoldReleaseVelocity = Vector3.zero;
				}
			}
			VRRig.DetachLocalPlayerFromPhotonView();
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x00205F10 File Offset: 0x00204110
		private void FixedUpdate_HandHolds(float timeDelta)
		{
			if (this.activeHandHold.objectHeld == null)
			{
				if (this.wasHoldingHandhold)
				{
					this.playerRigidBody.linearVelocity = Vector3.ClampMagnitude(this.secondLastPreHandholdVelocity, 5.5f * this.scale);
				}
				this.wasHoldingHandhold = false;
				return;
			}
			Vector3 vector = this.activeHandHold.objectHeld.TransformPoint(this.activeHandHold.localPositionHeld);
			Vector3 position = this.activeHandHold.grabber.transform.position;
			this.secondLastPreHandholdVelocity = this.lastPreHandholdVelocity;
			this.lastPreHandholdVelocity = this.playerRigidBody.linearVelocity;
			this.wasHoldingHandhold = true;
			if (this.isHandHoldMoving)
			{
				this.lastPreHandholdVelocity = this.movingHandHoldReleaseVelocity;
				this.playerRigidBody.linearVelocity = Vector3.zero;
				Vector3 vector2 = vector - position;
				this.playerRigidBody.transform.position += vector2;
				this.movingHandHoldReleaseVelocity = vector2 / timeDelta;
				Quaternion rotationDelta = this.activeHandHold.objectHeld.rotation * Quaternion.Inverse(this.lastHandHoldRotation);
				this.RotateWithSurface(rotationDelta, vector);
				this.lastHandHoldRotation = this.activeHandHold.objectHeld.rotation;
				return;
			}
			this.playerRigidBody.linearVelocity = (vector - position) / timeDelta;
			if (this.activeHandHold.applyRotation)
			{
				this.turnParent.transform.RotateAround(vector, base.transform.up, this.activeHandHold.localRotationalOffset - (this.activeHandHold.grabber.transform.rotation.eulerAngles.y - this.activeHandHold.objectHeld.rotation.eulerAngles.y));
			}
		}

		// Token: 0x060063BB RID: 25531 RVA: 0x002064B2 File Offset: 0x002046B2
		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|406_0(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
				return;
			}
			if (val < -maxDist)
			{
				val = -maxDist;
			}
		}

		// Token: 0x040071D7 RID: 29143
		private static GTPlayer _instance;

		// Token: 0x040071D8 RID: 29144
		public static bool hasInstance;

		// Token: 0x040071D9 RID: 29145
		public Camera mainCamera;

		// Token: 0x040071DA RID: 29146
		public SphereCollider headCollider;

		// Token: 0x040071DB RID: 29147
		public CapsuleCollider bodyCollider;

		// Token: 0x040071DC RID: 29148
		private float bodyInitialRadius;

		// Token: 0x040071DD RID: 29149
		private float bodyInitialHeight;

		// Token: 0x040071DE RID: 29150
		private RaycastHit bodyHitInfo;

		// Token: 0x040071DF RID: 29151
		private RaycastHit lastHitInfoHand;

		// Token: 0x040071E0 RID: 29152
		public GorillaVelocityTracker bodyVelocityTracker;

		// Token: 0x040071E1 RID: 29153
		public PlayerAudioManager audioManager;

		// Token: 0x040071E2 RID: 29154
		[SerializeField]
		private GTPlayer.HandState leftHand;

		// Token: 0x040071E3 RID: 29155
		[SerializeField]
		private GTPlayer.HandState rightHand;

		// Token: 0x040071E4 RID: 29156
		private GTPlayer.HandState[] stiltStates = new GTPlayer.HandState[12];

		// Token: 0x040071E5 RID: 29157
		private bool anyHandIsColliding;

		// Token: 0x040071E6 RID: 29158
		private bool anyHandWasColliding;

		// Token: 0x040071E7 RID: 29159
		private bool anyHandIsSliding;

		// Token: 0x040071E8 RID: 29160
		private bool anyHandWasSliding;

		// Token: 0x040071E9 RID: 29161
		private bool anyHandIsSticking;

		// Token: 0x040071EA RID: 29162
		private bool anyHandWasSticking;

		// Token: 0x040071EB RID: 29163
		private bool forceRBSync;

		// Token: 0x040071EC RID: 29164
		public Vector3 lastHeadPosition;

		// Token: 0x040071ED RID: 29165
		private Vector3 lastRigidbodyPosition;

		// Token: 0x040071EE RID: 29166
		private Rigidbody playerRigidBody;

		// Token: 0x040071EF RID: 29167
		private RigidbodyInterpolation playerRigidbodyInterpolationDefault;

		// Token: 0x040071F0 RID: 29168
		public int velocityHistorySize;

		// Token: 0x040071F1 RID: 29169
		public float maxArmLength = 1f;

		// Token: 0x040071F2 RID: 29170
		public float unStickDistance = 1f;

		// Token: 0x040071F3 RID: 29171
		public float velocityLimit;

		// Token: 0x040071F4 RID: 29172
		public float slideVelocityLimit;

		// Token: 0x040071F5 RID: 29173
		public float maxJumpSpeed;

		// Token: 0x040071F6 RID: 29174
		private float _jumpMultiplier;

		// Token: 0x040071F7 RID: 29175
		public float minimumRaycastDistance = 0.05f;

		// Token: 0x040071F8 RID: 29176
		public float defaultSlideFactor = 0.03f;

		// Token: 0x040071F9 RID: 29177
		public float slidingMinimum = 0.9f;

		// Token: 0x040071FA RID: 29178
		public float defaultPrecision = 0.995f;

		// Token: 0x040071FB RID: 29179
		public float teleportThresholdNoVel = 1f;

		// Token: 0x040071FC RID: 29180
		public float frictionConstant = 1f;

		// Token: 0x040071FD RID: 29181
		public float slideControl = 0.00425f;

		// Token: 0x040071FE RID: 29182
		public float stickDepth = 0.01f;

		// Token: 0x040071FF RID: 29183
		private Vector3[] velocityHistory;

		// Token: 0x04007200 RID: 29184
		private Vector3[] slideAverageHistory;

		// Token: 0x04007201 RID: 29185
		private int velocityIndex;

		// Token: 0x04007202 RID: 29186
		private Vector3 currentVelocity;

		// Token: 0x04007203 RID: 29187
		private Vector3 averagedVelocity;

		// Token: 0x04007204 RID: 29188
		private Vector3 lastPosition;

		// Token: 0x04007205 RID: 29189
		public Vector3 bodyOffset;

		// Token: 0x04007206 RID: 29190
		public LayerMask locomotionEnabledLayers;

		// Token: 0x04007207 RID: 29191
		public LayerMask waterLayer;

		// Token: 0x04007208 RID: 29192
		public bool wasHeadTouching;

		// Token: 0x04007209 RID: 29193
		public int currentMaterialIndex;

		// Token: 0x0400720A RID: 29194
		public Vector3 headSlideNormal;

		// Token: 0x0400720B RID: 29195
		public float headSlipPercentage;

		// Token: 0x0400720C RID: 29196
		[SerializeField]
		private Transform cosmeticsHeadTarget;

		// Token: 0x0400720D RID: 29197
		[SerializeField]
		private float nativeScale = 1f;

		// Token: 0x0400720E RID: 29198
		[SerializeField]
		private float scaleMultiplier = 1f;

		// Token: 0x0400720F RID: 29199
		private NativeSizeChangerSettings activeSizeChangerSettings;

		// Token: 0x04007210 RID: 29200
		public bool debugMovement;

		// Token: 0x04007211 RID: 29201
		public bool disableMovement;

		// Token: 0x04007212 RID: 29202
		[NonSerialized]
		public bool inOverlay;

		// Token: 0x04007213 RID: 29203
		[NonSerialized]
		public bool isUserPresent;

		// Token: 0x04007214 RID: 29204
		public GameObject turnParent;

		// Token: 0x04007215 RID: 29205
		public GorillaSurfaceOverride currentOverride;

		// Token: 0x04007216 RID: 29206
		public MaterialDatasSO materialDatasSO;

		// Token: 0x04007217 RID: 29207
		private float degreesTurnedThisFrame;

		// Token: 0x04007218 RID: 29208
		private Vector3 bodyOffsetVector;

		// Token: 0x04007219 RID: 29209
		private Vector3 movementToProjectedAboveCollisionPlane;

		// Token: 0x0400721A RID: 29210
		private MeshCollider meshCollider;

		// Token: 0x0400721B RID: 29211
		private Mesh collidedMesh;

		// Token: 0x0400721C RID: 29212
		private GTPlayer.MaterialData foundMatData;

		// Token: 0x0400721D RID: 29213
		private string findMatName;

		// Token: 0x0400721E RID: 29214
		private int vertex1;

		// Token: 0x0400721F RID: 29215
		private int vertex2;

		// Token: 0x04007220 RID: 29216
		private int vertex3;

		// Token: 0x04007221 RID: 29217
		private List<int> trianglesList = new List<int>(1000000);

		// Token: 0x04007222 RID: 29218
		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		// Token: 0x04007223 RID: 29219
		private int[] sharedMeshTris;

		// Token: 0x04007224 RID: 29220
		private float lastRealTime;

		// Token: 0x04007225 RID: 29221
		private float calcDeltaTime;

		// Token: 0x04007226 RID: 29222
		private float tempRealTime;

		// Token: 0x04007227 RID: 29223
		private Vector3 slideVelocity;

		// Token: 0x04007228 RID: 29224
		private Vector3 slideAverageNormal;

		// Token: 0x04007229 RID: 29225
		private RaycastHit tempHitInfo;

		// Token: 0x0400722A RID: 29226
		private RaycastHit junkHit;

		// Token: 0x0400722B RID: 29227
		private Vector3 firstPosition;

		// Token: 0x0400722C RID: 29228
		private RaycastHit tempIterativeHit;

		// Token: 0x0400722D RID: 29229
		private float maxSphereSize1;

		// Token: 0x0400722E RID: 29230
		private float maxSphereSize2;

		// Token: 0x0400722F RID: 29231
		private Collider[] overlapColliders = new Collider[10];

		// Token: 0x04007230 RID: 29232
		private int overlapAttempts;

		// Token: 0x04007231 RID: 29233
		private float averageSlipPercentage;

		// Token: 0x04007232 RID: 29234
		private Vector3 surfaceDirection;

		// Token: 0x04007233 RID: 29235
		public float iceThreshold = 0.9f;

		// Token: 0x04007234 RID: 29236
		private float bodyMaxRadius;

		// Token: 0x04007235 RID: 29237
		public float bodyLerp = 0.17f;

		// Token: 0x04007236 RID: 29238
		private bool areBothTouching;

		// Token: 0x04007237 RID: 29239
		private float slideFactor;

		// Token: 0x04007238 RID: 29240
		[DebugOption]
		public bool didAJump;

		// Token: 0x04007239 RID: 29241
		private bool updateRB;

		// Token: 0x0400723A RID: 29242
		private Renderer slideRenderer;

		// Token: 0x0400723B RID: 29243
		private RaycastHit[] rayCastNonAllocColliders;

		// Token: 0x0400723C RID: 29244
		private Vector3[] crazyCheckVectors;

		// Token: 0x0400723D RID: 29245
		private RaycastHit emptyHit;

		// Token: 0x0400723E RID: 29246
		private int bufferCount;

		// Token: 0x0400723F RID: 29247
		private Vector3 lastOpenHeadPosition;

		// Token: 0x04007240 RID: 29248
		private List<Material> tempMaterialArray = new List<Material>(16);

		// Token: 0x04007241 RID: 29249
		private Vector3? antiDriftLastPosition;

		// Token: 0x04007242 RID: 29250
		private const float CameraFarClipDefault = 500f;

		// Token: 0x04007243 RID: 29251
		private const float CameraNearClipDefault = 0.01f;

		// Token: 0x04007244 RID: 29252
		private const float CameraNearClipTiny = 0.002f;

		// Token: 0x04007245 RID: 29253
		private Dictionary<GameObject, PhysicsMaterial> bodyTouchedSurfaces;

		// Token: 0x04007246 RID: 29254
		private bool primaryButtonPressed = true;

		// Token: 0x04007247 RID: 29255
		[Header("Swimming")]
		public PlayerSwimmingParameters swimmingParams;

		// Token: 0x04007248 RID: 29256
		public WaterParameters waterParams;

		// Token: 0x04007249 RID: 29257
		public List<GTPlayer.LiquidProperties> liquidPropertiesList = new List<GTPlayer.LiquidProperties>(16);

		// Token: 0x0400724A RID: 29258
		public bool debugDrawSwimming;

		// Token: 0x0400724B RID: 29259
		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		// Token: 0x0400724C RID: 29260
		public GameObject geodeHitEffects;

		// Token: 0x0400724D RID: 29261
		[Header("Freeze Tag")]
		public float freezeTagHandSlidePercent = 0.88f;

		// Token: 0x0400724E RID: 29262
		public bool debugFreezeTag;

		// Token: 0x0400724F RID: 29263
		public float frozenBodyBuoyancyFactor = 1.5f;

		// Token: 0x04007251 RID: 29265
		[Space]
		private WaterVolume leftHandWaterVolume;

		// Token: 0x04007252 RID: 29266
		private WaterVolume rightHandWaterVolume;

		// Token: 0x04007253 RID: 29267
		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		// Token: 0x04007254 RID: 29268
		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		// Token: 0x04007255 RID: 29269
		private Vector3 swimmingVelocity = Vector3.zero;

		// Token: 0x04007256 RID: 29270
		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		// Token: 0x04007257 RID: 29271
		private bool bodyInWater;

		// Token: 0x04007258 RID: 29272
		private bool headInWater;

		// Token: 0x04007259 RID: 29273
		private bool audioSetToUnderwater;

		// Token: 0x0400725A RID: 29274
		private float buoyancyExtension;

		// Token: 0x0400725B RID: 29275
		private float lastWaterSurfaceJumpTimeLeft = -1f;

		// Token: 0x0400725C RID: 29276
		private float lastWaterSurfaceJumpTimeRight = -1f;

		// Token: 0x0400725D RID: 29277
		private float waterSurfaceJumpCooldown = 0.1f;

		// Token: 0x0400725E RID: 29278
		private float leftHandNonDiveHapticsAmount;

		// Token: 0x0400725F RID: 29279
		private float rightHandNonDiveHapticsAmount;

		// Token: 0x04007260 RID: 29280
		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x04007261 RID: 29281
		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x04007262 RID: 29282
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04007264 RID: 29284
		private Quaternion playerRotationOverride;

		// Token: 0x04007265 RID: 29285
		private int playerRotationOverrideFrame = -1;

		// Token: 0x04007266 RID: 29286
		private float playerRotationOverrideDecayRate = Mathf.Exp(1.5f);

		// Token: 0x04007268 RID: 29288
		private ContactPoint[] bodyCollisionContacts = new ContactPoint[8];

		// Token: 0x04007269 RID: 29289
		private int bodyCollisionContactsCount;

		// Token: 0x0400726A RID: 29290
		private ContactPoint bodyGroundContact;

		// Token: 0x0400726B RID: 29291
		private float bodyGroundContactTime;

		// Token: 0x0400726C RID: 29292
		private const float movingSurfaceVelocityLimit = 40f;

		// Token: 0x0400726D RID: 29293
		private bool exitMovingSurface;

		// Token: 0x0400726E RID: 29294
		private float exitMovingSurfaceThreshold = 6f;

		// Token: 0x0400726F RID: 29295
		private bool isClimbableMoving;

		// Token: 0x04007270 RID: 29296
		private Quaternion lastClimbableRotation;

		// Token: 0x04007271 RID: 29297
		private int lastAttachedToMovingSurfaceFrame;

		// Token: 0x04007272 RID: 29298
		private const int MIN_FRAMES_OFF_SURFACE_TO_DETACH = 3;

		// Token: 0x04007273 RID: 29299
		private bool isHandHoldMoving;

		// Token: 0x04007274 RID: 29300
		private Quaternion lastHandHoldRotation;

		// Token: 0x04007275 RID: 29301
		private Vector3 movingHandHoldReleaseVelocity;

		// Token: 0x04007276 RID: 29302
		private GTPlayer.MovingSurfaceContactPoint lastMovingSurfaceContact;

		// Token: 0x04007277 RID: 29303
		private int lastMovingSurfaceID = -1;

		// Token: 0x04007278 RID: 29304
		private BuilderPiece lastMonkeBlock;

		// Token: 0x04007279 RID: 29305
		private Quaternion lastMovingSurfaceRot;

		// Token: 0x0400727A RID: 29306
		private RaycastHit lastMovingSurfaceHit;

		// Token: 0x0400727B RID: 29307
		private Vector3 lastMovingSurfaceTouchLocal;

		// Token: 0x0400727C RID: 29308
		private Vector3 lastMovingSurfaceTouchWorld;

		// Token: 0x0400727D RID: 29309
		private Vector3 movingSurfaceOffset;

		// Token: 0x0400727E RID: 29310
		private bool wasMovingSurfaceMonkeBlock;

		// Token: 0x0400727F RID: 29311
		private Vector3 lastMovingSurfaceVelocity;

		// Token: 0x04007280 RID: 29312
		private bool wasBodyOnGround;

		// Token: 0x04007281 RID: 29313
		private BasePlatform currentPlatform;

		// Token: 0x04007282 RID: 29314
		private BasePlatform lastPlatformTouched;

		// Token: 0x04007283 RID: 29315
		private Vector3 lastFrameTouchPosLocal;

		// Token: 0x04007284 RID: 29316
		private Vector3 lastFrameTouchPosWorld;

		// Token: 0x04007285 RID: 29317
		private bool lastFrameHasValidTouchPos;

		// Token: 0x04007286 RID: 29318
		private Vector3 refMovement = Vector3.zero;

		// Token: 0x04007287 RID: 29319
		private Vector3 platformTouchOffset;

		// Token: 0x04007288 RID: 29320
		private Vector3 debugLastRightHandPosition;

		// Token: 0x04007289 RID: 29321
		private Vector3 debugPlatformDeltaPosition;

		// Token: 0x0400728A RID: 29322
		public double tempFreezeRightHandEnableTime;

		// Token: 0x0400728B RID: 29323
		public double tempFreezeLeftHandEnableTime;

		// Token: 0x0400728C RID: 29324
		private const float climbingMaxThrowSpeed = 5.5f;

		// Token: 0x0400728D RID: 29325
		private const float climbHelperSmoothSnapSpeed = 12f;

		// Token: 0x0400728E RID: 29326
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x0400728F RID: 29327
		private GorillaClimbable currentClimbable;

		// Token: 0x04007290 RID: 29328
		private GorillaHandClimber currentClimber;

		// Token: 0x04007291 RID: 29329
		private Vector3 climbHelperTargetPos = Vector3.zero;

		// Token: 0x04007292 RID: 29330
		private Transform climbHelper;

		// Token: 0x04007293 RID: 29331
		private GorillaRopeSwing currentSwing;

		// Token: 0x04007294 RID: 29332
		private GorillaZipline currentZipline;

		// Token: 0x04007295 RID: 29333
		[SerializeField]
		private ConnectedControllerHandler controllerState;

		// Token: 0x04007296 RID: 29334
		public int sizeLayerMask;

		// Token: 0x04007297 RID: 29335
		public bool InReportMenu;

		// Token: 0x04007298 RID: 29336
		private LayerChanger layerChanger;

		// Token: 0x0400729B RID: 29339
		private bool hasCorrectedForTracking;

		// Token: 0x0400729C RID: 29340
		private float halloweenLevitationStrength;

		// Token: 0x0400729D RID: 29341
		private float halloweenLevitationFullStrengthDuration;

		// Token: 0x0400729E RID: 29342
		private float halloweenLevitationTotalDuration = 1f;

		// Token: 0x0400729F RID: 29343
		private float halloweenLevitationBonusStrength;

		// Token: 0x040072A0 RID: 29344
		private float halloweenLevitateBonusOffAtYSpeed;

		// Token: 0x040072A1 RID: 29345
		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		// Token: 0x040072A2 RID: 29346
		private float lastTouchedGroundTimestamp;

		// Token: 0x040072A3 RID: 29347
		private bool teleportToTrain;

		// Token: 0x040072A4 RID: 29348
		public bool isAttachedToTrain;

		// Token: 0x040072A5 RID: 29349
		private bool stuckLeft;

		// Token: 0x040072A6 RID: 29350
		private bool stuckRight;

		// Token: 0x040072A7 RID: 29351
		private float lastScale;

		// Token: 0x040072A8 RID: 29352
		private Vector3 currentSlopDirection;

		// Token: 0x040072A9 RID: 29353
		private Vector3 lastSlopeDirection = Vector3.zero;

		// Token: 0x040072AA RID: 29354
		private readonly Dictionary<Object, Action<GTPlayer>> gravityOverrides = new Dictionary<Object, Action<GTPlayer>>();

		// Token: 0x040072AD RID: 29357
		private int hoverAllowedCount;

		// Token: 0x040072AE RID: 29358
		[Header("Hoverboard")]
		[SerializeField]
		private float hoverIdealHeight = 0.5f;

		// Token: 0x040072AF RID: 29359
		[SerializeField]
		private float hoverCarveSidewaysSpeedLossFactor = 1f;

		// Token: 0x040072B0 RID: 29360
		[SerializeField]
		private AnimationCurve hoverCarveAngleResponsiveness;

		// Token: 0x040072B1 RID: 29361
		[SerializeField]
		private HoverboardVisual hoverboardVisual;

		// Token: 0x040072B2 RID: 29362
		[SerializeField]
		private float sidewaysDrag = 0.1f;

		// Token: 0x040072B3 RID: 29363
		[SerializeField]
		private float hoveringSlowSpeed = 0.1f;

		// Token: 0x040072B4 RID: 29364
		[SerializeField]
		private float hoveringSlowStoppingFactor = 0.95f;

		// Token: 0x040072B5 RID: 29365
		[SerializeField]
		private float hoverboardPaddleBoostMultiplier = 0.1f;

		// Token: 0x040072B6 RID: 29366
		[SerializeField]
		private float hoverboardPaddleBoostMax = 10f;

		// Token: 0x040072B7 RID: 29367
		[SerializeField]
		private float hoverboardBoostGracePeriod = 1f;

		// Token: 0x040072B8 RID: 29368
		[SerializeField]
		private float hoverBodyHasCollisionsOutsideRadius = 0.5f;

		// Token: 0x040072B9 RID: 29369
		[SerializeField]
		private float hoverBodyCollisionRadiusUpOffset = 0.2f;

		// Token: 0x040072BA RID: 29370
		[SerializeField]
		private float hoverGeneralUpwardForce = 8f;

		// Token: 0x040072BB RID: 29371
		[SerializeField]
		private float hoverTiltAdjustsForwardFactor = 0.2f;

		// Token: 0x040072BC RID: 29372
		[SerializeField]
		private float hoverMinGrindSpeed = 1f;

		// Token: 0x040072BD RID: 29373
		[SerializeField]
		private float hoverSlamJumpStrengthFactor = 25f;

		// Token: 0x040072BE RID: 29374
		[SerializeField]
		private float hoverMaxPaddleSpeed = 35f;

		// Token: 0x040072BF RID: 29375
		[SerializeField]
		private HoverboardAudio hoverboardAudio;

		// Token: 0x040072C0 RID: 29376
		private bool hasHoverPoint;

		// Token: 0x040072C1 RID: 29377
		private float boostEnabledUntilTimestamp;

		// Token: 0x040072C2 RID: 29378
		private GTPlayer.HoverBoardCast[] hoverboardCasts = new GTPlayer.HoverBoardCast[]
		{
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 1f, 0.36f),
				localDirection = Vector3.down,
				distance = 1f,
				sphereRadius = 0.2f,
				intersectToVelocityCap = 0.1f
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, 0.36f),
				localDirection = Vector3.forward,
				distance = 0.25f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, -0.1f),
				localDirection = -Vector3.forward,
				distance = 0.24f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			}
		};

		// Token: 0x040072C3 RID: 29379
		private Vector3 hoverboardPlayerLocalPos;

		// Token: 0x040072C4 RID: 29380
		private Quaternion hoverboardPlayerLocalRot;

		// Token: 0x040072C5 RID: 29381
		private bool didHoverLastFrame;

		// Token: 0x040072C6 RID: 29382
		private bool hasLeftHandTentacleMove;

		// Token: 0x040072C7 RID: 29383
		private bool hasRightHandTentacleMove;

		// Token: 0x040072C8 RID: 29384
		private Vector3 leftHandTentacleMove;

		// Token: 0x040072C9 RID: 29385
		private Vector3 rightHandTentacleMove;

		// Token: 0x040072CA RID: 29386
		private GTPlayer.HandHoldState activeHandHold;

		// Token: 0x040072CB RID: 29387
		private GTPlayer.HandHoldState secondaryHandHold;

		// Token: 0x040072CC RID: 29388
		public PhysicsMaterial slipperyMaterial;

		// Token: 0x040072CD RID: 29389
		private bool wasHoldingHandhold;

		// Token: 0x040072CE RID: 29390
		private Vector3 secondLastPreHandholdVelocity;

		// Token: 0x040072CF RID: 29391
		private Vector3 lastPreHandholdVelocity;

		// Token: 0x040072D0 RID: 29392
		[Header("Native Scale Adjustment")]
		[SerializeField]
		private AnimationCurve nativeScaleMagnitudeAdjustmentFactor;

		// Token: 0x02000F7E RID: 3966
		[Serializable]
		public struct HandState
		{
			// Token: 0x060063BC RID: 25532 RVA: 0x002064C8 File Offset: 0x002046C8
			public void Init(GTPlayer gtPlayer, bool isLeftHand, float maxArmLength)
			{
				this.gtPlayer = gtPlayer;
				this.isLeftHand = isLeftHand;
				this.maxArmLength = maxArmLength;
				this.lastPosition = this.controllerTransform.position;
				this.lastRotation = this.controllerTransform.rotation;
				if (this.handFollower != null)
				{
					this.handFollower.transform.position = this.lastPosition;
					this.handFollower.transform.rotation = this.lastRotation;
				}
				this.wasColliding = false;
				this.slipSetToMaxFrameIdx = -1;
			}

			// Token: 0x060063BD RID: 25533 RVA: 0x00206554 File Offset: 0x00204754
			public void OnTeleport()
			{
				this.wasColliding = false;
				this.isColliding = false;
				this.isSliding = false;
				this.wasSliding = false;
				this.handFollower.position = this.controllerTransform.position;
				this.handFollower.rotation = this.controllerTransform.rotation;
				this.lastPosition = this.handFollower.transform.position;
				this.lastRotation = this.handFollower.transform.rotation;
			}

			// Token: 0x060063BE RID: 25534 RVA: 0x002065D5 File Offset: 0x002047D5
			public Vector3 GetLastPosition()
			{
				return this.lastPosition + this.gtPlayer.MovingSurfaceMovement();
			}

			// Token: 0x060063BF RID: 25535 RVA: 0x002065ED File Offset: 0x002047ED
			public bool SlipOverriddenToMax()
			{
				return this.slipSetToMaxFrameIdx == Time.frameCount;
			}

			// Token: 0x060063C0 RID: 25536 RVA: 0x002065FC File Offset: 0x002047FC
			public void FirstIteration(ref Vector3 totalMove, ref int divisor, float paddleBoostFactor)
			{
				if (this.hasCustomBoost)
				{
					this.boostVectorThisFrame = this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity(false, 0.15f, false) * this.customBoostFactor;
				}
				else
				{
					this.boostVectorThisFrame = (this.gtPlayer.enableHoverMode ? (this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity(false, 0.15f, false) * paddleBoostFactor) : Vector3.zero);
				}
				Vector3 vector = this.GetCurrentHandPosition() + this.gtPlayer.movingSurfaceOffset;
				Vector3 vector2 = this.GetLastPosition();
				Vector3 vector3 = vector - vector2;
				bool flag = this.gtPlayer.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT;
				if (!this.gtPlayer.didAJump && this.wasSliding && Vector3.Dot(this.gtPlayer.slideAverageNormal, Vector3.up) > 0f)
				{
					vector3 += Vector3.Project(-this.gtPlayer.slideAverageNormal * this.gtPlayer.stickDepth * this.gtPlayer.scale, Vector3.down);
				}
				float num = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
				if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
				{
					num = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
				}
				Vector3 vector4 = Vector3.zero;
				if (flag && !this.gtPlayer.exitMovingSurface)
				{
					vector4 = Vector3.Project(-this.gtPlayer.lastMovingSurfaceHit.normal * (this.gtPlayer.stickDepth * this.gtPlayer.scale), Vector3.down);
					if (this.gtPlayer.scale < 0.5f)
					{
						Vector3 normalized = this.gtPlayer.MovingSurfaceMovement().normalized;
						if (normalized != Vector3.zero)
						{
							float num2 = Vector3.Dot(Vector3.up, normalized);
							if ((double)num2 > 0.9 || (double)num2 < -0.9)
							{
								vector4 *= 6f;
								num *= 1.1f;
							}
						}
					}
				}
				Vector3 vector5;
				RaycastHit lastHitInfoHand;
				Vector3 vector6;
				if (this.gtPlayer.IterativeCollisionSphereCast(vector2, num, vector3 + vector4, this.boostVectorThisFrame, out vector5, true, out this.slipPercentage, out lastHitInfoHand, this.SlipOverriddenToMax()) && !this.isHolding && !this.gtPlayer.InReportMenu)
				{
					if (this.wasColliding && this.slipPercentage <= this.gtPlayer.defaultSlideFactor && !this.boostVectorThisFrame.IsLongerThan(0f))
					{
						vector6 = vector2 - vector;
					}
					else
					{
						vector6 = vector5 - vector;
					}
					this.isSliding = (this.slipPercentage > this.gtPlayer.iceThreshold);
					this.slideNormal = this.gtPlayer.tempHitInfo.normal;
					this.isColliding = true;
					this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
					this.surfaceOverride = this.gtPlayer.currentOverride;
					this.gtPlayer.lastHitInfoHand = lastHitInfoHand;
					this.lastHitInfo = lastHitInfoHand;
				}
				else
				{
					vector6 = Vector3.zero;
					this.slipPercentage = 0f;
					this.isSliding = false;
					this.slideNormal = Vector3.up;
					this.isColliding = false;
					this.materialTouchIndex = 0;
					this.surfaceOverride = null;
				}
				bool flag2 = this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid;
				this.isColliding = (this.isColliding && flag2);
				this.isSliding = (this.isSliding && flag2);
				if (this.isColliding)
				{
					this.gtPlayer.anyHandIsColliding = true;
					if (this.isSliding)
					{
						this.gtPlayer.anyHandIsSliding = true;
					}
					else
					{
						this.gtPlayer.anyHandIsSticking = true;
					}
				}
				if (this.isColliding || this.wasColliding)
				{
					if (!this.surfaceOverride || !this.surfaceOverride.disablePushBackEffect)
					{
						totalMove += vector6;
					}
					divisor++;
				}
			}

			// Token: 0x060063C1 RID: 25537 RVA: 0x00206A80 File Offset: 0x00204C80
			public void FinalizeHandPosition()
			{
				Vector3 vector = this.GetLastPosition();
				if (Time.time < this.tempFreezeUntilTimestamp)
				{
					this.finalPositionThisFrame = vector;
				}
				else
				{
					Vector3 movementVector = this.GetCurrentHandPosition() - vector;
					float sphereRadius = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
					if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
					{
						sphereRadius = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
					}
					Vector3 vector2;
					float num;
					RaycastHit lastHitInfoHand;
					if (this.gtPlayer.IterativeCollisionSphereCast(vector, sphereRadius, movementVector, this.boostVectorThisFrame, out vector2, this.gtPlayer.areBothTouching, out num, out lastHitInfoHand, false) && !this.isHolding)
					{
						this.isColliding = true;
						this.isSliding = (num > this.gtPlayer.iceThreshold);
						this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
						this.surfaceOverride = this.gtPlayer.currentOverride;
						this.gtPlayer.lastHitInfoHand = lastHitInfoHand;
						this.lastHitInfo = lastHitInfoHand;
						this.finalPositionThisFrame = vector2;
					}
					else
					{
						this.finalPositionThisFrame = this.GetCurrentHandPosition();
					}
				}
				bool flag = this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid;
				this.isColliding = (this.isColliding && flag);
				this.isSliding = (this.isSliding && flag);
				if (this.isColliding)
				{
					this.gtPlayer.anyHandIsColliding = true;
					if (this.isSliding)
					{
						this.gtPlayer.anyHandIsSliding = true;
						return;
					}
					this.gtPlayer.anyHandIsSticking = true;
				}
			}

			// Token: 0x060063C2 RID: 25538 RVA: 0x002065ED File Offset: 0x002047ED
			public bool IsSlipOverriddenToMax()
			{
				return this.slipSetToMaxFrameIdx == Time.frameCount;
			}

			// Token: 0x060063C3 RID: 25539 RVA: 0x00206C38 File Offset: 0x00204E38
			public Vector3 GetCurrentHandPosition()
			{
				Vector3 position = this.gtPlayer.headCollider.transform.position;
				if (this.gtPlayer.inOverlay)
				{
					return position + this.gtPlayer.headCollider.transform.up * -0.5f * this.gtPlayer.scale;
				}
				Vector3 vector = this.gtPlayer.PositionWithOffset(this.controllerTransform, this.handOffset);
				if ((vector - position).IsShorterThan(this.maxArmLength * this.gtPlayer.scale))
				{
					return vector;
				}
				return position + (vector - position).normalized * this.maxArmLength * this.gtPlayer.scale;
			}

			// Token: 0x060063C4 RID: 25540 RVA: 0x00206D08 File Offset: 0x00204F08
			public void PositionHandFollower()
			{
				this.handFollower.position = this.finalPositionThisFrame;
				this.handFollower.rotation = this.lastRotation;
			}

			// Token: 0x060063C5 RID: 25541 RVA: 0x00206D2C File Offset: 0x00204F2C
			public void OnEndOfFrame()
			{
				this.wasColliding = this.isColliding;
				this.wasSliding = this.isSliding;
				this.lastPosition = this.finalPositionThisFrame;
				if (Time.time > this.tempFreezeUntilTimestamp)
				{
					this.lastRotation = this.controllerTransform.rotation * this.handRotOffset;
				}
			}

			// Token: 0x060063C6 RID: 25542 RVA: 0x00206D86 File Offset: 0x00204F86
			public void TempFreezeHand(float freezeDuration)
			{
				this.tempFreezeUntilTimestamp = Math.Max(this.tempFreezeUntilTimestamp, Time.time + freezeDuration);
			}

			// Token: 0x060063C7 RID: 25543 RVA: 0x00206DA0 File Offset: 0x00204FA0
			public void GetHandTapData(out bool wasHandTouching, out bool wasSliding, out int handMatIndex, out GorillaSurfaceOverride surfaceOverride, out RaycastHit handHitInfo, out Vector3 handPosition, out GorillaVelocityTracker handVelocityTracker)
			{
				wasHandTouching = this.wasColliding;
				wasSliding = this.wasSliding;
				handMatIndex = this.materialTouchIndex;
				surfaceOverride = this.surfaceOverride;
				handHitInfo = this.lastHitInfo;
				handPosition = this.finalPositionThisFrame;
				handVelocityTracker = this.velocityTracker;
			}

			// Token: 0x040072D1 RID: 29393
			[NonSerialized]
			public Vector3 lastPosition;

			// Token: 0x040072D2 RID: 29394
			[NonSerialized]
			public Quaternion lastRotation;

			// Token: 0x040072D3 RID: 29395
			[NonSerialized]
			public bool isLeftHand;

			// Token: 0x040072D4 RID: 29396
			[NonSerialized]
			public bool wasColliding;

			// Token: 0x040072D5 RID: 29397
			[NonSerialized]
			public bool isColliding;

			// Token: 0x040072D6 RID: 29398
			[NonSerialized]
			public bool wasSliding;

			// Token: 0x040072D7 RID: 29399
			[NonSerialized]
			public bool isSliding;

			// Token: 0x040072D8 RID: 29400
			[NonSerialized]
			public bool isHolding;

			// Token: 0x040072D9 RID: 29401
			[NonSerialized]
			public Vector3 slideNormal;

			// Token: 0x040072DA RID: 29402
			[NonSerialized]
			public float slipPercentage;

			// Token: 0x040072DB RID: 29403
			[NonSerialized]
			public Vector3 hitPoint;

			// Token: 0x040072DC RID: 29404
			[NonSerialized]
			private Vector3 boostVectorThisFrame;

			// Token: 0x040072DD RID: 29405
			[NonSerialized]
			public Vector3 finalPositionThisFrame;

			// Token: 0x040072DE RID: 29406
			[NonSerialized]
			public int slipSetToMaxFrameIdx;

			// Token: 0x040072DF RID: 29407
			[NonSerialized]
			public int materialTouchIndex;

			// Token: 0x040072E0 RID: 29408
			[NonSerialized]
			public GorillaSurfaceOverride surfaceOverride;

			// Token: 0x040072E1 RID: 29409
			[NonSerialized]
			public RaycastHit hitInfo;

			// Token: 0x040072E2 RID: 29410
			[NonSerialized]
			public RaycastHit lastHitInfo;

			// Token: 0x040072E3 RID: 29411
			[NonSerialized]
			private GTPlayer gtPlayer;

			// Token: 0x040072E4 RID: 29412
			[SerializeField]
			public Transform handFollower;

			// Token: 0x040072E5 RID: 29413
			[SerializeField]
			public Transform controllerTransform;

			// Token: 0x040072E6 RID: 29414
			[SerializeField]
			public GorillaVelocityTracker velocityTracker;

			// Token: 0x040072E7 RID: 29415
			[SerializeField]
			public GorillaVelocityTracker interactPointVelocityTracker;

			// Token: 0x040072E8 RID: 29416
			[SerializeField]
			public Vector3 handOffset;

			// Token: 0x040072E9 RID: 29417
			[SerializeField]
			public Quaternion handRotOffset;

			// Token: 0x040072EA RID: 29418
			[NonSerialized]
			public float tempFreezeUntilTimestamp;

			// Token: 0x040072EB RID: 29419
			[NonSerialized]
			public bool canTag;

			// Token: 0x040072EC RID: 29420
			[NonSerialized]
			public bool canStun;

			// Token: 0x040072ED RID: 29421
			private float maxArmLength;

			// Token: 0x040072EE RID: 29422
			[NonSerialized]
			public bool isActive;

			// Token: 0x040072EF RID: 29423
			[NonSerialized]
			public float customBoostFactor;

			// Token: 0x040072F0 RID: 29424
			[NonSerialized]
			public bool hasCustomBoost;
		}

		// Token: 0x02000F7F RID: 3967
		private enum MovingSurfaceContactPoint
		{
			// Token: 0x040072F2 RID: 29426
			NONE,
			// Token: 0x040072F3 RID: 29427
			RIGHT,
			// Token: 0x040072F4 RID: 29428
			LEFT,
			// Token: 0x040072F5 RID: 29429
			BODY
		}

		// Token: 0x02000F80 RID: 3968
		[Serializable]
		public struct MaterialData
		{
			// Token: 0x040072F6 RID: 29430
			public string matName;

			// Token: 0x040072F7 RID: 29431
			public bool overrideAudio;

			// Token: 0x040072F8 RID: 29432
			public AudioClip audio;

			// Token: 0x040072F9 RID: 29433
			public bool overrideSlidePercent;

			// Token: 0x040072FA RID: 29434
			public float slidePercent;

			// Token: 0x040072FB RID: 29435
			public int surfaceEffectIndex;
		}

		// Token: 0x02000F81 RID: 3969
		[Serializable]
		public struct LiquidProperties
		{
			// Token: 0x040072FC RID: 29436
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			// Token: 0x040072FD RID: 29437
			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			// Token: 0x040072FE RID: 29438
			[Range(0f, 3f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			// Token: 0x040072FF RID: 29439
			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		// Token: 0x02000F82 RID: 3970
		public enum LiquidType
		{
			// Token: 0x04007301 RID: 29441
			Water,
			// Token: 0x04007302 RID: 29442
			Lava
		}

		// Token: 0x02000F83 RID: 3971
		private struct HoverBoardCast
		{
			// Token: 0x04007303 RID: 29443
			public Vector3 localOrigin;

			// Token: 0x04007304 RID: 29444
			public Vector3 localDirection;

			// Token: 0x04007305 RID: 29445
			public float sphereRadius;

			// Token: 0x04007306 RID: 29446
			public float distance;

			// Token: 0x04007307 RID: 29447
			public float intersectToVelocityCap;

			// Token: 0x04007308 RID: 29448
			public bool isSolid;

			// Token: 0x04007309 RID: 29449
			public bool didHit;

			// Token: 0x0400730A RID: 29450
			public Vector3 pointHit;

			// Token: 0x0400730B RID: 29451
			public Vector3 normalHit;
		}

		// Token: 0x02000F84 RID: 3972
		private struct HandHoldState
		{
			// Token: 0x0400730C RID: 29452
			public GorillaGrabber grabber;

			// Token: 0x0400730D RID: 29453
			public Transform objectHeld;

			// Token: 0x0400730E RID: 29454
			public Vector3 localPositionHeld;

			// Token: 0x0400730F RID: 29455
			public float localRotationalOffset;

			// Token: 0x04007310 RID: 29456
			public bool applyRotation;
		}
	}
}
