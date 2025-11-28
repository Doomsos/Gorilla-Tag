using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class DoorSlidingOpenAudio : MonoBehaviour, IBuildValidation, ITickSystemTick
{
	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000A16 RID: 2582 RVA: 0x0003687E File Offset: 0x00034A7E
	// (set) Token: 0x06000A17 RID: 2583 RVA: 0x00036886 File Offset: 0x00034A86
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06000A18 RID: 2584 RVA: 0x0003688F File Offset: 0x00034A8F
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00036897 File Offset: 0x00034A97
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x000368A0 File Offset: 0x00034AA0
	public bool BuildValidationCheck()
	{
		if (this.button == null)
		{
			Debug.LogError("reference button missing for doorslidingopenaudio", base.gameObject);
			return false;
		}
		if (this.audioSource == null)
		{
			Debug.LogError("missing audio source on doorslidingopenaudio", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x000368F0 File Offset: 0x00034AF0
	void ITickSystemTick.Tick()
	{
		if (this.button.ghostLab.IsDoorMoving(this.button.forSingleDoor, this.button.buttonIndex))
		{
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				return;
			}
		}
		else if (this.audioSource.isPlaying)
		{
			this.audioSource.time = 0f;
			this.audioSource.GTStop();
		}
	}

	// Token: 0x04000C67 RID: 3175
	public GhostLabButton button;

	// Token: 0x04000C68 RID: 3176
	public AudioSource audioSource;
}
