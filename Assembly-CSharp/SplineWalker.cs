using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000C80 RID: 3200
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x06004E2A RID: 20010 RVA: 0x001958E0 File Offset: 0x00193AE0
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06004E2B RID: 20011 RVA: 0x001958F0 File Offset: 0x00193AF0
	private void Update()
	{
		if (this.goingForward)
		{
			this.progress += Time.deltaTime / this.duration;
			if (this.progress > 1f)
			{
				if (this.mode == SplineWalkerMode.Once)
				{
					this.progress = 1f;
				}
				else if (this.mode == SplineWalkerMode.Loop)
				{
					this.progress -= 1f;
				}
				else
				{
					this.progress = 2f - this.progress;
					this.goingForward = false;
				}
			}
		}
		else
		{
			this.progress -= Time.deltaTime / this.duration;
			if (this.progress < 0f)
			{
				this.progress = -this.progress;
				this.goingForward = true;
			}
		}
		if (this.linearSpline != null && this.walkLinearPath)
		{
			Vector3 vector = this.linearSpline.Evaluate(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = vector;
			}
			else
			{
				base.transform.localPosition = vector;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(vector + this.linearSpline.GetForwardTangent(this.progress, 0.01f));
				return;
			}
		}
		else if (this.spline != null)
		{
			Vector3 point = this.spline.GetPoint(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = point;
			}
			else
			{
				base.transform.localPosition = point;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(point + this.spline.GetDirection(this.progress));
			}
		}
	}

	// Token: 0x06004E2C RID: 20012 RVA: 0x00195A9E File Offset: 0x00193C9E
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x04005D41 RID: 23873
	public BezierSpline spline;

	// Token: 0x04005D42 RID: 23874
	public LinearSpline linearSpline;

	// Token: 0x04005D43 RID: 23875
	public float duration;

	// Token: 0x04005D44 RID: 23876
	public bool lookForward;

	// Token: 0x04005D45 RID: 23877
	public SplineWalkerMode mode;

	// Token: 0x04005D46 RID: 23878
	public bool walkLinearPath;

	// Token: 0x04005D47 RID: 23879
	public bool useWorldPosition;

	// Token: 0x04005D48 RID: 23880
	public float progress;

	// Token: 0x04005D49 RID: 23881
	private bool goingForward = true;

	// Token: 0x04005D4A RID: 23882
	public bool DoNetworkSync = true;

	// Token: 0x04005D4B RID: 23883
	private PhotonView _view;
}
