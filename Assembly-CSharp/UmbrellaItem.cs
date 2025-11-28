using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class UmbrellaItem : TransferrableObject
{
	// Token: 0x06001EE2 RID: 7906 RVA: 0x000A3C86 File Offset: 0x000A1E86
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000A3C98 File Offset: 0x000A1E98
	public override void OnActivate()
	{
		base.OnActivate();
		float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float soundVolume = 0.08f;
		int soundIndex;
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			soundIndex = this.SoundIdOpen;
			this.itemState = TransferrableObject.ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(this.umbrellaRainDestroyTrigger);
		}
		else
		{
			soundIndex = this.SoundIdClose;
			this.itemState = TransferrableObject.ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		}
		base.ActivateItemFX(hapticStrength, fixedDeltaTime, soundIndex, soundVolume);
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x000A3D30 File Offset: 0x000A1F30
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000A3D3E File Offset: 0x000A1F3E
	internal override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000A3D5E File Offset: 0x000A1F5E
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		this.itemState = TransferrableObject.ItemStates.State1;
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000A3D8B File Offset: 0x000A1F8B
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.OnActivate();
		}
		return true;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000A3DB4 File Offset: 0x000A1FB4
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaItem.UmbrellaStates itemState = (UmbrellaItem.UmbrellaStates)this.itemState;
		if (itemState != this.previousUmbrellaState)
		{
			this.OnUmbrellaStateChanged();
		}
		this.UpdateAngles((itemState == UmbrellaItem.UmbrellaStates.UmbrellaOpen) ? this.startingAngles : this.endingAngles, this.lerpValue);
		this.previousUmbrellaState = itemState;
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000A3E04 File Offset: 0x000A2004
	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = this.itemState == TransferrableObject.ItemStates.State0;
		GameObject[] array = this.gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		ParticleSystem[] array2;
		if (flag)
		{
			array2 = this.particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			return;
		}
		array2 = this.particlesEmitOnOpen;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop();
		}
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000A3E78 File Offset: 0x000A2078
	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < this.umbrellaBones.Length; i++)
		{
			this.umbrellaBones[i].localRotation = Quaternion.Lerp(this.umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x000A3EC0 File Offset: 0x000A20C0
	protected void GenerateAngles()
	{
		this.startingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int i = 0; i < this.endingAngles.Length; i++)
		{
			this.startingAngles[i] = this.umbrellaToCopy.startingAngles[i];
		}
		this.endingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int j = 0; j < this.endingAngles.Length; j++)
		{
			this.endingAngles[j] = this.umbrellaToCopy.endingAngles[j];
		}
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0400292E RID: 10542
	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	// Token: 0x0400292F RID: 10543
	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	// Token: 0x04002930 RID: 10544
	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	// Token: 0x04002931 RID: 10545
	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	private UmbrellaItem umbrellaToCopy;

	// Token: 0x04002932 RID: 10546
	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	// Token: 0x04002933 RID: 10547
	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	// Token: 0x04002934 RID: 10548
	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	// Token: 0x04002935 RID: 10549
	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	// Token: 0x04002936 RID: 10550
	[GorillaSoundLookup]
	public int SoundIdOpen = 64;

	// Token: 0x04002937 RID: 10551
	[GorillaSoundLookup]
	public int SoundIdClose = 65;

	// Token: 0x04002938 RID: 10552
	private UmbrellaItem.UmbrellaStates previousUmbrellaState = UmbrellaItem.UmbrellaStates.UmbrellaOpen;

	// Token: 0x020004AC RID: 1196
	private enum UmbrellaStates
	{
		// Token: 0x0400293A RID: 10554
		UmbrellaOpen = 1,
		// Token: 0x0400293B RID: 10555
		UmbrellaClosed
	}
}
