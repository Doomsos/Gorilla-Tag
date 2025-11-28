using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000CB6 RID: 3254
public class TeleportNode : GorillaTriggerBox
{
	// Token: 0x06004F74 RID: 20340 RVA: 0x001995DC File Offset: 0x001977DC
	public override void OnBoxTriggered()
	{
		if (Time.time - this.teleportTime < 0.1f)
		{
			return;
		}
		base.OnBoxTriggered();
		Transform transform;
		if (!this.teleportFromRef.TryResolve<Transform>(out transform))
		{
			Debug.LogError("[TeleportNode] Failed to resolve teleportFromRef.");
			return;
		}
		Transform transform2;
		if (!this.teleportToRef.TryResolve<Transform>(out transform2))
		{
			Debug.LogError("[TeleportNode] Failed to resolve teleportToRef.");
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("[TeleportNode] GTPlayer.Instance is null.");
			return;
		}
		Physics.SyncTransforms();
		Vector3 position = transform2.TransformPoint(transform.InverseTransformPoint(instance.transform.position));
		Quaternion quaternion = Quaternion.Inverse(transform.rotation) * instance.transform.rotation;
		Quaternion rotation = transform2.rotation * quaternion;
		base.StartCoroutine(this.DelayedTeleport(instance, position, rotation));
		this.teleportTime = Time.time;
	}

	// Token: 0x06004F75 RID: 20341 RVA: 0x001996B6 File Offset: 0x001978B6
	private IEnumerator DelayedTeleport(GTPlayer p, Vector3 position, Quaternion rotation)
	{
		yield return null;
		p.TeleportTo(position, rotation, true, false);
		yield break;
	}

	// Token: 0x04005DF7 RID: 24055
	[SerializeField]
	private XSceneRef teleportFromRef;

	// Token: 0x04005DF8 RID: 24056
	[SerializeField]
	private XSceneRef teleportToRef;

	// Token: 0x04005DF9 RID: 24057
	private float teleportTime;
}
