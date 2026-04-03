using System;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels;

public class VoxelActions_Imported : MonoBehaviour
{
	public float strength = 0.1f;

	public float radius = 3f;

	public float miningStrength = 1f;

	public float miningRadius = 0.5f;

	public byte unmineMaterial;

	private VoxelWorld _voxelWorld;

	private UnityEngine.BoundsInt _lastBounds;

	private Vector3 _lastHitPoint;

	private Vector3 _lastGridPoint;

	private Vector3 _lastVertex;

	[SerializeField]
	private bool _showDebug;

	[SerializeField]
	private bool _centerOnly;

	[SerializeField]
	private bool _cascade;

	private void OnEnable()
	{
		_voxelWorld = VoxelWorld.GetFor(base.gameObject);
	}

	public void Dig(Vector3 origin)
	{
		UnityEngine.BoundsInt bounds = GetBounds(origin);
		_voxelWorld.SetVoxelDensityCustom(bounds, DigAt);
		byte DigAt(int3 point, byte density)
		{
			float num = math.distance(origin, point);
			if (!(num > radius))
			{
				return (byte)math.clamp((float)(int)density - strength * math.lerp(255f, 0f, num / radius), 0f, 255f);
			}
			return density;
		}
	}

	public void Add(Vector3 origin)
	{
		UnityEngine.BoundsInt bounds = GetBounds(origin);
		_voxelWorld.SetVoxelDensityCustom(bounds, AddAt);
		byte AddAt(int3 point, byte density)
		{
			float num = math.distance(origin, point);
			if (!(num > radius))
			{
				return (byte)math.clamp((float)(int)density + strength * math.lerp(255f, 0f, num / radius), 0f, 255f);
			}
			return density;
		}
	}

