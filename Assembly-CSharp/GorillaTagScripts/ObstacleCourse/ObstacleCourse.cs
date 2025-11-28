using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E30 RID: 3632
	public class ObstacleCourse : MonoBehaviour
	{
		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x06005A9D RID: 23197 RVA: 0x001D0E0C File Offset: 0x001CF00C
		// (set) Token: 0x06005A9E RID: 23198 RVA: 0x001D0E14 File Offset: 0x001CF014
		public int winnerActorNumber { get; private set; }

		// Token: 0x06005A9F RID: 23199 RVA: 0x001D0E20 File Offset: 0x001CF020
		private void Awake()
		{
			this.numPlayersOnCourse = 0;
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter += this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit += this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped += this.OnEndLineTrigger;
		}

		// Token: 0x06005AA0 RID: 23200 RVA: 0x001D0E94 File Offset: 0x001CF094
		private void OnDestroy()
		{
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter -= this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit -= this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped -= this.OnEndLineTrigger;
		}

		// Token: 0x06005AA1 RID: 23201 RVA: 0x001D0F01 File Offset: 0x001CF101
		private void Start()
		{
			this.RestartTimer(false);
		}

		// Token: 0x06005AA2 RID: 23202 RVA: 0x001D0F0A File Offset: 0x001CF10A
		public void InvokeUpdate()
		{
			if (NetworkSystem.Instance.InRoom && ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Finished && Time.time - this.startTime >= this.cooldownTime)
			{
				this.RestartTimer(true);
			}
		}

		// Token: 0x06005AA3 RID: 23203 RVA: 0x001D0F48 File Offset: 0x001CF148
		public void OnPlayerEnterZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse++;
			}
		}

		// Token: 0x06005AA4 RID: 23204 RVA: 0x001D0F64 File Offset: 0x001CF164
		public void OnPlayerExitZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse--;
			}
		}

		// Token: 0x06005AA5 RID: 23205 RVA: 0x001D0F80 File Offset: 0x001CF180
		private void RestartTimer(bool playFx = true)
		{
			this.UpdateState(ObstacleCourse.RaceState.Started, playFx);
		}

		// Token: 0x06005AA6 RID: 23206 RVA: 0x001D0F8A File Offset: 0x001CF18A
		private void EndRace()
		{
			this.UpdateState(ObstacleCourse.RaceState.Finished, true);
			this.startTime = Time.time;
		}

		// Token: 0x06005AA7 RID: 23207 RVA: 0x001D0FA0 File Offset: 0x001CF1A0
		public void PlayWinningEffects()
		{
			if (this.confettiParticle)
			{
				this.confettiParticle.Play();
			}
			if (this.bannerRenderer)
			{
				UberShaderProperty baseColor = UberShader.BaseColor;
				Material material = this.bannerRenderer.material;
				RigContainer rigContainer = this.winnerRig;
				baseColor.SetValue<Color?>(material, (rigContainer != null) ? new Color?(rigContainer.Rig.playerColor) : default(Color?));
			}
			this.audioSource.GTPlay();
		}

		// Token: 0x06005AA8 RID: 23208 RVA: 0x001D1016 File Offset: 0x001CF216
		public void OnEndLineTrigger(VRRig rig)
		{
			if (ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.winnerActorNumber = rig.creator.ActorNumber;
				this.winnerRig = rig.rigContainer;
				this.EndRace();
			}
		}

		// Token: 0x06005AA9 RID: 23209 RVA: 0x001D104F File Offset: 0x001CF24F
		public void Deserialize(int _winnerActorNumber, ObstacleCourse.RaceState _currentState)
		{
			if (!ObstacleCourseManager.Instance.IsMine)
			{
				this.winnerActorNumber = _winnerActorNumber;
				VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(this.winnerActorNumber), out this.winnerRig);
				this.UpdateState(_currentState, true);
			}
		}

		// Token: 0x06005AAA RID: 23210 RVA: 0x001D1090 File Offset: 0x001CF290
		private void UpdateState(ObstacleCourse.RaceState state, bool playFX = true)
		{
			this.currentState = state;
			WinnerScoreboard winnerScoreboard = this.scoreboard;
			RigContainer rigContainer = this.winnerRig;
			winnerScoreboard.UpdateBoard((rigContainer != null) ? rigContainer.Rig.playerNameVisible : null, this.currentState);
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.PlayWinningEffects();
			}
			else if (this.currentState == ObstacleCourse.RaceState.Started && this.bannerRenderer)
			{
				UberShader.BaseColor.SetValue<Color>(this.bannerRenderer.material, Color.white);
			}
			this.UpdateStartingGate();
		}

		// Token: 0x06005AAB RID: 23211 RVA: 0x001D1114 File Offset: 0x001CF314
		private void UpdateStartingGate()
		{
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, 90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, -90f);
				return;
			}
			if (this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, -90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, 90f);
			}
		}

		// Token: 0x040067E1 RID: 26593
		public WinnerScoreboard scoreboard;

		// Token: 0x040067E3 RID: 26595
		private RigContainer winnerRig;

		// Token: 0x040067E4 RID: 26596
		public ObstacleCourseZoneTrigger[] zoneTriggers;

		// Token: 0x040067E5 RID: 26597
		[HideInInspector]
		public ObstacleCourse.RaceState currentState;

		// Token: 0x040067E6 RID: 26598
		[SerializeField]
		private ParticleSystem confettiParticle;

		// Token: 0x040067E7 RID: 26599
		[SerializeField]
		private Renderer bannerRenderer;

		// Token: 0x040067E8 RID: 26600
		[SerializeField]
		private TappableBell TappableBell;

		// Token: 0x040067E9 RID: 26601
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040067EA RID: 26602
		[SerializeField]
		private float cooldownTime = 20f;

		// Token: 0x040067EB RID: 26603
		public GameObject leftGate;

		// Token: 0x040067EC RID: 26604
		public GameObject rightGate;

		// Token: 0x040067ED RID: 26605
		private int numPlayersOnCourse;

		// Token: 0x040067EE RID: 26606
		private float startTime;

		// Token: 0x02000E31 RID: 3633
		public enum RaceState
		{
			// Token: 0x040067F0 RID: 26608
			Started,
			// Token: 0x040067F1 RID: 26609
			Waiting,
			// Token: 0x040067F2 RID: 26610
			Finished
		}
	}
}
