using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010FC RID: 4348
	public class GlowBugsInJar : MonoBehaviour
	{
		// Token: 0x06006CE1 RID: 27873 RVA: 0x0023C0F0 File Offset: 0x0023A2F0
		private void OnEnable()
		{
			this.shakeStarted = false;
			this.UpdateGlow(0f);
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShakeEvent);
			}
		}

		// Token: 0x06006CE2 RID: 27874 RVA: 0x0023C1C8 File Offset: 0x0023A3C8
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShakeEvent);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006CE3 RID: 27875 RVA: 0x0023C218 File Offset: 0x0023A418
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShakeEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args != null && args.Length == 1)
			{
				object obj = args[0];
				if (obj is bool)
				{
					bool flag = (bool)obj;
					if (flag)
					{
						this.ShakeStartLocal();
						return;
					}
					this.ShakeEndLocal();
					return;
				}
			}
		}

		// Token: 0x06006CE4 RID: 27876 RVA: 0x0023C278 File Offset: 0x0023A478
		public void HandleOnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					true
				});
			}
			this.ShakeStartLocal();
		}

		// Token: 0x06006CE5 RID: 27877 RVA: 0x0023C2D2 File Offset: 0x0023A4D2
		private void ShakeStartLocal()
		{
			this.currentGlowAmount = 0f;
			this.shakeStarted = true;
			this.shakeTimer = 0f;
		}

		// Token: 0x06006CE6 RID: 27878 RVA: 0x0023C2F4 File Offset: 0x0023A4F4
		public void HandleOnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					false
				});
			}
			this.ShakeEndLocal();
		}

		// Token: 0x06006CE7 RID: 27879 RVA: 0x0023C34E File Offset: 0x0023A54E
		private void ShakeEndLocal()
		{
			this.shakeStarted = false;
			this.shakeTimer = 0f;
		}

		// Token: 0x06006CE8 RID: 27880 RVA: 0x0023C364 File Offset: 0x0023A564
		public void Update()
		{
			if (this.shakeStarted)
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount < 1f)
				{
					this.currentGlowAmount += this.glowIncreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
					return;
				}
			}
			else
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount > 0f)
				{
					this.currentGlowAmount -= this.glowDecreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
				}
			}
		}

		// Token: 0x06006CE9 RID: 27881 RVA: 0x0023C430 File Offset: 0x0023A630
		private void UpdateGlow(float value)
		{
			if (this.renderers.Length != 0)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Material material = this.renderers[i].material;
					Color color = material.GetColor(this.shaderProperty);
					color.a = value;
					material.SetColor(this.shaderProperty, color);
					material.EnableKeyword("_EMISSION");
				}
			}
		}

		// Token: 0x04007DF9 RID: 32249
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04007DFA RID: 32250
		[Space]
		[Tooltip("Time interval - every X seconds update the glow value")]
		[SerializeField]
		private float glowUpdateInterval = 2f;

		// Token: 0x04007DFB RID: 32251
		[Tooltip("step increment - increase the glow value one step for N amount")]
		[SerializeField]
		private float glowIncreaseStepAmount = 0.1f;

		// Token: 0x04007DFC RID: 32252
		[Tooltip("step decrement - decrease the glow value one step for N amount")]
		[SerializeField]
		private float glowDecreaseStepAmount = 0.2f;

		// Token: 0x04007DFD RID: 32253
		[Space]
		[SerializeField]
		private string shaderProperty = "_EmissionColor";

		// Token: 0x04007DFE RID: 32254
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04007DFF RID: 32255
		private bool shakeStarted = true;

		// Token: 0x04007E00 RID: 32256
		private static int EmissionColor;

		// Token: 0x04007E01 RID: 32257
		private float currentGlowAmount;

		// Token: 0x04007E02 RID: 32258
		private float shakeTimer;

		// Token: 0x04007E03 RID: 32259
		private RubberDuckEvents _events;

		// Token: 0x04007E04 RID: 32260
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);
	}
}
