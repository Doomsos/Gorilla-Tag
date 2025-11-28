using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200037C RID: 892
[ExecuteInEditMode]
public class LckRawImageFillCanvas : UIBehaviour
{
	// Token: 0x06001529 RID: 5417 RVA: 0x00077FB6 File Offset: 0x000761B6
	private void OnEnable()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x00077FB6 File Offset: 0x000761B6
	private void Update()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x00077FC0 File Offset: 0x000761C0
	private void UpdateSizeDelta()
	{
		if (this._rawImage == null || this._rawImage.texture == null)
		{
			return;
		}
		RectTransform rectTransform = this._rawImage.rectTransform;
		Vector2 sizeDelta = ((RectTransform)rectTransform.parent).sizeDelta;
		Vector2 vector;
		vector..ctor((float)this._rawImage.texture.width, (float)this._rawImage.texture.height);
		float num = sizeDelta.x / sizeDelta.y;
		float num2 = vector.x / vector.y;
		float num3 = num / num2;
		Vector2 vector2;
		vector2..ctor(sizeDelta.x, sizeDelta.x / num2);
		Vector2 vector3;
		vector3..ctor(sizeDelta.y * num2, sizeDelta.y);
		switch (this._scaleType)
		{
		case LckRawImageFillCanvas.ScaleType.Fill:
			rectTransform.sizeDelta = ((num3 > 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Inset:
			rectTransform.sizeDelta = ((num3 < 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Stretch:
			rectTransform.sizeDelta = sizeDelta;
			return;
		default:
			return;
		}
	}

	// Token: 0x04001FB2 RID: 8114
	[SerializeField]
	private RawImage _rawImage;

	// Token: 0x04001FB3 RID: 8115
	[SerializeField]
	private LckRawImageFillCanvas.ScaleType _scaleType;

	// Token: 0x0200037D RID: 893
	private enum ScaleType
	{
		// Token: 0x04001FB5 RID: 8117
		Fill,
		// Token: 0x04001FB6 RID: 8118
		Inset,
		// Token: 0x04001FB7 RID: 8119
		Stretch
	}
}
