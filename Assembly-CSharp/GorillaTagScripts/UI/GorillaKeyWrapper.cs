using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI
{
	// Token: 0x02000E2C RID: 3628
	public class GorillaKeyWrapper<TBinding> : MonoBehaviour where TBinding : Enum
	{
		// Token: 0x06005A89 RID: 23177 RVA: 0x001D0628 File Offset: 0x001CE828
		public void Start()
		{
			if (!this.defineButtonsManually)
			{
				this.FindMatchingButtons(base.gameObject);
				return;
			}
			if (this.buttons.Count > 0)
			{
				for (int i = this.buttons.Count - 1; i >= 0; i--)
				{
					if (this.buttons[i].IsNull())
					{
						this.buttons.RemoveAt(i);
					}
					else
					{
						this.buttons[i].OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
					}
				}
			}
		}

		// Token: 0x06005A8A RID: 23178 RVA: 0x001D06B4 File Offset: 0x001CE8B4
		public void OnDestroy()
		{
			for (int i = 0; i < this.buttons.Count; i++)
			{
				if (this.buttons[i].IsNotNull())
				{
					this.buttons[i].OnKeyButtonPressed.RemoveListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
				}
			}
		}

		// Token: 0x06005A8B RID: 23179 RVA: 0x001D070C File Offset: 0x001CE90C
		public void FindMatchingButtons(GameObject obj)
		{
			if (obj.IsNull())
			{
				return;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				Transform child = obj.transform.GetChild(i);
				if (child.IsNotNull())
				{
					this.FindMatchingButtons(child.gameObject);
				}
			}
			GorillaKeyButton<TBinding> component = obj.GetComponent<GorillaKeyButton<TBinding>>();
			if (component.IsNotNull() && !this.buttons.Contains(component))
			{
				this.buttons.Add(component);
				component.OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
			}
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x001D0799 File Offset: 0x001CE999
		private void OnKeyButtonPressed(TBinding binding)
		{
			UnityEvent<TBinding> onKeyPressed = this.OnKeyPressed;
			if (onKeyPressed == null)
			{
				return;
			}
			onKeyPressed.Invoke(binding);
		}

		// Token: 0x040067C2 RID: 26562
		public UnityEvent<TBinding> OnKeyPressed = new UnityEvent<TBinding>();

		// Token: 0x040067C3 RID: 26563
		public bool defineButtonsManually;

		// Token: 0x040067C4 RID: 26564
		public List<GorillaKeyButton<TBinding>> buttons = new List<GorillaKeyButton<TBinding>>();
	}
}
