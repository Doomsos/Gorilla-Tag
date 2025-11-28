using System;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x020002D2 RID: 722
[NetworkBehaviourWeaved(1)]
public class GTSplineAnimateFixedUpdater : NetworkComponent
{
	// Token: 0x060011C8 RID: 4552 RVA: 0x0005DF87 File Offset: 0x0005C187
	protected override void Awake()
	{
		base.Awake();
		this.splineAnimateRef.AddCallbackOnLoad(new Action(this.InitSplineAnimate));
		this.splineAnimateRef.AddCallbackOnUnload(new Action(this.ClearSplineAnimate));
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0005DFBD File Offset: 0x0005C1BD
	private void InitSplineAnimate()
	{
		this.isSplineLoaded = this.splineAnimateRef.TryResolve<SplineAnimate>(out this.splineAnimate);
		if (this.isSplineLoaded && this.splineAnimate != null)
		{
			this.splineAnimate.enabled = false;
		}
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0005DFF8 File Offset: 0x0005C1F8
	private void ClearSplineAnimate()
	{
		this.splineAnimate = null;
		this.isSplineLoaded = false;
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0005E008 File Offset: 0x0005C208
	private void FixedUpdate()
	{
		if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
		{
			if (this.isSplineLoaded)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f) % this.Duration;
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
				return;
			}
		}
		else
		{
			this.progress = (this.progress + Time.fixedDeltaTime) % this.Duration;
			if (this.isSplineLoaded)
			{
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
			}
		}
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x060011CC RID: 4556 RVA: 0x0005E0BD File Offset: 0x0005C2BD
	// (set) Token: 0x060011CD RID: 4557 RVA: 0x0005E0E3 File Offset: 0x0005C2E3
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe float Netdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(float*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(float*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0005E10A File Offset: 0x0005C30A
	public override void WriteDataFusion()
	{
		this.Netdata = this.progress + 1f;
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0005E11E File Offset: 0x0005C31E
	public override void ReadDataFusion()
	{
		this.SharedReadData(this.Netdata);
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0005E12C File Offset: 0x0005C32C
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.progress + 1f);
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0005E154 File Offset: 0x0005C354
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float incomingValue = (float)stream.ReceiveNext();
		this.SharedReadData(incomingValue);
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0005E184 File Offset: 0x0005C384
	private void SharedReadData(float incomingValue)
	{
		if (float.IsNaN(incomingValue) || incomingValue > this.Duration + 1f || incomingValue < 0f)
		{
			return;
		}
		this.progressLerpEnd = incomingValue;
		if (this.progressLerpEnd < this.progress)
		{
			if (this.progress < this.Duration)
			{
				this.progressLerpEnd += this.Duration;
			}
			else
			{
				this.progress -= this.Duration;
			}
		}
		this.progressLerpStart = this.progress;
		this.progressLerpStartTime = Time.time;
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0005E213 File Offset: 0x0005C413
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Netdata = this._Netdata;
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x0005E22B File Offset: 0x0005C42B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Netdata = this.Netdata;
	}

	// Token: 0x04001649 RID: 5705
	[SerializeField]
	private XSceneRef splineAnimateRef;

	// Token: 0x0400164A RID: 5706
	[SerializeField]
	private float Duration;

	// Token: 0x0400164B RID: 5707
	private const float progressLerpDuration = 1f;

	// Token: 0x0400164C RID: 5708
	private SplineAnimate splineAnimate;

	// Token: 0x0400164D RID: 5709
	private bool isSplineLoaded;

	// Token: 0x0400164E RID: 5710
	private float progress;

	// Token: 0x0400164F RID: 5711
	private float progressLerpStart;

	// Token: 0x04001650 RID: 5712
	private float progressLerpEnd;

	// Token: 0x04001651 RID: 5713
	private float progressLerpStartTime;

	// Token: 0x04001652 RID: 5714
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Netdata", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private float _Netdata;
}
