using UnityEngine;

public abstract class ObservableBehavior : MonoBehaviour, IGorillaSliceableSimple
{
	private bool firstFrame = true;

	protected bool observable = true;

	[SerializeField]
	private ObservableBehaviorRule observableBehaviorRule;

	private float dist;

	public float Distance => dist;

	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		UnityOnEnable();
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (observable)
		{
			observable = false;
			OnLostObservable();
		}
		UnityOnDisable();
	}

	private void OnDestroy()
	{
		if (observable)
		{
			observable = false;
			OnLostObservable();
		}
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		Transform transform = Camera.main.transform;
		dist = Vector3.Distance(transform.position, base.transform.position);
		float num = ((!(observableBehaviorRule != null) || !observableBehaviorRule.InverseObservable) ? Vector3.Dot((transform.position - base.transform.position).normalized, transform.transform.forward) : Vector3.Dot((base.transform.position - transform.position).normalized, base.transform.forward));
		bool flag = observableBehaviorRule == null || (observableBehaviorRule.ObservableDistanceRange.x <= dist && dist <= observableBehaviorRule.ObservableDistanceRange.y && observableBehaviorRule.ObservableDotRange.x <= num && num <= observableBehaviorRule.ObservableDotRange.y);
		if (firstFrame || observable != flag)
		{
			if (flag)
			{
				OnBecameObservable();
			}
			else
			{
				OnLostObservable();
			}
		}
		observable = flag;
		firstFrame = false;
		if (flag)
		{
			ObservableSliceUpdate();
		}
	}

	protected virtual void UnityOnEnable()
	{
	}

	protected virtual void UnityOnDisable()
	{
	}

	protected abstract void OnLostObservable();

	protected abstract void OnBecameObservable();

	protected abstract void ObservableSliceUpdate();
}
