using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChan
{
	// Token: 0x02001179 RID: 4473
	public class IdleChanger : MonoBehaviour
	{
		// Token: 0x060070E7 RID: 28903 RVA: 0x0024F11D File Offset: 0x0024D31D
		private void Start()
		{
			this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
			this.previousState = this.currentState;
			base.StartCoroutine("RandomChange");
			this.kb = Keyboard.current;
		}

		// Token: 0x060070E8 RID: 28904 RVA: 0x0024F154 File Offset: 0x0024D354
		private void Update()
		{
			if (this.kb.upArrowKey.wasPressedThisFrame || this.kb.spaceKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Next", true);
				this.UnityChanB.SetBool("Next", true);
			}
			if (this.kb.downArrowKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Back", true);
				this.UnityChanB.SetBool("Back", true);
			}
			if (this.UnityChanA.GetBool("Next"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Next", false);
					this.UnityChanB.SetBool("Next", false);
					this.previousState = this.currentState;
				}
			}
			if (this.UnityChanA.GetBool("Back"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Back", false);
					this.UnityChanB.SetBool("Back", false);
					this.previousState = this.currentState;
				}
			}
		}

		// Token: 0x060070E9 RID: 28905 RVA: 0x0024F2B0 File Offset: 0x0024D4B0
		private void OnGUI()
		{
			if (this.isGUI)
			{
				GUI.Box(new Rect((float)(Screen.width - 110), 10f, 100f, 90f), "Change Motion");
				if (GUI.Button(new Rect((float)(Screen.width - 100), 40f, 80f, 20f), "Next"))
				{
					this.UnityChanA.SetBool("Next", true);
					this.UnityChanB.SetBool("Next", true);
				}
				if (GUI.Button(new Rect((float)(Screen.width - 100), 70f, 80f, 20f), "Back"))
				{
					this.UnityChanA.SetBool("Back", true);
					this.UnityChanB.SetBool("Back", true);
				}
			}
		}

		// Token: 0x060070EA RID: 28906 RVA: 0x0024F385 File Offset: 0x0024D585
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				if (this._random)
				{
					float num = Random.Range(0f, 1f);
					if (num < this._threshold)
					{
						this.UnityChanA.SetBool("Back", true);
						this.UnityChanB.SetBool("Back", true);
					}
					else if (num >= this._threshold)
					{
						this.UnityChanA.SetBool("Next", true);
						this.UnityChanB.SetBool("Next", true);
					}
				}
				yield return new WaitForSeconds(this._interval);
			}
			yield break;
		}

		// Token: 0x040080FB RID: 33019
		private AnimatorStateInfo currentState;

		// Token: 0x040080FC RID: 33020
		private AnimatorStateInfo previousState;

		// Token: 0x040080FD RID: 33021
		public bool _random;

		// Token: 0x040080FE RID: 33022
		public float _threshold = 0.5f;

		// Token: 0x040080FF RID: 33023
		public float _interval = 10f;

		// Token: 0x04008100 RID: 33024
		public bool isGUI = true;

		// Token: 0x04008101 RID: 33025
		public Animator UnityChanA;

		// Token: 0x04008102 RID: 33026
		public Animator UnityChanB;

		// Token: 0x04008103 RID: 33027
		private Keyboard kb;
	}
}
