using System;
using UnityEngine;

// Token: 0x020004DB RID: 1243
[RequireComponent(typeof(SphereCollider))]
public class HapticsWithDistance : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06001FF5 RID: 8181 RVA: 0x000A9E47 File Offset: 0x000A8047
	private bool OnWrongLayer()
	{
		return base.gameObject.layer != 18;
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000A9E5B File Offset: 0x000A805B
	public void SetVibrationMult(float mult)
	{
		this.vibrationMult = mult;
	}

	// Token: 0x06001FF7 RID: 8183 RVA: 0x000A9E64 File Offset: 0x000A8064
	public void FingerFlexVibrationMult(bool dummy, float mult)
	{
		this.SetVibrationMult(mult);
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x000A9E6D File Offset: 0x000A806D
	private void Awake()
	{
		this.inverseColliderRadius = 1f / base.GetComponent<SphereCollider>().radius;
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x000A9E88 File Offset: 0x000A8088
	private void OnTriggerEnter(Collider other)
	{
		GorillaGrabber gorillaGrabber;
		if (other.TryGetComponent<GorillaGrabber>(ref gorillaGrabber) && gorillaGrabber.enabled)
		{
			if (gorillaGrabber.IsLeftHand)
			{
				this.leftOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
				return;
			}
			if (gorillaGrabber.IsRightHand)
			{
				this.rightOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
			}
		}
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x000A9EDC File Offset: 0x000A80DC
	private void OnTriggerExit(Collider other)
	{
		if (this.leftOfflineHand == other.transform)
		{
			this.leftOfflineHand = null;
			if (!this.rightOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
				return;
			}
		}
		else if (this.rightOfflineHand == other.transform)
		{
			this.rightOfflineHand = null;
			if (!this.leftOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x000A9F44 File Offset: 0x000A8144
	private void OnDisable()
	{
		this.leftOfflineHand = null;
		this.rightOfflineHand = null;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x06001FFC RID: 8188 RVA: 0x000A9F5A File Offset: 0x000A815A
	// (set) Token: 0x06001FFD RID: 8189 RVA: 0x000A9F62 File Offset: 0x000A8162
	public bool TickRunning { get; set; }

	// Token: 0x06001FFE RID: 8190 RVA: 0x000A9F6C File Offset: 0x000A816C
	public void Tick()
	{
		Vector3 position = base.transform.position;
		if (this.leftOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(true, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.leftOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
		if (this.rightOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(false, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.rightOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
	}

	// Token: 0x04002A50 RID: 10832
	[SerializeField]
	[Tooltip("X is the normalized distance and should start at 0 and end at 1. Y is the vibration amplitude and can be anywhere from 0-1.")]
	private AnimationCurve vibrationIntensityByDistance;

	// Token: 0x04002A51 RID: 10833
	private float inverseColliderRadius;

	// Token: 0x04002A52 RID: 10834
	private float vibrationMult = 1f;

	// Token: 0x04002A53 RID: 10835
	private Transform leftOfflineHand;

	// Token: 0x04002A54 RID: 10836
	private Transform rightOfflineHand;
}
