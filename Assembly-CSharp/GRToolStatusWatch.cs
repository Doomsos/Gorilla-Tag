using System;
using System.Text;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000750 RID: 1872
public class GRToolStatusWatch : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06003057 RID: 12375 RVA: 0x001088A0 File Offset: 0x00106AA0
	public void OnEntityInit()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.UpdateVisuals();
		this.progression = this.gameEntity.manager.GetComponent<GhostReactorManager>().reactor.toolProgression;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.UpdateSnappedPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.RemoveSnappedPlayer));
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003059 RID: 12377 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x0600305A RID: 12378 RVA: 0x0010893C File Offset: 0x00106B3C
	public void UpdateSnappedPlayer()
	{
		this.currentPlayer = GRPlayer.Get(this.gameEntity.snappedByActorNumber);
		this.lastKills = -1;
		this.lastCredits = -1;
		this.lastJuice = -1;
		this.lastGrade = -1;
		if (this.currentPlayer == GRPlayer.GetLocal())
		{
			this.state = GRToolStatusWatch.WatchState.SnappedLocal;
		}
		else
		{
			this.state = GRToolStatusWatch.WatchState.SnappedRemote;
		}
		this.disabledText.text = "LEAVE ME ALONE!\n\nTHIS IS ONLY FOR MY OWNER!!!";
		this.UpdateVisuals();
	}

	// Token: 0x0600305B RID: 12379 RVA: 0x001089B3 File Offset: 0x00106BB3
	public void RemoveSnappedPlayer()
	{
		this.currentPlayer = null;
		this.state = GRToolStatusWatch.WatchState.Dropped;
		this.disabledText.text = "LOW POWER\n\nPUT ME ON";
		this.UpdateVisuals();
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x001089D9 File Offset: 0x00106BD9
	private void Update()
	{
		if (this.currentPlayer == null)
		{
			return;
		}
		this.UpdateVisuals();
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x001089F0 File Offset: 0x00106BF0
	private void UpdateVisuals()
	{
		bool flag = this.state == GRToolStatusWatch.WatchState.SnappedLocal || this.state == GRToolStatusWatch.WatchState.SnappedRemote;
		if (this.disabledVisuals.activeSelf == flag)
		{
			this.disabledVisuals.SetActive(!flag);
		}
		if (this.enabledVisuals.activeSelf != flag)
		{
			this.enabledVisuals.SetActive(flag);
		}
		if (this.state != GRToolStatusWatch.WatchState.SnappedLocal)
		{
			return;
		}
		if (this.visibleHP != this.currentPlayer.Hp / 100)
		{
			this.visibleHP = this.currentPlayer.Hp / 100;
			for (int i = 0; i < this.healthHearts.Length; i++)
			{
				if (this.healthHearts[i].activeSelf != i < this.visibleHP)
				{
					this.healthHearts[i].SetActive(i < this.visibleHP);
				}
			}
		}
		if (this.visibleShield != this.currentPlayer.ShieldHp / 100)
		{
			this.visibleShield = this.currentPlayer.ShieldHp / 100;
			if (this.shieldSymbol.activeSelf != this.visibleShield > 0)
			{
				this.shieldSymbol.SetActive(this.visibleShield > 0);
			}
		}
		this.gimbaledCompass.LookAt(this.homeBase, Vector3.up);
		int num = (int)this.currentPlayer.synchronizedSessionStats[5];
		int shiftCredits = this.currentPlayer.ShiftCredits;
		int numberOfResearchPoints = this.progression.GetNumberOfResearchPoints();
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(this.currentPlayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item1;
		int item2 = gradePointDetails.Item2;
		if (num == this.lastKills && shiftCredits == this.lastCredits && numberOfResearchPoints == this.lastJuice && item2 == this.lastGrade)
		{
			return;
		}
		this.sb.Clear();
		this.sb.Append(num);
		this.sb.Append("\n\n");
		this.sb.Append(numberOfResearchPoints);
		this.sb.Append("\n\n");
		this.sb.Append(shiftCredits);
		this.sb.Append("\n\n\n");
		this.sb.Append(GhostReactorProgression.GetTitleNameFromLevel(item).get_Chars(0));
		this.sb.Append(item2);
		this.statsText.text = this.sb.ToString();
		this.lastKills = num;
		this.lastCredits = shiftCredits;
		this.lastJuice = numberOfResearchPoints;
		this.lastGrade = item2;
	}

	// Token: 0x04003F64 RID: 16228
	public GameEntity gameEntity;

	// Token: 0x04003F65 RID: 16229
	private GRPlayer currentPlayer;

	// Token: 0x04003F66 RID: 16230
	private int visibleHP;

	// Token: 0x04003F67 RID: 16231
	private int visibleShield;

	// Token: 0x04003F68 RID: 16232
	public GameObject disabledVisuals;

	// Token: 0x04003F69 RID: 16233
	public GameObject enabledVisuals;

	// Token: 0x04003F6A RID: 16234
	public GameObject[] healthHearts;

	// Token: 0x04003F6B RID: 16235
	public GameObject shieldSymbol;

	// Token: 0x04003F6C RID: 16236
	public Vector3 homeBase;

	// Token: 0x04003F6D RID: 16237
	public Transform gimbaledCompass;

	// Token: 0x04003F6E RID: 16238
	public TextMeshPro statsText;

	// Token: 0x04003F6F RID: 16239
	public TextMeshPro disabledText;

	// Token: 0x04003F70 RID: 16240
	private int lastKills;

	// Token: 0x04003F71 RID: 16241
	private int lastCredits;

	// Token: 0x04003F72 RID: 16242
	private int lastJuice;

	// Token: 0x04003F73 RID: 16243
	private int lastGrade;

	// Token: 0x04003F74 RID: 16244
	private StringBuilder sb = new StringBuilder();

	// Token: 0x04003F75 RID: 16245
	private GRToolStatusWatch.WatchState state;

	// Token: 0x04003F76 RID: 16246
	private GRToolProgressionManager progression;

	// Token: 0x02000751 RID: 1873
	private enum WatchState
	{
		// Token: 0x04003F78 RID: 16248
		Dropped,
		// Token: 0x04003F79 RID: 16249
		SnappedLocal,
		// Token: 0x04003F7A RID: 16250
		SnappedRemote
	}
}
