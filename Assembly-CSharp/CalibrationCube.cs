using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x02000444 RID: 1092
public class CalibrationCube : MonoBehaviour
{
	// Token: 0x06001ACE RID: 6862 RVA: 0x0008D7D7 File Offset: 0x0008B9D7
	private void Awake()
	{
		this.calibratedLength = this.baseLength;
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x0008D7E8 File Offset: 0x0008B9E8
	private void Start()
	{
		try
		{
			this.OnCollisionExit(null);
		}
		catch
		{
		}
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x00002789 File Offset: 0x00000989
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x00002789 File Offset: 0x00000989
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x0008D814 File Offset: 0x0008BA14
	public void RecalibrateSize(bool pressed)
	{
		this.lastCalibratedLength = this.calibratedLength;
		this.calibratedLength = (this.rightController.transform.position - this.leftController.transform.position).magnitude;
		this.calibratedLength = ((this.calibratedLength > this.maxLength) ? this.maxLength : ((this.calibratedLength < this.minLength) ? this.minLength : this.calibratedLength));
		float num = this.calibratedLength / this.lastCalibratedLength;
		Vector3 localScale = this.playerBody.transform.localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Clear();
		this.playerBody.transform.localScale = new Vector3(1f, 1f, 1f);
		this.playerBody.GetComponentInChildren<TransformReset>().ResetTransforms();
		this.playerBody.transform.localScale = num * localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Build();
		this.playerBody.GetComponentInChildren<VRRig>().SetHeadBodyOffset();
		GorillaPlaySpace.Instance.bodyColliderOffset *= num;
		GorillaPlaySpace.Instance.bodyCollider.gameObject.transform.localScale *= num;
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x00002789 File Offset: 0x00000989
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x0008D970 File Offset: 0x0008BB70
	private void OnCollisionExit(Collision collision)
	{
		try
		{
			bool flag = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyName name = assemblies[i].GetName();
				if (!this.calibrationPresetsTest3[0].Contains(name.Name))
				{
					flag = true;
				}
			}
			if (!flag || Application.platform == 11)
			{
				GorillaComputer.instance.includeUpdatedServerSynchTest = 0;
			}
		}
		catch
		{
		}
	}

	// Token: 0x04002450 RID: 9296
	public PrimaryButtonWatcher watcher;

	// Token: 0x04002451 RID: 9297
	public GameObject rightController;

	// Token: 0x04002452 RID: 9298
	public GameObject leftController;

	// Token: 0x04002453 RID: 9299
	public GameObject playerBody;

	// Token: 0x04002454 RID: 9300
	private float calibratedLength;

	// Token: 0x04002455 RID: 9301
	private float lastCalibratedLength;

	// Token: 0x04002456 RID: 9302
	public float minLength = 1f;

	// Token: 0x04002457 RID: 9303
	public float maxLength = 2.5f;

	// Token: 0x04002458 RID: 9304
	public float baseLength = 1.61f;

	// Token: 0x04002459 RID: 9305
	public string[] calibrationPresets;

	// Token: 0x0400245A RID: 9306
	public string[] calibrationPresetsTest;

	// Token: 0x0400245B RID: 9307
	public string[] calibrationPresetsTest2;

	// Token: 0x0400245C RID: 9308
	public string[] calibrationPresetsTest3;

	// Token: 0x0400245D RID: 9309
	public string[] calibrationPresetsTest4;

	// Token: 0x0400245E RID: 9310
	public string outputstring;

	// Token: 0x0400245F RID: 9311
	private List<string> stringList = new List<string>();
}
