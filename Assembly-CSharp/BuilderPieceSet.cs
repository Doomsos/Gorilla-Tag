using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

// Token: 0x0200058B RID: 1419
[CreateAssetMenu(fileName = "BuilderPieceSet01", menuName = "Gorilla Tag/Builder/PieceSet", order = 0)]
public class BuilderPieceSet : ScriptableObject
{
	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002400 RID: 9216 RVA: 0x000BF539 File Offset: 0x000BD739
	public string SetName
	{
		get
		{
			return this.setName;
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000BF541 File Offset: 0x000BD741
	public int GetIntIdentifier()
	{
		return this.playfabID.GetStaticHash();
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000BF550 File Offset: 0x000BD750
	public DateTime GetScheduleDateTime()
	{
		if (this.isScheduled)
		{
			try
			{
				return DateTime.Parse(this.scheduledDate, CultureInfo.InvariantCulture);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
		return DateTime.MinValue;
	}

	// Token: 0x04002EFA RID: 12026
	[Tooltip("Display Name - Fallback for Localization")]
	public string setName;

	// Token: 0x04002EFB RID: 12027
	public GameObject displayModel;

	// Token: 0x04002EFC RID: 12028
	[Tooltip("If this should error if no localization is found")]
	public bool isLocalized;

	// Token: 0x04002EFD RID: 12029
	[Tooltip("Localized Display Name")]
	public LocalizedString setLocName;

	// Token: 0x04002EFE RID: 12030
	[FormerlySerializedAs("uniqueId")]
	[Tooltip("If purchaseable, this should be a valid playfabID starting with LD\nIf a starter set, this just needs to be a unique string from the other set IDs")]
	public string playfabID;

	// Token: 0x04002EFF RID: 12031
	[Tooltip("(Optional) Default Material ID applied to all prefabs with BuilderMaterialOptions")]
	public string materialId;

	// Token: 0x04002F00 RID: 12032
	[Tooltip("(Optional) If this set is not available on launch day use scheduling")]
	public bool isScheduled;

	// Token: 0x04002F01 RID: 12033
	public string scheduledDate = "1/1/0001 00:00:00";

	// Token: 0x04002F02 RID: 12034
	[Tooltip("A group of pieces on the same shelf")]
	public List<BuilderPieceSet.BuilderPieceSubset> subsets;

	// Token: 0x0200058C RID: 1420
	public enum BuilderPieceCategory
	{
		// Token: 0x04002F04 RID: 12036
		FLAT,
		// Token: 0x04002F05 RID: 12037
		TALL,
		// Token: 0x04002F06 RID: 12038
		HALF_HEIGHT,
		// Token: 0x04002F07 RID: 12039
		BEAM,
		// Token: 0x04002F08 RID: 12040
		SLOPE,
		// Token: 0x04002F09 RID: 12041
		OVERSIZED,
		// Token: 0x04002F0A RID: 12042
		SPECIAL_DISPLAY,
		// Token: 0x04002F0B RID: 12043
		FUNCTIONAL = 18,
		// Token: 0x04002F0C RID: 12044
		DECORATIVE,
		// Token: 0x04002F0D RID: 12045
		MISC
	}

	// Token: 0x0200058D RID: 1421
	[Serializable]
	public class BuilderPieceSubset
	{
		// Token: 0x06002404 RID: 9220 RVA: 0x000BF5AB File Offset: 0x000BD7AB
		public string GetShelfButtonName()
		{
			return this.shelfButtonName;
		}

		// Token: 0x04002F0E RID: 12046
		[Tooltip("(Optional) Text to put on the shelf button if not the set name")]
		public string shelfButtonName;

		// Token: 0x04002F0F RID: 12047
		public LocalizedString localizedShelfButtonName;

		// Token: 0x04002F10 RID: 12048
		public BuilderPieceSet.BuilderPieceCategory pieceCategory;

		// Token: 0x04002F11 RID: 12049
		public List<BuilderPieceSet.PieceInfo> pieceInfos;
	}

	// Token: 0x0200058E RID: 1422
	[Serializable]
	public struct PieceInfo
	{
		// Token: 0x04002F12 RID: 12050
		public BuilderPiece piecePrefab;

		// Token: 0x04002F13 RID: 12051
		[Tooltip("(Optional) should this piece use a materialID other than the set's materialID")]
		public bool overrideSetMaterial;

		// Token: 0x04002F14 RID: 12052
		[Tooltip("material type string should match an entry in this prefab's BuilderMaterialOptions\nIf multiple are in the list the piece will cycle through materials when spawned\nTo have each variant on the shelf create a new pieceInfo for each color")]
		public string[] pieceMaterialTypes;
	}

	// Token: 0x0200058F RID: 1423
	public class BuilderDisplayGroup
	{
		// Token: 0x06002406 RID: 9222 RVA: 0x000BF5B3 File Offset: 0x000BD7B3
		public BuilderDisplayGroup()
		{
			this.displayName = string.Empty;
			this.pieceSubsets = new List<BuilderPieceSet.BuilderPieceSubset>();
			this.defaultMaterial = string.Empty;
			this.setID = -1;
			this.uniqueGroupID = string.Empty;
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x000BF5EE File Offset: 0x000BD7EE
		public BuilderDisplayGroup(string groupName, string material, int inSetID, string groupID)
		{
			this.displayName = groupName;
			this.pieceSubsets = new List<BuilderPieceSet.BuilderPieceSubset>();
			this.defaultMaterial = material;
			this.setID = inSetID;
			this.uniqueGroupID = groupID;
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x000BF61E File Offset: 0x000BD81E
		public int GetDisplayGroupIdentifier()
		{
			return this.uniqueGroupID.GetStaticHash();
		}

		// Token: 0x04002F15 RID: 12053
		public string displayName;

		// Token: 0x04002F16 RID: 12054
		public List<BuilderPieceSet.BuilderPieceSubset> pieceSubsets;

		// Token: 0x04002F17 RID: 12055
		public string defaultMaterial;

		// Token: 0x04002F18 RID: 12056
		public int setID;

		// Token: 0x04002F19 RID: 12057
		public string uniqueGroupID;
	}
}
