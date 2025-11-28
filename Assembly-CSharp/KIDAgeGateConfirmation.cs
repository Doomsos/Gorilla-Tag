using System;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000A3B RID: 2619
public class KIDAgeGateConfirmation : MonoBehaviour
{
	// Token: 0x1700063B RID: 1595
	// (get) Token: 0x0600423C RID: 16956 RVA: 0x0015E864 File Offset: 0x0015CA64
	private IntVariable UserAgeVar
	{
		get
		{
			if (this._userAgeVar == null)
			{
				this._userAgeVar = (this._localizedTextBody.StringReference["user-age"] as IntVariable);
				if (this._userAgeVar == null)
				{
					Debug.LogError("[Localization::KID_AGE_GATE_CONFIRMATION] Failed to get [user-age] smart variable as IntVariable");
				}
			}
			return this._userAgeVar;
		}
	}

	// Token: 0x1700063C RID: 1596
	// (get) Token: 0x0600423D RID: 16957 RVA: 0x0015E8B1 File Offset: 0x0015CAB1
	// (set) Token: 0x0600423E RID: 16958 RVA: 0x0015E8B9 File Offset: 0x0015CAB9
	public KidAgeConfirmationResult Result { get; private set; }

	// Token: 0x0600423F RID: 16959 RVA: 0x0015E8C2 File Offset: 0x0015CAC2
	private void Start()
	{
		this.Result = KidAgeConfirmationResult.None;
	}

	// Token: 0x06004240 RID: 16960 RVA: 0x0015E8CB File Offset: 0x0015CACB
	public void OnConfirm()
	{
		this.Result = KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x06004241 RID: 16961 RVA: 0x0015E8D4 File Offset: 0x0015CAD4
	public void OnBack()
	{
		this.Result = KidAgeConfirmationResult.Back;
	}

	// Token: 0x06004242 RID: 16962 RVA: 0x0015E8DD File Offset: 0x0015CADD
	public void Reset(int userAge)
	{
		this.Result = KidAgeConfirmationResult.None;
		if (this.UserAgeVar == null)
		{
			Debug.LogError("[LOCALIZATION::KID_AGE_GATE_CONFIRMATION] Unable to update [UserAgeVar] value, as it is null");
			return;
		}
		this.UserAgeVar.Value = userAge;
	}

	// Token: 0x04005360 RID: 21344
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _localizedTextBody;

	// Token: 0x04005361 RID: 21345
	private IntVariable _userAgeVar;
}
