using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Raylib_CsLo;
using CLemmix4.Lemmix.Gadget;
using CLemmix4.Lemmix.Utils;
using CLemmix4.Lemmix.Core;
using static Raylib_CsLo.Raylib;
using static CLemmix4.RaylibMethods;
using static CLemmix4.Lemmix.Utils.Common;
using static CLemmix4.Lemmix.Skills.skillNameHolders;

namespace CLemmix4.Lemmix.Skills
{

		public class SklReleaseslower : absSkill
		{
			public override string Name => RELEASESLOWER;

			public  SpriteDefinition _SpriteDef;
public override SpriteDefinition SpriteDef 
{

get {
if (_SpriteDef == null)
	_SpriteDef = new SpriteDefinition(){ CellH = 48, CellW = 32, Cols =1, Rows = 1, Name="RELEASESLOWER", Path="gfx/btnMinus.png",  WidthFromCenter = 8 };
return _SpriteDef;} 
}
			
		}


}