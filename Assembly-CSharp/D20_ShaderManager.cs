using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class D20_ShaderManager : MonoBehaviour
{
	// Token: 0x06000BFC RID: 3068 RVA: 0x00040AE8 File Offset: 0x0003ECE8
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00040B4D File Offset: 0x0003ED4D
	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	// Token: 0x04000EA9 RID: 3753
	private Rigidbody rb;

	// Token: 0x04000EAA RID: 3754
	private Vector3 lastPosition;

	// Token: 0x04000EAB RID: 3755
	public float updateInterval = 0.1f;

	// Token: 0x04000EAC RID: 3756
	public Vector3 velocity;

	// Token: 0x04000EAD RID: 3757
	private Material material;
}
