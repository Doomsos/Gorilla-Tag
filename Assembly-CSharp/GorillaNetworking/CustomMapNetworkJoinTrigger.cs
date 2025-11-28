using System;

namespace GorillaNetworking
{
	// Token: 0x02000EF5 RID: 3829
	public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06006021 RID: 24609 RVA: 0x001F01FC File Offset: 0x001EE3FC
		public override string GetFullDesiredGameModeString()
		{
			return string.Concat(new string[]
			{
				this.networkZone,
				GorillaComputer.instance.currentQueue,
				CustomMapLoader.LoadedMapModId.ToString(),
				"_",
				CustomMapLoader.LoadedMapModFileId.ToString(),
				base.GetDesiredGameType()
			});
		}

		// Token: 0x06006022 RID: 24610 RVA: 0x001F0263 File Offset: 0x001EE463
		public override byte GetRoomSize()
		{
			return CustomMapLoader.GetRoomSizeForCurrentlyLoadedMap();
		}
	}
}
