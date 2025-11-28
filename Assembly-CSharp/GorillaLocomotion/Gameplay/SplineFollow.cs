using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA4 RID: 4004
	public sealed class SplineFollow : MonoBehaviour
	{
		// Token: 0x06006496 RID: 25750 RVA: 0x0020CE9C File Offset: 0x0020B09C
		public void Start()
		{
			base.transform.rotation *= this._rotationFix;
			this._smoothRotationTrackingRateExp = Mathf.Exp(this._smoothRotationTrackingRate);
			this._progress = this._splineProgressOffset;
			this._progressPerFixedUpdate = Time.fixedDeltaTime / this._duration;
			this._secondsToCycles = (double)(1f / this._duration);
			this._nativeSpline = new NativeSpline(this._unitySpline.Spline, this._unitySpline.transform.localToWorldMatrix, 4);
			if (this._approximate)
			{
				this.CalculateApproximationNodes();
			}
		}

		// Token: 0x06006497 RID: 25751 RVA: 0x0020CF44 File Offset: 0x0020B144
		private void CalculateApproximationNodes()
		{
			for (int i = 0; i < this._approximationResolution; i++)
			{
				float3 @float;
				float3 float2;
				float3 float3;
				SplineUtility.Evaluate<NativeSpline>(this._nativeSpline, (float)i / (float)this._approximationResolution, ref @float, ref float2, ref float3);
				SplineFollow.SplineNode splineNode = new SplineFollow.SplineNode(@float, float2, float3);
				this._approximationNodes.Add(splineNode);
			}
			if (this._nativeSpline.Closed)
			{
				this._approximationNodes.Add(this._approximationNodes[0]);
			}
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x0020CFC8 File Offset: 0x0020B1C8
		private void FixedUpdate()
		{
			if (!this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x06006499 RID: 25753 RVA: 0x0020CFD8 File Offset: 0x0020B1D8
		private void Update()
		{
			if (this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x0020CFE8 File Offset: 0x0020B1E8
		private void FollowSpline()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this._secondsToCycles + (double)this._splineProgressOffset;
				this._progress = (float)(num % 1.0);
			}
			else
			{
				this._progress = (this._progress + this._progressPerFixedUpdate) % 1f;
			}
			SplineFollow.SplineNode splineNode = this.EvaluateSpline(this._progress);
			base.transform.position = splineNode.Position;
			Quaternion quaternion = Quaternion.LookRotation(splineNode.Tangent) * this._rotationFix;
			base.transform.rotation = Quaternion.Slerp(quaternion, base.transform.rotation, Mathf.Exp(-this._smoothRotationTrackingRateExp * Time.deltaTime));
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x0020D0A4 File Offset: 0x0020B2A4
		private SplineFollow.SplineNode EvaluateSpline(float t)
		{
			t %= 1f;
			if (this._approximate)
			{
				float num = t * (float)this._approximationNodes.Count;
				int num2 = (int)num;
				float t2 = num - (float)num2;
				num2 %= this._approximationNodes.Count;
				SplineFollow.SplineNode a = this._approximationNodes[num2];
				SplineFollow.SplineNode b = this._approximationNodes[(num2 + 1) % this._approximationNodes.Count];
				return SplineFollow.SplineNode.Lerp(a, b, t2);
			}
			float3 @float;
			float3 float2;
			float3 float3;
			SplineUtility.Evaluate<NativeSpline>(this._nativeSpline, t, ref @float, ref float2, ref float3);
			return new SplineFollow.SplineNode(@float, float2, float3);
		}

		// Token: 0x0600649C RID: 25756 RVA: 0x0020D140 File Offset: 0x0020B340
		private void OnDestroy()
		{
			this._nativeSpline.Dispose();
		}

		// Token: 0x0400743E RID: 29758
		[SerializeField]
		[Tooltip("If true, approximates the spline position. Only use when exact position does not matter.")]
		private bool _approximate;

		// Token: 0x0400743F RID: 29759
		[SerializeField]
		private SplineContainer _unitySpline;

		// Token: 0x04007440 RID: 29760
		[SerializeField]
		private float _duration;

		// Token: 0x04007441 RID: 29761
		private double _secondsToCycles;

		// Token: 0x04007442 RID: 29762
		[SerializeField]
		private float _smoothRotationTrackingRate = 0.5f;

		// Token: 0x04007443 RID: 29763
		private float _smoothRotationTrackingRateExp;

		// Token: 0x04007444 RID: 29764
		private float _progressPerFixedUpdate;

		// Token: 0x04007445 RID: 29765
		[SerializeField]
		private float _splineProgressOffset;

		// Token: 0x04007446 RID: 29766
		[SerializeField]
		private Quaternion _rotationFix = Quaternion.identity;

		// Token: 0x04007447 RID: 29767
		private NativeSpline _nativeSpline;

		// Token: 0x04007448 RID: 29768
		private float _progress;

		// Token: 0x04007449 RID: 29769
		[Header("Approximate Spline Parameters")]
		[SerializeField]
		[Range(4f, 200f)]
		private int _approximationResolution = 100;

		// Token: 0x0400744A RID: 29770
		private readonly List<SplineFollow.SplineNode> _approximationNodes = new List<SplineFollow.SplineNode>();

		// Token: 0x02000FA5 RID: 4005
		private struct SplineNode
		{
			// Token: 0x0600649E RID: 25758 RVA: 0x0020D180 File Offset: 0x0020B380
			public SplineNode(Vector3 position, Vector3 tangent, Vector3 up)
			{
				this.Position = position;
				this.Tangent = tangent;
				this.Up = up;
			}

			// Token: 0x0600649F RID: 25759 RVA: 0x0020D1A8 File Offset: 0x0020B3A8
			public static SplineFollow.SplineNode Lerp(SplineFollow.SplineNode a, SplineFollow.SplineNode b, float t)
			{
				return new SplineFollow.SplineNode(Vector3.Lerp(a.Position, b.Position, t), Vector3.Lerp(a.Tangent, b.Tangent, t), Vector3.Lerp(a.Up, b.Up, t));
			}

			// Token: 0x0400744B RID: 29771
			public readonly Vector3 Position;

			// Token: 0x0400744C RID: 29772
			public readonly Vector3 Tangent;

			// Token: 0x0400744D RID: 29773
			public readonly Vector3 Up;
		}
	}
}
