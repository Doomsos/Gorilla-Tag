using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000D66 RID: 3430
	public class JsonWriter
	{
		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x060053F0 RID: 21488 RVA: 0x001A8684 File Offset: 0x001A6884
		// (set) Token: 0x060053F1 RID: 21489 RVA: 0x001A868C File Offset: 0x001A688C
		public int IndentValue
		{
			get
			{
				return this.indent_value;
			}
			set
			{
				this.indentation = this.indentation / this.indent_value * value;
				this.indent_value = value;
			}
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x060053F2 RID: 21490 RVA: 0x001A86AA File Offset: 0x001A68AA
		// (set) Token: 0x060053F3 RID: 21491 RVA: 0x001A86B2 File Offset: 0x001A68B2
		public bool PrettyPrint
		{
			get
			{
				return this.pretty_print;
			}
			set
			{
				this.pretty_print = value;
			}
		}

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x060053F4 RID: 21492 RVA: 0x001A86BB File Offset: 0x001A68BB
		public TextWriter TextWriter
		{
			get
			{
				return this.writer;
			}
		}

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x060053F5 RID: 21493 RVA: 0x001A86C3 File Offset: 0x001A68C3
		// (set) Token: 0x060053F6 RID: 21494 RVA: 0x001A86CB File Offset: 0x001A68CB
		public bool Validate
		{
			get
			{
				return this.validate;
			}
			set
			{
				this.validate = value;
			}
		}

		// Token: 0x060053F8 RID: 21496 RVA: 0x001A86E0 File Offset: 0x001A68E0
		public JsonWriter()
		{
			this.inst_string_builder = new StringBuilder();
			this.writer = new StringWriter(this.inst_string_builder);
			this.Init();
		}

		// Token: 0x060053F9 RID: 21497 RVA: 0x001A870A File Offset: 0x001A690A
		public JsonWriter(StringBuilder sb) : this(new StringWriter(sb))
		{
		}

		// Token: 0x060053FA RID: 21498 RVA: 0x001A8718 File Offset: 0x001A6918
		public JsonWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.Init();
		}

		// Token: 0x060053FB RID: 21499 RVA: 0x001A873C File Offset: 0x001A693C
		private void DoValidation(Condition cond)
		{
			if (!this.context.ExpectingValue)
			{
				this.context.Count++;
			}
			if (!this.validate)
			{
				return;
			}
			if (this.has_reached_end)
			{
				throw new JsonException("A complete JSON symbol has already been written");
			}
			switch (cond)
			{
			case Condition.InArray:
				if (!this.context.InArray)
				{
					throw new JsonException("Can't close an array here");
				}
				break;
			case Condition.InObject:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't close an object here");
				}
				break;
			case Condition.NotAProperty:
				if (this.context.InObject && !this.context.ExpectingValue)
				{
					throw new JsonException("Expected a property");
				}
				break;
			case Condition.Property:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't add a property here");
				}
				break;
			case Condition.Value:
				if (!this.context.InArray && (!this.context.InObject || !this.context.ExpectingValue))
				{
					throw new JsonException("Can't add a value here");
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060053FC RID: 21500 RVA: 0x001A8860 File Offset: 0x001A6A60
		private void Init()
		{
			this.has_reached_end = false;
			this.hex_seq = new char[4];
			this.indentation = 0;
			this.indent_value = 4;
			this.pretty_print = false;
			this.validate = true;
			this.ctx_stack = new Stack<WriterContext>();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
		}

		// Token: 0x060053FD RID: 21501 RVA: 0x001A88C4 File Offset: 0x001A6AC4
		private static void IntToHex(int n, char[] hex)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = n % 16;
				if (num < 10)
				{
					hex[3 - i] = (char)(48 + num);
				}
				else
				{
					hex[3 - i] = (char)(65 + (num - 10));
				}
				n >>= 4;
			}
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x001A8905 File Offset: 0x001A6B05
		private void Indent()
		{
			if (this.pretty_print)
			{
				this.indentation += this.indent_value;
			}
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x001A8924 File Offset: 0x001A6B24
		private void Put(string str)
		{
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				for (int i = 0; i < this.indentation; i++)
				{
					this.writer.Write(' ');
				}
			}
			this.writer.Write(str);
		}

		// Token: 0x06005400 RID: 21504 RVA: 0x001A8970 File Offset: 0x001A6B70
		private void PutNewline()
		{
			this.PutNewline(true);
		}

		// Token: 0x06005401 RID: 21505 RVA: 0x001A897C File Offset: 0x001A6B7C
		private void PutNewline(bool add_comma)
		{
			if (add_comma && !this.context.ExpectingValue && this.context.Count > 1)
			{
				this.writer.Write(',');
			}
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				this.writer.Write('\n');
			}
		}

		// Token: 0x06005402 RID: 21506 RVA: 0x001A89D8 File Offset: 0x001A6BD8
		private void PutString(string str)
		{
			this.Put(string.Empty);
			this.writer.Write('"');
			int length = str.Length;
			int i = 0;
			while (i < length)
			{
				char c = str.get_Chars(i);
				switch (c)
				{
				case '\b':
					this.writer.Write("\\b");
					break;
				case '\t':
					this.writer.Write("\\t");
					break;
				case '\n':
					this.writer.Write("\\n");
					break;
				case '\v':
					goto IL_E4;
				case '\f':
					this.writer.Write("\\f");
					break;
				case '\r':
					this.writer.Write("\\r");
					break;
				default:
					if (c != '"' && c != '\\')
					{
						goto IL_E4;
					}
					this.writer.Write('\\');
					this.writer.Write(str.get_Chars(i));
					break;
				}
				IL_141:
				i++;
				continue;
				IL_E4:
				if (str.get_Chars(i) >= ' ' && str.get_Chars(i) <= '~')
				{
					this.writer.Write(str.get_Chars(i));
					goto IL_141;
				}
				JsonWriter.IntToHex((int)str.get_Chars(i), this.hex_seq);
				this.writer.Write("\\u");
				this.writer.Write(this.hex_seq);
				goto IL_141;
			}
			this.writer.Write('"');
		}

		// Token: 0x06005403 RID: 21507 RVA: 0x001A8B3E File Offset: 0x001A6D3E
		private void Unindent()
		{
			if (this.pretty_print)
			{
				this.indentation -= this.indent_value;
			}
		}

		// Token: 0x06005404 RID: 21508 RVA: 0x001A8B5B File Offset: 0x001A6D5B
		public override string ToString()
		{
			if (this.inst_string_builder == null)
			{
				return string.Empty;
			}
			return this.inst_string_builder.ToString();
		}

		// Token: 0x06005405 RID: 21509 RVA: 0x001A8B78 File Offset: 0x001A6D78
		public void Reset()
		{
			this.has_reached_end = false;
			this.ctx_stack.Clear();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
			if (this.inst_string_builder != null)
			{
				this.inst_string_builder.Remove(0, this.inst_string_builder.Length);
			}
		}

		// Token: 0x06005406 RID: 21510 RVA: 0x001A8BD3 File Offset: 0x001A6DD3
		public void Write(bool boolean)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(boolean ? "true" : "false");
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005407 RID: 21511 RVA: 0x001A8C03 File Offset: 0x001A6E03
		public void Write(decimal number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005408 RID: 21512 RVA: 0x001A8C30 File Offset: 0x001A6E30
		public void Write(double number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			string text = Convert.ToString(number, JsonWriter.number_format);
			this.Put(text);
			if (text.IndexOf('.') == -1 && text.IndexOf('E') == -1)
			{
				this.writer.Write(".0");
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06005409 RID: 21513 RVA: 0x001A8C8F File Offset: 0x001A6E8F
		public void Write(int number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x0600540A RID: 21514 RVA: 0x001A8CBB File Offset: 0x001A6EBB
		public void Write(long number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x0600540B RID: 21515 RVA: 0x001A8CE7 File Offset: 0x001A6EE7
		public void Write(string str)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			if (str == null)
			{
				this.Put("null");
			}
			else
			{
				this.PutString(str);
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x0600540C RID: 21516 RVA: 0x001A8D19 File Offset: 0x001A6F19
		public void Write(ulong number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x0600540D RID: 21517 RVA: 0x001A8D48 File Offset: 0x001A6F48
		public void WriteArrayEnd()
		{
			this.DoValidation(Condition.InArray);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("]");
		}

		// Token: 0x0600540E RID: 21518 RVA: 0x001A8DB4 File Offset: 0x001A6FB4
		public void WriteArrayStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("[");
			this.context = new WriterContext();
			this.context.InArray = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x0600540F RID: 21519 RVA: 0x001A8E08 File Offset: 0x001A7008
		public void WriteObjectEnd()
		{
			this.DoValidation(Condition.InObject);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("}");
		}

		// Token: 0x06005410 RID: 21520 RVA: 0x001A8E74 File Offset: 0x001A7074
		public void WriteObjectStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("{");
			this.context = new WriterContext();
			this.context.InObject = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x06005411 RID: 21521 RVA: 0x001A8EC8 File Offset: 0x001A70C8
		public void WritePropertyName(string property_name)
		{
			this.DoValidation(Condition.Property);
			this.PutNewline();
			this.PutString(property_name);
			if (this.pretty_print)
			{
				if (property_name.Length > this.context.Padding)
				{
					this.context.Padding = property_name.Length;
				}
				for (int i = this.context.Padding - property_name.Length; i >= 0; i--)
				{
					this.writer.Write(' ');
				}
				this.writer.Write(": ");
			}
			else
			{
				this.writer.Write(':');
			}
			this.context.ExpectingValue = true;
		}

		// Token: 0x04006178 RID: 24952
		private static NumberFormatInfo number_format = NumberFormatInfo.InvariantInfo;

		// Token: 0x04006179 RID: 24953
		private WriterContext context;

		// Token: 0x0400617A RID: 24954
		private Stack<WriterContext> ctx_stack;

		// Token: 0x0400617B RID: 24955
		private bool has_reached_end;

		// Token: 0x0400617C RID: 24956
		private char[] hex_seq;

		// Token: 0x0400617D RID: 24957
		private int indentation;

		// Token: 0x0400617E RID: 24958
		private int indent_value;

		// Token: 0x0400617F RID: 24959
		private StringBuilder inst_string_builder;

		// Token: 0x04006180 RID: 24960
		private bool pretty_print;

		// Token: 0x04006181 RID: 24961
		private bool validate;

		// Token: 0x04006182 RID: 24962
		private TextWriter writer;
	}
}
