using System;
using UnityEngine;

// Token: 0x02000622 RID: 1570
public class GameLight : MonoBehaviour
{
	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x060027E7 RID: 10215 RVA: 0x000D46B8 File Offset: 0x000D28B8
	// (set) Token: 0x060027E8 RID: 10216 RVA: 0x000D46C0 File Offset: 0x000D28C0
	public float InitialIntensity { get; private set; }

	// Token: 0x060027E9 RID: 10217 RVA: 0x000D46C9 File Offset: 0x000D28C9
	public void Awake()
	{
		this.intensityMult = 1;
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000D46D2 File Offset: 0x000D28D2
	private void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		}
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000D46F0 File Offset: 0x000D28F0
	private void Start()
	{
		this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		this.initialized = true;
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000D470D File Offset: 0x000D290D
	private void OnDisable()
	{
		if (this.initialized)
		{
			GameLightingManager.instance.RemoveGameLight(this);
		}
	}

	// Token: 0x0400335F RID: 13151
	public Light light;

	// Token: 0x04003360 RID: 13152
	public bool negativeLight;

	// Token: 0x04003361 RID: 13153
	public bool isHighPriorityPlayerLight;

	// Token: 0x04003362 RID: 13154
	public Vector3 cachedPosition;

	// Token: 0x04003363 RID: 13155
	public Vector4 cachedColorAndIntensity;

	// Token: 0x04003364 RID: 13156
	public int lightId;

	// Token: 0x04003365 RID: 13157
	public int intensityMult = 1;

	// Token: 0x04003366 RID: 13158
	private bool initialized;
}
