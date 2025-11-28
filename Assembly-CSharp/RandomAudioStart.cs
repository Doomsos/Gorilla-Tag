using System;
using UnityEngine;

// Token: 0x020004B9 RID: 1209
public class RandomAudioStart : MonoBehaviour, IBuildValidation
{
	// Token: 0x06001F30 RID: 7984 RVA: 0x000A56CD File Offset: 0x000A38CD
	public bool BuildValidationCheck()
	{
		if (this.audioSource == null)
		{
			Debug.LogError("audio source is missing for RandomAudioStart, it won't work correctly", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x000A56F0 File Offset: 0x000A38F0
	private void OnEnable()
	{
		this.audioSource.time = Random.value * this.audioSource.clip.length;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000A5713 File Offset: 0x000A3913
	[ContextMenu("Assign Audio Source")]
	public void AssignAudioSource()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x04002981 RID: 10625
	public AudioSource audioSource;
}
