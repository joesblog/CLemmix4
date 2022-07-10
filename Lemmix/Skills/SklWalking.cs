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

	public class SklWalking : absSkill
	{
		public override string Name => WALKING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 10, CellW = 16, Cols = 2, Rows = 8, Name = "WALKING", Path = "styles/default/lemmings/walker.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 4;

		#region cst

		public override bool Handle(Lemming L)
		{
			bool r = true;
			int LemDY = 0;

			L.LemX += L.LemDx;
			LemDY = FindGroundPixel(L,L.LemX, L.LemY);

			//handle sliders (ToDo)

			if (LemDY < -6)
			{
				if (L.LemIsClimber)
				{
					((absSkill)CLIMBING).Transition(L); 

				}
				else
				{
					TurnAround(L);
					L.LemX += L.LemDx;
				}
			}
			else if (LemDY < -2)
			{
				((absSkill)ASCENDING).Transition(L);

				L.LemY += -2;
			}
			else if (LemDY < 1)
			{
				L.LemY += LemDY;
			}


			LemDY = FindGroundPixel(L, L.LemX, L.LemY);
			if (LemDY > 3)
			{
				L.LemY += 4;
				((absSkill)FALLING).Transition(L);

			}
			else if (LemDY > 0)
			{
				L.LemY += LemDY;
			}


			return r;

		}
		#endregion
	}


}