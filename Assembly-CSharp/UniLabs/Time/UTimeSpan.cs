using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000D77 RID: 3447
	[JsonObject(1)]
	[Serializable]
	public class UTimeSpan : ISerializationCallbackReceiver, IComparable<UTimeSpan>, IComparable<TimeSpan>
	{
		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x0600546C RID: 21612 RVA: 0x001AAAA3 File Offset: 0x001A8CA3
		// (set) Token: 0x0600546D RID: 21613 RVA: 0x001AAAAB File Offset: 0x001A8CAB
		[JsonProperty("TimeSpan")]
		public TimeSpan TimeSpan { get; set; }

		// Token: 0x0600546E RID: 21614 RVA: 0x001AAAB4 File Offset: 0x001A8CB4
		[JsonConstructor]
		public UTimeSpan()
		{
			this.TimeSpan = TimeSpan.Zero;
		}

		// Token: 0x0600546F RID: 21615 RVA: 0x001AAAC7 File Offset: 0x001A8CC7
		public UTimeSpan(TimeSpan timeSpan)
		{
			this.TimeSpan = timeSpan;
		}

		// Token: 0x06005470 RID: 21616 RVA: 0x001AAAD6 File Offset: 0x001A8CD6
		public UTimeSpan(long ticks) : this(new TimeSpan(ticks))
		{
		}

		// Token: 0x06005471 RID: 21617 RVA: 0x001AAAE4 File Offset: 0x001A8CE4
		public UTimeSpan(int hours, int minutes, int seconds) : this(new TimeSpan(hours, minutes, seconds))
		{
		}

		// Token: 0x06005472 RID: 21618 RVA: 0x001AAAF4 File Offset: 0x001A8CF4
		public UTimeSpan(int days, int hours, int minutes, int seconds) : this(new TimeSpan(days, hours, minutes, seconds))
		{
		}

		// Token: 0x06005473 RID: 21619 RVA: 0x001AAB06 File Offset: 0x001A8D06
		public UTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) : this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
		{
		}

		// Token: 0x06005474 RID: 21620 RVA: 0x001AAB1A File Offset: 0x001A8D1A
		public static implicit operator TimeSpan(UTimeSpan uTimeSpan)
		{
			if (uTimeSpan == null)
			{
				return TimeSpan.Zero;
			}
			return uTimeSpan.TimeSpan;
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x001AAB2B File Offset: 0x001A8D2B
		public static implicit operator UTimeSpan(TimeSpan timeSpan)
		{
			return new UTimeSpan(timeSpan);
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x001AAB34 File Offset: 0x001A8D34
		public int CompareTo(TimeSpan other)
		{
			return this.TimeSpan.CompareTo(other);
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x001AAB50 File Offset: 0x001A8D50
		public int CompareTo(UTimeSpan other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.TimeSpan.CompareTo(other.TimeSpan);
		}

		// Token: 0x06005478 RID: 21624 RVA: 0x001AAB7C File Offset: 0x001A8D7C
		protected bool Equals(UTimeSpan other)
		{
			return this.TimeSpan.Equals(other.TimeSpan);
		}

		// Token: 0x06005479 RID: 21625 RVA: 0x001AAB9D File Offset: 0x001A8D9D
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UTimeSpan)obj)));
		}

		// Token: 0x0600547A RID: 21626 RVA: 0x001AABCC File Offset: 0x001A8DCC
		public override int GetHashCode()
		{
			return this.TimeSpan.GetHashCode();
		}

		// Token: 0x0600547B RID: 21627 RVA: 0x001AABF0 File Offset: 0x001A8DF0
		public void OnAfterDeserialize()
		{
			TimeSpan timeSpan;
			this.TimeSpan = (TimeSpan.TryParse(this._TimeSpan, CultureInfo.InvariantCulture, ref timeSpan) ? timeSpan : TimeSpan.Zero);
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x001AAC20 File Offset: 0x001A8E20
		public void OnBeforeSerialize()
		{
			this._TimeSpan = this.TimeSpan.ToString();
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x001AAC47 File Offset: 0x001A8E47
		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x0600547E RID: 21630 RVA: 0x001AAC4F File Offset: 0x001A8E4F
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x040061CE RID: 25038
		[HideInInspector]
		[SerializeField]
		private string _TimeSpan;
	}
}
