using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class GRBay : MonoBehaviour
{
	// Token: 0x06002ACB RID: 10955 RVA: 0x000E63C6 File Offset: 0x000E45C6
	private void Awake()
	{
		if (this.playerName != null)
		{
			this.playerName.text = null;
		}
		if (this.maxDropText != null)
		{
			this.maxDropText.text = null;
		}
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x000E63FC File Offset: 0x000E45FC
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
		if (this.shuttleLoc != GRShuttleGroupLoc.Invalid && this.shuttleIndex >= 0 && this.shuttleIndex < 10)
		{
			this.unlockShuttle = GRElevatorManager._instance.GetPlayerShuttle(this.shuttleLoc, this.shuttleIndex);
			if (this.unlockShuttle != null)
			{
				this.unlockShuttle.SetBay(this);
			}
		}
		this.Refresh();
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x000E6468 File Offset: 0x000E4668
	public void SetOpen(bool open)
	{
		if (this.hideWhenOpen != null)
		{
			for (int i = 0; i < this.hideWhenOpen.Count; i++)
			{
				if (this.hideWhenOpen[i] != null)
				{
					this.hideWhenOpen[i].SetActive(!open);
				}
				else
				{
					Debug.LogErrorFormat("Why is hideWhenOpen null {0} at {1}", new object[]
					{
						base.gameObject.name,
						i
					});
				}
			}
		}
		else
		{
			Debug.LogErrorFormat("Why is hideWhenOpen null {0}", new object[]
			{
				base.gameObject.name
			});
		}
		if (this.hideWhenClosed != null)
		{
			for (int j = 0; j < this.hideWhenClosed.Count; j++)
			{
				if (this.hideWhenClosed[j] != null)
				{
					this.hideWhenClosed[j].SetActive(open);
				}
				else
				{
					Debug.LogErrorFormat("Why is hideWhenClosed null {0} at {1} ", new object[]
					{
						base.gameObject.name,
						j
					});
				}
			}
		}
		else
		{
			Debug.LogErrorFormat("Why is hideWhenClosed null {0}", new object[]
			{
				base.gameObject.name
			});
		}
		if (this.bayDoorAnimation != null && this.isOpen != open)
		{
			if (open)
			{
				this.bayDoorAnimation.Play("BayDoor_Open");
				this.bayDoorAnimation.PlayQueued("BayDoor_Open_Idle");
			}
			else
			{
				this.bayDoorAnimation.Play("BayDoor_Close");
				this.bayDoorAnimation.PlayQueued("BayDoor_Close_Idle");
			}
		}
		this.isOpen = open;
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x000E65FC File Offset: 0x000E47FC
	public void Refresh()
	{
		bool open = true;
		if (this.unlockShuttle != null)
		{
			NetPlayer owner = this.unlockShuttle.GetOwner();
			bool flag = owner != null && this.unlockShuttle.IsPodUnlocked();
			open = (this.unlockShuttle.GetState() == GRShuttleState.Docked && flag);
			if (this.playerName != null)
			{
				this.playerName.text = ((!flag) ? null : owner.SanitizedNickName);
			}
			if (this.maxDropText != null)
			{
				int num = this.unlockShuttle.GetMaxDropFloor() + 1;
				this.maxDropText.text = ((!flag) ? null : num.ToString());
			}
			for (int i = 0; i < this.showWhenOwned.Count; i++)
			{
				this.showWhenOwned[i].SetActive(flag);
			}
			for (int j = 0; j < this.showWhenNotOwned.Count; j++)
			{
				this.showWhenNotOwned[j].SetActive(!flag);
			}
		}
		else if (this.unlockByDrillLevel > 0)
		{
			open = ((this.reactor != null && this.reactor.GetDepthLevel() >= this.unlockByDrillLevel) || GhostReactorManager.bayUnlockEnabled);
		}
		this.SetOpen(open);
	}

	// Token: 0x04003739 RID: 14137
	public List<GameObject> hideWhenOpen;

	// Token: 0x0400373A RID: 14138
	public List<GameObject> hideWhenClosed;

	// Token: 0x0400373B RID: 14139
	public Animation bayDoorAnimation;

	// Token: 0x0400373C RID: 14140
	private bool isOpen;

	// Token: 0x0400373D RID: 14141
	public TMP_Text playerName;

	// Token: 0x0400373E RID: 14142
	public TMP_Text maxDropText;

	// Token: 0x0400373F RID: 14143
	public List<GameObject> showWhenOwned;

	// Token: 0x04003740 RID: 14144
	public List<GameObject> showWhenNotOwned;

	// Token: 0x04003741 RID: 14145
	public int unlockByDrillLevel = -1;

	// Token: 0x04003742 RID: 14146
	public GRShuttleGroupLoc shuttleLoc = GRShuttleGroupLoc.Invalid;

	// Token: 0x04003743 RID: 14147
	public int shuttleIndex = -1;

	// Token: 0x04003744 RID: 14148
	[NonSerialized]
	public bool debugForceUnlockedByLevel;

	// Token: 0x04003745 RID: 14149
	private GRShuttle unlockShuttle;

	// Token: 0x04003746 RID: 14150
	private GhostReactor reactor;
}
