using System;
using GorillaExtensions;
using TagEffects;
using UnityEngine;

// Token: 0x02000351 RID: 849
[RequireComponent(typeof(Collider))]
public class HandEffectsTester : MonoBehaviour, IHandEffectsTrigger
{
	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001434 RID: 5172 RVA: 0x000743A1 File Offset: 0x000725A1
	public bool Static
	{
		get
		{
			return this.isStatic;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001435 RID: 5173 RVA: 0x000743A9 File Offset: 0x000725A9
	Transform IHandEffectsTrigger.Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x06001436 RID: 5174 RVA: 0x000743B1 File Offset: 0x000725B1
	VRRig IHandEffectsTrigger.Rig
	{
		get
		{
			return null;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x06001437 RID: 5175 RVA: 0x000743B4 File Offset: 0x000725B4
	IHandEffectsTrigger.Mode IHandEffectsTrigger.EffectMode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001438 RID: 5176 RVA: 0x000743BC File Offset: 0x000725BC
	bool IHandEffectsTrigger.FingersDown
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.FistBump || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06001439 RID: 5177 RVA: 0x000743D3 File Offset: 0x000725D3
	bool IHandEffectsTrigger.FingersUp
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.HighFive || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x0600143A RID: 5178 RVA: 0x000743E9 File Offset: 0x000725E9
	// (set) Token: 0x0600143B RID: 5179 RVA: 0x000743F1 File Offset: 0x000725F1
	public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600143C RID: 5180 RVA: 0x000743FA File Offset: 0x000725FA
	public bool RightHand { get; }

	// Token: 0x0600143D RID: 5181 RVA: 0x00074402 File Offset: 0x00072602
	private void Awake()
	{
		this.triggerZone = base.GetComponent<Collider>();
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x00074410 File Offset: 0x00072610
	private void OnEnable()
	{
		if (!HandEffectsTriggerRegistry.HasInstance)
		{
			HandEffectsTriggerRegistry.FindInstance();
		}
		HandEffectsTriggerRegistry.Instance.Register(this);
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x00074429 File Offset: 0x00072629
	private void OnDisable()
	{
		HandEffectsTriggerRegistry.Instance.Unregister(this);
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001440 RID: 5184 RVA: 0x00074436 File Offset: 0x00072636
	Vector3 IHandEffectsTrigger.Velocity
	{
		get
		{
			if (this.mode == IHandEffectsTrigger.Mode.HighFive)
			{
				return Vector3.zero;
			}
			IHandEffectsTrigger.Mode mode = this.mode;
			return Vector3.zero;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06001441 RID: 5185 RVA: 0x00074454 File Offset: 0x00072654
	TagEffectPack IHandEffectsTrigger.CosmeticEffectPack
	{
		get
		{
			return this.cosmeticEffectPack;
		}
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x00002789 File Offset: 0x00000989
	public void OnTriggerEntered(IHandEffectsTrigger other)
	{
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x0007445C File Offset: 0x0007265C
	public bool InTriggerZone(IHandEffectsTrigger t)
	{
		if (!(base.transform.position - t.Transform.position).IsShorterThan(this.triggerZone.bounds.size))
		{
			return false;
		}
		RaycastHit raycastHit;
		switch (this.mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			return t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), ref raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.FistBump:
			return t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), ref raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.HighFive_And_FistBump:
			return (t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), ref raycastHit, this.triggerRadius)) || (t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), ref raycastHit, this.triggerRadius));
		}
		return this.triggerZone.Raycast(new Ray(t.Transform.position, this.triggerZone.bounds.center - t.Transform.position), ref raycastHit, this.triggerRadius);
	}

	// Token: 0x04001ED9 RID: 7897
	[SerializeField]
	private TagEffectPack cosmeticEffectPack;

	// Token: 0x04001EDA RID: 7898
	private Collider triggerZone;

	// Token: 0x04001EDB RID: 7899
	public IHandEffectsTrigger.Mode mode;

	// Token: 0x04001EDC RID: 7900
	[SerializeField]
	private float triggerRadius = 0.07f;

	// Token: 0x04001EDD RID: 7901
	[SerializeField]
	private bool isStatic = true;
}
