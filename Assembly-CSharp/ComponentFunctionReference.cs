using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000925 RID: 2341
[Serializable]
public sealed class ComponentFunctionReference<TResult>
{
	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x06003BDF RID: 15327 RVA: 0x0013C480 File Offset: 0x0013A680
	public bool IsValid
	{
		get
		{
			return this._selection.component || !string.IsNullOrEmpty(this._selection.methodName);
		}
	}

	// Token: 0x06003BE0 RID: 15328 RVA: 0x0013C4A9 File Offset: 0x0013A6A9
	private IEnumerable<ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>> GetMethodOptions()
	{
		if (this._target == null)
		{
			yield break;
		}
		yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>("NONE", default(ComponentFunctionReference<TResult>.MethodRef));
		Type type = typeof(GameObject);
		BindingFlags flags = 52;
		foreach (MethodInfo methodInfo in type.GetMethods(flags))
		{
			if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(TResult))
			{
				string text = type.Name + "/" + methodInfo.Name;
				yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text, new ComponentFunctionReference<TResult>.MethodRef(this._target, methodInfo));
			}
		}
		MethodInfo[] array = null;
		foreach (Component comp in this._target.GetComponents<Component>())
		{
			type = comp.GetType();
			foreach (MethodInfo methodInfo2 in type.GetMethods(flags))
			{
				if (methodInfo2.GetParameters().Length == 0 && methodInfo2.ReturnType == typeof(TResult))
				{
					string text2 = type.Name + "/" + methodInfo2.Name;
					yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text2, new ComponentFunctionReference<TResult>.MethodRef(comp, methodInfo2));
				}
			}
			array = null;
			comp = null;
		}
		Component[] array2 = null;
		yield break;
	}

	// Token: 0x06003BE1 RID: 15329 RVA: 0x0013C4BC File Offset: 0x0013A6BC
	public TResult Invoke()
	{
		if (this._cached == null)
		{
			this.Cache();
		}
		if (this._cached == null)
		{
			return default(TResult);
		}
		return this._cached.Invoke();
	}

	// Token: 0x06003BE2 RID: 15330 RVA: 0x0013C4F4 File Offset: 0x0013A6F4
	public void Cache()
	{
		this._cached = null;
		if (this._selection.component == null || string.IsNullOrEmpty(this._selection.methodName))
		{
			return;
		}
		MethodInfo method = this._selection.component.GetType().GetMethod(this._selection.methodName, 52, null, Type.EmptyTypes, null);
		if (method != null)
		{
			this._cached = (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), this._selection.component, method);
		}
	}

	// Token: 0x04004C6E RID: 19566
	[SerializeField]
	private GameObject _target;

	// Token: 0x04004C6F RID: 19567
	[SerializeField]
	private ComponentFunctionReference<TResult>.MethodRef _selection;

	// Token: 0x04004C70 RID: 19568
	private Func<TResult> _cached;

	// Token: 0x02000926 RID: 2342
	[Serializable]
	private struct MethodRef
	{
		// Token: 0x06003BE4 RID: 15332 RVA: 0x0013C587 File Offset: 0x0013A787
		public MethodRef(Object obj, MethodInfo m)
		{
			this.component = obj;
			this.methodName = m.Name;
		}

		// Token: 0x04004C71 RID: 19569
		public Object component;

		// Token: 0x04004C72 RID: 19570
		public string methodName;
	}
}
