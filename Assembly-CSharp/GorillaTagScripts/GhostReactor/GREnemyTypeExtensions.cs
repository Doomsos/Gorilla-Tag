using System;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000E3D RID: 3645
	public static class GREnemyTypeExtensions
	{
		// Token: 0x06005AE2 RID: 23266 RVA: 0x001D1940 File Offset: 0x001CFB40
		public static Type GetComponentType(this GREnemyType enemyType)
		{
			Type result;
			switch (enemyType)
			{
			case GREnemyType.Chaser:
				result = typeof(GREnemyChaser);
				break;
			case GREnemyType.Pest:
				result = typeof(GREnemyPest);
				break;
			case GREnemyType.Phantom:
				result = typeof(GREnemyPhantom);
				break;
			case GREnemyType.Ranged:
				result = typeof(GREnemyRanged);
				break;
			case GREnemyType.Summoner:
				result = typeof(GREnemySummoner);
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x06005AE3 RID: 23267 RVA: 0x001D19B0 File Offset: 0x001CFBB0
		public static GREnemyType? GetEnemyType(this GameEntity entity)
		{
			GameObject gameObject = entity.gameObject;
			foreach (object obj in Enum.GetValues(typeof(GREnemyType)))
			{
				GREnemyType grenemyType = (GREnemyType)obj;
				Type componentType = grenemyType.GetComponentType();
				if (componentType != null && gameObject.GetComponent(componentType) != null)
				{
					return new GREnemyType?(grenemyType);
				}
			}
			return default(GREnemyType?);
		}
	}
}
