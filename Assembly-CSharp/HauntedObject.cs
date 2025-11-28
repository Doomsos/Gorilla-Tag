using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000802 RID: 2050
public class HauntedObject : MonoBehaviour
{
	// Token: 0x060035F5 RID: 13813 RVA: 0x00124D80 File Offset: 0x00122F80
	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(ref lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(ref wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.animators = base.transform.GetComponentsInChildren<Animator>();
	}

	// Token: 0x060035F6 RID: 13814 RVA: 0x00124E3C File Offset: 0x0012303C
	private void OnDestroy()
	{
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(ref lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(ref wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
	}

	// Token: 0x060035F7 RID: 13815 RVA: 0x00124EC7 File Offset: 0x001230C7
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x060035F8 RID: 13816 RVA: 0x00124EF0 File Offset: 0x001230F0
	private void TriggerEffects(GameObject go)
	{
		if (base.gameObject != go)
		{
			return;
		}
		if (this.rattle)
		{
			base.StartCoroutine(this.Shake());
		}
		if (this.audioSource && this.hauntedSound)
		{
			this.audioSource.GTPlayOneShot(this.hauntedSound, 1f);
		}
		if (this.FBXprefab)
		{
			ObjectPools.instance.Instantiate(this.FBXprefab, base.transform.position, true);
		}
		if (this.TurnOffLight != null)
		{
			base.StartCoroutine(this.TurnOff());
		}
		foreach (Animator animator in this.animators)
		{
			if (animator)
			{
				animator.SetTrigger(HauntedObject._animHaunted);
			}
		}
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x00124FC2 File Offset: 0x001231C2
	private IEnumerator Shake()
	{
		while (this.passedTime < this.duration)
		{
			this.passedTime += Time.deltaTime;
			base.transform.position = new Vector3(this.initialPos.x + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.y + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.z);
			yield return null;
		}
		this.passedTime = 0f;
		yield break;
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x00124FD1 File Offset: 0x001231D1
	private IEnumerator TurnOff()
	{
		this.TurnOffLight.gameObject.SetActive(false);
		while (this.lightPassedTime < this.TurnOffDuration)
		{
			this.lightPassedTime += Time.deltaTime;
			yield return null;
		}
		this.TurnOffLight.SetActive(true);
		this.lightPassedTime = 0f;
		yield break;
	}

	// Token: 0x04004545 RID: 17733
	private static readonly int _animHaunted = Animator.StringToHash("Haunted");

	// Token: 0x04004546 RID: 17734
	private const string _lurkerGhost = "LurkerGhost";

	// Token: 0x04004547 RID: 17735
	private const string _wanderingGhost = "WanderingGhost";

	// Token: 0x04004548 RID: 17736
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04004549 RID: 17737
	public float speed = 60f;

	// Token: 0x0400454A RID: 17738
	public float amount = 0.01f;

	// Token: 0x0400454B RID: 17739
	public float duration = 1f;

	// Token: 0x0400454C RID: 17740
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x0400454D RID: 17741
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x0400454E RID: 17742
	public float TurnOffDuration = 2f;

	// Token: 0x0400454F RID: 17743
	private Vector3 initialPos;

	// Token: 0x04004550 RID: 17744
	private float passedTime;

	// Token: 0x04004551 RID: 17745
	private float lightPassedTime;

	// Token: 0x04004552 RID: 17746
	private GameObject lurkerGhost;

	// Token: 0x04004553 RID: 17747
	private GameObject wanderingGhost;

	// Token: 0x04004554 RID: 17748
	private Animator[] animators;

	// Token: 0x04004555 RID: 17749
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004556 RID: 17750
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
