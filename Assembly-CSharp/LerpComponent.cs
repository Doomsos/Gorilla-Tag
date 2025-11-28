using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020009E0 RID: 2528
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x170005F6 RID: 1526
	// (get) Token: 0x06004076 RID: 16502 RVA: 0x001594C4 File Offset: 0x001576C4
	// (set) Token: 0x06004077 RID: 16503 RVA: 0x001594CC File Offset: 0x001576CC
	public float Lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this._lerp, num))
			{
				LerpChangedEvent onLerpChanged = this._onLerpChanged;
				if (onLerpChanged != null)
				{
					onLerpChanged.Invoke(num);
				}
			}
			this._lerp = num;
		}
	}

	// Token: 0x170005F7 RID: 1527
	// (get) Token: 0x06004078 RID: 16504 RVA: 0x00159507 File Offset: 0x00157707
	// (set) Token: 0x06004079 RID: 16505 RVA: 0x0015950F File Offset: 0x0015770F
	public float LerpTime
	{
		get
		{
			return this._lerpLength;
		}
		set
		{
			this._lerpLength = ((value < 0f) ? 0f : value);
		}
	}

	// Token: 0x170005F8 RID: 1528
	// (get) Token: 0x0600407A RID: 16506 RVA: 0x00027DED File Offset: 0x00025FED
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600407B RID: 16507
	protected abstract void OnLerp(float t);

	// Token: 0x0600407C RID: 16508 RVA: 0x00159527 File Offset: 0x00157727
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x0600407D RID: 16509 RVA: 0x00159538 File Offset: 0x00157738
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x0600407E RID: 16510 RVA: 0x00159563 File Offset: 0x00157763
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x0600407F RID: 16511 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x06004080 RID: 16512 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x06004081 RID: 16513 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x06004082 RID: 16514 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x06004083 RID: 16515 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x0400518F RID: 20879
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x04005190 RID: 20880
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x04005191 RID: 20881
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x04005192 RID: 20882
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x04005193 RID: 20883
	[NonSerialized]
	private bool _previewing;

	// Token: 0x04005194 RID: 20884
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x04005195 RID: 20885
	[NonSerialized]
	private bool _rendering;

	// Token: 0x04005196 RID: 20886
	[NonSerialized]
	private int _lastState;

	// Token: 0x04005197 RID: 20887
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x04005198 RID: 20888
	[NonSerialized]
	private float _prevLerpTo;
}
