using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x02001176 RID: 4470
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x060070D7 RID: 28887 RVA: 0x00002789 File Offset: 0x00000989
		private void Awake()
		{
		}

		// Token: 0x060070D8 RID: 28888 RVA: 0x0024EEEE File Offset: 0x0024D0EE
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x060070D9 RID: 28889 RVA: 0x0024EF02 File Offset: 0x0024D102
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x060070DA RID: 28890 RVA: 0x0024EF18 File Offset: 0x0024D118
		private void Update()
		{
			if (!this.timerStarted)
			{
				this.eyeStatus = AutoBlink.Status.Close;
				this.timerStarted = true;
			}
			if (this.timerStarted)
			{
				this.timeRemining -= Time.deltaTime;
				if (this.timeRemining <= 0f)
				{
					this.eyeStatus = AutoBlink.Status.Open;
					this.ResetTimer();
					return;
				}
				if (this.timeRemining <= this.timeBlink * 0.3f)
				{
					this.eyeStatus = AutoBlink.Status.HalfClose;
				}
			}
		}

		// Token: 0x060070DB RID: 28891 RVA: 0x0024EF8C File Offset: 0x0024D18C
		private void LateUpdate()
		{
			if (this.isActive && this.isBlink)
			{
				switch (this.eyeStatus)
				{
				case AutoBlink.Status.Close:
					this.SetCloseEyes();
					return;
				case AutoBlink.Status.HalfClose:
					this.SetHalfCloseEyes();
					return;
				case AutoBlink.Status.Open:
					this.SetOpenEyes();
					this.isBlink = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060070DC RID: 28892 RVA: 0x0024EFDE File Offset: 0x0024D1DE
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x060070DD RID: 28893 RVA: 0x0024F004 File Offset: 0x0024D204
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x060070DE RID: 28894 RVA: 0x0024F02A File Offset: 0x0024D22A
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x060070DF RID: 28895 RVA: 0x0024F050 File Offset: 0x0024D250
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				float num = Random.Range(0f, 1f);
				if (!this.isBlink && num > this.threshold)
				{
					this.isBlink = true;
				}
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		// Token: 0x040080E7 RID: 32999
		public bool isActive = true;

		// Token: 0x040080E8 RID: 33000
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x040080E9 RID: 33001
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x040080EA RID: 33002
		public float ratio_Close = 85f;

		// Token: 0x040080EB RID: 33003
		public float ratio_HalfClose = 20f;

		// Token: 0x040080EC RID: 33004
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x040080ED RID: 33005
		private bool timerStarted;

		// Token: 0x040080EE RID: 33006
		private bool isBlink;

		// Token: 0x040080EF RID: 33007
		public float timeBlink = 0.4f;

		// Token: 0x040080F0 RID: 33008
		private float timeRemining;

		// Token: 0x040080F1 RID: 33009
		public float threshold = 0.3f;

		// Token: 0x040080F2 RID: 33010
		public float interval = 3f;

		// Token: 0x040080F3 RID: 33011
		private AutoBlink.Status eyeStatus;

		// Token: 0x02001177 RID: 4471
		private enum Status
		{
			// Token: 0x040080F5 RID: 33013
			Close,
			// Token: 0x040080F6 RID: 33014
			HalfClose,
			// Token: 0x040080F7 RID: 33015
			Open
		}
	}
}
