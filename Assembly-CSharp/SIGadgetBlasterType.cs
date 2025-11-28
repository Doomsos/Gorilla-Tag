using System;

// Token: 0x020000C4 RID: 196
public interface SIGadgetBlasterType
{
	// Token: 0x060004C5 RID: 1221
	void OnUpdateAuthority(float dt);

	// Token: 0x060004C6 RID: 1222
	void OnUpdateRemote(float dt);

	// Token: 0x060004C7 RID: 1223
	void SetStateShared();

	// Token: 0x060004C8 RID: 1224
	void NetworkFireProjectile(object[] data);

	// Token: 0x060004C9 RID: 1225
	void ApplyUpgradeNodes(SIUpgradeSet withUpgrades);
}
