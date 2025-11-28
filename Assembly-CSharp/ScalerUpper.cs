using System;
using UnityEngine;

// Token: 0x0200043F RID: 1087
public class ScalerUpper : MonoBehaviour
{
	// Token: 0x06001ABF RID: 6847 RVA: 0x0008D4E8 File Offset: 0x0008B6E8
	private void Update()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one * this.scaleCurve.Evaluate(this.t);
		}
		this.t += Time.deltaTime;
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x0008D547 File Offset: 0x0008B747
	private void OnEnable()
	{
		this.t = 0f;
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x0008D554 File Offset: 0x0008B754
	private void OnDisable()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one;
		}
	}

	// Token: 0x04002434 RID: 9268
	[SerializeField]
	private Transform[] target;

	// Token: 0x04002435 RID: 9269
	[SerializeField]
	private AnimationCurve scaleCurve;

	// Token: 0x04002436 RID: 9270
	private float t;
}
