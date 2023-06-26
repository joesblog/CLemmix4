using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CLemmix4.Lemmix.Utils.Token;

namespace CLemmix4.Lemmix.Utils
{
	public class Tokenizer
	{
		public delegate void evTokenizer(int row, int col, string msg);
		public event evTokenizer onErr;

		public List<Token> tokens = new List<Token>();
		public Tokenizer(string source)
		{
			Source = source;
			Lines = Source.TrimEnd().Split('\n').Select(o => o.TrimEnd() + "\n").ToList();
		}

		public string Source { get; set; }
		public List<string> Lines { get; set; }

		public string line { get; set; }
		public string fullline { get; set; }

		public enmTokenKind lastKind = enmTokenKind.EOL;
		public int col = 0; public int row = 0;


		public List<Token> tokenize()
		{
			tokens = new List<Token>();
			row = 0;
			col = 0;
			lastKind = enmTokenKind.EOL;
			eatTokens();

			secondPass();

			return tokens;
		}

		private void secondPass()
		{

			for (int i = 0; i < tokens.Count(); i++)
			{
				Token p = tokens.ElementAtOrDefault(i - 1);
				Token c = tokens.ElementAtOrDefault(i);
				Token n = tokens.ElementAtOrDefault(i+1);

				if (c.tKind.HasFlag( enmTokenKind.PROPERTYID) && (n != null && n.tKind.HasFlag( enmTokenKind.EOL)))
					c.tKind |= enmTokenKind.FLAGON;
			}

		}

		private void eatTokens()
		{
			lastKind = enmTokenKind.EOL;
			while (true)
			{
				Token token = eatGoodToken();
				if (token == null) continue;
				tokens.Add(token);
				lastKind = token.tKind;
				if (token.tKind == enmTokenKind.EOF) break;
			}
		}

		private Token eatGoodToken()
		{
			Token tk = eatToken();
			if (tk != null)
			{
				var kind = tk.tKind;
				if (kind == enmTokenKind.WS) return null;
				//if (kind == enmTokenKind.EOL) return null;

			}
			return tk;

		}
		static Regex rgMatchUPPERCASE = new Regex(@"^([\$_A-Z]+)$");
		static Regex rgMatchData = new Regex(@"([^\s]+)");
		static Regex rgMatchNumber = new Regex(@"^[0-9]+$");
		static Regex rgMatchNumberWithModsPrefix = new Regex(@"^([\-\+]){1}([0-9]+)([\%]){0,}$");
		static Regex rgMatchNumberWithModsSuffix = new Regex(@"^([0-9]+)([\%]){1}$");

		private Token eatToken()
		{
			string single = eatChar();
			string pair = single;
			if (line != "") pair += line[0];
			if (single == "") return eatEndOfFile();
			if (single == "\n") return eatEndOfLine(single);
			if (single == " ") return eatWhiteSpace(single);
			if (single == "\t") return eatWhiteSpace(single);
			if (single == "$") return eatObjectMark(single);
			if (single == "#") return eatComment(single);
			if (rgMatchData.IsMatch(pair))
			{
				return eatData(single);
			}

			return new TokenError(this, row, col, enmTokenKind.UNKNOWN, single);

		}

		private Token eatComment(string single)
		{
			var tk = new Token(this, row, col, enmTokenKind.COMMENT);
			string c = single;

			while (true)
			{
				c = eatChar();
				tk.value += c;
				if (c == "\n") break;
			}
			return tk;
		}

		private Token eatObjectMark(string c)
		{
			string s = c;
			var tk = new Token(this, row, col, enmTokenKind.UNKNOWN);
			while (line != "")
			{
				if (rgMatchData.IsMatch($"{line[0]}")) s += eatChar();
				else break;
			}
			tk.value = s;
			if (tk.value.ToUpper() == "$END")
			{
				tk.tKind = enmTokenKind.OBJECT_CLOSE;
			}
			else
			{
				tk.tKind = enmTokenKind.OBJECT_OPEN;
			}

			return tk;
		}

