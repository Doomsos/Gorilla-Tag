using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E32 RID: 3634
	[NetworkBehaviourWeaved(9)]
	public class ObstacleCourseManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06005AAD RID: 23213 RVA: 0x001D11EE File Offset: 0x001CF3EE
		// (set) Token: 0x06005AAE RID: 23214 RVA: 0x001D11F5 File Offset: 0x001CF3F5
		public static ObstacleCourseManager Instance { get; private set; }

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x06005AAF RID: 23215 RVA: 0x001D11FD File Offset: 0x001CF3FD
		// (set) Token: 0x06005AB0 RID: 23216 RVA: 0x001D1205 File Offset: 0x001CF405
		public bool TickRunning { get; set; }

		// Token: 0x06005AB1 RID: 23217 RVA: 0x001D120E File Offset: 0x001CF40E
		protected override void Awake()
		{
			base.Awake();
			ObstacleCourseManager.Instance = this;
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x001D121C File Offset: 0x001CF41C
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x06005AB3 RID: 23219 RVA: 0x001D1230 File Offset: 0x001CF430
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnEnable();
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06005AB4 RID: 23220 RVA: 0x001D1244 File Offset: 0x001CF444
		public void Tick()
		{
			foreach (ObstacleCourse obstacleCourse in this.allObstaclesCourses)
			{
				obstacleCourse.InvokeUpdate();
			}
		}

		// Token: 0x06005AB5 RID: 23221 RVA: 0x001D1294 File Offset: 0x001CF494
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			this.allObstaclesCourses.Clear();
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x06005AB6 RID: 23222 RVA: 0x001D12A7 File Offset: 0x001CF4A7
		// (set) Token: 0x06005AB7 RID: 23223 RVA: 0x001D12D1 File Offset: 0x001CF4D1
		[Networked]
		[NetworkedWeaved(0, 9)]
		public unsafe ObstacleCourseData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ObstacleCourseData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ObstacleCourseManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ObstacleCourseData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005AB8 RID: 23224 RVA: 0x001D12FC File Offset: 0x001CF4FC
		public override void WriteDataFusion()
		{
			this.Data = new ObstacleCourseData(this.allObstaclesCourses);
		}

		// Token: 0x06005AB9 RID: 23225 RVA: 0x001D1310 File Offset: 0x001CF510
		public override void ReadDataFusion()
		{
			for (int i = 0; i < this.Data.ObstacleCourseCount; i++)
			{
				int winnerActorNumber = this.Data.WinnerActorNumber[i];
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)this.Data.CurrentRaceState[i];
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(winnerActorNumber, raceState);
				}
			}
		}

		// Token: 0x06005ABA RID: 23226 RVA: 0x001D1390 File Offset: 0x001CF590
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allObstaclesCourses.Count);
			for (int i = 0; i < this.allObstaclesCourses.Count; i++)
			{
				stream.SendNext(this.allObstaclesCourses[i].winnerActorNumber);
				stream.SendNext(this.allObstaclesCourses[i].currentState);
			}
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x001D1410 File Offset: 0x001CF610
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int winnerActorNumber = (int)stream.ReceiveNext();
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)stream.ReceiveNext();
				if (this.allObstaclesCourses[i].currentState != raceState)
				{
					this.allObstaclesCourses[i].Deserialize(winnerActorNumber, raceState);
				}
			}
		}

		// Token: 0x06005ABD RID: 23229 RVA: 0x001D1495 File Offset: 0x001CF695
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06005ABE RID: 23230 RVA: 0x001D14AD File Offset: 0x001CF6AD
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x040067F4 RID: 26612
		public List<ObstacleCourse> allObstaclesCourses = new List<ObstacleCourse>();

		// Token: 0x040067F6 RID: 26614
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 9)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private ObstacleCourseData _Data;
	}
}
