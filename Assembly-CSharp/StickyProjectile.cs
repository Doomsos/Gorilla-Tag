using System;
using GorillaLocomotion.Swimming;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004EB RID: 1259
public class StickyProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x06002059 RID: 8281 RVA: 0x000AB794 File Offset: 0x000A9994
	private void Awake()
	{
		this.stickyPart.GetLocalPositionAndRotation(ref this.stickyPartLocalPosition, ref this.stickyPartLocalRotation);
		this.stickyPartLocalScale = this.stickyPart.localScale;
		this.headZoneInversePosition = this.INVERSE_HEAD_ROTATION * this.headZonePosition;
		this.headZoneInverseLocalPosition = this.INVERSE_HEAD_ROTATION * this.localHeadZonePosition;
		this.rb = base.GetComponent<Rigidbody>();
		this.rbwi = base.GetComponent<RigidbodyWaterInteraction>();
		this.collider = base.GetComponent<Collider>();
		this.pcc = base.GetComponent<PlayerColoredCosmetic>();
		this.triggerLayer = LayerMask.NameToLayer("Gorilla Tag Collider");
		UnityEvent onReset = this.OnReset;
		if (onReset == null)
		{
			return;
		}
		onReset.Invoke();
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x000AB848 File Offset: 0x000A9A48
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		UnityEvent onLaunch = this.OnLaunch;
		if (onLaunch != null)
		{
			onLaunch.Invoke();
		}
		this.stickyPart.SetParent(base.transform, false);
		this.stickyPart.SetLocalPositionAndRotation(this.stickyPartLocalPosition, this.stickyPartLocalRotation);
		this.stickyPart.localScale = this.stickyPartLocalScale;
		base.transform.SetPositionAndRotation(startPosition, startRotation);
		base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		this.rb.isKinematic = false;
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.rb.linearVelocity = velocity;
		if (this.faceVelocityWhileAirborne)
		{
			TickSystem<object>.AddTickCallback(this);
			this.rb.angularVelocity = Vector3.zero;
		}
		else
		{
			this.rb.angularVelocity = Random.onUnitSphere * Random.Range(this.launchRandomSpinSpeedMinMax.x, this.launchRandomSpinSpeedMinMax.y);
		}
		this.rbwi.enabled = true;
		this.collider.enabled = true;
		if (this.pcc != null)
		{
			this.pcc.UpdateColor(ownerRig.playerColor);
		}
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000AB984 File Offset: 0x000A9B84
	private void StickTo(Transform otherTransform, Vector3 position, Quaternion rotation)
	{
		this.stickyPart.parent = otherTransform;
		this.stickyPart.SetPositionAndRotation(position + rotation * this.stickyPartLocalPosition, rotation * this.stickyPartLocalRotation);
		this.rb.isKinematic = true;
		this.rbwi.enabled = false;
		this.collider.enabled = false;
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000AB9EC File Offset: 0x000A9BEC
	private void OnCollisionEnter(Collision collision)
	{
		TickSystem<object>.RemoveTickCallback(this);
		ContactPoint contact = collision.GetContact(0);
		this.StickTo(collision.transform, contact.point, this.alignToHitNormal ? Quaternion.LookRotation(contact.normal, Random.onUnitSphere) : base.transform.rotation);
		this.stickEvents.InvokeAll(StickyProjectile.StickFlags.Wall, false);
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000ABA50 File Offset: 0x000A9C50
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != this.triggerLayer)
		{
			return;
		}
		TickSystem<object>.RemoveTickCallback(this);
		Vector3 vector = Time.fixedDeltaTime * 2f * this.rb.linearVelocity;
		Vector3 vector2 = base.transform.position - vector;
		Vector3 vector3;
		Quaternion rotation;
		if (this.alignToHitNormal)
		{
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			other.Raycast(new Ray(vector2, vector / magnitude), ref raycastHit, 2f * magnitude);
			vector3 = raycastHit.point;
			rotation = Quaternion.LookRotation(raycastHit.normal, Random.onUnitSphere);
		}
		else
		{
			vector3 = other.ClosestPoint(vector2);
			rotation = base.transform.rotation;
		}
		VRRig componentInParent = other.GetComponentInParent<VRRig>();
		if (componentInParent != null)
		{
			if (this.headZoneRadius > 0f && string.Equals(other.name, "SpeakerHeadCollider"))
			{
				Vector3 vector4;
				Quaternion quaternion;
				other.transform.GetPositionAndRotation(ref vector4, ref quaternion);
				Vector3 vector5 = quaternion * this.headZoneInversePosition + vector4;
				if ((vector3 - vector5).magnitude <= this.headZoneRadius * componentInParent.scaleFactor)
				{
					if (componentInParent.isOfflineVRRig)
					{
						this.StickTo(other.transform, quaternion * this.headZoneInverseLocalPosition + vector4, quaternion * this.INVERSE_HEAD_ROTATION);
						this.stickyPart.localScale *= this.scaleOnLocalHeadZone;
						this.stickEvents.InvokeAll(StickyProjectile.StickFlags.LocalHeadZone, false);
						return;
					}
					this.StickTo(other.transform, vector5, quaternion * this.INVERSE_HEAD_ROTATION);
					this.stickEvents.InvokeAll(StickyProjectile.StickFlags.RemoteHeadZone, false);
					return;
				}
				else if (componentInParent.isOfflineVRRig)
				{
					this.stickyPart.localScale *= this.scaleOnLocalHead;
				}
			}
			this.stickEvents.InvokeAll(componentInParent.isOfflineVRRig ? StickyProjectile.StickFlags.LocalPlayer : StickyProjectile.StickFlags.RemotePlayer, false);
		}
		else
		{
			this.stickEvents.InvokeAll(StickyProjectile.StickFlags.Wall, false);
		}
		this.StickTo(other.transform, vector3, rotation);
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x000ABC6D File Offset: 0x000A9E6D
	private void OnEnable()
	{
		this.stickyPart.gameObject.SetActive(true);
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x000ABC80 File Offset: 0x000A9E80
	private void OnDisable()
	{
		this.stickyPart.gameObject.SetActive(false);
		UnityEvent onReset = this.OnReset;
		if (onReset == null)
		{
			return;
		}
		onReset.Invoke();
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06002060 RID: 8288 RVA: 0x000ABCA3 File Offset: 0x000A9EA3
	// (set) Token: 0x06002061 RID: 8289 RVA: 0x000ABCAB File Offset: 0x000A9EAB
	public bool TickRunning { get; set; }

	// Token: 0x06002062 RID: 8290 RVA: 0x000ABCB4 File Offset: 0x000A9EB4
	public void Tick()
	{
		this.rb.rotation = Quaternion.LookRotation(this.rb.linearVelocity);
	}

	// Token: 0x04002ACE RID: 10958
	[SerializeField]
	private Transform stickyPart;

	// Token: 0x04002ACF RID: 10959
	[Tooltip("Align the positive Z direction of this object to the rigidbody's velocity.")]
	[SerializeField]
	private bool faceVelocityWhileAirborne;

	// Token: 0x04002AD0 RID: 10960
	[Tooltip("Set the rigidbody's angular velocity to a random unit Vector3, multiplied by a random value in this range.")]
	[SerializeField]
	private Vector2 launchRandomSpinSpeedMinMax = new Vector2(90f, 360f);

	// Token: 0x04002AD1 RID: 10961
	[Tooltip("When enabled, the positive Z direction will face away from whatever surface the projectile hit. When disabled, it will keep its original rotation.")]
	[SerializeField]
	private bool alignToHitNormal = true;

	// Token: 0x04002AD2 RID: 10962
	[Space]
	[SerializeField]
	public UnityEvent OnReset;

	// Token: 0x04002AD3 RID: 10963
	[SerializeField]
	public UnityEvent OnLaunch;

	// Token: 0x04002AD4 RID: 10964
	[Tooltip("Scale the 'Sticky Part' by this value when hitting the local player's head. Usually used to prevent things from obscuring your vision too much.")]
	[SerializeField]
	private float scaleOnLocalHead = 0.7f;

	// Token: 0x04002AD5 RID: 10965
	[Tooltip("The radius of the head zone. Can be set to 0 to disable head zone functionality.")]
	[SerializeField]
	private float headZoneRadius = 0.15f;

	// Token: 0x04002AD6 RID: 10966
	[Tooltip("The local origin of the head zone, relative to the player rig's head transform. When a shot hits inside the zone, the 'Sticky Part' will be moved to this position relative to the hit player's head.")]
	[SerializeField]
	private Vector3 headZonePosition = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x04002AD7 RID: 10967
	[Tooltip("Scale the 'Sticky Part' by this value when hitting the local player's head zone. Can override 'Scale On Local Head' in case you want it to appear larger for emphasis.")]
	[SerializeField]
	private float scaleOnLocalHeadZone = 1f;

	// Token: 0x04002AD8 RID: 10968
	[Tooltip("When a shot hits inside a remote player's head zone, it will be moved to the 'Head Zone Relative Position'. For the local player, it will instead be moved here. This DOES NOT AFFECT the actual origin of the head zone for hit-detection purposes, it is purely visual after-the-fact.")]
	[SerializeField]
	private Vector3 localHeadZonePosition = new Vector3(0f, 0.05f, 0.2f);

	// Token: 0x04002AD9 RID: 10969
	[SerializeField]
	private FlagEvents<StickyProjectile.StickFlags> stickEvents;

	// Token: 0x04002ADA RID: 10970
	private readonly Quaternion INVERSE_HEAD_ROTATION = Quaternion.Inverse(Quaternion.Euler(0f, 270f, 252.3229f));

	// Token: 0x04002ADB RID: 10971
	private Vector3 headZoneInversePosition;

	// Token: 0x04002ADC RID: 10972
	private Vector3 headZoneInverseLocalPosition;

	// Token: 0x04002ADD RID: 10973
	private Vector3 stickyPartLocalPosition;

	// Token: 0x04002ADE RID: 10974
	private Quaternion stickyPartLocalRotation;

	// Token: 0x04002ADF RID: 10975
	private Vector3 stickyPartLocalScale;

	// Token: 0x04002AE0 RID: 10976
	private Rigidbody rb;

	// Token: 0x04002AE1 RID: 10977
	private RigidbodyWaterInteraction rbwi;

	// Token: 0x04002AE2 RID: 10978
	private Collider collider;

	// Token: 0x04002AE3 RID: 10979
	private PlayerColoredCosmetic pcc;

	// Token: 0x04002AE4 RID: 10980
	private int triggerLayer;

	// Token: 0x020004EC RID: 1260
	[Flags]
	public enum StickFlags
	{
		// Token: 0x04002AE7 RID: 10983
		Wall = 1,
		// Token: 0x04002AE8 RID: 10984
		LocalPlayer = 2,
		// Token: 0x04002AE9 RID: 10985
		RemotePlayer = 4,
		// Token: 0x04002AEA RID: 10986
		LocalHeadZone = 8,
		// Token: 0x04002AEB RID: 10987
		RemoteHeadZone = 16
	}
}
