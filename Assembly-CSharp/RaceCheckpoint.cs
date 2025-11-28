using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class RaceCheckpoint : MonoBehaviour
{
	// Token: 0x06001244 RID: 4676 RVA: 0x0005FF9F File Offset: 0x0005E19F
	public void Init(RaceCheckpointManager manager, int index)
	{
		this.manager = manager;
		this.checkpointIndex = index;
		this.SetIsCorrectCheckpoint(index == 0);
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0005FFB9 File Offset: 0x0005E1B9
	public void SetIsCorrectCheckpoint(bool isCorrect)
	{
		this.isCorrect = isCorrect;
		this.banner.sharedMaterial = (isCorrect ? this.activeCheckpointMat : this.wrongCheckpointMat);
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x0005FFDE File Offset: 0x0005E1DE
	private void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		if (this.isCorrect)
		{
			this.manager.OnCheckpointReached(this.checkpointIndex, this.checkpointSound);
			return;
		}
		this.wrongCheckpointSound.Play();
	}

	// Token: 0x040016DA RID: 5850
	[SerializeField]
	private MeshRenderer banner;

	// Token: 0x040016DB RID: 5851
	[SerializeField]
	private Material activeCheckpointMat;

	// Token: 0x040016DC RID: 5852
	[SerializeField]
	private Material wrongCheckpointMat;

	// Token: 0x040016DD RID: 5853
	[SerializeField]
	private SoundBankPlayer checkpointSound;

	// Token: 0x040016DE RID: 5854
	[SerializeField]
	private SoundBankPlayer wrongCheckpointSound;

	// Token: 0x040016DF RID: 5855
	private RaceCheckpointManager manager;

	// Token: 0x040016E0 RID: 5856
	private int checkpointIndex;

	// Token: 0x040016E1 RID: 5857
	private bool isCorrect;
}
