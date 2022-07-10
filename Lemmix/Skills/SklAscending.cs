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

	public class SklAscending : absSkill
	{
		public override string Name => ASCENDING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 10, CellW = 16, Cols = 2, Rows = 1, Name = "ASCENDING", Path = "styles/default/lemmings/ascender.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 1;

		#region cst
		public override bool Handle(Lemming L)
		{
			int dy = 0;
			bool r = true;

			while (dy < 2 && L.LemAscended < 5 && HasPixelAt(L,L.LemX, L.LemY - 1))
			{
				dy++;
				L.LemY--;
				L.LemAscended++;
			}

			if (dy < 2 && !HasPixelAt(L,L.LemX, L.LemY - 1))
			{
				//	L.LemActionNext = WALKING;
				L.skillHandler.ActionNext = WALKING;
			}
			else if ((L.LemAscended == 4 && HasPixelAt(L,L.LemX, L.LemY - 1) && HasPixelAt(L,L.LemX, L.LemY - 2)) ||
							(L.LemAscended >= 5 && HasPixelAt(L,L.LemX, L.LemY - 1)))
			{
				L.LemX -= L.LemDx;
				//Transition(L, FALLING, true);
				((absSkill)FALLING).Transition(L, true);
			}

			return true;
		}
		#endregion
	}


}