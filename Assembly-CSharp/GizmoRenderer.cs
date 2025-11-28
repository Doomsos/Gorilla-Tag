using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020009D1 RID: 2513
public class GizmoRenderer : MonoBehaviour
{
	// Token: 0x0600402B RID: 16427 RVA: 0x001586DC File Offset: 0x001568DC
	private void Update()
	{
		this.RenderGizmos();
	}

	// Token: 0x0600402C RID: 16428 RVA: 0x001586E4 File Offset: 0x001568E4
	private unsafe void RenderGizmos()
	{
		if (this.renderMode == GizmoRenderer.RenderMode.Never)
		{
			return;
		}
		if (this.gizmos == null)
		{
			return;
		}
		int num = this.gizmos.Length;
		if (num == 0)
		{
			return;
		}
		CommandBuilder commandBuilder = *Draw.ingame;
		Transform transform = base.transform;
		for (int i = 0; i < num; i++)
		{
			GizmoRenderer.GizmoInfo gizmoInfo = this.gizmos[i];
			if (gizmoInfo.render)
			{
				Transform transform2 = gizmoInfo.target ? gizmoInfo.target : transform;
				using (commandBuilder.InLocalSpace(transform2))
				{
					using (commandBuilder.WithLineWidth(gizmoInfo.lineWidth, false))
					{
						GizmoRenderer.gRenderFuncs[(int)gizmoInfo.type].Invoke(commandBuilder, gizmoInfo);
					}
				}
			}
		}
	}

	// Token: 0x0600402D RID: 16429 RVA: 0x001587D0 File Offset: 0x001569D0
	private static void RenderPlaneWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WirePlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x0600402E RID: 16430 RVA: 0x001587F6 File Offset: 0x001569F6
	private static void RenderPlaneSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidPlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x0600402F RID: 16431 RVA: 0x0015881C File Offset: 0x00156A1C
	private static void RenderGridWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireGrid(gizmo.center, gizmo.rotation, gizmo.gridCells, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06004030 RID: 16432 RVA: 0x00158848 File Offset: 0x00156A48
	private static void RenderBoxWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x06004031 RID: 16433 RVA: 0x00158869 File Offset: 0x00156A69
	private static void RenderBoxSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x06004032 RID: 16434 RVA: 0x0015888A File Offset: 0x00156A8A
	private static void RenderSphereWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireSphere(gizmo.center, gizmo.radius * 0.5f, gizmo.color);
	}

