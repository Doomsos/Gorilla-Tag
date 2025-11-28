using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009B6 RID: 2486
internal class VirtualStumpTeleporterSerializer : GorillaSerializer
{
	// Token: 0x06003F95 RID: 16277 RVA: 0x00154FB3 File Offset: 0x001531B3
	public void NotifyPlayerTeleporting(short teleporterIdx, AudioSource localPlayerTeleporterAudioSource)
	{
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[]
			{
				false,
				teleporterIdx
			});
		}
	}

	// Token: 0x06003F96 RID: 16278 RVA: 0x00154FF0 File Offset: 0x001531F0
	public void NotifyPlayerReturning(short teleporterIdx)
	{
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		Debug.Log(string.Format("[VRTeleporterSerializer::NotifyPlayerReturning] Sending RPC to activate VFX at idx: {0}", teleporterIdx));
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[]
			{
				true,
				teleporterIdx
			});
		}
	}

	// Token: 0x06003F97 RID: 16279 RVA: 0x0015504C File Offset: 0x0015324C
	[PunRPC]
	private void ActivateTeleportVFX(bool returning, short teleporterIdx, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ActivateTeleportVFX");
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[13].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VirtualStumpTeleporter virtualStumpTeleporter = this.teleporters[(int)teleporterIdx];
		if (virtualStumpTeleporter.IsNotNull())
		{
			virtualStumpTeleporter.PlayTeleportEffects(false, !returning, null, false);
		}
	}

	// Token: 0x06003F98 RID: 16280 RVA: 0x001550DC File Offset: 0x001532DC
	public short GetTeleporterIndex(VirtualStumpTeleporter teleporter)
	{
		short num = 0;
		while ((int)num < this.teleporters.Count)
		{
			if (this.teleporters[(int)num] == teleporter)
			{
				return num;
			}
			num += 1;
		}
		return -1;
	}

	// Token: 0x040050BB RID: 20667
	[SerializeField]
	public List<VirtualStumpTeleporter> teleporters = new List<VirtualStumpTeleporter>();

	// Token: 0x040050BC RID: 20668
	[SerializeField]
	public List<ParticleSystem> teleporterVFX = new List<ParticleSystem>();

	// Token: 0x040050BD RID: 20669
	[SerializeField]
	public List<ParticleSystem> returnVFX = new List<ParticleSystem>();

	// Token: 0x040050BE RID: 20670
	[SerializeField]
	public List<AudioSource> teleportAudioSource = new List<AudioSource>();

	// Token: 0x040050BF RID: 20671
	[SerializeField]
	public List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x040050C0 RID: 20672
	[SerializeField]
	public List<AudioClip> observerSoundClips = new List<AudioClip>();
}