		private Token eatData(string c)
		{
			string s = c;
			var tk = new Token(this, row, col, enmTokenKind.DATA);

			while (line != "")
			{
				if (rgMatchData.IsMatch($"{line[0]}")) s += eatChar();
				else break;
			}

			//gobble the white space afterwards
			{
				Regex ws = new Regex(@"[\s\t]");
				int lp = 0;
				string tline = line;
				while (tline != null && tline != "" && ws.IsMatch($"{tline[0]}"))
				{
					if (tline == "" || tline == null) break;
					if (tline[0] != '\n')
						tk.whitespaceafter += tline[0];

					tline = tline.Substring(1);
				}

			}
			tk.value = s;

			if (rgMatchUPPERCASE.IsMatch(tk.value))
			{
				tk.tKind |= enmTokenKind.UPPERCASE;
			}

			if (fullline.Trim().StartsWith(tk.value))
			{
				tk.tKind |= enmTokenKind.SOL;
			}

			if (tk.tKind.HasFlag(enmTokenKind.UPPERCASE) && tk.tKind.HasFlag(enmTokenKind.SOL))
			{
				tk.tKind = enmTokenKind.DATA | enmTokenKind.PROPERTYID;
			}


			
			if (rgMatchNumber.IsMatch(tk.value))
			{
				tk.tKind |= enmTokenKind.NUMBER;
			}

			if (rgMatchNumberWithModsPrefix.IsMatch(tk.value) || rgMatchNumberWithModsSuffix.IsMatch(tk.value))
			{
				tk.tKind |= enmTokenKind.NUMBER_WITH_MODS;
			}
			
	

			return tk;

		}

		private Token eatWhiteSpace(string c)
		{
			string s = c;
			var tk = new Token(this, row, col, enmTokenKind.WS);
			while (line[0] == ' ') s += eatChar();
			tk.value = s;
			return tk;
		}

		private Token eatEndOfLine(string c)
		{

			var tk = new Token(this, row, col, enmTokenKind.EOL);

			var lst = tokens.LastOrDefault();
			if (lst != null && lst.tKind.HasFlag(enmTokenKind.EOL)) return null;

			return tk;

		}

		private Token eatEndOfFile()
		{
			return new Token(this, row, col, enmTokenKind.EOF);



		}

		public void eatLine()
		{
			if (Lines.Count == 0) return;

			line = Lines.FirstOrDefault();
			fullline = line;
			Lines.RemoveAt(0);
			row++;
			col = 0;


		}
		private string eatChar()
		{
			if (line.isNull()) eatLine();
			if (line.isNull()) return "";
			col += 1;
			if (line == "") { line = null; return "\n"; }

			var c = line[0];
			line = line.Substring(1);
			if (c == '\n') return "\n";
			else if (c < ' ')
			{
				onErr?.Invoke(row, col, $"Invalid char:{c}");
			}
			return new string(new char[] { c });
		}
	}

	public class Token
	{
		public static int tkId = 0;

		public static object tkLock = new object();
		public Token(Tokenizer tkz, int row, int col, enmTokenKind tkind = enmTokenKind.UNKNOWN, string value = "")
		{
			lock (tkLock)
			{
				this.tokenizer = tkz;
				this.row = row;
				this.col = col;
				this.tKind = tkind;
				this.kind = $"{this.tKind}";
				this.value = value;
				this.ID = ++tkId;
			}
		}
		[Flags]
		public enum enmTokenKind
		{
			UNKNOWN = 1 << 0, NAME = 1 << 1, NUMBER = 1 << 2, OBJECT_OPEN = 1 << 3, OBJECT_CLOSE = 1 << 4,
			EOL = 1 << 5, EOF = 1 << 6, WS = 1 << 7, DATA = 1 << 8, NUMBER_PREFIX = 1 << 9,
			NUMBER_SUFFIX = 1 << 10, SOL = 1 << 11, UPPERCASE = 1 << 12, PROPERTYID = 1 << 13, FLAGON = 1<<14, COMMENT = 1<<15, NUMBER_WITH_MODS = 1<<16

		}

