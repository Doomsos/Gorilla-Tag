using System;
using Drawing;
using UnityEngine;

// Token: 0x020009C6 RID: 2502
public class ComputePenetration : MonoBehaviour
{
	// Token: 0x06004000 RID: 16384 RVA: 0x00157F0B File Offset: 0x0015610B
	public void Compute()
	{
		if (this.colliderA == null)
		{
			return;
		}
		this.colliderB == null;
	}

	// Token: 0x06004001 RID: 16385 RVA: 0x00157F2C File Offset: 0x0015612C
	public void OnDrawGizmos()
	{
		if (this.colliderA.AsNull<Collider>() == null)
		{
			return;
		}
		if (this.colliderB.AsNull<Collider>() == null)
		{
			return;
		}
		Transform transform = this.colliderA.transform;
		Transform transform2 = this.colliderB.transform;
		if (this.lastUpdate.HasElapsed(0.5f, true))
		{
			this.overlapped = Physics.ComputePenetration(this.colliderA, transform.position, transform.rotation, this.colliderB, transform2.position, transform2.rotation, ref this.direction, ref this.distance);
		}
		Color color = this.overlapped ? Color.red : Color.green;
		this.DrawCollider(this.colliderA, color);
		this.DrawCollider(this.colliderB, color);
		if (this.overlapped)
		{
			Vector3 position = this.colliderB.transform.position;
			Vector3 vector = position + this.direction * this.distance;
			Gizmos.DrawLine(position, vector);
		}
	}

	// Token: 0x06004002 RID: 16386 RVA: 0x0015802C File Offset: 0x0015622C
	private unsafe void DrawCollider(Collider c, Color color)
	{
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(c.transform.localToWorldMatrix))
		{
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = c as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = c as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = c as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.PopColor();
		}
	}

	// Token: 0x04005130 RID: 20784
	public Collider colliderA;

	// Token: 0x04005131 RID: 20785
	public Collider colliderB;

	// Token: 0x04005132 RID: 20786
	public bool overlapped;

	// Token: 0x04005133 RID: 20787
	public Vector3 direction;

	// Token: 0x04005134 RID: 20788
	public float distance;

	// Token: 0x04005135 RID: 20789
	private TimeSince lastUpdate = TimeSince.Now();
}
