using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class SIGadgetWing : SIGadget
{
	// Token: 0x06000526 RID: 1318 RVA: 0x0001E378 File Offset: 0x0001C578
	private void Awake()
	{
		if (this.m_buttonActivatable == null)
		{
			this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0001E43B File Offset: 0x0001C63B
	private void OnGrabbed()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001E43B File Offset: 0x0001C63B
	private void OnSnapped()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00002789 File Offset: 0x00000989
	private void OnReleased()
	{
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x00002789 File Offset: 0x00000989
	private void OnUnsnapped()
	{
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001E454 File Offset: 0x0001C654
	protected override void OnUpdateAuthority(float dt)
	{
		Vector3 position = this.m_wingCenter.transform.position;
		SIGadgetWing_EState state = this._state;
		this._state = (this.m_buttonActivatable.CheckInput(true, true, 0.25f, true, true) ? SIGadgetWing_EState.TriggerPressed : SIGadgetWing_EState.Idle);
		if (state != this._state)
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)this._state);
			this._lastWingPos = position;
		}
		if (this._state != SIGadgetWing_EState.TriggerPressed)
		{
			return;
		}
		Vector3 vector = this._lastWingPos - position;
		Vector3 up = this.m_wingCenter.transform.up;
		float num = Mathf.Max(Vector3.Dot(vector, up), 0f);
		double num2 = PhotonNetwork.Time - (double)GTPlayer.Instance.LastTouchedGroundAtNetworkTime;
		float num3 = Mathf.Lerp(this.m_flapStrength, this.m_flapDecayedStrength, (float)num2 / this.m_decayDuration);
		Vector3 force = up * (num * num3);
		GTPlayer.Instance.AddForce(force, 1);
		this._lastWingPos = position;
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnUpdateRemote(float dt)
	{
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001E54A File Offset: 0x0001C74A
	public override void OnEntityStateChange(long prevState, long newState)
	{
		if (newState == prevState || newState < 0L || newState >= 2L)
		{
			return;
		}
		this.m_gtAnimator.SetState(newState);
	}

	// Token: 0x04000657 RID: 1623
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x04000658 RID: 1624
	[SerializeField]
	private float m_flapStrength;

	// Token: 0x04000659 RID: 1625
	[SerializeField]
	private float m_flapDecayedStrength;

	// Token: 0x0400065A RID: 1626
	[SerializeField]
	private float m_decayDuration;

	// Token: 0x0400065B RID: 1627
	[SerializeField]
	private float m_liftStrength;

	// Token: 0x0400065C RID: 1628
	[SerializeField]
	private float m_liftCap;

	// Token: 0x0400065D RID: 1629
	[SerializeField]
	private Transform m_wingCenter;

	// Token: 0x0400065E RID: 1630
	[SerializeField]
	private GTAnimator m_gtAnimator;

	// Token: 0x0400065F RID: 1631
	private Vector3 _lastWingPos;

	// Token: 0x04000660 RID: 1632
	private SIGadgetWing_EState _state;
}
