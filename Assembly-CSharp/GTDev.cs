using System;
using System.Diagnostics;
using Cysharp.Text;
using Drawing;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public static class GTDev
{
	// Token: 0x06001181 RID: 4481 RVA: 0x0005C793 File Offset: 0x0005A993
	[RuntimeInitializeOnLoadMethod(3)]
	private static void InitializeOnLoad()
	{
		GTDev.FetchDevID();
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void Log<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void Log<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void LogError<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void LogError<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void LogWarning<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void LogWarning<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void LogSilent<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	public static void LogSilent<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogBetaOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogBetaOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorEd<T>(T msg, string channel = null)
	{
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorEd<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorBeta<T>(T msg, string channel = null)
	{
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorBeta<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06001193 RID: 4499 RVA: 0x0005C79B File Offset: 0x0005A99B
	public static int DevID
	{
		get
		{
			return GTDev.FetchDevID();
		}
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0005C7A4 File Offset: 0x0005A9A4
	private static int FetchDevID()
	{
		if (GTDev.gHasDevID)
		{
			return GTDev.gDevID;
		}
		int i = StaticHash.Compute(SystemInfo.deviceUniqueIdentifier);
		int i2 = StaticHash.Compute(Environment.UserDomainName);
		int i3 = StaticHash.Compute(Environment.UserName);
		int i4 = StaticHash.Compute(Application.unityVersion);
		GTDev.gDevID = StaticHash.Compute(i, i2, i3, i4);
		GTDev.gHasDevID = true;
		return GTDev.gDevID;
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("_GTDEV_ON_")]
	private static void _Log<T>(Action<object, Object> log, Action<object> logNoCtx, T msg, Object ctx, string channel)
	{
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x0005C801 File Offset: 0x0005AA01
	private static Mesh SphereMesh()
	{
		if (!GTDev.gSphereMesh)
		{
			GTDev.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
		}
		return GTDev.gSphereMesh;
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0005C824 File Offset: 0x0005AA24
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Collider col, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		if (color.a.Approx0(1E-06f))
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = col.transform.localToWorldMatrix;
		SRand srand = new SRand(localToWorldMatrix.QuantizedId128().GetHashCode());
		color.r = srand.NextFloat();
		color.g = srand.NextFloat();
		color.b = srand.NextFloat();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.PushMatrix(localToWorldMatrix);
			commandBuilder.PushLineWidth(2f, true);
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = col as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = col as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = col as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius, color);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius, color);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.Label2D(Vector3.zero, col.name, 16f, LabelAlignment.Center);
			commandBuilder.PopColor();
			commandBuilder.PopLineWidth();
			commandBuilder.PopMatrix();
		}
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x0005C9C8 File Offset: 0x0005ABC8
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Vector3 vec, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		else
		{
			color.a = GTDev.gDefaultColor.a;
		}
		string text = ZString.Format<float, float, float>("{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}", vec.x, vec.y, vec.z);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.Cross(vec, 0.64f, color);
			}
			commandBuilder.Label2D(vec + Vector3.down * 0.64f, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0005CABC File Offset: 0x0005ACBC
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D<T>(this T value, Vector3 position, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		string text = ZString.Concat<T>(value);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.Label2D(position, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x04001609 RID: 5641
	[OnEnterPlay_Set(0)]
	private static int gDevID;

	// Token: 0x0400160A RID: 5642
	[OnEnterPlay_Set(false)]
	private static bool gHasDevID;

	// Token: 0x0400160B RID: 5643
	private static readonly Color gDefaultColor = new Color(0f, 1f, 1f, 0.32f);

	// Token: 0x0400160C RID: 5644
	private const string kFormatF = "{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}";

	// Token: 0x0400160D RID: 5645
	private const float kDuration = 8f;

	// Token: 0x0400160E RID: 5646
	private static Mesh gSphereMesh;
}
