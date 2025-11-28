using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002D0 RID: 720
public class GTPosRotConstraints : MonoBehaviour, ISpawnable
{
	// Token: 0x060011BE RID: 4542 RVA: 0x0005DB9C File Offset: 0x0005BD9C
	public void Awake()
	{
		if (this._shouldCallOnSpawnDuringAwake)
		{
			VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
			if (componentInParent == null)
			{
				return;
			}
			((ISpawnable)this).OnSpawn(componentInParent);
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060011BF RID: 4543 RVA: 0x0005DBCC File Offset: 0x0005BDCC
	// (set) Token: 0x060011C0 RID: 4544 RVA: 0x0005DBD4 File Offset: 0x0005BDD4
	public bool IsSpawned { get; set; }

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x060011C1 RID: 4545 RVA: 0x0005DBDD File Offset: 0x0005BDDD
	// (set) Token: 0x060011C2 RID: 4546 RVA: 0x0005DBE5 File Offset: 0x0005BDE5
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060011C3 RID: 4547 RVA: 0x0005DBF0 File Offset: 0x0005BDF0
	void ISpawnable.OnSpawn(VRRig rig)
	{
		Transform[] array = Array.Empty<Transform>();
		string text;
		if (rig != null && !GTHardCodedBones.TryGetBoneXforms(rig, out array, out text))
		{
			Debug.LogError("GTPosRotConstraints: Error getting bone Transforms: " + text, this);
			return;
		}
		for (int i = 0; i < this.constraints.Length; i++)
		{
			GorillaPosRotConstraint gorillaPosRotConstraint = this.constraints[i];
			if (Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.x, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.y, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.z, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.w, 0f))
			{
				gorillaPosRotConstraint.rotationOffset = Quaternion.identity;
			}
			if (!gorillaPosRotConstraint.follower)
			{
				Debug.LogError(string.Concat(new string[]
				{
					string.Format("{0}: Disabling component! At index {1}, Transform `follower` is ", "GTPosRotConstraints", i),
					"null. Affected component path: ",
					base.transform.GetPathQ(),
					"\n- Affected component path: ",
					base.transform.GetPathQ()
				}), this);
				base.enabled = false;
				return;
			}
			if (gorillaPosRotConstraint.sourceGorillaBone == GTHardCodedBones.EBone.None)
			{
				if (!gorillaPosRotConstraint.source)
				{
					if (string.IsNullOrEmpty(gorillaPosRotConstraint.sourceRelativePath))
					{
						Debug.LogError(string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i) + "null, not EBone, and `sourceRelativePath` is null or empty.\n- Affected component path: " + base.transform.GetPathQ(), this);
						base.enabled = false;
						return;
					}
					if (!base.transform.TryFindByPath(gorillaPosRotConstraint.sourceRelativePath, out gorillaPosRotConstraint.source, false))
					{
						Debug.LogError(string.Concat(new string[]
						{
							string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i),
							"null, not EBone, and could not find by path: \"",
							gorillaPosRotConstraint.sourceRelativePath,
							"\"\n- Affected component path: ",
							base.transform.GetPathQ()
						}), this);
						base.enabled = false;
						return;
					}
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
			else
			{
				if (rig == null)
				{
					Debug.LogError("GTPosRotConstraints: Disabling component! `VRRig` could not be found in parents, but " + string.Format("bone at index {0} is set to use EBone `{1}` but without `VRRig` it cannot ", i, gorillaPosRotConstraint.sourceGorillaBone) + "be resolved.\n- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				int boneIndex = GTHardCodedBones.GetBoneIndex(gorillaPosRotConstraint.sourceGorillaBone);
				if (boneIndex <= 0)
				{
					Debug.LogError(string.Format("{0}: (should never happen) Disabling component! At index {1}, could ", "GTPosRotConstraints", i) + string.Format("not find EBone `{0}`.\n", gorillaPosRotConstraint.sourceGorillaBone) + "- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				gorillaPosRotConstraint.source = array[boneIndex];
				if (!gorillaPosRotConstraint.source)
				{
					Debug.LogError(string.Concat(new string[]
					{
						string.Format("{0}: Disabling component! At index {1}, bone {2} was ", "GTPosRotConstraints", i, gorillaPosRotConstraint.sourceGorillaBone),
						"not present in `VRRig` path: ",
						rig.transform.GetPathQ(),
						"\n- Affected component path: ",
						base.transform.GetPathQ()
					}), this);
					base.enabled = false;
					return;
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
		}
		if (base.isActiveAndEnabled && !this._registerOnEnable)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x0005DF67 File Offset: 0x0005C167
	protected void OnEnable()
	{
		if (this.IsSpawned || this._registerOnEnable)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x0005DF7F File Offset: 0x0005C17F
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x0400163E RID: 5694
	[SerializeField]
	private bool _shouldCallOnSpawnDuringAwake;

	// Token: 0x0400163F RID: 5695
	[Tooltip("Used for actors that get disabled and re-enabled")]
	[SerializeField]
	private bool _registerOnEnable;

	// Token: 0x04001640 RID: 5696
	public GorillaPosRotConstraint[] constraints;
}
