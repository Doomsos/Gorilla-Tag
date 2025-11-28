using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000CF5 RID: 3317
public class ZoneDef : MonoBehaviour
{
	// Token: 0x1700077D RID: 1917
	// (get) Token: 0x06005084 RID: 20612 RVA: 0x0019E844 File Offset: 0x0019CA44
	public GroupJoinZoneAB groupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.groupZone,
				b = this.groupZoneB
			};
		}
	}

	// Token: 0x06005085 RID: 20613 RVA: 0x0019E874 File Offset: 0x0019CA74
	public bool IsSameZone(ZoneDef other)
	{
		return !(other == null) && this.zoneId == other.zoneId && this.subZoneId == other.subZoneId;
	}

	// Token: 0x04005FE8 RID: 24552
	public GTZone zoneId;

	// Token: 0x04005FE9 RID: 24553
	[FormerlySerializedAs("subZoneType")]
	[FormerlySerializedAs("subZone")]
	public GTSubZone subZoneId;

	// Token: 0x04005FEA RID: 24554
	public GroupJoinZoneA groupZone;

	// Token: 0x04005FEB RID: 24555
	public GroupJoinZoneB groupZoneB;

	// Token: 0x04005FEC RID: 24556
	public int trackStayIntervalSec = 30;

	// Token: 0x04005FED RID: 24557
	[Space]
	public bool trackEnter = true;

	// Token: 0x04005FEE RID: 24558
	public bool trackExit;

	// Token: 0x04005FEF RID: 24559
	public bool trackStay = true;
}
