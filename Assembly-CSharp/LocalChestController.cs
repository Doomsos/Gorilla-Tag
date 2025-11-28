using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020002DD RID: 733
public class LocalChestController : MonoBehaviour
{
	// Token: 0x060011F2 RID: 4594 RVA: 0x0005E8D8 File Offset: 0x0005CAD8
	private void OnTriggerEnter(Collider other)
	{
		if (this.isOpen)
		{
			return;
		}
		TransformFollow component = other.GetComponent<TransformFollow>();
		if (component == null)
		{
			return;
		}
		Transform transformToFollow = component.transformToFollow;
		if (transformToFollow == null)
		{
			return;
		}
		VRRig componentInParent = transformToFollow.GetComponentInParent<VRRig>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.playerCollectionVolume != null && !this.playerCollectionVolume.containedRigs.Contains(componentInParent))
		{
			return;
		}
		this.isOpen = true;
		this.director.Play();
	}

	// Token: 0x04001699 RID: 5785
	public PlayableDirector director;

	// Token: 0x0400169A RID: 5786
	public MazePlayerCollection playerCollectionVolume;

	// Token: 0x0400169B RID: 5787
	private bool isOpen;
}
