using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.DependencyInjection;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200036C RID: 876
public class LckEntitlementsManager : MonoBehaviour
{
	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060014E2 RID: 5346 RVA: 0x00077028 File Offset: 0x00075228
	// (set) Token: 0x060014E3 RID: 5347 RVA: 0x0007702F File Offset: 0x0007522F
	public static bool LckEntitlementsEnabled { get; private set; }

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x060014E4 RID: 5348 RVA: 0x00077037 File Offset: 0x00075237
	// (set) Token: 0x060014E5 RID: 5349 RVA: 0x0007703E File Offset: 0x0007523E
	public static LckEntitlementsManager Instance { get; private set; }

	// Token: 0x060014E6 RID: 5350 RVA: 0x00077046 File Offset: 0x00075246
	private void Awake()
	{
		if (LckEntitlementsManager.Instance != null && LckEntitlementsManager.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LckEntitlementsManager.Instance = this;
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x00077074 File Offset: 0x00075274
	private void OnEnable()
	{
		this.InitializeFeatureAsync();
		this._cleanupProcessedPlayersCoroutine = base.StartCoroutine(this.CleanupProcessedPlayersCoroutine());
		this._getEntitlementsBatchingCoroutine = base.StartCoroutine(this.ProcessBatchedRemotePlayersCoroutine());
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x000770A1 File Offset: 0x000752A1
	private void OnDisable()
	{
		if (this._cleanupProcessedPlayersCoroutine != null)
		{
			base.StopCoroutine(this._cleanupProcessedPlayersCoroutine);
			this._cleanupProcessedPlayersCoroutine = null;
		}
		if (this._getEntitlementsBatchingCoroutine != null)
		{
			base.StopCoroutine(this._getEntitlementsBatchingCoroutine);
			this._getEntitlementsBatchingCoroutine = null;
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x000770DC File Offset: 0x000752DC
	private Task InitializeFeatureAsync()
	{
		LckEntitlementsManager.<InitializeFeatureAsync>d__25 <InitializeFeatureAsync>d__;
		<InitializeFeatureAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitializeFeatureAsync>d__.<>4__this = this;
		<InitializeFeatureAsync>d__.<>1__state = -1;
		<InitializeFeatureAsync>d__.<>t__builder.Start<LckEntitlementsManager.<InitializeFeatureAsync>d__25>(ref <InitializeFeatureAsync>d__);
		return <InitializeFeatureAsync>d__.<>t__builder.Task;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x0007711F File Offset: 0x0007531F
	public void OnLocalPlayerSpawned(string localUserId)
	{
		if (!this.ShouldProcessPlayer(localUserId))
		{
			return;
		}
		base.StartCoroutine(this.ProcessLocalPlayerSpawn(localUserId));
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0007713C File Offset: 0x0007533C
	public void OnRemotePlayerSpawned(string remoteUserId)
	{
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			return;
		}
		if (!this.ShouldProcessPlayer(remoteUserId))
		{
			return;
		}
		HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
		lock (remotePlayersToGetEntitlementsFor)
		{
			if (this._remotePlayersToGetEntitlementsFor.Add(remoteUserId))
			{
				Debug.Log("LCK: Queued remote player " + remoteUserId + " for batched entitlements check.");
			}
		}
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x000771B0 File Offset: 0x000753B0
	private IEnumerator ProcessLocalPlayerSpawn(string userId)
	{
		yield return new WaitUntil(() => this._currentState > LckEntitlementsManager.FeatureState.Checking);
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			yield break;
		}
		base.StartCoroutine(this.AnnouncePlayerPresenceForSession(userId));
		yield break;
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000771C8 File Offset: 0x000753C8
	private bool ShouldProcessPlayer(string userId)
	{
		LckEntitlementsManager.PlayerProcessRecord playerProcessRecord;
		if (!this._processedPlayers.TryGetValue(userId, ref playerProcessRecord))
		{
			playerProcessRecord = new LckEntitlementsManager.PlayerProcessRecord();
			this._processedPlayers[userId] = playerProcessRecord;
		}
		if (Time.time < playerProcessRecord.TimeoutUntilTimestamp)
		{
			Debug.LogWarning("LCK: Player " + userId + " is on a timeout. Entitlements Manager will ignore spawn event.");
			return false;
		}
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.AttemptCount = 0;
		}
		playerProcessRecord.AttemptCount++;
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.TimeoutUntilTimestamp = Time.time + 60f;
			Debug.LogWarning(string.Format("LCK: Player {0} exceeded max attempts. Applying a {1}-minute timeout.", userId, 1f));
			return false;
		}
		Debug.Log(string.Format("LCK: Processing player {0} (Attempt {1}/{2}).", userId, playerProcessRecord.AttemptCount, 3));
		return true;
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x00077292 File Offset: 0x00075492
	private IEnumerator ProcessBatchedRemotePlayersCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(15f);
			HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
			List<string> list;
			lock (remotePlayersToGetEntitlementsFor)
			{
				if (this._remotePlayersToGetEntitlementsFor.Count == 0)
				{
					continue;
				}
				list = Enumerable.ToList<string>(this._remotePlayersToGetEntitlementsFor);
				this._remotePlayersToGetEntitlementsFor.Clear();
			}
			if (list.Count > 0)
			{
				Debug.Log(string.Format("LCK: Processing a batch of {0} remote player(s).", list.Count));
				base.StartCoroutine(this.GetCosmeticsForPlayersCoroutine(list, "ProcessBatchedRemotePlayersCoroutine"));
			}
		}
		yield break;
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000772A1 File Offset: 0x000754A1
	private IEnumerator AnnouncePlayerPresenceForSession(string localPlayerId)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			Debug.LogError("LCK: Called AnnouncePlayerPresenceForSession() but no room was found. Player not announced.");
			yield break;
		}
		string sessionId = "DefaultSessionId";
		Debug.Log(string.Concat(new string[]
		{
			"LCK: Announcing Presence for local player with UserId: ",
			localPlayerId,
			" + Session ID: ",
			sessionId,
			"."
		}));
		int num;
		for (int attempt = 1; attempt <= 2; attempt = num + 1)
		{
			LckEntitlementsManager.<>c__DisplayClass31_0 CS$<>8__locals1 = new LckEntitlementsManager.<>c__DisplayClass31_0();
			CS$<>8__locals1.announcementAsync = this._lckCosmeticsCoordinator.AnnouncePlayerPresenceForSessionAsync(localPlayerId, sessionId);
			yield return new WaitUntil(() => CS$<>8__locals1.announcementAsync.IsCompleted);
			if (!CS$<>8__locals1.announcementAsync.IsFaulted && CS$<>8__locals1.announcementAsync.Result.IsOk)
			{
				Debug.Log("LCK: Successfully set session entitlement.");
				yield break;
			}
			string text = CS$<>8__locals1.announcementAsync.IsFaulted ? CS$<>8__locals1.announcementAsync.Exception.ToString() : CS$<>8__locals1.announcementAsync.Result.Message.ToString();
			Debug.LogError(string.Format("LCK: Error setting session entitlement (Attempt {0}/{1}): {2}", attempt, 2, text));
			CS$<>8__locals1 = null;
			num = attempt;
		}
		Debug.LogError("LCK: All attempts to set session entitlement failed.");
		yield break;
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000772B7 File Offset: 0x000754B7
	private IEnumerator GetCosmeticsForPlayersCoroutine(IEnumerable<string> playerUserIds, string methodNameForLogging)
	{
		List<string> userIdList = ((playerUserIds != null) ? Enumerable.ToList<string>(playerUserIds) : null) ?? new List<string>();
		if (userIdList.Count == 0)
		{
			yield break;
		}
		if (PhotonNetwork.CurrentRoom == null)
		{
			Debug.LogError("LCK: Called " + methodNameForLogging + " but no room was found.");
			yield break;
		}
		string sessionId = "DefaultSessionId";
		Debug.Log(string.Concat(new string[]
		{
			"LCK: Calling ",
			methodNameForLogging,
			" for session: ",
			sessionId,
			" for players: ",
			string.Join(", ", userIdList),
			"."
		}));
		int num;
		for (int attempt = 1; attempt <= 2; attempt = num + 1)
		{
			LckEntitlementsManager.<>c__DisplayClass32_0 CS$<>8__locals1 = new LckEntitlementsManager.<>c__DisplayClass32_0();
			CS$<>8__locals1.getUserCosmeticsTask = this._lckCosmeticsCoordinator.GetUserCosmeticsForSessionAsync(userIdList, sessionId);
			yield return new WaitUntil(() => CS$<>8__locals1.getUserCosmeticsTask.IsCompleted);
			if (!CS$<>8__locals1.getUserCosmeticsTask.IsFaulted && CS$<>8__locals1.getUserCosmeticsTask.Result.IsOk)
			{
				Debug.Log("LCK: Successfully called " + methodNameForLogging + " endpoint.");
				yield break;
			}
			string text = CS$<>8__locals1.getUserCosmeticsTask.IsFaulted ? CS$<>8__locals1.getUserCosmeticsTask.Exception.ToString() : CS$<>8__locals1.getUserCosmeticsTask.Result.Message.ToString();
			Debug.LogError(string.Format("LCK: Error in {0} (Attempt {1}/{2}): {3}", new object[]
			{
				methodNameForLogging,
				attempt,
				2,
				text
			}));
			CS$<>8__locals1 = null;
			num = attempt;
		}
		Debug.LogError("LCK: All attempts to call " + methodNameForLogging + " failed.");
		yield break;
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000772D4 File Offset: 0x000754D4
	private IEnumerator CleanupProcessedPlayersCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(60f);
			List<string> list = Enumerable.ToList<string>(Enumerable.Select<KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord>, string>(Enumerable.Where<KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord>>(this._processedPlayers, (KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord> pair) => pair.Value.TimeoutUntilTimestamp > 0f && Time.time > pair.Value.TimeoutUntilTimestamp), (KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord> pair) => pair.Key));
			if (Enumerable.Any<string>(list))
			{
				using (List<string>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						this._processedPlayers.Remove(text);
					}
					continue;
				}
				yield break;
			}
		}
	}

	// Token: 0x04001F6F RID: 8047
	[InjectLck]
	private ILckCosmeticsCoordinator _lckCosmeticsCoordinator;

	// Token: 0x04001F70 RID: 8048
	[InjectLck]
	private ILckCosmeticsFeatureFlagManager _featureFlagManager;

	// Token: 0x04001F71 RID: 8049
	private const int MAX_API_CALL_ATTEMPTS = 2;

	// Token: 0x04001F72 RID: 8050
	private const int MAX_CONSECUTIVE_ATTEMPTS = 3;

	// Token: 0x04001F73 RID: 8051
	private const float ABUSE_TIMEOUT_MINUTES = 1f;

	// Token: 0x04001F74 RID: 8052
	private const float BATCH_GET_ENTITLEMENTS_INTERVAL_SECONDS = 15f;

	// Token: 0x04001F75 RID: 8053
	private const string DEFAULT_SESSION_ID = "DefaultSessionId";

	// Token: 0x04001F77 RID: 8055
	private LckEntitlementsManager.FeatureState _currentState;

	// Token: 0x04001F78 RID: 8056
	private readonly HashSet<string> _remotePlayersToGetEntitlementsFor = new HashSet<string>();

	// Token: 0x04001F79 RID: 8057
	private Coroutine _getEntitlementsBatchingCoroutine;

	// Token: 0x04001F7A RID: 8058
	private readonly Dictionary<string, LckEntitlementsManager.PlayerProcessRecord> _processedPlayers = new Dictionary<string, LckEntitlementsManager.PlayerProcessRecord>();

	// Token: 0x04001F7B RID: 8059
	private Coroutine _cleanupProcessedPlayersCoroutine;

	// Token: 0x0200036D RID: 877
	private class PlayerProcessRecord
	{
		// Token: 0x04001F7D RID: 8061
		public int AttemptCount;

		// Token: 0x04001F7E RID: 8062
		public float TimeoutUntilTimestamp;
	}

	// Token: 0x0200036E RID: 878
	private enum FeatureState
	{
		// Token: 0x04001F80 RID: 8064
		Checking,
		// Token: 0x04001F81 RID: 8065
		Enabled,
		// Token: 0x04001F82 RID: 8066
		Disabled
	}
}
