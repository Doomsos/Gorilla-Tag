using System;

namespace BoingKit
{
	// Token: 0x02001188 RID: 4488
	public struct Version : IEquatable<Version>
	{
		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x0600713B RID: 28987 RVA: 0x00251C60 File Offset: 0x0024FE60
		public readonly int MajorVersion { get; }

		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x0600713C RID: 28988 RVA: 0x00251C68 File Offset: 0x0024FE68
		public readonly int MinorVersion { get; }

		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x0600713D RID: 28989 RVA: 0x00251C70 File Offset: 0x0024FE70
		public readonly int Revision { get; }

		// Token: 0x0600713E RID: 28990 RVA: 0x00251C78 File Offset: 0x0024FE78
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.MajorVersion.ToString(),
				".",
				this.MinorVersion.ToString(),
				".",
				this.Revision.ToString()
			});
		}

		// Token: 0x0600713F RID: 28991 RVA: 0x00251CD3 File Offset: 0x0024FED3
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x06007140 RID: 28992 RVA: 0x00251CF5 File Offset: 0x0024FEF5
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x06007141 RID: 28993 RVA: 0x00251D0C File Offset: 0x0024FF0C
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x06007142 RID: 28994 RVA: 0x00251D61 File Offset: 0x0024FF61
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x06007143 RID: 28995 RVA: 0x00251D6D File Offset: 0x0024FF6D
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x06007144 RID: 28996 RVA: 0x00251D85 File Offset: 0x0024FF85
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x06007145 RID: 28997 RVA: 0x00251DB8 File Offset: 0x0024FFB8
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x040081BF RID: 33215
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x040081C0 RID: 33216
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x040081C1 RID: 33217
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
