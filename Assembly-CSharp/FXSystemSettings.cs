using System;
using UnityEngine;

// Token: 0x02000BE2 RID: 3042
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x06004B22 RID: 19234 RVA: 0x001886B0 File Offset: 0x001868B0
	public void Awake()
	{
		int num = (this.callLimits != null) ? this.callLimits.Length : 0;
		int num2 = (this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0;
		for (int i = 0; i < num; i++)
		{
			FXType key = this.callLimits[i].Key;
			int num3 = (int)key;
			if (num3 < 0 || num3 >= 24)
			{
				string text = "NO_PATH_AT_RUNTIME";
				Debug.LogError("FXSystemSettings: (this should never happen) `callLimits.Key` is out of bounds of `callSettings`! Path=\"" + text + "\"", this);
			}
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping.");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType key = this.CallLimitsCooldown[i].Key;
			int num3 = (int)key;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
		for (int i = 0; i < this.callSettings.Length; i++)
		{
			if (this.callSettings[i] == null)
			{
				this.callSettings[i] = new LimiterType
				{
					CallLimitSettings = new CallLimiter(0, 0f, 0f),
					Key = (FXType)i
				};
			}
		}
	}

	// Token: 0x04005B48 RID: 23368
	private const string preLog = "FXSystemSettings: ";

	// Token: 0x04005B49 RID: 23369
	private const string preErr = "ERROR!!!  FXSystemSettings: ";

	// Token: 0x04005B4A RID: 23370
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x04005B4B RID: 23371
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x04005B4C RID: 23372
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x04005B4D RID: 23373
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[24];
}
