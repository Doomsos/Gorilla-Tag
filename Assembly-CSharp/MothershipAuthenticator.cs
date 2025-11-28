using System;
using GorillaExtensions;
using Steamworks;
using UnityEngine;

// Token: 0x02000B91 RID: 2961
public class MothershipAuthenticator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004930 RID: 18736 RVA: 0x00180B59 File Offset: 0x0017ED59
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = null;
		}
	}

	// Token: 0x06004931 RID: 18737 RVA: 0x00180B74 File Offset: 0x0017ED74
	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		if (MothershipAuthenticator.Instance.SteamAuthenticator == null)
		{
			MothershipAuthenticator.Instance.SteamAuthenticator = MothershipAuthenticator.Instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
		}
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
	}

	// Token: 0x06004932 RID: 18738 RVA: 0x00180C08 File Offset: 0x0017EE08
	public void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	// Token: 0x06004933 RID: 18739 RVA: 0x00180C1A File Offset: 0x0017EE1A
	private void LogInWithInsecure()
	{
		MothershipClientApiUnity.LogInWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log("Logged in with Mothership Id " + LoginResponse.MothershipPlayerId);
			MothershipClientApiUnity.OpenNotificationsSocket();
			Action onLoginSuccess = this.OnLoginSuccess;
			if (onLoginSuccess == null)
			{
				return;
			}
			onLoginSuccess.Invoke();
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.Log(string.Format("Failed to log in, error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure.Invoke(1);
			}
			Action<string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure.Invoke(MothershipError.Message);
		});
	}

	// Token: 0x06004934 RID: 18740 RVA: 0x00180C46 File Offset: 0x0017EE46
	private void LogInWithSteam()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			string nonce = resp.Nonce;
			SteamAuthTicket ticketHandle = HAuthTicket.Invalid;
			Action<LoginResponse> <>9__4;
			Action<MothershipError, int> <>9__5;
			ticketHandle = this.SteamAuthenticator.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				string nonce = nonce;
				Action<LoginResponse> action;
				if ((action = <>9__4) == null)
				{
					action = (<>9__4 = delegate(LoginResponse successResp)
					{
						ticketHandle.Dispose();
						Debug.Log("Logged in to Mothership with Steam");
						MothershipClientApiUnity.OpenNotificationsSocket();
						Action onLoginSuccess = this.OnLoginSuccess;
						if (onLoginSuccess == null)
						{
							return;
						}
						onLoginSuccess.Invoke();
					});
				}
				Action<MothershipError, int> action2;
				if ((action2 = <>9__5) == null)
				{
					action2 = (<>9__5 = delegate(MothershipError MothershipError, int errorCode)
					{
						ticketHandle.Dispose();
						Debug.Log(string.Format("Couldn't log into Mothership with Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
						{
							MothershipError.Message,
							MothershipError.TraceId,
							errorCode,
							MothershipError.MothershipErrorCode
						}));
						Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
						if (onLoginAttemptFailure != null)
						{
							onLoginAttemptFailure.Invoke(1);
						}
						Action<string> onLoginFailure = this.OnLoginFailure;
						if (onLoginFailure == null)
						{
							return;
						}
						onLoginFailure.Invoke(MothershipError.Message);
					});
				}
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce, ticket, action, action2);
			}, delegate(EResult error)
			{
				Debug.Log(string.Format("Couldn't get an auth ticket for logging into Mothership with Steam {0}", error));
				Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
				if (onLoginAttemptFailure != null)
				{
					onLoginAttemptFailure.Invoke(1);
				}
				Action<string> onLoginFailure = this.OnLoginFailure;
				if (onLoginFailure == null)
				{
					return;
				}
				onLoginFailure.Invoke(error.ToString());
			});
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.Log(string.Format("Couldn't start Mothership auth for Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure.Invoke(1);
			}
			Action<string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure.Invoke(MothershipError.Message);
		});
	}

	// Token: 0x06004935 RID: 18741 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004936 RID: 18742 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004937 RID: 18743 RVA: 0x00180C66 File Offset: 0x0017EE66
	public void SliceUpdate()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			MothershipClientApiUnity.Tick(Time.deltaTime);
		}
	}

	// Token: 0x040059C3 RID: 22979
	public static volatile MothershipAuthenticator Instance;

	// Token: 0x040059C4 RID: 22980
	public MetaAuthenticator MetaAuthenticator;

	// Token: 0x040059C5 RID: 22981
	public SteamAuthenticator SteamAuthenticator;

	// Token: 0x040059C6 RID: 22982
	public string TestNickname;

	// Token: 0x040059C7 RID: 22983
	public string TestAccountId;

	// Token: 0x040059C8 RID: 22984
	public bool UseConstantTestAccountId;

	// Token: 0x040059C9 RID: 22985
	public int MaxMetaLoginAttempts = 5;

	// Token: 0x040059CA RID: 22986
	public Action OnLoginSuccess;

	// Token: 0x040059CB RID: 22987
	public Action<string> OnLoginFailure;

	// Token: 0x040059CC RID: 22988
	public Action<int> OnLoginAttemptFailure;
}
