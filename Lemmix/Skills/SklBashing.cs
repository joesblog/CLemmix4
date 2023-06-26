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

	public class SklBashing : absSkill
	{
		public override string Name => BASHING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 10, CellW = 16, Cols = 2, Rows = 32, Name = "BASHING", Path = "styles/default/lemmings/basher.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 16;

		#region cst

		private SpriteDefinition _SpriteDefMask;

		

		public SpriteDefinition SpriteDefMask
		{
			get {

				if (_SpriteDefMask == null)
					_SpriteDefMask = new SpriteDefinition() { CellH = 10, CellW = 16, Cols = 2, Rows = 4, Name = "BASHER", Path = "styles/default/mask/basher.png", WidthFromCenter = 8 };
				return _SpriteDefMask;
			}
		}
		bool BasherIndestructableCheck(int x, int y, int direction)
		{
			//ToDo implement indestructable check
			return false;
		}

		void BasherTurn(Lemming L, bool SteelSound)
		{
			L.LemX -= L.LemDx;
			//Transition(L, WALKING, true);
			((absSkill)WALKING).Transition(L, true);
			//cue sound effect
		}

		bool BasherStepUpCheck(Lemming L, int x, int y, int Direction, int Step)
		{
			bool Result = true;
			if (Step == -1)
			{
				if (((!HasPixelAt(L,x + Direction, y + Step - 1))
			 && HasPixelAt(L,x + Direction, y + Step)
			 && HasPixelAt(L,x + 2 * Direction, y + Step)
			 && HasPixelAt(L,x + 2 * Direction, y + Step - 1)
			 && HasPixelAt(L,x + 2 * Direction, y + Step - 2))
		 ) Result = false;

				if (((!HasPixelAt(L,x + Direction, y + Step - 2))
			 && HasPixelAt(L,x + Direction, y + Step)
			 && HasPixelAt(L,x + Direction, y + Step - 1)
			 && HasPixelAt(L,x + 2 * Direction, y + Step - 1)
			 && HasPixelAt(L,x + 2 * Direction, y + Step - 2)
		 )) Result = false;


				if ((HasPixelAt(L,x + Direction, y + Step - 2)
			 && HasPixelAt(L,x + Direction, y + Step - 1)
			 && HasPixelAt(L,x + Direction, y + Step)
		 )) Result = false;
			}
			else if (Step == -2)
			{
				if (((!HasPixelAt(L,x + Direction, y + Step))
			 && HasPixelAt(L,x + Direction, y + Step + 1)
			 && HasPixelAt(L,x + 2 * Direction, y + Step + 1)
			 && HasPixelAt(L,x + 2 * Direction, y + Step)
			 && HasPixelAt(L,x + 2 * Direction, y + Step - 1)
		 )) Result = false;

				if ((!HasPixelAt(L,x + Direction, y + Step - 1))
			 && HasPixelAt(L,x + Direction, y + Step)
			 && HasPixelAt(L,x + 2 * Direction, y + Step)
			 && HasPixelAt(L,x + 2 * Direction, y + Step - 1)
		 ) Result = false;

				if (HasPixelAt(L,x + Direction, y + Step - 1)
			 && HasPixelAt(L,x + Direction, y + Step))
				{

				}
				else
				{
					Result = false;
				}
			}

			return Result;
		}

		public unsafe void ApplyBashingMask(Lemming L, int maskFrame)
		{
			Rectangle S;
			Rectangle D;

			S = new Rectangle(0, 0, 16, 10);
			S.x = L.LemDx == 1 ? 16 : 0;
			S.y = maskFrame * 10;


			D = new Rectangle();
			D.x = L.LemX - 8;
			D.y = L.LemY - 10;
			D.width = 16;
			D.height = 10;


			if (!S.CheckRectCopy(D))
			{
				//throw new Exception("rect does not match");
			}
			L.dbgString = $"{maskFrame}";
			string maskname = "BASHER";
 
			var msd = SpriteDefMask;
			msd.initCheck();

			ImageDrawCS_ApplyAlphaMask(ref L.pm.imgLevel, msd.imgSprite, S, D, BLANK);
			ImageDrawCS_ApplyAlphaMask(ref L.pm.imgPhysics, msd.imgSprite, S, D, BLANK);

			var rem = new Rectangle();
			if (L.LemDx == 1)
			{
				rem = new Rectangle(L.LemX - 4, L.LemY - 9, 8, 10);
				RemoveTerrain(L,rem);
				/*	fixed (Image* ptr = &lpm.imgLevel)
					{
						ImageDrawRectangle(ptr, (int)rem.x, (int)rem.y, (int)rem.width, (int)rem.height, GREEN);
					}*/

			}


			L.pm.imgInvalid = true;
		}


		public override bool Handle(Lemming L)
		{

			bool r = true;
			int LemDY = 0;
			int n = 0;
			bool continueWork = false;
			if (L.LemPhysicsFrame.In(2, 3, 4, 5))
			{
				ApplyBashingMask(L, L.LemPhysicsFrame - 2);
			}

			if (L.LemPhysicsFrame == 5)
			{
				continueWork = false;

				for (n = 1; n < 14; n++)
				{//ToDo check for steel :4184
					if (HasPixelAt(L,L.LemX + n * L.LemDx, L.LemY - 6))
					{
						continueWork = true;
					}
					if (HasPixelAt(L,L.LemX + n * L.LemDx, L.LemY - 5))
					{
						continueWork = true;
					}
				}

				if (!continueWork)
				{
					if (HasPixelAt(L, L.LemX, L.LemY))
						((absSkill)WALKING).Transition(L);
					else ((absSkill)FALLING).Transition(L);
				}
			}

			if(L.LemPhysicsFrame.In(11, 12, 13, 14, 15))
			{
				L.LemX += L.LemDx;
				LemDY = FindGroundPixel(L,L.LemX, L.LemY);
				//L.dbgString = $"DY:{LemDY}";
				//ToDo: dehoist check

				if (LemDY == 4)
				{
					L.LemY += LemDY;
				}
				else if (LemDY == 3)
				{
					L.LemY += LemDY;
					((absSkill)WALKING).Transition(L);
				}
				else if (LemDY.In(0, 1, 2))
				{
					//ToDO basher indestructable check :4233

					L.LemY += LemDY;
				}

			}


			return r;

		}

		#endregion
	}


}