using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class BalloonDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x06001939 RID: 6457 RVA: 0x00087210 File Offset: 0x00085410
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x0008725C File Offset: 0x0008545C
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x00087288 File Offset: 0x00085488
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000872D8 File Offset: 0x000854D8
	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float num3 = this.bouyancyForce * num2 * this.balloonScale;
		this.rb.AddForce(new Vector3(0f, num3, 0f) * this.rb.mass, 0);
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x00087350 File Offset: 0x00085550
	private void ApplyUpRightForce()
	{
		Vector3 vector = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque * this.balloonScale;
		this.rb.AddTorque(vector);
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x00087398 File Offset: 0x00085598
	private void ApplyAntiSpinForce()
	{
		Vector3 vector = this.rb.transform.InverseTransformDirection(this.rb.angularVelocity);
		this.rb.AddRelativeTorque(0f, -vector.y * this.antiSpinTorque, 0f);
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x000873E4 File Offset: 0x000855E4
	private void ApplyAirResistance()
	{
		this.rb.linearVelocity *= 1f - this.airResistance;
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x00087408 File Offset: 0x00085608
	private void ApplyDistanceConstraint()
	{
		this.knot.transform.position - base.transform.position;
		Vector3 vector = this.grabPt.transform.position - this.knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = this.stringLength * this.balloonScale;
		if (magnitude > num)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.linearVelocity, normalized) * normalized;
			float num2 = magnitude - num;
			float num3 = num2 / Time.fixedDeltaTime;
			if (vector2.magnitude < num3)
			{
				float num4 = num3 - vector2.magnitude;
				float num5 = Mathf.Clamp01(num2 / this.stringStretch);
				Vector3 vector3 = Mathf.Lerp(0f, num4, num5 * num5) * normalized * this.stringStrength;
				this.rb.AddForceAtPosition(vector3 * this.rb.mass, this.knot.transform.position, 1);
			}
		}
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x00087524 File Offset: 0x00085724
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		bool flag = !this.enableDynamics && enable;
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!kinematic && flag)
			{
				this.rb.linearVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x0008759F File Offset: 0x0008579F
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.enableDistanceConstraints = enable;
		this.balloonScale = scale;
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06001943 RID: 6467 RVA: 0x000875AF File Offset: 0x000857AF
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x000875CC File Offset: 0x000857CC
	private void FixedUpdate()
	{
		if (this.enableDynamics && !this.rb.isKinematic)
		{
			this.ApplyBouyancyForce();
			if (this.antiSpinTorque > 0f)
			{
				this.ApplyAntiSpinForce();
			}
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			this.rb.linearVelocity = linearVelocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
		}
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000029BC File Offset: 0x00000BBC
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x0008765F File Offset: 0x0008585F
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x00087668 File Offset: 0x00085868
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		if (!other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
		{
			return;
		}
		if (!this.rb)
		{
			return;
		}
		transferOwnership = true;
		TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
		if (!component)
		{
			return;
		}
		Vector3 vector = (component.transform.position - component.prevPos) / Time.deltaTime;
		force = vector * this.bopSpeed;
		force = Mathf.Min(this.maximumVelocity, force.magnitude) * force.normalized * this.balloonScale;
		if (this.bopSpeedCap > 0f && force.IsLongerThan(this.bopSpeedCap))
		{
			force = force.normalized * this.bopSpeedCap;
		}
		collisionPt = other.ClosestPointOnBounds(base.transform.position);
		this.rb.AddForceAtPosition(force * this.rb.mass, collisionPt, 1);
		if (this.balloonBopSource != null)
		{
			this.balloonBopSource.GTPlay();
		}
		GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component2 != null)
		{
			float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, amplitude, fixedDeltaTime);
		}
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x00027DED File Offset: 0x00025FED
	public bool ReturnStep()
	{
		return true;
	}

	// Token: 0x04002299 RID: 8857
	private Rigidbody rb;

	// Token: 0x0400229A RID: 8858
	private Collider balloonCollider;

	// Token: 0x0400229B RID: 8859
	private Bounds bounds;

	// Token: 0x0400229C RID: 8860
	public float bouyancyForce = 1f;

	// Token: 0x0400229D RID: 8861
	public float bouyancyMinHeight = 10f;

	// Token: 0x0400229E RID: 8862
	public float bouyancyMaxHeight = 20f;

	// Token: 0x0400229F RID: 8863
	private float bouyancyActualHeight = 20f;

	// Token: 0x040022A0 RID: 8864
	public float varianceMaxheight = 5f;

	// Token: 0x040022A1 RID: 8865
	public float airResistance = 0.01f;

	// Token: 0x040022A2 RID: 8866
	public GameObject knot;

	// Token: 0x040022A3 RID: 8867
	private Rigidbody knotRb;

	// Token: 0x040022A4 RID: 8868
	public Transform grabPt;

	// Token: 0x040022A5 RID: 8869
	private Transform grabPtInitParent;

	// Token: 0x040022A6 RID: 8870
	public float stringLength = 2f;

	// Token: 0x040022A7 RID: 8871
	public float stringStrength = 0.9f;

	// Token: 0x040022A8 RID: 8872
	public float stringStretch = 0.1f;

	// Token: 0x040022A9 RID: 8873
	public float maximumVelocity = 2f;

	// Token: 0x040022AA RID: 8874
	public float upRightTorque = 1f;

	// Token: 0x040022AB RID: 8875
	public float antiSpinTorque;

	// Token: 0x040022AC RID: 8876
	private bool enableDynamics;

	// Token: 0x040022AD RID: 8877
	private bool enableDistanceConstraints;

	// Token: 0x040022AE RID: 8878
	public float balloonScale = 1f;

	// Token: 0x040022AF RID: 8879
	public float bopSpeed = 1f;

	// Token: 0x040022B0 RID: 8880
	public float bopSpeedCap;

	// Token: 0x040022B1 RID: 8881
	[SerializeField]
	private AudioSource balloonBopSource;
}
