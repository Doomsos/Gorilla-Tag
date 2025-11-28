using System;
using System.Collections.Generic;
using GorillaTagScripts.Builder;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class BuilderPieceScaleHandler : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x060022BD RID: 8893 RVA: 0x000B5B08 File Offset: 0x000B3D08
	public void OnPieceCreate(int pieceType, int pieceId)
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.SetScale(this.myPiece.GetScale());
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.SetScale(this.myPiece.GetScale());
		}
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000B5BB0 File Offset: 0x000B3DB0
	public void OnPieceDestroy()
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.RevertScale();
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.RevertScale();
		}
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPieceActivate()
	{
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPieceDeactivate()
	{
	}

	// Token: 0x04002D67 RID: 11623
	[SerializeField]
	private BuilderPiece myPiece;

	// Token: 0x04002D68 RID: 11624
	[SerializeField]
	private List<BuilderScaleAudioRadius> audioScalers = new List<BuilderScaleAudioRadius>();

	// Token: 0x04002D69 RID: 11625
	[SerializeField]
	private List<BuilderScaleParticles> particleScalers = new List<BuilderScaleParticles>();
}
