using System;
using System.Text;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001104 RID: 4356
	public class NetworkedRandomProvider : MonoBehaviour
	{
		// Token: 0x06006D02 RID: 27906 RVA: 0x0023CBDE File Offset: 0x0023ADDE
		private void Awake()
		{
			if (this.parentTransferable == null)
			{
				this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			}
		}

		// Token: 0x06006D03 RID: 27907 RVA: 0x0023CBFA File Offset: 0x0023ADFA
		private void OnEnable()
		{
			this.EnsureOwner();
		}

		// Token: 0x06006D04 RID: 27908 RVA: 0x0023CC04 File Offset: 0x0023AE04
		private void OnValidate()
		{
			if (this.windowSeconds < 0.01f)
			{
				this.windowSeconds = 0.01f;
			}
			if (this.floatRange.y < this.floatRange.x)
			{
				ref float ptr = ref this.floatRange.x;
				float y = this.floatRange.y;
				float x = this.floatRange.x;
				ptr = y;
				this.floatRange.y = x;
			}
			if (this.doubleMax < this.doubleMin)
			{
				double num = this.doubleMax;
				double num2 = this.doubleMin;
				this.doubleMin = num;
				this.doubleMax = num2;
			}
		}

		// Token: 0x06006D05 RID: 27909 RVA: 0x0023CCA4 File Offset: 0x0023AEA4
		private void Update()
		{
			long num = (long)Math.Floor(this.GetSharedTime() / (double)this.windowSeconds);
			this.debugWindow = num;
		}

		// Token: 0x06006D06 RID: 27910 RVA: 0x0023CCCD File Offset: 0x0023AECD
		private bool ShowFloatRange()
		{
			return this.outputMode == NetworkedRandomProvider.OutputMode.FloatRange;
		}

		// Token: 0x06006D07 RID: 27911 RVA: 0x0023CCD8 File Offset: 0x0023AED8
		private bool ShowDoubleRange()
		{
			return this.outputMode == NetworkedRandomProvider.OutputMode.DoubleRange;
		}

		// Token: 0x06006D08 RID: 27912 RVA: 0x0023CCE3 File Offset: 0x0023AEE3
		private long GetWindowIndex()
		{
			return (long)Math.Floor(this.GetSharedTime() / (double)this.windowSeconds);
		}

		// Token: 0x06006D09 RID: 27913 RVA: 0x0023CCF9 File Offset: 0x0023AEF9
		private double GetSharedTime()
		{
			if (PhotonNetwork.InRoom)
			{
				return PhotonNetwork.Time;
			}
			return (double)Time.realtimeSinceStartup;
		}

		// Token: 0x06006D0A RID: 27914 RVA: 0x0023CD0E File Offset: 0x0023AF0E
		private static ulong Mix64(ulong x)
		{
			x += 11400714819323198485UL;
			x = (x ^ x >> 30) * 13787848793156543929UL;
			x = (x ^ x >> 27) * 10723151780598845931UL;
			x ^= x >> 31;
			return x;
		}

		// Token: 0x06006D0B RID: 27915 RVA: 0x0023CD4A File Offset: 0x0023AF4A
		private static ulong BuildSeed(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (ulong)(windowIndex ^ (long)((long)((ulong)ownerId) << 32) ^ (long)((ulong)objectSalt * 11400714819323198485UL) ^ (long)((ulong)roomSalt * 15183679468541472403UL));
		}

		// Token: 0x06006D0C RID: 27916 RVA: 0x0023CD6D File Offset: 0x0023AF6D
		private static float UnitFloat01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (uint)(NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 40) * 5.9604645E-08f;
		}

		// Token: 0x06006D0D RID: 27917 RVA: 0x0023CD89 File Offset: 0x0023AF89
		private static double UnitDouble01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 11) * 1.1102230246251565E-16;
		}

		// Token: 0x06006D0E RID: 27918 RVA: 0x0023CDA8 File Offset: 0x0023AFA8
		public float NextFloat01()
		{
			this.EnsureOwner();
			long windowIndex = this.GetWindowIndex();
			uint num;
			if (!this.includeRoomNameInSeed)
			{
				num = 0U;
			}
			else
			{
				string s;
				if (!PhotonNetwork.InRoom)
				{
					s = "no_room";
				}
				else
				{
					Room currentRoom = PhotonNetwork.CurrentRoom;
					s = (((currentRoom != null) ? currentRoom.Name : null) ?? "no_room");
				}
				num = NetworkedRandomProvider.StableHash(s);
			}
			uint roomSalt = num;
			float result = NetworkedRandomProvider.UnitFloat01(windowIndex, this.OwnerID, this.objectSalt, roomSalt);
			this.debugResult = result;
			return result;
		}

		// Token: 0x06006D0F RID: 27919 RVA: 0x0023CE18 File Offset: 0x0023B018
		public float NextFloat(float min, float max)
		{
			float num = this.NextFloat01();
			if (max < min)
			{
				float num2 = max;
				float num3 = min;
				min = num2;
				max = num3;
			}
			return Mathf.Lerp(min, max, num);
		}

		// Token: 0x06006D10 RID: 27920 RVA: 0x0023CE40 File Offset: 0x0023B040
		public double NextDouble(double min, double max)
		{
			this.EnsureOwner();
			long windowIndex = this.GetWindowIndex();
			uint num;
			if (!this.includeRoomNameInSeed)
			{
				num = 0U;
			}
			else
			{
				string s;
				if (!PhotonNetwork.InRoom)
				{
					s = "no_room";
				}
				else
				{
					Room currentRoom = PhotonNetwork.CurrentRoom;
					s = (((currentRoom != null) ? currentRoom.Name : null) ?? "no_room");
				}
				num = NetworkedRandomProvider.StableHash(s);
			}
			uint roomSalt = num;
			double num2 = NetworkedRandomProvider.UnitDouble01(windowIndex, this.OwnerID, this.objectSalt, roomSalt);
			if (max < min)
			{
				double num3 = max;
				double num4 = min;
				min = num3;
				max = num4;
			}
			double num5 = min + (max - min) * num2;
			this.debugResult = (float)num5;
			return num5;
		}

		// Token: 0x06006D11 RID: 27921 RVA: 0x0023CEC4 File Offset: 0x0023B0C4
		public float GetSelectedAsFloat()
		{
			switch (this.outputMode)
			{
			default:
				return this.NextFloat01();
			case NetworkedRandomProvider.OutputMode.Double01:
				return (float)this.NextDouble(0.0, 1.0);
			case NetworkedRandomProvider.OutputMode.FloatRange:
				return this.NextFloat(this.floatRange.x, this.floatRange.y);
			case NetworkedRandomProvider.OutputMode.DoubleRange:
				return (float)this.NextDouble(this.doubleMin, this.doubleMax);
			}
		}

		// Token: 0x06006D12 RID: 27922 RVA: 0x0023CF40 File Offset: 0x0023B140
		public double GetSelectedAsDouble()
		{
			switch (this.outputMode)
			{
			default:
				return (double)this.NextFloat01();
			case NetworkedRandomProvider.OutputMode.Double01:
				return this.NextDouble(0.0, 1.0);
			case NetworkedRandomProvider.OutputMode.FloatRange:
				return (double)this.NextFloat(this.floatRange.x, this.floatRange.y);
			case NetworkedRandomProvider.OutputMode.DoubleRange:
				return this.NextDouble(this.doubleMin, this.doubleMax);
			}
		}

		// Token: 0x06006D13 RID: 27923 RVA: 0x0023CFBC File Offset: 0x0023B1BC
		private static uint StableHash(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0U;
			}
			uint num = 2166136261U;
			for (int i = 0; i < s.Length; i++)
			{
				num ^= (uint)s.get_Chars(i);
				num *= 16777619U;
			}
			return num;
		}

		// Token: 0x06006D14 RID: 27924 RVA: 0x0023CFFD File Offset: 0x0023B1FD
		private void EnsureOwner()
		{
			if (this.OwnerID == 0)
			{
				this.TrySetID();
			}
		}

		// Token: 0x06006D15 RID: 27925 RVA: 0x0023D010 File Offset: 0x0023B210
		private void TrySetID()
		{
			if (this.parentTransferable == null)
			{
				string name = base.gameObject.scene.name;
				string text = "/";
				string hierarchyPath = NetworkedRandomProvider.GetHierarchyPath(base.transform);
				Type type = base.GetType();
				string s = name + text + hierarchyPath + ((type != null) ? type.ToString() : null);
				this.OwnerID = s.GetStaticHash();
				return;
			}
			if (this.parentTransferable.IsLocalObject())
			{
				PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
				if (instance != null)
				{
					string playFabPlayerId = instance.GetPlayFabPlayerId();
					Type type2 = base.GetType();
					this.OwnerID = (playFabPlayerId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
					return;
				}
			}
			else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
			{
				string userId = this.parentTransferable.targetRig.creator.UserId;
				Type type3 = base.GetType();
				this.OwnerID = (userId + ((type3 != null) ? type3.ToString() : null)).GetStaticHash();
			}
		}

		// Token: 0x06006D16 RID: 27926 RVA: 0x0023D11C File Offset: 0x0023B31C
		private static string GetHierarchyPath(Transform t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (t != null)
			{
				stringBuilder.Insert(0, "/" + t.name + "#" + t.GetSiblingIndex().ToString());
				t = t.parent;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04007E2A RID: 32298
		[Header("Time Granularity")]
		[Min(0.01f)]
		[Tooltip("Length of the time bucket (seconds). Within a bucket the pick is fixed; re-rolls next bucket.")]
		[SerializeField]
		private float windowSeconds = 1f;

		// Token: 0x04007E2B RID: 32299
		[Tooltip("Mix room name into seed so different rooms never collide.")]
		[SerializeField]
		private bool includeRoomNameInSeed = true;

		// Token: 0x04007E2C RID: 32300
		[Tooltip("Optional - If multiple component live on the same cosmetic, use different salts.")]
		[SerializeField]
		private int objectSalt;

		// Token: 0x04007E2D RID: 32301
		[Header("Output")]
		[SerializeField]
		private NetworkedRandomProvider.OutputMode outputMode;

		// Token: 0x04007E2E RID: 32302
		[SerializeField]
		private Vector2 floatRange = new Vector2(0f, 1f);

		// Token: 0x04007E2F RID: 32303
		[SerializeField]
		private double doubleMin;

		// Token: 0x04007E30 RID: 32304
		[SerializeField]
		private double doubleMax = 1.0;

		// Token: 0x04007E31 RID: 32305
		private TransferrableObject parentTransferable;

		// Token: 0x04007E32 RID: 32306
		private int OwnerID;

		// Token: 0x04007E33 RID: 32307
		[Header("Debug")]
		[SerializeField]
		private long debugWindow;

		// Token: 0x04007E34 RID: 32308
		[SerializeField]
		private float debugResult;

		// Token: 0x02001105 RID: 4357
		public enum OutputMode
		{
			// Token: 0x04007E36 RID: 32310
			Float01,
			// Token: 0x04007E37 RID: 32311
			Double01,
			// Token: 0x04007E38 RID: 32312
			FloatRange,
			// Token: 0x04007E39 RID: 32313
			DoubleRange
		}
	}
}
