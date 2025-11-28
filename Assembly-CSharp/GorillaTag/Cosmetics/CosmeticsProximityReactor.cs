using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010DA RID: 4314
	public class CosmeticsProximityReactor : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06006C26 RID: 27686 RVA: 0x002379A0 File Offset: 0x00235BA0
		// (set) Token: 0x06006C27 RID: 27687 RVA: 0x002379A8 File Offset: 0x00235BA8
		public bool IsMatched { get; set; }

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06006C28 RID: 27688 RVA: 0x002379B1 File Offset: 0x00235BB1
		// (set) Token: 0x06006C29 RID: 27689 RVA: 0x002379B9 File Offset: 0x00235BB9
		private VRRig MyRig { get; set; }

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06006C2A RID: 27690 RVA: 0x002379C2 File Offset: 0x00235BC2
		// (set) Token: 0x06006C2B RID: 27691 RVA: 0x002379CA File Offset: 0x00235BCA
		public bool IsSpawned { get; set; }

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06006C2C RID: 27692 RVA: 0x002379D3 File Offset: 0x00235BD3
		// (set) Token: 0x06006C2D RID: 27693 RVA: 0x002379DB File Offset: 0x00235BDB
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06006C2E RID: 27694 RVA: 0x002379E4 File Offset: 0x00235BE4
		public bool IsBelow
		{
			get
			{
				using (List<CosmeticsProximityReactor.InteractionSetting>.Enumerator enumerator = this.blocks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.wasBelow)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x06006C2F RID: 27695 RVA: 0x00237A40 File Offset: 0x00235C40
		public void OnSpawn(VRRig rig)
		{
			if (this.MyRig == null)
			{
				this.MyRig = rig;
			}
		}

		// Token: 0x06006C30 RID: 27696 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x06006C31 RID: 27697 RVA: 0x00237A57 File Offset: 0x00235C57
		private void Start()
		{
			this.IsMatched = false;
			if (CosmeticsProximityReactorManager.Instance != null)
			{
				CosmeticsProximityReactorManager.Instance.Register(this);
			}
		}

		// Token: 0x06006C32 RID: 27698 RVA: 0x00237A78 File Offset: 0x00235C78
		private void OnEnable()
		{
			if (this.MyRig == null)
			{
				this.MyRig = base.GetComponentInParent<VRRig>();
			}
			if (CosmeticsProximityReactorManager.Instance != null)
			{
				CosmeticsProximityReactorManager.Instance.Register(this);
			}
		}

		// Token: 0x06006C33 RID: 27699 RVA: 0x00237AAC File Offset: 0x00235CAC
		private void OnDisable()
		{
			if (CosmeticsProximityReactorManager.Instance)
			{
				CosmeticsProximityReactorManager.Instance.Unregister(this);
			}
		}

		// Token: 0x06006C34 RID: 27700 RVA: 0x00237AC8 File Offset: 0x00235CC8
		public IReadOnlyList<string> GetTypes()
		{
			List<string> sharedKeysCache = CosmeticsProximityReactorManager.SharedKeysCache;
			sharedKeysCache.Clear();
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.interactionKeys != null && interactionSetting.interactionKeys.Count != 0)
				{
					foreach (string text in interactionSetting.interactionKeys)
					{
						if (!string.IsNullOrEmpty(text) && !sharedKeysCache.Contains(text))
						{
							sharedKeysCache.Add(text);
						}
					}
				}
			}
			return sharedKeysCache;
		}

		// Token: 0x06006C35 RID: 27701 RVA: 0x00237B98 File Offset: 0x00235D98
		public bool IsGorillaBody()
		{
			return this.itemKind == CosmeticsProximityReactor.ItemKind.GorillaBody;
		}

		// Token: 0x06006C36 RID: 27702 RVA: 0x00237BA3 File Offset: 0x00235DA3
		public bool IsCosmeticItem()
		{
			return this.itemKind == CosmeticsProximityReactor.ItemKind.Cosmetic;
		}

		// Token: 0x06006C37 RID: 27703 RVA: 0x00237BB0 File Offset: 0x00235DB0
		public bool AcceptsAnySource()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.gorillaBodyMask != CosmeticsProximityReactor.GorillaBodyPart.None)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006C38 RID: 27704 RVA: 0x00237C14 File Offset: 0x00235E14
		public bool AcceptsThisSource(CosmeticsProximityReactor.GorillaBodyPart kind)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006C39 RID: 27705 RVA: 0x00237C7C File Offset: 0x00235E7C
		public float GetCosmeticPairThresholdWith(CosmeticsProximityReactor other, out bool any)
		{
			any = false;
			float num = float.MaxValue;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.SharesKeyWith(interactionSetting2))
						{
							any = true;
							float num2 = Mathf.Min(interactionSetting.proximityThreshold, interactionSetting2.proximityThreshold);
							if (num2 < num)
							{
								num = num2;
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06006C3A RID: 27706 RVA: 0x00237D7C File Offset: 0x00235F7C
		public float GetSourceThresholdFor(CosmeticsProximityReactor gorillaBody, out bool any)
		{
			any = false;
			float num = float.MaxValue;
			CosmeticsProximityReactor.GorillaBodyPart kind = gorillaBody.gorillaBodyParts;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.AllowsRig(this.MyRig, gorillaBody.MyRig))
				{
					any = true;
					if (interactionSetting.proximityThreshold < num)
					{
						num = interactionSetting.proximityThreshold;
					}
				}
			}
			return num;
		}

		// Token: 0x06006C3B RID: 27707 RVA: 0x00237E14 File Offset: 0x00236014
		public void OnCosmeticBelowWith(CosmeticsProximityReactor other, Vector3 contact)
		{
			float time = Time.time;
			bool flag = false;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					bool flag2 = false;
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.SharesKeyWith(interactionSetting2))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						interactionSetting.FireBelow(this.MyRig, contact, time);
						if (interactionSetting.wasBelow)
						{
							interactionSetting.isMatched = true;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				this.IsMatched = true;
			}
		}

		// Token: 0x06006C3C RID: 27708 RVA: 0x00237F2C File Offset: 0x0023612C
		public void WhileCosmeticBelowWith(CosmeticsProximityReactor other, Vector3 contact)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					bool flag = false;
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.SharesKeyWith(interactionSetting2))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						interactionSetting.FireWhile(this.MyRig, contact);
					}
				}
			}
		}

		// Token: 0x06006C3D RID: 27709 RVA: 0x00238024 File Offset: 0x00236224
		public void OnCosmeticAboveAll()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched)
				{
					interactionSetting.FireAbove(this.MyRig);
				}
			}
			this.RefreshAggregateMatched();
		}

		// Token: 0x06006C3E RID: 27710 RVA: 0x00238094 File Offset: 0x00236294
		public void OnSourceBelow(Vector3 contact, CosmeticsProximityReactor.GorillaBodyPart kind, VRRig sourceRig)
		{
			float time = Time.time;
			bool flag = false;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.AllowsRig(this.MyRig, sourceRig))
				{
					interactionSetting.FireBelow(this.MyRig, contact, time);
					if (interactionSetting.wasBelow)
					{
						interactionSetting.isMatched = true;
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.RefreshAggregateMatched();
			}
		}

		// Token: 0x06006C3F RID: 27711 RVA: 0x00238134 File Offset: 0x00236334
		public void WhileSourceBelow(Vector3 contact, CosmeticsProximityReactor.GorillaBodyPart kind, VRRig sourceRig)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.isMatched && interactionSetting.AllowsRig(this.MyRig, sourceRig))
				{
					interactionSetting.FireWhile(this.MyRig, contact);
				}
			}
		}

		// Token: 0x06006C40 RID: 27712 RVA: 0x002381B8 File Offset: 0x002363B8
		public void OnSourceAboveAll()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.isMatched)
				{
					interactionSetting.FireAbove(this.MyRig);
				}
			}
			this.RefreshAggregateMatched();
		}

		// Token: 0x06006C41 RID: 27713 RVA: 0x00238228 File Offset: 0x00236428
		public bool HasAnyCosmeticMatch()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006C42 RID: 27714 RVA: 0x0023828C File Offset: 0x0023648C
		private bool HasAnyGorillaBodyPartMatch()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.isMatched)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006C43 RID: 27715 RVA: 0x002382F0 File Offset: 0x002364F0
		public void RefreshAggregateMatched()
		{
			this.IsMatched = (this.HasAnyCosmeticMatch() || this.HasAnyGorillaBodyPartMatch());
		}

		// Token: 0x04007CCC RID: 31948
		[Tooltip("Is this object a Cosmetic or a gorilla body part like hand? (gorilla body slot is reserved for Gorilla Player Networked)")]
		public CosmeticsProximityReactor.ItemKind itemKind;

		// Token: 0x04007CCD RID: 31949
		[FormerlySerializedAs("sourceKinds")]
		public CosmeticsProximityReactor.GorillaBodyPart gorillaBodyParts;

		// Token: 0x04007CCE RID: 31950
		public List<CosmeticsProximityReactor.InteractionSetting> blocks = new List<CosmeticsProximityReactor.InteractionSetting>();

		// Token: 0x04007CCF RID: 31951
		[Tooltip("If enabled, this cosmetic ignores other instances that share the same PlayFabID.")]
		public bool ignoreSameCosmeticInstances;

		// Token: 0x04007CD0 RID: 31952
		public string PlayFabID = "";

		// Token: 0x04007CD1 RID: 31953
		[Tooltip("If collider is not assigned, we will use the position of this object to find the distance between two cosmetic/body part")]
		public Collider collider;

		// Token: 0x04007CD4 RID: 31956
		private RubberDuckEvents _events;

		// Token: 0x020010DB RID: 4315
		public enum ItemKind
		{
			// Token: 0x04007CD8 RID: 31960
			Cosmetic,
			// Token: 0x04007CD9 RID: 31961
			GorillaBody
		}

		// Token: 0x020010DC RID: 4316
		[Flags]
		public enum GorillaBodyPart
		{
			// Token: 0x04007CDB RID: 31963
			None = 0,
			// Token: 0x04007CDC RID: 31964
			HandLeft = 1,
			// Token: 0x04007CDD RID: 31965
			HandRight = 2,
			// Token: 0x04007CDE RID: 31966
			Mouth = 4
		}

		// Token: 0x020010DD RID: 4317
		public enum InteractionMode
		{
			// Token: 0x04007CE0 RID: 31968
			CosmeticToCosmetic,
			// Token: 0x04007CE1 RID: 31969
			GorillaBodyToCosmetic
		}

		// Token: 0x020010DE RID: 4318
		public enum TargetType
		{
			// Token: 0x04007CE3 RID: 31971
			Owner,
			// Token: 0x04007CE4 RID: 31972
			Others,
			// Token: 0x04007CE5 RID: 31973
			All
		}

		// Token: 0x020010DF RID: 4319
		[Serializable]
		public class InteractionSetting
		{
			// Token: 0x06006C45 RID: 27717 RVA: 0x00238327 File Offset: 0x00236527
			public bool IsCosmeticToCosmetic()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic;
			}

			// Token: 0x06006C46 RID: 27718 RVA: 0x00238332 File Offset: 0x00236532
			public bool IsGorillaBodyToCosmetic()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic;
			}

			// Token: 0x06006C47 RID: 27719 RVA: 0x0023833D File Offset: 0x0023653D
			public bool AcceptsGorillaBodyPart(CosmeticsProximityReactor.GorillaBodyPart kind)
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && (this.gorillaBodyMask & kind) > CosmeticsProximityReactor.GorillaBodyPart.None;
			}

			// Token: 0x06006C48 RID: 27720 RVA: 0x00238358 File Offset: 0x00236558
			public bool SharesKeyWith(CosmeticsProximityReactor.InteractionSetting other)
			{
				if (this.mode != CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic)
				{
					return false;
				}
				if (other == null)
				{
					return false;
				}
				if (other.mode != CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic)
				{
					return false;
				}
				if (this.interactionKeys == null || other.interactionKeys == null)
				{
					return false;
				}
				foreach (string text in this.interactionKeys)
				{
					if (!string.IsNullOrEmpty(text) && other.interactionKeys.Contains(text))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006C49 RID: 27721 RVA: 0x002383EC File Offset: 0x002365EC
			public bool CanPlay(float now)
			{
				return now - this.lastEffectTime >= this.cooldownTime;
			}

			// Token: 0x06006C4A RID: 27722 RVA: 0x00238404 File Offset: 0x00236604
			public void FireBelow(VRRig rig, Vector3 contact, float now)
			{
				if (!this.wasBelow && this.CanPlay(now))
				{
					if (rig != null && rig.isLocal)
					{
						UnityEvent<Vector3> unityEvent = this.onBelowLocal;
						if (unityEvent != null)
						{
							unityEvent.Invoke(contact);
						}
					}
					UnityEvent<Vector3> unityEvent2 = this.onBelowShared;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(contact);
					}
					this.wasBelow = true;
					this.lastEffectTime = now;
				}
			}

			// Token: 0x06006C4B RID: 27723 RVA: 0x00238465 File Offset: 0x00236665
			public void FireWhile(VRRig rig, Vector3 contact)
			{
				if (rig != null && rig.isLocal)
				{
					UnityEvent<Vector3> unityEvent = this.whileBelowLocal;
					if (unityEvent != null)
					{
						unityEvent.Invoke(contact);
					}
				}
				UnityEvent<Vector3> unityEvent2 = this.whileBelowShared;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke(contact);
			}

			// Token: 0x06006C4C RID: 27724 RVA: 0x0023849C File Offset: 0x0023669C
			public void FireAbove(VRRig rig)
			{
				if (this.wasBelow)
				{
					if (rig != null && rig.isLocal)
					{
						UnityEvent unityEvent = this.onAboveLocal;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					UnityEvent unityEvent2 = this.onAboveShared;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasBelow = false;
					this.isMatched = false;
				}
			}

			// Token: 0x06006C4D RID: 27725 RVA: 0x002384F4 File Offset: 0x002366F4
			public bool AllowsRig(VRRig myRig, VRRig otherRig)
			{
				if (myRig == null || otherRig == null)
				{
					return true;
				}
				switch (this.targetType)
				{
				case CosmeticsProximityReactor.TargetType.Owner:
					return myRig == otherRig;
				case CosmeticsProximityReactor.TargetType.Others:
					return myRig != otherRig;
				}
				return true;
			}

			// Token: 0x04007CE6 RID: 31974
			[Tooltip("Determines what type of interaction this block handles.\n• CosmeticToCosmetic: triggers when two cosmetics with matching keys are nearby.\n• GorillaBodyToCosmetic: triggers when a Gorilla body part (hand, head, etc.) is near this cosmetic.")]
			public CosmeticsProximityReactor.InteractionMode mode;

			// Token: 0x04007CE7 RID: 31975
			[Tooltip("List of shared string identifiers that link this cosmetic to others.\nCosmetics with matching keys can trigger interactions with each other.")]
			public List<string> interactionKeys = new List<string>();

			// Token: 0x04007CE8 RID: 31976
			[Tooltip("Specifies which Gorilla body parts (e.g., Hands, Head) can trigger this interaction.\nUse this when the Mode is set to GorillaBodyToCosmetic.")]
			public CosmeticsProximityReactor.GorillaBodyPart gorillaBodyMask;

			// Token: 0x04007CE9 RID: 31977
			[Tooltip("The distance threshold (in meters) for triggering the interaction.\nIf another object enters this range, the OnBelow and WhileBelow events are fired.")]
			public float proximityThreshold = 0.15f;

			// Token: 0x04007CEA RID: 31978
			[Tooltip("Minimum time (in seconds) between consecutive triggers for this interaction block.\nPrevents rapid re-triggering when objects remain within proximity.")]
			[SerializeField]
			private float cooldownTime = 0.5f;

			// Token: 0x04007CEB RID: 31979
			[Tooltip("Who is allowed to trigger this block (if gorilla body part is selected).\n• Owner: only this cosmetic's own rig/body can trigger this.\n• Others: only other players' rigs/bodies can trigger this.\n• All: anyone can trigger.\n\nNote: everyone will still be able to see the result when it triggers.")]
			public CosmeticsProximityReactor.TargetType targetType = CosmeticsProximityReactor.TargetType.All;

			// Token: 0x04007CEC RID: 31980
			public UnityEvent<Vector3> onBelowLocal;

			// Token: 0x04007CED RID: 31981
			public UnityEvent<Vector3> onBelowShared;

			// Token: 0x04007CEE RID: 31982
			public UnityEvent<Vector3> whileBelowLocal;

			// Token: 0x04007CEF RID: 31983
			public UnityEvent<Vector3> whileBelowShared;

			// Token: 0x04007CF0 RID: 31984
			public UnityEvent onAboveLocal;

			// Token: 0x04007CF1 RID: 31985
			public UnityEvent onAboveShared;

			// Token: 0x04007CF2 RID: 31986
			[NonSerialized]
			public bool wasBelow;

			// Token: 0x04007CF3 RID: 31987
			[NonSerialized]
			public bool isMatched;

			// Token: 0x04007CF4 RID: 31988
			[NonSerialized]
			public float lastEffectTime = -9999f;
		}
	}
}
