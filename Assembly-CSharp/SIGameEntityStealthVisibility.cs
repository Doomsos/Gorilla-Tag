using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class SIGameEntityStealthVisibility : MonoBehaviour
{
	// Token: 0x0600066B RID: 1643 RVA: 0x00024CD2 File Offset: 0x00022ED2
	private void OnEnable()
	{
		this.revealRange = Mathf.Min(this.revealRange, this.hideRange);
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00024CEB File Offset: 0x00022EEB
	private void OnDisable()
	{
		this.SetVisibility(true);
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00024CF4 File Offset: 0x00022EF4
	private void LateUpdate()
	{
		Vector3 position = GTPlayer.Instance.transform.position;
		float num = Vector3.SqrMagnitude(base.transform.position - position);
		if (this.isStealthed && num < this.revealRange * this.revealRange)
		{
			this.SetVisibility(true);
			return;
		}
		if (!this.isStealthed && num > this.hideRange * this.hideRange)
		{
			this.SetVisibility(false);
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00024D68 File Offset: 0x00022F68
	private void SetVisibility(bool visible)
	{
		this.isStealthed = !visible;
		for (int i = 0; i < this.stealthedComponents.Length; i++)
		{
			this.stealthedComponents[i].enabled = visible;
		}
	}

	// Token: 0x04000819 RID: 2073
	[SerializeField]
	private Renderer[] stealthedComponents;

	// Token: 0x0400081A RID: 2074
	[SerializeField]
	private float revealRange = 5f;

	// Token: 0x0400081B RID: 2075
	[SerializeField]
	private float hideRange = 8f;

	// Token: 0x0400081C RID: 2076
	private bool isStealthed;
}
