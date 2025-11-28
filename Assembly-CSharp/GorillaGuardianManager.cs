using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000781 RID: 1921
public sealed class GorillaGuardianManager : GorillaGameManager
{
	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x06003233 RID: 12851 RVA: 0x0010ED0B File Offset: 0x0010CF0B
	// (set) Token: 0x06003234 RID: 12852 RVA: 0x0010ED13 File Offset: 0x0010CF13
	public bool isPlaying { get; private set; }

	// Token: 0x06003235 RID: 12853 RVA: 0x0010ED1C File Offset: 0x0010CF1C
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.isPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StartPlaying();
			}
		}
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x0010ED80 File Offset: 0x0010CF80
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.isPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StopPlaying();
			}
		}
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x0010EDE4 File Offset: 0x0010CFE4
	public override void ResetGame()
	{
		base.ResetGame();
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x0010EDEC File Offset: 0x0010CFEC
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GuardianRPCs>();
	}

	// Token: 0x06003239 RID: 12857 RVA: 0x00002789 File Offset: 0x00000989
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x000743B1 File Offset: 0x000725B1
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x0600323C RID: 12860 RVA: 0x0010EDFC File Offset: 0x0010CFFC
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this.IsPlayerGuardian(myPlayer) && !this.IsHoldingPlayer();
	}

	// Token: 0x0600323D RID: 12861 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalIsTagged(NetPlayer player)
	{
		return false;
	}

	// Token: 0x0600323E RID: 12862 RVA: 0x0010EE12 File Offset: 0x0010D012
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return player != null && !this.IsPlayerGuardian(player);
	}

	// Token: 0x0600323F RID: 12863 RVA: 0x0010EE24 File Offset: 0x0010D024
	public bool IsPlayerGuardian(NetPlayer player)
	{
		using (List<GorillaGuardianZoneManager>.Enumerator enumerator = GorillaGuardianZoneManager.zoneManagers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsPlayerGuardian(player))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003240 RID: 12864 RVA: 0x0010EE80 File Offset: 0x0010D080
	public void RequestEjectGuardian(NetPlayer player)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.EjectGuardian(player);
			return;
		}
		GameMode.ActiveNetworkHandler.SendRPC("GuardianRequestEject", false, Array.Empty<object>());
	}

	// Token: 0x06003241 RID: 12865 RVA: 0x0010EEA8 File Offset: 0x0010D0A8
	public void EjectGuardian(NetPlayer player)
	{
		foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
		{
			if (gorillaGuardianZoneManager.IsPlayerGuardian(player))
			{
				gorillaGuardianZoneManager.SetGuardian(null);
			}
		}
	}

	// Token: 0x06003242 RID: 12866 RVA: 0x0010EF04 File Offset: 0x0010D104
	public void LaunchPlayer(NetPlayer launcher, Vector3 velocity)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(launcher, out rigContainer))
		{
			return;
		}
		if (Vector3.Magnitude(VRRigCache.Instance.localRig.Rig.transform.position - rigContainer.Rig.transform.position) > this.requiredGuardianDistance + Mathf.Epsilon)
		{
			return;
		}
		if (velocity.sqrMagnitude > this.maxLaunchVelocity * this.maxLaunchVelocity)
		{
			return;
		}
		GTPlayer.Instance.DoLaunch(velocity);
	}

	// Token: 0x06003243 RID: 12867 RVA: 0x0010EF88 File Offset: 0x0010D188
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		base.LocalTag(taggedPlayer, taggingPlayer, bodyHit, leftHand);
		if (bodyHit)
		{
			return;
		}
		RigContainer rigContainer;
		Vector3 vector;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && this.CheckSlap(taggingPlayer, taggedPlayer, leftHand, out vector))
		{
			GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", taggedPlayer, new object[]
			{
				vector
			});
			rigContainer.Rig.ApplyLocalTrajectoryOverride(vector);
			GameMode.ActiveNetworkHandler.SendRPC("ShowSlapEffects", true, new object[]
			{
				rigContainer.Rig.transform.position,
				vector.normalized
			});
			this.LocalPlaySlapEffect(rigContainer.Rig.transform.position, vector.normalized);
		}
	}

	// Token: 0x06003244 RID: 12868 RVA: 0x0010F04C File Offset: 0x0010D24C
	private bool CheckSlap(NetPlayer slapper, NetPlayer target, bool leftHand, out Vector3 velocity)
	{
		velocity = Vector3.zero;
		if (this.IsHoldingPlayer(leftHand))
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(slapper, out rigContainer))
		{
			return false;
		}
		Vector3 vector = GTPlayer.Instance.GetHandVelocityTracker(leftHand).GetAverageVelocity(true, 0.15f, false);
		Vector3 vector2 = leftHand ? rigContainer.Rig.leftHandHoldsPlayer.transform.right : rigContainer.Rig.rightHandHoldsPlayer.transform.right;
		if (Vector3.Dot(vector.normalized, vector2) < this.slapFrontAlignmentThreshold && Vector3.Dot(vector.normalized, vector2) > this.slapBackAlignmentThreshold)
		{
			return false;
		}
		if (vector.magnitude < this.launchMinimumStrength)
		{
			return false;
		}
		vector = Vector3.ClampMagnitude(vector, this.maxLaunchVelocity);
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(target, out rigContainer2))
		{
			return false;
		}
		if (this.IsRigBeingHeld(rigContainer2.Rig) || rigContainer2.Rig.IsLocalTrajectoryOverrideActive())
		{
			return false;
		}
		if (!this.CheckLaunchRetriggerDelay(rigContainer2.Rig))
		{
			return false;
		}
		vector *= this.launchStrengthMultiplier;
		Vector3 vector3;
		if (rigContainer2.Rig.IsOnGround(this.launchGroundHeadCheckDist, this.launchGroundHandCheckDist, out vector3))
		{
			vector += vector3 * this.launchGroundKickup * Mathf.Clamp01(1f - Vector3.Dot(vector3, vector.normalized));
		}
		velocity = vector;
		return true;
	}

	// Token: 0x06003245 RID: 12869 RVA: 0x0010F1B8 File Offset: 0x0010D3B8
	public override void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
		base.HandleHandTap(tappingPlayer, hitTappable, leftHand, handVelocity, tapSurfaceNormal);
		if (hitTappable != null)
		{
			TappableGuardianIdol tappableGuardianIdol = hitTappable as TappableGuardianIdol;
			if (tappableGuardianIdol != null && tappableGuardianIdol.isActivationReady)
			{
				tappableGuardianIdol.isActivationReady = false;
				GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tapHapticStrength * this.hapticStrength, GorillaTagger.Instance.tapHapticDuration * this.hapticDuration);
			}
		}
		if (!this.IsPlayerGuardian(tappingPlayer))
		{
			return;
		}
		if (this.IsHoldingPlayer(leftHand))
		{
			return;
		}
		float num = Vector3.Dot(Vector3.down, handVelocity);
		if (num < this.slamTriggerTapSpeed || Vector3.Dot(Vector3.down, handVelocity.normalized) < this.slamTriggerAngle)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(tappingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand;
		Vector3 vector = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * rigContainer.Rig.scaleFactor;
		Vector3 vector2 = vrmap.rigTarget.position - vector;
		float num2 = Mathf.Clamp01((num - this.slamTriggerTapSpeed) / (this.slamMaxTapSpeed - this.slamTriggerTapSpeed));
		num2 = Mathf.Lerp(this.slamMinStrengthMultiplier, this.slamMaxStrengthMultiplier, num2);
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer2;
			if (RoomSystem.PlayersInRoom[i] != tappingPlayer && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer2))
			{
				VRRig rig = rigContainer2.Rig;
				if (!this.IsRigBeingHeld(rig) && this.CheckLaunchRetriggerDelay(rig))
				{
					Vector3 position = rig.transform.position;
					if (Vector3.SqrMagnitude(position - vector2) < this.slamRadius * this.slamRadius)
					{
						Vector3 vector3 = (position - vector2).normalized * num2;
						vector3 = Vector3.ClampMagnitude(vector3, this.maxLaunchVelocity);
						GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", RoomSystem.PlayersInRoom[i], new object[]
						{
							vector3
						});
					}
				}
			}
		}
		this.LocalPlaySlamEffect(vector2, Vector3.up);
		GameMode.ActiveNetworkHandler.SendRPC("ShowSlamEffect", true, new object[]
		{
			vector2,
			Vector3.up
		});
	}

	// Token: 0x06003246 RID: 12870 RVA: 0x0010F42A File Offset: 0x0010D62A
	private bool CheckLaunchRetriggerDelay(VRRig launchedRig)
	{
		return launchedRig.fxSettings.callSettings[7].CallLimitSettings.CheckCallTime(Time.time);
	}

	// Token: 0x06003247 RID: 12871 RVA: 0x0010F448 File Offset: 0x0010D648
	private bool IsHoldingPlayer()
	{
		return this.IsHoldingPlayer(true) || this.IsHoldingPlayer(false);
	}

	// Token: 0x06003248 RID: 12872 RVA: 0x0010F45C File Offset: 0x0010D65C
	private bool IsHoldingPlayer(bool leftHand)
	{
		return (leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment is HoldableHand) || (!leftHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment is HoldableHand);
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x0010F4B8 File Offset: 0x0010D6B8
	private bool IsRigBeingHeld(VRRig rig)
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment != null)
		{
			HoldableHand holdableHand = EquipmentInteractor.instance.leftHandHeldEquipment as HoldableHand;
			if (holdableHand != null && holdableHand.Rig == rig)
			{
				return true;
			}
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null)
		{
			HoldableHand holdableHand2 = EquipmentInteractor.instance.rightHandHeldEquipment as HoldableHand;
			if (holdableHand2 != null && holdableHand2.Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x0010F52C File Offset: 0x0010D72C
	public override GameModeType GameType()
	{
		return GameModeType.Guardian;
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x0010F52F File Offset: 0x0010D72F
	public void PlaySlapEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlapEffect(location, direction);
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x0010F539 File Offset: 0x0010D739
	private void LocalPlaySlapEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slapImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x0010F554 File Offset: 0x0010D754
	public void PlaySlamEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlamEffect(location, direction);
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x0010F55E File Offset: 0x0010D75E
	private void LocalPlaySlamEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slamImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x040040A2 RID: 16546
	[Space]
	[SerializeField]
	private float slapFrontAlignmentThreshold = 0.7f;

	// Token: 0x040040A3 RID: 16547
	[SerializeField]
	private float slapBackAlignmentThreshold = 0.7f;

	// Token: 0x040040A4 RID: 16548
	[SerializeField]
	private float launchMinimumStrength = 6f;

	// Token: 0x040040A5 RID: 16549
	[SerializeField]
	private float launchStrengthMultiplier = 1f;

	// Token: 0x040040A6 RID: 16550
	[SerializeField]
	private float launchGroundHeadCheckDist = 1.2f;

	// Token: 0x040040A7 RID: 16551
	[SerializeField]
	private float launchGroundHandCheckDist = 0.4f;

	// Token: 0x040040A8 RID: 16552
	[SerializeField]
	private float launchGroundKickup = 3f;

	// Token: 0x040040A9 RID: 16553
	[Space]
	[SerializeField]
	private float slamTriggerTapSpeed = 7f;

	// Token: 0x040040AA RID: 16554
	[SerializeField]
	private float slamMaxTapSpeed = 16f;

	// Token: 0x040040AB RID: 16555
	[SerializeField]
	private float slamTriggerAngle = 0.7f;

	// Token: 0x040040AC RID: 16556
	[SerializeField]
	private float slamRadius = 2.4f;

	// Token: 0x040040AD RID: 16557
	[SerializeField]
	private float slamMinStrengthMultiplier = 3f;

	// Token: 0x040040AE RID: 16558
	[SerializeField]
	private float slamMaxStrengthMultiplier = 10f;

	// Token: 0x040040AF RID: 16559
	[Space]
	[SerializeField]
	private GameObject slapImpactPrefab;

	// Token: 0x040040B0 RID: 16560
	[SerializeField]
	private GameObject slamImpactPrefab;

	// Token: 0x040040B1 RID: 16561
	[Space]
	[SerializeField]
	private float hapticStrength = 1f;

	// Token: 0x040040B2 RID: 16562
	[SerializeField]
	private float hapticDuration = 1f;

	// Token: 0x040040B4 RID: 16564
	private float requiredGuardianDistance = 10f;

	// Token: 0x040040B5 RID: 16565
	private float maxLaunchVelocity = 20f;
}
