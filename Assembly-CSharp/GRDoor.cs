using System;
using UnityEngine;

// Token: 0x02000705 RID: 1797
[Serializable]
public class GRDoor
{
	// Token: 0x06002E14 RID: 11796 RVA: 0x000FA9BB File Offset: 0x000F8BBB
	public void Setup()
	{
		this.doorState = GRDoor.DoorState.Closed;
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x000FA9C4 File Offset: 0x000F8BC4
	public void SetDoorState(GRDoor.DoorState newState)
	{
		if (newState == this.doorState)
		{
			return;
		}
		this.doorState = newState;
		if (this.doorState == GRDoor.DoorState.Closed)
		{
			this.animation.clip = this.closeAnim;
			this.animation.Play();
			this.closeDoorSound.Play(null);
			return;
		}
		this.animation.clip = this.openAnim;
		this.animation.Play();
		this.openDoorSound.Play(null);
	}

	// Token: 0x04003C35 RID: 15413
	public GRDoor.DoorState doorState;

	// Token: 0x04003C36 RID: 15414
	public Animation animation;

	// Token: 0x04003C37 RID: 15415
	public AnimationClip openAnim;

	// Token: 0x04003C38 RID: 15416
	public AnimationClip closeAnim;

	// Token: 0x04003C39 RID: 15417
	public AbilitySound openDoorSound;

	// Token: 0x04003C3A RID: 15418
	public AbilitySound closeDoorSound;

	// Token: 0x02000706 RID: 1798
	public enum DoorState
	{
		// Token: 0x04003C3C RID: 15420
		Closed,
		// Token: 0x04003C3D RID: 15421
		Open
	}
}
