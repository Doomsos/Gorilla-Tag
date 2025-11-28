using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EDF RID: 3807
	public class CreditsView : MonoBehaviour
	{
		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06005F28 RID: 24360 RVA: 0x001E94B9 File Offset: 0x001E76B9
		private int TotalPages
		{
			get
			{
				return Enumerable.Sum<CreditsSection>(this.creditsSections, (CreditsSection section) => this.PagesPerSection(section));
			}
		}

		// Token: 0x06005F29 RID: 24361 RVA: 0x001E94D4 File Offset: 0x001E76D4
		private void Start()
		{
			CreditsSection[] array = new CreditsSection[3];
			int num = 0;
			CreditsSection creditsSection = new CreditsSection();
			creditsSection.Title = "DEV TEAM";
			List<string> list = new List<string>();
			list.Add("Anton \"NtsFranz\" Franzluebbers");
			list.Add("Carlo Grossi Jr");
			list.Add("Cody O'Quinn");
			list.Add("David Neubelt");
			list.Add("David \"AA_DavidY\" Yee");
			list.Add("Derek \"DunkTrain\" Arabian");
			list.Add("Elie Arabian");
			list.Add("John Sleeper");
			list.Add("Haunted Army");
			list.Add("Kerestell Smith");
			list.Add("Keith \"ElectronicWall\" Taylor");
			list.Add("Laura \"Poppy\" Lorian");
			list.Add("Lilly Tothill");
			list.Add("Matt \"Crimity\" Ostgard");
			list.Add("Nick Taylor");
			list.Add("Ross Furmidge");
			list.Add("Sasha \"Kayze\" Sanders");
			creditsSection.Entries = list;
			array[num] = creditsSection;
			int num2 = 1;
			CreditsSection creditsSection2 = new CreditsSection();
			creditsSection2.Title = "SPECIAL THANKS";
			List<string> list2 = new List<string>();
			list2.Add("The \"Sticks\"");
			list2.Add("Alpha Squad");
			list2.Add("Meta");
			list2.Add("Scout House");
			list2.Add("Mighty PR");
			list2.Add("Caroline Arabian");
			list2.Add("Clarissa & Declan");
			list2.Add("Calum Haigh");
			list2.Add("EZ ICE");
			list2.Add("Gwen");
			creditsSection2.Entries = list2;
			array[num2] = creditsSection2;
			int num3 = 2;
			CreditsSection creditsSection3 = new CreditsSection();
			creditsSection3.Title = "MUSIC BY";
			List<string> list3 = new List<string>();
			list3.Add("Stunshine");
			list3.Add("David Anderson Kirk");
			list3.Add("Jaguar Jen");
			list3.Add("Audiopfeil");
			list3.Add("Owlobe");
			creditsSection3.Entries = list3;
			array[num3] = creditsSection3;
			this.creditsSections = array;
			PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", delegate(string result)
			{
				this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error fetching credits data: " + error.ErrorMessage);
			}, false);
		}

		// Token: 0x06005F2A RID: 24362 RVA: 0x001E96E2 File Offset: 0x001E78E2
		private int PagesPerSection(CreditsSection section)
		{
			return (int)Math.Ceiling((double)section.Entries.Count / (double)this.pageSize);
		}

		// Token: 0x06005F2B RID: 24363 RVA: 0x001E96FE File Offset: 0x001E78FE
		private IEnumerable<string> PageOfSection(CreditsSection section, int page)
		{
			return Enumerable.Take<string>(Enumerable.Skip<string>(section.Entries, this.pageSize * page), this.pageSize);
		}

		// Token: 0x06005F2C RID: 24364 RVA: 0x001E9720 File Offset: 0x001E7920
		[return: TupleElementNames(new string[]
		{
			"creditsSection",
			"subPage"
		})]
		private ValueTuple<CreditsSection, int> GetPageEntries(int page)
		{
			int num = 0;
			foreach (CreditsSection creditsSection in this.creditsSections)
			{
				int num2 = this.PagesPerSection(creditsSection);
				if (num + num2 > page)
				{
					int num3 = page - num;
					return new ValueTuple<CreditsSection, int>(creditsSection, num3);
				}
				num += num2;
			}
			return new ValueTuple<CreditsSection, int>(Enumerable.First<CreditsSection>(this.creditsSections), 0);
		}

		// Token: 0x06005F2D RID: 24365 RVA: 0x001E977C File Offset: 0x001E797C
		public void ProcessButtonPress(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.currentPage++;
				this.currentPage %= this.TotalPages;
			}
		}

		// Token: 0x06005F2E RID: 24366 RVA: 0x001E97A4 File Offset: 0x001E79A4
		public string GetScreenText()
		{
			return this.GetPage(this.currentPage);
		}

		// Token: 0x06005F2F RID: 24367 RVA: 0x001E97B4 File Offset: 0x001E79B4
		private string GetPage(int page)
		{
			ValueTuple<CreditsSection, int> pageEntries = this.GetPageEntries(page);
			CreditsSection item = pageEntries.Item1;
			int item2 = pageEntries.Item2;
			IEnumerable<string> enumerable = this.PageOfSection(item, item2);
			string defaultResult = "CREDITS";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS", out text, defaultResult);
			defaultResult = "(CONT)";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_CONTINUED", out text2, defaultResult);
			string text3 = text + " - " + ((item2 == 0) ? item.Title : (item.Title + " " + text2));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(text3);
			stringBuilder.AppendLine();
			foreach (string text4 in enumerable)
			{
				stringBuilder.AppendLine(text4);
			}
			for (int i = 0; i < this.pageSize - Enumerable.Count<string>(enumerable); i++)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			defaultResult = "PRESS ENTER TO CHANGE PAGES";
			string text5;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_PRESS_ENTER", out text5, defaultResult);
			stringBuilder.AppendLine(text5);
			return stringBuilder.ToString();
		}

		// Token: 0x04006D08 RID: 27912
		private const string CREDITS_KEY = "CREDITS";

		// Token: 0x04006D09 RID: 27913
		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		// Token: 0x04006D0A RID: 27914
		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		// Token: 0x04006D0B RID: 27915
		private CreditsSection[] creditsSections;

		// Token: 0x04006D0C RID: 27916
		public int pageSize = 7;

		// Token: 0x04006D0D RID: 27917
		private int currentPage;

		// Token: 0x04006D0E RID: 27918
		private const string PlayFabKey = "CreditsData";
	}
}
