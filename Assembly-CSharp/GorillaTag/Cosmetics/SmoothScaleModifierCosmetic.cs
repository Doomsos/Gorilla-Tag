using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001115 RID: 4373
	public class SmoothScaleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06006D7E RID: 28030 RVA: 0x0023F5B9 File Offset: 0x0023D7B9
		private void Awake()
		{
			this.initialScale = this.objectPrefab.transform.localScale;
		}

		// Token: 0x06006D7F RID: 28031 RVA: 0x0023F5D1 File Offset: 0x0023D7D1
		private void OnEnable()
		{
			this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
		}

		// Token: 0x06006D80 RID: 28032 RVA: 0x0023F5DC File Offset: 0x0023D7DC
		private void Update()
		{
			switch (this.currentState)
			{
			case SmoothScaleModifierCosmetic.State.None:
			case SmoothScaleModifierCosmetic.State.Scaled:
				break;
			case SmoothScaleModifierCosmetic.State.Reset:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.initialScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.initialScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.initialScale;
					if (this.onReset != null)
					{
						this.onReset.Invoke();
					}
					this.UpdateState(SmoothScaleModifierCosmetic.State.None);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaling:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.targetScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.targetScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.targetScale;
					if (this.onScaled != null)
					{
						this.onScaled.Invoke();
					}
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaled);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006D81 RID: 28033 RVA: 0x0023F6E7 File Offset: 0x0023D8E7
		private void SmoothScale(Vector3 initial, Vector3 target)
		{
			this.objectPrefab.transform.localScale = Vector3.MoveTowards(initial, target, this.speed * Time.deltaTime);
		}

		// Token: 0x06006D82 RID: 28034 RVA: 0x0023F70C File Offset: 0x0023D90C
		private void UpdateState(SmoothScaleModifierCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x06006D83 RID: 28035 RVA: 0x0023F715 File Offset: 0x0023D915
		public void TriggerScale()
		{
			if (this.currentState != SmoothScaleModifierCosmetic.State.Scaled)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Scaling);
			}
		}

		// Token: 0x06006D84 RID: 28036 RVA: 0x0023F727 File Offset: 0x0023D927
		public void TriggerReset()
		{
			if (this.currentState != SmoothScaleModifierCosmetic.State.Reset)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
			}
		}

		// Token: 0x04007EE0 RID: 32480
		[Tooltip("The GameObject to scale up or down. This should reference the cosmetic mesh or object you want to visually modify.")]
		[SerializeField]
		private GameObject objectPrefab;

		// Token: 0x04007EE1 RID: 32481
		[Tooltip("The target scale applied when scaling is triggered.")]
		[SerializeField]
		private Vector3 targetScale = new Vector3(2f, 2f, 2f);

		// Token: 0x04007EE2 RID: 32482
		[Tooltip("Speed at which the object scales toward its target or initial size")]
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04007EE3 RID: 32483
		[Tooltip("Invoked once when the object reaches the target scale.")]
		public UnityEvent onScaled;

		// Token: 0x04007EE4 RID: 32484
		[Tooltip("Invoked once when the object returns to its initial scale.")]
		public UnityEvent onReset;

		// Token: 0x04007EE5 RID: 32485
		private SmoothScaleModifierCosmetic.State currentState;

		// Token: 0x04007EE6 RID: 32486
		private Vector3 initialScale;

		// Token: 0x02001116 RID: 4374
		private enum State
		{
			// Token: 0x04007EE8 RID: 32488
			None,
			// Token: 0x04007EE9 RID: 32489
			Reset,
			// Token: 0x04007EEA RID: 32490
			Scaling,
			// Token: 0x04007EEB RID: 32491
			Scaled
		}
	}
}
