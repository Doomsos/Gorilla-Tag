using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200110C RID: 4364
	public class ParticleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06006D2E RID: 27950 RVA: 0x0023DB38 File Offset: 0x0023BD38
		private void Awake()
		{
			this.StoreOriginalValues();
			this.currentIndex = -1;
		}

		// Token: 0x06006D2F RID: 27951 RVA: 0x0023DB47 File Offset: 0x0023BD47
		private void OnValidate()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x06006D30 RID: 27952 RVA: 0x0023DB47 File Offset: 0x0023BD47
		private void OnEnable()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x06006D31 RID: 27953 RVA: 0x0023DB4F File Offset: 0x0023BD4F
		private void OnDisable()
		{
			this.ResetToOriginal();
		}

		// Token: 0x06006D32 RID: 27954 RVA: 0x0023DB58 File Offset: 0x0023BD58
		private void StoreOriginalValues()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			this.originalStartSize = main.startSize.constant;
			this.originalStartColor = main.startColor.color;
		}

		// Token: 0x06006D33 RID: 27955 RVA: 0x0023DBAA File Offset: 0x0023BDAA
		public void ApplySetting(ParticleSettingsSO setting)
		{
			this.SetStartSize(setting.startSize);
			this.SetStartColor(setting.startColor);
		}

		// Token: 0x06006D34 RID: 27956 RVA: 0x0023DBC4 File Offset: 0x0023BDC4
		public void ApplySettingLerp(ParticleSettingsSO setting)
		{
			this.LerpStartSize(setting.startSize);
			this.LerpStartColor(setting.startColor);
		}

		// Token: 0x06006D35 RID: 27957 RVA: 0x0023DBE0 File Offset: 0x0023BDE0
		public void MoveToNextSetting()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[this.currentIndex];
				this.ApplySetting(setting);
			}
		}

		// Token: 0x06006D36 RID: 27958 RVA: 0x0023DC2C File Offset: 0x0023BE2C
		public void MoveToNextSettingLerp()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[this.currentIndex];
				this.ApplySettingLerp(setting);
			}
		}

		// Token: 0x06006D37 RID: 27959 RVA: 0x0023DC75 File Offset: 0x0023BE75
		public void ResetSettings()
		{
			this.currentIndex = -1;
			this.ResetToOriginal();
		}

		// Token: 0x06006D38 RID: 27960 RVA: 0x0023DC84 File Offset: 0x0023BE84
		public void MoveToSettingIndex(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[index];
				this.ApplySetting(setting);
			}
		}

		// Token: 0x06006D39 RID: 27961 RVA: 0x0023DCB0 File Offset: 0x0023BEB0
		public void MoveToSettingIndexLerp(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[index];
				this.ApplySettingLerp(setting);
			}
		}

		// Token: 0x06006D3A RID: 27962 RVA: 0x0023DCDC File Offset: 0x0023BEDC
		public void SetStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startSize = size;
			this.targetSize = default(float?);
		}

		// Token: 0x06006D3B RID: 27963 RVA: 0x0023DD20 File Offset: 0x0023BF20
		public void IncreaseStartSize(float delta)
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			float constant = main.startSize.constant;
			main.startSize = constant + delta;
			this.targetSize = default(float?);
		}

		// Token: 0x06006D3C RID: 27964 RVA: 0x0023DD74 File Offset: 0x0023BF74
		public void LerpStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			if (Mathf.Abs(this.ps.main.startSize.constant - size) < 0.01f)
			{
				return;
			}
			this.targetSize = new float?(size);
		}

		// Token: 0x06006D3D RID: 27965 RVA: 0x0023DDC8 File Offset: 0x0023BFC8
		public void SetStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startColor = color;
			this.targetColor = default(Color?);
		}

		// Token: 0x06006D3E RID: 27966 RVA: 0x0023DE0C File Offset: 0x0023C00C
		public void LerpStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			Color color2 = this.ps.main.startColor.color;
			if (this.IsColorApproximatelyEqual(color2, color, 0.0001f))
			{
				return;
			}
			this.targetColor = new Color?(color);
		}

		// Token: 0x06006D3F RID: 27967 RVA: 0x0023DE60 File Offset: 0x0023C060
		public void SetStartValues(float size, Color color)
		{
			this.SetStartSize(size);
			this.SetStartColor(color);
		}

		// Token: 0x06006D40 RID: 27968 RVA: 0x0023DE70 File Offset: 0x0023C070
		public void LerpStartValues(float size, Color color)
		{
			this.LerpStartSize(size);
			this.LerpStartColor(color);
		}

		// Token: 0x06006D41 RID: 27969 RVA: 0x0023DE80 File Offset: 0x0023C080
		private void Update()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			if (this.targetSize != null)
			{
				float num = Mathf.Lerp(main.startSize.constant, this.targetSize.Value, Time.deltaTime * this.transitionSpeed);
				main.startSize = num;
				if (Mathf.Abs(num - this.targetSize.Value) < 0.01f)
				{
					main.startSize = this.targetSize.Value;
					this.targetSize = default(float?);
				}
			}
			if (this.targetColor != null)
			{
				Color color = Color.Lerp(main.startColor.color, this.targetColor.Value, Time.deltaTime * this.transitionSpeed);
				main.startColor = color;
				if (this.IsColorApproximatelyEqual(color, this.targetColor.Value, 0.0001f))
				{
					main.startColor = this.targetColor.Value;
					this.targetColor = default(Color?);
				}
			}
		}

		// Token: 0x06006D42 RID: 27970 RVA: 0x0023DFB0 File Offset: 0x0023C1B0
		[ContextMenu("Reset To Original")]
		public void ResetToOriginal()
		{
			if (this.ps == null)
			{
				return;
			}
			this.targetSize = default(float?);
			this.targetColor = default(Color?);
			ParticleSystem.MainModule main = this.ps.main;
			main.startSize = this.originalStartSize;
			main.startColor = this.originalStartColor;
		}

		// Token: 0x06006D43 RID: 27971 RVA: 0x0023E014 File Offset: 0x0023C214
		private bool IsColorApproximatelyEqual(Color a, Color b, float threshold = 0.0001f)
		{
			float num = a.r - b.r;
			float num2 = a.g - b.g;
			float num3 = a.b - b.b;
			float num4 = a.a - b.a;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4 < threshold;
		}

		// Token: 0x04007E62 RID: 32354
		[SerializeField]
		private ParticleSystem ps;

		// Token: 0x04007E63 RID: 32355
		[Tooltip("For calling gradual functions only")]
		[SerializeField]
		private float transitionSpeed = 5f;

		// Token: 0x04007E64 RID: 32356
		public ParticleSettingsSO[] particleSettings = new ParticleSettingsSO[0];

		// Token: 0x04007E65 RID: 32357
		private float originalStartSize;

		// Token: 0x04007E66 RID: 32358
		private Color originalStartColor;

		// Token: 0x04007E67 RID: 32359
		private float? targetSize;

		// Token: 0x04007E68 RID: 32360
		private Color? targetColor;

		// Token: 0x04007E69 RID: 32361
		private int currentIndex;
	}
}
