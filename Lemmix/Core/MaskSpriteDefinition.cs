//using Raylib_cs;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/

namespace CLemmix4.Lemmix.Core
{
	public class MaskSpriteDefinition : SpriteDefinition
	{
		public int[] mask;
		public bool maskSetup = false;
		public override void initCheck()
		{
			base.initCheck();
			if (!maskSetup)
			{//Todo
				maskSetup = true;
			}
		}
	}

 

}
