using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E0E RID: 3598
	public class CMSTeleporter : CMSTrigger
	{
		// Token: 0x060059CD RID: 22989 RVA: 0x001CB9AC File Offset: 0x001C9BAC
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(TeleporterSettings))
			{
				TeleporterSettings teleporterSettings = (TeleporterSettings)settings;
				this.TeleportPoints = teleporterSettings.TeleportPoints;
				this.matchTeleportPointRotation = teleporterSettings.matchTeleportPointRotation;
				this.maintainVelocity = teleporterSettings.maintainVelocity;
			}
			for (int i = this.TeleportPoints.Count - 1; i >= 0; i--)
			{
				if (this.TeleportPoints[i] == null)
				{
					this.TeleportPoints.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x060059CE RID: 22990 RVA: 0x001CBA3C File Offset: 0x001C9C3C
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally && GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				if (this.TeleportPoints.Count != 0)
				{
					Transform transform = this.TeleportPoints[Random.Range(0, this.TeleportPoints.Count)];
					if (transform != null)
					{
						instance.TeleportTo(transform, this.matchTeleportPointRotation, this.maintainVelocity);
					}
				}
			}
		}

		// Token: 0x040066DE RID: 26334
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		[NotNull]
		public List<Transform> TeleportPoints = new List<Transform>();

		// Token: 0x040066DF RID: 26335
		public bool matchTeleportPointRotation;

		// Token: 0x040066E0 RID: 26336
		public bool maintainVelocity;
	}
}
