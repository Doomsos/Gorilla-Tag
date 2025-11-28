using System;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	public static class GREnemyTypeExtensions
	{
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
