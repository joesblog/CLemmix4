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


	public class Gadget_Entrance : absGadget
	{
		public override string GadgetName => "ENTRANCE";

		public Vector2 Pos { get; }

		public Gadget_Entrance() : base(null, null)
		{ 
		
		}

		public bool AnimComplete = false;
		public bool AnimReadyToGo = true;
		public bool EntranceOpen = false;
		public void OpenEntrance()
		{
			AnimReadyToGo = true;
		}
		public Gadget_Entrance(LevelPack.LevelData.LevelGadget lvlGadget, GadgetHandler handler) : base(lvlGadget, handler)
		{
			this.Pos = new Vector2(lvlGadget.X + lvlGadget.EffectData.Trigger_X, lvlGadget.Y + lvlGadget.EffectData.Trigger_Y);
				
				// new Vector2(lvlGadget.EffectData.Trigger_X, lvlGadget.EffectData.Trigger_Y);
		}

		int c = 10;

		public override void DrawOfFrame(Image lastFrame)
		{
			//	if (!AnimReadyToGo || AnimComplete) return;
			int frHeight = this.gadgetAnimTexture.imgMain.height / this.GadgetDef.EffectData.Primary_Animation.Frames;
			int frWidth = this.gadgetAnimTexture.imgMain.width;
			if (AnimComplete)
			{
				Rectangle srcRec = new Rectangle(0, 0, frWidth, frHeight);
				Rectangle dstRec = new Rectangle(this.GadgetDef.X, this.GadgetDef.Y, frWidth, frHeight);
				ImageDrawCS3(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, srcRec, dstRec, WHITE, this.gadHandler.lpm, GadgetDef, lastFrame, ref this.gadHandler.fmask);

			}
			else {
				if (++c >= 5)
				{
					++this.frameCur;
					if (this.frameCur > (this.frameMax - 1))
					{
						this.frameCur = 0;
						AnimComplete = true;
						EntranceOpen = true;
						return;
					}


					Rectangle srcRec = new Rectangle(0, frHeight * this.frameCur, frWidth, frHeight);
					Rectangle dstRec = new Rectangle(this.GadgetDef.X, this.GadgetDef.Y, frWidth, frHeight);


					ImageDrawCS3(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, srcRec, dstRec, WHITE, this.gadHandler.lpm, GadgetDef, lastFrame, ref this.gadHandler.fmask);
					//ImageDraw(ref this.gadHandler.lpm.imgGadgets, gadgetAnimTexture.imgMain, srcRec, dstRec, WHITE);
					c = 0;
				}
			}

			


		}

	}
}
