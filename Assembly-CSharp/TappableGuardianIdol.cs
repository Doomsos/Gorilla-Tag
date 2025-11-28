using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008F0 RID: 2288
[DisallowMultipleComponent]
public class TappableGuardianIdol : Tappable
{
	// Token: 0x17000565 RID: 1381
	// (get) Token: 0x06003A8B RID: 14987 RVA: 0x0013567B File Offset: 0x0013387B
	// (set) Token: 0x06003A8C RID: 14988 RVA: 0x00135683 File Offset: 0x00133883
	public bool isChangingPositions { get; private set; }

	// Token: 0x06003A8D RID: 14989 RVA: 0x0013568C File Offset: 0x0013388C
	protected override void OnEnable()
	{
		base.OnEnable();
		this._colliderBaseRadius = this.tapCollision.radius;
	}

	// Token: 0x06003A8E RID: 14990 RVA: 0x001356A5 File Offset: 0x001338A5
	protected override void OnDisable()
	{
		base.OnDisable();
		this.isChangingPositions = false;
		this._activationState = -1;
		this.isActivationReady = true;
		this.tapCollision.radius = this._colliderBaseRadius;
	}

	// Token: 0x06003A8F RID: 14991 RVA: 0x001356D3 File Offset: 0x001338D3
	public void OnZoneActiveStateChanged(bool zoneActive)
	{
		this._zoneIsActive = zoneActive;
		this.idolVisualRoot.SetActive(this._zoneIsActive);
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x001356F0 File Offset: 0x001338F0
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (info.Sender.IsLocal)
		{
			this.zoneManager.SetScaleCenterPoint(base.transform);
		}
		if (!this.isChangingPositions)
		{
			if (!this.zoneManager.IsZoneValid())
			{
				return;
			}
			RigContainer rigContainer;
			if (PhotonNetwork.LocalPlayer.IsMasterClient && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (Vector3.Magnitude(rigContainer.Rig.transform.position - base.transform.position) > this.requiredTapDistance + Mathf.Epsilon)
				{
					return;
				}
				this.zoneManager.IdolWasTapped(info.Sender);
			}
			if (!this.zoneManager.IsPlayerGuardian(info.Sender))
			{
				this.tapFX.Play();
			}
		}
	}

	// Token: 0x06003A91 RID: 14993 RVA: 0x001357B8 File Offset: 0x001339B8
	public void SetPosition(Vector3 position)
	{
		base.transform.position = position + new Vector3(0f, this.activeHeight, 0f);
		this.UpdateStageActivatedObjects();
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		base.StartCoroutine(this.<SetPosition>g__Unshrink|49_0());
	}

