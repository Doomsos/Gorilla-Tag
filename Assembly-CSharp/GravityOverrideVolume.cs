using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class GravityOverrideVolume : MonoBehaviour
{
	// Token: 0x06002179 RID: 8569 RVA: 0x000AF938 File Offset: 0x000ADB38
	private void OnEnable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter += this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit += this.OnColliderExitedVolume;
		}
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x000AF976 File Offset: 0x000ADB76
	private void OnDisable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit -= this.OnColliderExitedVolume;
		}
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x000AF9B4 File Offset: 0x000ADBB4
	private void OnColliderEnteredVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000AF9F4 File Offset: 0x000ADBF4
	private void OnColliderExitedVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000AFA28 File Offset: 0x000ADC28
	public void GravityOverrideFunction(GTPlayer player)
	{
		GravityOverrideVolume.GravityType gravityType = this.gravityType;
		if (gravityType == GravityOverrideVolume.GravityType.Directional)
		{
			Vector3 forward = this.referenceTransform.forward;
			player.AddForce(forward * this.strength, 5);
			return;
		}
		if (gravityType != GravityOverrideVolume.GravityType.Radial)
		{
			return;
		}
		Vector3 normalized = (this.referenceTransform.position - player.headCollider.transform.position).normalized;
		player.AddForce(normalized * this.strength, 5);
	}

	// Token: 0x04002C36 RID: 11318
	[SerializeField]
	private GravityOverrideVolume.GravityType gravityType;

	// Token: 0x04002C37 RID: 11319
	[SerializeField]
	private float strength = 9.8f;

	// Token: 0x04002C38 RID: 11320
	[SerializeField]
	[Tooltip("In Radial: the center point of gravity, In Directional: the forward vector of this transform defines the direction")]
	private Transform referenceTransform;

	// Token: 0x04002C39 RID: 11321
	[SerializeField]
	private CompositeTriggerEvents triggerEvents;

	// Token: 0x0200052E RID: 1326
	public enum GravityType
	{
		// Token: 0x04002C3B RID: 11323
		Directional,
		// Token: 0x04002C3C RID: 11324
		Radial
	}
}
