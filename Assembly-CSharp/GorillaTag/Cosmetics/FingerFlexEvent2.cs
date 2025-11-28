using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010F6 RID: 4342
	public class FingerFlexEvent2 : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006CCE RID: 27854 RVA: 0x0023BAF4 File Offset: 0x00239CF4
		private bool TryLinkToNextEvent(int index)
		{
			if (index < this.list.Length - 1)
			{
				if (this.list[index].IsFlexTrigger && this.list[index + 1].IsReleaseTrigger)
				{
					this.list[index].linkIndex = index + 1;
					this.list[index + 1].linkIndex = index;
					return true;
				}
				this.list[index + 1].linkIndex = -1;
			}
			this.list[index].linkIndex = -1;
			return false;
		}

		// Token: 0x06006CCF RID: 27855 RVA: 0x0023BB70 File Offset: 0x00239D70
		private void Awake()
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			this.myTransferrable = base.GetComponentInParent<TransferrableObject>();
			for (int i = 0; i < this.list.Length; i++)
			{
				FingerFlexEvent2.FlexEvent flexEvent = this.list[i];
				if (this.myTransferrable.IsNull() && flexEvent.UsesTransferrable)
				{
					this.myTransferrable = base.GetComponentInParent<TransferrableObject>();
				}
				if (flexEvent.tryLink && this.TryLinkToNextEvent(i))
				{
					FingerFlexEvent2.FlexEvent flexEvent2 = this.list[i + 1];
					flexEvent.releaseThreshold = flexEvent2.releaseThreshold;
					flexEvent2.flexThreshold = flexEvent.flexThreshold;
					flexEvent2.fingerType = flexEvent.fingerType;
					flexEvent2.handType = flexEvent.handType;
					flexEvent2.networked = flexEvent.networked;
					i++;
				}
			}
		}

		// Token: 0x06006CD0 RID: 27856 RVA: 0x0023BC34 File Offset: 0x00239E34
		private void CalcFlex(bool disable)
		{
			for (int i = 0; i < this.list.Length; i++)
			{
				FingerFlexEvent2.FlexEvent flexEvent = this.list[i];
				if ((flexEvent.networked || this.myRig.isOfflineVRRig) && (!flexEvent.UsesTransferrable || !this.myTransferrable.IsNull()))
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					switch (flexEvent.handType)
					{
					case FingerFlexEvent2.FlexEvent.HandType.TransferrableHeldHand:
						flag = (this.myTransferrable.currentState == TransferrableObject.PositionState.InLeftHand);
						flag2 = (this.myTransferrable.currentState == TransferrableObject.PositionState.InRightHand);
						flag3 = (flag || flag2);
						break;
					case FingerFlexEvent2.FlexEvent.HandType.TransferrableEquippedSide:
						flag = ((this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.LeftBack)) > BodyDockPositions.DropPositions.None);
						flag2 = ((this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.RightBack)) > BodyDockPositions.DropPositions.None);
						break;
					case FingerFlexEvent2.FlexEvent.HandType.LeftHand:
						flag = true;
						break;
					case FingerFlexEvent2.FlexEvent.HandType.RightHand:
						flag2 = true;
						break;
					}
					if ((!flag || !flag2) && (flag || flag2 || flexEvent.wasHeld))
					{
						float num;
						if (disable || (flexEvent.wasHeld && !flag3))
						{
							num = 0f;
						}
						else
						{
							FingerFlexEvent2.FlexEvent.FingerType fingerType = flexEvent.fingerType;
							float num2;
							switch (fingerType)
							{
							case FingerFlexEvent2.FlexEvent.FingerType.Thumb:
								num2 = (flag ? this.myRig.leftThumb.calcT : this.myRig.rightThumb.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.Index:
								num2 = (flag ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.Middle:
								num2 = (flag ? this.myRig.leftMiddle.calcT : this.myRig.rightMiddle.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.IndexAndMiddle:
								num2 = (flag ? Mathf.Min(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Min(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT));
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.IndexOrMiddle:
								num2 = (flag ? Mathf.Max(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Max(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT));
								break;
							default:
								<PrivateImplementationDetails>.ThrowSwitchExpressionException(fingerType);
								break;
							}
							num = num2;
						}
						float flexValue = num;
						flexEvent.ProcessState(flag, flexValue);
						flexEvent.wasHeld = (flag3 && !disable);
						if (flexEvent.IsLinked)
						{
							FingerFlexEvent2.FlexEvent flexEvent2 = this.list[i + 1];
							flexEvent2.ProcessState(flag, flexValue);
							flexEvent2.wasHeld = flag3;
							i++;
						}
					}
				}
			}
		}

		// Token: 0x06006CD1 RID: 27857 RVA: 0x0001877F File Offset: 0x0001697F
		public void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006CD2 RID: 27858 RVA: 0x0023BEE6 File Offset: 0x0023A0E6
		public void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
			this.CalcFlex(true);
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06006CD3 RID: 27859 RVA: 0x0023BEF5 File Offset: 0x0023A0F5
		// (set) Token: 0x06006CD4 RID: 27860 RVA: 0x0023BEFD File Offset: 0x0023A0FD
		public bool TickRunning { get; set; }

		// Token: 0x06006CD5 RID: 27861 RVA: 0x0023BF06 File Offset: 0x0023A106
		public void Tick()
		{
			this.CalcFlex(false);
		}

		// Token: 0x04007DD3 RID: 32211
		public FingerFlexEvent2.FlexEvent[] list;

		// Token: 0x04007DD4 RID: 32212
		private VRRig myRig;

		// Token: 0x04007DD5 RID: 32213
		private TransferrableObject myTransferrable;

		// Token: 0x020010F7 RID: 4343
		[Serializable]
		public class FlexEvent
		{
			// Token: 0x17000A57 RID: 2647
			// (get) Token: 0x06006CD7 RID: 27863 RVA: 0x0023BF0F File Offset: 0x0023A10F
			public bool IsFlexTrigger
			{
				get
				{
					return this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnFlex;
				}
			}

			// Token: 0x17000A58 RID: 2648
			// (get) Token: 0x06006CD8 RID: 27864 RVA: 0x0023BF1A File Offset: 0x0023A11A
			public bool IsReleaseTrigger
			{
				get
				{
					return this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnRelease;
				}
			}

			// Token: 0x17000A59 RID: 2649
			// (get) Token: 0x06006CD9 RID: 27865 RVA: 0x0023BF28 File Offset: 0x0023A128
			public bool UsesTransferrable
			{
				get
				{
					FingerFlexEvent2.FlexEvent.HandType handType = this.handType;
					return handType == FingerFlexEvent2.FlexEvent.HandType.TransferrableHeldHand || handType == FingerFlexEvent2.FlexEvent.HandType.TransferrableEquippedSide;
				}
			}

			// Token: 0x17000A5A RID: 2650
			// (get) Token: 0x06006CDA RID: 27866 RVA: 0x0023BF4C File Offset: 0x0023A14C
			public bool HasValidLink
			{
				get
				{
					return this.linkIndex >= 0;
				}
			}

			// Token: 0x17000A5B RID: 2651
			// (get) Token: 0x06006CDB RID: 27867 RVA: 0x0023BF5A File Offset: 0x0023A15A
			public bool IsLinked
			{
				get
				{
					return this.tryLink && this.linkIndex >= 0;
				}
			}

			// Token: 0x17000A5C RID: 2652
			// (get) Token: 0x06006CDC RID: 27868 RVA: 0x0023BF72 File Offset: 0x0023A172
			private bool ShowMainProperties
			{
				get
				{
					return !this.IsLinked || this.IsFlexTrigger;
				}
			}

			// Token: 0x17000A5D RID: 2653
			// (get) Token: 0x06006CDD RID: 27869 RVA: 0x0023BF84 File Offset: 0x0023A184
			private bool ShowFlexThreshold
			{
				get
				{
					return this.ShowMainProperties;
				}
			}

			// Token: 0x17000A5E RID: 2654
			// (get) Token: 0x06006CDE RID: 27870 RVA: 0x0023BF8C File Offset: 0x0023A18C
			private bool ShowReleaseThreshold
			{
				get
				{
					return (!this.IsLinked || this.IsReleaseTrigger) && !this.IsFlexTrigger;
				}
			}

			// Token: 0x06006CDF RID: 27871 RVA: 0x0023BFAC File Offset: 0x0023A1AC
			public void ProcessState(bool leftHand, float flexValue)
			{
				this.currentState = ((flexValue < this.releaseThreshold) ? FingerFlexEvent2.FlexEvent.RangeState.Below : ((flexValue >= this.flexThreshold) ? FingerFlexEvent2.FlexEvent.RangeState.Above : FingerFlexEvent2.FlexEvent.RangeState.Within));
				if (this.ShowMainProperties && this.currentState != this.lastState && this.continuousProperties != null && this.continuousProperties.Count > 0)
				{
					float f = Mathf.InverseLerp(this.releaseThreshold, this.flexThreshold, flexValue);
					this.continuousProperties.ApplyAll(f);
				}
				if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Above && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Below)
				{
					this.lastThresholdTime = Time.time;
					this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Above;
					if (this.IsFlexTrigger)
					{
						UnityEvent<bool, float> unityEvent = this.unityEvent;
						if (unityEvent == null)
						{
							return;
						}
						unityEvent.Invoke(leftHand, flexValue);
						return;
					}
				}
				else if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Below && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Above)
				{
					this.lastThresholdTime = Time.time;
					this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Below;
					if (this.IsReleaseTrigger)
					{
						UnityEvent<bool, float> unityEvent2 = this.unityEvent;
						if (unityEvent2 == null)
						{
							return;
						}
						unityEvent2.Invoke(leftHand, flexValue);
					}
				}
			}

			// Token: 0x04007DD7 RID: 32215
			public FingerFlexEvent2.FlexEvent.TriggerType triggerType;

			// Token: 0x04007DD8 RID: 32216
			public bool tryLink = true;

			// Token: 0x04007DD9 RID: 32217
			[HideInInspector]
			public int linkIndex = -1;

			// Token: 0x04007DDA RID: 32218
			[Space]
			public FingerFlexEvent2.FlexEvent.FingerType fingerType = FingerFlexEvent2.FlexEvent.FingerType.Index;

			// Token: 0x04007DDB RID: 32219
			[Space]
			public FingerFlexEvent2.FlexEvent.HandType handType;

			// Token: 0x04007DDC RID: 32220
			private const string ADVANCED = "Advanced Properties";

			// Token: 0x04007DDD RID: 32221
			[Tooltip("When this is checked, all players in the room will fire the event. Otherwise, only the local player will fire it. You should usually leave this on, unless you're using it for something local like controller haptics.")]
			public bool networked = true;

			// Token: 0x04007DDE RID: 32222
			[Range(0.01f, 0.75f)]
			public float flexThreshold = 0.75f;

			// Token: 0x04007DDF RID: 32223
			[Range(0.01f, 1f)]
			public float releaseThreshold = 0.01f;

			// Token: 0x04007DE0 RID: 32224
			public ContinuousPropertyArray continuousProperties;

			// Token: 0x04007DE1 RID: 32225
			public UnityEvent<bool, float> unityEvent;

			// Token: 0x04007DE2 RID: 32226
			[NonSerialized]
			public bool wasHeld;

			// Token: 0x04007DE3 RID: 32227
			[NonSerialized]
			public bool marginError;

			// Token: 0x04007DE4 RID: 32228
			private FingerFlexEvent2.FlexEvent.RangeState currentState;

			// Token: 0x04007DE5 RID: 32229
			private FingerFlexEvent2.FlexEvent.RangeState lastState;

			// Token: 0x04007DE6 RID: 32230
			private float lastThresholdTime = -100000f;

			// Token: 0x020010F8 RID: 4344
			public enum TriggerType
			{
				// Token: 0x04007DE8 RID: 32232
				OnFlex,
				// Token: 0x04007DE9 RID: 32233
				OnRelease = 2
			}

			// Token: 0x020010F9 RID: 4345
			public enum FingerType
			{
				// Token: 0x04007DEB RID: 32235
				Thumb,
				// Token: 0x04007DEC RID: 32236
				Index,
				// Token: 0x04007DED RID: 32237
				Middle,
				// Token: 0x04007DEE RID: 32238
				IndexAndMiddle,
				// Token: 0x04007DEF RID: 32239
				IndexOrMiddle
			}

			// Token: 0x020010FA RID: 4346
			public enum HandType
			{
				// Token: 0x04007DF1 RID: 32241
				TransferrableHeldHand,
				// Token: 0x04007DF2 RID: 32242
				TransferrableEquippedSide,
				// Token: 0x04007DF3 RID: 32243
				LeftHand,
				// Token: 0x04007DF4 RID: 32244
				RightHand
			}

			// Token: 0x020010FB RID: 4347
			private enum RangeState
			{
				// Token: 0x04007DF6 RID: 32246
				Below,
				// Token: 0x04007DF7 RID: 32247
				Within,
				// Token: 0x04007DF8 RID: 32248
				Above
			}
		}
	}
}
