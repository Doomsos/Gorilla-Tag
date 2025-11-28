using System;
using System.Collections;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts
{
	// Token: 0x0200104A RID: 4170
	public class FirecrackerProjectile : MonoBehaviour, ITickSystemTick, IProjectile
	{
		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x0600691D RID: 26909 RVA: 0x0022337D File Offset: 0x0022157D
		// (set) Token: 0x0600691E RID: 26910 RVA: 0x00223385 File Offset: 0x00221585
		public bool TickRunning { get; set; }

		// Token: 0x0600691F RID: 26911 RVA: 0x0022338E File Offset: 0x0022158E
		public void Tick()
		{
			if (Time.time - this.timeCreated > this.forceBackToPoolAfterSec || Time.time - this.timeExploded > this.explosionTime)
			{
				UnityEvent<FirecrackerProjectile> onDetonationComplete = this.OnDetonationComplete;
				if (onDetonationComplete == null)
				{
					return;
				}
				onDetonationComplete.Invoke(this);
			}
		}

		// Token: 0x06006920 RID: 26912 RVA: 0x002233CC File Offset: 0x002215CC
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.m_timer.Start();
			this.timeExploded = float.PositiveInfinity;
			this.timeCreated = float.PositiveInfinity;
			this.collisionEntered = false;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(true);
			}
			UnityEvent onEnableObject = this.OnEnableObject;
			if (onEnableObject == null)
			{
				return;
			}
			onEnableObject.Invoke();
		}

		// Token: 0x06006921 RID: 26913 RVA: 0x00223430 File Offset: 0x00221630
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.m_timer.Stop();
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

		// Token: 0x06006922 RID: 26914 RVA: 0x0022345B File Offset: 0x0022165B
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.m_timer.callback = new Action(this.Detonate);
		}

		// Token: 0x06006923 RID: 26915 RVA: 0x0022348C File Offset: 0x0022168C
		private void Detonate()
		{
			this.m_timer.Stop();
			this.timeExploded = Time.time;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x06006924 RID: 26916 RVA: 0x002234C4 File Offset: 0x002216C4
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

		// Token: 0x06006925 RID: 26917 RVA: 0x002235AC File Offset: 0x002217AC
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rb.linearVelocity = velocity;
		}

		// Token: 0x06006926 RID: 26918 RVA: 0x002235FC File Offset: 0x002217FC
		private void OnCollisionEnter(Collision other)
		{
			if (this.collisionEntered)
			{
				return;
			}
			Vector3 point = other.contacts[0].point;
			Vector3 normal = other.contacts[0].normal;
			UnityEvent<FirecrackerProjectile, Vector3> onCollisionEntered = this.OnCollisionEntered;
			if (onCollisionEntered != null)
			{
				onCollisionEntered.Invoke(this, normal);
			}
			if (this.sizzleDuration > 0f)
			{
				base.StartCoroutine(this.Sizzle(point, normal));
			}
			else
			{
				UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
				if (onDetonationStart != null)
				{
					onDetonationStart.Invoke(this, point);
				}
				this.Detonate(point, normal);
			}
			this.collisionEntered = true;
		}

		// Token: 0x06006927 RID: 26919 RVA: 0x00223689 File Offset: 0x00221889
		private IEnumerator Sizzle(Vector3 contactPoint, Vector3 normal)
		{
			if (this.audioSource && this.sizzleAudioClip != null)
			{
				this.audioSource.GTPlayOneShot(this.sizzleAudioClip, 1f);
			}
			yield return new WaitForSeconds(this.sizzleDuration);
			UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
			if (onDetonationStart != null)
			{
				onDetonationStart.Invoke(this, contactPoint);
			}
			this.Detonate(contactPoint, normal);
			yield break;
		}

		// Token: 0x06006928 RID: 26920 RVA: 0x002236A8 File Offset: 0x002218A8
		private void Detonate(Vector3 contactPoint, Vector3 normal)
		{
			this.timeExploded = Time.time;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.explosionEffect, contactPoint, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer soundBankPlayer;
			if (gameObject.TryGetComponent<SoundBankPlayer>(ref soundBankPlayer) && soundBankPlayer.soundBank)
			{
				soundBankPlayer.Play();
			}
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x040077B8 RID: 30648
		[SerializeField]
		private GameObject explosionEffect;

		// Token: 0x040077B9 RID: 30649
		[SerializeField]
		private float forceBackToPoolAfterSec = 20f;

		// Token: 0x040077BA RID: 30650
		[SerializeField]
		private float explosionTime = 5f;

		// Token: 0x040077BB RID: 30651
		[SerializeField]
		private GameObject disableWhenHit;

		// Token: 0x040077BC RID: 30652
		[SerializeField]
		private float sizzleDuration;

		// Token: 0x040077BD RID: 30653
		[SerializeField]
		private AudioClip sizzleAudioClip;

		// Token: 0x040077BE RID: 30654
		[Space]
		public UnityEvent OnEnableObject;

		// Token: 0x040077BF RID: 30655
		public UnityEvent<FirecrackerProjectile, Vector3> OnCollisionEntered;

		// Token: 0x040077C0 RID: 30656
		public UnityEvent<FirecrackerProjectile, Vector3> OnDetonationStart;

		// Token: 0x040077C1 RID: 30657
		public UnityEvent<FirecrackerProjectile> OnDetonationComplete;

		// Token: 0x040077C2 RID: 30658
		private Rigidbody rb;

		// Token: 0x040077C3 RID: 30659
		private float timeCreated = float.PositiveInfinity;

		// Token: 0x040077C4 RID: 30660
		private float timeExploded = float.PositiveInfinity;

		// Token: 0x040077C5 RID: 30661
		private AudioSource audioSource;

		// Token: 0x040077C6 RID: 30662
		private TickSystemTimer m_timer = new TickSystemTimer(40f);

		// Token: 0x040077C7 RID: 30663
		private bool collisionEntered;

		// Token: 0x040077C8 RID: 30664
		[SerializeField]
		private bool useTransferrableObjectState;

		// Token: 0x040077C9 RID: 30665
		[SerializeField]
		protected UnityEvent OnResetProjectileState;

		// Token: 0x040077CA RID: 30666
		[SerializeField]
		protected string boolADebugName;

		// Token: 0x040077CB RID: 30667
		[SerializeField]
		protected UnityEvent OnItemStateBoolATrue;

		// Token: 0x040077CC RID: 30668
		[SerializeField]
		protected UnityEvent OnItemStateBoolAFalse;

		// Token: 0x040077CD RID: 30669
		[SerializeField]
		protected string boolBDebugName;

		// Token: 0x040077CE RID: 30670
		[SerializeField]
		protected UnityEvent OnItemStateBoolBTrue;

		// Token: 0x040077CF RID: 30671
		[SerializeField]
		protected UnityEvent OnItemStateBoolBFalse;

		// Token: 0x040077D0 RID: 30672
		[SerializeField]
		protected string boolCDebugName;

		// Token: 0x040077D1 RID: 30673
		[SerializeField]
		protected UnityEvent OnItemStateBoolCTrue;

		// Token: 0x040077D2 RID: 30674
		[SerializeField]
		protected UnityEvent OnItemStateBoolCFalse;

		// Token: 0x040077D3 RID: 30675
		[SerializeField]
		protected string boolDDebugName;

		// Token: 0x040077D4 RID: 30676
		[SerializeField]
		protected UnityEvent OnItemStateBoolDTrue;

		// Token: 0x040077D5 RID: 30677
		[SerializeField]
		protected UnityEvent OnItemStateBoolDFalse;

		// Token: 0x040077D6 RID: 30678
		[SerializeField]
		protected UnityEvent<int> OnItemStateIntChanged;
	}
}
