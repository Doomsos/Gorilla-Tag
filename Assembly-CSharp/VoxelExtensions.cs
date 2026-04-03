using System;
using PlayFab.Internal;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

public static class VoxelExtensions
{
	private static UnityEngine.BoundsInt _lastBounds;

	private static Vector3 _lastHitPoint;

	private static Vector3 _lastGridPoint;

	private static Vector3 _lastVertex;

	private static bool _showDebug;

	private static bool _centerOnly;

	private static bool _cascade = true;

	public static void Mine(this VoxelWorld world, Collision collision, VoxelAction action)
	{
		switch (action.operation)
		{
		case OperationType.Subtract:
			world.Mine(collision.ToRaycastHit(), action);
			break;
		case OperationType.Add:
			world.UnMine(collision.ToRaycastHit(), action);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public static void Mine(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		switch (world.MeshGenerationMode)
		{
		case MeshGenerationMode.MarchingCubes:
			world.Mine_MarchingCubes(hit, action);
			break;
		case MeshGenerationMode.SurfaceNets:
			world.Mine_SurfaceNets(hit, action);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public static void UnMine(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		switch (world.MeshGenerationMode)
		{
		case MeshGenerationMode.MarchingCubes:
			Debug.LogWarning("UnMine not implemented for Marching Cubes");
			break;
		case MeshGenerationMode.SurfaceNets:
			world.UnMine_SurfaceNets(hit, action);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private static void Mine_MarchingCubes(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 origin = hit.point;
		_lastHitPoint = origin;
		int triangleIndex = hit.triangleIndex;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.red, 20f);
		}
		if (hit.collider is MeshCollider { sharedMesh: var sharedMesh } meshCollider)
		{
			if (sharedMesh == null || triangleIndex < 0 || triangleIndex >= sharedMesh.triangles.Length / 3)
			{
				Debug.LogWarning($"Invalid triangle index {triangleIndex} for mesh {sharedMesh?.name}");
				return;
			}
			Vector3 vector = meshCollider.transform.InverseTransformPoint(origin);
			int[] triangles = sharedMesh.triangles;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3 vector2 = vertices[triangles[triangleIndex * 3]];
			Vector3 vector3 = vertices[triangles[triangleIndex * 3 + 1]];
			Vector3 vector4 = vertices[triangles[triangleIndex * 3 + 2]];
			Vector3 position = ((!((vector - vector2).sqrMagnitude < (vector - vector3).sqrMagnitude)) ? (((vector - vector3).sqrMagnitude < (vector - vector4).sqrMagnitude) ? vector3 : vector4) : (((vector - vector2).sqrMagnitude < (vector - vector4).sqrMagnitude) ? vector2 : vector4));
			position = (_lastVertex = meshCollider.transform.TransformPoint(position));
			if (_showDebug)
			{
				Debug.Log($"Closest vertex to {origin}: {position}");
			}
			if (_showDebug)
			{
				Debug.DrawLine(origin, position, Color.blue, 20f);
			}
			position = world.GetLocalPosition(position);
			origin = position.SnapToInt();
			if (_cascade && !world.GetDensityAt(origin).IsSolid())
			{
				if (_showDebug)
				{
					Debug.Log($"Hit air at {origin}, moving to next voxel");
				}
				if (_showDebug)
				{
					Debug.DrawLine(origin, origin + (position - origin).normalized, Color.cyan, 20f);
				}
				origin += (position - origin).normalized;
			}
		}
		_lastGridPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.white, 20f);
		}
		int total = 0;
		int dirt = 0;
		int stone = 0;
		int3 v = origin.ToInt3();
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position2 = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position2, vector3Int * 2);
		world.SetVoxelDataCustom(_lastBounds, MineAt);
		Debug.Log($"Mined [{total} = {stone}S + {dirt}D] at {origin} with bounds {_lastBounds}");
		if (total > 0)
		{
			SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hit.point, hit.normal, dirt, stone);
		}
		(byte density, byte material) MineAt(int3 point, (byte density, byte material) data)
		{
			(byte density, byte material) tuple = data;
			byte item = tuple.density;
			byte b = tuple.material;
			float num = ((b == 0) ? action.strength : (action.strength * 0.2f));
			float num2 = math.distance(origin, point);
			byte b2 = ((num2 > action.radius) ? item : ((byte)math.clamp((float)(int)item - num * math.lerp(255f, 0f, num2 / action.radius), 0f, 255f)));
			if (_showDebug && item != b2)
			{
				Debug.Log($"Hit at {origin}->{point}=d{num2:F2} with density {item}[{item.ToFloat()}] -> {b2}[{b2.ToFloat()}]");
			}
			if (item.IsSolid())
			{
				int num3 = (int)((float)(item - b2) / 10f);
				total += num3;
				switch (b)
				{
				case 0:
					dirt += num3;
					break;
				case 1:
					stone += num3;
					break;
				}
				if (!b2.IsSolid())
				{
					b = 0;
				}
			}
			return (density: b2, material: b);
		}
	}

