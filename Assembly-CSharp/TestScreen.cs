using System;
using UnityEngine;

// Token: 0x02000395 RID: 917
public class TestScreen : ArcadeGame
{
	// Token: 0x060015F2 RID: 5618 RVA: 0x000743B1 File Offset: 0x000725B1
	public override byte[] GetNetworkState()
	{
		return null;
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x00002789 File Offset: 0x00000989
	public override void SetNetworkState(byte[] b)
	{
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x0007A9F0 File Offset: 0x00078BF0
	private int buttonToLightIndex(int player, ArcadeButtons button)
	{
		int num = 0;
		if (button <= ArcadeButtons.RIGHT)
		{
			switch (button)
			{
			case ArcadeButtons.GRAB:
				num = 0;
				break;
			case ArcadeButtons.UP:
				num = 1;
				break;
			case ArcadeButtons.GRAB | ArcadeButtons.UP:
				break;
			case ArcadeButtons.DOWN:
				num = 2;
				break;
			default:
				if (button != ArcadeButtons.LEFT)
				{
					if (button == ArcadeButtons.RIGHT)
					{
						num = 4;
					}
				}
				else
				{
					num = 3;
				}
				break;
			}
		}
		else if (button != ArcadeButtons.B0)
		{
			if (button != ArcadeButtons.B1)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					num = 7;
				}
			}
			else
			{
				num = 6;
			}
		}
		else
		{
			num = 5;
		}
		return (player * 8 + num) % this.lights.Length;
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x0007AA67 File Offset: 0x00078C67
	protected override void ButtonUp(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.red;
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x0007AA82 File Offset: 0x00078C82
	protected override void ButtonDown(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.green;
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnTimeout()
	{
	}

	// Token: 0x0400203E RID: 8254
	[SerializeField]
	private SpriteRenderer[] lights;

	// Token: 0x0400203F RID: 8255
	[SerializeField]
	private Transform dot;
}
