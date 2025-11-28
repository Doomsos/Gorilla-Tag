using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class ElfLauncherElf : MonoBehaviour
{
	// Token: 0x06000F73 RID: 3955 RVA: 0x00052160 File Offset: 0x00050360
	private void OnEnable()
	{
		base.StartCoroutine(this.ReturnToPoolAfterDelayCo());
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x0005216F File Offset: 0x0005036F
	private IEnumerator ReturnToPoolAfterDelayCo()
	{
		yield return new WaitForSeconds(this.destroyAfterDuration);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0005217E File Offset: 0x0005037E
	private void OnCollisionEnter(Collision collision)
	{
		if (this.bounceAudioCoolingDownUntilTimestamp > Time.time)
		{
			return;
		}
		this.bounceAudio.Play();
		this.bounceAudioCoolingDownUntilTimestamp = Time.time + this.bounceAudioCooldownDuration;
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x000521AB File Offset: 0x000503AB
	private void FixedUpdate()
	{
		this.rb.AddForce(base.transform.lossyScale.x * Physics.gravity * this.rb.mass, 0);
	}

	// Token: 0x04001314 RID: 4884
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04001315 RID: 4885
	[SerializeField]
	private SoundBankPlayer bounceAudio;

	// Token: 0x04001316 RID: 4886
	[SerializeField]
	private float bounceAudioCooldownDuration;

	// Token: 0x04001317 RID: 4887
	[SerializeField]
	private float destroyAfterDuration;

	// Token: 0x04001318 RID: 4888
	private float bounceAudioCoolingDownUntilTimestamp;
}
