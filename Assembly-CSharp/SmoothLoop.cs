using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class SmoothLoop : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x06001F38 RID: 7992 RVA: 0x000A5755 File Offset: 0x000A3955
	public bool BuildValidationCheck()
	{
		if (this.source == null)
		{
			Debug.LogError("missing audio source, this will fail", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x000A5778 File Offset: 0x000A3978
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.GTStop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000A57F8 File Offset: 0x000A39F8
	public void SliceUpdate()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.source.time > this.source.clip.length * this.loopEnd)
		{
			this.source.time = this.loopStart;
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000A5838 File Offset: 0x000A3A38
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.sourceCheck())
		{
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000A589C File Offset: 0x000A3A9C
	private bool sourceCheck()
	{
		if (!this.source || !this.source.clip)
		{
			Debug.LogError("SmoothLoop: Disabling because AudioSource is null or has no clip assigned. Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			base.StopAllCoroutines();
			return false;
		}
		return true;
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000A58F3 File Offset: 0x000A3AF3
	public IEnumerator DelayedStart()
	{
		if (!this.sourceCheck())
		{
			yield break;
		}
		yield return new WaitForSeconds(this.delay);
		this.source.GTPlay();
		yield break;
	}

	// Token: 0x04002983 RID: 10627
	public AudioSource source;

	// Token: 0x04002984 RID: 10628
	public float delay;

	// Token: 0x04002985 RID: 10629
	public bool randomStart;

	// Token: 0x04002986 RID: 10630
	[SerializeField]
	[Range(0f, 1f)]
	private float loopStart = 0.1f;

	// Token: 0x04002987 RID: 10631
	[SerializeField]
	[Range(0f, 1f)]
	private float loopEnd = 0.95f;
}
