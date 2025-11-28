using System;
using UnityEngine;

// Token: 0x02000C1F RID: 3103
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x06004C53 RID: 19539 RVA: 0x0018D1C5 File Offset: 0x0018B3C5
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x06004C54 RID: 19540 RVA: 0x0018D1D3 File Offset: 0x0018B3D3
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x06004C55 RID: 19541 RVA: 0x0018D1DC File Offset: 0x0018B3DC
	private void Update()
	{
		if (this.timer >= this.timerTarget)
		{
			int num = Random.Range(0, this.audioSources.Length);
			int num2 = Random.Range(0, this.audioClips.Length);
			this.audioSources[num].clip = this.audioClips[num2];
			this.audioSources[num].GTPlay();
			this.SetTarget();
			return;
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x06004C56 RID: 19542 RVA: 0x0018D250 File Offset: 0x0018B450
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x04005C41 RID: 23617
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x04005C42 RID: 23618
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04005C43 RID: 23619
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x04005C44 RID: 23620
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x04005C45 RID: 23621
	private float timer;

	// Token: 0x04005C46 RID: 23622
	private float timerTarget;
}
