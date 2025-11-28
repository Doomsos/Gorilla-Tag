using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NexusSDK;
using UnityEngine;

// Token: 0x020004AF RID: 1199
public class NexusManager : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06001F0A RID: 7946 RVA: 0x000A500F File Offset: 0x000A320F
	public NexusManager.Environment CurrentEnvironment
	{
		get
		{
			return this.environment;
		}
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x000A5017 File Offset: 0x000A3217
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

	// Token: 0x06001F0C RID: 7948 RVA: 0x000A503A File Offset: 0x000A323A
	private void Start()
	{
		SDKInitializer.Init((this.environment == NexusManager.Environment.SANDBOX) ? "nexus_pk_ba155a8c229740489d214f024e25f25c" : "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb", (this.environment == NexusManager.Environment.SANDBOX) ? "sandbox" : "production");
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000A506C File Offset: 0x000A326C
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

	// Token: 0x06001F0E RID: 7950 RVA: 0x000A50C0 File Offset: 0x000A32C0
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

	// Token: 0x06001F0F RID: 7951 RVA: 0x000A510B File Offset: 0x000A330B
	public bool BuildValidationCheck()
	{
		if (this.defaultNexusGroupId == null)
		{
			Debug.LogError("You have to set defaultNexusGroupId in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x0400295C RID: 10588
	private const string ENV_PRODUCTION = "production";

	// Token: 0x0400295D RID: 10589
	private const string ENV_SANDBOX = "sandbox";

	// Token: 0x0400295E RID: 10590
	private const string ENV_PRODUCTION_API_KEY = "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb";

	// Token: 0x0400295F RID: 10591
	private const string ENV_SANDBOX_API_KEY = "nexus_pk_ba155a8c229740489d214f024e25f25c";

	// Token: 0x04002960 RID: 10592
	[SerializeField]
	private NexusGroupId defaultNexusGroupId;

	// Token: 0x04002961 RID: 10593
	private NexusManager.Environment environment = NexusManager.Environment.SANDBOX;

	// Token: 0x04002962 RID: 10594
	public static NexusManager instance;

	// Token: 0x04002963 RID: 10595
	private Member[] validatedMembers;

	// Token: 0x020004B0 RID: 1200
	public enum Environment
	{
		// Token: 0x04002965 RID: 10597
		PRODUCTION,
		// Token: 0x04002966 RID: 10598
		SANDBOX
	}

	// Token: 0x020004B1 RID: 1201
	[Serializable]
	public class MemberCode
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06001F11 RID: 7953 RVA: 0x000A5147 File Offset: 0x000A3347
		// (set) Token: 0x06001F12 RID: 7954 RVA: 0x000A514F File Offset: 0x000A334F
		public string memberCode { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06001F13 RID: 7955 RVA: 0x000A5158 File Offset: 0x000A3358
		// (set) Token: 0x06001F14 RID: 7956 RVA: 0x000A5160 File Offset: 0x000A3360
		public NexusGroupId groupId { get; set; }
	}

	// Token: 0x020004B2 RID: 1202
	[Serializable]
	public struct GetMembersRequest
	{
		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06001F16 RID: 7958 RVA: 0x000A5169 File Offset: 0x000A3369
		// (set) Token: 0x06001F17 RID: 7959 RVA: 0x000A5171 File Offset: 0x000A3371
		public int page { readonly get; set; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06001F18 RID: 7960 RVA: 0x000A517A File Offset: 0x000A337A
		// (set) Token: 0x06001F19 RID: 7961 RVA: 0x000A5182 File Offset: 0x000A3382
		public int pageSize { readonly get; set; }
	}
}
