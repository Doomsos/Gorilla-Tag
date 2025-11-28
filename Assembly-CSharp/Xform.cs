using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020009FE RID: 2558
[ExecuteAlways]
public class Xform : MonoBehaviour
{
	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x06004177 RID: 16759 RVA: 0x0015BFB7 File Offset: 0x0015A1B7
	public float3 localExtents
	{
		get
		{
			return this.localScale * 0.5f;
		}
	}

	// Token: 0x06004178 RID: 16760 RVA: 0x0015BFC9 File Offset: 0x0015A1C9
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, this.localRotation, this.localScale);
	}

	// Token: 0x06004179 RID: 16761 RVA: 0x0015BFEC File Offset: 0x0015A1EC
	public Matrix4x4 TRS()
	{
		if (this.parent.AsNull<Transform>() == null)
		{
			return this.LocalTRS();
		}
		return this.parent.localToWorldMatrix * this.LocalTRS();
	}

	// Token: 0x0600417A RID: 16762 RVA: 0x0015C020 File Offset: 0x0015A220
	private unsafe void Update()
	{
		Matrix4x4 matrix4x = this.TRS();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(matrix4x))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.PlaneWithNormal(Xform.AXIS_XR_RT * 0.5f, Xform.AXIS_XR_RT, Xform.F2_ONE, Xform.CR);
				commandBuilder.PlaneWithNormal(Xform.AXIS_YG_UP * 0.5f, Xform.AXIS_YG_UP, Xform.F2_ONE, Xform.CG);
				commandBuilder.PlaneWithNormal(Xform.AXIS_ZB_FW * 0.5f, Xform.AXIS_ZB_FW, Xform.F2_ONE, Xform.CB);
				commandBuilder.WireBox(float3.zero, quaternion.identity, 1f, this.displayColor);
			}
		}
	}

	// Token: 0x04005252 RID: 21074
	public Transform parent;

	// Token: 0x04005253 RID: 21075
	[Space]
	public Color displayColor = SRand.New().NextColor();

	// Token: 0x04005254 RID: 21076
	[Space]
	public float3 localPosition = float3.zero;

	// Token: 0x04005255 RID: 21077
	public float3 localScale = Vector3.one;

	// Token: 0x04005256 RID: 21078
	public Quaternion localRotation = quaternion.identity;

	// Token: 0x04005257 RID: 21079
	private static readonly float3 F3_ONE = 1f;

	// Token: 0x04005258 RID: 21080
	private static readonly float2 F2_ONE = 1f;

	// Token: 0x04005259 RID: 21081
	private static readonly float3 AXIS_ZB_FW = new float3(0f, 0f, 1f);

	// Token: 0x0400525A RID: 21082
	private static readonly float3 AXIS_YG_UP = new float3(0f, 1f, 0f);

	// Token: 0x0400525B RID: 21083
	private static readonly float3 AXIS_XR_RT = new float3(1f, 0f, 0f);

	// Token: 0x0400525C RID: 21084
	private static readonly Color CR = new Color(1f, 0f, 0f, 0.24f);

	// Token: 0x0400525D RID: 21085
	private static readonly Color CG = new Color(0f, 1f, 0f, 0.24f);

	// Token: 0x0400525E RID: 21086
	private static readonly Color CB = new Color(0f, 0f, 1f, 0.24f);
}
