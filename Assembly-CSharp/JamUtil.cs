using UnityEngine;

public static class JamUtil
{
	public static bool IsPlaying => Application.isPlaying;

	public static void Destroy(Object obj)
	{
		Object.Destroy(obj);
	}

	public static RaycastHit ToRaycastHit(this Collision collision)
	{
		if (collision.ConvertToRaycast(out var hit))
		{
			Debug.Log($"Hit {hit.collider.name} [{collision.contacts[0].otherCollider == hit.collider}]", hit.collider);
		}
		else
		{
			Debug.LogError($"No hit! ({collision})");
		}
		return hit;
	}

	public static bool ConvertToRaycast(this Collision collision, out RaycastHit hit)
	{
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		LayerMask layerMask = 1 << collision.gameObject.layer;
		bool result = Physics.Raycast(new Ray(point + normal * 0.01f, -normal), out hit, 0.1f, layerMask, QueryTriggerInteraction.Ignore);
		Debug.Log($"Match: {collision.contacts[0].otherCollider == hit.collider}");
		return result;
	}
}
