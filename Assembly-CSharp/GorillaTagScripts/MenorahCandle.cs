using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DFA RID: 3578
	public class MenorahCandle : MonoBehaviourPun
	{
		// Token: 0x06005949 RID: 22857 RVA: 0x00002789 File Offset: 0x00000989
		private void Awake()
		{
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x001C9170 File Offset: 0x001C7370
		private void Start()
		{
			this.EnableCandle(false);
			this.EnableFlame(false);
			this.litDate = new DateTime(this.year, this.month, this.day);
			this.currentDate = DateTime.Now;
			this.EnableCandle(this.CandleShouldBeVisible());
			this.EnableFlame(false);
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x001C91EE File Offset: 0x001C73EE
		private void UpdateMenorah()
		{
			this.EnableCandle(this.CandleShouldBeVisible());
			if (this.ShouldLightCandle())
			{
				this.EnableFlame(true);
				return;
			}
			if (this.ShouldSnuffCandle())
			{
				this.EnableFlame(false);
			}
		}

		// Token: 0x0600594C RID: 22860 RVA: 0x001C921B File Offset: 0x001C741B
		private void OnTimeChanged()
		{
			this.currentDate = GorillaComputer.instance.GetServerTime();
			this.UpdateMenorah();
		}

		// Token: 0x0600594D RID: 22861 RVA: 0x001C9235 File Offset: 0x001C7435
		public void OnTimeEventStart()
		{
			this.activeTimeEventDay = true;
			this.UpdateMenorah();
		}

		// Token: 0x0600594E RID: 22862 RVA: 0x001C9244 File Offset: 0x001C7444
		public void OnTimeEventEnd()
		{
			this.activeTimeEventDay = false;
			this.UpdateMenorah();
		}

		// Token: 0x0600594F RID: 22863 RVA: 0x001C9253 File Offset: 0x001C7453
		private void EnableCandle(bool enable)
		{
			if (this.candle)
			{
				this.candle.SetActive(enable);
			}
		}

		// Token: 0x06005950 RID: 22864 RVA: 0x001C926E File Offset: 0x001C746E
		private bool CandleShouldBeVisible()
		{
			return this.currentDate >= this.litDate;
		}

		// Token: 0x06005951 RID: 22865 RVA: 0x001C9281 File Offset: 0x001C7481
		private void EnableFlame(bool enable)
		{
			if (this.flame)
			{
				this.flame.SetActive(enable);
			}
		}

		// Token: 0x06005952 RID: 22866 RVA: 0x001C929C File Offset: 0x001C749C
		private bool ShouldLightCandle()
		{
			return !this.activeTimeEventDay && this.CandleShouldBeVisible() && !this.flame.activeSelf;
		}

		// Token: 0x06005953 RID: 22867 RVA: 0x001C92BE File Offset: 0x001C74BE
		private bool ShouldSnuffCandle()
		{
			return this.activeTimeEventDay && this.flame.activeSelf;
		}

		// Token: 0x06005954 RID: 22868 RVA: 0x001C92D5 File Offset: 0x001C74D5
		private void OnDestroy()
		{
			if (GorillaComputer.instance)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x04006677 RID: 26231
		public int day;

		// Token: 0x04006678 RID: 26232
		public int month;

		// Token: 0x04006679 RID: 26233
		public int year;

		// Token: 0x0400667A RID: 26234
		public GameObject flame;

		// Token: 0x0400667B RID: 26235
		public GameObject candle;

		// Token: 0x0400667C RID: 26236
		private DateTime litDate;

		// Token: 0x0400667D RID: 26237
		private bool activeTimeEventDay;

		// Token: 0x0400667E RID: 26238
		private DateTime currentDate;
	}
}
