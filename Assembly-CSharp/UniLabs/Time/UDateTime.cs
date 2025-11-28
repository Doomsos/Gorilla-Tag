using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000D76 RID: 3446
	[JsonObject(1)]
	[Serializable]
	public class UDateTime : ISerializationCallbackReceiver, IComparable<UDateTime>, IComparable<DateTime>
	{
		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x0600545C RID: 21596 RVA: 0x001AA917 File Offset: 0x001A8B17
		// (set) Token: 0x0600545D RID: 21597 RVA: 0x001AA91F File Offset: 0x001A8B1F
		[JsonProperty("DateTime")]
		public DateTime DateTime { get; set; }

		// Token: 0x0600545E RID: 21598 RVA: 0x001AA928 File Offset: 0x001A8B28
		[JsonConstructor]
		public UDateTime()
		{
			this.DateTime = DateTime.UnixEpoch;
		}

		// Token: 0x0600545F RID: 21599 RVA: 0x001AA93B File Offset: 0x001A8B3B
		public UDateTime(DateTime dateTime)
		{
			this.DateTime = dateTime;
		}

		// Token: 0x06005460 RID: 21600 RVA: 0x001AA94A File Offset: 0x001A8B4A
		public static implicit operator DateTime(UDateTime udt)
		{
			return udt.DateTime;
		}

		// Token: 0x06005461 RID: 21601 RVA: 0x001AA952 File Offset: 0x001A8B52
		public static implicit operator UDateTime(DateTime dt)
		{
			return new UDateTime
			{
				DateTime = dt
			};
		}

		// Token: 0x06005462 RID: 21602 RVA: 0x001AA960 File Offset: 0x001A8B60
		public int CompareTo(DateTime other)
		{
			return this.DateTime.CompareTo(other);
		}

		// Token: 0x06005463 RID: 21603 RVA: 0x001AA97C File Offset: 0x001A8B7C
		public int CompareTo(UDateTime other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.DateTime.CompareTo(other.DateTime);
		}

		// Token: 0x06005464 RID: 21604 RVA: 0x001AA9A8 File Offset: 0x001A8BA8
		protected bool Equals(UDateTime other)
		{
			return this.DateTime.Equals(other.DateTime);
		}

		// Token: 0x06005465 RID: 21605 RVA: 0x001AA9C9 File Offset: 0x001A8BC9
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UDateTime)obj)));
		}

		// Token: 0x06005466 RID: 21606 RVA: 0x001AA9F8 File Offset: 0x001A8BF8
		public override int GetHashCode()
		{
			return this.DateTime.GetHashCode();
		}

		// Token: 0x06005467 RID: 21607 RVA: 0x001AAA14 File Offset: 0x001A8C14
		public override string ToString()
		{
			return this.DateTime.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06005468 RID: 21608 RVA: 0x001AAA34 File Offset: 0x001A8C34
		public void OnAfterDeserialize()
		{
			DateTime dateTime;
			this.DateTime = (DateTime.TryParse(this._DateTime, CultureInfo.InvariantCulture, 128, ref dateTime) ? dateTime : DateTime.MinValue);
		}

		// Token: 0x06005469 RID: 21609 RVA: 0x001AAA68 File Offset: 0x001A8C68
		public void OnBeforeSerialize()
		{
			this._DateTime = this.DateTime.ToString("o", CultureInfo.InvariantCulture);
		}

		// Token: 0x0600546A RID: 21610 RVA: 0x001AAA93 File Offset: 0x001A8C93
		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x0600546B RID: 21611 RVA: 0x001AAA9B File Offset: 0x001A8C9B
		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x040061CC RID: 25036
		[HideInInspector]
		[SerializeField]
		private string _DateTime;
	}
}
