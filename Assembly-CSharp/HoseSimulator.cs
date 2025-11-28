using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class HoseSimulator : MonoBehaviour, ISpawnable
{
	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x00053FB0 File Offset: 0x000521B0
	// (set) Token: 0x06000FF5 RID: 4085 RVA: 0x00053FB8 File Offset: 0x000521B8
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x00053FC1 File Offset: 0x000521C1
	// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x00053FC9 File Offset: 0x000521C9
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x00053FD4 File Offset: 0x000521D4
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchors = rig.cosmeticReferences.Get(this.startAnchorRef).GetComponent<HoseSimulatorAnchors>();
		if (this.skinnedMeshRenderer != null)
		{
			Bounds localBounds = this.skinnedMeshRenderer.localBounds;
			localBounds.extents = this.localBoundsOverride;
			this.skinnedMeshRenderer.localBounds = localBounds;
		}
		this.hoseSectionLengths = new float[this.hoseBones.Length - 1];
		this.hoseBonePositions = new Vector3[this.hoseBones.Length];
		this.hoseBoneVelocities = new Vector3[this.hoseBones.Length];
		for (int i = 0; i < this.hoseSectionLengths.Length; i++)
		{
			float num = 1f;
			this.hoseSectionLengths[i] = num;
			this.totalHoseLength += num;
		}
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0005409C File Offset: 0x0005229C
	private void LateUpdate()
	{
		if (this.myHoldable.InLeftHand())
		{
			this.isLeftHanded = true;
		}
		else if (this.myHoldable.InRightHand())
		{
			this.isLeftHanded = false;
		}
		for (int i = 0; i < this.miscBones.Length; i++)
		{
			Transform transform = this.isLeftHanded ? this.anchors.miscAnchorsLeft[i] : this.anchors.miscAnchorsRight[i];
			this.miscBones[i].transform.position = transform.position;
			this.miscBones[i].transform.rotation = transform.rotation;
		}
		this.startAnchor = (this.isLeftHanded ? this.anchors.leftAnchorPoint : this.anchors.rightAnchorPoint);
		float x = this.myHoldable.transform.lossyScale.x;
		float num = 0f;
		Vector3 position = this.startAnchor.position;
		Vector3 ctrl = position + this.startAnchor.forward * this.startStiffness * x;
		Vector3 position2 = this.endAnchor.position;
		Vector3 ctrl2 = position2 - this.endAnchor.forward * this.endStiffness * x;
		for (int j = 0; j < this.hoseBones.Length; j++)
		{
			float num2 = num / this.totalHoseLength;
			Vector3 vector = BezierUtils.BezierSolve(num2, position, ctrl, ctrl2, position2);
			Vector3 vector2 = BezierUtils.BezierSolve(num2 + 0.1f, position, ctrl, ctrl2, position2);
			if (this.firstUpdate)
			{
				this.hoseBones[j].transform.position = vector;
				this.hoseBonePositions[j] = vector;
				this.hoseBoneVelocities[j] = Vector3.zero;
			}
			else
			{
				this.hoseBoneVelocities[j] *= this.damping;
				this.hoseBonePositions[j] += this.hoseBoneVelocities[j] * Time.deltaTime;
				float num3 = this.hoseBoneMaxDisplacement[j] * x;
				if ((vector - this.hoseBonePositions[j]).IsLongerThan(num3))
				{
					Vector3 vector3 = vector + (this.hoseBonePositions[j] - vector).normalized * num3;
					this.hoseBoneVelocities[j] += (vector3 - this.hoseBonePositions[j]) / Time.deltaTime;
					this.hoseBonePositions[j] = vector3;
				}
				this.hoseBones[j].transform.position = this.hoseBonePositions[j];
			}
			this.hoseBones[j].transform.rotation = Quaternion.LookRotation(vector2 - vector, this.endAnchor.transform.up);
			if (j < this.hoseSectionLengths.Length)
			{
				num += this.hoseSectionLengths[j];
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x000543DA File Offset: 0x000525DA
	private void OnDrawGizmosSelected()
	{
		if (this.hoseBonePositions != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLineStrip(this.hoseBonePositions, false);
		}
	}

	// Token: 0x040013D8 RID: 5080
	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x040013D9 RID: 5081
	[SerializeField]
	private Vector3 localBoundsOverride;

	// Token: 0x040013DA RID: 5082
	[SerializeField]
	private Transform[] miscBones;

	// Token: 0x040013DB RID: 5083
	[SerializeField]
	private Transform[] hoseBones;

	// Token: 0x040013DC RID: 5084
	[SerializeField]
	private float[] hoseBoneMaxDisplacement;

	// Token: 0x040013DD RID: 5085
	[SerializeField]
	private CosmeticRefID startAnchorRef;

	// Token: 0x040013DE RID: 5086
	private Transform startAnchor;

	// Token: 0x040013DF RID: 5087
	[SerializeField]
	private float startStiffness = 0.5f;

	// Token: 0x040013E0 RID: 5088
	[SerializeField]
	private Transform endAnchor;

	// Token: 0x040013E1 RID: 5089
	[SerializeField]
	private float endStiffness = 0.5f;

	// Token: 0x040013E2 RID: 5090
	private Vector3[] hoseBonePositions;

	// Token: 0x040013E3 RID: 5091
	private Vector3[] hoseBoneVelocities;

	// Token: 0x040013E4 RID: 5092
	[SerializeField]
	private float damping = 0.97f;

	// Token: 0x040013E5 RID: 5093
	private float[] hoseSectionLengths;

	// Token: 0x040013E6 RID: 5094
	private float totalHoseLength;

	// Token: 0x040013E7 RID: 5095
	private bool firstUpdate = true;

	// Token: 0x040013E8 RID: 5096
	private HoseSimulatorAnchors anchors;

	// Token: 0x040013E9 RID: 5097
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x040013EA RID: 5098
	private bool isLeftHanded;
}
