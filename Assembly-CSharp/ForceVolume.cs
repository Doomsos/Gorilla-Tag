using System;
using GorillaExtensions;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000B64 RID: 2916
public class ForceVolume : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060047CE RID: 18382 RVA: 0x00179F5C File Offset: 0x0017815C
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x060047CF RID: 18383 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060047D0 RID: 18384 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060047D1 RID: 18385 RVA: 0x00179F74 File Offset: 0x00178174
	public void SliceUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x060047D2 RID: 18386 RVA: 0x00179FC4 File Offset: 0x001781C4
	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		return rb != null && xf != null;
	}

	// Token: 0x060047D3 RID: 18387 RVA: 0x0017A024 File Offset: 0x00178224
	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.enterClip == null)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.enterClip, 1f);
			this.audioState = ForceVolume.AudioState.Enter;
		}
		this.enterPos = transform.position;
	}

	// Token: 0x060047D4 RID: 18388 RVA: 0x0017A094 File Offset: 0x00178294
	public void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			this.audioState = ForceVolume.AudioState.None;
		}
	}

	// Token: 0x060047D5 RID: 18389 RVA: 0x0017A0EC File Offset: 0x001782EC
	public void OnTriggerStay(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource && !this.audioSource.isPlaying)
		{
			ForceVolume.AudioState audioState = this.audioState;
			if (audioState != ForceVolume.AudioState.Enter)
			{
				if (audioState == ForceVolume.AudioState.Loop)
				{
					if (this.loopClip != null)
					{
						this.audioSource.enabled = true;
						this.audioSource.GTPlayOneShot(this.loopClip, 1f);
					}
					this.audioState = ForceVolume.AudioState.Loop;
				}
			}
			else
			{
				if (this.loopCresendoClip != null)
				{
					this.audioSource.enabled = true;
					this.audioSource.GTPlayOneShot(this.loopCresendoClip, 1f);
				}
				this.audioState = ForceVolume.AudioState.Crescendo;
			}
		}
		if (this.disableGrip)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		VRRig.LocalRig.BreakHandLinks();
		SizeManager sizeManager = null;
		if (this.scaleWithSize)
		{
			sizeManager = rigidbody.GetComponent<SizeManager>();
		}
		Vector3 vector = rigidbody.linearVelocity;
		if (this.scaleWithSize && sizeManager)
		{
			vector /= sizeManager.currentScale;
		}
		Vector3 vector2 = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 vector3 = base.transform.position + vector2 - transform.position;
		float num = vector3.magnitude + 0.0001f;
		Vector3 vector4 = vector3 / num;
		float num2 = Vector3.Dot(vector, vector4);
		float num3 = this.accel;
		if (this.maxDepth > -1f)
		{
			float num4 = Vector3.Dot(transform.position - this.enterPos, vector4);
			float num5 = this.maxDepth - num4;
			float num6 = 0f;
			if (num5 > 0.0001f)
			{
				num6 = num2 * num2 / num5;
			}
			num3 = Mathf.Max(this.accel, num6);
		}
		float deltaTime = Time.deltaTime;
		Vector3 vector5 = base.transform.up * num3 * deltaTime;
		vector += vector5;
		Vector3 vector6 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
		Vector3 vector7 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
		Vector3 vector8 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
		float num7 = 1f;
		float num8 = 1f;
		if (this.dampenLateralVelocity)
		{
			num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
			num8 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
		}
		vector = vector6 + num7 * vector7 + num8 * vector8;
		if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
		{
			vector -= num2 * vector4;
			if (num > this.pullTOCenterMinDistance)
			{
				num2 += this.pullToCenterAccel * deltaTime;
				float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, num9);
			}
			else
			{
				num2 = 0f;
			}
			vector += num2 * vector4;
			if (vector.magnitude > 0.0001f)
			{
				Vector3 vector9 = Vector3.Cross(base.transform.up, vector4);
				float magnitude = vector9.magnitude;
				if (magnitude > 0.0001f)
				{
					vector9 /= magnitude;
					num2 = Vector3.Dot(vector, vector9);
					vector -= num2 * vector9;
					num2 -= this.pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					vector += num2 * vector9;
				}
			}
		}
		if (this.scaleWithSize && sizeManager)
		{
			vector *= sizeManager.currentScale;
		}
		rigidbody.linearVelocity = vector;
	}

	// Token: 0x060047D6 RID: 18390 RVA: 0x0017A510 File Offset: 0x00178710
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x060047D7 RID: 18391 RVA: 0x0017A580 File Offset: 0x00178780
	public void SetPropertiesFromPlaceholder(ForceVolumeProperties properties, AudioSource volumeAudioSource, Collider colliderVolume)
	{
		this.accel = properties.accel;
		this.maxDepth = properties.maxDepth;
		this.maxSpeed = properties.maxSpeed;
		this.disableGrip = properties.disableGrip;
		this.dampenLateralVelocity = properties.dampenLateralVelocity;
		this.dampenXVelPerc = properties.dampenXVel;
		this.dampenZVelPerc = properties.dampenZVel;
		this.applyPullToCenterAcceleration = properties.applyPullToCenterAcceleration;
		this.pullToCenterAccel = properties.pullToCenterAccel;
		this.pullToCenterMaxSpeed = properties.pullToCenterMaxSpeed;
		this.pullTOCenterMinDistance = properties.pullToCenterMinDistance;
		this.enterClip = properties.enterClip;
		this.exitClip = properties.exitClip;
		this.loopClip = properties.loopClip;
		this.loopCresendoClip = properties.loopCrescendoClip;
		if (volumeAudioSource.IsNotNull())
		{
			this.audioSource = volumeAudioSource;
		}
		if (colliderVolume.IsNotNull())
		{
			this.volume = colliderVolume;
		}
	}

	// Token: 0x04005885 RID: 22661
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04005886 RID: 22662
	[SerializeField]
	private float accel;

	// Token: 0x04005887 RID: 22663
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x04005888 RID: 22664
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04005889 RID: 22665
	[SerializeField]
	private bool disableGrip;

	// Token: 0x0400588A RID: 22666
	[SerializeField]
	private bool dampenLateralVelocity = true;

	// Token: 0x0400588B RID: 22667
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x0400588C RID: 22668
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x0400588D RID: 22669
	[SerializeField]
	private bool applyPullToCenterAcceleration = true;

	// Token: 0x0400588E RID: 22670
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x0400588F RID: 22671
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x04005890 RID: 22672
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x04005891 RID: 22673
	private Collider volume;

	// Token: 0x04005892 RID: 22674
	public AudioClip enterClip;

	// Token: 0x04005893 RID: 22675
	public AudioClip exitClip;

	// Token: 0x04005894 RID: 22676
	public AudioClip loopClip;

	// Token: 0x04005895 RID: 22677
	public AudioClip loopCresendoClip;

	// Token: 0x04005896 RID: 22678
	public AudioSource audioSource;

	// Token: 0x04005897 RID: 22679
	private Vector3 enterPos;

	// Token: 0x04005898 RID: 22680
	private ForceVolume.AudioState audioState;

	// Token: 0x02000B65 RID: 2917
	private enum AudioState
	{
		// Token: 0x0400589A RID: 22682
		None,
		// Token: 0x0400589B RID: 22683
		Enter,
		// Token: 0x0400589C RID: 22684
		Crescendo,
		// Token: 0x0400589D RID: 22685
		Loop,
		// Token: 0x0400589E RID: 22686
		Exit
	}
}
