using System;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E0F RID: 3599
	public class CMSTrigger : MonoBehaviour
	{
		// Token: 0x060059D0 RID: 22992 RVA: 0x001CBABB File Offset: 0x001C9CBB
		public void OnEnable()
		{
			if (this.onEnableTriggerDelay > 0.0)
			{
				this.enabledTime = (double)Time.time;
			}
		}

		// Token: 0x060059D1 RID: 22993 RVA: 0x001CBADA File Offset: 0x001C9CDA
		public byte GetID()
		{
			return this.id;
		}

		// Token: 0x060059D2 RID: 22994 RVA: 0x001CBAE4 File Offset: 0x001C9CE4
		public virtual void CopyTriggerSettings(TriggerSettings settings)
		{
			this.id = settings.triggerId;
			this.triggeredBy = settings.triggeredBy;
			float num = Math.Max(settings.validationDistance, 2f);
			this.validationDistanceSquared = num * num;
			if (this.triggeredBy == null)
			{
				if (settings.triggeredByHead && !settings.triggeredByBody)
				{
					this.triggeredBy = 2;
				}
				else if (settings.triggeredByBody && !settings.triggeredByHead)
				{
					this.triggeredBy = 3;
				}
				else if (settings.triggeredByHands && !settings.triggeredByHead && !settings.triggeredByBody)
				{
					this.triggeredBy = 1;
				}
				else
				{
					this.triggeredBy = 4;
				}
			}
			TriggerSource triggerSource = this.triggeredBy;
			if (triggerSource != 1)
			{
				if (triggerSource - 2 <= 2)
				{
					base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				}
			}
			else
			{
				base.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			}
			this.onEnableTriggerDelay = settings.onEnableTriggerDelay;
			this.generalRetriggerDelay = settings.generalRetriggerDelay;
			this.retriggerAfterDuration = settings.retriggerAfterDuration;
			if (Math.Abs(settings.retriggerDelay - 2f) > 0.001f && Math.Abs(settings.retriggerStayDuration - 2.0) < 0.001)
			{
				settings.retriggerStayDuration = (double)settings.retriggerDelay;
			}
			this.retriggerStayDuration = Math.Max(this.generalRetriggerDelay, settings.retriggerStayDuration);
			if (this.retriggerStayDuration <= 0.0)
			{
				this.retriggerAfterDuration = false;
			}
			this.numAllowedTriggers = settings.numAllowedTriggers;
			this.syncedToAllPlayers = settings.syncedToAllPlayers_private;
			if (this.syncedToAllPlayers)
			{
				CMSSerializer.RegisterTrigger(base.gameObject.scene.name, this);
			}
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x060059D3 RID: 22995 RVA: 0x001CBCB5 File Offset: 0x001C9EB5
		public void OnTriggerEnter(Collider triggeringCollider)
		{
			if (this.ValidateCollider(triggeringCollider) && this.CanTrigger())
			{
				this.OnTriggerActivation(triggeringCollider);
			}
		}

		// Token: 0x060059D4 RID: 22996 RVA: 0x001CBCD0 File Offset: 0x001C9ED0
		private void OnTriggerStay(Collider other)
		{
			if (!this.retriggerAfterDuration)
			{
				return;
			}
			if (this.ValidateCollider(other) && this.CanTrigger())
			{
				double num = (double)Time.time;
				if (NetworkSystem.Instance.InRoom)
				{
					num = PhotonNetwork.Time;
				}
				if (this.lastTriggerTime + this.retriggerStayDuration <= num)
				{
					this.OnTriggerActivation(other);
				}
			}
		}

		// Token: 0x060059D5 RID: 22997 RVA: 0x001CBD28 File Offset: 0x001C9F28
		private bool ValidateCollider(Collider other)
		{
			GameObject gameObject = other.gameObject;
			bool flag = gameObject == GorillaTagger.Instance.headCollider.gameObject && (this.triggeredBy == 2 || this.triggeredBy == 4);
			bool flag2;
			if (GorillaTagger.Instance.bodyCollider.enabled)
			{
				flag2 = (gameObject == GorillaTagger.Instance.bodyCollider.gameObject && (this.triggeredBy == 3 || this.triggeredBy == 4));
			}
			else
			{
				flag2 = (gameObject == VRRig.LocalRig.gameObject && (this.triggeredBy == 3 || this.triggeredBy == 4));
			}
			bool flag3 = (gameObject == GorillaTagger.Instance.leftHandTriggerCollider.gameObject || gameObject == GorillaTagger.Instance.rightHandTriggerCollider.gameObject) && this.triggeredBy == 1;
			return flag || flag2 || flag3;
		}

		// Token: 0x060059D6 RID: 22998 RVA: 0x001CBE1A File Offset: 0x001CA01A
		private void OnTriggerActivation(Collider activatingCollider)
		{
			if (this.syncedToAllPlayers)
			{
				CMSSerializer.RequestTrigger(this.id);
				return;
			}
			this.Trigger(-1.0, true, false);
		}

		// Token: 0x060059D7 RID: 22999 RVA: 0x001CBE44 File Offset: 0x001CA044
		public bool CanTrigger()
		{
			if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
			{
				return false;
			}
			if (this.onEnableTriggerDelay > 0.0 && (double)Time.time - this.enabledTime < this.onEnableTriggerDelay)
			{
				return false;
			}
			if (this.generalRetriggerDelay <= 0.0)
			{
				return true;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (PhotonNetwork.Time - this.lastTriggerTime < -1.0)
				{
					this.lastTriggerTime = -(4294967.295 - this.lastTriggerTime);
				}
				if (this.lastTriggerTime + this.generalRetriggerDelay <= PhotonNetwork.Time)
				{
					return true;
				}
			}
			else if (this.lastTriggerTime + this.generalRetriggerDelay <= (double)Time.time)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060059D8 RID: 23000 RVA: 0x001CBF10 File Offset: 0x001CA110
		public virtual void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			if (!ignoreTriggerCount)
			{
				if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
				{
					return;
				}
				this.numTimesTriggered += 1;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (triggerTime < 0.0)
				{
					triggerTime = PhotonNetwork.Time;
				}
			}
			else if (originatedLocally)
			{
				triggerTime = (double)Time.time;
			}
			this.lastTriggerTime = triggerTime;
			if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
			{
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
			}
		}

		// Token: 0x060059D9 RID: 23001 RVA: 0x001CBFB4 File Offset: 0x001CA1B4
		public void ResetTrigger(bool onlyResetTriggerCount = false)
		{
			if (!onlyResetTriggerCount)
			{
				this.lastTriggerTime = -1.0;
			}
			this.numTimesTriggered = 0;
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			CMSSerializer.ResetTrigger(this.id);
		}

		// Token: 0x060059DA RID: 23002 RVA: 0x001CC008 File Offset: 0x001CA208
		public void SetTriggerCount(byte value)
		{
			this.numTimesTriggered = Math.Min(value, this.numAllowedTriggers);
			if (this.numTimesTriggered >= this.numAllowedTriggers)
			{
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
			}
		}

		// Token: 0x060059DB RID: 23003 RVA: 0x001CC058 File Offset: 0x001CA258
		public void SetLastTriggerTime(double value)
		{
			this.lastTriggerTime = value;
		}

		// Token: 0x040066E1 RID: 26337
		public const byte INVALID_TRIGGER_ID = 255;

		// Token: 0x040066E2 RID: 26338
		public const double MAX_PHOTON_SERVER_TIME = 4294967.295;

		// Token: 0x040066E3 RID: 26339
		public const float MINIMUM_VALIDATION_DISTANCE = 2f;

		// Token: 0x040066E4 RID: 26340
		public bool syncedToAllPlayers;

		// Token: 0x040066E5 RID: 26341
		public float validationDistanceSquared;

		// Token: 0x040066E6 RID: 26342
		public TriggerSource triggeredBy = 4;

		// Token: 0x040066E7 RID: 26343
		public double onEnableTriggerDelay;

		// Token: 0x040066E8 RID: 26344
		public double generalRetriggerDelay;

		// Token: 0x040066E9 RID: 26345
		public bool retriggerAfterDuration;

		// Token: 0x040066EA RID: 26346
		public double retriggerStayDuration = 2.0;

		// Token: 0x040066EB RID: 26347
		public byte numAllowedTriggers;

		// Token: 0x040066EC RID: 26348
		private byte numTimesTriggered;

		// Token: 0x040066ED RID: 26349
		private double lastTriggerTime = -1.0;

		// Token: 0x040066EE RID: 26350
		private double enabledTime = -1.0;

		// Token: 0x040066EF RID: 26351
		public byte id = byte.MaxValue;
	}
}
