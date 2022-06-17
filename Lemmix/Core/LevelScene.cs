//using Raylib_cs;
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
using CLemmix4.Lemmix.Utils;
using System.Diagnostics;

namespace CLemmix4.Lemmix.Core
{





	public class SceneManager
	{

		public Dictionary<int, absScene> NavStack = new Dictionary<int, absScene>();
		public KeyboardKey quitKey = KeyboardKey.KEY_ESCAPE;
		public bool running = true;
		private int StartWidth { get; }
		private int StartHeight { get; }
		public string Title { get; }
		static int lastSceneId = 0;
		public int currentSceneId { get; set; } = -1;


		public SceneManager(int startWidth = 1024, int startHeight = 768, string Title = "Core")
		{
			StartWidth = startWidth;
			StartHeight = startHeight;
			ScreenWidth = StartWidth;
			ScreenHeight = StartHeight;
			this.Title = Title;
		}

		public void Initialize(absScene startScene)
		{
			Raylib.InitWindow(StartWidth, StartHeight, Title);
			ScreenWidth = StartWidth;
			ScreenHeight = StartHeight;
			currentSceneId = AddScene(startScene);

			currentScene.SetupScene();
			Loop();

		}

		public int AddScene(absScene scene)
		{
			int id = ++lastSceneId;
			NavStack.Add(id, scene);
			return id;

		}
		public void removeScene(int id)
		{
			NavStack.Remove(id);

			if (currentSceneId == id)
			{
				currentSceneId = NavStack.Last().Key;
			}



		}
		public void Loop()
		{

			while (running)
			{

				if (currentScene != null)
				{
					currentScene.Input();
					if (IsWindowResized())
					{
						this.ScreenWidth = GetScreenWidth();
						this.ScreenHeight = GetScreenHeight();
						currentScene.onWindowReisized(ScreenWidth, ScreenHeight);
					}
					currentScene.Render();
				}


			}
		}

		public absScene currentScene
		{
			get
			{
				if (NavStack.ContainsKey(currentSceneId))
					return NavStack[currentSceneId];
				else return null;
			}

		}

		public int ScreenWidth { get; private set; }
		public int ScreenHeight { get; private set; }
	}

	public abstract class absScene
	{

		protected Camera2D cam;
		protected SceneManager manager;
		public virtual void onWindowReisized(int newWidth, int newHeight)
		{

		}


		public absScene(SceneManager coreManager)
		{
			manager = coreManager;
		}


		public virtual void SetupScene()
		{

		}



		public unsafe virtual void Input()
		{

		}

		public abstract void Render();

		public virtual void CleanUp() { }

	}


	public class LevelPlayManager
	{
		public LemHandler lemHandler { get; set; }
		public int[] mask;
		public Image imgLevel;
		//public Texture texLevel;
		public Texture texLevel;
 
		public Image imgPhysics;
		//public Texture texPhysics;
		public Texture texPhysics;
		public int size;
		public bool imgInvalid = false;

	}
	public class LevelScene : absScene
	{

		public int ow = 320;
		public int oh = 240;
		public float rW;

		public LevelPlayManager pm = new LevelPlayManager();

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
			SetMousePosition(manager.ScreenWidth / 2, manager.ScreenHeight / 2);
			screenWM = manager.ScreenWidth * 0.05f;
			base.onWindowReisized(newWidth, newHeight);

		}

		public LevelScene(SceneManager coreManager, LevelPack.LevelData _level) : base(coreManager)
		{
			pm = new LevelPlayManager();

			pm.lemHandler = new LemHandler(_level, pm);

			pm.size = pm.lemHandler.lvl.Width * pm.lemHandler.lvl.Height;
			onWindowReisized(manager.ScreenWidth, manager.ScreenHeight);
		}


		Thread threadLemmingControl;
		ThreadStart tsLemmingControl;
		bool thAllowWork = false;

		public override void SetupScene()
		{
			using (var tchterrain = new TextureCacheData())
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


			tsLemmingControl = new ThreadStart(() => { thmUpdateLemmings(ref cam); });
			threadLemmingControl = new Thread(tsLemmingControl);
			threadLemmingControl.Name = "LEMMING CONTROL";
			threadLemmingControl.Start();

				startAddThread();
		}
		int waitcount = 0;
		void startAddThread()
		{

			Thread tadd = new Thread(new ThreadStart(() =>
			{

				Random r = new Random();

				while (pm.lemHandler.lems.Count < 50000)
				{
						pm.lemHandler.AddLemming(new Lemming(this.pm) { LemX = (r.Next(100, 340)), LemY = 30, LemAction = Lemming.enmLemmingState.FALLING });
					Thread.Sleep(1 * 5);
				}

			}));

			tadd.Start();
		}

		bool gameFinished;
		static int handleclock = 10; // 60 is normal
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
			var img = GenImageColor(pm.lemHandler.lvl.Width, pm.lemHandler.lvl.Height,WHITE);

			ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			//ImageFormat(ref img, (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			//ImageAlphaClear(ref img, Color.WHITE, 1f);
			//ImageAlphaClear(ref img, BLANK, 1f);
			ImageAlphaClear(ref img, BLANK, 1f);
			
