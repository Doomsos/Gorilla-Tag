using System;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000E3E RID: 3646
	public static class GRGameObjectExtensions
	{
		// Token: 0x06005AE4 RID: 23268 RVA: 0x001D1A40 File Offset: 0x001CFC40
		public static GRTool.GRToolType GetToolType(this GameObject obj)
		{
			if (obj.GetComponentInParent<GRToolClub>() != null)
			{
				return GRTool.GRToolType.Club;
			}
			if (obj.GetComponentInParent<GRToolCollector>() != null)
			{
				return GRTool.GRToolType.Collector;
			}
			if (obj.GetComponentInParent<GRToolFlash>() != null)
			{
				return GRTool.GRToolType.Flash;
			}
			if (obj.GetComponentInParent<GRToolLantern>() != null)
			{
				return GRTool.GRToolType.Lantern;
			}
			if (obj.GetComponentInParent<GRToolRevive>() != null)
			{
				return GRTool.GRToolType.Revive;
			}
			if (obj.GetComponentInParent<GRToolShieldGun>() != null)
			{
				return GRTool.GRToolType.ShieldGun;
			}
			if (obj.GetComponentInParent<GRToolDirectionalShield>() != null)
			{
				return GRTool.GRToolType.DirectionalShield;
			}
			GRTool componentInParent = obj.GetComponentInParent<GRTool>();
			if (componentInParent != null && componentInParent.toolType == GRTool.GRToolType.HockeyStick)
			{
				return GRTool.GRToolType.HockeyStick;
			}
			componentInParent = obj.GetComponentInParent<GRTool>();
			if (componentInParent != null && componentInParent.toolType == GRTool.GRToolType.DockWrist)
			{
				return GRTool.GRToolType.DockWrist;
			}
			return GRTool.GRToolType.None;
		}
	}
}
