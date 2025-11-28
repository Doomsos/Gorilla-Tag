using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005D4 RID: 1492
public class Decelerate : MonoBehaviour
{
	// Token: 0x060025B1 RID: 9649 RVA: 0x000C96D4 File Offset: 0x000C78D4
	public void Restart()
	{
		base.enabled = true;
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000C96E0 File Offset: 0x000C78E0
	private void Update()
	{
		if (!this._rigidbody)
		{
			return;
		}
		Vector3 vector = this._rigidbody.linearVelocity;
		vector *= this._friction;
		if (vector.Approx0(0.001f))
		{
			this._rigidbody.linearVelocity = Vector3.zero;
			UnityEvent unityEvent = this.onStop;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			base.enabled = false;
		}
		else
		{
			this._rigidbody.linearVelocity = vector;
		}
		if (this._resetOrientationOnRelease && !this._rigidbody.rotation.Approx(Quaternion.identity, 1E-06f))
		{
			this._rigidbody.rotation = Quaternion.identity;
		}
	}

	// Token: 0x0400314B RID: 12619
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x0400314C RID: 12620
	[SerializeField]
	private float _friction = 0.875f;

	// Token: 0x0400314D RID: 12621
	[SerializeField]
	private bool _resetOrientationOnRelease;

	// Token: 0x0400314E RID: 12622
	public UnityEvent onStop;
}
