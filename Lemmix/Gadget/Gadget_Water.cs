using Raylib_CsLo;
using static CLemmix4.RaylibMethods;
using static Raylib_CsLo.Raylib;

namespace CLemmix4.Lemmix.Gadget
{
	public class Gadget_Water : absGadget
	{

		public override string GadgetName => "WATER";

		public Gadget_Water() : base(null, null) { }
		public Gadget_Water(LevelPack.LevelData.LevelGadget lvlGadget, GadgetHandler handler) : base(lvlGadget, handler)
		{
		}


		public override void LogicOfFrame()
		{
			base.LogicOfFrame();
		}
		//https://www.lemmingsforums.net/index.php?topic=4337.0
		int c = 10;
		public int v1 = 48;
		public unsafe override void DrawOfFrame(Image lastFrame)
		{
			if (++c >= 5)
			{
				++this.frameCur;
				if (this.frameCur > (this.frameMax - 1))
				{
					this.frameCur = 0;
				}

				int frHeight = this.gadgetAnimTexture.imgMain.height / this.GadgetDef.EffectData.Primary_Animation.Frames;
				Rectangle srcRec = new Rectangle(0, frHeight * this.frameCur, 64, frHeight);

				Rectangle dstRec = new Rectangle(this.GadgetDef.X, this.GadgetDef.Y, this.GadgetDef.Width, this.GadgetDef.Height);
				var margin = this.GadgetDef.EffectData.Primary_Animation.CutRect;

				Color tC = WHITE;

				//	ImageDrawCS3(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, srcRec, dstRec, tC, this.gadHandler.lpm, GadgetDef, lastFrame, ref this.gadHandler.fmask);
				if (this.GadgetDef.EffectData.Primary_Animation.NINE_SLICE_BOTTOM > 0)
				{
					int nsb = this.GadgetDef.EffectData.Primary_Animation.NINE_SLICE_BOTTOM;
					Rectangle src9 = new Rectangle(srcRec.x, srcRec.y + srcRec.height - nsb, srcRec.width, nsb);
					Rectangle dst9 = new Rectangle(dstRec.x, dstRec.y + dstRec.height - nsb, dstRec.width, nsb);
					ImageDrawCS3(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, src9, dst9, WHITE, this.gadHandler.lpm, GadgetDef, lastFrame, ref this.gadHandler.fmask);

				}

				if (this.GadgetDef.EffectData.Primary_Animation.NINE_SLICE_TOP > 0)
				{
					int nst = this.GadgetDef.EffectData.Primary_Animation.NINE_SLICE_TOP;
					Rectangle src9 = new Rectangle(srcRec.x, srcRec.y, srcRec.width, nst);
					Rectangle dst9 = new Rectangle(dstRec.x, dstRec.y, dstRec.width, nst);
					ImageDrawCS3(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, src9, dst9, WHITE, this.gadHandler.lpm, GadgetDef, lastFrame, ref this.gadHandler.fmask);

				}
				bool drawlines = false;
				fixed (Image* ptr = &this.gadHandler.lpm.imgGadgets)
				{
					if (drawlines)

						if (GadgetDef.Flags.HasFlag(LevelPack.LevelData.LevelGadget.FlagsGadget.NO_OVERWRITE))
						{
							ImageDrawRectangleLines(ptr, dstRec, 1, RED);
						}
						else
						{
							ImageDrawRectangleLines(ptr, dstRec, 1, BLUE);

						}
				}

				c = 0;
			}
		}
	}



}
