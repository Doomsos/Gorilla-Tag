using System;
using UnityEngine;

// Token: 0x020009D0 RID: 2512
[Serializable]
public struct FrameStamp
{
	// Token: 0x170005F4 RID: 1524
	// (get) Token: 0x06004025 RID: 16421 RVA: 0x00158639 File Offset: 0x00156839
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06004026 RID: 16422 RVA: 0x00158648 File Offset: 0x00156848
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06004027 RID: 16423 RVA: 0x0015866A File Offset: 0x0015686A
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06004028 RID: 16424 RVA: 0x00158681 File Offset: 0x00156881
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x06004029 RID: 16425 RVA: 0x0015868E File Offset: 0x0015688E
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x0600402A RID: 16426 RVA: 0x00158698 File Offset: 0x00156898
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
