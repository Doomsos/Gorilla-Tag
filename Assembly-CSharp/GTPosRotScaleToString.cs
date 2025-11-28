using System;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x0200031E RID: 798
public static class GTPosRotScaleToString
{
	// Token: 0x06001361 RID: 4961 RVA: 0x00070304 File Offset: 0x0006E504
	public static string ToString(Vector3 pos, Vector3 rot, Vector3 scale, bool isWorldSpace, string parentPath = null)
	{
		string text = isWorldSpace ? "WorldPRS" : "LocalPRS";
		string text2 = string.Concat(new string[]
		{
			text,
			" { p=",
			GTPosRotScaleToString.ValToStr(pos),
			", r=",
			GTPosRotScaleToString.ValToStr(rot),
			", s=",
			GTPosRotScaleToString.ValToStr(scale)
		});
		if (!string.IsNullOrEmpty(parentPath))
		{
			text2 = text2 + " parent=\"" + parentPath + "\"";
		}
		return text2 + " }";
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x0007038D File Offset: 0x0006E58D
	private static string ValToStr(Vector3 v)
	{
		return string.Format("({0:R}, {1:R}, {2:R})", v.x, v.y, v.z);
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x000703BA File Offset: 0x0006E5BA
	public static bool ParseIsWorldSpace(string input)
	{
		return input.Contains("WorldPRS");
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x000703C8 File Offset: 0x0006E5C8
	public static string ParseParentPath(string input)
	{
		MatchCollection matchCollection = Regex.Matches(input, "parent\\s*=\\s*\"(?<parent>.*?)\"");
		if (matchCollection.Count <= 0)
		{
			return null;
		}
		return matchCollection[0].Groups["parent"].Value;
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x00070407 File Offset: 0x0006E607
	public static bool TryParsePos(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Pos, input, out v);
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x00070415 File Offset: 0x0006E615
	public static bool TryParseRot(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Rot, input, out v);
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00070423 File Offset: 0x0006E623
	public static bool TryParseScale(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Scale, input, out v) || GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x00070441 File Offset: 0x0006E641
	public static bool TryParseVec3(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x00070450 File Offset: 0x0006E650
	private static bool TryParseVec3_internal(Regex regex, string input, out Vector3 v)
	{
		v = Vector3.zero;
		MatchCollection matchCollection = regex.Matches(input);
		if (matchCollection.Count <= 0)
		{
			return false;
		}
		v = GTPosRotScaleToString.StringToVector3(matchCollection[0]);
		return true;
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x00070490 File Offset: 0x0006E690
	private static Vector3 StringToVector3(Match match)
	{
		float num = float.Parse(match.Groups["x"].Value);
		float num2 = float.Parse(match.Groups["y"].Value);
		float num3 = float.Parse(match.Groups["z"].Value);
		return new Vector3(num, num2, num3);
	}

	// Token: 0x04001CE0 RID: 7392
	public const string k_LocalPRSLabel = "LocalPRS";

	// Token: 0x04001CE1 RID: 7393
	public const string k_WorldPRSLabel = "WorldPRS";
}
