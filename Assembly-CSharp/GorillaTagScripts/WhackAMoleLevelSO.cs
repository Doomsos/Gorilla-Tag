using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DA6 RID: 3494
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WhackAMoleLevelSetting", order = 1)]
	public class WhackAMoleLevelSO : ScriptableObject
	{
		// Token: 0x060055E3 RID: 21987 RVA: 0x001B047C File Offset: 0x001AE67C
		public int GetMinScore(bool isCoop)
		{
			if (!isCoop)
			{
				return this.minScore;
			}
			return this.minScore * 2;
		}

		// Token: 0x04006308 RID: 25352
		public int levelNumber;

		// Token: 0x04006309 RID: 25353
		public float levelDuration;

		// Token: 0x0400630A RID: 25354
		[Tooltip("For how long do the moles stay visible?")]
		public float showMoleDuration;

		// Token: 0x0400630B RID: 25355
		[Tooltip("How fast we pick a random new mole?")]
		public float pickNextMoleTime;

		// Token: 0x0400630C RID: 25356
		[Tooltip("Minimum score to get in order to be able to proceed to the next level")]
		[SerializeField]
		private int minScore;

		// Token: 0x0400630D RID: 25357
		[Tooltip("Chance of each mole being a hazard mole at the start, and end, of the level.")]
		public Vector2 hazardMoleChance = new Vector2(0f, 0.5f);

		// Token: 0x0400630E RID: 25358
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 minimumMoleCount = new Vector2(1f, 2f);

		// Token: 0x0400630F RID: 25359
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 maximumMoleCount = new Vector2(1.5f, 3f);
	}
}
