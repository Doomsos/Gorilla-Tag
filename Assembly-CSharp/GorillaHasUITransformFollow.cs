using System;
using UnityEngine;

// Token: 0x02000789 RID: 1929
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x06003291 RID: 12945 RVA: 0x0011079C File Offset: 0x0010E99C
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x06003292 RID: 12946 RVA: 0x001107D8 File Offset: 0x0010E9D8
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x00110808 File Offset: 0x0010EA08
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x00110838 File Offset: 0x0010EA38
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x040040E5 RID: 16613
	public GorillaUITransformFollow[] transformFollowers;
}
