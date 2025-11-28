using System;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class SIGadgetPlatformDeployerPlatform : MonoBehaviour, ISIGameDeployable
{
	// Token: 0x060005F6 RID: 1526 RVA: 0x00021A94 File Offset: 0x0001FC94
	public void ApplyUpgrades(SIUpgradeSet upgrades)
	{
		bool flag = upgrades.Contains(SIUpgradeType.Platform_Duration);
		float num = flag ? this.extendedDuration : this.defaultDuration;
		this.timeToDie = Time.time + num;
		this.extendedDurationFrame.SetActive(flag);
		this.checkBounds = new Bounds(this.activeCollider.center, this.activeCollider.size);
		Vector3 size = this.checkBounds.size;
		Vector3 lossyScale = this.activeCollider.transform.lossyScale;
		size.x *= lossyScale.x;
		size.y *= lossyScale.y;
		size.z *= lossyScale.z;
		this.checkBounds.size = size;
		this.checkOffset = this.activeCollider.transform.position;
		this.checkRot = this.activeCollider.transform.rotation;
		this.CheckHeadOverlap();
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00021B88 File Offset: 0x0001FD88
	public void CheckHeadOverlap()
	{
		if (this.activeCollider == null)
		{
			return;
		}
		Vector3 position = GorillaTagger.Instance.headCollider.transform.position;
		float num = GorillaTagger.Instance.headCollider.radius * GorillaTagger.Instance.headCollider.transform.lossyScale.x;
		Vector3 vector = Quaternion.Inverse(this.checkRot) * (position - this.checkOffset);
		if (Vector3.Magnitude(this.checkBounds.ClosestPoint(vector) - vector) < num)
		{
			this.isOverlappingHead = true;
			this.activeCollider.enabled = false;
			return;
		}
		this.isOverlappingHead = false;
		this.activeCollider.enabled = true;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00021C41 File Offset: 0x0001FE41
	private void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		if (this.isOverlappingHead)
		{
			this.CheckHeadOverlap();
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00021C6F File Offset: 0x0001FE6F
	private void OnDisable()
	{
		Action onDisabled = this.OnDisabled;
		if (onDisabled != null)
		{
			onDisabled.Invoke();
		}
		this.OnDisabled = null;
	}

	// Token: 0x0400075C RID: 1884
	[SerializeField]
	private GameObject extendedDurationFrame;

	// Token: 0x0400075D RID: 1885
	[SerializeField]
	private float defaultDuration = 10f;

	// Token: 0x0400075E RID: 1886
	[SerializeField]
	private float extendedDuration = 20f;

	// Token: 0x0400075F RID: 1887
	[SerializeField]
	private BoxCollider activeCollider;

	// Token: 0x04000760 RID: 1888
	private bool isOverlappingHead;

	// Token: 0x04000761 RID: 1889
	private float timeToDie = -1f;

	// Token: 0x04000762 RID: 1890
	private Bounds checkBounds;

	// Token: 0x04000763 RID: 1891
	private Vector3 checkOffset;

	// Token: 0x04000764 RID: 1892
	private Quaternion checkRot;

	// Token: 0x04000765 RID: 1893
	public Action OnDisabled;
}
