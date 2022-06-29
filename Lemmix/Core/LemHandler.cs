//using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
/*using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using static Raylib_cs.Rlgl;*/
using static Raylib_CsLo.Raylib;
using Raylib_CsLo;
using static CLemmix4.RaylibMethods;
using CLemmix4.Lemmix.Utils;
using static CLemmix4.Lemmix.Utils.Common;
using CLemmix4.Lemmix.Gadget;
using System.Numerics;

namespace CLemmix4.Lemmix.Core
{
	public class LemHandler
	{

		public const int LEMMING_MAX_Y = 9;
		public const int MAX_FALLDISTANCE = 62;
		public event EventHandler<int> onTimeUpdate;

		public bool flagGameFinished = false;
		public int fCurrentIteration = 0;
		public int fClockFrame = 0;
		public int DelayEndFrames = 0;
		/*public Image imgLevel;
		public Image imgPhysics;*/
		public System.Collections.Concurrent.ConcurrentBag<Lemming> lems { get; set; } = new System.Collections.Concurrent.ConcurrentBag<Lemming>();
		public bool pause = false;
		public LevelPack.LevelData lvl { get; }
		public int TimePlay { get; private set; }
		/*	public Texture texLevel { get; internal set; }
			public Texture texPhysics { get; internal set; }*/

		public LevelPlayManager pm { get; set; }

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

