using System;
using UnityEngine;

// Token: 0x020005DB RID: 1499
public class FingerFlexReactor : MonoBehaviour
{
	// Token: 0x060025C2 RID: 9666 RVA: 0x000C9A7C File Offset: 0x000C7C7C
	private void Setup()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (!this._rig)
		{
			return;
		}
		this._fingers = new VRMap[]
		{
			this._rig.leftThumb,
			this._rig.leftIndex,
			this._rig.leftMiddle,
			this._rig.rightThumb,
			this._rig.rightIndex,
			this._rig.rightMiddle
		};
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000C9B03 File Offset: 0x000C7D03
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000C9B0B File Offset: 0x000C7D0B
	private void FixedUpdate()
	{
		this.UpdateBlendShapes();
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000C9B14 File Offset: 0x000C7D14
	public void UpdateBlendShapes()
	{
		if (!this._rig)
		{
			return;
		}
		if (this._blendShapeTargets == null || this._fingers == null)
		{
			return;
		}
		if (this._blendShapeTargets.Length == 0 || this._fingers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._blendShapeTargets.Length; i++)
		{
			FingerFlexReactor.BlendShapeTarget blendShapeTarget = this._blendShapeTargets[i];
			if (blendShapeTarget != null)
			{
				int sourceFinger = (int)blendShapeTarget.sourceFinger;
				if (sourceFinger != -1)
				{
					SkinnedMeshRenderer targetRenderer = blendShapeTarget.targetRenderer;
					if (targetRenderer)
					{
						float lerpValue = FingerFlexReactor.GetLerpValue(this._fingers[sourceFinger]);
						Vector2 inputRange = blendShapeTarget.inputRange;
						Vector2 outputRange = blendShapeTarget.outputRange;
						float num = MathUtils.Linear(lerpValue, inputRange.x, inputRange.y, outputRange.x, outputRange.y);
						blendShapeTarget.currentValue = num;
						targetRenderer.SetBlendShapeWeight(blendShapeTarget.blendShapeIndex, num);
					}
				}
			}
		}
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000C9BE8 File Offset: 0x000C7DE8
	private static float GetLerpValue(VRMap map)
	{
		VRMapThumb vrmapThumb = map as VRMapThumb;
		float result;
		if (vrmapThumb == null)
		{
			VRMapIndex vrmapIndex = map as VRMapIndex;
			if (vrmapIndex == null)
			{
				VRMapMiddle vrmapMiddle = map as VRMapMiddle;
				if (vrmapMiddle == null)
				{
					result = 0f;
				}
				else
				{
					result = vrmapMiddle.calcT;
				}
			}
			else
			{
				result = vrmapIndex.calcT;
			}
		}
		else
		{
			result = ((vrmapThumb.calcT > 0.1f) ? 1f : 0f);
		}
		return result;
	}

	// Token: 0x04003170 RID: 12656
	[SerializeField]
	private VRRig _rig;

	// Token: 0x04003171 RID: 12657
	[SerializeField]
	private VRMap[] _fingers = new VRMap[0];

	// Token: 0x04003172 RID: 12658
	[SerializeField]
	private FingerFlexReactor.BlendShapeTarget[] _blendShapeTargets = new FingerFlexReactor.BlendShapeTarget[0];

	// Token: 0x020005DC RID: 1500
	[Serializable]
	public class BlendShapeTarget
	{
		// Token: 0x04003173 RID: 12659
		public FingerFlexReactor.FingerMap sourceFinger;

		// Token: 0x04003174 RID: 12660
		public SkinnedMeshRenderer targetRenderer;

		// Token: 0x04003175 RID: 12661
		public int blendShapeIndex;

		// Token: 0x04003176 RID: 12662
		public Vector2 inputRange = new Vector2(0f, 1f);

		// Token: 0x04003177 RID: 12663
		public Vector2 outputRange = new Vector2(0f, 1f);

		// Token: 0x04003178 RID: 12664
		[NonSerialized]
		public float currentValue;
	}

	// Token: 0x020005DD RID: 1501
	public enum FingerMap
	{
		// Token: 0x0400317A RID: 12666
		None = -1,
		// Token: 0x0400317B RID: 12667
		LeftThumb,
		// Token: 0x0400317C RID: 12668
		LeftIndex,
		// Token: 0x0400317D RID: 12669
		LeftMiddle,
		// Token: 0x0400317E RID: 12670
		RightThumb,
		// Token: 0x0400317F RID: 12671
		RightIndex,
		// Token: 0x04003180 RID: 12672
		RightMiddle
	}
}
