using System;
using UnityEngine;

namespace com.AnotherAxiom.SpaceFight
{
	// Token: 0x02000F66 RID: 3942
	public class SpaceFight : ArcadeGame
	{
		// Token: 0x06006296 RID: 25238 RVA: 0x001FC448 File Offset: 0x001FA648
		private void Update()
		{
			for (int i = 0; i < 2; i++)
			{
				if (base.getButtonState(i, ArcadeButtons.UP))
				{
					this.move(this.player[i], 0.15f);
					this.clamp(this.player[i]);
				}
				if (base.getButtonState(i, ArcadeButtons.RIGHT))
				{
					this.turn(this.player[i], true);
				}
				if (base.getButtonState(i, ArcadeButtons.LEFT))
				{
					this.turn(this.player[i], false);
				}
				if (this.projectilesFired[i])
				{
					this.move(this.projectile[i], 0.5f);
					if (Vector2.Distance(this.player[1 - i].localPosition, this.projectile[i].localPosition) < 0.25f)
					{
						base.PlaySound(1, 2);
						this.player[1 - i].Rotate(0f, 0f, 180f);
						this.projectilesFired[i] = false;
					}
					if (Mathf.Abs(this.projectile[i].localPosition.x) > this.tableSize.x || Mathf.Abs(this.projectile[i].localPosition.y) > this.tableSize.y)
					{
						this.projectilesFired[i] = false;
					}
				}
				if (!this.projectilesFired[i])
				{
					this.projectile[i].position = this.player[i].position;
					this.projectile[i].rotation = this.player[i].rotation;
				}
			}
		}

		// Token: 0x06006297 RID: 25239 RVA: 0x001FC5D8 File Offset: 0x001FA7D8
		private void clamp(Transform tr)
		{
			tr.localPosition = new Vector2(Mathf.Clamp(tr.localPosition.x, -this.tableSize.x, this.tableSize.x), Mathf.Clamp(tr.localPosition.y, -this.tableSize.y, this.tableSize.y));
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x001FC643 File Offset: 0x001FA843
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.TRIGGER)
			{
				if (!this.projectilesFired[player])
				{
					base.PlaySound(0, 3);
				}
				this.projectilesFired[player] = true;
			}
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x001FC668 File Offset: 0x001FA868
		private void move(Transform p, float speed)
		{
			p.Translate(p.up * Time.deltaTime * speed, 0);
		}

		// Token: 0x0600629A RID: 25242 RVA: 0x001FC687 File Offset: 0x001FA887
		private void turn(Transform p, bool cw)
		{
			p.Rotate(0f, 0f, (float)(cw ? 180 : -180) * Time.deltaTime);
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x001FC6B0 File Offset: 0x001FA8B0
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P1LocX = this.player[0].localPosition.x;
			this.netStateCur.P1LocY = this.player[0].localPosition.y;
			this.netStateCur.P1Rot = this.player[0].localRotation.eulerAngles.z;
			this.netStateCur.P2LocX = this.player[1].localPosition.x;
			this.netStateCur.P2LocY = this.player[1].localPosition.y;
			this.netStateCur.P2Rot = this.player[1].localRotation.eulerAngles.z;
			this.netStateCur.P1PrLocX = this.projectile[0].localPosition.x;
			this.netStateCur.P1PrLocY = this.projectile[0].localPosition.y;
			this.netStateCur.P2PrLocX = this.projectile[1].localPosition.x;
			this.netStateCur.P2PrLocY = this.projectile[1].localPosition.y;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x001FC830 File Offset: 0x001FAA30
		public override void SetNetworkState(byte[] b)
		{
			SpaceFight.SpaceFlightNetState spaceFlightNetState = (SpaceFight.SpaceFlightNetState)ArcadeGame.UnwrapNetState(b);
			this.player[0].localPosition = new Vector2(spaceFlightNetState.P1LocX, spaceFlightNetState.P1LocY);
			this.player[0].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P1Rot);
			this.player[1].localPosition = new Vector2(spaceFlightNetState.P2LocX, spaceFlightNetState.P2LocY);
			this.player[1].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P2Rot);
			this.projectile[0].localPosition = new Vector2(spaceFlightNetState.P1PrLocX, spaceFlightNetState.P1PrLocY);
			this.projectile[1].localPosition = new Vector2(spaceFlightNetState.P2PrLocX, spaceFlightNetState.P2PrLocY);
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x00002789 File Offset: 0x00000989
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x00002789 File Offset: 0x00000989
		public override void OnTimeout()
		{
		}

		// Token: 0x04007139 RID: 28985
		[SerializeField]
		private Transform[] player;

		// Token: 0x0400713A RID: 28986
		[SerializeField]
		private Transform[] projectile;

		// Token: 0x0400713B RID: 28987
		[SerializeField]
		private Vector2 tableSize;

		// Token: 0x0400713C RID: 28988
		private bool[] projectilesFired = new bool[2];

		// Token: 0x0400713D RID: 28989
		private SpaceFight.SpaceFlightNetState netStateLast;

		// Token: 0x0400713E RID: 28990
		private SpaceFight.SpaceFlightNetState netStateCur;

		// Token: 0x02000F67 RID: 3943
		[Serializable]
		private struct SpaceFlightNetState : IEquatable<SpaceFight.SpaceFlightNetState>
		{
			// Token: 0x060062A0 RID: 25248 RVA: 0x001FC930 File Offset: 0x001FAB30
			public bool Equals(SpaceFight.SpaceFlightNetState other)
			{
				return this.P1LocX.Approx(other.P1LocX, 1E-06f) && this.P1LocY.Approx(other.P1LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P2LocX.Approx(other.P2LocX, 1E-06f) && this.P2LocY.Approx(other.P2LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P1PrLocX.Approx(other.P1PrLocX, 1E-06f) && this.P1PrLocY.Approx(other.P1PrLocY, 1E-06f) && this.P2PrLocX.Approx(other.P2PrLocX, 1E-06f) && this.P2PrLocY.Approx(other.P2PrLocY, 1E-06f);
			}

			// Token: 0x0400713F RID: 28991
			public float P1LocX;

			// Token: 0x04007140 RID: 28992
			public float P1LocY;

			// Token: 0x04007141 RID: 28993
			public float P1Rot;

			// Token: 0x04007142 RID: 28994
			public float P2LocX;

			// Token: 0x04007143 RID: 28995
			public float P2LocY;

			// Token: 0x04007144 RID: 28996
			public float P2Rot;

			// Token: 0x04007145 RID: 28997
			public float P1PrLocX;

			// Token: 0x04007146 RID: 28998
			public float P1PrLocY;

			// Token: 0x04007147 RID: 28999
			public float P2PrLocX;

			// Token: 0x04007148 RID: 29000
			public float P2PrLocY;
		}
	}
}
