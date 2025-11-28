using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02000F6D RID: 3949
	public class JoustPlayer : MonoBehaviour
	{
		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x060062BB RID: 25275 RVA: 0x001FD93D File Offset: 0x001FBB3D
		// (set) Token: 0x060062BC RID: 25276 RVA: 0x001FD945 File Offset: 0x001FBB45
		public float HorizontalSpeed
		{
			get
			{
				return this.HSpeed;
			}
			set
			{
				this.HSpeed = value;
			}
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x001FD950 File Offset: 0x001FBB50
		private void LateUpdate()
		{
			this.velocity.x = this.HSpeed * 0.001f;
			if (this.flap)
			{
				this.velocity.y = Mathf.Min(this.velocity.y + 0.0005f, 0.0005f);
				this.flap = false;
			}
			else
			{
				this.velocity.y = Mathf.Max(this.velocity.y - Time.deltaTime * 0.0001f, -0.001f);
				int i = 0;
				while (i < Physics2D.RaycastNonAlloc(base.transform.position, this.velocity.normalized, this.raycastHitResults, this.velocity.magnitude))
				{
					JoustTerrain joustTerrain;
					if (this.raycastHitResults[i].collider.TryGetComponent<JoustTerrain>(ref joustTerrain))
					{
						this.velocity.y = 0f;
						if (joustTerrain.transform.localPosition.y < base.transform.localPosition.y)
						{
							base.transform.localPosition = new Vector2(base.transform.localPosition.x, joustTerrain.transform.localPosition.y + this.raycastHitResults[i].collider.bounds.size.y);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.transform.Translate(this.velocity);
			if ((double)Mathf.Abs(base.transform.localPosition.x) > 4.5)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x * -0.95f, base.transform.localPosition.y);
			}
		}

		// Token: 0x060062BE RID: 25278 RVA: 0x001FDB36 File Offset: 0x001FBD36
		public void Flap()
		{
			this.flap = true;
		}

		// Token: 0x0400717F RID: 29055
		private Vector2 velocity;

		// Token: 0x04007180 RID: 29056
		private RaycastHit2D[] raycastHitResults = new RaycastHit2D[8];

		// Token: 0x04007181 RID: 29057
		private float HSpeed;

		// Token: 0x04007182 RID: 29058
		private bool flap;
	}
}
