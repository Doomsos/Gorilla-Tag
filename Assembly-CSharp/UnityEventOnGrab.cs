using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004F6 RID: 1270
[RequireComponent(typeof(TransferrableObject))]
public class UnityEventOnGrab : MonoBehaviour
{
	// Token: 0x060020A9 RID: 8361 RVA: 0x000AD708 File Offset: 0x000AB908
	private void Awake()
	{
		TransferrableObject componentInParent = base.GetComponentInParent<TransferrableObject>();
		Behaviour[] behavioursEnabledOnlyWhileHeld = componentInParent.behavioursEnabledOnlyWhileHeld;
		Behaviour[] array = new Behaviour[behavioursEnabledOnlyWhileHeld.Length + 1];
		for (int i = 0; i < behavioursEnabledOnlyWhileHeld.Length; i++)
		{
			array[i] = behavioursEnabledOnlyWhileHeld[i];
		}
		array[behavioursEnabledOnlyWhileHeld.Length] = this;
		componentInParent.behavioursEnabledOnlyWhileHeld = array;
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x000AD74F File Offset: 0x000AB94F
	private void OnEnable()
	{
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000AD761 File Offset: 0x000AB961
	private void OnDisable()
	{
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04002B44 RID: 11076
	[SerializeField]
	private UnityEvent onGrab;

	// Token: 0x04002B45 RID: 11077
	[SerializeField]
	private UnityEvent onRelease;
}
