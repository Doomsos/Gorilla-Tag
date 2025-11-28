using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E57 RID: 3671
	public class BuilderPieceToggle : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent, IBuilderTappable
	{
		// Token: 0x06005BAE RID: 23470 RVA: 0x001D6DAC File Offset: 0x001D4FAC
		private void Awake()
		{
			this.colliders.Clear();
			if (this.toggleType == BuilderPieceToggle.ToggleType.OnTriggerEnter)
			{
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
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x001D6E64 File Offset: 0x001D5064
		private void OnDestroy()
		{
			foreach (BuilderSmallHandTrigger builderSmallHandTrigger in this.handTriggers)
			{
				if (!(builderSmallHandTrigger == null))
				{
					builderSmallHandTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerEntered));
				}
			}
			foreach (BuilderSmallMonkeTrigger builderSmallMonkeTrigger in this.bodyTriggers)
			{
				if (!(builderSmallMonkeTrigger == null))
				{
					builderSmallMonkeTrigger.onPlayerEnteredTrigger -= new Action<int>(this.OnBodyTriggerEntered);
				}
			}
		}

		// Token: 0x06005BB0 RID: 23472 RVA: 0x001D6EE4 File Offset: 0x001D50E4
		private bool CanTap()
		{
			return (!this.onlySmallMonkeTaps || !this.myPiece.GetTable().isTableMutable || (double)VRRigCache.Instance.localRig.Rig.scaleFactor <= 0.99) && this.toggleType == BuilderPieceToggle.ToggleType.OnTap && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced;
		}

		// Token: 0x06005BB1 RID: 23473 RVA: 0x001D6F45 File Offset: 0x001D5145
		public void OnTapLocal(float tapStrength)
		{
			if (!this.CanTap())
			{
				Debug.Log("BuilderPieceToggle Can't Tap");
				return;
			}
			Debug.Log("Tap Local");
			this.ToggleStateRequest();
		}

		// Token: 0x06005BB2 RID: 23474 RVA: 0x001D6F6A File Offset: 0x001D516A
		private bool CanTrigger()
		{
			return this.toggleType == BuilderPieceToggle.ToggleType.OnTriggerEnter && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced;
		}

		// Token: 0x06005BB3 RID: 23475 RVA: 0x001D6F85 File Offset: 0x001D5185
		private void OnHandTriggerEntered()
		{
			if (this.CanTrigger())
			{
				this.ToggleStateRequest();
				return;
			}
			Debug.Log("BuilderPieceToggle Can't Trigger");
		}

		// Token: 0x06005BB4 RID: 23476 RVA: 0x001D6FA0 File Offset: 0x001D51A0
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
				this.ToggleStateMaster(player.GetPlayerRef());
				return;
			}
			Debug.Log("BuilderPieceToggle Can't Trigger");
		}

		// Token: 0x06005BB5 RID: 23477 RVA: 0x001D6FEC File Offset: 0x001D51EC
		private void ToggleStateRequest()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			BuilderPieceToggle.ToggleStates toggleStates = (this.toggleState == BuilderPieceToggle.ToggleStates.Off) ? BuilderPieceToggle.ToggleStates.On : BuilderPieceToggle.ToggleStates.Off;
			Debug.Log("BuilderPieceToggle" + string.Format(" Requesting state {0}", toggleStates));
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, (byte)toggleStates);
		}

		// Token: 0x06005BB6 RID: 23478 RVA: 0x001D7054 File Offset: 0x001D5254
		private void ToggleStateMaster(Player instigator)
		{
			BuilderPieceToggle.ToggleStates toggleStates = (this.toggleState == BuilderPieceToggle.ToggleStates.Off) ? BuilderPieceToggle.ToggleStates.On : BuilderPieceToggle.ToggleStates.Off;
			Debug.Log("BuilderPieceToggle" + string.Format(" Set Master state {0}", toggleStates));
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)toggleStates, instigator, NetworkSystem.Instance.ServerTimestamp);
		}

		// Token: 0x06005BB7 RID: 23479 RVA: 0x001D70BC File Offset: 0x001D52BC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				Debug.Log("BuilderPieceToggle State Invalid");
				return;
			}
			Debug.Log("BuilderPieceToggle" + string.Format(" State Changed {0}", newState));
			if ((BuilderPieceToggle.ToggleStates)newState != this.toggleState)
			{
				if (newState == 1)
				{
					Debug.Log("BuilderPieceToggle Toggled On");
					UnityEvent toggledOn = this.ToggledOn;
					if (toggledOn != null)
					{
						toggledOn.Invoke();
					}
				}
				else
				{
					Debug.Log("BuilderPieceToggle Toggled Off");
					this.ToggledOff.Invoke();
				}
			}
			this.toggleState = (BuilderPieceToggle.ToggleStates)newState;
		}

		// Token: 0x06005BB8 RID: 23480 RVA: 0x001D7144 File Offset: 0x001D5344
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				Debug.Log("BuilderPieceToggle State Invalid or Player Null");
				return;
			}
			Debug.Log("BuilderPieceToggle" + string.Format(" State Request {0}", newState));
			if (newState != (byte)this.toggleState)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
				return;
			}
			Debug.Log("BuilderPieceToggle Same State");
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x001D71D1 File Offset: 0x001D53D1
		public bool IsStateValid(byte state)
		{
			Debug.Log(string.Format("Is State Valid? {0}", state));
			return state <= 1;
		}

		// Token: 0x06005BBA RID: 23482 RVA: 0x00002789 File Offset: 0x00000989
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06005BBC RID: 23484 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06005BBD RID: 23485 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x001D71F0 File Offset: 0x001D53F0
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = true;
			}
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x001D7244 File Offset: 0x001D5444
		public void OnPieceDeactivate()
		{
			this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			foreach (Collider collider in this.colliders)
			{
				collider.enabled = false;
			}
		}

		// Token: 0x040068F9 RID: 26873
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040068FA RID: 26874
		[SerializeField]
		private BuilderPieceToggle.ToggleType toggleType;

		// Token: 0x040068FB RID: 26875
		public bool onlySmallMonkeTaps;

		// Token: 0x040068FC RID: 26876
		[SerializeField]
		private BuilderSmallHandTrigger[] handTriggers;

		// Token: 0x040068FD RID: 26877
		[SerializeField]
		private BuilderSmallMonkeTrigger[] bodyTriggers;

		// Token: 0x040068FE RID: 26878
		[SerializeField]
		protected UnityEvent ToggledOn;

		// Token: 0x040068FF RID: 26879
		[SerializeField]
		protected UnityEvent ToggledOff;

		// Token: 0x04006900 RID: 26880
		private List<Collider> colliders = new List<Collider>(5);

		// Token: 0x04006901 RID: 26881
		private BuilderPieceToggle.ToggleStates toggleState;

		// Token: 0x02000E58 RID: 3672
		[Serializable]
		private enum ToggleType
		{
			// Token: 0x04006903 RID: 26883
			OnTap,
			// Token: 0x04006904 RID: 26884
			OnTriggerEnter
		}

		// Token: 0x02000E59 RID: 3673
		private enum ToggleStates
		{
			// Token: 0x04006906 RID: 26886
			Off,
			// Token: 0x04006907 RID: 26887
			On
		}
	}
}
