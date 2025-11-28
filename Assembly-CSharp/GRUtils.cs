using System;

// Token: 0x0200074B RID: 1867
public class GRUtils
{
	// Token: 0x0600303C RID: 12348 RVA: 0x00108234 File Offset: 0x00106434
	public static string GetToolName(GRTool.GRToolType toolType)
	{
		switch (toolType)
		{
		case GRTool.GRToolType.Club:
			return "Baton";
		case GRTool.GRToolType.Collector:
			return "Collector";
		case GRTool.GRToolType.Flash:
			return "Flash";
		case GRTool.GRToolType.Lantern:
			return "Lantern";
		case GRTool.GRToolType.Revive:
			return "Revive";
		case GRTool.GRToolType.ShieldGun:
			return "Shield";
		case GRTool.GRToolType.DirectionalShield:
			return "Deflector";
		case GRTool.GRToolType.DockWrist:
			return "Dock";
		case GRTool.GRToolType.HockeyStick:
			return "Stick";
		}
		return "Unknown";
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x001082B4 File Offset: 0x001064B4
	public static GRToolProgressionManager.ToolParts GetToolPart(GRTool.GRToolType toolType)
	{
		switch (toolType)
		{
		case GRTool.GRToolType.Club:
			return GRToolProgressionManager.ToolParts.Baton;
		case GRTool.GRToolType.Collector:
			return GRToolProgressionManager.ToolParts.Collector;
		case GRTool.GRToolType.Flash:
			return GRToolProgressionManager.ToolParts.Flash;
		case GRTool.GRToolType.Lantern:
			return GRToolProgressionManager.ToolParts.Lantern;
		case GRTool.GRToolType.Revive:
			return GRToolProgressionManager.ToolParts.Revive;
		case GRTool.GRToolType.ShieldGun:
			return GRToolProgressionManager.ToolParts.ShieldGun;
		case GRTool.GRToolType.DirectionalShield:
			return GRToolProgressionManager.ToolParts.DirectionalShield;
		case GRTool.GRToolType.DockWrist:
			return GRToolProgressionManager.ToolParts.DockWrist;
		case GRTool.GRToolType.HockeyStick:
			return GRToolProgressionManager.ToolParts.HockeyStick;
		}
		return GRToolProgressionManager.ToolParts.None;
	}
}
