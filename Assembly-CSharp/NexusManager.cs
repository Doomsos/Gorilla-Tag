using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NexusSDK;
using UnityEngine;

public class NexusManager : MonoBehaviour, IBuildValidation
{
	public NexusManager.Environment CurrentEnvironment
	{
		get
		{
			return this.environment;
		}
	}

	private void Awake()
	{
		if (NexusManager.instance == null)
		{
			this.environment = NexusManager.Environment.PRODUCTION;
			NexusManager.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	private void Start()
	{
		SDKInitializer.Init((this.environment == NexusManager.Environment.SANDBOX) ? "nexus_pk_ba155a8c229740489d214f024e25f25c" : "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb", (this.environment == NexusManager.Environment.SANDBOX) ? "sandbox" : "production");
	}

	public Task<Member> VerifyCreatorCode(string terminalId, string code, NexusGroupId id)
	{
		NexusManager.<VerifyCreatorCode>d__15 <VerifyCreatorCode>d__;
		<VerifyCreatorCode>d__.<>t__builder = AsyncTaskMethodBuilder<Member>.Create();
		<VerifyCreatorCode>d__.terminalId = terminalId;
		<VerifyCreatorCode>d__.code = code;
		<VerifyCreatorCode>d__.id = id;
		<VerifyCreatorCode>d__.<>1__state = -1;
		<VerifyCreatorCode>d__.<>t__builder.Start<NexusManager.<VerifyCreatorCode>d__15>(ref <VerifyCreatorCode>d__);
		return <VerifyCreatorCode>d__.<>t__builder.Task;
	}

	public Task<bool> VerifyCreatorCodeJIT(string memberCode, string groupCode)
	{
		NexusManager.<VerifyCreatorCodeJIT>d__16 <VerifyCreatorCodeJIT>d__;
		<VerifyCreatorCodeJIT>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<VerifyCreatorCodeJIT>d__.memberCode = memberCode;
		<VerifyCreatorCodeJIT>d__.groupCode = groupCode;
		<VerifyCreatorCodeJIT>d__.<>1__state = -1;
		<VerifyCreatorCodeJIT>d__.<>t__builder.Start<NexusManager.<VerifyCreatorCodeJIT>d__16>(ref <VerifyCreatorCodeJIT>d__);
		return <VerifyCreatorCodeJIT>d__.<>t__builder.Task;
	}

	public bool BuildValidationCheck()
	{
		if (this.defaultNexusGroupId == null)
		{
			Debug.LogError("You have to set defaultNexusGroupId in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	private const string ENV_PRODUCTION = "production";

	private const string ENV_SANDBOX = "sandbox";

	private const string ENV_PRODUCTION_API_KEY = "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb";

	private const string ENV_SANDBOX_API_KEY = "nexus_pk_ba155a8c229740489d214f024e25f25c";

	[SerializeField]
	private NexusGroupId defaultNexusGroupId;

	private NexusManager.Environment environment = NexusManager.Environment.SANDBOX;

	public static NexusManager instance;

	private Member[] validatedMembers;

	public enum Environment
	{
		PRODUCTION,
		SANDBOX
	}

	[Serializable]
	public class MemberCode
	{
		public string memberCode { get; set; }

		public NexusGroupId groupId { get; set; }
	}

	[Serializable]
	public struct GetMembersRequest
	{
		public int page { readonly get; set; }

		public int pageSize { readonly get; set; }
	}
}
