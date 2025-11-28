using System;
using System.Collections;
using UnityEngine;

// Token: 0x020008E2 RID: 2274
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x06003A3A RID: 14906 RVA: 0x00133A0B File Offset: 0x00131C0B
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x00133A36 File Offset: 0x00131C36
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x0000528D File Offset: 0x0000348D
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x00133A45 File Offset: 0x00131C45
	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	// Token: 0x0400497A RID: 18810
	private Camera myCamera;

	// Token: 0x0400497B RID: 18811
	private float frameRate;

	// Token: 0x0400497C RID: 18812
	private float timeToNextFrame;
}
