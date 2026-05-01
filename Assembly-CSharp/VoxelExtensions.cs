using System;
using System.Runtime.CompilerServices;
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

	private static VoxelAction _opAction;

	private static Vector3 _opOrigin;

	private static int _opTotalMined;

	private static int _opDirtMined;

	private static int _opStoneMined;

	private static byte _opDensity;

	private static byte _opMaterialId;

	public static void Mine(this VoxelWorld world, Collision collision, VoxelAction action)
	{
		world.Mine(collision.ToRaycastHit(), action);
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

	private static void Mine_MarchingCubes(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 vector = (_lastHitPoint = hit.point);
		int triangleIndex = hit.triangleIndex;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.red, 20f);
		}
		if (hit.collider is MeshCollider { sharedMesh: var sharedMesh } meshCollider)
		{
			if (sharedMesh == null || triangleIndex < 0 || triangleIndex >= sharedMesh.triangles.Length / 3)
			{
				Debug.LogWarning($"Invalid triangle index {triangleIndex} for mesh {sharedMesh?.name}");
				return;
			}
			Vector3 vector2 = meshCollider.transform.InverseTransformPoint(vector);
			int[] triangles = sharedMesh.triangles;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3 vector3 = vertices[triangles[triangleIndex * 3]];
			Vector3 vector4 = vertices[triangles[triangleIndex * 3 + 1]];
			Vector3 vector5 = vertices[triangles[triangleIndex * 3 + 2]];
			Vector3 position = ((!((vector2 - vector3).sqrMagnitude < (vector2 - vector4).sqrMagnitude)) ? (((vector2 - vector4).sqrMagnitude < (vector2 - vector5).sqrMagnitude) ? vector4 : vector5) : (((vector2 - vector3).sqrMagnitude < (vector2 - vector5).sqrMagnitude) ? vector3 : vector5));
			position = (_lastVertex = meshCollider.transform.TransformPoint(position));
			if (_showDebug)
			{
				Debug.Log($"Closest vertex to {vector}: {position}");
			}
			if (_showDebug)
			{
				Debug.DrawLine(vector, position, Color.blue, 20f);
			}
			position = world.GetLocalPosition(position);
			vector = position.SnapToInt();
			if (_cascade && !world.GetDensityAt(vector).IsSolid())
			{
				if (_showDebug)
				{
					Debug.Log($"Hit air at {vector}, moving to next voxel");
				}
				if (_showDebug)
				{
					Debug.DrawLine(vector, vector + (position - vector).normalized, Color.cyan, 20f);
				}
				vector += (position - vector).normalized;
			}
		}
		_lastGridPoint = vector;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.white, 20f);
		}
		int3 v = vector.ToInt3();
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position2 = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position2, vector3Int * 2);
		VoxelManager.Mine(world, _lastBounds, hit.point, hit.normal, vector, action);
	}

	private static void Mine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 end = (_lastHitPoint = hit.point);
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, end, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(GetTriangleCenter(hit));
		end = localPosition.SnapToInt();
		if (_cascade && world.GetDensityAt(end) == 0)
		{
			if (_showDebug)
			{
				Debug.Log($"Hit air at {end}, moving to next voxel");
			}
			int3 closestCardinalNeighbour = end.ToInt3().GetClosestCardinalNeighbour(localPosition - hit.normal * 0.5f);
			if (_showDebug)
			{
				Debug.DrawLine(end, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			end = closestCardinalNeighbour.ToFloat3();
		}
		_lastGridPoint = end;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, end, Color.white, 20f);
		}
		int3 v = end.ToInt3();
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		VoxelManager.Mine(world, _lastBounds, hit.point, hit.normal, end, action);
	}

	public static (int dirt, int stone) PerformLocalMiningOperation(this VoxelWorld world, UnityEngine.BoundsInt bounds, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action)
	{
		_lastBounds = bounds;
		_opAction = action;
		_opOrigin = origin;
		_opTotalMined = 0;
		_opDirtMined = 0;
		_opStoneMined = 0;
		switch (_opAction.operation)
		{
		case OperationType.Subtract:
			world.SetVoxelDataCustom(bounds, MineAt);
			if (_opTotalMined > 0)
			{
				SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hitPoint, hitNormal, _opDirtMined, _opStoneMined);
			}
			break;
		case OperationType.Add:
			world.SetVoxelDataCustom(bounds, UnMineAt);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return (dirt: _opDirtMined, stone: _opStoneMined);
	}

	public static void PerformLocalOperation(this VoxelWorld world, Vector3 localPosition, VoxelAction action)
	{
		UnityEngine.BoundsInt bounds = world.GetBounds(localPosition, action.radius);
		_opAction = action;
		_opOrigin = localPosition;
		switch (_opAction.operation)
		{
		case OperationType.Subtract:
			world.SetVoxelDensityCustom(bounds, SubtractAt);
			break;
		case OperationType.Add:
			world.SetVoxelDensityCustom(bounds, AddAt);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private static (byte density, byte material) MineAt(int3 point, (byte density, byte material) data)
	{
		(byte density, byte material) tuple = data;
		byte item = tuple.density;
		byte item2 = tuple.material;
		float num = ((item2 == 0) ? _opAction.strength : (_opAction.strength * 0.2f));
		float num2 = math.distance(_opOrigin, point);
		byte b = ((num2 > _opAction.radius) ? item : ((byte)math.clamp((float)(int)item - num * math.lerp(255f, 0f, num2 / _opAction.radius), 0f, 255f)));
		if (_showDebug && item != b)
		{
			Debug.Log($"Hit at {_opOrigin}->{point}=d{num2:F2} with density {item}[{item.ToFloat()}] -> {b}[{b.ToFloat()}]");
		}
		if (item.IsSolid())
		{
			int num3 = (int)((float)(item - b) / 10f);
			_opTotalMined += num3;
			switch (item2)
			{
			case 0:
				_opDirtMined += num3;
				break;
			case 1:
				_opStoneMined += num3;
				break;
			}
		}
		return (density: b, material: item2);
	}

	private static (byte density, byte material) UnMineAt(int3 point, (byte density, byte material) data)
	{
		(byte density, byte material) tuple = data;
		byte item = tuple.density;
		byte b = tuple.material;
		float num = math.distance(_opOrigin, point);
		byte b2 = ((num > _opAction.radius) ? item : ((byte)math.clamp((float)(int)item + _opAction.strength * math.lerp(255f, 0f, num / _opAction.radius), 0f, 255f)));
		if (item != b2)
		{
			b = _opAction.material;
		}
		if (_showDebug && item != b2)
		{
			Debug.Log($"Unmined at {_opOrigin}->{point}=d{num:F2} with density {item}[{item.ToFloat()}] -> {b2}[{b2.ToFloat()}]");
		}
		if (!item.IsSolid() && b2.IsSolid())
		{
			int num2 = (int)((float)(b2 - item) / 10f);
			_opTotalMined += num2;
			switch (b)
			{
			case 0:
				_opDirtMined += num2;
				break;
			case 1:
				_opStoneMined += num2;
				break;
			}
		}
		return (density: b2, material: b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte SubtractAt(int3 point, byte density)
	{
		float num = math.distance(_opOrigin, point);
		if (!(num > _opAction.radius))
		{
			return (byte)math.clamp((float)(int)density - _opAction.strength * math.lerp(255f, 0f, num / _opAction.radius), 0f, 255f);
		}
		return density;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte AddAt(int3 point, byte density)
	{
		float num = math.distance(_opOrigin, point);
		if (!(num > _opAction.radius))
		{
			return (byte)math.clamp((float)(int)density + _opAction.strength * math.lerp(255f, 0f, num / _opAction.radius), 0f, 255f);
		}
		return density;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (byte density, byte materialID) SetVoxelAt(int3 point, (byte density, byte materialId) data)
	{
		return (density: _opDensity, materialID: _opMaterialId);
	}

	public static void PerformAction(this VoxelWorld world, Vector3 position, VoxelAction action)
	{
		VoxelManager.PerformOperation(world, position, action);
	}

	public static void Dig(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelManager.PerformOperation(world, position, new VoxelAction(OperationType.Subtract, radius, strength, 0));
	}

	public static void Add(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelManager.PerformOperation(world, position, new VoxelAction(OperationType.Add, radius, strength, 0));
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

	public static void SetVoxels(this VoxelWorld world, UnityEngine.BoundsInt worldBounds, byte density, byte materialId, bool immediate = true)
	{
		_opDensity = density;
		_opMaterialId = materialId;
		world.SetVoxelDataCustom(worldBounds, SetVoxelAt, immediate);
	}

	public static void SetVoxels(this VoxelWorld world, int3[] voxels, byte density, byte materialId, bool immediate = true)
	{
		_opDensity = density;
		_opMaterialId = materialId;
		world.SetVoxelDataCustom(voxels, SetVoxelAt, immediate);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetVoxelCount(this UnityEngine.BoundsInt bounds)
	{
		return (bounds.max.x - bounds.min.x + 1) * (bounds.max.y - bounds.min.y + 1) * (bounds.max.z - bounds.min.z + 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains(this UnityEngine.BoundsInt a, UnityEngine.BoundsInt b)
	{
		if (a.Contains(b.min))
		{
			return a.Contains(b.max);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UnityEngine.BoundsInt Union(this UnityEngine.BoundsInt a, UnityEngine.BoundsInt b)
	{
		return new UnityEngine.BoundsInt(VectorUtilities.Min(a.min, b.min), VectorUtilities.Max(a.max, b.max));
	}

	public static UnityEngine.BoundsInt GetBounds(this VoxelWorld world, float3 point, float radius)
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

	public static string GetFullPath(this Component component)
	{
		if (!component)
		{
			return "";
		}
		return component.gameObject.GetFullPath() + "/" + component.GetType().Name;
	}

	public static string GetFullPath(this GameObject go)
	{
		if (!go)
		{
			return "";
		}
		string text = go.name;
		Transform parent = go.transform.parent;
		while ((bool)parent)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return go.scene.name + "/" + text;
	}

	public static int GenerateHashcodeFromPath(this Component component)
	{
		return component.GetFullPath().GetHashCode();
	}

	public static int GenerateHashcodeFromPath(this GameObject go)
	{
		return go.GetFullPath().GetHashCode();
	}
}
