using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class CritterSpawnTrigger : MonoBehaviour
{
	// Token: 0x0600029E RID: 670 RVA: 0x000106AC File Offset: 0x0000E8AC
	private ValueDropdownList<int> GetCritterTypeList()
	{
		return new ValueDropdownList<int>();
	}

	// Token: 0x0600029F RID: 671 RVA: 0x000106B4 File Offset: 0x0000E8B4
	private void OnTriggerEnter(Collider other)
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.realtimeSinceStartup < this._nextSpawnTime)
		{
			return;
		}
		CrittersActor componentInParent = other.GetComponentInParent<CrittersActor>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.crittersActorType != this.triggerActorType)
		{
			return;
		}
		if (this.requiredSubObjectIndex >= 0 && componentInParent.subObjectIndex != this.requiredSubObjectIndex)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.triggerActorName) && !componentInParent.GetActorSubtype().Contains(this.triggerActorName))
		{
			return;
		}
		CrittersManager.instance.DespawnActor(componentInParent);
		CrittersManager.instance.SpawnCritter(this.critterType, this.spawnPoint.position, this.spawnPoint.rotation);
		this._nextSpawnTime = Time.realtimeSinceStartup + this.triggerCooldown;
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001077E File Offset: 0x0000E97E
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.position, this.spawnPoint.position);
		Gizmos.DrawWireSphere(this.spawnPoint.position, 0.1f);
	}

	// Token: 0x04000316 RID: 790
	[Header("Trigger Settings")]
	[SerializeField]
	private CrittersActor.CrittersActorType triggerActorType;

	// Token: 0x04000317 RID: 791
	[SerializeField]
	private int requiredSubObjectIndex = -1;

	// Token: 0x04000318 RID: 792
	[SerializeField]
	private string triggerActorName;

	// Token: 0x04000319 RID: 793
	[SerializeField]
	private float triggerCooldown = 1f;

	// Token: 0x0400031A RID: 794
	[Header("Spawn Settings")]
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x0400031B RID: 795
	[SerializeField]
	private int critterType;

	// Token: 0x0400031C RID: 796
	private float _nextSpawnTime;
}
