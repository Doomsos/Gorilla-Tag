using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Modio;
using Modio.Customizations;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps.ModIO
{
	// Token: 0x02000E29 RID: 3625
	public class ModIOTermsOfUse_v2 : LegalAgreements
	{
		// Token: 0x06005A7E RID: 23166 RVA: 0x001CFAEC File Offset: 0x001CDCEC
		protected override void Awake()
		{
			if (ModIOTermsOfUse_v2.modioTermsInstance != null)
			{
				Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
				base.gameObject.SetActive(false);
				return;
			}
			ModIOTermsOfUse_v2.modioTermsInstance = this;
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
			base.enabled = false;
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x001CFB44 File Offset: 0x001CDD44
		public Task<Error> ShowTerms()
		{
			ModIOTermsOfUse_v2.<ShowTerms>d__7 <ShowTerms>d__;
			<ShowTerms>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<ShowTerms>d__.<>4__this = this;
			<ShowTerms>d__.<>1__state = -1;
			<ShowTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<ShowTerms>d__7>(ref <ShowTerms>d__);
			return <ShowTerms>d__.<>t__builder.Task;
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x001CFB88 File Offset: 0x001CDD88
		public override Task StartLegalAgreements()
		{
			ModIOTermsOfUse_v2.<StartLegalAgreements>d__8 <StartLegalAgreements>d__;
			<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartLegalAgreements>d__.<>4__this = this;
			<StartLegalAgreements>d__.<>1__state = -1;
			<StartLegalAgreements>d__.<>t__builder.Start<ModIOTermsOfUse_v2.<StartLegalAgreements>d__8>(ref <StartLegalAgreements>d__);
			return <StartLegalAgreements>d__.<>t__builder.Task;
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x001CFBCC File Offset: 0x001CDDCC
		private void UpdateTextFromTerms()
		{
			this.tmpTitle.text = "Mod.io Terms of Use";
			this.tmpBody.text = "Loading...";
			this.cachedTermsText = this.termsOfUse.TermsText + "\n\n";
			this.cachedTermsText = this.cachedTermsText + this.FormatAgreementText(this.fullTermsOfUse) + "\n\n\n";
			this.cachedTermsText += this.FormatAgreementText(this.fullPrivacyPolicy);
			this.tmpBody.text = this.cachedTermsText;
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x001CFC64 File Offset: 0x001CDE64
		private string FormatAgreementText(Agreement agreement)
		{
			string text = string.Concat(new string[]
			{
				agreement.Name,
				"\n\nEffective Date: ",
				agreement.DateLive.ToLongDateString(),
				"\n\n",
				agreement.Content
			});
			text = Regex.Replace(text, "<!--[^>]*(-->)", "");
			text = text.Replace("<h1>", "<b>");
			text = text.Replace("</h1>", "</b>");
			text = text.Replace("<h2>", "<b>");
			text = text.Replace("</h2>", "</b>");
			text = text.Replace("<h3>", "<b>");
			text = text.Replace("</h3>", "</b>");
			text = text.Replace("<hr>", "");
			text = text.Replace("<br>", "\n");
			text = text.Replace("</li>", "</indent>\n");
			text = text.Replace("<strong>", "<b>");
			text = text.Replace("</strong>", "</b>");
			text = text.Replace("<em>", "<i>");
			text = text.Replace("</em>", "</i>");
			text = Regex.Replace(text, "<a[^>]*>{1}", "");
			text = text.Replace("</a>", "");
			Match match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
			while (match.Success)
			{
				text = text.Remove(match.Index, match.Length);
				text = text.Insert(match.Index, "\n<align=\"center\">");
				int num = text.IndexOf("</p>", match.Index, 4);
				text = text.Remove(num, 4);
				text = text.Insert(num, "</align>");
				match = Regex.Match(text, "<p[^>]*align:center[^>]*>{1}");
			}
			text = text.Replace("<p>", "\n");
			text = text.Replace("</p>", "");
			text = Regex.Replace(text, "<ol[^>]*>{1}", "<ol>");
			int num2 = text.IndexOf("<ol>", 5);
			bool flag = num2 != -1;
			while (flag)
			{
				int num3 = text.IndexOf("</ol>", num2, 5);
				text = text.Remove(num2, "<ol>".Length);
				int num4 = text.IndexOf("<li>", num2, 5);
				bool flag2 = num4 != -1;
				int num5 = 0;
				while (flag2)
				{
					text = text.Remove(num4, "<li>".Length);
					text = text.Insert(num4, this.GetStringForListItemIdx_LowerAlpha(num5++));
					num3 = text.IndexOf("</ol>", num2, 5);
					num4 = text.IndexOf("<li>", num2, 5);
					flag2 = (num4 != -1 && num4 < num3);
				}
				text = text.Remove(num3, "</ol>".Length);
				num2 = text.IndexOf("<ol>", 5);
				flag = (num2 != -1);
			}
			text = Regex.Replace(text, "<ul[^>]*>{1}", "<ul>");
			int num6 = text.IndexOf("<ul>", 5);
			bool flag3 = num6 != -1;
			while (flag3)
			{
				int num7 = text.IndexOf("</ul>", num6, 5);
				text = text.Remove(num6, "<ul>".Length);
				int num8 = text.IndexOf("<li>", num6, 5);
				bool flag4 = num8 != -1;
				while (flag4)
				{
					text = text.Remove(num8, "<li>".Length);
					text = text.Insert(num8, "  - <indent=5%>");
					num7 = text.IndexOf("</ul>", num6, 5);
					num8 = text.IndexOf("<li>", num6, 5);
					flag4 = (num8 != -1 && num8 < num7);
				}
				text = text.Remove(num7, "</ul>".Length);
				num6 = text.IndexOf("<ul>", 5);
				flag3 = (num6 != -1);
			}
			text = Regex.Replace(text, "<table[^>]*>{1}", "");
			text = text.Replace("<tbody>", "");
			text = text.Replace("<tr>", "");
			text = text.Replace("<td>", "");
			text = text.Replace("<center>", "");
			text = text.Replace("</table>", "");
			text = text.Replace("</tbody>", "");
			text = text.Replace("</tr>", "\n");
			text = text.Replace("</td>", "");
			return text.Replace("</center>", "");
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x001D00F0 File Offset: 0x001CE2F0
		private string GetStringForListItemIdx_LowerAlpha(int idx)
		{
			switch (idx)
			{
			case 0:
				return "  a. <indent=5%>";
			case 1:
				return "  b. <indent=5%>";
			case 2:
				return "  c. <indent=5%>";
			case 3:
				return "  d. <indent=5%>";
			case 4:
				return "  e. <indent=5%>";
			case 5:
				return "  f. <indent=5%>";
			case 6:
				return "  g. <indent=5%>";
			case 7:
				return "  h. <indent=5%>";
			case 8:
				return "  i. <indent=5%>";
			case 9:
				return "  j. <indent=5%>";
			case 10:
				return "  k. <indent=5%>";
			case 11:
				return "  l. <indent=5%>";
			case 12:
				return "  m. <indent=5%>";
			case 13:
				return "  n. <indent=5%>";
			case 14:
				return "  o. <indent=5%>";
			case 15:
				return "  p. <indent=5%>";
			case 16:
				return "  q. <indent=5%>";
			case 17:
				return "  r. <indent=5%>";
			case 18:
				return "  s. <indent=5%>";
			case 19:
				return "  t. <indent=5%>";
			case 20:
				return "  u. <indent=5%>";
			case 21:
				return "  v. <indent=5%>";
			case 22:
				return "  w. <indent=5%>";
			case 23:
				return "  x. <indent=5%>";
			case 24:
				return "  y. <indent=5%>";
			case 25:
				return "  z. <indent=5%>";
			default:
				return "";
			}
		}

		// Token: 0x040067B2 RID: 26546
		[SerializeField]
		private string confirmString = "Press and Hold to Confirm";

		// Token: 0x040067B3 RID: 26547
		private static ModIOTermsOfUse_v2 modioTermsInstance;

		// Token: 0x040067B4 RID: 26548
		private TermsOfUse termsOfUse;

		// Token: 0x040067B5 RID: 26549
		private Agreement fullTermsOfUse;

		// Token: 0x040067B6 RID: 26550
		private Agreement fullPrivacyPolicy;

		// Token: 0x040067B7 RID: 26551
		private string cachedTermsText;
	}
}
