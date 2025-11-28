using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class RaceCheckpointManager : MonoBehaviour
{
	// Token: 0x06001248 RID: 4680 RVA: 0x00060020 File Offset: 0x0005E220
	private void Start()
	{
		this.visual = base.GetComponent<RaceVisual>();
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].Init(this, i);
		}
		this.OnRaceEnd();
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00060064 File Offset: 0x0005E264
	public void OnRaceStart()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(i == 0);
		}
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x00060098 File Offset: 0x0005E298
	public void OnRaceEnd()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(false);
		}
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x000600C6 File Offset: 0x0005E2C6
	public void OnCheckpointReached(int index, SoundBankPlayer checkpointSound)
	{
		this.checkpoints[index].SetIsCorrectCheckpoint(false);
		this.checkpoints[(index + 1) % this.checkpoints.Length].SetIsCorrectCheckpoint(true);
		this.visual.OnCheckpointPassed(index, checkpointSound);
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x000600FC File Offset: 0x0005E2FC
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpointIdx)
	{
		return checkpointIdx >= 0 && checkpointIdx < this.checkpoints.Length && player.IsPositionInRange(this.checkpoints[checkpointIdx].transform.position, 6f);
	}

	// Token: 0x040016E2 RID: 5858
	[SerializeField]
	private RaceCheckpoint[] checkpoints;

	// Token: 0x040016E3 RID: 5859
	private RaceVisual visual;
}
