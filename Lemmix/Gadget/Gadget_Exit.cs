using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static CLemmix4.RaylibMethods;
using static Raylib_CsLo.Raylib;

namespace CLemmix4.Lemmix.Gadget
{
	public class Gadget_Exit : absGadget
	{

		public override string GadgetName => "EXIT";
		public Vector2 Pos { get; }

		public Gadget_Exit() : base(null, null)
		{

		}

		public Gadget_Exit(LevelPack.LevelData.LevelGadget lvlGadget, GadgetHandler handler) : base(lvlGadget, handler)
		{
		}
		int c = 10;

		public override void DrawOfFrame(Image lastFrame)
		{
			if (++c >= 5)
			{
				++this.frameCur;
				if (this.frameCur > (this.frameMax - 1))
				{
					this.frameCur = 0;
				}
				int frHeight = this.gadgetAnimTexture.imgMain.height / this.GadgetDef.EffectData.Primary_Animation.Frames;
				int frWidth = this.gadgetAnimTexture.imgMain.width;
				Rectangle srcRec = new Rectangle(0, frHeight * this.frameCur, frWidth, frHeight);
				Rectangle dstRec = new Rectangle(this.GadgetDef.X, this.GadgetDef.Y, frWidth, frHeight);

				ImageDrawCS3(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, srcRec, dstRec, WHITE, this.gadHandler.lpm, GadgetDef, lastFrame, ref this.gadHandler.fmask);
				c = 0;
			}


			//	base.DrawOfFrame(lastFrame);
		}
	}
}
