using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000BE4 RID: 3044
internal class GuardianRPCs : RPCNetworkBase
{
	// Token: 0x06004B27 RID: 19239 RVA: 0x001889CE File Offset: 0x00186BCE
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.guardianManager = (GorillaGuardianManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06004B28 RID: 19240 RVA: 0x001889E8 File Offset: 0x00186BE8
	[PunRPC]
	public void GuardianRequestEject(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GuardianRequestEject");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (photonMessageInfoWrapped.Sender != null)
		{
			this.guardianManager.EjectGuardian(photonMessageInfoWrapped.Sender);
		}
	}

	// Token: 0x06004B29 RID: 19241 RVA: 0x00188A24 File Offset: 0x00186C24
	[PunRPC]
	public void GuardianLaunchPlayer(Vector3 velocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GuardianLaunchPlayer");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent LaunchPlayer when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (!velocity.IsValid(num))
		{
			return;
		}
		if (!this.launchCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.guardianManager.LaunchPlayer(photonMessageInfoWrapped.Sender, velocity);
	}

	// Token: 0x06004B2A RID: 19242 RVA: 0x00188AB8 File Offset: 0x00186CB8
	[PunRPC]
	public void ShowSlapEffects(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ShowSlapEffects");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent ShowSlapEffects when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (location.IsValid(num))
		{
			float num2 = 10000f;
			if (direction.IsValid(num2))
			{
				if (!this.slapFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlapEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x06004B2B RID: 19243 RVA: 0x00188B58 File Offset: 0x00186D58
	[PunRPC]
	public void ShowSlamEffect(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ShowSlamEffect");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent ShowSlamEffect when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (location.IsValid(num))
		{
			float num2 = 10000f;
			if (direction.IsValid(num2))
			{
				if (!this.slamFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlamEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x04005B50 RID: 23376
	private GameModeSerializer serializer;

	// Token: 0x04005B51 RID: 23377
	private GorillaGuardianManager guardianManager;

	// Token: 0x04005B52 RID: 23378
	private CallLimiter launchCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04005B53 RID: 23379
	private CallLimiter slapFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04005B54 RID: 23380
	private CallLimiter slamFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);
}
