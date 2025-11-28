using System;
using UnityEngine;

// Token: 0x0200070B RID: 1803
public class GRSpringMovement
{
	// Token: 0x06002E4F RID: 11855 RVA: 0x000FBCEC File Offset: 0x000F9EEC
	public GRSpringMovement(float _tension, float _dampening)
	{
		this.tension = _tension;
		this.dampening = _dampening;
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x000FBD1F File Offset: 0x000F9F1F
	public void Reset()
	{
		this.pos = 0f;
		this.target = 0f;
		this.speed = 0f;
		this.wasAlreadyAtTargetLastUpdate = false;
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x000FBD49 File Offset: 0x000F9F49
	public void SetHardStopAtTarget(bool _hardStopAtTarget)
	{
		if (this.hardStopAtTarget == _hardStopAtTarget)
		{
			return;
		}
		this.hardStopAtTarget = _hardStopAtTarget;
		this.speed = 0f;
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x000FBD68 File Offset: 0x000F9F68
	public void Update()
	{
		this.wasAlreadyAtTargetLastUpdate = (this.pos == this.target && this.speed == 0f);
		float num = this.pos;
		float num2 = 0.001f;
		float num3 = Mathf.Min(Time.deltaTime, 0.05f);
		float num4 = 6.2832f / this.tension;
		float num5 = num4 * num4 * (this.target - this.pos) - 2f * this.dampening * num4 * this.speed;
		this.speed += num5 * num3;
		this.pos += this.speed * num3;
		if (this.hardStopAtTarget)
		{
			if ((num <= this.pos && this.pos + num2 >= this.target) || (num >= this.pos && this.pos - num2 <= this.target))
			{
				this.speed = 0f;
				this.pos = this.target;
				return;
			}
		}
		else if (Mathf.Abs(num - this.target) < num2 && Mathf.Abs(this.speed) < num2)
		{
			this.speed = 0f;
			this.pos = this.target;
		}
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000FBE99 File Offset: 0x000FA099
	public bool HitTargetLastUpdate()
	{
		return this.IsAtTarget() && !this.wasAlreadyAtTargetLastUpdate;
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000FBEAE File Offset: 0x000FA0AE
	public bool IsAtTarget()
	{
		return this.pos == this.target && this.speed == 0f;
	}

	// Token: 0x04003C74 RID: 15476
	public float tension = 1f;

	// Token: 0x04003C75 RID: 15477
	public float dampening = 0.7f;

	// Token: 0x04003C76 RID: 15478
	public float target;

	// Token: 0x04003C77 RID: 15479
	public bool hardStopAtTarget = true;

	// Token: 0x04003C78 RID: 15480
	public float pos;

	// Token: 0x04003C79 RID: 15481
	public float speed;

	// Token: 0x04003C7A RID: 15482
	private bool wasAlreadyAtTargetLastUpdate;
}
