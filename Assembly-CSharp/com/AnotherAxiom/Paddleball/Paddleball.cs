using System;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000F68 RID: 3944
	public class Paddleball : ArcadeGame
	{
		// Token: 0x060062A1 RID: 25249 RVA: 0x001FCA22 File Offset: 0x001FAC22
		protected override void Awake()
		{
			base.Awake();
			this.yPosToByteFactor = 255f / (2f * this.tableSizeBall.y);
			this.byteToYPosFactor = 1f / this.yPosToByteFactor;
		}

		// Token: 0x060062A2 RID: 25250 RVA: 0x001FCA5C File Offset: 0x001FAC5C
		private void Start()
		{
			this.whiteWinScreen.SetActive(false);
			this.blackWinScreen.SetActive(false);
			this.titleScreen.SetActive(true);
			this.ball.gameObject.SetActive(false);
			this.currentScreenMode = Paddleball.ScreenMode.Title;
			this.paddleIdle = new float[this.p.Length];
			for (int i = 0; i < this.p.Length; i++)
			{
				this.p[i].gameObject.SetActive(false);
				this.paddleIdle[i] = 30f;
			}
			this.gameBallSpeed = this.initialBallSpeed;
			this.scoreR = (this.scoreL = 0);
			this.scoreFormat = this.scoreDisplay.text;
			this.UpdateScore();
		}

		// Token: 0x060062A3 RID: 25251 RVA: 0x001FCB20 File Offset: 0x001FAD20
		private void Update()
		{
			if (this.currentScreenMode == Paddleball.ScreenMode.Gameplay)
			{
				this.ball.Translate(this.ballTrajectory.normalized * Time.deltaTime * this.gameBallSpeed);
				if (this.ball.localPosition.y > this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.y < -this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, -this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.x > this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreL++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreL >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.WhiteWin);
					}
				}
				if (this.ball.localPosition.x < -this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(-this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreR++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreR >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.BlackWin);
					}
				}
			}
			if (this.returnToTitleAfterTimestamp != 0f && Time.time > this.returnToTitleAfterTimestamp)
			{
				this.ChangeScreen(Paddleball.ScreenMode.Title);
			}
			for (int i = 0; i < this.p.Length; i++)
			{
				if (base.IsPlayerLocallyControlled(i))
				{
					float num = this.requestedPos[i];
					if (base.getButtonState(i, ArcadeButtons.UP))
					{
						this.requestedPos[i] += Time.deltaTime * this.paddleSpeed;
					}
					else if (base.getButtonState(i, ArcadeButtons.DOWN))
					{
						this.requestedPos[i] -= Time.deltaTime * this.paddleSpeed;
					}
					this.requestedPos[i] = Mathf.Clamp(this.requestedPos[i], -this.tableSizePaddle.y, this.tableSizePaddle.y);
				}
				float num2;
				if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
				{
					num2 = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.requestedPos[i], Time.deltaTime * this.paddleSpeed);
				}
				else
				{
					num2 = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.officialPos[i], Time.deltaTime * this.paddleSpeed);
				}
				this.p[i].transform.localPosition = this.p[i].transform.localPosition.WithY(Mathf.Clamp(num2, -this.tableSizePaddle.y, this.tableSizePaddle.y));
				if (base.getButtonState(i, ArcadeButtons.GRAB))
				{
					this.paddleIdle[i] = 0f;
					Paddleball.ScreenMode screenMode = this.currentScreenMode;
					if (screenMode != Paddleball.ScreenMode.Title)
					{
						if (screenMode == Paddleball.ScreenMode.Gameplay)
						{
							this.returnToTitleAfterTimestamp = Time.time + 30f;
						}
					}
					else
					{
						this.ChangeScreen(Paddleball.ScreenMode.Gameplay);
					}
				}
				else
				{
					this.paddleIdle[i] += Time.deltaTime;
				}
				bool flag = this.paddleIdle[i] < 30f;
				if (this.p[i].gameObject.activeSelf != flag)
				{
					if (flag)
					{
						base.PlaySound(4, 3);
						Vector3 localPosition = this.p[i].transform.localPosition;
						localPosition.y = 0f;
						this.requestedPos[i] = localPosition.y;
						this.p[i].transform.localPosition = localPosition;
					}
					this.p[i].gameObject.SetActive(this.paddleIdle[i] < 30f);
				}
				if (this.p[i].gameObject.activeInHierarchy && Mathf.Abs(this.ball.localPosition.x - this.p[i].transform.localPosition.x) < 0.1f && Mathf.Abs(this.ball.localPosition.y - this.p[i].transform.localPosition.y) < 0.5f)
				{
					this.ballTrajectory.y = (this.ball.localPosition.y - this.p[i].transform.localPosition.y) * 1.25f;
					float x = this.ballTrajectory.x;
					if (this.p[i].Right)
					{
						this.ballTrajectory.x = Mathf.Abs(this.ballTrajectory.y) - 1f;
					}
					else
					{
						this.ballTrajectory.x = 1f - Mathf.Abs(this.ballTrajectory.y);
					}
					if (x > 0f != this.ballTrajectory.x > 0f)
					{
						base.PlaySound(1, 3);
					}
					this.ballTrajectory.Normalize();
					this.gameBallSpeed += this.ballSpeedBoost;
				}
			}
		}

		// Token: 0x060062A4 RID: 25252 RVA: 0x001FD188 File Offset: 0x001FB388
		private void UpdateScore()
		{
			if (this.scoreFormat == null)
			{
				return;
			}
			this.scoreL = Mathf.Clamp(this.scoreL, 0, 10);
			this.scoreR = Mathf.Clamp(this.scoreR, 0, 10);
			this.scoreDisplay.text = string.Format(this.scoreFormat, this.scoreL, this.scoreR);
		}

		// Token: 0x060062A5 RID: 25253 RVA: 0x001FD1F2 File Offset: 0x001FB3F2
		private float ByteToYPos(byte Y)
		{
			return (float)Y / this.yPosToByteFactor - this.tableSizeBall.y;
		}

		// Token: 0x060062A6 RID: 25254 RVA: 0x001FD209 File Offset: 0x001FB409
		private byte YPosToByte(float Y)
		{
			return (byte)Mathf.RoundToInt((Y + this.tableSizeBall.y) * this.yPosToByteFactor);
		}

		// Token: 0x060062A7 RID: 25255 RVA: 0x001FD228 File Offset: 0x001FB428
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P0LocY = this.YPosToByte(this.p[0].transform.localPosition.y);
			this.netStateCur.P1LocY = this.YPosToByte(this.p[1].transform.localPosition.y);
			this.netStateCur.P2LocY = this.YPosToByte(this.p[2].transform.localPosition.y);
			this.netStateCur.P3LocY = this.YPosToByte(this.p[3].transform.localPosition.y);
			this.netStateCur.BallLocX = this.ball.localPosition.x;
			this.netStateCur.BallLocY = this.YPosToByte(this.ball.localPosition.y);
			this.netStateCur.BallTrajectoryX = (byte)((this.ballTrajectory.x + 1f) * 127.5f);
			this.netStateCur.BallTrajectoryY = (byte)((this.ballTrajectory.y + 1f) * 127.5f);
			this.netStateCur.BallSpeed = this.gameBallSpeed;
			this.netStateCur.ScoreLeft = this.scoreL;
			this.netStateCur.ScoreRight = this.scoreR;
			this.netStateCur.ScreenMode = (int)this.currentScreenMode;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x060062A8 RID: 25256 RVA: 0x001FD3DC File Offset: 0x001FB5DC
		public override void SetNetworkState(byte[] b)
		{
			Paddleball.PaddleballNetState paddleballNetState = (Paddleball.PaddleballNetState)ArcadeGame.UnwrapNetState(b);
			this.officialPos[0] = this.ByteToYPos(paddleballNetState.P0LocY);
			this.officialPos[1] = this.ByteToYPos(paddleballNetState.P1LocY);
			this.officialPos[2] = this.ByteToYPos(paddleballNetState.P2LocY);
			this.officialPos[3] = this.ByteToYPos(paddleballNetState.P3LocY);
			Vector2 vector;
			vector..ctor(paddleballNetState.BallLocX, this.ByteToYPos(paddleballNetState.BallLocY));
			Vector2 normalized = new Vector2((float)paddleballNetState.BallTrajectoryX * 0.007843138f - 1f, (float)paddleballNetState.BallTrajectoryY * 0.007843138f - 1f).normalized;
			Vector2 vector2 = vector - normalized * Vector2.Dot(vector, normalized);
			Vector2 vector3 = this.ball.localPosition.xy();
			Vector2 vector4 = vector3 - this.ballTrajectory * Vector2.Dot(vector3, this.ballTrajectory);
			if ((vector2 - vector4).IsLongerThan(0.1f))
			{
				this.ball.localPosition = vector;
				this.ballTrajectory = normalized.xy();
			}
			this.gameBallSpeed = paddleballNetState.BallSpeed;
			this.ChangeScreen((Paddleball.ScreenMode)paddleballNetState.ScreenMode);
			if (this.scoreL != paddleballNetState.ScoreLeft || this.scoreR != paddleballNetState.ScoreRight)
			{
				this.scoreL = paddleballNetState.ScoreLeft;
				this.scoreR = paddleballNetState.ScoreRight;
				this.UpdateScore();
			}
		}

		// Token: 0x060062A9 RID: 25257 RVA: 0x00002789 File Offset: 0x00000989
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x00002789 File Offset: 0x00000989
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
		}

		// Token: 0x060062AB RID: 25259 RVA: 0x001FD558 File Offset: 0x001FB758
		private void ChangeScreen(Paddleball.ScreenMode mode)
		{
			if (this.currentScreenMode == mode)
			{
				return;
			}
			switch (this.currentScreenMode)
			{
			case Paddleball.ScreenMode.Title:
				this.titleScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(false);
				break;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(false);
				break;
			}
			this.currentScreenMode = mode;
			switch (mode)
			{
			case Paddleball.ScreenMode.Title:
				this.gameBallSpeed = this.initialBallSpeed;
				this.scoreL = 0;
				this.scoreR = 0;
				this.UpdateScore();
				this.returnToTitleAfterTimestamp = 0f;
				this.titleScreen.SetActive(true);
				return;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + 30f;
				return;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			default:
				return;
			}
		}

		// Token: 0x060062AC RID: 25260 RVA: 0x001FD68F File Offset: 0x001FB88F
		public override void OnTimeout()
		{
			this.ChangeScreen(Paddleball.ScreenMode.Title);
		}

		// Token: 0x060062AD RID: 25261 RVA: 0x001FD698 File Offset: 0x001FB898
		public override void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			this.requestedPos[player] = this.ByteToYPos((byte)stream.ReceiveNext());
		}

		// Token: 0x060062AE RID: 25262 RVA: 0x001FD6B3 File Offset: 0x001FB8B3
		public override void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.YPosToByte(this.requestedPos[player]));
		}

		// Token: 0x04007149 RID: 29001
		[SerializeField]
		private PaddleballPaddle[] p;

		// Token: 0x0400714A RID: 29002
		private float[] requestedPos = new float[4];

		// Token: 0x0400714B RID: 29003
		private float[] officialPos = new float[4];

		// Token: 0x0400714C RID: 29004
		[SerializeField]
		private Transform ball;

		// Token: 0x0400714D RID: 29005
		[SerializeField]
		private Vector2 ballTrajectory;

		// Token: 0x0400714E RID: 29006
		[SerializeField]
		private float paddleSpeed = 1f;

		// Token: 0x0400714F RID: 29007
		[SerializeField]
		private float initialBallSpeed = 1f;

		// Token: 0x04007150 RID: 29008
		[SerializeField]
		private float ballSpeedBoost = 0.02f;

		// Token: 0x04007151 RID: 29009
		private float gameBallSpeed = 1f;

		// Token: 0x04007152 RID: 29010
		[SerializeField]
		private Vector2 tableSizeBall;

		// Token: 0x04007153 RID: 29011
		[SerializeField]
		private Vector2 tableSizePaddle;

		// Token: 0x04007154 RID: 29012
		[SerializeField]
		private GameObject blackWinScreen;

		// Token: 0x04007155 RID: 29013
		[SerializeField]
		private GameObject whiteWinScreen;

		// Token: 0x04007156 RID: 29014
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x04007157 RID: 29015
		[SerializeField]
		private float winScreenDuration;

		// Token: 0x04007158 RID: 29016
		private float returnToTitleAfterTimestamp;

		// Token: 0x04007159 RID: 29017
		private int scoreL;

		// Token: 0x0400715A RID: 29018
		private int scoreR;

		// Token: 0x0400715B RID: 29019
		private string scoreFormat;

		// Token: 0x0400715C RID: 29020
		[SerializeField]
		private TMP_Text scoreDisplay;

		// Token: 0x0400715D RID: 29021
		private float[] paddleIdle;

		// Token: 0x0400715E RID: 29022
		private Paddleball.ScreenMode currentScreenMode;

		// Token: 0x0400715F RID: 29023
		private const int AUDIO_WALLBOUNCE = 0;

		// Token: 0x04007160 RID: 29024
		private const int AUDIO_PADDLEBOUNCE = 1;

		// Token: 0x04007161 RID: 29025
		private const int AUDIO_SCORE = 2;

		// Token: 0x04007162 RID: 29026
		private const int AUDIO_WIN = 3;

		// Token: 0x04007163 RID: 29027
		private const int AUDIO_PLAYERJOIN = 4;

		// Token: 0x04007164 RID: 29028
		private const int VAR_REQUESTEDPOS = 0;

		// Token: 0x04007165 RID: 29029
		private const int MAXSCORE = 10;

		// Token: 0x04007166 RID: 29030
		private float yPosToByteFactor;

		// Token: 0x04007167 RID: 29031
		private float byteToYPosFactor;

		// Token: 0x04007168 RID: 29032
		private const float directionToByteFactor = 127.5f;

		// Token: 0x04007169 RID: 29033
		private const float byteToDirectionFactor = 0.007843138f;

		// Token: 0x0400716A RID: 29034
		private Paddleball.PaddleballNetState netStateLast;

		// Token: 0x0400716B RID: 29035
		private Paddleball.PaddleballNetState netStateCur;

		// Token: 0x02000F69 RID: 3945
		private enum ScreenMode
		{
			// Token: 0x0400716D RID: 29037
			Title,
			// Token: 0x0400716E RID: 29038
			Gameplay,
			// Token: 0x0400716F RID: 29039
			WhiteWin,
			// Token: 0x04007170 RID: 29040
			BlackWin
		}

		// Token: 0x02000F6A RID: 3946
		[Serializable]
		private struct PaddleballNetState : IEquatable<Paddleball.PaddleballNetState>
		{
			// Token: 0x060062B0 RID: 25264 RVA: 0x001FD728 File Offset: 0x001FB928
			public bool Equals(Paddleball.PaddleballNetState other)
			{
				return this.P0LocY == other.P0LocY && this.P1LocY == other.P1LocY && this.P2LocY == other.P2LocY && this.P3LocY == other.P3LocY && this.BallLocX.Approx(other.BallLocX, 1E-06f) && this.BallLocY == other.BallLocY && this.BallTrajectoryX == other.BallTrajectoryX && this.BallTrajectoryY == other.BallTrajectoryY && this.BallSpeed.Approx(other.BallSpeed, 1E-06f) && this.ScoreLeft == other.ScoreLeft && this.ScoreRight == other.ScoreRight && this.ScreenMode == other.ScreenMode;
			}

			// Token: 0x04007171 RID: 29041
			public byte P0LocY;

			// Token: 0x04007172 RID: 29042
			public byte P1LocY;

			// Token: 0x04007173 RID: 29043
			public byte P2LocY;

			// Token: 0x04007174 RID: 29044
			public byte P3LocY;

			// Token: 0x04007175 RID: 29045
			public float BallLocX;

			// Token: 0x04007176 RID: 29046
			public byte BallLocY;

			// Token: 0x04007177 RID: 29047
			public byte BallTrajectoryX;

			// Token: 0x04007178 RID: 29048
			public byte BallTrajectoryY;

			// Token: 0x04007179 RID: 29049
			public float BallSpeed;

			// Token: 0x0400717A RID: 29050
			public int ScoreLeft;

			// Token: 0x0400717B RID: 29051
			public int ScoreRight;

			// Token: 0x0400717C RID: 29052
			public int ScreenMode;
		}
	}
}
