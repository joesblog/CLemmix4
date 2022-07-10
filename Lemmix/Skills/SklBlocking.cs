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

	public class SklBlocking : absSkill
	{
		public override string Name => BLOCKING;

		public SpriteDefinition _SpriteDef;
		public override SpriteDefinition SpriteDef
		{

			get
			{
				if (_SpriteDef == null)
					_SpriteDef = new SpriteDefinition() { CellH = 13, CellW = 16, Cols = 2, Rows = 16, Name = "BLOCKING", Path = "styles/default/lemmings/blocker.png", WidthFromCenter = 8 };
				return _SpriteDef;
			}
		}
		public override int SpriteAnimFrames => 16;

		#region cst

		public override bool Handle(Lemming L)
		{
			if (!HasPixelAt(L, L.LemX, L.LemY))
				((absSkill)FALLING).Transition(L);
			return true;
		}


		internal override void Transition(Lemming L, bool DoTurn = false)
		{
			base.Transition(L, DoTurn);
			L.LemHasBlockerField = true;
			L.pm.lemHandler.SetBlockerMap();
		}


		#endregion
	}




}