using System;
using GorillaTag;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000321 RID: 801
public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06001375 RID: 4981 RVA: 0x00070756 File Offset: 0x0006E956
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x06001376 RID: 4982 RVA: 0x0007076A File Offset: 0x0006E96A
	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x00070772 File Offset: 0x0006E972
	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
		this.wasAboveThreshold = false;
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x00070787 File Offset: 0x0006E987
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x0007078F File Offset: 0x0006E98F
	protected void OnDisable()
	{
		this.wasAboveThreshold = false;
		ThermalManager.Unregister(this);
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x0007079E File Offset: 0x0006E99E
	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	// Token: 0x04001CED RID: 7405
	public float radius = 0.2f;

	// Token: 0x04001CEE RID: 7406
	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	// Token: 0x04001CEF RID: 7407
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04001CF0 RID: 7408
	[Tooltip("Optional: Fire events if temperature goes below or above this threshold - Celsius")]
	public float temperatureThreshold;

	// Token: 0x04001CF1 RID: 7409
	[Space]
	public UnityEvent OnAboveThreshold;

	// Token: 0x04001CF2 RID: 7410
	public UnityEvent OnBelowThreshold;

	// Token: 0x04001CF3 RID: 7411
	[DebugOption]
	public float celsius;

	// Token: 0x04001CF4 RID: 7412
	public bool wasAboveThreshold;

	// Token: 0x04001CF5 RID: 7413
	private float defaultCelsius;
}
