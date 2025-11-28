using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class ParachuteProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x0600101C RID: 4124 RVA: 0x00054BC2 File Offset: 0x00052DC2
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x00054BD0 File Offset: 0x00052DD0
	private void OnEnable()
	{
		this.launched = false;
		this.landTime = 0f;
		this.launchedTime = 0f;
		this.peakTime = float.MaxValue;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (!this.TickRunning)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x00054C30 File Offset: 0x00052E30
	private void OnDisable()
	{
		this.launched = false;
		if (this.TickRunning)
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x00054C48 File Offset: 0x00052E48
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		this.parachuteDeployed = false;
		this.landed = false;
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.ChangeUp(Vector3.up);
		this.rb.freezeRotation = true;
		if (ownerRig == null)
		{
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		}
		this.rb.isKinematic = false;
		this.rb.linearVelocity = velocity;
		this.rb.linearDamping = this.initialDrag;
		this.rb.angularDamping = this.initialAngularDrag;
		this.launchedTime = Time.time;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (velocity.y > 0f)
		{
			this.peakTime = velocity.y / (-1f * Physics.gravity.y);
		}
		else
		{
			this.peakTime = 0f;
		}
		this.launched = true;
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x00054D84 File Offset: 0x00052F84
	private void OnPeakReached()
	{
		this.parachuteDeployed = true;
		this.parachute.SetActive(true);
		this.monkeMeshFilter.mesh = this.parachutingMesh;
		this.ChangeUp(Vector3.up);
		this.rb.linearDamping = this.parachuteDrag;
		this.rb.angularDamping = this.parachuteAngularDrag;
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00054DE4 File Offset: 0x00052FE4
	private void OnLanded(Collision collision)
	{
		this.landTime = Time.time;
		this.landed = true;
		ContactPoint contact = collision.GetContact(0);
		this.rb.isKinematic = true;
		this.rb.position = contact.point + contact.normal * (this.groundOffset * base.transform.localScale.x);
		this.ChangeUp(contact.normal);
		this.monkeMeshFilter.mesh = this.landedMesh;
		this.parachute.SetActive(false);
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x00054E7C File Offset: 0x0005307C
	private void ChangeUp(Vector3 newUp)
	{
		Vector3 vector = Vector3.Cross(this.rb.transform.right, newUp);
		if (vector.sqrMagnitude < 1E-45f)
		{
			vector = Vector3.Cross(Vector3.Cross(newUp, this.rb.transform.forward), newUp);
		}
		this.rb.rotation = Quaternion.LookRotation(vector, newUp);
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x00054EE0 File Offset: 0x000530E0
	private void PlayImpactEffects(Vector3 position, Vector3 normal)
	{
		if (this.impactEffect != null)
		{
			Vector3 position2 = position + this.impactEffectOffset * normal;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.impactEffect, position2, true);
			gameObject.transform.localScale = base.transform.localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
		}
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x00054F5C File Offset: 0x0005315C
	public void OnTriggerEvent(bool isLeft, Collider col)
	{
		if (this.parachuteDeployed)
		{
			this.PlayImpactEffects(base.transform.position, Vector3.up);
			GorillaTriggerColliderHandIndicator componentInParent = col.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent != null)
			{
				float amplitude = GorillaTagger.Instance.tapHapticStrength / 2f;
				float fixedDeltaTime = Time.fixedDeltaTime;
				GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, amplitude, fixedDeltaTime);
			}
		}
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00054FC0 File Offset: 0x000531C0
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.launched || this.landed)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		if (collision.collider.attachedRigidbody != null)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (!this.parachuteDeployed)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (Vector3.Angle(contact.normal, Vector3.up) < this.groudUpThreshold)
		{
			this.OnLanded(collision);
			return;
		}
		this.PlayImpactEffects(contact.point, contact.normal);
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06001026 RID: 4134 RVA: 0x00055089 File Offset: 0x00053289
	// (set) Token: 0x06001027 RID: 4135 RVA: 0x00055091 File Offset: 0x00053291
	public bool TickRunning { get; set; }

	// Token: 0x06001028 RID: 4136 RVA: 0x0005509C File Offset: 0x0005329C
	public void Tick()
	{
		if (!this.parachuteDeployed && Time.time > this.launchedTime + this.parachuteDeployDelay && Time.time >= this.launchedTime + this.peakTime)
		{
			this.OnPeakReached();
		}
		if (this.landed && Time.time > this.landTime + this.destroyOnLandDelay)
		{
			this.PlayImpactEffects(base.transform.position, base.transform.up);
		}
	}

	// Token: 0x04001415 RID: 5141
	[SerializeField]
	private MeshFilter monkeMeshFilter;

	// Token: 0x04001416 RID: 5142
	[SerializeField]
	private GameObject parachute;

	// Token: 0x04001417 RID: 5143
	[SerializeField]
	private Mesh launchMesh;

	// Token: 0x04001418 RID: 5144
	[SerializeField]
	private Mesh parachutingMesh;

	// Token: 0x04001419 RID: 5145
	[SerializeField]
	private Mesh landedMesh;

	// Token: 0x0400141A RID: 5146
	[Tooltip("time to wait after launch before deploying the parachute")]
	[SerializeField]
	private float parachuteDeployDelay = 1f;

	// Token: 0x0400141B RID: 5147
	[Tooltip("time to wait after landing before destroying")]
	[SerializeField]
	private float destroyOnLandDelay = 3f;

	// Token: 0x0400141C RID: 5148
	[Tooltip("How far from the collision point should the projectile sit when landed")]
	[SerializeField]
	private float groundOffset;

	// Token: 0x0400141D RID: 5149
	[Tooltip("Acceptable angle in degrees of surface from world up to be considered the ground")]
	[SerializeField]
	private float groudUpThreshold = 45f;

	// Token: 0x0400141E RID: 5150
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialDrag;

	// Token: 0x0400141F RID: 5151
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialAngularDrag = 0.05f;

	// Token: 0x04001420 RID: 5152
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteDrag = 5f;

	// Token: 0x04001421 RID: 5153
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteAngularDrag = 10f;

	// Token: 0x04001422 RID: 5154
	[SerializeField]
	private GameObject impactEffect;

	// Token: 0x04001423 RID: 5155
	[SerializeField]
	private float impactEffectScaleMultiplier = 1f;

	// Token: 0x04001424 RID: 5156
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x04001425 RID: 5157
	private Rigidbody rb;

	// Token: 0x04001426 RID: 5158
	private bool launched;

	// Token: 0x04001427 RID: 5159
	private float launchedTime;

	// Token: 0x04001428 RID: 5160
	private float landTime;

	// Token: 0x04001429 RID: 5161
	private float peakTime = float.MaxValue;

	// Token: 0x0400142A RID: 5162
	private bool parachuteDeployed;

	// Token: 0x0400142B RID: 5163
	private bool landed;
}
