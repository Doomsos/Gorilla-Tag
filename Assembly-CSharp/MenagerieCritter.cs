using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class MenagerieCritter : MonoBehaviour, IHoldableObject, IEyeScannable
{
	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000333 RID: 819 RVA: 0x00013816 File Offset: 0x00011A16
	public Menagerie.CritterData CritterData
	{
		get
		{
			return this._critterData;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000334 RID: 820 RVA: 0x0001381E File Offset: 0x00011A1E
	// (set) Token: 0x06000335 RID: 821 RVA: 0x00013828 File Offset: 0x00011A28
	public MenagerieSlot Slot
	{
		get
		{
			return this._slot;
		}
		set
		{
			if (value == this._slot)
			{
				return;
			}
			if (this._slot && this._slot.critter == this)
			{
				this._slot.critter = null;
			}
			this._slot = value;
			if (this._slot)
			{
				this._slot.critter = this;
			}
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x00013890 File Offset: 0x00011A90
	private void Update()
	{
		this.UpdateAnimation();
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00013898 File Offset: 0x00011A98
	public void ApplyCritterData(Menagerie.CritterData critterData)
	{
		this._critterData = critterData;
		this._critterConfiguration = this._critterData.GetConfiguration();
		this._critterData.instance = this;
		this._critterData.GetConfiguration().ApplyVisualsTo(this.visuals, false);
		this.visuals.SetAppearance(this._critterData.appearance);
		this._animRoot = this.visuals.bodyRoot;
		this._bodyScale = this._animRoot.localScale;
		this.PlayAnimation(this.heldAnimation, Random.value);
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0001392C File Offset: 0x00011B2C
	private void PlayAnimation(CrittersAnim anim, float time = 0f)
	{
		this._currentAnim = anim;
		this._currentAnimTime = time;
		if (this._currentAnim == null)
		{
			this._animRoot.localPosition = Vector3.zero;
			this._animRoot.localRotation = Quaternion.identity;
			this._animRoot.localScale = this._bodyScale;
		}
	}

	// Token: 0x06000339 RID: 825 RVA: 0x00013980 File Offset: 0x00011B80
	private void UpdateAnimation()
	{
		if (this._currentAnim != null)
		{
			this._currentAnimTime += Time.deltaTime * this._currentAnim.playSpeed;
			this._currentAnimTime %= 1f;
			float num = this._currentAnim.squashAmount.Evaluate(this._currentAnimTime);
			float num2 = this._currentAnim.forwardOffset.Evaluate(this._currentAnimTime);
			float num3 = this._currentAnim.horizontalOffset.Evaluate(this._currentAnimTime);
			float num4 = this._currentAnim.verticalOffset.Evaluate(this._currentAnimTime);
			this._animRoot.localPosition = Vector3.Scale(this._bodyScale, new Vector3(num3, num4, num2));
			float num5 = 1f - num;
			num5 *= 0.5f;
			num5 += 1f;
			this._animRoot.localScale = Vector3.Scale(this._bodyScale, new Vector3(num5, num, num5));
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600033A RID: 826 RVA: 0x00002076 File Offset: 0x00000276
	public bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00002789 File Offset: 0x00000989
	public void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00013A80 File Offset: 0x00011C80
	public void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeld = true;
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		if (this.grabbedHaptics)
		{
			CrittersManager.PlayHaptics(this.grabbedHaptics, this.grabbedHapticsStrength, this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(true);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		base.transform.parent = grabbingHand.transform;
		this.isHeld = true;
		this.heldBy = grabbingHand;
		Action onDataChange = this.OnDataChange;
		if (onDataChange == null)
		{
			return;
		}
		onDataChange.Invoke();
	}

	// Token: 0x0600033D RID: 829 RVA: 0x00013B2C File Offset: 0x00011D2C
	public bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		if (this.grabbedHaptics)
		{
			CrittersManager.StopHaptics(this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(false);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		Action<MenagerieCritter> onReleased = this.OnReleased;
		if (onReleased != null)
		{
			onReleased.Invoke(this);
		}
		Action onDataChange = this.OnDataChange;
		if (onDataChange != null)
		{
			onDataChange.Invoke();
		}
		this.ResetToTransform();
		return true;
	}

	// Token: 0x0600033E RID: 830 RVA: 0x00013BFF File Offset: 0x00011DFF
	public void ResetToTransform()
	{
		base.transform.parent = this._slot.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = quaternion.identity;
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00002789 File Offset: 0x00000989
	public void DropItemCleanup()
	{
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000340 RID: 832 RVA: 0x00010327 File Offset: 0x0000E527
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000341 RID: 833 RVA: 0x00013C3C File Offset: 0x00011E3C
	Vector3 IEyeScannable.Position
	{
		get
		{
			return this.bodyCollider.bounds.center;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000342 RID: 834 RVA: 0x00013C5C File Offset: 0x00011E5C
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this.bodyCollider.bounds;
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000343 RID: 835 RVA: 0x00013C69 File Offset: 0x00011E69
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.BuildEyeScannerData();
		}
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00013C71 File Offset: 0x00011E71
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00013C79 File Offset: 0x00011E79
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x06000346 RID: 838 RVA: 0x00013C84 File Offset: 0x00011E84
	private IList<KeyValueStringPair> BuildEyeScannerData()
	{
		this.eyeScanData[0] = new KeyValueStringPair("Name", this._critterConfiguration.critterName);
		this.eyeScanData[1] = new KeyValueStringPair("Type", this._critterConfiguration.animalType.ToString());
		this.eyeScanData[2] = new KeyValueStringPair("Temperament", this._critterConfiguration.behaviour.temperament);
		this.eyeScanData[3] = new KeyValueStringPair("Habitat", this._critterConfiguration.biome.GetHabitatDescription());
		this.eyeScanData[4] = new KeyValueStringPair("Size", this.visuals.Appearance.size.ToString("0.00"));
		this.eyeScanData[5] = new KeyValueStringPair("State", this.GetCurrentStateName());
		return this.eyeScanData;
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000347 RID: 839 RVA: 0x00013D80 File Offset: 0x00011F80
	// (remove) Token: 0x06000348 RID: 840 RVA: 0x00013DB8 File Offset: 0x00011FB8
	public event Action OnDataChange;

	// Token: 0x06000349 RID: 841 RVA: 0x00013DED File Offset: 0x00011FED
	private string GetCurrentStateName()
	{
		if (!this.isHeld)
		{
			return "Content";
		}
		return "Happy";
	}

	// Token: 0x0600034B RID: 843 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00013E3B File Offset: 0x0001203B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00013E43 File Offset: 0x00012043
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x040003C3 RID: 963
	public CritterVisuals visuals;

	// Token: 0x040003C4 RID: 964
	public Collider bodyCollider;

	// Token: 0x040003C5 RID: 965
	[Header("Feedback")]
	public CrittersAnim heldAnimation;

	// Token: 0x040003C6 RID: 966
	public AudioClip grabbedHaptics;

	// Token: 0x040003C7 RID: 967
	public float grabbedHapticsStrength = 1f;

	// Token: 0x040003C8 RID: 968
	public GameObject grabbedFX;

	// Token: 0x040003C9 RID: 969
	private CrittersAnim _currentAnim;

	// Token: 0x040003CA RID: 970
	private float _currentAnimTime;

	// Token: 0x040003CB RID: 971
	private Transform _animRoot;

	// Token: 0x040003CC RID: 972
	private Vector3 _bodyScale;

	// Token: 0x040003CD RID: 973
	public MenagerieCritter.MenagerieCritterState currentState = MenagerieCritter.MenagerieCritterState.Displaying;

	// Token: 0x040003CE RID: 974
	private CritterConfiguration _critterConfiguration;

	// Token: 0x040003CF RID: 975
	private Menagerie.CritterData _critterData;

	// Token: 0x040003D0 RID: 976
	private MenagerieSlot _slot;

	// Token: 0x040003D1 RID: 977
	private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>();

	// Token: 0x040003D2 RID: 978
	private GameObject heldBy;

	// Token: 0x040003D3 RID: 979
	private bool isHeld;

	// Token: 0x040003D4 RID: 980
	private bool isHeldLeftHand;

	// Token: 0x040003D5 RID: 981
	public Action<MenagerieCritter> OnReleased;

	// Token: 0x040003D6 RID: 982
	private KeyValueStringPair[] eyeScanData = new KeyValueStringPair[6];

	// Token: 0x02000081 RID: 129
	public enum MenagerieCritterState
	{
		// Token: 0x040003D9 RID: 985
		Donating,
		// Token: 0x040003DA RID: 986
		Displaying
	}
}
