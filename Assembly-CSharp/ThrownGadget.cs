using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class ThrownGadget : MonoBehaviour
{
	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000685 RID: 1669 RVA: 0x00025128 File Offset: 0x00023328
	// (remove) Token: 0x06000686 RID: 1670 RVA: 0x00025160 File Offset: 0x00023360
	public event Action OnActivated;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000687 RID: 1671 RVA: 0x00025198 File Offset: 0x00023398
	// (remove) Token: 0x06000688 RID: 1672 RVA: 0x000251D0 File Offset: 0x000233D0
	public event Action OnThrown;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000689 RID: 1673 RVA: 0x00025208 File Offset: 0x00023408
	// (remove) Token: 0x0600068A RID: 1674 RVA: 0x00025240 File Offset: 0x00023440
	public event Action OnHitSurface;

	// Token: 0x0600068B RID: 1675 RVA: 0x00025275 File Offset: 0x00023475
	private void OnEnable()
	{
		this.isHeldLocal = false;
		this.lastThrowerLocal = false;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00025285 File Offset: 0x00023485
	public bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00025298 File Offset: 0x00023498
	public bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x000252B1 File Offset: 0x000234B1
	public bool IsHeldByAnother()
	{
		return this.IsHeld() && !this.IsHeldLocal();
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x000252C8 File Offset: 0x000234C8
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? 4 : 5) > 0.25f;
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00025330 File Offset: 0x00023530
	public void Update()
	{
		bool flag = this.IsHeldLocal();
		if (flag)
		{
			this.lastThrowerLocal = true;
			this.UpdateActivation();
		}
		else if (this.isHeldLocal)
		{
			Action onThrown = this.OnThrown;
			if (onThrown != null)
			{
				onThrown.Invoke();
			}
		}
		else if (this.IsHeldByAnother())
		{
			this.lastThrowerLocal = false;
		}
		this.isHeldLocal = flag;
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00025388 File Offset: 0x00023588
	private void UpdateActivation()
	{
		bool flag = this.IsButtonHeld();
		if (!this.activationButtonLastInput && flag)
		{
			Action onActivated = this.OnActivated;
			if (onActivated != null)
			{
				onActivated.Invoke();
			}
		}
		this.activationButtonLastInput = flag;
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x000253C1 File Offset: 0x000235C1
	public void OnCollisionEnter(Collision collision)
	{
		if (this.lastThrowerLocal)
		{
			Action onHitSurface = this.OnHitSurface;
			if (onHitSurface == null)
			{
				return;
			}
			onHitSurface.Invoke();
		}
	}

	// Token: 0x04000887 RID: 2183
	public GameEntity gameEntity;

	// Token: 0x0400088B RID: 2187
	private bool isHeldLocal;

	// Token: 0x0400088C RID: 2188
	private bool lastThrowerLocal;

	// Token: 0x0400088D RID: 2189
	private bool activationButtonLastInput;
}
