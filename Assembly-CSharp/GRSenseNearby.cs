using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[Serializable]
public class GRSenseNearby
{
	public void Setup(Transform headTransform)
	{
		this.rigsNearby = new List<VRRig>();
		this.headTransform = headTransform;
	}

	public void OnHitByPlayer(int hitByActorId)
	{
		GRPlayer grplayer = GRPlayer.Get(hitByActorId);
		if (grplayer != null)
		{
			VRRig rig = grplayer.gamePlayer.rig;
			if (!this.rigsNearby.Contains(rig))
			{
				this.rigsNearby.Add(rig);
			}
		}
	}

	public void UpdateNearby(List<VRRig> allRigs, GRSenseLineOfSight senseLineOfSight)
	{
		Vector3 position = this.headTransform.position;
		Vector3 forward = this.headTransform.rotation * Vector3.forward;
		this.RemoveNotNearby(position);
		this.AddNearby(position, forward, allRigs);
		this.RemoveNoLineOfSight(position, senseLineOfSight);
	}

	public bool IsAnyoneNearby()
	{
		return !GhostReactorManager.AggroDisabled && this.rigsNearby != null && this.rigsNearby.Count > 0;
	}

	public bool IsAnyoneNearby(float range)
	{
		if (!this.IsAnyoneNearby())
		{
			return false;
		}
		Vector3 position = this.headTransform.position;
		float num = range * range;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			if (!(this.rigsNearby[i] == null) && (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	public static Vector3 GetRigTestLocation(VRRig rig)
	{
		return rig.transform.position;
	}

	public void AddNearby(Vector3 position, Vector3 forward, List<VRRig> allRigs)
	{
		float num = this.range * this.range;
		float num2 = Mathf.Cos(this.fov * 0.017453292f);
		for (int i = 0; i < allRigs.Count; i++)
		{
			VRRig vrrig = allRigs[i];
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component.State != GRPlayer.GRPlayerState.Ghost && !component.InStealthMode && !this.rigsNearby.Contains(vrrig))
			{
				Vector3 a = GRSenseNearby.GetRigTestLocation(vrrig) - position;
				float sqrMagnitude = a.sqrMagnitude;
				float num3 = this.hearingRange * this.hearingRange;
				if (sqrMagnitude >= num3)
				{
					if (sqrMagnitude >= num)
					{
						goto IL_C4;
					}
					if (sqrMagnitude > 0f)
					{
						float d = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(a / d, forward) < num2)
						{
							goto IL_C4;
						}
					}
				}
				this.rigsNearby.Add(vrrig);
			}
			IL_C4:;
		}
	}

	public void RemoveNotNearby(Vector3 position)
	{
		float num = this.exitRange * this.exitRange;
		int i = 0;
		while (i < this.rigsNearby.Count)
		{
			VRRig vrrig = this.rigsNearby[i];
			if (!(vrrig != null))
			{
				goto IL_58;
			}
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if ((GRSenseNearby.GetRigTestLocation(vrrig) - position).sqrMagnitude > num || component.State == GRPlayer.GRPlayerState.Ghost || component.InStealthMode)
			{
				goto IL_58;
			}
			IL_68:
			i++;
			continue;
			IL_58:
			this.rigsNearby.RemoveAt(i);
			i--;
			goto IL_68;
		}
	}

	public void RemoveNoLineOfSight(Vector3 headPos, GRSenseLineOfSight senseLineOfSight)
	{
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			Vector3 rigTestLocation = GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]);
			if (!senseLineOfSight.HasLineOfSight(headPos, rigTestLocation))
			{
				this.rigsNearby.RemoveAt(i);
				i--;
			}
		}
	}

	public VRRig PickClosest(out float outDistanceSq)
	{
		Vector3 position = this.headTransform.position;
		float num = float.MaxValue;
		VRRig result = null;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			float sqrMagnitude = (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = this.rigsNearby[i];
			}
		}
		outDistanceSq = num;
		return result;
	}

	public float range;

	public float hearingRange;

	public float exitRange;

	public float fov;

	[ReadOnly]
	public List<VRRig> rigsNearby;

	private Transform headTransform;
}
