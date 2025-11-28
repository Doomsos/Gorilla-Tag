using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200043D RID: 1085
public class RigEventVolume : MonoBehaviour
{
	// Token: 0x06001AB2 RID: 6834 RVA: 0x0008D02C File Offset: 0x0008B22C
	private void OnTriggerEnter(Collider other)
	{
		VRRig rig;
		if (other.gameObject.TryGetComponent<VRRig>(ref rig) && !this.gameObjects.Contains(other.gameObject))
		{
			this.gameObjects.Add(other.gameObject);
			this.countChanged(this.gameObjects.Count - 1, this.gameObjects.Count, rig);
		}
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x0008D08C File Offset: 0x0008B28C
	private void OnTriggerExit(Collider other)
	{
		VRRig rig;
		if (other.gameObject.TryGetComponent<VRRig>(ref rig) && this.gameObjects.Contains(other.gameObject))
		{
			this.gameObjects.Remove(other.gameObject);
			this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, rig);
		}
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x0008D0EC File Offset: 0x0008B2EC
	private void countChanged(int oldValue, int newValue, VRRig rig)
	{
		if (newValue > oldValue)
		{
			UnityEvent<VRRig> rigEnters = this.RigEnters;
			if (rigEnters != null)
			{
				rigEnters.Invoke(rig);
			}
			switch (newValue)
			{
			case 1:
			{
				UnityEvent upTo = this.UpTo1;
				if (upTo != null)
				{
					upTo.Invoke();
				}
				break;
			}
			case 2:
			{
				UnityEvent upTo2 = this.UpTo2;
				if (upTo2 != null)
				{
					upTo2.Invoke();
				}
				break;
			}
			case 3:
			{
				UnityEvent upTo3 = this.UpTo3;
				if (upTo3 != null)
				{
					upTo3.Invoke();
				}
				break;
			}
			case 4:
			{
				UnityEvent upTo4 = this.UpTo4;
				if (upTo4 != null)
				{
					upTo4.Invoke();
				}
				break;
			}
			case 5:
			{
				UnityEvent upTo5 = this.UpTo5;
				if (upTo5 != null)
				{
					upTo5.Invoke();
				}
				break;
			}
			case 6:
			{
				UnityEvent upTo6 = this.UpTo6;
				if (upTo6 != null)
				{
					upTo6.Invoke();
				}
				break;
			}
			case 7:
			{
				UnityEvent upTo7 = this.UpTo7;
				if (upTo7 != null)
				{
					upTo7.Invoke();
				}
				break;
			}
			case 8:
			{
				UnityEvent upTo8 = this.UpTo8;
				if (upTo8 != null)
				{
					upTo8.Invoke();
				}
				break;
			}
			case 9:
			{
				UnityEvent upTo9 = this.UpTo9;
				if (upTo9 != null)
				{
					upTo9.Invoke();
				}
				break;
			}
			case 10:
			{
				UnityEvent upTo10 = this.UpTo10;
				if (upTo10 != null)
				{
					upTo10.Invoke();
				}
				break;
			}
			}
		}
		if (newValue < oldValue)
		{
			UnityEvent<VRRig> rigExits = this.RigExits;
			if (rigExits != null)
			{
				rigExits.Invoke(rig);
			}
			switch (newValue)
			{
			case 0:
			{
				UnityEvent downTo = this.DownTo0;
				if (downTo == null)
				{
					return;
				}
				downTo.Invoke();
				return;
			}
			case 1:
			{
				UnityEvent downTo2 = this.DownTo1;
				if (downTo2 == null)
				{
					return;
				}
				downTo2.Invoke();
				return;
			}
			case 2:
			{
				UnityEvent downTo3 = this.DownTo2;
				if (downTo3 == null)
				{
					return;
				}
				downTo3.Invoke();
				return;
			}
			case 3:
			{
				UnityEvent downTo4 = this.DownTo3;
				if (downTo4 == null)
				{
					return;
				}
				downTo4.Invoke();
				return;
			}
			case 4:
			{
				UnityEvent downTo5 = this.DownTo4;
				if (downTo5 == null)
				{
					return;
				}
				downTo5.Invoke();
				return;
			}
			case 5:
			{
				UnityEvent downTo6 = this.DownTo5;
				if (downTo6 == null)
				{
					return;
				}
				downTo6.Invoke();
				return;
			}
			case 6:
			{
				UnityEvent downTo7 = this.DownTo6;
				if (downTo7 == null)
				{
					return;
				}
				downTo7.Invoke();
				return;
			}
			case 7:
			{
				UnityEvent downTo8 = this.DownTo7;
				if (downTo8 == null)
				{
					return;
				}
				downTo8.Invoke();
				return;
			}
			case 8:
			{
				UnityEvent downTo9 = this.DownTo8;
				if (downTo9 == null)
				{
					return;
				}
				downTo9.Invoke();
				return;
			}
			case 9:
			{
				UnityEvent downTo10 = this.DownTo9;
				if (downTo10 == null)
				{
					return;
				}
				downTo10.Invoke();
				break;
			}
			default:
				return;
			}
		}
	}

	// Token: 0x04002411 RID: 9233
	private List<GameObject> gameObjects = new List<GameObject>();

	// Token: 0x04002412 RID: 9234
	[SerializeField]
	private UnityEvent<VRRig> RigEnters;

	// Token: 0x04002413 RID: 9235
	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	// Token: 0x04002414 RID: 9236
	[SerializeField]
	private UnityEvent UpTo1;

	// Token: 0x04002415 RID: 9237
	[SerializeField]
	private UnityEvent DownTo0;

	// Token: 0x04002416 RID: 9238
	[SerializeField]
	private UnityEvent UpTo2;

	// Token: 0x04002417 RID: 9239
	[SerializeField]
	private UnityEvent DownTo1;

	// Token: 0x04002418 RID: 9240
	[SerializeField]
	private UnityEvent UpTo3;

	// Token: 0x04002419 RID: 9241
	[SerializeField]
	private UnityEvent DownTo2;

	// Token: 0x0400241A RID: 9242
	[SerializeField]
	private UnityEvent UpTo4;

	// Token: 0x0400241B RID: 9243
	[SerializeField]
	private UnityEvent DownTo3;

	// Token: 0x0400241C RID: 9244
	[SerializeField]
	private UnityEvent UpTo5;

	// Token: 0x0400241D RID: 9245
	[SerializeField]
	private UnityEvent DownTo4;

	// Token: 0x0400241E RID: 9246
	[SerializeField]
	private UnityEvent UpTo6;

	// Token: 0x0400241F RID: 9247
	[SerializeField]
	private UnityEvent DownTo5;

	// Token: 0x04002420 RID: 9248
	[SerializeField]
	private UnityEvent UpTo7;

	// Token: 0x04002421 RID: 9249
	[SerializeField]
	private UnityEvent DownTo6;

	// Token: 0x04002422 RID: 9250
	[SerializeField]
	private UnityEvent UpTo8;

	// Token: 0x04002423 RID: 9251
	[SerializeField]
	private UnityEvent DownTo7;

	// Token: 0x04002424 RID: 9252
	[SerializeField]
	private UnityEvent UpTo9;

	// Token: 0x04002425 RID: 9253
	[SerializeField]
	private UnityEvent DownTo8;

	// Token: 0x04002426 RID: 9254
	[SerializeField]
	private UnityEvent UpTo10;

	// Token: 0x04002427 RID: 9255
	[SerializeField]
	private UnityEvent DownTo9;
}
