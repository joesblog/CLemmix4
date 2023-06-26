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

	public class SklHoisting : absSkill
	{
		public override string Name => HOISTING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 12, CellW = 16, Cols = 2, Rows = 8, Name = "HOISTING", Path = "styles/default/lemmings/hoister.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 8;

		#region cst

		public override bool Handle(Lemming L)
		{

			bool r = true;
			if (L.LemEndOfAnimation)
			{
				((absSkill)WALKING).Transition(L);
			}
			else if (L.LemPhysicsFrame == 1 && L.LemIsStartingAction)
			{
				L.LemY -= 1;
			}
			else if (L.LemPhysicsFrame <= 4)
			{
				L.LemY -= 2;
			}
			return r;

		}
		#endregion
	}

	


}