using System;
using System.Collections;
using UnityEngine;

// Token: 0x020008E0 RID: 2272
public class SodaBubble : MonoBehaviour
{
	// Token: 0x06003A31 RID: 14897 RVA: 0x0013390B File Offset: 0x00131B0B
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x0013391A File Offset: 0x00131B1A
	private IEnumerator PopCoroutine()
	{
		this.audioSource.GTPlay();
		this.bubbleMesh.gameObject.SetActive(false);
		this.bubbleCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.bubbleMesh.gameObject.SetActive(true);
		this.bubbleCollider.gameObject.SetActive(true);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04004973 RID: 18803
	public MeshRenderer bubbleMesh;

	// Token: 0x04004974 RID: 18804
	public Rigidbody body;

	// Token: 0x04004975 RID: 18805
	public MeshCollider bubbleCollider;

	// Token: 0x04004976 RID: 18806
	public AudioSource audioSource;
}
