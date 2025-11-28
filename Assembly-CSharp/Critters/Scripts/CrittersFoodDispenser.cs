using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Critters.Scripts
{
	// Token: 0x0200114B RID: 4427
	public class CrittersFoodDispenser : CrittersActor
	{
		// Token: 0x06006FC7 RID: 28615 RVA: 0x0024667A File Offset: 0x0024487A
		public override void Initialize()
		{
			base.Initialize();
			this.heldByPlayer = false;
		}

		// Token: 0x06006FC8 RID: 28616 RVA: 0x00246689 File Offset: 0x00244889
		public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
		{
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06006FC9 RID: 28617 RVA: 0x002466A4 File Offset: 0x002448A4
		protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
		{
			base.RemoteGrabbedBy(grabbingActor);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06006FCA RID: 28618 RVA: 0x002466B9 File Offset: 0x002448B9
		public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
		{
			base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
			this.heldByPlayer = false;
		}

		// Token: 0x06006FCB RID: 28619 RVA: 0x002466CF File Offset: 0x002448CF
		protected override void HandleRemoteReleased()
		{
			base.HandleRemoteReleased();
			this.heldByPlayer = false;
		}

		// Token: 0x04008032 RID: 32818
		[FormerlySerializedAs("isHeldByPlayer")]
		public bool heldByPlayer;
	}
}
