using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DA9 RID: 3497
	[RequireComponent(typeof(Collider))]
	public class BuilderPieceHandHold : MonoBehaviour, IGorillaGrabable, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x060055F4 RID: 22004 RVA: 0x001B0665 File Offset: 0x001AE865
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.myCollider = base.GetComponent<Collider>();
			this.initialized = true;
		}

		// Token: 0x060055F5 RID: 22005 RVA: 0x001B0683 File Offset: 0x001AE883
		public bool IsHandHoldMoving()
		{
			return this.myPiece.IsPieceMoving();
		}

		// Token: 0x060055F6 RID: 22006 RVA: 0x001B0690 File Offset: 0x001AE890
		public bool MomentaryGrabOnly()
		{
			return this.forceMomentary;
		}

		// Token: 0x060055F7 RID: 22007 RVA: 0x001B0698 File Offset: 0x001AE898
		public virtual bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && (!this.myPiece.GetTable().isTableMutable || grabber.Player.scale < 0.5f);
		}

		// Token: 0x060055F8 RID: 22008 RVA: 0x001B06D0 File Offset: 0x001AE8D0
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Initialize();
			grabbedTransform = base.transform;
			Vector3 position = grabber.transform.position;
			localGrabbedPosition = base.transform.InverseTransformPoint(position);
			this.activeGrabbers.Add(grabber);
			this.isGrabbed = true;
			Vector3 vector;
			grabber.Player.AddHandHold(base.transform, localGrabbedPosition, grabber, grabber.IsRightHand, false, out vector);
		}

		// Token: 0x060055F9 RID: 22009 RVA: 0x001B073D File Offset: 0x001AE93D
		public void OnGrabReleased(GorillaGrabber grabber)
		{
			this.Initialize();
			this.activeGrabbers.Remove(grabber);
			this.isGrabbed = (this.activeGrabbers.Count < 1);
			grabber.Player.RemoveHandHold(grabber, grabber.IsRightHand);
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x060055FA RID: 22010 RVA: 0x001B0778 File Offset: 0x001AE978
		// (set) Token: 0x060055FB RID: 22011 RVA: 0x001B0780 File Offset: 0x001AE980
		public bool TickRunning { get; set; }

		// Token: 0x060055FC RID: 22012 RVA: 0x001B078C File Offset: 0x001AE98C
		public void Tick()
		{
			if (!this.isGrabbed)
			{
				return;
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				if (gorillaGrabber != null && gorillaGrabber.Player.scale > 0.5f)
				{
					this.OnGrabReleased(gorillaGrabber);
				}
			}
		}

		// Token: 0x060055FD RID: 22013 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x060055FE RID: 22014 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060055FF RID: 22015 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005600 RID: 22016 RVA: 0x001B0804 File Offset: 0x001AEA04
		public void OnPieceActivate()
		{
			if (!this.TickRunning && this.myPiece.GetTable().isTableMutable)
			{
				TickSystem<object>.AddCallbackTarget(this);
			}
		}

		// Token: 0x06005601 RID: 22017 RVA: 0x001B0828 File Offset: 0x001AEA28
		public void OnPieceDeactivate()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
			}
			foreach (GorillaGrabber grabber in this.activeGrabbers)
			{
				this.OnGrabReleased(grabber);
			}
		}

		// Token: 0x06005603 RID: 22019 RVA: 0x00013E3B File Offset: 0x0001203B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x04006317 RID: 25367
		private bool initialized;

		// Token: 0x04006318 RID: 25368
		private Collider myCollider;

		// Token: 0x04006319 RID: 25369
		[SerializeField]
		private bool forceMomentary = true;

		// Token: 0x0400631A RID: 25370
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400631B RID: 25371
		private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>(2);

		// Token: 0x0400631C RID: 25372
		private bool isGrabbed;
	}
}
