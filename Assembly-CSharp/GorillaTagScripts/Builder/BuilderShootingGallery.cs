using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E63 RID: 3683
	public class BuilderShootingGallery : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06005C0B RID: 23563 RVA: 0x001D8D04 File Offset: 0x001D6F04
		private void Awake()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
			this.wheelHitNotifier.OnProjectileHit += this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit += this.OnCowboyHit;
		}

		// Token: 0x06005C0C RID: 23564 RVA: 0x001D8D88 File Offset: 0x001D6F88
		private void OnDestroy()
		{
			this.wheelHitNotifier.OnProjectileHit -= this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit -= this.OnCowboyHit;
		}

		// Token: 0x06005C0D RID: 23565 RVA: 0x001D8DB8 File Offset: 0x001D6FB8
		private void OnWheelHit(SlingshotProjectile projectile, Collision collision)
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
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x06005C0E RID: 23566 RVA: 0x001D8E28 File Offset: 0x001D7028
		private void OnCowboyHit(SlingshotProjectile projectile, Collision collision)
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
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 2);
			}
		}

		// Token: 0x06005C0F RID: 23567 RVA: 0x001D8E98 File Offset: 0x001D7098
		private void CowboyHitEffects()
		{
			if (this.cowboyHitSound != null)
			{
				this.cowboyHitSound.Play();
			}
			if (this.cowboyHitAnimation != null && this.cowboyHitAnimation.clip != null)
			{
				this.cowboyHitAnimation.Play();
			}
		}

		// Token: 0x06005C10 RID: 23568 RVA: 0x001D8EEC File Offset: 0x001D70EC
		private void WheelHitEffects()
		{
			if (this.wheelHitSound != null)
			{
				this.wheelHitSound.Play();
			}
			if (this.wheelHitAnimation != null && this.wheelHitAnimation.clip != null)
			{
				this.wheelHitAnimation.Play();
			}
		}

		// Token: 0x06005C11 RID: 23569 RVA: 0x001D8F40 File Offset: 0x001D7140
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderShootingGallery.FunctionalState.Idle;
			this.cowboyInitLocalPos = this.cowboyTransform.transform.localPosition;
			this.cowboyInitLocalRotation = this.cowboyTransform.transform.localRotation;
			this.wheelInitLocalRot = this.wheelTransform.transform.localRotation;
			this.distance = Vector3.Distance(this.cowboyStart.position, this.cowboyEnd.position);
			this.cowboyCycleDuration = this.distance / (this.cowboyVelocity * this.myPiece.GetScale());
			this.wheelCycleDuration = 1f / this.wheelVelocity;
		}

		// Token: 0x06005C12 RID: 23570 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005C13 RID: 23571 RVA: 0x001D8FE8 File Offset: 0x001D71E8
		public void OnPiecePlacementDeserialized()
		{
			if (!this.activated && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x06005C14 RID: 23572 RVA: 0x001D9018 File Offset: 0x001D7218
		public void OnPieceActivate()
		{
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
			if (!this.activated)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x06005C15 RID: 23573 RVA: 0x001D9078 File Offset: 0x001D7278
		public void OnPieceDeactivate()
		{
			if (this.currentState != BuilderShootingGallery.FunctionalState.Idle)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			if (this.activated)
			{
				this.myPiece.GetTable().UnregisterFunctionalPieceFixedUpdate(this);
				this.activated = false;
			}
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
		}

		// Token: 0x06005C16 RID: 23574 RVA: 0x001D9114 File Offset: 0x001D7314
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
			if (newState == 1 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.WheelHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			else if (newState == 2 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.CowboyHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderShootingGallery.FunctionalState)newState;
		}

		// Token: 0x06005C17 RID: 23575 RVA: 0x001D9198 File Offset: 0x001D7398
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
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06005C18 RID: 23576 RVA: 0x001D91FD File Offset: 0x001D73FD
		public bool IsStateValid(byte state)
		{
			return state <= 2;
		}

		// Token: 0x06005C19 RID: 23577 RVA: 0x001D9208 File Offset: 0x001D7408
		public void FunctionalPieceUpdate()
		{
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06005C1A RID: 23578 RVA: 0x001D925C File Offset: 0x001D745C
		public void FunctionalPieceFixedUpdate()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			this.currT = this.CowboyCycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			float num = this.currForward ? this.currT : (1f - this.currT);
			float num2 = this.WheelCycleCompletionPercent();
			float num3 = this.cowboyCurve.Evaluate(num);
			this.cowboyTransform.localPosition = Vector3.Lerp(this.cowboyStart.localPosition, this.cowboyEnd.localPosition, num3);
			Quaternion localRotation = Quaternion.AngleAxis(num2 * 360f, Vector3.right);
			this.wheelTransform.localRotation = localRotation;
		}

		// Token: 0x06005C1B RID: 23579 RVA: 0x001D9303 File Offset: 0x001D7503
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06005C1C RID: 23580 RVA: 0x001D9325 File Offset: 0x001D7525
		private long CowboyCycleLengthMs()
		{
			return (long)(this.cowboyCycleDuration * 1000f);
		}

		// Token: 0x06005C1D RID: 23581 RVA: 0x001D9334 File Offset: 0x001D7534
		private long WheelCycleLengthMs()
		{
			return (long)(this.wheelCycleDuration * 1000f);
		}

		// Token: 0x06005C1E RID: 23582 RVA: 0x001D9344 File Offset: 0x001D7544
		public double CowboyPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CowboyCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005C1F RID: 23583 RVA: 0x001D9370 File Offset: 0x001D7570
		public double WheelPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.WheelCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06005C20 RID: 23584 RVA: 0x001D939B File Offset: 0x001D759B
		public int CowboyCycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CowboyCycleLengthMs());
		}

		// Token: 0x06005C21 RID: 23585 RVA: 0x001D93AB File Offset: 0x001D75AB
		public float CowboyCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.CowboyPlatformTime() / (double)this.cowboyCycleDuration), 0f, 1f);
		}

		// Token: 0x06005C22 RID: 23586 RVA: 0x001D93CB File Offset: 0x001D75CB
		public float WheelCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.WheelPlatformTime() / (double)this.wheelCycleDuration), 0f, 1f);
		}

		// Token: 0x06005C23 RID: 23587 RVA: 0x001D93EB File Offset: 0x001D75EB
		public bool IsEvenCycle()
		{
			return this.CowboyCycleCount() % 2 == 0;
		}

		// Token: 0x0400696C RID: 26988
		public BuilderPiece myPiece;

		// Token: 0x0400696D RID: 26989
		[SerializeField]
		private Transform wheelTransform;

		// Token: 0x0400696E RID: 26990
		[SerializeField]
		private Transform cowboyTransform;

		// Token: 0x0400696F RID: 26991
		[SerializeField]
		private SlingshotProjectileHitNotifier wheelHitNotifier;

		// Token: 0x04006970 RID: 26992
		[SerializeField]
		private SlingshotProjectileHitNotifier cowboyHitNotifier;

		// Token: 0x04006971 RID: 26993
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04006972 RID: 26994
		[SerializeField]
		protected SoundBankPlayer wheelHitSound;

		// Token: 0x04006973 RID: 26995
		[SerializeField]
		protected Animation wheelHitAnimation;

		// Token: 0x04006974 RID: 26996
		[SerializeField]
		protected SoundBankPlayer cowboyHitSound;

		// Token: 0x04006975 RID: 26997
		[SerializeField]
		private Animation cowboyHitAnimation;

		// Token: 0x04006976 RID: 26998
		[SerializeField]
		private float hitCooldown = 1f;

		// Token: 0x04006977 RID: 26999
		private double lastHitTime;

		// Token: 0x04006978 RID: 27000
		private BuilderShootingGallery.FunctionalState currentState;

		// Token: 0x04006979 RID: 27001
		private bool activated;

		// Token: 0x0400697A RID: 27002
		[SerializeField]
		private float cowboyVelocity;

		// Token: 0x0400697B RID: 27003
		[SerializeField]
		private Transform cowboyStart;

		// Token: 0x0400697C RID: 27004
		[SerializeField]
		private Transform cowboyEnd;

		// Token: 0x0400697D RID: 27005
		[SerializeField]
		private AnimationCurve cowboyCurve;

		// Token: 0x0400697E RID: 27006
		[SerializeField]
		private float wheelVelocity;

		// Token: 0x0400697F RID: 27007
		private Quaternion cowboyInitLocalRotation = Quaternion.identity;

		// Token: 0x04006980 RID: 27008
		private Vector3 cowboyInitLocalPos = Vector3.zero;

		// Token: 0x04006981 RID: 27009
		private Quaternion wheelInitLocalRot = Quaternion.identity;

		// Token: 0x04006982 RID: 27010
		private float cowboyCycleDuration;

		// Token: 0x04006983 RID: 27011
		private float wheelCycleDuration;

		// Token: 0x04006984 RID: 27012
		private float distance;

		// Token: 0x04006985 RID: 27013
		private float currT;

		// Token: 0x04006986 RID: 27014
		private bool currForward;

		// Token: 0x04006987 RID: 27015
		private float dtSinceServerUpdate;

		// Token: 0x04006988 RID: 27016
		private int lastServerTimeStamp;

		// Token: 0x04006989 RID: 27017
		private float rotateStartAmt;

		// Token: 0x0400698A RID: 27018
		private float rotateAmt;

		// Token: 0x02000E64 RID: 3684
		private enum FunctionalState
		{
			// Token: 0x0400698C RID: 27020
			Idle,
			// Token: 0x0400698D RID: 27021
			HitWheel,
			// Token: 0x0400698E RID: 27022
			HitCowboy
		}
	}
}
