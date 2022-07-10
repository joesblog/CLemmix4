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

	public class SklBuilding : absSkill
	{
		public override string Name => BUILDING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 13, CellW = 16, Cols = 2, Rows = 16, Name = "BUILDING", Path = "styles/default/lemmings/builder.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 16;

		#region cst

		internal override void Transition(Lemming L, bool DoTurn = false)
		{
			base.Transition(L, DoTurn);
			L.LemNumberOfBricksLeft = 12;
			L.LemConstructivePositionFreeze = false;

		}

		public override bool Handle(Lemming L)
		{
			bool r = true;

			if (L.LemPhysicsFrame == 9)
				LayBrick(L);

			else if (L.LemPhysicsFrame == 10 && L.LemNumberOfBricksLeft <= 3)
			{
				//sound effec tbuilder warning
			}
			else if (L.LemPhysicsFrame == 0)
			{
				L.LemNumberOfBricksLeft--;

				if (HasPixelAt(L,L.LemX + L.LemDx, L.LemY - 2))
				{
					((absSkill)WALKING).Transition(L, true);

				}
				else if (
					 HasPixelAt(L,L.LemX + L.LemDx, L.LemY - 3) ||
					 HasPixelAt(L,L.LemX + 2 * L.LemDx, L.LemY - 2) ||
					 (HasPixelAt(L,L.LemX + 2 * L.LemDx, L.LemY - 10) && (L.LemNumberOfBricksLeft > 0)))
				{
					L.LemY--;
					L.LemX += L.LemDx;
					((absSkill)WALKING).Transition(L, true);
					//Transition(L, WALKING, true);
				}
				else
				{
					L.LemY--;
					L.LemX += (2 * L.LemDx);

					if (HasPixelAt(L,L.LemX, L.LemY - 2) || HasPixelAt(L,L.LemX, L.LemY - 3) || HasPixelAt(L,L.LemX + L.LemDx, L.LemY - 3)
						|| (HasPixelAt(L,L.LemX + L.LemDx, L.LemY - 9) && (L.LemNumberOfBricksLeft > 0)))
					{
						((absSkill)WALKING).Transition(L, true);

					}
					else if (L.LemNumberOfBricksLeft == 0)
					{
						((absSkill)SHRUGGING).Transition(L);

					}
				}


			}
			if (L.LemPhysicsFrame == 0) L.LemConstructivePositionFreeze = false;

			return r;

		}

		public static void LayBrick(Lemming L)
		{
			int BrickPosY, n;

			if (L.LemAction == BUILDING)
			{
				BrickPosY = L.LemY - 1;
			}
			else
			{
				BrickPosY = L.LemY;
			}
			/*
			 *   for n := 0 to 5 do
		AddConstructivePixel(L.LemX + n*L.LemDx, BrickPosY, BrickPixelColors[12 - L.LemNumberOfBricksLeft]);*/

			for (n = 0; n < 5; n++)
				AddConstructivePixel(L,L.LemX + n * L.LemDx, BrickPosY, RED);
		}
		#endregion
	}



}