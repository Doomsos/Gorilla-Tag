using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000E93 RID: 3731
	public class AIEntity : MonoBehaviour
	{
		// Token: 0x06005D57 RID: 23895 RVA: 0x001DFA68 File Offset: 0x001DDC68
		protected void Awake()
		{
			this.navMeshAgent = base.gameObject.GetComponent<NavMeshAgent>();
			this.animator = base.gameObject.GetComponent<Animator>();
			if (this.waypointsContainer != null)
			{
				foreach (Transform transform in this.waypointsContainer.GetComponentsInChildren<Transform>())
				{
					this.waypoints.Add(transform);
				}
			}
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x001DFAD0 File Offset: 0x001DDCD0
		protected void ChooseRandomTarget()
		{
			int randomTarget = Random.Range(0, GorillaParent.instance.vrrigs.Count);
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == GorillaParent.instance.vrrigs[randomTarget].creator);
			if (num == -1)
			{
				num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
			}
			if (num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, ref navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x001DFBC0 File Offset: 0x001DDDC0
		protected void ChooseClosestTarget()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				if (vrrig2.head != null && !(vrrig2.head.rigTarget == null))
				{
					float sqrMagnitude = (base.transform.position - vrrig2.head.rigTarget.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.minChaseRange * this.minChaseRange && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						vrrig = vrrig2;
					}
				}
			}
			if (vrrig != null)
			{
				this.targetPlayer = vrrig.creator;
				this.followTarget = vrrig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, ref navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x04006B32 RID: 27442
		public GameObject waypointsContainer;

		// Token: 0x04006B33 RID: 27443
		public Transform circleCenter;

		// Token: 0x04006B34 RID: 27444
		public float circleRadius;

		// Token: 0x04006B35 RID: 27445
		public float angularSpeed;

		// Token: 0x04006B36 RID: 27446
		public float patrolSpeed;

		// Token: 0x04006B37 RID: 27447
		public float fleeSpeed;

		// Token: 0x04006B38 RID: 27448
		public NavMeshAgent navMeshAgent;

		// Token: 0x04006B39 RID: 27449
		public Animator animator;

		// Token: 0x04006B3A RID: 27450
		public float fleeRang;

		// Token: 0x04006B3B RID: 27451
		public float fleeSpeedMult;

		// Token: 0x04006B3C RID: 27452
		public float minChaseRange;

		// Token: 0x04006B3D RID: 27453
		public float attackDistance;

		// Token: 0x04006B3E RID: 27454
		public float navMeshSampleRange = 5f;

		// Token: 0x04006B3F RID: 27455
		internal readonly List<Transform> waypoints = new List<Transform>();

		// Token: 0x04006B40 RID: 27456
		internal float defaultSpeed;

		// Token: 0x04006B41 RID: 27457
		public Transform followTarget;

		// Token: 0x04006B42 RID: 27458
		public NetPlayer targetPlayer;

		// Token: 0x04006B43 RID: 27459
		public bool targetIsOnNavMesh;
	}
}
