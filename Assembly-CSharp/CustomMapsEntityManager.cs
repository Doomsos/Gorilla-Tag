using Fusion;
using UnityEngine;

[NetworkBehaviourWeaved(0)]
public class CustomMapsEntityManager : GameEntityManager
{
	public override bool IsPositionInManagerBounds(Vector3 pos)
	{
		if (CustomMapLoader.CanLoadEntities)
		{
			return true;
		}
		return base.IsPositionInManagerBounds(pos);
	}

	protected override bool IsInZone()
	{
		if (!CustomMapLoader.CanLoadEntities)
		{
			return base.IsInZone();
		}
		bool flag = true;
		for (int i = 0; i < zoneComponents.Count; i++)
		{
			flag &= zoneComponents[i].IsZoneReady();
		}
		return flag;
	}

	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool P_0)
	{
		base.CopyBackingFieldsToState(P_0);
	}

	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
