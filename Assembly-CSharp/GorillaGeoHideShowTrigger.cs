using System;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using UnityEngine;

// Token: 0x0200077F RID: 1919
public class GorillaGeoHideShowTrigger : GorillaTriggerBox, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x06003224 RID: 12836 RVA: 0x0010EB19 File Offset: 0x0010CD19
	protected void Awake()
	{
		((IGuidedRefObject)this).GuidedRefInitialize();
	}

	// Token: 0x06003225 RID: 12837 RVA: 0x0010EB24 File Offset: 0x0010CD24
	public override void OnBoxTriggered()
	{
		if (!this._guidedRefsAreFullyResolved)
		{
			return;
		}
		if (this.makeSureThisIsDisabled != null)
		{
			foreach (GameObject gameObject in this.makeSureThisIsDisabled)
			{
				if (gameObject == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject.SetActive(false);
			}
		}
		if (this.makeSureThisIsEnabled != null)
		{
			foreach (GameObject gameObject2 in this.makeSureThisIsEnabled)
			{
				if (gameObject2 == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject2.SetActive(true);
			}
		}
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x0010EBDE File Offset: 0x0010CDDE
	void IGuidedRefObject.GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverArray<GorillaGeoHideShowTrigger, GameObject>(this, "makeSureThisIsDisabled", ref this.makeSureThisIsDisabled, ref this.makeSureThisIsDisabled_gRefs);
		GuidedRefHub.RegisterReceiverArray<GorillaGeoHideShowTrigger, GameObject>(this, "makeSureThisIsEnabled", ref this.makeSureThisIsEnabled, ref this.makeSureThisIsEnabled_gRefs);
		GuidedRefHub.ReceiverFullyRegistered<GorillaGeoHideShowTrigger>(this);
	}

	// Token: 0x06003227 RID: 12839 RVA: 0x0010EC14 File Offset: 0x0010CE14
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		return GuidedRefHub.TryResolveArrayItem<GorillaGeoHideShowTrigger, GameObject>(this, this.makeSureThisIsDisabled, this.makeSureThisIsDisabled_gRefs, target) || GuidedRefHub.TryResolveArrayItem<GorillaGeoHideShowTrigger, GameObject>(this, this.makeSureThisIsDisabled, this.makeSureThisIsEnabled_gRefs, target);
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x0010EC45 File Offset: 0x0010CE45
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
		this._guidedRefsAreFullyResolved = true;
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x0010EC4E File Offset: 0x0010CE4E
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
		this._guidedRefsAreFullyResolved = false;
	}

	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x0600322A RID: 12842 RVA: 0x0010EC57 File Offset: 0x0010CE57
	// (set) Token: 0x0600322B RID: 12843 RVA: 0x0010EC5F File Offset: 0x0010CE5F
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x0600322D RID: 12845 RVA: 0x000743A9 File Offset: 0x000725A9
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x000178ED File Offset: 0x00015AED
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x0400409B RID: 16539
	[SerializeField]
	private GameObject[] makeSureThisIsDisabled;

	// Token: 0x0400409C RID: 16540
	[SerializeField]
	private GuidedRefReceiverArrayInfo makeSureThisIsDisabled_gRefs = new GuidedRefReceiverArrayInfo(false);

	// Token: 0x0400409D RID: 16541
	[SerializeField]
	private GameObject[] makeSureThisIsEnabled;

	// Token: 0x0400409E RID: 16542
	[SerializeField]
	private GuidedRefReceiverArrayInfo makeSureThisIsEnabled_gRefs = new GuidedRefReceiverArrayInfo(false);

	// Token: 0x0400409F RID: 16543
	private bool _guidedRefsAreFullyResolved;
}
