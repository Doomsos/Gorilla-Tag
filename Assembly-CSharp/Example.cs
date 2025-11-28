using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class Example : MonoBehaviour
{
	// Token: 0x060003A2 RID: 930 RVA: 0x000164E4 File Offset: 0x000146E4
	private void OnDrawGizmos()
	{
		if (this.debugPoint)
		{
			DebugExtension.DrawPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale);
		}
		if (this.debugBounds)
		{
			DebugExtension.DrawBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color);
		}
		if (this.debugCircle)
		{
			DebugExtension.DrawCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius);
		}
		if (this.debugWireSphere)
		{
			Gizmos.color = this.debugWireSphere_Color;
			Gizmos.DrawWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Radius);
		}
		if (this.debugCylinder)
		{
			DebugExtension.DrawCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius);
		}
		if (this.debugCone)
		{
			DebugExtension.DrawCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle);
		}
		if (this.debugArrow)
		{
			DebugExtension.DrawArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color);
		}
		if (this.debugCapsule)
		{
			DebugExtension.DrawCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius);
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00016670 File Offset: 0x00014870
	private void Update()
	{
		DebugExtension.DebugPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale, 0f, true);
		DebugExtension.DebugBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color, 0f, true);
		DebugExtension.DebugCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius, 0f, true);
		DebugExtension.DebugWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Color, this.debugWireSphere_Radius, 0f, true);
		DebugExtension.DebugCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius, 0f, true);
		DebugExtension.DebugCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle, 0f, true);
		DebugExtension.DebugArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color, 0f, true);
		DebugExtension.DebugCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius, 0f, true);
	}

	// Token: 0x04000400 RID: 1024
	public bool debugPoint;

	// Token: 0x04000401 RID: 1025
	public Vector3 debugPoint_Position;

	// Token: 0x04000402 RID: 1026
	public float debugPoint_Scale;

	// Token: 0x04000403 RID: 1027
	public Color debugPoint_Color;

	// Token: 0x04000404 RID: 1028
	public bool debugBounds;

	// Token: 0x04000405 RID: 1029
	public Vector3 debugBounds_Position;

	// Token: 0x04000406 RID: 1030
	public Vector3 debugBounds_Size;

	// Token: 0x04000407 RID: 1031
	public Color debugBounds_Color;

	// Token: 0x04000408 RID: 1032
	public bool debugCircle;

	// Token: 0x04000409 RID: 1033
	public Vector3 debugCircle_Up;

	// Token: 0x0400040A RID: 1034
	public float debugCircle_Radius;

	// Token: 0x0400040B RID: 1035
	public Color debugCircle_Color;

	// Token: 0x0400040C RID: 1036
	public bool debugWireSphere;

	// Token: 0x0400040D RID: 1037
	public float debugWireSphere_Radius;

	// Token: 0x0400040E RID: 1038
	public Color debugWireSphere_Color;

	// Token: 0x0400040F RID: 1039
	public bool debugCylinder;

	// Token: 0x04000410 RID: 1040
	public Vector3 debugCylinder_End;

	// Token: 0x04000411 RID: 1041
	public float debugCylinder_Radius;

	// Token: 0x04000412 RID: 1042
	public Color debugCylinder_Color;

	// Token: 0x04000413 RID: 1043
	public bool debugCone;

	// Token: 0x04000414 RID: 1044
	public Vector3 debugCone_Direction;

	// Token: 0x04000415 RID: 1045
	public float debugCone_Angle;

	// Token: 0x04000416 RID: 1046
	public Color debugCone_Color;

	// Token: 0x04000417 RID: 1047
	public bool debugArrow;

	// Token: 0x04000418 RID: 1048
	public Vector3 debugArrow_Direction;

	// Token: 0x04000419 RID: 1049
	public Color debugArrow_Color;

	// Token: 0x0400041A RID: 1050
	public bool debugCapsule;

	// Token: 0x0400041B RID: 1051
	public Vector3 debugCapsule_End;

	// Token: 0x0400041C RID: 1052
	public float debugCapsule_Radius;

	// Token: 0x0400041D RID: 1053
	public Color debugCapsule_Color;
}
