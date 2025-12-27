using System;

public class UpgradeSessionData
{
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, default(int?), this.status);
	}

	public readonly SessionStatus status;

	public readonly TMPSession session;
}
