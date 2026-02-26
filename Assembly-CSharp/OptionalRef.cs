using System;
using UnityEngine;

[Serializable]
public class OptionalRef<T> where T : UnityEngine.Object
{
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
		}
	}

	public T Value
	{
		get
		{
			if (this)
			{
				return this._target;
			}
			return default(T);
		}
		set
		{
			this._target = (value ? value : default(T));
		}
	}

	public static implicit operator bool(OptionalRef<T> r)
	{
		if (r == null)
		{
			return false;
		}
		if (!r._enabled)
		{
			return false;
		}
		UnityEngine.Object @object = r._target;
		return @object != null && @object;
	}

	public static implicit operator T(OptionalRef<T> r)
	{
		if (r == null)
		{
			return default(T);
		}
		if (!r._enabled)
		{
			return default(T);
		}
		UnityEngine.Object @object = r._target;
		if (@object == null)
		{
			return default(T);
		}
		if (!@object)
		{
			return default(T);
		}
		return @object as T;
	}

	public static implicit operator UnityEngine.Object(OptionalRef<T> r)
	{
		if (r == null)
		{
			return null;
		}
		if (!r._enabled)
		{
			return null;
		}
		UnityEngine.Object @object = r._target;
		if (@object == null)
		{
			return null;
		}
		if (!@object)
		{
			return null;
		}
		return @object;
	}

	[SerializeField]
	private bool _enabled;

	[SerializeField]
	private T _target;
}
