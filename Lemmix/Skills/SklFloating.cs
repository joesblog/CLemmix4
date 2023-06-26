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

	public class SklFloating : absSkill
	{
		public override string Name => FLOATING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 16, CellW = 16, Cols = 2, Rows = 17, Name = "FLOATING", Path = "styles/default/lemmings/floater.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 17;




		#region cst
		public override bool TryAssign(Lemming L)
		{

			L.LemIsFloater = true;
			return true;

		}

		static int[] FloaterFallTable = new int[] { 3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
		public override bool Handle(Lemming L)
		{
			bool r = true;
			int MaxFallDist = FloaterFallTable[L.LemPhysicsFrame];

			//updraft todo
			int GPMax = Math.Max(FindGroundPixel(L,L.LemX, L.LemY), 0);
			if (MaxFallDist > GPMax)
			{
				//found solid terrain
				L.LemY += GPMax;
				//L.LemActionNext = WALKING;
				L.skillHandler.ActionNext = WALKING;

			}
			else
			{
				L.LemY += MaxFallDist;
			}
			return r;
		}

		#endregion
	}


}