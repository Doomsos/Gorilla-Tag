using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000520 RID: 1312
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x06002157 RID: 8535 RVA: 0x000AF49D File Offset: 0x000AD69D
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000AF4B0 File Offset: 0x000AD6B0
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x00002789 File Offset: 0x00000989
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x04002BE3 RID: 11235
	public GameObject handToStickTo;

	// Token: 0x04002BE4 RID: 11236
	public bool isLeftHand;

	// Token: 0x04002BE5 RID: 11237
	public float hapticStrength;

	// Token: 0x04002BE6 RID: 11238
	private Rigidbody thisRigidbody;

	// Token: 0x04002BE7 RID: 11239
	private XRController controllerReference;
}
