using UnityEngine;
using Voxels;

public class VoxelInteractor : MonoBehaviour
{
	public VoxelAction action = new VoxelAction
	{
		strength = 1f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	public bool ApplyVoxelAction(Collision collision)
	{
		ChunkComponent component = collision.gameObject.GetComponent<ChunkComponent>();
		if ((bool)component)
		{
			component.World.Mine(collision, action);
		}
		return component;
	}

	public bool ApplyVoxelAction(RaycastHit hit)
	{
		ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
		if ((bool)component)
		{
			component.World.Mine(hit, action);
		}
		return component;
	}
}
