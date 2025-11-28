using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004D4 RID: 1236
public class BubbleGumEvents : MonoBehaviour
{
	// Token: 0x06001FDA RID: 8154 RVA: 0x000A991D File Offset: 0x000A7B1D
	private void OnEnable()
	{
		this._edible.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x000A9957 File Offset: 0x000A7B57
	private void OnDisable()
	{
		this._edible.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000A9991 File Offset: 0x000A7B91
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000A999C File Offset: 0x000A7B9C
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x000A99A8 File Offset: 0x000A7BA8
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		GorillaTagger instance = GorillaTagger.Instance;
		GameObject gameObject = null;
		if (isViewRig && instance != null)
		{
			gameObject = instance.gameObject;
		}
		else if (!isViewRig)
		{
			gameObject = rig.gameObject;
		}
		if (!BubbleGumEvents.gTargetCache.TryGetValue(gameObject, ref this._bubble))
		{
			this._bubble = Enumerable.FirstOrDefault<GumBubble>(gameObject.GetComponentsInChildren<GumBubble>(true), (GumBubble g) => g.transform.parent.name == "$gum");
			if (isViewRig)
			{
				this._bubble.audioSource = instance.offlineVRRig.tagSound;
				this._bubble.targetScale = Vector3.one * 1.36f;
			}
			else
			{
				this._bubble.audioSource = rig.tagSound;
				this._bubble.targetScale = Vector3.one * 2f;
			}
			BubbleGumEvents.gTargetCache.Add(gameObject, this._bubble);
		}
		GumBubble bubble = this._bubble;
		if (bubble != null)
		{
			bubble.transform.parent.gameObject.SetActive(true);
		}
		GumBubble bubble2 = this._bubble;
		if (bubble2 == null)
		{
			return;
		}
		bubble2.InflateDelayed();
	}

	// Token: 0x04002A33 RID: 10803
	[SerializeField]
	private EdibleHoldable _edible;

	// Token: 0x04002A34 RID: 10804
	[SerializeField]
	private GumBubble _bubble;

	// Token: 0x04002A35 RID: 10805
	private static Dictionary<GameObject, GumBubble> gTargetCache = new Dictionary<GameObject, GumBubble>(16);

	// Token: 0x020004D5 RID: 1237
	public enum EdibleState
	{
		// Token: 0x04002A37 RID: 10807
		A = 1,
		// Token: 0x04002A38 RID: 10808
		B,
		// Token: 0x04002A39 RID: 10809
		C = 4,
		// Token: 0x04002A3A RID: 10810
		D = 8
	}
}
