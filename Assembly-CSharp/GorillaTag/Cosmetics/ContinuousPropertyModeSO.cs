using System;
using System.Linq;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010D1 RID: 4305
	public class ContinuousPropertyModeSO : ScriptableObject
	{
		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06006BF1 RID: 27633 RVA: 0x00236D2E File Offset: 0x00234F2E
		private string GetTestDescription
		{
			get
			{
				if (this.castData.Length == 0)
				{
					return "";
				}
				return "Sample Description: " + this.GetDescriptionForCast(this.castData[0].target);
			}
		}

		// Token: 0x06006BF2 RID: 27634 RVA: 0x00236D60 File Offset: 0x00234F60
		public bool IsCastValid(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006BF3 RID: 27635 RVA: 0x00236D9C File Offset: 0x00234F9C
		public ContinuousProperty.Cast GetClosestCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return this.castData[i].target;
				}
			}
			return ContinuousProperty.Cast.Null;
		}

		// Token: 0x06006BF4 RID: 27636 RVA: 0x00236DE8 File Offset: 0x00234FE8
		public ContinuousProperty.DataFlags GetFlagsForCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (this.castData[i].target == cast)
				{
					return this.castData[i].additionalFlags | this.flags;
				}
			}
			return this.flags;
		}

		// Token: 0x06006BF5 RID: 27637 RVA: 0x00236E3C File Offset: 0x0023503C
		public ContinuousProperty.DataFlags GetFlagsForClosestCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return this.castData[i].additionalFlags | this.flags;
				}
			}
			return this.flags;
		}

		// Token: 0x06006BF6 RID: 27638 RVA: 0x00236E94 File Offset: 0x00235094
		public string GetDescriptionForCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast) || this.castData.Length == 1)
				{
					if (!this.replaceDescription.IsNullOrEmpty())
					{
						return this.replaceDescription;
					}
					switch (this.descriptionStyle)
					{
					case ContinuousPropertyModeSO.DescriptionStyle.Continuous:
						return string.Concat(new string[]
						{
							"sets the ",
							this.castData[i].whatItSets,
							" on the ",
							this.castData[i].target.ToString(),
							" using the height of the curve at the provided time.",
							(" " + this.afterSentence).TrimEnd()
						});
					case ContinuousPropertyModeSO.DescriptionStyle.SingleThreshold:
						return this.castData[i].whatItSets + " the " + this.type.ToString() + " when entering the 'true' part of the range.";
					case ContinuousPropertyModeSO.DescriptionStyle.DualThreshold:
					{
						string[] array = this.castData[i].whatItSets.Split('|', 0);
						if (array.Length != 2)
						{
							return string.Format("Error! '{0}'s '{1}.{2}' does not have two string separated by '|'.", base.name, this.castData[i].target, "whatItSets");
						}
						return string.Concat(new string[]
						{
							array[0],
							" the ",
							this.castData[i].target.ToString(),
							" when entering the 'true' part of the range, ",
							array[1],
							" the ",
							this.castData[i].target.ToString(),
							" when entering the 'false' part of the range."
						});
					}
					}
				}
			}
			return "Invalid target\n\n" + this.ListValidCasts();
		}

		// Token: 0x06006BF7 RID: 27639 RVA: 0x00237086 File Offset: 0x00235286
		public string ListValidCasts()
		{
			return "Valid targets: " + string.Join<ContinuousProperty.Cast>(", ", Enumerable.Select<ContinuousPropertyModeSO.CastData, ContinuousProperty.Cast>(this.castData, (ContinuousPropertyModeSO.CastData x) => x.target));
		}

		// Token: 0x04007C8D RID: 31885
		public ContinuousProperty.Type type;

		// Token: 0x04007C8E RID: 31886
		public ContinuousProperty.DataFlags flags;

		// Token: 0x04007C8F RID: 31887
		public ContinuousPropertyModeSO.CastData[] castData;

		// Token: 0x04007C90 RID: 31888
		[Space]
		public ContinuousPropertyModeSO.DescriptionStyle descriptionStyle;

		// Token: 0x04007C91 RID: 31889
		[TextArea]
		public string afterSentence;

		// Token: 0x04007C92 RID: 31890
		[TextArea]
		public string replaceDescription;

		// Token: 0x020010D2 RID: 4306
		[Serializable]
		public struct CastData
		{
			// Token: 0x04007C93 RID: 31891
			public ContinuousProperty.Cast target;

			// Token: 0x04007C94 RID: 31892
			public ContinuousProperty.DataFlags additionalFlags;

			// Token: 0x04007C95 RID: 31893
			public string whatItSets;
		}

		// Token: 0x020010D3 RID: 4307
		public enum DescriptionStyle
		{
			// Token: 0x04007C97 RID: 31895
			Continuous,
			// Token: 0x04007C98 RID: 31896
			SingleThreshold,
			// Token: 0x04007C99 RID: 31897
			DualThreshold
		}
	}
}
