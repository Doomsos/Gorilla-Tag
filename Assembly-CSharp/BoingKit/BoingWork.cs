using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011A7 RID: 4519
	public static class BoingWork
	{
		// Token: 0x060071F5 RID: 29173 RVA: 0x00255820 File Offset: 0x00253A20
		internal static Vector3 ComputeTranslationalResults(Transform t, Vector3 src, Vector3 dst, BoingBehavior b)
		{
			if (!b.LockTranslationX && !b.LockTranslationY && !b.LockTranslationZ)
			{
				return dst;
			}
			Vector3 vector = dst - src;
			BoingManager.TranslationLockSpace translationLockSpace = b.TranslationLockSpace;
			if (translationLockSpace != BoingManager.TranslationLockSpace.Global)
			{
				if (translationLockSpace == BoingManager.TranslationLockSpace.Local)
				{
					if (b.LockTranslationX)
					{
						vector -= Vector3.Project(vector, t.right);
					}
					if (b.LockTranslationY)
					{
						vector -= Vector3.Project(vector, t.up);
					}
					if (b.LockTranslationZ)
					{
						vector -= Vector3.Project(vector, t.forward);
					}
				}
			}
			else
			{
				if (b.LockTranslationX)
				{
					vector.x = 0f;
				}
				if (b.LockTranslationY)
				{
					vector.y = 0f;
				}
				if (b.LockTranslationZ)
				{
					vector.z = 0f;
				}
			}
			return src + vector;
		}

		// Token: 0x020011A8 RID: 4520
		public enum EffectorFlags
		{
			// Token: 0x04008261 RID: 33377
			ContinuousMotion
		}

		// Token: 0x020011A9 RID: 4521
		public enum ReactorFlags
		{
			// Token: 0x04008263 RID: 33379
			TwoDDistanceCheck,
			// Token: 0x04008264 RID: 33380
			TwoDPositionInfluence,
			// Token: 0x04008265 RID: 33381
			TwoDRotationInfluence,
			// Token: 0x04008266 RID: 33382
			EnablePositionEffect,
			// Token: 0x04008267 RID: 33383
			EnableRotationEffect,
			// Token: 0x04008268 RID: 33384
			EnableScaleEffect,
			// Token: 0x04008269 RID: 33385
			GlobalReactionUpVector,
			// Token: 0x0400826A RID: 33386
			EnablePropagation,
			// Token: 0x0400826B RID: 33387
			AnchorPropagationAtBorder,
			// Token: 0x0400826C RID: 33388
			FixedUpdate,
			// Token: 0x0400826D RID: 33389
			EarlyUpdate,
			// Token: 0x0400826E RID: 33390
			LateUpdate
		}

		// Token: 0x020011AA RID: 4522
		[Serializable]
		public struct Params
		{
			// Token: 0x060071F6 RID: 29174 RVA: 0x002558F8 File Offset: 0x00253AF8
			public static void Copy(ref BoingWork.Params from, ref BoingWork.Params to)
			{
				to.PositionParameterMode = from.PositionParameterMode;
				to.RotationParameterMode = from.RotationParameterMode;
				to.PositionExponentialHalfLife = from.PositionExponentialHalfLife;
				to.PositionOscillationHalfLife = from.PositionOscillationHalfLife;
				to.PositionOscillationFrequency = from.PositionOscillationFrequency;
				to.PositionOscillationDampingRatio = from.PositionOscillationDampingRatio;
				to.MoveReactionMultiplier = from.MoveReactionMultiplier;
				to.LinearImpulseMultiplier = from.LinearImpulseMultiplier;
				to.RotationExponentialHalfLife = from.RotationExponentialHalfLife;
				to.RotationOscillationHalfLife = from.RotationOscillationHalfLife;
				to.RotationOscillationFrequency = from.RotationOscillationFrequency;
				to.RotationOscillationDampingRatio = from.RotationOscillationDampingRatio;
				to.RotationReactionMultiplier = from.RotationReactionMultiplier;
				to.AngularImpulseMultiplier = from.AngularImpulseMultiplier;
				to.ScaleExponentialHalfLife = from.ScaleExponentialHalfLife;
				to.ScaleOscillationHalfLife = from.ScaleOscillationHalfLife;
				to.ScaleOscillationFrequency = from.ScaleOscillationFrequency;
				to.ScaleOscillationDampingRatio = from.ScaleOscillationDampingRatio;
			}

			// Token: 0x060071F7 RID: 29175 RVA: 0x002559E0 File Offset: 0x00253BE0
			public void Init()
			{
				this.InstanceID = -1;
				this.Bits.Clear();
				this.TwoDPlane = TwoDPlaneEnum.XZ;
				this.PositionParameterMode = ParameterMode.OscillationByHalfLife;
				this.RotationParameterMode = ParameterMode.OscillationByHalfLife;
				this.ScaleParameterMode = ParameterMode.OscillationByHalfLife;
				this.PositionExponentialHalfLife = 0.02f;
				this.PositionOscillationHalfLife = 0.1f;
				this.PositionOscillationFrequency = 5f;
				this.PositionOscillationDampingRatio = 0.5f;
				this.MoveReactionMultiplier = 1f;
				this.LinearImpulseMultiplier = 1f;
				this.RotationExponentialHalfLife = 0.02f;
				this.RotationOscillationHalfLife = 0.1f;
				this.RotationOscillationFrequency = 5f;
				this.RotationOscillationDampingRatio = 0.5f;
				this.RotationReactionMultiplier = 1f;
				this.AngularImpulseMultiplier = 1f;
				this.ScaleExponentialHalfLife = 0.02f;
				this.ScaleOscillationHalfLife = 0.1f;
				this.ScaleOscillationFrequency = 5f;
				this.ScaleOscillationDampingRatio = 0.5f;
				this.Instance.Reset();
			}

			// Token: 0x060071F8 RID: 29176 RVA: 0x00255AD6 File Offset: 0x00253CD6
			public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
			{
				this.Instance.AccumulateTarget(ref this, ref effector, dt);
			}

			// Token: 0x060071F9 RID: 29177 RVA: 0x00255AE6 File Offset: 0x00253CE6
			public void EndAccumulateTargets()
			{
				this.Instance.EndAccumulateTargets(ref this);
			}

			// Token: 0x060071FA RID: 29178 RVA: 0x00255AF4 File Offset: 0x00253CF4
			public void Execute(float dt)
			{
				this.Instance.Execute(ref this, dt);
			}

			// Token: 0x060071FB RID: 29179 RVA: 0x00255B04 File Offset: 0x00253D04
			public void Execute(BoingBones bones, float dt)
			{
				float maxLen = bones.MaxCollisionResolutionSpeed * dt;
				for (int i = 0; i < bones.BoneData.Length; i++)
				{
					BoingBones.Chain chain = bones.BoneChains[i];
					BoingBones.Bone[] array = bones.BoneData[i];
					if (array != null)
					{
						foreach (BoingBones.Bone bone in array)
						{
							if (chain.ParamsOverride == null)
							{
								bone.Instance.Execute(ref bones.Params, dt);
							}
							else
							{
								bone.Instance.Execute(ref chain.ParamsOverride.Params, dt);
							}
						}
						BoingBones.Bone bone2 = array[0];
						bone2.ScaleWs = (bone2.BlendedScaleLs = bone2.CachedScaleLs);
						bone2.UpdateBounds();
						chain.Bounds = bone2.Bounds;
						Vector3 position = bone2.Position;
						for (int k = 1; k < array.Length; k++)
						{
							BoingBones.Bone bone3 = array[k];
							BoingBones.Bone bone4 = array[bone3.ParentIndex];
							Vector3 vector = bone4.Instance.PositionSpring.Value - bone3.Instance.PositionSpring.Value;
							Vector3 vector2 = VectorUtil.NormalizeSafe(vector, Vector3.zero);
							float magnitude = vector.magnitude;
							float num = magnitude - bone3.FullyStiffToParentLength;
							float num2 = bone3.LengthStiffnessT * num;
							BoingBones.Bone bone5 = bone3;
							bone5.Instance.PositionSpring.Value = bone5.Instance.PositionSpring.Value + num2 * vector2;
							Vector3 vector3 = Vector3.Project(bone3.Instance.PositionSpring.Velocity, vector2);
							BoingBones.Bone bone6 = bone3;
							bone6.Instance.PositionSpring.Velocity = bone6.Instance.PositionSpring.Velocity - bone3.LengthStiffnessT * vector3;
							if (bone3.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 position2 = bone3.Position;
								Vector3 vector4 = bone3.Instance.PositionSpring.Value - position;
								vector4 = VectorUtil.ClampBend(vector4, position2 - position, bone3.BendAngleCap);
								bone3.Instance.PositionSpring.Value = position + vector4;
							}
							if (bone3.SquashAndStretch > 0f)
							{
								float num3 = magnitude * MathUtil.InvSafe(bone3.FullyStiffToParentLength);
								Vector3 vector5 = VectorUtil.ComponentWiseDivSafe(Mathf.Clamp(Mathf.Sqrt(1f / num3), 1f / Mathf.Max(1f, chain.MaxStretch), Mathf.Max(1f, chain.MaxSquash)) * Vector3.one, bone4.ScaleWs);
								bone3.BlendedScaleLs = Vector3.Lerp(Vector3.Lerp(bone3.CachedScaleLs, vector5, bone3.SquashAndStretch), bone3.CachedScaleLs, bone3.AnimationBlend);
							}
							else
							{
								bone3.BlendedScaleLs = bone3.CachedScaleLs;
							}
							bone3.ScaleWs = VectorUtil.ComponentWiseMult(bone4.ScaleWs, bone3.BlendedScaleLs);
							bone3.UpdateBounds();
							chain.Bounds.Encapsulate(bone3.Bounds);
						}
						chain.Bounds.Expand(0.2f * Vector3.one);
						if (chain.EnableBoingKitCollision)
						{
							foreach (BoingBoneCollider boingBoneCollider in bones.BoingColliders)
							{
								if (!(boingBoneCollider == null) && chain.Bounds.Intersects(boingBoneCollider.Bounds))
								{
									foreach (BoingBones.Bone bone7 in array)
									{
										Vector3 vector6;
										if (bone7.Bounds.Intersects(boingBoneCollider.Bounds) && boingBoneCollider.Collide(bone7.Instance.PositionSpring.Value, bones.MinScale * bone7.CollisionRadius, out vector6))
										{
											BoingBones.Bone bone8 = bone7;
											bone8.Instance.PositionSpring.Value = bone8.Instance.PositionSpring.Value + VectorUtil.ClampLength(vector6, 0f, maxLen);
											BoingBones.Bone bone9 = bone7;
											bone9.Instance.PositionSpring.Velocity = bone9.Instance.PositionSpring.Velocity - Vector3.Project(bone7.Instance.PositionSpring.Velocity, vector6);
										}
									}
								}
							}
						}
						SphereCollider sharedSphereCollider = BoingManager.SharedSphereCollider;
						if (chain.EnableUnityCollision && sharedSphereCollider != null)
						{
							sharedSphereCollider.enabled = true;
							foreach (Collider collider in bones.UnityColliders)
							{
								if (!(collider == null) && chain.Bounds.Intersects(collider.bounds))
								{
									foreach (BoingBones.Bone bone10 in array)
									{
										if (bone10.Bounds.Intersects(collider.bounds))
										{
											sharedSphereCollider.center = bone10.Instance.PositionSpring.Value;
											sharedSphereCollider.radius = bone10.CollisionRadius;
											Vector3 vector7;
											float num4;
											if (Physics.ComputePenetration(sharedSphereCollider, Vector3.zero, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, ref vector7, ref num4))
											{
												BoingBones.Bone bone11 = bone10;
												bone11.Instance.PositionSpring.Value = bone11.Instance.PositionSpring.Value + VectorUtil.ClampLength(vector7 * num4, 0f, maxLen);
												BoingBones.Bone bone12 = bone10;
												bone12.Instance.PositionSpring.Velocity = bone12.Instance.PositionSpring.Velocity - Vector3.Project(bone10.Instance.PositionSpring.Velocity, vector7);
											}
										}
									}
								}
							}
							sharedSphereCollider.enabled = false;
						}
						if (chain.EnableInterChainCollision)
						{
							foreach (BoingBones.Bone bone13 in array)
							{
								for (int n = i + 1; n < bones.BoneData.Length; n++)
								{
									BoingBones.Chain chain2 = bones.BoneChains[n];
									BoingBones.Bone[] array3 = bones.BoneData[n];
									if (array3 != null && chain2.EnableInterChainCollision && chain.Bounds.Intersects(chain2.Bounds))
									{
										foreach (BoingBones.Bone bone14 in array3)
										{
											Vector3 vector8;
											if (Collision.SphereSphere(bone13.Instance.PositionSpring.Value, bones.MinScale * bone13.CollisionRadius, bone14.Instance.PositionSpring.Value, bones.MinScale * bone14.CollisionRadius, out vector8))
											{
												vector8 = VectorUtil.ClampLength(vector8, 0f, maxLen);
												float num5 = bone14.CollisionRadius * MathUtil.InvSafe(bone13.CollisionRadius + bone14.CollisionRadius);
												BoingBones.Bone bone15 = bone13;
												bone15.Instance.PositionSpring.Value = bone15.Instance.PositionSpring.Value + num5 * vector8;
												BoingBones.Bone bone16 = bone14;
												bone16.Instance.PositionSpring.Value = bone16.Instance.PositionSpring.Value - (1f - num5) * vector8;
												BoingBones.Bone bone17 = bone13;
												bone17.Instance.PositionSpring.Velocity = bone17.Instance.PositionSpring.Velocity - Vector3.Project(bone13.Instance.PositionSpring.Velocity, vector8);
												BoingBones.Bone bone18 = bone14;
												bone18.Instance.PositionSpring.Velocity = bone18.Instance.PositionSpring.Velocity - Vector3.Project(bone14.Instance.PositionSpring.Velocity, vector8);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x060071FC RID: 29180 RVA: 0x002562D7 File Offset: 0x002544D7
			public void PullResults(BoingBones bones)
			{
				this.Instance.PullResults(bones);
			}

			// Token: 0x060071FD RID: 29181 RVA: 0x002562E5 File Offset: 0x002544E5
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0f;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding0;
				this.m_padding2 = (float)this.m_padding0;
			}

			// Token: 0x0400826F RID: 33391
			public static readonly int Stride = 112 + BoingWork.Params.InstanceData.Stride;

			// Token: 0x04008270 RID: 33392
			public int InstanceID;

			// Token: 0x04008271 RID: 33393
			public Bits32 Bits;

			// Token: 0x04008272 RID: 33394
			public TwoDPlaneEnum TwoDPlane;

			// Token: 0x04008273 RID: 33395
			private int m_padding0;

			// Token: 0x04008274 RID: 33396
			public ParameterMode PositionParameterMode;

			// Token: 0x04008275 RID: 33397
			public ParameterMode RotationParameterMode;

			// Token: 0x04008276 RID: 33398
			public ParameterMode ScaleParameterMode;

			// Token: 0x04008277 RID: 33399
			private int m_padding1;

			// Token: 0x04008278 RID: 33400
			[Range(0f, 5f)]
			public float PositionExponentialHalfLife;

			// Token: 0x04008279 RID: 33401
			[Range(0f, 5f)]
			public float PositionOscillationHalfLife;

			// Token: 0x0400827A RID: 33402
			[Range(0f, 10f)]
			public float PositionOscillationFrequency;

			// Token: 0x0400827B RID: 33403
			[Range(0f, 1f)]
			public float PositionOscillationDampingRatio;

			// Token: 0x0400827C RID: 33404
			[Range(0f, 10f)]
			public float MoveReactionMultiplier;

			// Token: 0x0400827D RID: 33405
			[Range(0f, 10f)]
			public float LinearImpulseMultiplier;

			// Token: 0x0400827E RID: 33406
			[Range(0f, 5f)]
			public float RotationExponentialHalfLife;

			// Token: 0x0400827F RID: 33407
			[Range(0f, 5f)]
			public float RotationOscillationHalfLife;

			// Token: 0x04008280 RID: 33408
			[Range(0f, 10f)]
			public float RotationOscillationFrequency;

			// Token: 0x04008281 RID: 33409
			[Range(0f, 1f)]
			public float RotationOscillationDampingRatio;

			// Token: 0x04008282 RID: 33410
			[Range(0f, 10f)]
			public float RotationReactionMultiplier;

			// Token: 0x04008283 RID: 33411
			[Range(0f, 10f)]
			public float AngularImpulseMultiplier;

			// Token: 0x04008284 RID: 33412
			[Range(0f, 5f)]
			public float ScaleExponentialHalfLife;

			// Token: 0x04008285 RID: 33413
			[Range(0f, 5f)]
			public float ScaleOscillationHalfLife;

			// Token: 0x04008286 RID: 33414
			[Range(0f, 10f)]
			public float ScaleOscillationFrequency;

			// Token: 0x04008287 RID: 33415
			[Range(0f, 1f)]
			public float ScaleOscillationDampingRatio;

			// Token: 0x04008288 RID: 33416
			public Vector3 RotationReactionUp;

			// Token: 0x04008289 RID: 33417
			private float m_padding2;

			// Token: 0x0400828A RID: 33418
			public BoingWork.Params.InstanceData Instance;

			// Token: 0x020011AB RID: 4523
			public struct InstanceData
			{
				// Token: 0x060071FF RID: 29183 RVA: 0x00256334 File Offset: 0x00254534
				public void Reset()
				{
					this.PositionSpring.Reset();
					this.RotationSpring.Reset();
					this.ScaleSpring.Reset(Vector3.one, Vector3.zero);
					this.PositionPropagationWorkData = Vector3.zero;
					this.RotationPropagationWorkData = Vector3.zero;
				}

				// Token: 0x06007200 RID: 29184 RVA: 0x00256388 File Offset: 0x00254588
				public void Reset(Vector3 position, bool instantAccumulation)
				{
					this.PositionSpring.Reset(position);
					this.RotationSpring.Reset();
					this.ScaleSpring.Reset(Vector3.one, Vector3.zero);
					this.PositionPropagationWorkData = Vector3.zero;
					this.RotationPropagationWorkData = Vector3.zero;
					this.m_instantAccumulation = (instantAccumulation ? 1 : 0);
				}

				// Token: 0x06007201 RID: 29185 RVA: 0x002563EC File Offset: 0x002545EC
				public void PrepareExecute(ref BoingWork.Params p, Vector3 position, Quaternion rotation, Vector3 scale, bool accumulateEffectors)
				{
					this.PositionOrigin = position;
					this.PositionTarget = position;
					this.RotationTarget = (this.RotationOrigin = QuaternionUtil.ToVector4(rotation));
					this.ScaleTarget = scale;
					this.m_minScale = VectorUtil.MinComponent(scale);
					if (accumulateEffectors)
					{
						this.PositionTarget = Vector3.zero;
						this.RotationTarget = Vector4.zero;
						this.m_numEffectors = 0;
						this.m_upWs = (p.Bits.IsBitSet(6) ? p.RotationReactionUp : (rotation * VectorUtil.NormalizeSafe(p.RotationReactionUp, Vector3.up)));
						return;
					}
					this.m_numEffectors = -1;
					this.m_upWs = Vector3.zero;
				}

				// Token: 0x06007202 RID: 29186 RVA: 0x002564A8 File Offset: 0x002546A8
				public void PrepareExecute(ref BoingWork.Params p, Vector3 gridCenter, Quaternion gridRotation, Vector3 cellOffset)
				{
					this.PositionOrigin = gridCenter + cellOffset;
					this.RotationOrigin = QuaternionUtil.ToVector4(Quaternion.identity);
					this.PositionTarget = Vector3.zero;
					this.RotationTarget = Vector4.zero;
					this.m_numEffectors = 0;
					this.m_upWs = (p.Bits.IsBitSet(6) ? p.RotationReactionUp : (gridRotation * VectorUtil.NormalizeSafe(p.RotationReactionUp, Vector3.up)));
					this.m_minScale = 1f;
				}

				// Token: 0x06007203 RID: 29187 RVA: 0x0025653C File Offset: 0x0025473C
				public void AccumulateTarget(ref BoingWork.Params p, ref BoingEffector.Params effector, float dt)
				{
					Vector3 vector = effector.Bits.IsBitSet(0) ? VectorUtil.GetClosestPointOnSegment(this.PositionOrigin, effector.PrevPosition, effector.CurrPosition) : effector.CurrPosition;
					Vector3 vector2 = this.PositionOrigin - vector;
					Vector3 vector3 = vector2;
					if (p.Bits.IsBitSet(0))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector2.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector2.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector2.x = 0f;
							break;
						}
					}
					if (Mathf.Abs(vector2.x) > effector.Radius || Mathf.Abs(vector2.y) > effector.Radius || Mathf.Abs(vector2.z) > effector.Radius || vector2.sqrMagnitude > effector.Radius * effector.Radius)
					{
						return;
					}
					float magnitude = vector2.magnitude;
					float num = (effector.Radius - effector.FullEffectRadius > MathUtil.Epsilon) ? (1f - Mathf.Clamp01((magnitude - effector.FullEffectRadius) / (effector.Radius - effector.FullEffectRadius))) : 1f;
					Vector3 vector4 = this.m_upWs;
					Vector3 vector5 = this.m_upWs;
					Vector3 vector6 = VectorUtil.NormalizeSafe(vector3, this.m_upWs);
					Vector3 vector7 = vector6;
					if (p.Bits.IsBitSet(1))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector6.z = 0f;
							vector4.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector6.y = 0f;
							vector4.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector6.x = 0f;
							vector4.x = 0f;
							break;
						}
						if (vector4.sqrMagnitude < MathUtil.Epsilon)
						{
							switch (p.TwoDPlane)
							{
							case TwoDPlaneEnum.XY:
								vector4 = Vector3.up;
								break;
							case TwoDPlaneEnum.XZ:
								vector4 = Vector3.forward;
								break;
							case TwoDPlaneEnum.YZ:
								vector4 = Vector3.up;
								break;
							}
						}
						else
						{
							vector4.Normalize();
						}
						vector6 = VectorUtil.NormalizeSafe(vector6, vector4);
					}
					if (p.Bits.IsBitSet(2))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector7.z = 0f;
							vector5.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector7.y = 0f;
							vector5.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector7.x = 0f;
							vector5.x = 0f;
							break;
						}
						if (vector5.sqrMagnitude < MathUtil.Epsilon)
						{
							switch (p.TwoDPlane)
							{
							case TwoDPlaneEnum.XY:
								vector5 = Vector3.up;
								break;
							case TwoDPlaneEnum.XZ:
								vector5 = Vector3.forward;
								break;
							case TwoDPlaneEnum.YZ:
								vector5 = Vector3.up;
								break;
							}
						}
						else
						{
							vector5.Normalize();
						}
						vector7 = VectorUtil.NormalizeSafe(vector7, vector5);
					}
					if (p.Bits.IsBitSet(3))
					{
						Vector3 vector8 = num * p.MoveReactionMultiplier * effector.MoveDistance * vector6;
						this.PositionTarget += vector8;
						this.PositionSpring.Velocity = this.PositionSpring.Velocity + num * p.LinearImpulseMultiplier * effector.LinearImpulse * effector.LinearVelocityDir * (60f * dt);
					}
					if (p.Bits.IsBitSet(4))
					{
						Vector3 vector9 = VectorUtil.NormalizeSafe(Vector3.Cross(vector5, vector7), VectorUtil.FindOrthogonal(vector5));
						Vector3 v = num * p.RotationReactionMultiplier * effector.RotateAngle * vector9;
						this.RotationTarget += QuaternionUtil.ToVector4(QuaternionUtil.FromAngularVector(v));
						Vector3 v2 = VectorUtil.NormalizeSafe(Vector3.Cross(effector.LinearVelocityDir, vector7 - 0.01f * Vector3.up), vector9);
						float num2 = num * p.AngularImpulseMultiplier * effector.AngularImpulse * (60f * dt);
						Vector4 vector10 = QuaternionUtil.ToVector4(QuaternionUtil.FromAngularVector(v2));
						this.RotationSpring.VelocityVec = this.RotationSpring.VelocityVec + num2 * vector10;
					}
					this.m_numEffectors++;
				}

				// Token: 0x06007204 RID: 29188 RVA: 0x002569E4 File Offset: 0x00254BE4
				public void EndAccumulateTargets(ref BoingWork.Params p)
				{
					if (this.m_numEffectors > 0)
					{
						this.PositionTarget *= this.m_minScale / (float)this.m_numEffectors;
						this.PositionTarget += this.PositionOrigin;
						this.RotationTarget /= (float)this.m_numEffectors;
						this.RotationTarget = QuaternionUtil.ToVector4(QuaternionUtil.FromVector4(this.RotationTarget, true) * QuaternionUtil.FromVector4(this.RotationOrigin, true));
						return;
					}
					this.PositionTarget = this.PositionOrigin;
					this.RotationTarget = this.RotationOrigin;
				}

				// Token: 0x06007205 RID: 29189 RVA: 0x00256A8C File Offset: 0x00254C8C
				public void Execute(ref BoingWork.Params p, float dt)
				{
					bool flag = this.m_numEffectors >= 0;
					bool flag2 = flag ? (this.PositionSpring.Velocity.sqrMagnitude > MathUtil.Epsilon || (this.PositionSpring.Value - this.PositionTarget).sqrMagnitude > MathUtil.Epsilon) : p.Bits.IsBitSet(3);
					bool flag3 = flag ? (this.RotationSpring.VelocityVec.sqrMagnitude > MathUtil.Epsilon || (this.RotationSpring.ValueVec - this.RotationTarget).sqrMagnitude > MathUtil.Epsilon) : p.Bits.IsBitSet(4);
					bool flag4 = p.Bits.IsBitSet(5) && (this.ScaleSpring.Value - this.ScaleTarget).sqrMagnitude > MathUtil.Epsilon;
					if (this.m_numEffectors == 0)
					{
						bool flag5 = true;
						if (flag2)
						{
							flag5 = false;
						}
						else
						{
							this.PositionSpring.Reset(this.PositionTarget);
						}
						if (flag3)
						{
							flag5 = false;
						}
						else
						{
							this.RotationSpring.Reset(QuaternionUtil.FromVector4(this.RotationTarget, true));
						}
						if (flag5)
						{
							return;
						}
					}
					if (this.m_instantAccumulation != 0)
					{
						this.PositionSpring.Value = this.PositionTarget;
						this.RotationSpring.ValueVec = this.RotationTarget;
						this.ScaleSpring.Value = this.ScaleTarget;
						this.m_instantAccumulation = 0;
					}
					else
					{
						if (flag2)
						{
							switch (p.PositionParameterMode)
							{
							case ParameterMode.Exponential:
								this.PositionSpring.TrackExponential(this.PositionTarget, p.PositionExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.PositionSpring.TrackHalfLife(this.PositionTarget, p.PositionOscillationFrequency, p.PositionOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.PositionSpring.TrackDampingRatio(this.PositionTarget, p.PositionOscillationFrequency * MathUtil.TwoPi, p.PositionOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.PositionSpring.Value = this.PositionTarget;
							this.PositionSpring.Velocity = Vector3.zero;
						}
						if (flag3)
						{
							switch (p.RotationParameterMode)
							{
							case ParameterMode.Exponential:
								this.RotationSpring.TrackExponential(this.RotationTarget, p.RotationExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.RotationSpring.TrackHalfLife(this.RotationTarget, p.RotationOscillationFrequency, p.RotationOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.RotationSpring.TrackDampingRatio(this.RotationTarget, p.RotationOscillationFrequency * MathUtil.TwoPi, p.RotationOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.RotationSpring.ValueVec = this.RotationTarget;
							this.RotationSpring.VelocityVec = Vector4.zero;
						}
						if (flag4)
						{
							switch (p.ScaleParameterMode)
							{
							case ParameterMode.Exponential:
								this.ScaleSpring.TrackExponential(this.ScaleTarget, p.ScaleExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.ScaleSpring.TrackHalfLife(this.ScaleTarget, p.ScaleOscillationFrequency, p.ScaleOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.ScaleSpring.TrackDampingRatio(this.ScaleTarget, p.ScaleOscillationFrequency * MathUtil.TwoPi, p.ScaleOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.ScaleSpring.Value = this.ScaleTarget;
							this.ScaleSpring.Velocity = Vector3.zero;
						}
					}
					if (!flag)
					{
						if (!flag2)
						{
							this.PositionSpring.Reset(this.PositionTarget);
						}
						if (!flag3)
						{
							this.RotationSpring.Reset(this.RotationTarget);
						}
					}
				}

				// Token: 0x06007206 RID: 29190 RVA: 0x00256E3C File Offset: 0x0025503C
				public void PullResults(BoingBones bones)
				{
					for (int i = 0; i < bones.BoneData.Length; i++)
					{
						BoingBones.Chain chain = bones.BoneChains[i];
						BoingBones.Bone[] array = bones.BoneData[i];
						if (array != null)
						{
							foreach (BoingBones.Bone bone in array)
							{
								bone.CachedPositionWs = bone.Position;
								bone.CachedPositionLs = bone.Transform.localPosition;
								bone.CachedRotationWs = bone.Rotation;
								bone.CachedRotationLs = bone.Transform.localRotation;
								bone.CachedScaleLs = bone.LocalScale;
							}
							for (int k = 0; k < array.Length; k++)
							{
								BoingBones.Bone bone2 = array[k];
								if (k == 0 && !chain.LooseRoot)
								{
									bone2.BlendedPositionWs = bone2.CachedPositionWs;
								}
								else
								{
									bone2.BlendedPositionWs = Vector3.Lerp(bone2.Instance.PositionSpring.Value, bone2.CachedPositionWs, bone2.AnimationBlend);
								}
							}
							for (int l = 0; l < array.Length; l++)
							{
								BoingBones.Bone bone3 = array[l];
								if (l == 0 && !chain.LooseRoot)
								{
									bone3.BlendedRotationWs = bone3.CachedRotationWs;
								}
								else if (bone3.ChildIndices == null)
								{
									if (bone3.ParentIndex >= 0)
									{
										BoingBones.Bone bone4 = array[bone3.ParentIndex];
										bone3.BlendedRotationWs = bone4.BlendedRotationWs * (bone4.RotationInverseWs * bone3.CachedRotationWs);
									}
								}
								else
								{
									Vector3 cachedPositionWs = bone3.CachedPositionWs;
									Vector3 vector = BoingWork.ComputeTranslationalResults(bone3.Transform, cachedPositionWs, bone3.BlendedPositionWs, bones);
									Quaternion quaternion = bones.TwistPropagation ? bone3.SpringRotationWs : bone3.CachedRotationWs;
									Quaternion quaternion2 = Quaternion.Inverse(quaternion);
									if (bones.EnableRotationEffect)
									{
										Vector4 vector2 = Vector3.zero;
										float num = 0f;
										foreach (int num2 in bone3.ChildIndices)
										{
											if (num2 >= 0)
											{
												BoingBones.Bone bone5 = array[num2];
												Vector3 cachedPositionWs2 = bone5.CachedPositionWs;
												Vector3 vector3 = VectorUtil.NormalizeSafe(cachedPositionWs2 - cachedPositionWs, Vector3.zero);
												Vector3 vector4 = VectorUtil.NormalizeSafe(BoingWork.ComputeTranslationalResults(bone5.Transform, cachedPositionWs2, bone5.BlendedPositionWs, bones) - vector, Vector3.zero);
												Quaternion quaternion3 = Quaternion.FromToRotation(vector3, vector4);
												Vector4 vector5 = QuaternionUtil.ToVector4(quaternion2 * quaternion3);
												float num3 = Mathf.Max(MathUtil.Epsilon, chain.MaxLengthFromRoot - bone5.LengthFromRoot);
												vector2 += num3 * vector5;
												num += num3;
											}
										}
										if (num > 0f)
										{
											Vector4 v = vector2 / num;
											bone3.RotationBackPropDeltaPs = QuaternionUtil.FromVector4(v, true);
											bone3.BlendedRotationWs = quaternion * bone3.RotationBackPropDeltaPs * quaternion;
										}
										else if (bone3.ParentIndex >= 0)
										{
											BoingBones.Bone bone6 = array[bone3.ParentIndex];
											bone3.BlendedRotationWs = bone6.BlendedRotationWs * (bone6.RotationInverseWs * quaternion);
										}
									}
								}
							}
							for (int m = 0; m < array.Length; m++)
							{
								BoingBones.Bone bone7 = array[m];
								if (m == 0 && !chain.LooseRoot)
								{
									bone7.Instance.PositionSpring.Reset(bone7.CachedPositionWs);
									bone7.Instance.RotationSpring.Reset(bone7.CachedRotationWs);
								}
								else
								{
									bone7.Transform.SetPositionAndRotation(BoingWork.ComputeTranslationalResults(bone7.Transform, bone7.Position, bone7.BlendedPositionWs, bones), bone7.BlendedRotationWs);
									bone7.Transform.localScale = bone7.BlendedScaleLs;
								}
							}
						}
					}
				}

				// Token: 0x06007207 RID: 29191 RVA: 0x0025721C File Offset: 0x0025541C
				private void SuppressWarnings()
				{
					this.m_padding0 = 0f;
					this.m_padding1 = 0f;
					this.m_padding2 = 0f;
					this.m_padding3 = 0;
					this.m_padding4 = 0;
					this.m_padding5 = 0f;
					this.m_padding0 = this.m_padding1;
					this.m_padding1 = this.m_padding2;
					this.m_padding2 = (float)this.m_padding3;
					this.m_padding3 = this.m_padding4;
					this.m_padding4 = (int)this.m_padding0;
					this.m_padding5 = this.m_padding0;
				}

				// Token: 0x0400828B RID: 33419
				public static readonly int Stride = 144 + 2 * Vector3Spring.Stride + QuaternionSpring.Stride;

				// Token: 0x0400828C RID: 33420
				public Vector3 PositionTarget;

				// Token: 0x0400828D RID: 33421
				private float m_padding0;

				// Token: 0x0400828E RID: 33422
				public Vector3 PositionOrigin;

				// Token: 0x0400828F RID: 33423
				private float m_padding1;

				// Token: 0x04008290 RID: 33424
				public Vector4 RotationTarget;

				// Token: 0x04008291 RID: 33425
				public Vector4 RotationOrigin;

				// Token: 0x04008292 RID: 33426
				public Vector3 ScaleTarget;

				// Token: 0x04008293 RID: 33427
				private float m_padding2;

				// Token: 0x04008294 RID: 33428
				private int m_numEffectors;

				// Token: 0x04008295 RID: 33429
				private int m_instantAccumulation;

				// Token: 0x04008296 RID: 33430
				private int m_padding3;

				// Token: 0x04008297 RID: 33431
				private int m_padding4;

				// Token: 0x04008298 RID: 33432
				private Vector3 m_upWs;

				// Token: 0x04008299 RID: 33433
				private float m_minScale;

				// Token: 0x0400829A RID: 33434
				public Vector3Spring PositionSpring;

				// Token: 0x0400829B RID: 33435
				public QuaternionSpring RotationSpring;

				// Token: 0x0400829C RID: 33436
				public Vector3Spring ScaleSpring;

				// Token: 0x0400829D RID: 33437
				public Vector3 PositionPropagationWorkData;

				// Token: 0x0400829E RID: 33438
				private float m_padding5;

				// Token: 0x0400829F RID: 33439
				public Vector4 RotationPropagationWorkData;
			}
		}

		// Token: 0x020011AC RID: 4524
		public struct Output
		{
			// Token: 0x06007209 RID: 29193 RVA: 0x002572C8 File Offset: 0x002554C8
			public Output(int instanceID, ref Vector3Spring positionSpring, ref QuaternionSpring rotationSpring, ref Vector3Spring scaleSpring)
			{
				this.InstanceID = instanceID;
				this.m_padding0 = (this.m_padding1 = (this.m_padding2 = 0));
				this.PositionSpring = positionSpring;
				this.RotationSpring = rotationSpring;
				this.ScaleSpring = scaleSpring;
			}

			// Token: 0x0600720A RID: 29194 RVA: 0x0025731C File Offset: 0x0025551C
			public void GatherOutput(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
			{
				BoingBehavior boingBehavior;
				if (!behaviorMap.TryGetValue(this.InstanceID, ref boingBehavior))
				{
					return;
				}
				if (!boingBehavior.isActiveAndEnabled)
				{
					return;
				}
				if (boingBehavior.UpdateMode != updateMode)
				{
					return;
				}
				boingBehavior.GatherOutput(ref this);
			}

			// Token: 0x0600720B RID: 29195 RVA: 0x00257354 File Offset: 0x00255554
			public void GatherOutput(Dictionary<int, BoingReactor> reactorMap, BoingManager.UpdateMode updateMode)
			{
				BoingReactor boingReactor;
				if (!reactorMap.TryGetValue(this.InstanceID, ref boingReactor))
				{
					return;
				}
				if (!boingReactor.isActiveAndEnabled)
				{
					return;
				}
				if (boingReactor.UpdateMode != updateMode)
				{
					return;
				}
				boingReactor.GatherOutput(ref this);
			}

			// Token: 0x0600720C RID: 29196 RVA: 0x0025738C File Offset: 0x0025558C
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding2;
				this.m_padding2 = this.m_padding0;
			}

			// Token: 0x040082A0 RID: 33440
			public static readonly int Stride = 16 + Vector3Spring.Stride + QuaternionSpring.Stride;

			// Token: 0x040082A1 RID: 33441
			public int InstanceID;

			// Token: 0x040082A2 RID: 33442
			public int m_padding0;

			// Token: 0x040082A3 RID: 33443
			public int m_padding1;

			// Token: 0x040082A4 RID: 33444
			public int m_padding2;

			// Token: 0x040082A5 RID: 33445
			public Vector3Spring PositionSpring;

			// Token: 0x040082A6 RID: 33446
			public QuaternionSpring RotationSpring;

			// Token: 0x040082A7 RID: 33447
			public Vector3Spring ScaleSpring;
		}
	}
}
