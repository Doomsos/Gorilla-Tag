using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020006FC RID: 1788
[Serializable]
public class GRSenseNearby
{
	// Token: 0x06002DE1 RID: 11745 RVA: 0x000F9425 File Offset: 0x000F7625
	public void Setup(Transform headTransform)
	{
		this.rigsNearby = new List<VRRig>();
		this.headTransform = headTransform;
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000F943C File Offset: 0x000F763C
	public void UpdateNearby(List<VRRig> allRigs, GRSenseLineOfSight senseLineOfSight)
	{
		Vector3 position = this.headTransform.position;
		Vector3 forward = this.headTransform.rotation * Vector3.forward;
		this.RemoveNotNearby(position);
		this.AddNearby(position, forward, allRigs);
		this.RemoveNoLineOfSight(position, senseLineOfSight);
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000F9483 File Offset: 0x000F7683
	public bool IsAnyoneNearby()
	{
		return !GhostReactorManager.AggroDisabled && this.rigsNearby != null && this.rigsNearby.Count > 0;
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000F94A4 File Offset: 0x000F76A4
	public static Vector3 GetRigTestLocation(VRRig rig)
	{
		return rig.transform.position;
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000F94B4 File Offset: 0x000F76B4
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
				Vector3 vector = GRSenseNearby.GetRigTestLocation(vrrig) - position;
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					if (sqrMagnitude > 0f)
					{
						float num3 = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(vector / num3, forward) < num2)
						{
							goto IL_AB;
						}
					}
					this.rigsNearby.Add(vrrig);
				}
			}
			IL_AB:;
		}
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000F957C File Offset: 0x000F777C
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

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000F9604 File Offset: 0x000F7804
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

	// Token: 0x06002DE8 RID: 11752 RVA: 0x000F9654 File Offset: 0x000F7854
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

	// Token: 0x04003BEC RID: 15340
	public float range;

	// Token: 0x04003BED RID: 15341
	public float exitRange;

	// Token: 0x04003BEE RID: 15342
	public float fov;

	// Token: 0x04003BEF RID: 15343
	[ReadOnly]
	public List<VRRig> rigsNearby;

	// Token: 0x04003BF0 RID: 15344
	private Transform headTransform;
}
