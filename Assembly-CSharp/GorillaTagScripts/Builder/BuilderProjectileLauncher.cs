using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E5C RID: 3676
	public class BuilderProjectileLauncher : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent
	{
		// Token: 0x06005BD5 RID: 23509 RVA: 0x001D79F4 File Offset: 0x001D5BF4
		private void LaunchProjectile(int timeStamp)
		{
			if (Time.time > this.lastFireTime + this.fireCooldown)
			{
				this.lastFireTime = Time.time;
				int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
				try
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(hash, true);
					this.projectileScale = this.myPiece.GetScale();
					gameObject.transform.localScale = Vector3.one * this.projectileScale;
					BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
					int num = HashCode.Combine<int, int>(this.myPiece.pieceId, timeStamp);
					if (this.allProjectiles.ContainsKey(num))
					{
						this.allProjectiles.Remove(num);
					}
					this.allProjectiles.Add(num, component);
					SlingshotProjectile.AOEKnockbackConfig aoeknockbackConfig = new SlingshotProjectile.AOEKnockbackConfig
					{
						aeoOuterRadius = this.knockbackConfig.aeoOuterRadius * this.projectileScale,
						aeoInnerRadius = this.knockbackConfig.aeoInnerRadius * this.projectileScale,
						applyAOEKnockback = this.knockbackConfig.applyAOEKnockback,
						impactVelocityThreshold = this.knockbackConfig.impactVelocityThreshold * this.projectileScale,
						knockbackVelocity = this.knockbackConfig.knockbackVelocity * this.projectileScale,
						playerProximityEffect = this.knockbackConfig.playerProximityEffect
					};
					component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeknockbackConfig);
					component.gravityMultiplier = this.gravityMultiplier;
					component.Launch(this.launchPosition.position, this.launchVelocity * this.projectileScale * this.launchPosition.up, this, num, this.projectileScale, timeStamp);
					if (this.launchSound != null && this.launchSound.clip != null)
					{
						this.launchSound.Play();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					throw;
				}
			}
		}

		// Token: 0x06005BD6 RID: 23510 RVA: 0x001D7BDC File Offset: 0x001D5DDC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if ((BuilderProjectileLauncher.FunctionalState)newState == this.currentState)
			{
				return;
			}
			this.currentState = (BuilderProjectileLauncher.FunctionalState)newState;
			if (newState == 1)
			{
				this.LaunchProjectile(timeStamp);
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06005BD7 RID: 23511 RVA: 0x00002789 File Offset: 0x00000989
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
		}

		// Token: 0x06005BD8 RID: 23512 RVA: 0x001D68AB File Offset: 0x001D4AAB
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06005BD9 RID: 23513 RVA: 0x001D7C30 File Offset: 0x001D5E30
		public void FunctionalPieceUpdate()
		{
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].UpdateProjectile();
			}
			if (PhotonNetwork.IsMasterClient && this.lastFireTime + this.fireCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06005BDA RID: 23514 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06005BDB RID: 23515 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005BDC RID: 23516 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005BDD RID: 23517 RVA: 0x001D7CB1 File Offset: 0x001D5EB1
		public void OnPieceActivate()
		{
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06005BDE RID: 23518 RVA: 0x001D7CC4 File Offset: 0x001D5EC4
		public void OnPieceDeactivate()
		{
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].Deactivate();
			}
		}

		// Token: 0x06005BDF RID: 23519 RVA: 0x001D7D0B File Offset: 0x001D5F0B
		public void RegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Add(projectile);
		}

		// Token: 0x06005BE0 RID: 23520 RVA: 0x001D7D19 File Offset: 0x001D5F19
		public void UnRegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Remove(projectile);
			this.allProjectiles.Remove(projectile.projectileId);
		}

		// Token: 0x0400691B RID: 26907
		private List<BuilderProjectile> launchedProjectiles = new List<BuilderProjectile>();

		// Token: 0x0400691C RID: 26908
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x0400691D RID: 26909
		[SerializeField]
		protected float fireCooldown = 2f;

		// Token: 0x0400691E RID: 26910
		[Tooltip("launch in Y direction")]
		[SerializeField]
		private Transform launchPosition;

		// Token: 0x0400691F RID: 26911
		[SerializeField]
		private float launchVelocity;

		// Token: 0x04006920 RID: 26912
		[SerializeField]
		private AudioSource launchSound;

		// Token: 0x04006921 RID: 26913
		[SerializeField]
		protected GameObject projectilePrefab;

		// Token: 0x04006922 RID: 26914
		protected float projectileScale = 0.06f;

		// Token: 0x04006923 RID: 26915
		[SerializeField]
		protected float gravityMultiplier = 1f;

		// Token: 0x04006924 RID: 26916
		public SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

		// Token: 0x04006925 RID: 26917
		private float lastFireTime;

		// Token: 0x04006926 RID: 26918
		private BuilderProjectileLauncher.FunctionalState currentState;

		// Token: 0x04006927 RID: 26919
		private Dictionary<int, BuilderProjectile> allProjectiles = new Dictionary<int, BuilderProjectile>();

		// Token: 0x02000E5D RID: 3677
		private enum FunctionalState
		{
			// Token: 0x04006929 RID: 26921
			Idle,
			// Token: 0x0400692A RID: 26922
			Fire
		}
	}
}
