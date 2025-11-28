using System;
using UnityEngine;

// Token: 0x02000701 RID: 1793
public class GRShieldCollider : MonoBehaviour
{
	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x06002E04 RID: 11780 RVA: 0x000FA5CE File Offset: 0x000F87CE
	public float KnockbackVelocity
	{
		get
		{
			return this.knockbackVelocity;
		}
	}

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x06002E05 RID: 11781 RVA: 0x000FA5D6 File Offset: 0x000F87D6
	public GRToolDirectionalShield ShieldTool
	{
		get
		{
			return this.shieldTool;
		}
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000FA5DE File Offset: 0x000F87DE
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.OnEnemyBlocked(enemyPosition);
		}
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000FA5FA File Offset: 0x000F87FA
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.BlockHittable(enemyPosition, enemyAttackDirection, hittable, this);
		}
	}

	// Token: 0x04003C23 RID: 15395
	[SerializeField]
	private float knockbackVelocity = 3f;

	// Token: 0x04003C24 RID: 15396
	[SerializeField]
	private GRToolDirectionalShield shieldTool;
}
