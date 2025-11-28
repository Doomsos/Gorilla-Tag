using System;
using System.Text;
using AOT;
using Steamworks;
using UnityEngine;

// Token: 0x02000BD4 RID: 3028
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x170006F1 RID: 1777
	// (get) Token: 0x06004AF0 RID: 19184 RVA: 0x0018801A File Offset: 0x0018621A
	protected static SteamManager Instance
	{
		get
		{
			if (SteamManager.s_instance == null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return SteamManager.s_instance;
		}
	}

	// Token: 0x170006F2 RID: 1778
	// (get) Token: 0x06004AF1 RID: 19185 RVA: 0x0018803E File Offset: 0x0018623E
	public static bool Initialized
	{
		get
		{
			return SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x06004AF2 RID: 19186 RVA: 0x0018804A File Offset: 0x0018624A
	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x06004AF3 RID: 19187 RVA: 0x00188052 File Offset: 0x00186252
	[RuntimeInitializeOnLoadMethod(4)]
	private static void InitOnPlayMode()
	{
		SteamManager.s_EverInitialized = false;
		SteamManager.s_instance = null;
	}

	// Token: 0x06004AF4 RID: 19188 RVA: 0x00188060 File Offset: 0x00186260
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			string text = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
			DllNotFoundException ex2 = ex;
			Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null), this);
			Application.Quit();
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x06004AF5 RID: 19189 RVA: 0x00188148 File Offset: 0x00186348
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x06004AF6 RID: 19190 RVA: 0x00188196 File Offset: 0x00186396
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06004AF7 RID: 19191 RVA: 0x001881BA File Offset: 0x001863BA
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x04005B1D RID: 23325
	protected static bool s_EverInitialized;

	// Token: 0x04005B1E RID: 23326
	protected static SteamManager s_instance;

	// Token: 0x04005B1F RID: 23327
	protected bool m_bInitialized;

	// Token: 0x04005B20 RID: 23328
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
}
