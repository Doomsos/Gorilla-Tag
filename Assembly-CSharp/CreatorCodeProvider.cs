using System;
using Cosmetics;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class CreatorCodeProvider : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060000D8 RID: 216 RVA: 0x0000592B File Offset: 0x00003B2B
	string ICreatorCodeProvider.TerminalId
	{
		get
		{
			return this.nexusCreatorCode.GroupId.Code + this.nexusCreatorCode.Code;
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x0000594D File Offset: 0x00003B4D
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusCreatorCode == null)
		{
			Debug.LogError("The CreatorCodeProvider component on " + base.name + " must be assigned a nexusCreatorCode.");
			return false;
		}
		return true;
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000597A File Offset: 0x00003B7A
	void ICreatorCodeProvider.GetCreatorCode(out string code, out NexusGroupId[] groups)
	{
		code = this.nexusCreatorCode.Code;
		groups = new NexusGroupId[]
		{
			this.nexusCreatorCode.GroupId
		};
	}

	// Token: 0x040000EF RID: 239
	[SerializeField]
	private NexusCreatorCode nexusCreatorCode;
}
