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

		public class SklNone : absSkill
		{
			public override string Name => NONE;

		public override bool Handle(Lemming L)
		{
			//L.LemAction.TransitionTo(L, FALLING);
			((absSkill)FALLING).Transition(L);
			return base.Handle(L);

		}


	}


}