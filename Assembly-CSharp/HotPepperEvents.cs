using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004DC RID: 1244
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x06002000 RID: 8192 RVA: 0x000AA026 File Offset: 0x000A8226
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000AA060 File Offset: 0x000A8260
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000AA09A File Offset: 0x000A829A
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000AA0A5 File Offset: 0x000A82A5
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000AA0B0 File Offset: 0x000A82B0
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		rig.transform.Find("RigAnchor/rig/body/head/gorillaface/spicy").gameObject.GetComponent<HotPepperFace>().PlayFX(1f);
	}

	// Token: 0x04002A56 RID: 10838
	[SerializeField]
	private EdibleHoldable _pepper;

	// Token: 0x020004DD RID: 1245
	public enum EdibleState
	{
		// Token: 0x04002A58 RID: 10840
		A = 1,
		// Token: 0x04002A59 RID: 10841
		B,
		// Token: 0x04002A5A RID: 10842
		C = 4,
		// Token: 0x04002A5B RID: 10843
		D = 8
	}
}
