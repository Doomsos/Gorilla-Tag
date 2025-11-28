using System;

// Token: 0x0200072F RID: 1839
public class GRToolScannable : GRScannable
{
	// Token: 0x06002F68 RID: 12136 RVA: 0x00101CB4 File Offset: 0x000FFEB4
	public override void Start()
	{
		base.Start();
		if (this.gameEntity != null)
		{
			this.tool = this.gameEntity.GetComponent<GRTool>();
			this.upgradePiece = this.gameEntity.GetComponent<GRToolUpgradePiece>();
		}
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x00101CEC File Offset: 0x000FFEEC
	private void FetchMetadata(GhostReactor reactor)
	{
		if (this.metadata == null)
		{
			GRToolProgressionManager.ToolParts toolParts = GRToolProgressionManager.ToolParts.None;
			if (this.tool != null)
			{
				toolParts = GRUtils.GetToolPart(this.tool.toolType);
			}
			else if (this.upgradePiece != null)
			{
				toolParts = this.upgradePiece.matchingUpgrade;
			}
			if (toolParts != GRToolProgressionManager.ToolParts.None)
			{
				this.metadata = reactor.toolProgression.GetPartMetadata(toolParts);
			}
		}
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x00101D53 File Offset: 0x000FFF53
	public override string GetTitleText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.name;
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x00101D75 File Offset: 0x000FFF75
	public override string GetBodyText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.description;
	}

	// Token: 0x06002F6C RID: 12140 RVA: 0x00101D97 File Offset: 0x000FFF97
	public override string GetAnnotationText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.annotation;
	}

	// Token: 0x04003DFE RID: 15870
	private GRTool tool;

	// Token: 0x04003DFF RID: 15871
	private GRToolUpgradePiece upgradePiece;

	// Token: 0x04003E00 RID: 15872
	private GRToolProgressionManager.ToolProgressionMetaData metadata;
}
