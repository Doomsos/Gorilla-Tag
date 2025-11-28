using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000667 RID: 1639
public class GRAbilityBase
{
	// Token: 0x060029EB RID: 10731 RVA: 0x000E287C File Offset: 0x000E0A7C
	public virtual void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.root = root;
		this.anim = anim;
		this.agent = agent;
		this.head = head;
		this.audioSource = audioSource;
		this.lineOfSight = lineOfSight;
		this.rb = agent.GetComponent<Rigidbody>();
		this.entity = agent.GetComponent<GameEntity>();
		this.attributes = agent.GetComponent<GRAttributes>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x000E28EA File Offset: 0x000E0AEA
	public virtual void Start()
	{
		this.startTime = Time.timeAsDouble;
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Stop()
	{
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool IsDone()
	{
		return false;
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Think(float dt)
	{
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000E28F7 File Offset: 0x000E0AF7
	public virtual void Update(float dt)
	{
		this.UpdateShared(dt);
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000E28F7 File Offset: 0x000E0AF7
	public virtual void UpdateRemote(float dt)
	{
		this.UpdateShared(dt);
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void UpdateShared(float dt)
	{
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x000E2900 File Offset: 0x000E0B00
	protected virtual void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x0400361E RID: 13854
	protected GameAgent agent;

	// Token: 0x0400361F RID: 13855
	protected GameEntity entity;

	// Token: 0x04003620 RID: 13856
	protected Animation anim;

	// Token: 0x04003621 RID: 13857
	protected Transform root;

	// Token: 0x04003622 RID: 13858
	protected Transform head;

	// Token: 0x04003623 RID: 13859
	protected AudioSource audioSource;

	// Token: 0x04003624 RID: 13860
	protected GRSenseLineOfSight lineOfSight;

	// Token: 0x04003625 RID: 13861
	protected Rigidbody rb;

	// Token: 0x04003626 RID: 13862
	protected GRAttributes attributes;

	// Token: 0x04003627 RID: 13863
	[ReadOnly]
	public double startTime;

	// Token: 0x04003628 RID: 13864
	protected int walkableArea = -1;
}
