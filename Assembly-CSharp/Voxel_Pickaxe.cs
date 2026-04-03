using System;
using UnityEngine;
using UnityEngine.Audio;
using Voxels;

public class Voxel_Pickaxe : MonoBehaviour
{
	[Serializable]
	public struct InteractionPoint
	{
		public Transform transform;

		public Vector3 previousPosition;

		public Vector3 position;
	}

	public VoxelAction mine = new VoxelAction
	{
		strength = 1f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	public InteractionPoint[] points;

	public AudioResource goodHit;

	public AudioResource badHit;

	public AudioSource sound;

	public float hitCooldown = 0.5f;

	public float minHitSpeed = 1f;

	public float minMineSpeed = 5f;

	public float alignThreshold = 0.7f;

	private int _layerMask;

	private float _nextHitTime;

	public bool Held { get; set; }

	private void Reset()
	{
		_layerMask = LayerMask.GetMask("Default");
	}

	private void Awake()
	{
		_layerMask = LayerMask.GetMask("Default");
		if (sound.transform == base.transform)
		{
			Debug.LogError("Audio source for " + base.name + " must be on a separate gameobject!", this);
		}
	}

	private void OnEnable()
	{
		ResetVelocity();
	}

	private void OnDisable()
	{
	}

	private void FixedUpdate()
	{
		if (Held)
		{
			for (int i = 0; i < points.Length; i++)
			{
				UpdateInteractionPoint(ref points[i]);
			}
		}
	}

	private void StartGrabbing()
	{
		Held = true;
		ResetVelocity();
	}

	private void StopGrabbing()
	{
		Held = false;
	}

	private void ResetVelocity()
	{
		for (int i = 0; i < points.Length; i++)
		{
			points[i].position = (points[i].previousPosition = points[i].transform.position);
		}
	}

	private void UpdateInteractionPoint(ref InteractionPoint point)
	{
		point.previousPosition = point.position;
		point.position = point.transform.position;
		if (Time.time < _nextHitTime)
		{
			return;
		}
		Vector3 vector = (point.position - point.previousPosition) / Time.fixedDeltaTime;
		float magnitude = vector.magnitude;
		if (magnitude < minHitSpeed)
		{
			return;
		}
		bool flag = Vector3.Dot(vector.normalized, point.transform.forward) >= alignThreshold;
		if (Physics.Linecast(point.previousPosition, point.position, out var hitInfo, _layerMask, QueryTriggerInteraction.Ignore))
		{
			ChunkComponent component = hitInfo.collider.GetComponent<ChunkComponent>();
			if ((bool)component && flag && magnitude >= minMineSpeed)
			{
				Play(goodHit, hitInfo.point);
				component.World.Mine(hitInfo, mine);
			}
			else
			{
				Play(badHit, hitInfo.point);
			}
			_nextHitTime = Time.time + hitCooldown;
		}
	}

	private void Play(AudioResource resource, Vector3 position)
	{
		if ((bool)resource)
		{
			sound.Stop();
			sound.resource = resource;
			sound.transform.position = position;
			sound.Play();
		}
	}

	public void OnEntityInit()
	{
		sound.transform.parent = null;
	}

	public void OnEntityDestroy()
	{
		sound.transform.parent = base.transform;
	}

	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	private void OnDrawGizmosSelected()
	{
		if (points != null)
		{
			Gizmos.color = Color.green;
			InteractionPoint[] array = points;
			for (int i = 0; i < array.Length; i++)
			{
				InteractionPoint interactionPoint = array[i];
				Gizmos.DrawWireSphere(interactionPoint.transform.position, 0.02f);
				Gizmos.DrawLine(interactionPoint.transform.position, interactionPoint.transform.position + interactionPoint.transform.forward * 0.5f);
			}
		}
	}
}
