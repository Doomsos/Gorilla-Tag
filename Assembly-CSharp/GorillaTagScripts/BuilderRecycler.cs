using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB9 RID: 3513
	public class BuilderRecycler : MonoBehaviour
	{
		// Token: 0x06005681 RID: 22145 RVA: 0x001B34B8 File Offset: 0x001B16B8
		private void Awake()
		{
			this.hasFans = (this.effectBehaviors.Count > 0 && this.bladeSoundPlayer != null && this.recycleParticles != null);
			this.hasPipes = (this.outputPipes.Count > 0);
		}

		// Token: 0x06005682 RID: 22146 RVA: 0x001B350C File Offset: 0x001B170C
		private void Start()
		{
			if (this.hasPipes)
			{
				this.numPipes = Mathf.Min(this.outputPipes.Count, 3);
				this.props = new MaterialPropertyBlock();
				this.ResetOutputPipes();
				this.totalRecycledCost = new int[3];
				this.currentChainCost = new int[3];
				for (int i = 0; i < this.totalRecycledCost.Length; i++)
				{
					this.totalRecycledCost[i] = 0;
					this.currentChainCost[i] = 0;
				}
			}
			this.zoneRenderers.Clear();
			if (this.hasPipes)
			{
				this.zoneRenderers.AddRange(this.outputPipes);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					Renderer component = monoBehaviour.GetComponent<Renderer>();
					if (component != null)
					{
						this.zoneRenderers.Add(component);
					}
				}
			}
			this.inBuilderZone = true;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06005683 RID: 22147 RVA: 0x001B3640 File Offset: 0x001B1840
		private void OnDestroy()
		{
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06005684 RID: 22148 RVA: 0x001B3678 File Offset: 0x001B1878
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
			if (flag && !this.inBuilderZone)
			{
				using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Renderer renderer = enumerator.Current;
						renderer.enabled = true;
					}
					goto IL_8B;
				}
			}
			if (!flag && this.inBuilderZone)
			{
				foreach (Renderer renderer2 in this.zoneRenderers)
				{
					renderer2.enabled = false;
				}
			}
			IL_8B:
			this.inBuilderZone = flag;
		}

		// Token: 0x06005685 RID: 22149 RVA: 0x001B3734 File Offset: 0x001B1934
		private void OnTriggerEnter(Collider other)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(other);
			if (builderPieceFromCollider == null)
			{
				return;
			}
			if (!builderPieceFromCollider.isBuiltIntoTable && !builderPieceFromCollider.isArmShelf)
			{
				this.table.RequestRecyclePiece(builderPieceFromCollider, true, this.recyclerID);
			}
		}

		// Token: 0x06005686 RID: 22150 RVA: 0x001B3778 File Offset: 0x001B1978
		public void OnRecycleRequestedAtRecycler(BuilderPiece piece)
		{
			if (this.hasPipes)
			{
				this.AddPieceCost(piece.cost);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					monoBehaviour.enabled = true;
				}
				this.recycleParticles.SetActive(true);
				this.bladeSoundPlayer.Play();
				this.timeToStopBlades = (double)(Time.time + this.recycleEffectDuration);
				this.playingBladeEffect = true;
			}
		}

		// Token: 0x06005687 RID: 22151 RVA: 0x001B3818 File Offset: 0x001B1A18
		private void AddPieceCost(BuilderResources cost)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					this.totalRecycledCost[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			if (!this.playingPipeEffect)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x06005688 RID: 22152 RVA: 0x001B38A0 File Offset: 0x001B1AA0
		private Vector2 GetUVShiftOffset()
		{
			float y = Shader.GetGlobalVector(ShaderProps._Time).y;
			Vector4 vector;
			vector..ctor(500f, 0f, 0f, 0f);
			Vector4 vector2 = vector / this.recycleEffectDuration;
			return new Vector2(-1f * (Mathf.Floor(y * vector2.x) * 1f / vector.x % 1f) * vector.x - vector.x + 165f, 0f);
		}

		// Token: 0x06005689 RID: 22153 RVA: 0x001B392C File Offset: 0x001B1B2C
		private void UpdatePipeLoop()
		{
			bool flag = false;
			for (int i = 0; i < this.numPipes; i++)
			{
				if (this.totalRecycledCost[i] > 0)
				{
					flag = true;
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					Vector4 vector = new Vector4(500f, 0f, 0f, 0f) / this.recycleEffectDuration;
					Vector2 uvshiftOffset = this.GetUVShiftOffset();
					this.props.SetColor(ShaderProps._BaseColor, this.builderResourceColors.colors[i].color);
					this.props.SetVector(ShaderProps._UvShiftRate, vector);
					this.props.SetVector(ShaderProps._UvShiftOffset, uvshiftOffset);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
					this.totalRecycledCost[i] = Mathf.Max(this.totalRecycledCost[i] - 1, 0);
				}
				else
				{
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					this.props.SetColor(ShaderProps._BaseColor, Color.black);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
				}
			}
			if (flag)
			{
				this.playingPipeEffect = true;
				this.timeToCheckPipes = (double)(Time.time + this.recycleEffectDuration);
				return;
			}
			this.playingPipeEffect = false;
		}

		// Token: 0x0600568A RID: 22154 RVA: 0x001B3A90 File Offset: 0x001B1C90
		private void ResetOutputPipes()
		{
			foreach (MeshRenderer meshRenderer in this.outputPipes)
			{
				meshRenderer.GetPropertyBlock(this.props, 1);
				this.props.SetColor(ShaderProps._BaseColor, Color.black);
				meshRenderer.SetPropertyBlock(this.props, 1);
			}
		}

		// Token: 0x0600568B RID: 22155 RVA: 0x001B3B0C File Offset: 0x001B1D0C
		public void UpdateRecycler()
		{
			if (this.playingBladeEffect && (double)Time.time > this.timeToStopBlades)
			{
				if (this.hasFans)
				{
					foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
					{
						monoBehaviour.enabled = false;
					}
					this.recycleParticles.SetActive(false);
				}
				this.playingBladeEffect = false;
			}
			if (this.playingPipeEffect && (double)Time.time > this.timeToCheckPipes)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x040063A2 RID: 25506
		public float recycleEffectDuration = 0.25f;

		// Token: 0x040063A3 RID: 25507
		private double timeToStopBlades = double.MinValue;

		// Token: 0x040063A4 RID: 25508
		private bool playingBladeEffect;

		// Token: 0x040063A5 RID: 25509
		private bool playingPipeEffect;

		// Token: 0x040063A6 RID: 25510
		private double timeToCheckPipes = double.MinValue;

		// Token: 0x040063A7 RID: 25511
		public List<MonoBehaviour> effectBehaviors;

		// Token: 0x040063A8 RID: 25512
		public GameObject recycleParticles;

		// Token: 0x040063A9 RID: 25513
		public SoundBankPlayer bladeSoundPlayer;

		// Token: 0x040063AA RID: 25514
		public List<MeshRenderer> outputPipes;

		// Token: 0x040063AB RID: 25515
		public BuilderResourceColors builderResourceColors;

		// Token: 0x040063AC RID: 25516
		private bool hasFans;

		// Token: 0x040063AD RID: 25517
		private bool hasPipes;

		// Token: 0x040063AE RID: 25518
		private MaterialPropertyBlock props;

		// Token: 0x040063AF RID: 25519
		private int[] totalRecycledCost;

		// Token: 0x040063B0 RID: 25520
		private int[] currentChainCost;

		// Token: 0x040063B1 RID: 25521
		private int numPipes;

		// Token: 0x040063B2 RID: 25522
		internal int recyclerID = -1;

		// Token: 0x040063B3 RID: 25523
		internal BuilderTable table;

		// Token: 0x040063B4 RID: 25524
		private List<Renderer> zoneRenderers = new List<Renderer>(10);

		// Token: 0x040063B5 RID: 25525
		private bool inBuilderZone;
	}
}
