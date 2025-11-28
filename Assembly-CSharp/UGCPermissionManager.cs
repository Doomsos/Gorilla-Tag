using System;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x02000A76 RID: 2678
internal class UGCPermissionManager : MonoBehaviour
{
	// Token: 0x06004356 RID: 17238 RVA: 0x00165C6A File Offset: 0x00163E6A
	public static void UsePlayFabSafety()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.PlayFabPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x06004357 RID: 17239 RVA: 0x00165C8C File Offset: 0x00163E8C
	public static void UseKID()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.KIDPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x17000660 RID: 1632
	// (get) Token: 0x06004358 RID: 17240 RVA: 0x00165CAE File Offset: 0x00163EAE
	public static bool IsUGCDisabled
	{
		get
		{
			return !UGCPermissionManager.isUGCEnabled.GetValueOrDefault();
		}
	}

	// Token: 0x06004359 RID: 17241 RVA: 0x00165CBD File Offset: 0x00163EBD
	public static void CheckPermissions()
	{
		UGCPermissionManager.IUGCPermissions iugcpermissions = UGCPermissionManager.permissions;
		if (iugcpermissions == null)
		{
			return;
		}
		iugcpermissions.CheckPermissions();
	}

	// Token: 0x0600435A RID: 17242 RVA: 0x00165CCE File Offset: 0x00163ECE
	public static void SubscribeToUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x0600435B RID: 17243 RVA: 0x00165CE5 File Offset: 0x00163EE5
	public static void UnsubscribeFromUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x0600435C RID: 17244 RVA: 0x00165CFC File Offset: 0x00163EFC
	public static void SubscribeToUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x0600435D RID: 17245 RVA: 0x00165D13 File Offset: 0x00163F13
	public static void UnsubscribeFromUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x0600435E RID: 17246 RVA: 0x00165D2C File Offset: 0x00163F2C
	private static void SetUGCEnabled(bool enabled)
	{
		bool? flag = UGCPermissionManager.isUGCEnabled;
		if (!(enabled == flag.GetValueOrDefault() & flag != null))
		{
			Debug.LogFormat("[UGCPermissionManager][KID] UGC state changed: [{0}]", new object[]
			{
				enabled ? "ENABLED" : "DISABLED"
			});
			UGCPermissionManager.isUGCEnabled = new bool?(enabled);
			if (enabled)
			{
				Debug.Log("[UGCPermissionManager][KID] Invoking onUGCEnabled");
				Action action = UGCPermissionManager.onUGCEnabled;
				if (action == null)
				{
					return;
				}
				action.Invoke();
				return;
			}
			else
			{
				Debug.Log("[UGCPermissionManager][KID] Invoking onUGCDisabled");
				Action action2 = UGCPermissionManager.onUGCDisabled;
				if (action2 == null)
				{
					return;
				}
				action2.Invoke();
			}
		}
	}

	// Token: 0x040054E7 RID: 21735
	[OnEnterPlay_SetNull]
	private static UGCPermissionManager.IUGCPermissions permissions;

	// Token: 0x040054E8 RID: 21736
	[OnEnterPlay_SetNull]
	private static Action onUGCEnabled;

	// Token: 0x040054E9 RID: 21737
	[OnEnterPlay_SetNull]
	private static Action onUGCDisabled;

	// Token: 0x040054EA RID: 21738
	private static bool? isUGCEnabled;

	// Token: 0x02000A77 RID: 2679
	private interface IUGCPermissions
	{
		// Token: 0x06004360 RID: 17248
		void Initialize();

		// Token: 0x06004361 RID: 17249
		void CheckPermissions();
	}

	// Token: 0x02000A78 RID: 2680
	private class PlayFabPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06004362 RID: 17250 RVA: 0x00165DB6 File Offset: 0x00163FB6
		public PlayFabPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x06004363 RID: 17251 RVA: 0x00165DC8 File Offset: 0x00163FC8
		public void Initialize()
		{
			bool safety = PlayFabAuthenticator.instance.GetSafety();
			Debug.LogFormat("[UGCPermissionManager][KID] UGC initialized from Playfab: [{0}]", new object[]
			{
				safety ? "DISABLED" : "ENABLED"
			});
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action.Invoke(!safety);
		}

		// Token: 0x06004364 RID: 17252 RVA: 0x00002789 File Offset: 0x00000989
		public void CheckPermissions()
		{
		}

		// Token: 0x040054EB RID: 21739
		private Action<bool> setUGCEnabled;
	}

	// Token: 0x02000A79 RID: 2681
	private class KIDPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06004365 RID: 17253 RVA: 0x00165E18 File Offset: 0x00164018
		public KIDPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x06004366 RID: 17254 RVA: 0x00165E27 File Offset: 0x00164027
		private void SetUGCEnabled(bool enabled)
		{
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action.Invoke(enabled);
		}

		// Token: 0x06004367 RID: 17255 RVA: 0x00165E3A File Offset: 0x0016403A
		public void Initialize()
		{
			Debug.Log("[UGCPermissionManager][KID] Initializing with KID");
			this.CheckPermissions();
			KIDManager.RegisterSessionUpdatedCallback_UGC(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdate));
		}

		// Token: 0x06004368 RID: 17256 RVA: 0x00165E60 File Offset: 0x00164060
		public void CheckPermissions()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Mods);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, permissionDataByFeature.Enabled, permissionDataByFeature.ManagedBy);
		}

		// Token: 0x06004369 RID: 17257 RVA: 0x00165E94 File Offset: 0x00164094
		private void OnKIDSessionUpdate(bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.Log("[UGCPermissionManager][KID] KID session update.");
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, isEnabled, managedBy);
		}

		// Token: 0x0600436A RID: 17258 RVA: 0x00165EC4 File Offset: 0x001640C4
		private void ProcessPermissionKID(bool hasOptedIn, bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.LogFormat("[UGCPermissionManager][KID] Process KID permissions - opted in: [{0}], enabled: [{1}], managedBy: [{2}].", new object[]
			{
				hasOptedIn,
				isEnabled,
				managedBy
			});
			if (managedBy == 3)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC prohibited.");
				this.SetUGCEnabled(false);
				return;
			}
			if (managedBy != 1)
			{
				if (managedBy == 2)
				{
					Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by guardian. (opted in: [{0}], enabled: [{1}])", new object[]
					{
						hasOptedIn,
						isEnabled
					});
					this.SetUGCEnabled(isEnabled);
				}
				return;
			}
			if (isEnabled)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC managed by player and enabled - opting in and enabling UGC.");
				if (!hasOptedIn)
				{
					KIDManager.SetFeatureOptIn(EKIDFeatures.Mods, true);
				}
				this.SetUGCEnabled(true);
				return;
			}
			Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by player and disabled by default - using opt in status. (opted in: [{0}])", new object[]
			{
				hasOptedIn
			});
			this.SetUGCEnabled(hasOptedIn);
		}

		// Token: 0x040054EC RID: 21740
		private Action<bool> setUGCEnabled;
	}
}
