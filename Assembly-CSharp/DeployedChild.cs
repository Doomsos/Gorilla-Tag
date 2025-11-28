using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class DeployedChild : MonoBehaviour
{
	// Token: 0x06000F4B RID: 3915 RVA: 0x00051510 File Offset: 0x0004F710
	public void Deploy(DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this._parent = parent;
		this._parent.DeployChild();
		Transform transform = base.transform;
		transform.position = launchPos;
		transform.rotation = launchRot;
		transform.localScale = this._parent.transform.lossyScale;
		this._rigidbody.linearVelocity = releaseVel;
		this._isRemote = isRemote;
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x0005156D File Offset: 0x0004F76D
	public void ReturnToParent(float delay)
	{
		if (delay > 0f)
		{
			base.StartCoroutine(this.ReturnToParentDelayed(delay));
			return;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x0005159F File Offset: 0x0004F79F
	private IEnumerator ReturnToParentDelayed(float delay)
	{
		float start = Time.time;
		while (Time.time < start + delay)
		{
			yield return null;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
		yield break;
	}

	// Token: 0x040012D4 RID: 4820
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040012D5 RID: 4821
	[SerializeReference]
	private DeployableObject _parent;

	// Token: 0x040012D6 RID: 4822
	private bool _isRemote;
}
