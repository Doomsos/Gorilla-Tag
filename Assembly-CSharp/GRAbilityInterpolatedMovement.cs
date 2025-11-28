using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000669 RID: 1641
[Serializable]
public class GRAbilityInterpolatedMovement
{
	// Token: 0x060029FB RID: 10747 RVA: 0x000E2AD0 File Offset: 0x000E0CD0
	public void Setup(Transform root)
	{
		this.root = root;
		this.rb = root.gameObject.GetComponent<Rigidbody>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000E2AFA File Offset: 0x000E0CFA
	public void InitFromVelocityAndDuration(Vector3 velocity, float duration)
	{
		this.velocity = velocity;
		this.duration = duration;
		float magnitude = velocity.magnitude;
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000E2B14 File Offset: 0x000E0D14
	public void Start()
	{
		this.startPos = this.root.position;
		this.endPos = this.startPos + this.velocity * this.duration;
		this.endTime = Time.timeAsDouble + (double)this.duration;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.endPos, ref navMeshHit, 5f, this.walkableArea))
		{
			this.endPos = navMeshHit.position;
		}
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x00002789 File Offset: 0x00000989
	public void Stop()
	{
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000E2B8E File Offset: 0x000E0D8E
	public bool IsDone()
	{
		return Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000E2BA0 File Offset: 0x000E0DA0
	public void Update(float dt)
	{
		Vector3 position = this.root.position;
		float num = Mathf.Clamp01(1f - (float)((this.endTime - Time.timeAsDouble) / (double)this.duration));
		GRAbilityInterpolatedMovement.InterpType interpType = this.interpolationType;
		Vector3 vector;
		if (interpType != GRAbilityInterpolatedMovement.InterpType.Linear && interpType == GRAbilityInterpolatedMovement.InterpType.EaseOut)
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, AbilityHelperFunctions.EaseOutPower(num, 2.5f));
		}
		else
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, num);
		}
		vector.y = Mathf.Lerp(this.startPos.y, this.endPos.y, num * num);
		NavMeshHit navMeshHit;
		if (NavMesh.Raycast(position, vector, ref navMeshHit, this.walkableArea))
		{
			vector = navMeshHit.position;
		}
		this.root.position = vector;
		if (this.rb != null)
		{
			this.rb.position = vector;
		}
	}

	// Token: 0x0400362E RID: 13870
	public Vector3 velocity = Vector3.zero;

	// Token: 0x0400362F RID: 13871
	private Vector3 startPos;

	// Token: 0x04003630 RID: 13872
	private Vector3 endPos;

	// Token: 0x04003631 RID: 13873
	public float duration;

	// Token: 0x04003632 RID: 13874
	public double endTime;

	// Token: 0x04003633 RID: 13875
	public float maxVelocityMagnitude = 2f;

	// Token: 0x04003634 RID: 13876
	private Transform root;

	// Token: 0x04003635 RID: 13877
	private Rigidbody rb;

	// Token: 0x04003636 RID: 13878
	public GRAbilityInterpolatedMovement.InterpType interpolationType;

	// Token: 0x04003637 RID: 13879
	private int walkableArea = -1;

	// Token: 0x0200066A RID: 1642
	public enum InterpType
	{
		// Token: 0x04003639 RID: 13881
		Linear,
		// Token: 0x0400363A RID: 13882
		EaseOut
	}
}
