﻿//using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.RlGl;
using Raylib_CsLo;
using static CLemmix4.RaylibMethods;
using System.Numerics;
using System.Threading;
using System.Diagnostics;

namespace CLemmix4.Lemmix.Core
{


	public class LevelScene_SkillsBox
	{
		public bool allowReleaseRate = true;
		public bool allowPause = true;
		public bool allowNuke = true;




	}




	public class LevelScene : absScene
	{

		public int ow = 320;
		public int oh = 240;
		public float rW;
		public LevelPlayManager pm;// = new LevelPlayManager();
		LevelGUI lGui;
		//	public LemHandler pm.lemHandler { get; }
		//public LevelPack.LevelData lvl { get; }
		//public Image pm.imgLevel;
		//public Texture pm.texLevel;

		//private Image pm.imgPhysics;
		//public Texture pm.texPhysics;

		public float screenWM { get; private set; }

		Color bg = new Color(0, 0, 52, 255);

		Vector2 prevMouse;

		public override void onWindowReisized(int newWidth, int newHeight)
		{

			rW = (float)manager.ScreenWidth / (float)ow;
			rW = 3.85f;
			SetMousePosition(manager.ScreenWidth / 2, manager.ScreenHeight / 2);
			screenWM = manager.ScreenWidth * 0.05f;
			base.onWindowReisized(newWidth, newHeight);

		}

		public LevelScene(SceneManager coreManager, LevelPack.LevelData _level) : base(coreManager)
		{
			pm = new LevelPlayManager(_level);

			pm.lemHandler = new LemHandler(_level, pm);
			pm.Width = pm.lemHandler.lvl.Width;
			pm.Height = pm.lemHandler.lvl.Height;

			pm.size = pm.lemHandler.lvl.Width * pm.lemHandler.lvl.Height;

			pm.gadgHandler = new Gadget.GadgetHandler(_level, pm);
			onWindowReisized(manager.ScreenWidth, manager.ScreenHeight);
		}


		Thread threadLemmingControl;
		ThreadStart tsLemmingControl;
		bool thAllowWork = false;

		TextureCacheData tchterrain;
		public override void SetupScene()
		{
			lGui = new LevelGUI(this);
			lGui.Setup();
			pm.lemHandler.onTimeUpdate += LemHandler_onTimeUpdate;
			tchterrain = new TextureCacheData();
			//	using (var tchterrain = new TextureCacheData())
			{


				cam = new Camera2D() { offset = new System.Numerics.Vector2(0, 0), target = new System.Numerics.Vector2(0, 0), rotation = 0, zoom = 1 };
				//cam = new Camera2D(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0), 0, 1);
				cam.zoom = rW;
				cam.target = GetScreenToWorld2D(cam.offset + new System.Numerics.Vector2(pm.lemHandler.lvl.Start_X, pm.lemHandler.lvl.Start_Y - 80), cam);
				foreach (var i in pm.lemHandler.lvl.Terrain.GroupBy(o => o + o.Style).Select(o => o.First()))
				{
					var op = tchterrain[i];
				}
				SetTargetFPS(60);
				SetMousePosition(manager.ScreenWidth / 2, manager.ScreenHeight / 2);
				SetupLevelLayout(tchterrain);
				/*pm.lemHandler.pm.imgLevel = this.pm.imgLevel;
				pm.lemHandler.pm.imgPhysics = this.pm.imgPhysics;
				pm.lemHandler.pm.texLevel = this.pm.texLevel;
				pm.lemHandler.pm.texPhysics = this.pm.texPhysics;*/
				//t1 = new LemSprite(this.LevelImage) { LemX = 230, LemY = 53 };
				Random r = new Random();

				//pm.lemHandler.lems = new List<Lemming>();
				for (int i = 0; i < 3; i++)
				{
					//pm.lemHandler.AddLemming(new Lemming(this.LevelImage) { LemX = (r.Next(100, 300)), LemY = 30, LemAction = Lemming.enmLemmingState.FALLING });
					//if (i % 2 == 0) pm.lemHandler.lems.Last().LemDX = -1; else pm.lemHandler.lems.Last().LemDX = 1;
				}

			}


			pm.gadgHandler.Setup();
			pm.SetupSpawners();
			tsLemmingControl = new ThreadStart(() => { thmUpdateLemmings(ref cam); });
			threadLemmingControl = new Thread(tsLemmingControl);
			threadLemmingControl.Name = "LEMMING CONTROL";
			threadLemmingControl.Start();


			//startAddThread();
		}

