using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class CrittersFoodSettings : CrittersActorSettings
{
	// Token: 0x060001B4 RID: 436 RVA: 0x0000A974 File Offset: 0x00008B74
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersFood crittersFood = (CrittersFood)this.parentActor;
		crittersFood.maxFood = this._maxFood;
		crittersFood.currentFood = this._currentFood;
		crittersFood.startingSize = this._startingSize;
		crittersFood.currentSize = this._currentSize;
		crittersFood.food = this._food;
		crittersFood.disableWhenEmpty = this._disableWhenEmpty;
		crittersFood.SpawnData(this._maxFood, this._currentFood, this._startingSize);
	}

	// Token: 0x040001FB RID: 507
	public float _maxFood;

	// Token: 0x040001FC RID: 508
	public float _currentFood;

	// Token: 0x040001FD RID: 509
	public float _startingSize;

	// Token: 0x040001FE RID: 510
	public float _currentSize;

	// Token: 0x040001FF RID: 511
	public Transform _food;

	// Token: 0x04000200 RID: 512
	public bool _disableWhenEmpty;
}
