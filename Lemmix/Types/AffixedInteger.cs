using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix.Types
{
	public class AffixedInteger
	{
		public enum enmPrefix { NONE, MINUS, PLUS}
		public enum enmmSuffix { NONE, PERCENTAGE}

		public enmPrefix prefix = enmPrefix.NONE;
		public enmmSuffix suffix = enmmSuffix.NONE;

		public int Value = 0;

		public AffixedInteger(enmPrefix prefix, enmmSuffix suffix, int value)
		{
			this.prefix = prefix;
			this.suffix = suffix;
			Value = value;
		}

		public AffixedInteger(enmPrefix prefix, int value)
		{
			this.prefix = prefix;
			Value = value;
		}

		public AffixedInteger(enmmSuffix suffix, int value)
		{
			this.suffix = suffix;
			Value = value;
		}

		public AffixedInteger(int value)
		{
			Value = value;
		}


		static Regex rgPrefix = new Regex(@"^(?<P>[\-\+])(?<N>\d+)?$");
		static Regex rgPrefixSuffix = new Regex(@"^(?<P>[\-\+])(?<N>\d+)(?<S>[\%])$");
		static Regex rgSuffix = new Regex(@"^(?<N>\d+)(?<S>[\%])$");
		static Regex rgNumOnly = new Regex(@"^(?<N>\d+)$");
		public AffixedInteger(Utils.Token t)
		{

			string c = t.value;

			Match m;
			if (rgPrefixSuffix.IsMatch(c))
			{
				m = rgPrefixSuffix.Match(c);

				int.TryParse(m.Groups["N"].Value, out Value);
				prefix = choosePrefix(m.Groups["P"].Value);
				suffix = chooseSuffix(m.Groups["S"].Value);
			}
			else if (rgPrefix.IsMatch(c))
			{
				m = rgPrefix.Match(c);

				int.TryParse(m.Groups["N"].Value, out Value);
				prefix = choosePrefix(m.Groups["P"].Value);

			}
			else if (rgSuffix.IsMatch(c))
			{
				m = rgSuffix.Match(c);

				int.TryParse(m.Groups["N"].Value, out Value);
				suffix = chooseSuffix(m.Groups["S"].Value);

			}
			else if (rgNumOnly.IsMatch(c))
			{ 
				m = rgNumOnly.Match(c);

				int.TryParse(m.Groups["N"].Value, out Value);

			}

		}

		static enmmSuffix chooseSuffix(string v)
		{
			switch (v)
			{
				case "%": return enmmSuffix.PERCENTAGE;
				default: return enmmSuffix.NONE;
			}
		}

		static enmPrefix choosePrefix(string v)
		{
			switch (v)
			{
				case "-": return enmPrefix.MINUS;
				case "+": return enmPrefix.PLUS;
				default:return enmPrefix.NONE;
			}
		}

	}
}
