using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EF6 RID: 3830
	public class GhostReactorProgression : MonoBehaviour
	{
		// Token: 0x06006024 RID: 24612 RVA: 0x001F0272 File Offset: 0x001EE472
		public void Awake()
		{
			GhostReactorProgression.instance = this;
		}

		// Token: 0x06006025 RID: 24613 RVA: 0x001F027C File Offset: 0x001EE47C
		public void Start()
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnTrackRead += new Action<string, int>(this.OnTrackRead);
				ProgressionManager.Instance.OnTrackSet += new Action<string, int>(this.OnTrackSet);
				ProgressionManager.Instance.OnNodeUnlocked += delegate(string a, string b)
				{
					this.OnNodeUnlocked();
				};
				return;
			}
			Debug.Log("GRP: ProgressionManager is null!");
		}

		// Token: 0x06006026 RID: 24614 RVA: 0x001F02E4 File Offset: 0x001EE4E4
		public void GetStartingProgression(GRPlayer grPlayer)
		{
			GhostReactorProgression.<GetStartingProgression>d__6 <GetStartingProgression>d__;
			<GetStartingProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GetStartingProgression>d__.<>4__this = this;
			<GetStartingProgression>d__.grPlayer = grPlayer;
			<GetStartingProgression>d__.<>1__state = -1;
			<GetStartingProgression>d__.<>t__builder.Start<GhostReactorProgression.<GetStartingProgression>d__6>(ref <GetStartingProgression>d__);
		}

		// Token: 0x06006027 RID: 24615 RVA: 0x001F0323 File Offset: 0x001EE523
		public void SetProgression(int progressionAmountToAdd, GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
			ProgressionManager.Instance.SetProgression(this.progressionTrackId, progressionAmountToAdd);
		}

		// Token: 0x06006028 RID: 24616 RVA: 0x001F033D File Offset: 0x001EE53D
		public void UnlockProgressionTreeNode(string treeId, string nodeId, GhostReactor reactor)
		{
			this._reactor = reactor;
			ProgressionManager.Instance.UnlockNode(treeId, nodeId);
		}

		// Token: 0x06006029 RID: 24617 RVA: 0x001F0354 File Offset: 0x001EE554
		private void OnTrackRead(string trackId, int progress)
		{
			if (this._grPlayer == null)
			{
				Debug.Log("GRP: OnTrackRead Failure: player is null");
				return;
			}
			if (trackId != this.progressionTrackId)
			{
				Debug.Log(string.Format("GRP: OnTrackRead Failure: track [{0}] progressionTrack [{1}] progress {2}", trackId, this.progressionTrackId, progress));
				return;
			}
			this._grPlayer.SetProgressionData(progress, progress, false);
		}

		// Token: 0x0600602A RID: 24618 RVA: 0x001F03B3 File Offset: 0x001EE5B3
		private void OnTrackSet(string trackId, int progress)
		{
			if (this._grPlayer == null)
			{
				return;
			}
			if (trackId != this.progressionTrackId)
			{
				return;
			}
			this._grPlayer.SetProgressionData(progress, this._grPlayer.CurrentProgression.redeemedPoints, false);
		}

		// Token: 0x0600602B RID: 24619 RVA: 0x001F03F0 File Offset: 0x001EE5F0
		private void OnNodeUnlocked()
		{
			if (this._reactor != null && this._reactor.toolProgression != null)
			{
				this._reactor.toolProgression.UpdateInventory();
				this._reactor.toolProgression.SetPendingTreeToProcess();
				this._reactor.UpdateLocalPlayerFromProgression();
			}
		}

		// Token: 0x0600602C RID: 24620 RVA: 0x001F044C File Offset: 0x001EE64C
		[return: TupleElementNames(new string[]
		{
			"tier",
			"grade",
			"totalPointsToNextLevel",
			"partialPointsToNextLevel"
		})]
		public static ValueTuple<int, int, int, int> GetGradePointDetails(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num2 = num;
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					break;
				}
			}
			if (points > num)
			{
				return new ValueTuple<int, int, int, int>(i - 1, 0, 0, 0);
			}
			int pointsPerGrade = GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
			int num3 = (points - num2) / pointsPerGrade;
			int num4 = (points - num2) % pointsPerGrade;
			return new ValueTuple<int, int, int, int>(i, num3, pointsPerGrade, num4);
		}

		// Token: 0x0600602D RID: 24621 RVA: 0x001F04F4 File Offset: 0x001EE6F4
		public static string GetTitleNameAndGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName + " " + (GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1).ToString();
				}
			}
			return "null";
		}

		// Token: 0x0600602E RID: 24622 RVA: 0x001F05C0 File Offset: 0x001EE7C0
		public static string GetTitleName(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x0600602F RID: 24623 RVA: 0x001F063C File Offset: 0x001EE83C
		public static string GetTitleNameFromLevel(int level)
		{
			GhostReactorProgression.LoadGRPSO();
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				if (GhostReactorProgression.grPSO.progressionData[i].tierId >= level)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x06006030 RID: 24624 RVA: 0x001F069C File Offset: 0x001EE89C
		public static int GetGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1;
				}
			}
			return -1;
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x001F0738 File Offset: 0x001EE938
		public static int GetTitleLevel(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierId;
				}
			}
			return -1;
		}

		// Token: 0x06006032 RID: 24626 RVA: 0x001F07AF File Offset: 0x001EE9AF
		public static void LoadGRPSO()
		{
			if (GhostReactorProgression.grPSO == null)
			{
				GhostReactorProgression.grPSO = Resources.Load<GRProgressionScriptableObject>("ProgressionTiersData");
			}
		}

		// Token: 0x04006ED5 RID: 28373
		public static GhostReactorProgression instance;

		// Token: 0x04006ED6 RID: 28374
		private string progressionTrackId = "a0208736-e696-489b-81cd-c0c772489cc5";

		// Token: 0x04006ED7 RID: 28375
		private GRPlayer _grPlayer;

		// Token: 0x04006ED8 RID: 28376
		private GhostReactor _reactor;

		// Token: 0x04006ED9 RID: 28377
		public static GRProgressionScriptableObject grPSO;

		// Token: 0x04006EDA RID: 28378
		public const string grPSODirectory = "ProgressionTiersData";
	}
}