		private void LemHandler_onTimeUpdate(object sender, int e)
		{
			TimeSpan ts = new TimeSpan(0, 0, e);
			this.lGui.timeString = $"{ts.TotalMinutes:00}:{ts.Seconds}:00";
		}

		int waitcount = 0;
		void startAddThread()
		{

			Thread tadd = new Thread(new ThreadStart(() =>
			{

				Random r = new Random();

				while (pm.lemHandler.lems.Count < 1)
				{

					var nl = new Lemming(this.pm) { LemX = (r.Next(100, 340)), LemY = 30, LemAction = Lemming.enmLemmingState.NONE };
					pm.lemHandler.Transition(nl, Lemming.enmLemmingState.FALLING);
					pm.lemHandler.AddLemming(nl);

					Thread.Sleep(2 * 1000);
				}

			}));

			tadd.Start();
		}

		bool gameFinished;
		static int handleclock = 60; // 60 is normal
		int clockframe = 0;
		public void thmUpdateLemmings(ref Camera2D cam)
		{
			for (; ; )
			{
				if (thAllowWork)
				{
					if (gameFinished)
					{
						//exit;
					}
					pm.lemHandler.UpdateLemmings(ref cam);
					//adjust spawn check
					//queued action
					//replay action

					//clear shadows

					clockframe++;
					if (clockframe >= 1000) clockframe = 0;
					Thread.Sleep(handleclock);

				}
				else
				{
					Thread.Sleep(1);
				}

			}
		}

