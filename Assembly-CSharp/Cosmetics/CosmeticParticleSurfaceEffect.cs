using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x02000FC2 RID: 4034
	[RequireComponent(typeof(TransferrableObject))]
	public class CosmeticParticleSurfaceEffect : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x0600663F RID: 26175 RVA: 0x00214DCF File Offset: 0x00212FCF
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			if (this.surfaceEffectPrefab != null)
			{
				this.surfaceEffectHash = PoolUtils.GameObjHashCode(this.surfaceEffectPrefab);
			}
		}

		// Token: 0x06006640 RID: 26176 RVA: 0x00214DFC File Offset: 0x00212FFC
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				this.owner = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (this.owner != null)
				{
					this._events.Init(this.owner);
					this.isLocal = this.owner.IsLocal;
				}
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnSpawnReplicated);
				this._events.Deactivate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnTriggerEffectReplicated);
			}
			if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
			{
				return;
			}
			if (this.surfaceEffectHash != 0)
			{
				this._pool = ObjectPools.instance.GetPoolByHash(this.surfaceEffectHash);
				if (this._pool != null)
				{
					this.foundPool = true;
				}
				else
				{
					GTDev.LogError<string>("CosmeticParticleSurfaceEffect " + base.gameObject.name + " no Object pool found for surface effect prefab. Has it been added to Global Object Pools?", null);
				}
			}
			this.spawnCallLimiter.Reset();
			this.destroyCallLimiter.Reset();
			this.lastHitTime = float.MinValue;
		}

		// Token: 0x06006641 RID: 26177 RVA: 0x00214FB4 File Offset: 0x002131B4
		private void OnDisable()
		{
			this.StopParticles();
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnSpawnReplicated);
				this._events.Deactivate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnTriggerEffectReplicated);
				this._events.Dispose();
				this._events = null;
			}
			this.surfaceEffectNum.Clear();
			foreach (SeedPacketTriggerHandler seedPacketTriggerHandler in this.surfaceEffects)
			{
				if (!(seedPacketTriggerHandler == null))
				{
					seedPacketTriggerHandler.onTriggerEntered.RemoveListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
				}
			}
			this.surfaceEffects.Clear();
		}

		// Token: 0x06006642 RID: 26178 RVA: 0x002150A0 File Offset: 0x002132A0
		private void OnDestroy()
		{
			this.surfaceEffectNum.Clear();
			this.surfaceEffects.Clear();
		}

		// Token: 0x06006643 RID: 26179 RVA: 0x002150B8 File Offset: 0x002132B8
		public void StartParticles()
		{
			if (!this.isSpawning)
			{
				this.isSpawning = true;
				this.particleStartedTime = Time.time;
				if (!this.particles.isPlaying)
				{
					this.particles.Play();
				}
			}
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006644 RID: 26180 RVA: 0x00215108 File Offset: 0x00213308
		public void StopParticles()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			this.isSpawning = false;
			this.particleStartedTime = float.MinValue;
			this.lastHitTime = float.MinValue;
			if (this.particles.isPlaying)
			{
				this.particles.Stop();
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x06006645 RID: 26181 RVA: 0x00215158 File Offset: 0x00213358
		// (set) Token: 0x06006646 RID: 26182 RVA: 0x00215160 File Offset: 0x00213360
		public bool TickRunning { get; set; }

		// Token: 0x06006647 RID: 26183 RVA: 0x0021516C File Offset: 0x0021336C
		public void Tick()
		{
			if (this.transferrableObject == null || !this.transferrableObject.InHand())
			{
				this.StopParticles();
				return;
			}
			if (this.isSpawning && this.stopAfterSeconds > 0f && Time.time >= this.particleStartedTime + this.stopAfterSeconds)
			{
				this.StopParticles();
				return;
			}
			if (!this.isLocal)
			{
				return;
			}
			if (this.isSpawning && Time.time > this.placeEffectCooldown + this.lastHitTime)
			{
				int num = Physics.RaycastNonAlloc(this.rayCastOrigin.position, this.useWorldDirection ? this.worldDirection : this.rayCastOrigin.forward, this.hits, this.rayCastDistance, this.rayCastLayerMask, 1);
				if (num > 0)
				{
					int num2 = 0;
					float distance = this.hits[num2].distance;
					for (int i = 1; i < num; i++)
					{
						if (this.hits[i].distance < distance)
						{
							num2 = i;
							distance = this.hits[i].distance;
						}
					}
					this.hitPoint = this.hits[num2];
					this.lastHitTime = Time.time;
					base.Invoke("SpawnEffect", distance * this.placeEffectDelayMultiplier);
				}
			}
		}

		// Token: 0x06006648 RID: 26184 RVA: 0x002152B8 File Offset: 0x002134B8
		private void SpawnEffect()
		{
			if (!this.isLocal)
			{
				return;
			}
			long num = BitPackUtils.PackWorldPosForNetwork(this.hitPoint.point);
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.hitPoint.normal);
			int num3 = this.currentEffect;
			this.currentEffect++;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					num,
					num2,
					num3
				});
			}
			this.SpawnLocal(this.hitPoint.point, this.hitPoint.normal, num3);
		}

		// Token: 0x06006649 RID: 26185 RVA: 0x0021537C File Offset: 0x0021357C
		private void OnSpawnReplicated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (!this || sender != target || this.owner == null || info.senderID != this.owner.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnSpawnReplicated");
			if (!this.spawnCallLimiter.CheckCallTime(Time.time) || args.Length != 3 || !(args[0] is long) || !(args[1] is long) || !(args[2] is int))
			{
				return;
			}
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
			Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork((long)args[1]);
			float num = 10000f;
			if (vector.IsValid(num))
			{
				float num2 = 10000f;
				if (vector2.IsValid(num2))
				{
					if (Vector3.Distance(this.rayCastOrigin.position, vector) > this.rayCastDistance + 2f)
					{
						return;
					}
					vector2.Normalize();
					if (vector2 == Vector3.zero)
					{
						vector2 = Vector3.up;
					}
					int identifier = (int)args[2];
					this.SpawnLocal(vector, vector2, identifier);
					return;
				}
			}
		}

		// Token: 0x0600664A RID: 26186 RVA: 0x00215484 File Offset: 0x00213684
		private void SpawnLocal(Vector3 position, Vector3 up, int identifier)
		{
			if (this.surfaceEffectHash != 0 && !this.foundPool)
			{
				this._pool = ObjectPools.instance.GetPoolByHash(this.surfaceEffectHash);
				if (this._pool == null)
				{
					return;
				}
				this.foundPool = true;
			}
			if (this.foundPool && this._pool.GetInactiveCount() > 0)
			{
				this.ClearOldObjects();
				GameObject gameObject = this._pool.Instantiate(true);
				gameObject.transform.position = position;
				gameObject.transform.up = up;
				SeedPacketTriggerHandler seedPacketTriggerHandler;
				if (gameObject.TryGetComponent<SeedPacketTriggerHandler>(ref seedPacketTriggerHandler))
				{
					int num = this.surfaceEffects.IndexOf(seedPacketTriggerHandler);
					if (num >= 0)
					{
						this.surfaceEffectNum[num] = identifier;
					}
					else
					{
						this.surfaceEffectNum.Add(identifier);
						this.surfaceEffects.Add(seedPacketTriggerHandler);
					}
					seedPacketTriggerHandler.onTriggerEntered.AddListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
				}
			}
		}

		// Token: 0x0600664B RID: 26187 RVA: 0x00215568 File Offset: 0x00213768
		private void ClearOldObjects()
		{
			for (int i = this.surfaceEffects.Count - 1; i >= 0; i--)
			{
				if (this.surfaceEffects[i] == null)
				{
					this.surfaceEffects.RemoveAt(i);
					this.surfaceEffectNum.RemoveAt(i);
				}
				else if (!this.surfaceEffects[i].gameObject.activeSelf)
				{
					this.surfaceEffects[i].onTriggerEntered.RemoveListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
					this.surfaceEffects.RemoveAt(i);
					this.surfaceEffectNum.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600664C RID: 26188 RVA: 0x00215614 File Offset: 0x00213814
		private void OnTriggerEffectLocal(SeedPacketTriggerHandler seedPacketTriggerHandlerTriggerHandlerEvent)
		{
			int num = this.surfaceEffects.IndexOf(seedPacketTriggerHandlerTriggerHandlerEvent);
			if (num >= 0 && num < this.surfaceEffectNum.Count)
			{
				int num2 = this.surfaceEffectNum[num];
				if (PhotonNetwork.InRoom && this._events != null && this._events.Deactivate != null)
				{
					this._events.Deactivate.RaiseOthers(new object[]
					{
						num2
					});
				}
				this.surfaceEffects.RemoveAt(num);
				this.surfaceEffectNum.RemoveAt(num);
			}
		}

		// Token: 0x0600664D RID: 26189 RVA: 0x002156AC File Offset: 0x002138AC
		private void OnTriggerEffectReplicated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnTriggerEffectReplicated");
			if (!this.destroyCallLimiter.CheckCallTime(Time.time) || args.Length != 1 || !(args[0] is int))
			{
				return;
			}
			this.ClearOldObjects();
			int num = (int)args[0];
			int num2 = this.surfaceEffectNum.IndexOf(num);
			if (num2 >= 0 && num2 < this.surfaceEffects.Count)
			{
				SeedPacketTriggerHandler seedPacketTriggerHandler = this.surfaceEffects[num2];
				if (seedPacketTriggerHandler != null)
				{
					seedPacketTriggerHandler.ToggleEffects();
					seedPacketTriggerHandler.onTriggerEntered.RemoveListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
				}
				this.surfaceEffects.RemoveAt(num2);
				this.surfaceEffectNum.RemoveAt(num2);
			}
		}

		// Token: 0x040074CD RID: 29901
		[Tooltip("autoStop particle system this many seconds after starting")]
		[SerializeField]
		private float stopAfterSeconds = 3f;

		// Token: 0x040074CE RID: 29902
		[Tooltip("particle system to play on start particles")]
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x040074CF RID: 29903
		[Tooltip("Distance in meters to check for a surface hit")]
		[SerializeField]
		private float rayCastDistance = 20f;

		// Token: 0x040074D0 RID: 29904
		[Tooltip("The position for the start of the rayCast.\nThe forward (z+) axis of this transform will be used as the rayCast direction\nThis should visually line up with the spawned particles")]
		[SerializeField]
		private Transform rayCastOrigin;

		// Token: 0x040074D1 RID: 29905
		[Tooltip("Use a world direction vector for the raycast instead of the rayCastOrigin forward?")]
		[SerializeField]
		private bool useWorldDirection;

		// Token: 0x040074D2 RID: 29906
		[SerializeField]
		private Vector3 worldDirection = Vector3.down;

		// Token: 0x040074D3 RID: 29907
		[Tooltip("Layers to check for surface collision")]
		[SerializeField]
		private LayerMask rayCastLayerMask = 513;

		// Token: 0x040074D4 RID: 29908
		[Tooltip("Prefab from the global object pool to spawn on surface hit\nIf it should be destroyed on touch, add a SeedPacketTriggerHandler to the prefab")]
		[SerializeField]
		private GameObject surfaceEffectPrefab;

		// Token: 0x040074D5 RID: 29909
		[Tooltip("Seconds per meter to wait before spawning a surface effect on hit.\n A good value would be somewhat close to 1/particle velocity ")]
		[SerializeField]
		private float placeEffectDelayMultiplier = 3f;

		// Token: 0x040074D6 RID: 29910
		[Tooltip("Time to wait between spawning surface effects")]
		[SerializeField]
		private float placeEffectCooldown = 2f;

		// Token: 0x040074D7 RID: 29911
		private float particleStartedTime;

		// Token: 0x040074D8 RID: 29912
		private bool isSpawning;

		// Token: 0x040074D9 RID: 29913
		private float lastHitTime = float.MinValue;

		// Token: 0x040074DA RID: 29914
		private RaycastHit hitPoint;

		// Token: 0x040074DB RID: 29915
		private RaycastHit[] hits = new RaycastHit[5];

		// Token: 0x040074DC RID: 29916
		private TransferrableObject transferrableObject;

		// Token: 0x040074DD RID: 29917
		private bool isLocal;

		// Token: 0x040074DE RID: 29918
		private NetPlayer owner;

		// Token: 0x040074DF RID: 29919
		private int surfaceEffectHash;

		// Token: 0x040074E0 RID: 29920
		private RubberDuckEvents _events;

		// Token: 0x040074E1 RID: 29921
		private CallLimiter spawnCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x040074E2 RID: 29922
		private CallLimiter destroyCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x040074E3 RID: 29923
		private SinglePool _pool;

		// Token: 0x040074E4 RID: 29924
		private bool foundPool;

		// Token: 0x040074E5 RID: 29925
		private int currentEffect;

		// Token: 0x040074E6 RID: 29926
		private List<int> surfaceEffectNum = new List<int>();

		// Token: 0x040074E7 RID: 29927
		private List<SeedPacketTriggerHandler> surfaceEffects = new List<SeedPacketTriggerHandler>(10);
	}
}
