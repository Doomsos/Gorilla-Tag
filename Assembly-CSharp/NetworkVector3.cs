using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000C5D RID: 3165
internal class NetworkVector3
{
	// Token: 0x1700073D RID: 1853
	// (get) Token: 0x06004D7E RID: 19838 RVA: 0x00191CFE File Offset: 0x0018FEFE
	public Vector3 CurrentSyncTarget
	{
		get
		{
			return this._currentSyncTarget;
		}
	}

	// Token: 0x06004D7F RID: 19839 RVA: 0x00191D08 File Offset: 0x0018FF08
	public void SetNewSyncTarget(Vector3 newTarget)
	{
		Vector3 currentSyncTarget = this.CurrentSyncTarget;
		ref currentSyncTarget.SetValueSafe(newTarget);
		this.distanceTraveled = currentSyncTarget - this._currentSyncTarget;
		this._currentSyncTarget = currentSyncTarget;
		this.lastSetNetTime = PhotonNetwork.Time;
	}

	// Token: 0x06004D80 RID: 19840 RVA: 0x00191D4C File Offset: 0x0018FF4C
	public Vector3 GetPredictedFuture()
	{
		float num = (float)(PhotonNetwork.Time - this.lastSetNetTime) * (float)PhotonNetwork.SerializationRate;
		Vector3 vector = this.distanceTraveled * num;
		return this._currentSyncTarget + vector;
	}

	// Token: 0x06004D81 RID: 19841 RVA: 0x00191D87 File Offset: 0x0018FF87
	public void Reset()
	{
		this._currentSyncTarget = Vector3.zero;
		this.distanceTraveled = Vector3.zero;
		this.lastSetNetTime = 0.0;
	}

	// Token: 0x04005CE3 RID: 23779
	private double lastSetNetTime;

	// Token: 0x04005CE4 RID: 23780
	private Vector3 _currentSyncTarget = Vector3.zero;

	// Token: 0x04005CE5 RID: 23781
	private Vector3 distanceTraveled = Vector3.zero;
}
