using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E5E RID: 3678
	public class BuilderProjectileTarget : MonoBehaviour, IBuilderPieceFunctional
	{
		// Token: 0x06005BE2 RID: 23522 RVA: 0x001D7D9C File Offset: 0x001D5F9C
		private void Awake()
		{
			this.hitNotifier.OnProjectileHit += this.OnProjectileHit;
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
		}

		// Token: 0x06005BE3 RID: 23523 RVA: 0x001D7E08 File Offset: 0x001D6008
		private void OnDestroy()
		{
			this.hitNotifier.OnProjectileHit -= this.OnProjectileHit;
		}

		// Token: 0x06005BE4 RID: 23524 RVA: 0x001D7E21 File Offset: 0x001D6021
		private void OnDisable()
		{
			this.hitCount = 0;
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x06005BE5 RID: 23525 RVA: 0x001D7E54 File Offset: 0x001D6054
		private void OnProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 11);
			}
		}

		// Token: 0x06005BE6 RID: 23526 RVA: 0x001D7EC2 File Offset: 0x001D60C2
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (instigator == null)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 11)
			{
				return;
			}
			this.lastHitTime = (double)Time.time;
			this.hitCount = Mathf.Clamp((int)newState, 0, 10);
			this.PlayHitEffects();
		}

		// Token: 0x06005BE7 RID: 23527 RVA: 0x001D7EFC File Offset: 0x001D60FC
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (instigator == null)
			{
				return;
			}
			if (newState != 11)
			{
				return;
			}
			this.hitCount++;
			this.hitCount %= 11;
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)this.hitCount, instigator.GetPlayerRef(), timeStamp);
		}

		// Token: 0x06005BE8 RID: 23528 RVA: 0x001D7F75 File Offset: 0x001D6175
		public bool IsStateValid(byte state)
		{
			return state <= 11;
		}

		// Token: 0x06005BE9 RID: 23529 RVA: 0x001D7F80 File Offset: 0x001D6180
		private void PlayHitEffects()
		{
			if (this.hitSoundbank != null)
			{
				this.hitSoundbank.Play();
			}
			if (this.hitAnimation != null && this.hitAnimation.clip != null)
			{
				this.hitAnimation.Play();
			}
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x06005BEA RID: 23530 RVA: 0x00002789 File Offset: 0x00000989
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x06005BEB RID: 23531 RVA: 0x001D7FFC File Offset: 0x001D61FC
		public float GetInteractionDistace()
		{
			return 20f;
		}

		// Token: 0x0400692B RID: 26923
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400692C RID: 26924
		[SerializeField]
		private SlingshotProjectileHitNotifier hitNotifier;

		// Token: 0x0400692D RID: 26925
		[SerializeField]
		protected float hitCooldown = 2f;

		// Token: 0x0400692E RID: 26926
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected SoundBankPlayer hitSoundbank;

		// Token: 0x0400692F RID: 26927
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected Animation hitAnimation;

		// Token: 0x04006930 RID: 26928
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04006931 RID: 26929
		[SerializeField]
		private TMP_Text scoreText;

		// Token: 0x04006932 RID: 26930
		private double lastHitTime;

		// Token: 0x04006933 RID: 26931
		private int hitCount;

		// Token: 0x04006934 RID: 26932
		private const byte MAX_SCORE = 10;

		// Token: 0x04006935 RID: 26933
		private const byte HIT = 11;
	}
}
