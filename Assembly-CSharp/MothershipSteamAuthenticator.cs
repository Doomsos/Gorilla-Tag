using System;
using System.Text;
using Steamworks;
using UnityEngine;

// Token: 0x02000B93 RID: 2963
public class MothershipSteamAuthenticator : MonoBehaviour
{
	// Token: 0x06004943 RID: 18755 RVA: 0x00180F6F File Offset: 0x0017F16F
	public void StartSteamLogIn()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			string nonce = resp.Nonce;
			this.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce, ticket, delegate(LoginResponse successResp)
				{
					Debug.Log("Logged in to Mothership with Steam");
				}, delegate(MothershipError finalError, int finalErrorCode)
				{
					Debug.LogFormat("Could not log into Mothership with Steam {0} {1}", new object[]
					{
						finalError,
						finalErrorCode
					});
				});
			}, delegate(EResult error)
			{
			});
		}, delegate(MothershipError error, int errorcode)
		{
			Debug.LogFormat("Couldn't start mothership auth for steam {0} {1}", new object[]
			{
				error,
				errorcode
			});
		});
	}

	// Token: 0x06004944 RID: 18756 RVA: 0x00180FA4 File Offset: 0x0017F1A4
	public HAuthTicket GetAuthTicket(Action<string> successCallback, Action<EResult> failureCallback)
	{
		HAuthTicket ticketHandle = HAuthTicket.Invalid;
		Callback<GetAuthSessionTicketResponse_t> ticketCallback = null;
		byte[] ticketBlob = new byte[1024];
		uint ticketSize = 0U;
		ticketCallback = Callback<GetAuthSessionTicketResponse_t>.Create(delegate(GetAuthSessionTicketResponse_t response)
		{
			if (response.m_hAuthTicket != ticketHandle)
			{
				return;
			}
			ticketCallback.Dispose();
			ticketCallback = null;
			if (response.m_eResult != 1)
			{
				Action<EResult> failureCallback3 = failureCallback;
				if (failureCallback3 == null)
				{
					return;
				}
				failureCallback3.Invoke(response.m_eResult);
				return;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (uint num = 0U; num < ticketSize; num += 1U)
				{
					stringBuilder.AppendFormat("{0:x2}", ticketBlob[(int)num]);
				}
				Action<string> successCallback2 = successCallback;
				if (successCallback2 == null)
				{
					return;
				}
				successCallback2.Invoke(stringBuilder.ToString());
				return;
			}
		});
		SteamNetworkingIdentity steamNetworkingIdentity = default(SteamNetworkingIdentity);
		ticketHandle = SteamUser.GetAuthSessionTicket(ticketBlob, ticketBlob.Length, ref ticketSize, ref steamNetworkingIdentity);
		if (ticketHandle == HAuthTicket.Invalid)
		{
			Action<EResult> failureCallback2 = failureCallback;
			if (failureCallback2 != null)
			{
				failureCallback2.Invoke(2);
			}
		}
		return ticketHandle;
	}

	// Token: 0x06004945 RID: 18757 RVA: 0x00181058 File Offset: 0x0017F258
	public HAuthTicket GetAuthTicketForWebApi(string authenticatorId, Action<string> successCallback, Action<EResult> failureCallback)
	{
		HAuthTicket ticketHandle = HAuthTicket.Invalid;
		Callback<GetTicketForWebApiResponse_t> ticketCallback = null;
		ticketCallback = Callback<GetTicketForWebApiResponse_t>.Create(delegate(GetTicketForWebApiResponse_t response)
		{
			if (response.m_hAuthTicket != ticketHandle)
			{
				return;
			}
			ticketCallback.Dispose();
			ticketCallback = null;
			if (response.m_eResult != 1)
			{
				Action<EResult> failureCallback3 = failureCallback;
				if (failureCallback3 == null)
				{
					return;
				}
				failureCallback3.Invoke(response.m_eResult);
				return;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < response.m_cubTicket; i++)
				{
					stringBuilder.AppendFormat("{0:x2}", response.m_rgubTicket[i]);
				}
				Action<string> successCallback2 = successCallback;
				if (successCallback2 == null)
				{
					return;
				}
				successCallback2.Invoke(stringBuilder.ToString());
				return;
			}
		});
		ticketHandle = SteamUser.GetAuthTicketForWebApi(authenticatorId);
		if (ticketHandle == HAuthTicket.Invalid)
		{
			Action<EResult> failureCallback2 = failureCallback;
			if (failureCallback2 != null)
			{
				failureCallback2.Invoke(2);
			}
		}
		return ticketHandle;
	}
}