	private static void Mine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		Vector3 origin = hit.point;
		_lastHitPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(GetTriangleCenter(hit));
		origin = localPosition.SnapToInt();
		if (_cascade && world.GetDensityAt(origin) == 0)
		{
			if (_showDebug)
			{
				Debug.Log($"Hit air at {origin}, moving to next voxel");
			}
			int3 closestCardinalNeighbour = origin.ToInt3().GetClosestCardinalNeighbour(localPosition - hit.normal * 0.5f);
			if (_showDebug)
			{
				Debug.DrawLine(origin, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			origin = closestCardinalNeighbour.ToFloat3();
		}
		_lastGridPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.white, 20f);
		}
		int total = 0;
		int dirt = 0;
		int stone = 0;
		int3 v = origin.ToInt3();
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		world.SetVoxelDataCustom(_lastBounds, MineAt);
		Debug.Log($"Mined {stone}S + {dirt}D = {total} at {origin} with bounds {_lastBounds}");
		if (total > 0)
		{
			SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hit.point, hit.normal, dirt, stone);
		}
		(byte density, byte material) MineAt(int3 point, (byte density, byte material) data)
		{
			(byte density, byte material) tuple = data;
			byte item = tuple.density;
			byte b = tuple.material;
			float num = ((b == 0) ? action.strength : (action.strength * 0.2f));
			float num2 = math.distance(origin, point);
			byte b2 = ((num2 > action.radius) ? item : ((byte)math.clamp((float)(int)item - num * math.lerp(255f, 0f, num2 / action.radius), 0f, 255f)));
			if (_showDebug && item != b2)
			{
				Debug.Log($"Hit at {origin}->{point}=d{num2:F2} with density {item}[{item.ToFloat()}] -> {b2}[{b2.ToFloat()}]");
			}
			if (item.IsSolid())
			{
				int num3 = (int)((float)(item - b2) / 10f);
				total += num3;
				switch (b)
				{
				case 0:
					dirt += num3;
					break;
				case 1:
					stone += num3;
					break;
				}
				if (!b2.IsSolid())
				{
					b = 0;
				}
			}
			return (density: b2, material: b);
		}
	}

	private static void UnMine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 origin = hit.point;
		_lastHitPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(GetTriangleCenter(hit));
		origin = localPosition.SnapToInt();
		if (_cascade && world.GetDensityAt(origin) == byte.MaxValue)
		{
			if (_showDebug)
			{
				Debug.Log($"Hit solid block at {origin}, moving to next voxel");
			}
			int3 closestCardinalNeighbour = origin.ToInt3().GetClosestCardinalNeighbour(localPosition + hit.normal * 0.5f);
			if (_showDebug)
			{
				Debug.DrawLine(origin, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			origin = closestCardinalNeighbour.ToFloat3();
		}
		_lastGridPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.white, 20f);
		}
		int total = 0;
		int dirt = 0;
		int stone = 0;
		int3 v = origin.ToInt3();
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		world.SetVoxelDataCustom(_lastBounds, UnMineAt);
		Debug.Log($"UnMined {stone}S + {dirt}D = {total} at {origin} with bounds {_lastBounds}");
		(byte density, byte material) UnMineAt(int3 point, (byte density, byte material) data)
		{
			(byte density, byte material) tuple = data;
			byte item = tuple.density;
			byte b = tuple.material;
			float num = math.distance(origin, point);
			byte b2 = ((num > action.radius) ? item : ((byte)math.clamp((float)(int)item + action.strength * math.lerp(255f, 0f, num / action.radius), 0f, 255f)));
			if (item != b2)
			{
				b = action.material;
			}
			if (_showDebug && item != b2)
			{
				Debug.Log($"Unmined at {origin}->{point}=d{num:F2} with density {item}[{item.ToFloat()}] -> {b2}[{b2.ToFloat()}]");
			}
			if (!item.IsSolid() && b2.IsSolid())
			{
				int num2 = (int)((float)(b2 - item) / 10f);
				total += num2;
				switch (b)
				{
				case 0:
					dirt += num2;
					break;
				case 1:
					stone += num2;
					break;
				}
			}
			return (density: b2, material: b);
		}
	}

	public static void SetVoxel(this VoxelWorld world, int x, int y, int z, byte density, byte materialId)
	{
		Vector3Int position = new Vector3Int(x, y, z);
		UnityEngine.BoundsInt worldBounds = new UnityEngine.BoundsInt(position, Vector3Int.zero);
		world.SetVoxelDataCustom(worldBounds, SetVoxelAt);
		(byte density, byte material) SetVoxelAt(int3 point, (byte density, byte material) data)
		{
			return (density: density, material: materialId);
		}
	}

	public static void SetVoxels(this VoxelWorld world, int3[] voxels, byte density, byte materialId)
	{
		world.SetVoxelDataCustom(voxels, SetVoxelAt);
		(byte density, byte material) SetVoxelAt(int3 point, (byte density, byte material) data)
		{
			return (density: density, material: materialId);
		}
	}

	public static void Dig(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		position = world.GetLocalPosition(position);
		radius /= world.Scale;
		UnityEngine.BoundsInt bounds = world.GetBounds(position, radius);
		world.SetVoxelDensityCustom(bounds, DigAt);
		byte DigAt(int3 point, byte density)
		{
			float num = math.distance(position, point);
			if (!(num > radius))
			{
				return (byte)math.clamp((float)(int)density - strength * math.lerp(255f, 0f, num / radius), 0f, 255f);
			}
			return density;
		}
	}

	public static void Add(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		position = world.GetLocalPosition(position);
		radius /= world.Scale;
		UnityEngine.BoundsInt bounds = world.GetBounds(position, radius);
		world.SetVoxelDensityCustom(bounds, AddAt);
		byte AddAt(int3 point, byte density)
		{
			float num = math.distance(position, point);
			if (!(num > radius))
			{
				return (byte)math.clamp((float)(int)density + strength * math.lerp(255f, 0f, num / radius), 0f, 255f);
			}
			return density;
		}
	}

	private static UnityEngine.BoundsInt GetBounds(this VoxelWorld world, float3 point, float radius)
	{
		int3 voxelForLocalPosition = world.GetVoxelForLocalPosition(point);
		int num = Mathf.CeilToInt(radius);
		return new UnityEngine.BoundsInt((voxelForLocalPosition - num).ToVectorInt(), new int3(num * 2).ToVectorInt());
	}

	private static Vector3 GetTriangleCenter(RaycastHit hit)
	{
		var (vector, vector2, vector3) = GetWorldTriangle(hit);
		return (vector + vector2 + vector3) / 3f;
	}

	private static (Vector3 v1, Vector3 v2, Vector3 v3) GetWorldTriangle(RaycastHit hit)
	{
		if (hit.collider is MeshCollider { sharedMesh: var sharedMesh } meshCollider)
		{
			if (sharedMesh == null || hit.triangleIndex < 0 || hit.triangleIndex >= sharedMesh.triangles.Length / 3)
			{
				Debug.LogWarning($"Invalid triangle index {hit.triangleIndex} for mesh {sharedMesh?.name}");
				return (v1: Vector3.zero, v2: Vector3.zero, v3: Vector3.zero);
			}
			int[] triangles = sharedMesh.triangles;
			Vector3[] vertices = sharedMesh.vertices;
			Transform transform = meshCollider.transform;
			Vector3 item = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
			Vector3 item2 = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
			Vector3 item3 = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
			return (v1: item, v2: item2, v3: item3);
		}
		return (v1: Vector3.zero, v2: Vector3.zero, v3: Vector3.zero);
	}
}
