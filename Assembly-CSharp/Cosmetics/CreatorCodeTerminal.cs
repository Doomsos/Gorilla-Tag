using System;
using TMPro;
using UnityEngine;

namespace Cosmetics
{
	// Token: 0x02000FC6 RID: 4038
	public class CreatorCodeTerminal : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
	{
		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x0600665A RID: 26202 RVA: 0x00215E7E File Offset: 0x0021407E
		public NexusGroupId[] NexusGroups
		{
			get
			{
				return this.nexusGroups;
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x0600665B RID: 26203 RVA: 0x00215E86 File Offset: 0x00214086
		public string TerminalId
		{
			get
			{
				return this.termId;
			}
		}

		// Token: 0x0600665C RID: 26204 RVA: 0x00215E90 File Offset: 0x00214090
		public void Awake()
		{
			this.termId = string.Empty;
			for (int i = 0; i < this.nexusGroups.Length; i++)
			{
				this.termId += this.nexusGroups[i].Code;
			}
			this.HookupToCreatorCodes();
		}

		// Token: 0x0600665D RID: 26205 RVA: 0x00215EDF File Offset: 0x002140DF
		private void OnDestroy()
		{
			this.UnhookFromCreatorCodes();
		}

		// Token: 0x0600665E RID: 26206 RVA: 0x00215EE8 File Offset: 0x002140E8
		public void HookupToCreatorCodes()
		{
			CreatorCodes.InitializedEvent += new Action(this.OnCreatorCodesInitialized);
			CreatorCodes.OnCreatorCodeChangedEvent += new Action<string>(this.OnCreatorCodeChanged);
			CreatorCodes.OnCreatorCodeFailureEvent += new Action<string>(this.OnCreatorCodeFailure);
			if (CreatorCodes.Intialized)
			{
				this.OnCreatorCodesInitialized();
			}
		}

		// Token: 0x0600665F RID: 26207 RVA: 0x00215F35 File Offset: 0x00214135
		public void UnhookFromCreatorCodes()
		{
			CreatorCodes.InitializedEvent -= new Action(this.OnCreatorCodesInitialized);
			CreatorCodes.OnCreatorCodeChangedEvent -= new Action<string>(this.OnCreatorCodeChanged);
			CreatorCodes.OnCreatorCodeFailureEvent -= new Action<string>(this.OnCreatorCodeFailure);
		}

		// Token: 0x06006660 RID: 26208 RVA: 0x00215F6A File Offset: 0x0021416A
		private void OnCreatorCodesInitialized()
		{
			this.OnCreatorCodeChanged(this.termId);
		}

		// Token: 0x06006661 RID: 26209 RVA: 0x00215F78 File Offset: 0x00214178
		public void OnCreatorCodeChanged(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode(this.termId);
			string text = "CREATOR CODE:";
			CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus(this.termId);
			if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
			{
				if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
				{
					text += " VALID";
				}
			}
			else
			{
				text += " VALIDATING";
			}
			this.creatorCodeTitle.text = text;
		}

		// Token: 0x06006662 RID: 26210 RVA: 0x00215FEA File Offset: 0x002141EA
		public void CreatorCodeInput(string character)
		{
			CreatorCodes.AppendKey(this.termId, character);
		}

		// Token: 0x06006663 RID: 26211 RVA: 0x00215FF8 File Offset: 0x002141F8
		public void CreatorCodeDelete()
		{
			CreatorCodes.DeleteCharacter(this.termId);
		}

		// Token: 0x06006664 RID: 26212 RVA: 0x00216005 File Offset: 0x00214205
		public void OnCreatorCodeValid(string id, string s, NexusGroupId ngid)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALID";
		}

		// Token: 0x06006665 RID: 26213 RVA: 0x00216026 File Offset: 0x00214226
		public void OnCreatorCodeValidating(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}

		// Token: 0x06006666 RID: 26214 RVA: 0x00216047 File Offset: 0x00214247
		public void CreatorCodeInvalid(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		// Token: 0x06006667 RID: 26215 RVA: 0x00216047 File Offset: 0x00214247
		public void OnCreatorCodeFailure(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		// Token: 0x06006668 RID: 26216 RVA: 0x00216068 File Offset: 0x00214268
		bool IBuildValidation.BuildValidationCheck()
		{
			if (this.nexusGroups.Length == 0)
			{
				Debug.LogError("You have to set at least one nexus group in " + base.name + " or things will not work!");
				return false;
			}
			return true;
		}

		// Token: 0x06006669 RID: 26217 RVA: 0x00216090 File Offset: 0x00214290
		public void GetCreatorCode(out string code, out NexusGroupId[] groups)
		{
			code = CreatorCodes.getCurrentCreatorCode(this.termId);
			groups = this.nexusGroups;
		}

		// Token: 0x0400750E RID: 29966
		private string termId;

		// Token: 0x0400750F RID: 29967
		[SerializeField]
		private TMP_Text creatorCodeField;

		// Token: 0x04007510 RID: 29968
		[SerializeField]
		private TMP_Text creatorCodeTitle;

		// Token: 0x04007511 RID: 29969
		[SerializeField]
		private NexusGroupId[] nexusGroups;
	}
}
