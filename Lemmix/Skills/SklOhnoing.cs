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

		public class SklOhnoing : absSkill
		{
			public override string Name => OHNOING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 10, CellW = 16, Cols = 2, Rows = 16, Name = "SPLATTING", Path = "styles/default/lemmings/ohnoer.png", WidthFromCenter = 8, PosYOffset = 0 };
				return _SpriteDef;
			}
		}

		public override int SpriteAnimFrames => 16;


		public override bool Handle(Lemming L)
		{

			if (L.LemEndOfAnimation)
			{
				((absSkill)EXPLODING).Transition(L);
			}

			return true;
		}
	}


}