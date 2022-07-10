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

	public class SklFalling : absSkill
	{

		public const int LEMMING_MAX_Y = 9;
		public const int MAX_FALLDISTANCE = 62;
		public override string Name => FALLING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 10, CellW = 16, Cols = 2, Rows = 4, Name = "FALLING", Path = "styles/default/lemmings/faller.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 4;



	 


		public override bool Handle(Lemming L)
		{
			bool r = true;
			int currFalDistance = 0;
			int maxFallDistance = 3;

			bool IsFallFatal()
			{
				return (!L.LemIsFloater) && L.LemFallen > MAX_FALLDISTANCE;

			}

			bool CheckFloaterTransition()
			{
				bool rx = false;

				if (L.LemIsFloater && L.LemTrueFallen > 16 && currFalDistance == 0)
				{

					L.skillHandler.Transition(FLOATING);
					//Transition(L, FLOATING);
					rx = true;
				}

				return rx;

			}

			if (CheckFloaterTransition()) return r;
			//check for floater or glider TODO





			//ToDo check for updraft

			//todo: check for floater/glider transition

			//move lem until hit the ground
			while (currFalDistance < maxFallDistance && !L.pm.lemHandler.HasPixelAt(L.LemX, L.LemY))
			{
				//check for floater

				L.LemY++;
				currFalDistance++;
				L.LemFallen++;
				L.LemTrueFallen++;

				//check updraft
			}


			if (L.LemFallen > MAX_FALLDISTANCE) L.LemFallen = MAX_FALLDISTANCE + 1;
			if (L.LemTrueFallen > MAX_FALLDISTANCE) L.LemTrueFallen = MAX_FALLDISTANCE + 1;

			//	L.LemActionNext = WALKING;
			L.skillHandler.ActionNext = WALKING;

			return r;
		}
	}


}