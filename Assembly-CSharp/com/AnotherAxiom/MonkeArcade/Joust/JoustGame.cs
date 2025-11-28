using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02000F6C RID: 3948
	public class JoustGame : ArcadeGame
	{
		// Token: 0x060062B3 RID: 25267 RVA: 0x001FD832 File Offset: 0x001FBA32
		public override byte[] GetNetworkState()
		{
			return new byte[0];
		}

		// Token: 0x060062B4 RID: 25268 RVA: 0x00002789 File Offset: 0x00000989
		public override void SetNetworkState(byte[] obj)
		{
		}

		// Token: 0x060062B5 RID: 25269 RVA: 0x001FD83A File Offset: 0x001FBA3A
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button != ArcadeButtons.GRAB)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					this.joustPlayers[player].Flap();
					return;
				}
			}
			else
			{
				this.joustPlayers[player].gameObject.SetActive(true);
			}
		}

		// Token: 0x060062B6 RID: 25270 RVA: 0x001FD869 File Offset: 0x001FBA69
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.GRAB)
			{
				this.joustPlayers[player].gameObject.SetActive(false);
			}
		}

		// Token: 0x060062B7 RID: 25271 RVA: 0x001FD884 File Offset: 0x001FBA84
		private void Start()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				this.joustPlayers[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x001FD8B8 File Offset: 0x001FBAB8
		private void Update()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				if (this.joustPlayers[i].gameObject.activeInHierarchy)
				{
					int num = (base.getButtonState(i, ArcadeButtons.LEFT) ? -1 : 0) + (base.getButtonState(i, ArcadeButtons.RIGHT) ? 1 : 0);
					this.joustPlayers[i].HorizontalSpeed = Mathf.Clamp(this.joustPlayers[i].HorizontalSpeed + (float)num * Time.deltaTime, -1f, 1f);
				}
			}
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x00002789 File Offset: 0x00000989
		public override void OnTimeout()
		{
		}

		// Token: 0x0400717E RID: 29054
		[SerializeField]
		private JoustPlayer[] joustPlayers;
	}
}
