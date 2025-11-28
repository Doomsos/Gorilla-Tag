using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class ActivateGO : MonoBehaviour
{
	// Token: 0x06000018 RID: 24 RVA: 0x00002327 File Offset: 0x00000527
	private void OnEnable()
	{
		this.active = PlayerPrefFlags.Check(this.flag);
		this.SetGOsActive(0);
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Combine(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002361 File Offset: 0x00000561
	private void OnDisable()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002361 File Offset: 0x00000561
	private void OnDestroy()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002383 File Offset: 0x00000583
	public void OnFlagChange(PlayerPrefFlags.Flag f, bool value)
	{
		if (f != this.flag)
		{
			return;
		}
		this.active = value;
		this.SetGOsActive(this.flashes);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x000023A4 File Offset: 0x000005A4
	private void SetGOsActive(int fls)
	{
		ActivateGO.<SetGOsActive>d__10 <SetGOsActive>d__;
		<SetGOsActive>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetGOsActive>d__.<>4__this = this;
		<SetGOsActive>d__.fls = fls;
		<SetGOsActive>d__.<>1__state = -1;
		<SetGOsActive>d__.<>t__builder.Start<ActivateGO.<SetGOsActive>d__10>(ref <SetGOsActive>d__);
	}

	// Token: 0x0600001D RID: 29 RVA: 0x000023E4 File Offset: 0x000005E4
	private void toggle(List<Renderer> renderers, bool state)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			if ((this.layerMask.value & 1 << renderers[i].gameObject.layer) != 0)
			{
				renderers[i].forceRenderingOff = !state;
			}
		}
	}

	// Token: 0x04000008 RID: 8
	[SerializeField]
	private GameObject targetGO;

	// Token: 0x04000009 RID: 9
	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	// Token: 0x0400000A RID: 10
	[SerializeField]
	private int flashes;

	// Token: 0x0400000B RID: 11
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x0400000C RID: 12
	private bool active;

	// Token: 0x0400000D RID: 13
	private bool flashing;
}
