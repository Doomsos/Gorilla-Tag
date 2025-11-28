using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000AC RID: 172
public class HoldableLighterCosmetic : MonoBehaviour
{
	// Token: 0x06000456 RID: 1110 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00019001 File Offset: 0x00017201
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x0001901B File Offset: 0x0001721B
	private bool IsMyItem()
	{
		return this.rig != null && this.rig.isOfflineVRRig;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00019038 File Offset: 0x00017238
	private void DebugPull()
	{
		this.TriggerPulled();
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00019040 File Offset: 0x00017240
	private void DebugRelease()
	{
		this.TriggerReleased();
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00019048 File Offset: 0x00017248
	public void TriggerPulled()
	{
		this.triggerHeld = true;
		if (this.OwnerID == 0)
		{
			this.TrySetID();
		}
		double time = PhotonNetwork.Time;
		switch (this.GetResultAtTime(time, this.OwnerID))
		{
		case HoldableLighterCosmetic.LighterResult.Flicker:
		{
			UnityEvent onFlicker = this.OnFlicker;
			if (onFlicker != null)
			{
				onFlicker.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Light:
		{
			UnityEvent onLight = this.OnLight;
			if (onLight != null)
			{
				onLight.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.1f, 0.1f);
				return;
			}
			break;
		}
		case HoldableLighterCosmetic.LighterResult.Explode:
		{
			UnityEvent onExplode = this.OnExplode;
			if (onExplode != null)
			{
				onExplode.Invoke();
			}
			if (this.parentTransferable.IsMyItem())
			{
				GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), 0.75f, 0.5f);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00019150 File Offset: 0x00017350
	private HoldableLighterCosmetic.LighterResult GetResultAtTime(double photonTime, int seed)
	{
		int num = (int)Math.Floor(photonTime);
		float num2 = (float)new Random(seed ^ num).NextDouble();
		if (num2 < this.explodeWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Explode;
		}
		if (num2 < this.explodeWeight + this.lightWeight)
		{
			return HoldableLighterCosmetic.LighterResult.Light;
		}
		return HoldableLighterCosmetic.LighterResult.Flicker;
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00019192 File Offset: 0x00017392
	public void TriggerReleased()
	{
		this.triggerHeld = false;
		UnityEvent onTriggerRelease = this.OnTriggerRelease;
		if (onTriggerRelease == null)
		{
			return;
		}
		onTriggerRelease.Invoke();
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x000191AC File Offset: 0x000173AC
	private void TrySetID()
	{
		if (this.parentTransferable.IsLocalObject())
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
		{
			string userId = this.parentTransferable.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x040004E6 RID: 1254
	private int OwnerID;

	// Token: 0x040004E7 RID: 1255
	[Header("Weights (0 to 1 total)")]
	[Range(0f, 1f)]
	public float flickerWeight = 0.5f;

	// Token: 0x040004E8 RID: 1256
	[Range(0f, 1f)]
	public float lightWeight = 0.3f;

	// Token: 0x040004E9 RID: 1257
	[Range(0f, 1f)]
	public float explodeWeight = 0.2f;

	// Token: 0x040004EA RID: 1258
	[Header("Unity Events")]
	public UnityEvent OnFlicker;

	// Token: 0x040004EB RID: 1259
	public UnityEvent OnLight;

	// Token: 0x040004EC RID: 1260
	public UnityEvent OnExplode;

	// Token: 0x040004ED RID: 1261
	public UnityEvent OnTriggerRelease;

	// Token: 0x040004EE RID: 1262
	private HoldableLighterCosmetic.LighterResult[] resultTimeline;

	// Token: 0x040004EF RID: 1263
	private bool triggerHeld;

	// Token: 0x040004F0 RID: 1264
	private float lastCheckTime;

	// Token: 0x040004F1 RID: 1265
	private VRRig rig;

	// Token: 0x040004F2 RID: 1266
	private TransferrableObject parentTransferable;

	// Token: 0x020000AD RID: 173
	public enum LighterResult
	{
		// Token: 0x040004F4 RID: 1268
		Flicker,
		// Token: 0x040004F5 RID: 1269
		Light,
		// Token: 0x040004F6 RID: 1270
		Explode
	}
}
