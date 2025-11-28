using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000EA1 RID: 3745
	public class CurrencyBoard : MonoBehaviour
	{
		// Token: 0x06005DAA RID: 23978 RVA: 0x001E1350 File Offset: 0x001DF550
		public void OnEnable()
		{
			CosmeticsController.instance.AddCurrencyBoard(this);
		}

		// Token: 0x06005DAB RID: 23979 RVA: 0x001E135F File Offset: 0x001DF55F
		public void OnDisable()
		{
			CosmeticsController.instance.RemoveCurrencyBoard(this);
		}

		// Token: 0x06005DAC RID: 23980 RVA: 0x001E1370 File Offset: 0x001DF570
		public void UpdateCurrencyBoard(bool checkedDaily, bool gotDaily, int currencyBalance, int secTilTomorrow)
		{
			if (this.dailyRocksTextTMP.IsNotNull())
			{
				this.dailyRocksTextTMP.text = (checkedDaily ? (gotDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...") : "CHECKING DAILY ROCKS...");
			}
			if (this.currencyBoardTextTMP.IsNotNull())
			{
				this.currencyBoardTextTMP.text = string.Concat(new string[]
				{
					currencyBalance.ToString(),
					"\n\n",
					(secTilTomorrow / 3600).ToString(),
					" HR, ",
					(secTilTomorrow % 3600 / 60).ToString(),
					"MIN"
				});
			}
		}

		// Token: 0x04006BAF RID: 27567
		public TMP_Text dailyRocksTextTMP;

		// Token: 0x04006BB0 RID: 27568
		public TMP_Text currencyBoardTextTMP;
	}
}
