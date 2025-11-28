using System;
using System.Collections.Generic;
using System.Text;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02001029 RID: 4137
	public class GorillaMaterialReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x0600689C RID: 26780 RVA: 0x00221038 File Offset: 0x0021F238
		public void PopulateRuntimeLookupArrays()
		{
			this._momentEnumCount = ((GorillaMaterialReaction.EMomentInState[])Enum.GetValues(typeof(GorillaMaterialReaction.EMomentInState))).Length;
			this._matCount = this._ownerVRRig.materialsToChangeTo.Length;
			this._mat_x_moment_x_activeBool_to_gObjs = new GameObject[this._momentEnumCount * this._matCount * 2][];
			for (int i = 0; i < this._matCount; i++)
			{
				for (int j = 0; j < this._momentEnumCount; j++)
				{
					GorillaMaterialReaction.EMomentInState emomentInState = (GorillaMaterialReaction.EMomentInState)j;
					List<GameObject> list = new List<GameObject>();
					List<GameObject> list2 = new List<GameObject>();
					foreach (GorillaMaterialReaction.ReactionEntry reactionEntry in this._statusEffectReactions)
					{
						int[] statusMaterialIndexes = reactionEntry.statusMaterialIndexes;
						for (int l = 0; l < statusMaterialIndexes.Length; l++)
						{
							if (statusMaterialIndexes[l] == i)
							{
								foreach (GorillaMaterialReaction.GameObjectStates gameObjectStates2 in reactionEntry.gameObjectStates)
								{
									switch (emomentInState)
									{
									case GorillaMaterialReaction.EMomentInState.OnEnter:
										if (gameObjectStates2.onEnter.change)
										{
											if (gameObjectStates2.onEnter.activeState)
											{
												list.Add(base.gameObject);
											}
											else
											{
												list2.Add(base.gameObject);
											}
										}
										break;
									case GorillaMaterialReaction.EMomentInState.OnStay:
										if (gameObjectStates2.onStay.change)
										{
											if (gameObjectStates2.onEnter.activeState)
											{
												list.Add(base.gameObject);
											}
											else
											{
												list2.Add(base.gameObject);
											}
										}
										break;
									case GorillaMaterialReaction.EMomentInState.OnExit:
										if (gameObjectStates2.onExit.change)
										{
											if (gameObjectStates2.onEnter.activeState)
											{
												list.Add(base.gameObject);
											}
											else
											{
												list2.Add(base.gameObject);
											}
										}
										break;
									default:
										Debug.LogError(string.Format("Unhandled enum value for {0}: {1}", "EMomentInState", emomentInState));
										break;
									}
								}
							}
						}
					}
					int num = i * this._momentEnumCount * 2 + j * 2;
					this._mat_x_moment_x_activeBool_to_gObjs[num] = list2.ToArray();
					this._mat_x_moment_x_activeBool_to_gObjs[num + 1] = list.ToArray();
				}
			}
		}

		// Token: 0x0600689D RID: 26781 RVA: 0x00221267 File Offset: 0x0021F467
		protected void Awake()
		{
			this.RemoveAndReportNulls();
			this.PopulateRuntimeLookupArrays();
		}

		// Token: 0x0600689E RID: 26782 RVA: 0x00221278 File Offset: 0x0021F478
		protected void OnEnable()
		{
			if (this._ownerVRRig == null)
			{
				this._ownerVRRig = base.GetComponentInParent<VRRig>(true);
			}
			if (this._ownerVRRig == null)
			{
				Debug.LogError("GorillaMaterialReaction: Disabling because could not find VRRig! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			this._reactionsRemaining = 0;
			for (int i = 0; i < this._statusEffectReactions.Length; i++)
			{
				this._reactionsRemaining += this._statusEffectReactions[i].gameObjectStates.Length;
			}
			this._currentMatIndexStartTime = 0.0;
			TickSystem<object>.AddCallbackTarget(this);
		}

		// Token: 0x0600689F RID: 26783 RVA: 0x00036897 File Offset: 0x00034A97
		protected void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x060068A0 RID: 26784 RVA: 0x00221320 File Offset: 0x0021F520
		// (set) Token: 0x060068A1 RID: 26785 RVA: 0x00221328 File Offset: 0x0021F528
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x060068A2 RID: 26786 RVA: 0x00221334 File Offset: 0x0021F534
		void ITickSystemPost.PostTick()
		{
			if (!GorillaComputer.hasInstance || this._ownerVRRig == null)
			{
				return;
			}
			GorillaComputer instance = GorillaComputer.instance;
			int num = (GorillaGameManager.instance == null) ? 0 : GorillaGameManager.instance.MyMatIndex(this._ownerVRRig.creator);
			if (this._previousMatIndex == num && this._reactionsRemaining <= 0)
			{
				return;
			}
			double num2 = (double)instance.startupMillis / 1000.0 + Time.realtimeSinceStartupAsDouble;
			bool flag = false;
			if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnExit && this._previousMatIndex != num)
			{
				this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnEnter;
				flag = true;
				this._currentMatIndexStartTime = num2;
				this._currentMomentDuration = -1.0;
				GorillaGameManager instance2 = GorillaGameManager.instance;
				if (instance2 != null)
				{
					GorillaTagManager gorillaTagManager = instance2 as GorillaTagManager;
					if (gorillaTagManager != null)
					{
						this._currentMomentDuration = (double)gorillaTagManager.tagCoolDown;
					}
				}
			}
			else if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnEnter && this._previousMatIndex == num && (this._currentMomentDuration < 0.0 || this._currentMomentDuration < num2 - this._currentMatIndexStartTime))
			{
				this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnStay;
				flag = true;
				this._currentMomentDuration = -1.0;
			}
			else if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnStay && this._previousMatIndex != num)
			{
				this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnExit;
				flag = true;
				this._currentMomentDuration = -1.0;
			}
			this._previousMatIndex = num;
			if (!flag)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				GameObject[] array = this._mat_x_moment_x_activeBool_to_gObjs[(int)(num * this._momentEnumCount * 2 + this._currentMomentInState * GorillaMaterialReaction.EMomentInState.OnExit + i)];
				for (int j = 0; j < array.Length; j++)
				{
					array[j].SetActive(i == 1);
				}
			}
		}

		// Token: 0x060068A3 RID: 26787 RVA: 0x002214EC File Offset: 0x0021F6EC
		private void RemoveAndReportNulls()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (this._statusEffectReactions == null)
			{
				Debug.Log(string.Format("{0}: The array `{1}` is null. ", "GorillaMaterialReaction", this._statusEffectReactions) + "(this should never happen)");
				this._statusEffectReactions = Array.Empty<GorillaMaterialReaction.ReactionEntry>();
			}
			for (int i = 0; i < this._statusEffectReactions.Length; i++)
			{
				GorillaMaterialReaction.GameObjectStates[] gameObjectStates = this._statusEffectReactions[i].gameObjectStates;
				if (gameObjectStates == null)
				{
					this._statusEffectReactions[i].gameObjectStates = Array.Empty<GorillaMaterialReaction.GameObjectStates>();
				}
				else
				{
					int num = 0;
					int[] array = new int[gameObjectStates.Length];
					for (int j = 0; j < gameObjectStates.Length; j++)
					{
						if (gameObjectStates[j].gameObject == null)
						{
							array[num] = j;
							num++;
						}
						else
						{
							array[num] = -1;
						}
					}
					if (num == 0)
					{
						return;
					}
					stringBuilder.Clear();
					stringBuilder.Append("GorillaMaterialReaction");
					stringBuilder.Append(": Removed null references in array `");
					stringBuilder.Append("_statusEffectReactions");
					stringBuilder.Append("[").Append(i).Append("].").Append("gameObjectStates");
					stringBuilder.Append("' at indexes: ");
					stringBuilder.AppendJoin<int>(", ", array);
					stringBuilder.Append(".");
					Debug.LogError(stringBuilder.ToString(), this);
					GorillaMaterialReaction.GameObjectStates[] array2 = new GorillaMaterialReaction.GameObjectStates[gameObjectStates.Length - num];
					int num2 = 0;
					for (int k = 0; k < gameObjectStates.Length; k++)
					{
						if (!(gameObjectStates[k].gameObject == null))
						{
							array2[num2++] = gameObjectStates[k];
						}
					}
					this._statusEffectReactions[i].gameObjectStates = array2;
				}
			}
		}

		// Token: 0x0400773F RID: 30527
		[SerializeField]
		private GorillaMaterialReaction.ReactionEntry[] _statusEffectReactions;

		// Token: 0x04007740 RID: 30528
		private int _previousMatIndex;

		// Token: 0x04007741 RID: 30529
		private GorillaMaterialReaction.EMomentInState _currentMomentInState;

		// Token: 0x04007742 RID: 30530
		private double _currentMatIndexStartTime;

		// Token: 0x04007743 RID: 30531
		private double _currentMomentDuration;

		// Token: 0x04007744 RID: 30532
		private int _reactionsRemaining;

		// Token: 0x04007745 RID: 30533
		private int _momentEnumCount;

		// Token: 0x04007746 RID: 30534
		private int _matCount;

		// Token: 0x04007747 RID: 30535
		private GameObject[][] _mat_x_moment_x_activeBool_to_gObjs;

		// Token: 0x04007748 RID: 30536
		private VRRig _ownerVRRig;

		// Token: 0x0200102A RID: 4138
		[Serializable]
		public struct ReactionEntry
		{
			// Token: 0x0400774A RID: 30538
			[Tooltip("If any of these statuses are true then this reaction will be executed.")]
			public int[] statusMaterialIndexes;

			// Token: 0x0400774B RID: 30539
			public GorillaMaterialReaction.GameObjectStates[] gameObjectStates;
		}

		// Token: 0x0200102B RID: 4139
		[Serializable]
		public struct GameObjectStates
		{
			// Token: 0x0400774C RID: 30540
			public GameObject gameObject;

			// Token: 0x0400774D RID: 30541
			[GorillaMaterialReaction.MomentInStateAttribute]
			public GorillaMaterialReaction.MomentInStateActiveOption onEnter;

			// Token: 0x0400774E RID: 30542
			[GorillaMaterialReaction.MomentInStateAttribute]
			public GorillaMaterialReaction.MomentInStateActiveOption onStay;

			// Token: 0x0400774F RID: 30543
			[GorillaMaterialReaction.MomentInStateAttribute]
			public GorillaMaterialReaction.MomentInStateActiveOption onExit;
		}

		// Token: 0x0200102C RID: 4140
		[Serializable]
		public struct MomentInStateActiveOption
		{
			// Token: 0x04007750 RID: 30544
			public bool change;

			// Token: 0x04007751 RID: 30545
			public bool activeState;
		}

		// Token: 0x0200102D RID: 4141
		public enum EMomentInState
		{
			// Token: 0x04007753 RID: 30547
			OnEnter,
			// Token: 0x04007754 RID: 30548
			OnStay,
			// Token: 0x04007755 RID: 30549
			OnExit
		}

		// Token: 0x0200102E RID: 4142
		public class MomentInStateAttribute : Attribute
		{
		}
	}
}
