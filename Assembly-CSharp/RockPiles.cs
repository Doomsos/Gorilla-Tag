using System;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class RockPiles : MonoBehaviour
{
	// Token: 0x06000CFF RID: 3327 RVA: 0x00045FC0 File Offset: 0x000441C0
	public void Show(int visiblePercentage)
	{
		if (visiblePercentage <= 0)
		{
			this.ShowRock(-1);
			return;
		}
		int rockToShow = -1;
		int num = -1;
		for (int i = 0; i < this._rocks.Length; i++)
		{
			RockPiles.RockPile rockPile = this._rocks[i];
			if (visiblePercentage >= rockPile.threshold && num < rockPile.threshold)
			{
				rockToShow = i;
				num = rockPile.threshold;
			}
		}
		this.ShowRock(rockToShow);
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x00046020 File Offset: 0x00044220
	private void ShowRock(int rockToShow)
	{
		for (int i = 0; i < this._rocks.Length; i++)
		{
			this._rocks[i].visual.SetActive(i == rockToShow);
		}
	}

	// Token: 0x04000FFC RID: 4092
	[SerializeField]
	private RockPiles.RockPile[] _rocks;

	// Token: 0x020001DD RID: 477
	[Serializable]
	public struct RockPile
	{
		// Token: 0x04000FFD RID: 4093
		public GameObject visual;

		// Token: 0x04000FFE RID: 4094
		public int threshold;
	}
}
