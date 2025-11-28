using System;
using UnityEngine;

// Token: 0x02000701 RID: 1793
public class GRShieldCollider : MonoBehaviour
{
	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x06002E04 RID: 11780 RVA: 0x000FA5AE File Offset: 0x000F87AE
	public float KnockbackVelocity
	{
		get
		{
			return this.knockbackVelocity;
		}
	}

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x06002E05 RID: 11781 RVA: 0x000FA5B6 File Offset: 0x000F87B6
	public GRToolDirectionalShield ShieldTool
	{
		get
		{
			return this.shieldTool;
		}
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000FA5BE File Offset: 0x000F87BE
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.OnEnemyBlocked(enemyPosition);
		}
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000FA5DA File Offset: 0x000F87DA
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
