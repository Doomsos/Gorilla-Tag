using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020009E4 RID: 2532
[Serializable]
[StructLayout(2, Size = 64)]
public struct m4x4
{
	// Token: 0x0600408E RID: 16526 RVA: 0x00159654 File Offset: 0x00157854
	public m4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
	{
		this = default(m4x4);
		this.m00 = m00;
		this.m01 = m01;
		this.m02 = m02;
		this.m03 = m03;
		this.m10 = m10;
		this.m11 = m11;
		this.m12 = m12;
		this.m13 = m13;
		this.m20 = m20;
		this.m21 = m21;
		this.m22 = m22;
		this.m23 = m23;
		this.m30 = m30;
		this.m31 = m31;
		this.m32 = m32;
		this.m33 = m33;
	}

	// Token: 0x0600408F RID: 16527 RVA: 0x001596E5 File Offset: 0x001578E5
	public m4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
	{
		this = default(m4x4);
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06004090 RID: 16528 RVA: 0x0015970C File Offset: 0x0015790C
	public void Clear()
	{
		this.m00 = 0f;
		this.m01 = 0f;
		this.m02 = 0f;
		this.m03 = 0f;
		this.m10 = 0f;
		this.m11 = 0f;
		this.m12 = 0f;
		this.m13 = 0f;
		this.m20 = 0f;
		this.m21 = 0f;
		this.m22 = 0f;
		this.m23 = 0f;
		this.m30 = 0f;
		this.m31 = 0f;
		this.m32 = 0f;
		this.m33 = 0f;
	}

	// Token: 0x06004091 RID: 16529 RVA: 0x001597C9 File Offset: 0x001579C9
	public void SetRow0(ref Vector4 v)
	{
		this.m00 = v.x;
		this.m01 = v.y;
		this.m02 = v.z;
		this.m03 = v.w;
	}

	// Token: 0x06004092 RID: 16530 RVA: 0x001597FB File Offset: 0x001579FB
	public void SetRow1(ref Vector4 v)
	{
		this.m10 = v.x;
		this.m11 = v.y;
		this.m12 = v.z;
		this.m13 = v.w;
	}

	// Token: 0x06004093 RID: 16531 RVA: 0x0015982D File Offset: 0x00157A2D
	public void SetRow2(ref Vector4 v)
	{
		this.m20 = v.x;
		this.m21 = v.y;
		this.m22 = v.z;
		this.m23 = v.w;
	}

	// Token: 0x06004094 RID: 16532 RVA: 0x0015985F File Offset: 0x00157A5F
	public void SetRow3(ref Vector4 v)
	{
		this.m30 = v.x;
		this.m31 = v.y;
		this.m32 = v.z;
		this.m33 = v.w;
	}

	// Token: 0x06004095 RID: 16533 RVA: 0x00159894 File Offset: 0x00157A94
	public void Transpose()
	{
		float num = this.m01;
		float num2 = this.m02;
		float num3 = this.m03;
		float num4 = this.m10;
		float num5 = this.m12;
		float num6 = this.m13;
		float num7 = this.m20;
		float num8 = this.m21;
		float num9 = this.m23;
		float num10 = this.m30;
		float num11 = this.m31;
		float num12 = this.m32;
		this.m01 = num4;
		this.m02 = num7;
		this.m03 = num10;
		this.m10 = num;
		this.m12 = num8;
		this.m13 = num11;
		this.m20 = num2;
		this.m21 = num5;
		this.m23 = num12;
		this.m30 = num3;
		this.m31 = num6;
		this.m32 = num9;
	}

	// Token: 0x06004096 RID: 16534 RVA: 0x00159959 File Offset: 0x00157B59
	public void Set(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06004097 RID: 16535 RVA: 0x0015998C File Offset: 0x00157B8C
	public void SetTransposed(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.m00 = row0.x;
		this.m01 = row1.x;
		this.m02 = row2.x;
		this.m03 = row3.x;
		this.m10 = row0.y;
		this.m11 = row1.y;
		this.m12 = row2.y;
		this.m13 = row3.y;
		this.m20 = row0.z;
		this.m21 = row1.z;
		this.m22 = row2.z;
		this.m23 = row3.z;
		this.m30 = row0.w;
		this.m31 = row1.w;
		this.m32 = row2.w;
		this.m33 = row3.w;
	}

	// Token: 0x06004098 RID: 16536 RVA: 0x00159A60 File Offset: 0x00157C60
	public void Set(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m01;
		this.m02 = x.m02;
		this.m03 = x.m03;
		this.m10 = x.m10;
		this.m11 = x.m11;
		this.m12 = x.m12;
		this.m13 = x.m13;
		this.m20 = x.m20;
		this.m21 = x.m21;
		this.m22 = x.m22;
		this.m23 = x.m23;
		this.m30 = x.m30;
		this.m31 = x.m31;
		this.m32 = x.m32;
		this.m33 = x.m33;
	}

	// Token: 0x06004099 RID: 16537 RVA: 0x00159B30 File Offset: 0x00157D30
	public void SetTransposed(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m10;
		this.m02 = x.m20;
		this.m03 = x.m30;
		this.m10 = x.m01;
		this.m11 = x.m11;
		this.m12 = x.m21;
		this.m13 = x.m31;
		this.m20 = x.m02;
		this.m21 = x.m12;
		this.m22 = x.m22;
		this.m23 = x.m32;
		this.m30 = x.m03;
		this.m31 = x.m13;
		this.m32 = x.m23;
		this.m33 = x.m33;
	}

	// Token: 0x0600409A RID: 16538 RVA: 0x00159C00 File Offset: 0x00157E00
	public void Push(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m01;
		x.m02 = this.m02;
		x.m03 = this.m03;
		x.m10 = this.m10;
		x.m11 = this.m11;
		x.m12 = this.m12;
		x.m13 = this.m13;
		x.m20 = this.m20;
		x.m21 = this.m21;
		x.m22 = this.m22;
		x.m23 = this.m23;
		x.m30 = this.m30;
		x.m31 = this.m31;
		x.m32 = this.m32;
		x.m33 = this.m33;
	}

	// Token: 0x0600409B RID: 16539 RVA: 0x00159CD0 File Offset: 0x00157ED0
	public void PushTransposed(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m10;
		x.m02 = this.m20;
		x.m03 = this.m30;
		x.m10 = this.m01;
		x.m11 = this.m11;
		x.m12 = this.m21;
		x.m13 = this.m31;
		x.m20 = this.m02;
		x.m21 = this.m12;
		x.m22 = this.m22;
		x.m23 = this.m32;
		x.m30 = this.m03;
		x.m31 = this.m13;
		x.m32 = this.m23;
		x.m33 = this.m33;
	}

	// Token: 0x0600409C RID: 16540 RVA: 0x00159D9D File Offset: 0x00157F9D
	public static ref m4x4 From(ref Matrix4x4 src)
	{
		return Unsafe.As<Matrix4x4, m4x4>(ref src);
	}

	// Token: 0x040051A3 RID: 20899
	[FixedBuffer(typeof(float), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(30, SizeConst = 16)]
	public m4x4.<data_f>e__FixedBuffer data_f;

	// Token: 0x040051A4 RID: 20900
	[FixedBuffer(typeof(int), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(30, SizeConst = 16)]
	public m4x4.<data_i>e__FixedBuffer data_i;

	// Token: 0x040051A5 RID: 20901
	[FixedBuffer(typeof(ushort), 32)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(30, SizeConst = 32)]
	public m4x4.<data_h>e__FixedBuffer data_h;

	// Token: 0x040051A6 RID: 20902
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 r0;

	// Token: 0x040051A7 RID: 20903
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 r1;

	// Token: 0x040051A8 RID: 20904
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 r2;

	// Token: 0x040051A9 RID: 20905
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 r3;

	// Token: 0x040051AA RID: 20906
	[NonSerialized]
	[FieldOffset(0)]
	public float m00;

	// Token: 0x040051AB RID: 20907
	[NonSerialized]
	[FieldOffset(4)]
	public float m01;

	// Token: 0x040051AC RID: 20908
	[NonSerialized]
	[FieldOffset(8)]
	public float m02;

	// Token: 0x040051AD RID: 20909
	[NonSerialized]
	[FieldOffset(12)]
	public float m03;

	// Token: 0x040051AE RID: 20910
	[NonSerialized]
	[FieldOffset(16)]
	public float m10;

	// Token: 0x040051AF RID: 20911
	[NonSerialized]
	[FieldOffset(20)]
	public float m11;

	// Token: 0x040051B0 RID: 20912
	[NonSerialized]
	[FieldOffset(24)]
	public float m12;

	// Token: 0x040051B1 RID: 20913
	[NonSerialized]
	[FieldOffset(28)]
	public float m13;

	// Token: 0x040051B2 RID: 20914
	[NonSerialized]
	[FieldOffset(32)]
	public float m20;

	// Token: 0x040051B3 RID: 20915
	[NonSerialized]
	[FieldOffset(36)]
	public float m21;

	// Token: 0x040051B4 RID: 20916
	[NonSerialized]
	[FieldOffset(40)]
	public float m22;

	// Token: 0x040051B5 RID: 20917
	[NonSerialized]
	[FieldOffset(44)]
	public float m23;

	// Token: 0x040051B6 RID: 20918
	[NonSerialized]
	[FieldOffset(48)]
	public float m30;

	// Token: 0x040051B7 RID: 20919
	[NonSerialized]
	[FieldOffset(52)]
	public float m31;

	// Token: 0x040051B8 RID: 20920
	[NonSerialized]
	[FieldOffset(56)]
	public float m32;

	// Token: 0x040051B9 RID: 20921
	[NonSerialized]
	[FieldOffset(60)]
	public float m33;

	// Token: 0x040051BA RID: 20922
	[HideInInspector]
	[FieldOffset(0)]
	public int i00;

	// Token: 0x040051BB RID: 20923
	[HideInInspector]
	[FieldOffset(4)]
	public int i01;

	// Token: 0x040051BC RID: 20924
	[HideInInspector]
	[FieldOffset(8)]
	public int i02;

	// Token: 0x040051BD RID: 20925
	[HideInInspector]
	[FieldOffset(12)]
	public int i03;

	// Token: 0x040051BE RID: 20926
	[HideInInspector]
	[FieldOffset(16)]
	public int i10;

	// Token: 0x040051BF RID: 20927
	[HideInInspector]
	[FieldOffset(20)]
	public int i11;

	// Token: 0x040051C0 RID: 20928
	[HideInInspector]
	[FieldOffset(24)]
	public int i12;

	// Token: 0x040051C1 RID: 20929
	[HideInInspector]
	[FieldOffset(28)]
	public int i13;

	// Token: 0x040051C2 RID: 20930
	[HideInInspector]
	[FieldOffset(32)]
	public int i20;

	// Token: 0x040051C3 RID: 20931
	[HideInInspector]
	[FieldOffset(36)]
	public int i21;

	// Token: 0x040051C4 RID: 20932
	[HideInInspector]
	[FieldOffset(40)]
	public int i22;

	// Token: 0x040051C5 RID: 20933
	[HideInInspector]
	[FieldOffset(44)]
	public int i23;

	// Token: 0x040051C6 RID: 20934
	[HideInInspector]
	[FieldOffset(48)]
	public int i30;

	// Token: 0x040051C7 RID: 20935
	[HideInInspector]
	[FieldOffset(52)]
	public int i31;

	// Token: 0x040051C8 RID: 20936
	[HideInInspector]
	[FieldOffset(56)]
	public int i32;

	// Token: 0x040051C9 RID: 20937
	[HideInInspector]
	[FieldOffset(60)]
	public int i33;

	// Token: 0x040051CA RID: 20938
	[NonSerialized]
	[FieldOffset(0)]
	public ushort h00_a;

	// Token: 0x040051CB RID: 20939
	[NonSerialized]
	[FieldOffset(2)]
	public ushort h00_b;

	// Token: 0x040051CC RID: 20940
	[NonSerialized]
	[FieldOffset(4)]
	public ushort h01_a;

	// Token: 0x040051CD RID: 20941
	[NonSerialized]
	[FieldOffset(6)]
	public ushort h01_b;

	// Token: 0x040051CE RID: 20942
	[NonSerialized]
	[FieldOffset(8)]
	public ushort h02_a;

	// Token: 0x040051CF RID: 20943
	[NonSerialized]
	[FieldOffset(10)]
	public ushort h02_b;

	// Token: 0x040051D0 RID: 20944
	[NonSerialized]
	[FieldOffset(12)]
	public ushort h03_a;

	// Token: 0x040051D1 RID: 20945
	[NonSerialized]
	[FieldOffset(14)]
	public ushort h03_b;

	// Token: 0x040051D2 RID: 20946
	[NonSerialized]
	[FieldOffset(16)]
	public ushort h10_a;

	// Token: 0x040051D3 RID: 20947
	[NonSerialized]
	[FieldOffset(18)]
	public ushort h10_b;

	// Token: 0x040051D4 RID: 20948
	[NonSerialized]
	[FieldOffset(20)]
	public ushort h11_a;

	// Token: 0x040051D5 RID: 20949
	[NonSerialized]
	[FieldOffset(22)]
	public ushort h11_b;

	// Token: 0x040051D6 RID: 20950
	[NonSerialized]
	[FieldOffset(24)]
	public ushort h12_a;

	// Token: 0x040051D7 RID: 20951
	[NonSerialized]
	[FieldOffset(26)]
	public ushort h12_b;

	// Token: 0x040051D8 RID: 20952
	[NonSerialized]
	[FieldOffset(28)]
	public ushort h13_a;

	// Token: 0x040051D9 RID: 20953
	[NonSerialized]
	[FieldOffset(30)]
	public ushort h13_b;

	// Token: 0x040051DA RID: 20954
	[NonSerialized]
	[FieldOffset(32)]
	public ushort h20_a;

	// Token: 0x040051DB RID: 20955
	[NonSerialized]
	[FieldOffset(34)]
	public ushort h20_b;

	// Token: 0x040051DC RID: 20956
	[NonSerialized]
	[FieldOffset(36)]
	public ushort h21_a;

	// Token: 0x040051DD RID: 20957
	[NonSerialized]
	[FieldOffset(38)]
	public ushort h21_b;

	// Token: 0x040051DE RID: 20958
	[NonSerialized]
	[FieldOffset(40)]
	public ushort h22_a;

	// Token: 0x040051DF RID: 20959
	[NonSerialized]
	[FieldOffset(42)]
	public ushort h22_b;

	// Token: 0x040051E0 RID: 20960
	[NonSerialized]
	[FieldOffset(44)]
	public ushort h23_a;

	// Token: 0x040051E1 RID: 20961
	[NonSerialized]
	[FieldOffset(46)]
	public ushort h23_b;

	// Token: 0x040051E2 RID: 20962
	[NonSerialized]
	[FieldOffset(48)]
	public ushort h30_a;

	// Token: 0x040051E3 RID: 20963
	[NonSerialized]
	[FieldOffset(50)]
	public ushort h30_b;

	// Token: 0x040051E4 RID: 20964
	[NonSerialized]
	[FieldOffset(52)]
	public ushort h31_a;

	// Token: 0x040051E5 RID: 20965
	[NonSerialized]
	[FieldOffset(54)]
	public ushort h31_b;

	// Token: 0x040051E6 RID: 20966
	[NonSerialized]
	[FieldOffset(56)]
	public ushort h32_a;

	// Token: 0x040051E7 RID: 20967
	[NonSerialized]
	[FieldOffset(58)]
	public ushort h32_b;

	// Token: 0x040051E8 RID: 20968
	[NonSerialized]
	[FieldOffset(60)]
	public ushort h33_a;

	// Token: 0x040051E9 RID: 20969
	[NonSerialized]
	[FieldOffset(62)]
	public ushort h33_b;

	// Token: 0x020009E5 RID: 2533
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(0, Size = 64)]
	public struct <data_f>e__FixedBuffer
	{
		// Token: 0x040051EA RID: 20970
		public float FixedElementField;
	}

	// Token: 0x020009E6 RID: 2534
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(0, Size = 64)]
	public struct <data_h>e__FixedBuffer
	{
		// Token: 0x040051EB RID: 20971
		public ushort FixedElementField;
	}

	// Token: 0x020009E7 RID: 2535
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(0, Size = 64)]
	public struct <data_i>e__FixedBuffer
	{
		// Token: 0x040051EC RID: 20972
		public int FixedElementField;
	}
}
