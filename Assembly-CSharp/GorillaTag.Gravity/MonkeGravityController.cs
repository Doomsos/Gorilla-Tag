using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Gravity;

public class MonkeGravityController : MonoBehaviour, ICallbackUnique, ICallBack
{
	[SerializeField]
	private Collider m_activatorCollider;

	[SerializeField]
	protected Rigidbody m_targetRigidBody;

	[SerializeField]
	protected Transform m_targetTransform;

	[SerializeField]
	private bool m_instantRotation;

	[SerializeField]
	private bool m_useRotation;

	[SerializeField]
	private bool m_overrideForceMode;

	[SerializeField]
	private ForceMode m_forceModeOverride;

	[SerializeField]
	private BasicGravityZone m_alwaysInZone;

	private bool m_register;

	private readonly List<BasicGravityZone> m_gravityZones = new List<BasicGravityZone>(3);

	private bool m_needsRotationRecovery;

	protected bool m_globalGravityIntent;

	public Collider ActivatorCollider => m_activatorCollider;

	public Rigidbody TargetRigidBody => m_targetRigidBody;

	public Transform TargetTransform => m_targetTransform;

	public virtual float Scale => TargetTransform.localScale.x;

	protected bool InstantRotation => m_instantRotation;

	protected bool OverrideForceMode => m_overrideForceMode;

	public bool Register => m_register;

	public Vector3 GravityUp { get; private set; } = Vector3.up;

	public Vector3 GravityDown { get; private set; } = Vector3.down;

	public float GravityMultiplier { get; set; } = 1f;

	public Vector3 PersonalGravityDirection { get; set; } = Vector3.up;

	public int GravityZonesCount => m_gravityZones.Count;

	bool ICallbackUnique.Registered { get; set; }

	public void SetPersonalGravityDirection(Vector3 direction)
	{
		PersonalGravityDirection = direction.normalized;
	}

	public void SetPersonalGravityDirection(Transform reference)
	{
		PersonalGravityDirection = reference.up;
	}

	protected virtual void Awake()
	{
		if (m_targetRigidBody.IsNull())
		{
			m_targetRigidBody = GetComponent<Rigidbody>();
		}
		if (m_targetTransform.IsNull())
		{
			m_targetTransform = base.transform;
		}
		if (m_alwaysInZone != null)
		{
			m_alwaysInZone.AddTarget(this);
		}
		else if (m_activatorCollider.IsNull())
		{
			m_activatorCollider = GetComponent<Collider>();
			if (m_activatorCollider.IsNull())
			{
				return;
			}
		}
		if (!m_targetRigidBody.IsNull())
		{
			m_register = true;
			m_globalGravityIntent = m_targetRigidBody.useGravity;
		}
	}

	protected virtual void OnEnable()
	{
		if (m_register)
		{
			MonkeGravityManager.AddMonkeGravityController(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (m_register)
		{
			m_targetRigidBody.useGravity = m_globalGravityIntent;
			MonkeGravityManager.RemoveMonkeGravityController(this);
			for (int num = m_gravityZones.Count - 1; num > -1; num--)
			{
				BasicGravityZone basicGravityZone = m_gravityZones[num];
				basicGravityZone.RemoveTarget(this);
				OnLeftGravityZone(basicGravityZone);
			}
		}
	}

	public virtual void CallBack()
	{
		if (m_gravityZones.Count < 1 || !m_gravityZones[m_gravityZones.Count - 1].GetGravityInfo(this, out var info))
		{
			info = MonkeGravityManager.DefaultGravityInfo;
		}
		GravityUp = info.gravityUpDirection;
		GravityDown = -GravityUp;
		if (m_gravityZones.Count < 1)
		{
			ApplyGravityForce(Physics.gravity);
		}
		if ((info.rotate || m_needsRotationRecovery) && m_useRotation)
		{
			ApplyGravityUpRotation(in info.rotationDirection, info.rotationSpeed * Time.fixedDeltaTime);
		}
	}

	public virtual Vector3 GetWorldPoint()
	{
		return m_targetTransform.position;
	}

	public virtual void OnEnteredGravityZone(BasicGravityZone zone)
	{
		if (!m_gravityZones.Contains(zone))
		{
			m_gravityZones.Add(zone);
		}
		if (m_targetRigidBody.useGravity)
		{
			m_targetRigidBody.useGravity = false;
		}
	}

	public virtual void OnLeftGravityZone(BasicGravityZone zone)
	{
		m_gravityZones.Remove(zone);
		if (m_gravityZones.Count < 1)
		{
			m_targetRigidBody.useGravity = m_globalGravityIntent;
			m_needsRotationRecovery = true;
		}
	}

	public virtual void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
	{
		if (!m_targetRigidBody.isKinematic)
		{
			m_targetRigidBody.AddForce(force * GravityMultiplier, m_overrideForceMode ? m_forceModeOverride : forceType);
		}
	}

	public void ClearRotationRecovery()
	{
		m_needsRotationRecovery = false;
	}

	public virtual void ApplyGravityUpRotation(in Vector3 upDir, float speed)
	{
		Vector3 up = m_targetTransform.up;
		Vector3 toDirection = (m_instantRotation ? upDir : Vector3.RotateTowards(up, upDir, speed, 0f));
		Quaternion quaternion = Quaternion.FromToRotation(up, toDirection);
		m_targetRigidBody.MoveRotation(quaternion * m_targetTransform.rotation);
		if (quaternion == Quaternion.identity)
		{
			m_needsRotationRecovery = false;
		}
	}
}
