using System;
using BoingKit;
using UnityEngine;

// Token: 0x020008EE RID: 2286
public class SyncRigidBodyToMovement : MonoBehaviour
{
	// Token: 0x06003A82 RID: 14978 RVA: 0x0013543A File Offset: 0x0013363A
	private void Awake()
	{
		this.targetParent = this.targetRigidbody.transform.parent;
		this.targetRigidbody.transform.parent = null;
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x00135474 File Offset: 0x00133674
	private void OnEnable()
	{
		this.targetRigidbody.gameObject.SetActive(true);
		this.targetRigidbody.transform.position = base.transform.position;
		this.targetRigidbody.transform.rotation = base.transform.rotation;
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x001354C8 File Offset: 0x001336C8
	private void OnDisable()
	{
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06003A85 RID: 14981 RVA: 0x001354DC File Offset: 0x001336DC
	private void FixedUpdate()
	{
		this.targetRigidbody.linearVelocity = (base.transform.position - this.targetRigidbody.position) / Time.fixedDeltaTime;
		this.targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(this.targetRigidbody.rotation) * base.transform.rotation) / Time.fixedDeltaTime;
	}

	// Token: 0x040049D4 RID: 18900
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x040049D5 RID: 18901
	private Transform targetParent;
}