		public static Dictionary<Lemming.enmLemmingState, SpriteDefinition> dictLemmingSpriteDefs = new Dictionary<Lemming.enmLemmingState, SpriteDefinition>()
			{
				{ Lemming.enmLemmingState.WALKING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 8, Name="WALKING", Path="styles/default/lemmings/walker.png" , WidthFromCenter = 8  } },
				{ Lemming.enmLemmingState.FALLING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 4, Name="FALLING", Path="styles/default/lemmings/faller.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.ASCENDING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 1, Name="ASCENDING", Path="styles/default/lemmings/ascender.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.FLOATING,new SpriteDefinition(){ CellH = 16, CellW = 16, Cols =2, Rows = 17, Name="FLOATING", Path="styles/default/lemmings/floater.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.CLIMBING,new SpriteDefinition(){ CellH = 12, CellW = 16, Cols =2, Rows = 8, Name="CLIMBING", Path="styles/default/lemmings/climber.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.HOISTING,new SpriteDefinition(){ CellH = 12, CellW = 16, Cols =2, Rows = 8, Name="HOISTING", Path="styles/default/lemmings/hoister.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.BASHING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 32, Name="BASHING", Path="styles/default/lemmings/basher.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.RELEASESLOWER,new SpriteDefinition(){ CellH = 48, CellW = 32, Cols =1, Rows = 1, Name="RELEASESLOWER", Path="gfx/btnMinus.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.RELEASEFASTER,new SpriteDefinition(){ CellH = 48, CellW = 32, Cols =1, Rows = 1, Name="RELEASEFASTER", Path="gfx/btnPlus.png",  WidthFromCenter = 8 } },
			};

		public Dictionary<string, SpriteDefinition> maskSpriteDefs = new Dictionary<string, SpriteDefinition>()
			{
				{"BASHER",new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 4, Name="BASHER", Path="styles/default/mask/basher.png", WidthFromCenter = 8 } }
			};

		public enum enmRemovalMode
		{
			RM_NEUTRAL = 0, RM_SAVE = 1 << 0, RM_KILL = 1 << 1, RM_ZOMBIE = 1 << 2
		}

		public static Color col_phy_terr = new Color(0, 10, 0, 255);
		public static Color col_phy_nocut = new Color(10, 0, 0, 255);
		public LemHandler(LevelPack.LevelData levelData, LevelPlayManager _lpm)
		{
			lvl = levelData;
			pm = _lpm;
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

			//lock (lemlock)
			//{
			if (lems == null) lems = new System.Collections.Concurrent.ConcurrentBag<Lemming>();
			lems.Add(l);
		//	pm.LemmingsOut++;
			//}

			//long memaf = GC.GetTotalMemory(true);




		}

		private void RemoveLemming(Lemming l, enmRemovalMode RemovalMode, bool Silent = false)
		{
			if (IsSimulating) return;

			LemmignsRemoved += 1;
			pm.LemmingsOut -= 1;
			l.LemRemoved = true;

			switch (RemovalMode)
			{
				case enmRemovalMode.RM_NEUTRAL:
				default:
					if (!Silent)
					{
						//ToDo Soundeffect
					}
					break;
			}
		}

		public void HandleDraw()
		{
			lock (lemlock)
			{
				foreach (var i in lems)
				{
					if (i.LemRemoved) continue;
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

			/*	for (i = 0; i < lems.Count; i++)
				{
					var L = lems[i];
					L.UnderMouse = false;
					L.PositionalRectangle = new Rectangle(L.LemX - L.spriteDef.WidthFromCenter / 2, L.LemY - L.spriteDef.CellH, L.spriteDef.WidthFromCenter, L.spriteDef.CellH);
					if (CheckCollisionPointRec(a, L.PositionalRectangle))
					{
						L.UnderMouse = true;
					}
				}*/
			foreach (var L in lems)
			{
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
			//Lemming currentLemming = null;
			bool continueWithLem = false;

			//handle zombie stuff, (not yet!) TODO

			//		for (i = 0; i < lems.Count; i++)
			foreach (var currentLemming in lems)
			{
				//currentLemming = lems[i];
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
					continueWithLem = CheckLevelBoundies(currentLemming);

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
				onTimeUpdate?.Invoke(this, TimePlay);
			}

			switch (fCurrentIteration)
			{
				case 35:
					{
						pm.EnableSpawneres();
						break;
					}
			}

		}


		int c1 = 0;
		/// <summary>
		/// Lemgame : 5628
		/// </summary>
		public void CheckReleaseLemming()
		{

			
			 
			if (pm.spawners == null) return;
			
			if (pm.NextLemmingCoundown > 0)
			{
				pm.NextLemmingCoundown--;
			}

			if (pm.NextLemmingCoundown == 0)
			{
				pm.NextLemmingCoundown = pm.SpawnInterval;
				if (pm.LemmingsToRelease > 0)
				{
					foreach (var i in pm.spawners)//5644
					{
						if (!i.Open) continue;
						var nl = new Lemming(this.pm) { LemX = (int)i.Pos.X, LemY = (int)i.Pos.Y, LemAction = Lemming.enmLemmingState.NONE };
						pm.lemHandler.Transition(nl, Lemming.enmLemmingState.FALLING);
						pm.lemHandler.AddLemming(nl);
						pm.LemmingsToRelease--;
						pm.LemmingsOut++;

					}

				}

			}


		}
		public void CheckUpdateNuking() { }
		public void UpdateGadgets() { }

		private void CheckTiggerArea(Lemming currentLemming, bool IsPostTeleportCheck = false)
		{


			if (currentLemming.LemActionNext != Lemming.enmLemmingState.NONE)
			{
				Transition(currentLemming, currentLemming.LemActionNext);
			}
		}


		public bool CheckLevelBoundies(Lemming L)
		{
			bool Result = true;
			if ((L.LemY <= 0) | (L.LemY > LEMMING_MAX_Y + pm.imgPhysics.height))
			{
				RemoveLemming(L, enmRemovalMode.RM_NEUTRAL);
				Result = false;
			}
			if ((L.LemX < 0) | (L.LemX >= pm.imgPhysics.width))
			{
				RemoveLemming(L, enmRemovalMode.RM_NEUTRAL);

				Result = false;
			}

			return Result;
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

			ImageDrawCS_ApplyAlphaMask(ref pm.imgLevel, msd.imgSprite, S, D, BLANK);
			ImageDrawCS_ApplyAlphaMask(ref pm.imgPhysics, msd.imgSprite, S, D, BLANK);

			var rem = new Rectangle();
			if (L.LemDX == 1)
			{
				rem = new Rectangle(L.LemX - 4, L.LemY - 9, 8, 10);
				RemoveTerrain(rem);
				/*	fixed (Image* ptr = &lpm.imgLevel)
					{
						ImageDrawRectangle(ptr, (int)rem.x, (int)rem.y, (int)rem.width, (int)rem.height, GREEN);
					}*/

			}


			pm.imgInvalid = true;
		}


		public enum enmMaskDir { NONE, LEFT, RIGHT };//3006
		List<int> t1 = new List<int>();
		private bool IsSimulating;
		public int LemmignsRemoved;


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
				int iW = pm.imgPhysics.width;
				int iH = pm.imgPhysics.height;

				int ry = 0;
				int rx = 0;

				//ImageDraw(ref lpm.imgLevel, msd.imgSprite, new Rectangle(smx,smy,emx,emy), new Rectangle(PosX,PosY,mw,mh), Color.WHITE);


				int _y = 0; int _x = 0;
				for (int y = smy; y < emy; y++)
				{
					for (int x = smx; x < emx; x++)
					{
						Color clr = GetImageColor(msd.imgSprite, x, y);

						ImageDrawPixel(ref pm.imgLevel, _x, _y, clr);

						_x++;
					}
					_y++;
				}

				pm.imgInvalid = true;
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
						ImageDrawPixel(ref pm.imgLevel, mapX, mapY, clrMask);
						ImageDrawPixel(ref pm.imgPhysics, mapX, mapY, clrMask);
						rx++;
					}
					ry++;
				}






				if (invalidate)
					pm.imgInvalid = true;

			}

		}

		public unsafe void RemoveTerrain(Rectangle r) => RemoveTerrain(r.x, r.y, r.width, r.height);
		public unsafe void RemoveTerrain(float x, float y, float w, float h) => RemoveTerrain((int)x, (int)y, (int)w, (int)h);
		//750 rendering
		public unsafe void RemoveTerrain(int x, int y, int w, int h)
		{
			//	Debug.WriteLine($"{x},{y},{w},{h}");
			Color* terrPtr;
			Color* physPtr;
			int mapW = pm.imgPhysics.width;
			int mapH = pm.imgPhysics.height;
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

					ImageDrawPixel(ref pm.imgLevel, cx, cy, BLANK);
					ImageDrawPixel(ref pm.imgPhysics, cx, cy, BLANK);

				}
			}

			//UpdateTexture(lpm.texLevel, terrPtr);
			//UpdateTexture(lpm.texPhysics, physPtr);
			//UnloadImageColors(terrPtr);
			//UnloadImageColors(physPtr);
			pm.imgInvalid = true;
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
			return GetImageColor(pm.imgPhysics, x, y).a == 255;
		}


	}



}
