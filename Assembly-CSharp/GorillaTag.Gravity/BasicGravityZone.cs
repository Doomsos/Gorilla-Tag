using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Gravity;

public class BasicGravityZone : MonoBehaviour, ICallbackUnique, ICallBack
{
	[Header("Gravity Settings")]
	[Tooltip("negative number pulls, positive number expels")]
	public float gravityStrength = -9.81f;

	[Tooltip("Filter which players are affected based on scale. Small = scale < 1")]
	[SerializeField]
	private GravityZoneScaleFilter scaleFilter;

	[Header("Rotation Settings")]
	[Tooltip("If enabled, rotates the target away from gravity direction to be upside down")]
	[SerializeField]
	protected bool invertRotationDirection;

	[SerializeField]
	protected bool rotateTarget = true;

	[SerializeField]
	protected float rotationSpeed = 10f;

	protected Vector3 m_gravityDirection;

	protected ListProcessor<MonkeGravityController> m_gravityTargets = new ListProcessor<MonkeGravityController>(5);

	private Dictionary<MonkeGravityController, GravityInfo> m_targetGravityInfos = new Dictionary<MonkeGravityController, GravityInfo>(5);

	private IReadOnlyList<MonkeGravityController> GravityTargets => m_gravityTargets.GetReadonlyList();

	bool ICallbackUnique.Registered { get; set; }

	protected virtual void Awake()
	{
		m_gravityDirection = base.gameObject.transform.up;
		invertRotationDirection = (gravityStrength > 0f && !invertRotationDirection) || (gravityStrength <= 0f && invertRotationDirection);
	}

	protected virtual void OnEnable()
	{
		m_gravityTargets.ItemProcessor = ProcessGravityTargets;
	}

	protected virtual void OnDisable()
	{
		m_gravityTargets.ItemProcessor = ProcessRemoveTargets;
		m_gravityTargets.ProcessList();
		m_gravityTargets.Clear();
		m_targetGravityInfos.Clear();
		MonkeGravityManager.RemoveGravityCallback(this);
	}

	public virtual void CallBack()
	{
		m_gravityTargets.ProcessList();
	}

	private void ProcessRemoveTargets(in MonkeGravityController target)
	{
		target.OnLeftGravityZone(this);
	}

	private void ProcessGravityTargets(in MonkeGravityController targetController)
	{
		if (!PassesScaleFilter(targetController))
		{
			OnTargetFilteredOut(targetController);
		}
		GravityInfo value = default(GravityInfo);
		Vector3 offsetFromGravity = GetGravityVectorAtPoint(targetController.GetWorldPoint(), in targetController);
		Vector3 gravityDirection = (value.gravityUpDirection = offsetFromGravity.normalized);
		value.rotationDirection = GetRotationDirection(in gravityDirection);
		value.gravityStrength = GetGravityStrength(in offsetFromGravity);
		value.rotationSpeed = GetRotationSpeed(in offsetFromGravity);
		value.rotate = GetRotationIntent(in offsetFromGravity);
		m_targetGravityInfos[targetController] = value;
		if (value.gravityStrength != 0f)
		{
			targetController.ApplyGravityForce(gravityDirection * value.gravityStrength);
		}
	}

	protected virtual Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
	{
		return m_gravityDirection;
	}

	protected virtual float GetGravityStrength(in Vector3 offsetFromGravity)
	{
		return gravityStrength;
	}

	protected virtual bool GetRotationIntent(in Vector3 offsetFromGravity)
	{
		return rotateTarget;
	}

	protected virtual Vector3 GetRotationDirection(in Vector3 gravityDirection)
	{
		if (invertRotationDirection)
		{
			return -gravityDirection;
		}
		return gravityDirection;
	}

	protected virtual float GetRotationSpeed(in Vector3 offsetFromGravity)
	{
		return rotationSpeed;
	}

	public bool GetGravityInfo(MonkeGravityController target, out GravityInfo info)
	{
		return m_targetGravityInfos.TryGetValue(target, out info);
	}

	public void RemoveTarget(MonkeGravityController target)
	{
		if (target.Register && m_gravityTargets.Remove(in target))
		{
			m_targetGravityInfos.Remove(target);
			target.OnLeftGravityZone(this);
			OnTargetExited(target);
			if (m_gravityTargets.Count < 1)
			{
				MonkeGravityManager.RemoveGravityCallback(this);
			}
		}
	}

	public void AddTarget(MonkeGravityController target)
	{
		if (target.Register && !m_gravityTargets.Contains(in target))
		{
			m_gravityTargets.Add(in target);
			target.OnEnteredGravityZone(this);
			MonkeGravityManager.AddGravityCallback(this);
		}
	}

	protected virtual void OnTargetExited(MonkeGravityController target)
	{
	}

	protected virtual void OnTargetFilteredOut(MonkeGravityController target)
	{
	}

	private bool PassesScaleFilter(MonkeGravityController target)
	{
		if (scaleFilter == GravityZoneScaleFilter.Anyone)
		{
			return true;
		}
		bool flag = target.Scale < 1f;
		if (scaleFilter != GravityZoneScaleFilter.SmallOnly)
		{
			return !flag;
		}
		return flag;
	}

	private void OnTriggerEnter(Collider other)
	{
		(bool, MonkeGravityController) monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
		if (monkeGravityController.Item1)
		{
			AddTarget(monkeGravityController.Item2);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		(bool, MonkeGravityController) monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
		if (monkeGravityController.Item1)
		{
			RemoveTarget(monkeGravityController.Item2);
		}
	}
}
