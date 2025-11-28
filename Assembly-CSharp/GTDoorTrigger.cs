using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x020002CC RID: 716
public class GTDoorTrigger : MonoBehaviour
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060011AB RID: 4523 RVA: 0x0005D415 File Offset: 0x0005B615
	public int overlapCount
	{
		get
		{
			return this.overlappingColliders.Count;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060011AC RID: 4524 RVA: 0x0005D422 File Offset: 0x0005B622
	public bool TriggeredThisFrame
	{
		get
		{
			return this.lastTriggeredFrame == Time.frameCount;
		}
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0005D434 File Offset: 0x0005B634
	public void ValidateOverlappingColliders()
	{
		for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
		{
			if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
			{
				this.overlappingColliders.RemoveAt(i);
			}
		}
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0005D4A4 File Offset: 0x0005B6A4
	private void OnTriggerEnter(Collider other)
	{
		if (!this.overlappingColliders.Contains(other))
		{
			this.overlappingColliders.Add(other);
		}
		this.lastTriggeredFrame = Time.frameCount;
		this.TriggeredEvent.Invoke();
		if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
		{
			this.timeline.Play();
		}
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0005D528 File Offset: 0x0005B728
	private void OnTriggerExit(Collider other)
	{
		this.overlappingColliders.Remove(other);
	}

	// Token: 0x0400162A RID: 5674
	[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
	public PlayableDirector timeline;

	// Token: 0x0400162B RID: 5675
	private int lastTriggeredFrame = -1;

	// Token: 0x0400162C RID: 5676
	private List<Collider> overlappingColliders = new List<Collider>(20);

	// Token: 0x0400162D RID: 5677
	internal UnityEvent TriggeredEvent = new UnityEvent();
}
