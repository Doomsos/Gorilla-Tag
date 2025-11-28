using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001022 RID: 4130
	[RequireComponent(typeof(AudioSource))]
	[NetworkBehaviourWeaved(2)]
	public class SportScoreboard : NetworkComponent
	{
		// Token: 0x06006868 RID: 26728 RVA: 0x0021FEEC File Offset: 0x0021E0EC
		protected override void Awake()
		{
			base.Awake();
			SportScoreboard.Instance = this;
			this.audioSource = base.GetComponent<AudioSource>();
			this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x06006869 RID: 26729 RVA: 0x0021FF55 File Offset: 0x0021E155
		public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
		{
			this.scoreVisuals[TeamIndex] = visuals;
			this.UpdateScoreboard();
		}

		// Token: 0x0600686A RID: 26730 RVA: 0x0021FF68 File Offset: 0x0021E168
		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				if (!(this.scoreVisuals[i] == null))
				{
					int num = this.teamScores[i];
					if (this.scoreVisuals[i].score1s != null)
					{
						this.scoreVisuals[i].score1s.SetUVOffset(num % 10);
					}
					if (this.scoreVisuals[i].score10s != null)
					{
						this.scoreVisuals[i].score10s.SetUVOffset(num / 10 % 10);
					}
				}
			}
		}

		// Token: 0x0600686B RID: 26731 RVA: 0x00220004 File Offset: 0x0021E204
		private void OnScoreUpdated()
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				if (this.teamScores[i] > this.teamScoresPrev[i] && this.teamParameters[i].goalScoredAudio != null && this.teamScores[i] < this.matchEndScore)
				{
					this.audioSource.GTPlayOneShot(this.teamParameters[i].goalScoredAudio, 1f);
				}
				this.teamScoresPrev[i] = this.teamScores[i];
			}
			if (!this.runningMatchEndCoroutine)
			{
				for (int j = 0; j < this.teamScores.Count; j++)
				{
					if (this.teamScores[j] >= this.matchEndScore)
					{
						base.StartCoroutine(this.MatchEndCoroutine(j));
						break;
					}
				}
			}
			this.UpdateScoreboard();
		}

		// Token: 0x0600686C RID: 26732 RVA: 0x002200F8 File Offset: 0x0021E2F8
		public void TeamScored(int team)
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				if (team >= 0 && team < this.teamScores.Count)
				{
					this.teamScores[team] = this.teamScores[team] + 1;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x0600686D RID: 26733 RVA: 0x00220148 File Offset: 0x0021E348
		public void ResetScores()
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					this.teamScores[i] = 0;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x0600686E RID: 26734 RVA: 0x0022018E File Offset: 0x0021E38E
		private IEnumerator MatchEndCoroutine(int winningTeam)
		{
			this.runningMatchEndCoroutine = true;
			if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && this.teamParameters[winningTeam].matchWonAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.teamParameters[winningTeam].matchWonAudio, 1f);
			}
			yield return new WaitForSeconds(this.matchEndScoreResetDelayTime);
			this.runningMatchEndCoroutine = false;
			this.ResetScores();
			yield break;
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x0600686F RID: 26735 RVA: 0x002201A4 File Offset: 0x0021E3A4
		[Networked]
		[Capacity(2)]
		[NetworkedWeaved(0, 2)]
		[NetworkedWeavedArray(2, 1, typeof(ElementReaderWriterInt32))]
		public unsafe NetworkArray<int> Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing SportScoreboard.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkArray<int>((byte*)(this.Ptr + 0), 2, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x06006870 RID: 26736 RVA: 0x002201E0 File Offset: 0x0021E3E0
		public override void WriteDataFusion()
		{
			this.Data.CopyFrom(this.teamScores, 0, this.teamScores.Count);
		}

		// Token: 0x06006871 RID: 26737 RVA: 0x00220210 File Offset: 0x0021E410
		public override void ReadDataFusion()
		{
			this.teamScores.Clear();
			this.Data.CopyTo(this.teamScores);
			this.OnScoreUpdated();
		}

		// Token: 0x06006872 RID: 26738 RVA: 0x00220244 File Offset: 0x0021E444
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				stream.SendNext(this.teamScores[i]);
			}
		}

		// Token: 0x06006873 RID: 26739 RVA: 0x00220280 File Offset: 0x0021E480
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				this.teamScores[i] = (int)stream.ReceiveNext();
			}
			this.OnScoreUpdated();
		}

		// Token: 0x06006875 RID: 26741 RVA: 0x002202FB File Offset: 0x0021E4FB
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			NetworkBehaviourUtils.InitializeNetworkArray<int>(this.Data, this._Data, "Data");
		}

		// Token: 0x06006876 RID: 26742 RVA: 0x0022031D File Offset: 0x0021E51D
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			NetworkBehaviourUtils.CopyFromNetworkArray<int>(this.Data, ref this._Data);
		}

		// Token: 0x04007703 RID: 30467
		[OnEnterPlay_SetNull]
		public static SportScoreboard Instance;

		// Token: 0x04007704 RID: 30468
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x04007705 RID: 30469
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x04007706 RID: 30470
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x04007707 RID: 30471
		private List<int> teamScores = new List<int>();

		// Token: 0x04007708 RID: 30472
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x04007709 RID: 30473
		private bool runningMatchEndCoroutine;

		// Token: 0x0400770A RID: 30474
		private AudioSource audioSource;

		// Token: 0x0400770B RID: 30475
		private SportScoreboardVisuals[] scoreVisuals;

		// Token: 0x0400770C RID: 30476
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 2)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private int[] _Data;

		// Token: 0x02001023 RID: 4131
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x0400770D RID: 30477
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x0400770E RID: 30478
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
