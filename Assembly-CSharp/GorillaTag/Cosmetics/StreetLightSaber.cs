using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200111C RID: 4380
	public class StreetLightSaber : MonoBehaviour
	{
		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06006DA3 RID: 28067 RVA: 0x00240157 File Offset: 0x0023E357
		private StreetLightSaber.State CurrentState
		{
			get
			{
				return StreetLightSaber.values[this.currentIndex];
			}
		}

		// Token: 0x06006DA4 RID: 28068 RVA: 0x00240168 File Offset: 0x0023E368
		private void Awake()
		{
			foreach (StreetLightSaber.StaffStates staffStates in this.allStates)
			{
				this.allStatesDict[staffStates.state] = staffStates;
			}
			this.currentIndex = 0;
			this.autoSwitchEnabledTime = 0f;
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(ref list))
			{
				this.meshRenderer.GetSharedMaterials(list);
				this.instancedMaterial = new Material(list[this.materialIndex]);
				list[this.materialIndex] = this.instancedMaterial;
				this.meshRenderer.SetSharedMaterials(list);
			}
		}

		// Token: 0x06006DA5 RID: 28069 RVA: 0x00240230 File Offset: 0x0023E430
		private void Update()
		{
			if (this.autoSwitch && Time.time - this.autoSwitchEnabledTime > this.autoSwitchTimer)
			{
				this.UpdateStateAuto();
			}
		}

		// Token: 0x06006DA6 RID: 28070 RVA: 0x00240254 File Offset: 0x0023E454
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x06006DA7 RID: 28071 RVA: 0x00240261 File Offset: 0x0023E461
		private void OnEnable()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x06006DA8 RID: 28072 RVA: 0x0024026C File Offset: 0x0023E46C
		public void UpdateStateManual()
		{
			int newIndex = (this.currentIndex + 1) % StreetLightSaber.values.Length;
			this.SwitchState(newIndex);
		}

		// Token: 0x06006DA9 RID: 28073 RVA: 0x00240294 File Offset: 0x0023E494
		private void UpdateStateAuto()
		{
			StreetLightSaber.State state = (this.CurrentState == StreetLightSaber.State.Green) ? StreetLightSaber.State.Red : StreetLightSaber.State.Green;
			int newIndex = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, state);
			this.SwitchState(newIndex);
			this.autoSwitchEnabledTime = Time.time;
		}

		// Token: 0x06006DAA RID: 28074 RVA: 0x002402CD File Offset: 0x0023E4CD
		public void EnableAutoSwitch(bool enable)
		{
			this.autoSwitch = enable;
		}

		// Token: 0x06006DAB RID: 28075 RVA: 0x00240261 File Offset: 0x0023E461
		public void ResetStaff()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x06006DAC RID: 28076 RVA: 0x002402D8 File Offset: 0x0023E4D8
		public void HitReceived(Vector3 contact)
		{
			if (this.velocityTracker != null && this.velocityTracker.GetLatestVelocity(true).magnitude >= this.minHitVelocityThreshold)
			{
				StreetLightSaber.StaffStates staffStates = this.allStatesDict[this.CurrentState];
				if (staffStates == null)
				{
					return;
				}
				staffStates.OnSuccessfulHit.Invoke(contact);
			}
		}

		// Token: 0x06006DAD RID: 28077 RVA: 0x00240330 File Offset: 0x0023E530
		private void SwitchState(int newIndex)
		{
			if (newIndex == this.currentIndex)
			{
				return;
			}
			StreetLightSaber.State currentState = this.CurrentState;
			StreetLightSaber.State state = StreetLightSaber.values[newIndex];
			StreetLightSaber.StaffStates staffStates;
			if (this.allStatesDict.TryGetValue(currentState, ref staffStates))
			{
				UnityEvent onExitState = staffStates.onExitState;
				if (onExitState != null)
				{
					onExitState.Invoke();
				}
			}
			this.currentIndex = newIndex;
			StreetLightSaber.StaffStates staffStates2;
			if (this.allStatesDict.TryGetValue(state, ref staffStates2))
			{
				UnityEvent onEnterState = staffStates2.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
				if (this.trailRenderer != null)
				{
					this.trailRenderer.startColor = staffStates2.color;
				}
				if (this.meshRenderer != null)
				{
					this.instancedMaterial.SetColor(this.hashId, staffStates2.color);
				}
			}
		}

		// Token: 0x06006DAE RID: 28078 RVA: 0x002403E4 File Offset: 0x0023E5E4
		private void ForceSwitchTo(StreetLightSaber.State targetState)
		{
			int num = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, targetState);
			if (num >= 0)
			{
				this.SwitchState(num);
			}
		}

		// Token: 0x04007F16 RID: 32534
		[SerializeField]
		private float autoSwitchTimer = 5f;

		// Token: 0x04007F17 RID: 32535
		[SerializeField]
		private TrailRenderer trailRenderer;

		// Token: 0x04007F18 RID: 32536
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04007F19 RID: 32537
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04007F1A RID: 32538
		[SerializeField]
		private int materialIndex;

		// Token: 0x04007F1B RID: 32539
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04007F1C RID: 32540
		[SerializeField]
		private float minHitVelocityThreshold;

		// Token: 0x04007F1D RID: 32541
		private static readonly StreetLightSaber.State[] values = (StreetLightSaber.State[])Enum.GetValues(typeof(StreetLightSaber.State));

		// Token: 0x04007F1E RID: 32542
		[Space]
		[Header("Staff State Settings")]
		public StreetLightSaber.StaffStates[] allStates = new StreetLightSaber.StaffStates[0];

		// Token: 0x04007F1F RID: 32543
		private int currentIndex;

		// Token: 0x04007F20 RID: 32544
		private Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates> allStatesDict = new Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates>();

		// Token: 0x04007F21 RID: 32545
		private bool autoSwitch;

		// Token: 0x04007F22 RID: 32546
		private float autoSwitchEnabledTime;

		// Token: 0x04007F23 RID: 32547
		private int hashId;

		// Token: 0x04007F24 RID: 32548
		private Material instancedMaterial;

		// Token: 0x0200111D RID: 4381
		[Serializable]
		public class StaffStates
		{
			// Token: 0x04007F25 RID: 32549
			public StreetLightSaber.State state;

			// Token: 0x04007F26 RID: 32550
			public Color color;

			// Token: 0x04007F27 RID: 32551
			public UnityEvent onEnterState;

			// Token: 0x04007F28 RID: 32552
			public UnityEvent onExitState;

			// Token: 0x04007F29 RID: 32553
			public UnityEvent<Vector3> OnSuccessfulHit;
		}

		// Token: 0x0200111E RID: 4382
		public enum State
		{
			// Token: 0x04007F2B RID: 32555
			Off,
			// Token: 0x04007F2C RID: 32556
			Green,
			// Token: 0x04007F2D RID: 32557
			Red
		}
	}
}
