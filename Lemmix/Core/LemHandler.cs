using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using CLemmix4.Lemmix.Gadget;
using CLemmix4.Lemmix.Utils;
using static Raylib_CsLo.Raylib;
using static CLemmix4.RaylibMethods;
using static CLemmix4.Lemmix.Utils.Common;

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
		public static Dictionary<Lemming.enmLemmingState, int> LemmingMethodAnimFrames = 
			
			
			new Dictionary<Lemming.enmLemmingState, int>()
			{
			{Lemming.enmLemmingState.None,0},{Lemming.enmLemmingState.WALKING,4},{Lemming.enmLemmingState.ASCENDING,1},
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
				{ Lemming.enmLemmingState.DIGGING,new SpriteDefinition(){ CellH = 14, CellW = 16, Cols =2, Rows = 16, Name="DIGGING", Path="styles/default/lemmings/digger.png",  WidthFromCenter = 8, PosYOffset = 3 } },
				{ Lemming.enmLemmingState.BUILDING,new SpriteDefinition(){ CellH = 13, CellW = 16, Cols =2, Rows = 16, Name="BUILDING", Path="styles/default/lemmings/builder.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.SHRUGGING,new SpriteDefinition(){ CellH = 10, CellW = 16, Cols =2, Rows = 8, Name="SHRUGGING", Path="styles/default/lemmings/shrugger.png",  WidthFromCenter = 8 } },
				{ Lemming.enmLemmingState.BLOCKING,new SpriteDefinition(){ CellH = 13, CellW = 16, Cols =2, Rows = 16, Name="BLOCKING", Path="styles/default/lemmings/blocker.png",  WidthFromCenter = 8 } },
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
			LemmingMethods.Add(Lemming.enmLemmingState.DIGGING, HandleDigging);
			LemmingMethods.Add(Lemming.enmLemmingState.BUILDING, HandleBuilding);
			LemmingMethods.Add(Lemming.enmLemmingState.SHRUGGING, HandleShrugging);
			LemmingMethods.Add(Lemming.enmLemmingState.BLOCKING, HandleBlocking);

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
				CheckAdjustSpawnInterval();
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
		public void CheckAdjustSpawnInterval()
		{

			if (pm.SpawnIntervalModifier == 0) return;

			var newSi = pm.SpawnInterval + pm.SpawnIntervalModifier;
			if (CheckIfLegalSI(newSi))
				pm.SpawnInterval = newSi;

		}

		private bool CheckIfLegalSI(int aSi)
		{
			if (aSi < LevelPlayManager.MINIMUM_SI || aSi > lvl.Max_Spawn_interval)
				return false;


			return true;
		}

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
						var nl = new Lemming(this.pm) { LemX = (int)i.Pos.X, LemY = (int)i.Pos.Y, LemAction = Lemming.enmLemmingState.None };
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

		private void CheckTiggerArea(Lemming L, bool IsPostTeleportCheck = false)
		{


			if (L.LemActionNext != Lemming.enmLemmingState.None)
			{
				Transition(L, L.LemActionNext);
			}

			//blocker fields

			if (HasTriggerAt(L.LemX, L.LemY, enmTriggers.FORCELEFT, L))
			{
				HandleForceField(L, -1);
			}
			else if (HasTriggerAt(L.LemX, L.LemY, enmTriggers.FORCERIGHT, L))
			{
				HandleForceField(L, 1);

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
			Lemming.enmLemmingState[] oneTimers = new Lemming.enmLemmingState[] { Lemming.enmLemmingState.HOISTING, Lemming.enmLemmingState.SHRUGGING };

			bool r = false;
			l.LemXOld = l.LemX;
			l.LemYOld = l.LemY;
			l.lemDXOld = l.LemDx;
			l.LemActionOld = l.LemAction;
			l.LemActionNext = Lemming.enmLemmingState.None;
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
			if (L.LemHasBlockerField && !NewAction.In(Lemming.enmLemmingState.OHNOING, Lemming.enmLemmingState.STONING))
			{
				L.LemHasBlockerField = false;
				SetBlockerMap();
			}


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

			if (L.LemHasBlockerField && !NewAction.In(Lemming.enmLemmingState.OHNOING, Lemming.enmLemmingState.STONING))
			{
				L.LemHasBlockerField = false;
				SetBlockerMap();
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

				case Lemming.enmLemmingState.BUILDING:
					{
						L.LemNumberOfBricksLeft = 12;
						L.LemConstructivePositionFreeze = false;
						break;
					}
				case Lemming.enmLemmingState.BLOCKING:
					{
						L.LemHasBlockerField = true;
						SetBlockerMap();
						break;
					}
			}

		}

	

		private void SetBlockerField(Lemming L)
		{
			int x, y, step;

			x = L.LemX - 6;
			if (L.LemDx == 1 )
			{
				x++;
			}

			for (step = 0; step <= 11; step++)
			{
				for (y = L.LemY - 6; y <= L.LemY + 4; y++)
				{
					switch (step)
					{
						case >= 0 and <= 3:
							{
								WriteBlockerMap(x + step, y, L, enmDomStates.FORCELEFT);
								break;
							}
						case >= 4 and <= 7:
							{
								WriteBlockerMap(x + step, y, L, enmDomStates.BLOCKER);

								break;
							}
						case >= 8 and <= 11:
							{
								WriteBlockerMap(x + step, y, L, enmDomStates.FORCERIGHT);

								break;
							}
					}
				}
			}
		}

		BackingMap<int> tmp1;
		private void SetBlockerMap()
		{
			pm.BlockerMap.Clear();
			tmp1 = new BackingMap<int>(lvl.Width, lvl.Height);
			foreach (var i in pm.lemHandler.lems)
			{
				if (i.LemHasBlockerField && !i.LemRemoved)
				{
					SetBlockerField(i);
				}
			}
		}

		private void WriteBlockerMap(int x, int y, Lemming L, enmDomStates state)
		{
			if (tmp1 == null) tmp1 = new BackingMap<int>(lvl.Width, lvl.Height);
			tmp1[x, y] = (int)state;
			pm.BlockerMap[x, y] = new LevelPlayManager.BackingMapData() { ObjectId = L.ID, State = state };

			var imgPixel = ColorAlphaBlend(YELLOW, GetImageColor(pm.imgLevel, x, y), WHITE);

			ImageDrawPixel(ref pm.imgLevel, x, y, imgPixel);
			pm.imgInvalid = true;
		//	pm.BlockerMap.UpdateRenderTexture(this.pm.mainCam);
		}

		public bool CheckForOverlappingBlockerField(Lemming L)
		{
			int X;
			bool Result = false;
		X = L.LemX - 6;
			if (L.LemDx == 1) X++;

			Result = HasTriggerAt(X, L.LemY - 6,  enmTriggers.BLOCKER)
				|| HasTriggerAt(X + 11, L.LemY - 6, enmTriggers.BLOCKER)
				|| HasTriggerAt(X, L.LemY + 4, enmTriggers.BLOCKER) 
				|| HasTriggerAt(X + 11, L.LemY + 4, enmTriggers.BLOCKER);

			return Result;
		}


		public enmDomStates ReadBlockerMap(int X, int Y, Lemming L = null)
		{
			int CheckPosX;
			enmDomStates Result = enmDomStates.NONE;
			if ((X >= 0) && (X < pm.LevelData.Width) && (Y >= 0) && (Y < pm.LevelData.Height))
			{
				var resLup = pm.BlockerMap[X, Y];
				Result = resLup.State;

				if (Result != enmDomStates.NONE)
				{
					this.fLastBlockerCheckLem = lems.FirstOrDefault(o => o.ID == resLup.ObjectId);
				}
				else
					this.fLastBlockerCheckLem = null;

				if (fLastBlockerCheckLem != null)
				{


					if (Result != enmDomStates.NONE && L != null && L.LemAction == Lemming.enmLemmingState.BUILDING)
					{
						if (fLastBlockerCheckLem.LemDx == L.LemDx)
							CheckPosX = L.LemX + 2 * L.LemDx;
						else
							CheckPosX = L.LemX + 3 * L.LemDx;

						if ((L.LemY >= fLastBlockerCheckLem.LemY - 1) && (L.LemY <= fLastBlockerCheckLem.LemY + 3) && (fLastBlockerCheckLem.LemX == CheckPosX))
						{
							Result = enmDomStates.NONE;
							return Result;
						}

					}


					if (IsSimulating && Result.In(enmDomStates.FORCELEFT, enmDomStates.FORCERIGHT))
					{
						if (!HasPixelAt(fLastBlockerCheckLem.LemX, fLastBlockerCheckLem.LemY))
						{

							Result = enmDomStates.NONE;
							return Result;
						}
					}

				}
			}
			else Result = enmDomStates.NONE;

			return Result;
		}
		private bool HandleForceField(Lemming L, int Direction)
		{
			bool Result = false;
			//ToDo This wont work with the enums... will probablywork when moved to decent classes.
			if (L.LemDx == -Direction)// && !L.LemAction.In(Lemming.enmLemmingState.DEHOISTING, Lemming.enmLemmingState.HOISTING))
			{
				Result = true;
				TurnAround(L);

				//miners todo	
				if (L.LemAction == Lemming.enmLemmingState.MINING)
				{

				}
				else if (L.LemAction.In(Lemming.enmLemmingState.BUILDING, Lemming.enmLemmingState.PLATFORMING) /*&& L.LemPhysicsFrame >= 9*/)
				{
					LayBrick(L);
				}
				else if (L.LemAction.In(Lemming.enmLemmingState.CLIMBING, Lemming.enmLemmingState.SLIDING, Lemming.enmLemmingState.DEHOISTING))
				{
					L.LemX += L.LemDx;
					if (!L.LemIsStartingAction) L.LemY++;
					Transition(L, Lemming.enmLemmingState.WALKING);
				}


				//builders


			}
			return Result;
		}

		public void TurnAround(Lemming L) => L.LemDx = -L.LemDx;


		public bool MayAssignBlocker(Lemming L)
		{
			bool Result = false;
			Result = L.LemAction.In(Lemming.enmLemmingState.WALKING, Lemming.enmLemmingState.SHRUGGING,
				Lemming.enmLemmingState.BUILDING, Lemming.enmLemmingState.BASHING, Lemming.enmLemmingState.DIGGING, Lemming.enmLemmingState.MINING) && (!CheckForOverlappingBlockerField(L));



			return Result;
		}
		public bool HandleBlocking(Lemming L)
		{
			if (!HasPixelAt(L.LemX, L.LemY)) Transition(L, Lemming.enmLemmingState.FALLING);
			return true;
		}

		public bool HandleShrugging(Lemming L)
		{
			if (L.LemEndOfAnimation) Transition(L, Lemming.enmLemmingState.WALKING);
			return true;
		}
		public bool HandleBuilding(Lemming L)
		{
			bool r = true;

			if (L.LemPhysicsFrame == 9)
				LayBrick(L);

			else if (L.LemPhysicsFrame == 10 && L.LemNumberOfBricksLeft <= 3)
			{
				//sound effec tbuilder warning
			}
			else if (L.LemPhysicsFrame == 0)
			{
				L.LemNumberOfBricksLeft--;

				if (HasPixelAt(L.LemX + L.LemDx, L.LemY - 2))
				{
					Transition(L, Lemming.enmLemmingState.WALKING, true);
				}
				else if (
					 HasPixelAt(L.LemX + L.LemDx, L.LemY - 3) ||
					 HasPixelAt(L.LemX + 2 * L.LemDx, L.LemY - 2) ||
					 (HasPixelAt(L.LemX + 2 * L.LemDx, L.LemY - 10) && (L.LemNumberOfBricksLeft > 0)))
				{
					L.LemY--;
					L.LemX += L.LemDx;
					Transition(L, Lemming.enmLemmingState.WALKING, true);
				}
				else
				{
					L.LemY--;
					L.LemX += (2 * L.LemDx);

					if (HasPixelAt(L.LemX, L.LemY - 2) || HasPixelAt(L.LemX, L.LemY - 3) || HasPixelAt(L.LemX + L.LemDx, L.LemY - 3)
						|| (HasPixelAt(L.LemX + L.LemDx, L.LemY - 9) && (L.LemNumberOfBricksLeft > 0)))
					{
						Transition(L, Lemming.enmLemmingState.WALKING, true);
					}
					else if (L.LemNumberOfBricksLeft == 0)
					{
						Transition(L, Lemming.enmLemmingState.SHRUGGING);
					}
				}


			}
			if (L.LemPhysicsFrame == 0) L.LemConstructivePositionFreeze = false;

			return r;
		}

		public void LayBrick(Lemming L)
		{
			int BrickPosY, n;

			if (L.LemAction == Lemming.enmLemmingState.BUILDING)
			{
				BrickPosY = L.LemY - 1;
			}
			else
			{
				BrickPosY = L.LemY;
			}
			/*
			 *   for n := 0 to 5 do
    AddConstructivePixel(L.LemX + n*L.LemDx, BrickPosY, BrickPixelColors[12 - L.LemNumberOfBricksLeft]);*/

			for (n = 0; n < 5; n++)
				AddConstructivePixel(L.LemX + n * L.LemDx, BrickPosY, RED);
		}

		public void AddConstructivePixel(int x, int y, Color c)
		{
			ImageDrawPixel(ref pm.imgLevel, x, y, c);
			ImageDrawPixel(ref pm.imgPhysics, x, y, col_phy_terr);
			pm.imgInvalid = true;

		}
		public bool HandleDigging(Lemming L)
		{
			bool continueWork = false;
			bool r = true;
			if (L.LemIsStartingAction)
			{
				L.LemIsStartingAction = false;
				DigOneRow(L.LemX, L.LemY - 1);
				L.LemPhysicsFrame--;
			}

			if (L.LemPhysicsFrame.In(0, 8))
			{
				L.LemY++;
				continueWork = DigOneRow(L.LemX, L.LemY - 1);
				//indestructable check

				if (!continueWork)
					Transition(L, Lemming.enmLemmingState.FALLING);
			}
			return r;
		}

		private bool DigOneRow(int PosX, int PosY)
		{

			int n;
			bool Result = false;
			for (n = -4; n < 4; n++)
			{
				if (HasPixelAt(PosX + n, PosY)) //& !HasIndestructibleAt(PosX + n, PosY, 0, baDigging))
				{
					RemovePixelAt(PosX + n, PosY);

					if ((n > -4) & (n < 4)) Result = true;
				}

				if (!IsSimulating)
				{
					RemoveTerrain(new Rectangle(PosX - 4, PosY, 9, 1));
				}
			}


			return Result;



		}

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
				L.LemX -= L.LemDx;
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
					if (HasPixelAt(L.LemX + n * L.LemDx, L.LemY - 6))
					{
						continueWork = true;
					}
					if (HasPixelAt(L.LemX + n * L.LemDx, L.LemY - 5))
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
				L.LemX += L.LemDx;
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

			L.LemX += L.LemDx;
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
					L.LemX += L.LemDx;
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
				L.LemX -= L.LemDx;
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
				FoundClip = HasPixelAt(L.LemX - L.LemDx, L.LemY - 6 - L.LemPhysicsFrame)
		|| (HasPixelAt(L.LemX - L.LemDx, L.LemY - 5 - L.LemPhysicsFrame) && !L.LemIsStartingAction);

				if (L.LemPhysicsFrame == 0)
					FoundClip = FoundClip && HasPixelAt(L.LemX - L.LemDx, L.LemY - 7);

				if (FoundClip)
				{
					if (!L.LemIsStartingAction) L.LemY = L.LemY - L.LemPhysicsFrame + 3;

					//ToDo handle slider
					L.LemX -= L.LemDx;
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
				FoundClip = HasPixelAt(L.LemX - L.LemDx, L.LemY - 7);

				if (L.LemPhysicsFrame == 7)
					FoundClip = FoundClip && HasPixelAt(L.LemX - L.LemDx, L.LemY - 7);

				if (FoundClip)
				{
					L.LemY--;
					//ToDo slider

					L.LemX -= L.LemDx;
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
			S.x = L.LemDx == 1 ? 16 : 0;
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
			if (L.LemDx == 1)
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
		private Lemming fLastBlockerCheckLem = null;

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

		public unsafe void RemovePixelAt(int cx, int cy)
		{
			Color* terrPtr;
			Color* physPtr;
			int mapW = pm.imgPhysics.width;
			int mapH = pm.imgPhysics.height;

			ImageDrawPixel(ref pm.imgLevel, cx, cy, BLANK);
			ImageDrawPixel(ref pm.imgPhysics, cx, cy, BLANK);

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

		public bool HasTriggerAt(int X, int Y, enmTriggers triggerType, Lemming L = null)
		{
			bool Result = false;
			fLastBlockerCheckLem = null;

			switch (triggerType)
			{
				case enmTriggers.FORCELEFT:
					{
						Result = ReadBlockerMap(X, Y, L) == enmDomStates.FORCELEFT;
						break;
					}

				case enmTriggers.FORCERIGHT:
					{
						Result = ReadBlockerMap(X, Y, L) == enmDomStates.FORCERIGHT;
						break;
					}

				case enmTriggers.BLOCKER:
					{

						Result =  ReadBlockerMap(X, Y, L) == enmDomStates.BLOCKER || ReadBlockerMap(X, Y, L) == enmDomStates.FORCELEFT || ReadBlockerMap(X, Y, L) == enmDomStates.FORCERIGHT;
					
						break;
					}
			}
			return Result;
		}

	}



}
