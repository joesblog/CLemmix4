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

	public class SklMining : absSkill
	{
		public override string Name => MINING;


		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 14, CellW = 16, Cols = 2, Rows = 24, Name = MINING, Path = "styles/default/lemmings/miner.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 24;

		private SpriteDefinition _SpriteDefMask;
		public SpriteDefinition SpriteDefMask
		{
			get
			{

				if (_SpriteDefMask == null)
					_SpriteDefMask = new SpriteDefinition() { CellH = 13, CellW = 16, Cols = 2, Rows = 2, Name = "MINING", Path = "styles/default/mask/miner.png", WidthFromCenter = 8 };
				return _SpriteDefMask;
			}
		}

		public override void InitCheckSprite()
		{
			SpriteDef.initCheck();
			SpriteDefMask.initCheck();
		}

		public override bool Handle(Lemming L)
		{
			bool r = true;

			if (L.LemPhysicsFrame.In(1, 2))
			{
				ApplyMinerMask(L, L.LemPhysicsFrame - 1, 0, 0);
			}
			else if (L.LemPhysicsFrame.In(3, 15))
			{
				//slider and dehoist

				L.LemX += 2 * L.LemDx;
				L.LemY++;

				//slider dehoist 2

				//check for indestructable

				if (!HasPixelAt(L, L.LemX - L.LemDx, L.LemY - 1)  //can walk over it
					&& !HasPixelAt(L, L.LemX - L.LemDx, L.LemY)
					&& !HasPixelAt(L, L.LemX - L.LemDx, L.LemY + 2))
				{
					L.LemX -= L.LemDx;
					L.LemY++;
					((absSkill)FALLING).Transition(L);
					L.LemFallen = L.LemFallen + 1;

				}
				else if (!HasPixelAt(L, L.LemX, L.LemY))
				{
					L.LemY++;
					((absSkill)FALLING).Transition(L);
				}
			}

			return r;

		}

		private void ApplyMinerMask(Lemming L, int MaskFrame, int AdjustX, int AdjustY)
		{
			Rectangle S;
			Rectangle D;

			//int MaskX = l.LemX + l.LemDx - 8 + AdjustX;
			//int MaskY = l.LemY + MaskFrame - 12 + AdjustY;
			S = new Rectangle(0, 0, 16, 13);

 
			S.x = L.LemDx == 1 ? 16 : 0;
			S.y = MaskFrame * 13;


			D = new Rectangle();
			D.x = L.LemX - 8;
			D.y = L.LemY - 13;
			D.width = 16;
			D.height = 13;

			var msd = SpriteDefMask;
			msd.initCheck();

			ImageDrawCS_ApplyAlphaMask(ref L.pm.imgLevel, msd.imgSprite, S, D, BLANK);
			ImageDrawCS_ApplyAlphaMask(ref L.pm.imgPhysics, msd.imgSprite, S, D, BLANK);
			L.pm.imgInvalid = true;

		}
	}


}