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

	public class SklClimbing : absSkill
	{
		public override string Name => CLIMBING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 12, CellW = 16, Cols = 2, Rows = 8, Name = "CLIMBING", Path = "styles/default/lemmings/climber.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 8;

		#region cst
		public override bool TryAssign(Lemming L)
		{

			L.LemIsClimber = true;
			return true;

		}

		public override bool Handle(Lemming L)
		{
			bool FoundClip = false;
			bool r = true;
			if (L.LemPhysicsFrame <= 3)
			{
				FoundClip = HasPixelAt(L, L.LemX - L.LemDx, L.LemY - 6 - L.LemPhysicsFrame)
		|| (HasPixelAt(L, L.LemX - L.LemDx, L.LemY - 5 - L.LemPhysicsFrame) && !L.LemIsStartingAction);

				if (L.LemPhysicsFrame == 0)
					FoundClip = FoundClip && HasPixelAt(L, L.LemX - L.LemDx, L.LemY - 7);

				if (FoundClip)
				{
					if (!L.LemIsStartingAction) L.LemY = L.LemY - L.LemPhysicsFrame + 3;

					//ToDo handle slider
					L.LemX -= L.LemDx;
					((absSkill)FALLING).Transition(L,true);

					L.LemFallen++;

				}
				else if (!HasPixelAt(L, L.LemX, L.LemY - 7 - L.LemPhysicsFrame))
				{
					if (!(L.LemIsStartingAction && L.LemPhysicsFrame == 1))
					{
						L.LemY = L.LemY - L.LemPhysicsFrame + 2;
						L.LemIsStartingAction = false;
					}
									((absSkill)HOISTING).Transition(L);

				}
			}
			else
			{
				L.LemY--;
				L.LemIsStartingAction = false;
				FoundClip = HasPixelAt(L, L.LemX - L.LemDx, L.LemY - 7);

				if (L.LemPhysicsFrame == 7)
					FoundClip = FoundClip && HasPixelAt(L, L.LemX - L.LemDx, L.LemY - 7);

				if (FoundClip)
				{
					L.LemY--;
					//ToDo slider

					L.LemX -= L.LemDx;
					((absSkill)FALLING).Transition(L,true);

				}
			}


			return r;
		}
		#endregion
	}


}