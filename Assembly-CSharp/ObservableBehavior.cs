using System;
using UnityEngine;

public abstract class ObservableBehavior : MonoBehaviour, IGorillaSliceableSimple
{
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.UnityOnEnable();
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.UnityOnDisable();
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		Transform transform = Camera.main.transform;
		float num = Vector3.Distance(transform.position, base.transform.position);
		float num2 = Vector3.Dot((transform.position - base.transform.position).normalized, transform.transform.forward);
		bool flag = this.observableBehaviorRule.ObservableDistanceRange.x <= num && num <= this.observableBehaviorRule.ObservableDistanceRange.y && this.observableBehaviorRule.ObservableDotRange.x <= num2 && num2 <= this.observableBehaviorRule.ObservableDotRange.y;
		if (this.firstFrame || this.observable != flag)
		{
			if (flag)
			{
				this.OnBecameObservable();
			}
			else
			{
				this.OnLostObservable();
			}
		}
		this.observable = flag;
		this.firstFrame = false;
	}

	protected virtual void UnityOnEnable()
	{
	}

	protected virtual void UnityOnDisable()
	{
	}

	protected abstract void OnLostObservable();

	protected abstract void OnBecameObservable();

	private bool firstFrame = true;

	private bool observable = true;

	[SerializeField]
	private ObservableBehaviorRule observableBehaviorRule;
}
