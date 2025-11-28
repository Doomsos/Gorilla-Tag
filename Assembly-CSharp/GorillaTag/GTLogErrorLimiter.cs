using System;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Text;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FE2 RID: 4066
	public class GTLogErrorLimiter
	{
		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x060066DB RID: 26331 RVA: 0x00217530 File Offset: 0x00215730
		// (set) Token: 0x060066DC RID: 26332 RVA: 0x00217538 File Offset: 0x00215738
		public string baseMessage
		{
			get
			{
				return this._baseMessage;
			}
			set
			{
				this._baseMessage = (value ?? "__NULL__");
			}
		}

		// Token: 0x060066DD RID: 26333 RVA: 0x0021754A File Offset: 0x0021574A
		public GTLogErrorLimiter(string baseMessage, int countdown = 10, string occurrencesJoinString = "\n- ")
		{
			this.baseMessage = baseMessage;
			this.countdown = countdown;
			this.sb = ZString.CreateStringBuilder();
			this.sb.Append(this.baseMessage);
			this.occurrencesJoinString = occurrencesJoinString;
		}

		// Token: 0x060066DE RID: 26334 RVA: 0x00217584 File Offset: 0x00215784
		public void Log(string subMessage = "", Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.countdown < 0)
			{
				return;
			}
			if (this.countdown == 0)
			{
				this.sb.Insert(0, "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n");
			}
			this.sb.Append(subMessage ?? "__NULL__");
			this.sb.Append("\n\nError origin - Caller: ");
			this.sb.Append(caller ?? "__NULL__");
			this.sb.Append(", Line: ");
			this.sb.Append(line);
			this.sb.Append("File: ");
			this.sb.Append(sourceFilePath ?? "__NULL__");
			Debug.LogError(this.sb.ToString(), context);
			this.sb.Clear();
			this.sb.Append(this.baseMessage);
			this.countdown--;
			this.occurrenceCount = 0;
		}

		// Token: 0x060066DF RID: 26335 RVA: 0x00217679 File Offset: 0x00215879
		public void Log(Object obj, Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (!obj)
			{
				this.Log("__NULL__", context, caller, sourceFilePath, line);
				return;
			}
			this.Log(obj.ToString(), null, "Log", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\GTLogErrorLimiter.cs", 137);
		}

		// Token: 0x060066E0 RID: 26336 RVA: 0x002176B1 File Offset: 0x002158B1
		public void AddOccurrence(string s)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append(s);
		}

		// Token: 0x060066E1 RID: 26337 RVA: 0x002176E7 File Offset: 0x002158E7
		public void AddOccurrence(StringBuilder stringBuilder)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append<StringBuilder>(stringBuilder);
		}

		// Token: 0x060066E2 RID: 26338 RVA: 0x00217720 File Offset: 0x00215920
		public void AddOccurence(GameObject gObj)
		{
			this.occurrenceCount++;
			if (gObj == null)
			{
				this.AddOccurrence("__NULL__");
				return;
			}
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Q(gObj.GetPath());
		}

		// Token: 0x060066E3 RID: 26339 RVA: 0x0021777C File Offset: 0x0021597C
		public void LogOccurrences(Component component = null, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.Log("\"" + component.GetComponentPath(int.MaxValue) + "\"", obj, caller, sourceFilePath, line);
		}

		// Token: 0x060066E4 RID: 26340 RVA: 0x002177DC File Offset: 0x002159DC
		public void LogOccurrences(Utf16ValueStringBuilder subMessage, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.sb.Append<Utf16ValueStringBuilder>(subMessage);
			this.Log("", obj, caller, sourceFilePath, line);
		}

		// Token: 0x04007565 RID: 30053
		private const string __NULL__ = "__NULL__";

		// Token: 0x04007566 RID: 30054
		public int countdown;

		// Token: 0x04007567 RID: 30055
		public int occurrenceCount;

		// Token: 0x04007568 RID: 30056
		public string occurrencesJoinString;

		// Token: 0x04007569 RID: 30057
		private string _baseMessage;

		// Token: 0x0400756A RID: 30058
		public Utf16ValueStringBuilder sb;

		// Token: 0x0400756B RID: 30059
		private const string k_lastMsgHeader = "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n";
	}
}
