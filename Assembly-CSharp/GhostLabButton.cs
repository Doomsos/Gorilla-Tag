using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class GhostLabButton : GorillaPressableButton, IBuildValidation
{
	// Token: 0x06000A29 RID: 2601 RVA: 0x00036D44 File Offset: 0x00034F44
	public bool BuildValidationCheck()
	{
		if (this.ghostLab == null)
		{
			Debug.LogError("ghostlab is missing", this);
			return false;
		}
		return true;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00036D62 File Offset: 0x00034F62
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.ghostLab.DoorButtonPress(this.buttonIndex, this.forSingleDoor);
	}

	// Token: 0x04000C78 RID: 3192
	public GhostLab ghostLab;

	// Token: 0x04000C79 RID: 3193
	public int buttonIndex;

	// Token: 0x04000C7A RID: 3194
	public bool forSingleDoor;
}
