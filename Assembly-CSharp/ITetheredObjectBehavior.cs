using System;
using UnityEngine;

// Token: 0x02000415 RID: 1045
internal interface ITetheredObjectBehavior
{
	// Token: 0x060019C5 RID: 6597
	void DbgClear();

	// Token: 0x060019C6 RID: 6598
	void EnableDistanceConstraints(bool v, float playerScale);

	// Token: 0x060019C7 RID: 6599
	void EnableDynamics(bool enable, bool collider, bool kinematic);

	// Token: 0x060019C8 RID: 6600
	bool IsEnabled();

	// Token: 0x060019C9 RID: 6601
	void ReParent();

	// Token: 0x060019CA RID: 6602
	bool ReturnStep();

	// Token: 0x060019CB RID: 6603
	void TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership);
}
