using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000A9A RID: 2714
public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	// Token: 0x06004430 RID: 17456 RVA: 0x0016921F File Offset: 0x0016741F
	private void Awake()
	{
		this.CheckLocalizationReferences();
	}

	// Token: 0x06004431 RID: 17457 RVA: 0x00169228 File Offset: 0x00167428
	public Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__8 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.description = description;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__8>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x06004432 RID: 17458 RVA: 0x00169274 File Offset: 0x00167474
	public Task ShowAgeDiscrepancyScreenWithAwait(int userAge, int accAge, int lowestAge)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__9 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.userAge = userAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.accAge = accAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.lowestAge = lowestAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__9>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x06004433 RID: 17459 RVA: 0x001692D0 File Offset: 0x001674D0
	private Task WaitForCompletion()
	{
		KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__10 <WaitForCompletion>d__;
		<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForCompletion>d__.<>4__this = this;
		<WaitForCompletion>d__.<>1__state = -1;
		<WaitForCompletion>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__10>(ref <WaitForCompletion>d__);
		return <WaitForCompletion>d__.<>t__builder.Task;
	}

	// Token: 0x06004434 RID: 17460 RVA: 0x00169313 File Offset: 0x00167513
	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	// Token: 0x06004435 RID: 17461 RVA: 0x0016893E File Offset: 0x00166B3E
	public void OnQuitPressed()
	{
		Application.Quit();
	}

	// Token: 0x06004436 RID: 17462 RVA: 0x0016931C File Offset: 0x0016751C
	private void CheckLocalizationReferences()
	{
		if (this._bodyLocStr != null && this._userAgeVar != null && this._accountAgeVar != null && this._lowestAgeVar != null)
		{
			return;
		}
		if (this._bodyTextLoc == null)
		{
			Debug.LogError("[LOCALIZATION::KIDUI_AGE_DISCREPANCY_SCREEN] [_bodyTextLoc] is not set, unable to localize smart string");
			return;
		}
		this._bodyLocStr = this._bodyTextLoc.StringReference;
		this._userAgeVar = (this._bodyLocStr["user-age"] as IntVariable);
		this._accountAgeVar = (this._bodyLocStr["account-age"] as IntVariable);
		this._lowestAgeVar = (this._bodyLocStr["lowest-age"] as IntVariable);
	}

	// Token: 0x040055B4 RID: 21940
	[SerializeField]
	private TMP_Text _descriptionText;

	// Token: 0x040055B5 RID: 21941
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _bodyTextLoc;

	// Token: 0x040055B6 RID: 21942
	private bool _hasCompleted;

	// Token: 0x040055B7 RID: 21943
	private LocalizedString _bodyLocStr;

	// Token: 0x040055B8 RID: 21944
	private IntVariable _userAgeVar;

	// Token: 0x040055B9 RID: 21945
	private IntVariable _accountAgeVar;

	// Token: 0x040055BA RID: 21946
	private IntVariable _lowestAgeVar;
}
