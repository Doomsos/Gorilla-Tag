using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class ColliderEnabledManager : MonoBehaviour
{
	// Token: 0x060000B9 RID: 185 RVA: 0x00005399 File Offset: 0x00003599
	private void Start()
	{
		this.floorEnabled = true;
		this.floorCollidersEnabled = true;
		ColliderEnabledManager.instance = this;
	}

	// Token: 0x060000BA RID: 186 RVA: 0x000053AF File Offset: 0x000035AF
	private void OnDestroy()
	{
		ColliderEnabledManager.instance = null;
	}

	// Token: 0x060000BB RID: 187 RVA: 0x000053B7 File Offset: 0x000035B7
	public void DisableFloorForFrame()
	{
		this.floorEnabled = false;
	}

	// Token: 0x060000BC RID: 188 RVA: 0x000053C0 File Offset: 0x000035C0
	private void LateUpdate()
	{
		if (!this.floorEnabled && this.floorCollidersEnabled)
		{
			this.DisableFloor();
		}
		if (!this.floorCollidersEnabled && Time.time > this.timeDisabled + this.disableLength)
		{
			this.floorCollidersEnabled = true;
		}
		Collider[] array = this.floorCollider;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.floorCollidersEnabled;
		}
		if (this.floorCollidersEnabled)
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsBeforeMaterial;
			}
		}
		else
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsAfterMaterial;
			}
		}
		this.floorEnabled = true;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00005480 File Offset: 0x00003680
	private void DisableFloor()
	{
		this.floorCollidersEnabled = false;
		this.timeDisabled = Time.time;
	}

	// Token: 0x040000CF RID: 207
	public static ColliderEnabledManager instance;

	// Token: 0x040000D0 RID: 208
	public Collider[] floorCollider;

	// Token: 0x040000D1 RID: 209
	public bool floorEnabled;

	// Token: 0x040000D2 RID: 210
	public bool wasFloorEnabled;

	// Token: 0x040000D3 RID: 211
	public bool floorCollidersEnabled;

	// Token: 0x040000D4 RID: 212
	[GorillaSoundLookup]
	public int wallsBeforeMaterial;

	// Token: 0x040000D5 RID: 213
	[GorillaSoundLookup]
	public int wallsAfterMaterial;

	// Token: 0x040000D6 RID: 214
	public GorillaSurfaceOverride[] walls;

	// Token: 0x040000D7 RID: 215
	public float timeDisabled;

	// Token: 0x040000D8 RID: 216
	public float disableLength;
}
