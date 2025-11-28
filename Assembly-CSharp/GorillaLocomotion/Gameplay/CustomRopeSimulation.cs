using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F96 RID: 3990
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x0600642D RID: 25645 RVA: 0x0020AAD8 File Offset: 0x00208CD8
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nodeCount; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = position;
				this.nodes.Add(gameObject.transform);
				position.y -= this.nodeDistance;
			}
			this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
			this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, 4, 1);
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x0020AB88 File Offset: 0x00208D88
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x0020AB98 File Offset: 0x00208D98
		private void Update()
		{
			IJobExtensions.Run<SolveRopeJob>(new SolveRopeJob
			{
				fixedDeltaTime = Time.deltaTime,
				gravity = this.gravity,
				nodes = this.burstNodes,
				nodeDistance = this.nodeDistance,
				rootPos = base.transform.position
			});
			for (int i = 0; i < this.burstNodes.Length; i++)
			{
				this.nodes[i].position = this.burstNodes[i].curPos;
				if (i > 0)
				{
					Vector3 vector = this.burstNodes[i - 1].curPos - this.burstNodes[i].curPos;
					this.nodes[i].up = -vector;
				}
			}
		}

		// Token: 0x040073D3 RID: 29651
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x040073D4 RID: 29652
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x040073D5 RID: 29653
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x040073D6 RID: 29654
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x040073D7 RID: 29655
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x040073D8 RID: 29656
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
