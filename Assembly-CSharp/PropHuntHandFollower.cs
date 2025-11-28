using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x0200020D RID: 525
public class PropHuntHandFollower : MonoBehaviour, ICallBack
{
	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000E6F RID: 3695 RVA: 0x0004C370 File Offset: 0x0004A570
	// (set) Token: 0x06000E70 RID: 3696 RVA: 0x0004C378 File Offset: 0x0004A578
	public bool hasProp
	{
		get
		{
			return this._hasProp;
		}
		private set
		{
			this._hasProp = value;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0004C381 File Offset: 0x0004A581
	// (set) Token: 0x06000E72 RID: 3698 RVA: 0x0004C389 File Offset: 0x0004A589
	public bool IsInstantiatingAsync { get; private set; }

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0004C392 File Offset: 0x0004A592
	// (set) Token: 0x06000E74 RID: 3700 RVA: 0x0004C39A File Offset: 0x0004A59A
	public VRRig attachedToRig { get; private set; }

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000E75 RID: 3701 RVA: 0x0004C3A3 File Offset: 0x0004A5A3
	public bool IsLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0004C3AB File Offset: 0x0004A5AB
	public void Awake()
	{
		this.attachedToRig = base.GetComponent<VRRig>();
		this.attachedToRig.propHuntHandFollower = this;
		this._isLocal = this.attachedToRig.isOfflineVRRig;
		this.raycastHits = new RaycastHit[20];
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0004C3E3 File Offset: 0x0004A5E3
	public void Start()
	{
		this.attachedToRig.AddLateUpdateCallback(this);
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0004C3F1 File Offset: 0x0004A5F1
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropHandFollower(this);
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0004C3F9 File Offset: 0x0004A5F9
	private void OnDisable()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		this.DestroyProp();
		GorillaPropHuntGameManager.UnregisterPropHandFollower(this);
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0004C410 File Offset: 0x0004A610
	public void DestroyProp()
	{
		if (!this.hasProp || this._prop == null)
		{
			return;
		}
		PropHuntGrabbableProp prop;
		PropHuntTaggableProp prop2;
		if (this._prop.TryGetComponent<PropHuntGrabbableProp>(ref prop))
		{
			PropHuntPools.ReturnGrabbableProp(prop);
		}
		else if (this._prop.TryGetComponent<PropHuntTaggableProp>(ref prop2))
		{
			PropHuntPools.ReturnTaggableProp(prop2);
		}
		this._prop = null;
		this.hasProp = false;
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0004C470 File Offset: 0x0004A670
	public static void DestroyProp_NoPool(List<MeshCollider> _colliders, ref bool hasProp, ref GameObject _prop)
	{
		foreach (MeshCollider meshCollider in _colliders)
		{
			if (!(meshCollider == null))
			{
				meshCollider.gameObject.transform.parent = null;
				meshCollider.gameObject.SetActive(false);
			}
		}
		if (hasProp)
		{
			Object.Destroy(_prop);
		}
		_prop = null;
		hasProp = false;
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x00002789 File Offset: 0x00000989
	public void OnRoundStart()
	{
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0004C4F0 File Offset: 0x0004A6F0
	public void CreateProp()
	{
		if (this.hasProp)
		{
			this.DestroyProp();
		}
		this._isLeftHand = false;
		int num = GorillaPropHuntGameManager.instance.GetSeed();
		if (NetworkSystem.Instance.InRoom)
		{
			num += this.attachedToRig.OwningNetPlayer.ActorNumber;
		}
		SRand srand = new SRand(num);
		string cosmeticId = GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt());
		PropHuntTaggableProp propHuntTaggableProp;
		if (this._isLocal)
		{
			PropHuntGrabbableProp propHuntGrabbableProp;
			if (PropHuntPools.TryGetGrabbableProp(cosmeticId, out propHuntGrabbableProp))
			{
				this._grabbableProp = propHuntGrabbableProp;
				this._taggableProp = null;
				this._prop = propHuntGrabbableProp.gameObject;
				this._propOffset = this._grabbableProp.offset;
				propHuntGrabbableProp.handFollower = this;
				this.hasProp = true;
				for (int i = 0; i < propHuntGrabbableProp.interactionPoints.Count; i++)
				{
					propHuntGrabbableProp.interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
		}
		else if (PropHuntPools.TryGetTaggableProp(cosmeticId, out propHuntTaggableProp))
		{
			this._taggableProp = propHuntTaggableProp;
			this._grabbableProp = null;
			this._prop = propHuntTaggableProp.gameObject;
			this._propOffset = propHuntTaggableProp.offset;
			propHuntTaggableProp.ownerRig = this.attachedToRig;
			this.hasProp = true;
		}
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0004C620 File Offset: 0x0004A820
	public void OnPropLoaded(AsyncOperationHandle<GameObject> handle)
	{
		this.IsInstantiatingAsync = false;
		CosmeticSO debugCosmeticSO = null;
		if (PropHuntHandFollower.TryPrepPropTemplate(handle.Result, this._isLocal, debugCosmeticSO, this._colliders, this._interactionPoints, out this._grabbableProp, out this._taggableProp))
		{
			this._prop = handle.Result;
			this.hasProp = (this._prop != null);
			this._prop.SetActive(true);
			if (this._isLocal)
			{
				this._propOffset = this._grabbableProp.offset;
				this._grabbableProp.handFollower = this;
				for (int i = 0; i < this._interactionPoints.Count; i++)
				{
					this._interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
			this._propOffset = this._taggableProp.offset;
			this._taggableProp.ownerRig = this.attachedToRig;
		}
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0004C708 File Offset: 0x0004A908
	public static bool TryPrepPropTemplate(GameObject _prop, bool _isLocal, CosmeticSO debugCosmeticSO, List<MeshCollider> _colliders, List<InteractionPoint> ref_interactionPoints, out PropHuntGrabbableProp grabbableProp, out PropHuntTaggableProp taggableProp)
	{
		if (_isLocal)
		{
			grabbableProp = _prop.AddComponent<PropHuntGrabbableProp>();
			taggableProp = null;
			grabbableProp.interactionPoints = ref_interactionPoints;
		}
		else
		{
			taggableProp = _prop.AddComponent<PropHuntTaggableProp>();
			grabbableProp = null;
		}
		bool flag = false;
		bool flag2 = true;
		Bounds bounds = default(Bounds);
		int num = 0;
		foreach (MeshRenderer meshRenderer in _prop.GetComponentsInChildren<MeshRenderer>())
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			if (!(component == null))
			{
				Mesh sharedMesh = component.sharedMesh;
				if (!(sharedMesh == null) && sharedMesh.isReadable)
				{
					flag = true;
					if (flag2)
					{
						bounds = meshRenderer.bounds;
					}
					else
					{
						bounds.Encapsulate(meshRenderer.bounds);
					}
					MeshCollider meshCollider;
					if (num >= _colliders.Count)
					{
						GameObject gameObject = new GameObject("PropHuntTaggable");
						gameObject.layer = 14;
						meshCollider = gameObject.AddComponent<MeshCollider>();
						meshCollider.convex = true;
						meshCollider.isTrigger = true;
						if (_isLocal)
						{
							ref_interactionPoints.Add(gameObject.AddComponent<InteractionPoint>());
						}
						_colliders.Add(meshCollider);
					}
					else
					{
						meshCollider = _colliders[num];
						meshCollider.gameObject.SetActive(true);
					}
					meshCollider.transform.parent = _prop.transform;
					meshCollider.transform.position = meshRenderer.transform.position;
					meshCollider.transform.rotation = meshRenderer.transform.rotation;
					meshCollider.sharedMesh = sharedMesh;
					num++;
					flag2 = false;
				}
			}
		}
		if (!flag)
		{
			bool flag3 = true;
			PropHuntHandFollower.DestroyProp_NoPool(_colliders, ref flag3, ref _prop);
			return false;
		}
		Vector3 offset = _prop.transform.InverseTransformPoint(bounds.center);
		if (_isLocal)
		{
			grabbableProp.interactionPoints = ref_interactionPoints;
			grabbableProp.offset = offset;
		}
		else
		{
			taggableProp.offset = offset;
		}
		return true;
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0004C8CC File Offset: 0x0004AACC
	void ICallBack.CallBack()
	{
		if (!this.hasProp || this._prop.IsNull())
		{
			return;
		}
		Transform transform = this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget;
		Vector3 sourcePos = transform.position;
		if (this.attachedToRig.isLocal)
		{
			sourcePos = (this._isLeftHand ? this.attachedToRig.leftHand.overrideTarget.position : this.attachedToRig.rightHand.overrideTarget.position);
		}
		if ((this._isLeftHand ? Mathf.Max(this.attachedToRig.leftIndex.calcT, this.attachedToRig.leftMiddle.calcT) : Mathf.Max(this.attachedToRig.rightIndex.calcT, this.attachedToRig.rightMiddle.calcT)) > 0.5f)
		{
			this._prop.transform.rotation = transform.TransformRotation(this._lastRelativeAngle);
			this._prop.transform.position = this.GeoCollisionPoint(sourcePos, transform.TransformPoint(this._lastRelativePos) + this._prop.transform.TransformVector(this._propOffset)) - this._prop.transform.TransformVector(this._propOffset);
			this._networkLastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
			this._networkLastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
			return;
		}
		Vector3 v = transform.transform.position - this._prop.transform.TransformPoint(this._propOffset);
		if (v.IsLongerThan(GorillaPropHuntGameManager.instance.HandFollowDistance))
		{
			float num = v.magnitude - GorillaPropHuntGameManager.instance.HandFollowDistance;
			this._prop.transform.position = this.GeoCollisionPoint(sourcePos, this._prop.transform.position + this._prop.transform.TransformVector(this._propOffset) + v.normalized * num) - this._prop.transform.TransformVector(this._propOffset);
		}
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
		this._networkLastRelativePos = this._lastRelativePos;
		this._networkLastRelativeAngle = this._lastRelativeAngle;
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0004CB80 File Offset: 0x0004AD80
	public Vector3 GeoCollisionPoint(Vector3 sourcePos, Vector3 targetPos)
	{
		Vector3 vector = targetPos - sourcePos;
		int num = Physics.RaycastNonAlloc(sourcePos, vector.normalized, this.raycastHits, vector.magnitude, this.collisionLayers, 1);
		if (num > 0)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			Vector3 result = targetPos;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector2 = this.raycastHits[i].point - sourcePos;
				if (vector2.sqrMagnitude < sqrMagnitude)
				{
					result = this.raycastHits[i].point;
					sqrMagnitude = vector2.sqrMagnitude;
				}
			}
			return result;
		}
		return targetPos;
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0004CC1C File Offset: 0x0004AE1C
	public void SwitchHand(bool newIsLeftHand)
	{
		if (this._isLeftHand == newIsLeftHand)
		{
			return;
		}
		this._isLeftHand = newIsLeftHand;
		Transform transform = this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget;
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0004CC9D File Offset: 0x0004AE9D
	public void SetProp(bool isLeftHand, Vector3 propPos, Quaternion propRot)
	{
		this._isLeftHand = isLeftHand;
		this._lastRelativePos = propPos;
		this._lastRelativeAngle = propRot;
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0004CCB4 File Offset: 0x0004AEB4
	public long GetRelativePosRotLong()
	{
		if (this._prop.IsNull())
		{
			return BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity);
		}
		return BitPackUtils.PackHandPosRotForNetwork(this._lastRelativePos, this._lastRelativeAngle);
	}

	// Token: 0x0400116C RID: 4460
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x0400116D RID: 4461
	private const bool _k_isBetaOrEditor = false;

	// Token: 0x0400116E RID: 4462
	private const float HandFollowDistance = 0.1f;

	// Token: 0x0400116F RID: 4463
	private bool _hasProp;

	// Token: 0x04001172 RID: 4466
	private bool _isLocal;

	// Token: 0x04001173 RID: 4467
	private GameObject _prop;

	// Token: 0x04001174 RID: 4468
	private bool _isLeftHand;

	// Token: 0x04001175 RID: 4469
	private Vector3 _propOffset;

	// Token: 0x04001176 RID: 4470
	private readonly List<MeshCollider> _colliders = new List<MeshCollider>(4);

	// Token: 0x04001177 RID: 4471
	private readonly List<InteractionPoint> _interactionPoints = new List<InteractionPoint>(4);

	// Token: 0x04001178 RID: 4472
	private Vector3 _lastRelativePos;

	// Token: 0x04001179 RID: 4473
	private Quaternion _lastRelativeAngle;

	// Token: 0x0400117A RID: 4474
	private Vector3 _networkLastRelativePos;

	// Token: 0x0400117B RID: 4475
	private Quaternion _networkLastRelativeAngle;

	// Token: 0x0400117C RID: 4476
	public LayerMask collisionLayers;

	// Token: 0x0400117D RID: 4477
	private Vector3 targetPoint;

	// Token: 0x0400117E RID: 4478
	private RaycastHit[] raycastHits;

	// Token: 0x0400117F RID: 4479
	private PropHuntGrabbableProp _grabbableProp;

	// Token: 0x04001180 RID: 4480
	private PropHuntTaggableProp _taggableProp;
}
