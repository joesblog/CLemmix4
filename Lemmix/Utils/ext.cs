using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix.Utils
{
	public static class ext
	{
		public static bool isList(this PropertyInfo inf)
		{ 
		return inf.PropertyType.IsGenericType && (inf.PropertyType.GetGenericTypeDefinition() == typeof(List<>));

		}

		public static bool innerTypeOf(this PropertyInfo inf, Type t)
		{
			return inf.PropertyType.GetGenericArguments()[0] == t;

		}


		public static bool isNull(this string i) => string.IsNullOrEmpty(i);
		public static bool isNotNull(this string i) => !string.IsNullOrEmpty(i);
		public static bool isNullWS(this string i) => i != null && string.IsNullOrWhiteSpace(i);
		public static bool isNotNullWS(this string i) => i != null && !string.IsNullOrWhiteSpace(i);

		public static string TrimJustWSEnd(this string value)
		{
			return Regex.Replace(value.Trim(), @"[^\S\r\n]+", " ");
		}

		public static bool isUpper(this string i) => Regex.IsMatch(i, @"^[A-Z_]+$");
		public static bool isLower(this string i) => Regex.IsMatch(i, @"^[a-z_]+$");


		public static bool ImplementsGenericInterface (this Type tp, Type interfaceType)   
		{
			bool r = false;

			r = tp.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType);

			return r;
		}

		public static void test(object obj)
		{
			Dictionary<int, string> r = new Dictionary<int, string>();
			if (obj is System.Enum)
			{
				foreach (var i in Enum.GetValues(obj.GetType()))
				{
					int a = (int)i;
					string s = i.ToString();
				}
			}
		}

 
	}


}
