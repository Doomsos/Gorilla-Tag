using System;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000D68 RID: 3432
	internal class Lexer
	{
		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x06005413 RID: 21523 RVA: 0x001A8F4A File Offset: 0x001A714A
		// (set) Token: 0x06005414 RID: 21524 RVA: 0x001A8F52 File Offset: 0x001A7152
		public bool AllowComments
		{
			get
			{
				return this.allow_comments;
			}
			set
			{
				this.allow_comments = value;
			}
		}

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x06005415 RID: 21525 RVA: 0x001A8F5B File Offset: 0x001A715B
		// (set) Token: 0x06005416 RID: 21526 RVA: 0x001A8F63 File Offset: 0x001A7163
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.allow_single_quoted_strings;
			}
			set
			{
				this.allow_single_quoted_strings = value;
			}
		}

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x06005417 RID: 21527 RVA: 0x001A8F6C File Offset: 0x001A716C
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x06005418 RID: 21528 RVA: 0x001A8F74 File Offset: 0x001A7174
		public int Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x06005419 RID: 21529 RVA: 0x001A8F7C File Offset: 0x001A717C
		public string StringValue
		{
			get
			{
				return this.string_value;
			}
		}

		// Token: 0x0600541A RID: 21530 RVA: 0x001A8F84 File Offset: 0x001A7184
		static Lexer()
		{
			Lexer.PopulateFsmTables();
		}

		// Token: 0x0600541B RID: 21531 RVA: 0x001A8F8C File Offset: 0x001A718C
		public Lexer(TextReader reader)
		{
			this.allow_comments = true;
			this.allow_single_quoted_strings = true;
			this.input_buffer = 0;
			this.string_buffer = new StringBuilder(128);
			this.state = 1;
			this.end_of_input = false;
			this.reader = reader;
			this.fsm_context = new FsmContext();
			this.fsm_context.L = this;
		}

		// Token: 0x0600541C RID: 21532 RVA: 0x001A8FF0 File Offset: 0x001A71F0
		private static int HexValue(int digit)
		{
			switch (digit)
			{
			case 65:
				break;
			case 66:
				return 11;
			case 67:
				return 12;
			case 68:
				return 13;
			case 69:
				return 14;
			case 70:
				return 15;
			default:
				switch (digit)
				{
				case 97:
					break;
				case 98:
					return 11;
				case 99:
					return 12;
				case 100:
					return 13;
				case 101:
					return 14;
				case 102:
					return 15;
				default:
					return digit - 48;
				}
				break;
			}
			return 10;
		}

		// Token: 0x0600541D RID: 21533 RVA: 0x001A9058 File Offset: 0x001A7258
		private static void PopulateFsmTables()
		{
			Lexer.fsm_handler_table = new Lexer.StateHandler[]
			{
				new Lexer.StateHandler(Lexer.State1),
				new Lexer.StateHandler(Lexer.State2),
				new Lexer.StateHandler(Lexer.State3),
				new Lexer.StateHandler(Lexer.State4),
				new Lexer.StateHandler(Lexer.State5),
				new Lexer.StateHandler(Lexer.State6),
				new Lexer.StateHandler(Lexer.State7),
				new Lexer.StateHandler(Lexer.State8),
				new Lexer.StateHandler(Lexer.State9),
				new Lexer.StateHandler(Lexer.State10),
				new Lexer.StateHandler(Lexer.State11),
				new Lexer.StateHandler(Lexer.State12),
				new Lexer.StateHandler(Lexer.State13),
				new Lexer.StateHandler(Lexer.State14),
				new Lexer.StateHandler(Lexer.State15),
				new Lexer.StateHandler(Lexer.State16),
				new Lexer.StateHandler(Lexer.State17),
				new Lexer.StateHandler(Lexer.State18),
				new Lexer.StateHandler(Lexer.State19),
				new Lexer.StateHandler(Lexer.State20),
				new Lexer.StateHandler(Lexer.State21),
				new Lexer.StateHandler(Lexer.State22),
				new Lexer.StateHandler(Lexer.State23),
				new Lexer.StateHandler(Lexer.State24),
				new Lexer.StateHandler(Lexer.State25),
				new Lexer.StateHandler(Lexer.State26),
				new Lexer.StateHandler(Lexer.State27),
				new Lexer.StateHandler(Lexer.State28)
			};
			Lexer.fsm_return_table = new int[]
			{
				65542,
				0,
				65537,
				65537,
				0,
				65537,
				0,
				65537,
				0,
				0,
				65538,
				0,
				0,
				0,
				65539,
				0,
				0,
				65540,
				65541,
				65542,
				0,
				0,
				65541,
				65542,
				0,
				0,
				0,
				0
			};
		}

		// Token: 0x0600541E RID: 21534 RVA: 0x001A9240 File Offset: 0x001A7440
		private static char ProcessEscChar(int esc_char)
		{
			if (esc_char <= 92)
			{
				if (esc_char <= 39)
				{
					if (esc_char != 34 && esc_char != 39)
					{
						return '?';
					}
				}
				else if (esc_char != 47 && esc_char != 92)
				{
					return '?';
				}
				return Convert.ToChar(esc_char);
			}
			if (esc_char <= 102)
			{
				if (esc_char == 98)
				{
					return '\b';
				}
				if (esc_char == 102)
				{
					return '\f';
				}
			}
			else
			{
				if (esc_char == 110)
				{
					return '\n';
				}
				if (esc_char == 114)
				{
					return '\r';
				}
				if (esc_char == 116)
				{
					return '\t';
				}
			}
			return '?';
		}

		// Token: 0x0600541F RID: 21535 RVA: 0x001A92A8 File Offset: 0x001A74A8
		private static bool State1(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 32 && (ctx.L.input_char < 9 || ctx.L.input_char > 13))
				{
					if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 3;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 91)
					{
						if (num <= 39)
						{
							if (num == 34)
							{
								ctx.NextState = 19;
								ctx.Return = true;
								return true;
							}
							if (num != 39)
							{
								return false;
							}
							if (!ctx.L.allow_single_quoted_strings)
							{
								return false;
							}
							ctx.L.input_char = 34;
							ctx.NextState = 23;
							ctx.Return = true;
							return true;
						}
						else
						{
							switch (num)
							{
							case 44:
								break;
							case 45:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 2;
								return true;
							case 46:
								return false;
							case 47:
								if (!ctx.L.allow_comments)
								{
									return false;
								}
								ctx.NextState = 25;
								return true;
							case 48:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 4;
								return true;
							default:
								if (num != 58 && num != 91)
								{
									return false;
								}
								break;
							}
						}
					}
					else if (num <= 110)
					{
						if (num != 93)
						{
							if (num == 102)
							{
								ctx.NextState = 12;
								return true;
							}
							if (num != 110)
							{
								return false;
							}
							ctx.NextState = 16;
							return true;
						}
					}
					else
					{
						if (num == 116)
						{
							ctx.NextState = 9;
							return true;
						}
						if (num != 123 && num != 125)
						{
							return false;
						}
					}
					ctx.NextState = 1;
					ctx.Return = true;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06005420 RID: 21536 RVA: 0x001A94A0 File Offset: 0x001A76A0
		private static bool State2(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 3;
				return true;
			}
			if (ctx.L.input_char == 48)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 4;
				return true;
			}
			return false;
		}

		// Token: 0x06005421 RID: 21537 RVA: 0x001A9534 File Offset: 0x001A7734
		private static bool State3(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 69)
					{
						if (num != 44)
						{
							if (num == 46)
							{
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 5;
								return true;
							}
							if (num != 69)
							{
								return false;
							}
							goto IL_F4;
						}
					}
					else if (num != 93)
					{
						if (num == 101)
						{
							goto IL_F4;
						}
						if (num != 125)
						{
							return false;
						}
					}
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
					IL_F4:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06005422 RID: 21538 RVA: 0x001A9670 File Offset: 0x001A7870
		private static bool State4(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			int num = ctx.L.input_char;
			if (num <= 69)
			{
				if (num != 44)
				{
					if (num == 46)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 5;
						return true;
					}
					if (num != 69)
					{
						return false;
					}
					goto IL_BB;
				}
			}
			else if (num != 93)
			{
				if (num == 101)
				{
					goto IL_BB;
				}
				if (num != 125)
				{
					return false;
				}
			}
			ctx.L.UngetChar();
			ctx.Return = true;
			ctx.NextState = 1;
			return true;
			IL_BB:
			ctx.L.string_buffer.Append((char)ctx.L.input_char);
			ctx.NextState = 7;
			return true;
		}

		// Token: 0x06005423 RID: 21539 RVA: 0x001A9760 File Offset: 0x001A7960
		private static bool State5(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 6;
				return true;
			}
			return false;
		}

		// Token: 0x06005424 RID: 21540 RVA: 0x001A97C0 File Offset: 0x001A79C0
		private static bool State6(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 69)
					{
						if (num != 44)
						{
							if (num != 69)
							{
								return false;
							}
							goto IL_C9;
						}
					}
					else if (num != 93)
					{
						if (num == 101)
						{
							goto IL_C9;
						}
						if (num != 125)
						{
							return false;
						}
					}
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
					IL_C9:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06005425 RID: 21541 RVA: 0x001A98D0 File Offset: 0x001A7AD0
		private static bool State7(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			int num = ctx.L.input_char;
			if (num == 43 || num == 45)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			return false;
		}

		// Token: 0x06005426 RID: 21542 RVA: 0x001A996C File Offset: 0x001A7B6C
		private static bool State8(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num == 44 || num == 93 || num == 125)
					{
						ctx.L.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005427 RID: 21543 RVA: 0x001A9A41 File Offset: 0x001A7C41
		private static bool State9(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 114)
			{
				ctx.NextState = 10;
				return true;
			}
			return false;
		}

		// Token: 0x06005428 RID: 21544 RVA: 0x001A9A69 File Offset: 0x001A7C69
		private static bool State10(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 117)
			{
				ctx.NextState = 11;
				return true;
			}
			return false;
		}

		// Token: 0x06005429 RID: 21545 RVA: 0x001A9A91 File Offset: 0x001A7C91
		private static bool State11(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600542A RID: 21546 RVA: 0x001A9ABF File Offset: 0x001A7CBF
		private static bool State12(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 97)
			{
				ctx.NextState = 13;
				return true;
			}
			return false;
		}

		// Token: 0x0600542B RID: 21547 RVA: 0x001A9AE7 File Offset: 0x001A7CE7
		private static bool State13(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.NextState = 14;
				return true;
			}
			return false;
		}

		// Token: 0x0600542C RID: 21548 RVA: 0x001A9B0F File Offset: 0x001A7D0F
		private static bool State14(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 115)
			{
				ctx.NextState = 15;
				return true;
			}
			return false;
		}

		// Token: 0x0600542D RID: 21549 RVA: 0x001A9A91 File Offset: 0x001A7C91
		private static bool State15(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600542E RID: 21550 RVA: 0x001A9B37 File Offset: 0x001A7D37
		private static bool State16(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 117)
			{
				ctx.NextState = 17;
				return true;
			}
			return false;
		}

		// Token: 0x0600542F RID: 21551 RVA: 0x001A9B5F File Offset: 0x001A7D5F
		private static bool State17(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.NextState = 18;
				return true;
			}
			return false;
		}

		// Token: 0x06005430 RID: 21552 RVA: 0x001A9B87 File Offset: 0x001A7D87
		private static bool State18(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06005431 RID: 21553 RVA: 0x001A9BB8 File Offset: 0x001A7DB8
		private static bool State19(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num == 34)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 20;
					return true;
				}
				if (num == 92)
				{
					ctx.StateStack = 19;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		// Token: 0x06005432 RID: 21554 RVA: 0x001A9C38 File Offset: 0x001A7E38
		private static bool State20(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 34)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06005433 RID: 21555 RVA: 0x001A9C68 File Offset: 0x001A7E68
		private static bool State21(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num <= 92)
			{
				if (num <= 39)
				{
					if (num != 34 && num != 39)
					{
						return false;
					}
				}
				else if (num != 47 && num != 92)
				{
					return false;
				}
			}
			else if (num <= 102)
			{
				if (num != 98 && num != 102)
				{
					return false;
				}
			}
			else if (num != 110)
			{
				switch (num)
				{
				case 114:
				case 116:
					break;
				case 115:
					return false;
				case 117:
					ctx.NextState = 22;
					return true;
				default:
					return false;
				}
			}
			ctx.L.string_buffer.Append(Lexer.ProcessEscChar(ctx.L.input_char));
			ctx.NextState = ctx.StateStack;
			return true;
		}

		// Token: 0x06005434 RID: 21556 RVA: 0x001A9D1C File Offset: 0x001A7F1C
		private static bool State22(FsmContext ctx)
		{
			int num = 0;
			int num2 = 4096;
			ctx.L.unichar = 0;
			while (ctx.L.GetChar())
			{
				if ((ctx.L.input_char < 48 || ctx.L.input_char > 57) && (ctx.L.input_char < 65 || ctx.L.input_char > 70) && (ctx.L.input_char < 97 || ctx.L.input_char > 102))
				{
					return false;
				}
				ctx.L.unichar += Lexer.HexValue(ctx.L.input_char) * num2;
				num++;
				num2 /= 16;
				if (num == 4)
				{
					ctx.L.string_buffer.Append(Convert.ToChar(ctx.L.unichar));
					ctx.NextState = ctx.StateStack;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06005435 RID: 21557 RVA: 0x001A9E10 File Offset: 0x001A8010
		private static bool State23(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num == 39)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 24;
					return true;
				}
				if (num == 92)
				{
					ctx.StateStack = 23;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		// Token: 0x06005436 RID: 21558 RVA: 0x001A9E90 File Offset: 0x001A8090
		private static bool State24(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 39)
			{
				ctx.L.input_char = 34;
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06005437 RID: 21559 RVA: 0x001A9ECC File Offset: 0x001A80CC
		private static bool State25(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 42)
			{
				ctx.NextState = 27;
				return true;
			}
			if (num != 47)
			{
				return false;
			}
			ctx.NextState = 26;
			return true;
		}

		// Token: 0x06005438 RID: 21560 RVA: 0x001A9F12 File Offset: 0x001A8112
		private static bool State26(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 10)
				{
					ctx.NextState = 1;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06005439 RID: 21561 RVA: 0x001A9F3C File Offset: 0x001A813C
		private static bool State27(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 42)
				{
					ctx.NextState = 28;
					return true;
				}
			}
			return true;
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x001A9F68 File Offset: 0x001A8168
		private static bool State28(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 42)
				{
					if (ctx.L.input_char == 47)
					{
						ctx.NextState = 1;
						return true;
					}
					ctx.NextState = 27;
					return true;
				}
			}
			return true;
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x001A9FB8 File Offset: 0x001A81B8
		private bool GetChar()
		{
			if ((this.input_char = this.NextChar()) != -1)
			{
				return true;
			}
			this.end_of_input = true;
			return false;
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x001A9FE1 File Offset: 0x001A81E1
		private int NextChar()
		{
			if (this.input_buffer != 0)
			{
				int result = this.input_buffer;
				this.input_buffer = 0;
				return result;
			}
			return this.reader.Read();
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x001AA004 File Offset: 0x001A8204
		public bool NextToken()
		{
			this.fsm_context.Return = false;
			while (Lexer.fsm_handler_table[this.state - 1](this.fsm_context))
			{
				if (this.end_of_input)
				{
					return false;
				}
				if (this.fsm_context.Return)
				{
					this.string_value = this.string_buffer.ToString();
					this.string_buffer.Remove(0, this.string_buffer.Length);
					this.token = Lexer.fsm_return_table[this.state - 1];
					if (this.token == 65542)
					{
						this.token = this.input_char;
					}
					this.state = this.fsm_context.NextState;
					return true;
				}
				this.state = this.fsm_context.NextState;
			}
			throw new JsonException(this.input_char);
		}

		// Token: 0x0600543E RID: 21566 RVA: 0x001AA0D9 File Offset: 0x001A82D9
		private void UngetChar()
		{
			this.input_buffer = this.input_char;
		}

		// Token: 0x04006187 RID: 24967
		private static int[] fsm_return_table;

		// Token: 0x04006188 RID: 24968
		private static Lexer.StateHandler[] fsm_handler_table;

		// Token: 0x04006189 RID: 24969
		private bool allow_comments;

		// Token: 0x0400618A RID: 24970
		private bool allow_single_quoted_strings;

		// Token: 0x0400618B RID: 24971
		private bool end_of_input;

		// Token: 0x0400618C RID: 24972
		private FsmContext fsm_context;

		// Token: 0x0400618D RID: 24973
		private int input_buffer;

		// Token: 0x0400618E RID: 24974
		private int input_char;

		// Token: 0x0400618F RID: 24975
		private TextReader reader;

		// Token: 0x04006190 RID: 24976
		private int state;

		// Token: 0x04006191 RID: 24977
		private StringBuilder string_buffer;

		// Token: 0x04006192 RID: 24978
		private string string_value;

		// Token: 0x04006193 RID: 24979
		private int token;

		// Token: 0x04006194 RID: 24980
		private int unichar;

		// Token: 0x02000D69 RID: 3433
		// (Invoke) Token: 0x06005440 RID: 21568
		private delegate bool StateHandler(FsmContext ctx);
	}
}