	public void Mine(RaycastHit hit)
	{
		switch (_voxelWorld.MeshGenerationMode)
		{
		case MeshGenerationMode.MarchingCubes:
			Mine_MarchingCubes(hit);
			break;
		case MeshGenerationMode.SurfaceNets:
			Mine_SurfaceNets(hit);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void UnMine(RaycastHit hit)
	{
		switch (_voxelWorld.MeshGenerationMode)
		{
		case MeshGenerationMode.MarchingCubes:
			Debug.LogWarning("UnMine not implemented for Marching Cubes");
			break;
		case MeshGenerationMode.SurfaceNets:
			UnMine_SurfaceNets(hit);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void Mine_MarchingCubes(RaycastHit hit)
	{
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
			origin = position.SnapToInt();
			if (_cascade && !_voxelWorld.GetDensityAt(origin).IsSolid())
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
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(miningRadius);
		Vector3Int position2 = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position2, vector3Int * 2);
		_voxelWorld.SetVoxelDataCustom(_lastBounds, MineAt);
		Debug.Log($"Mined {stone}S + {dirt}D = {total} at {origin} with bounds {_lastBounds}");
		(byte density, byte material) MineAt(int3 point, (byte density, byte material) data)
		{
			(byte density, byte material) tuple = data;
			byte item = tuple.density;
			byte b = tuple.material;
			float num = math.distance(origin, point);
			byte b2 = ((num > miningRadius) ? item : ((byte)math.clamp((float)(int)item - miningStrength * math.lerp(255f, 0f, num / miningRadius), 0f, 255f)));
			if (_showDebug && item != b2)
			{
				Debug.Log($"Hit at {origin}->{point}=d{num:F2} with density {item}[{item.ToFloat()}] -> {b2}[{b2.ToFloat()}]");
			}
			if (item.IsSolid())
			{
				int num2 = (int)((float)(item - b2) / 10f);
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
				if (!b2.IsSolid())
				{
					b = 0;
				}
			}
			return (density: b2, material: b);
		}
	}

	private void Mine_SurfaceNets(RaycastHit hit)
	{
		Vector3 origin = hit.point;
		_lastHitPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.red, 20f);
		}
		Vector3 triangleCenter = GetTriangleCenter(hit);
		origin = triangleCenter.SnapToInt();
		if (_cascade && _voxelWorld.GetDensityAt(origin) == 0)
		{
			if (_showDebug)
			{
				Debug.Log($"Hit air at {origin}, moving to next voxel");
			}
			int3 closestCardinalNeighbour = origin.ToInt3().GetClosestCardinalNeighbour(triangleCenter - hit.normal * 0.5f);
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
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(miningRadius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		_voxelWorld.SetVoxelDataCustom(_lastBounds, MineAt);
		Debug.Log($"Mined {stone}S + {dirt}D = {total} at {origin} with bounds {_lastBounds}");
		(byte density, byte material) MineAt(int3 point, (byte density, byte material) data)
		{
			(byte density, byte material) tuple = data;
			byte item = tuple.density;
			byte b = tuple.material;
			float num = math.distance(origin, point);
			byte b2 = ((num > miningRadius) ? item : ((byte)math.clamp((float)(int)item - miningStrength * math.lerp(255f, 0f, num / miningRadius), 0f, 255f)));
			if (_showDebug && item != b2)
			{
				Debug.Log($"Hit at {origin}->{point}=d{num:F2} with density {item}[{item.ToFloat()}] -> {b2}[{b2.ToFloat()}]");
			}
			if (item.IsSolid())
			{
				int num2 = (int)((float)(item - b2) / 10f);
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
				if (!b2.IsSolid())
				{
					b = 0;
				}
			}
			return (density: b2, material: b);
		}
	}

	private void UnMine_SurfaceNets(RaycastHit hit)
	{
		Vector3 origin = hit.point;
		_lastHitPoint = origin;
		if (_showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, origin, Color.red, 20f);
		}
		Vector3 triangleCenter = GetTriangleCenter(hit);
		origin = triangleCenter.SnapToInt();
		if (_cascade && _voxelWorld.GetDensityAt(origin) == byte.MaxValue)
		{
			if (_showDebug)
			{
				Debug.Log($"Hit solid block at {origin}, moving to next voxel");
			}
			int3 closestCardinalNeighbour = origin.ToInt3().GetClosestCardinalNeighbour(triangleCenter + hit.normal * 0.5f);
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
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(miningRadius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		_lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		_voxelWorld.SetVoxelDataCustom(_lastBounds, UnMineAt);
		Debug.Log($"UnMined {stone}S + {dirt}D = {total} at {origin} with bounds {_lastBounds}");
		(byte density, byte material) UnMineAt(int3 point, (byte density, byte material) data)
		{
			(byte density, byte material) tuple = data;
			byte item = tuple.density;
			byte item2 = tuple.material;
			float num = math.distance(origin, point);
			byte b = ((num > miningRadius) ? item : ((byte)math.clamp((float)(int)item + miningStrength * math.lerp(255f, 0f, num / miningRadius), 0f, 255f)));
			if (item != b)
			{
				item2 = unmineMaterial;
			}
			if (_showDebug && item != b)
			{
				Debug.Log($"Unmined at {origin}->{point}=d{num:F2} with density {item}[{item.ToFloat()}] -> {b}[{b.ToFloat()}]");
			}
			if (!item.IsSolid() && b.IsSolid())
			{
				int num2 = (int)((float)(b - item) / 10f);
				total += num2;
				switch (item2)
				{
				case 0:
					dirt += num2;
					break;
				case 1:
					stone += num2;
					break;
				}
			}
			return (density: b, material: item2);
		}
	}

	private Vector3 GetTriangleCenter(RaycastHit hit)
	{
		var (vector, vector2, vector3) = GetWorldTriangle(hit);
		return (vector + vector2 + vector3) / 3f;
	}

	private (Vector3 v1, Vector3 v2, Vector3 v3) GetWorldTriangle(RaycastHit hit)
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
			Transform obj = meshCollider.transform;
			Vector3 item = obj.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
			Vector3 item2 = obj.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
			Vector3 item3 = obj.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
			return (v1: item, v2: item2, v3: item3);
		}
		return (v1: Vector3.zero, v2: Vector3.zero, v3: Vector3.zero);
	}

	private UnityEngine.BoundsInt GetBounds(float3 point)
	{
		int3 obj = point.RoundToInt();
		int num = Mathf.CeilToInt(radius);
		int3 int5 = num;
		return new UnityEngine.BoundsInt((obj - int5).ToVectorInt(), new int3(num * 2).ToVectorInt());
	}
}
