using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000FC9 RID: 4041
	[Serializable]
	public struct GTDirectAssetRef<T> : IEquatable<T> where T : Object
	{
		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06006672 RID: 26226 RVA: 0x002162E7 File Offset: 0x002144E7
		// (set) Token: 0x06006673 RID: 26227 RVA: 0x002162EF File Offset: 0x002144EF
		public T obj
		{
			get
			{
				return this._obj;
			}
			set
			{
				this._obj = value;
				this.edAssetPath = null;
			}
		}

		// Token: 0x06006674 RID: 26228 RVA: 0x002162EF File Offset: 0x002144EF
		public GTDirectAssetRef(T theObj)
		{
			this._obj = theObj;
			this.edAssetPath = null;
		}

		// Token: 0x06006675 RID: 26229 RVA: 0x002162FF File Offset: 0x002144FF
		public static implicit operator T(GTDirectAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		// Token: 0x06006676 RID: 26230 RVA: 0x00216308 File Offset: 0x00214508
		public static implicit operator GTDirectAssetRef<T>(T other)
		{
			return new GTDirectAssetRef<T>
			{
				obj = other
			};
		}

		// Token: 0x06006677 RID: 26231 RVA: 0x00216326 File Offset: 0x00214526
		public bool Equals(T other)
		{
			return this.obj == other;
		}

		// Token: 0x06006678 RID: 26232 RVA: 0x00216340 File Offset: 0x00214540
		public override bool Equals(object other)
		{
			T t = other as T;
			return t != null && this.Equals(t);
		}

		// Token: 0x06006679 RID: 26233 RVA: 0x0021636A File Offset: 0x0021456A
		public override int GetHashCode()
		{
			if (!(this.obj != null))
			{
				return 0;
			}
			return this.obj.GetHashCode();
		}

		// Token: 0x0600667A RID: 26234 RVA: 0x00216391 File Offset: 0x00214591
		public static bool operator ==(GTDirectAssetRef<T> left, T right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600667B RID: 26235 RVA: 0x0021639B File Offset: 0x0021459B
		public static bool operator !=(GTDirectAssetRef<T> left, T right)
		{
			return !(left == right);
		}

		// Token: 0x0400751F RID: 29983
		[SerializeField]
		[HideInInspector]
		internal T _obj;

		// Token: 0x04007520 RID: 29984
		[FormerlySerializedAs("assetPath")]
		public string edAssetPath;
	}
}
