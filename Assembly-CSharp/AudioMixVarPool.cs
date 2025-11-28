using System;
using UnityEngine;

// Token: 0x02000539 RID: 1337
[CreateAssetMenu(fileName = "New AudioMixVarPool", menuName = "ScriptableObjects/AudioMixVarPool", order = 0)]
public class AudioMixVarPool : ScriptableObject
{
	// Token: 0x060021AA RID: 8618 RVA: 0x000B0130 File Offset: 0x000AE330
	public bool Rent(out AudioMixVar mixVar)
	{
		for (int i = 0; i < this._vars.Length; i++)
		{
			if (!this._vars[i].taken)
			{
				this._vars[i].taken = true;
				mixVar = this._vars[i];
				return true;
			}
		}
		mixVar = null;
		return false;
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000B0180 File Offset: 0x000AE380
	public void Return(AudioMixVar mixVar)
	{
		if (mixVar == null)
		{
			return;
		}
		int num = this._vars.IndexOfRef(mixVar);
		if (num == -1)
		{
			return;
		}
		this._vars[num].taken = false;
	}

	// Token: 0x04002C57 RID: 11351
	[SerializeField]
	private AudioMixVar[] _vars = new AudioMixVar[0];
}
