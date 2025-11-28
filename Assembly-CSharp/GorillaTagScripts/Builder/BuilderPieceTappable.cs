using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E54 RID: 3668
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(GorillaSurfaceOverride))]
	public class BuilderPieceTappable : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional, IBuilderTappable
	{
		// Token: 0x06005B92 RID: 23442 RVA: 0x001D6732 File Offset: 0x001D4932
		public virtual bool CanTap()
		{
			return this.isPieceActive && Time.time > this.lastTapTime + this.tapCooldown;
		}

		// Token: 0x06005B93 RID: 23443 RVA: 0x001D6752 File Offset: 0x001D4952
		public void OnTapLocal(float tapStrength)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (!this.CanTap())
			{
				return;
			}
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
		}

		// Token: 0x06005B94 RID: 23444 RVA: 0x001D678B File Offset: 0x001D498B
		public virtual void OnTapReplicated()
		{
			UnityEvent onTapped = this.OnTapped;
			if (onTapped == null)
			{
				return;
			}
			onTapped.Invoke();
		}

		// Token: 0x06005B95 RID: 23445 RVA: 0x001D679D File Offset: 0x001D499D
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderPieceTappable.FunctionalState.Idle;
		}

		// Token: 0x06005B96 RID: 23446 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005B97 RID: 23447 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005B98 RID: 23448 RVA: 0x001D67A6 File Offset: 0x001D49A6
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
		}

		// Token: 0x06005B99 RID: 23449 RVA: 0x001D67B0 File Offset: 0x001D49B0
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderPieceTappable.FunctionalState.Tap)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06005B9A RID: 23450 RVA: 0x001D6800 File Offset: 0x001D4A00
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderPieceTappable.FunctionalState.Tap)
			{
				this.lastTapTime = Time.time;
				this.OnTapReplicated();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderPieceTappable.FunctionalState)newState;
		}

		// Token: 0x06005B9B RID: 23451 RVA: 0x001D6850 File Offset: 0x001D4A50
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
			if (newState == 1 && this.CanTap())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06005B9C RID: 23452 RVA: 0x001D68AB File Offset: 0x001D4AAB
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06005B9D RID: 23453 RVA: 0x001D68B4 File Offset: 0x001D4AB4
		public void FunctionalPieceUpdate()
		{
			if (this.lastTapTime + this.tapCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x040068E5 RID: 26853
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040068E6 RID: 26854
		[SerializeField]
		protected float tapCooldown = 0.5f;

		// Token: 0x040068E7 RID: 26855
		private bool isPieceActive;

		// Token: 0x040068E8 RID: 26856
		private float lastTapTime;

		// Token: 0x040068E9 RID: 26857
		private BuilderPieceTappable.FunctionalState currentState;

		// Token: 0x040068EA RID: 26858
		[Tooltip("Called on all clients when this collider is tapped by anyone")]
		[SerializeField]
		protected UnityEvent OnTapped;

		// Token: 0x02000E55 RID: 3669
		private enum FunctionalState
		{
			// Token: 0x040068EC RID: 26860
			Idle,
			// Token: 0x040068ED RID: 26861
			Tap
		}
	}
}
