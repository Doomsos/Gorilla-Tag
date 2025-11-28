using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010C6 RID: 4294
	[Serializable]
	public class ContinuousProperty
	{
		// Token: 0x06006BAB RID: 27563 RVA: 0x0023539C File Offset: 0x0023359C
		private static ContinuousProperty.Cast GetTargetCast(Object o)
		{
			ContinuousProperty.Cast result;
			if (!(o is ParticleSystem))
			{
				if (!(o is SkinnedMeshRenderer))
				{
					if (!(o is Animator))
					{
						if (!(o is AudioSource))
						{
							if (!(o is VoicePitchShiftCosmetic))
							{
								if (!(o is Rigidbody))
								{
									if (!(o is Transform))
									{
										if (!(o is Renderer))
										{
											if (!(o is Behaviour))
											{
												if (!(o is GameObject))
												{
													result = ContinuousProperty.Cast.Null;
												}
												else
												{
													result = ContinuousProperty.Cast.GameObject;
												}
											}
											else
											{
												result = ContinuousProperty.Cast.Behaviour;
											}
										}
										else
										{
											result = ContinuousProperty.Cast.Renderer;
										}
									}
									else
									{
										result = ContinuousProperty.Cast.Transform;
									}
								}
								else
								{
									result = ContinuousProperty.Cast.Rigidbody;
								}
							}
							else
							{
								result = ContinuousProperty.Cast.VoicePitchShiftCosmetic;
							}
						}
						else
						{
							result = ContinuousProperty.Cast.AudioSource;
						}
					}
					else
					{
						result = ContinuousProperty.Cast.Animator;
					}
				}
				else
				{
					result = ContinuousProperty.Cast.SkinnedMeshRenderer;
				}
			}
			else
			{
				result = ContinuousProperty.Cast.ParticleSystem;
			}
			return result;
		}

		// Token: 0x06006BAC RID: 27564 RVA: 0x00235450 File Offset: 0x00233650
		public static bool CastMatches(ContinuousProperty.Cast cast, ContinuousProperty.Cast test)
		{
			if (cast <= ContinuousProperty.Cast.Any)
			{
				if (cast == ContinuousProperty.Cast.Null)
				{
					return false;
				}
				if (cast == ContinuousProperty.Cast.Any)
				{
					return true;
				}
			}
			else
			{
				if (cast == ContinuousProperty.Cast.Renderer)
				{
					return test == ContinuousProperty.Cast.Renderer || test == ContinuousProperty.Cast.SkinnedMeshRenderer;
				}
				if (cast == ContinuousProperty.Cast.Behaviour)
				{
					return test != ContinuousProperty.Cast.Transform && test != ContinuousProperty.Cast.GameObject && test != ContinuousProperty.Cast.Rigidbody;
				}
			}
			return test == cast;
		}

		// Token: 0x06006BAD RID: 27565 RVA: 0x002354C9 File Offset: 0x002336C9
		public static bool HasAllFlags(ContinuousProperty.DataFlags flags, ContinuousProperty.DataFlags test)
		{
			return (flags & test) == test;
		}

		// Token: 0x06006BAE RID: 27566 RVA: 0x002354D1 File Offset: 0x002336D1
		public static bool HasAnyFlag(ContinuousProperty.DataFlags flags, ContinuousProperty.DataFlags test)
		{
			return (flags & test) > ContinuousProperty.DataFlags.None;
		}

		// Token: 0x06006BAF RID: 27567 RVA: 0x002354DC File Offset: 0x002336DC
		private static void GetAllValidObjectsNonAlloc(Transform t, List<Object> objects)
		{
			objects.Clear();
			objects.Add(t.gameObject);
			foreach (Component @object in t.GetComponents<Component>())
			{
				if (ContinuousProperty.IsValidObject(@object.GetType()))
				{
					objects.Add(@object);
				}
			}
		}

		// Token: 0x06006BB0 RID: 27568 RVA: 0x00235528 File Offset: 0x00233728
		private static bool IsValidObject(System.Type t)
		{
			return t != typeof(Renderer) && t != typeof(ParticleSystemRenderer);
		}

		// Token: 0x06006BB1 RID: 27569 RVA: 0x00235550 File Offset: 0x00233750
		public ContinuousProperty()
		{
		}

		// Token: 0x06006BB2 RID: 27570 RVA: 0x002355B4 File Offset: 0x002337B4
		public ContinuousProperty(ContinuousPropertyModeSO mode, Transform initialTarget, Vector2 range = default(Vector2))
		{
			this.mode = mode;
			this.target = initialTarget;
			this.range = range;
			this.ShiftTarget(0);
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06006BB3 RID: 27571 RVA: 0x00235634 File Offset: 0x00233834
		private string ModeTooltip
		{
			get
			{
				if (!this.mode)
				{
					return "";
				}
				return string.Format("{0}: {1}", this.mode.type, this.mode.GetDescriptionForCast(ContinuousProperty.GetTargetCast(this.target)));
			}
		}

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06006BB4 RID: 27572 RVA: 0x00235684 File Offset: 0x00233884
		private bool ModeInfoVisible
		{
			get
			{
				return this.mode == null;
			}
		}

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06006BB5 RID: 27573 RVA: 0x00235692 File Offset: 0x00233892
		private bool ModeErrorVisible
		{
			get
			{
				return !this.IsValid();
			}
		}

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06006BB6 RID: 27574 RVA: 0x0023569D File Offset: 0x0023389D
		private string ModeErrorMessage
		{
			get
			{
				if (!(this.mode != null))
				{
					return "How did we get here?";
				}
				return "I couldn't find any valid target to apply my '" + this.mode.name + "' to in the whole prefab.\n\n" + this.mode.ListValidCasts();
			}
		}

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06006BB7 RID: 27575 RVA: 0x002356D8 File Offset: 0x002338D8
		public ContinuousPropertyModeSO Mode
		{
			get
			{
				return this.mode;
			}
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06006BB8 RID: 27576 RVA: 0x002356E0 File Offset: 0x002338E0
		public ContinuousProperty.Type MyType
		{
			get
			{
				if (!(this.mode != null))
				{
					return ContinuousProperty.Type.Color;
				}
				return this.mode.type;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06006BB9 RID: 27577 RVA: 0x002356FD File Offset: 0x002338FD
		private bool HasTarget
		{
			get
			{
				return this.MyType != ContinuousProperty.Type.UnityEvent;
			}
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06006BBA RID: 27578 RVA: 0x0023570C File Offset: 0x0023390C
		private bool TargetInfoVisible
		{
			get
			{
				return this.HasTarget && this.target == null;
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06006BBB RID: 27579 RVA: 0x00235724 File Offset: 0x00233924
		private string TargetTooltip
		{
			get
			{
				if (!(this.mode != null))
				{
					return "";
				}
				return this.mode.ListValidCasts();
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06006BBC RID: 27580 RVA: 0x00235745 File Offset: 0x00233945
		private bool ShiftButtonsVisible
		{
			get
			{
				return this.mode != null;
			}
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06006BBD RID: 27581 RVA: 0x00235753 File Offset: 0x00233953
		public Object Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06006BBE RID: 27582 RVA: 0x0023575B File Offset: 0x0023395B
		private void PreviousTarget()
		{
			this.ShiftTarget(-1);
		}

		// Token: 0x06006BBF RID: 27583 RVA: 0x00235765 File Offset: 0x00233965
		private void NextTarget()
		{
			this.ShiftTarget(1);
		}

		// Token: 0x06006BC0 RID: 27584 RVA: 0x00235770 File Offset: 0x00233970
		public bool ShiftTarget(int shiftAmount)
		{
			if (this.mode == null)
			{
				return false;
			}
			int num = -1;
			Transform transform;
			if (!(this.target != null))
			{
				transform = null;
			}
			else
			{
				GameObject gameObject = this.target as GameObject;
				transform = (((gameObject != null) ? gameObject.transform : null) ?? ((Component)this.target).transform);
			}
			Transform transform2 = transform;
			Transform transform3 = transform2;
			if (transform3 == null)
			{
				return false;
			}
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(transform3);
			List<Object> list = new List<Object>();
			List<Object> list2 = new List<Object>();
			Transform transform4;
			while (stack.TryPop(ref transform4))
			{
				if (num < 0 && transform4 == transform2)
				{
					num = list.Count;
				}
				ContinuousProperty.GetAllValidObjectsNonAlloc(transform4, list2);
				foreach (Object @object in list2)
				{
					if (this.mode.IsCastValid(ContinuousProperty.GetTargetCast(@object)))
					{
						if (@object == this.target)
						{
							num = list.Count;
						}
						list.Add(@object);
					}
				}
				for (int i = transform4.childCount - 1; i >= 0; i--)
				{
					stack.Push(transform4.GetChild(i));
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			this.target = list[(num < 0) ? 0 : ((num + shiftAmount + list.Count) % list.Count)];
			return true;
		}

		// Token: 0x06006BC1 RID: 27585 RVA: 0x002358F0 File Offset: 0x00233AF0
		private void OnModeOrTargetChanged()
		{
			if (!this.IsValid())
			{
				this.ShiftTarget(0);
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06006BC2 RID: 27586 RVA: 0x00235902 File Offset: 0x00233B02
		// (set) Token: 0x06006BC3 RID: 27587 RVA: 0x0023590A File Offset: 0x00233B0A
		public bool IsShaderProperty_Cached { get; private set; }

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06006BC4 RID: 27588 RVA: 0x00235913 File Offset: 0x00233B13
		// (set) Token: 0x06006BC5 RID: 27589 RVA: 0x0023591B File Offset: 0x00233B1B
		public bool UsesThreshold_Cached { get; private set; }

		// Token: 0x06006BC6 RID: 27590 RVA: 0x00235924 File Offset: 0x00233B24
		public bool IsValid()
		{
			return this.mode == null || this.target == null || this.mode.IsCastValid(ContinuousProperty.GetTargetCast(this.target));
		}

		// Token: 0x06006BC7 RID: 27591 RVA: 0x0023595A File Offset: 0x00233B5A
		public int GetTargetInstanceID()
		{
			return this.target.GetInstanceID();
		}

		// Token: 0x06006BC8 RID: 27592 RVA: 0x00235967 File Offset: 0x00233B67
		private bool HasAllFlags(ContinuousProperty.DataFlags test)
		{
			return this.mode != null && ContinuousProperty.HasAllFlags(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
		}

		// Token: 0x06006BC9 RID: 27593 RVA: 0x00235995 File Offset: 0x00233B95
		private bool HasAnyFlag(ContinuousProperty.DataFlags test)
		{
			return this.mode != null && ContinuousProperty.HasAnyFlag(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06006BCA RID: 27594 RVA: 0x002359C3 File Offset: 0x00233BC3
		private bool HasGradient
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasColor);
			}
		}

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06006BCB RID: 27595 RVA: 0x002359CC File Offset: 0x00233BCC
		private bool HasCurve
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasCurve);
			}
		}

		// Token: 0x06006BCC RID: 27596 RVA: 0x002359D8 File Offset: 0x00233BD8
		private string DynamicIntLabel()
		{
			if (!this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
			{
				ContinuousProperty.Type myType = this.MyType;
				if (myType != ContinuousProperty.Type.Color && myType != ContinuousProperty.Type.BlendShape)
				{
					return "Int Value";
				}
			}
			return "Material Index";
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06006BCD RID: 27597 RVA: 0x00235A08 File Offset: 0x00233C08
		private bool HasInt
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasInteger);
			}
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06006BCE RID: 27598 RVA: 0x00235A11 File Offset: 0x00233C11
		public int IntValue
		{
			get
			{
				return this.intValue;
			}
		}

		// Token: 0x06006BCF RID: 27599 RVA: 0x00235A19 File Offset: 0x00233C19
		private string DynamicStringLabel()
		{
			if (this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
			{
				return "Property Name";
			}
			if (this.HasAllFlags(ContinuousProperty.DataFlags.IsAnimatorParameter))
			{
				return "Parameter Name";
			}
			return "String Value";
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06006BD0 RID: 27600 RVA: 0x00235A40 File Offset: 0x00233C40
		private bool HasString
		{
			get
			{
				return this.HasAnyFlag(ContinuousProperty.DataFlags.IsShaderProperty | ContinuousProperty.DataFlags.IsAnimatorParameter);
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06006BD1 RID: 27601 RVA: 0x00235A4A File Offset: 0x00233C4A
		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06006BD2 RID: 27602 RVA: 0x00235A52 File Offset: 0x00233C52
		private bool HasBezier
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.BezierInterpolation;
			}
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06006BD3 RID: 27603 RVA: 0x00235A5D File Offset: 0x00233C5D
		private bool MissingBezier
		{
			get
			{
				return this.bezierCurve == null;
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06006BD4 RID: 27604 RVA: 0x00235A6B File Offset: 0x00233C6B
		private bool AxisError
		{
			get
			{
				return !Enum.IsDefined(typeof(ContinuousProperty.RotationAxis), this.localAxis);
			}
		}

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06006BD5 RID: 27605 RVA: 0x00235A8A File Offset: 0x00233C8A
		private bool HasAxisMode
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasAxis);
			}
		}

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06006BD6 RID: 27606 RVA: 0x00235A93 File Offset: 0x00233C93
		private bool InterpolationError
		{
			get
			{
				return !Enum.IsDefined(typeof(ContinuousProperty.InterpolationMode), this.interpolationMode);
			}
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06006BD7 RID: 27607 RVA: 0x00235AB2 File Offset: 0x00233CB2
		private bool HasInterpolationMode
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasInterpolation);
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06006BD8 RID: 27608 RVA: 0x00235ABC File Offset: 0x00233CBC
		private bool HasStopAction
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.PlayStop && this.target is ParticleSystem;
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06006BD9 RID: 27609 RVA: 0x00235AD8 File Offset: 0x00233CD8
		private bool HasXforms
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.TransformInterpolation;
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06006BDA RID: 27610 RVA: 0x00235AE3 File Offset: 0x00233CE3
		private bool MissingXforms
		{
			get
			{
				return this.transformA == null || this.transformB == null;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06006BDB RID: 27611 RVA: 0x00235B01 File Offset: 0x00233D01
		private bool HasOffsets
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.OffsetInterpolation;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06006BDC RID: 27612 RVA: 0x00235B0D File Offset: 0x00233D0D
		private string ThresholdErrorMessage
		{
			get
			{
				return "The threshold will always be " + ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal ^ this.range.x >= this.range.y) ? "true." : "false.");
			}
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06006BDD RID: 27613 RVA: 0x00235B4C File Offset: 0x00233D4C
		private string ThresholdTooltip
		{
			get
			{
				if (!this.ThresholdError)
				{
					return "The threshold will be true" + ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal) ? ((this.range.x > 0f && this.range.y < 1f) ? string.Format(" between {0} and {1}", this.range.x, this.range.y) : ((this.range.x > 0f) ? (" above " + this.range.x.ToString()) : (" below " + this.range.y.ToString()))) : (((this.range.x > 0f) ? (" below " + this.range.x.ToString()) : "") + ((this.range.x > 0f && this.range.y < 1f) ? " and" : "") + ((this.range.y < 1f) ? (" above " + this.range.y.ToString()) : ""))) + ", and false otherwise.";
				}
				return this.ThresholdErrorMessage;
			}
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06006BDE RID: 27614 RVA: 0x00235CBE File Offset: 0x00233EBE
		private bool HasThreshold
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasThreshold);
			}
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06006BDF RID: 27615 RVA: 0x00235CCC File Offset: 0x00233ECC
		private bool ThresholdError
		{
			get
			{
				return (this.range.x <= 0f && this.range.y >= 1f) || this.range.x >= this.range.y;
			}
		}

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06006BE0 RID: 27616 RVA: 0x00235D1A File Offset: 0x00233F1A
		private bool HasEventMode
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.UnityEvent && !this.HasAnyFlag(ContinuousProperty.DataFlags.HasThreshold);
			}
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06006BE1 RID: 27617 RVA: 0x00235D36 File Offset: 0x00233F36
		private bool HasUnityEvent
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.UnityEvent;
			}
		}

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06006BE2 RID: 27618 RVA: 0x00235D42 File Offset: 0x00233F42
		public bool RunOnlyLocally
		{
			get
			{
				return this.runOnlyLocally;
			}
		}

		// Token: 0x06006BE3 RID: 27619 RVA: 0x00235D4A File Offset: 0x00233F4A
		public void SetRigIsLocal(bool v)
		{
			this.rigLocal = v;
		}

		// Token: 0x06006BE4 RID: 27620 RVA: 0x00235D54 File Offset: 0x00233F54
		public void Init()
		{
			if (this.mode == null)
			{
				this.internalSwitchValue = 0;
				return;
			}
			ContinuousProperty.Type type = this.mode.type;
			ContinuousProperty.Cast cast = this.mode.GetClosestCast(ContinuousProperty.GetTargetCast(this.target));
			ContinuousProperty.DataFlags dataFlags = this.mode.GetFlagsForCast(cast);
			if (cast == ContinuousProperty.Cast.Null || (type == ContinuousProperty.Type.BezierInterpolation && this.MissingBezier) || (type == ContinuousProperty.Type.TransformInterpolation && this.MissingXforms) || (type == ContinuousProperty.Type.UnityEvent && this.unityEvent == null))
			{
				this.internalSwitchValue = 0;
				this.IsShaderProperty_Cached = false;
				this.UsesThreshold_Cached = false;
				return;
			}
			if (type == ContinuousProperty.Type.Color && ContinuousProperty.CastMatches(ContinuousProperty.Cast.Renderer, cast))
			{
				type = ContinuousProperty.Type.ShaderColor;
				cast = ContinuousProperty.Cast.Renderer;
				dataFlags |= ContinuousProperty.DataFlags.IsShaderProperty;
				this.stringValue = "_BaseColor";
			}
			else if (type == ContinuousProperty.Type.PlayStop && cast == ContinuousProperty.Cast.Animator)
			{
				type = ContinuousProperty.Type.EnableDisable;
				cast = ContinuousProperty.Cast.Behaviour;
			}
			this.internalSwitchValue = (int)(type | (ContinuousProperty.Type)cast | (ContinuousProperty.Type)(this.HasAxisMode ? this.localAxis : ((ContinuousProperty.RotationAxis)0)) | (ContinuousProperty.Type)(this.HasInterpolationMode ? this.interpolationMode : ((ContinuousProperty.InterpolationMode)0)) | (ContinuousProperty.Type)(this.HasEventMode ? this.eventMode : ((ContinuousProperty.EventMode)0)));
			this.IsShaderProperty_Cached = ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.IsShaderProperty);
			this.UsesThreshold_Cached = ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.HasThreshold);
			if (cast == ContinuousProperty.Cast.ParticleSystem)
			{
				this.particleMain = ((ParticleSystem)this.target).main;
				this.particleEmission = ((ParticleSystem)this.target).emission;
				this.speedCurveCache = this.particleMain.startSpeed;
				this.rateCurveCache = this.particleEmission.rateOverTime;
			}
			if (this.IsShaderProperty_Cached)
			{
				this.stringHash = Shader.PropertyToID(this.stringValue);
			}
			else if (ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.IsAnimatorParameter))
			{
				this.stringHash = Animator.StringToHash(this.stringValue);
			}
			if (!ContinuousProperty.HasAnyFlag(dataFlags, ContinuousProperty.DataFlags.HasCurve))
			{
				this.curve = AnimationCurves.Linear;
			}
		}

		// Token: 0x06006BE5 RID: 27621 RVA: 0x00235F23 File Offset: 0x00234123
		public void InitThreshold()
		{
			if (!this.UsesThreshold_Cached)
			{
				return;
			}
			this.CheckThreshold(0f);
			if (this.IsShaderProperty_Cached)
			{
				return;
			}
			this.previousBoolValue = !this.previousBoolValue;
			this.Apply(0f, 0f, null);
		}

		// Token: 0x06006BE6 RID: 27622 RVA: 0x00235F64 File Offset: 0x00234164
		public void Apply(float f, float deltaTime, MaterialPropertyBlock mpb)
		{
			if (this.runOnlyLocally && !this.rigLocal)
			{
				return;
			}
			int num = this.internalSwitchValue | (int)this.CheckThreshold(f);
			if (num <= 1057808)
			{
				if (num <= 6157)
				{
					if (num <= 3083)
					{
						if (num <= 2049)
						{
							if (num == 0)
							{
								return;
							}
							if (num != 2049)
							{
								return;
							}
							((Transform)this.target).localScale = this.curve.Evaluate(f) * Vector3.one;
							return;
						}
						else
						{
							if (num == 3072)
							{
								this.particleMain.startColor = this.color.Evaluate(f);
								return;
							}
							if (num == 3073)
							{
								this.particleMain.startSize = this.curve.Evaluate(f);
								return;
							}
							if (num != 3083)
							{
								return;
							}
							this.particleMain.startSpeed = this.ScaleCurve(this.speedCurveCache, this.curve.Evaluate(f));
							return;
						}
					}
					else if (num <= 4098)
					{
						if (num == 3084)
						{
							this.particleEmission.rateOverTime = this.ScaleCurve(this.rateCurveCache, this.curve.Evaluate(f));
							return;
						}
						if (num != 4098)
						{
							return;
						}
						((SkinnedMeshRenderer)this.target).SetBlendShapeWeight(this.intValue, this.curve.Evaluate(f) * 100f);
						return;
					}
					else
					{
						if (num == 5123)
						{
							((Animator)this.target).SetFloat(this.stringHash, this.curve.Evaluate(f));
							return;
						}
						if (num == 5131)
						{
							((Animator)this.target).speed = this.curve.Evaluate(f);
							return;
						}
						if (num != 6157)
						{
							return;
						}
						((AudioSource)this.target).volume = Mathf.Clamp01(this.curve.Evaluate(f));
						return;
					}
				}
				else if (num <= 1051663)
				{
					if (num <= 7173)
					{
						if (num == 6158)
						{
							((AudioSource)this.target).pitch = Mathf.Clamp(this.curve.Evaluate(f), -3f, 3f);
							return;
						}
						switch (num)
						{
						case 7171:
							mpb.SetFloat(this.stringHash, this.curve.Evaluate(f));
							return;
						case 7172:
							mpb.SetVector(this.stringHash, new Vector2(this.curve.Evaluate(f), 0f));
							return;
						case 7173:
							mpb.SetColor(this.stringHash, this.color.Evaluate(f));
							return;
						default:
							return;
						}
					}
					else
					{
						if (num == 11278)
						{
							((VoicePitchShiftCosmetic)this.target).Pitch = this.curve.Evaluate(f);
							return;
						}
						if (num == 1049617)
						{
							this.unityEvent.Invoke(this.curve.Evaluate(f));
							return;
						}
						if (num != 1051663)
						{
							return;
						}
						((ParticleSystem)this.target).Play();
						return;
					}
				}
				else if (num <= 1054735)
				{
					if (num != 1053706)
					{
						if (num == 1053714)
						{
							((Animator)this.target).SetTrigger(this.stringHash);
							return;
						}
						if (num != 1054735)
						{
							return;
						}
						((AudioSource)this.target).Play();
						return;
					}
				}
				else
				{
					if (num == 1055760)
					{
						goto IL_7AB;
					}
					if (num == 1056784)
					{
						goto IL_7C2;
					}
					if (num != 1057808)
					{
						return;
					}
					goto IL_7D9;
				}
			}
			else if (num <= 3150858)
			{
				if (num <= 2103311)
				{
					if (num <= 2100239)
					{
						if (num == 2098193)
						{
							return;
						}
						if (num != 2100239)
						{
							return;
						}
						((ParticleSystem)this.target).Stop(true, this.stopType);
						return;
					}
					else if (num != 2102282)
					{
						if (num == 2102290)
						{
							return;
						}
						if (num != 2103311)
						{
							return;
						}
						((AudioSource)this.target).Stop();
						return;
					}
				}
				else if (num <= 2106384)
				{
					if (num == 2104336)
					{
						goto IL_7AB;
					}
					if (num == 2105360)
					{
						goto IL_7C2;
					}
					if (num != 2106384)
					{
						return;
					}
					goto IL_7D9;
				}
				else
				{
					if (num != 3146769 && num != 3148815)
					{
						return;
					}
					return;
				}
			}
			else if (num <= 3154960)
			{
				if (num <= 3151887)
				{
					if (num != 3150866)
					{
						return;
					}
					return;
				}
				else
				{
					if (num != 3152912 && num != 3153936)
					{
						return;
					}
					return;
				}
			}
			else if (num <= 8389649)
			{
				if (num == 4195345)
				{
					this.unityEvent.Invoke(this.curve.Evaluate(f));
					return;
				}
				switch (num)
				{
				case 4196358:
					((Transform)this.target).position = this.bezierCurve.GetPoint(this.curve.Evaluate(f));
					return;
				case 4196359:
					((Transform)this.target).localRotation = Quaternion.Euler(this.curve.Evaluate(f) * 360f, 0f, 0f);
					return;
				case 4196360:
					((Transform)this.target).position = Vector3.Lerp(this.transformA.position, this.transformB.position, this.curve.Evaluate(f));
					return;
				case 4196361:
					((Transform)this.target).localPosition = Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, this.curve.Evaluate(f));
					return;
				default:
				{
					if (num != 8389649)
					{
						return;
					}
					float num2 = this.curve.Evaluate(f);
					float num3 = 1f / num2;
					this.frequencyTimer += deltaTime;
					if (this.frequencyTimer >= num3)
					{
						this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - num3, num3);
						this.unityEvent.Invoke(num2);
						return;
					}
					return;
				}
				}
			}
			else
			{
				switch (num)
				{
				case 8390662:
					((Transform)this.target).rotation = Quaternion.LookRotation(this.bezierCurve.GetDirection(this.curve.Evaluate(f)));
					return;
				case 8390663:
					((Transform)this.target).localRotation = Quaternion.Euler(0f, this.curve.Evaluate(f) * 360f, 0f);
					return;
				case 8390664:
					((Transform)this.target).rotation = Quaternion.Slerp(this.transformA.rotation, this.transformB.rotation, this.curve.Evaluate(f));
					return;
				case 8390665:
					((Transform)this.target).localRotation = Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, this.curve.Evaluate(f));
					return;
				default:
					if (num != 12583953)
					{
						switch (num)
						{
						case 12584966:
						{
							float t = this.curve.Evaluate(f);
							((Transform)this.target).SetPositionAndRotation(this.bezierCurve.GetPoint(t), Quaternion.LookRotation(this.bezierCurve.GetDirection(t)));
							return;
						}
						case 12584967:
							((Transform)this.target).localRotation = Quaternion.Euler(0f, 0f, this.curve.Evaluate(f) * 360f);
							return;
						case 12584968:
						{
							Vector3 vector;
							Quaternion quaternion;
							this.transformA.GetPositionAndRotation(ref vector, ref quaternion);
							Vector3 vector2;
							Quaternion quaternion2;
							this.transformB.GetPositionAndRotation(ref vector2, ref quaternion2);
							float num4 = this.curve.Evaluate(f);
							((Transform)this.target).SetPositionAndRotation(Vector3.Lerp(vector, vector2, num4), Quaternion.Slerp(quaternion, quaternion2, num4));
							return;
						}
						case 12584969:
						{
							float num5 = this.curve.Evaluate(f);
							((Transform)this.target).SetLocalPositionAndRotation(Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, num5), Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, num5));
							return;
						}
						default:
							return;
						}
					}
					else
					{
						float num6 = this.curve.Evaluate(f);
						float num7 = 1f - Mathf.Exp(-num6 * deltaTime);
						if (Random.value < num7)
						{
							this.unityEvent.Invoke(num6);
							return;
						}
						return;
					}
					break;
				}
			}
			((Animator)this.target).SetBool(this.stringHash, this.previousBoolValue);
			return;
			IL_7AB:
			((Renderer)this.target).enabled = this.previousBoolValue;
			return;
			IL_7C2:
			((Behaviour)this.target).enabled = this.previousBoolValue;
			return;
			IL_7D9:
			((GameObject)this.target).SetActive(this.previousBoolValue);
		}

		// Token: 0x06006BE7 RID: 27623 RVA: 0x00236838 File Offset: 0x00234A38
		private ParticleSystem.MinMaxCurve ScaleCurve(in ParticleSystem.MinMaxCurve inCurve, float scale)
		{
			ParticleSystem.MinMaxCurve result = inCurve;
			switch (result.mode)
			{
			case 0:
				result.constant *= scale;
				break;
			case 1:
			case 2:
				result.curveMultiplier *= scale;
				break;
			case 3:
				result.constantMin *= scale;
				result.constantMax *= scale;
				break;
			}
			return result;
		}

		// Token: 0x06006BE8 RID: 27624 RVA: 0x002368B0 File Offset: 0x00234AB0
		private bool CheckContinuousEvent(float f, float deltaTime)
		{
			ContinuousProperty.EventMode eventMode = this.eventMode;
			if (eventMode == ContinuousProperty.EventMode.Passthrough)
			{
				return true;
			}
			if (eventMode != ContinuousProperty.EventMode.Frequency)
			{
				if (eventMode != ContinuousProperty.EventMode.AveragePerSecond)
				{
					return false;
				}
				float num = 1f - Mathf.Exp(-f * deltaTime);
				return Random.value < num;
			}
			else
			{
				this.frequencyTimer += deltaTime;
				if (this.frequencyTimer < f)
				{
					return false;
				}
				this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - f, f);
				return true;
			}
		}

		// Token: 0x06006BE9 RID: 27625 RVA: 0x0023692C File Offset: 0x00234B2C
		private ContinuousProperty.ThresholdResult CheckThreshold(float f)
		{
			if (!this.UsesThreshold_Cached)
			{
				return ContinuousProperty.ThresholdResult.Null;
			}
			bool flag = f >= this.range.x && f <= this.range.y;
			if (!this.previousBoolValue && ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal && flag) || (this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && !flag)))
			{
				this.previousBoolValue = true;
				return ContinuousProperty.ThresholdResult.RisingEdge;
			}
			if (this.previousBoolValue && ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal && !flag) || (this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && flag)))
			{
				this.previousBoolValue = false;
				return ContinuousProperty.ThresholdResult.FallingEdge;
			}
			return ContinuousProperty.ThresholdResult.Unchanged;
		}

		// Token: 0x04007C24 RID: 31780
		[SerializeField]
		private ContinuousPropertyModeSO mode;

		// Token: 0x04007C25 RID: 31781
		[FormerlySerializedAs("component")]
		[SerializeField]
		protected Object target;

		// Token: 0x04007C28 RID: 31784
		[SerializeField]
		private Gradient color;

		// Token: 0x04007C29 RID: 31785
		[SerializeField]
		private AnimationCurve curve = AnimationCurves.Linear;

		// Token: 0x04007C2A RID: 31786
		[FormerlySerializedAs("materialIndex")]
		[SerializeField]
		private int intValue;

		// Token: 0x04007C2B RID: 31787
		[SerializeField]
		private string stringValue;

		// Token: 0x04007C2C RID: 31788
		[SerializeField]
		private BezierCurve bezierCurve;

		// Token: 0x04007C2D RID: 31789
		private const string ENUM_ERROR = "Internal values were changed at some point. Please select a new value.";

		// Token: 0x04007C2E RID: 31790
		[SerializeField]
		private ContinuousProperty.RotationAxis localAxis = ContinuousProperty.RotationAxis.X;

		// Token: 0x04007C2F RID: 31791
		[SerializeField]
		private ContinuousProperty.InterpolationMode interpolationMode = ContinuousProperty.InterpolationMode.PositionAndRotation;

		// Token: 0x04007C30 RID: 31792
		[SerializeField]
		private ParticleSystemStopBehavior stopType = 1;

		// Token: 0x04007C31 RID: 31793
		[SerializeField]
		private Transform transformA;

		// Token: 0x04007C32 RID: 31794
		[SerializeField]
		private Transform transformB;

		// Token: 0x04007C33 RID: 31795
		[SerializeField]
		private XformOffset offsetA;

		// Token: 0x04007C34 RID: 31796
		[SerializeField]
		private XformOffset offsetB;

		// Token: 0x04007C35 RID: 31797
		[SerializeField]
		private Vector2 range = new Vector2(0.5f, 1f);

		// Token: 0x04007C36 RID: 31798
		[SerializeField]
		private ContinuousProperty.ThresholdOption thresholdOption = ContinuousProperty.ThresholdOption.Normal;

		// Token: 0x04007C37 RID: 31799
		[SerializeField]
		private ContinuousProperty.EventMode eventMode = ContinuousProperty.EventMode.Passthrough;

		// Token: 0x04007C38 RID: 31800
		[SerializeField]
		private UnityEvent<float> unityEvent;

		// Token: 0x04007C39 RID: 31801
		[Tooltip("Check this box if only the owner/local player is supposed to run this property.")]
		[SerializeField]
		private bool runOnlyLocally;

		// Token: 0x04007C3A RID: 31802
		private bool rigLocal;

		// Token: 0x04007C3B RID: 31803
		private int internalSwitchValue;

		// Token: 0x04007C3C RID: 31804
		private ParticleSystem.MainModule particleMain;

		// Token: 0x04007C3D RID: 31805
		private ParticleSystem.EmissionModule particleEmission;

		// Token: 0x04007C3E RID: 31806
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x04007C3F RID: 31807
		private ParticleSystem.MinMaxCurve rateCurveCache;

		// Token: 0x04007C40 RID: 31808
		private float frequencyTimer;

		// Token: 0x04007C41 RID: 31809
		private bool previousBoolValue;

		// Token: 0x04007C42 RID: 31810
		private int stringHash;

		// Token: 0x020010C7 RID: 4295
		public enum Type
		{
			// Token: 0x04007C44 RID: 31812
			Color,
			// Token: 0x04007C45 RID: 31813
			Scale,
			// Token: 0x04007C46 RID: 31814
			BlendShape,
			// Token: 0x04007C47 RID: 31815
			Float,
			// Token: 0x04007C48 RID: 31816
			ShaderVector2_X,
			// Token: 0x04007C49 RID: 31817
			ShaderColor,
			// Token: 0x04007C4A RID: 31818
			BezierInterpolation,
			// Token: 0x04007C4B RID: 31819
			AxisAngle,
			// Token: 0x04007C4C RID: 31820
			TransformInterpolation,
			// Token: 0x04007C4D RID: 31821
			OffsetInterpolation,
			// Token: 0x04007C4E RID: 31822
			Boolean,
			// Token: 0x04007C4F RID: 31823
			Speed,
			// Token: 0x04007C50 RID: 31824
			Rate,
			// Token: 0x04007C51 RID: 31825
			Volume,
			// Token: 0x04007C52 RID: 31826
			Pitch,
			// Token: 0x04007C53 RID: 31827
			PlayStop,
			// Token: 0x04007C54 RID: 31828
			EnableDisable,
			// Token: 0x04007C55 RID: 31829
			UnityEvent,
			// Token: 0x04007C56 RID: 31830
			Trigger
		}

		// Token: 0x020010C8 RID: 4296
		public enum Cast
		{
			// Token: 0x04007C58 RID: 31832
			Null,
			// Token: 0x04007C59 RID: 31833
			Any = 1024,
			// Token: 0x04007C5A RID: 31834
			Transform = 2048,
			// Token: 0x04007C5B RID: 31835
			ParticleSystem = 3072,
			// Token: 0x04007C5C RID: 31836
			SkinnedMeshRenderer = 4096,
			// Token: 0x04007C5D RID: 31837
			Animator = 5120,
			// Token: 0x04007C5E RID: 31838
			AudioSource = 6144,
			// Token: 0x04007C5F RID: 31839
			Renderer = 7168,
			// Token: 0x04007C60 RID: 31840
			Behaviour = 8192,
			// Token: 0x04007C61 RID: 31841
			GameObject = 9216,
			// Token: 0x04007C62 RID: 31842
			Rigidbody = 10240,
			// Token: 0x04007C63 RID: 31843
			VoicePitchShiftCosmetic = 11264
		}

		// Token: 0x020010C9 RID: 4297
		[Flags]
		public enum DataFlags
		{
			// Token: 0x04007C65 RID: 31845
			None = 0,
			// Token: 0x04007C66 RID: 31846
			[Tooltip("Expose the AnimationCurve for single values")]
			HasCurve = 1,
			// Token: 0x04007C67 RID: 31847
			[Tooltip("Expose the Gradient for colors")]
			HasColor = 2,
			// Token: 0x04007C68 RID: 31848
			[Tooltip("Select which axis it should rotate on")]
			HasAxis = 4,
			// Token: 0x04007C69 RID: 31849
			[Tooltip("Expose the integer, usually for material index")]
			HasInteger = 8,
			// Token: 0x04007C6A RID: 31850
			[Tooltip("Select whether to use position, rotation, or both when interpolating")]
			HasInterpolation = 16,
			// Token: 0x04007C6B RID: 31851
			[Tooltip("Expose the string and hash it into a shader property ID")]
			IsShaderProperty = 32,
			// Token: 0x04007C6C RID: 31852
			[Tooltip("Expose the string and hash it into an animator parameter ID")]
			IsAnimatorParameter = 64,
			// Token: 0x04007C6D RID: 31853
			[Tooltip("Expose the threshold range as a dual slider")]
			HasThreshold = 128
		}

		// Token: 0x020010CA RID: 4298
		private enum ThresholdResult
		{
			// Token: 0x04007C6F RID: 31855
			Null,
			// Token: 0x04007C70 RID: 31856
			RisingEdge = 1048576,
			// Token: 0x04007C71 RID: 31857
			FallingEdge = 2097152,
			// Token: 0x04007C72 RID: 31858
			Unchanged = 3145728
		}

		// Token: 0x020010CB RID: 4299
		private enum ThresholdOption
		{
			// Token: 0x04007C74 RID: 31860
			Invert,
			// Token: 0x04007C75 RID: 31861
			Normal
		}

		// Token: 0x020010CC RID: 4300
		private enum RotationAxis
		{
			// Token: 0x04007C77 RID: 31863
			X = 4194304,
			// Token: 0x04007C78 RID: 31864
			Y = 8388608,
			// Token: 0x04007C79 RID: 31865
			Z = 12582912
		}

		// Token: 0x020010CD RID: 4301
		public enum InterpolationMode
		{
			// Token: 0x04007C7B RID: 31867
			Position = 4194304,
			// Token: 0x04007C7C RID: 31868
			Rotation = 8388608,
			// Token: 0x04007C7D RID: 31869
			PositionAndRotation = 12582912
		}

		// Token: 0x020010CE RID: 4302
		public enum EventMode
		{
			// Token: 0x04007C7F RID: 31871
			Passthrough = 4194304,
			// Token: 0x04007C80 RID: 31872
			Frequency = 8388608,
			// Token: 0x04007C81 RID: 31873
			AveragePerSecond = 12582912
		}
	}
}
