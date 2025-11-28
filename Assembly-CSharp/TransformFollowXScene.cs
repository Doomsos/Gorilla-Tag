using System;
using UnityEngine;

// Token: 0x020008FB RID: 2299
public class TransformFollowXScene : MonoBehaviour
{
	// Token: 0x06003AC4 RID: 15044 RVA: 0x0013647F File Offset: 0x0013467F
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06003AC5 RID: 15045 RVA: 0x00136492 File Offset: 0x00134692
	private void Start()
	{
		this.refToFollow.TryResolve<Transform>(out this.transformToFollow);
	}

	// Token: 0x06003AC6 RID: 15046 RVA: 0x001364A8 File Offset: 0x001346A8
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04004A2A RID: 18986
	public XSceneRef refToFollow;

	// Token: 0x04004A2B RID: 18987
	private Transform transformToFollow;

	// Token: 0x04004A2C RID: 18988
	public Vector3 offset;

	// Token: 0x04004A2D RID: 18989
	public Vector3 prevPos;
}
