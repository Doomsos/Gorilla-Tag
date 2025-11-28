using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000354 RID: 852
public class HandHoldBehaviourActivation : Tappable
{
	// Token: 0x0600145B RID: 5211 RVA: 0x000749EC File Offset: 0x00072BEC
	protected override void OnEnable()
	{
		base.OnEnable();
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x00074A2C File Offset: 0x00072C2C
	public override void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		byte b = CollectionExtensions.GetValueOrDefault<int, byte>(this.m_playerGrabCounts, sender.Sender.ActorNumber, 0);
		b += 1;
		if (b > 2)
		{
			return;
		}
		this.m_playerGrabCounts[sender.Sender.ActorNumber] = b;
		this.grabs++;
		if (this.grabs < 2)
		{
			this.ActivationStart.Invoke();
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x00074A94 File Offset: 0x00072C94
	public override void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		byte b;
		if (!this.m_playerGrabCounts.TryGetValue(sender.Sender.ActorNumber, ref b) || b < 1)
		{
			return;
		}
		b -= 1;
		this.m_playerGrabCounts[sender.Sender.ActorNumber] = b;
		bool flag = this.grabs > 0;
		this.grabs = Mathf.Max(0, this.grabs - 1);
		if (flag && this.grabs < 1)
		{
			this.ActivationStop.Invoke();
		}
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x00074B10 File Offset: 0x00072D10
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		byte b;
		if (!this.m_playerGrabCounts.TryGetValue(player.ActorNumber, ref b))
		{
			return;
		}
		bool flag = this.grabs > 0;
		this.grabs = Mathf.Max(0, this.grabs - (int)b);
		this.m_playerGrabCounts.Remove(player.ActorNumber);
		if (flag && this.grabs < 1)
		{
			this.ActivationStop.Invoke();
		}
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x00074B78 File Offset: 0x00072D78
	private void OnLeftRoom()
	{
		byte valueOrDefault = CollectionExtensions.GetValueOrDefault<int, byte>(this.m_playerGrabCounts, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
		if (this.grabs > 0 && valueOrDefault < 1)
		{
			this.ActivationStop.Invoke();
		}
		this.grabs = (int)valueOrDefault;
		this.m_playerGrabCounts.Clear();
		this.m_playerGrabCounts[NetworkSystem.Instance.LocalPlayer.ActorNumber] = valueOrDefault;
	}

	// Token: 0x04001EF2 RID: 7922
	[SerializeField]
	private UnityEvent ActivationStart;

	// Token: 0x04001EF3 RID: 7923
	[SerializeField]
	private UnityEvent ActivationStop;

	// Token: 0x04001EF4 RID: 7924
	private int grabs;

	// Token: 0x04001EF5 RID: 7925
	private readonly Dictionary<int, byte> m_playerGrabCounts = new Dictionary<int, byte>(10);
}
