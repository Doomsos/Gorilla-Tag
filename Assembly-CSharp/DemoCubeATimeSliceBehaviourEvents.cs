using System;
using PerformanceSystems;
using UnityEngine;

// Token: 0x02000C1C RID: 3100
public class DemoCubeATimeSliceBehaviourEvents : TimeSliceLodBehaviour
{
	// Token: 0x06004C43 RID: 19523 RVA: 0x0018D024 File Offset: 0x0018B224
	protected new void Awake()
	{
		base.Awake();
		this._renderer = base.GetComponent<Renderer>();
	}

	// Token: 0x06004C44 RID: 19524 RVA: 0x0018D038 File Offset: 0x0018B238
	public override void SliceUpdate(float deltaTime)
	{
		float num = 0f;
		for (int i = 0; i < this._iterationsOfExpensiveOp; i++)
		{
			num += Mathf.Sqrt((float)i * deltaTime);
		}
	}

	// Token: 0x06004C45 RID: 19525 RVA: 0x0018D068 File Offset: 0x0018B268
	public void OnLod0Enter()
	{
		this._renderer.material = this._red;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C46 RID: 19526 RVA: 0x0018D087 File Offset: 0x0018B287
	public void OnLod1Enter()
	{
		this._renderer.material = this._green;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C47 RID: 19527 RVA: 0x000396A0 File Offset: 0x000378A0
	public void OnLodExit()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04005C37 RID: 23607
	[SerializeField]
	private int _iterationsOfExpensiveOp = 200;

	// Token: 0x04005C38 RID: 23608
	[SerializeField]
	private Material _red;

	// Token: 0x04005C39 RID: 23609
	[SerializeField]
	private Material _green;

	// Token: 0x04005C3A RID: 23610
	private Renderer _renderer;
}
