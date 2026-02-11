using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PersistLog : MonoBehaviour
{
	private void OnEnable()
	{
		PersistLog.<OnEnable>d__1 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<PersistLog.<OnEnable>d__1>(ref <OnEnable>d__);
	}

	private void OnDisable()
	{
		this.OnDestroy();
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= this.LogMessageReceived;
		if (this.sr == null)
		{
			return;
		}
		this.sr.Close();
		this.sr = null;
	}

	private void LogMessageReceived(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.sr.Write(string.Format("T+{0} >> {1}\n==========================\n{2}\n\n", Time.time, msg, strace));
			this.sr.Flush();
		}
	}

	private StreamWriter sr;
}
