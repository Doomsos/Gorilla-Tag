using System;
using PerformanceSystems;
using UnityEngine;

// Token: 0x02000C1D RID: 3101
public class DemoSpheresRotate : TimeSliceLodBehaviour
{
	// Token: 0x06004C49 RID: 19529 RVA: 0x0018D099 File Offset: 0x0018B299
	public void OnLod0Enter()
	{
		this._renderer.material = this._red;
		this.SwapToTimeSlicer(0);
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C4A RID: 19530 RVA: 0x0018D0BF File Offset: 0x0018B2BF
	public void OnLod1Enter()
	{
		this._renderer.material = this._green;
		this.SwapToTimeSlicer(1);
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C4B RID: 19531 RVA: 0x0018D0E5 File Offset: 0x0018B2E5
	public void OnLod2Enter()
	{
		this._renderer.material = this._black;
		this.SwapToTimeSlicer(2);
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C4C RID: 19532 RVA: 0x000396A0 File Offset: 0x000378A0
	public void OnLodExit()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004C4D RID: 19533 RVA: 0x0018D10B File Offset: 0x0018B30B
	public override void SliceUpdate(float deltaTime)
	{
		base.transform.Rotate(Vector3.up * this._rotationSpeed * deltaTime);
	}

	// Token: 0x06004C4E RID: 19534 RVA: 0x0018D12E File Offset: 0x0018B32E
	private void SwapToTimeSlicer(int index)
	{
		if (this._timeSliceControllerAssets[index] == this._timeSliceControllerAsset)
		{
			return;
		}
		this._timeSliceControllerAsset.RemoveTimeSliceBehaviour(this);
		this._timeSliceControllerAsset = this._timeSliceControllerAssets[index];
		this._timeSliceControllerAsset.AddTimeSliceBehaviour(this);
	}

	// Token: 0x04005C3B RID: 23611
	[SerializeField]
	private TimeSliceControllerAsset[] _timeSliceControllerAssets;

	// Token: 0x04005C3C RID: 23612
	[SerializeField]
	private float _rotationSpeed = 10f;

	// Token: 0x04005C3D RID: 23613
	[SerializeField]
	private Material _red;

	// Token: 0x04005C3E RID: 23614
	[SerializeField]
	private Material _green;

	// Token: 0x04005C3F RID: 23615
	[SerializeField]
	private Material _black;

	// Token: 0x04005C40 RID: 23616
	[SerializeField]
	private Renderer _renderer;
}
