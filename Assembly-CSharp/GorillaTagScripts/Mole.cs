using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000D9B RID: 3483
	public class Mole : Tappable
	{
		// Token: 0x14000095 RID: 149
		// (add) Token: 0x06005581 RID: 21889 RVA: 0x001AE598 File Offset: 0x001AC798
		// (remove) Token: 0x06005582 RID: 21890 RVA: 0x001AE5D0 File Offset: 0x001AC7D0
		public event Mole.MoleTapEvent OnTapped;

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x06005583 RID: 21891 RVA: 0x001AE605 File Offset: 0x001AC805
		// (set) Token: 0x06005584 RID: 21892 RVA: 0x001AE60D File Offset: 0x001AC80D
		public bool IsLeftSideMole { get; set; }

		// Token: 0x06005585 RID: 21893 RVA: 0x001AE618 File Offset: 0x001AC818
		private void Awake()
		{
			this.currentState = Mole.MoleState.Hidden;
			Vector3 position = base.transform.position;
			this.origin = (this.target = position);
			this.visiblePosition = new Vector3(position.x, position.y + this.positionOffset, position.z);
			this.hiddenPosition = new Vector3(position.x, position.y - this.positionOffset, position.z);
			this.travelTime = this.normalTravelTime;
			this.animCurve = (this.normalAnimCurve = AnimationCurves.EaseInOutQuad);
			this.hitAnimCurve = AnimationCurves.EaseOutBack;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				if (this.moleTypes[i].isHazard)
				{
					this.hazardMoles.Add(i);
				}
				else
				{
					this.safeMoles.Add(i);
				}
			}
			this.randomMolePickedIndex = -1;
		}

		// Token: 0x06005586 RID: 21894 RVA: 0x001AE700 File Offset: 0x001AC900
		public void InvokeUpdate()
		{
			if (this.currentState == Mole.MoleState.Ready)
			{
				return;
			}
			switch (this.currentState)
			{
			case Mole.MoleState.Reset:
			case Mole.MoleState.Hidden:
				this.currentState = Mole.MoleState.Ready;
				break;
			case Mole.MoleState.TransitionToVisible:
			case Mole.MoleState.TransitionToHidden:
			{
				float num = this.animCurve.Evaluate(Mathf.Clamp01((Time.time - this.animStartTime) / this.travelTime));
				base.transform.position = Vector3.Lerp(this.origin, this.target, num);
				if (num >= 1f)
				{
					this.currentState++;
				}
				break;
			}
			}
			if (Time.time - this.currentTime >= this.showMoleDuration && this.currentState > Mole.MoleState.Ready && this.currentState < Mole.MoleState.TransitionToHidden)
			{
				this.HideMole(false);
			}
		}

		// Token: 0x06005587 RID: 21895 RVA: 0x001AE7CB File Offset: 0x001AC9CB
		public bool CanPickMole()
		{
			return this.currentState == Mole.MoleState.Ready;
		}

		// Token: 0x06005588 RID: 21896 RVA: 0x001AE7D8 File Offset: 0x001AC9D8
		public void ShowMole(float _showMoleDuration, int randomMoleTypeIndex)
		{
			if (randomMoleTypeIndex >= this.moleTypes.Length || randomMoleTypeIndex < 0)
			{
				return;
			}
			this.randomMolePickedIndex = randomMoleTypeIndex;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				this.moleTypes[i].gameObject.SetActive(i == randomMoleTypeIndex);
				if (this.moleTypes[i].monkeMoleDefaultMaterial != null)
				{
					this.moleTypes[i].MeshRenderer.material = this.moleTypes[i].monkeMoleDefaultMaterial;
				}
			}
			this.showMoleDuration = _showMoleDuration;
			this.origin = base.transform.position;
			this.target = this.visiblePosition;
			this.animCurve = this.normalAnimCurve;
			this.currentState = Mole.MoleState.TransitionToVisible;
			this.animStartTime = (this.currentTime = Time.time);
			this.travelTime = this.normalTravelTime;
		}

		// Token: 0x06005589 RID: 21897 RVA: 0x001AE8B0 File Offset: 0x001ACAB0
		public void HideMole(bool isHit = false)
		{
			if (this.currentState < Mole.MoleState.TransitionToVisible || this.currentState > Mole.MoleState.Visible)
			{
				return;
			}
			this.origin = base.transform.position;
			this.target = this.hiddenPosition;
			this.animCurve = (isHit ? this.hitAnimCurve : this.normalAnimCurve);
			this.animStartTime = Time.time;
			this.travelTime = (isHit ? this.hitTravelTime : this.normalTravelTime);
			this.currentState = Mole.MoleState.TransitionToHidden;
		}

		// Token: 0x0600558A RID: 21898 RVA: 0x001AE930 File Offset: 0x001ACB30
		public bool CanTap()
		{
			Mole.MoleState moleState = this.currentState;
			return moleState == Mole.MoleState.TransitionToVisible || moleState == Mole.MoleState.Visible;
		}

		// Token: 0x0600558B RID: 21899 RVA: 0x001AE955 File Offset: 0x001ACB55
		public override bool CanTap(bool isLeftHand)
		{
			return this.CanTap();
		}

		// Token: 0x0600558C RID: 21900 RVA: 0x001AE960 File Offset: 0x001ACB60
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!this.CanTap())
			{
				return;
			}
			bool flag = info.Sender.ActorNumber == NetworkSystem.Instance.LocalPlayerID;
			bool isLeft = flag && GorillaTagger.Instance.lastLeftTap >= GorillaTagger.Instance.lastRightTap;
			MoleTypes moleTypes = null;
			if (this.randomMolePickedIndex >= 0 && this.randomMolePickedIndex < this.moleTypes.Length)
			{
				moleTypes = this.moleTypes[this.randomMolePickedIndex];
			}
			if (moleTypes != null)
			{
				Mole.MoleTapEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(moleTypes, base.transform.position, flag, isLeft);
			}
		}

		// Token: 0x0600558D RID: 21901 RVA: 0x001AE9FE File Offset: 0x001ACBFE
		public void ResetPosition()
		{
			base.transform.position = this.hiddenPosition;
			this.currentState = Mole.MoleState.Reset;
		}

		// Token: 0x0600558E RID: 21902 RVA: 0x001AEA18 File Offset: 0x001ACC18
		public int GetMoleTypeIndex(bool useHazardMole)
		{
			if (!useHazardMole)
			{
				return this.safeMoles[Random.Range(0, this.safeMoles.Count)];
			}
			return this.hazardMoles[Random.Range(0, this.hazardMoles.Count)];
		}

		// Token: 0x0400627A RID: 25210
		public float positionOffset = 0.2f;

		// Token: 0x0400627B RID: 25211
		public MoleTypes[] moleTypes;

		// Token: 0x0400627C RID: 25212
		private float showMoleDuration;

		// Token: 0x0400627D RID: 25213
		private Vector3 visiblePosition;

		// Token: 0x0400627E RID: 25214
		private Vector3 hiddenPosition;

		// Token: 0x0400627F RID: 25215
		private float currentTime;

		// Token: 0x04006280 RID: 25216
		private float animStartTime;

		// Token: 0x04006281 RID: 25217
		private float travelTime;

		// Token: 0x04006282 RID: 25218
		private float normalTravelTime = 0.3f;

		// Token: 0x04006283 RID: 25219
		private float hitTravelTime = 0.2f;

		// Token: 0x04006284 RID: 25220
		private AnimationCurve animCurve;

		// Token: 0x04006285 RID: 25221
		private AnimationCurve normalAnimCurve;

		// Token: 0x04006286 RID: 25222
		private AnimationCurve hitAnimCurve;

		// Token: 0x04006287 RID: 25223
		private Mole.MoleState currentState;

		// Token: 0x04006288 RID: 25224
		private Vector3 origin;

		// Token: 0x04006289 RID: 25225
		private Vector3 target;

		// Token: 0x0400628A RID: 25226
		private int randomMolePickedIndex;

		// Token: 0x0400628C RID: 25228
		public CallLimiter rpcCooldown;

		// Token: 0x0400628D RID: 25229
		private int moleScore;

		// Token: 0x0400628E RID: 25230
		private List<int> safeMoles = new List<int>();

		// Token: 0x0400628F RID: 25231
		private List<int> hazardMoles = new List<int>();

		// Token: 0x02000D9C RID: 3484
		// (Invoke) Token: 0x06005591 RID: 21905
		public delegate void MoleTapEvent(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeft);

		// Token: 0x02000D9D RID: 3485
		public enum MoleState
		{
			// Token: 0x04006292 RID: 25234
			Reset,
			// Token: 0x04006293 RID: 25235
			Ready,
			// Token: 0x04006294 RID: 25236
			TransitionToVisible,
			// Token: 0x04006295 RID: 25237
			Visible,
			// Token: 0x04006296 RID: 25238
			TransitionToHidden,
			// Token: 0x04006297 RID: 25239
			Hidden
		}
	}
}
