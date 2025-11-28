using System;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E1 RID: 1249
public class PaperPlaneProjectile : MonoBehaviour
{
	// Token: 0x14000044 RID: 68
	// (add) Token: 0x0600201B RID: 8219 RVA: 0x000AA5C0 File Offset: 0x000A87C0
	// (remove) Token: 0x0600201C RID: 8220 RVA: 0x000AA5F8 File Offset: 0x000A87F8
	public event PaperPlaneProjectile.PaperPlaneHit OnHit;

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x0600201D RID: 8221 RVA: 0x000AA62D File Offset: 0x000A882D
	public Transform transform
	{
		get
		{
			return this._tCached;
		}
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x0600201E RID: 8222 RVA: 0x000AA635 File Offset: 0x000A8835
	public VRRig MyRig
	{
		get
		{
			return this.myRig;
		}
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000AA63D File Offset: 0x000A883D
	private void Awake()
	{
		this._tCached = base.transform;
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000AA657 File Offset: 0x000A8857
	private void Start()
	{
		this.ResetProjectile();
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x000AA65F File Offset: 0x000A885F
	public void ResetProjectile()
	{
		this._timeElapsed = 0f;
		this.flyingObject.SetActive(true);
		this.crashingObject.SetActive(false);
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x000AA684 File Offset: 0x000A8884
	internal void SetTransferrableState(TransferrableObject.SyncOptions syncType, int state)
	{
		if (!this.useTransferrableObjectState)
		{
			return;
		}
		if (syncType != TransferrableObject.SyncOptions.Bool)
		{
			if (syncType != TransferrableObject.SyncOptions.Int)
			{
				return;
			}
			UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
			if (onItemStateIntChanged == null)
			{
				return;
			}
			onItemStateIntChanged.Invoke(state);
			return;
		}
		else
		{
			bool flag = (state & 1) != 0;
			bool flag2 = (state & 2) != 0;
			bool flag3 = (state & 4) != 0;
			bool flag4 = (state & 8) != 0;
			if (flag)
			{
				UnityEvent onItemStateBoolATrue = this.OnItemStateBoolATrue;
				if (onItemStateBoolATrue != null)
				{
					onItemStateBoolATrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolAFalse = this.OnItemStateBoolAFalse;
				if (onItemStateBoolAFalse != null)
				{
					onItemStateBoolAFalse.Invoke();
				}
			}
			if (flag2)
			{
				UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
				if (onItemStateBoolBTrue != null)
				{
					onItemStateBoolBTrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
				if (onItemStateBoolBFalse != null)
				{
					onItemStateBoolBFalse.Invoke();
				}
			}
			if (flag3)
			{
				UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
				if (onItemStateBoolCTrue != null)
				{
					onItemStateBoolCTrue.Invoke();
				}
			}
			else
			{
				UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
				if (onItemStateBoolCFalse != null)
				{
					onItemStateBoolCFalse.Invoke();
				}
			}
			if (flag4)
			{
				UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
				if (onItemStateBoolDTrue == null)
				{
					return;
				}
				onItemStateBoolDTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
				if (onItemStateBoolDFalse == null)
				{
					return;
				}
				onItemStateBoolDFalse.Invoke();
				return;
			}
		}
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x000AA76C File Offset: 0x000A896C
	public void Launch(Vector3 startPos, Quaternion startRot, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		if (this.enableRotation)
		{
			this.transform.rotation = startRot;
		}
		else
		{
			this.transform.LookAt(this.transform.position + vel.normalized);
		}
		this._direction = vel.normalized;
		this._speed = Mathf.Clamp(this.speedCurve.Evaluate(vel.magnitude), this.minSpeed, this.maxSpeed);
		this._stopped = false;
		this.scaleFactor = 0.7f * (this.transform.lossyScale.x - 1f + 1.4285715f);
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000AA834 File Offset: 0x000A8A34
	private void Update()
	{
		if (this._stopped)
		{
			if (!this.crashingObject.gameObject.activeSelf)
			{
				if (ObjectPools.instance)
				{
					ObjectPools.instance.Destroy(base.gameObject);
					return;
				}
				base.gameObject.SetActive(false);
			}
			return;
		}
		this._timeElapsed += Time.deltaTime;
		this.nextPos = this.transform.position + this._direction * this._speed * Time.deltaTime * this.scaleFactor;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos), this.layerMask.value) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		if (this._timeElapsed < this.maxFlightTime)
		{
			SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
			if (this.results[0].collider.TryGetComponent<SlingshotProjectileHitNotifier>(ref slingshotProjectileHitNotifier))
			{
				slingshotProjectileHitNotifier.InvokeHit(this, this.results[0].collider);
			}
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(this.nextPos);
			}
		}
		this._stopped = true;
		this._timeElapsed = 0f;
		PaperPlaneProjectile.PaperPlaneHit onHit = this.OnHit;
		if (onHit != null)
		{
			onHit(this.nextPos);
		}
		this.OnHit = null;
		this.flyingObject.SetActive(false);
		this.crashingObject.SetActive(true);
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x000AAA26 File Offset: 0x000A8C26
	internal void SetVRRig(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000AAA2F File Offset: 0x000A8C2F
	private void OnDisable()
	{
		if (this.useTransferrableObjectState)
		{
			UnityEvent onResetProjectileState = this.OnResetProjectileState;
			if (onResetProjectileState == null)
			{
				return;
			}
			onResetProjectileState.Invoke();
		}
	}

	// Token: 0x04002A77 RID: 10871
	private const float speedScaleRatio = 0.7f;

	// Token: 0x04002A79 RID: 10873
	[Space]
	[NonSerialized]
	private float _timeElapsed;

	// Token: 0x04002A7A RID: 10874
	[NonSerialized]
	private float _speed;

	// Token: 0x04002A7B RID: 10875
	[NonSerialized]
	private Vector3 _direction;

	// Token: 0x04002A7C RID: 10876
	[NonSerialized]
	private bool _stopped;

	// Token: 0x04002A7D RID: 10877
	private Transform _tCached;

	// Token: 0x04002A7E RID: 10878
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x04002A7F RID: 10879
	private Vector3 nextPos;

	// Token: 0x04002A80 RID: 10880
	private RaycastHit[] results = new RaycastHit[1];

	// Token: 0x04002A81 RID: 10881
	[Tooltip("Maximum lifetime in seconds for the projectile")]
	[SerializeField]
	private float maxFlightTime = 7.5f;

	// Token: 0x04002A82 RID: 10882
	[Tooltip("Collisions are ignored for minFlightTime seconds after launch")]
	[SerializeField]
	private float minFlightTime = 0.5f;

	// Token: 0x04002A83 RID: 10883
	[Tooltip("Hand speed to projectile launch Speed")]
	[SerializeField]
	private AnimationCurve speedCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f),
		new Keyframe(6.324555f, 20f, 6.324555f, 6.324555f)
	});

	// Token: 0x04002A84 RID: 10884
	[Tooltip("maximum speed of launched projectile (clamped after applying speed curve)")]
	[SerializeField]
	private float maxSpeed = 10f;

	// Token: 0x04002A85 RID: 10885
	[Tooltip("minimum speed of launched projectile (clamped after applying speed curve)")]
	[SerializeField]
	private float minSpeed = 1f;

	// Token: 0x04002A86 RID: 10886
	[SerializeField]
	private bool enableRotation;

	// Token: 0x04002A87 RID: 10887
	[Tooltip("Objects enabled when launched and disabled on Hit")]
	[SerializeField]
	private GameObject flyingObject;

	// Token: 0x04002A88 RID: 10888
	[Tooltip("Objects disabled when launched and enabled on Hit")]
	[SerializeField]
	private GameObject crashingObject;

	// Token: 0x04002A89 RID: 10889
	[Tooltip("Layers the projectile collides with")]
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04002A8A RID: 10890
	[SerializeField]
	private bool useTransferrableObjectState;

	// Token: 0x04002A8B RID: 10891
	[SerializeField]
	protected UnityEvent OnResetProjectileState;

	// Token: 0x04002A8C RID: 10892
	[SerializeField]
	protected string boolADebugName;

	// Token: 0x04002A8D RID: 10893
	[SerializeField]
	protected UnityEvent OnItemStateBoolATrue;

	// Token: 0x04002A8E RID: 10894
	[SerializeField]
	protected UnityEvent OnItemStateBoolAFalse;

	// Token: 0x04002A8F RID: 10895
	[SerializeField]
	protected string boolBDebugName;

	// Token: 0x04002A90 RID: 10896
	[SerializeField]
	protected UnityEvent OnItemStateBoolBTrue;

	// Token: 0x04002A91 RID: 10897
	[SerializeField]
	protected UnityEvent OnItemStateBoolBFalse;

	// Token: 0x04002A92 RID: 10898
	[SerializeField]
	protected string boolCDebugName;

	// Token: 0x04002A93 RID: 10899
	[SerializeField]
	protected UnityEvent OnItemStateBoolCTrue;

	// Token: 0x04002A94 RID: 10900
	[SerializeField]
	protected UnityEvent OnItemStateBoolCFalse;

	// Token: 0x04002A95 RID: 10901
	[SerializeField]
	protected string boolDDebugName;

	// Token: 0x04002A96 RID: 10902
	[SerializeField]
	protected UnityEvent OnItemStateBoolDTrue;

	// Token: 0x04002A97 RID: 10903
	[SerializeField]
	protected UnityEvent OnItemStateBoolDFalse;

	// Token: 0x04002A98 RID: 10904
	[SerializeField]
	protected UnityEvent<int> OnItemStateIntChanged;

	// Token: 0x04002A99 RID: 10905
	private VRRig myRig;

	// Token: 0x04002A9A RID: 10906
	private float scaleFactor;

	// Token: 0x020004E2 RID: 1250
	// (Invoke) Token: 0x06002029 RID: 8233
	public delegate void PaperPlaneHit(Vector3 endPoint);
}
