using System;
using UnityEngine;

// Token: 0x020009D0 RID: 2512
[Serializable]
public struct FrameStamp
{
	// Token: 0x170005F4 RID: 1524
	// (get) Token: 0x06004025 RID: 16421 RVA: 0x00158659 File Offset: 0x00156859
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06004026 RID: 16422 RVA: 0x00158668 File Offset: 0x00156868
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06004027 RID: 16423 RVA: 0x0015868A File Offset: 0x0015688A
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06004028 RID: 16424 RVA: 0x001586A1 File Offset: 0x001568A1
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x06004029 RID: 16425 RVA: 0x001586AE File Offset: 0x001568AE
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x0600402A RID: 16426 RVA: 0x001586B8 File Offset: 0x001568B8
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x04005149 RID: 20809
	private int _lastFrame;
}