			pm.mask = new int[pm.size];
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
						if (ix > pm.size - 1 || ix < 0) continue;
						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && (pm.mask[ix] & 1) == 1)
						{
							//	continue;
						}

						var imgToUse = tc.imgMain;

						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_HORIZONTAL) && i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_VERTICAL)) imgToUse = tc.imgVertHorizFlip;
						else if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_HORIZONTAL)) imgToUse = tc.imgHorizFlip;
						else if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.FLIP_VERTICAL)) imgToUse = tc.imgVertFlip;

						var colsrc = GetImageColor(imgToUse, x, y);

						byte a = colsrc.a;
						byte r = colsrc.r;
						byte g = colsrc.g;
						byte b = colsrc.b;



						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.ERASE))
						{

							if (a > 0)
							{
								ImageDrawPixel(ref img, xpos, ypos, BLANK);
								pm.mask[ix] = 0;

								continue;
							}
						}
						if (a > 0)
						{
							if (!(i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && (pm.mask[ix] & 1) == 1))
							{
								ImageDrawPixel(ref img, xpos, ypos, colsrc);

							}

							pm.mask[ix] |= 1;
							continue;
						}


					}
				}
			}
			pm.imgLevel = img;
			pm.texLevel = LoadTextureFromImage(img);
			pm.imgPhysics = GenImageColor(pm.lemHandler.lvl.Width, pm.lemHandler.lvl.Height, BLANK);
			//ImageFormat(ref pm.imgPhysics, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			ImageFormat(ref pm.imgPhysics, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			//ImageAlphaClear(ref pm.imgPhysics, BLANK, 1f);
			ImageAlphaClear(ref pm.imgPhysics, BLANK, 1f);

			for (int y = 0; y < pm.lemHandler.lvl.Height; y++)
			{
				for (int x = 0; x < pm.lemHandler.lvl.Width; x++)
				{
					if ((pm.mask[y * pm.lemHandler.lvl.Width + x] & 1) == 1)
					{
						//ImageDrawPixel(ref pm.imgPhysics, x, y, LemHandler.col_phy_terr);
						fixed (Image* ptr = &pm.imgPhysics)
						{
							ImageDrawPixel(ptr, x, y, LemHandler.col_phy_terr);
						}
					}
				}
			}
			pm.texPhysics = LoadTextureFromImage(pm.imgPhysics);

		}

		List<Lemming> curLemmings = new List<Lemming>();
		int lastMframe = 0;
		public unsafe override void Input()
		{

			var thisPos = GetMousePosition();
			var delta = prevMouse - thisPos;
			var a = GetScreenToWorld2D(prevMouse, cam);

			if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
			{

				if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
					pm.lemHandler.AddLemming(new Lemming(pm) { LemX = (int)a.X, LemY = (int)a.Y, LemAction = Lemming.enmLemmingState.FALLING });
				else if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
				{

					pm.lemHandler.ApplyMaskSprite("BASHER", ++lastMframe % 4, (int)a.X, (int)a.Y, LemHandler.enmMaskDir.RIGHT);
				}
				else
				{
					/*ImageDrawCircle(ref pm.imgLevel, (int)a.X, (int)a.Y, 10, BLANK);
					ImageAlphaCrop(ref pm.imgLevel, 1f);

					ImageDrawCircle(ref pm.imgPhysics, (int)a.X, (int)a.Y, 10, BLANK);
					ImageAlphaCrop(ref pm.imgPhysics, 1f);
					//UnloadTexture(pm.texLevel);
					//pm.texLevel = LoadTextureFromImage(pm.imgLevel);

					Color* pixels = LoadImageColors(pm.imgLevel);
					UpdateTexture(pm.texLevel, pixels);
					UnloadImageColors(pixels);

					pixels = LoadImageColors(pm.imgPhysics);
					UpdateTexture(pm.texPhysics, pixels);
					UnloadImageColors(pixels);*/

					var um = pm.lemHandler.lems.Where(o => o.UnderMouse).ToList();
					foreach (var i in um)
					{
						//i.LemActionNext = Lemming.enmLemmingState.BASHING;
						pm.lemHandler.Transition(i, Lemming.enmLemmingState.BASHING);
					}

				}

			}
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
				cam.target.X += GetMouseDelta().X; // = GetScreenToWorld2D(cam.offset + delta, cam);
				if (cam.zoom > 1)
					cam.target.Y += GetMouseDelta().Y;
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


			Raylib.BeginDrawing();

			Raylib.ClearBackground(bg);

			BeginMode2D(cam);

			DrawTexture(pm.texLevel, 0, 0, WHITE);


			//DrawTexture(pm.texPhysics, 0, 0, WHITE);

			var a = GetScreenToWorld2D(prevMouse, cam);
			//DrawText($"{a}", (int)a.X, (int)a.Y, 12, WHITE);

			var c = GetImageColor(pm.imgLevel, (int)a.X, (int)a.Y);
			//	var hpa = Lemming.HasPixelAt((int)a.X, (int)a.Y, LevelImage);
			//foreach (var i in pm.lemHandler.getLemmingEnumerator().) i.Draw();
			////for (int i = 0; i < pm.lemHandler.lems.Count; i++)
			//{
			//	pm.lemHandler.lems[i].Draw();
			//}
			pm.lemHandler.HandleDraw();

			if (pm.lemHandler.lems.Count() > 0)
			//	DrawText($"LemmingCount: {pm.lemHandler.lems.Count()}\n{pm.lemHandler.lems.Last().LemAction}\n{GetFPS()}Fps.\n", (int)a.X + 10, (int)a.Y + 10, 18, RED);
			//DrawText($"LEMS:{pm.lemHandler.lems.Last().LemAction}\n{GetFPS()}\n{c}\n{hpa}\n{a}", (int)a.X + 10, (int)a.Y + 20, 12, WHITE);
			foreach (var i in curLemmings)
			{
				DrawRectangleLines((int)i.PositionalRectangle.x, (int)i.PositionalRectangle.y, (int)i.PositionalRectangle.width, (int)i.PositionalRectangle.height, GREEN);
			}

			//	t1.Draw();
			EndMode2D();
			Raylib.EndDrawing();
		}



	}

	public class LemHandler
	{
		public const int MAX_FALLDISTANCE = 62;
		public bool flagGameFinished = false;
		public int fCurrentIteration = 0;
		public int fClockFrame = 0;
		public int DelayEndFrames = 0;
		/*public Image imgLevel;
		public Image imgPhysics;*/
		public List<Lemming> lems { get; set; } = new List<Lemming>();
		public bool pause = false;
		public LevelPack.LevelData lvl { get; }
		public int TimePlay { get; private set; }
		/*	public Texture texLevel { get; internal set; }
			public Texture texPhysics { get; internal set; }*/

		public LevelPlayManager lpm { get; set; }

		public Dictionary<Lemming.enmLemmingState, Func<Lemming, bool>> LemmingMethods = new Dictionary<Lemming.enmLemmingState, Func<Lemming, bool>>();
		public static Dictionary<Lemming.enmLemmingState, int> LemmingMethodAnimFrames = new Dictionary<Lemming.enmLemmingState, int>()
			{
			{Lemming.enmLemmingState.NONE,0},{Lemming.enmLemmingState.WALKING,4},{Lemming.enmLemmingState.ASCENDING,1},
				{Lemming.enmLemmingState.DIGGING,16},{Lemming.enmLemmingState.CLIMBING,8},{Lemming.enmLemmingState.DROWNING,16},
				{Lemming.enmLemmingState.HOISTING,8},{Lemming.enmLemmingState.BUILDING,16},{Lemming.enmLemmingState.BASHING,16},
				{Lemming.enmLemmingState.MINING,24},{Lemming.enmLemmingState.FALLING,4},{Lemming.enmLemmingState.FLOATING,17},
				{Lemming.enmLemmingState.SPLATTING,16},{Lemming.enmLemmingState.EXITING,8},{Lemming.enmLemmingState.VAPORIZING,14},
				{Lemming.enmLemmingState.BLOCKING,16},{Lemming.enmLemmingState.SHRUGGING,8},{Lemming.enmLemmingState.OHNOING,16},
				{Lemming.enmLemmingState.EXPLODING,1},{Lemming.enmLemmingState.TOWALKING,0},{Lemming.enmLemmingState.PLATFORMING,16},
				{Lemming.enmLemmingState.STACKING,8},{Lemming.enmLemmingState.STONING,16},{Lemming.enmLemmingState.STONEFINISH,1},
				{Lemming.enmLemmingState.SWIMMING,8},{Lemming.enmLemmingState.GLIDING,17},{Lemming.enmLemmingState.FIXING,16},
				{Lemming.enmLemmingState.CLONING,0},{Lemming.enmLemmingState.FENCING,16},{Lemming.enmLemmingState.REACHING,8},
				{Lemming.enmLemmingState.SHIMMYING,20},{Lemming.enmLemmingState.JUMPING,13},{Lemming.enmLemmingState.DEHOISTING,7},
				{Lemming.enmLemmingState.SLIDING,1},{Lemming.enmLemmingState.LASERING,12},
			};

		public static Dictionary<Lemming.enmLemmingState, SpriteDefinition> lemmingSpriteDefs = new Dictionary<Lemming.enmLemmingState, SpriteDefinition>()
			{
				{ Lemming.enmLemmingState.WALKING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 8, Name="WALKING", Path="styles/lemmings/walker.png" , WidthFromCenter = 8  } },
				{ Lemming.enmLemmingState.FALLING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 4, Name="FALLING", Path="styles/lemmings/faller.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.ASCENDING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 1, Name="ASCENDING", Path="styles/lemmings/ascender.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.FLOATING,new SpriteDefinition(){ CellH = 16, CellW = 16, Cols =2, Rows = 17, Name="FLOATING", Path="styles/lemmings/floater.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.CLIMBING,new SpriteDefinition(){ CellH = 12, CellW = 16, Cols =2, Rows = 8, Name="CLIMBING", Path="styles/lemmings/climber.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.HOISTING,new SpriteDefinition(){ CellH = 12, CellW = 16, Cols =2, Rows = 8, Name="HOISTING", Path="styles/lemmings/hoister.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.BASHING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 32, Name="BASHING", Path="styles/lemmings/basher.png",  WidthFromCenter = 8 } },
			};

		public Dictionary<string, SpriteDefinition> maskSpriteDefs = new Dictionary<string, SpriteDefinition>()
			{
				{"BASHER",new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 4, Name="BASHER", Path="styles/mask/basher.png", WidthFromCenter = 8 } }
			};

		public static Color col_phy_terr = new Color(0, 10, 0, 255);
		public static Color col_phy_nocut = new Color(0, 11, 0, 255);
		public LemHandler(LevelPack.LevelData levelData, LevelPlayManager _lpm)
		{
			lvl = levelData;
			lpm = _lpm;
			TimePlay = lvl.Time_Limit;
			LemmingMethods.Add(Lemming.enmLemmingState.WALKING, HandleWalking);
			LemmingMethods.Add(Lemming.enmLemmingState.ASCENDING, HandleAscending);
			LemmingMethods.Add(Lemming.enmLemmingState.FALLING, HandleFalling);
			LemmingMethods.Add(Lemming.enmLemmingState.FLOATING, HandleFloating);
			LemmingMethods.Add(Lemming.enmLemmingState.CLIMBING, HandleClimbing);
			LemmingMethods.Add(Lemming.enmLemmingState.HOISTING, HandleHoisting);
			LemmingMethods.Add(Lemming.enmLemmingState.BASHING, HandleBashing);
		}

		private object lemlock = new object();
		public void AddLemming(Lemming l)
		{

			lock (lemlock)
			{
				if (lems == null) lems = new List<Lemming>();

				lems.Add(l);
			}

			long memaf = GC.GetTotalMemory(true);




		}

		public void HandleDraw()
		{
			lock (lemlock)
			{
				foreach (var i in lems)
				{
					i.Draw();
				}
			}
		}


		public void UpdateLemmings(ref Camera2D cam)
		{
			if (flagGameFinished)
			{
				//handleExit
				Exit();
			}
			if (!pause)
			{
				CheckForGameFinished();
				CheckForQueuedAction();
				CheckForReplayAction();

				IncrementIteration();
				CheckReleaseLemming();
				CheckLemmings();
				CheckUpdateNuking();
				UpdateGadgets();
			}

			UpdateLemmingPositionalRectangles(ref cam);

		}

		private void UpdateLemmingPositionalRectangles(ref Camera2D cam)
		{

			//	DrawRectangleLinesEx(new Rectangle(LemX - (spriteDef.CellW / 2), LemY - spriteDef.CellH, spriteDef.CellW, spriteDef.CellH), 1f, ToDraw);
			//	DrawRectangleLinesEx(new Rectangle(LemX  - spriteDef.WidthFromCenter/2, LemY- spriteDef.CellH, spriteDef.WidthFromCenter, spriteDef.CellH), 1f, ToDraw);
			//
			int i = 0;
			var x = GetMousePosition();
			var a = GetScreenToWorld2D(x, cam);

			for (i = 0; i < lems.Count; i++)
			{
				var L = lems[i];
				L.UnderMouse = false;
				L.PositionalRectangle = new Rectangle(L.LemX - L.spriteDef.WidthFromCenter / 2, L.LemY - L.spriteDef.CellH, L.spriteDef.WidthFromCenter, L.spriteDef.CellH);
				if (CheckCollisionPointRec(a, L.PositionalRectangle))
				{
					L.UnderMouse = true;
				}
			}
		}

		public void CheckLemmings()
		{

			int i = 0;
			Lemming currentLemming = null;
			bool continueWithLem = false;

			//handle zombie stuff, (not yet!) TODO

			for (i = 0; i < lems.Count; i++)
			{
				currentLemming = lems[i];
				continueWithLem = true;
				if (currentLemming.LemParticleTimer >= 0)
				{
					currentLemming.LemParticleTimer--;
				}

				if (currentLemming.LemRemoved) continue;

				//handle teleporting

				//handle explosion coutndown

				//let lemmings move
				//call handleLem
				continueWithLem = HandleLemming(currentLemming);

				if (continueWithLem)
					CheckTiggerArea(currentLemming);
				
			}
		}



		private void CheckForGameFinished()
		{



		}

		public void Exit() { }
		public void CheckAdjustSpawnInterval() { }
		public void CheckForQueuedAction() { }
		public void CheckForReplayAction() { }
		public void IncrementIteration()
		{
			fCurrentIteration++;
			fClockFrame++;
			if (DelayEndFrames > 0) DelayEndFrames--;

			//handle particles

			//handle time up 
			if (fClockFrame == 17)
			{
				fClockFrame = 0;
				if (TimePlay > -5999) TimePlay--;
				if (TimePlay == 0) { /*times up*/ }

			}

		}
		public void CheckReleaseLemming() { }
		public void CheckUpdateNuking() { }
		public void UpdateGadgets() { }

		private void CheckTiggerArea(Lemming currentLemming, bool IsPostTeleportCheck = false)
		{


			if (currentLemming.LemActionNext != Lemming.enmLemmingState.NONE)
			{
				Transition(currentLemming, currentLemming.LemActionNext);
			}
		}


		public bool HandleLemming(Lemming l)
		{
			Lemming.enmLemmingState[] oneTimers = new Lemming.enmLemmingState[] { Lemming.enmLemmingState.HOISTING };

			bool r = false;
			l.LemXOld = l.LemX;
			l.LemYOld = l.LemY;
			l.lemDXOld = l.LemDX;
			l.LemActionOld = l.LemAction;
			l.LemActionNext = Lemming.enmLemmingState.NONE;
			l.fLemJupToHoistAdvance = false;
			l.LemFrame++;
			l.LemPhysicsFrame++;
			//3420
			if (l.LemPhysicsFrame > l.LemMaxPhysicsFrame)
			{
				l.LemPhysicsFrame = 0;
				if (l.LemAction == Lemming.enmLemmingState.FLOATING) l.LemPhysicsFrame = 9;

				if (oneTimers.Contains(l.LemAction)) l.LemEndOfAnimation = true;
			}
			var Action = LemmingMethods[l.LemAction];
			if (Action != null)
			{
				r = Action?.Invoke(l) ?? false;
			}
			return r;
		}

		public void Transition(Lemming L, Lemming.enmLemmingState NewAction, bool DoTurn = false)
		{
			int i = 0;
			bool OldIsStartingAction = false;

			if (DoTurn) TurnAround(L);

			if (NewAction == Lemming.enmLemmingState.TOWALKING) NewAction = Lemming.enmLemmingState.WALKING;

			//ToDo handle blockers

			if (!HasPixelAt(L.LemX, L.LemY) && NewAction == Lemming.enmLemmingState.WALKING)
				NewAction = Lemming.enmLemmingState.FALLING;


			if (L.LemAction == NewAction) return;

			//set initial fall according to previous skill 

			if (NewAction == Lemming.enmLemmingState.FALLING)
			{
				//ignore swimming TODO

				L.LemFallen = 1;
				if ((new Lemming.enmLemmingState[] { Lemming.enmLemmingState.WALKING, Lemming.enmLemmingState.BASHING }).Contains(L.LemAction))
				{
					L.LemFallen = 3;
				}

				//ToDo, handle mining, digging, blocking, jumping, and lazering

				L.LemTrueFallen = L.LemFallen;
			}

			//ToDo handle shyming, jumpin, clibmingsliding , dehosting :1470 -> :1510

			L.LemAction = NewAction;
			L.LemFrame = 0;
			L.LemPhysicsFrame = 0;
			L.LemEndOfAnimation = false;
			OldIsStartingAction = L.LemIsStartingAction; //ToDo
			L.LemIsStartingAction = true;
			L.LemInitialFall = false;

			L.LemMaxFrame = -1;
			L.LemMaxFrame = LemmingMethodAnimFrames[L.LemAction];
			//	L.LemPhysicsFrame = LemmingMethodAnimFrames[NewAction];
			L.LemMaxPhysicsFrame = LemmingMethodAnimFrames[NewAction] - 1;
			switch (L.LemAction)
			{
				case Lemming.enmLemmingState.ASCENDING:
					{
						L.LemAscended = 0;
						break;
					}
			}

		}

		public void TurnAround(Lemming L) => L.LemDX = -L.LemDX;

		public bool HandleBashing(Lemming L)
		{
			bool r = true;
			int LemDY = 0;
			int n = 0;
			bool continueWork = false;

			bool BasherIndestructableCheck(int x, int y, int direction)
			{
				//ToDo implement indestructable check
				return false;
			}

			void BasherTurn(Lemming L, bool SteelSound)
			{
				L.LemX -= L.LemDX;
				Transition(L, Lemming.enmLemmingState.WALKING, true);
				//cue sound effect
			}

			bool BasherStepUpCheck(int x, int y, int Direction, int Step)
			{
				bool Result = true;
				if (Step == -1)
				{
					if (((!HasPixelAt(x + Direction, y + Step - 1))
				 && HasPixelAt(x + Direction, y + Step)
				 && HasPixelAt(x + 2 * Direction, y + Step)
				 && HasPixelAt(x + 2 * Direction, y + Step - 1)
				 && HasPixelAt(x + 2 * Direction, y + Step - 2))
			 ) Result = false;

					if (((!HasPixelAt(x + Direction, y + Step - 2))
				 && HasPixelAt(x + Direction, y + Step)
				 && HasPixelAt(x + Direction, y + Step - 1)
				 && HasPixelAt(x + 2 * Direction, y + Step - 1)
				 && HasPixelAt(x + 2 * Direction, y + Step - 2)
			 )) Result = false;


					if ((HasPixelAt(x + Direction, y + Step - 2)
				 && HasPixelAt(x + Direction, y + Step - 1)
				 && HasPixelAt(x + Direction, y + Step)
			 )) Result = false;
				}
				else if (Step == -2)
				{
					if (((!HasPixelAt(x + Direction, y + Step))
				 && HasPixelAt(x + Direction, y + Step + 1)
				 && HasPixelAt(x + 2 * Direction, y + Step + 1)
				 && HasPixelAt(x + 2 * Direction, y + Step)
				 && HasPixelAt(x + 2 * Direction, y + Step - 1)
			 )) Result = false;

					if ((!HasPixelAt(x + Direction, y + Step - 1))
				 && HasPixelAt(x + Direction, y + Step)
				 && HasPixelAt(x + 2 * Direction, y + Step)
				 && HasPixelAt(x + 2 * Direction, y + Step - 1)
			 ) Result = false;

					if (HasPixelAt(x + Direction, y + Step - 1)
				 && HasPixelAt(x + Direction, y + Step))
					{

					}
					else
					{
						Result = false;
					}
				}

				return Result;
			}
			//TODO:  function DoTurnAtSteel(L: TLemming): Boolean;
			//4114
			if (L.LemPhysicsFrame.In(2, 3, 4, 5))
			{

				ApplyBashingMask(L, L.LemPhysicsFrame - 2);


			}

			if (L.LemPhysicsFrame == 5)
			{
				continueWork = false;

				for (n = 1; n < 14; n++)
				{//ToDo check for steel :4184
					if (HasPixelAt(L.LemX + n * L.LemDX, L.LemY - 6))
					{
						continueWork = true;
					}
					if (HasPixelAt(L.LemX + n * L.LemDX, L.LemY - 5))
					{
						continueWork = true;
					}
				}

				if (!continueWork)
				{
					if (HasPixelAt(L.LemX, L.LemY))
						Transition(L, Lemming.enmLemmingState.WALKING);
					else Transition(L, Lemming.enmLemmingState.FALLING);
				}
			}

			if (L.LemPhysicsFrame.In(11, 12, 13, 14, 15))
			{
				L.LemX += L.LemDX;
				LemDY = FindGroundPixel(L.LemX, L.LemY);
				L.dbgString = $"DY:{LemDY}";
				//ToDo: dehoist check

				if (LemDY == 4)
				{
					L.LemY += LemDY;
				}
				else if (LemDY == 3)
				{
					L.LemY += LemDY;
					Transition(L, Lemming.enmLemmingState.WALKING);
				}
				else if (LemDY.In(0, 1, 2))
				{
					//ToDO basher indestructable check :4233

					L.LemY += LemDY;
				}

			}


			return r;
		}

		public bool HandleWalking(Lemming L)
		{
			bool r = true;
			int LemDY = 0;

			L.LemX += L.LemDX;
			LemDY = FindGroundPixel(L.LemX, L.LemY);

			//handle sliders (ToDo)

			if (LemDY < -6)
			{
				if (L.LemIsClimber)
				{
					Transition(L, Lemming.enmLemmingState.CLIMBING);
				}
				else
				{
					TurnAround(L);
					L.LemX += L.LemDX;
				}
			}
			else if (LemDY < -2)
			{
				Transition(L, Lemming.enmLemmingState.ASCENDING);
				L.LemY += -2;
			}
			else if (LemDY < 1)
			{
				L.LemY += LemDY;
			}


			LemDY = FindGroundPixel(L.LemX, L.LemY);
			if (LemDY > 3)
			{
				L.LemY += 4;
				Transition(L, Lemming.enmLemmingState.FALLING);
			}
			else if (LemDY > 0)
			{
				L.LemY += LemDY;
			}


			return r;
		}
		public bool HandleAscending(Lemming L)
		{
			int dy = 0;
			bool r = true;

			while (dy < 2 && L.LemAscended < 5 && HasPixelAt(L.LemX, L.LemY - 1))
			{
				dy++;
				L.LemY--;
				L.LemAscended++;
			}

			if (dy < 2 && !HasPixelAt(L.LemX, L.LemY - 1))
			{
				L.LemActionNext = Lemming.enmLemmingState.WALKING;
			}
			else if ((L.LemAscended == 4 && HasPixelAt(L.LemX, L.LemY - 1) && HasPixelAt(L.LemX, L.LemY - 2)) ||
							(L.LemAscended >= 5 && HasPixelAt(L.LemX, L.LemY - 1)))
			{
				L.LemX -= L.LemDX;
				Transition(L, Lemming.enmLemmingState.FALLING, true);
			}

			return true;
		}
		public bool HandleFalling(Lemming L)
		{
			int currFalDistance = 0;
			int maxFallDistance = 3;
			bool r = true;

			bool IsFallFatal()
			{
				return (!L.LemIsFloater) && L.LemFallen > MAX_FALLDISTANCE;

			}

			bool CheckFloaterTransition()
			{
				bool r = false;

				if (L.LemIsFloater && L.LemTrueFallen > 16 && currFalDistance == 0)
				{
					Transition(L, Lemming.enmLemmingState.FLOATING);
					r = true;
				}

				return r;

			}
			if (CheckFloaterTransition()) return r;

			//check for floater or glider TODO





			//ToDo check for updraft

			//todo: check for floater/glider transition

			//move lem until hit the ground
			while (currFalDistance < maxFallDistance && !HasPixelAt(L.LemX, L.LemY))
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

			//we'd normally check for a splat here but we wont so lets just make them walk it off.

			L.LemActionNext = Lemming.enmLemmingState.WALKING;



			return r;
		}

		static int[] FloaterFallTable = new int[] { 3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
		public bool HandleFloating(Lemming L)
		{
			bool r = true;
			int MaxFallDist = FloaterFallTable[L.LemPhysicsFrame];

			//updraft todo
			int GPMax = Math.Max(FindGroundPixel(L.LemX, L.LemY), 0);
			if (MaxFallDist > GPMax)
			{
				//found solid terrain
				L.LemY += GPMax;
				L.LemActionNext = Lemming.enmLemmingState.WALKING;
			}
			else
			{
				L.LemY += MaxFallDist;
			}
			return r;

		}

		public bool HandleClimbing(Lemming L)
		{
			bool FoundClip = false;
			bool r = true;
			if (L.LemPhysicsFrame <= 3)
			{
				FoundClip = HasPixelAt(L.LemX - L.LemDX, L.LemY - 6 - L.LemPhysicsFrame)
		|| (HasPixelAt(L.LemX - L.LemDX, L.LemY - 5 - L.LemPhysicsFrame) && !L.LemIsStartingAction);

				if (L.LemPhysicsFrame == 0)
					FoundClip = FoundClip && HasPixelAt(L.LemX - L.LemDX, L.LemY - 7);

				if (FoundClip)
				{
					if (!L.LemIsStartingAction) L.LemY = L.LemY - L.LemPhysicsFrame + 3;

					//ToDo handle slider
					L.LemX -= L.LemDX;
					Transition(L, Lemming.enmLemmingState.FALLING, true);
					L.LemFallen++;

				}
				else if (!HasPixelAt(L.LemX, L.LemY - 7 - L.LemPhysicsFrame))
				{
					if (!(L.LemIsStartingAction && L.LemPhysicsFrame == 1))
					{
						L.LemY = L.LemY - L.LemPhysicsFrame + 2;
						L.LemIsStartingAction = false;
					}
					Transition(L, Lemming.enmLemmingState.HOISTING);
				}
			}
			else
			{
				L.LemY--;
				L.LemIsStartingAction = false;
				FoundClip = HasPixelAt(L.LemX - L.LemDX, L.LemY - 7);

				if (L.LemPhysicsFrame == 7)
					FoundClip = FoundClip && HasPixelAt(L.LemX - L.LemDX, L.LemY - 7);

				if (FoundClip)
				{
					L.LemY--;
					//ToDo slider

					L.LemX -= L.LemDX;
					Transition(L, Lemming.enmLemmingState.FALLING, true);
				}
			}


			return r;
		}
		public bool HandleHoisting(Lemming L)
		{
			bool r = true;
			if (L.LemEndOfAnimation)
			{
				Transition(L, Lemming.enmLemmingState.WALKING);
			}
			else if (L.LemPhysicsFrame == 1 && L.LemIsStartingAction)
			{
				L.LemY -= 1;
			}
			else if (L.LemPhysicsFrame <= 4)
			{
				L.LemY -= 2;
			}
			return r;
		}

		public unsafe void ApplyBashingMask(Lemming L, int maskFrame)
		{
			Rectangle S;
			Rectangle D;

			S = new Rectangle(0, 0, 16, 10);
			S.x = L.LemDX == 1 ? 16 : 0;
			S.y = maskFrame * 10;


			D = new Rectangle();
			D.x = L.LemX - 8;
			D.y = L.LemY - 10;
			D.width = 16;
			D.height = 10;


			if (!S.CheckRectCopy(D))
			{
				//throw new Exception("rect does not match");
			}
			L.dbgString = $"{maskFrame}";
			string maskname = "BASHER";
			//ApplyMaskSprite("BASHER", maskFrame, S, D, enmMaskDir.RIGHT);
			//	ApplyMaskSprite("BASHER", maskFrame, L.LemX - 8, L.LemY - 10, enmMaskDir.RIGHT);
			//ImageDraw(ref lpm.imgLevel, maskSpriteDefs[maskname], S, D, RED);
			//ImageDraw(ref lpm.imgLevel, maskSpriteDefs[maskname].imgSprite, S, D, RED);
			var msd = maskSpriteDefs[maskname];
			msd.initCheck();

			ImageDrawCS_ApplyAlphaMask(ref lpm.imgLevel, msd.imgSprite, S, D, BLANK);
			ImageDrawCS_ApplyAlphaMask(ref lpm.imgPhysics, msd.imgSprite, S, D, BLANK);


			lpm.imgInvalid = true;
		}


		public enum enmMaskDir { NONE, LEFT, RIGHT };//3006
		List<int> t1 = new List<int>();
		public unsafe void ApplyMaskSprite(string maskname, int maskFrame, int PosX, int PosY, enmMaskDir maskDir = enmMaskDir.NONE, bool invalidate = true)
		{


			if (maskSpriteDefs.ContainsKey(maskname))
			{
				var msd = maskSpriteDefs[maskname];
				msd.initCheck();
				int mw = msd.CellW;
				int mh = msd.CellH;
				int smx = maskDir == enmMaskDir.RIGHT ? mw : 0;
				int smy = mh * maskFrame;

				int emx = smx + mw;
				int emy = smy + mh;
				int iW = lpm.imgPhysics.width;
				int iH = lpm.imgPhysics.height;

				int ry = 0;
				int rx = 0;

				//ImageDraw(ref lpm.imgLevel, msd.imgSprite, new Rectangle(smx,smy,emx,emy), new Rectangle(PosX,PosY,mw,mh), Color.WHITE);


				int _y = 0; int _x = 0;
				for (int y = smy; y < emy; y++)
				{
					for (int x = smx; x < emx; x++)
					{
						Color clr = GetImageColor(msd.imgSprite, x, y);

						ImageDrawPixel(ref lpm.imgLevel, _x, _y, clr);

						_x++;
					}
					_y++;
				}

				lpm.imgInvalid = true;
				return;

				for (int y = smy; y < emy; y++)
				{
					int mapY = PosY + ry;
					if (mapY < 0) continue;
					if (mapY > iH) break;
					for (int x = smx; x < emx; x++)
					{
						int mapX = PosX + rx;

						if (mapX < 0) continue;
						if (mapX > iW) break;

						Color clrMask = GetImageColor(msd.imgSprite, x, y);
						if (clrMask.a == 0)
						{

						}
						ImageDrawPixel(ref lpm.imgLevel, mapX, mapY, clrMask);
						ImageDrawPixel(ref lpm.imgPhysics, mapX, mapY, clrMask);
						rx++;
					}
					ry++;
				}






				if (invalidate)
					lpm.imgInvalid = true;

			}

		}

		//750 rendering
		public unsafe void RemoveTerrain(int x, int y, int w, int h)
		{
			//	Debug.WriteLine($"{x},{y},{w},{h}");
			Color* terrPtr;
			Color* physPtr;
			int mapW = lpm.imgPhysics.width;
			int mapH = lpm.imgPhysics.height;
			//terrPtr = LoadImageColors(lpm.imgLevel);
			//physPtr = LoadImageColors(lpm.imgPhysics);
			int cx = 0; int cy = 0;

			Color blank = new Color(0, 0, 0, 0);
			for (cy = y; cy < y + h - 1; cy++)
			{
				if (cy < 0) continue;
				if (cy >= mapH) break;
				for (cx = x; cx < x + w - 1; cx++)
				{
					if (cx < 0) continue;
					if (cx >= mapW) break;
					//((Color*)lpm.imgLevel.data)[cy * mapW + cx] = blank;
					//((Color*)terrPtr)[cy * mapW + cx] = blank;

					ImageDrawPixel(ref lpm.imgLevel, cx, cy, BLANK);
					ImageDrawPixel(ref lpm.imgPhysics, cx, cy, BLANK);

				}
			}

			//UpdateTexture(lpm.texLevel, terrPtr);
			//UpdateTexture(lpm.texPhysics, physPtr);
			//UnloadImageColors(terrPtr);
			//UnloadImageColors(physPtr);
			lpm.imgInvalid = true;
		}


		public int FindGroundPixel(int x, int y)
		{
			int r = 0;

			if (HasPixelAt(x, y))
			{
				while (HasPixelAt(x, y + r - 1) && (r > -7))
				{
					r--;
				}
			}
			else
			{
				r++;
				while (!HasPixelAt(x, y + r) && (r < 4))
				{
					r++;
				}
			}
			return r;

		}

		public bool HasPixelAt(int x, int y)
		{
			//return GetImageColor(imgLevel, x, y).a == 255;
			//return GetImageColor(imgPhysics, x, y).a == 255;
			return GetImageColor(lpm.imgPhysics, x, y).a == 255;
		}


	}




	public class Lemming
	{

		#region sprite stuff
		public int spriteH = 10;
		public int spriteW = 16;
		public int spriteRows = 8;
		public int spriteCols = 1;
		public Vector2 frameSize = new Vector2(32, 80);
		public string dbgString = "~";

		#endregion

		public LevelPlayManager lpm { get; }

		public enum enmLemmingState
		{
			NONE, WALKING, ASCENDING, DIGGING, CLIMBING, DROWNING,
			HOISTING, BUILDING, BASHING, MINING, FALLING, FLOATING,
			SPLATTING, EXITING, VAPORIZING, BLOCKING, SHRUGGING,
			OHNOING, EXPLODING, TOWALKING, PLATFORMING,
			STACKING, STONING, STONEFINISH, SWIMMING,
			GLIDING, FIXING, FENCING, REACHING, SHIMMYING,
			JUMPING, DEHOISTING, SLIDING, LASERING,
			CLONING,
		}

		public enum lemmingstate
		{
			none, walking, falling, ascending
		}

		//	Texture spriteTex;


		private Texture getSpriteTexture(enmLemmingState act)
		{
			if (LemHandler.lemmingSpriteDefs.ContainsKey(this.LemAction))
			{
				var x = LemHandler.lemmingSpriteDefs[this.LemAction];
				x.initCheck();
				/*if (!x.TextureSetup)
				{
					x.Texture = LoadTexture(x.Path);
					x.TextureSetup = true;
				}*/
				return x.Texture;
			}
			else
			{
				var x = LemHandler.lemmingSpriteDefs[enmLemmingState.WALKING];
				/*	if (!x.TextureSetup)
					{
						x.Texture = LoadTexture(x.Path);
					}*/
				x.initCheck();
				return x.Texture;


			}

		}
		public Texture spriteTex
		{
			get
			{
				return getSpriteTexture(this.LemAction);

			}
		}

		public SpriteDefinition spriteDef
		{
			get
			{
				if (LemHandler.lemmingSpriteDefs.ContainsKey(this.LemAction))
				{
					return LemHandler.lemmingSpriteDefs[this.LemAction];
				}
				else
				{
					return LemHandler.lemmingSpriteDefs[enmLemmingState.WALKING];
				}
			}
		}
		public static Dictionary<string, Texture> lemTextures = new Dictionary<string, Texture>();

		public void checkSpriteInit(enmLemmingState action, ref Texture tex)
		{


			if (tex.height == 0)
			{
				if (LemHandler.lemmingSpriteDefs.ContainsKey(this.LemAction))
				{
					var x = LemHandler.lemmingSpriteDefs[this.LemAction];
					if (!lemTextures.ContainsKey(x.Name))
						lemTextures.Add(x.Name, LoadTexture(x.Path));
				}

			}
		}
		public int LemX = 0;
		public int LemY = 0;

		public int LemParticleTimer { get; set; }
		public bool UnderMouse = false;

		public int LemMaxPhysicsFrame = 0;

		//	public int LemDY = -1;

		public int LemXOld = 0;
		public int LemYOld = 0;

		public int LemDX = 1;
		public enmLemmingState LemAction = enmLemmingState.NONE;
		public enmLemmingState LemActionOld = enmLemmingState.NONE;
		public enmLemmingState LemActionNext = enmLemmingState.NONE;

		public bool fLemJupToHoistAdvance = false;



		public Lemming(LevelPlayManager _lpm)
		{
			this.lpm = _lpm;


		}












		//public int framecounter = 0;
		internal bool LemRemoved;
		internal int lemDXOld;
		internal int LemFrame;
		internal int LemPhysicsFrame;
		public bool LemIsClimber = true;
		public bool LemIsFloater = true;
		internal int LemAscended = 0;
		internal int LemFallen = 0;
		internal int LemTrueFallen = 0;
		internal bool LemEndOfAnimation;
		internal bool LemIsStartingAction;
		internal bool LemInitialFall;
		internal int LemMaxFrame;

		public void Draw(Vector2 vec) => Draw((int)vec.X, (int)vec.Y);
		public void Draw(int _x, int _y)
		{
			LemX = _x;
			LemY = _y;

		}



		int frame = 0;
		int maxFrame = 8;
		internal Rectangle PositionalRectangle;

		public void Draw()
		{
			Rectangle curFrame = new Rectangle(0, 0, spriteDef.CellW, spriteDef.CellH);

			/*LemFrame++;
			if (LemFrame > LemMaxFrame)
			{
				frame++;
				if (frame >= spriteDef.Rows)
				{
					frame = 0;
				}
				LemFrame = 0;

			}*/


			curFrame.y = spriteDef.CellH * LemPhysicsFrame;

			if (LemDX < 0)
				curFrame.x = 0;
			else
				curFrame.x = spriteDef.CellW;

			Color ToDraw = WHITE;
			if (UnderMouse) ToDraw = RED;

			//	DrawTextureRec(spriteTex, curFrame, new Vector2(LemX - 8, LemY - 10), ToDraw);
			DrawTextureRec(spriteTex, curFrame, new Vector2(LemX - spriteDef.WidthFromCenter, LemY - spriteDef.CellH), ToDraw);
		//	DrawText(dbgString, LemX - 5, LemY - 20, 10, WHITE);
			//DrawRectangleLinesEx(new Rectangle(LemX  - spriteDef.WidthFromCenter/2, LemY- spriteDef.CellH, spriteDef.WidthFromCenter, spriteDef.CellH), 1f, ToDraw);
		}


	}

	public class SpriteDefinition
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public Texture Texture;
		public Image imgSprite;
		public bool CanBeDisposed = false;

		public bool TextureSetup = false;
		public bool ImageSetup = false;
		public int Rows { get; set; }
		public int Cols { get; set; }
		public int CellW { get; set; }
		public int CellH { get; set; }

		public int WidthFromCenter { get; set; }

		public virtual void initCheck()
		{
			if (!TextureSetup)
			{
				imgSprite = LoadImage(Path);
		/*		if (imgSprite.format != PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8)
					ImageFormat(ref imgSprite, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);*/
				Texture = LoadTextureFromImage(imgSprite);
				TextureSetup = true;
			}
		}

	}

	public class MaskSpriteDefinition : SpriteDefinition
	{
		public int[] mask;
		public bool maskSetup = false;
		public override void initCheck()
		{
			base.initCheck();
			if (!maskSetup)
			{//Todo
				maskSetup = true;
			}
		}
	}
}
