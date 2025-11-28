using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001169 RID: 4457
	public class GizmosUtil
	{
		// Token: 0x0600705C RID: 28764 RVA: 0x00248A0F File Offset: 0x00246C0F
		public static void DrawLine(Vector3 v0, Vector3 v1, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(v0, v1);
		}

		// Token: 0x0600705D RID: 28765 RVA: 0x00248A20 File Offset: 0x00246C20
		public static void DrawLines(Vector3[] aVert, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < aVert.Length; i += 2)
			{
				Gizmos.DrawLine(aVert[i], aVert[i + 1]);
			}
		}

		// Token: 0x0600705E RID: 28766 RVA: 0x00248A58 File Offset: 0x00246C58
		public static void DrawLineStrip(Vector3[] aVert, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < aVert.Length; i++)
			{
				Gizmos.DrawLine(aVert[i], aVert[i + 1]);
			}
		}

		// Token: 0x0600705F RID: 28767 RVA: 0x00248A90 File Offset: 0x00246C90
		public static void DrawBox(Vector3 center, Quaternion rotation, Vector3 dimensions, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			if (dimensions.x < MathUtil.Epsilon || dimensions.y < MathUtil.Epsilon || dimensions.z < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			if (style != GizmosUtil.Style.Wireframe)
			{
				if (style - GizmosUtil.Style.FlatShaded <= 1)
				{
					mesh = PrimitiveMeshFactory.BoxFlatShaded();
				}
			}
			else
			{
				mesh = PrimitiveMeshFactory.BoxWireframe();
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, dimensions);
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, dimensions);
		}

		// Token: 0x06007060 RID: 28768 RVA: 0x00248B0C File Offset: 0x00246D0C
		public static void DrawCylinder(Vector3 center, Quaternion rotation, float height, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CylinderWireframe(numSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CylinderFlatShaded(numSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CylinderSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06007061 RID: 28769 RVA: 0x00248B94 File Offset: 0x00246D94
		public static void DrawCylinder(Vector3 point0, Vector3 point1, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 center = 0.5f * (point0 + point1);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector.normalized, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCylinder(center, rotation, magnitude, radius, numSegments, color, style);
		}

		// Token: 0x06007062 RID: 28770 RVA: 0x00248C18 File Offset: 0x00246E18
		public static void DrawSphere(Vector3 center, Quaternion rotation, float radius, int latSegments, int longSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.SphereWireframe(latSegments, longSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.SphereFlatShaded(latSegments, longSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.SphereSmoothShaded(latSegments, longSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, new Vector3(radius, radius, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, new Vector3(radius, radius, radius));
		}

		// Token: 0x06007063 RID: 28771 RVA: 0x00248C9A File Offset: 0x00246E9A
		public static void DrawSphere(Vector3 center, float radius, int latSegments, int longSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			GizmosUtil.DrawSphere(center, Quaternion.identity, radius, latSegments, longSegments, color, style);
		}

		// Token: 0x06007064 RID: 28772 RVA: 0x00248CB0 File Offset: 0x00246EB0
		public static void DrawCapsule(Vector3 center, Quaternion rotation, float height, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			Mesh mesh2 = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			}
			if (mesh == null || mesh2 == null)
			{
				return;
			}
			Vector3 vector = rotation * Vector3.up;
			Vector3 vector2 = 0.5f * (height - radius) * vector;
			Vector3 vector3 = center + vector2;
			Vector3 vector4 = center - vector2;
			Quaternion quaternion = Quaternion.AngleAxis(180f, vector) * rotation;
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, vector3, rotation, new Vector3(radius, radius, radius));
				Gizmos.DrawWireMesh(mesh, vector4, quaternion, new Vector3(-radius, -radius, radius));
				Gizmos.DrawWireMesh(mesh2, center, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, vector3, rotation, new Vector3(radius, radius, radius));
			Gizmos.DrawMesh(mesh, vector4, quaternion, new Vector3(-radius, -radius, radius));
			Gizmos.DrawMesh(mesh2, center, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06007065 RID: 28773 RVA: 0x00248E04 File Offset: 0x00247004
		public static void DrawCapsule(Vector3 point0, Vector3 point1, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 center = 0.5f * (point0 + point1);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector.normalized, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCapsule(center, rotation, magnitude, radius, latSegmentsPerCap, longSegmentsPerCap, color, style);
		}

		// Token: 0x06007066 RID: 28774 RVA: 0x00248E88 File Offset: 0x00247088
		public static void DrawCone(Vector3 baseCenter, Quaternion rotation, float height, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.ConeWireframe(numSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.ConeFlatShaded(numSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.ConeSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, baseCenter, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, baseCenter, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06007067 RID: 28775 RVA: 0x00248F10 File Offset: 0x00247110
		public static void DrawCone(Vector3 baseCenter, Vector3 top, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			Vector3 vector = top - baseCenter;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCone(baseCenter, rotation, magnitude, radius, numSegments, color, style);
		}

		// Token: 0x06007068 RID: 28776 RVA: 0x00248F7C File Offset: 0x0024717C
		public static void DrawArrow(Vector3 from, Vector3 to, float coneRadius, float coneHeight, int numSegments, float stemThickness, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			Vector3 vector = to - from;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCone(to - coneHeight * vector, rotation, coneHeight, coneRadius, numSegments, color, style);
			if (stemThickness <= 0f)
			{
				if (style != GizmosUtil.Style.Wireframe)
				{
					to -= coneHeight * vector;
				}
				GizmosUtil.DrawLine(from, to, color);
				return;
			}
			if (coneHeight < magnitude)
			{
				to -= coneHeight * vector;
				GizmosUtil.DrawCylinder(from, to, 0.5f * stemThickness, numSegments, color, style);
			}
		}

		// Token: 0x06007069 RID: 28777 RVA: 0x00249042 File Offset: 0x00247242
		public static void DrawArrow(Vector3 from, Vector3 to, float size, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			GizmosUtil.DrawArrow(from, to, 0.5f * size, size, 8, 0f, color, style);
		}

		// Token: 0x0200116A RID: 4458
		public enum Style
		{
			// Token: 0x0400809E RID: 32926
			Wireframe,
			// Token: 0x0400809F RID: 32927
			FlatShaded,
			// Token: 0x040080A0 RID: 32928
			SmoothShaded
		}
	}
}
