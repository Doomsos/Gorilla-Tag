using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000103 RID: 259
public class SIUpgradeXformOffsetter : MonoBehaviour
{
	// Token: 0x06000680 RID: 1664 RVA: 0x00024FC8 File Offset: 0x000231C8
	protected void Awake()
	{
		if (this.m_superInfectionGadget == null)
		{
			Debug.LogError("[SIUpgradeXformOffsetter]  ERROR!!!  Awake: Disabling component because `m_superInfectionGadget` is null. Path=" + base.transform.GetPathQ(), this);
			base.enabled = false;
			return;
		}
		foreach (SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp siupgradeXformOffsetOp in this.m_upgradeXformOffsetOps)
		{
			if (!(siupgradeXformOffsetOp.xform != null) && !(siupgradeXformOffsetOp.targetXform != null))
			{
				Debug.LogError("[SIUpgradeXformOffsetter]  ERROR!!!  Awake: Disabling component because null reference in `m_upgradeXformOffsetOps` array. Path=" + base.transform.GetPathQ(), this);
				base.enabled = false;
				return;
			}
		}
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00025062 File Offset: 0x00023262
	protected void OnEnable()
	{
		SIGadget superInfectionGadget = this.m_superInfectionGadget;
		superInfectionGadget.OnPostRefreshVisuals = (Action<SIUpgradeSet>)Delegate.Combine(superInfectionGadget.OnPostRefreshVisuals, new Action<SIUpgradeSet>(this._HandleGadgetOnPostRefreshVisuals));
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0002508B File Offset: 0x0002328B
	protected void OnDisable()
	{
		SIGadget superInfectionGadget = this.m_superInfectionGadget;
		superInfectionGadget.OnPostRefreshVisuals = (Action<SIUpgradeSet>)Delegate.Remove(superInfectionGadget.OnPostRefreshVisuals, new Action<SIUpgradeSet>(this._HandleGadgetOnPostRefreshVisuals));
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000250B4 File Offset: 0x000232B4
	private void _HandleGadgetOnPostRefreshVisuals(SIUpgradeSet upgradeSet)
	{
		for (int i = 0; i < this.m_upgradeXformOffsetOps.Length; i++)
		{
			SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp siupgradeXformOffsetOp = this.m_upgradeXformOffsetOps[i];
			if (upgradeSet.Contains(siupgradeXformOffsetOp.upgradeType))
			{
				siupgradeXformOffsetOp.xform.SetLocalPositionAndRotation(siupgradeXformOffsetOp.targetXform.localPosition, siupgradeXformOffsetOp.targetXform.localRotation);
				siupgradeXformOffsetOp.xform.localScale = siupgradeXformOffsetOp.targetXform.localScale;
			}
		}
	}

	// Token: 0x04000880 RID: 2176
	private const string preLog = "[SIUpgradeXformOffsetter]  ";

	// Token: 0x04000881 RID: 2177
	private const string preErr = "[SIUpgradeXformOffsetter]  ERROR!!!  ";

	// Token: 0x04000882 RID: 2178
	[SerializeField]
	private SIGadget m_superInfectionGadget;

	// Token: 0x04000883 RID: 2179
	[SerializeField]
	private SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp[] m_upgradeXformOffsetOps;

	// Token: 0x02000104 RID: 260
	[Serializable]
	public struct SIUpgradeXformOffsetOp
	{
		// Token: 0x04000884 RID: 2180
		public SIUpgradeType upgradeType;

		// Token: 0x04000885 RID: 2181
		public Transform xform;

		// Token: 0x04000886 RID: 2182
		[FormerlySerializedAs("newParent")]
		public Transform targetXform;
	}
}
