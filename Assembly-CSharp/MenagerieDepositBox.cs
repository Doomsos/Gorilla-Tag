using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class MenagerieDepositBox : MonoBehaviour
{
	// Token: 0x0600034E RID: 846 RVA: 0x00013E4C File Offset: 0x0001204C
	public void OnTriggerEnter(Collider other)
	{
		MenagerieCritter component = other.transform.parent.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Combine(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00013E94 File Offset: 0x00012094
	public void OnTriggerExit(Collider other)
	{
		MenagerieCritter component = other.transform.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Remove(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x040003DB RID: 987
	public Action<MenagerieCritter> OnCritterInserted;
}
