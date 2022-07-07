using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLemmix4.Lemmix.Utils
{
	public class EnumFlagger<T> where T : System.Enum
	{

		Dictionary<int, bool> flags;
		


		public EnumFlagger(T enm)
		{
			foreach (var i in Enum.GetValues(typeof(T)))
			{
				flags.Add((int)i, false);
			}
		}

	 

	}

	public class test
	{

		public void dotest()
		{
			EnumFlagger<CLemmix4.Lemmix.Core.Lemming.enmLemmingState> m = new EnumFlagger<Core.Lemming.enmLemmingState>(Core.Lemming.enmLemmingState.None);
			 
	 
		}
	}
}
