using System;
using UnityEngine;

// Token: 0x020006BF RID: 1727
public class GREnemyXRayVisionEffect : MonoBehaviour
{
	// Token: 0x06002C7A RID: 11386 RVA: 0x00002789 File Offset: 0x00000989
	public void Awake()
	{
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x000F12B6 File Offset: 0x000EF4B6
	private void Start()
	{
		base.InvokeRepeating("UpdateEffect", 0f, 0.5f);
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x000F12CD File Offset: 0x000EF4CD
	private bool ShouldShowEffect()
	{
		return GRPlayer.GetLocal().HasXRayVision();
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x000F12D9 File Offset: 0x000EF4D9
	private void UpdateEffect()
	{
		this.enemyXRayEffect.SetActive(this.ShouldShowEffect());
	}

	// Token: 0x040039BE RID: 14782
	public GameObject enemyXRayEffect;
}
