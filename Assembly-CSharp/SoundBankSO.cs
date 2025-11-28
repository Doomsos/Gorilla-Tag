using System;
using UnityEngine;

// Token: 0x02000C75 RID: 3189
[CreateAssetMenu(menuName = "Gorilla Tag/SoundBankSO")]
public class SoundBankSO : ScriptableObject
{
	// Token: 0x04005D18 RID: 23832
	public AudioClip[] sounds;

	// Token: 0x04005D19 RID: 23833
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x04005D1A RID: 23834
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
