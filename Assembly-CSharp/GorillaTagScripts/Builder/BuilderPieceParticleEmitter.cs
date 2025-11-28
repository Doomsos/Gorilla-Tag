using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E53 RID: 3667
	public class BuilderPieceParticleEmitter : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06005B89 RID: 23433 RVA: 0x001D65C0 File Offset: 0x001D47C0
		private void OnZoneChanged()
		{
			this.inBuilderZone = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.inBuilderZone && this.isPieceActive)
			{
				this.StartParticles();
				return;
			}
			if (!this.inBuilderZone)
			{
				this.StopParticles();
			}
		}

		// Token: 0x06005B8A RID: 23434 RVA: 0x001D6614 File Offset: 0x001D4814
		private void StopParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
		}

		// Token: 0x06005B8B RID: 23435 RVA: 0x001D6674 File Offset: 0x001D4874
		private void StartParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
		}

		// Token: 0x06005B8C RID: 23436 RVA: 0x001D66D0 File Offset: 0x001D48D0
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.StopParticles();
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06005B8D RID: 23437 RVA: 0x001D6704 File Offset: 0x001D4904
		public void OnPieceDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}

		// Token: 0x06005B8E RID: 23438 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005B8F RID: 23439 RVA: 0x001D672C File Offset: 0x001D492C
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			if (this.inBuilderZone)
			{
				this.StartParticles();
			}
		}

		// Token: 0x06005B90 RID: 23440 RVA: 0x001D6743 File Offset: 0x001D4943
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			this.StopParticles();
		}

		// Token: 0x040068E1 RID: 26849
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040068E2 RID: 26850
		[SerializeField]
		private List<ParticleSystem> particles;

		// Token: 0x040068E3 RID: 26851
		private bool inBuilderZone;

		// Token: 0x040068E4 RID: 26852
		private bool isPieceActive;
	}
}
