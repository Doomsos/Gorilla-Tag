using System;
using System.Collections.Generic;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000320 RID: 800
[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600136C RID: 4972 RVA: 0x0007054C File Offset: 0x0006E74C
	public void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTime = Time.time;
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x00070580 File Offset: 0x0006E780
	public void SliceUpdate()
	{
		float num = Time.time - this.lastTime;
		this.lastTime = Time.time;
		for (int i = 0; i < ThermalManager.receivers.Count; i++)
		{
			ThermalReceiver thermalReceiver = ThermalManager.receivers[i];
			Transform transform = thermalReceiver.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num2 = 20f;
			for (int j = 0; j < ThermalManager.sources.Count; j++)
			{
				ThermalSourceVolume thermalSourceVolume = ThermalManager.sources[j];
				Transform transform2 = thermalSourceVolume.transform;
				float x2 = transform2.lossyScale.x;
				float num3 = Vector3.Distance(transform2.position, position);
				float num4 = 1f - Mathf.InverseLerp(thermalSourceVolume.innerRadius * x2, thermalSourceVolume.outerRadius * x2, num3 - thermalReceiver.radius * x);
				num2 += thermalSourceVolume.celsius * num4;
			}
			thermalReceiver.celsius = Mathf.Lerp(thermalReceiver.celsius, num2, num * thermalReceiver.conductivity);
			ContinuousPropertyArray continuousProperties = thermalReceiver.continuousProperties;
			if (continuousProperties != null)
			{
				continuousProperties.ApplyAll(thermalReceiver.celsius);
			}
			if (!thermalReceiver.wasAboveThreshold && thermalReceiver.celsius > thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = true;
				UnityEvent onAboveThreshold = thermalReceiver.OnAboveThreshold;
				if (onAboveThreshold != null)
				{
					onAboveThreshold.Invoke();
				}
			}
			else if (thermalReceiver.wasAboveThreshold && thermalReceiver.celsius < thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = false;
				UnityEvent onBelowThreshold = thermalReceiver.OnBelowThreshold;
				if (onBelowThreshold != null)
				{
					onBelowThreshold.Invoke();
				}
			}
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x00070700 File Offset: 0x0006E900
	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x0007070D File Offset: 0x0006E90D
	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x0007071B File Offset: 0x0006E91B
	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x00070728 File Offset: 0x0006E928
	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	// Token: 0x04001CE9 RID: 7401
	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	// Token: 0x04001CEA RID: 7402
	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	// Token: 0x04001CEB RID: 7403
	[NonSerialized]
	public static ThermalManager instance;

	// Token: 0x04001CEC RID: 7404
	private float lastTime;
}
