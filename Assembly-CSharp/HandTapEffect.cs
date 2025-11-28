using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000269 RID: 617
public class HandTapEffect : MonoBehaviour
{
	// Token: 0x06000FE1 RID: 4065 RVA: 0x00053A24 File Offset: 0x00051C24
	private void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		this.leftHandEffect.handContext = componentInParent.LeftHandEffect;
		this.rightHandEffect.handContext = componentInParent.RightHandEffect;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00053A5A File Offset: 0x00051C5A
	private void OnEnable()
	{
		this.leftHandEffect.OnEnable();
		this.rightHandEffect.OnEnable();
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00053A72 File Offset: 0x00051C72
	private void OnDisable()
	{
		this.leftHandEffect.OnDisable();
		this.rightHandEffect.OnDisable();
	}

	// Token: 0x040013B9 RID: 5049
	public HandTapEffect.HandTapEffectLeftRight leftHandEffect;

	// Token: 0x040013BA RID: 5050
	public HandTapEffect.HandTapEffectLeftRight rightHandEffect;

	// Token: 0x0200026A RID: 618
	[Serializable]
	public class HandTapEffectDownUp
	{
		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x00053A8A File Offset: 0x00051C8A
		public bool HasOverrides
		{
			get
			{
				return this.overrides.overrideSurfacePrefab || this.overrides.overrideGamemodePrefab || this.overrides.overrideSound;
			}
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00053AB4 File Offset: 0x00051CB4
		internal void OnTap(HandEffectContext handContext)
		{
			UnityEvent unityEvent = this.onTapUnityEvents;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			for (int i = 0; i < this.onTapBehaviours.Length; i++)
			{
				this.onTapBehaviours[i].OnTap(handContext);
			}
		}

		// Token: 0x040013BB RID: 5051
		public HandTapBehaviour[] onTapBehaviours;

		// Token: 0x040013BC RID: 5052
		public UnityEvent onTapUnityEvents;

		// Token: 0x040013BD RID: 5053
		[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
		public HashWrapper onTapPrefabToSpawn;

		// Token: 0x040013BE RID: 5054
		public HandTapOverrides overrides;
	}

	// Token: 0x0200026B RID: 619
	[Serializable]
	public class HandTapEffectLeftRight
	{
		// Token: 0x06000FE8 RID: 4072 RVA: 0x00053AF4 File Offset: 0x00051CF4
		public void OnEnable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = true;
			}
			if (this.downTapEffect.onTapPrefabToSpawn != -1)
			{
				this.handContext.AddFXPrefab(this.downTapEffect.onTapPrefabToSpawn);
			}
			if (this.downTapEffect.HasOverrides)
			{
				this.handContext.DownTapOverrides = this.downTapEffect.overrides;
			}
			if (this.upTapEffect.HasOverrides)
			{
				this.handContext.UpTapOverrides = this.upTapEffect.overrides;
			}
			this.handContext.handTapDown += new Action<HandEffectContext>(this.downTapEffect.OnTap);
			this.handContext.handTapUp += new Action<HandEffectContext>(this.upTapEffect.OnTap);
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00053BC4 File Offset: 0x00051DC4
		public void OnDisable()
		{
			if (this.separateUpTapCooldown)
			{
				this.handContext.SeparateUpTapCooldown = false;
			}
			if (this.downTapEffect.onTapPrefabToSpawn != -1)
			{
				this.handContext.RemoveFXPrefab(this.downTapEffect.onTapPrefabToSpawn);
			}
			if (this.downTapEffect.HasOverrides && this.handContext.DownTapOverrides == this.downTapEffect.overrides)
			{
				this.handContext.DownTapOverrides = null;
			}
			if (this.upTapEffect.HasOverrides && this.handContext.UpTapOverrides == this.upTapEffect.overrides)
			{
				this.handContext.UpTapOverrides = null;
			}
			this.handContext.handTapDown -= new Action<HandEffectContext>(this.downTapEffect.OnTap);
			this.handContext.handTapUp -= new Action<HandEffectContext>(this.upTapEffect.OnTap);
		}

		// Token: 0x040013BF RID: 5055
		public bool separateUpTapCooldown;

		// Token: 0x040013C0 RID: 5056
		public HandTapEffect.HandTapEffectDownUp downTapEffect;

		// Token: 0x040013C1 RID: 5057
		public HandTapEffect.HandTapEffectDownUp upTapEffect;

		// Token: 0x040013C2 RID: 5058
		internal HandEffectContext handContext;
	}
}
