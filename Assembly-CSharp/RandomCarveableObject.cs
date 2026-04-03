using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

public class RandomCarveableObject : MonoBehaviour
{
	[SerializeField]
	private Transform spawnPoint;

	[SerializeField]
	private GameObject[] prefabs;

	[SerializeField]
	private byte materialId;

	[SerializeField]
	private VoxelWorld world;

	private GameObject _carveable;

	private int3[] _voxels;

	private UnityEngine.BoundsInt _voxelBounds;

	private void Start()
	{
		if ((object)world == null)
		{
			world = GetComponentInParent<VoxelWorld>();
		}
		world.UpdateWorld = false;
		for (int num = spawnPoint.childCount - 1; num >= 0; num--)
		{
			JamUtil.Destroy(spawnPoint.GetChild(num).gameObject);
		}
		SpawnNewCarveable();
	}

	private void SelectCarveable()
	{
		for (int num = spawnPoint.childCount - 1; num >= 0; num--)
		{
			JamUtil.Destroy(spawnPoint.GetChild(num).gameObject);
		}
		_carveable = Object.Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Length)], spawnPoint.position, spawnPoint.rotation, spawnPoint);
		_carveable.transform.localScale = Vector3.one;
	}

	private void ClearBounds()
	{
		SetBoundsDensity(0);
	}

	private void FillBounds()
	{
		SetBoundsDensity(byte.MaxValue);
	}

	private void SetBoundsDensity(byte density)
	{
		world.SetVoxels(_voxels, density, materialId);
	}

	private void CollectVoxelSet()
	{
		Bounds localBounds = _carveable.GetComponentInChildren<Renderer>().localBounds;
		Vector3 min = localBounds.min;
		Vector3 max = localBounds.max;
		HashSet<int3> hashSet = new HashSet<int3>();
		for (float num = min.x; num <= max.x; num += 0.1f)
		{
			for (float num2 = min.y; num2 <= max.y; num2 += 0.1f)
			{
				for (float num3 = min.z; num3 <= max.z; num3 += 0.1f)
				{
					hashSet.Add(GetVoxel(num, num2, num3));
				}
			}
		}
		_voxels = hashSet.ToArray();
		_voxelBounds = VoxelWorld.GetBoundsFor(_voxels);
		int3 GetVoxel(float x, float y, float z)
		{
			return world.GetVoxelForWorldPosition(_carveable.transform.TransformPoint(new Vector3(x, y, z)));
		}
	}

	public async void SpawnNewCarveable()
	{
		await UniTask.Yield();
		if ((bool)_carveable)
		{
			ClearBounds();
		}
		SelectCarveable();
		CollectVoxelSet();
		_carveable.gameObject.SetActive(value: false);
		world.SetWorldBounds(_voxelBounds);
		world.UpdateWorld = true;
		await UniTask.WaitUntil(() => world.BoundsChunksLoaded(_voxelBounds));
		FillBounds();
		_carveable.gameObject.SetActive(value: true);
	}
}
