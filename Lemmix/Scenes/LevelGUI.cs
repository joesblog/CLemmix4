//using Raylib_cs;
using System;
using System.Collections.Generic;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;
using static CLemmix4.RaylibMethods;
using System.Numerics;
using CLemmix4.Lemmix.Skills;
using static CLemmix4.Lemmix.Skills.skillNameHolders;

namespace CLemmix4.Lemmix.Core
{
	public class LevelGUI : IDisposable
	{
		//area should be 20% of screen height

		Camera2D guiCamera;
		private Texture texSkillsBG;
		private Rectangle rectBar;
		private Rectangle rectDiag;
		private Rectangle rectTime;
		private Rectangle rectIn;
		private Rectangle rectOut;
		private Rectangle rectAll;

		public LevelScene levelScene { get; set; }
		public SpriteFont fnt { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		public int Ypos { get; private set; }
		public LevelScene Lsn { get; }

		//public Lemming.enmLemmingState SelectedSkill = Lemming.enmLemmingState.None;
		public absSkill SelectedSkill = (absSkill)SkillHandler.lupSkillNameSkill["NONE"];
		public Dictionary<int, absSkill> dictAvailSkills = new Dictionary<int, absSkill>();




		public void Setup()
		{
			fnt = new SpriteFont("gfx/panel_font.png", 16, 32, '!', '~');
			Height = (int)Math.Ceiling(((float)GetScreenHeight()) * 0.2);
			Width = GetScreenWidth();
			dictAvailSkills.Add(0, RELEASESLOWER);
			dictAvailSkills.Add(1, RELEASEFASTER);
			dictAvailSkills.Add(2, CLIMBING);
			dictAvailSkills.Add(3, FLOATING);
			dictAvailSkills.Add(7, BASHING);
			dictAvailSkills.Add(8, DIGGING);
			dictAvailSkills.Add(5, BUILDING);
			dictAvailSkills.Add(4, BLOCKING);
			Ypos = GetScreenHeight() - Height;
			guiCamera = new Camera2D()
			{
				offset = new Vector2(0, Ypos),
				zoom = 1
			};
			texSkillsBG = LoadTexture("gfx/skillbutton.png");

			Func<float, float, Rectangle> CalcRectForBar = (wPc, xPc) =>
			{

				float fh = ((float)Height) * 0.37f;

				float fw = (float)Width;
				float pWidth = fw * wPc;
				float pXPos = fw * xPc;

				return new Rectangle(pXPos, 0, pWidth, fh);

			};


			this.rectBar = new Rectangle(0, 0, Width, ((float)Height) * 0.37f);
			this.rectDiag = new Rectangle(0, rectBar.height, Width, ((float)Height) * 0.37f);
			this.rectTime = CalcRectForBar(0.22f, 0.74f);
			this.rectIn = CalcRectForBar(0.16f, 0.53f);
			this.rectOut = CalcRectForBar(0.15f, 0.32f);

			calcSkillRect();


			this.rectAll = new Rectangle(0, 0, Width, Height);
		}

		private void calcSkillRect()
		{

			this.toolItemWidth = this.Width * 0.05f;

			float toolsWidth = toolItemWidth * 12; //this.Width * 0.60f;
			float toolsHeight = this.Height * 0.6229f;
			float toolsPosY = this.Height - toolsHeight;

			this.rectSkills = new Rectangle(0, toolsPosY, toolsWidth, toolsHeight);


		}

		public LevelGUI(LevelScene lsn)
		{
			Lsn = lsn;
		}

		public string timeString = "00-00";
		bool diagMode = false;
		float scale = 1.5f;
		private float toolItemWidth;
		private Rectangle rectSkills;

		public void RenderGui()
		{

			BeginMode2D(guiCamera);

			DrawRectangle(rectAll, BLACK);


			renderStatus();
			renderSkillsBox();

			if (diagMode)
				renderDiag();

			EndMode2D();

		}

		Dictionary<int, Rectangle> dictSkillDestRect = new Dictionary<int, Rectangle>();
		Rectangle rectSkillSource = new Rectangle(0, 0, 32, 48);
		private int selectedSkillId = -1;

		private void renderSkillsBox()
		{

			DrawRectangle(rectSkills, RED);
			for (int i = 0; i < 12; i++)
			{
				if (!dictSkillDestRect.ContainsKey(i))
				{
					Rectangle r = new Rectangle();
					r.x = i * this.toolItemWidth;
					r.y = this.rectSkills.y;
					r.width = this.toolItemWidth;
					r.height = this.rectSkills.height;
					dictSkillDestRect.Add(i, r);
				}
				var cr = dictSkillDestRect[i];

				DrawTexturePro(this.texSkillsBG, rectSkillSource, dictSkillDestRect[i], new Vector2(0, 0), 0, WHITE);
				if (dictAvailSkills.ContainsKey(i))
				{
				
					if (dictAvailSkills[i].GetSpriteDefinition().CellW == rectSkillSource.width)
					{

						DrawTexturePro(dictAvailSkills[i].GetSpriteDefinition().Texture, rectSkillSource, dictSkillDestRect[i], new Vector2(0, 0), 0, WHITE);

					}
					else
					{
						// a sprite
						Rectangle curFrame = new Rectangle(dictAvailSkills[i].GetSpriteDefinition().CellW, 0, dictAvailSkills[i].GetSpriteDefinition().CellW, dictAvailSkills[i].GetSpriteDefinition().CellH);

						var dest = new Rectangle();
						dest.y = this.rectSkills.y + (this.rectSkills.y * 0.52f);
						dest.width = this.toolItemWidth * 0.84f;
						dest.height = this.rectSkills.height * 0.52f;
						dest.x = cr.x + ((this.toolItemWidth / 2));
						dest.x -= (dest.width / 2);

						DrawTexturePro(dictAvailSkills[i].GetSpriteDefinition().Texture, curFrame, dest, new Vector2(0, 0), 0, WHITE);

					}

					if (selectedSkillId == i)
					{

						DrawRectangleLinesEx(cr, 4, YELLOW);
					}


				}
			}
		}
		int lastSkillChangeCount = 0;
		public void InputCheck()
		{
			var mp = GetMousePosition();

			var mp2 = GetScreenToWorld2D(mp, this.guiCamera);




			if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
			{
				int? seli = null;

				foreach (var i in dictSkillDestRect)
				{
					if (CheckCollisionPointRec(mp2, i.Value))
					{
						seli = i.Key;
						break;
					}
				}
				if (seli.HasValue && (seli == 0 || seli == 1))
				{
					Lsn.pm.ReleaseRateChanging = true;

					if (dictAvailSkills.ContainsKey(seli.Value))
					{
						if (++lastSkillChangeCount >= 5)
						{
							if (seli == 0)
							{ //gm:6275
					 
									Lsn.pm.SpawnIntervalModifier = 1;
							}
							else if (seli == 1)
							{
								Lsn.pm.SpawnIntervalModifier = -1;
							}

							//if (seli == 0) Lsn.pm.ReleaseRate--;
							//if (seli == 1) Lsn.pm.ReleaseRate++;
							lastSkillChangeCount = 0;
							//Lsn.pm.SpawnIntervalModifier = 0;
						}

					}
					selectedSkillId = -1; SelectedSkill = NONE;
					Lsn.pm.ReleaseRateChanging = false;

				}
			}
			if (IsMouseButtonUp(MouseButton.MOUSE_BUTTON_LEFT) && Lsn.pm.SpawnIntervalModifier != 0) Lsn.pm.SpawnIntervalModifier = 0;


			if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
			{
				int? seli = null;
				foreach (var i in dictSkillDestRect)
				{
					if (CheckCollisionPointRec(mp2, i.Value))
					{
						seli = i.Key;
						break;
					}
				}

				if (seli.HasValue && seli.Value > 1)
				{
					selectedSkillId = seli.Value;
					if (dictAvailSkills.ContainsKey(seli.Value))
					{
						SelectedSkill = dictAvailSkills[seli.Value];
					}
				}

			
			}



		}

		private void renderDiag()
		{
			fnt.DrawString($"{Lsn.pm.mainCam.offset}|{Lsn.pm.mainCam.target }|{Lsn.pm.mainCam.zoom}\t{Lsn.pm.lemHandler.lvl.Width}|{GetRenderWidth() / Lsn.pm.mainCam.zoom}", rectDiag, 1, 4);

		}

		private void renderStatus()
		{
			fnt.DrawString("TIME " + timeString, rectTime, scale);
			//fnt.DrawString($"FR:{GetFrameTime()}", rectTime, scale);
			fnt.DrawString("IN 00%", rectIn, scale);
			fnt.DrawString($"OUT {Lsn.pm.LemmingsOut:00}", rectOut, scale);
			fnt.DrawString($"RR:{Lsn.pm.ReleaseRate} SMOD{Lsn.pm.SpawnIntervalModifier}", this.rectBar, scale);
		}

		public void Dispose()
		{
			fnt.Dispose();
			UnloadTexture(texSkillsBG);
		}
	}



}