	// Token: 0x06004033 RID: 16435 RVA: 0x001588AC File Offset: 0x00156AAC
	private static void RenderSphereSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		Matrix4x4 matrix4x = Matrix4x4.TRS(gizmo.center, quaternion.identity, new float3(gizmo.radius));
		using (draw.WithMatrix(matrix4x))
		{
			draw.SolidMesh(GizmoRenderer.gSphereMesh, gizmo.color);
		}
	}

	// Token: 0x06004034 RID: 16436 RVA: 0x00158920 File Offset: 0x00156B20
	private static void RenderLabel3D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label3D(gizmo.center, gizmo.rotation, gizmo.text, gizmo.textSize * 0.1f, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x06004035 RID: 16437 RVA: 0x0015895D File Offset: 0x00156B5D
	private static void RenderLabel2D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label2D(gizmo.center, gizmo.text, gizmo.textSize * gizmo.textPPU, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x06004036 RID: 16438 RVA: 0x00158997 File Offset: 0x00156B97
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitializeOnLoad()
	{
		GizmoRenderer.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
	}

	// Token: 0x06004037 RID: 16439 RVA: 0x001589A8 File Offset: 0x00156BA8
	private static Color GetRandomColor()
	{
		Color result = Color.HSVToRGB((float)(DateTime.UtcNow.Ticks % 65536L) / 65535f, 1f, 1f, true);
		result.a = 1f;
		return result;
	}

	// Token: 0x0400514A RID: 20810
	public GizmoRenderer.RenderMode renderMode = GizmoRenderer.RenderMode.Always;

	// Token: 0x0400514B RID: 20811
	public bool includeInBuild;

	// Token: 0x0400514C RID: 20812
	public GizmoRenderer.GizmoInfo[] gizmos = new GizmoRenderer.GizmoInfo[0];

	// Token: 0x0400514D RID: 20813
	private static readonly Action<CommandBuilder, GizmoRenderer.GizmoInfo>[] gRenderFuncs = new Action<CommandBuilder, GizmoRenderer.GizmoInfo>[]
	{
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel3D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel2D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderGridWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneWire)
	};

	// Token: 0x0400514E RID: 20814
	private static readonly LabelAlignment[] gLabelAligns = new LabelAlignment[]
	{
		LabelAlignment.Center,
		LabelAlignment.MiddleRight,
		LabelAlignment.MiddleLeft,
		LabelAlignment.BottomCenter,
		LabelAlignment.BottomRight,
		LabelAlignment.BottomLeft,
		LabelAlignment.TopRight,
		LabelAlignment.TopLeft,
		LabelAlignment.TopCenter
	};

	// Token: 0x0400514F RID: 20815
	private static Mesh gSphereMesh;

	// Token: 0x020009D2 RID: 2514
	[Serializable]
	public class GizmoInfo
	{
		// Token: 0x04005150 RID: 20816
		public bool render = true;

		// Token: 0x04005151 RID: 20817
		public GizmoRenderer.GizmoType type;

		// Token: 0x04005152 RID: 20818
		public Color color = GizmoRenderer.GetRandomColor();

		// Token: 0x04005153 RID: 20819
		public uint lineWidth = 1U;

		// Token: 0x04005154 RID: 20820
		[Space]
		public Transform target;

		// Token: 0x04005155 RID: 20821
		[Space]
		public float3 center = float3.zero;

		// Token: 0x04005156 RID: 20822
		public float3 size = Vector3.one;

		// Token: 0x04005157 RID: 20823
		public float radius = 1f;

		// Token: 0x04005158 RID: 20824
		public quaternion rotation = quaternion.identity;

		// Token: 0x04005159 RID: 20825
		[Space]
		public string text = string.Empty;

		// Token: 0x0400515A RID: 20826
		public float textSize = 4f;

		// Token: 0x0400515B RID: 20827
		public GizmoRenderer.TextAlign textAlign;

		// Token: 0x0400515C RID: 20828
		public uint textPPU = 24U;

		// Token: 0x0400515D RID: 20829
		[Space]
		public int2 gridCells = new int2(4);
	}

	// Token: 0x020009D3 RID: 2515
	[Flags]
	public enum RenderMode : uint
	{
		// Token: 0x0400515F RID: 20831
		Never = 0U,
		// Token: 0x04005160 RID: 20832
		InEditor = 1U,
		// Token: 0x04005161 RID: 20833
		InBuild = 2U,
		// Token: 0x04005162 RID: 20834
		Always = 3U
	}

	// Token: 0x020009D4 RID: 2516
	public enum GizmoType : uint
	{
		// Token: 0x04005164 RID: 20836
		BoxWire,
		// Token: 0x04005165 RID: 20837
		BoxSolid,
		// Token: 0x04005166 RID: 20838
		SphereWire,
		// Token: 0x04005167 RID: 20839
		SphereSolid,
		// Token: 0x04005168 RID: 20840
		Label3D,
		// Token: 0x04005169 RID: 20841
		Label2D,
		// Token: 0x0400516A RID: 20842
		GridWire,
		// Token: 0x0400516B RID: 20843
		PlaneSolid,
		// Token: 0x0400516C RID: 20844
		PlaneWire
	}

	// Token: 0x020009D5 RID: 2517
	public enum TextAlign : uint
	{
		// Token: 0x0400516E RID: 20846
		Center,
		// Token: 0x0400516F RID: 20847
		MiddleRight,
		// Token: 0x04005170 RID: 20848
		MiddleLeft,
		// Token: 0x04005171 RID: 20849
		BottomCenter,
		// Token: 0x04005172 RID: 20850
		BottomRight,
		// Token: 0x04005173 RID: 20851
		BottomLeft,
		// Token: 0x04005174 RID: 20852
		TopRight,
		// Token: 0x04005175 RID: 20853
		TopLeft,
		// Token: 0x04005176 RID: 20854
		TopCenter
	}
}
