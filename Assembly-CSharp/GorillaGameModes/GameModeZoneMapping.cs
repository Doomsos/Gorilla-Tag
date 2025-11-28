using System;
using System.Collections.Generic;
using GameObjectScheduling;
using GorillaNetworking;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000D8C RID: 3468
	[CreateAssetMenu(fileName = "New Game Mode Zone Map", menuName = "Game Settings/Game Mode Zone Map", order = 2)]
	public class GameModeZoneMapping : ScriptableObject
	{
		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x06005522 RID: 21794 RVA: 0x001AC925 File Offset: 0x001AAB25
		public HashSet<GameModeType> AllModes
		{
			get
			{
				this.init();
				return this.allModes;
			}
		}

		// Token: 0x06005523 RID: 21795 RVA: 0x001AC934 File Offset: 0x001AAB34
		private void init()
		{
			if (this.allModes != null)
			{
				return;
			}
			this.allModes = new HashSet<GameModeType>();
			for (int i = 0; i < this.defaultGameModes.Length; i++)
			{
				if (!this.allModes.Contains(this.defaultGameModes[i]))
				{
					this.allModes.Add(this.defaultGameModes[i]);
				}
			}
			this.publicZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			this.privateZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			for (int j = 0; j < this.zoneGameModes.Length; j++)
			{
				for (int k = 0; k < this.zoneGameModes[j].zone.Length; k++)
				{
					this.publicZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					for (int l = 0; l < this.zoneGameModes[j].modes.Length; l++)
					{
						if (!this.allModes.Contains(this.zoneGameModes[j].modes[l]))
						{
							this.allModes.Add(this.zoneGameModes[j].modes[l]);
						}
					}
					if (this.zoneGameModes[j].privateModes.Length != 0)
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].privateModes));
						for (int m = 0; m < this.zoneGameModes[j].privateModes.Length; m++)
						{
							if (!this.allModes.Contains(this.zoneGameModes[j].privateModes[m]))
							{
								this.allModes.Add(this.zoneGameModes[j].privateModes[m]);
							}
						}
					}
					else
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					}
				}
			}
			this.modeNameLookup = new Dictionary<GameModeType, string>();
			for (int n = 0; n < this.gameModeNameOverrides.Length; n++)
			{
				this.modeNameLookup.Add(this.gameModeNameOverrides[n].mode, this.gameModeNameOverrides[n].displayName);
			}
			this.isNewLookup = new HashSet<GameModeType>(this.newThisUpdate);
			this.gameModeTypeCountdownsLookup = new Dictionary<GameModeType, CountdownTextDate>();
			for (int num = 0; num < this.gameModeTypeCountdowns.Length; num++)
			{
				this.gameModeTypeCountdownsLookup.Add(this.gameModeTypeCountdowns[num].mode, this.gameModeTypeCountdowns[num].countdownTextDate);
			}
		}

		// Token: 0x06005524 RID: 21796 RVA: 0x001ACC0C File Offset: 0x001AAE0C
		public HashSet<GameModeType> GetModesForZone(GTZone zone, bool isPrivate)
		{
			this.init();
			if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
			{
				return this.privateZoneGameModesLookup[zone];
			}
			if (this.publicZoneGameModesLookup.ContainsKey(zone))
			{
				return this.publicZoneGameModesLookup[zone];
			}
			return new HashSet<GameModeType>(this.defaultGameModes);
		}

		// Token: 0x06005525 RID: 21797 RVA: 0x001ACC63 File Offset: 0x001AAE63
		internal string GetModeName(GameModeType mode)
		{
			this.init();
			if (this.modeNameLookup.ContainsKey(mode))
			{
				return this.modeNameLookup[mode];
			}
			return mode.ToString().ToUpper();
		}

		// Token: 0x06005526 RID: 21798 RVA: 0x001ACC98 File Offset: 0x001AAE98
		internal bool IsNew(GameModeType mode)
		{
			this.init();
			return this.isNewLookup.Contains(mode);
		}

		// Token: 0x06005527 RID: 21799 RVA: 0x001ACCAC File Offset: 0x001AAEAC
		internal CountdownTextDate GetCountdown(GameModeType mode)
		{
			this.init();
			if (this.gameModeTypeCountdownsLookup.ContainsKey(mode))
			{
				return this.gameModeTypeCountdownsLookup[mode];
			}
			return null;
		}

		// Token: 0x06005528 RID: 21800 RVA: 0x001ACCD0 File Offset: 0x001AAED0
		internal GameModeType VerifyModeForZone(GTZone zone, GameModeType mode, bool isPrivate)
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				zone = GTZone.customMaps;
			}
			if (zone == GTZone.none)
			{
				if (this.allModes.Contains(mode))
				{
					return mode;
				}
				return GameModeType.Casual;
			}
			else
			{
				HashSet<GameModeType> hashSet;
				if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
				{
					hashSet = this.privateZoneGameModesLookup[zone];
				}
				else if (this.publicZoneGameModesLookup.ContainsKey(zone))
				{
					hashSet = this.publicZoneGameModesLookup[zone];
				}
				else
				{
					hashSet = new HashSet<GameModeType>(this.defaultGameModes);
				}
				if (hashSet.Contains(mode))
				{
					return mode;
				}
				using (HashSet<GameModeType>.Enumerator enumerator = hashSet.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
				return GameModeType.Casual;
			}
		}

		// Token: 0x0400620B RID: 25099
		[SerializeField]
		private GameModeNameOverrides[] gameModeNameOverrides;

		// Token: 0x0400620C RID: 25100
		[SerializeField]
		private GameModeType[] defaultGameModes;

		// Token: 0x0400620D RID: 25101
		[SerializeField]
		private ZoneGameModes[] zoneGameModes;

		// Token: 0x0400620E RID: 25102
		[SerializeField]
		private GameModeTypeCountdown[] gameModeTypeCountdowns;

		// Token: 0x0400620F RID: 25103
		[SerializeField]
		private GameModeType[] newThisUpdate;

		// Token: 0x04006210 RID: 25104
		private Dictionary<GTZone, HashSet<GameModeType>> publicZoneGameModesLookup;

		// Token: 0x04006211 RID: 25105
		private Dictionary<GTZone, HashSet<GameModeType>> privateZoneGameModesLookup;

		// Token: 0x04006212 RID: 25106
		private Dictionary<GameModeType, string> modeNameLookup;

		// Token: 0x04006213 RID: 25107
		private HashSet<GameModeType> isNewLookup;

		// Token: 0x04006214 RID: 25108
		private Dictionary<GameModeType, CountdownTextDate> gameModeTypeCountdownsLookup;

		// Token: 0x04006215 RID: 25109
		private HashSet<GameModeType> allModes;
	}
}
