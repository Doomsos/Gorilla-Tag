using System;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class SetAnimatorBoolCosmetic : MonoBehaviour
{
	// Token: 0x0600042B RID: 1067 RVA: 0x00002789 File Offset: 0x00000989
	private void OnAnimatorValueChanged()
	{
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x000184C2 File Offset: 0x000166C2
	public void SetAnimatorBool(bool value)
	{
		if (this.bool1Hash == 0)
		{
			this.bool1Hash = Animator.StringToHash(this.boolParameterName);
		}
		this.animator.SetBool(this.bool1Hash, value);
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x000184EF File Offset: 0x000166EF
	public void SetAnimatorBool2(bool value)
	{
		if (this.bool2Hash == 0)
		{
			this.bool2Hash = Animator.StringToHash(this.bool2ParameterName);
		}
		this.animator.SetBool(this.bool2Hash, value);
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0001851C File Offset: 0x0001671C
	public void SetAnimatorBool3(bool value)
	{
		if (this.bool3Hash == 0)
		{
			this.bool3Hash = Animator.StringToHash(this.bool3ParameterName);
		}
		this.animator.SetBool(this.bool3Hash, value);
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00018549 File Offset: 0x00016749
	public void SetAnimatorBool4(bool value)
	{
		if (this.bool4Hash == 0)
		{
			this.bool4Hash = Animator.StringToHash(this.bool4ParameterName);
		}
		this.animator.SetBool(this.bool4Hash, value);
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00018576 File Offset: 0x00016776
	public void SetAnimatorBool5(bool value)
	{
		if (this.bool5Hash == 0)
		{
			this.bool5Hash = Animator.StringToHash(this.bool5ParameterName);
		}
		this.animator.SetBool(this.bool5Hash, value);
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x000185A3 File Offset: 0x000167A3
	public void SetAnimatorInteger1(int value)
	{
		if (this.int1Hash == 0)
		{
			this.int1Hash = Animator.StringToHash(this.int1ParameterName);
		}
		this.animator.SetInteger(this.int1Hash, value);
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x000185D0 File Offset: 0x000167D0
	public void SetAnimatorInteger2(int value)
	{
		if (this.int2Hash == 0)
		{
			this.int2Hash = Animator.StringToHash(this.int2ParameterName);
		}
		this.animator.SetInteger(this.int2Hash, value);
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x000185FD File Offset: 0x000167FD
	public void SetAnimatorInteger3(int value)
	{
		if (this.int3Hash == 0)
		{
			this.int3Hash = Animator.StringToHash(this.int3ParameterName);
		}
		this.animator.SetInteger(this.int3Hash, value);
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x0001862A File Offset: 0x0001682A
	public void SetAnimatorInteger4(int value)
	{
		if (this.int4Hash == 0)
		{
			this.int4Hash = Animator.StringToHash(this.int4ParameterName);
		}
		this.animator.SetInteger(this.int4Hash, value);
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00018657 File Offset: 0x00016857
	public void SetAnimatorFloat1(float value)
	{
		if (this.float1Hash == 0)
		{
			this.float1Hash = Animator.StringToHash(this.float1ParameterName);
		}
		this.animator.SetFloat(this.float1Hash, value);
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00018684 File Offset: 0x00016884
	public void SetAnimatorFloat2(float value)
	{
		if (this.float2Hash == 0)
		{
			this.float2Hash = Animator.StringToHash(this.float2ParameterName);
		}
		this.animator.SetFloat(this.float2Hash, value);
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x000186B1 File Offset: 0x000168B1
	public void SetAnimatorFloat3(float value)
	{
		if (this.float3Hash == 0)
		{
			this.float3Hash = Animator.StringToHash(this.float3ParameterName);
		}
		this.animator.SetFloat(this.float3Hash, value);
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x000186DE File Offset: 0x000168DE
	public void SetAnimatorFloat4(float value)
	{
		if (this.float4Hash == 0)
		{
			this.float4Hash = Animator.StringToHash(this.float4ParameterName);
		}
		this.animator.SetFloat(this.float4Hash, value);
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x0001870B File Offset: 0x0001690B
	public void SetAnimatorTrigger(string triggerName)
	{
		this.animator.SetTrigger(triggerName);
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00018719 File Offset: 0x00016919
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x04000499 RID: 1177
	[SerializeField]
	private Animator animator;

	// Token: 0x0400049A RID: 1178
	[SerializeField]
	private string boolParameterName;

	// Token: 0x0400049B RID: 1179
	[SerializeField]
	private string bool2ParameterName;

	// Token: 0x0400049C RID: 1180
	[SerializeField]
	private string bool3ParameterName;

	// Token: 0x0400049D RID: 1181
	[SerializeField]
	private string bool4ParameterName;

	// Token: 0x0400049E RID: 1182
	[SerializeField]
	private string bool5ParameterName;

	// Token: 0x0400049F RID: 1183
	[SerializeField]
	private string int1ParameterName;

	// Token: 0x040004A0 RID: 1184
	[SerializeField]
	private string int2ParameterName;

	// Token: 0x040004A1 RID: 1185
	[SerializeField]
	private string int3ParameterName;

	// Token: 0x040004A2 RID: 1186
	[SerializeField]
	private string int4ParameterName;

	// Token: 0x040004A3 RID: 1187
	[SerializeField]
	private string float1ParameterName;

	// Token: 0x040004A4 RID: 1188
	[SerializeField]
	private string float2ParameterName;

	// Token: 0x040004A5 RID: 1189
	[SerializeField]
	private string float3ParameterName;

	// Token: 0x040004A6 RID: 1190
	[SerializeField]
	private string float4ParameterName;

	// Token: 0x040004A7 RID: 1191
	private int bool1Hash;

	// Token: 0x040004A8 RID: 1192
	private int bool2Hash;

	// Token: 0x040004A9 RID: 1193
	private int bool3Hash;

	// Token: 0x040004AA RID: 1194
	private int bool4Hash;

	// Token: 0x040004AB RID: 1195
	private int bool5Hash;

	// Token: 0x040004AC RID: 1196
	private const int MAX_BOOLS = 5;

	// Token: 0x040004AD RID: 1197
	private int int1Hash;

	// Token: 0x040004AE RID: 1198
	private int int2Hash;

	// Token: 0x040004AF RID: 1199
	private int int3Hash;

	// Token: 0x040004B0 RID: 1200
	private int int4Hash;

	// Token: 0x040004B1 RID: 1201
	private const int MAX_INTS = 4;

	// Token: 0x040004B2 RID: 1202
	private int float1Hash;

	// Token: 0x040004B3 RID: 1203
	private int float2Hash;

	// Token: 0x040004B4 RID: 1204
	private int float3Hash;

	// Token: 0x040004B5 RID: 1205
	private int float4Hash;

	// Token: 0x040004B6 RID: 1206
	private const int MAX_FLOATS = 4;
}
