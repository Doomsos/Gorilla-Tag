using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200054C RID: 1356
public class MonkeBallBallResetTrigger : MonoBehaviour
{
	// Token: 0x0600222E RID: 8750 RVA: 0x000B2C6C File Offset: 0x000B0E6C
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			GameBallPlayer gameBallPlayer = (component.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.heldByActorNumber);
			if (gameBallPlayer == null)
			{
				gameBallPlayer = ((component.lastHeldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.lastHeldByActorNumber));
				if (gameBallPlayer == null)
				{
					return;
				}
			}
			this._lastBall = component;
			int num = gameBallPlayer.teamId;
			if (num == -1)
			{
				num = component.lastHeldByTeamId;
			}
			if (num >= 0 && num < this.teamMaterials.Length)
			{
				this.trigger.sharedMaterial = this.teamMaterials[num];
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(true, num);
			}
		}
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000B2D24 File Offset: 0x000B0F24
	private void OnTriggerExit(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (component == this._lastBall)
			{
				this.trigger.sharedMaterial = this.neutralMaterial;
				this._lastBall = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(false, -1);
			}
		}
	}

	// Token: 0x04002CCB RID: 11467
	public Renderer trigger;

	// Token: 0x04002CCC RID: 11468
	public Material[] teamMaterials;

	// Token: 0x04002CCD RID: 11469
	public Material neutralMaterial;

	// Token: 0x04002CCE RID: 11470
	private GameBall _lastBall;
}
