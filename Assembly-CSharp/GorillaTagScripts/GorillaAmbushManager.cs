using System;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DE7 RID: 3559
	public sealed class GorillaAmbushManager : GorillaTagManager
	{
		// Token: 0x060058AA RID: 22698 RVA: 0x001C6703 File Offset: 0x001C4903
		public override GameModeType GameType()
		{
			if (!this.isGhostTag)
			{
				return GameModeType.Ambush;
			}
			return GameModeType.Ghost;
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x060058AB RID: 22699 RVA: 0x001C6710 File Offset: 0x001C4910
		public static int HandEffectHash
		{
			get
			{
				return GorillaAmbushManager.handTapHash;
			}
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x060058AC RID: 22700 RVA: 0x001C6717 File Offset: 0x001C4917
		// (set) Token: 0x060058AD RID: 22701 RVA: 0x001C671E File Offset: 0x001C491E
		public static float HandFXScaleModifier { get; private set; }

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x060058AE RID: 22702 RVA: 0x001C6726 File Offset: 0x001C4926
		// (set) Token: 0x060058AF RID: 22703 RVA: 0x001C672E File Offset: 0x001C492E
		public bool isGhostTag { get; private set; }

		// Token: 0x060058B0 RID: 22704 RVA: 0x001C6737 File Offset: 0x001C4937
		public override void Awake()
		{
			base.Awake();
			if (this.handTapFX != null)
			{
				GorillaAmbushManager.handTapHash = PoolUtils.GameObjHashCode(this.handTapFX);
			}
			GorillaAmbushManager.HandFXScaleModifier = this.handTapScaleFactor;
		}

		// Token: 0x060058B1 RID: 22705 RVA: 0x001C6768 File Offset: 0x001C4968
		private void Start()
		{
			this.hasScryingPlane = this.scryingPlaneRef.TryResolve<MeshRenderer>(out this.scryingPlane);
			this.hasScryingPlane3p = this.scryingPlane3pRef.TryResolve<MeshRenderer>(out this.scryingPlane3p);
		}

		// Token: 0x060058B2 RID: 22706 RVA: 0x001C6798 File Offset: 0x001C4998
		public override string GameModeName()
		{
			if (!this.isGhostTag)
			{
				return "AMBUSH";
			}
			return "GHOST";
		}

		// Token: 0x060058B3 RID: 22707 RVA: 0x001C67B0 File Offset: 0x001C49B0
		public override string GameModeNameRoomLabel()
		{
			string text = this.isGhostTag ? "GAME_MODE_GHOST_ROOM_LABEL" : "GAME_MODE_AMBUSH_ROOM_LABEL";
			string defaultResult = this.isGhostTag ? "(GHOST GAME)" : "(AMBUSH GAME)";
			string result;
			if (!LocalisationManager.TryGetKeyForCurrentLocale(text, out result, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [" + text + "]");
			}
			return result;
		}

		// Token: 0x060058B4 RID: 22708 RVA: 0x001C6808 File Offset: 0x001C4A08
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			int materialIndex = this.MyMatIndex(rig.creator);
			rig.ChangeMaterialLocal(materialIndex);
			bool flag = base.IsInfected(rig.Creator);
			bool flag2 = base.IsInfected(NetworkSystem.Instance.LocalPlayer);
			rig.bodyRenderer.SetGameModeBodyType(flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default);
			rig.SetInvisibleToLocalPlayer(flag && !flag2);
			if (this.isGhostTag && rig.isOfflineVRRig)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(flag);
				if (this.hasScryingPlane)
				{
					this.scryingPlane.enabled = flag2;
				}
				if (this.hasScryingPlane3p)
				{
					this.scryingPlane3p.enabled = flag2;
				}
			}
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x001C68AE File Offset: 0x001C4AAE
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (!base.IsInfected(forPlayer))
			{
				return 0;
			}
			return 13;
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x001C68C0 File Offset: 0x001C4AC0
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				GorillaSkin.ApplyToRig(vrrig, null, GorillaSkin.SkinType.gameMode);
				vrrig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
				vrrig.SetInvisibleToLocalPlayer(false);
			}
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (this.hasScryingPlane)
			{
				this.scryingPlane.enabled = false;
			}
			if (this.hasScryingPlane3p)
			{
				this.scryingPlane3p.enabled = false;
			}
		}

		// Token: 0x040065D4 RID: 26068
		public GameObject handTapFX;

		// Token: 0x040065D5 RID: 26069
		public GorillaSkin ambushSkin;

		// Token: 0x040065D6 RID: 26070
		[SerializeField]
		private AudioClip[] firstPersonTaggedSounds;

		// Token: 0x040065D7 RID: 26071
		[SerializeField]
		private float firstPersonTaggedSoundVolume;

		// Token: 0x040065D8 RID: 26072
		private static int handTapHash = -1;

		// Token: 0x040065D9 RID: 26073
		public float handTapScaleFactor = 0.5f;

		// Token: 0x040065DB RID: 26075
		public float crawlingSpeedForMaxVolume;

		// Token: 0x040065DD RID: 26077
		[SerializeField]
		private XSceneRef scryingPlaneRef;

		// Token: 0x040065DE RID: 26078
		[SerializeField]
		private XSceneRef scryingPlane3pRef;

		// Token: 0x040065DF RID: 26079
		private const int STEALTH_MATERIAL_INDEX = 13;

		// Token: 0x040065E0 RID: 26080
		private MeshRenderer scryingPlane;

		// Token: 0x040065E1 RID: 26081
		private bool hasScryingPlane;

		// Token: 0x040065E2 RID: 26082
		private MeshRenderer scryingPlane3p;

		// Token: 0x040065E3 RID: 26083
		private bool hasScryingPlane3p;
	}
}
