using System;
using UnityEngine;

// Token: 0x0200077B RID: 1915
public class GorillaEyeExpressions : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060031DB RID: 12763 RVA: 0x0010E31D File Offset: 0x0010C51D
	private void Awake()
	{
		this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x0010E32B File Offset: 0x0010C52B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060031DE RID: 12766 RVA: 0x0010E34A File Offset: 0x0010C54A
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.CheckEyeEffects();
		this.UpdateEyeExpression();
	}

	// Token: 0x060031DF RID: 12767 RVA: 0x0010E378 File Offset: 0x0010C578
	private void CheckEyeEffects()
	{
		if (this.loudness == null)
		{
			this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
		}
		if (this.loudness.IsSpeaking && this.loudness.Loudness > this.screamVolume)
		{
			this.overrideDuration = this.screamDuration;
			this.overrideUV = this.ScreamUV;
			return;
		}
		if (this.overrideDuration > 0f)
		{
			this.overrideDuration -= this.deltaTime;
			if (this.overrideDuration <= 0f)
			{
				this.overrideUV = this.BaseUV;
			}
		}
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x0010E414 File Offset: 0x0010C614
	private void UpdateEyeExpression()
	{
		this.targetFace.GetComponent<Renderer>().material.SetVector(this._BaseMap_ST, new Vector4(0.5f, 1f, this.overrideUV.x, this.overrideUV.y));
	}

	// Token: 0x0400406C RID: 16492
	public GameObject targetFace;

	// Token: 0x0400406D RID: 16493
	[Space]
	[SerializeField]
	private float screamVolume = 0.2f;

	// Token: 0x0400406E RID: 16494
	[SerializeField]
	private float screamDuration = 0.5f;

	// Token: 0x0400406F RID: 16495
	[SerializeField]
	private Vector2 ScreamUV = new Vector2(0.8f, 0f);

	// Token: 0x04004070 RID: 16496
	private Vector2 BaseUV = Vector3.zero;

	// Token: 0x04004071 RID: 16497
	private GorillaSpeakerLoudness loudness;

	// Token: 0x04004072 RID: 16498
	private float overrideDuration;

	// Token: 0x04004073 RID: 16499
	private Vector2 overrideUV;

	// Token: 0x04004074 RID: 16500
	private float timeLastUpdated;

	// Token: 0x04004075 RID: 16501
	private float deltaTime;

	// Token: 0x04004076 RID: 16502
	private ShaderHashId _BaseMap_ST = "_BaseMap_ST";
}
