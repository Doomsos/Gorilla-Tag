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
		// (get) Token: 0x0600546C RID: 21612 RVA: 0x001AAAC3 File Offset: 0x001A8CC3
		// (set) Token: 0x0600546D RID: 21613 RVA: 0x001AAACB File Offset: 0x001A8CCB
		[JsonProperty("TimeSpan")]
		public TimeSpan TimeSpan { get; set; }

		// Token: 0x0600546E RID: 21614 RVA: 0x001AAAD4 File Offset: 0x001A8CD4
		[JsonConstructor]
		public UTimeSpan()
		{
			this.TimeSpan = TimeSpan.Zero;
		}

		// Token: 0x0600546F RID: 21615 RVA: 0x001AAAE7 File Offset: 0x001A8CE7
		public UTimeSpan(TimeSpan timeSpan)
		{
			this.TimeSpan = timeSpan;
		}

		// Token: 0x06005470 RID: 21616 RVA: 0x001AAAF6 File Offset: 0x001A8CF6
		public UTimeSpan(long ticks) : this(new TimeSpan(ticks))
		{
		}

		// Token: 0x06005471 RID: 21617 RVA: 0x001AAB04 File Offset: 0x001A8D04
		public UTimeSpan(int hours, int minutes, int seconds) : this(new TimeSpan(hours, minutes, seconds))
		{
		}

		// Token: 0x06005472 RID: 21618 RVA: 0x001AAB14 File Offset: 0x001A8D14
		public UTimeSpan(int days, int hours, int minutes, int seconds) : this(new TimeSpan(days, hours, minutes, seconds))
		{
		}

		// Token: 0x06005473 RID: 21619 RVA: 0x001AAB26 File Offset: 0x001A8D26
		public UTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) : this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
		{
		}

		// Token: 0x06005474 RID: 21620 RVA: 0x001AAB3A File Offset: 0x001A8D3A
		public static implicit operator TimeSpan(UTimeSpan uTimeSpan)
		{
			if (uTimeSpan == null)
			{
				return TimeSpan.Zero;
			}
			return uTimeSpan.TimeSpan;
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x001AAB4B File Offset: 0x001A8D4B
		public static implicit operator UTimeSpan(TimeSpan timeSpan)
		{
			return new UTimeSpan(timeSpan);
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x001AAB54 File Offset: 0x001A8D54
		public int CompareTo(TimeSpan other)
		{
			return this.TimeSpan.CompareTo(other);
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x001AAB70 File Offset: 0x001A8D70
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

		// Token: 0x06005478 RID: 21624 RVA: 0x001AAB9C File Offset: 0x001A8D9C
		protected bool Equals(UTimeSpan other)
		{
			return this.TimeSpan.Equals(other.TimeSpan);
		}

		// Token: 0x06005479 RID: 21625 RVA: 0x001AABBD File Offset: 0x001A8DBD
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UTimeSpan)obj)));
		}

		// Token: 0x0600547A RID: 21626 RVA: 0x001AABEC File Offset: 0x001A8DEC
		public override int GetHashCode()
		{
			return this.TimeSpan.GetHashCode();
		}

		// Token: 0x0600547B RID: 21627 RVA: 0x001AAC10 File Offset: 0x001A8E10
		public void OnAfterDeserialize()
		{
			TimeSpan timeSpan;
			this.TimeSpan = (TimeSpan.TryParse(this._TimeSpan, CultureInfo.InvariantCulture, ref timeSpan) ? timeSpan : TimeSpan.Zero);
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x001AAC40 File Offset: 0x001A8E40
		public void OnBeforeSerialize()
		{
			this._TimeSpan = this.TimeSpan.ToString();
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x001AAC67 File Offset: 0x001A8E67
		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x0600547E RID: 21630 RVA: 0x001AAC6F File Offset: 0x001A8E6F
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
