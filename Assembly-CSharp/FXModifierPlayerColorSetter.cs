using System;
using UnityEngine;

// Token: 0x020002BD RID: 701
[RequireComponent(typeof(PlayerColoredCosmetic))]
public class FXModifierPlayerColorSetter : FXModifier
{
	// Token: 0x06001144 RID: 4420 RVA: 0x0005BF3F File Offset: 0x0005A13F
	public override void UpdateScale(float scale, Color color)
	{
		this.playerColoredCosmetic.UpdateColor(color);
	}

	// Token: 0x040015CF RID: 5583
	[SerializeField]
	private PlayerColoredCosmetic playerColoredCosmetic;
}
