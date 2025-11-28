using System;
using System.Collections.Generic;
using System.IO;

namespace LitJson
{
	// Token: 0x02000D63 RID: 3427
	public class JsonReader
	{
		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x060053DB RID: 21467 RVA: 0x001A7DF4 File Offset: 0x001A5FF4
		// (set) Token: 0x060053DC RID: 21468 RVA: 0x001A7E01 File Offset: 0x001A6001
		public bool AllowComments
		{
			get
			{
				return this.lexer.AllowComments;
			}
			set
			{
				this.lexer.AllowComments = value;
			}
		}

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x060053DD RID: 21469 RVA: 0x001A7E0F File Offset: 0x001A600F
		// (set) Token: 0x060053DE RID: 21470 RVA: 0x001A7E1C File Offset: 0x001A601C
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.lexer.AllowSingleQuotedStrings;
			}
			set
			{
				this.lexer.AllowSingleQuotedStrings = value;
			}
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x060053DF RID: 21471 RVA: 0x001A7E2A File Offset: 0x001A602A
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x060053E0 RID: 21472 RVA: 0x001A7E32 File Offset: 0x001A6032
		public bool EndOfJson
		{
			get
			{
				return this.end_of_json;
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x060053E1 RID: 21473 RVA: 0x001A7E3A File Offset: 0x001A603A
		public JsonToken Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x060053E2 RID: 21474 RVA: 0x001A7E42 File Offset: 0x001A6042
		public object Value
		{
			get
			{
				return this.token_value;
			}
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x001A7E4A File Offset: 0x001A604A
		static JsonReader()
		{
			JsonReader.PopulateParseTable();
		}

		// Token: 0x060053E4 RID: 21476 RVA: 0x001A7E51 File Offset: 0x001A6051
		public JsonReader(string json_text) : this(new StringReader(json_text), true)
		{
		}

		// Token: 0x060053E5 RID: 21477 RVA: 0x001A7E60 File Offset: 0x001A6060
		public JsonReader(TextReader reader) : this(reader, false)
		{
		}

		// Token: 0x060053E6 RID: 21478 RVA: 0x001A7E6C File Offset: 0x001A606C
		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.read_started = false;
			this.automaton_stack = new Stack<int>();
			this.automaton_stack.Push(65553);
			this.automaton_stack.Push(65543);
			this.lexer = new Lexer(reader);
			this.end_of_input = false;
			this.end_of_json = false;
			this.reader = reader;
			this.reader_is_owned = owned;
		}

		// Token: 0x060053E7 RID: 21479 RVA: 0x001A7EF8 File Offset: 0x001A60F8
		private static void PopulateParseTable()
		{
			JsonReader.parse_table = new Dictionary<int, IDictionary<int, int[]>>();
			JsonReader.TableAddRow(ParserToken.Array);
			JsonReader.TableAddCol(ParserToken.Array, 91, new int[]
			{
				91,
				65549
			});
			JsonReader.TableAddRow(ParserToken.ArrayPrime);
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 34, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 91, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 93, new int[]
			{
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 123, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65537, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65538, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65539, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65540, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddRow(ParserToken.Object);
			JsonReader.TableAddCol(ParserToken.Object, 123, new int[]
			{
				123,
				65545
			});
			JsonReader.TableAddRow(ParserToken.ObjectPrime);
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 34, new int[]
			{
				65546,
				65547,
				125
			});
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 125, new int[]
			{
				125
			});
			JsonReader.TableAddRow(ParserToken.Pair);
			JsonReader.TableAddCol(ParserToken.Pair, 34, new int[]
			{
				65552,
				58,
				65550
			});
			JsonReader.TableAddRow(ParserToken.PairRest);
			JsonReader.TableAddCol(ParserToken.PairRest, 44, new int[]
			{
				44,
				65546,
				65547
			});
			JsonReader.TableAddCol(ParserToken.PairRest, 125, new int[]
			{
				65554
			});
			JsonReader.TableAddRow(ParserToken.String);
			JsonReader.TableAddCol(ParserToken.String, 34, new int[]
			{
				34,
				65541,
				34
			});
			JsonReader.TableAddRow(ParserToken.Text);
			JsonReader.TableAddCol(ParserToken.Text, 91, new int[]
			{
				65548
			});
			JsonReader.TableAddCol(ParserToken.Text, 123, new int[]
			{
				65544
			});
			JsonReader.TableAddRow(ParserToken.Value);
			JsonReader.TableAddCol(ParserToken.Value, 34, new int[]
			{
				65552
			});
			JsonReader.TableAddCol(ParserToken.Value, 91, new int[]
			{
				65548
			});
			JsonReader.TableAddCol(ParserToken.Value, 123, new int[]
			{
				65544
			});
			JsonReader.TableAddCol(ParserToken.Value, 65537, new int[]
			{
				65537
			});
			JsonReader.TableAddCol(ParserToken.Value, 65538, new int[]
			{
				65538
			});
			JsonReader.TableAddCol(ParserToken.Value, 65539, new int[]
			{
				65539
			});
			JsonReader.TableAddCol(ParserToken.Value, 65540, new int[]
			{
				65540
			});
			JsonReader.TableAddRow(ParserToken.ValueRest);
			JsonReader.TableAddCol(ParserToken.ValueRest, 44, new int[]
			{
				44,
				65550,
				65551
			});
			JsonReader.TableAddCol(ParserToken.ValueRest, 93, new int[]
			{
				65554
			});
		}

		// Token: 0x060053E8 RID: 21480 RVA: 0x001A8271 File Offset: 0x001A6471
		private static void TableAddCol(ParserToken row, int col, params int[] symbols)
		{
			JsonReader.parse_table[(int)row].Add(col, symbols);
		}

		// Token: 0x060053E9 RID: 21481 RVA: 0x001A8285 File Offset: 0x001A6485
		private static void TableAddRow(ParserToken rule)
		{
			JsonReader.parse_table.Add((int)rule, new Dictionary<int, int[]>());
		}

		// Token: 0x060053EA RID: 21482 RVA: 0x001A8298 File Offset: 0x001A6498
		private void ProcessNumber(string number)
		{
			double num;
			if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, ref num))
			{
				this.token = JsonToken.Double;
				this.token_value = num;
				return;
			}
			int num2;
			if (int.TryParse(number, ref num2))
			{
				this.token = JsonToken.Int;
				this.token_value = num2;
				return;
			}
			long num3;
			if (long.TryParse(number, ref num3))
			{
				this.token = JsonToken.Long;
				this.token_value = num3;
				return;
			}
			this.token = JsonToken.Int;
			this.token_value = 0;
		}

		// Token: 0x060053EB RID: 21483 RVA: 0x001A8334 File Offset: 0x001A6534
		private void ProcessSymbol()
		{
			if (this.current_symbol == 91)
			{
				this.token = JsonToken.ArrayStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 93)
			{
				this.token = JsonToken.ArrayEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 123)
			{
				this.token = JsonToken.ObjectStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 125)
			{
				this.token = JsonToken.ObjectEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 34)
			{
				if (this.parser_in_string)
				{
					this.parser_in_string = false;
					this.parser_return = true;
					return;
				}
				if (this.token == JsonToken.None)
				{
					this.token = JsonToken.String;
				}
				this.parser_in_string = true;
				return;
			}
			else
			{
				if (this.current_symbol == 65541)
				{
					this.token_value = this.lexer.StringValue;
					return;
				}
				if (this.current_symbol == 65539)
				{
					this.token = JsonToken.Boolean;
					this.token_value = false;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65540)
				{
					this.token = JsonToken.Null;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65537)
				{
					this.ProcessNumber(this.lexer.StringValue);
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65546)
				{
					this.token = JsonToken.PropertyName;
					return;
				}
				if (this.current_symbol == 65538)
				{
					this.token = JsonToken.Boolean;
					this.token_value = true;
					this.parser_return = true;
				}
				return;
			}
		}

		// Token: 0x060053EC RID: 21484 RVA: 0x001A84A6 File Offset: 0x001A66A6
		private bool ReadToken()
		{
			if (this.end_of_input)
			{
				return false;
			}
			this.lexer.NextToken();
			if (this.lexer.EndOfInput)
			{
				this.Close();
				return false;
			}
			this.current_input = this.lexer.Token;
			return true;
		}

		// Token: 0x060053ED RID: 21485 RVA: 0x001A84E5 File Offset: 0x001A66E5
		public void Close()
		{
			if (this.end_of_input)
			{
				return;
			}
			this.end_of_input = true;
			this.end_of_json = true;
			if (this.reader_is_owned)
			{
				this.reader.Close();
			}
			this.reader = null;
		}

		// Token: 0x060053EE RID: 21486 RVA: 0x001A8518 File Offset: 0x001A6718
		public bool Read()
		{
			if (this.end_of_input)
			{
				return false;
			}
			if (this.end_of_json)
			{
				this.end_of_json = false;
				this.automaton_stack.Clear();
				this.automaton_stack.Push(65553);
				this.automaton_stack.Push(65543);
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.token = JsonToken.None;
			this.token_value = null;
			if (!this.read_started)
			{
				this.read_started = true;
				if (!this.ReadToken())
				{
					return false;
				}
			}
			while (!this.parser_return)
			{
				this.current_symbol = this.automaton_stack.Pop();
				this.ProcessSymbol();
				if (this.current_symbol == this.current_input)
				{
					if (!this.ReadToken())
					{
						if (this.automaton_stack.Peek() != 65553)
						{
							throw new JsonException("Input doesn't evaluate to proper JSON text");
						}
						return this.parser_return;
					}
				}
				else
				{
					int[] array;
					try
					{
						array = JsonReader.parse_table[this.current_symbol][this.current_input];
					}
					catch (KeyNotFoundException inner_exception)
					{
						throw new JsonException((ParserToken)this.current_input, inner_exception);
					}
					if (array[0] != 65554)
					{
						for (int i = array.Length - 1; i >= 0; i--)
						{
							this.automaton_stack.Push(array[i]);
						}
					}
				}
			}
			if (this.automaton_stack.Peek() == 65553)
			{
				this.end_of_json = true;
			}
			return true;
		}

		// Token: 0x0400615F RID: 24927
		private static IDictionary<int, IDictionary<int, int[]>> parse_table;

		// Token: 0x04006160 RID: 24928
		private Stack<int> automaton_stack;

		// Token: 0x04006161 RID: 24929
		private int current_input;

		// Token: 0x04006162 RID: 24930
		private int current_symbol;

		// Token: 0x04006163 RID: 24931
		private bool end_of_json;

		// Token: 0x04006164 RID: 24932
		private bool end_of_input;

		// Token: 0x04006165 RID: 24933
		private Lexer lexer;

		// Token: 0x04006166 RID: 24934
		private bool parser_in_string;

		// Token: 0x04006167 RID: 24935
		private bool parser_return;

		// Token: 0x04006168 RID: 24936
		private bool read_started;

		// Token: 0x04006169 RID: 24937
		private TextReader reader;

		// Token: 0x0400616A RID: 24938
		private bool reader_is_owned;

		// Token: 0x0400616B RID: 24939
		private object token_value;

		// Token: 0x0400616C RID: 24940
		private JsonToken token;
	}
}
