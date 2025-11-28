using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E66 RID: 3686
	public class BuilderSmallMonkeTrigger : MonoBehaviour
	{
		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x06005C28 RID: 23592 RVA: 0x001D961C File Offset: 0x001D781C
		public int overlapCount
		{
			get
			{
				return this.overlappingColliders.Count;
			}
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x06005C29 RID: 23593 RVA: 0x001D9629 File Offset: 0x001D7829
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x1400009B RID: 155
		// (add) Token: 0x06005C2A RID: 23594 RVA: 0x001D9638 File Offset: 0x001D7838
		// (remove) Token: 0x06005C2B RID: 23595 RVA: 0x001D9670 File Offset: 0x001D7870
		public event Action<int> onPlayerEnteredTrigger;

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x06005C2C RID: 23596 RVA: 0x001D96A8 File Offset: 0x001D78A8
		// (remove) Token: 0x06005C2D RID: 23597 RVA: 0x001D96E0 File Offset: 0x001D78E0
		public event Action onTriggerFirstEntered;

		// Token: 0x1400009D RID: 157
		// (add) Token: 0x06005C2E RID: 23598 RVA: 0x001D9718 File Offset: 0x001D7918
		// (remove) Token: 0x06005C2F RID: 23599 RVA: 0x001D9750 File Offset: 0x001D7950
		public event Action onTriggerLastExited;

		// Token: 0x06005C30 RID: 23600 RVA: 0x001D9788 File Offset: 0x001D7988
		public void ValidateOverlappingColliders()
		{
			for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
			{
				if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
				{
					this.overlappingColliders.RemoveAt(i);
				}
				else
				{
					VRRig vrrig = this.overlappingColliders[i].attachedRigidbody.gameObject.GetComponent<VRRig>();
					if (vrrig == null)
					{
						if (GTPlayer.Instance.bodyCollider == this.overlappingColliders[i] || GTPlayer.Instance.headCollider == this.overlappingColliders[i])
						{
							vrrig = GorillaTagger.Instance.offlineVRRig;
						}
						else
						{
							this.overlappingColliders.RemoveAt(i);
						}
					}
					if (!this.ignoreScale && vrrig != null && (double)vrrig.scaleFactor > 0.99)
					{
						this.overlappingColliders.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x06005C31 RID: 23601 RVA: 0x001D98AC File Offset: 0x001D7AAC
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other) && !(GTPlayer.Instance.headCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(vrrig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (!this.ignoreScale && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			if (vrrig != null)
			{
				Action<int> action = this.onPlayerEnteredTrigger;
				if (action != null)
				{
					action.Invoke(vrrig.OwningNetPlayer.ActorNumber);
				}
			}
			bool flag = this.overlappingColliders.Count == 0;
			if (!this.overlappingColliders.Contains(other))
			{
				this.overlappingColliders.Add(other);
			}
			this.lastTriggeredFrame = Time.frameCount;
			if (flag)
			{
				Action action2 = this.onTriggerFirstEntered;
				if (action2 == null)
				{
					return;
				}
				action2.Invoke();
			}
		}

		// Token: 0x06005C32 RID: 23602 RVA: 0x001D99CB File Offset: 0x001D7BCB
		private void OnTriggerExit(Collider other)
		{
			if (this.overlappingColliders.Remove(other) && this.overlappingColliders.Count == 0)
			{
				Action action = this.onTriggerLastExited;
				if (action == null)
				{
					return;
				}
				action.Invoke();
			}
		}

		// Token: 0x04006999 RID: 27033
		private int lastTriggeredFrame = -1;

		// Token: 0x0400699A RID: 27034
		private List<Collider> overlappingColliders = new List<Collider>(20);

		// Token: 0x0400699E RID: 27038
		private bool hasCheckedZone;

		// Token: 0x0400699F RID: 27039
		private bool ignoreScale;
	}
}