		public Tokenizer tokenizer { get; }
		public int row { get; set; }
		public int col { get; set; }

		public string whitespaceafter { get; set; } = "";
		public string kind { get; private set; }

		private enmTokenKind _tkind;
		public enmTokenKind tKind
		{
			get
			{

				return _tkind;
			}
			set
			{
				_tkind = value;
				kind = $"{_tkind}";
			}
		}
		public string value { get; set; }
		public int ID { get; }



	}


	public class TokenError : Token
	{
		public string errorType = "UNKOWN";
		public TokenError(Tokenizer tkz, int row, int col, enmTokenKind kind, string value = "", string err = "UNKOWN") : base(tkz, row, col, kind, value)
		{
			errorType = err;
		}
	}


	public class TKStreamNoEOL : TKStream
	{
		public TKStreamNoEOL(List<Token> tks) : base()
		{
			
			tokens = tks.Where(o => o.tKind != enmTokenKind.EOL).ToList();
			//var tkTemp = tks.Where(o => o.kind != EOL).ToList();

			tokenCount = tokens.Count();

		}

	}

	public class TKStream
	{
		public int pos = 0;
		public List<Token> tokens { get; internal set; }
		public string name { get; set; }
		internal int tokenCount = -1;

		public Stack<int> lastPositions { get; set; } = new Stack<int>();
		internal bool stackPushAllowed = true;
		internal void pushToStack(int? npos = null)
		{
			if (stackPushAllowed)
				if (npos.HasValue)
					lastPositions.Push(npos.Value);
				else lastPositions.Push(pos);
		}
		public TKStream() { }
		public TKStream(List<Token> tks)
		{
			tokens = tks;
			tokenCount = tokens.Count();
		}
		public Token this[int i]
		{
			get
			{
				if (i < tokenCount && i >=0 )
					return tokens[i];
				else return null;
			}
		}

		public Token Last
		{
			get
			{
				return tokens.Last();
			}
		}


		/// <summary>
		/// get the row,col of the current token
		/// </summary>
		/// <returns></returns>
		public Tuple<int, int> getInnerTokenPos()
		{
			if (peek() != null)
				return Tuple.Create(peek().row, peek().col);
			else return null;
		}

		public string ToKindString() => String.Join("\n", tokens.Select(o => o.kind));
		public string ToValueString() => String.Join("\n", tokens.Select(o => o.value));
		public string ToKindValueString()
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < tokens.Count(); i++)
			{

				var c = tokens[i];
				bool isPos = i == pos;
				string strpos = isPos ? $"{i:00000}=>" : $"{i:00000}  ";
				sb.AppendLine($"{strpos}\t{c.tKind}\t{c.value}");
				sb.AppendLine("--------------------------------------");
			}

