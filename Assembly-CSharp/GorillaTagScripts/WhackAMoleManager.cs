using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DA7 RID: 3495
	public class WhackAMoleManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060055E5 RID: 21989 RVA: 0x001B04C2 File Offset: 0x001AE6C2
		private void Awake()
		{
			WhackAMoleManager.instance = this;
			this.allGames.Clear();
		}

		// Token: 0x060055E6 RID: 21990 RVA: 0x0001773D File Offset: 0x0001593D
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060055E7 RID: 21991 RVA: 0x00017746 File Offset: 0x00015946
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060055E8 RID: 21992 RVA: 0x001B04D8 File Offset: 0x001AE6D8
		public void SliceUpdate()
		{
			foreach (WhackAMole whackAMole in this.allGames)
			{
				whackAMole.InvokeUpdate();
			}
		}

		// Token: 0x060055E9 RID: 21993 RVA: 0x001B0528 File Offset: 0x001AE728
		private void OnDestroy()
		{
			WhackAMoleManager.instance = null;
		}

		// Token: 0x060055EA RID: 21994 RVA: 0x001B0530 File Offset: 0x001AE730
		public void Register(WhackAMole whackAMole)
		{
			this.allGames.Add(whackAMole);
		}

		// Token: 0x060055EB RID: 21995 RVA: 0x001B053F File Offset: 0x001AE73F
		public void Unregister(WhackAMole whackAMole)
		{
			this.allGames.Remove(whackAMole);
		}

		// Token: 0x04006310 RID: 25360
		public static WhackAMoleManager instance;

		// Token: 0x04006311 RID: 25361
		public HashSet<WhackAMole> allGames = new HashSet<WhackAMole>();
	}
}
