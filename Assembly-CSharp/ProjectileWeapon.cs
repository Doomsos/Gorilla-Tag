using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200027C RID: 636
public abstract class ProjectileWeapon : TransferrableObject
{
	// Token: 0x06001050 RID: 4176
	protected abstract Vector3 GetLaunchPosition();

	// Token: 0x06001051 RID: 4177
	protected abstract Vector3 GetLaunchVelocity();

	// Token: 0x06001052 RID: 4178 RVA: 0x000556D7 File Offset: 0x000538D7
	internal override void OnEnable()
	{
		base.OnEnable();
		if (base.myOnlineRig != null)
		{
			base.myOnlineRig.projectileWeapon = this;
		}
		if (base.myRig != null)
		{
			base.myRig.projectileWeapon = this;
		}
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x00055714 File Offset: 0x00053914
	protected void LaunchProjectile()
	{
		int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		int trailHash = PoolUtils.GameObjHashCode(this.projectileTrail);
		GameObject gameObject = ObjectPools.instance.Instantiate(hash, true);
		float num = Mathf.Abs(base.transform.lossyScale.x);
		gameObject.transform.localScale = Vector3.one * num;
		Vector3 launchPosition = this.GetLaunchPosition();
		Vector3 launchVelocity = this.GetLaunchVelocity();
		bool blueTeam;
		bool orangeTeam;
		bool flag;
		this.GetIsOnTeams(out blueTeam, out orangeTeam, out flag);
		this.AttachTrail(trailHash, gameObject, launchPosition, blueTeam, orangeTeam, flag && this.targetRig, this.targetRig ? this.targetRig.playerColor : default(Color));
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		if (NetworkSystem.Instance.InRoom)
		{
			int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(component, launchVelocity, launchPosition, num);
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, blueTeam, orangeTeam, projectileCount, num, flag, base.myRig.playerColor);
			TransferrableObject.PositionState currentState = this.currentState;
			RoomSystem.SendLaunchProjectile(launchPosition, launchVelocity, RoomSystem.ProjectileSource.ProjectileWeapon, projectileCount, false, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			this.PlayLaunchSfx();
		}
		else
		{
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, blueTeam, orangeTeam, 0, num, flag, base.myRig.playerColor);
			this.PlayLaunchSfx();
		}
		PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x0005588C File Offset: 0x00053A8C
	internal virtual SlingshotProjectile LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfoWrapped info)
	{
		GameObject gameObject = null;
		SlingshotProjectile slingshotProjectile = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		try
		{
			int hash = -1;
			int num = -1;
			if (projectileSource == RoomSystem.ProjectileSource.ProjectileWeapon)
			{
				if (this.currentState == TransferrableObject.PositionState.OnChest || this.currentState == TransferrableObject.PositionState.None)
				{
					return null;
				}
				hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
				num = PoolUtils.GameObjHashCode(this.projectileTrail);
			}
			gameObject = ObjectPools.instance.Instantiate(hash, true);
			slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
			bool blueTeam;
			bool orangeTeam;
			bool flag;
			this.GetIsOnTeams(out blueTeam, out orangeTeam, out flag);
			if (flag && !shouldOverrideColor && this.targetRig)
			{
				shouldOverrideColor = true;
				color = this.targetRig.playerColor;
			}
			if (num != -1)
			{
				this.AttachTrail(num, slingshotProjectile.gameObject, location, blueTeam, orangeTeam, shouldOverrideColor, color);
			}
			slingshotProjectile.Launch(location, velocity, player, blueTeam, orangeTeam, projectileCounter, scale, shouldOverrideColor, color);
			this.PlayLaunchSfx();
		}
		catch
		{
			GorillaNot.instance.SendReport("projectile error", player.UserId, player.NickName);
			if (slingshotProjectile != null && slingshotProjectile)
			{
				slingshotProjectile.transform.position = Vector3.zero;
				slingshotProjectile.Deactivate();
				slingshotProjectile = null;
			}
			else if (gameObject.IsNotNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
		}
		return slingshotProjectile;
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x000559D4 File Offset: 0x00053BD4
	protected void GetIsOnTeams(out bool blueTeam, out bool orangeTeam, out bool shouldUsePlayerColor)
	{
		NetPlayer player = base.OwningPlayer();
		blueTeam = false;
		orangeTeam = false;
		shouldUsePlayerColor = false;
		if (GorillaGameManager.instance != null)
		{
			GorillaPaintbrawlManager component = GorillaGameManager.instance.GetComponent<GorillaPaintbrawlManager>();
			if (component != null)
			{
				blueTeam = component.OnBlueTeam(player);
				orangeTeam = component.OnRedTeam(player);
				shouldUsePlayerColor = (!blueTeam && !orangeTeam);
			}
		}
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x00055A34 File Offset: 0x00053C34
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x00055A84 File Offset: 0x00053C84
	private void PlayLaunchSfx()
	{
		if (this.shootSfx != null && this.shootSfxClips != null && this.shootSfxClips.Length != 0)
		{
			this.shootSfx.GTPlayOneShot(this.shootSfxClips[Random.Range(0, this.shootSfxClips.Length)], 1f);
		}
	}

	// Token: 0x04001448 RID: 5192
	[SerializeField]
	protected GameObject projectilePrefab;

	// Token: 0x04001449 RID: 5193
	[SerializeField]
	private GameObject projectileTrail;

	// Token: 0x0400144A RID: 5194
	public AudioClip[] shootSfxClips;

	// Token: 0x0400144B RID: 5195
	public AudioSource shootSfx;
}
