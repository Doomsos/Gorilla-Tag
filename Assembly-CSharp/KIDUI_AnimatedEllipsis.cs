using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000A9F RID: 2719
public class KIDUI_AnimatedEllipsis : MonoBehaviour
{
	// Token: 0x0600443E RID: 17470 RVA: 0x00169686 File Offset: 0x00167886
	private void Awake()
	{
		if (this._ellipsisObjects != null)
		{
			return;
		}
		this.SetupEllipsis();
	}

	// Token: 0x0600443F RID: 17471 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06004440 RID: 17472 RVA: 0x00169697 File Offset: 0x00167897
	private void OnDisable()
	{
		this.StopAnimation();
	}

	// Token: 0x06004441 RID: 17473 RVA: 0x001696A0 File Offset: 0x001678A0
	private void SetupEllipsis()
	{
		if (this._ellipsisRoot == null)
		{
			this._ellipsisRoot = base.gameObject;
		}
		this._ellipsisObjects = new ValueTuple<GameObject, float, float, float>[this._ellipsisStartingValues.Count];
		for (int i = 0; i < this._ellipsisStartingValues.Count; i++)
		{
			float num = this._ellipsisStartingValues[i];
			this._ellipsisObjects[i].Item1 = Object.Instantiate<GameObject>(this._ellipsisPrefab, this._ellipsisRoot.transform);
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			this._ellipsisObjects[i].Item2 = (this._ellipsisObjects[i].Item3 = num);
		}
	}

	// Token: 0x06004442 RID: 17474 RVA: 0x00169776 File Offset: 0x00167976
	private IEnumerator EllipsisAnimation()
	{
		int currIndex = 0;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				int num = i - currIndex;
				if (num < 0)
				{
					num = this._ellipsisStartingValues.Count + num;
				}
				float num2 = this._ellipsisStartingValues[num];
				this._ellipsisObjects[i].Item1.transform.localScale = Vector3.one * num2;
			}
			int num3 = currIndex;
			currIndex = num3 + 1;
			if (currIndex >= this._ellipsisObjects.Length)
			{
				currIndex = 0;
			}
			yield return new WaitForSeconds(this._pauseBetweenScale);
		}
		yield break;
	}

	// Token: 0x06004443 RID: 17475 RVA: 0x00169785 File Offset: 0x00167985
	private IEnumerator EllipsisAnimation2()
	{
		float time = 0f;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				float offsetTime = this._scaleDuration / (float)(this._ellipsisObjects.Length + 1) * (float)i;
				float num = this.LerpLoop(this._startingScale, this._endScale, time, offsetTime, this._scaleDuration);
				this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			}
			time += Time.deltaTime * this._animationSpeedMultiplier;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004444 RID: 17476 RVA: 0x00169794 File Offset: 0x00167994
	public Task StartAnimation()
	{
		KIDUI_AnimatedEllipsis.<StartAnimation>d__24 <StartAnimation>d__;
		<StartAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAnimation>d__.<>4__this = this;
		<StartAnimation>d__.<>1__state = -1;
		<StartAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StartAnimation>d__24>(ref <StartAnimation>d__);
		return <StartAnimation>d__.<>t__builder.Task;
	}

	// Token: 0x06004445 RID: 17477 RVA: 0x001697D8 File Offset: 0x001679D8
	public Task StopAnimation()
	{
		KIDUI_AnimatedEllipsis.<StopAnimation>d__25 <StopAnimation>d__;
		<StopAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StopAnimation>d__.<>4__this = this;
		<StopAnimation>d__.<>1__state = -1;
		<StopAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StopAnimation>d__25>(ref <StopAnimation>d__);
		return <StopAnimation>d__.<>t__builder.Task;
	}

	// Token: 0x06004446 RID: 17478 RVA: 0x0016981C File Offset: 0x00167A1C
	public float LerpLoop(float start, float end, float time, float offsetTime, float duration)
	{
		float num = (offsetTime - time) % duration / duration;
		float num2 = this._ellipsisAnimationCurve.Evaluate(num);
		return Mathf.Lerp(start, end, num2);
	}

	// Token: 0x040055CC RID: 21964
	[Header("Ellipsis Spawning")]
	[SerializeField]
	private bool _animateOnStart = true;

	// Token: 0x040055CD RID: 21965
	[SerializeField]
	private int _ellipsisCount = 3;

	// Token: 0x040055CE RID: 21966
	[SerializeField]
	private GameObject _ellipsisPrefab;

	// Token: 0x040055CF RID: 21967
	[SerializeField]
	private GameObject _ellipsisRoot;

	// Token: 0x040055D0 RID: 21968
	[SerializeField]
	private List<float> _ellipsisStartingValues = new List<float>();

	// Token: 0x040055D1 RID: 21969
	[Header("Animation Settings")]
	[SerializeField]
	private bool _shouldLerp;

	// Token: 0x040055D2 RID: 21970
	[SerializeField]
	private AnimationCurve _ellipsisAnimationCurve;

	// Token: 0x040055D3 RID: 21971
	[SerializeField]
	private float _animationSpeedMultiplier = 0.25f;

	// Token: 0x040055D4 RID: 21972
	[SerializeField]
	private float _startingScale = 0.33f;

	// Token: 0x040055D5 RID: 21973
	[SerializeField]
	private float _intermediaryScale = 0.66f;

	// Token: 0x040055D6 RID: 21974
	[SerializeField]
	private float _endScale = 1f;

	// Token: 0x040055D7 RID: 21975
	[SerializeField]
	private float _scaleDuration = 0.25f;

	// Token: 0x040055D8 RID: 21976
	[SerializeField]
	private float _pauseBetweenScale = 0.25f;

	// Token: 0x040055D9 RID: 21977
	[SerializeField]
	private float _pauseBetweenCycles = 0.5f;

	// Token: 0x040055DA RID: 21978
	private bool _runAnimation;

	// Token: 0x040055DB RID: 21979
	private float _nextChange;

	// Token: 0x040055DC RID: 21980
	[TupleElementNames(new string[]
	{
		"ellipsis",
		"startingScale",
		"currentScale",
		"lerpT"
	})]
	private ValueTuple<GameObject, float, float, float>[] _ellipsisObjects;

	// Token: 0x040055DD RID: 21981
	private Coroutine _animationCoroutine;
}