	// Token: 0x06003A92 RID: 14994 RVA: 0x0013581A File Offset: 0x00133A1A
	public void MovePositions(Vector3 finalPosition)
	{
		if (this.isChangingPositions)
		{
			return;
		}
		this.transitionPos = finalPosition + this.fallStartOffset;
		this.finalPos = finalPosition;
		base.StartCoroutine(this.TransitionToNextIdol());
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x0013584C File Offset: 0x00133A4C
	public void UpdateActivationProgress(float rawProgress, bool progressing)
	{
		this.isActivationReady = !progressing;
		if (rawProgress <= 0f && !progressing)
		{
			if (this._activationState >= 0)
			{
				if (this._activationRoutine != null)
				{
					base.StopCoroutine(this._activationRoutine);
					this._activationRoutine = null;
				}
				this.idolMeshRoot.transform.localScale = Vector3.one;
			}
			this._activationState = -1;
			this.UpdateStageActivatedObjects();
			this._audio.GTStop();
			return;
		}
		int num = (int)rawProgress;
		progressing &= (this._activationStageSounds.Length > num);
		if (this._activationState == num || !progressing)
		{
			return;
		}
		if (this._activationRoutine != null)
		{
			base.StopCoroutine(this._activationRoutine);
		}
		this._activationRoutine = base.StartCoroutine(this.ShowActivationEffect());
		this._activationState = num;
		this.UpdateStageActivatedObjects();
		TappableGuardianIdol.IdolActivationSound idolActivationSound = this._activationStageSounds[num];
		this._audio.GTPlayOneShot(idolActivationSound.activation, this._audio.volume);
		this._audio.clip = idolActivationSound.loop;
		this._audio.loop = true;
		this._audio.GTPlay();
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x00135963 File Offset: 0x00133B63
	public void StartLookingAround()
	{
		if (this._lookRoutine != null)
		{
			base.StopCoroutine(this._lookRoutine);
		}
		this._lookRoutine = base.StartCoroutine(this.DoLookingAround());
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x0013598B File Offset: 0x00133B8B
	public void StopLookingAround()
	{
		if (this._lookRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this._lookRoutine);
		this._lookRoot.localRotation = Quaternion.identity;
		this._lookRoutine = null;
	}

	// Token: 0x06003A96 RID: 14998 RVA: 0x001359B9 File Offset: 0x00133BB9
	private IEnumerator DoLookingAround()
	{
		TappableGuardianIdol.<>c__DisplayClass54_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.nextLookTime = Time.time;
		CS$<>8__locals1._lookDirection = this._lookRoot.rotation;
		yield return null;
		for (;;)
		{
			if (Time.time >= CS$<>8__locals1.nextLookTime)
			{
				this.<DoLookingAround>g__PickLookTarget|54_0(ref CS$<>8__locals1);
			}
			this._lookRoot.rotation = Quaternion.Slerp(this._lookRoot.rotation, CS$<>8__locals1._lookDirection, Time.deltaTime * Mathf.Max(1f, (float)this._activationState * this._baseLookRate));
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003A97 RID: 14999 RVA: 0x001359C8 File Offset: 0x00133BC8
	private void UpdateStageActivatedObjects()
	{
		foreach (TappableGuardianIdol.StageActivatedObject stageActivatedObject in this._stageActivatedObjects)
		{
			stageActivatedObject.UpdateActiveState(this._activationState);
		}
	}

	// Token: 0x06003A98 RID: 15000 RVA: 0x001359FF File Offset: 0x00133BFF
	private IEnumerator ShowActivationEffect()
	{
		float bulgeDuration = 1f;
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / bulgeDuration;
			float num = Mathf.Lerp(1f, this.bulgeScale, this.bulgeCurve.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		this._activationRoutine = null;
		yield break;
	}

	// Token: 0x06003A99 RID: 15001 RVA: 0x00135A0E File Offset: 0x00133C0E
	private IEnumerator TransitionToNextIdol()
	{
		this.isChangingPositions = true;
		this._audio.GTStop();
		if (this.knockbackOnTrigger)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		if (this.explodeFX)
		{
			ObjectPools.instance.Instantiate(this.explodeFX, base.transform.position, true);
		}
		this.UpdateActivationProgress(-1f, false);
		this.idolMeshRoot.SetActive(false);
		this.tapCollision.enabled = false;
		base.transform.position = this.transitionPos;
		yield return new WaitForSeconds(this.floatDuration);
		this.idolMeshRoot.SetActive(true);
		this.tapCollision.enabled = true;
		if (this.startFallFX)
		{
			ObjectPools.instance.Instantiate(this.startFallFX, this.transitionPos, true);
		}
		this._audio.GTPlayOneShot(this._descentSound, 1f);
		this.trailFX.Play();
		float fall = 0f;
		Vector3 startPos = this.transitionPos;
		Vector3 destinationPos = this.finalPos;
		while (fall < this.fallDuration)
		{
			fall += Time.deltaTime;
			base.transform.position = Vector3.Lerp(startPos, destinationPos, fall / this.fallDuration);
			yield return null;
		}
		base.transform.position = destinationPos;
		this.trailFX.Stop();
		if (this.landedFX)
		{
			ObjectPools.instance.Instantiate(this.landedFX, destinationPos, true);
		}
		if (this.knockbackOnLand)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		yield return new WaitForSeconds(this.inactiveDuration);
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		float activateLerp = 0f;
		startPos = this.finalPos;
		destinationPos = this.finalPos + new Vector3(0f, this.activeHeight, 0f);
		AnimationCurve animCurve = AnimationCurves.EaseInOutQuad;
		while (activateLerp < 1f)
		{
			activateLerp = Mathf.Clamp01(activateLerp + Time.deltaTime / this.activationDuration);
			base.transform.position = Vector3.Lerp(startPos, destinationPos, animCurve.Evaluate(activateLerp));
			yield return null;
		}
		if (this.activatedFX)
		{
			ObjectPools.instance.Instantiate(this.activatedFX, base.transform.position, true);
		}
		if (this.knockbackOnActivate)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		this.isChangingPositions = false;
		yield break;
	}

	// Token: 0x06003A9A RID: 15002 RVA: 0x00135A1D File Offset: 0x00133C1D
	private float EaseInOut(float input)
	{
		if (input >= 0.5f)
		{
			return 1f - Mathf.Pow(-2f * input + 2f, 3f) / 2f;
		}
		return 4f * input * input * input;
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x00135B54 File Offset: 0x00133D54
	[CompilerGenerated]
	private IEnumerator <SetPosition>g__Unshrink|49_0()
	{
		float lerpVal = 0f;
		float growDuration = 0.5f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / growDuration;
			float num = Mathf.Lerp(0f, 1f, AnimationCurves.EaseOutQuad.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x00135B64 File Offset: 0x00133D64
	[CompilerGenerated]
	private void <DoLookingAround>g__PickLookTarget|54_0(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		Transform transform = this.<DoLookingAround>g__GetClosestPlayerPosition|54_2(ref A_1);
		A_1._lookDirection = (transform ? Quaternion.LookRotation(transform.position - this._lookRoot.position) : Quaternion.Euler((float)Random.Range(-15, 15), this._lookRoot.rotation.eulerAngles.y + (float)Random.Range(-45, 45), 0f));
		this.<DoLookingAround>g__SetLookTime|54_1(ref A_1);
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x00135BE2 File Offset: 0x00133DE2
	[CompilerGenerated]
	private void <DoLookingAround>g__SetLookTime|54_1(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		A_1.nextLookTime = Time.time + this._lookInterval / (float)this._activationState * 0.5f + Random.value;
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x00135C0C File Offset: 0x00133E0C
	[CompilerGenerated]
	private Transform <DoLookingAround>g__GetClosestPlayerPosition|54_2(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		if (Random.value < this._randomLookChance)
		{
			return null;
		}
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		Transform result = null;
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (!(vrrig == null))
			{
				bool flag = vrrig.OwningNetPlayer == this.zoneManager.CurrentGuardian;
				float num2 = Vector3.SqrMagnitude(vrrig.transform.position - position) * (float)(flag ? 100 : 1);
				if (num2 < num)
				{
					num = num2;
					result = vrrig.transform;
				}
			}
		}
		return result;
	}

	// Token: 0x040049DD RID: 18909
	[SerializeField]
	private GorillaGuardianZoneManager zoneManager;

	// Token: 0x040049DE RID: 18910
	[SerializeField]
	private float floatDuration = 2f;

	// Token: 0x040049DF RID: 18911
	[SerializeField]
	private float fallDuration = 1.5f;

	// Token: 0x040049E0 RID: 18912
	[SerializeField]
	private float inactiveDuration = 2f;

	// Token: 0x040049E1 RID: 18913
	[SerializeField]
	private float activationDuration = 1f;

	// Token: 0x040049E2 RID: 18914
	[SerializeField]
	private float activeHeight = 1f;

	// Token: 0x040049E3 RID: 18915
	[SerializeField]
	private bool knockbackOnTrigger;

	// Token: 0x040049E4 RID: 18916
	[SerializeField]
	private bool knockbackOnLand = true;

	// Token: 0x040049E5 RID: 18917
	[SerializeField]
	private bool knockbackOnActivate;

	// Token: 0x040049E6 RID: 18918
	[SerializeField]
	private Vector3 fallStartOffset = new Vector3(3f, 20f, 3f);

	// Token: 0x040049E7 RID: 18919
	[SerializeField]
	private ParticleSystem trailFX;

	// Token: 0x040049E8 RID: 18920
	[SerializeField]
	private ParticleSystem tapFX;

	// Token: 0x040049E9 RID: 18921
	[SerializeField]
	private GameObject explodeFX;

	// Token: 0x040049EA RID: 18922
	[SerializeField]
	private GameObject startFallFX;

	// Token: 0x040049EB RID: 18923
	[SerializeField]
	private GameObject landedFX;

	// Token: 0x040049EC RID: 18924
	[SerializeField]
	private GameObject activatedFX;

	// Token: 0x040049ED RID: 18925
	[SerializeField]
	private SphereCollider tapCollision;

	// Token: 0x040049EE RID: 18926
	[SerializeField]
	private GameObject idolVisualRoot;

	// Token: 0x040049EF RID: 18927
	[SerializeField]
	private GameObject idolMeshRoot;

	// Token: 0x040049F0 RID: 18928
	[SerializeField]
	private AnimationCurve bulgeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x040049F1 RID: 18929
	[SerializeField]
	private float bulgeScale = 1.1f;

	// Token: 0x040049F2 RID: 18930
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x040049F3 RID: 18931
	[SerializeField]
	private AudioClip[] _descentSound;

	// Token: 0x040049F4 RID: 18932
	[SerializeField]
	private AudioClip[] _activateSound;

	// Token: 0x040049F5 RID: 18933
	[SerializeField]
	private TappableGuardianIdol.IdolActivationSound[] _activationStageSounds;

	// Token: 0x040049F6 RID: 18934
	[SerializeField]
	private TappableGuardianIdol.StageActivatedObject[] _stageActivatedObjects;

	// Token: 0x040049F7 RID: 18935
	[Header("Look Around")]
	[SerializeField]
	private Transform _lookRoot;

	// Token: 0x040049F8 RID: 18936
	[SerializeField]
	private float _lookInterval = 10f;

	// Token: 0x040049F9 RID: 18937
	[SerializeField]
	private float _baseLookRate = 1f;

	// Token: 0x040049FA RID: 18938
	[SerializeField]
	private float _randomLookChance = 0.25f;

	// Token: 0x040049FB RID: 18939
	private Coroutine _lookRoutine;

	// Token: 0x040049FD RID: 18941
	private Vector3 transitionPos;

	// Token: 0x040049FE RID: 18942
	private Vector3 finalPos;

	// Token: 0x040049FF RID: 18943
	private int _activationState;

	// Token: 0x04004A00 RID: 18944
	private Coroutine _activationRoutine;

	// Token: 0x04004A01 RID: 18945
	private float _colliderBaseRadius;

	// Token: 0x04004A02 RID: 18946
	private bool _zoneIsActive = true;

	// Token: 0x04004A03 RID: 18947
	public bool isActivationReady;

	// Token: 0x04004A04 RID: 18948
	private float requiredTapDistance = 3f;

	// Token: 0x020008F1 RID: 2289
	[Serializable]
	public struct IdolActivationSound
	{
		// Token: 0x04004A05 RID: 18949
		public AudioClip activation;

		// Token: 0x04004A06 RID: 18950
		public AudioClip loop;
	}

	// Token: 0x020008F2 RID: 2290
	[Serializable]
	public struct StageActivatedObject
	{
		// Token: 0x06003AA0 RID: 15008 RVA: 0x00135CD8 File Offset: 0x00133ED8
		public void UpdateActiveState(int stage)
		{
			bool active = stage >= this.min && stage <= this.max;
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(active);
			}
		}

		// Token: 0x04004A07 RID: 18951
		public GameObject[] objects;

		// Token: 0x04004A08 RID: 18952
		public int min;

		// Token: 0x04004A09 RID: 18953
		public int max;
	}
}
