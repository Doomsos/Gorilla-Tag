using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000938 RID: 2360
public class CustomMapsGorillaRopeSwing : GorillaRopeSwing
{
	// Token: 0x06003C48 RID: 15432 RVA: 0x0013E7C6 File Offset: 0x0013C9C6
	protected override void Awake()
	{
		base.CalculateId(true);
		base.StartCoroutine(this.WaitForRopeLength());
	}

	// Token: 0x06003C49 RID: 15433 RVA: 0x00002789 File Offset: 0x00000989
	protected override void Start()
	{
	}

	// Token: 0x06003C4A RID: 15434 RVA: 0x0013E7DC File Offset: 0x0013C9DC
	protected override void OnEnable()
	{
		if (!this.isRopeLengthSet)
		{
			return;
		}
		base.OnEnable();
	}

	// Token: 0x06003C4B RID: 15435 RVA: 0x0013E7ED File Offset: 0x0013C9ED
	public void SetRopeLength(int length)
	{
		this.ropeLength = length;
		this.isRopeLengthSet = true;
	}

	// Token: 0x06003C4C RID: 15436 RVA: 0x0013E800 File Offset: 0x0013CA00
	public void SetRopeProperties(GTObjectPlaceholder placeholder)
	{
		this.ropePlaceholder = placeholder;
		this.ropeLength = this.ropePlaceholder.ropeLength;
		this.ropeBitGenOffset = this.ropePlaceholder.ropeSegmentGenerationOffset;
		this.preExistingSegments = this.ropePlaceholder.ropeSwingSegments;
		this.ropeScale = this.ropePlaceholder.transform.localScale;
		base.transform.localScale = Vector3.one;
		this.isRopeLengthSet = true;
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x0013E874 File Offset: 0x0013CA74
	private IEnumerator WaitForRopeLength()
	{
		while (!this.isRopeLengthSet)
		{
			yield return null;
		}
		this.RopeGeneration();
		base.Awake();
		base.OnEnable();
		base.Start();
		yield break;
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x0013E884 File Offset: 0x0013CA84
	private void RopeGeneration()
	{
		List<Transform> list = new List<Transform>();
		if (this.preExistingSegments != null && this.preExistingSegments.Count > 0)
		{
			for (int i = 0; i < this.preExistingSegments.Count; i++)
			{
				this.preExistingSegments[i].transform.SetParent(base.transform);
				GorillaClimbable gorillaClimbable = this.preExistingSegments[i].AddComponent<GorillaClimbable>();
				gorillaClimbable.snapX = this.snapX;
				gorillaClimbable.snapY = this.snapY;
				gorillaClimbable.snapZ = this.snapZ;
				gorillaClimbable.maxDistanceSnap = this.maxDistanceSnap;
				gorillaClimbable.clip = this.onGrabSFX;
				gorillaClimbable.clipOnFullRelease = this.OnReleaseSFX;
				GorillaRopeSegment gorillaRopeSegment = this.preExistingSegments[i].AddComponent<GorillaRopeSegment>();
				gorillaRopeSegment.swing = this;
				gorillaRopeSegment.boneIndex = this.preExistingSegments[i].boneIndex;
				list.Add(this.preExistingSegments[i].transform);
			}
			base.transform.localScale = this.ropeScale;
			this.ropePlaceholder.transform.localScale = Vector3.one;
		}
		else
		{
			Vector3 vector = Vector3.zero;
			float y = this.prefabRopeBit.GetComponentInChildren<Renderer>().bounds.size.y;
			WaterVolume[] array = Object.FindObjectsByType<WaterVolume>(0);
			List<Collider> list2 = new List<Collider>(array.Length);
			WaterVolume[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				foreach (Collider collider in array2[j].volumeColliders)
				{
					if (!(collider == null))
					{
						list2.Add(collider);
					}
				}
			}
			for (int k = 0; k < this.ropeLength + 1; k++)
			{
				bool flag = false;
				if (list2.Count > 0)
				{
					Collider collider2 = list2[0];
					if (collider2 != null)
					{
						Vector3 vector2 = base.transform.position + vector;
						Vector3 vector3 = vector2 + new Vector3(0f, -y, 0f);
						flag = (collider2.bounds.Contains(vector2) || collider2.bounds.Contains(vector3));
					}
				}
				GameObject gameObject = Object.Instantiate<GameObject>(flag ? this.partiallyUnderwaterPrefab : this.prefabRopeBit, base.transform);
				gameObject.name = string.Format("RopeBone_{0:00}", k);
				gameObject.transform.localPosition = vector;
				gameObject.transform.localRotation = Quaternion.identity;
				vector += new Vector3(0f, -this.ropeBitGenOffset, 0f);
				GorillaRopeSegment component = gameObject.GetComponent<GorillaRopeSegment>();
				component.swing = this;
				component.boneIndex = k;
				list.Add(gameObject.transform);
			}
			list[0].GetComponent<BoxCollider>().center = new Vector3(0f, -0.65f, 0f);
			list[0].GetComponent<BoxCollider>().size = new Vector3(0.3f, 0.65f, 0.3f);
		}
		if (list.Count > 0)
		{
			Enumerable.Last<Transform>(list).gameObject.SetActive(false);
		}
		this.nodes = list.ToArray();
	}

	// Token: 0x04004CF8 RID: 19704
	[SerializeField]
	private GameObject partiallyUnderwaterPrefab;

	// Token: 0x04004CF9 RID: 19705
	private bool isRopeLengthSet;

	// Token: 0x04004CFA RID: 19706
	private List<RopeSwingSegment> preExistingSegments;

	// Token: 0x04004CFB RID: 19707
	private GTObjectPlaceholder ropePlaceholder;

	// Token: 0x04004CFC RID: 19708
	private Vector3 ropeScale = Vector3.one;

	// Token: 0x04004CFD RID: 19709
	public bool snapX;

	// Token: 0x04004CFE RID: 19710
	public bool snapY;

	// Token: 0x04004CFF RID: 19711
	public bool snapZ;

	// Token: 0x04004D00 RID: 19712
	public float maxDistanceSnap = 0.05f;

	// Token: 0x04004D01 RID: 19713
	public AudioClip onGrabSFX;

	// Token: 0x04004D02 RID: 19714
	public AudioClip OnReleaseSFX;
}
