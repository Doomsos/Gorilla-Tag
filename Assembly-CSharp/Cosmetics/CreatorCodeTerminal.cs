using System;
using TMPro;
using UnityEngine;

namespace Cosmetics
{
	public class CreatorCodeTerminal : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
	{
		public NexusGroupId[] NexusGroups
		{
			get
			{
				return this.nexusGroups;
			}
		}

		public string TerminalId
		{
			get
			{
				return this.termId;
			}
		}

		public void Awake()
		{
			this.termId = string.Empty;
			for (int i = 0; i < this.nexusGroups.Length; i++)
			{
				this.termId += this.nexusGroups[i].Code;
			}
			this.HookupToCreatorCodes();
		}

		private void OnDestroy()
		{
			this.UnhookFromCreatorCodes();
		}

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

		public void UnhookFromCreatorCodes()
		{
			CreatorCodes.InitializedEvent -= new Action(this.OnCreatorCodesInitialized);
			CreatorCodes.OnCreatorCodeChangedEvent -= new Action<string>(this.OnCreatorCodeChanged);
			CreatorCodes.OnCreatorCodeFailureEvent -= new Action<string>(this.OnCreatorCodeFailure);
		}

		private void OnCreatorCodesInitialized()
		{
			this.OnCreatorCodeChanged(this.termId);
		}

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

		public void CreatorCodeInput(string character)
		{
			CreatorCodes.AppendKey(this.termId, character);
		}

		public void CreatorCodeDelete()
		{
			CreatorCodes.DeleteCharacter(this.termId);
		}

		public void OnCreatorCodeValid(string id, string s, NexusGroupId ngid)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALID";
		}

		public void OnCreatorCodeValidating(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}

		public void CreatorCodeInvalid(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		public void OnCreatorCodeFailure(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		bool IBuildValidation.BuildValidationCheck()
		{
			if (this.nexusGroups.Length == 0)
			{
				Debug.LogError("You have to set at least one nexus group in " + base.name + " or things will not work!");
				return false;
			}
			return true;
		}

		public void GetCreatorCode(out string code, out NexusGroupId[] groups)
		{
			code = CreatorCodes.getCurrentCreatorCode(this.termId);
			groups = this.nexusGroups;
		}

		private string termId;

		[SerializeField]
		private TMP_Text creatorCodeField;

		[SerializeField]
		private TMP_Text creatorCodeTitle;

		[SerializeField]
		private NexusGroupId[] nexusGroups;
	}
}
