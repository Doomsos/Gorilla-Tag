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
		// (get) Token: 0x06005C28 RID: 23592 RVA: 0x001D95FC File Offset: 0x001D77FC
		public int overlapCount
		{
			get
			{
				return this.overlappingColliders.Count;
			}
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x06005C29 RID: 23593 RVA: 0x001D9609 File Offset: 0x001D7809
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x1400009B RID: 155
		// (add) Token: 0x06005C2A RID: 23594 RVA: 0x001D9618 File Offset: 0x001D7818
		// (remove) Token: 0x06005C2B RID: 23595 RVA: 0x001D9650 File Offset: 0x001D7850
		public event Action<int> onPlayerEnteredTrigger;

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x06005C2C RID: 23596 RVA: 0x001D9688 File Offset: 0x001D7888
		// (remove) Token: 0x06005C2D RID: 23597 RVA: 0x001D96C0 File Offset: 0x001D78C0
		public event Action onTriggerFirstEntered;

		// Token: 0x1400009D RID: 157
		// (add) Token: 0x06005C2E RID: 23598 RVA: 0x001D96F8 File Offset: 0x001D78F8
		// (remove) Token: 0x06005C2F RID: 23599 RVA: 0x001D9730 File Offset: 0x001D7930
		public event Action onTriggerLastExited;

		// Token: 0x06005C30 RID: 23600 RVA: 0x001D9768 File Offset: 0x001D7968
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

		// Token: 0x06005C31 RID: 23601 RVA: 0x001D988C File Offset: 0x001D7A8C
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

		// Token: 0x06005C32 RID: 23602 RVA: 0x001D99AB File Offset: 0x001D7BAB
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
