using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000827 RID: 2087
public class MovingPlatform : BasePlatform
{
	// Token: 0x060036D8 RID: 14040 RVA: 0x001284D0 File Offset: 0x001266D0
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x001284DF File Offset: 0x001266DF
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x001284EE File Offset: 0x001266EE
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x060036DB RID: 14043 RVA: 0x00128517 File Offset: 0x00126717
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x00128528 File Offset: 0x00126728
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x00128553 File Offset: 0x00126753
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x00128564 File Offset: 0x00126764
	public float CycleCompletionPercent()
	{
		float num = (float)(this.PlatformTime() / (double)this.cycleLength);
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.startDelay > 0f)
		{
			float num2 = this.startDelay / this.cycleLength;
			if (num <= num2)
			{
				num = 0f;
			}
			else
			{
				num = (num - num2) / (1f - num2);
			}
		}
		return num;
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x001285C6 File Offset: 0x001267C6
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x001285E0 File Offset: 0x001267E0
	private void Awake()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.initLocalRotation = base.transform.localRotation;
		if (this.pivot != null)
		{
			this.initOffset = this.pivot.transform.position - this.startXf.transform.position;
		}
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x001286B0 File Offset: 0x001268B0
	private void OnEnable()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		base.transform.localRotation = this.initLocalRotation;
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x00128739 File Offset: 0x00126939
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x00128754 File Offset: 0x00126954
	private Vector3 UpdateArc()
	{
		float num = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 vector = Quaternion.AngleAxis(num, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + vector;
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x001287B2 File Offset: 0x001269B2
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x060036E5 RID: 14053 RVA: 0x001287CB File Offset: 0x001269CB
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x001287F8 File Offset: 0x001269F8
	private void SetupContext()
	{
		double time = PhotonNetwork.Time;
		if (this.lastServerTime == time)
		{
			this.dtSinceServerUpdate += Time.fixedDeltaTime;
		}
		else
		{
			this.dtSinceServerUpdate = 0f;
			this.lastServerTime = time;
		}
		float num = this.currT;
		this.currT = this.CycleCompletionPercent();
		this.currForward = this.CycleForward();
		this.percent = this.currT;
		if (this.reverseDirOnCycle)
		{
			this.percent = (this.currForward ? this.currT : (1f - this.currT));
		}
		if (this.reverseDir)
		{
			this.percent = 1f - this.percent;
		}
		this.smoothedPercent = this.percent;
		this.lastNT = time;
		this.lastT = Time.time;
	}

	// Token: 0x060036E7 RID: 14055 RVA: 0x001288C8 File Offset: 0x00126AC8
	private void Update()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.SetupContext();
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		bool flag = false;
		switch (this.platformType)
		{
		case MovingPlatform.PlatformType.PointToPoint:
			vector = this.UpdatePointToPoint();
			break;
		case MovingPlatform.PlatformType.Arc:
			vector = this.UpdateArc();
			flag = true;
			break;
		case MovingPlatform.PlatformType.Rotation:
			quaternion = this.UpdateRotation();
			flag = true;
			break;
		case MovingPlatform.PlatformType.ContinuousRotation:
			quaternion = this.UpdateContinuousRotation();
			flag = true;
			break;
		}
		if (!this.debugMovement)
		{
			this.lastPos = this.rb.position;
			this.lastRot = this.rb.rotation;
			if (this.platformType != MovingPlatform.PlatformType.Rotation)
			{
				this.rb.MovePosition(vector);
			}
			if (flag)
			{
				this.rb.MoveRotation(quaternion);
			}
		}
		else
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
			base.transform.position = vector;
			if (flag)
			{
				base.transform.rotation = quaternion;
			}
		}
		this.deltaPosition = vector - this.lastPos;
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x001289E9 File Offset: 0x00126BE9
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04004632 RID: 17970
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04004633 RID: 17971
	public float cycleLength;

	// Token: 0x04004634 RID: 17972
	public float smoothingHalflife = 0.1f;

	// Token: 0x04004635 RID: 17973
	public float rotateStartAmt;

	// Token: 0x04004636 RID: 17974
	public float rotateAmt;

	// Token: 0x04004637 RID: 17975
	public bool reverseDirOnCycle = true;

	// Token: 0x04004638 RID: 17976
	public bool reverseDir;

	// Token: 0x04004639 RID: 17977
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x0400463A RID: 17978
	private Rigidbody rb;

	// Token: 0x0400463B RID: 17979
	public Transform startXf;

	// Token: 0x0400463C RID: 17980
	public Transform endXf;

	// Token: 0x0400463D RID: 17981
	public Vector3 platformInitLocalPos;

	// Token: 0x0400463E RID: 17982
	private Vector3 startPos;

	// Token: 0x0400463F RID: 17983
	private Vector3 endPos;

	// Token: 0x04004640 RID: 17984
	private Quaternion startRot;

	// Token: 0x04004641 RID: 17985
	private Quaternion endRot;

	// Token: 0x04004642 RID: 17986
	public float startPercentage;

	// Token: 0x04004643 RID: 17987
	public float startDelay;

	// Token: 0x04004644 RID: 17988
	public bool startNextCycle;

	// Token: 0x04004645 RID: 17989
	public Transform pivot;

	// Token: 0x04004646 RID: 17990
	private Quaternion initLocalRotation;

	// Token: 0x04004647 RID: 17991
	private Vector3 initOffset;

	// Token: 0x04004648 RID: 17992
	private float currT;

	// Token: 0x04004649 RID: 17993
	private float percent;

	// Token: 0x0400464A RID: 17994
	private float smoothedPercent = -1f;

	// Token: 0x0400464B RID: 17995
	private bool currForward;

	// Token: 0x0400464C RID: 17996
	private float dtSinceServerUpdate;

	// Token: 0x0400464D RID: 17997
	private double lastServerTime;

	// Token: 0x0400464E RID: 17998
	public Vector3 currentVelocity;

	// Token: 0x0400464F RID: 17999
	public Vector3 rotationalAxis;

	// Token: 0x04004650 RID: 18000
	public float angularVelocity;

	// Token: 0x04004651 RID: 18001
	public Vector3 rotationPivot;

	// Token: 0x04004652 RID: 18002
	public Vector3 lastPos;

	// Token: 0x04004653 RID: 18003
	public Quaternion lastRot;

	// Token: 0x04004654 RID: 18004
	public Vector3 deltaPosition;

	// Token: 0x04004655 RID: 18005
	public bool debugMovement;

	// Token: 0x04004656 RID: 18006
	private double lastNT;

	// Token: 0x04004657 RID: 18007
	private float lastT;

	// Token: 0x02000828 RID: 2088
	public enum PlatformType
	{
		// Token: 0x04004659 RID: 18009
		PointToPoint,
		// Token: 0x0400465A RID: 18010
		Arc,
		// Token: 0x0400465B RID: 18011
		Rotation,
		// Token: 0x0400465C RID: 18012
		Child,
		// Token: 0x0400465D RID: 18013
		ContinuousRotation
	}
}
