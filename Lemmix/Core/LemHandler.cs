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
using static CLemmix4.Lemmix.Skills.skillNameHolders;
using CLemmix4.Lemmix.Skills;
using System.CodeDom;

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

		public  void RemoveLemming(Lemming l, enmRemovalMode RemovalMode, bool Silent = false)
		{
			if (IsSimulating) return;

			LemmignsRemoved += 1;
			pm.LemmingsOut -= 1;
			l.LemRemoved = true;

			switch (RemovalMode)
			{
				case enmRemovalMode.RM_SAVE:
					{
						pm.LemmingsIn++;
				 

						break;
					}

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
				/*if (currentLemming.LemParticleTimer >= 0)
				{
					currentLemming.LemParticleTimer--;
				}*/

				if (currentLemming.LemRemoved) continue;

				//handle teleporting

				

				//handle explosion coutndown
			 if (fCurrentIteration % 17 == 0)
				ProcessLemQueue(currentLemming);

 




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


			if (this.Nuking) return;
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
						var nl = new Lemming(this.pm) { LemX = (int)i.Pos.X, LemY = (int)i.Pos.Y, LemAction = NONE };

						//	pm.lemHandler.Transition(nl, Lemming.enmLemmingState.FALLING);
						nl.skillHandler.Transition(FALLING);
						pm.lemHandler.AddLemming(nl);
						pm.LemmingsToRelease--;
						pm.LemmingsOut++;

					}

				}

			}


		}

		public bool ExploderAssignInProgress = false;
		public void CheckUpdateNuking() {
			 
			if (Nuking)
			{
				var x = lems.Where(o => !o.IsNuking).FirstOrDefault();
				if (x != null)
				{
					((Skills.absSkill)STARTBOMBING).TryAssign(x);
					x.IsNuking = true;
				}
				


			}
		}
		public void UpdateGadgets() { }

		private void CheckTiggerArea(Lemming L, bool IsPostTeleportCheck = false)
		{


			//if (L.LemActionNext != NONE)
			if (L.skillHandler.ActionNext != NONE)
			{
				L.skillHandler.Transition();
				//L.skillHandler.Transition(L.skillHandler.ActionNext);
				//Transition(L, L.LemActionNext);
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

			if (HasTriggerAt(L.LemX, L.LemY, enmTriggers.EXIT, L))
			{
				HandleExiting(L);
			}


    }

    private void HandleExiting(Lemming l)
    {
			RemoveLemming(l, enmRemovalMode.RM_SAVE);
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



		public bool HandleLemming(Lemming L)
		{
			//Lemming.enmLemmingState[] oneTimers = new Lemming.enmLemmingState[] { Lemming.enmLemmingState.HOISTING, Lemming.enmLemmingState.SHRUGGING };

			string[] oneTimers = new string[] { HOISTING, SHRUGGING, SPLATTING, EXPLODING,OHNOING };
	 
			bool r = false;
			L.LemXOld = L.LemX;
			L.LemYOld = L.LemY;
			L.lemDXOld = L.LemDx;
			//	l.LemActionOld = l.LemAction;
			L.skillHandler.ActionOld = L.skillHandler.ActionCurrent;
			//l.LemActionNext = NONE;
			L.skillHandler.ActionNext = NONE;
			L.fLemJupToHoistAdvance = false;
			L.LemFrame++;
			L.LemPhysicsFrame++;
			//3420
			if (L.LemPhysicsFrame > L.LemMaxPhysicsFrame)
			{
				L.LemPhysicsFrame = 0;
				if (L.LemAction == FLOATING) L.LemPhysicsFrame = 9;

			 
				if (L.LemAction.In(oneTimers)) L.LemEndOfAnimation = true;
			}
	 
			r = L.skillHandler.Handle();
			return r;
		}

		public void ProcessLemQueue(Lemming L)
		{
			if (L.LemQueue != null)
			{
				if (L.LemQueueSP >= L.LemQueue.Count)
				{
					L.LemQueue = null;
					L.LemQueueSP = 0;
				}
				else {
					var cur = L.LemQueue[L.LemQueueSP];
					switch (cur.type)
					{
				 
						case Lemming.LQueueItem.QueueType.TRANSITION:
							{

								cur.countdown--;

								L.OverHeadText = $"{cur.countdown}";
								if (cur.countdown <= 0)
								{
									L.LemQueueSP++;
									Transition(L, cur.skill);
									L.OverHeadText = null;

								}




								break;
							}
					}
				}
			}
		}

		public void Transition(Lemming L, absSkill NewAction, bool DoTurn = false)
		{
			int i = 0;
			bool OldIsStartingAction = false;

			if (DoTurn) TurnAround(L);

			if (NewAction == TOWALKING) NewAction = WALKING;

			//ToDo handle blockers
			if (L.LemHasBlockerField && !NewAction.In(OHNOING, STONING))
			{
				L.LemHasBlockerField = false;
				SetBlockerMap();
			}


			if (!HasPixelAt(L.LemX, L.LemY) && NewAction == WALKING)
				NewAction = FALLING;


			if (L.LemAction == NewAction) return;

			//set initial fall according to previous skill 

			if (NewAction == FALLING)
			{
				//ignore swimming TODO

				L.LemFallen = 1;
				//	if ((new Lemming.enmLemmingState[] { Lemming.enmLemmingState.WALKING, Lemming.enmLemmingState.BASHING }).Contains(L.LemAction))
				if (L.LemAction.In(WALKING, BASHING))
				{
					L.LemFallen = 3;
				}

				//ToDo, handle mining, digging, blocking, jumping, and lazering

				L.LemTrueFallen = L.LemFallen;
			}

			//if (L.LemHasBlockerField && !NewAction.In(Lemming.enmLemmingState.OHNOING, Lemming.enmLemmingState.STONING))
			if (L.LemHasBlockerField && !NewAction.In(OHNOING, STONING))
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
			//L.LemMaxFrame = LemmingMethodAnimFrames[L.LemAction];
			L.LemMaxFrame = L.LemAction.SpriteAnimFrames;
			//	L.LemPhysicsFrame = LemmingMethodAnimFrames[NewAction];
			//	L.LemMaxPhysicsFrame = LemmingMethodAnimFrames[NewAction] - 1;
			L.LemMaxPhysicsFrame = L.LemAction.SpriteAnimFrames - 1;
			switch (L.LemAction.Name)
			{
				case ASCENDING:
					{
						L.LemAscended = 0;
						break;
					}

				case BUILDING:
					{
						L.LemNumberOfBricksLeft = 12;
						L.LemConstructivePositionFreeze = false;
						break;
					}
				case BLOCKING:
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
			if (L.LemDx == 1)
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
		public void SetBlockerMap()
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

			/*var imgPixel = ColorAlphaBlend(YELLOW, GetImageColor(pm.imgLevel, x, y), WHITE);

			ImageDrawPixel(ref pm.imgLevel, x, y, imgPixel);
			pm.imgInvalid = true;*/
			//	pm.BlockerMap.UpdateRenderTexture(this.pm.mainCam);
		}

		public bool CheckForOverlappingBlockerField(Lemming L)
		{
			int X;
			bool Result = false;
			X = L.LemX - 6;
			if (L.LemDx == 1) X++;

			Result = HasTriggerAt(X, L.LemY - 6, enmTriggers.BLOCKER)
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


					if (Result != enmDomStates.NONE && L != null && L.LemAction == BUILDING)
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
				if (L.LemAction == MINING)
				{

				}
				else if (L.LemAction.In(BUILDING, PLATFORMING) /*&& L.LemPhysicsFrame >= 9*/)
				{
					 SklBuilding.LayBrick(L);
				}
				else if (L.LemAction.In(CLIMBING, SLIDING, DEHOISTING))
				{
					L.LemX += L.LemDx;
					if (!L.LemIsStartingAction) L.LemY++;
					Transition(L, WALKING);
				}


				//builders


			}
			return Result;
		}

		public void TurnAround(Lemming L) => L.LemDx = -L.LemDx;


		public bool MayAssignBlocker(Lemming L)
		{
			bool Result = false;
			/*Result = L.LemAction.In(Lemming.enmLemmingState.WALKING, Lemming.enmLemmingState.SHRUGGING,
				Lemming.enmLemmingState.BUILDING, Lemming.enmLemmingState.BASHING, Lemming.enmLemmingState.DIGGING, Lemming.enmLemmingState.MINING) && (!CheckForOverlappingBlockerField(L));*/



			return Result;
		}
 
 

		public void LayBrick(Lemming L)
		{
			int BrickPosY, n;

			if (L.LemAction == BUILDING)
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
			pm.mask[y * pm.Width + x] |= LevelPlayManager.MaskData.TERRAIN | LevelPlayManager.MaskData.NO_OVERWRITE; ;
			pm.imgInvalid = true;

		}
 


		public enum enmMaskDir { NONE, LEFT, RIGHT };//3006
		List<int> t1 = new List<int>();
		public bool IsSimulating;
		public int LemmignsRemoved;
		private Lemming fLastBlockerCheckLem = null;
		internal bool Nuking;

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

						Result = ReadBlockerMap(X, Y, L) == enmDomStates.BLOCKER || ReadBlockerMap(X, Y, L) == enmDomStates.FORCELEFT || ReadBlockerMap(X, Y, L) == enmDomStates.FORCERIGHT;

						break;
					}

				case enmTriggers.EXIT:
				{

				//ToDo we could really optimize this? calculate the exits at the beginning and store them in an array/list
						var j = pm.gadgHandler.gadgets.FirstOrDefault(o => o.GadgetName == "EXIT");
						if (j != null)
						{
							int xX = j.GadgetDef.X + j.GadgetDef.EffectData.Trigger_X;
							int xY = j.GadgetDef.Y + j.GadgetDef.EffectData.Trigger_Y;
							int xW = j.GadgetDef.EffectData.Trigger_Width;
							int xH = j.GadgetDef.EffectData.Trigger_Height;


							Rectangle rec = new Rectangle(xX, xY, xW, xH);
							Vector2 pt = new Vector2(X, Y);

							if (CheckCollisionPointRec(pt, rec))
							{
								return  true;
							}


			 
						}
				break;
				}


			}
			return Result;
		}

	}



}
