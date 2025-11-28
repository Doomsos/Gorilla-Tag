using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020008DD RID: 2269
public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	// Token: 0x1700055B RID: 1371
	// (get) Token: 0x06003A28 RID: 14888 RVA: 0x00133871 File Offset: 0x00131A71
	// (set) Token: 0x06003A29 RID: 14889 RVA: 0x00133879 File Offset: 0x00131A79
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x06003A2A RID: 14890 RVA: 0x00133884 File Offset: 0x00131A84
	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x001338D3 File Offset: 0x00131AD3
	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x001338F3 File Offset: 0x00131AF3
	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x0400496B RID: 18795
	public ScienceExperimentElementID elementID;

	// Token: 0x0400496C RID: 18796
	private Transform followElement;
}
