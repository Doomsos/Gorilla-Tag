using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000851 RID: 2129
public class MouthReactorCosmetic : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06003816 RID: 14358 RVA: 0x0012CC15 File Offset: 0x0012AE15
	private void ResetReactorTransform()
	{
		if (this.reactorTransform == null)
		{
			this.reactorTransform = base.transform;
		}
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x0012CC31 File Offset: 0x0012AE31
	private void ResetRadius()
	{
		this.reactorRadius = 0.1666667f;
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x06003818 RID: 14360 RVA: 0x0012CC3E File Offset: 0x0012AE3E
	private bool IsRadiusChanged
	{
		get
		{
			return this.reactorRadius != 0.1666667f;
		}
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x0012CC50 File Offset: 0x0012AE50
	private void ResetOffset()
	{
		this.mouthOffset = MouthReactorCosmetic.DEFAULT_OFFSET;
	}

	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x0600381A RID: 14362 RVA: 0x0012CC5D File Offset: 0x0012AE5D
	private bool IsOffsetChanged
	{
		get
		{
			return this.mouthOffset != MouthReactorCosmetic.DEFAULT_OFFSET;
		}
	}

	// Token: 0x0600381B RID: 14363 RVA: 0x0012CC6F File Offset: 0x0012AE6F
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600381C RID: 14364 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x0600381D RID: 14365 RVA: 0x0012CC8B File Offset: 0x0012AE8B
	// (set) Token: 0x0600381E RID: 14366 RVA: 0x0012CC93 File Offset: 0x0012AE93
	public bool TickRunning { get; set; }

	// Token: 0x0600381F RID: 14367 RVA: 0x0012CC9C File Offset: 0x0012AE9C
	public void Tick()
	{
		Vector3 vector = this.myRig.head.rigTarget.TransformPoint(this.mouthOffset);
		float sqrMagnitude = (this.reactorTransform.TransformPoint(this.reactorOffset) - vector).sqrMagnitude;
		if (sqrMagnitude < this.reactorRadius * this.reactorRadius)
		{
			if ((!this.mustExitBeforeRefire || !this.wasInside) && Time.time - this.lastInsideTime >= this.eventRefireDelay)
			{
				UnityEvent unityEvent = this.onInsideMouth;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this.lastInsideTime = Time.time;
			}
			this.wasInside = true;
		}
		else
		{
			this.wasInside = false;
		}
		if (this.continuousProperties.Count > 0)
		{
			this.continuousProperties.ApplyAll(Mathf.Min(0f, Mathf.Sqrt(sqrMagnitude) - this.reactorRadius));
		}
	}

	// Token: 0x04004739 RID: 18233
	private static readonly Vector3 DEFAULT_OFFSET = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x0400473A RID: 18234
	private const float DEFAULT_RADIUS = 0.1666667f;

	// Token: 0x0400473B RID: 18235
	[Tooltip("The transform to check against the mouth's position. Defaults to the transform this script is attached to.")]
	public Transform reactorTransform;

	// Token: 0x0400473C RID: 18236
	[Tooltip("Offset the relative position of the reactor transform.")]
	public Vector3 reactorOffset = Vector3.zero;

	// Token: 0x0400473D RID: 18237
	[Tooltip("How close the reactor needs to be to the mouth to trigger the event.")]
	public float reactorRadius = 0.1666667f;

	// Token: 0x0400473E RID: 18238
	[Tooltip("The continuous value is the distance to the mouth. When inside the mouth radius, the value will always be 0.")]
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x0400473F RID: 18239
	[Tooltip("After the event fires, it must wait this many seconds before it fires again.")]
	public float eventRefireDelay = 0.6f;

	// Token: 0x04004740 RID: 18240
	[Tooltip("After the event fires, prevent firing again until the reactor transform is moved outside the mouth and then back in.")]
	public bool mustExitBeforeRefire = true;

	// Token: 0x04004741 RID: 18241
	public UnityEvent onInsideMouth;

	// Token: 0x04004742 RID: 18242
	public Vector3 mouthOffset = MouthReactorCosmetic.DEFAULT_OFFSET;

	// Token: 0x04004743 RID: 18243
	private VRRig myRig;

	// Token: 0x04004744 RID: 18244
	private float lastInsideTime;

	// Token: 0x04004745 RID: 18245
	private bool wasInside;
}
