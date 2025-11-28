using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class HideFirstFrame : MonoBehaviour
{
	// Token: 0x06000095 RID: 149 RVA: 0x0000501E File Offset: 0x0000321E
	private void Awake()
	{
		this._cam = base.GetComponent<Camera>();
		this._farClipPlane = this._cam.farClipPlane;
		this._cam.farClipPlane = this._cam.nearClipPlane + 0.1f;
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00005059 File Offset: 0x00003259
	public IEnumerator Start()
	{
		int num;
		for (int i = 0; i < this._frameDelay; i = num + 1)
		{
			yield return null;
			num = i;
		}
		this._cam.farClipPlane = this._farClipPlane;
		yield break;
	}

	// Token: 0x040000B6 RID: 182
	[SerializeField]
	private int _frameDelay = 1;

	// Token: 0x040000B7 RID: 183
	private Camera _cam;

	// Token: 0x040000B8 RID: 184
	private float _farClipPlane;
}
