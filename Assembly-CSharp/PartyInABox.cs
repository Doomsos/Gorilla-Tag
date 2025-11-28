using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class PartyInABox : MonoBehaviour
{
	// Token: 0x06001048 RID: 4168 RVA: 0x0005553E File Offset: 0x0005373E
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x0005553E File Offset: 0x0005373E
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00055546 File Offset: 0x00053746
	public void Cranked_ReleaseParty()
	{
		if (!this.parentHoldable.IsLocalObject())
		{
			return;
		}
		this.ReleaseParty();
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x0005555C File Offset: 0x0005375C
	public void ReleaseParty()
	{
		if (this.isReleased)
		{
			return;
		}
		if (this.parentHoldable.IsLocalObject())
		{
			this.parentHoldable.itemState |= TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(true, this.partyHapticStrength, this.partyHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.partyHapticStrength, this.partyHapticDuration);
		}
		this.isReleased = true;
		this.spring.enabled = true;
		this.anim.Play();
		this.particles.Play();
		this.partyAudio.Play();
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x000555F8 File Offset: 0x000537F8
	private void Update()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			return;
		}
		if (this.parentHoldable.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.isReleased)
			{
				this.ReleaseParty();
				return;
			}
		}
		else if (this.isReleased)
		{
			this.Reset();
		}
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00055650 File Offset: 0x00053850
	public void Reset()
	{
		this.isReleased = false;
		this.parentHoldable.itemState &= (TransferrableObject.ItemStates)(-2);
		this.spring.enabled = false;
		this.anim.Stop();
		foreach (PartyInABox.ForceTransform forceTransform in this.forceTransforms)
		{
			forceTransform.Apply();
		}
	}

	// Token: 0x0400143C RID: 5180
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x0400143D RID: 5181
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x0400143E RID: 5182
	[SerializeField]
	private Animation anim;

	// Token: 0x0400143F RID: 5183
	[SerializeField]
	private SpringyWobbler spring;

	// Token: 0x04001440 RID: 5184
	[SerializeField]
	private AudioSource partyAudio;

	// Token: 0x04001441 RID: 5185
	[SerializeField]
	private float partyHapticStrength;

	// Token: 0x04001442 RID: 5186
	[SerializeField]
	private float partyHapticDuration;

	// Token: 0x04001443 RID: 5187
	private bool isReleased;

	// Token: 0x04001444 RID: 5188
	[SerializeField]
	private PartyInABox.ForceTransform[] forceTransforms;

	// Token: 0x0200027B RID: 635
	[Serializable]
	private struct ForceTransform
	{
		// Token: 0x0600104F RID: 4175 RVA: 0x000556B3 File Offset: 0x000538B3
		public void Apply()
		{
			this.transform.localPosition = this.localPosition;
			this.transform.localRotation = this.localRotation;
		}

		// Token: 0x04001445 RID: 5189
		public Transform transform;

		// Token: 0x04001446 RID: 5190
		public Vector3 localPosition;

		// Token: 0x04001447 RID: 5191
		public Quaternion localRotation;
	}
}
