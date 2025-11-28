using System;
using UnityEngine;

// Token: 0x02000C8E RID: 3214
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x06004E8C RID: 20108 RVA: 0x00196EA5 File Offset: 0x001950A5
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x06004E8D RID: 20109 RVA: 0x00196ED4 File Offset: 0x001950D4
	private void Update()
	{
		Vector3 vector = this.x0z(base.transform.forward);
		Vector3 vector2 = this.x0z(this.referenceTransform.forward);
		float num = Vector3.Dot(vector, vector2);
		UIMatchRotation.State state = this.state;
		if (state != UIMatchRotation.State.Ready)
		{
			if (state != UIMatchRotation.State.Rotating)
			{
				return;
			}
			base.transform.forward = Vector3.Lerp(base.transform.forward, vector2, Time.deltaTime * this.lerpSpeed);
			if (Vector3.Dot(base.transform.forward, vector2) > 0.995f)
			{
				this.state = UIMatchRotation.State.Ready;
			}
		}
		else if (num < 1f - this.threshold)
		{
			this.state = UIMatchRotation.State.Rotating;
			return;
		}
	}

	// Token: 0x06004E8E RID: 20110 RVA: 0x00196F78 File Offset: 0x00195178
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x04005D6E RID: 23918
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x04005D6F RID: 23919
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x04005D70 RID: 23920
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x04005D71 RID: 23921
	private UIMatchRotation.State state;

	// Token: 0x02000C8F RID: 3215
	private enum State
	{
		// Token: 0x04005D73 RID: 23923
		Ready,
		// Token: 0x04005D74 RID: 23924
		Rotating
	}
}
