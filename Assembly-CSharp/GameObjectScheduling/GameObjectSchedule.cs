using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02001156 RID: 4438
	[CreateAssetMenu(fileName = "New Game Object Schedule", menuName = "Game Object Scheduling/Game Object Schedule", order = 0)]
	public class GameObjectSchedule : ScriptableObject
	{
		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06006FFC RID: 28668 RVA: 0x00247046 File Offset: 0x00245246
		public GameObjectSchedule.GameObjectScheduleNode[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06006FFD RID: 28669 RVA: 0x0024704E File Offset: 0x0024524E
		public bool InitialState
		{
			get
			{
				return this.initialState;
			}
		}

		// Token: 0x06006FFE RID: 28670 RVA: 0x00247058 File Offset: 0x00245258
		public int GetCurrentNodeIndex(DateTime currentDate, int startFrom = 0)
		{
			if (startFrom >= this.nodes.Length)
			{
				return int.MaxValue;
			}
			for (int i = -1; i < this.nodes.Length - 1; i++)
			{
				if (currentDate < this.nodes[i + 1].DateTime)
				{
					return i;
				}
			}
			return int.MaxValue;
		}

		// Token: 0x06006FFF RID: 28671 RVA: 0x002470A9 File Offset: 0x002452A9
		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this._validate();
			this.validated = true;
		}

		// Token: 0x06007000 RID: 28672 RVA: 0x002470C4 File Offset: 0x002452C4
		private void _validate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Validate();
			}
			List<GameObjectSchedule.GameObjectScheduleNode> list = new List<GameObjectSchedule.GameObjectScheduleNode>(this.nodes);
			list.Sort((GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2) => e1.DateTime.CompareTo(e2.DateTime));
			this.nodes = list.ToArray();
		}

		// Token: 0x06007001 RID: 28673 RVA: 0x00247130 File Offset: 0x00245330
		public static void GenerateDailyShuffle(DateTime startDate, DateTime endDate, GameObjectSchedule[] schedules)
		{
			TimeSpan timeSpan = TimeSpan.FromDays(1.0);
			int num = schedules.Length - 1;
			int num2 = schedules.Length - 2;
			DateTime dateTime = startDate;
			List<GameObjectSchedule.GameObjectScheduleNode>[] array = new List<GameObjectSchedule.GameObjectScheduleNode>[schedules.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<GameObjectSchedule.GameObjectScheduleNode>();
			}
			while (dateTime < endDate)
			{
				int num3 = Random.Range(0, schedules.Length - 2);
				if (num <= num3)
				{
					num3++;
					if (num2 <= num3)
					{
						num3++;
					}
				}
				else if (num2 <= num3)
				{
					num3++;
					if (num <= num3)
					{
						num3++;
					}
				}
				array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = false
				});
				array[num3].Add(new GameObjectSchedule.GameObjectScheduleNode
				{
					activeDateTime = dateTime.ToString(),
					activeState = true
				});
				dateTime += timeSpan;
				num2 = num;
				num = num3;
			}
			array[num].Add(new GameObjectSchedule.GameObjectScheduleNode
			{
				activeDateTime = dateTime.ToString(),
				activeState = false
			});
			for (int j = 0; j < array.Length; j++)
			{
				schedules[j].nodes = array[j].ToArray();
			}
		}

		// Token: 0x0400805A RID: 32858
		[SerializeField]
		private bool initialState;

		// Token: 0x0400805B RID: 32859
		[SerializeField]
		private GameObjectSchedule.GameObjectScheduleNode[] nodes;

		// Token: 0x0400805C RID: 32860
		[SerializeField]
		private SchedulingOptions options;

		// Token: 0x0400805D RID: 32861
		private bool validated;

		// Token: 0x02001157 RID: 4439
		[Serializable]
		public class GameObjectScheduleNode
		{
			// Token: 0x17000A79 RID: 2681
			// (get) Token: 0x06007003 RID: 28675 RVA: 0x00247267 File Offset: 0x00245467
			public bool ActiveState
			{
				get
				{
					return this.activeState;
				}
			}

			// Token: 0x17000A7A RID: 2682
			// (get) Token: 0x06007004 RID: 28676 RVA: 0x0024726F File Offset: 0x0024546F
			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
			}

			// Token: 0x06007005 RID: 28677 RVA: 0x00247278 File Offset: 0x00245478
			public void Validate()
			{
				try
				{
					this.dateTime = DateTime.Parse(this.activeDateTime, CultureInfo.InvariantCulture);
				}
				catch
				{
					this.dateTime = DateTime.MinValue;
				}
			}

			// Token: 0x0400805E RID: 32862
			[SerializeField]
			public string activeDateTime = "1/1/0001 00:00:00";

			// Token: 0x0400805F RID: 32863
			[SerializeField]
			[Tooltip("Check to turn on. Uncheck to turn off.")]
			public bool activeState = true;

			// Token: 0x04008060 RID: 32864
			private DateTime dateTime;
		}
	}
}
