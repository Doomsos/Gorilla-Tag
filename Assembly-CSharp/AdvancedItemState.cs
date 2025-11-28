using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000496 RID: 1174
[Serializable]
public class AdvancedItemState
{
	// Token: 0x06001E20 RID: 7712 RVA: 0x0009F38A File Offset: 0x0009D58A
	public void Encode()
	{
		this._encodedValue = this.EncodeData();
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x0009F398 File Offset: 0x0009D598
	public void Decode()
	{
		AdvancedItemState advancedItemState = this.DecodeData(this._encodedValue);
		this.index = advancedItemState.index;
		this.preData = advancedItemState.preData;
		this.limitAxis = advancedItemState.limitAxis;
		this.reverseGrip = advancedItemState.reverseGrip;
		this.angle = advancedItemState.angle;
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x0009F3F0 File Offset: 0x0009D5F0
	public Quaternion GetQuaternion()
	{
		Vector3 one = Vector3.one;
		if (this.reverseGrip)
		{
			switch (this.limitAxis)
			{
			case LimitAxis.NoMovement:
				return Quaternion.identity;
			case LimitAxis.YAxis:
				return Quaternion.identity;
			case LimitAxis.XAxis:
			case LimitAxis.ZAxis:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Quaternion.identity;
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x0009F444 File Offset: 0x0009D644
	[return: TupleElementNames(new string[]
	{
		"grabPointIndex",
		"YRotation",
		"XRotation",
		"ZRotation"
	})]
	public ValueTuple<int, float, float, float> DecodeAdvancedItemState(int encodedValue)
	{
		int num = encodedValue >> 21 & 255;
		float num2 = (float)(encodedValue >> 14 & 127) / 128f * 360f;
		float num3 = (float)(encodedValue >> 7 & 127) / 128f * 360f;
		float num4 = (float)(encodedValue & 127) / 128f * 360f;
		return new ValueTuple<int, float, float, float>(num, num2, num3, num4);
	}

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001E24 RID: 7716 RVA: 0x0009F49E File Offset: 0x0009D69E
	private float EncodedDeltaRotation
	{
		get
		{
			return this.GetEncodedDeltaRotation();
		}
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x0009F4A6 File Offset: 0x0009D6A6
	public float GetEncodedDeltaRotation()
	{
		return Mathf.Abs(Mathf.Atan2(this.angleVectorWhereUpIsStandard.x, this.angleVectorWhereUpIsStandard.y)) / 3.1415927f;
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x0009F4D0 File Offset: 0x0009D6D0
	public void DecodeDeltaRotation(float encodedDelta, bool isFlipped)
	{
		float num = encodedDelta * 3.1415927f;
		if (isFlipped)
		{
			this.angleVectorWhereUpIsStandard = new Vector2(-Mathf.Sin(num), Mathf.Cos(num));
		}
		else
		{
			this.angleVectorWhereUpIsStandard = new Vector2(Mathf.Sin(num), Mathf.Cos(num));
		}
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
		case LimitAxis.XAxis:
		case LimitAxis.ZAxis:
			return;
		case LimitAxis.YAxis:
		{
			Vector3 vector;
			vector..ctor(this.angleVectorWhereUpIsStandard.x, 0f, this.angleVectorWhereUpIsStandard.y);
			Vector3 vector2 = this.reverseGrip ? Vector3.down : Vector3.up;
			this.deltaRotation = Quaternion.LookRotation(vector, vector2);
			return;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x0009F584 File Offset: 0x0009D784
	public int EncodeData()
	{
		int num = 0;
		if (this.index >= 32 | this.index < 0)
		{
			throw new ArgumentOutOfRangeException(string.Format("Index is invalid {0}", this.index));
		}
		num |= this.index << 25;
		AdvancedItemState.PointType pointType = this.preData.pointType;
		num |= (int)((int)(pointType & (AdvancedItemState.PointType)7) << 22);
		num |= (int)((int)this.limitAxis << 19);
		num |= (this.reverseGrip ? 1 : 0) << 18;
		bool flag = this.angleVectorWhereUpIsStandard.x < 0f;
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num2 = (int)(this.GetEncodedDeltaRotation() * 512f) & 511;
			num |= (flag ? 1 : 0) << 17;
			num |= num2 << 9;
			int num3 = (int)(this.preData.distAlongLine * 256f) & 255;
			num |= num3;
		}
		else
		{
			int num4 = (int)(this.GetEncodedDeltaRotation() * 65536f) & 65535;
			num |= (flag ? 1 : 0) << 17;
			num |= num4 << 1;
		}
		return num;
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x0009F6A0 File Offset: 0x0009D8A0
	public AdvancedItemState DecodeData(int encoded)
	{
		AdvancedItemState advancedItemState = new AdvancedItemState();
		advancedItemState.index = (encoded >> 25 & 31);
		advancedItemState.limitAxis = (LimitAxis)(encoded >> 19 & 7);
		advancedItemState.reverseGrip = ((encoded >> 18 & 1) == 1);
		AdvancedItemState.PointType pointType = (AdvancedItemState.PointType)(encoded >> 22 & 7);
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType,
				distAlongLine = (float)(encoded & 255) / 256f
			};
			this.DecodeDeltaRotation((float)(encoded >> 9 & 511) / 512f, (encoded >> 17 & 1) > 0);
		}
		else
		{
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType
			};
			this.DecodeDeltaRotation((float)(encoded >> 1 & 65535) / 65536f, (encoded >> 17 & 1) > 0);
		}
		return advancedItemState;
	}

	// Token: 0x04002851 RID: 10321
	private int _encodedValue;

	// Token: 0x04002852 RID: 10322
	public Vector2 angleVectorWhereUpIsStandard;

	// Token: 0x04002853 RID: 10323
	public Quaternion deltaRotation;

	// Token: 0x04002854 RID: 10324
	public int index;

	// Token: 0x04002855 RID: 10325
	public AdvancedItemState.PreData preData;

	// Token: 0x04002856 RID: 10326
	public LimitAxis limitAxis;

	// Token: 0x04002857 RID: 10327
	public bool reverseGrip;

	// Token: 0x04002858 RID: 10328
	public float angle;

	// Token: 0x02000497 RID: 1175
	[Serializable]
	public class PreData
	{
		// Token: 0x04002859 RID: 10329
		public float distAlongLine;

		// Token: 0x0400285A RID: 10330
		public AdvancedItemState.PointType pointType;
	}

	// Token: 0x02000498 RID: 1176
	public enum PointType
	{
		// Token: 0x0400285C RID: 10332
		Standard,
		// Token: 0x0400285D RID: 10333
		DistanceBased
	}
}
