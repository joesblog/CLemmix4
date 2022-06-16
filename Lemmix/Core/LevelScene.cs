using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Numerics;
using System.Threading;

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



		public virtual void Input()
		{

		}

		public abstract void Render();

		public virtual void CleanUp() { }

	}

	public class LevelScene : absScene
	{

		public int ow = 320;
		public int oh = 240;
		public float rW;

		public LemHandler lemHandler { get; }
		//public LevelPack.LevelData lvl { get; }
		public int size { get; }
		public int[] mask { get; private set; }
		public Image LevelImage { get; private set; }
		public Texture2D LevelTexture { get; private set; }
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
			lemHandler = new LemHandler(_level);

			size = lemHandler.lvl.Width * lemHandler.lvl.Height;
			onWindowReisized(manager.ScreenWidth, manager.ScreenHeight);
		}


		Thread threadLemmingControl;
		ThreadStart tsLemmingControl;
		bool thAllowWork = false;

		public override void SetupScene()
		{
			using (var tchterrain = new TextureCacheData())
			{
				cam = new Camera2D(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0), 0, 1);
				cam.zoom = rW;
				cam.target = GetScreenToWorld2D(cam.offset + new System.Numerics.Vector2(lemHandler.lvl.Start_X, lemHandler.lvl.Start_Y - 80), cam);
				foreach (var i in lemHandler.lvl.Terrain.GroupBy(o => o + o.Style).Select(o => o.First()))
				{
					var op = tchterrain[i];
				}
				SetTargetFPS(60);
				SetMousePosition(manager.ScreenWidth / 2, manager.ScreenHeight / 2);
				SetupLevelLayout(tchterrain);
				lemHandler.LevelImage = this.LevelImage;
				//t1 = new LemSprite(this.LevelImage) { LemX = 230, LemY = 53 };
				Random r = new Random();

				//lemHandler.lems = new List<Lemming>();
				for (int i = 0; i < 3; i++)
				{
					lemHandler.AddLemming(new Lemming(this.LevelImage) { LemX = (r.Next(100, 300)), LemY = 30, LemAction = Lemming.enmLemmingState.FALLING });
					//if (i % 2 == 0) lemHandler.lems.Last().LemDX = -1; else lemHandler.lems.Last().LemDX = 1;
				}

			}
			tsLemmingControl = new ThreadStart(() => { thmUpdateLemmings(ref cam); });
			threadLemmingControl = new Thread(tsLemmingControl);
			threadLemmingControl.Name = "LEMMING CONTROL";
			threadLemmingControl.Start();

		//	startAddThread();
		}
		int waitcount = 0;
		void startAddThread()
		{

			Thread tadd = new Thread(new ThreadStart(() =>
			{

				Random r = new Random();

				while (lemHandler.lems.Count < 5000)
				{
					if (waitcount++ > 100)
					{
						lemHandler.AddLemming(new Lemming(this.LevelImage) { LemX = (r.Next(100, 340)), LemY = 30, LemAction = Lemming.enmLemmingState.FALLING });
					}
					Thread.Sleep(1 * 50);
				}

			}));

			tadd.Start();
		}

		bool gameFinished;
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
					lemHandler.UpdateLemmings(ref cam);
					//adjust spawn check
					//queued action
					//replay action

					//clear shadows


					Thread.Sleep(60);

				}
				else
				{
					Thread.Sleep(1);
				}

			}
		}

		void SetupLevelLayout(TextureCacheData tchterrain)
		{
			var img = GenImageColor(lemHandler.lvl.Width, lemHandler.lvl.Height, WHITE);
			ImageFormat(ref img, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
			//ImageAlphaClear(ref img, Color.WHITE, 1f);
			ImageAlphaClear(ref img, Color.BLANK, 1f);

			mask = new int[size];
			foreach (var i in lemHandler.lvl.Terrain)
			{
				var tc = tchterrain[i];
				for (int y = 0; y < tc.Height; y++)
				{
					int ypos = y + i.Y;
					for (int x = 0; x < tc.Width; x++)
					{
						int xpos = x + i.X;
						int ix = ypos * lemHandler.lvl.Width + xpos;
						int locix = y * tc.Width + x;
						if (ix > size - 1 || ix < 0) continue;
						if (i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && (mask[ix] & 1) == 1)
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
								ImageDrawPixel(ref img, xpos, ypos, Color.BLANK);
								mask[ix] = 0;

								continue;
							}
						}
						if (a > 0)
						{
							if (!(i.Flags.HasFlag(LevelPack.LevelData.LevelTerrain.FlagsTerrain.NO_OVERWRITE) && (mask[ix] & 1) == 1))
							{
								ImageDrawPixel(ref img, xpos, ypos, colsrc);

							}

							mask[ix] |= 1;
							continue;
						}


					}
				}
			}
			LevelImage = img;
			LevelTexture = LoadTextureFromImage(img);


		}

		List<Lemming> curLemmings = new List<Lemming>();
		public override void Input()
		{

			var thisPos = GetMousePosition();
			var delta = prevMouse - thisPos;
			var a = GetScreenToWorld2D(prevMouse, cam);

			if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
			{


				lemHandler.AddLemming(new Lemming(LevelImage) { LemX = (int)a.X, LemY = (int)a.Y, LemAction = Lemming.enmLemmingState.FALLING });

			}
			if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_MIDDLE))
			{
				cam.zoom = rW;
				cam.offset = new Vector2(0, 0);
				cam.target = GetScreenToWorld2D(cam.offset + new System.Numerics.Vector2(lemHandler.lvl.Start_X, lemHandler.lvl.Start_Y - 80), cam);
			}
			//curLemmings.Clear();

			if (IsKeyPressed(KeyboardKey.KEY_P))
			{
				lemHandler.pause = !lemHandler.pause;
			}

			cam.zoom += GetMouseWheelMove();
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





		public override void Render()
		{

			if (!thAllowWork) thAllowWork = true;



			DrawText($"LemmingCount: {lemHandler.lems.Count()}", 0, 0, 24, WHITE);

			Raylib.BeginDrawing();

			Raylib.ClearBackground(bg);

			BeginMode2D(cam);

			DrawTexture(LevelTexture, 0, 0, WHITE);

			var a = GetScreenToWorld2D(prevMouse, cam);
			//DrawText($"{a}", (int)a.X, (int)a.Y, 12, WHITE);

			var c = GetImageColor(LevelImage, (int)a.X, (int)a.Y);
			var hpa = Lemming.HasPixelAt((int)a.X, (int)a.Y, LevelImage);
			//foreach (var i in lemHandler.getLemmingEnumerator().) i.Draw();
			////for (int i = 0; i < lemHandler.lems.Count; i++)
			//{
			//	lemHandler.lems[i].Draw();
			//}
			lemHandler.HandleDraw();

			//DrawText($"LEMS:{lemHandler.lems.Last().LemAction}\n{GetFPS()}\n{c}\n{hpa}\n{a}", (int)a.X + 10, (int)a.Y + 20, 12, WHITE);
			foreach (var i in curLemmings)
			{
				DrawRectangleLines((int)i.PositionalRectangle.x, (int)i.PositionalRectangle.y, (int)i.PositionalRectangle.width, (int)i.PositionalRectangle.height, Color.GREEN);
			}
			//	t1.Draw();
			EndMode2D();
			Raylib.EndDrawing();
		}


		public class LemHandler
		{
			public const int MAX_FALLDISTANCE = 62;
			public bool flagGameFinished = false;
			public int fCurrentIteration = 0;
			public int fClockFrame = 0;
			public int DelayEndFrames = 0;
			public Image LevelImage;
			public List<Lemming> lems { get; set; } = new List<Lemming>();
			public bool pause = false;
			public LevelPack.LevelData lvl { get; }
			public int TimePlay { get; private set; }
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
				{ Lemming.enmLemmingState.WALKING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 8, Name="WALKING", Path="styles/walker.png" , WidthFromCenter = 8  } },
				{ Lemming.enmLemmingState.FALLING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 4, Name="FALLING", Path="styles/faller.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.ASCENDING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 1, Name="ASCENDING", Path="styles/ascender.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.FLOATING,new SpriteDefinition(){ CellH = 16, CellW = 16, Cols =2, Rows = 17, Name="FLOATING", Path="styles/floater.png",  WidthFromCenter = 8 } },
			};
			public LemHandler(LevelPack.LevelData levelData)
			{
				lvl = levelData;

				TimePlay = lvl.Time_Limit;
				LemmingMethods.Add(Lemming.enmLemmingState.WALKING, HandleWalking);
				LemmingMethods.Add(Lemming.enmLemmingState.ASCENDING, HandleAscending);
				LemmingMethods.Add(Lemming.enmLemmingState.FALLING, HandleFalling);
				LemmingMethods.Add(Lemming.enmLemmingState.FLOATING, HandleFloating);
			}

			private object lemlock = new object();
			public void AddLemming(Lemming l)
			{
				lock (lemlock)
				{
					if (lems == null) lems = new List<Lemming>();

					lems.Add(l);
				}
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
				L.LemPhysicsFrame = LemmingMethodAnimFrames[NewAction];
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
				return GetImageColor(LevelImage, x, y).a == 255;
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


			#endregion
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

			//	Texture2D spriteTex;


			private Texture2D getSpriteTexture(enmLemmingState act)
			{
				if (LemHandler.lemmingSpriteDefs.ContainsKey(this.LemAction))
				{
					var x = LemHandler.lemmingSpriteDefs[this.LemAction];
					if (!x.TextureSetup)
					{
						x.Texture = LoadTexture(x.Path);
						x.TextureSetup = true;
					}
					return x.Texture;
				}
				else
				{
					var x = LemHandler.lemmingSpriteDefs[enmLemmingState.WALKING];
					if (!x.TextureSetup)
					{
						x.Texture = LoadTexture(x.Path);
					}
					return x.Texture;


				}

			}
			public Texture2D spriteTex
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
			public static Dictionary<string, Texture2D> lemTextures = new Dictionary<string, Texture2D>();

			public void checkSpriteInit(enmLemmingState action, ref Texture2D tex)
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
			public Image LvlImg { get; }
			public int LemParticleTimer { get; set; }
			public bool UnderMouse { get; internal set; }
			public int LemMaxPhysicsFrame = 0;

			//	public int LemDY = -1;

			public int LemXOld = 0;
			public int LemYOld = 0;

			public int LemDX = 1;
			public enmLemmingState LemAction = enmLemmingState.NONE;
			public enmLemmingState LemActionOld = enmLemmingState.NONE;
			public enmLemmingState LemActionNext = enmLemmingState.NONE;

			public bool fLemJupToHoistAdvance = false;



			public Lemming(Image lvlImg)
			{

				/*	if (!lemTextures.ContainsKey("walker"))
						lemTextures.Add("walker", LoadTexture("styles/walker.png"));*/

				//	spriteTex = lemTextures["walker"];

				//spriteTex = LoadTexture("styles/walker.png");
				//lemTextures.Add("walker", "styles/walker.png");
				LvlImg = lvlImg;
			}










			public static int FindGroundPixel(int x, int y, Image img)
			{
				int r = 0;

				if (HasPixelAt(x, y, img))
				{
					while (HasPixelAt(x, y + r - 1, img) && (r > -7))
					{
						r--;
					}
				}
				else
				{
					r++;
					while (!HasPixelAt(x, y + r, img) && (r < 4))
					{
						r++;
					}
				}
				return r;

			}

			public static bool HasPixelAt(int x, int y, Image img)
			{
				return GetImageColor(img, x, y).a == 255;
			}

			//public int framecounter = 0;
			internal bool LemRemoved;
			internal int lemDXOld;
			internal int LemFrame;
			internal int LemPhysicsFrame;
			internal bool LemIsClimber;
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

				Color ToDraw = Color.WHITE;
				if (UnderMouse) ToDraw = Color.RED;

				//	DrawTextureRec(spriteTex, curFrame, new Vector2(LemX - 8, LemY - 10), ToDraw);
				DrawTextureRec(spriteTex, curFrame, new Vector2(LemX - spriteDef.WidthFromCenter  , LemY - spriteDef.CellH), ToDraw);
				//DrawRectangleLinesEx(new Rectangle(LemX  - spriteDef.WidthFromCenter/2, LemY- spriteDef.CellH, spriteDef.WidthFromCenter, spriteDef.CellH), 1f, ToDraw);
			}


		}

		public class SpriteDefinition
		{
			public string Name { get; set; }
			public string Path { get; set; }
			public Texture2D Texture { get; set; }
			public bool CanBeDisposed = false;
			public bool TextureSetup = false;
			public int Rows { get; set; }
			public int Cols { get; set; }
			public int CellW { get; set; }
			public int CellH { get; set; }

			public int WidthFromCenter { get; set; }

		}
	}
}
