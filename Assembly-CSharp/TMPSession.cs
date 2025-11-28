using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KID.Model;
using UnityEngine;

// Token: 0x02000A28 RID: 2600
public class TMPSession
{
	// Token: 0x17000638 RID: 1592
	// (get) Token: 0x060041FA RID: 16890 RVA: 0x0015CF95 File Offset: 0x0015B195
	public bool IsValidSession
	{
		get
		{
			return (this.IsDefault && this.Permissions != null && this.Permissions.Count > 0) || (!this.IsDefault && this.SessionId != Guid.Empty);
		}
	}

	// Token: 0x060041FB RID: 16891 RVA: 0x0015CFD4 File Offset: 0x0015B1D4
	public TMPSession(Session session, KIDDefaultSession defaultSession, int? age, SessionStatus status)
	{
		this.Permissions = new Dictionary<EKIDFeatures, Permission>();
		this.OptedInPermissions = new HashSet<EKIDFeatures>();
		this.Age = age.GetValueOrDefault();
		this.SessionStatus = status;
		if (session == null && defaultSession == null)
		{
			return;
		}
		if (session == null)
		{
			this.IsDefault = true;
			this.AgeStatus = defaultSession.AgeStatus;
			this.InitialiseDefaultPermissionSet(defaultSession);
			return;
		}
		this.SessionId = session.SessionId;
		this.Etag = session.Etag;
		this.AgeStatus = session.AgeStatus;
		this.KidStatus = session.Status;
		this.DateOfBirth = session.DateOfBirth;
		this.KUID = session.Kuid;
		this.Jurisdiction = session.Jurisdiction;
		this.ManagedBy = session.ManagedBy;
		this.Age = this.GetAgeFromDateOfBirth();
		for (int i = 0; i < session.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(session.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, session.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x060041FC RID: 16892 RVA: 0x0015D11C File Offset: 0x0015B31C
	public void SetOptInPermissions(string[] optedInPermissions)
	{
		if (optedInPermissions == null || optedInPermissions.Length == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning without setting.");
			return;
		}
		int num = 0;
		for (;;)
		{
			int num2 = num;
			int? num3 = (optedInPermissions != null) ? new int?(optedInPermissions.Length) : default(int?);
			if (!(num2 < num3.GetValueOrDefault() & num3 != null))
			{
				break;
			}
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(optedInPermissions[num]);
			if (ekidfeatures != null)
			{
				this.OptInToPermission(ekidfeatures.Value, true);
			}
			num++;
		}
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Constructor OptedInPermissions: {0}", this.GetOptedInPermissions()));
	}

	// Token: 0x060041FD RID: 16893 RVA: 0x0015D1A3 File Offset: 0x0015B3A3
	public bool TryGetPermission(EKIDFeatures feature, out Permission permission)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.LogError("[KID::SESSION] Tried retreiving permission for [" + feature.ToStandardisedString() + "], but does not exist");
			permission = null;
			return false;
		}
		permission = this.Permissions[feature];
		return true;
	}

	// Token: 0x060041FE RID: 16894 RVA: 0x0015D1E1 File Offset: 0x0015B3E1
	public List<Permission> GetAllPermissions()
	{
		return Enumerable.ToList<Permission>(this.Permissions.Values);
	}

	// Token: 0x060041FF RID: 16895 RVA: 0x0015D1F4 File Offset: 0x0015B3F4
	public bool HasPermissionForFeature(EKIDFeatures feature)
	{
		Permission permission;
		if (!this.TryGetPermission(feature, out permission))
		{
			Debug.LogError("[KID::SESSION] Tried checking for permission but couldn't find [" + feature.ToStandardisedString() + "]. Assuming disabled");
			return false;
		}
		return permission.Enabled;
	}

	// Token: 0x06004200 RID: 16896 RVA: 0x0015D230 File Offset: 0x0015B430
	public void OptInToPermission(EKIDFeatures feature, bool optIn)
	{
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Opting in to permission for [{0}] with optIn: {1}", feature.ToStandardisedString(), optIn));
		if (optIn && !this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Add(feature);
			return;
		}
		if (!optIn && this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Remove(feature);
			return;
		}
	}

	// Token: 0x06004201 RID: 16897 RVA: 0x0015D296 File Offset: 0x0015B496
	public bool HasOptedInToPermission(EKIDFeatures feature)
	{
		return this.OptedInPermissions.Contains(feature);
	}

	// Token: 0x06004202 RID: 16898 RVA: 0x0015D2A4 File Offset: 0x0015B4A4
	public string[] GetOptedInPermissions()
	{
		if (this.OptedInPermissions == null || this.OptedInPermissions.Count == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning empty array.");
			return Array.Empty<string>();
		}
		return Enumerable.ToArray<string>(Enumerable.Select<EKIDFeatures, string>(this.OptedInPermissions, (EKIDFeatures f) => f.ToStandardisedString()));
	}

	// Token: 0x06004203 RID: 16899 RVA: 0x0015D308 File Offset: 0x0015B508
	public void UpdatePermission(EKIDFeatures feature, Permission newData)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.Log("[KID::SESSION] Trying to update permission, but could not find [" + feature.ToStandardisedString() + "] in dictionary. Will add new one");
			this.Permissions.Add(feature, null);
		}
		this.Permissions[feature] = newData;
	}

	// Token: 0x06004204 RID: 16900 RVA: 0x0015D358 File Offset: 0x0015B558
	private void InitialiseDefaultPermissionSet(KIDDefaultSession defaultSession)
	{
		for (int i = 0; i < defaultSession.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(defaultSession.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, defaultSession.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x06004205 RID: 16901 RVA: 0x0015D3DC File Offset: 0x0015B5DC
	private int GetAgeFromDateOfBirth()
	{
		DateTime today = DateTime.Today;
		int num = today.Year - this.DateOfBirth.Year;
		int num2 = today.Month - this.DateOfBirth.Month;
		if (num2 < 0)
		{
			num--;
		}
		else if (num2 == 0 && today.Day - this.DateOfBirth.Day < 0)
		{
			num--;
		}
		return num;
	}

	// Token: 0x06004206 RID: 16902 RVA: 0x0015D440 File Offset: 0x0015B640
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("New TMPSession]:");
		stringBuilder.AppendLine(string.Format("    - Is Default    :   {0}", this.IsDefault));
		stringBuilder.AppendLine(string.Format("    - Is Valid      :   {0}", this.IsValidSession));
		stringBuilder.AppendLine(string.Format("    - SessionID     :   {0}", this.SessionId));
		stringBuilder.AppendLine(string.Format("    - Age           :   {0}", this.Age));
		stringBuilder.AppendLine(string.Format("    - AgeStatus     :   {0}", this.AgeStatus));
		stringBuilder.AppendLine(string.Format("    - SessionStatus :   {0}", this.KidStatus));
		stringBuilder.AppendLine("    - DoB           :   " + this.DateOfBirth.ToString());
		stringBuilder.AppendLine("    - KUID          :   " + this.KUID);
		stringBuilder.AppendLine("    - Jurisdiction  :   " + this.Jurisdiction);
		stringBuilder.AppendLine("    - PERMISSIONS   :");
		if (this.Permissions != null)
		{
			foreach (Permission permission in this.Permissions.Values)
			{
				stringBuilder.AppendLine(string.Format("        - {0} - Enabled: {1} - ManagedBy: {2}", permission.Name, permission.Enabled, permission.ManagedBy));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040052FF RID: 21247
	public readonly Guid SessionId;

	// Token: 0x04005300 RID: 21248
	public readonly string Etag;

	// Token: 0x04005301 RID: 21249
	public readonly AgeStatusType AgeStatus;

	// Token: 0x04005302 RID: 21250
	public readonly Session.StatusEnum KidStatus;

	// Token: 0x04005303 RID: 21251
	public readonly Session.ManagedByEnum ManagedBy;

	// Token: 0x04005304 RID: 21252
	public readonly DateTime DateOfBirth;

	// Token: 0x04005305 RID: 21253
	public readonly string Jurisdiction;

	// Token: 0x04005306 RID: 21254
	public readonly string KUID;

	// Token: 0x04005307 RID: 21255
	public readonly int Age;

	// Token: 0x04005308 RID: 21256
	public readonly bool IsDefault;

	// Token: 0x04005309 RID: 21257
	public readonly SessionStatus SessionStatus;

	// Token: 0x0400530A RID: 21258
	private Dictionary<EKIDFeatures, Permission> Permissions;

	// Token: 0x0400530B RID: 21259
	private HashSet<EKIDFeatures> OptedInPermissions;
}