			return sb.ToString();

		}

		public string ToKindValueHTML()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<html><body><table style='width:100%;' border='1'><thead><tr><th>Pos</th><th>Kind</th><th>Value</th></tr></thead><tbody>");

			for (int i = 0; i < tokens.Count(); i++)
			{

				var c = tokens[i];
				bool isPos = i == pos;
				string rowstyle = "";
				if (isPos) rowstyle = "style='background-color:blue; color:white;";

				sb.AppendLine($"<tr {rowstyle} ><td>{pos}</td><td>{c.kind}</td><td>{c.value}</td><?tr>");


			}

			sb.Append("</tbody></table>");

			return sb.ToString();
		}
		// => String.Join("\n", tokens.Select(o => $"{o.kind}\t{o.value}"));


		/// <summary>
		/// peek at a position, but do not advance the stream
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public Token peek(int amount = 0)
		{
			int peekToGo = pos + amount;
			if (peekToGo < tokenCount && peekToGo >= 0)
			{
				return tokens[pos + amount];
			}

			else return null;
		}


		/// <summary>
		/// pop the stack and go back one
		/// </summary>
		/// <returns></returns>
		public Token pop()
		{

			if (lastPositions.Count > 0)
			{
				pos = lastPositions.Pop();
				return tokens[pos];
			}
			return null;

		}

		/// <summary>
		/// advance the stream by one and return the result
		/// </summary>
		/// <returns></returns>
		public Token read()
		{
			if (pos + 1 < tokenCount)
			{
				pushToStack(pos);
				return tokens[++pos];
			}

			else return null;
		}

		/// <summary>
		/// go back one on the streammm and return the token
		/// </summary>
		/// <returns></returns>
		public Token rw()
		{
			if (pos - 1 >= 0)
			{
				pushToStack(pos);
				return tokens[--pos];
			}
			else return null;
		}
		/// <summary>
		/// skip {amount} and return the token
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public Token skip(int amount)
		{
			int posToGo = pos + amount;
			if (posToGo < tokenCount && posToGo >= 0)
			{
				pushToStack(pos);
				pos = posToGo;
				return tokens[pos];
			}
			return null;
		}


		/// <summary>
		/// grab {amount} of tokens from current position
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public Token[] ReadGrab(int amount)
		{
			Token[] r = new Token[amount];

			for (int i = 0; i < amount; i++)
			{
				r[i] = read();
			}
			return r;
		}

		public Token[] PeekGrab(int amount)
		{
			if (amount < 0) return null;
			Token[] r = new Token[amount];

			for (int i = 0; i < amount; i++)
			{
				r[i] = peek(i);
			}
			return r;
		}

		public List<Token> PeekGrabRange(int start, int end)
		{
			List<Token> r = new List<Token>();
			if (end > start)
			{

				for (int i = start; i < end; i++)
				{
					r.Add(this[i]);
				}
				return r;
			}

			return null;
		}
		public List<Token> ReadGrabRange(int start, int end)
		{
			List<Token> r = new List<Token>();
			if (end > start)
			{

				for (int i = start; i < end; i++)
				{
					r.Add(this[i]);
				}
				this.pos = end;
				return r;
			}

			return null;
		}

		/// <summary>
		/// goto a position in the stream and return the token
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Token Jump(int i)
		{
			if (i >= 0 && i < tokenCount)
			{
				pushToStack(pos);
				pos = i;
				return tokens[pos];
			}

			else return null;
		}

		public Token Jump(Token t)
		{
			var tk = tokens.IndexOf(t);
			if (tk > -1)
			{
				pushToStack(tk);
				pos = tk;
				return tokens[pos];
			}
			else return null;


		}

		/// <summary>
		/// hunt a token from the current position of kind
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="tc">token to be set</param>
		/// <returns>position</returns>
		public int? huntKind(enmTokenKind kind, out Token tc)
		{
			pushToStack(pos);
			stackPushAllowed = false;
			Token c = peek();
			int? ix = null;
			tc = null;
			while (!(c = peek()).tKind.HasFlag(kind))
			{
				read();
				ix = pos;
			}
			if (c.tKind.HasFlag(kind))
			{
				tc = c;
				read();
			}
			stackPushAllowed = true;
			return ix;
		}

		/// <summary>
		/// hunt a token from the current position of kind
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="tc">token to be set</param>
		/// <returns>position</returns>
		public int? huntValue(string Value, out Token tc)
		{
			pushToStack(pos);
			stackPushAllowed = false;
			Token c = peek();
			int? ix = null;
			tc = null;
			while ((c = peek()).value == Value)
			{
				read();
				ix = pos;
			}
			if (c.value == Value)
			{
				tc = c;
				read();
			}
			stackPushAllowed = true;
			return ix;
		}



		public Tuple<Stack<int>, int> dumpPositions()
		{
			return new Tuple<Stack<int>, int>(lastPositions, pos);
		}

		public void restorePositions(Tuple<Stack<int>, int> dump)
		{
			lastPositions = dump.Item1;
			pos = dump.Item2;
		}









	}
}
