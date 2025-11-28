using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000834 RID: 2100
public class Tappable : MonoBehaviour
{
	// Token: 0x06003738 RID: 14136 RVA: 0x0012994E File Offset: 0x00127B4E
	public void Validate()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003739 RID: 14137 RVA: 0x00129957 File Offset: 0x00127B57
	protected virtual void OnEnable()
	{
		if (!this.useStaticId)
		{
			this.CalculateId(false);
		}
		TappableManager.Register(this);
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x0012996E File Offset: 0x00127B6E
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanTap(bool isLeftHand)
	{
		return true;
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x00129978 File Offset: 0x00127B78
	public void OnTap(float tapStrength)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", 0, new object[]
		{
			this.tappableId,
			tapStrength
		});
	}

	// Token: 0x0600373D RID: 14141 RVA: 0x001299D4 File Offset: 0x00127BD4
	public void OnGrab()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnGrabRPC", 0, new object[]
		{
			this.tappableId
		});
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x00129A28 File Offset: 0x00127C28
	public void OnRelease()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnReleaseRPC", 0, new object[]
		{
			this.tappableId
		});
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x0012994E File Offset: 0x00127B4E
	private void EdRecalculateId()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x00129A7C File Offset: 0x00127C7C
	private void CalculateId(bool force = false)
	{
		Transform transform = base.transform;
		int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
		int staticHash = base.GetType().Name.GetStaticHash();
		int hashCode2 = transform.position.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
		if (this.useStaticId)
		{
			if (string.IsNullOrEmpty(this.staticId) || force)
			{
				int instanceID = transform.GetInstanceID();
				int num2 = StaticHash.Compute(num, instanceID);
				this.staticId = string.Format("#ID_{0:X8}", num2);
			}
			this.tappableId = this.staticId.GetStaticHash();
			return;
		}
		this.tappableId = (Application.isPlaying ? num : 0);
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x00129B46 File Offset: 0x00127D46
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		this.CalculateId(false);
	}

	// Token: 0x040046AB RID: 18091
	public int tappableId;

	// Token: 0x040046AC RID: 18092
	public string staticId;

	// Token: 0x040046AD RID: 18093
	public bool useStaticId;

	// Token: 0x040046AE RID: 18094
	[Tooltip("If true, tap cooldown will be ignored.  Tapping will be allowed/disallowed based on result of CanTap()")]
	public bool overrideTapCooldown;

	// Token: 0x040046AF RID: 18095
	[Space]
	public TappableManager manager;

	// Token: 0x040046B0 RID: 18096
	public RpcTarget rpcTarget;
}
