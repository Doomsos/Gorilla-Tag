using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008EF RID: 2287
public class TappableBeeHive : Tappable
{
	// Token: 0x06003A87 RID: 14983 RVA: 0x00135534 File Offset: 0x00133734
	private void Awake()
	{
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			Debug.LogError("TappableBeeHive: Disabling because swarmEmergePoint is null at: " + base.transform.GetPath(), this);
			base.enabled = false;
			return;
		}
		base.GetComponent<SlingshotProjectileHitNotifier>().OnProjectileHit += this.OnSlingshotHit;
	}

	// Token: 0x06003A88 RID: 14984 RVA: 0x00135598 File Offset: 0x00133798
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x0013560C File Offset: 0x0013380C
	public void OnSlingshotHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x040049D6 RID: 18902
	[SerializeField]
	private GameObject swarmEmergeFromPoint;

	// Token: 0x040049D7 RID: 18903
	[SerializeField]
	private GameObject swarmEmergeToPoint;

	// Token: 0x040049D8 RID: 18904
	[SerializeField]
	private GameObject honeycombSurface;

	// Token: 0x040049D9 RID: 18905
	[SerializeField]
	private float honeycombDisableDuration;

	// Token: 0x040049DA RID: 18906
	[NonSerialized]
	private TimeSince _timeSinceLastTap;

	// Token: 0x040049DB RID: 18907
	private float reenableHoneycombAtTimestamp;

	// Token: 0x040049DC RID: 18908
	private Coroutine reenableHoneycombCoroutine;
}
