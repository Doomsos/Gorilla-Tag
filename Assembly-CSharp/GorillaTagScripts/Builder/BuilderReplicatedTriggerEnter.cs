using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E5F RID: 3679
	public class BuilderReplicatedTriggerEnter : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06005BED RID: 23533 RVA: 0x001D8018 File Offset: 0x001D6218
		private void Awake()
		{
			this.colliders.Clear();
			foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
			{
				builderSmallHandTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerEntered));
				Collider component = builderSmallHandTrigger.GetComponent<Collider>();
				if (component != null)
				{
					this.colliders.Add(component);
				}
			}
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
			{
				builderSmallMonkeTrigger.onPlayerEnteredTrigger += new Action<int>(this.OnBodyTriggerEntered);
				Collider component2 = builderSmallMonkeTrigger.GetComponent<Collider>();
				if (component2 != null)
				{
					this.colliders.Add(component2);
				}
			}
		}

		// Token: 0x06005BEE RID: 23534 RVA: 0x001D80C4 File Offset: 0x001D62C4
		private void OnDestroy()
		{
			BuilderSmallHandTrigger[] array = this.handTriggers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
			}
			BuilderSmallMonkeTrigger[] array2 = this.bodyTriggers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].onPlayerEnteredTrigger -= new Action<int>(this.OnBodyTriggerEntered);
			}
		}

		// Token: 0x06005BEF RID: 23535 RVA: 0x001D8128 File Offset: 0x001D6328
		private void PlayTriggerEffects(NetPlayer target)
		{
			UnityEvent onTriggered = this.OnTriggered;
			if (onTriggered != null)
			{
				onTriggered.Invoke();
			}
			if (this.animationOnTrigger != null && this.animationOnTrigger.clip != null)
			{
				this.animationOnTrigger.Rewind();
				this.animationOnTrigger.Play();
			}
			if (this.activateSoundBank != null)
			{
				this.activateSoundBank.Play();
			}
			if (target.IsLocal)
			{
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				if (rig != null)
				{
					float num = 1.5f * rig.scaleFactor;
					if ((rig.transform.position - base.transform.position).sqrMagnitude > num * num)
					{
						return;
					}
					GTPlayer.Instance.SetMaximumSlipThisFrame();
					GTPlayer.Instance.ApplyKnockback(this.knockbackDirection.forward, this.knockbackVelocity * rig.scaleFactor, false);
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
				}
			}
		}

		// Token: 0x06005BF0 RID: 23536 RVA: 0x001D8261 File Offset: 0x001D6461
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x06005BF1 RID: 23537 RVA: 0x001D828C File Offset: 0x001D648C
		private void OnBodyTriggerEntered(int playerNumber)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerNumber);
			if (player == null)
			{
				return;
			}
			if (this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, player.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06005BF2 RID: 23538 RVA: 0x001D82EF File Offset: 0x001D64EF
		private bool CanTrigger()
		{
			return this.isPieceActive && this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.Idle && Time.time > this.lastTriggerTime + this.triggerCooldown;
		}

		// Token: 0x06005BF3 RID: 23539 RVA: 0x001D8317 File Offset: 0x001D6517
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderReplicatedTriggerEnter.FunctionalState.Idle;
		}

		// Token: 0x06005BF4 RID: 23540 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005BF5 RID: 23541 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005BF6 RID: 23542 RVA: 0x001D8320 File Offset: 0x001D6520
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x06005BF7 RID: 23543 RVA: 0x001D8378 File Offset: 0x001D6578
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x06005BF8 RID: 23544 RVA: 0x001D840C File Offset: 0x001D660C
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderReplicatedTriggerEnter.FunctionalState.TriggerEntered)
			{
				this.lastTriggerTime = Time.time;
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
				this.PlayTriggerEffects(instigator);
			}
			this.currentState = (BuilderReplicatedTriggerEnter.FunctionalState)newState;
		}

		// Token: 0x06005BF9 RID: 23545 RVA: 0x001D845C File Offset: 0x001D665C
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if (newState == 1 && this.CanTrigger())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06005BFA RID: 23546 RVA: 0x001D68CB File Offset: 0x001D4ACB
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06005BFB RID: 23547 RVA: 0x001D84B8 File Offset: 0x001D66B8
		public void FunctionalPieceUpdate()
		{
			if (this.lastTriggerTime + this.triggerCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x04006936 RID: 26934
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04006937 RID: 26935
		[Tooltip("How long in seconds to wait between trigger events")]
		[SerializeField]
		protected float triggerCooldown = 0.5f;

		// Token: 0x04006938 RID: 26936
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x04006939 RID: 26937
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x0400693A RID: 26938
		[Tooltip("Optional Animation to play when triggered")]
		[SerializeField]
		private Animation animationOnTrigger;

		// Token: 0x0400693B RID: 26939
		[Tooltip("Optional Sound to play when triggered")]
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x0400693C RID: 26940
		[Tooltip("Knockback the triggering player?")]
		[SerializeField]
		private bool knockbackOnTriggerEnter;

		// Token: 0x0400693D RID: 26941
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x0400693E RID: 26942
		[Tooltip("uses Forward of the transform provided")]
		[SerializeField]
		private Transform knockbackDirection;

		// Token: 0x0400693F RID: 26943
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x04006940 RID: 26944
		private bool isPieceActive;

		// Token: 0x04006941 RID: 26945
		private float lastTriggerTime;

		// Token: 0x04006942 RID: 26946
		private BuilderReplicatedTriggerEnter.FunctionalState currentState;

		// Token: 0x04006943 RID: 26947
		public UnityEvent OnTriggered;

		// Token: 0x02000E60 RID: 3680
		private enum FunctionalState
		{
			// Token: 0x04006945 RID: 26949
			Idle,
			// Token: 0x04006946 RID: 26950
			TriggerEntered
		}
	}
}
