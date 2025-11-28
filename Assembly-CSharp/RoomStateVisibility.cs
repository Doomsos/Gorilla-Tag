using System;
using UnityEngine;

// Token: 0x02000315 RID: 789
public class RoomStateVisibility : MonoBehaviour
{
	// Token: 0x06001341 RID: 4929 RVA: 0x0006F94B File Offset: 0x0006DB4B
	private void Start()
	{
		this.OnRoomChanged();
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent += new Action(this.OnRoomChanged);
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x0006F989 File Offset: 0x0006DB89
	private void OnDestroy()
	{
		RoomSystem.JoinedRoomEvent -= new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent -= new Action(this.OnRoomChanged);
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x0006F9C4 File Offset: 0x0006DBC4
	private void OnRoomChanged()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			base.gameObject.SetActive(this.enableOutOfRoom);
			return;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			base.gameObject.SetActive(this.enableInPrivateRoom);
			return;
		}
		base.gameObject.SetActive(this.enableInRoom);
	}

	// Token: 0x04001CC5 RID: 7365
	[SerializeField]
	private bool enableOutOfRoom;

	// Token: 0x04001CC6 RID: 7366
	[SerializeField]
	private bool enableInRoom = true;

	// Token: 0x04001CC7 RID: 7367
	[SerializeField]
	private bool enableInPrivateRoom = true;
}
