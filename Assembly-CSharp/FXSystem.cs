using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000BDD RID: 3037
public static class FXSystem
{
	// Token: 0x06004B18 RID: 19224 RVA: 0x00188484 File Offset: 0x00186684
	public static void PlayFXForRig(FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX();
			return;
		}
		if (FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			context.OnPlayFX();
		}
	}

	// Token: 0x06004B19 RID: 19225 RVA: 0x001884C0 File Offset: 0x001866C0
	public static void PlayFXForRigValidated(List<int> hashes, FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		for (int i = 0; i < hashes.Count; i++)
		{
			if (!ObjectPools.instance.DoesPoolExist(hashes[i]))
			{
				return;
			}
		}
		FXSystem.PlayFXForRig(fxType, context, info);
	}

	// Token: 0x06004B1A RID: 19226 RVA: 0x001884FC File Offset: 0x001866FC
	public static void PlayFX<T>(FXType fxType, IFXContextParems<T> context, T args, PhotonMessageInfoWrapped info) where T : FXSArgs
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX(args);
			return;
		}
		if (FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			context.OnPlayFX(args);
		}
	}

	// Token: 0x06004B1B RID: 19227 RVA: 0x00188538 File Offset: 0x00186738
	public static void PlayFXForRig<T>(FXType fxType, IFXEffectContext<T> context, PhotonMessageInfoWrapped info) where T : IFXEffectContextObject
	{
		FXSystemSettings settings = context.settings;
		if (!settings.forLocalRig && !FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			return;
		}
		FXSystem.PlayFX(context.effectContext);
	}

	// Token: 0x06004B1C RID: 19228 RVA: 0x00188578 File Offset: 0x00186778
	public static void PlayFX(IFXEffectContextObject effectContext)
	{
		effectContext.OnTriggerActions();
		List<int> prefabPoolIds = effectContext.PrefabPoolIds;
		if (prefabPoolIds != null)
		{
			int count = prefabPoolIds.Count;
			for (int i = 0; i < count; i++)
			{
				int num = prefabPoolIds[i];
				if (num != -1)
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(num, effectContext.Position, effectContext.Rotation, false);
					gameObject.SetActive(true);
					effectContext.OnPlayVisualFX(num, gameObject);
				}
			}
		}
		AudioSource soundSource = effectContext.SoundSource;
		if (soundSource.IsNull())
		{
			return;
		}
		AudioClip sound = effectContext.Sound;
		if (sound.IsNotNull())
		{
			soundSource.volume = effectContext.Volume;
			soundSource.pitch = effectContext.Pitch;
			soundSource.GTPlayOneShot(sound, 1f);
			effectContext.OnPlaySoundFX(soundSource);
		}
	}

	// Token: 0x06004B1D RID: 19229 RVA: 0x00188634 File Offset: 0x00186834
	public static bool CheckCallSpam(FXSystemSettings settings, int index, double serverTime)
	{
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[index];
		if (!callLimitType.UseNetWorkTime)
		{
			return callLimitType.CallLimitSettings.CheckCallTime(Time.time);
		}
		return callLimitType.CallLimitSettings.CheckCallServerTime(serverTime);
	}
}
