using System;
using emotitron.Compression;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FF3 RID: 4083
	public class DrinkableHoldable : TransferrableObject
	{
		// Token: 0x06006723 RID: 26403 RVA: 0x00218A58 File Offset: 0x00216C58
		internal override void OnEnable()
		{
			base.OnEnable();
			base.enabled = (this.containerLiquid != null);
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.sipSoundCooldown, this.containerLiquid.fillAmount, this.coolingDown);
			this.myByteArray = new byte[32];
		}

		// Token: 0x06006724 RID: 26404 RVA: 0x00218AAC File Offset: 0x00216CAC
		protected override void LateUpdateLocal()
		{
			if (!this.containerLiquid.isActiveAndEnabled || !GorillaParent.hasInstance || !GorillaComputer.hasInstance)
			{
				base.LateUpdateLocal();
				return;
			}
			float num = (float)((GorillaComputer.instance.startupMillis + (long)Time.realtimeSinceStartup * 1000L) % 259200000L) / 1000f;
			if (Mathf.Abs(num - this.lastTimeSipSoundPlayed) > 129600f)
			{
				this.lastTimeSipSoundPlayed = num;
			}
			float num2 = this.sipRadius * this.sipRadius;
			bool flag = (GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2;
			if (!flag)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!vrrig.isOfflineVRRig)
					{
						if (flag || vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						flag = ((vrrig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2);
					}
				}
			}
			if (flag)
			{
				this.containerLiquid.fillAmount = Mathf.Clamp01(this.containerLiquid.fillAmount - this.sipRate * Time.deltaTime);
				if (num > this.lastTimeSipSoundPlayed + this.sipSoundCooldown)
				{
					if (!this.wasSipping)
					{
						this.lastTimeSipSoundPlayed = num;
						this.coolingDown = true;
					}
				}
				else
				{
					this.coolingDown = false;
				}
			}
			this.wasSipping = flag;
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.lastTimeSipSoundPlayed, this.containerLiquid.fillAmount, this.coolingDown);
			base.LateUpdateLocal();
		}

		// Token: 0x06006725 RID: 26405 RVA: 0x00218CA8 File Offset: 0x00216EA8
		protected override void LateUpdateReplicated()
		{
			base.LateUpdateReplicated();
			int itemState = (int)this.itemState;
			this.UnpackValuesNonstatic(itemState, out this.lastTimeSipSoundPlayed, out this.containerLiquid.fillAmount, out this.coolingDown);
		}

		// Token: 0x06006726 RID: 26406 RVA: 0x00218CE1 File Offset: 0x00216EE1
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (this.coolingDown && !this.wasCoolingDown)
			{
				this.sipSoundBankPlayer.Play();
			}
			this.wasCoolingDown = this.coolingDown;
		}

		// Token: 0x06006727 RID: 26407 RVA: 0x00218D10 File Offset: 0x00216F10
		private static int PackValues(float cooldownStartTime, float fillAmount, bool coolingDown)
		{
			byte[] array = new byte[32];
			int num = 0;
			array.WriteBool(coolingDown, ref num);
			array.Write((ulong)((double)cooldownStartTime * 100.0), ref num, 25);
			array.Write((ulong)((double)fillAmount * 63.0), ref num, 6);
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x06006728 RID: 26408 RVA: 0x00218D64 File Offset: 0x00216F64
		private void UnpackValuesNonstatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			DrinkableHoldable.GetBytes(packed, ref this.myByteArray);
			int num = 0;
			coolingDown = this.myByteArray.ReadBool(ref num);
			cooldownStartTime = (float)(this.myByteArray.Read(ref num, 25) / 100.0);
			fillAmount = this.myByteArray.Read(ref num, 6) / 63f;
		}

		// Token: 0x06006729 RID: 26409 RVA: 0x00218DC8 File Offset: 0x00216FC8
		public static void GetBytes(int value, ref byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)(value >> 8 * i & 255);
			}
		}

		// Token: 0x0600672A RID: 26410 RVA: 0x00218DF8 File Offset: 0x00216FF8
		private static void UnpackValuesStatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			byte[] bytes = BitConverter.GetBytes(packed);
			int num = 0;
			coolingDown = bytes.ReadBool(ref num);
			cooldownStartTime = (float)(bytes.Read(ref num, 25) / 100.0);
			fillAmount = bytes.Read(ref num, 6) / 63f;
		}

		// Token: 0x040075B0 RID: 30128
		[AssignInCorePrefab]
		public ContainerLiquid containerLiquid;

		// Token: 0x040075B1 RID: 30129
		[AssignInCorePrefab]
		[SoundBankInfo]
		public SoundBankPlayer sipSoundBankPlayer;

		// Token: 0x040075B2 RID: 30130
		[AssignInCorePrefab]
		public float sipRate = 0.1f;

		// Token: 0x040075B3 RID: 30131
		[AssignInCorePrefab]
		public float sipSoundCooldown = 0.5f;

		// Token: 0x040075B4 RID: 30132
		[AssignInCorePrefab]
		public Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x040075B5 RID: 30133
		[AssignInCorePrefab]
		public float sipRadius = 0.15f;

		// Token: 0x040075B6 RID: 30134
		private float lastTimeSipSoundPlayed;

		// Token: 0x040075B7 RID: 30135
		private bool wasSipping;

		// Token: 0x040075B8 RID: 30136
		private bool coolingDown;

		// Token: 0x040075B9 RID: 30137
		private bool wasCoolingDown;

		// Token: 0x040075BA RID: 30138
		private byte[] myByteArray;
	}
}
