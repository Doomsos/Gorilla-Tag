using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F98 RID: 3992
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		// Token: 0x06006431 RID: 25649 RVA: 0x0020ACD8 File Offset: 0x00208ED8
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

		// Token: 0x06006432 RID: 25650 RVA: 0x0020AD00 File Offset: 0x00208F00
		private void Simulate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				Vector3 vector = burstRopeNode.curPos - burstRopeNode.lastPos;
				burstRopeNode.lastPos = burstRopeNode.curPos;
				Vector3 vector2 = burstRopeNode.curPos + vector;
				vector2 += this.gravity * this.fixedDeltaTime;
				burstRopeNode.curPos = vector2;
				this.nodes[i] = burstRopeNode;
			}
		}

		// Token: 0x06006433 RID: 25651 RVA: 0x0020AD8C File Offset: 0x00208F8C
		private void ApplyConstraint()
		{
			BurstRopeNode burstRopeNode = this.nodes[0];
			burstRopeNode.curPos = this.rootPos;
			this.nodes[0] = burstRopeNode;
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				BurstRopeNode burstRopeNode2 = this.nodes[i];
				BurstRopeNode burstRopeNode3 = this.nodes[i + 1];
				float magnitude = (burstRopeNode2.curPos - burstRopeNode3.curPos).magnitude;
				float num = Mathf.Abs(magnitude - this.nodeDistance);
				Vector3 vector = Vector3.zero;
				if (magnitude > this.nodeDistance)
				{
					vector = (burstRopeNode2.curPos - burstRopeNode3.curPos).normalized;
				}
				else if (magnitude < this.nodeDistance)
				{
					vector = (burstRopeNode3.curPos - burstRopeNode2.curPos).normalized;
				}
				Vector3 vector2 = vector * num;
				burstRopeNode2.curPos -= vector2 * 0.5f;
				burstRopeNode3.curPos += vector2 * 0.5f;
				this.nodes[i] = burstRopeNode2;
				this.nodes[i + 1] = burstRopeNode3;
			}
		}

		// Token: 0x040073DB RID: 29659
		[ReadOnly]
		public float fixedDeltaTime;

		// Token: 0x040073DC RID: 29660
		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		// Token: 0x040073DD RID: 29661
		[ReadOnly]
		public Vector3 gravity;

		// Token: 0x040073DE RID: 29662
		[ReadOnly]
		public Vector3 rootPos;

		// Token: 0x040073DF RID: 29663
		[ReadOnly]
		public float nodeDistance;
	}
}
