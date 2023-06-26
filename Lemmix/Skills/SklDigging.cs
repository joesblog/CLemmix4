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

	public class SklDigging : absSkill
	{
		public override string Name => DIGGING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 14, CellW = 16, Cols = 2, Rows = 16, Name = "DIGGING", Path = "styles/default/lemmings/digger.png", WidthFromCenter = 8, PosYOffset = 3 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 16;

		#region cst
		private bool DigOneRow(Lemming L, int PosX, int PosY)
		{

			int n;
			bool Result = false;
			for (n = -4; n < 4; n++)
			{
				if (HasPixelAt(L,PosX + n, PosY)) //& !HasIndestructibleAt(PosX + n, PosY, 0, baDigging))
				{
					RemovePixelAt(L,PosX + n, PosY);

					if ((n > -4) & (n < 4)) Result = true;
				}

				if (!L.pm.lemHandler.IsSimulating)
				{
					RemoveTerrain(L,new Rectangle(PosX - 4, PosY, 9, 1));
				}
			}
			return Result;
		}


		public override bool Handle(Lemming L)
		{
			bool continueWork = false;
			bool r = true;
			if (L.LemIsStartingAction)
			{
				L.LemIsStartingAction = false;
				DigOneRow(L, L.LemX, L.LemY - 1);
				L.LemPhysicsFrame--;
			}

			if (L.LemPhysicsFrame.In(0, 8))
			{
				L.LemY++;
				continueWork = DigOneRow(L, L.LemX, L.LemY - 1);
				//indestructable check

				if (!continueWork)
					((absSkill)FALLING).Transition(L);
			}
			return r;
		}

 
		#endregion
	}


}