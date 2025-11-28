using System;
using UnityEngine;

// Token: 0x02000446 RID: 1094
public class TransformReset : MonoBehaviour
{
	// Token: 0x06001AD8 RID: 6872 RVA: 0x0008DA78 File Offset: 0x0008BC78
	private void Awake()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.transformList = new TransformReset.OriginalGameObjectTransform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.transformList[i] = new TransformReset.OriginalGameObjectTransform(componentsInChildren[i]);
		}
		this.ResetTransforms();
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x0008DAC4 File Offset: 0x0008BCC4
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x0008DB14 File Offset: 0x0008BD14
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x0008DB58 File Offset: 0x0008BD58
	public void ResetTransforms()
	{
		this.tempTransformList = new TransformReset.OriginalGameObjectTransform[this.transformList.Length];
		for (int i = 0; i < this.transformList.Length; i++)
		{
			this.tempTransformList[i] = new TransformReset.OriginalGameObjectTransform(this.transformList[i].thisTransform);
		}
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x04002463 RID: 9315
	private TransformReset.OriginalGameObjectTransform[] transformList;

	// Token: 0x04002464 RID: 9316
	private TransformReset.OriginalGameObjectTransform[] tempTransformList;

	// Token: 0x02000447 RID: 1095
	private struct OriginalGameObjectTransform
	{
		// Token: 0x06001ADD RID: 6877 RVA: 0x0008DBF0 File Offset: 0x0008BDF0
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x0008DC11 File Offset: 0x0008BE11
		// (set) Token: 0x06001ADF RID: 6879 RVA: 0x0008DC19 File Offset: 0x0008BE19
		public Transform thisTransform
		{
			get
			{
				return this._thisTransform;
			}
			set
			{
				this._thisTransform = value;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x0008DC22 File Offset: 0x0008BE22
		// (set) Token: 0x06001AE1 RID: 6881 RVA: 0x0008DC2A File Offset: 0x0008BE2A
		public Vector3 thisPosition
		{
			get
			{
				return this._thisPosition;
			}
			set
			{
				this._thisPosition = value;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06001AE2 RID: 6882 RVA: 0x0008DC33 File Offset: 0x0008BE33
		// (set) Token: 0x06001AE3 RID: 6883 RVA: 0x0008DC3B File Offset: 0x0008BE3B
		public Quaternion thisRotation
		{
			get
			{
				return this._thisRotation;
			}
			set
			{
				this._thisRotation = value;
			}
		}

		// Token: 0x04002465 RID: 9317
		private Transform _thisTransform;

		// Token: 0x04002466 RID: 9318
		private Vector3 _thisPosition;

		// Token: 0x04002467 RID: 9319
		private Quaternion _thisRotation;
	}
}
