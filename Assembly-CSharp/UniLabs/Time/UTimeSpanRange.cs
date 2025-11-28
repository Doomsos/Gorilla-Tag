using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000D78 RID: 3448
	[JsonObject(1)]
	[Serializable]
	public class UTimeSpanRange
	{
		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x0600547F RID: 21631 RVA: 0x001AAC77 File Offset: 0x001A8E77
		// (set) Token: 0x06005480 RID: 21632 RVA: 0x001AAC84 File Offset: 0x001A8E84
		public TimeSpan Start
		{
			get
			{
				return this._Start;
			}
			set
			{
				this._Start = value;
			}
		}

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x06005481 RID: 21633 RVA: 0x001AAC92 File Offset: 0x001A8E92
		// (set) Token: 0x06005482 RID: 21634 RVA: 0x001AAC9F File Offset: 0x001A8E9F
		public TimeSpan End
		{
			get
			{
				return this._End;
			}
			set
			{
				this._End = value;
			}
		}

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x06005483 RID: 21635 RVA: 0x001AACAD File Offset: 0x001A8EAD
		public TimeSpan Duration
		{
			get
			{
				return this.End - this.Start;
			}
		}

		// Token: 0x06005484 RID: 21636 RVA: 0x001AACC0 File Offset: 0x001A8EC0
		public bool IsInRange(TimeSpan time)
		{
			return time >= this.Start && time <= this.End;
		}

		// Token: 0x06005485 RID: 21637 RVA: 0x00002050 File Offset: 0x00000250
		[JsonConstructor]
		public UTimeSpanRange()
		{
		}

		// Token: 0x06005486 RID: 21638 RVA: 0x001AACDE File Offset: 0x001A8EDE
		public UTimeSpanRange(TimeSpan start)
		{
			this._Start = start;
			this._End = start;
		}

		// Token: 0x06005487 RID: 21639 RVA: 0x001AACFE File Offset: 0x001A8EFE
		public UTimeSpanRange(TimeSpan start, TimeSpan end)
		{
			this._Start = start;
			this._End = end;
		}

		// Token: 0x06005488 RID: 21640 RVA: 0x001AAD1E File Offset: 0x001A8F1E
		private void OnStartChanged()
		{
			if (this._Start.CompareTo(this._End) > 0)
			{
				this._End.TimeSpan = this._Start.TimeSpan;
			}
		}

		// Token: 0x06005489 RID: 21641 RVA: 0x001AAD4A File Offset: 0x001A8F4A
		private void OnEndChanged()
		{
			if (this._End.CompareTo(this._Start) < 0)
			{
				this._Start.TimeSpan = this._End.TimeSpan;
			}
		}

		// Token: 0x040061CF RID: 25039
		[JsonProperty("Start")]
		[SerializeField]
		private UTimeSpan _Start;

		// Token: 0x040061D0 RID: 25040
		[JsonProperty("End")]
		[SerializeField]
		private UTimeSpan _End;
	}
}
