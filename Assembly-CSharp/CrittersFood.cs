using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class CrittersFood : CrittersActor
{
	// Token: 0x060001A8 RID: 424 RVA: 0x0000A5E1 File Offset: 0x000087E1
	public override void Initialize()
	{
		base.Initialize();
		this.currentFood = this.maxFood;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000A5F8 File Offset: 0x000087F8
	public void SpawnData(float _maxFood, float _currentFood, float _startingSize)
	{
		this.maxFood = _maxFood;
		this.currentFood = _currentFood;
		this.startingSize = _startingSize;
		this.currentSize = this.currentFood / this.maxFood * this.startingSize;
		this.food.localScale = new Vector3(this.currentSize, this.currentSize, this.currentSize);
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000A658 File Offset: 0x00008858
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasEnabled = base.gameObject.activeSelf;
		this.ProcessFood();
		bool flag2 = Mathf.FloorToInt(this.currentFood) != this.lastFood;
		this.lastFood = Mathf.FloorToInt(this.currentFood);
		if (this.currentFood == 0f && this.disableWhenEmpty)
		{
			this.isEnabled = false;
		}
		if (base.gameObject.activeSelf != this.isEnabled)
		{
			base.gameObject.SetActive(this.isEnabled);
		}
		this.updatedSinceLastFrame = (flag || flag2 || this.wasEnabled != this.isEnabled);
		return this.updatedSinceLastFrame;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000A71A File Offset: 0x0000891A
	public override void ProcessRemote()
	{
		base.ProcessRemote();
		if (!this.isEnabled)
		{
			return;
		}
		this.ProcessFood();
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000A734 File Offset: 0x00008934
	public void ProcessFood()
	{
		if (this.currentSize != this.currentFood / this.maxFood * this.startingSize)
		{
			this.currentSize = this.currentFood / this.maxFood * this.startingSize;
			this.food.localScale = new Vector3(this.currentSize, this.currentSize, this.currentSize);
			if (this.storeCollider != null)
			{
				this.storeCollider.radius = this.currentSize / 2f;
			}
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000A7BE File Offset: 0x000089BE
	public void Feed(float amountEaten)
	{
		this.currentFood = Mathf.Max(0f, this.currentFood - amountEaten);
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000A7D8 File Offset: 0x000089D8
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		int num;
		float value;
		float value2;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value2)))
		{
			return false;
		}
		this.currentFood = (float)num;
		this.maxFood = value.GetFinite();
		this.startingSize = value2.GetFinite();
		return true;
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000A83C File Offset: 0x00008A3C
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(Mathf.FloorToInt(this.currentFood));
		stream.SendNext(this.maxFood);
		stream.SendNext(this.startingSize);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000A888 File Offset: 0x00008A88
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(Mathf.FloorToInt(this.currentFood));
		objList.Add(this.maxFood);
		objList.Add(this.startingSize);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000A8DE File Offset: 0x00008ADE
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 3;
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000A8E8 File Offset: 0x00008AE8
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		float value;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 1], out value))
		{
			return this.TotalActorDataLength();
		}
		float value2;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 2], out value2))
		{
			return this.TotalActorDataLength();
		}
		this.currentFood = (float)num;
		this.maxFood = value.GetFinite();
		this.startingSize = value2.GetFinite();
		return this.TotalActorDataLength();
	}

	// Token: 0x040001F4 RID: 500
	public float maxFood;

	// Token: 0x040001F5 RID: 501
	public float currentFood;

	// Token: 0x040001F6 RID: 502
	private int lastFood;

	// Token: 0x040001F7 RID: 503
	public float startingSize;

	// Token: 0x040001F8 RID: 504
	public float currentSize;

	// Token: 0x040001F9 RID: 505
	public Transform food;

	// Token: 0x040001FA RID: 506
	public bool disableWhenEmpty = true;
}
