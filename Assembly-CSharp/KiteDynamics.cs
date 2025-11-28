using System;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class KiteDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x060019CC RID: 6604 RVA: 0x00089BAC File Offset: 0x00087DAC
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtPosition = this.grabPt.position;
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x00089C09 File Offset: 0x00087E09
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x00089C34 File Offset: 0x00087E34
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x00089C84 File Offset: 0x00087E84
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!enable)
			{
				this.rb.linearVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x00089CEE File Offset: 0x00087EEE
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.rb.useGravity = !enable;
		this.balloonScale = scale;
		this.grabPtPosition = this.grabPt.position;
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x060019D1 RID: 6609 RVA: 0x00089D17 File Offset: 0x00087F17
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x00089D34 File Offset: 0x00087F34
	private void FixedUpdate()
	{
		if (this.rb.isKinematic || this.rb.useGravity)
		{
			return;
		}
		if (this.enableDynamics)
		{
			Vector3 vector = (this.grabPt.position - this.grabPtPosition) * 100f;
			vector = Matrix4x4.Rotate(this.ctrlRotation).MultiplyVector(vector);
			this.rb.AddForce(vector, 0);
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			this.rb.linearVelocity = linearVelocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
			base.transform.LookAt(base.transform.position - this.rb.linearVelocity);
		}
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x000029BC File Offset: 0x00000BBC
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x0008765F File Offset: 0x0008585F
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x00089E0E File Offset: 0x0008800E
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		transferOwnership = false;
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x00089E14 File Offset: 0x00088014
	public bool ReturnStep()
	{
		this.rb.isKinematic = true;
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.grabPt.position, Time.deltaTime * this.returnSpeed);
		return base.transform.position == this.grabPt.position;
	}

	// Token: 0x04002335 RID: 9013
	private Rigidbody rb;

	// Token: 0x04002336 RID: 9014
	private Collider balloonCollider;

	// Token: 0x04002337 RID: 9015
	private Bounds bounds;

	// Token: 0x04002338 RID: 9016
	[SerializeField]
	private float bouyancyMinHeight = 10f;

	// Token: 0x04002339 RID: 9017
	[SerializeField]
	private float bouyancyMaxHeight = 20f;

	// Token: 0x0400233A RID: 9018
	private float bouyancyActualHeight = 20f;

	// Token: 0x0400233B RID: 9019
	[SerializeField]
	private float airResistance = 0.01f;

	// Token: 0x0400233C RID: 9020
	public GameObject knot;

	// Token: 0x0400233D RID: 9021
	private Rigidbody knotRb;

	// Token: 0x0400233E RID: 9022
	public Transform grabPt;

	// Token: 0x0400233F RID: 9023
	private Transform grabPtInitParent;

	// Token: 0x04002340 RID: 9024
	[SerializeField]
	private float maximumVelocity = 2f;

	// Token: 0x04002341 RID: 9025
	private bool enableDynamics;

	// Token: 0x04002342 RID: 9026
	[SerializeField]
	private float balloonScale = 1f;

	// Token: 0x04002343 RID: 9027
	private Vector3 grabPtPosition;

	// Token: 0x04002344 RID: 9028
	[SerializeField]
	private Quaternion ctrlRotation;

	// Token: 0x04002345 RID: 9029
	[SerializeField]
	private float returnSpeed = 50f;
}