		unsafe void SetupLevelLayout(TextureCacheData tchterrain)
		{
			var img = GenImageColor(pm.lemHandler.lvl.Width, pm.lemHandler.lvl.Height, WHITE);

			ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			//ImageFormat(ref img, (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			//ImageAlphaClear(ref img, Color.WHITE, 1f);
			//ImageAlphaClear(ref img, BLANK, 1f);
			ImageAlphaClear(ref img, BLANK, 1f);

			//pm.mask = new int[pm.size];
			pm.mask = new LevelPlayManager.MaskData[pm.size];
			foreach (var i in pm.lemHandler.lvl.Terrain)
			{
				var tc = tchterrain[i];
				for (int y = 0; y < tc.Height; y++)
				{
					int ypos = y + i.Y;
					for (int x = 0; x < tc.Width; x++)
					{
						int xpos = x + i.X;
						int ix = ypos * pm.lemHandler.lvl.Width + xpos;
						int locix = y * tc.Width + x;
						var imgToUse = tc.imgMain;

						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_HORIZONTAL) && i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_VERTICAL)) imgToUse = tc.imgVertHorizFlip;
						else if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_HORIZONTAL)) imgToUse = tc.imgHorizFlip;
						else if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_VERTICAL)) imgToUse = tc.imgVertFlip;

						var colsrc = GetImageColor(imgToUse, x, y);

						byte a = colsrc.a;
						byte r = colsrc.r;
						byte g = colsrc.g;
						byte b = colsrc.b;

						if (ix > pm.size - 1 || ix < 0) continue;

						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE))
						{
							if (a > 0)
								pm.mask[ix] |= LevelPlayManager.MaskData.NO_OVERWRITE;
						}
						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && (pm.mask[ix].HasFlag(LevelPlayManager.MaskData.TERRAIN)))
						{
							//	continue;
						}





						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.ERASE))
						{

							if (a > 0)
							{
								ImageDrawPixel(ref img, xpos, ypos, BLANK);
								//pm.mask[ix] = 0;
								pm.mask[ix] = LevelPlayManager.MaskData.ERASE;

								continue;
							}
						}
						if (a > 0)
						{
							if (!(i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && (pm.mask[ix].HasFlag(LevelPlayManager.MaskData.TERRAIN))))
							{
								ImageDrawPixel(ref img, xpos, ypos, colsrc);

							}

							pm.mask[ix] |= LevelPlayManager.MaskData.TERRAIN;
							continue;
						}


					}
				}
			}
			pm.imgLevel = img;
			pm.texLevel = LoadTextureFromImage(img);
			pm.imgPhysics = GenImageColor(pm.lemHandler.lvl.Width, pm.lemHandler.lvl.Height, BLANK);
			pm.imgGadgets = GenImageColor(pm.lemHandler.lvl.Width, pm.lemHandler.lvl.Height, BLANK);

			ImageFormat(ref pm.imgPhysics, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			ImageAlphaClear(ref pm.imgPhysics, BLANK, 1f);

			ImageFormat(ref pm.imgGadgets, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			ImageAlphaClear(ref pm.imgGadgets, BLANK, 1f);


			fixed (Image* ptr = &pm.imgPhysics)
				for (int y = 0; y < pm.lemHandler.lvl.Height; y++)
				{
					for (int x = 0; x < pm.lemHandler.lvl.Width; x++)
					{


						if ((pm.mask[y * pm.lemHandler.lvl.Width + x].HasFlag(LevelPlayManager.MaskData.TERRAIN)))
						{
							ImageDrawPixel(ptr, x, y, LemHandler.col_phy_terr);
						}
					}
				}
			pm.texPhysics = LoadTextureFromImage(pm.imgPhysics);
			pm.texGadgets = LoadTextureFromImage(pm.imgGadgets);

		}

		List<Lemming> curLemmings = new List<Lemming>();
		int lastMframe = 0;
		int addholder = 0;
		public unsafe override void Input()
		{

			var thisPos = GetMousePosition();
			var delta = prevMouse - thisPos;
			var a = GetScreenToWorld2D(prevMouse, cam);
			lGui.InputCheck();

			if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
			{

				if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) && IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
				{
					if (addholder++ >= 10)
					{
						var nl = new Lemming(pm) { LemX = (int)a.X, LemY = (int)a.Y, LemAction = Lemming.enmLemmingState.NONE };
						pm.lemHandler.Transition(nl, Lemming.enmLemmingState.FALLING);
						pm.lemHandler.AddLemming(nl);
						addholder = 0;
					}
					
				}
				if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) && IsKeyDown(KeyboardKey.KEY_LEFT_ALT))
				{
					var nl = new Lemming(pm) { LemX = (int)a.X, LemY = (int)a.Y, LemAction = Lemming.enmLemmingState.NONE };
					pm.lemHandler.Transition(nl, Lemming.enmLemmingState.FALLING);
					pm.lemHandler.AddLemming(nl);
				}

			}


			if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
			{

				if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
				{
					var nl = new Lemming(pm) { LemX = (int)a.X, LemY = (int)a.Y, LemAction = Lemming.enmLemmingState.NONE };
					pm.lemHandler.Transition(nl, Lemming.enmLemmingState.FALLING);
					pm.lemHandler.AddLemming(nl);
				}

			
				else
				{


					var um = pm.lemHandler.lems.Where(o => o.UnderMouse).FirstOrDefault();
					
					if (um != null && lGui.SelectedSkill > 0)
					{
						if (lGui.SelectedSkill == Lemming.enmLemmingState.CLIMBING)
						{
							um.LemIsClimber = true;
						}
						else if (lGui.SelectedSkill == Lemming.enmLemmingState.FLOATING)
						{
							um.LemIsFloater = true;
						}
						else {
							pm.lemHandler.Transition(um, lGui.SelectedSkill);
						}

					
					}
					/*foreach (var i in um)
					{
						//i.LemActionNext = Lemming.enmLemmingState.BASHING;
						if (i.LemAction == Lemming.enmLemmingState.BASHING)
						{
							pm.lemHandler.Transition(i, Lemming.enmLemmingState.WALKING);

						}
						else
						{
							pm.lemHandler.Transition(i, Lemming.enmLemmingState.BASHING);

						}
					}*/

				}

			}


			if (IsKeyReleased(KeyboardKey.KEY_F1)) pm.gadgHandler.toggleGadget(1 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F2)) pm.gadgHandler.toggleGadget(2 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F3)) pm.gadgHandler.toggleGadget(3 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F4)) pm.gadgHandler.toggleGadget(4 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F5)) pm.gadgHandler.toggleGadget(5 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F6)) pm.gadgHandler.toggleGadget(6 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F7)) pm.gadgHandler.toggleGadget(7 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F8)) pm.gadgHandler.toggleGadget(8 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F9)) pm.gadgHandler.toggleGadget(9 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F10)) pm.gadgHandler.toggleGadget(10 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F11)) pm.gadgHandler.toggleGadget(11 - 1);
			if (IsKeyReleased(KeyboardKey.KEY_F12)) pm.gadgHandler.toggleGadget(12 - 1);


			if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_MIDDLE))
			{
				cam.zoom = rW;
				cam.offset = new Vector2(0, 0);
				cam.target = GetScreenToWorld2D(cam.offset + new System.Numerics.Vector2(pm.lemHandler.lvl.Start_X, pm.lemHandler.lvl.Start_Y - 80), cam);
			}
			//curLemmings.Clear();

			if (IsKeyPressed(KeyboardKey.KEY_P))
			{
				pm.lemHandler.pause = !pm.lemHandler.pause;
			}
			float mw = GetMouseWheelMove();
			if (mw != 0)
			{


				cam.zoom += GetMouseWheelMove();

			}


			if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
			{

				var newDelta = cam.target.X + GetMouseDelta().X;
				var maxshow = GetRenderWidth() / cam.zoom;

				if (newDelta < 0)
					cam.target.X = 0;
				else if (newDelta + maxshow > pm.lemHandler.lvl.Width)
					cam.target.X = pm.lemHandler.lvl.Width - maxshow;
				else
				{
					cam.target.X += GetMouseDelta().X;
				}
				// = GetScreenToWorld2D(cam.offset + delta, cam);
				/*	if (cam.zoom > 1)
						cam.target.Y += GetMouseDelta().Y;*/
			}
			else
			{
				/*	if (thisPos.X > manager.ScreenWidth - (screenWM))
						cam.target.X += 0.75f;
					else if (thisPos.X < screenWM)
						cam.target.X -= 0.75f;*/
			}
			prevMouse = thisPos;

		}



		Color col1 = new Color(100, 50, 50, 100);
		Color col2 = new Color(50, 50, 100, 100);
		private Camera2D camGui;

		public unsafe override void Render()
		{

			if (!thAllowWork) thAllowWork = true;


			if (pm.imgInvalid)
			{
				Color* pxImage = LoadImageColors(pm.imgLevel);
				UpdateTexture(pm.texLevel, pxImage);
				UnloadImageColors(pxImage);

				Color* pxPhys = LoadImageColors(pm.imgPhysics);
				UpdateTexture(pm.texPhysics, pxPhys);
				UnloadImageColors(pxPhys);
				pm.imgInvalid = false;
			}
			pm.gadgHandler.RenderAll();


			Raylib.BeginDrawing();

			Raylib.ClearBackground(bg);

			BeginMode2D(cam);
			var a = GetScreenToWorld2D(prevMouse, cam);


			//DrawTexture(pm.texGadgets, 0, 0, WHITE);

			//	BeginBlendMode(BlendMode.BLEND_ALPHA);


			if (pm.lemHandler.lems.Count() > 0)
				//	DrawText($"LemmingCount: {pm.lemHandler.lems.Count()}\n{pm.lemHandler.lems.Last().LemAction}\n{GetFPS()}Fps.\n", (int)a.X + 10, (int)a.Y + 10, 18, RED);
				//DrawText($"LEMS:{pm.lemHandler.lems.Last().LemAction}\n{GetFPS()}\n{c}\n{hpa}\n{a}", (int)a.X + 10, (int)a.Y + 20, 12, WHITE);
				foreach (var i in curLemmings)
				{
					DrawRectangleLines((int)i.PositionalRectangle.x, (int)i.PositionalRectangle.y, (int)i.PositionalRectangle.width, (int)i.PositionalRectangle.height, GREEN);
				}

			DrawTexture(pm.texLevel, 0, 0, WHITE);
			DrawTextureRec(pm.texGadgetsTarget.texture, new Rectangle(0, 0, pm.texGadgetsTarget.texture.width, -pm.texGadgetsTarget.texture.height), new Vector2(0, 0), WHITE);
			pm.lemHandler.HandleDraw();

			foreach (var i in pm.gadgHandler.gadgets)
			{
				Rectangle dstRec = new Rectangle(i.GadgetDef.X, i.GadgetDef.Y, i.GadgetDef.Width, i.GadgetDef.Height);

				if (CheckCollisionPointRec(a, dstRec))
				{
					Color cl1 = i.GadgetDef.Flags.HasFlag(LevelPack.LevelData.LevelGadget.FlagsGadget.NO_OVERWRITE) ? RED : BLUE;
					//	DrawRectangleLines((int)dstRec.X, (int)dstRec.Y, (int)dstRec.width, (int)dstRec.height, cl1);
					//	DrawText($"{i.gadgetId}", (int)dstRec.X, (int)dstRec.Y, 12, WHITE);
				}
			}
			BeginBlendMode(BlendMode.BLEND_ADD_COLORS);
			foreach (var i in pm.lemHandler.lvl.Terrain)
			{
				if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.ERASE)) continue;
				Rectangle dstRec = new Rectangle(i.pos.X, i.pos.Y, tchterrain[i].Width, tchterrain[i].Height);
				if (CheckCollisionPointRec(a, dstRec))
				{
					Color cl1 = i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) ? col1 : col2;
					//DrawRectangle((int)dstRec.X, (int)dstRec.Y, (int)dstRec.width, (int)dstRec.height, cl1);

				}



			}
			EndBlendMode();
			//	EndBlendMode();
			DrawText($"{a}", (int)a.X, (int)a.Y, 12, WHITE);

			var c = GetImageColor(pm.imgLevel, (int)a.X, (int)a.Y);
			//	var hpa = Lemming.HasPixelAt((int)a.X, (int)a.Y, LevelImage);
			//foreach (var i in pm.lemHandler.getLemmingEnumerator().) i.Draw();
			////for (int i = 0; i < pm.lemHandler.lems.Count; i++)
			//{
			//	pm.lemHandler.lems[i].Draw();
			//}

			//	t1.Draw();
			//fntLem.DrawChar('H', new Vector2(100, 100),0.5f);
			EndMode2D();
			//	BeginMode2D(camGui);
			//fntLem.DrawString("WALKER\t\t2\t\tOUT\t12\t\tIN\t00%\t\tTIME\t4-48", new Vector2(0, 0), 1f, kerning: 1);
			lGui.RenderGui();
			//	DrawRectangle(0, 0, 100,  200, BLANK);
			//		EndMode2D();
			Raylib.EndDrawing();
		}



	}



}
