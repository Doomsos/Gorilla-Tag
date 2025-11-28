using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002E RID: 46
[RequireComponent(typeof(AudioSource))]
public class PlayAudioSourceDelay : MonoBehaviour
{
	// Token: 0x060000A6 RID: 166 RVA: 0x000051D9 File Offset: 0x000033D9
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		base.GetComponent<AudioSource>().GTPlay();
		yield break;
	}

	// Token: 0x040000C3 RID: 195
	[SerializeField]
	private float _delay;
}
