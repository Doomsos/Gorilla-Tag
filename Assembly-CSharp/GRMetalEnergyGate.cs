using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class GRMetalEnergyGate : MonoBehaviour
{
	// Token: 0x06002CC1 RID: 11457 RVA: 0x000F28FF File Offset: 0x000F0AFF
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x000F2930 File Offset: 0x000F0B30
	private void OnDisable()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange -= this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x000F2988 File Offset: 0x000F0B88
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(chargingEntityId);
		GRPlayer grplayer = null;
		if (gameEntity != null)
		{
			grplayer = GRPlayer.Get(gameEntity.heldByActorNumber);
		}
		if (grplayer != null)
		{
			grplayer.IncrementCoresSpentPlayer(energyChange);
		}
		if (this.state == GRMetalEnergyGate.State.Closed && tool.energy >= tool.GetEnergyMax())
		{
			if (grplayer != null)
			{
				grplayer.IncrementGatesUnlocked(1);
			}
			this.SetState(GRMetalEnergyGate.State.Open);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x000F2A20 File Offset: 0x000F0C20
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRMetalEnergyGate.State)nextState);
		}
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x000F2A38 File Offset: 0x000F0C38
	public void SetState(GRMetalEnergyGate.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.audioSource.PlayOneShot(this.doorOpenClip);
					for (int i = 0; i < this.enableObjectsOnOpen.Count; i++)
					{
						this.enableObjectsOnOpen[i].gameObject.SetActive(true);
					}
					for (int j = 0; j < this.disableObjectsOnOpen.Count; j++)
					{
						this.disableObjectsOnOpen[j].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.audioSource.PlayOneShot(this.doorCloseClip);
				for (int k = 0; k < this.enableObjectsOnOpen.Count; k++)
				{
					this.enableObjectsOnOpen[k].gameObject.SetActive(false);
				}
				for (int l = 0; l < this.disableObjectsOnOpen.Count; l++)
				{
					this.disableObjectsOnOpen[l].gameObject.SetActive(true);
				}
			}
			if (this.doorAnimationCoroutine == null)
			{
				this.doorAnimationCoroutine = base.StartCoroutine(this.UpdateDoorAnimation());
			}
		}
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x000F2B60 File Offset: 0x000F0D60
	public void OpenGate()
	{
		this.SetState(GRMetalEnergyGate.State.Open);
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x000F2B69 File Offset: 0x000F0D69
	public void CloseGate()
	{
		this.SetState(GRMetalEnergyGate.State.Closed);
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x000F2B72 File Offset: 0x000F0D72
	private IEnumerator UpdateDoorAnimation()
	{
		while ((this.state == GRMetalEnergyGate.State.Open && this.openProgress < 1f) || (this.state == GRMetalEnergyGate.State.Closed && this.openProgress > 0f))
		{
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.openProgress = Mathf.MoveTowards(this.openProgress, 1f, Time.deltaTime / this.doorOpenTime);
					float num = this.doorOpenCurve.Evaluate(this.openProgress);
					this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, num);
					this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, num);
				}
			}
			else
			{
				this.openProgress = Mathf.MoveTowards(this.openProgress, 0f, Time.deltaTime / this.doorOpenTime);
				float num2 = this.doorCloseCurve.Evaluate(this.openProgress);
				this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, num2);
				this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, num2);
			}
			yield return null;
		}
		this.doorAnimationCoroutine = null;
		yield break;
	}

	// Token: 0x04003A1A RID: 14874
	[SerializeField]
	public GRMetalEnergyGate.DoorParams upperDoor;

	// Token: 0x04003A1B RID: 14875
	[SerializeField]
	public GRMetalEnergyGate.DoorParams lowerDoor;

	// Token: 0x04003A1C RID: 14876
	[SerializeField]
	private float doorOpenTime = 1.5f;

	// Token: 0x04003A1D RID: 14877
	[SerializeField]
	private float doorCloseTime = 1.5f;

	// Token: 0x04003A1E RID: 14878
	[SerializeField]
	private AnimationCurve doorOpenCurve;

	// Token: 0x04003A1F RID: 14879
	[SerializeField]
	private AnimationCurve doorCloseCurve;

	// Token: 0x04003A20 RID: 14880
	[SerializeField]
	private AudioClip doorOpenClip;

	// Token: 0x04003A21 RID: 14881
	[SerializeField]
	private AudioClip doorCloseClip;

	// Token: 0x04003A22 RID: 14882
	[SerializeField]
	private List<Transform> enableObjectsOnOpen = new List<Transform>();

	// Token: 0x04003A23 RID: 14883
	[SerializeField]
	private List<Transform> disableObjectsOnOpen = new List<Transform>();

	// Token: 0x04003A24 RID: 14884
	[SerializeField]
	private GRTool tool;

	// Token: 0x04003A25 RID: 14885
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x04003A26 RID: 14886
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003A27 RID: 14887
	public GRMetalEnergyGate.State state;

	// Token: 0x04003A28 RID: 14888
	private float openProgress;

	// Token: 0x04003A29 RID: 14889
	private Coroutine doorAnimationCoroutine;

	// Token: 0x020006D1 RID: 1745
	public enum State
	{
		// Token: 0x04003A2B RID: 14891
		Closed,
		// Token: 0x04003A2C RID: 14892
		Open
	}

	// Token: 0x020006D2 RID: 1746
	[Serializable]
	public struct DoorParams
	{
		// Token: 0x04003A2D RID: 14893
		public Transform doorTransform;

		// Token: 0x04003A2E RID: 14894
		public Transform doorClosedPosition;

		// Token: 0x04003A2F RID: 14895
		public Transform doorOpenPosition;
	}
}
