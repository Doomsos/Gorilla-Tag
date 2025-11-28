using System;
using UnityEngine;

// Token: 0x0200085A RID: 2138
public class OwnerRig : MonoBehaviour, IVariable<VRRig>, IVariable, IRigAware
{
	// Token: 0x06003851 RID: 14417 RVA: 0x0012D6B3 File Offset: 0x0012B8B3
	public void TryFindRig()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (this._rig != null)
		{
			return;
		}
		this._rig = base.GetComponentInChildren<VRRig>();
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x0012D6DC File Offset: 0x0012B8DC
	public VRRig Get()
	{
		return this._rig;
	}

	// Token: 0x06003853 RID: 14419 RVA: 0x0012D6E4 File Offset: 0x0012B8E4
	public void Set(VRRig value)
	{
		this._rig = value;
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x0012D6ED File Offset: 0x0012B8ED
	public void Set(GameObject obj)
	{
		this._rig = ((obj != null) ? obj.GetComponentInParent<VRRig>() : null);
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x0012D6E4 File Offset: 0x0012B8E4
	void IRigAware.SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x0012D707 File Offset: 0x0012B907
	public static implicit operator bool(OwnerRig or)
	{
		return or != null && !(or == null) && or._rig != null && !(or._rig == null);
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x0012D734 File Offset: 0x0012B934
	public static implicit operator VRRig(OwnerRig or)
	{
		if (!or)
		{
			return null;
		}
		return or._rig;
	}

	// Token: 0x04004763 RID: 18275
	[SerializeField]
	private VRRig _rig;
}
