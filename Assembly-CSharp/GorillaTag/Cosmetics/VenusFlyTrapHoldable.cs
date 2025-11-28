using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001122 RID: 4386
	[RequireComponent(typeof(TransferrableObject))]
	public class VenusFlyTrapHoldable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06006DC0 RID: 28096 RVA: 0x002407D8 File Offset: 0x0023E9D8
		// (set) Token: 0x06006DC1 RID: 28097 RVA: 0x002407E0 File Offset: 0x0023E9E0
		public bool TickRunning { get; set; }

		// Token: 0x06006DC2 RID: 28098 RVA: 0x002407E9 File Offset: 0x0023E9E9
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x06006DC3 RID: 28099 RVA: 0x002407F8 File Offset: 0x0023E9F8
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent += this.TriggerEntered;
			this.state = VenusFlyTrapHoldable.VenusState.Open;
			this.localRotA = this.lipA.transform.localRotation;
			this.localRotB = this.lipB.transform.localRotation;
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnTriggerEvent);
			}
		}

		// Token: 0x06006DC4 RID: 28100 RVA: 0x00240910 File Offset: 0x0023EB10
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent -= this.TriggerEntered;
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnTriggerEvent);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006DC5 RID: 28101 RVA: 0x0024097C File Offset: 0x0023EB7C
		public void Tick()
		{
			if (this.transferrableObject.InHand() && this.audioSource && !this.audioSource.isPlaying && this.flyLoopingAudio != null)
			{
				this.audioSource.clip = this.flyLoopingAudio;
				this.audioSource.GTPlay();
			}
			if (!this.transferrableObject.InHand() && this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed && Time.time - this.closedStartedTime >= this.closedDuration)
			{
				this.UpdateState(VenusFlyTrapHoldable.VenusState.Opening);
				if (this.audioSource && this.openingAudio != null)
				{
					this.audioSource.GTPlayOneShot(this.openingAudio, 1f);
				}
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closing)
			{
				this.SmoothRotation(true);
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Opening)
			{
				this.SmoothRotation(false);
			}
		}

		// Token: 0x06006DC6 RID: 28102 RVA: 0x00240A8C File Offset: 0x0023EC8C
		private void SmoothRotation(bool isClosing)
		{
			if (isClosing)
			{
				Quaternion quaternion = Quaternion.Euler(this.targetRotationB);
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, quaternion, Time.deltaTime * this.speed);
				Quaternion quaternion2 = Quaternion.Euler(this.targetRotationA);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, quaternion2, Time.deltaTime * this.speed);
				if (Quaternion.Angle(this.lipB.transform.localRotation, quaternion) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, quaternion2) < 1f)
				{
					this.lipB.transform.localRotation = quaternion;
					this.lipA.transform.localRotation = quaternion2;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Closed);
					return;
				}
			}
			else
			{
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, this.localRotB, Time.deltaTime * this.speed / 2f);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, this.localRotA, Time.deltaTime * this.speed / 2f);
				if (Quaternion.Angle(this.lipB.transform.localRotation, this.localRotB) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, this.localRotA) < 1f)
				{
					this.lipB.transform.localRotation = this.localRotB;
					this.lipA.transform.localRotation = this.localRotA;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Open);
				}
			}
		}

		// Token: 0x06006DC7 RID: 28103 RVA: 0x00240C76 File Offset: 0x0023EE76
		private void UpdateState(VenusFlyTrapHoldable.VenusState newState)
		{
			this.state = newState;
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed)
			{
				this.closedStartedTime = Time.time;
			}
		}

		// Token: 0x06006DC8 RID: 28104 RVA: 0x00240C94 File Offset: 0x0023EE94
		private void TriggerEntered(TriggerEventNotifier notifier, Collider other)
		{
			if (this.state != VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(this.layers))
			{
				return;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(Array.Empty<object>());
			}
			this.OnTriggerLocal();
			GorillaTriggerColliderHandIndicator componentInChildren = other.GetComponentInChildren<GorillaTriggerColliderHandIndicator>();
			if (componentInChildren == null)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInChildren.isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006DC9 RID: 28105 RVA: 0x00240D2F File Offset: 0x0023EF2F
		private void OnTriggerEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnTriggerEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.OnTriggerLocal();
		}

		// Token: 0x06006DCA RID: 28106 RVA: 0x00240D5B File Offset: 0x0023EF5B
		private void OnTriggerLocal()
		{
			this.UpdateState(VenusFlyTrapHoldable.VenusState.Closing);
			if (this.audioSource && this.closingAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.closingAudio, 1f);
			}
		}

		// Token: 0x04007F44 RID: 32580
		[SerializeField]
		private GameObject lipA;

		// Token: 0x04007F45 RID: 32581
		[SerializeField]
		private GameObject lipB;

		// Token: 0x04007F46 RID: 32582
		[SerializeField]
		private Vector3 targetRotationA;

		// Token: 0x04007F47 RID: 32583
		[SerializeField]
		private Vector3 targetRotationB;

		// Token: 0x04007F48 RID: 32584
		[SerializeField]
		private float closedDuration = 3f;

		// Token: 0x04007F49 RID: 32585
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04007F4A RID: 32586
		[SerializeField]
		private UnityLayer layers;

		// Token: 0x04007F4B RID: 32587
		[SerializeField]
		private TriggerEventNotifier triggerEventNotifier;

		// Token: 0x04007F4C RID: 32588
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04007F4D RID: 32589
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04007F4E RID: 32590
		[SerializeField]
		private GameObject bug;

		// Token: 0x04007F4F RID: 32591
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007F50 RID: 32592
		[SerializeField]
		private AudioClip closingAudio;

		// Token: 0x04007F51 RID: 32593
		[SerializeField]
		private AudioClip openingAudio;

		// Token: 0x04007F52 RID: 32594
		[SerializeField]
		private AudioClip flyLoopingAudio;

		// Token: 0x04007F53 RID: 32595
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04007F54 RID: 32596
		private float closedStartedTime;

		// Token: 0x04007F55 RID: 32597
		private VenusFlyTrapHoldable.VenusState state;

		// Token: 0x04007F56 RID: 32598
		private Quaternion localRotA;

		// Token: 0x04007F57 RID: 32599
		private Quaternion localRotB;

		// Token: 0x04007F58 RID: 32600
		private RubberDuckEvents _events;

		// Token: 0x04007F59 RID: 32601
		private TransferrableObject transferrableObject;

		// Token: 0x02001123 RID: 4387
		private enum VenusState
		{
			// Token: 0x04007F5C RID: 32604
			Closed,
			// Token: 0x04007F5D RID: 32605
			Open,
			// Token: 0x04007F5E RID: 32606
			Closing,
			// Token: 0x04007F5F RID: 32607
			Opening
		}
	}
}
