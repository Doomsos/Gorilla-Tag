using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000297 RID: 663
[Serializable]
public struct GTSturdyComponentRef<T> where T : Component
{
	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x060010F2 RID: 4338 RVA: 0x0005B4F1 File Offset: 0x000596F1
	// (set) Token: 0x060010F3 RID: 4339 RVA: 0x0005B4F9 File Offset: 0x000596F9
	public Transform BaseXform
	{
		get
		{
			return this._baseXform;
		}
		set
		{
			this._baseXform = value;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x060010F4 RID: 4340 RVA: 0x0005B504 File Offset: 0x00059704
	// (set) Token: 0x060010F5 RID: 4341 RVA: 0x0005B573 File Offset: 0x00059773
	public T Value
	{
		get
		{
			if (!this._value)
			{
				return this._value;
			}
			if (string.IsNullOrEmpty(this._relativePath))
			{
				return default(T);
			}
			Transform transform;
			if (!this._baseXform.TryFindByPath(this._relativePath, out transform, false))
			{
				return default(T);
			}
			this._value = transform.GetComponent<T>();
			return this._value;
		}
		set
		{
			this._value = value;
			this._relativePath = ((!value) ? this._baseXform.GetRelativePath(value.transform) : string.Empty);
		}
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x0005B5AC File Offset: 0x000597AC
	public static implicit operator T(GTSturdyComponentRef<T> sturdyRef)
	{
		return sturdyRef.Value;
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x0005B5B8 File Offset: 0x000597B8
	public static implicit operator GTSturdyComponentRef<T>(T component)
	{
		return new GTSturdyComponentRef<T>
		{
			Value = component
		};
	}

	// Token: 0x0400153E RID: 5438
	[SerializeField]
	private T _value;

	// Token: 0x0400153F RID: 5439
	[SerializeField]
	private string _relativePath;

	// Token: 0x04001540 RID: 5440
	[SerializeField]
	private Transform _baseXform;
}
