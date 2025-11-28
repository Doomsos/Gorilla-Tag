using System;
using System.Reflection;
using UnityEngine;

// Token: 0x0200008C RID: 140
public static class DebugExtension
{
	// Token: 0x06000372 RID: 882 RVA: 0x00014504 File Offset: 0x00012704
	public static void DebugPoint(Vector3 position, Color color, float scale = 1f, float duration = 0f, bool depthTest = true)
	{
		color = ((color == default(Color)) ? Color.white : color);
		Debug.DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale, color, duration, depthTest);
		Debug.DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale, color, duration, depthTest);
		Debug.DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale, color, duration, depthTest);
	}

	// Token: 0x06000373 RID: 883 RVA: 0x000145BC File Offset: 0x000127BC
	public static void DebugPoint(Vector3 position, float scale = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugPoint(position, Color.white, scale, duration, depthTest);
	}

	// Token: 0x06000374 RID: 884 RVA: 0x000145CC File Offset: 0x000127CC
	public static void DebugBounds(Bounds bounds, Color color, float duration = 0f, bool depthTest = true)
	{
		Vector3 center = bounds.center;
		float x = bounds.extents.x;
		float y = bounds.extents.y;
		float z = bounds.extents.z;
		Vector3 vector = center + new Vector3(x, y, z);
		Vector3 vector2 = center + new Vector3(x, y, -z);
		Vector3 vector3 = center + new Vector3(-x, y, z);
		Vector3 vector4 = center + new Vector3(-x, y, -z);
		Vector3 vector5 = center + new Vector3(x, -y, z);
		Vector3 vector6 = center + new Vector3(x, -y, -z);
		Vector3 vector7 = center + new Vector3(-x, -y, z);
		Vector3 vector8 = center + new Vector3(-x, -y, -z);
		Debug.DrawLine(vector, vector3, color, duration, depthTest);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector2, vector4, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
		Debug.DrawLine(vector5, vector7, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector6, color, duration, depthTest);
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0001471E File Offset: 0x0001291E
	public static void DebugBounds(Bounds bounds, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugBounds(bounds, Color.white, duration, depthTest);
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00014730 File Offset: 0x00012930
	public static void DebugLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = transform.TransformPoint(center + -size * 0.5f);
		Vector3 vector2 = transform.TransformPoint(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = transform.TransformPoint(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = transform.TransformPoint(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = transform.TransformPoint(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = transform.TransformPoint(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = transform.TransformPoint(center + size * 0.5f);
		Vector3 vector8 = transform.TransformPoint(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector4, vector, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, vector7, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector5, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0001492F File Offset: 0x00012B2F
	public static void DebugLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugLocalCube(transform, size, Color.white, center, duration, depthTest);
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00014944 File Offset: 0x00012B44
	public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		color = ((color == default(Color)) ? Color.white : color);
		Vector3 vector = space.MultiplyPoint3x4(center + -size * 0.5f);
		Vector3 vector2 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = space.MultiplyPoint3x4(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = space.MultiplyPoint3x4(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = space.MultiplyPoint3x4(center + size * 0.5f);
		Vector3 vector8 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector4, vector, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, vector7, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector5, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00014B67 File Offset: 0x00012D67
	public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugLocalCube(space, size, Color.white, center, duration, depthTest);
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00014B7C File Offset: 0x00012D7C
	public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = up.normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0] = vector3.x;
		matrix4x[1] = vector3.y;
		matrix4x[2] = vector3.z;
		matrix4x[4] = vector.x;
		matrix4x[5] = vector.y;
		matrix4x[6] = vector.z;
		matrix4x[8] = vector2.x;
		matrix4x[9] = vector2.y;
		matrix4x[10] = vector2.z;
		Vector3 vector4 = position + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
		Vector3 vector5 = Vector3.zero;
		color = ((color == default(Color)) ? Color.white : color);
		for (int i = 0; i < 91; i++)
		{
			vector5.x = Mathf.Cos((float)(i * 4) * 0.017453292f);
			vector5.z = Mathf.Sin((float)(i * 4) * 0.017453292f);
			vector5.y = 0f;
			vector5 = position + matrix4x.MultiplyPoint3x4(vector5);
			Debug.DrawLine(vector4, vector5, color, duration, depthTest);
			vector4 = vector5;
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00014D06 File Offset: 0x00012F06
	public static void DebugCircle(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCircle(position, Vector3.up, color, radius, duration, depthTest);
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00014D18 File Offset: 0x00012F18
	public static void DebugCircle(Vector3 position, Vector3 up, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCircle(position, up, Color.white, radius, duration, depthTest);
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00014D2A File Offset: 0x00012F2A
	public static void DebugCircle(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCircle(position, Vector3.up, Color.white, radius, duration, depthTest);
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00014D40 File Offset: 0x00012F40
	public static void DebugWireSphere(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		float num = 10f;
		Vector3 vector;
		vector..ctor(position.x, position.y + radius * Mathf.Sin(0f), position.z + radius * Mathf.Cos(0f));
		Vector3 vector2;
		vector2..ctor(position.x + radius * Mathf.Cos(0f), position.y, position.z + radius * Mathf.Sin(0f));
		Vector3 vector3;
		vector3..ctor(position.x + radius * Mathf.Cos(0f), position.y + radius * Mathf.Sin(0f), position.z);
		for (int i = 1; i < 37; i++)
		{
			Vector3 vector4;
			vector4..ctor(position.x, position.y + radius * Mathf.Sin(num * (float)i * 0.017453292f), position.z + radius * Mathf.Cos(num * (float)i * 0.017453292f));
			Vector3 vector5;
			vector5..ctor(position.x + radius * Mathf.Cos(num * (float)i * 0.017453292f), position.y, position.z + radius * Mathf.Sin(num * (float)i * 0.017453292f));
			Vector3 vector6;
			vector6..ctor(position.x + radius * Mathf.Cos(num * (float)i * 0.017453292f), position.y + radius * Mathf.Sin(num * (float)i * 0.017453292f), position.z);
			Debug.DrawLine(vector, vector4, color, duration, depthTest);
			Debug.DrawLine(vector2, vector5, color, duration, depthTest);
			Debug.DrawLine(vector3, vector6, color, duration, depthTest);
			vector = vector4;
			vector2 = vector5;
			vector3 = vector6;
		}
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00014EED File Offset: 0x000130ED
	public static void DebugWireSphere(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugWireSphere(position, Color.white, radius, duration, depthTest);
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00014F00 File Offset: 0x00013100
	public static void DebugCylinder(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		DebugExtension.DebugCircle(start, vector, color, radius, duration, depthTest);
		DebugExtension.DebugCircle(end, -vector, color, radius, duration, depthTest);
		DebugExtension.DebugCircle((start + end) * 0.5f, vector, color, radius, duration, depthTest);
		Debug.DrawLine(start + vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector3, end - vector3, color, duration, depthTest);
		Debug.DrawLine(start + vector2, end + vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector2, end - vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector3, start + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector2, start + vector2, color, duration, depthTest);
		Debug.DrawLine(end - vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(end - vector2, end + vector2, color, duration, depthTest);
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00015047 File Offset: 0x00013247
	public static void DebugCylinder(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCylinder(start, end, Color.white, radius, duration, depthTest);
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0001505C File Offset: 0x0001325C
	public static void DebugCone(Vector3 position, Vector3 direction, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		float magnitude = direction.magnitude;
		Vector3 vector = direction;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * magnitude;
		direction = direction.normalized;
		Vector3 vector4 = Vector3.Slerp(vector, vector2, angle / 90f);
		Plane plane;
		plane..ctor(-direction, position + vector);
		Ray ray;
		ray..ctor(position, vector4);
		float num;
		plane.Raycast(ray, ref num);
		Debug.DrawRay(position, vector4.normalized * num, color);
		Debug.DrawRay(position, Vector3.Slerp(vector, -vector2, angle / 90f).normalized * num, color, duration, depthTest);
		Debug.DrawRay(position, Vector3.Slerp(vector, vector3, angle / 90f).normalized * num, color, duration, depthTest);
		Debug.DrawRay(position, Vector3.Slerp(vector, -vector3, angle / 90f).normalized * num, color, duration, depthTest);
		DebugExtension.DebugCircle(position + vector, direction, color, (vector - vector4.normalized * num).magnitude, duration, depthTest);
		DebugExtension.DebugCircle(position + vector * 0.5f, direction, color, (vector * 0.5f - vector4.normalized * (num * 0.5f)).magnitude, duration, depthTest);
	}

	// Token: 0x06000383 RID: 899 RVA: 0x000151F1 File Offset: 0x000133F1
	public static void DebugCone(Vector3 position, Vector3 direction, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCone(position, direction, Color.white, angle, duration, depthTest);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00015203 File Offset: 0x00013403
	public static void DebugCone(Vector3 position, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCone(position, Vector3.up, color, angle, duration, depthTest);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00015215 File Offset: 0x00013415
	public static void DebugCone(Vector3 position, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCone(position, Vector3.up, Color.white, angle, duration, depthTest);
	}

	// Token: 0x06000386 RID: 902 RVA: 0x0001522A File Offset: 0x0001342A
	public static void DebugArrow(Vector3 position, Vector3 direction, Color color, float duration = 0f, bool depthTest = true)
	{
		Debug.DrawRay(position, direction, color, duration, depthTest);
		DebugExtension.DebugCone(position + direction, -direction * 0.333f, color, 15f, duration, depthTest);
	}

	// Token: 0x06000387 RID: 903 RVA: 0x0001525C File Offset: 0x0001345C
	public static void DebugArrow(Vector3 position, Vector3 direction, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugArrow(position, direction, Color.white, duration, depthTest);
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0001526C File Offset: 0x0001346C
	public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		float magnitude = (start - end).magnitude;
		float num = Mathf.Max(0f, magnitude * 0.5f - radius);
		Vector3 vector4 = (end + start) * 0.5f;
		start = vector4 + (start - vector4).normalized * num;
		end = vector4 + (end - vector4).normalized * num;
		DebugExtension.DebugCircle(start, vector, color, radius, duration, depthTest);
		DebugExtension.DebugCircle(end, -vector, color, radius, duration, depthTest);
		Debug.DrawLine(start + vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector3, end - vector3, color, duration, depthTest);
		Debug.DrawLine(start + vector2, end + vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector2, end - vector2, color, duration, depthTest);
		for (int i = 1; i < 26; i++)
		{
			Debug.DrawLine(Vector3.Slerp(vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(vector3, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector3, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(vector2, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector2, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector3, vector, (float)i / 25f) + end, Vector3.Slerp(vector3, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector3, vector, (float)i / 25f) + end, Vector3.Slerp(-vector3, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector2, vector, (float)i / 25f) + end, Vector3.Slerp(vector2, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector2, vector, (float)i / 25f) + end, Vector3.Slerp(-vector2, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
		}
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000155DA File Offset: 0x000137DA
	public static void DebugCapsule(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCapsule(start, end, Color.white, radius, duration, depthTest);
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000155EC File Offset: 0x000137EC
	public static void DrawPoint(Vector3 position, Color color, float scale = 1f)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale);
		Gizmos.DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale);
		Gizmos.DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale);
		Gizmos.color = color2;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0001568D File Offset: 0x0001388D
	public static void DrawPoint(Vector3 position, float scale = 1f)
	{
		DebugExtension.DrawPoint(position, Color.white, scale);
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0001569C File Offset: 0x0001389C
	public static void DrawBounds(Bounds bounds, Color color)
	{
		Vector3 center = bounds.center;
		float x = bounds.extents.x;
		float y = bounds.extents.y;
		float z = bounds.extents.z;
		Vector3 vector = center + new Vector3(x, y, z);
		Vector3 vector2 = center + new Vector3(x, y, -z);
		Vector3 vector3 = center + new Vector3(-x, y, z);
		Vector3 vector4 = center + new Vector3(-x, y, -z);
		Vector3 vector5 = center + new Vector3(x, -y, z);
		Vector3 vector6 = center + new Vector3(x, -y, -z);
		Vector3 vector7 = center + new Vector3(-x, -y, z);
		Vector3 vector8 = center + new Vector3(-x, -y, -z);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(vector, vector3);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.DrawLine(vector5, vector7);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector6);
		Gizmos.color = color2;
	}

	// Token: 0x0600038D RID: 909 RVA: 0x000157DA File Offset: 0x000139DA
	public static void DrawBounds(Bounds bounds)
	{
		DebugExtension.DrawBounds(bounds, Color.white);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x000157E8 File Offset: 0x000139E8
	public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3))
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3 vector = transform.TransformPoint(center + -size * 0.5f);
		Vector3 vector2 = transform.TransformPoint(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = transform.TransformPoint(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = transform.TransformPoint(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = transform.TransformPoint(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = transform.TransformPoint(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = transform.TransformPoint(center + size * 0.5f);
		Vector3 vector8 = transform.TransformPoint(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, vector7);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector5);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.color = color2;
	}

	// Token: 0x0600038F RID: 911 RVA: 0x000159BB File Offset: 0x00013BBB
	public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3))
	{
		DebugExtension.DrawLocalCube(transform, size, Color.white, center);
	}

	// Token: 0x06000390 RID: 912 RVA: 0x000159CC File Offset: 0x00013BCC
	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3))
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3 vector = space.MultiplyPoint3x4(center + -size * 0.5f);
		Vector3 vector2 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = space.MultiplyPoint3x4(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = space.MultiplyPoint3x4(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = space.MultiplyPoint3x4(center + size * 0.5f);
		Vector3 vector8 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, vector7);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector5);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.color = color2;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00015BA7 File Offset: 0x00013DA7
	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3))
	{
		DebugExtension.DrawLocalCube(space, size, Color.white, center);
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00015BB8 File Offset: 0x00013DB8
	public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1f)
	{
		up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
		Vector3 vector = Vector3.Slerp(up, -up, 0.5f);
		Vector3 vector2 = Vector3.Cross(up, vector).normalized * radius;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0] = vector2.x;
		matrix4x[1] = vector2.y;
		matrix4x[2] = vector2.z;
		matrix4x[4] = up.x;
		matrix4x[5] = up.y;
		matrix4x[6] = up.z;
		matrix4x[8] = vector.x;
		matrix4x[9] = vector.y;
		matrix4x[10] = vector.z;
		Vector3 vector3 = position + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
		Vector3 vector4 = Vector3.zero;
		Color color2 = Gizmos.color;
		Gizmos.color = ((color == default(Color)) ? Color.white : color);
		for (int i = 0; i < 91; i++)
		{
			vector4.x = Mathf.Cos((float)(i * 4) * 0.017453292f);
			vector4.z = Mathf.Sin((float)(i * 4) * 0.017453292f);
			vector4.y = 0f;
			vector4 = position + matrix4x.MultiplyPoint3x4(vector4);
			Gizmos.DrawLine(vector3, vector4);
			vector3 = vector4;
		}
		Gizmos.color = color2;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00015D63 File Offset: 0x00013F63
	public static void DrawCircle(Vector3 position, Color color, float radius = 1f)
	{
		DebugExtension.DrawCircle(position, Vector3.up, color, radius);
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00015D72 File Offset: 0x00013F72
	public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1f)
	{
		DebugExtension.DrawCircle(position, position, Color.white, radius);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00015D81 File Offset: 0x00013F81
	public static void DrawCircle(Vector3 position, float radius = 1f)
	{
		DebugExtension.DrawCircle(position, Vector3.up, Color.white, radius);
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00015D94 File Offset: 0x00013F94
	public static void DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1f)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		DebugExtension.DrawCircle(start, vector, color, radius);
		DebugExtension.DrawCircle(end, -vector, color, radius);
		DebugExtension.DrawCircle((start + end) * 0.5f, vector, color, radius);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(start + vector3, end + vector3);
		Gizmos.DrawLine(start - vector3, end - vector3);
		Gizmos.DrawLine(start + vector2, end + vector2);
		Gizmos.DrawLine(start - vector2, end - vector2);
		Gizmos.DrawLine(start - vector3, start + vector3);
		Gizmos.DrawLine(start - vector2, start + vector2);
		Gizmos.DrawLine(end - vector3, end + vector3);
		Gizmos.DrawLine(end - vector2, end + vector2);
		Gizmos.color = color2;
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00015EB7 File Offset: 0x000140B7
	public static void DrawCylinder(Vector3 start, Vector3 end, float radius = 1f)
	{
		DebugExtension.DrawCylinder(start, end, Color.white, radius);
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00015EC8 File Offset: 0x000140C8
	public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45f)
	{
		float magnitude = direction.magnitude;
		Vector3 vector = direction;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * magnitude;
		direction = direction.normalized;
		Vector3 vector4 = Vector3.Slerp(vector, vector2, angle / 90f);
		Plane plane;
		plane..ctor(-direction, position + vector);
		Ray ray;
		ray..ctor(position, vector4);
		float num;
		plane.Raycast(ray, ref num);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position, vector4.normalized * num);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, -vector2, angle / 90f).normalized * num);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, vector3, angle / 90f).normalized * num);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, -vector3, angle / 90f).normalized * num);
		DebugExtension.DrawCircle(position + vector, direction, color, (vector - vector4.normalized * num).magnitude);
		DebugExtension.DrawCircle(position + vector * 0.5f, direction, color, (vector * 0.5f - vector4.normalized * (num * 0.5f)).magnitude);
		Gizmos.color = color2;
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00016055 File Offset: 0x00014255
	public static void DrawCone(Vector3 position, Vector3 direction, float angle = 45f)
	{
		DebugExtension.DrawCone(position, direction, Color.white, angle);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00016064 File Offset: 0x00014264
	public static void DrawCone(Vector3 position, Color color, float angle = 45f)
	{
		DebugExtension.DrawCone(position, Vector3.up, color, angle);
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00016073 File Offset: 0x00014273
	public static void DrawCone(Vector3 position, float angle = 45f)
	{
		DebugExtension.DrawCone(position, Vector3.up, Color.white, angle);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00016086 File Offset: 0x00014286
	public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position, direction);
		DebugExtension.DrawCone(position + direction, -direction * 0.333f, color, 15f);
		Gizmos.color = color2;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x000160C1 File Offset: 0x000142C1
	public static void DrawArrow(Vector3 position, Vector3 direction)
	{
		DebugExtension.DrawArrow(position, direction, Color.white);
	}

	// Token: 0x0600039E RID: 926 RVA: 0x000160D0 File Offset: 0x000142D0
	public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1f)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		float magnitude = (start - end).magnitude;
		float num = Mathf.Max(0f, magnitude * 0.5f - radius);
		Vector3 vector4 = (end + start) * 0.5f;
		start = vector4 + (start - vector4).normalized * num;
		end = vector4 + (end - vector4).normalized * num;
		DebugExtension.DrawCircle(start, vector, color, radius);
		DebugExtension.DrawCircle(end, -vector, color, radius);
		Gizmos.DrawLine(start + vector3, end + vector3);
		Gizmos.DrawLine(start - vector3, end - vector3);
		Gizmos.DrawLine(start + vector2, end + vector2);
		Gizmos.DrawLine(start - vector2, end - vector2);
		for (int i = 1; i < 26; i++)
		{
			Gizmos.DrawLine(Vector3.Slerp(vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(vector3, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector3, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(vector2, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector2, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(vector3, vector, (float)i / 25f) + end, Vector3.Slerp(vector3, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-vector3, vector, (float)i / 25f) + end, Vector3.Slerp(-vector3, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(vector2, vector, (float)i / 25f) + end, Vector3.Slerp(vector2, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-vector2, vector, (float)i / 25f) + end, Vector3.Slerp(-vector2, vector, (float)(i - 1) / 25f) + end);
		}
		Gizmos.color = color2;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0001640E File Offset: 0x0001460E
	public static void DrawCapsule(Vector3 start, Vector3 end, float radius = 1f)
	{
		DebugExtension.DrawCapsule(start, end, Color.white, radius);
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00016420 File Offset: 0x00014620
	public static string MethodsOfObject(object obj, bool includeInfo = false)
	{
		string text = "";
		MethodInfo[] methods = obj.GetType().GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			if (includeInfo)
			{
				string text2 = text;
				MethodInfo methodInfo = methods[i];
				text = text2 + ((methodInfo != null) ? methodInfo.ToString() : null) + "\n";
			}
			else
			{
				text = text + methods[i].Name + "\n";
			}
		}
		return text;
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00016484 File Offset: 0x00014684
	public static string MethodsOfType(Type type, bool includeInfo = false)
	{
		string text = "";
		MethodInfo[] methods = type.GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			if (includeInfo)
			{
				string text2 = text;
				MethodInfo methodInfo = methods[i];
				text = text2 + ((methodInfo != null) ? methodInfo.ToString() : null) + "\n";
			}
			else
			{
				text = text + methods[i].Name + "\n";
			}
		}
		return text;
	}
}
