using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000559 RID: 1369
public class Breakable : MonoBehaviour
{
	// Token: 0x06002297 RID: 8855 RVA: 0x000B4D78 File Offset: 0x000B2F78
	private void Awake()
	{
		this._breakSignal.OnSignal += this.BreakRPC;
		if (this._rigidbody.IsNotNull())
		{
			this.m_useGravity = this._rigidbody.useGravity;
		}
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000B4DB0 File Offset: 0x000B2FB0
	private void BreakRPC(int owner, PhotonSignalInfo info)
	{
		VRRig vrrig = base.GetComponent<OwnerRig>();
		if (vrrig == null)
		{
			return;
		}
		if (vrrig.OwningNetPlayer.ActorNumber != owner)
		{
			return;
		}
		if (!this.m_spamChecker.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.OnBreak(true, false);
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x000B4E00 File Offset: 0x000B3000
	private void Setup()
	{
		if (this._collider == null)
		{
			SphereCollider collider;
			this.GetOrAddComponent(out collider);
			this._collider = collider;
		}
		this._collider.enabled = true;
		if (this._rigidbody == null)
		{
			this.GetOrAddComponent(out this._rigidbody);
		}
		this._rigidbody.isKinematic = false;
		this._rigidbody.useGravity = false;
		this._rigidbody.constraints = 126;
		this.UpdatePhysMasks();
		if (this.rendererRoot == null)
		{
			this._renderers = base.GetComponentsInChildren<Renderer>();
			return;
		}
		this._renderers = this.rendererRoot.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000B4EA7 File Offset: 0x000B30A7
	private void OnCollisionEnter(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000B4EA7 File Offset: 0x000B30A7
	private void OnCollisionStay(Collision col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000B4EA7 File Offset: 0x000B30A7
	private void OnTriggerEnter(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000B4EA7 File Offset: 0x000B30A7
	private void OnTriggerStay(Collider col)
	{
		this.OnBreak(true, true);
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000B4EB1 File Offset: 0x000B30B1
	private void OnEnable()
	{
		this._breakSignal.Enable();
		this._broken = false;
		this.OnSpawn(true);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000B4ECC File Offset: 0x000B30CC
	private void OnDisable()
	{
		this._breakSignal.Disable();
		this._broken = false;
		this.OnReset(false);
		this.ShowRenderers(false);
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000B4EA7 File Offset: 0x000B30A7
	public void Break()
	{
		this.OnBreak(true, true);
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000B4EEE File Offset: 0x000B30EE
	public void Reset()
	{
		this.OnReset(true);
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000B4EF8 File Offset: 0x000B30F8
	protected virtual void ShowRenderers(bool visible)
	{
		if (this._renderers.IsNullOrEmpty<Renderer>())
		{
			return;
		}
		for (int i = 0; i < this._renderers.Length; i++)
		{
			Renderer renderer = this._renderers[i];
			if (renderer)
			{
				renderer.forceRenderingOff = !visible;
			}
		}
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000B4F44 File Offset: 0x000B3144
	protected virtual void OnReset(bool callback = true)
	{
		if (this._breakEffect && this._breakEffect.isPlaying)
		{
			this._breakEffect.Stop();
		}
		this.ShowRenderers(true);
		this._broken = false;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onReset;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000B4F98 File Offset: 0x000B3198
	protected virtual void OnSpawn(bool callback = true)
	{
		this.startTime = Time.time;
		this.endTime = this.startTime + this.canBreakDelay;
		this.ShowRenderers(true);
		if (this._rigidbody.IsNotNull())
		{
			this._rigidbody.detectCollisions = true;
			this._rigidbody.useGravity = this.m_useGravity;
		}
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onSpawn;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000B5008 File Offset: 0x000B3208
	protected virtual void OnBreak(bool callback = true, bool signal = true)
	{
		if (this._broken)
		{
			return;
		}
		if (Time.time < this.endTime)
		{
			return;
		}
		if (this._breakEffect)
		{
			if (this._breakEffect.isPlaying)
			{
				this._breakEffect.Stop();
			}
			this._breakEffect.Play();
		}
		if (signal && PhotonNetwork.InRoom)
		{
			VRRig vrrig = base.GetComponent<OwnerRig>();
			if (vrrig != null)
			{
				this._breakSignal.Raise(vrrig.OwningNetPlayer.ActorNumber);
			}
		}
		this.ShowRenderers(false);
		if (this._rigidbody.IsNotNull())
		{
			this._rigidbody.detectCollisions = false;
			this._rigidbody.useGravity = false;
		}
		this._broken = true;
		if (callback)
		{
			UnityEvent<Breakable> unityEvent = this.onBreak;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000B50D8 File Offset: 0x000B32D8
	private void UpdatePhysMasks()
	{
		int physicsMask = (int)this._physicsMask;
		if (this._collider)
		{
			this._collider.includeLayers = physicsMask;
			this._collider.excludeLayers = ~physicsMask;
		}
		if (this._rigidbody)
		{
			this._rigidbody.includeLayers = physicsMask;
			this._rigidbody.excludeLayers = ~physicsMask;
		}
	}

	// Token: 0x04002D2B RID: 11563
	[SerializeField]
	private Collider _collider;

	// Token: 0x04002D2C RID: 11564
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x04002D2D RID: 11565
	[SerializeField]
	private GameObject rendererRoot;

	// Token: 0x04002D2E RID: 11566
	[SerializeField]
	private Renderer[] _renderers = new Renderer[0];

	// Token: 0x04002D2F RID: 11567
	[Space]
	[SerializeField]
	private ParticleSystem _breakEffect;

	// Token: 0x04002D30 RID: 11568
	[SerializeField]
	private UnityLayerMask _physicsMask = UnityLayerMask.GorillaHand;

	// Token: 0x04002D31 RID: 11569
	public UnityEvent<Breakable> onSpawn;

	// Token: 0x04002D32 RID: 11570
	public UnityEvent<Breakable> onBreak;

	// Token: 0x04002D33 RID: 11571
	public UnityEvent<Breakable> onReset;

	// Token: 0x04002D34 RID: 11572
	public float canBreakDelay = 1f;

	// Token: 0x04002D35 RID: 11573
	[SerializeField]
	private PhotonSignal<int> _breakSignal = "_breakSignal";

	// Token: 0x04002D36 RID: 11574
	[SerializeField]
	private CallLimiter m_spamChecker = new CallLimiter(2, 1f, 0.5f);

	// Token: 0x04002D37 RID: 11575
	[Space]
	[NonSerialized]
	private bool _broken;

	// Token: 0x04002D38 RID: 11576
	private bool m_useGravity = true;

	// Token: 0x04002D39 RID: 11577
	private float startTime;

	// Token: 0x04002D3A RID: 11578
	private float endTime;
}
