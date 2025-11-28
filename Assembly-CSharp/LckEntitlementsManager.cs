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

public class LckEntitlementsManager : MonoBehaviour
{
	public static bool LckEntitlementsEnabled { get; private set; }

	public static LckEntitlementsManager Instance { get; private set; }

	private void Awake()
	{
		if (LckEntitlementsManager.Instance != null && LckEntitlementsManager.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LckEntitlementsManager.Instance = this;
	}

	private void OnEnable()
	{
		this.InitializeFeatureAsync();
		this._cleanupProcessedPlayersCoroutine = base.StartCoroutine(this.CleanupProcessedPlayersCoroutine());
		this._getEntitlementsBatchingCoroutine = base.StartCoroutine(this.ProcessBatchedRemotePlayersCoroutine());
	}

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

	private Task InitializeFeatureAsync()
	{
		LckEntitlementsManager.<InitializeFeatureAsync>d__25 <InitializeFeatureAsync>d__;
		<InitializeFeatureAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitializeFeatureAsync>d__.<>4__this = this;
		<InitializeFeatureAsync>d__.<>1__state = -1;
		<InitializeFeatureAsync>d__.<>t__builder.Start<LckEntitlementsManager.<InitializeFeatureAsync>d__25>(ref <InitializeFeatureAsync>d__);
		return <InitializeFeatureAsync>d__.<>t__builder.Task;
	}

	public void OnLocalPlayerSpawned(string localUserId)
	{
		if (!this.ShouldProcessPlayer(localUserId))
		{
			return;
		}
		base.StartCoroutine(this.ProcessLocalPlayerSpawn(localUserId));
	}

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

	[InjectLck]
	private ILckCosmeticsCoordinator _lckCosmeticsCoordinator;

	[InjectLck]
	private ILckCosmeticsFeatureFlagManager _featureFlagManager;

	private const int MAX_API_CALL_ATTEMPTS = 2;

	private const int MAX_CONSECUTIVE_ATTEMPTS = 3;

	private const float ABUSE_TIMEOUT_MINUTES = 1f;

	private const float BATCH_GET_ENTITLEMENTS_INTERVAL_SECONDS = 15f;

	private const string DEFAULT_SESSION_ID = "DefaultSessionId";

	private LckEntitlementsManager.FeatureState _currentState;

	private readonly HashSet<string> _remotePlayersToGetEntitlementsFor = new HashSet<string>();

	private Coroutine _getEntitlementsBatchingCoroutine;

	private readonly Dictionary<string, LckEntitlementsManager.PlayerProcessRecord> _processedPlayers = new Dictionary<string, LckEntitlementsManager.PlayerProcessRecord>();

	private Coroutine _cleanupProcessedPlayersCoroutine;

	private class PlayerProcessRecord
	{
		public int AttemptCount;

		public float TimeoutUntilTimestamp;
	}

	private enum FeatureState
	{
		Checking,
		Enabled,
		Disabled
	}
}
