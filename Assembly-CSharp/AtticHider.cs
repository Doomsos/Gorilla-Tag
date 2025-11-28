using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000537 RID: 1335
public class AtticHider : MonoBehaviour
{
	// Token: 0x0600219F RID: 8607 RVA: 0x000AFF7D File Offset: 0x000AE17D
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000AFFAB File Offset: 0x000AE1AB
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000AFFD4 File Offset: 0x000AE1D4
	private void OnZoneChanged()
	{
		if (this.AtticRenderer == null)
		{
			return;
		}
		if (ZoneManagement.instance.IsZoneActive(GTZone.attic))
		{
			if (this._coroutine != null)
			{
				base.StopCoroutine(this._coroutine);
				this._coroutine = null;
			}
			this._coroutine = base.StartCoroutine(this.WaitForAtticLoad());
			return;
		}
		if (this._coroutine != null)
		{
			base.StopCoroutine(this._coroutine);
			this._coroutine = null;
		}
		this.AtticRenderer.enabled = true;
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000B0053 File Offset: 0x000AE253
	private IEnumerator WaitForAtticLoad()
	{
		while (!ZoneManagement.instance.IsSceneLoaded(GTZone.attic))
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return null;
		this.AtticRenderer.enabled = false;
		this._coroutine = null;
		yield break;
	}

	// Token: 0x04002C52 RID: 11346
	[SerializeField]
	private MeshRenderer AtticRenderer;

	// Token: 0x04002C53 RID: 11347
	private Coroutine _coroutine;
}
