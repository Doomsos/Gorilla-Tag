using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F72 RID: 3954
	public class TagEffectsLibrary : MonoBehaviour
	{
		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x060062E2 RID: 25314 RVA: 0x001FE2DB File Offset: 0x001FC4DB
		public static float FistBumpSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.fistBumpSpeedThreshold;
			}
		}

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x060062E3 RID: 25315 RVA: 0x001FE2E7 File Offset: 0x001FC4E7
		public static float HighFiveSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.highFiveSpeedThreshold;
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x060062E4 RID: 25316 RVA: 0x001FE2F3 File Offset: 0x001FC4F3
		public static bool DebugMode
		{
			get
			{
				return TagEffectsLibrary._instance.debugMode;
			}
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x001FE2FF File Offset: 0x001FC4FF
		private void Awake()
		{
			if (TagEffectsLibrary._instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TagEffectsLibrary._instance = this;
			this.tagEffectsPool = new Dictionary<string, Queue<GameObjectOnDisableDispatcher>>();
			this.tagEffectsComboLookUp = new Dictionary<TagEffectsCombo, TagEffectPack[]>();
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x001FE338 File Offset: 0x001FC538
		public static void PlayEffect(Transform target, bool isLeftHand, float rigScale, TagEffectsLibrary.EffectType effectType, TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack, Quaternion rotation)
		{
			if (TagEffectsLibrary._instance == null)
			{
				return;
			}
			ModeTagEffect modeTagEffect = null;
			TagEffectPack tagEffectPack = null;
			GameModeType gameModeType = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
			for (int i = 0; i < TagEffectsLibrary._instance.defaultTagEffects.Length; i++)
			{
				if (TagEffectsLibrary._instance.defaultTagEffects[i] != null && TagEffectsLibrary._instance.defaultTagEffects[i].Modes.Contains(gameModeType))
				{
					modeTagEffect = TagEffectsLibrary._instance.defaultTagEffects[i];
					tagEffectPack = modeTagEffect.tagEffect;
					break;
				}
			}
			if (tagEffectPack == null)
			{
				return;
			}
			GameObject firstPerson = tagEffectPack.firstPerson;
			GameObject thirdPerson = tagEffectPack.thirdPerson;
			GameObject fistBump = tagEffectPack.fistBump;
			GameObject highFive = tagEffectPack.highFive;
			bool firstPersonParentEffect = tagEffectPack.firstPersonParentEffect;
			bool thirdPersonParentEffect = tagEffectPack.thirdPersonParentEffect;
			bool flag = tagEffectPack.fistBumpParentEffect;
			bool highFiveParentEffect = tagEffectPack.highFiveParentEffect;
			if (playerCosmeticTagEffectPack != null)
			{
				TagEffectPack tagEffectPack2 = TagEffectsLibrary.comboLookup(playerCosmeticTagEffectPack, otherPlayerCosmeticTagEffectPack);
				if (!modeTagEffect.blockFistBumpOverride && playerCosmeticTagEffectPack.fistBump != null)
				{
					fistBump = tagEffectPack2.fistBump;
					flag = tagEffectPack2.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockHiveFiveOverride && playerCosmeticTagEffectPack.highFive != null)
				{
					highFive = tagEffectPack2.highFive;
					highFiveParentEffect = tagEffectPack2.highFiveParentEffect;
				}
			}
			if (otherPlayerCosmeticTagEffectPack != null)
			{
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.firstPerson != null)
				{
					firstPerson = otherPlayerCosmeticTagEffectPack.firstPerson;
					firstPersonParentEffect = otherPlayerCosmeticTagEffectPack.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.thirdPerson != null)
				{
					thirdPerson = otherPlayerCosmeticTagEffectPack.thirdPerson;
					thirdPersonParentEffect = otherPlayerCosmeticTagEffectPack.thirdPersonParentEffect;
				}
			}
			switch (effectType)
			{
			case TagEffectsLibrary.EffectType.FIRST_PERSON:
				TagEffectsLibrary.placeEffects(firstPerson, target, firstPersonParentEffect ? 1f : rigScale, false, firstPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.THIRD_PERSON:
				TagEffectsLibrary.placeEffects(thirdPerson, target, thirdPersonParentEffect ? 1f : rigScale, false, thirdPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.HIGH_FIVE:
				TagEffectsLibrary.placeEffects(highFive, target, highFiveParentEffect ? 1f : rigScale, isLeftHand, highFiveParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.FIST_BUMP:
				TagEffectsLibrary.placeEffects(fistBump, target, flag ? 1f : rigScale, isLeftHand, flag, rotation);
				return;
			default:
				return;
			}
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x001FE558 File Offset: 0x001FC758
		private static TagEffectPack comboLookup(TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack)
		{
			if (otherPlayerCosmeticTagEffectPack == null)
			{
				return playerCosmeticTagEffectPack;
			}
			TagEffectsCombo tagEffectsCombo = new TagEffectsCombo();
			tagEffectsCombo.inputA = playerCosmeticTagEffectPack;
			tagEffectsCombo.inputB = otherPlayerCosmeticTagEffectPack;
			TagEffectPack[] array;
			if (!TagEffectsLibrary._instance.tagEffectsComboLookUp.TryGetValue(tagEffectsCombo, ref array))
			{
				return playerCosmeticTagEffectPack;
			}
			int num = 0;
			if (GorillaComputer.instance != null)
			{
				num = GorillaComputer.instance.GetServerTime().Second;
			}
			return array[num % array.Length];
		}

		// Token: 0x060062E8 RID: 25320 RVA: 0x001FE5C8 File Offset: 0x001FC7C8
		public static void placeEffects(GameObject prefab, Transform target, float scale, bool flipZAxis, bool parentEffect, Quaternion rotation)
		{
			if (prefab == null)
			{
				return;
			}
			Queue<GameObjectOnDisableDispatcher> queue;
			if (!TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(prefab.name, ref queue))
			{
				queue = new Queue<GameObjectOnDisableDispatcher>();
				TagEffectsLibrary._instance.tagEffectsPool.Add(prefab.name, queue);
			}
			if (queue.Count == 0 || (queue.Peek().gameObject.activeInHierarchy && queue.Count < 12))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(prefab, target.transform.position, rotation, parentEffect ? target : TagEffectsLibrary._instance.transform);
				gameObject.name = prefab.name;
				gameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
				GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher;
				if (!gameObject.TryGetComponent<GameObjectOnDisableDispatcher>(ref gameObjectOnDisableDispatcher))
				{
					gameObjectOnDisableDispatcher = gameObject.AddComponent<GameObjectOnDisableDispatcher>();
				}
				gameObjectOnDisableDispatcher.OnDisabled += TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				gameObject.SetActive(true);
				queue.Enqueue(gameObjectOnDisableDispatcher);
				return;
			}
			GameObjectOnDisableDispatcher recycledGameObject = queue.Dequeue();
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.RecycleGameObject(recycledGameObject, target, scale, flipZAxis, parentEffect));
		}

		// Token: 0x060062E9 RID: 25321 RVA: 0x001FE6E7 File Offset: 0x001FC8E7
		private static void NewGameObjectOnDisableDispatcher_OnDisabled(GameObjectOnDisableDispatcher goodd)
		{
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.ReclaimDisabled(goodd.transform));
		}

		// Token: 0x060062EA RID: 25322 RVA: 0x001FE704 File Offset: 0x001FC904
		private IEnumerator RecycleGameObject(GameObjectOnDisableDispatcher recycledGameObject, Transform target, float scale, bool flipZAxis, bool parentEffect)
		{
			if (recycledGameObject.gameObject.activeInHierarchy)
			{
				recycledGameObject.gameObject.SetActive(false);
				recycledGameObject.OnDisabled -= TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				yield return null;
			}
			recycledGameObject.transform.position = target.transform.position;
			recycledGameObject.transform.rotation = target.transform.rotation;
			recycledGameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
			recycledGameObject.transform.parent = (parentEffect ? target : TagEffectsLibrary._instance.transform);
			Queue<GameObjectOnDisableDispatcher> queue;
			if (TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(recycledGameObject.gameObject.name, ref queue))
			{
				recycledGameObject.gameObject.SetActive(true);
				queue.Enqueue(recycledGameObject);
			}
			yield break;
		}

		// Token: 0x060062EB RID: 25323 RVA: 0x001FE731 File Offset: 0x001FC931
		private IEnumerator ReclaimDisabled(Transform transform)
		{
			yield return null;
			transform.parent = TagEffectsLibrary._instance.transform;
			yield break;
		}

		// Token: 0x0400719B RID: 29083
		private const int OBJECT_QUEUE_LIMIT = 12;

		// Token: 0x0400719C RID: 29084
		[OnEnterPlay_SetNull]
		private static TagEffectsLibrary _instance;

		// Token: 0x0400719D RID: 29085
		[SerializeField]
		private float fistBumpSpeedThreshold = 1f;

		// Token: 0x0400719E RID: 29086
		[SerializeField]
		private float highFiveSpeedThreshold = 1f;

		// Token: 0x0400719F RID: 29087
		[SerializeField]
		private ModeTagEffect[] defaultTagEffects;

		// Token: 0x040071A0 RID: 29088
		[SerializeField]
		private TagEffectsComboResult[] tagEffectsCombos;

		// Token: 0x040071A1 RID: 29089
		[SerializeField]
		private bool debugMode;

		// Token: 0x040071A2 RID: 29090
		private Dictionary<string, Queue<GameObjectOnDisableDispatcher>> tagEffectsPool;

		// Token: 0x040071A3 RID: 29091
		private Dictionary<TagEffectsCombo, TagEffectPack[]> tagEffectsComboLookUp;

		// Token: 0x02000F73 RID: 3955
		public enum EffectType
		{
			// Token: 0x040071A5 RID: 29093
			FIRST_PERSON,
			// Token: 0x040071A6 RID: 29094
			THIRD_PERSON,
			// Token: 0x040071A7 RID: 29095
			HIGH_FIVE,
			// Token: 0x040071A8 RID: 29096
			FIST_BUMP
		}
	}
}
