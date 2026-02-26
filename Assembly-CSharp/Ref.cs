using System;
using UnityEngine;

[Serializable]
public class Ref<T> where T : class
{
	public T AsT
	{
		get
		{
			return this;
		}
		set
		{
			this._target = (value as UnityEngine.Object);
		}
	}

	public static implicit operator bool(Ref<T> r)
	{
		UnityEngine.Object @object = (r != null) ? r._target : null;
		return @object != null && @object != null;
	}

	public static implicit operator T(Ref<T> r)
	{
		UnityEngine.Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return default(T);
		}
		if (@object == null)
		{
			return default(T);
		}
		return @object as T;
	}

	public static implicit operator UnityEngine.Object(Ref<T> r)
	{
		UnityEngine.Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return null;
		}
		if (@object == null)
		{
			return null;
		}
		return @object;
	}

	[SerializeField]
	private UnityEngine.Object _target;
}
