using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusion;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000817 RID: 2071
[NetworkBehaviourWeaved(4)]
public class MagicCauldron : NetworkComponent
{
	// Token: 0x06003675 RID: 13941 RVA: 0x00126D70 File Offset: 0x00124F70
	private new void Awake()
	{
		this.currentIngredients.Clear();
		this.witchesComponent.Clear();
		this.currentStateElapsedTime = 0f;
		this.currentRecipeIndex = -1;
		this.ingredientIndex = -1;
		if (this.flyingWitchesContainer != null)
		{
			for (int i = 0; i < this.flyingWitchesContainer.transform.childCount; i++)
			{
				NoncontrollableBroomstick componentInChildren = this.flyingWitchesContainer.transform.GetChild(i).gameObject.GetComponentInChildren<NoncontrollableBroomstick>();
				this.witchesComponent.Add(componentInChildren);
				if (componentInChildren)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}
		if (this.reusableFXContext == null)
		{
			this.reusableFXContext = new MagicCauldron.IngrediantFXContext();
		}
		if (this.reusableIngrediantArgs == null)
		{
			this.reusableIngrediantArgs = new MagicCauldron.IngredientArgs();
		}
		this.reusableFXContext.fxCallBack = new MagicCauldron.IngrediantFXContext.Callback(this.OnIngredientAdd);
	}

	// Token: 0x06003676 RID: 13942 RVA: 0x00126E4E File Offset: 0x0012504E
	private new void Start()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06003677 RID: 13943 RVA: 0x00126E57 File Offset: 0x00125057
	private void LateUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06003678 RID: 13944 RVA: 0x00126E5F File Offset: 0x0012505F
	private IEnumerator LevitationSpellCoroutine()
	{
		GTPlayer.Instance.SetHalloweenLevitation(this.levitationStrength, this.levitationDuration, this.levitationBlendOutDuration, this.levitationBonusStrength, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield return new WaitForSeconds(this.levitationSpellDuration);
		GTPlayer.Instance.SetHalloweenLevitation(0f, this.levitationDuration, this.levitationBlendOutDuration, 0f, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield break;
	}

	// Token: 0x06003679 RID: 13945 RVA: 0x00126E70 File Offset: 0x00125070
	private void ChangeState(MagicCauldron.CauldronState state)
	{
		this.currentState = state;
		if (base.IsMine)
		{
			this.currentStateElapsedTime = 0f;
		}
		bool flag = state == MagicCauldron.CauldronState.summoned;
		foreach (NoncontrollableBroomstick noncontrollableBroomstick in this.witchesComponent)
		{
			if (noncontrollableBroomstick.gameObject.activeSelf != flag)
			{
				noncontrollableBroomstick.gameObject.SetActive(flag);
			}
		}
		if (this.currentState == MagicCauldron.CauldronState.summoned && Vector3.Distance(GTPlayer.Instance.transform.position, base.transform.position) < this.levitationRadius)
		{
			base.StartCoroutine(this.LevitationSpellCoroutine());
		}
		switch (this.currentState)
		{
		case MagicCauldron.CauldronState.notReady:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronNotReadyColor);
			return;
		case MagicCauldron.CauldronState.ready:
			this.UpdateCauldronColor(this.CauldronActiveColor);
			return;
		case MagicCauldron.CauldronState.recipeCollecting:
			if (this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
			{
				this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
				return;
			}
			break;
		case MagicCauldron.CauldronState.recipeActivated:
			if (this.audioSource && this.recipes[this.currentRecipeIndex].successAudio)
			{
				this.audioSource.GTPlayOneShot(this.recipes[this.currentRecipeIndex].successAudio, 1f);
			}
			if (this.successParticle)
			{
				this.successParticle.Play();
				return;
			}
			break;
		case MagicCauldron.CauldronState.summoned:
			break;
		case MagicCauldron.CauldronState.failed:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			this.audioSource.GTPlayOneShot(this.recipeFailedAudio, 1f);
			return;
		case MagicCauldron.CauldronState.cooldown:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x00127068 File Offset: 0x00125268
	private void UpdateState()
	{
		if (base.IsMine)
		{
			this.currentStateElapsedTime += Time.deltaTime;
			switch (this.currentState)
			{
			case MagicCauldron.CauldronState.notReady:
			case MagicCauldron.CauldronState.ready:
				break;
			case MagicCauldron.CauldronState.recipeCollecting:
				if (this.currentStateElapsedTime >= this.maxTimeToAddAllIngredients && !this.CheckIngredients())
				{
					this.ChangeState(MagicCauldron.CauldronState.failed);
					return;
				}
				break;
			case MagicCauldron.CauldronState.recipeActivated:
				if (this.currentStateElapsedTime >= this.waitTimeToSummonWitches)
				{
					this.ChangeState(MagicCauldron.CauldronState.summoned);
					return;
				}
				break;
			case MagicCauldron.CauldronState.summoned:
				if (this.currentStateElapsedTime >= this.summonWitchesDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.cooldown);
					return;
				}
				break;
			case MagicCauldron.CauldronState.failed:
				if (this.currentStateElapsedTime >= this.recipeFailedDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
					return;
				}
				break;
			case MagicCauldron.CauldronState.cooldown:
				if (this.currentStateElapsedTime >= this.cooldownDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600367B RID: 13947 RVA: 0x00127131 File Offset: 0x00125331
	public void OnEventStart()
	{
		this.ChangeState(MagicCauldron.CauldronState.ready);
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x00126E4E File Offset: 0x0012504E
	public void OnEventEnd()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x0012713A File Offset: 0x0012533A
	[PunRPC]
	public void OnIngredientAdd(int _ingredientIndex, PhotonMessageInfo info)
	{
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x0012714C File Offset: 0x0012534C
	[Rpc(1, 7)]
	public unsafe void RPC_OnIngredientAdd(int _ingredientIndex, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 1) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", base.Object, 1);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = _ingredientIndex;
							num2 += 4;
							ptr.Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, 0, 0);
							goto IL_12;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x001272B0 File Offset: 0x001254B0
	private void OnIngredientAddShared(int _ingredientIndex, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnIngredientAdd");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.reusableFXContext.playerSettings = rigContainer.Rig.fxSettings;
		this.reusableIngrediantArgs.key = _ingredientIndex;
		FXSystem.PlayFX<MagicCauldron.IngredientArgs>(FXType.HWIngredients, this.reusableFXContext, this.reusableIngrediantArgs, info);
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x00127314 File Offset: 0x00125514
	private void OnIngredientAdd(int _ingredientIndex)
	{
		if (this.audioSource)
		{
			this.audioSource.GTPlayOneShot(this.ingredientAddedAudio, 1f);
		}
		if (!RoomSystem.AmITheHost)
		{
			return;
		}
		if (_ingredientIndex < 0 || _ingredientIndex >= this.allIngredients.Length || (this.currentState != MagicCauldron.CauldronState.ready && this.currentState != MagicCauldron.CauldronState.recipeCollecting))
		{
			return;
		}
		MagicIngredientType magicIngredientType = this.allIngredients[_ingredientIndex];
		Debug.Log(string.Format("Received ingredient RPC {0} = {1}", _ingredientIndex, magicIngredientType));
		MagicIngredientType magicIngredientType2 = null;
		if (this.recipes[0].recipeIngredients.Count > this.currentIngredients.Count)
		{
			magicIngredientType2 = this.recipes[0].recipeIngredients[this.currentIngredients.Count];
		}
		if (!(magicIngredientType == magicIngredientType2))
		{
			Debug.Log(string.Format("Failure: Expected ingredient {0}, got {1} from recipe[{2}]", magicIngredientType2, magicIngredientType, this.currentIngredients.Count));
			this.ChangeState(MagicCauldron.CauldronState.failed);
			return;
		}
		this.ingredientIndex = _ingredientIndex;
		this.currentIngredients.Add(magicIngredientType);
		if (this.CheckIngredients())
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeActivated);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.ready)
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeCollecting);
			return;
		}
		this.UpdateCauldronColor(magicIngredientType.color);
	}

	// Token: 0x06003681 RID: 13953 RVA: 0x00127448 File Offset: 0x00125648
	private bool CheckIngredients()
	{
		foreach (MagicCauldron.Recipe recipe in this.recipes)
		{
			if (Enumerable.SequenceEqual<MagicIngredientType>(this.currentIngredients, recipe.recipeIngredients))
			{
				this.currentRecipeIndex = this.recipes.IndexOf(recipe);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003682 RID: 13954 RVA: 0x001274C0 File Offset: 0x001256C0
	private void UpdateCauldronColor(Color color)
	{
		if (this.bubblesParticle)
		{
			if (this.bubblesParticle.isPlaying)
			{
				if (this.currentState == MagicCauldron.CauldronState.failed || this.currentState == MagicCauldron.CauldronState.notReady)
				{
					this.bubblesParticle.Stop();
				}
			}
			else
			{
				this.bubblesParticle.Play();
			}
		}
		this.currentColor = this.cauldronColor;
		if (this.currentColor == color)
		{
			return;
		}
		if (this.rendr)
		{
			this._liquid.AnimateColorFromTo(this.cauldronColor, color, 1f);
			this.cauldronColor = color;
		}
		if (this.bubblesParticle)
		{
			this.bubblesParticle.main.startColor = color;
		}
	}

	// Token: 0x06003683 RID: 13955 RVA: 0x0012757C File Offset: 0x0012577C
	private void OnTriggerEnter(Collider other)
	{
		ThrowableSetDressing componentInParent = other.GetComponentInParent<ThrowableSetDressing>();
		if (componentInParent == null || componentInParent.IngredientTypeSO == null || componentInParent.InHand())
		{
			return;
		}
		if (componentInParent.IsLocalOwnedWorldShareable)
		{
			if (componentInParent.IngredientTypeSO != null && (this.currentState == MagicCauldron.CauldronState.ready || this.currentState == MagicCauldron.CauldronState.recipeCollecting))
			{
				int num = this.allIngredients.IndexOfRef(componentInParent.IngredientTypeSO);
				Debug.Log(string.Format("Sending ingredient RPC {0} = {1}", componentInParent.IngredientTypeSO, num));
				base.SendRPC("OnIngredientAdd", 1, new object[]
				{
					num
				});
				this.OnIngredientAdd(num);
			}
			componentInParent.StartRespawnTimer(0f);
		}
		if (componentInParent.IngredientTypeSO != null && this.splashParticle)
		{
			this.splashParticle.Play();
		}
	}

	// Token: 0x06003684 RID: 13956 RVA: 0x00127658 File Offset: 0x00125858
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		this.currentIngredients.Clear();
	}

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x06003685 RID: 13957 RVA: 0x00127671 File Offset: 0x00125871
	// (set) Token: 0x06003686 RID: 13958 RVA: 0x0012769B File Offset: 0x0012589B
	[Networked]
	[NetworkedWeaved(0, 4)]
	private unsafe MagicCauldron.MagicCauldronData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MagicCauldron.MagicCauldronData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MagicCauldron.MagicCauldronData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x001276C6 File Offset: 0x001258C6
	public override void WriteDataFusion()
	{
		this.Data = new MagicCauldron.MagicCauldronData(this.currentStateElapsedTime, this.currentRecipeIndex, this.currentState, this.ingredientIndex);
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x001276EC File Offset: 0x001258EC
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.CurrentStateElapsedTime, this.Data.CurrentRecipeIndex, this.Data.CurrentState, this.Data.IngredientIndex);
	}

	// Token: 0x06003689 RID: 13961 RVA: 0x00127738 File Offset: 0x00125938
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.currentStateElapsedTime);
		stream.SendNext(this.currentRecipeIndex);
		stream.SendNext(this.currentState);
		stream.SendNext(this.ingredientIndex);
	}

	// Token: 0x0600368A RID: 13962 RVA: 0x00127798 File Offset: 0x00125998
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float stateElapsedTime = (float)stream.ReceiveNext();
		int recipeIndex = (int)stream.ReceiveNext();
		MagicCauldron.CauldronState state = (MagicCauldron.CauldronState)stream.ReceiveNext();
		int num = (int)stream.ReceiveNext();
		this.ReadDataShared(stateElapsedTime, recipeIndex, state, num);
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x001277F0 File Offset: 0x001259F0
	private void ReadDataShared(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
	{
		MagicCauldron.CauldronState cauldronState = this.currentState;
		this.currentStateElapsedTime = stateElapsedTime;
		this.currentRecipeIndex = recipeIndex;
		this.currentState = state;
		this.ingredientIndex = ingredientIndex;
		if (cauldronState != this.currentState)
		{
			this.ChangeState(this.currentState);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.recipeCollecting && ingredientIndex != ingredientIndex && ingredientIndex >= 0 && ingredientIndex < this.allIngredients.Length)
		{
			this.UpdateCauldronColor(this.allIngredients[ingredientIndex].color);
		}
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x001278ED File Offset: 0x00125AED
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x00127905 File Offset: 0x00125B05
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x0012791C File Offset: 0x00125B1C
	[NetworkRpcWeavedInvoker(1, 1, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnIngredientAdd@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((MagicCauldron)behaviour).RPC_OnIngredientAdd(num3, info);
	}

	// Token: 0x040045CC RID: 17868
	public List<MagicCauldron.Recipe> recipes = new List<MagicCauldron.Recipe>();

	// Token: 0x040045CD RID: 17869
	public float maxTimeToAddAllIngredients = 30f;

	// Token: 0x040045CE RID: 17870
	public float summonWitchesDuration = 20f;

	// Token: 0x040045CF RID: 17871
	public float recipeFailedDuration = 5f;

	// Token: 0x040045D0 RID: 17872
	public float cooldownDuration = 30f;

	// Token: 0x040045D1 RID: 17873
	public MagicIngredientType[] allIngredients;

	// Token: 0x040045D2 RID: 17874
	public GameObject flyingWitchesContainer;

	// Token: 0x040045D3 RID: 17875
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040045D4 RID: 17876
	public AudioClip ingredientAddedAudio;

	// Token: 0x040045D5 RID: 17877
	public AudioClip recipeFailedAudio;

	// Token: 0x040045D6 RID: 17878
	public ParticleSystem bubblesParticle;

	// Token: 0x040045D7 RID: 17879
	public ParticleSystem successParticle;

	// Token: 0x040045D8 RID: 17880
	public ParticleSystem splashParticle;

	// Token: 0x040045D9 RID: 17881
	public Color CauldronActiveColor;

	// Token: 0x040045DA RID: 17882
	public Color CauldronFailedColor;

	// Token: 0x040045DB RID: 17883
	[Tooltip("only if we are using the time of day event")]
	public Color CauldronNotReadyColor;

	// Token: 0x040045DC RID: 17884
	private readonly List<NoncontrollableBroomstick> witchesComponent = new List<NoncontrollableBroomstick>();

	// Token: 0x040045DD RID: 17885
	private readonly List<MagicIngredientType> currentIngredients = new List<MagicIngredientType>();

	// Token: 0x040045DE RID: 17886
	private float currentStateElapsedTime;

	// Token: 0x040045DF RID: 17887
	private MagicCauldron.CauldronState currentState;

	// Token: 0x040045E0 RID: 17888
	[SerializeField]
	private Renderer rendr;

	// Token: 0x040045E1 RID: 17889
	private Color cauldronColor;

	// Token: 0x040045E2 RID: 17890
	private Color currentColor;

	// Token: 0x040045E3 RID: 17891
	private int currentRecipeIndex;

	// Token: 0x040045E4 RID: 17892
	private int ingredientIndex;

	// Token: 0x040045E5 RID: 17893
	private float waitTimeToSummonWitches = 2f;

	// Token: 0x040045E6 RID: 17894
	[Space]
	[SerializeField]
	private MagicCauldronLiquid _liquid;

	// Token: 0x040045E7 RID: 17895
	private MagicCauldron.IngrediantFXContext reusableFXContext = new MagicCauldron.IngrediantFXContext();

	// Token: 0x040045E8 RID: 17896
	private MagicCauldron.IngredientArgs reusableIngrediantArgs = new MagicCauldron.IngredientArgs();

	// Token: 0x040045E9 RID: 17897
	public bool testLevitationAlwaysOn;

	// Token: 0x040045EA RID: 17898
	public float levitationRadius;

	// Token: 0x040045EB RID: 17899
	public float levitationSpellDuration;

	// Token: 0x040045EC RID: 17900
	public float levitationStrength;

	// Token: 0x040045ED RID: 17901
	public float levitationDuration;

	// Token: 0x040045EE RID: 17902
	public float levitationBlendOutDuration;

	// Token: 0x040045EF RID: 17903
	public float levitationBonusStrength;

	// Token: 0x040045F0 RID: 17904
	public float levitationBonusOffAtYSpeed;

	// Token: 0x040045F1 RID: 17905
	public float levitationBonusFullAtYSpeed;

	// Token: 0x040045F2 RID: 17906
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 4)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private MagicCauldron.MagicCauldronData _Data;

	// Token: 0x02000818 RID: 2072
	private enum CauldronState
	{
		// Token: 0x040045F4 RID: 17908
		notReady,
		// Token: 0x040045F5 RID: 17909
		ready,
		// Token: 0x040045F6 RID: 17910
		recipeCollecting,
		// Token: 0x040045F7 RID: 17911
		recipeActivated,
		// Token: 0x040045F8 RID: 17912
		summoned,
		// Token: 0x040045F9 RID: 17913
		failed,
		// Token: 0x040045FA RID: 17914
		cooldown
	}

	// Token: 0x02000819 RID: 2073
	[Serializable]
	public struct Recipe
	{
		// Token: 0x040045FB RID: 17915
		public List<MagicIngredientType> recipeIngredients;

		// Token: 0x040045FC RID: 17916
		public AudioClip successAudio;
	}

	// Token: 0x0200081A RID: 2074
	private class IngredientArgs : FXSArgs
	{
		// Token: 0x040045FD RID: 17917
		public int key;
	}

	// Token: 0x0200081B RID: 2075
	private class IngrediantFXContext : IFXContextParems<MagicCauldron.IngredientArgs>
	{
		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06003691 RID: 13969 RVA: 0x0012797C File Offset: 0x00125B7C
		FXSystemSettings IFXContextParems<MagicCauldron.IngredientArgs>.settings
		{
			get
			{
				return this.playerSettings;
			}
		}

		// Token: 0x06003692 RID: 13970 RVA: 0x00127984 File Offset: 0x00125B84
		void IFXContextParems<MagicCauldron.IngredientArgs>.OnPlayFX(MagicCauldron.IngredientArgs args)
		{
			this.fxCallBack(args.key);
		}

		// Token: 0x040045FE RID: 17918
		public FXSystemSettings playerSettings;

		// Token: 0x040045FF RID: 17919
		public MagicCauldron.IngrediantFXContext.Callback fxCallBack;

		// Token: 0x0200081C RID: 2076
		// (Invoke) Token: 0x06003695 RID: 13973
		public delegate void Callback(int key);
	}

	// Token: 0x0200081D RID: 2077
	[NetworkStructWeaved(4)]
	[StructLayout(2, Size = 16)]
	private struct MagicCauldronData : INetworkStruct
	{
		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06003698 RID: 13976 RVA: 0x00127997 File Offset: 0x00125B97
		// (set) Token: 0x06003699 RID: 13977 RVA: 0x0012799F File Offset: 0x00125B9F
		public float CurrentStateElapsedTime { readonly get; set; }

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x0600369A RID: 13978 RVA: 0x001279A8 File Offset: 0x00125BA8
		// (set) Token: 0x0600369B RID: 13979 RVA: 0x001279B0 File Offset: 0x00125BB0
		public int CurrentRecipeIndex { readonly get; set; }

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x0600369C RID: 13980 RVA: 0x001279B9 File Offset: 0x00125BB9
		// (set) Token: 0x0600369D RID: 13981 RVA: 0x001279C1 File Offset: 0x00125BC1
		public MagicCauldron.CauldronState CurrentState { readonly get; set; }

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x0600369E RID: 13982 RVA: 0x001279CA File Offset: 0x00125BCA
		// (set) Token: 0x0600369F RID: 13983 RVA: 0x001279D2 File Offset: 0x00125BD2
		public int IngredientIndex { readonly get; set; }

		// Token: 0x060036A0 RID: 13984 RVA: 0x001279DB File Offset: 0x00125BDB
		public MagicCauldronData(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
		{
			this.CurrentStateElapsedTime = stateElapsedTime;
			this.CurrentRecipeIndex = recipeIndex;
			this.CurrentState = state;
			this.IngredientIndex = ingredientIndex;
		}
	}
}
