using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001180 RID: 4480
	public class BoingBones : BoingReactor
	{
		// Token: 0x06007116 RID: 28950 RVA: 0x002507AB File Offset: 0x0024E9AB
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007117 RID: 28951 RVA: 0x002507B3 File Offset: 0x0024E9B3
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007118 RID: 28952 RVA: 0x002507BB File Offset: 0x0024E9BB
		protected override void OnUpgrade(Version oldVersion, Version newVersion)
		{
			base.OnUpgrade(oldVersion, newVersion);
			if (oldVersion.Revision < 33)
			{
				this.TwistPropagation = false;
			}
		}

		// Token: 0x06007119 RID: 28953 RVA: 0x002507D7 File Offset: 0x0024E9D7
		public void OnValidate()
		{
			this.RescanBoneChains();
			this.UpdateCollisionRadius();
		}

		// Token: 0x0600711A RID: 28954 RVA: 0x002507E5 File Offset: 0x0024E9E5
		public override void OnEnable()
		{
			base.OnEnable();
			this.RescanBoneChains();
			this.Reboot();
		}

		// Token: 0x0600711B RID: 28955 RVA: 0x002507F9 File Offset: 0x0024E9F9
		public override void OnDisable()
		{
			base.OnDisable();
			this.Restore();
		}

		// Token: 0x0600711C RID: 28956 RVA: 0x00250808 File Offset: 0x0024EA08
		public void RescanBoneChains()
		{
			if (this.BoneChains == null)
			{
				return;
			}
			int num = this.BoneChains.Length;
			if (this.BoneData == null || this.BoneData.Length != num)
			{
				BoingBones.Bone[][] array = new BoingBones.Bone[num][];
				if (this.BoneData != null)
				{
					int i = 0;
					int num2 = Mathf.Min(this.BoneData.Length, num);
					while (i < num2)
					{
						array[i] = this.BoneData[i];
						i++;
					}
				}
				this.BoneData = array;
			}
			Queue<BoingBones.RescanEntry> queue = new Queue<BoingBones.RescanEntry>();
			for (int j = 0; j < num; j++)
			{
				BoingBones.Chain chain = this.BoneChains[j];
				bool flag = false;
				if (this.BoneData[j] == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot != chain.Root)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedExclusion != null != (chain.Exclusion != null))
				{
					flag = true;
				}
				if (!flag && chain.Exclusion != null)
				{
					if (chain.m_scannedExclusion.Length != chain.Exclusion.Length)
					{
						flag = true;
					}
					else
					{
						for (int k = 0; k < chain.m_scannedExclusion.Length; k++)
						{
							if (!(chain.m_scannedExclusion[k] == chain.Exclusion[k]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				Transform transform = (chain != null) ? chain.Root : null;
				int num3 = (transform != null) ? Codec.HashTransformHierarchy(transform) : -1;
				if (!flag && transform != null && chain.m_hierarchyHash != num3)
				{
					flag = true;
				}
				if (flag)
				{
					if (transform == null)
					{
						this.BoneData[j] = null;
					}
					else
					{
						chain.m_scannedRoot = chain.Root;
						chain.m_scannedExclusion = Enumerable.ToArray<Transform>(chain.Exclusion);
						chain.m_hierarchyHash = num3;
						chain.MaxLengthFromRoot = 0f;
						List<BoingBones.Bone> list = new List<BoingBones.Bone>();
						queue.Enqueue(new BoingBones.RescanEntry(transform, -1, 0f));
						while (queue.Count > 0)
						{
							BoingBones.RescanEntry rescanEntry = queue.Dequeue();
							if (!Enumerable.Contains<Transform>(chain.Exclusion, rescanEntry.Transform))
							{
								int count = list.Count;
								Transform transform2 = rescanEntry.Transform;
								int[] array2 = new int[transform2.childCount];
								for (int l = 0; l < array2.Length; l++)
								{
									array2[l] = -1;
								}
								int num4 = 0;
								int m = 0;
								int childCount = transform2.childCount;
								while (m < childCount)
								{
									Transform child = transform2.GetChild(m);
									if (!Enumerable.Contains<Transform>(chain.Exclusion, child))
									{
										float num5 = Vector3.Distance(rescanEntry.Transform.position, child.position);
										float lengthFromRoot = rescanEntry.LengthFromRoot + num5;
										queue.Enqueue(new BoingBones.RescanEntry(child, count, lengthFromRoot));
										num4++;
									}
									m++;
								}
								chain.MaxLengthFromRoot = Mathf.Max(rescanEntry.LengthFromRoot, chain.MaxLengthFromRoot);
								BoingBones.Bone bone = new BoingBones.Bone(transform2, rescanEntry.ParentIndex, rescanEntry.LengthFromRoot);
								if (num4 > 0)
								{
									bone.ChildIndices = array2;
								}
								list.Add(bone);
							}
						}
						for (int n = 0; n < list.Count; n++)
						{
							BoingBones.Bone bone2 = list[n];
							if (bone2.ParentIndex >= 0)
							{
								BoingBones.Bone bone3 = list[bone2.ParentIndex];
								int num6 = 0;
								while (bone3.ChildIndices[num6] >= 0)
								{
									num6++;
								}
								if (num6 < bone3.ChildIndices.Length)
								{
									bone3.ChildIndices[num6] = n;
								}
							}
						}
						if (list.Count != 0)
						{
							float num7 = MathUtil.InvSafe(chain.MaxLengthFromRoot);
							for (int num8 = 0; num8 < list.Count; num8++)
							{
								BoingBones.Bone bone4 = list[num8];
								float t = Mathf.Clamp01(bone4.LengthFromRoot * num7);
								bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
							}
							this.BoneData[j] = list.ToArray();
							this.Reboot(j);
						}
					}
				}
			}
		}

		// Token: 0x0600711D RID: 28957 RVA: 0x00250C24 File Offset: 0x0024EE24
		private void UpdateCollisionRadius()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					float num = MathUtil.InvSafe(chain.MaxLengthFromRoot);
					foreach (BoingBones.Bone bone in array)
					{
						float t = Mathf.Clamp01(bone.LengthFromRoot * num);
						bone.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
					}
				}
			}
		}

		// Token: 0x0600711E RID: 28958 RVA: 0x00250CAC File Offset: 0x0024EEAC
		public override void Reboot()
		{
			base.Reboot();
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				this.Reboot(i);
			}
		}

		// Token: 0x0600711F RID: 28959 RVA: 0x00250CDC File Offset: 0x0024EEDC
		public void Reboot(int iChain)
		{
			BoingBones.Bone[] array = this.BoneData[iChain];
			if (array == null)
			{
				return;
			}
			foreach (BoingBones.Bone bone in array)
			{
				bone.Instance.PositionSpring.Reset(bone.Position);
				bone.Instance.RotationSpring.Reset(bone.Rotation);
				bone.CachedPositionWs = bone.Position;
				bone.CachedPositionLs = bone.Transform.localPosition;
				bone.CachedRotationWs = bone.Rotation;
				bone.CachedRotationLs = bone.Transform.localRotation;
				bone.CachedScaleLs = bone.LocalScale;
			}
			this.CachedTransformValid = true;
		}

		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x06007120 RID: 28960 RVA: 0x00250D81 File Offset: 0x0024EF81
		internal float MinScale
		{
			get
			{
				return this.m_minScale;
			}
		}

		// Token: 0x06007121 RID: 28961 RVA: 0x00250D8C File Offset: 0x0024EF8C
		public override void PrepareExecute()
		{
			base.PrepareExecute();
			this.Params.Bits.SetBit(4, false);
			float fixedDeltaTime = Time.fixedDeltaTime;
			float num = (this.UpdateMode == BoingManager.UpdateMode.FixedUpdate) ? fixedDeltaTime : Time.deltaTime;
			this.m_minScale = Mathf.Min(base.transform.localScale.x, Mathf.Min(base.transform.localScale.y, base.transform.localScale.z));
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && !(chain.Root == null) && array.Length != 0)
				{
					Vector3 vector = chain.Gravity * num;
					float num2 = 0f;
					foreach (BoingBones.Bone bone in array)
					{
						if (bone.ParentIndex < 0)
						{
							if (!chain.LooseRoot)
							{
								bone.Instance.PositionSpring.Reset(bone.Position);
								bone.Instance.RotationSpring.Reset(bone.Rotation);
							}
							bone.LengthFromRoot = 0f;
						}
						else
						{
							BoingBones.Bone bone2 = array[bone.ParentIndex];
							float num3 = Vector3.Distance(bone.Position, bone2.Position);
							bone.LengthFromRoot = bone2.LengthFromRoot + num3;
							num2 = Mathf.Max(num2, bone.LengthFromRoot);
						}
					}
					float num4 = MathUtil.InvSafe(num2);
					foreach (BoingBones.Bone bone3 in array)
					{
						float t = bone3.LengthFromRoot * num4;
						bone3.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t, chain.AnimationBlendCustomCurve);
						bone3.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t, chain.LengthStiffnessCustomCurve);
						bone3.LengthStiffnessT = 1f - Mathf.Pow(1f - bone3.LengthStiffness, 30f * fixedDeltaTime);
						bone3.FullyStiffToParentLength = ((bone3.ParentIndex >= 0) ? Vector3.Distance(array[bone3.ParentIndex].Position, bone3.Position) : 0f);
						bone3.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t, chain.PoseStiffnessCustomCurve);
						bone3.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t, chain.BendAngleCapCustomCurve);
						bone3.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
						bone3.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t, chain.SquashAndStretchCustomCurve);
					}
					Vector3 position = array[0].Position;
					for (int l = 0; l < array.Length; l++)
					{
						BoingBones.Bone bone4 = array[l];
						float t2 = bone4.LengthFromRoot * num4;
						bone4.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t2, chain.AnimationBlendCustomCurve);
						bone4.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t2, chain.LengthStiffnessCustomCurve);
						bone4.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t2, chain.PoseStiffnessCustomCurve);
						bone4.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t2, chain.BendAngleCapCustomCurve);
						bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t2, chain.CollisionRadiusCustomCurve);
						bone4.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t2, chain.SquashAndStretchCustomCurve);
						if (l > 0)
						{
							BoingBones.Bone bone5 = bone4;
							bone5.Instance.PositionSpring.Velocity = bone5.Instance.PositionSpring.Velocity + vector;
						}
						bone4.RotationInverseWs = Quaternion.Inverse(bone4.Rotation);
						bone4.SpringRotationWs = bone4.Instance.RotationSpring.ValueQuat;
						bone4.SpringRotationInverseWs = Quaternion.Inverse(bone4.SpringRotationWs);
						Vector3 vector2 = bone4.Position;
						Quaternion rotation = bone4.Rotation;
						Vector3 localScale = bone4.LocalScale;
						if (bone4.ParentIndex >= 0)
						{
							BoingBones.Bone bone6 = array[bone4.ParentIndex];
							Vector3 position2 = bone6.Position;
							Vector3 value = bone6.Instance.PositionSpring.Value;
							Vector3 vector3 = bone6.SpringRotationInverseWs * (bone4.Instance.PositionSpring.Value - value);
							Quaternion quaternion = bone6.SpringRotationInverseWs * bone4.Instance.RotationSpring.ValueQuat;
							Vector3 position3 = bone4.Position;
							Quaternion rotation2 = bone4.Rotation;
							Vector3 vector4 = bone6.RotationInverseWs * (position3 - position2);
							Quaternion quaternion2 = bone6.RotationInverseWs * rotation2;
							float poseStiffness = bone4.PoseStiffness;
							Vector3 vector5 = Vector3.Lerp(vector3, vector4, poseStiffness);
							Quaternion quaternion3 = Quaternion.Slerp(quaternion, quaternion2, poseStiffness);
							vector2 = value + bone6.SpringRotationWs * vector5;
							rotation = bone6.SpringRotationWs * quaternion3;
							if (bone4.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 vector6 = vector2 - position;
								vector6 = VectorUtil.ClampBend(vector6, position3 - position, bone4.BendAngleCap);
								vector2 = position + vector6;
							}
						}
						if (chain.ParamsOverride == null)
						{
							bone4.Instance.PrepareExecute(ref this.Params, vector2, rotation, localScale, true);
						}
						else
						{
							bone4.Instance.PrepareExecute(ref chain.ParamsOverride.Params, vector2, rotation, localScale, true);
						}
					}
				}
			}
		}

		// Token: 0x06007122 RID: 28962 RVA: 0x00251344 File Offset: 0x0024F544
		public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && chain.EffectorReaction)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.AccumulateTarget(ref this.Params, ref effector, dt);
						}
						else
						{
							Bits32 bits = chain.ParamsOverride.Params.Bits;
							chain.ParamsOverride.Params.Bits = this.Params.Bits;
							bone.Instance.AccumulateTarget(ref chain.ParamsOverride.Params, ref effector, dt);
							chain.ParamsOverride.Params.Bits = bits;
						}
					}
				}
			}
		}

		// Token: 0x06007123 RID: 28963 RVA: 0x0025142C File Offset: 0x0024F62C
		public void EndAccumulateTargets()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.EndAccumulateTargets(ref this.Params);
						}
						else
						{
							bone.Instance.EndAccumulateTargets(ref chain.ParamsOverride.Params);
						}
					}
				}
			}
		}

		// Token: 0x06007124 RID: 28964 RVA: 0x002514B0 File Offset: 0x0024F6B0
		public override void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						BoingBones.Bone bone = array[j];
						if (j != 0 || chain.LooseRoot)
						{
							bone.Transform.SetLocalPositionAndRotation(bone.CachedPositionLs, bone.CachedRotationLs);
							bone.Transform.localScale = bone.CachedScaleLs;
						}
					}
				}
			}
		}

		// Token: 0x04008149 RID: 33097
		[SerializeField]
		internal BoingBones.Bone[][] BoneData;

		// Token: 0x0400814A RID: 33098
		public BoingBones.Chain[] BoneChains = new BoingBones.Chain[1];

		// Token: 0x0400814B RID: 33099
		public bool TwistPropagation = true;

		// Token: 0x0400814C RID: 33100
		[Range(0.1f, 20f)]
		public float MaxCollisionResolutionSpeed = 3f;

		// Token: 0x0400814D RID: 33101
		public BoingBoneCollider[] BoingColliders = new BoingBoneCollider[0];

		// Token: 0x0400814E RID: 33102
		public Collider[] UnityColliders = new Collider[0];

		// Token: 0x0400814F RID: 33103
		public bool DebugDrawRawBones;

		// Token: 0x04008150 RID: 33104
		public bool DebugDrawTargetBones;

		// Token: 0x04008151 RID: 33105
		public bool DebugDrawBoingBones;

		// Token: 0x04008152 RID: 33106
		public bool DebugDrawFinalBones;

		// Token: 0x04008153 RID: 33107
		public bool DebugDrawColliders;

		// Token: 0x04008154 RID: 33108
		public bool DebugDrawChainBounds;

		// Token: 0x04008155 RID: 33109
		public bool DebugDrawBoneNames;

		// Token: 0x04008156 RID: 33110
		public bool DebugDrawLengthFromRoot;

		// Token: 0x04008157 RID: 33111
		private float m_minScale = 1f;

		// Token: 0x02001181 RID: 4481
		[Serializable]
		public class Bone
		{
			// Token: 0x17000A90 RID: 2704
			// (get) Token: 0x06007126 RID: 28966 RVA: 0x00251590 File Offset: 0x0024F790
			internal Vector3 Position
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedPos)
					{
						this.updatedPos = false;
						this.position = this.Transform.position;
					}
					return this.position;
				}
			}

			// Token: 0x17000A91 RID: 2705
			// (get) Token: 0x06007127 RID: 28967 RVA: 0x002515BE File Offset: 0x0024F7BE
			internal Quaternion Rotation
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedRot)
					{
						this.updatedRot = false;
						this.rotation = this.Transform.rotation;
					}
					return this.rotation;
				}
			}

			// Token: 0x17000A92 RID: 2706
			// (get) Token: 0x06007128 RID: 28968 RVA: 0x002515EC File Offset: 0x0024F7EC
			internal Vector3 LocalScale
			{
				get
				{
					this.CheckResetFlags();
					if (this.updatedScale)
					{
						this.updatedScale = false;
						this.localScale = this.Transform.localScale;
					}
					return this.localScale;
				}
			}

			// Token: 0x06007129 RID: 28969 RVA: 0x0025161C File Offset: 0x0024F81C
			private void CheckResetFlags()
			{
				if (this.Transform.hasChanged)
				{
					this.updatedPos = (this.updatedRot = (this.updatedScale = true));
					this.Transform.hasChanged = false;
				}
			}

			// Token: 0x0600712A RID: 28970 RVA: 0x0025165B File Offset: 0x0024F85B
			internal void UpdateBounds()
			{
				this.Bounds = new Bounds(this.Instance.PositionSpring.Value, 2f * this.CollisionRadius * Vector3.one);
			}

			// Token: 0x0600712B RID: 28971 RVA: 0x00251690 File Offset: 0x0024F890
			internal Bone(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.RotationInverseWs = Quaternion.identity;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
				this.Instance.Reset();
				this.CachedPositionWs = transform.position;
				this.CachedPositionLs = transform.localPosition;
				this.CachedRotationWs = transform.rotation;
				this.CachedRotationLs = transform.localRotation;
				this.CachedScaleLs = transform.localScale;
				this.AnimationBlend = 0f;
				this.LengthStiffness = 0f;
				this.PoseStiffness = 0f;
				this.BendAngleCap = 180f;
				this.CollisionRadius = 0f;
			}

			// Token: 0x04008158 RID: 33112
			internal BoingWork.Params.InstanceData Instance;

			// Token: 0x04008159 RID: 33113
			internal Transform Transform;

			// Token: 0x0400815A RID: 33114
			internal Vector3 ScaleWs;

			// Token: 0x0400815B RID: 33115
			internal Vector3 CachedScaleLs;

			// Token: 0x0400815C RID: 33116
			internal Vector3 BlendedPositionWs;

			// Token: 0x0400815D RID: 33117
			internal Vector3 BlendedScaleLs;

			// Token: 0x0400815E RID: 33118
			internal Vector3 CachedPositionWs;

			// Token: 0x0400815F RID: 33119
			internal Vector3 CachedPositionLs;

			// Token: 0x04008160 RID: 33120
			internal Bounds Bounds;

			// Token: 0x04008161 RID: 33121
			internal Quaternion RotationInverseWs;

			// Token: 0x04008162 RID: 33122
			internal Quaternion SpringRotationWs;

			// Token: 0x04008163 RID: 33123
			internal Quaternion SpringRotationInverseWs;

			// Token: 0x04008164 RID: 33124
			internal Quaternion CachedRotationWs;

			// Token: 0x04008165 RID: 33125
			internal Quaternion CachedRotationLs;

			// Token: 0x04008166 RID: 33126
			internal Quaternion BlendedRotationWs;

			// Token: 0x04008167 RID: 33127
			internal Quaternion RotationBackPropDeltaPs;

			// Token: 0x04008168 RID: 33128
			internal int ParentIndex;

			// Token: 0x04008169 RID: 33129
			internal int[] ChildIndices;

			// Token: 0x0400816A RID: 33130
			internal float LengthFromRoot;

			// Token: 0x0400816B RID: 33131
			internal float AnimationBlend;

			// Token: 0x0400816C RID: 33132
			internal float LengthStiffness;

			// Token: 0x0400816D RID: 33133
			internal float LengthStiffnessT;

			// Token: 0x0400816E RID: 33134
			internal float FullyStiffToParentLength;

			// Token: 0x0400816F RID: 33135
			internal float PoseStiffness;

			// Token: 0x04008170 RID: 33136
			internal float BendAngleCap;

			// Token: 0x04008171 RID: 33137
			internal float CollisionRadius;

			// Token: 0x04008172 RID: 33138
			internal float SquashAndStretch;

			// Token: 0x04008173 RID: 33139
			private bool updatedPos;

			// Token: 0x04008174 RID: 33140
			private bool updatedRot;

			// Token: 0x04008175 RID: 33141
			private bool updatedScale;

			// Token: 0x04008176 RID: 33142
			private Vector3 position;

			// Token: 0x04008177 RID: 33143
			private Quaternion rotation;

			// Token: 0x04008178 RID: 33144
			private Vector3 localScale;
		}

		// Token: 0x02001182 RID: 4482
		[Serializable]
		public class Chain
		{
			// Token: 0x0600712C RID: 28972 RVA: 0x00251744 File Offset: 0x0024F944
			public static float EvaluateCurve(BoingBones.Chain.CurveType type, float t, AnimationCurve curve)
			{
				switch (type)
				{
				case BoingBones.Chain.CurveType.ConstantOne:
					return 1f;
				case BoingBones.Chain.CurveType.ConstantHalf:
					return 0.5f;
				case BoingBones.Chain.CurveType.ConstantZero:
					return 0f;
				case BoingBones.Chain.CurveType.RootOneTailHalf:
					return 1f - 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootOneTailZero:
					return 1f - Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootHalfTailOne:
					return 0.5f + 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootZeroTailOne:
					return Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.Custom:
					return curve.Evaluate(t);
				default:
					return 0f;
				}
			}

			// Token: 0x04008179 RID: 33145
			[Tooltip("Root Transform object from which to build a chain (or tree if a bone has multiple children) of bouncy boing bones.")]
			public Transform Root;

			// Token: 0x0400817A RID: 33146
			[Tooltip("List of Transform objects to exclude from chain building.")]
			public Transform[] Exclusion;

			// Token: 0x0400817B RID: 33147
			[Tooltip("Enable to allow reaction to boing effectors.")]
			public bool EffectorReaction = true;

			// Token: 0x0400817C RID: 33148
			[Tooltip("Enable to allow root Transform object to be sprung around as well. Otherwise, no effects will be applied to the root Transform object.")]
			public bool LooseRoot;

			// Token: 0x0400817D RID: 33149
			[Tooltip("Assign a SharedParamsOverride asset to override the parameters for this chain. Useful for chains using different parameters than that of the BoingBones component.")]
			public SharedBoingParams ParamsOverride;

			// Token: 0x0400817E RID: 33150
			[ConditionalField(null, null, null, null, null, null, null, Label = "Animation Blend", Tooltip = "Animation blend determines each bone's final transform between the original raw transform and its corresponding boing bone. 1.0 means 100% contribution from raw (or animated) transform. 0.0 means 100% contribution from boing bone.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's animation blend:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailZero;

			// Token: 0x0400817F RID: 33151
			[ConditionalField("AnimationBlendCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve AnimationBlendCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			// Token: 0x04008180 RID: 33152
			[ConditionalField(null, null, null, null, null, null, null, Label = "Length Stiffness", Tooltip = "Length stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original distance from its parent. 1.0 means 100% distance maintenance. 0.0 means 0% distance maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's length stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType LengthStiffnessCurveType;

			// Token: 0x04008181 RID: 33153
			[ConditionalField("LengthStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve LengthStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008182 RID: 33154
			[ConditionalField(null, null, null, null, null, null, null, Label = "Pose Stiffness", Tooltip = "Pose stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original transform. 1.0 means 100% original transform maintenance. 0.0 means 0% original transform maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType PoseStiffnessCurveType;

			// Token: 0x04008183 RID: 33155
			[ConditionalField("PoseStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve PoseStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008184 RID: 33156
			[ConditionalField(null, null, null, null, null, null, null, Label = "Bend Angle Cap", Tooltip = "Maximum bone bend angle cap.", Min = 0f, Max = 180f)]
			public float MaxBendAngleCap = 180f;

			// Token: 0x04008185 RID: 33157
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage(0.0 = 0 %; 1.0 = 100 %) of maximum bone bend angle cap.Bend angle cap limits how much each bone can bend relative to the root (in degrees). 1.0 means 100% maximum bend angle cap. 0.0 means 0% maximum bend angle cap.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType BendAngleCapCurveType;

			// Token: 0x04008186 RID: 33158
			[ConditionalField("BendAngleCapCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve BendAngleCapCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04008187 RID: 33159
			[ConditionalField(null, null, null, null, null, null, null, Label = "Collision Radius", Tooltip = "Maximum bone collision radius.")]
			public float MaxCollisionRadius = 0.1f;

			// Token: 0x04008188 RID: 33160
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of maximum bone collision radius.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's collision radius:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType CollisionRadiusCurveType;

			// Token: 0x04008189 RID: 33161
			[ConditionalField("CollisionRadiusCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve CollisionRadiusCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x0400818A RID: 33162
			[ConditionalField(null, null, null, null, null, null, null, Label = "Boing Kit Collision", Tooltip = "Enable to allow this chain to collide with Boing Kit's own implementation of lightweight colliders")]
			public bool EnableBoingKitCollision;

			// Token: 0x0400818B RID: 33163
			[ConditionalField(null, null, null, null, null, null, null, Label = "Unity Collision", Tooltip = "Enable to allow this chain to collide with Unity colliders.")]
			public bool EnableUnityCollision;

			// Token: 0x0400818C RID: 33164
			[ConditionalField(null, null, null, null, null, null, null, Label = "Inter-Chain Collision", Tooltip = "Enable to allow this chain to collide with other chain (under the same BoingBones component) with inter-chain collision enabled.")]
			public bool EnableInterChainCollision;

			// Token: 0x0400818D RID: 33165
			public Vector3 Gravity = Vector3.zero;

			// Token: 0x0400818E RID: 33166
			internal Bounds Bounds;

			// Token: 0x0400818F RID: 33167
			[ConditionalField(null, null, null, null, null, null, null, Label = "Squash & Stretch", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of each bone's squash & stretch effect. Squash & stretch is the effect of volume preservation by scaling bones based on how compressed or stretched the distances between bones become.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's squash & stretch effect amount:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType SquashAndStretchCurveType = BoingBones.Chain.CurveType.ConstantZero;

			// Token: 0x04008190 RID: 33168
			[ConditionalField("SquashAndStretchCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve SquashAndStretchCustomCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04008191 RID: 33169
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Squash", Tooltip = "Maximum squash amount. For example, 2.0 means a maximum scale of 200% when squashed.", Min = 1f, Max = 5f)]
			public float MaxSquash = 1.1f;

			// Token: 0x04008192 RID: 33170
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Stretch", Tooltip = "Maximum stretch amount. For example, 2.0 means a minimum scale of 50% when stretched (200% stretched).", Min = 1f, Max = 5f)]
			public float MaxStretch = 2f;

			// Token: 0x04008193 RID: 33171
			internal Transform m_scannedRoot;

			// Token: 0x04008194 RID: 33172
			internal Transform[] m_scannedExclusion;

			// Token: 0x04008195 RID: 33173
			internal int m_hierarchyHash = -1;

			// Token: 0x04008196 RID: 33174
			internal float MaxLengthFromRoot;

			// Token: 0x02001183 RID: 4483
			public enum CurveType
			{
				// Token: 0x04008198 RID: 33176
				ConstantOne,
				// Token: 0x04008199 RID: 33177
				ConstantHalf,
				// Token: 0x0400819A RID: 33178
				ConstantZero,
				// Token: 0x0400819B RID: 33179
				RootOneTailHalf,
				// Token: 0x0400819C RID: 33180
				RootOneTailZero,
				// Token: 0x0400819D RID: 33181
				RootHalfTailOne,
				// Token: 0x0400819E RID: 33182
				RootZeroTailOne,
				// Token: 0x0400819F RID: 33183
				Custom
			}
		}

		// Token: 0x02001184 RID: 4484
		private class RescanEntry
		{
			// Token: 0x0600712E RID: 28974 RVA: 0x002518F4 File Offset: 0x0024FAF4
			internal RescanEntry(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
			}

			// Token: 0x040081A0 RID: 33184
			internal Transform Transform;

			// Token: 0x040081A1 RID: 33185
			internal int ParentIndex;

			// Token: 0x040081A2 RID: 33186
			internal float LengthFromRoot;
		}
	}
}
