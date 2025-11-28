using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x0200031D RID: 797
public class StringFormatter
{
	// Token: 0x06001355 RID: 4949 RVA: 0x0006FD9F File Offset: 0x0006DF9F
	public StringFormatter(string[] spans, int[] indices)
	{
		this.spans = spans;
		this.indices = indices;
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x0006FDB8 File Offset: 0x0006DFB8
	public string Format(string term1)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(term1);
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x0006FE20 File Offset: 0x0006E020
	public string Format(Func<string> term1)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(term1.Invoke());
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0006FE8C File Offset: 0x0006E08C
	public string Format(string term1, string term2)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append((this.indices[i - 1] == 0) ? term1 : term2);
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x0006FF04 File Offset: 0x0006E104
	public string Format(string term1, string term2, string term3)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			int num = this.indices[i - 1];
			if (num != 0)
			{
				if (num != 1)
				{
					StringFormatter.builder.Append(term3);
				}
				else
				{
					StringFormatter.builder.Append(term2);
				}
			}
			else
			{
				StringFormatter.builder.Append(term1);
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x0006FF9C File Offset: 0x0006E19C
	public string Format(Func<string> term1, Func<string> term2)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			if (this.indices[i - 1] == 0)
			{
				StringFormatter.builder.Append(term1.Invoke());
			}
			else
			{
				StringFormatter.builder.Append(term2.Invoke());
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x00070028 File Offset: 0x0006E228
	public string Format(Func<string> term1, Func<string> term2, Func<string> term3)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			int num = this.indices[i - 1];
			if (num != 0)
			{
				if (num != 1)
				{
					StringFormatter.builder.Append(term3.Invoke());
				}
				else
				{
					StringFormatter.builder.Append(term2.Invoke());
				}
			}
			else
			{
				StringFormatter.builder.Append(term1.Invoke());
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x000700D0 File Offset: 0x0006E2D0
	public string Format(Func<string> term1, Func<string> term2, Func<string> term3, Func<string> term4)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			switch (this.indices[i - 1])
			{
			case 0:
				StringFormatter.builder.Append(term1.Invoke());
				break;
			case 1:
				StringFormatter.builder.Append(term2.Invoke());
				break;
			case 2:
				StringFormatter.builder.Append(term3.Invoke());
				break;
			default:
				StringFormatter.builder.Append(term4.Invoke());
				break;
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0007019C File Offset: 0x0006E39C
	public string Format(params string[] terms)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(terms[this.indices[i - 1]]);
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x00070210 File Offset: 0x0006E410
	public string Format(params Func<string>[] terms)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(terms[this.indices[i - 1]].Invoke());
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x00070288 File Offset: 0x0006E488
	public static StringFormatter Parse(string input)
	{
		int num = 0;
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		for (;;)
		{
			int num2 = input.IndexOf('%', num);
			if (num2 == -1)
			{
				break;
			}
			list.Add(input.Substring(num, num2 - num));
			list2.Add((int)(input.get_Chars(num2 + 1) - '0'));
			num = num2 + 2;
		}
		list.Add(input.Substring(num));
		return new StringFormatter(list.ToArray(), list2.ToArray());
	}

	// Token: 0x04001CDD RID: 7389
	private static StringBuilder builder = new StringBuilder();

	// Token: 0x04001CDE RID: 7390
	private string[] spans;

	// Token: 0x04001CDF RID: 7391
	private int[] indices;
}
